# Economy + energy analysis. Usage: pwsh tools/economy-energy.ps1 > analysis/economy-energy-data.txt
$root = Split-Path $PSScriptRoot
$files = Get-ChildItem "$root/data/raw/*.jsonl"
$ALL = @()
foreach ($f in $files) {
  $ev = @(Get-Content $f.FullName | ForEach-Object { $_ | ConvertFrom-Json })
  $sum = $ev | Where-Object type -eq 'run_summary' | Select-Object -First 1
  $ALL += [pscustomobject]@{ p = $sum.profile; s = $sum; ev = $ev }
}
function M($a){ $a=@($a|Where-Object{$_ -ne $null}); if($a.Count -eq 0){return 'n/a'}; $m=($a|Measure-Object -Average).Average; $so=@($a|Sort-Object); $med=$so[[int][math]::Floor(($so.Count-1)/2)]; '{0:N1} (med {1}, min {2}, max {3}, n={4})' -f $m,$med,$so[0],$so[-1],$a.Count }
function Avg($a,$prop){ if(-not $prop){ $x=@($a|Measure-Object -Average).Average } else { $x=($a|Measure-Object $prop -Average).Average }; $x }
function Hd($t){ "`n=== $t ===" }
function Of($p){ $ALL | Where-Object{$_.p -eq $p} }
function Ev($r,$type){ $r.ev | Where-Object{$_.type -eq $type} }
$profiles = 'human','perfect','sloppy','upgrade-first','caring'

Hd 'A. Coin sources/sinks per run (mean)'
foreach($p in $profiles){ $g=@(Of $p)
  $f = { param($k) '{0:N1}' -f (($g|ForEach-Object{[double]$_.s.$k}|Measure-Object -Average).Average) }
  "{0,-14} n={1} IN daily {2} toothless {3} merchant {4} | OUT upgrade {5} apple {6} tea {7} tonic {8} revive {9} | final {10}" -f $p,$g.Count,(& $f 'coinIn_daily'),(& $f 'coinIn_toothless_reward'),(& $f 'coinIn_merchant_sale'),(& $f 'coinOut_upgrade'),(& $f 'coinOut_shop_apple'),(& $f 'coinOut_shop_tea'),(& $f 'coinOut_shop_tonic'),(& $f 'coinOut_revive'),(& $f 'finalCoin') }

Hd 'B. Coin at day_end (before the nightly +100) per profile/day'
foreach($p in $profiles){ foreach($d in 1..4){
  $v = Of $p | ForEach-Object{ (Ev $_ 'day_end' | Where-Object{$_.day -eq $d} | Select-Object -First 1).coin }
  "{0,-14} d{1} coin: {2}" -f $p,$d,(M $v) } }
foreach($p in $profiles){ "{0,-14} finalCoin: {1}" -f $p,(M (Of $p|ForEach-Object{$_.s.finalCoin})) }
foreach($mc in 'sell','refuse'){ $g = @(Of 'human' | Where-Object{$_.s.merchantChoice -eq $mc}); "human merchant=$mc n=$($g.Count) finalCoin: $(M ($g|ForEach-Object{$_.s.finalCoin}))  spent: $(M ($g|ForEach-Object{$_.s.coinSpentTotal}))" }
"human merchantChoice values: " + ((Of 'human'|Group-Object {$_.s.merchantChoice}|ForEach-Object{"$($_.Name)=$($_.Count)"}) -join ', ')
foreach($p in $profiles){ "{0,-14} merchantChoice: {1}" -f $p,((Of $p|Group-Object {$_.s.merchantChoice}|ForEach-Object{"$($_.Name)=$($_.Count)"}) -join ', ') }

Hd 'C. Upgrades: day_end snapshots (not maxed): affordable-now / coin-limited / point-limited'
$up = [ordered]@{ QTE=@(100,1,3,'upgQte'); Energy=@(150,3,3,'upgEnergy'); Progress=@(20,2,2,'upgProgress') }
foreach($p in $profiles){ foreach($k in $up.Keys){ $c=$up[$k][0];$pt=$up[$k][1];$mx=$up[$k][2];$fld=$up[$k][3]
  $aff=0;$coinLim=0;$ptLim=0;$tot=0;$both=0
  foreach($r in (Of $p)){ foreach($d in (Ev $r 'day_end')){ if($d.$fld -lt $mx){ $tot++; if($d.coin -ge $c -and $d.points -ge $pt){$aff++}elseif($d.points -ge $pt){$coinLim++}elseif($d.coin -ge $c){$ptLim++}else{$both++} } } }
  "{0,-14} {1,-8} snapshots={2}: affordable-now {3}, coin-limited {4}, point-limited {5}, neither {6}" -f $p,$k,$tot,$aff,$coinLim,$ptLim,$both } }
foreach($p in $profiles){ "{0,-14} upgrades bought {1} | pointsLeft {2} | playerLevel {3} | pointsSpent {4}" -f $p,(M (Of $p|ForEach-Object{$_.s.upgQte+$_.s.upgEnergy+$_.s.upgProgress})),(M (Of $p|ForEach-Object{$_.s.pointsLeft})),(M (Of $p|ForEach-Object{$_.s.playerLevel})),(M (Of $p|ForEach-Object{$_.s.pointsSpent})) }
"Everything-bought total: QTE 3x100 + Energy 3x150 + Progress 2x20 = 790 coin; points 3x1 + 3x3 + 2x2 = 16"
Hd 'C2. upgrade_buy by name/day'
foreach($p in $profiles){ $x = Of $p | ForEach-Object{ Ev $_ 'upgrade_buy' }; "{0,-14} {1}" -f $p,((@($x)|Group-Object name,day|Sort-Object Name|ForEach-Object{"$($_.Name)=$($_.Count)"}) -join '; ') }
Hd 'C3. shop_buy by item/day'
foreach($p in $profiles){ $x = Of $p | ForEach-Object{ Ev $_ 'shop_buy' }; "{0,-14} {1}" -f $p,((@($x)|Group-Object item,day|ForEach-Object{"$($_.Name)=$($_.Count)"}) -join '; ') }

Hd 'D. Merchant Sell vs Refuse outcomes'
foreach($mc in 'sell','refuse'){ foreach($p in $profiles){ $g = @(Of $p | Where-Object{$_.s.merchantChoice -eq $mc}); if($g.Count){
  "{0,-6} {1,-14} n={2} finalCoin {3} | spent {4} | bigzDmg {5} | playerLv {6} | petsFinal {7} | outcome {8}" -f $mc,$p,$g.Count,(M ($g|ForEach-Object{$_.s.finalCoin})),(M ($g|ForEach-Object{$_.s.coinSpentTotal})),(M ($g|ForEach-Object{$_.s.bigzDmgDealt})),(M ($g|ForEach-Object{$_.s.playerLevel})),(M ($g|ForEach-Object{$_.s.petsFinal})),((@($g)|Group-Object {$_.s.outcome}|ForEach-Object{"$($_.Name)=$($_.Count)"}) -join ',') } } }
Hd 'D3. Coin right before merchant (refuse runs)'
foreach($p in 'human','upgrade-first'){ $v = Of $p | ForEach-Object{ $e=$_.ev; $m=$e|Where-Object{$_.type -eq 'door_event' -and $_.event -eq 'merchant'}|Select-Object -First 1; if($m -and $m.choice -eq 'refuse'){ ($e|Where-Object{$_.type -eq 'coin' -and $_.seq -lt $m.seq}|Select-Object -Last 1).coinAfter } }; "{0,-14} coin before merchant (refuse runs): {1}" -f $p,(M $v) }

Hd 'E. Energy per day: used / left at day_end (day 5 from run_summary)'
foreach($p in $profiles){ foreach($d in 1..5){ $u=@();$l=@();$mx=@(); foreach($r in (Of $p)){ $de=Ev $r 'day_end' | Where-Object{$_.day -eq $d}|Select-Object -First 1; if($de){$u+=$de.energyUsedToday;$l+=$de.energyLeft;$mx+=$de.maxEnergy} elseif($r.s.dayReached -ge $d){ $u+=[int]$r.s."energyUsed_d$d"; $l+=[int]$r.s."energyLeft_d$d" } }
  "{0,-14} d{1}: used {2} | left {3} | max {4} | runs left>0: {5}/{6}" -f $p,$d,(M $u),(M $l),(M $mx),(@($l|Where-Object{$_ -gt 0}).Count),$l.Count } }
Hd 'E2. Tonic effect and maxEnergy effect (per day_end, days 1-4)'
foreach($p in $profiles){ $rows=@(); foreach($r in (Of $p)){ foreach($d in (Ev $r 'day_end')){ $t=@(Ev $r 'energy' | Where-Object{$_.reason -eq 'tonic' -and $_.day -eq $d.day}).Count; $rows += [pscustomobject]@{tonic=$t;used=$d.energyUsedToday;max=$d.maxEnergy;left=$d.energyLeft} } }
  foreach($g in ($rows|Group-Object tonic)){ "{0,-14} tonics={1}: days {2} avg used {3:N2} avg max {4:N2} avg left {5:N2}" -f $p,$g.Name,$g.Count,(Avg $g.Group 'used'),(Avg $g.Group 'max'),(Avg $g.Group 'left') }
  foreach($g in ($rows|Group-Object max|Sort-Object Name)){ "{0,-14} maxEnergy={1}: days {2} avg used {3:N2} avg left {4:N2}" -f $p,$g.Name,$g.Count,(Avg $g.Group 'used'),(Avg $g.Group 'left') } }

Hd 'F. Return per Energy by care type (per qte_end)'
foreach($p in $profiles){ foreach($c in 'Train','Feed','Clean','Heal'){ $q = @(Of $p | ForEach-Object{ Ev $_ 'qte_end' | Where-Object{$_.care -eq $c} }); if($q.Count){
  $e = $q | ForEach-Object{ [pscustomobject]@{ exp=(10-$_.miss); pr=$_.progressDelta; st=$_.stomachDelta; cl=$_.cleanDelta; hp=$_.hpDelta; perf=$_.perfect } }
  "{0,-14} {1,-5} n={2} exp/E {3:N2} | progress {4:N1} | stomach {5:N1} | clean {6:N1} | hp {7:N1} | perfect {8:N1}" -f $p,$c,$q.Count,(Avg $e 'exp'),(Avg $e 'pr'),(Avg $e 'st'),(Avg $e 'cl'),(Avg $e 'hp'),(Avg $e 'perf') } } }
Hd 'F2. Care mix (care_select ok)'
foreach($p in $profiles){ $q = @(Of $p | ForEach-Object{ Ev $_ 'care_select' | Where-Object{$_.result -eq 'ok'} }); "{0,-14} total {1}: {2}" -f $p,$q.Count,(($q|Group-Object care|ForEach-Object{"$($_.Name)=$($_.Count) ($([math]::Round(100*$_.Count/$q.Count))%)"}) -join ', ') }
Hd 'F3. playerLevel / points at day_end'
foreach($p in $profiles){ foreach($d in 1..4){ "{0,-14} d{1} playerLevel {2} | points {3}" -f $p,$d,(M (Of $p|ForEach-Object{(Ev $_ 'day_end'|Where-Object{$_.day -eq $d}|Select-Object -First 1).playerLevel})),(M (Of $p|ForEach-Object{(Ev $_ 'day_end'|Where-Object{$_.day -eq $d}|Select-Object -First 1).points})) } }

Hd 'G. Upkeep: living pet snapshots at day_end (NightScene)'
foreach($p in $profiles){ $dayEnd = @(Of $p | ForEach-Object{ Ev $_ 'pet_snapshot' | Where-Object{ -not $_.dead -and $_.scene -eq 'NightScene'} })
  if($dayEnd.Count){ "{0,-14} n={1}: stomach avg {2:N0} (=0: {3}%), clean avg {4:N0} (<=50: {5}%, <=25: {6}%)" -f $p,$dayEnd.Count,(Avg $dayEnd 'stomach'),[math]::Round(100*@($dayEnd|Where-Object{$_.stomach -le 0}).Count/$dayEnd.Count),(Avg $dayEnd 'clean'),[math]::Round(100*@($dayEnd|Where-Object{$_.clean -le 50}).Count/$dayEnd.Count),[math]::Round(100*@($dayEnd|Where-Object{$_.clean -le 25}).Count/$dayEnd.Count) } }
"Pet snapshots at fight_start:"
foreach($p in $profiles){ $x=@(); foreach($r in (Of $p)){ foreach($f in (Ev $r 'fight_start')){ $x += @($r.ev|Where-Object{$_.type -eq 'pet_snapshot' -and $_.t -eq $f.t -and -not $_.dead}) } }
  if($x.Count){ "{0,-14} n={1} stomach {2:N0} clean {3:N0} clean<=50 {4}% clean<=25 {5}%" -f $p,$x.Count,(Avg $x 'stomach'),(Avg $x 'clean'),[math]::Round(100*@($x|Where-Object{$_.clean -le 50}).Count/$x.Count),[math]::Round(100*@($x|Where-Object{$_.clean -le 25}).Count/$x.Count) } }
Hd 'G2. starve events'
foreach($p in $profiles){ $x = @(Of $p | ForEach-Object{ Ev $_ 'starve' }); "{0,-14} starve events {1} over {2} runs (runs with >=1: {3}); by day: {4}; died: {5}" -f $p,$x.Count,@(Of $p).Count,@(Of $p|Where-Object{(Ev $_ 'starve')}).Count,(($x|Group-Object day|Sort-Object Name|ForEach-Object{"d$($_.Name)=$($_.Count)"}) -join ' '),@($x|Where-Object{$_.died}).Count }
Hd 'G3. Deaths by cause/day, revives, game over'
foreach($p in $profiles){ $x = @(Of $p | ForEach-Object{ Ev $_ 'pet_death' }); "{0,-14} pet_death: {1}; revives {2}; game_over {3}/{4}" -f $p,(($x|Group-Object cause,day|Sort-Object Name|ForEach-Object{"$($_.Name)=$($_.Count)"}) -join '; '),((Of $p|ForEach-Object{[int]$_.s.revives}|Measure-Object -Sum).Sum),@(Of $p|Where-Object{$_.s.outcome -eq 'game_over'}).Count,@(Of $p).Count }
"game_over detail:"
foreach($r in ($ALL|Where-Object{$_.s.outcome -eq 'game_over'})){ $d=Ev $r 'pet_death'|Select-Object -First 1; $c=(Ev $r 'coin'|Where-Object{$_.seq -lt $d.seq}|Select-Object -Last 1).coinAfter; if($c -eq $null){$c=150}; "{0} seed {1} cause {2} day {3} scene {4} coinAtDeath {5} (revive 250)" -f $r.p,$r.s.seed,$d.cause,$d.day,$d.scene,$c }
Hd 'G4. Pet deaths before day 5 (any)'
foreach($p in $profiles){ $x = @(Of $p | ForEach-Object{ Ev $_ 'pet_death' | Where-Object{$_.day -lt 5} }); "{0,-14} {1}" -f $p,$x.Count }
Hd 'G5. Toothless'
foreach($p in $profiles){ $g=@(Of $p|Where-Object{$_.s.toothlessWon -ne $null}); "{0,-14} toothlessWon {1}/{2}" -f $p,@($g|Where-Object{$_.s.toothlessWon -eq 1}).Count,$g.Count }
