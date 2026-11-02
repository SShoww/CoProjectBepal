<#
.SYNOPSIS
  Runs the BePal autoplay bot for N seeds x profiles with telemetry, then aggregates.
.EXAMPLE
  pwsh BEPAL/Docs/Balance/tools/run-batch.ps1 -Seeds 20 -Profiles perfect,sloppy,upgrade-first,careless
  pwsh BEPAL/Docs/Balance/tools/run-batch.ps1 -Seeds 5 -OutDir $env:TEMP\tel -Throttle 1 -NoAggregate
.NOTES
  Output: <OutDir>/*.jsonl (one file per run, raw), then ../runs.csv + ../summary.md via tools/Aggregate.
  Each run opens a (muted) game window for a few seconds. Runs are separate processes with their own output file
  (named by profile + seed), so -Throttle > 1 is safe; the shared %TEMP%/bepal_autoplay*.log is only a debug log.
#>
param(
    [int]$Seeds = 5,
    [int]$FirstSeed = 1,
    [string[]]$Profiles = @('perfect', 'sloppy', 'upgrade-first', 'caring', 'human'),
    [string]$OutDir = '',
    [int]$Throttle = 2,
    [switch]$Refuse,
    [switch]$NoBuild,
    [switch]$NoAggregate
)

$ErrorActionPreference = 'Stop'
$Profiles = @($Profiles | ForEach-Object { $_ -split ',' })   # also accepts -File ... -Profiles a,b
$repo = (Resolve-Path (Join-Path $PSScriptRoot '..\..\..\..')).Path
$project = Join-Path $repo 'BEPAL\Bepal_Game\Bepal'
if (-not $OutDir) { $OutDir = Join-Path $repo 'BEPAL\Docs\Balance\data\raw' }
New-Item -ItemType Directory -Force $OutDir | Out-Null
$OutDir = (Resolve-Path $OutDir).Path

if (-not $NoBuild) {
    dotnet build $project --nologo -v q
    if ($LASTEXITCODE -ne 0) { throw 'build failed' }
}

$jobs = foreach ($p in $Profiles) { foreach ($s in $FirstSeed..($FirstSeed + $Seeds - 1)) { [pscustomobject]@{ Profile = $p; Seed = $s } } }
Write-Host "Running $($jobs.Count) runs ($Seeds seeds x $($Profiles.Count) profiles), throttle $Throttle -> $OutDir"
$sw = [Diagnostics.Stopwatch]::StartNew()

$results = $jobs | ForEach-Object -ThrottleLimit $Throttle -Parallel {
    $extra = @(); if ($using:Refuse) { $extra += '--refuse' }
    $null = dotnet run --no-build --project $using:project -- --autoplay --bot $_.Profile --seed $_.Seed --telemetry $using:OutDir @extra
    [pscustomobject]@{ Profile = $_.Profile; Seed = $_.Seed; Exit = $LASTEXITCODE }
}

$bad = $results | Where-Object { $_.Exit -ne 0 }
Write-Host ("Done in {0:N0}s; {1} runs failed" -f $sw.Elapsed.TotalSeconds, @($bad).Count)
$bad | ForEach-Object { Write-Warning "failed: $($_.Profile) seed $($_.Seed) (exit $($_.Exit))" }

if (-not $NoAggregate) {
    $agg = Join-Path $PSScriptRoot 'Aggregate'
    dotnet run --project $agg -v q -- $OutDir (Split-Path $OutDir -Parent)
}
