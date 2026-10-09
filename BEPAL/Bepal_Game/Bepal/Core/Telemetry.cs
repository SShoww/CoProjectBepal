using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Bepal;

/// <summary>
/// Seed for every gameplay-relevant <see cref="Random"/> (Wheel zone placement, pet wandering) so a run can be
/// replayed: <c>--seed &lt;n&gt;</c>. Without the flag a random seed is picked (and recorded by Telemetry).
/// </summary>
public static class RunSeed
{
    public static int Seed { get; private set; } = Environment.TickCount;
    public static void Set(int seed) => Seed = seed;
    public static Random Make(int salt) => new(unchecked(Seed * 31 + salt));
}

/// <summary>
/// Per-run telemetry (BEPAL/Docs/Balance/telemetry-spec.md). No-op unless <see cref="Configure"/> was called
/// (<c>--telemetry &lt;dir&gt;</c>). Writes one JSONL file per process: one line per event (runId, seq, t = simulated
/// seconds, tReal, type, day, scene + event fields) and a final <c>run_summary</c> row built from running counters.
/// </summary>
public static class Telemetry
{
    public static bool Enabled { get; private set; }
    public static bool Active => Enabled && _started && !_ended;
    /// <summary>Set by Game1 to report the top scene's type name.</summary>
    public static Func<string>? SceneProbe;

    static string _dir = "", _profile = "", _mode = "", _runId = "", _path = "", _build = "";
    static bool _refuse, _started, _ended, _dirty;
    static float _t, _t0;
    static int _seq;
    static readonly Stopwatch Sw = new();
    static readonly List<string> Lines = new();
    static readonly Dictionary<string, double> C = new();
    static readonly Dictionary<string, string> S = new();
    static GameState? _gs;
    static int _energyToday;
    static string _enemy = "";
    static readonly HashSet<int> DayRecorded = new();
    static readonly JsonSerializerOptions Json = new() { NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowNamedFloatingPointLiterals };

    public static void Configure(string dir, string profile, string mode, bool refuse)
    {
        Enabled = true;
        _dir = dir;
        _profile = profile;
        _mode = mode;
        _refuse = refuse;
        AppDomain.CurrentDomain.ProcessExit += (_, _) => { End("quit"); Flush(); };
    }

    /// <summary>Advance simulated time (call once per scene update with the same dt the scenes got).</summary>
    public static void Tick(float dt) => _t += dt;

    static void Add(string key, double n = 1) => C[key] = C.GetValueOrDefault(key) + n;

    // ---------------- core ----------------

    public static void RunStart(GameState gs, Species starter)
    {
        if (!Enabled || _started) return;
        _started = true;
        _gs = gs;
        _t0 = _t;
        Sw.Start();
        _build = GitHash();
        var now = DateTime.Now;
        _runId = $"run_{now:yyyyMMdd_HHmmss}_{_profile}{(_refuse ? "-refuse" : "")}_{RunSeed.Seed}";
        Directory.CreateDirectory(_dir);
        _path = Path.Combine(_dir, _runId + ".jsonl");
        Emit("run_start", "starter", starter.ToString(), "startCoin", gs.Coin, "startEnergy", gs.Energy,
            "profile", _profile, "refuse", _refuse, "seed", RunSeed.Seed, "build", _build, "mode", _mode);
    }

    public static void Emit(string type, params object?[] kv)
    {
        if (!Active) return;
        var d = new Dictionary<string, object?>();
        for (int i = 0; i + 1 < kv.Length; i += 2) d[(string)kv[i]!] = kv[i + 1] is float f ? Math.Round(f, 3) : kv[i + 1];
        Account(type, d);
        var row = new Dictionary<string, object?>
        {
            ["runId"] = _runId, ["seq"] = _seq++, ["t"] = Math.Round(_t - _t0, 3), ["tReal"] = Math.Round(Sw.Elapsed.TotalSeconds, 3),
            ["type"] = type, ["day"] = _gs?.Day ?? 0, ["scene"] = SceneProbe?.Invoke() ?? "",
        };
        foreach (var (k, v) in d) row[k] = v;
        Lines.Add(JsonSerializer.Serialize(row, Json));
        _dirty = true;
    }

    public static void Flush()
    {
        if (!_started || !_dirty) return;
        try { File.WriteAllLines(_path, Lines); _dirty = false; }
        catch (Exception e) { Console.Error.WriteLine("telemetry flush failed: " + e.Message); }
    }

    /// <summary>Ends the run (first call wins), writes run_end + run_summary and flushes the file.</summary>
    public static void End(string outcome, string deathScene = "")
    {
        if (!Active) return;
        foreach (var p in _gs!.Pets) Snapshot(p);
        if (_energyToday > 0 || !DayRecorded.Contains(_gs.Day)) RecordDayEnergy(_gs.Day, _gs.Energy);
        Emit("run_end", "outcome", outcome, "dayReached", _gs.Day, "deathScene", deathScene);
        Emit("run_summary", Summary(outcome, deathScene));
        _ended = true;
        _dirty = true;
        Flush();
    }

    static object?[] Summary(string outcome, string deathScene)
    {
        var gs = _gs!;
        var f = new List<object?>
        {
            "profile", _profile, "refuse", _refuse, "seed", RunSeed.Seed, "build", _build, "mode", _mode, "outcome", outcome,
            "dayReached", gs.Day, "deathScene", deathScene,
            "tSimSec", Math.Round(_t - _t0, 3), "tRealSec", Math.Round(Sw.Elapsed.TotalSeconds, 3),
            "finalCoin", gs.Coin,
            "upgQte", gs.QteUpgrade, "upgEnergy", gs.EnergyUpgrade, "upgProgress", gs.ProgressUpgrade,
            "pointsLeft", gs.Points, "playerLevel", gs.PlayerLevel, "playerExp", gs.PlayerExp,
            "petsFinal", gs.Pets.Count, "petsAliveFinal", gs.Alive.Count(),
            "petLvlMax", gs.Pets.Count == 0 ? 0 : gs.Pets.Max(p => p.Level),
            "finalHpSum", gs.Pets.Sum(p => p.HpShown),
            "finalStomachAvg", gs.Pets.Count == 0 ? 0 : Math.Round(gs.Pets.Average(p => p.Stomach), 1),
            "finalCleanAvg", gs.Pets.Count == 0 ? 0 : Math.Round(gs.Pets.Average(p => p.Clean), 1),
            "pointsSpent", (int)C.GetValueOrDefault("pointsSpent"),
            "merchantChoice", S.GetValueOrDefault("merchantChoice", "n/a"),
            "deathDay", (int)C.GetValueOrDefault("deathDay", -1),
        };
        foreach (var k in C.Keys.OrderBy(k => k, StringComparer.Ordinal))
        {
            if (k is "pointsSpent" or "deathDay") continue;
            f.Add(k); f.Add(C[k]);
        }
        return f.ToArray();
    }

    // ---------------- running counters (summary) ----------------

    static string Str(Dictionary<string, object?> d, string k) => d.GetValueOrDefault(k)?.ToString() ?? "";
    static double Num(Dictionary<string, object?> d, string k) => d.TryGetValue(k, out var v) && v != null ? Convert.ToDouble(v) : 0;

    static void Account(string type, Dictionary<string, object?> d)
    {
        switch (type)
        {
            case "run_start":
                C["startCoin"] = Num(d, "startCoin");
                break;
            case "care_select":
                Add("careSelectPresses");
                Add("careSelect_" + Str(d, "result"));
                break;
            case "qte_press":
                Add("qte" + Str(d, "hit")[0] + "_" + Str(d, "care"));   // qteP_/qteG_/qteM_<care>
                break;
            case "fight_start":
                _enemy = Str(d, "enemy");
                break;
            case "fight_press":
                switch (Str(d, "kind"))
                {
                    case "attack": Add(Str(d, "hit") == "Perfect" ? "fightAtkP" : "fightAtkG"); Add("fightRounds"); break;
                    case "dodge": Add("fightDodgeOk"); break;
                    default: Add("fightMissPress"); break;
                }
                Add("fightDmgDealt", Num(d, "dmgDealt"));
                if (_enemy == "Big Z") Add("bigzDmgDealt", Num(d, "dmgDealt"));
                break;
            case "fight_hurt":
                Add(Str(d, "cause") == "too_slow" ? "fightTooSlow" : "fightMissHurt");
                Add("fightDmgTaken", Num(d, "dmgTaken"));
                break;
            case "fight_end":
                if (_enemy == "Toothless" && Str(d, "won") == "True") C["toothlessWon"] = 1;
                break;
            case "coin":
                double delta = Num(d, "delta");
                string why = Str(d, "reason");
                if (delta > 0) Add("coinEarnedTotal", delta); else Add("coinSpentTotal", -delta);
                Add((delta > 0 ? "coinIn_" : "coinOut_") + why, Math.Abs(delta));
                break;
            case "energy":
                if (Str(d, "reason") == "care") { Add("energyUsedTotal"); _energyToday++; }
                break;
            case "upgrade_buy":
                Add("pointsSpent", Num(d, "pointCost"));
                break;
            case "pet_death":
                Add("petDeaths");
                Add("petDeaths_d" + (_gs?.Day ?? 0));
                if (!C.ContainsKey("deathDay")) C["deathDay"] = _gs?.Day ?? 0;
                break;
            case "pet_revive": Add("revives"); break;
            case "starve": Add("starveEvents"); break;
            case "door_event":
                if (Str(d, "event") == "merchant") S["merchantChoice"] = Str(d, "choice");
                break;
        }
    }

    static void RecordDayEnergy(int day, int left)
    {
        DayRecorded.Add(day);
        C[$"energyUsed_d{day}"] = _energyToday;
        C[$"energyLeft_d{day}"] = left;
        _energyToday = 0;
    }

    // ---------------- typed helpers (one-line hooks in game code) ----------------

    public static void Coin(int delta, string reason, int after) => Emit("coin", "delta", delta, "reason", reason, "coinAfter", after);

    public static void Energy(int delta, string reason, int after, int max) =>
        Emit("energy", "delta", delta, "reason", reason, "energyAfter", after, "maxEnergy", max);

    public static void Exp(int delta, string reason, GameState gs, bool levelUp) =>
        Emit("exp", "delta", delta, "reason", reason, "level", gs.PlayerLevel, "exp", gs.PlayerExp, "points", gs.Points, "levelUp", levelUp);

    public static void Snapshot(Pet p) =>
        Emit("pet_snapshot", "pet", p.Name, "species", p.Species.ToString(), "hp", p.HpShown, "maxHp", p.MaxHp, "stomach", (int)p.Stomach,
            "clean", (int)p.Clean, "progress", (int)p.Progress, "maxProgress", p.MaxProgress, "level", p.Level, "atk", p.Atk, "dead", p.Dead);

    public static void PetDeath(Pet p, string cause, float hpBefore)
    {
        if (!Active) return;
        int total = (int)C.GetValueOrDefault("petDeaths") + 1;
        Emit("pet_death", "pet", p.Name, "cause", cause, "hpBefore", (int)MathF.Ceiling(hpBefore), "deathCountTotal", total);
    }

    public static void Revive(Pet p, int cost) =>
        Emit("pet_revive", "pet", p.Name, "cost", cost, "reviveCountTotal", (int)C.GetValueOrDefault("revives") + 1);

    public static void DayEnd(GameState gs)
    {
        if (!Active) return;
        int used = _energyToday;
        foreach (var p in gs.Pets) Snapshot(p);
        Emit("day_end", "coin", gs.Coin, "energyLeft", gs.Energy, "energyUsedToday", used, "maxEnergy", gs.MaxEnergy, "points", gs.Points,
            "playerLevel", gs.PlayerLevel, "playerExp", gs.PlayerExp, "upgQte", gs.QteUpgrade, "upgEnergy", gs.EnergyUpgrade,
            "upgProgress", gs.ProgressUpgrade, "alive", gs.Alive.Count(), "dead", gs.Pets.Count(p => p.Dead));
        RecordDayEnergy(gs.Day, gs.Energy);
        Flush();
    }

    public static void DoorEvent(string ev, string choice) => Emit("door_event", "event", ev, "choice", choice);

    static string GitHash()
    {
        try
        {
            var psi = new ProcessStartInfo("git", "rev-parse --short HEAD")
            {
                RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false, CreateNoWindow = true,
                WorkingDirectory = AppContext.BaseDirectory,
            };
            using var p = Process.Start(psi);
            if (p == null) return "unknown";
            var s = p.StandardOutput.ReadToEnd().Trim();
            p.WaitForExit(2000);
            return s.Length > 0 ? s : "unknown";
        }
        catch { return "unknown"; }
    }
}
