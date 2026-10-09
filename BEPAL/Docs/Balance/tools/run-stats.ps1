# Usage: pwsh -File run-stats.ps1  -> prints analysis tables to stdout. Source: ../data/raw/*.jsonl
$raw = Join-Path $PSScriptRoot '..\data\raw'
function Q($a,$p){ $a=@($a); if(-not $a.Count){return 0}; $s=@($a|Sort-Object); $i=($s.Count-1)*$p; $lo=[math]::Floor($i); $hi=[math]::Ceiling($i); [math]::Round($s[$lo]+($s[$hi]-$s[$lo])*($i-$lo),2) }
function Avg($a){ $a=@($a); if(-not $a.Count){return 0}; [math]::Round(($a|Measure-Object -Average).Average,2) }
function Pct($n,$d){ if($d -eq 0){'-'}else{ ('{0:N1}%' -f (100.0*$n/$d)) } }
$runs=@()
foreach($f in Get-ChildItem $raw -Filter *.jsonl){
  $ev = @(Get-Content $f.FullName | ForEach-Object { $_ | ConvertFrom-Json })
  $sum = $ev | Where-Object type -eq 'run_summary' | Select-Object -Last 1
  $runs += [pscustomobject]@{ prof=$sum.profile; seed=$sum.seed; sum=$sum; ev=$ev }
}
$profs='perfect','sloppy','upgrade-first','caring','human'
"## A. duration / outcome"
foreach($p in $profs){
  $r=@($runs|?{$_.prof -eq $p}); $d=@($r|%{$_.sum.tSimSec})
  $o=$r|group {$_.sum.outcome}|%{"$($_.Name)=$($_.Count)"}
  "$p n=$($r.Count) dur mean=$(Avg $d) med=$(Q $d .5) p10=$(Q $d .1) p90=$(Q $d .9) min=$(Q $d 0) max=$(Q $d 1) | $($o -join ' ')"
}
"## B. per-day time & activity share (mean sim s)"
foreach($p in $profs){
  $r=@($runs|?{$_.prof -eq $p})
  $dayT=@{1=@();2=@();3=@();4=@();5=@()}; $qteT=@();$fightT=@()
  foreach($x in $r){
    $prev=0; $reached=0
    foreach($d in 1..4){ $de=$x.ev|?{$_.type -eq 'day_end' -and $_.day -eq $d}|Select -First 1; if($de){$dayT[$d]+=$de.t-$prev;$prev=$de.t;$reached=$d} }
    if($reached -eq 4){ $dayT[5]+=$x.sum.tSimSec-$prev } elseif($reached -eq 0 -or $reached -lt 4){ $dayT[[math]::Min(5,$reached+1)]+=$x.sum.tSimSec-$prev }
    $q=0; $cs=$null
    foreach($e in $x.ev){ if($e.type -eq 'care_select' -and $e.result -eq 'ok'){$cs=$e.t} elseif($e.type -eq 'qte_end' -and $cs -ne $null){$q+=$e.t-$cs;$cs=$null} }
    $qteT+=$q
    $fightT+=[double](($x.ev|?{$_.type -eq 'fight_end'}|Measure-Object durationSim -Sum).Sum)
  }
  "$p days: " + ((1..5|%{"d$_=$(Avg $dayT[$_]) (n=$($dayT[$_].Count))"}) -join ' ') + " | qte(incl care select)=$(Avg $qteT) fight=$(Avg $fightT) total=$(Avg @($r|%{$_.sum.tSimSec}))"
}
"## C. presses per activity (all runs of profile)"
foreach($p in $profs){
  $r=@($runs|?{$_.prof -eq $p}); $ev=@($r|%{$_.ev})
  $cs=@($ev|?{$_.type -eq 'care_select'})
  $csok=@($cs|?{$_.result -eq 'ok'})
  "$p careSel: n=$($cs.Count) ok=$($csok.Count) gap=$(@($cs|?{$_.result -eq 'gap'}).Count) noenergy=$(@($cs|?{$_.result -eq 'noenergy'}).Count) okhit P/G/M=$(@($csok|?{$_.hit -eq 'Perfect'}).Count)/$(@($csok|?{$_.hit -eq 'Great'}).Count)/$(@($csok|?{$_.hit -eq 'Miss'}).Count) careChosen=" + (($csok|group care|%{"$($_.Name)=$($_.Count)"}) -join ',')
  $qp=@($ev|?{$_.type -eq 'qte_press'})
  foreach($gr in ($qp|group care)){ $n=$gr.Count;$cP=@($gr.Group|?{$_.hit -eq "Perfect"}).Count;$cG=@($gr.Group|?{$_.hit -eq "Great"}).Count;$cM=@($gr.Group|?{$_.hit -eq "Miss"}).Count
    "   qte $($gr.Name): n=$n P=$(Pct $cP $n) G=$(Pct $cG $n) M=$(Pct $cM $n)" }
  $fp=@($ev|?{$_.type -eq 'fight_press'})
  foreach($k in "attack","dodge"){ $fg=@($fp|?{$_.kind -eq $k}); $n=$fg.Count;$cP=@($fg|?{$_.hit -eq "Perfect"}).Count;$cG=@($fg|?{$_.hit -eq "Great"}).Count;$cM=@($fg|?{$_.hit -eq "Miss"}).Count
    "   fight $k : n=$n P=$(Pct $cP $n) G=$(Pct $cG $n) M=$(Pct $cM $n)" }
  $fh=@($ev|?{$_.type -eq 'fight_hurt'})
  "   hurt causes: " + (($fh|group cause|%{"$($_.Name)=$($_.Count)"}) -join ',') + " | fights=$(@($ev|?{$_.type -eq 'fight_end'}).Count) runs=$($r.Count) presses/run=$(Avg @($r|%{ @($_.ev|?{$_.type -in 'qte_press','fight_press','care_select'}).Count }))"
}
"## D. fights by enemy"
foreach($p in $profs){
  $r=@($runs|?{$_.prof -eq $p})
  foreach($en in 'Toothless','Big Z'){
    $fe=@($r|%{$_.ev}|?{$_.type -eq 'fight_end' -and $_.enemy -eq $en})
    if(-not $fe.Count){continue}
    $won=@($fe|?{$_.won}).Count
    "$p $en n=$($fe.Count) won=$won rounds mean=$(Avg @($fe|%{$_.rounds})) med=$(Q @($fe|%{$_.rounds}) .5) dealt mean=$(Avg @($fe|%{$_.dmgDealt})) med=$(Q @($fe|%{$_.dmgDealt}) .5) taken mean=$(Avg @($fe|%{$_.dmgTaken})) atkMiss=$(Avg @($fe|%{$_.attackMiss})) dodgeOk=$(Avg @($fe|%{$_.dodgeOk})) tooSlow=$(Avg @($fe|%{$_.tooSlow})) missPress=$(Avg @($fe|%{$_.missPress})) dur=$(Avg @($fe|%{$_.durationSim})) petsLost=$(Avg @($fe|%{$_.petsLost}))"
  }
  $fs=@($r|%{$_.ev}|?{$_.type -eq 'fight_start'})
  foreach($en in 'Toothless','Big Z'){ $s=@($fs|?{$_.enemy -eq $en}); if($s.Count){ "   start $en petHp mean=$(Avg @($s|%{$_.petHp})) min=$(Q @($s|%{$_.petHp}) 0) petAtk mean=$(Avg @($s|%{$_.petAtk})) seaTea=$(@($s|?{$_.seaTea}).Count) dodgeStart mean=$(Avg @($s|%{$_.dodgeStart}))" } }
}
"## E. game overs"
foreach($x in ($runs|?{$_.sum.outcome -eq 'game_over'})){
  $fe=$x.ev|?{$_.type -eq 'fight_end'}|Select -Last 1
  $bp=$x.ev|?{$_.type -eq 'bot_params'}
  $ps=@($x.ev|?{$_.type -eq 'pet_added'}).Count
  "$($x.prof) seed=$($x.seed) day=$($x.sum.dayReached) scene=$($x.sum.deathScene) petsAdded=$($ps) merchant=$($x.sum.merchantChoice) playerLv=$($x.sum.playerLevel) fight($($fe.enemy)): rounds=$($fe.rounds) dealt=$($fe.dmgDealt) taken=$($fe.dmgTaken) atkMiss=$($fe.attackMiss) missPress=$($fe.missPress) tooSlow=$($fe.tooSlow) | skill=$([math]::Round($bp.skill,2)) appetite=$([math]::Round($bp.appetite,2)) tame=$([math]::Round($bp.tameChance,2))"
}
"## F. human: params vs outcome"
$rows=foreach($x in ($runs|?{$_.prof -eq 'human'})){
  $bp=$x.ev|?{$_.type -eq 'bot_params'}
  $t=$x.ev|?{$_.type -eq 'fight_end' -and $_.enemy -eq 'Toothless'}|Select -First 1
  $qp=@($x.ev|?{$_.type -eq 'qte_press'})
  $dc=$x.ev|?{$_.type -eq 'door_event' -and $_.event -eq 'toothless'}|Select -First 1
  [pscustomobject]@{seed=$x.seed;skill=[math]::Round($bp.skill,2);appet=[math]::Round($bp.appetite,2);tame=[math]::Round($bp.tameChance,2);jit=$bp.jitter;door2=$dc.choice;out=$x.sum.outcome;day=$x.sum.dayReached;dur=$x.sum.tSimSec;tFought=[bool]$t;tWon=$t.won;tTaken=$t.dmgTaken;tDealt=$t.dmgDealt;pets=$x.sum.petsFinal;lvl=$x.sum.playerLevel;qMiss=$(if($qp.Count){[math]::Round(@($qp|?{$_.hit -eq 'Miss'}).Count/$qp.Count,2)}else{0});qn=$qp.Count;merch=$x.sum.merchantChoice;bigz=$x.sum.bigzDmgDealt;en=$x.sum.energyUsedTotal}
}
$rows | Sort-Object skill | Format-Table -AutoSize | Out-String -Width 250
"skill terciles:"
$s=@($rows|Sort-Object skill); $n=$s.Count
foreach($i in 0..2){ $g=@($s[([int]($i*$n/3))..([int](($i+1)*$n/3-1))])
 "tercile$i skill $($g[0].skill)-$($g[-1].skill) n=$($g.Count) gameover=$(@($g|?{$_.out -eq 'game_over'}).Count) toothFought=$(@($g|?{$_.tFought}).Count) toothWon=$(@($g|?{$_.tWon}).Count) meanQteMiss=$(Avg @($g|%{$_.qMiss})) meanDur=$(Avg @($g|%{$_.dur})) meanBigzDmg=$(Avg @($g|%{$_.bigz})) meanLvl=$(Avg @($g|%{$_.lvl})) meanEnergyUsed=$(Avg @($g|%{$_.en}))" }
"## G. door choices by profile"
foreach($p in $profs){ $r=@($runs|?{$_.prof -eq $p}); $de=@($r|%{$_.ev}|?{$_.type -eq 'door_event'}); "$p " + (($de|group {"$($_.event):$($_.choice)"}|sort Name|%{"$($_.Name)=$($_.Count)"}) -join ' ') }
"## H. BigZ dmg (boss hp ~10000)"
foreach($p in $profs){ $r=@($runs|?{$_.prof -eq $p}); $b=@($r|%{$_.sum.bigzDmgDealt}); "$p bigzDmg mean=$(Avg $b) med=$(Q $b .5) min=$(Q $b 0) max=$(Q $b 1)" }
