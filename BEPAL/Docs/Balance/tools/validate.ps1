# Telemetry validator (Q-20261009-run-telemetry). PowerShell 7.
# Usage: pwsh BEPAL/Docs/Balance/tools/validate.ps1
# Reads data/raw/*.jsonl + data/runs.csv, writes data/validation.md
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$rawDir = Join-Path $root 'data/raw'
$csvPath = Join-Path $root 'data/runs.csv'
$outPath = Join-Path $root 'data/validation.md'
$inv = [ordered]@{}
$invTitle = [ordered]@{
  I01 = 'Coin: start+sum(delta)==final; earned-spent==final-start; coinAfter chain; coin>=0; sinks==upgrade/shop events'
  I02 = 'Energy: used/day==care_select ok; used<=max+tonic; energyAfter chain & in [0,max+2*tonics]; day_end energy>=0'
  I03 = 'QTE: perfect+great+miss==10; 10 qte_press per qte_end; qte_end==care_select ok; qte_end counts == presses'
  I04 = 'care_select: ok count == energy(care) events; gap/noenergy never cost energy'
  I05 = 'EXP: sum(exp qte)==non-miss presses; points==level-1-pointsSpent; pointsSpent==sum(pointCost); exp<PlayerMaxExp(10+(L-1)*10)'
  I06 = 'Upgrades: max levels; maxEnergy==min(3+upgEnergy,6); cost per purchase == Balance'
  I07 = 'Fight: fight_end counters/dmg == recomputed from fight_press/hurt; won->enemyHp==0; Toothless dmg>=120; petsLost<=pets'
  I08 = 'Pet: 0<=stomach,clean<=100; 0<=hp<=maxHp; progress<maxProgress; level non-decreasing; revives*250==spent; deaths>=revives'
  I09 = 'Coin sources: daily==100*(dayReached-1); toothless in {0,200}; merchant in {0,5000}; sell->pet removed & no shop_buy'
  I10 = 'Run structure: 1 run_start/1 run_end/1 run_summary(last); seq contiguous; day & t monotonic; tReal>0; outcome valid; TBC->day5'
  I11 = 'day_end exactly once for each day 1..(dayReached-1)'
  I12 = 'run_summary counters == recomputed from events'
  I13 = 'No negative stats (hp, stomach, clean, coin, energy, exp, points, counters)'
  I14 = 'Deaths consistent: pet_death count==summary; deathDay==first death; starve died->pet_death; petsAliveFinal==non-dead snapshots'
  I15 = 'runs.csv matches jsonl (row set, shared columns vs run_summary)'
}
foreach ($k in $invTitle.Keys) { $inv[$k] = [ordered]@{} }
function Fail($i, $run, $msg) {
  if (-not $inv[$i].Contains($run)) { $inv[$i][$run] = [System.Collections.Generic.List[string]]::new() }
  if ($inv[$i][$run].Count -lt 3) { $inv[$i][$run].Add($msg) }
}
function N($x) { if ($null -eq $x -or $x -eq '') { 0 } else { [double]$x } }
function Eq($a, $b) { [math]::Abs((N $a) - (N $b)) -lt 0.0015 }
function Sum($arr, $f) { $s = 0.0; foreach ($e in $arr) { $s += N $e[$f] }; $s }

$runs = [ordered]@{}
$files = Get-ChildItem $rawDir -Filter *.jsonl | Sort-Object Name
foreach ($f in $files) {
  $ev = [System.Collections.Generic.List[hashtable]]::new()
  foreach ($line in [System.IO.File]::ReadLines($f.FullName)) { if ($line.Trim()) { $ev.Add((ConvertFrom-Json $line -AsHashtable)) } }
  $runs[$f.BaseName] = $ev
}
$rows = @{}
$sumRows = @{}
foreach ($rid in $runs.Keys) {
  $ev = $runs[$rid]
  $T = @($ev | ForEach-Object { $_['type'] })
  $by = @{}
  foreach ($e in $ev) { if (-not $by.ContainsKey($e['type'])) { $by[$e['type']] = [System.Collections.Generic.List[hashtable]]::new() }; $by[$e['type']].Add($e) }
  function Of($t) { if ($by.ContainsKey($t)) { ,@($by[$t]) } else { ,@() } }
  $rs = Of 'run_start'; $re = Of 'run_end'; $su = Of 'run_summary'
  # ---- I10
  if ($rs.Count -ne 1) { Fail 'I10' $rid "run_start count $($rs.Count)" }
  if ($re.Count -ne 1) { Fail 'I10' $rid "run_end count $($re.Count)" }
  if ($su.Count -ne 1) { Fail 'I10' $rid "run_summary count $($su.Count)" }
  if ($ev[$ev.Count-1]['type'] -ne 'run_summary') { Fail 'I10' $rid 'last event is not run_summary' }
  if ($ev[0]['type'] -ne 'run_start') { Fail 'I10' $rid 'first event is not run_start' }
  for ($i = 0; $i -lt $ev.Count; $i++) {
    if ([int]$ev[$i]['seq'] -ne $i) { Fail 'I10' $rid "seq gap at index $i (seq=$($ev[$i]['seq']))"; break }
    if ($ev[$i]['runId'] -ne $rid) { Fail 'I10' $rid "runId mismatch at seq $i"; break }
  }
  for ($i = 1; $i -lt $ev.Count; $i++) {
    if ((N $ev[$i]['day']) -lt (N $ev[$i-1]['day'])) { Fail 'I10' $rid "day decreased at seq $i"; break }
    if ((N $ev[$i]['t']) -lt (N $ev[$i-1]['t']) - 0.0005) { Fail 'I10' $rid "t decreased at seq $i"; break }
    if ((N $ev[$i]['tReal']) -lt (N $ev[$i-1]['tReal']) - 0.0005) { Fail 'I10' $rid "tReal decreased at seq $i"; break }
  }
  if ($su.Count -ne 1 -or $rs.Count -ne 1 -or $re.Count -ne 1) { continue }
  $S = $su[0]; $R0 = $rs[0]; $RE = $re[0]
  $sumRows[$rid] = $S
  if (-not ((N $S['tRealSec']) -gt 0)) { Fail 'I10' $rid 'tRealSec<=0' }
  if ($RE['outcome'] -notin 'to_be_continued', 'game_over', 'quit', 'timeout') { Fail 'I10' $rid "bad outcome $($RE['outcome'])" }
  if ($S['outcome'] -ne $RE['outcome'] -or (N $S['dayReached']) -ne (N $RE['dayReached'])) { Fail 'I10' $rid 'summary outcome/dayReached != run_end' }
  $dayReached = [int](N $RE['dayReached'])
  if ($RE['outcome'] -eq 'to_be_continued' -and $dayReached -ne 5) { Fail 'I10' $rid "TBC but dayReached=$dayReached" }

  # ---- I01 coin
  $coins = Of 'coin'; $startCoin = N $R0['startCoin']; $cur = $startCoin; $minCoin = $startCoin
  foreach ($c in $coins) {
    if (-not (Eq ($cur + (N $c['delta'])) $c['coinAfter'])) { Fail 'I01' $rid "coinAfter chain break at seq $($c['seq'])"; }
    $cur = N $c['coinAfter']; if ($cur -lt $minCoin) { $minCoin = $cur }
  }
  if ($minCoin -lt 0) { Fail 'I01' $rid "coin<0 ($minCoin)" }
  $dsum = Sum $coins 'delta'
  if (-not (Eq ($startCoin + $dsum) $S['finalCoin'])) { Fail 'I01' $rid "start+sum(delta)=$($startCoin+$dsum) != finalCoin $($S['finalCoin'])" }
  $earn = 0.0; $spent = 0.0
  foreach ($c in $coins) { if ((N $c['delta']) -gt 0) { $earn += N $c['delta'] } else { $spent -= N $c['delta'] } }
  if (-not (Eq $earn $S['coinEarnedTotal'])) { Fail 'I01' $rid "earned recomputed $earn != summary $($S['coinEarnedTotal'])" }
  if (-not (Eq ($earn - $spent) ((N $S['finalCoin']) - $startCoin))) { Fail 'I01' $rid 'earned-spent != final-start' }
  $upg = Of 'upgrade_buy'; $shop = Of 'shop_buy'
  $upgCoin = 0.0; foreach ($c in $coins) { if ($c['reason'] -eq 'upgrade') { $upgCoin -= N $c['delta'] } }
  if (-not (Eq $upgCoin (Sum $upg 'coinCost'))) { Fail 'I01' $rid "coin(upgrade) $upgCoin != sum upgrade_buy.coinCost" }
  $shopCoin = 0.0; foreach ($c in $coins) { if ("$($c['reason'])".StartsWith('shop_')) { $shopCoin -= N $c['delta'] } }
  if (-not (Eq $shopCoin (Sum $shop 'price'))) { Fail 'I01' $rid "coin(shop_*) $shopCoin != sum shop_buy.price" }

  # ---- I02 / I04 energy & care
  $cs = Of 'care_select'; $en = Of 'energy'
  $ok = @($cs | Where-Object { $_['result'] -eq 'ok' })
  $careEn = @($en | Where-Object { $_['reason'] -eq 'care' })
  if ($ok.Count -ne $careEn.Count) { Fail 'I04' $rid "care_select ok=$($ok.Count) vs energy(care)=$($careEn.Count)" }
  foreach ($c in $cs) { if ($c['result'] -ne 'ok' -and ($c['result'] -notin 'gap', 'noenergy')) { Fail 'I04' $rid "unknown result $($c['result'])" } }
  foreach ($c in $careEn) { if ((N $c['delta']) -ne -1) { Fail 'I04' $rid "care energy delta $($c['delta'])" } }
  $ec = N $R0['startEnergy']
  $maxE = 3; $tonicByDay = @{}
  foreach ($e in $en) {
    if (-not (Eq ($ec + (N $e['delta'])) $e['energyAfter'])) { Fail 'I02' $rid "energy chain break at seq $($e['seq'])" }
    $ec = N $e['energyAfter']; $maxE = N $e['maxEnergy']
    if ($e['reason'] -eq 'tonic') { $d = [int]$e['day']; $tonicByDay[$d] = (N $tonicByDay[$d]) + 1 }
    $allow = $maxE + 2 * (N $tonicByDay[[int]$e['day']])
    if ((N $e['energyAfter']) -lt 0 -or (N $e['energyAfter']) -gt $allow) { Fail 'I02' $rid "energyAfter $($e['energyAfter']) outside [0,$allow] seq $($e['seq'])" }
  }
  $okByDay = @{}; foreach ($c in $ok) { $d = [int]$c['day']; $okByDay[$d] = (N $okByDay[$d]) + 1 }
  $careEnByDay = @{}; foreach ($c in $careEn) { $d = [int]$c['day']; $careEnByDay[$d] = (N $careEnByDay[$d]) + 1 }
  $dayEnds = Of 'day_end'
  foreach ($d in 1..5) {
    $k = "energyUsed_d$d"
    if ($S.ContainsKey($k) -or $okByDay.ContainsKey($d)) {
      if (-not (Eq $S[$k] $okByDay[$d])) { Fail 'I02' $rid "summary $k=$($S[$k]) != care_select ok on day $d ($(N $okByDay[$d]))" }
    }
  }
  foreach ($de in $dayEnds) {
    $d = [int]$de['day']
    $allow = (N $de['maxEnergy']) + 2 * (N $tonicByDay[$d])
    if ((N $de['energyUsedToday']) -gt $allow) { Fail 'I02' $rid "day $d energyUsed $($de['energyUsedToday']) > $allow" }
    if ((N $de['energyLeft']) -lt 0) { Fail 'I02' $rid "day $d energyLeft<0" }
    if (-not (Eq $de['energyUsedToday'] $okByDay[$d])) { Fail 'I02' $rid "day_end $d energyUsedToday != ok care_select" }
  }
  if (-not (Eq $S['energyUsedTotal'] $ok.Count)) { Fail 'I02' $rid 'energyUsedTotal != ok count' }

  # ---- I03 qte
  $qe = Of 'qte_end'; $qp = Of 'qte_press'
  foreach ($q in $qe) {
    if ((N $q['perfect']) + (N $q['great']) + (N $q['miss']) -ne 10) { Fail 'I03' $rid "qte_end seq $($q['seq']) p+g+m != 10" }
    if ((N $q['attempts']) -ne 10) { Fail 'I03' $rid "qte_end attempts $($q['attempts'])" }
  }
  if ($qp.Count -ne 10 * $qe.Count) { Fail 'I03' $rid "qte_press $($qp.Count) != 10*qte_end $($qe.Count)" }
  if ($qe.Count -ne $ok.Count) { Fail 'I03' $rid "qte_end $($qe.Count) != care_select ok $($ok.Count)" }
  # per-qte consistency: presses between consecutive qte_end
  $buf = @(); $ix = 0
  foreach ($e in $ev) {
    if ($e['type'] -eq 'qte_press') { $buf += $e }
    elseif ($e['type'] -eq 'qte_end') {
      $p = @($buf | Where-Object { $_['hit'] -eq 'Perfect' }).Count; $g = @($buf | Where-Object { $_['hit'] -eq 'Great' }).Count; $m = @($buf | Where-Object { $_['hit'] -eq 'Miss' }).Count
      if ($p -ne (N $e['perfect']) -or $g -ne (N $e['great']) -or $m -ne (N $e['miss'])) { Fail 'I03' $rid "qte_end seq $($e['seq']) counts != its presses" }
      $idx = @($buf | ForEach-Object { [int]$_['idx'] }); if (($idx -join ',') -ne '1,2,3,4,5,6,7,8,9,10') { Fail 'I03' $rid "qte idx sequence $($idx -join ',') at seq $($e['seq'])" }
      $buf = @()
    }
  }
  if ($buf.Count) { Fail 'I03' $rid "$($buf.Count) dangling qte_press" }

  # ---- I05 exp / points
  $exps = Of 'exp'
  $qexp = Sum @($exps | Where-Object { $_['reason'] -eq 'qte' }) 'delta'
  $nonMiss = @($qp | Where-Object { $_['hit'] -ne 'Miss' }).Count
  if (-not (Eq $qexp $nonMiss)) { Fail 'I05' $rid "sum exp(qte)=$qexp != non-miss presses $nonMiss" }
  if (-not (Eq (Sum $qp 'expGain') $nonMiss)) { Fail 'I05' $rid 'sum expGain != non-miss presses' }
  foreach ($x in $exps) {
    $maxX = 10 + ((N $x['level']) - 1) * 10
    if ((N $x['exp']) -ge $maxX) { Fail 'I05' $rid "exp $($x['exp']) >= PlayerMaxExp $maxX (lvl $($x['level'])) seq $($x['seq'])" }
  }
  $spentPts = 0.0; $upgSeq = @($upg)
  $spentPtsTotal = Sum $upg 'pointCost'
  if (-not (Eq $S['pointsSpent'] $spentPtsTotal)) { Fail 'I05' $rid "summary pointsSpent != sum pointCost" }
  if (-not (Eq $S['pointsLeft'] ((N $S['playerLevel']) - 1 - $spentPtsTotal))) { Fail 'I05' $rid "pointsLeft $($S['pointsLeft']) != level-1-spent" }
  foreach ($de in $dayEnds) {
    $sp = 0.0; foreach ($u in $upg) { if ((N $u['seq']) -lt (N $de['seq'])) { $sp += N $u['pointCost'] } }
    if (-not (Eq $de['points'] ((N $de['playerLevel']) - 1 - $sp))) { Fail 'I05' $rid "day_end $($de['day']) points != level-1-spent" }
  }

  # ---- I06 upgrades
  $lvl = @{ QTE = 0; Energy = 0; Progress = 0 }; $cost = @{ QTE = @(100, 1); Energy = @(150, 3); Progress = @(20, 2) }; $mx = @{ QTE = 3; Energy = 3; Progress = 2 }
  foreach ($u in $upg) {
    $n = $u['name']; if (-not $lvl.ContainsKey($n)) { Fail 'I06' $rid "unknown upgrade $n"; continue }
    $lvl[$n]++
    if ((N $u['newLevel']) -ne $lvl[$n]) { Fail 'I06' $rid "$n newLevel $($u['newLevel']) expected $($lvl[$n])" }
    if ($lvl[$n] -gt $mx[$n]) { Fail 'I06' $rid "$n above max" }
    if ((N $u['coinCost']) -ne $cost[$n][0] -or (N $u['pointCost']) -ne $cost[$n][1]) { Fail 'I06' $rid "$n cost $($u['coinCost'])c/$($u['pointCost'])p" }
  }
  if ((N $S['upgQte']) -ne $lvl.QTE -or (N $S['upgEnergy']) -ne $lvl.Energy -or (N $S['upgProgress']) -ne $lvl.Progress) { Fail 'I06' $rid 'summary upgrade levels != upgrade_buy counts' }
  foreach ($de in $dayEnds) { if ((N $de['maxEnergy']) -ne [math]::Min(3 + (N $de['upgEnergy']), 6)) { Fail 'I06' $rid "day_end $($de['day']) maxEnergy" } }
  foreach ($e in $en) { if ((N $e['maxEnergy']) -gt 6) { Fail 'I06' $rid 'maxEnergy>6'; break } }

  # ---- I07 fight
  $fs = Of 'fight_start'; $fe = Of 'fight_end'; $fp = Of 'fight_press'; $fh = Of 'fight_hurt'; $sw = Of 'fight_swap'
  if ($fs.Count -ne $fe.Count) { Fail 'I07' $rid "fight_start $($fs.Count) != fight_end $($fe.Count)" }
  foreach ($x in $fe) {
    $lo = ($fs | Where-Object { (N $_['seq']) -lt (N $x['seq']) } | Select-Object -Last 1)['seq']
    $P = @($fp | Where-Object { (N $_['seq']) -gt $lo -and (N $_['seq']) -lt (N $x['seq']) })
    $H = @($fh | Where-Object { (N $_['seq']) -gt $lo -and (N $_['seq']) -lt (N $x['seq']) })
    $atk = @($P | Where-Object { $_['kind'] -eq 'attack' }); $ded = @($P | Where-Object { $_['kind'] -eq 'dodge' }); $mp = @($P | Where-Object { $_['kind'] -eq 'miss' })
    $atkHit = @($atk | Where-Object { $_['hit'] -ne 'Miss' }).Count
    $dodgeOk = @($ded | Where-Object { $_['hit'] -ne 'Miss' }).Count
    $slow = @($H | Where-Object { $_['cause'] -eq 'too_slow' }).Count
    $missP = @($H | Where-Object { $_['cause'] -eq 'miss_press' }).Count
    $tag = "fight_end seq $($x['seq'])"
    if ($atkHit -ne (N $x['attackHits'])) { Fail 'I07' $rid "$tag attackHits $($x['attackHits']) != $atkHit" }
    if ($dodgeOk -ne (N $x['dodgeOk'])) { Fail 'I07' $rid "$tag dodgeOk $($x['dodgeOk']) != $dodgeOk" }
    if ($slow -ne (N $x['tooSlow'])) { Fail 'I07' $rid "$tag tooSlow $($x['tooSlow']) != $slow" }
    if ($mp.Count -ne (N $x['missPress'])) { Fail 'I07' $rid "$tag missPress $($x['missPress']) != $($mp.Count)" }
    if (-not (Eq (Sum $P 'dmgDealt') $x['dmgDealt'])) { Fail 'I07' $rid "$tag dmgDealt $($x['dmgDealt']) != sum presses $(Sum $P 'dmgDealt')" }
    if (-not (Eq (Sum $H 'dmgTaken') $x['dmgTaken'])) { Fail 'I07' $rid "$tag dmgTaken $($x['dmgTaken']) != sum hurt $(Sum $H 'dmgTaken')" }
    if ($x['won'] -eq $true -and $P.Count -gt 0 -and (N $P[$P.Count-1]['enemyHp']) -ne 0) { Fail 'I07' $rid "$tag won but last enemyHp=$($P[$P.Count-1]['enemyHp'])" }
    if ($x['won'] -eq $true -and $x['enemy'] -eq 'Toothless' -and (N $x['dmgDealt']) -lt 120) { Fail 'I07' $rid "$tag Toothless won dmg<120" }
    if ((N $x['petsLost']) -gt 3) { Fail 'I07' $rid "$tag petsLost $($x['petsLost'])" }
    if (-not ((N $x['durationSim']) -gt 0)) { Fail 'I07' $rid "$tag durationSim<=0" }
  }
  if ($fe.Count) { if ((N $S['fightDmgDealt']) -ne (Sum $fe 'dmgDealt')) { Fail 'I07' $rid 'summary fightDmgDealt != sum fight_end' } }
  foreach ($h in $fh) { if ((N $h['hpLost']) -gt (N $h['dmgTaken'])) { Fail 'I07' $rid "hpLost>dmgTaken seq $($h['seq'])" } }

  # ---- I08 pets
  $snaps = Of 'pet_snapshot'; $lastLvl = @{}
  foreach ($p in $snaps) {
    $tag = "snap seq $($p['seq']) $($p['pet'])"
    if ((N $p['stomach']) -lt 0 -or (N $p['stomach']) -gt 100) { Fail 'I08' $rid "$tag stomach $($p['stomach'])" }
    if ((N $p['clean']) -lt 0 -or (N $p['clean']) -gt 100) { Fail 'I08' $rid "$tag clean $($p['clean'])" }
    if ((N $p['hp']) -lt 0 -or (N $p['hp']) -gt (N $p['maxHp'])) { Fail 'I08' $rid "$tag hp $($p['hp'])/$($p['maxHp'])" }
    if ((N $p['progress']) -ge (N $p['maxProgress'])) { Fail 'I08' $rid "$tag progress $($p['progress'])>=max $($p['maxProgress'])" }
    $key = $p['pet']; if ($lastLvl.ContainsKey($key) -and (N $p['level']) -lt $lastLvl[$key]) { Fail 'I08' $rid "$tag level decreased" }; $lastLvl[$key] = N $p['level']
  }
  $rev = Of 'pet_revive'
  $reviveSpent = 0.0; foreach ($c in $coins) { if ($c['reason'] -eq 'revive') { $reviveSpent -= N $c['delta'] } }
  if (-not (Eq ($rev.Count * 250) $reviveSpent)) { Fail 'I08' $rid "revives*250 != coin(revive) $reviveSpent" }
  $deaths = Of 'pet_death'
  if ($deaths.Count -lt $rev.Count) { Fail 'I08' $rid 'deaths < revives' }
  # care on dead pet: any qte/care_select between pet_death and pet_revive for that pet
  $dead = @{}
  foreach ($e in $ev) {
    switch ($e['type']) {
      'pet_death' { $dead[$e['pet']] = $true }
      'pet_revive' { $dead.Remove($e['pet']) }
      'care_select' { if ($e['result'] -eq 'ok' -and $dead.ContainsKey($e['pet'])) { Fail 'I08' $rid "care on dead pet $($e['pet']) seq $($e['seq'])" } }
    }
  }

  # ---- I09 sources
  $cin = @{}; foreach ($c in $coins) { if ((N $c['delta']) -gt 0) { $cin[$c['reason']] = (N $cin[$c['reason']]) + (N $c['delta']) } }
  $dailyN = @($coins | Where-Object { $_['reason'] -eq 'daily' }).Count
  if ((N $cin['daily']) -ne 100 * ($dayReached - 1)) { Fail 'I09' $rid "daily coin $(N $cin['daily']) != 100*(dayReached-1)=$(100*($dayReached-1))" }
  if ((N $cin['toothless_reward']) -notin 0, 200) { Fail 'I09' $rid "toothless_reward $($cin['toothless_reward'])" }
  if ((N $cin['merchant_sale']) -notin 0, 5000) { Fail 'I09' $rid "merchant_sale $($cin['merchant_sale'])" }
  $rem = @((Of 'pet_removed') | Where-Object { $_['via'] -eq 'merchant_sale' })
  if ((N $cin['merchant_sale']) -eq 5000) {
    if ($rem.Count -ne 1) { Fail 'I09' $rid "merchant sold but pet_removed=$($rem.Count)" }
    if ($shop.Count -gt 0) { Fail 'I09' $rid 'merchant sold but shop_buy present' }
  } elseif ($rem.Count -ne 0) { Fail 'I09' $rid 'pet_removed(merchant) without sale coin' }
  $mc = @((Of 'door_event') | Where-Object { $_['event'] -eq 'merchant' })
  if ($mc.Count -eq 1) { $want = $mc[0]['choice']; $got = $S['merchantChoice']; if ($want -ne $got) { Fail 'I09' $rid "merchantChoice $got != door_event $want" } }

  # ---- I11
  $deDays = @($dayEnds | ForEach-Object { [int]$_['day'] })
  $expected = if ($dayReached -ge 2) { 1..($dayReached - 1) } else { @() }
  if (($deDays -join ',') -ne ($expected -join ',')) { Fail 'I11' $rid "day_end days [$($deDays -join ',')] expected [$($expected -join ',')]" }

  # ---- I12 summary counters
  $cnt = @{}
  foreach ($p in $qp) { $k = "qte$(($p['hit']).Substring(0,1))_$($p['care'])"; $cnt[$k] = (N $cnt[$k]) + 1 }
  foreach ($k in @($S.Keys) | Where-Object { $_ -match '^qte[PGM]_' }) { if ((N $S[$k]) -ne (N $cnt[$k])) { Fail 'I12' $rid "$k summary $($S[$k]) != $(N $cnt[$k])" } }
  foreach ($k in $cnt.Keys) { if (-not $S.ContainsKey($k)) { Fail 'I12' $rid "$k missing in summary" } }
  foreach ($c in $coins) { }
  $cinAll = @{}; $cout = @{}
  foreach ($c in $coins) { if ((N $c['delta']) -gt 0) { $cinAll["coinIn_$($c['reason'])"] = (N $cinAll["coinIn_$($c['reason'])"]) + (N $c['delta']) } else { $cout["coinOut_$($c['reason'])"] = (N $cout["coinOut_$($c['reason'])"]) - (N $c['delta']) } }
  foreach ($k in $cinAll.Keys) { if (-not (Eq $S[$k] $cinAll[$k])) { Fail 'I12' $rid "$k summary $($S[$k]) != $($cinAll[$k])" } }
  foreach ($k in $cout.Keys) { if (-not (Eq $S[$k] $cout[$k])) { Fail 'I12' $rid "$k summary $($S[$k]) != $($cout[$k])" } }
  foreach ($k in @($S.Keys) | Where-Object { $_ -match '^coin(In|Out)_' }) { if (-not ($cinAll.ContainsKey($k) -or $cout.ContainsKey($k)) -and (N $S[$k]) -ne 0) { Fail 'I12' $rid "$k summary $($S[$k]) has no events" } }
  if ((N $S['careSelectPresses']) -ne $cs.Count) { Fail 'I12' $rid 'careSelectPresses' }
  if ((N $S['careSelect_ok']) -ne $ok.Count) { Fail 'I12' $rid 'careSelect_ok' }
  $gapN = @($cs | Where-Object { $_['result'] -eq 'gap' }).Count; if ($S.ContainsKey('careSelect_gap') -and (N $S['careSelect_gap']) -ne $gapN) { Fail 'I12' $rid 'careSelect_gap' }
  if ((N $S['starveEvents']) -ne (Of 'starve').Count) { Fail 'I12' $rid 'starveEvents' }
  $pdd = @{}; foreach ($d in $deaths) { $k = "petDeaths_d$($d['day'])"; $pdd[$k] = (N $pdd[$k]) + 1 }
  foreach ($k in $pdd.Keys) { if ((N $S[$k]) -ne $pdd[$k]) { Fail 'I12' $rid "$k summary $($S[$k]) != $($pdd[$k])" } }
  $aG = 0; $aP = 0; foreach ($x in $fp) { if ($x['kind'] -eq 'attack') { if ($x['hit'] -eq 'Great') { $aG++ } elseif ($x['hit'] -eq 'Perfect') { $aP++ } } }
  if ((N $S['fightAtkG']) -ne $aG -or (N $S['fightAtkP']) -ne $aP) { Fail 'I12' $rid 'fightAtkG/P' }
  if ((N $S['fightDodgeOk']) -ne (@($fp | Where-Object { $_['kind'] -eq 'dodge' -and $_['hit'] -ne 'Miss' }).Count)) { Fail 'I12' $rid 'fightDodgeOk' }
  if ((N $S['fightTooSlow']) -ne (@($fh | Where-Object { $_['cause'] -eq 'too_slow' }).Count)) { Fail 'I12' $rid 'fightTooSlow' }
  if ((N $S['fightMissPress']) -ne (@($fp | Where-Object { $_['kind'] -eq 'miss' }).Count)) { Fail 'I12' $rid 'fightMissPress' }
  if ((N $S['fightDmgTaken']) -ne (Sum $fe 'dmgTaken')) { Fail 'I12' $rid 'fightDmgTaken' }
  if ((N $S['petDeaths']) -ne $deaths.Count) { Fail 'I12' $rid 'petDeaths' }
  if ((N $S['startCoin']) -ne $startCoin) { Fail 'I12' $rid 'startCoin' }

  # ---- I13 negatives
  foreach ($e in $ev) {
    foreach ($k in 'hp', 'maxHp', 'stomach', 'clean', 'progress', 'coinAfter', 'energyAfter', 'energyLeft', 'points', 'pointsAfter', 'exp', 'level', 'petHp', 'enemyHp', 'hpBefore', 'hpAfter', 'dmgDealt', 'dmgTaken', 'hpLost', 'combo', 'playerExp', 'playerLevel') {
      if ($e.ContainsKey($k) -and $e[$k] -is [ValueType] -and (N $e[$k]) -lt 0) { Fail 'I13' $rid "$($e['type']).$k=$($e[$k]) seq $($e['seq'])" }
    }
  }
  foreach ($k in $S.Keys) { if ($k -notin 'deathDay', 'qteDeltaMin' -and $S[$k] -is [ValueType] -and (N $S[$k]) -lt 0) { Fail 'I13' $rid "run_summary.$k=$($S[$k])" } }

  # ---- I14 deaths
  if ($deaths.Count) { $fd = ($deaths | ForEach-Object { [int]$_['day'] } | Measure-Object -Minimum).Minimum } else { $fd = -1 }
  if ((N $S['deathDay']) -ne $fd) { Fail 'I14' $rid "deathDay summary $($S['deathDay']) != first death $fd" }
  foreach ($d in $deaths) { if ((N $d['deathCountTotal']) -ne ([array]::IndexOf(@($deaths), $d) + 1)) { Fail 'I14' $rid 'deathCountTotal not 1..n'; break } }
  foreach ($st in (Of 'starve')) {
    if ($st['died'] -eq $true) {
      $m = @($deaths | Where-Object { $_['pet'] -eq $st['pet'] -and $_['cause'] -eq 'starve' -and [int]$_['day'] -eq [int]$st['day'] })
      if ($m.Count -ne 1) { Fail 'I14' $rid "starve died but pet_death(starve) matches=$($m.Count) seq $($st['seq'])" }
    }
    if ((N $st['hpAfter']) -gt (N $st['hpBefore'])) { Fail 'I14' $rid 'starve hpAfter>hpBefore' }
  }
  foreach ($d in $deaths) { if ($d['cause'] -notin 'starve', 'fight', 'boss') { Fail 'I14' $rid "death cause $($d['cause'])" } }
  $lastSnaps = @($snaps | Where-Object { (N $_['seq']) -gt (N $dayEnds[-1]['seq']) } )
  if ($RE['outcome'] -ne 'quit') {
    $finalSnaps = [System.Collections.Generic.List[hashtable]]::new(); $lastRunEndSnap = @($snaps | Where-Object { (N $_['seq']) -lt (N $RE['seq']) -and (N $_['seq']) -gt ((N $RE['seq']) - 6) })
    if ($lastRunEndSnap.Count -gt 0) {
      $alive = @($lastRunEndSnap | Where-Object { $_['dead'] -ne $true }).Count
      if ($alive -ne (N $S['petsAliveFinal'])) { Fail 'I14' $rid "petsAliveFinal $($S['petsAliveFinal']) != alive in final snapshots $alive" }
    }
  }
}

# ---- I15 runs.csv
$csv = if (Test-Path $csvPath) { Import-Csv $csvPath } else { @() }
$csvIds = @($csv | ForEach-Object { $_.runId }); $jsIds = @($runs.Keys)
foreach ($m in ($jsIds | Where-Object { $_ -notin $csvIds })) { Fail 'I15' $m 'in jsonl, missing from runs.csv' }
foreach ($m in ($csvIds | Where-Object { $_ -notin $jsIds })) { Fail 'I15' $m 'in runs.csv, no jsonl' }
if (($csvIds | Select-Object -Unique).Count -ne $csvIds.Count) { Fail 'I15' 'runs.csv' 'duplicate runIds' }
$cmpCols = 0
foreach ($r in $csv) {
  if (-not $sumRows.ContainsKey($r.runId)) { continue }
  $S = $sumRows[$r.runId]
  foreach ($p in $r.PSObject.Properties) {
    $k = $p.Name; if ($k -in 'runId', 'profileKey', 'file', 'refuse', 't', 'pressesTotal') { continue }
    if ($k -in 'qtePresses', 'qtePerfectRate', 'qteGreatRate', 'qteMissRate', 'fightPresses', 'fightHitRate', 'energyLeftTotal', 'upgradesTotal', 'day') { continue }  # derived by Aggregate
    if (-not $S.ContainsKey($k)) { if ("$($p.Value)" -ne '' -and (N $p.Value) -ne 0) { Fail 'I15' $r.runId "csv $k='$($p.Value)' but absent from summary" }; continue }
    $a = "$($S[$k])"; $b = "$($p.Value)"
    $ok2 = if ($a -eq $b) { $true } elseif ($S[$k] -is [ValueType] -and $b -ne '') { Eq $S[$k] $b } elseif ($S[$k] -is [bool]) { "$a".ToLower() -eq $b.ToLower() } else { $false }
    if (-not $ok2) { Fail 'I15' $r.runId "csv $k='$b' != summary '$a'" }
  }
  if ((N $r.file.Length) -eq 0 -or $r.file -ne "$($r.runId).jsonl") { Fail 'I15' $r.runId "file column '$($r.file)'" }
}
# derived presses check
foreach ($r in $csv) {
  if (-not $runs.Contains($r.runId)) { continue }
  $ev = $runs[$r.runId]
  $np = @($ev | Where-Object { $_['type'] -in 'care_select', 'qte_press', 'fight_press' }).Count
  if ((N $r.pressesTotal) -ne $np) { Fail 'I15' $r.runId "pressesTotal csv $($r.pressesTotal) != $np" }
}

# ================= outliers & bot-ness =================
$feat = foreach ($r in $csv) { [pscustomobject]@{ run = $r.runId; profile = $r.profileKey; seed = [int]$r.seed; dur = N $r.tSimSec; presses = N $r.pressesTotal; coin = N $r.finalCoin } }
function Iqr($vals) { $s = @($vals | Sort-Object); $n = $s.Count; if ($n -lt 4) { return @(-1e18, 1e18) }; $q1 = $s[[int][math]::Floor(($n - 1) * 0.25)]; $q3 = $s[[int][math]::Floor(($n - 1) * 0.75)]; $i = $q3 - $q1; @(($q1 - 1.5 * $i), ($q3 + 1.5 * $i), $q1, $q3) }
$outl = [System.Collections.Generic.List[string]]::new()
foreach ($g in ($feat | Group-Object profile)) {
  foreach ($m in 'dur', 'presses', 'coin') {
    $b = Iqr ($g.Group | ForEach-Object { $_.$m })
    $hit = @($g.Group | Where-Object { $_.$m -lt $b[0] -or $_.$m -gt $b[1] })
    $q1 = if ($b.Count -gt 2) { [math]::Round($b[2], 1) } else { 'n/a' }; $q3 = if ($b.Count -gt 2) { [math]::Round($b[3], 1) } else { 'n/a' }
    $outl.Add("| $($g.Name) | $m | $q1 | $q3 | $($hit.Count) | $(($hit | ForEach-Object { "$($_.seed)=$([math]::Round($_.$m,1))" }) -join ', ') |")
  }
}
# bot-ness
$bot = [System.Collections.Generic.List[string]]::new()
$allQ = 0; $allP = 0; $allG = 0; $allM = 0; $gaps = [System.Collections.Generic.List[double]]::new(); $sig = @{}
foreach ($rid in $runs.Keys) {
  $ev = $runs[$rid]; $prof = ($ev | Where-Object { $_['type'] -eq 'run_summary' })['profile']
  $sg = ($ev | Where-Object { $_['type'] -in 'care_select', 'door_event', 'upgrade_buy', 'shop_buy', 'pet_revive', 'fight_start' } | ForEach-Object { "$($_['type']):$($_['care'])$($_['choice'])$($_['name'])$($_['item'])$($_['enemy'])$($_['result'])" }) -join '|'
  $h = [System.BitConverter]::ToString([System.Security.Cryptography.SHA1]::HashData([Text.Encoding]::UTF8.GetBytes($sg))).Substring(0, 8)
  if (-not $sig.ContainsKey($prof)) { $sig[$prof] = @{} }; $sig[$prof][$h] = (N $sig[$prof][$h]) + 1
  $prev = $null
  foreach ($e in $ev) { if ($e['type'] -eq 'qte_press') { if ($e['hit'] -eq 'Perfect') { $allP++ } elseif ($e['hit'] -eq 'Great') { $allG++ } else { $allM++ }; if ($null -ne $prev) { $gaps.Add((N $e['t']) - $prev) }; $prev = N $e['t'] } else { if ($e['type'] -ne 'exp') { $prev = $null } } }
}
$allQ = $allP + $allG + $allM
$bot.Add("- QTE presses total ${allQ}: Perfect $allP ($([math]::Round(100*$allP/$allQ,1))%), Great $allG, Miss $allM ($([math]::Round(100*$allM/$allQ,1))%). Humans will miss; a 0% miss rate is bot-only.")
$gs = @($gaps | Sort-Object)
$bot.Add("- Gap between consecutive QTE presses: n=$($gs.Count), min $([math]::Round($gs[0],3))s, median $([math]::Round($gs[[int]($gs.Count/2)],3))s, max $([math]::Round($gs[-1],3))s (bot reacts at needle-in-zone speed; no hesitation/idle).")
$tr = @($csv | ForEach-Object { (N $_.tSimSec) / [math]::Max(0.001, (N $_.tRealSec)) }); $bot.Add("- tSim/tReal ratio: min $([math]::Round(($tr|Measure-Object -Minimum).Minimum,0)), max $([math]::Round(($tr|Measure-Object -Maximum).Maximum,0)) (autoplay = 60 sim frames/real frame; tReal is meaningless for human pacing).")
foreach ($p in $sig.Keys | Sort-Object) { $bot.Add("- Route signature ($p): $($sig[$p].Count) distinct decision routes over $((($sig[$p].Values | Measure-Object -Sum).Sum)) runs (care/choice/upgrade/shop/fight sequence).") }
# profile pairs identical
$profs = @($feat | Select-Object -ExpandProperty profile -Unique)
foreach ($a in $profs) { foreach ($b in $profs) { if ($a -lt $b) {
  $ra = @($csv | Where-Object { $_.profileKey -eq $a } | Sort-Object { [int]$_.seed }); $rb = @($csv | Where-Object { $_.profileKey -eq $b } | Sort-Object { [int]$_.seed })
  if ($ra.Count -and $ra.Count -eq $rb.Count) { $same = 0; for ($i = 0; $i -lt $ra.Count; $i++) { if ((N $ra[$i].tSimSec) -eq (N $rb[$i].tSimSec) -and (N $ra[$i].pressesTotal) -eq (N $rb[$i].pressesTotal) -and (N $ra[$i].finalCoin) -eq (N $rb[$i].finalCoin)) { $same++ } }
    if ($same -gt 0) { $bot.Add("- **Profiles '$a' vs '$b' identical on same seed in $same/$($ra.Count) runs** (tSim, presses, finalCoin all equal) - not independent samples.") } } } } }
$bot.Add("- Seeds: each profile uses seeds 1..20 once ($($csv.Count) runs); same seed is reused across profiles, so seed is not an independent factor. Runs within profile differ only via wheel RNG.")
$mcs = $csv | Group-Object profileKey | ForEach-Object { "$($_.Name): coin sd=$([math]::Round((($_.Group | ForEach-Object { N $_.finalCoin }) | Measure-Object -Average -StandardDeviation).StandardDeviation,1)), outcome(s)=$((($_.Group|ForEach-Object{$_.outcome})|Select-Object -Unique) -join '/'), dayReached=$((($_.Group|ForEach-Object{$_.dayReached})|Select-Object -Unique) -join '/')" }
foreach ($m in $mcs) { $bot.Add("- Low variance: $m") }

# ================= write report =================
$sb = [System.Text.StringBuilder]::new()
[void]$sb.AppendLine('# Telemetry validation'); [void]$sb.AppendLine('')
[void]$sb.AppendLine("Generated by ``tools/validate.ps1`` on $(Get-Date -Format 'yyyy-MM-dd HH:mm'). Runs: $($runs.Count) jsonl, $($csv.Count) runs.csv rows."); [void]$sb.AppendLine('')
[void]$sb.AppendLine('## Invariants'); [void]$sb.AppendLine(''); [void]$sb.AppendLine('| id | result | failing runs | description |'); [void]$sb.AppendLine('|---|---|---:|---|')
foreach ($k in $invTitle.Keys) { $n = $inv[$k].Count; [void]$sb.AppendLine("| $k | $(if ($n -eq 0) { 'PASS' } else { 'FAIL' }) | $n/$($runs.Count) | $($invTitle[$k]) |") }
[void]$sb.AppendLine('')
foreach ($k in $invTitle.Keys) {
  if ($inv[$k].Count -eq 0) { continue }
  [void]$sb.AppendLine("### $k failures"); [void]$sb.AppendLine('')
  foreach ($rid in $inv[$k].Keys) { [void]$sb.AppendLine("- ``$rid``: $($inv[$k][$rid] -join '; ')") }
  [void]$sb.AppendLine('')
}
[void]$sb.AppendLine('## Outliers (IQR 1.5x, per profile)'); [void]$sb.AppendLine(''); [void]$sb.AppendLine('| profile | metric | Q1 | Q3 | #outliers | seed=value |'); [void]$sb.AppendLine('|---|---|---:|---:|---:|---|')
foreach ($l in $outl) { [void]$sb.AppendLine($l) }
[void]$sb.AppendLine(''); [void]$sb.AppendLine('## Bot-ness / representativeness flags'); [void]$sb.AppendLine('')
foreach ($l in $bot) { [void]$sb.AppendLine($l) }
[System.IO.File]::WriteAllText($outPath, $sb.ToString())
Write-Host "wrote $outPath"
foreach ($k in $invTitle.Keys) { Write-Host ("{0} {1} ({2} failing)" -f $k, $(if ($inv[$k].Count -eq 0) { 'PASS' } else { 'FAIL' }), $inv[$k].Count) }
