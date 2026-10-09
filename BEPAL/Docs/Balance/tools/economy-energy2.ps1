# Extra economy/energy queries (boss/toothless readiness, heal value, human appetite split). Usage: pwsh tools/economy-energy2.ps1 > analysis/economy-energy-data2.txt
$root = Split-Path $PSScriptRoot
$ALL = @()
foreach ($f in (Get-ChildItem "$root/data/raw/*.jsonl")) {
  $ev = @(Get-Content $f.FullName | ForEach-Object { $_ | ConvertFrom-Json })
  $sum = $ev | Where-Object type -eq 'run_summary' | Select-Object -First 1
  $ALL += [pscustomobject]@{ p = $sum.profile; s = $sum; ev = $ev }
}
function Avg($a,$prop){ ($a | Measure-Object $prop -Average).Average }
function Hd($t){ "`n=== $t ===" }
function Of($p){ $ALL | Where-Object{$_.p -eq $p} }
function Ev($r,$type){ $r.ev | Where-Object{$_.type -eq $type} }
$profiles = 'human','perfect','sloppy','upgrade-first','caring'

Hd 'H1. fight_start snapshots (enemy, pet hp/maxHp/atk)'
foreach($p in $profiles){ foreach($en in 'Toothless','Big Z'){ $x=@(Of $p | ForEach-Object{ Ev $_ 'fight_start' | Where-Object{$_.enemy -eq $en} }); if($x.Count){
  "{0,-14} {1,-9} n={2} petHp {3:N0}/{4:N0} ({5:N0}% of max) atk {6:N1} seaTea {7}" -f $p,$en,$x.Count,(Avg $x 'petHp'),(Avg $x 'petMaxHp'),(100*(Avg $x 'petHp')/(Avg $x 'petMaxHp')),(Avg $x 'petAtk'),@($x|Where-Object{$_.seaTea}).Count } } }
Hd 'H2. Toothless fight_end: dmgTaken, petsLost, won'
foreach($p in $profiles){ $x=@(Of $p | ForEach-Object{ Ev $_ 'fight_end' | Where-Object{$_.enemy -eq 'Toothless'} }); if($x.Count){ "{0,-14} n={1} dmgTaken {2:N1} won {3} petsLost {4}" -f $p,$x.Count,(Avg $x 'dmgTaken'),@($x|Where-Object{$_.won}).Count,@($x|Where-Object{$_.petsLost -gt 0}).Count } }
Hd 'H3. Boss damage by profile'
foreach($p in $profiles){ $x=@(Of $p | Where-Object{$_.s.dayReached -eq 5}); "{0,-14} n={1} bigzDmg {2:N0}" -f $p,$x.Count,(($x|ForEach-Object{[double]$_.s.bigzDmgDealt}|Measure-Object -Average).Average) }
Hd 'H4. Heal value: hpDelta of Heal QTEs'
foreach($p in $profiles){ $x=@(Of $p | ForEach-Object{ Ev $_ 'qte_end' | Where-Object{$_.care -eq 'Heal'} }); if($x.Count){ "{0,-14} n={1} hpDelta avg {2:N1}; =0: {3}; <=10: {4}; >=40: {5}" -f $p,$x.Count,(Avg $x 'hpDelta'),@($x|Where-Object{$_.hpDelta -le 0}).Count,@($x|Where-Object{$_.hpDelta -le 10}).Count,@($x|Where-Object{$_.hpDelta -ge 40}).Count } }
Hd 'H5. Human: energy by appetite (bot_params.appetite) days 1-4'
$hum = Of 'human'
foreach($grp in @(@('appetite>=0.5',{param($a) $a -ge 0.5}),@('appetite<0.5',{param($a) $a -lt 0.5}))){
  $rows=@(); $n=0; foreach($r in $hum){ $bp=Ev $r 'bot_params'|Select-Object -First 1; if(& $grp[1] $bp.appetite){ $n++; foreach($d in (Ev $r 'day_end')){ $rows += [pscustomobject]@{used=$d.energyUsedToday;left=$d.energyLeft;max=$d.maxEnergy} } } }
  "{0,-14} runs {1} days {2}: used {3:N2} left {4:N2} max {5:N2}; days left>0: {6}%" -f $grp[0],$n,$rows.Count,(Avg $rows 'used'),(Avg $rows 'left'),(Avg $rows 'max'),[math]::Round(100*@($rows|Where-Object{$_.left -gt 0}).Count/$rows.Count) }
Hd 'H6. Human outcomes: energy total used vs player level (all human runs reaching d5)'
$h5 = $hum | Where-Object{$_.s.dayReached -eq 5}
foreach($g in ($h5 | Group-Object {$_.s.playerLevel} | Sort-Object Name)){ "playerLevel {0}: runs {1}, energyUsedTotal avg {2:N1}" -f $g.Name,$g.Count,(($g.Group|ForEach-Object{[double]$_.s.energyUsedTotal}|Measure-Object -Average).Average) }
Hd 'H7. Exp thresholds: cumulative exp to reach level L = sum 10*(k) for k=1..L-1'
foreach($L in 2..7){ $c=0; foreach($k in 1..($L-1)){ $c += 10*$k }; "level {0}: cumulative exp {1} (= {2:N1} energy at 10 exp/E)" -f $L,$c,($c/10) }
Hd 'H8. Merchant sell moment: which pet sold, pets before'
foreach($p in 'perfect','caring','human'){ $x=@(Of $p | ForEach-Object{ Ev $_ 'pet_removed' }); "{0,-8} pet_removed: {1}" -f $p,(($x|Group-Object pet,via|ForEach-Object{"$($_.Name)=$($_.Count)"}) -join '; ') }
Hd 'H9. Human refuse runs: coin at d5 (idle) and purchases bought vs not'
$hr = $hum | Where-Object{$_.s.merchantChoice -eq 'refuse'}
"refuse n=$($hr.Count): final coin avg {0:N0}; runs with final coin >=250 (could revive): {1}; shop buys/run {2:N2}; upgrade coin/run {3:N0}" -f (($hr|ForEach-Object{[double]$_.s.finalCoin}|Measure-Object -Average).Average),@($hr|Where-Object{$_.s.finalCoin -ge 250}).Count,((($hr|ForEach-Object{@(Ev $_ 'shop_buy').Count})|Measure-Object -Average).Average),(($hr|ForEach-Object{[double]$_.s.coinOut_upgrade}|Measure-Object -Average).Average)
Hd 'H10. Human: tonic runs vs exp'
foreach($t in 0,1){ $g=@($hr | Where-Object{ (@(Ev $_ 'energy' | Where-Object{$_.reason -eq 'tonic'}).Count) -eq $t }); "tonic={0}: runs {1}, playerLevel {2:N2}, exp-ish energyUsed {3:N1}" -f $t,$g.Count,(($g|ForEach-Object{[double]$_.s.playerLevel}|Measure-Object -Average).Average),(($g|ForEach-Object{[double]$_.s.energyUsedTotal}|Measure-Object -Average).Average) }
Hd 'H11. Energy upgrade timing (human): bought day, energy used on days after vs maxEnergy'
foreach($r in ($hum | Where-Object{ @(Ev $_ 'upgrade_buy' | Where-Object{$_.name -eq 'Energy'}).Count -gt 0 })){ $b=Ev $r 'upgrade_buy' | Where-Object{$_.name -eq 'Energy'}|Select-Object -First 1; $de=(Ev $r 'day_end' | ForEach-Object{ "d$($_.day):$($_.energyUsedToday)/$($_.maxEnergy)" }) -join ' '; "seed {0}: Energy bought day {1} (coin {2}); {3}; appetite {4:N2}" -f $r.s.seed,$b.day,$b.coinCost,$de,(Ev $r 'bot_params'|Select-Object -First 1).appetite }
