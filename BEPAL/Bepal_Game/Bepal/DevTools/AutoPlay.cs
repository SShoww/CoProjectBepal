using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Bepal;

/// <summary>
/// Dev tool: <c>--autoplay [--refuse] [--bot &lt;profile&gt;] [--seed n]</c> drives the whole slice (menu -> Day 5 -> To be
/// continued) with injected input and writes <c>%TEMP%/bepal_autoplay.log</c>, to catch soft-locks and crashes. Picks
/// option 1 in every prompt (Tame, Sell) unless <c>--refuse</c> is given, which refuses the merchant and visits the shop.
/// Profiles (<c>--bot</c>): <c>perfect</c> (default; presses only on hits), <c>sloppy</c> (presses ~60% of zone passes plus
/// stray presses, so real misses happen), <c>upgrade-first</c> (spends Points/Coin on upgrades, revives, buys shop items;
/// implies --refuse), <c>careless</c> (only ever trains: never feeds or cleans; NOTE the perfect bot also always lands on Train, so the two currently play identically), <c>caring</c> (perfect presses, picks Feed/Clean/Heal by need).
/// </summary>
public class AutoPlay
{
    public static readonly string[] Profiles = { "perfect", "sloppy", "upgrade-first", "careless", "caring", "human" };

    readonly SceneManager _m;
    readonly bool _refuse;
    // human profile: per-seed parameters (drawn once from RunSeed with their own RNG)
    readonly Random _prm;
    readonly double _skill, _perfectWait, _stray, _appetite, _tameChance;
    readonly int _jitter;
    readonly double[] _careW = new double[4];     // Train, Feed, Clean, Heal
    readonly int[] _upgOrder = { 0, 1, 2 };        // upgrade rows by priority
    readonly double[] _shopP = new double[3];      // Crab Apple, Tonic, Sea Tea purchase probability
    int _hDay = -1, _stopEnergy;
    string _careTarget = "";
    bool _paramsLogged, _waitPerfect;
    Pet? _careFor;
    int _careEnergy = -1, _petCount = 2;
    readonly bool _patient;   // human: saves for the top-priority upgrade instead of buying the first affordable one
    Scene? _careScene;
    readonly Random _rng;
    int _frame;
    int _cooldown;
    string _lastTop = "";
    readonly HashSet<int> _doctorVisited = new();
    Point? _click;

    // sloppy: per-zone-pass decision
    bool _prevInZone, _willPress;
    // upgrade-first: shop items already bought during the current shop visit (bit mask)
    int _shopMask;

    public readonly string Profile;
    public bool Refuse => _refuse;
    /// <summary>True when a non-default profile is used (log file name then includes profile and seed).</summary>
    public bool CustomRun { get; }
    public readonly List<string> Log = new();
    public bool Finished;
    public bool Failed;

    public AutoPlay(SceneManager m, bool refuseMerchant, string profile = "perfect", int seed = 0)
    {
        if (!Profiles.Contains(profile))
            throw new ArgumentException($"unknown --bot profile '{profile}' (use: {string.Join(", ", Profiles)})");
        _m = m;
        Profile = profile;
        CustomRun = profile != "perfect";
        _refuse = refuseMerchant || profile == "upgrade-first";
        _rng = new Random(unchecked(seed * 7919 + 17));
        _prm = new Random(unchecked(seed * 104729 + 3));
        if (profile == "human")
        {
            _skill = 0.55 + 0.45 * _prm.NextDouble();
            _perfectWait = _prm.NextDouble();
            _stray = 0.004 * _prm.NextDouble();
            _jitter = _prm.Next(0, 25);
            _appetite = _prm.NextDouble() < 0.25 ? 0.15 + 0.35 * _prm.NextDouble() : 0.6 + 0.4 * _prm.NextDouble();
            _tameChance = _prm.NextDouble();
            _patient = _prm.NextDouble() < 0.5;
            for (int i = 0; i < 4; i++) _careW[i] = _prm.NextDouble() < 0.2 ? 0 : 0.2 + _prm.NextDouble();
            if (_careW.Sum() <= 0) _careW[0] = 1;
            for (int i = 2; i > 0; i--) { int j = _prm.Next(i + 1); (_upgOrder[i], _upgOrder[j]) = (_upgOrder[j], _upgOrder[i]); }
            for (int i = 0; i < 3; i++) _shopP[i] = _prm.NextDouble();
            _refuse = refuseMerchant || _prm.NextDouble() < 0.5;
        }
    }

    bool Sloppy => Profile == "sloppy";
    bool Human => Profile == "human";

    static readonly string[] CareNames = { nameof(Care.Train), nameof(Care.Feed), nameof(Care.Clean), nameof(Care.Heal) };

    string DrawCare()
    {
        double r = _rng.NextDouble() * _careW.Sum();
        for (int i = 0; i < 4; i++) { r -= _careW[i]; if (r <= 0) return CareNames[i]; }
        return CareNames[0];
    }

    /// <summary>human: per zone pass decide whether to press at all (skill) and whether to wait for Perfect or take an early Great.</summary>
    bool HumanPress(Hit hit, bool perfectOnly)
    {
        bool inZone = hit != Hit.Miss;
        if (inZone && !_prevInZone) { _willPress = _prm.NextDouble() < _skill; _waitPerfect = _prm.NextDouble() < _perfectWait; }
        _prevInZone = inZone;
        if (!inZone) return _rng.NextDouble() < _stray;
        if (!_willPress) return false;
        return (perfectOnly || _waitPerfect) ? hit == Hit.Perfect : _rng.NextDouble() < 0.5;
    }

    int Jitter => Human ? _rng.Next(0, _jitter + 1) : 0;

    /// <summary>Should the bot press Space now, given the wheel? perfectOnly = original QTE rule (Perfect only).</summary>
    bool WheelPress(Wheel w, bool perfectOnly)
    {
        var hit = w.Evaluate().hit;
        if (Human) return HumanPress(hit, perfectOnly);
        if (!Sloppy) return perfectOnly ? hit == Hit.Perfect : hit != Hit.Miss;
        bool inZone = hit != Hit.Miss;
        if (inZone && !_prevInZone) _willPress = _rng.NextDouble() < 0.6;
        _prevInZone = inZone;
        if (inZone) return _willPress && _rng.NextDouble() < 0.35;
        return _rng.NextDouble() < 0.002;   // stray press on a gap
    }

    public void Step()
    {
        _frame++;
        if (_m.Top is BaseScene bs0) _petCount = bs0.State.Pets.Count;
        _click = null;
        if (Human && !_paramsLogged && Telemetry.Active)
        {
            _paramsLogged = true;
            Telemetry.Emit("bot_params", "skill", _skill, "perfectWait", _perfectWait, "stray", _stray, "jitter", _jitter, "appetite", _appetite,
                "tameChance", _tameChance, "wTrain", _careW[0], "wFeed", _careW[1], "wClean", _careW[2], "wHeal", _careW[3],
                "upgOrder", string.Join(">", _upgOrder.Select(i => new[] { "QTE", "Energy", "Progress" }[i])),
                "patient", _patient, "shopApple", _shopP[0], "shopTonic", _shopP[1], "shopTea", _shopP[2], "refuseMerchant", _refuse);
            Log.Add($"bot_params skill={_skill:F2} appetite={_appetite:F2} jitter={_jitter} refuse={_refuse}");
        }
        var top = _m.Top;
        var name = top?.GetType().Name ?? "none";
        if (name != _lastTop)
        {
            string extra = top is BaseScene b
                ? $" day={b.State.Day} coin={b.State.Coin} energy={b.State.Energy} lv={b.State.PlayerLevel} pts={b.State.Points} pets=" +
                  string.Join(", ", b.State.Pets.Select(p => $"{p.Name}(hp{p.HpShown}/{p.MaxHp} s{(int)p.Stomach} c{(int)p.Clean} L{p.Level})"))
                : "";
            Log.Add($"[{_frame / 60f,7:F1}s] {name}{extra}");
            if (top is ShopScene && _lastTop != nameof(PetPickScene)) _shopMask = 0;   // PetPick (Crab Apple) returns to the same visit
            _lastTop = name;
            _prevInZone = false;
        }
        if (top is ToBeContinuedScene && !Finished)
        {
            Log.Add("RESULT: reached To be continued");
            Finished = true;
        }
        if (top is GameOverScene && !Finished)
        {
            Log.Add("RESULT: game over");
            Finished = true;
        }
        if (_frame > 60 * 60 * 30 && !Finished)
        {
            Log.Add($"RESULT: TIMEOUT (soft lock?) top={name}");
            Failed = true;
            Finished = true;
        }

        var keys = new List<Keys>();
        if (_cooldown > 0)
        {
            _cooldown--;
            Input.Inject(Array.Empty<Keys>());
            return;
        }

        switch (top)
        {
            case MainMenuScene or DialogueScene or ToBeContinuedScene or GameOverScene:
                keys.Add(Keys.Space);
                _cooldown = 3;
                break;
            case ChoiceScene c:
                bool second = c.Prompt.StartsWith("Toothless is at") || ((_refuse || _petCount < 2) && c.Prompt.StartsWith("Sell"));   // Sell is disabled with a single pet
                if (Human && c.Prompt.StartsWith("Toothless is at")) second = _prm.NextDouble() < _tameChance;
                keys.Add(second ? Keys.D2 : Keys.D1);
                _cooldown = 3;
                break;
            case PetPickScene:
                keys.AddRange(new[] { Keys.D1, Keys.D2, Keys.D3, Keys.D4 });
                _cooldown = 3;
                break;
            case ShopScene shop:
                if ((Profile == "upgrade-first" || Human) && ShopBrain(shop.State)) _cooldown = 4;
                else
                {
                    keys.Add(Keys.Escape);
                    _cooldown = 3;
                }
                break;
            case DoctorScene d:
                if ((Profile == "upgrade-first" || Human) && d.State.Coin >= Balance.ReviveCost && d.State.Pets.Any(p => p.Dead))
                    _click = DoctorScene.Card(0, d.State.Pets.Count(p => p.Dead)).Center;
                else keys.Add(Keys.Escape);
                _cooldown = 4;
                break;
            case UpgradeScene u:
                int pick = UpgradePick(u.State);
                if (pick >= 0) _click = UpgradeScene.PlusRect(pick).Center;
                else keys.Add(Keys.Escape);
                _cooldown = 4;
                break;
            case CareSelectScene cs:
                {
                    var (hit, zone) = cs.Wheel.Evaluate();
                    if (Human && (_careScene != cs))
                    {
                        _careScene = cs;
                        _careTarget = DrawCare();
                    }
                    string? only = Profile switch
                    {
                        "careless" => nameof(Care.Train),
                        "caring" => WantedCare(cs.Pet),
                        "human" => _careTarget,
                        _ => null,
                    };
                    bool ok = only != null ? hit != Hit.Miss && zone?.Label == only : WheelPress(cs.Wheel, false);
                    if (Human && ok) ok = _prm.NextDouble() < 0.5 + 0.5 * _skill;   // sometimes a human lets the zone slide by
                    if (ok)
                    {
                        keys.Add(Keys.Space);
                        _cooldown = 3 + Jitter;
                    }
                    break;
                }
            case QteScene q when WheelPress(q.Wheel, true):
            case FightScene f when WheelPress(f.Wheel, false):
                keys.Add(Keys.Space);
                _cooldown = 2 + Jitter;
                break;
            case BaseScene b:
                BaseBrain(b, keys);
                break;
        }
        Input.Inject(keys.ToArray(), _click);
    }

    /// <summary>caring: feed when hungry, clean when dirty, heal when hurt, otherwise train.</summary>
    static string WantedCare(Pet p) =>
        p.Stomach < 50 ? nameof(Care.Feed) : p.Clean < 50 ? nameof(Care.Clean) : p.Hp < p.MaxHp * 0.6f ? nameof(Care.Heal) : nameof(Care.Train);

    /// <summary>Index of the upgrade row to buy next (QTE -> Energy -> Progress), or -1.</summary>
    int UpgradePick(GameState gs)
    {
        foreach (int i in Human ? _upgOrder : new[] { 0, 1, 2 })
        {
            if (Human && _patient)
            {
                bool maxed = i == 0 ? gs.QteUpgrade >= Balance.QteMax
                    : i == 1 ? gs.EnergyUpgrade >= Balance.MaxEnergyCap - Balance.BaseEnergy : gs.ProgressUpgrade >= Balance.ProgressMax;
                if (maxed) continue;
                bool afford = i == 0 ? gs.Coin >= Balance.QteCoin && gs.Points >= Balance.QtePoints
                    : i == 1 ? gs.Coin >= Balance.EnergyCoin && gs.Points >= Balance.EnergyPoints
                    : gs.Coin >= Balance.ProgressCoin && gs.Points >= Balance.ProgressPoints;
                return afford ? i : -1;   // top-priority upgrade not affordable yet: wait
            }
            if (i == 0 && gs.QteUpgrade < Balance.QteMax && gs.Coin >= Balance.QteCoin && gs.Points >= Balance.QtePoints) return 0;
            if (i == 1 && gs.EnergyUpgrade < Balance.MaxEnergyCap - Balance.BaseEnergy && gs.Coin >= Balance.EnergyCoin && gs.Points >= Balance.EnergyPoints) return 1;
            if (i == 2 && gs.ProgressUpgrade < Balance.ProgressMax && gs.Coin >= Balance.ProgressCoin && gs.Points >= Balance.ProgressPoints) return 2;
        }
        return -1;
    }

    /// <summary>Buy each shop item once per visit if affordable (Sea Tea, Tonic, Apple). Returns true if it clicked.</summary>
    bool ShopBrain(GameState gs)
    {
        int[] price = { 25, 40, 18 };   // Crab Apple, Caffeine Tonic, Sea Tea (ShopScene.Items)
        foreach (int i in new[] { 2, 1, 0 })
        {
            if ((_shopMask & (1 << i)) != 0 || gs.Coin < price[i]) continue;
            if (i == 0 && !gs.Alive.Any()) continue;
            if (i == 2 && gs.SeaTea) continue;
            _shopMask |= 1 << i;
            if (Human && _prm.NextDouble() >= _shopP[i]) continue;
            _click = ShopScene.BuyRect(i).Center;
            return true;
        }
        return false;
    }

    void BaseBrain(BaseScene b, List<Keys> keys)
    {
        var gs = b.State;
        var pet = gs.Alive.FirstOrDefault();
        if (Human)
        {
            if (gs.Day != _hDay) { _hDay = gs.Day; _stopEnergy = gs.Energy - (int)Math.Round(gs.Energy * _appetite); }
            if (_careFor == null || _careFor.Dead || !gs.Pets.Contains(_careFor) || _careEnergy != gs.Energy)
            {
                var alive = gs.Alive.ToList();
                _careFor = alive.Count > 0 ? alive[_rng.Next(alive.Count)] : null;
                _careEnergy = gs.Energy;
            }
            pet = gs.Energy > _stopEnergy ? _careFor : null;
        }
        float target;
        BaseScene.Kind want;
        bool upg = Profile == "upgrade-first" || Human;
        if (gs.DoorEventPending) (target, want) = (BaseScene.DoorX, BaseScene.Kind.Door);
        else if (upg && gs.Pets.Any(p => p.Dead) && gs.Coin >= Balance.ReviveCost && !_doctorVisited.Contains(gs.Day))
        {
            (target, want) = (BaseScene.DoctorX, BaseScene.Kind.Doctor);
            if (b.Near?.Kind == want) _doctorVisited.Add(gs.Day);
        }
        else if (upg && UpgradePick(gs) >= 0) (target, want) = (BaseScene.UpgradeX, BaseScene.Kind.Upgrade);
        else if (gs.Energy > 0 && pet != null) (target, want) = (b.PetX(pet), BaseScene.Kind.Pet);
        else if (gs.Pets.Any(p => p.Dead) && !_doctorVisited.Contains(gs.Day))
        {
            (target, want) = (BaseScene.DoctorX, BaseScene.Kind.Doctor);
            if (b.Near?.Kind == want) _doctorVisited.Add(gs.Day);
        }
        else (target, want) = (BaseScene.BedX, BaseScene.Kind.Bed);

        if (b.Near is { } near && near.Kind == want && (want != BaseScene.Kind.Pet || near.Pet == pet))
        {
            keys.Add(Keys.Space);
            _cooldown = 4;
        }
        else keys.Add(target > b.PlayerX ? Keys.D : Keys.A);
    }
}
