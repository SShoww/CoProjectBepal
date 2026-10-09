// Aggregate: reads every *.jsonl telemetry file in <dir> (written by `Bepal --telemetry <dir>`), takes each file's
// `run_summary` row, and writes <out>/runs.csv (one row per run) and <out>/summary.md (per-profile statistics).
// Usage: dotnet run --project Aggregate -- <telemetryDir> [outDir]     (outDir defaults to <telemetryDir>/..)
using System.Globalization;
using System.Text;
using System.Text.Json;

if (args.Length < 1 || !Directory.Exists(args[0]))
{
    Console.Error.WriteLine("usage: Aggregate <telemetryDir> [outDir]");
    return 1;
}
string dir = Path.GetFullPath(args[0]);
string outDir = args.Length > 1 ? Path.GetFullPath(args[1]) : Path.GetFullPath(Path.Combine(dir, ".."));
Directory.CreateDirectory(outDir);
var inv = CultureInfo.InvariantCulture;

var runs = new List<Dictionary<string, object>>();
int skipped = 0;
foreach (var file in Directory.GetFiles(dir, "*.jsonl").OrderBy(f => f, StringComparer.Ordinal))
{
    Dictionary<string, object>? row = null;
    foreach (var line in File.ReadLines(file))
    {
        if (!line.Contains("\"run_summary\"")) continue;
        try
        {
            using var doc = JsonDocument.Parse(line);
            if (doc.RootElement.GetProperty("type").GetString() != "run_summary") continue;
            row = new Dictionary<string, object>();
            foreach (var p in doc.RootElement.EnumerateObject())
            {
                if (p.Name is "type" or "seq" or "scene" or "tReal") continue;
                row[p.Name] = p.Value.ValueKind switch
                {
                    JsonValueKind.Number => p.Value.GetDouble(),
                    JsonValueKind.True => 1.0,
                    JsonValueKind.False => 0.0,
                    _ => p.Value.ToString(),
                };
            }
        }
        catch (JsonException) { }
    }
    if (row == null) { skipped++; Console.Error.WriteLine("no run_summary (incomplete): " + Path.GetFileName(file)); continue; }
    row["file"] = Path.GetFileName(file);
    var startLine = File.ReadLines(file).FirstOrDefault(l => l.Contains("\"run_start\""));
    if (startLine != null)
        try
        {
            using var doc = JsonDocument.Parse(startLine);
            if (doc.RootElement.TryGetProperty("starter", out var st)) row["starter"] = st.ToString();
        }
        catch (JsonException) { }
    runs.Add(row);
}

double N(Dictionary<string, object> r, string k) => r.TryGetValue(k, out var v) && v is double d ? d : 0;
string S(Dictionary<string, object> r, string k) => r.TryGetValue(k, out var v) ? v.ToString() ?? "" : "";
string[] cares = { "Train", "Feed", "Clean", "Heal" };

// Derived columns
foreach (var r in runs)
{
    double qp = cares.Sum(c => N(r, "qteP_" + c)), qg = cares.Sum(c => N(r, "qteG_" + c)), qm = cares.Sum(c => N(r, "qteM_" + c));
    double qn = qp + qg + qm;
    r["qtePresses"] = qn;
    r["qtePerfectRate"] = qn > 0 ? qp / qn : 0.0;
    r["qteGreatRate"] = qn > 0 ? qg / qn : 0.0;
    r["qteMissRate"] = qn > 0 ? qm / qn : 0.0;
    double fp = N(r, "fightAtkP") + N(r, "fightAtkG") + N(r, "fightDodgeOk") + N(r, "fightMissPress");
    r["fightPresses"] = fp;
    r["fightHitRate"] = fp > 0 ? (fp - N(r, "fightMissPress")) / fp : 0.0;
    r["pressesTotal"] = N(r, "careSelectPresses") + qn + fp;
    r["upgradesTotal"] = N(r, "upgQte") + N(r, "upgEnergy") + N(r, "upgProgress");
    r["energyLeftTotal"] = Enumerable.Range(1, 5).Sum(d => N(r, "energyLeft_d" + d));
    r["profileKey"] = S(r, "profile") + (N(r, "refuse") > 0 ? "+refuse" : "");
}

// ---- runs.csv ----
string[] lead =
{
    "runId", "profileKey", "profile", "refuse", "seed", "build", "mode", "outcome", "dayReached", "deathDay", "deathScene", "tSimSec", "tRealSec",
    "pressesTotal", "careSelectPresses", "qtePresses", "qtePerfectRate", "qteGreatRate", "qteMissRate", "fightPresses", "fightHitRate",
    "startCoin", "finalCoin", "coinEarnedTotal", "coinSpentTotal", "energyUsedTotal", "energyLeftTotal", "upgradesTotal",
    "upgQte", "upgEnergy", "upgProgress", "pointsSpent", "pointsLeft", "playerLevel", "petDeaths", "revives", "starveEvents",
};
var cols = lead.Where(c => runs.Any(r => r.ContainsKey(c))).ToList();
foreach (var c in runs.SelectMany(r => r.Keys).Distinct().Where(c => !cols.Contains(c) && c != "file").OrderBy(c => c, StringComparer.Ordinal))
    cols.Add(c);
cols.Add("file");

string Csv(object? v) => v switch
{
    null => "",
    double d => d.ToString("0.###", inv),
    _ => Quote(v.ToString() ?? ""),
};
string Quote(string s) => s.Contains(',') || s.Contains('"') || s.Contains('\n') ? "\"" + s.Replace("\"", "\"\"") + "\"" : s;

var sb = new StringBuilder();
sb.AppendLine(string.Join(",", cols));
foreach (var r in runs)
    sb.AppendLine(string.Join(",", cols.Select(c => r.TryGetValue(c, out var v) ? Csv(v) : "")));
File.WriteAllText(Path.Combine(outDir, "runs.csv"), sb.ToString());

// ---- summary.md ----
static double Pct(List<double> sorted, double p)
{
    if (sorted.Count == 0) return double.NaN;
    double pos = (sorted.Count - 1) * p;
    int lo = (int)Math.Floor(pos), hi = (int)Math.Ceiling(pos);
    return sorted[lo] + (sorted[hi] - sorted[lo]) * (pos - lo);
}

(string label, string key)[] metrics =
{
    ("duration (sim s)", "tSimSec"), ("day reached", "dayReached"), ("presses total", "pressesTotal"), ("QTE presses", "qtePresses"),
    ("QTE Perfect rate", "qtePerfectRate"), ("QTE Great rate", "qteGreatRate"), ("QTE Miss rate", "qteMissRate"),
    ("fight presses", "fightPresses"), ("fight hit rate", "fightHitRate"),
    ("final coin", "finalCoin"), ("coin earned", "coinEarnedTotal"), ("coin spent", "coinSpentTotal"),
    ("energy used", "energyUsedTotal"), ("energy left (sum at day end)", "energyLeftTotal"),
    ("upgrades bought", "upgradesTotal"), ("points left", "pointsLeft"), ("player level", "playerLevel"),
    ("pet deaths", "petDeaths"), ("revives", "revives"), ("starve events", "starveEvents"),
};

var md = new StringBuilder();
md.AppendLine("# Telemetry summary");
md.AppendLine();
md.AppendLine($"Source: `{dir}` - {runs.Count} runs" + (skipped > 0 ? $" ({skipped} incomplete files skipped)" : "") + ". Generated by `BEPAL/Docs/Balance/tools/Aggregate`.");
foreach (var g in runs.GroupBy(r => S(r, "profileKey")).OrderBy(g => g.Key, StringComparer.Ordinal))
{
    var list = g.ToList();
    md.AppendLine();
    md.AppendLine($"## Profile `{g.Key}` (n = {list.Count}, seeds {string.Join(",", list.Select(r => N(r, "seed").ToString("0", inv)).OrderBy(x => x, StringComparer.Ordinal))})");
    md.AppendLine();
    md.AppendLine("| metric | mean | median | p10 | p90 |");
    md.AppendLine("|---|---:|---:|---:|---:|");
    foreach (var (label, key) in metrics)
    {
        var v = list.Select(r => N(r, key)).OrderBy(x => x).ToList();
        string F(double d) => key.EndsWith("Rate") ? (d * 100).ToString("0.0", inv) + "%" : d.ToString("0.##", inv);
        md.AppendLine($"| {label} | {F(v.Average())} | {F(Pct(v, 0.5))} | {F(Pct(v, 0.1))} | {F(Pct(v, 0.9))} |");
    }
    md.AppendLine();
    md.AppendLine("Outcomes: " + string.Join(", ", list.GroupBy(r => S(r, "outcome")).OrderBy(x => x.Key, StringComparer.Ordinal).Select(x => $"{x.Key} = {x.Count()}")));
    md.AppendLine();
    md.AppendLine("Pet deaths by day (sum over runs): " + string.Join(", ", Enumerable.Range(1, 5).Select(d => $"d{d} = {list.Sum(r => N(r, "petDeaths_d" + d)):0}")));
    var firstDeaths = list.Where(r => N(r, "deathDay") > 0).GroupBy(r => (int)N(r, "deathDay")).OrderBy(x => x.Key).ToList();
    md.AppendLine();
    md.AppendLine("Runs with a first pet death, by day: " + (firstDeaths.Count == 0 ? "none" : string.Join(", ", firstDeaths.Select(x => $"d{x.Key} = {x.Count()}"))));
}
File.WriteAllText(Path.Combine(outDir, "summary.md"), md.ToString());

// ---- players.md (real play: one section per --player name, one column per run; no averaging) ----
var played = runs.Where(r => S(r, "mode") == "play" || S(r, "player") != "").ToList();
if (played.Count > 0)
{
    string P(double d) => (d * 100).ToString("0.0", inv) + "%";
    string I(double d) => d.ToString("0.##", inv);
    string Started(Dictionary<string, object> r)
    {
        var parts = S(r, "runId").Split('_');   // run_yyyyMMdd_HHmmss_...
        return parts.Length > 2 && parts[1].Length == 8 && parts[2].Length == 6
            ? $"{parts[1][..4]}-{parts[1][4..6]}-{parts[1][6..]} {parts[2][..2]}:{parts[2][2..4]}" : S(r, "runId");
    }
    string Qte(Dictionary<string, object> r, string care)
    {
        double p = N(r, "qteP_" + care), g = N(r, "qteG_" + care), m = N(r, "qteM_" + care);
        return p + g + m == 0 ? "-" : $"{p:0} / {g:0} / {m:0}";
    }
    string Sources(Dictionary<string, object> r, string prefix) =>
        string.Join(", ", r.Keys.Where(k => k.StartsWith(prefix)).OrderBy(k => k, StringComparer.Ordinal).Select(k => $"{k[prefix.Length..]} {N(r, k):0}")) is { Length: > 0 } s ? s : "-";
    (string label, Func<Dictionary<string, object>, string> get)[] rows =
    {
        ("started", Started), ("build", r => S(r, "build")), ("starter", r => S(r, "starter")),
        ("outcome", r => S(r, "outcome")), ("day reached", r => I(N(r, "dayReached"))),
        ("first pet death (day)", r => N(r, "deathDay") > 0 ? I(N(r, "deathDay")) : "none"),
        ("real time (min)", r => (N(r, "tRealSec") / 60).ToString("0.0", inv)),
        ("presses total", r => I(N(r, "pressesTotal"))),
        ("care select ok / presses", r => $"{N(r, "careSelect_ok"):0} / {N(r, "careSelectPresses"):0}"),
        ("QTE Perfect / Great / Miss", r => $"{P(N(r, "qtePerfectRate"))} / {P(N(r, "qteGreatRate"))} / {P(N(r, "qteMissRate"))}"),
        ("QTE Train P/G/M", r => Qte(r, "Train")), ("QTE Feed P/G/M", r => Qte(r, "Feed")),
        ("QTE Clean P/G/M", r => Qte(r, "Clean")), ("QTE Heal P/G/M", r => Qte(r, "Heal")),
        ("fight hit rate", r => P(N(r, "fightHitRate"))),
        ("fight attack P / G", r => $"{N(r, "fightAtkP"):0} / {N(r, "fightAtkG"):0}"),
        ("fight dodge ok / too slow / miss press", r => $"{N(r, "fightDodgeOk"):0} / {N(r, "fightTooSlow"):0} / {N(r, "fightMissPress"):0}"),
        ("fight dmg dealt / taken", r => $"{N(r, "fightDmgDealt"):0} / {N(r, "fightDmgTaken"):0}"),
        ("Toothless won", r => N(r, "toothlessWon") > 0 ? "yes" : "no"), ("Big Z dmg dealt", r => I(N(r, "bigzDmgDealt"))),
        ("merchant", r => S(r, "merchantChoice")),
        ("coin start / earned / spent / final", r => $"{N(r, "startCoin"):0} / {N(r, "coinEarnedTotal"):0} / {N(r, "coinSpentTotal"):0} / {N(r, "finalCoin"):0}"),
        ("coin in", r => Sources(r, "coinIn_")), ("coin out", r => Sources(r, "coinOut_")),
        ("energy used / left (total)", r => $"{N(r, "energyUsedTotal"):0} / {N(r, "energyLeftTotal"):0}"),
        ("energy used per day d1..d5", r => string.Join(" ", Enumerable.Range(1, 5).Select(d => N(r, "energyUsed_d" + d).ToString("0", inv)))),
        ("upgrades QTE / Energy / Progress", r => $"{N(r, "upgQte"):0} / {N(r, "upgEnergy"):0} / {N(r, "upgProgress"):0}"),
        ("points spent / left", r => $"{N(r, "pointsSpent"):0} / {N(r, "pointsLeft"):0}"),
        ("player level", r => I(N(r, "playerLevel"))), ("pet level max", r => I(N(r, "petLvlMax"))),
        ("pets final / alive", r => $"{N(r, "petsFinal"):0} / {N(r, "petsAliveFinal"):0}"),
        ("pet deaths / revives / starve", r => $"{N(r, "petDeaths"):0} / {N(r, "revives"):0} / {N(r, "starveEvents"):0}"),
        ("final stomach / clean avg", r => $"{I(N(r, "finalStomachAvg"))} / {I(N(r, "finalCleanAvg"))}"),
    };

    var pm = new StringBuilder();
    pm.AppendLine("# Telemetry per player");
    pm.AppendLine();
    pm.AppendLine($"Source: `{dir}` - {played.Count} real-play runs. One section per `--player` name, one column per run (oldest first); nothing is averaged. Generated by `BEPAL/Docs/Balance/tools/Aggregate`. QTE P/G/M = Perfect / Great / Miss presses.");
    foreach (var g in played.GroupBy(r => S(r, "player") is { Length: > 0 } n ? n : "(no name)").OrderBy(g => g.Key, StringComparer.Ordinal))
    {
        var list = g.OrderBy(r => S(r, "runId"), StringComparer.Ordinal).ToList();
        pm.AppendLine();
        pm.AppendLine($"## {g.Key} ({list.Count} run{(list.Count == 1 ? "" : "s")}; reached To be continued {list.Count(r => S(r, "outcome") == "to_be_continued")}, game over {list.Count(r => S(r, "outcome") == "game_over")})");
        pm.AppendLine();
        pm.AppendLine("| | " + string.Join(" | ", list.Select((_, i) => $"run {i + 1}")) + " |");
        pm.AppendLine("|---|" + string.Concat(list.Select(_ => "---|")));
        foreach (var (label, get) in rows)
            pm.AppendLine($"| {label} | " + string.Join(" | ", list.Select(get)) + " |");
        pm.AppendLine();
        pm.AppendLine("Files: " + string.Join(", ", list.Select((r, i) => $"run {i + 1} = `{S(r, "file")}`")));
    }
    File.WriteAllText(Path.Combine(outDir, "players.md"), pm.ToString());
    Console.WriteLine($"{played.Count} real-play runs -> {Path.Combine(outDir, "players.md")}");
}

Console.WriteLine($"{runs.Count} runs -> {Path.Combine(outDir, "runs.csv")}, {Path.Combine(outDir, "summary.md")}");
return 0;
