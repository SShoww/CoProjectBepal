param([string]$Root = (Join-Path $PSScriptRoot '..'))
$d = Join-Path $Root 'analysis/upgrades-data'
$runs=Import-Csv "$d/runs.csv"; $buys=Import-Csv "$d/buys.csv"; $pr=Import-Csv "$d/presses.csv"; $fi=Import-Csv "$d/fights.csv"
$lv=Import-Csv "$d/levelups.csv"; $shop=Import-Csv "$d/shop.csv"; $qe=Import-Csv "$d/qends.csv"; $days=Import-Csv "$d/days.csv"
function Hd($t){ "`n### $t" }
function avg($a){ if(!$a){return 'n/a'}; [math]::Round(($a|%{[double]$_}|measure -Average).Average,2) }
Hd 'A. per profile: runs buying each upgrade / avg counts / points'
foreach($p in 'perfect','sloppy','upgrade-first','caring','human'){
 $r=$runs|?{$_.prof -eq $p}
 "{0,-14} n={1} anyBuy={2} Q>0={3} E>0={4} P>0={5} avgPlv={6} avgPtsLeft={7} avgCoinOutUpg={8} over(game_over)={9}" -f $p,$r.Count,($r|?{[int]$_.upgQ+[int]$_.upgE+[int]$_.upgP -gt 0}).Count,($r|?{[int]$_.upgQ -gt 0}).Count,($r|?{[int]$_.upgE -gt 0}).Count,($r|?{[int]$_.upgP -gt 0}).Count,(avg $r.plv),(avg $r.ptsLeft),(avg $r.coinOutUpg),($r|?{$_.outcome -eq 'game_over'}).Count }
Hd 'B. buy timing (profile,name,level -> day counts)'
$buys|group prof,name,lvl|sort Name|%{ $dd=($_.Group|group day|sort Name|%{"d$($_.Name):$($_.Count)"}) -join ' '; "{0,-26} n={1} {2}  avgCoinAfter={3} avgPtsAfter={4}" -f $_.Name,$_.Count,$dd,(avg $_.Group.coinAfter),(avg $_.Group.ptsAfter) }
Hd 'C. level-ups: day of reaching each level (profile)'
$lv|group prof,newLevel|sort Name|%{ $dd=($_.Group|group day|sort Name|%{"d$($_.Name):$($_.Count)"}) -join ' '; "{0,-22} n={1} {2}" -f $_.Name,$_.Count,$dd }
Hd 'D. human: upgOrder x patient -> purchases'
$runs|?{$_.prof -eq 'human'}|group upgOrder,patient|sort Name|%{ "{0,-32} n={1} Q={2} E={3} P={4} anyBuy={5}" -f $_.Name,$_.Count,(avg $_.Group.upgQ),(avg $_.Group.upgE),(avg $_.Group.upgP),($_.Group|?{[int]$_.upgQ+[int]$_.upgE+[int]$_.upgP -gt 0}).Count }
Hd 'E. Train QTE results by qteUpgrade level at press time (per profile)'
$pr|?{$_.care -eq 'Train'}|group prof,upgQ|sort Name|%{ $g=$_.Group;$n=$g.Count; "{0,-20} n={1,5} P={2:P1} G={3:P1} M={4:P1}" -f $_.Name,$n,(($g|?{$_.hit -eq 'Perfect'}).Count/$n),(($g|?{$_.hit -eq 'Great'}).Count/$n),(($g|?{$_.hit -eq 'Miss'}).Count/$n) }
Hd 'E2. non-Train control by upgQ (human)'
$pr|?{$_.care -ne 'Train' -and $_.prof -eq 'human'}|group upgQ|sort Name|%{ $g=$_.Group;$n=$g.Count; "upgQ={0} n={1,5} P={2:P1} G={3:P1} M={4:P1}" -f $_.Name,$n,(($g|?{$_.hit -eq 'Perfect'}).Count/$n),(($g|?{$_.hit -eq 'Great'}).Count/$n),(($g|?{$_.hit -eq 'Miss'}).Count/$n) }
Hd 'E3. human Train: miss/perfect by skill tercile x upgQ>0'
$ht=$pr|?{$_.care -eq 'Train' -and $_.prof -eq 'human'}
foreach($b in @(@(0.55,0.7),@(0.7,0.85),@(0.85,1.01))){ foreach($u in 0,1){ $g=$ht|?{[double]$_.skill -ge $b[0] -and [double]$_.skill -lt $b[1] -and (([int]$_.upgQ -gt 0) -eq [bool]$u)}; $n=$g.Count; if($n){ "skill[{0}-{1}) upg={2} n={3,4} P={4:P1} M={5:P1} seeds={6}" -f $b[0],$b[1],$u,$n,(($g|?{$_.hit -eq 'Perfect'}).Count/$n),(($g|?{$_.hit -eq 'Miss'}).Count/$n),($g|select -Unique seed).Count } } }
Hd 'F. Train qte_end progressDelta per Perfect press by upgP (profile)'
$qe|?{$_.care -eq 'Train'}|group prof,upgP|sort Name|%{ $g=$_.Group; $pp=($g|%{[int]$_.perfect}|measure -Sum).Sum; $gg=($g|%{[int]$_.great}|measure -Sum).Sum; $pg=($g|%{[double]$_.prog}|measure -Sum).Sum; "{0,-22} sessions={1} perf={2} great={3} totalProg={4} prog/session={5} petLvlUps/session={6}" -f $_.Name,$g.Count,$pp,$gg,$pg,(avg $g.prog),(avg $g.petLvl) }
Hd 'G. energy: maxE / used / left per day_end by profile'
$days|group prof,day|sort Name|%{ "{0,-18} maxE={1} used={2} left={3}" -f $_.Name,(avg $_.Group.maxE),(avg $_.Group.used),(avg $_.Group.energyLeft) }
Hd 'H. human energy upgrade vs none: eUsed, final petLvlMax'
$hr=$runs|?{$_.prof -eq 'human'}
foreach($k in 'upgE','upgQ','upgP'){ foreach($v in 0..3){ $g=$hr|?{[int]$_.$k -eq $v}; if($g){ "{0}={1} n={2} eUsed={3} petLvlMax={4} plv={5} bigz={6} toBTC={7} skill={8}" -f $k,$v,$g.Count,(avg $g.eUsed),(avg $g.petLvlMax),(avg $g.plv),(avg $g.bigz),($g|?{$_.outcome -eq 'to_be_continued'}).Count,(avg $g.skill) } } }
Hd 'I. fights by profile/enemy: won, dmg, dodge'
$fi|group prof,enemy|sort Name|%{ $g=$_.Group; "{0,-26} n={1} won={2} dmgDealt={3} dmgTaken={4} tooSlow={5} missPress={6} petsLost={7}" -f $_.Name,$g.Count,($g|?{$_.won -eq 'True'}).Count,(avg $g.dmgDealt),(avg $g.dmgTaken),(avg $g.tooSlow),(avg $g.missPress),(avg $g.petsLost) }
Hd 'I2. BigZ by upgQ at fight (upgrade-first=3 vs perfect=0)'
$fi|?{$_.enemy -ne 'Toothless'}|group prof,upgQ|sort Name|%{ $g=$_.Group; "{0,-22} n={1} won={2} dmgDealt={3} dmgTaken={4} petsLost={5} atkHits={6}" -f $_.Name,$g.Count,($g|?{$_.won -eq 'True'}).Count,(avg $g.dmgDealt),(avg $g.dmgTaken),(avg $g.petsLost),(avg $g.atkHits) }
Hd 'J. shop buys (profile,item,day)'
$shop|group prof,item|sort Name|%{ $dd=($_.Group|group day|sort Name|%{"d$($_.Name):$($_.Count)"}) -join ' '; "{0,-30} n={1} {2}" -f $_.Name,$_.Count,$dd }
Hd 'K. Sea Tea: fights with tea vs without (non-Toothless, human+upgrade-first)'
$fi|?{$_.prof -in 'human','upgrade-first'}|group enemy,tea|sort Name|%{ $g=$_.Group; "{0,-22} n={1} dodgeStart={2} dodgeOk={3} tooSlow={4} missPress={5} dmgTaken={6} won={7}" -f $_.Name,$g.Count,(avg $g.dodgeStart),(avg $g.dodgeOk),(avg $g.tooSlow),(avg $g.missPress),(avg $g.dmgTaken),($g|?{$_.won -eq 'True'}).Count }
Hd 'L. coin reachable: max coin on day_end by day and profile (no-spend profiles)'
$days|?{$_.prof -in 'perfect','caring'}|group prof,day|sort Name|%{ "{0,-18} coin={1} pts={2} plv={3}" -f $_.Name,(avg $_.Group.coin),(avg $_.Group.pts),(avg $_.Group.plv) }
Hd 'M. human QTE buyers: within-run Train Perfect/Miss rate before vs after first QTE buy (paired by run)'
$ht=$pr|?{$_.care -eq 'Train' -and $_.prof -eq 'human'}
$rows=@()
foreach($g in ($ht|group seed)){ $a=$g.Group|?{[int]$_.upgQ -eq 0}; $b=$g.Group|?{[int]$_.upgQ -ge 1}; if($a.Count -ge 10 -and $b.Count -ge 10){ $rows+=[pscustomobject]@{seed=$g.Name;skill=[double]$g.Group[0].skill;nA=$a.Count;nB=$b.Count;pA=($a|?{$_.hit -eq 'Perfect'}).Count/$a.Count;pB=($b|?{$_.hit -eq 'Perfect'}).Count/$b.Count;mA=($a|?{$_.hit -eq 'Miss'}).Count/$a.Count;mB=($b|?{$_.hit -eq 'Miss'}).Count/$b.Count} } }
$rows|sort skill|ft -AutoSize|out-string -width 200
"paired runs={0}  mean Perfect before={1:P1} after={2:P1}; mean Miss before={3:P1} after={4:P1}" -f $rows.Count,(avg $rows.pA),(avg $rows.pB),(avg $rows.mA),(avg $rows.mB)
$dd=$rows|%{$_.pB-$_.pA}; "mean dPerfect={0:P1} dMiss={1:P1} improved={2}/{3}" -f (avg $dd),(avg ($rows|%{$_.mB-$_.mA})),($dd|?{$_ -gt 0}).Count,$dd.Count
Hd 'N. per run: Train sessions, Train presses, petLvlMax by profile and (human) upgP / merchant'
$ts=$qe|?{$_.care -eq 'Train'}|group prof,seed
$tm=@{}; foreach($g in $ts){ $tm[$g.Name]=$g.Count }
$runs|%{ $_|Add-Member -Force trainSess ([int]$tm["$($_.prof), $($_.seed)"]) -PassThru }|group prof,merchant|sort Name|%{ "{0,-26} n={1} trainSess={2} petLvlMax={3} plv={4}" -f $_.Name,$_.Count,(avg $_.Group.trainSess),(avg $_.Group.petLvlMax),(avg $_.Group.plv) }
$runs|?{$_.prof -eq 'human'}|%{ $_|Add-Member -Force trainSess ([int]$tm["$($_.prof), $($_.seed)"]) -PassThru }|group upgP|sort Name|%{ "human upgP={0} n={1} trainSess={2} petLvlMax={3}" -f $_.Name,$_.Count,(avg $_.Group.trainSess),(avg $_.Group.petLvlMax) }
Hd 'O. Sea Tea in BigZ (human only, fights with/without tea) + dodge success'
$fi|?{$_.enemy -ne 'Toothless' -and $_.prof -eq 'human'}|group tea|%{ $g=$_.Group; "tea={0} n={1} dodgeOk={2} tooSlow={3} missPress={4} dodgeRate={5:P1} dmgTaken={6} petsLost={7}" -f $_.Name,$g.Count,(avg $g.dodgeOk),(avg $g.tooSlow),(avg $g.missPress),(($g|%{[double]$_.dodgeOk}|measure -Sum).Sum/(($g|%{[double]$_.dodgeOk+[double]$_.tooSlow+[double]$_.missPress}|measure -Sum).Sum)),(avg $g.dmgTaken),(avg $g.petsLost) }
$fi|?{$_.enemy -ne 'Toothless'}|group prof,tea|sort Name|%{ $g=$_.Group; "{0,-26} n={1} dodgeRate={2:P1} dmgTakenPerDodge={3}" -f $_.Name,$g.Count,(($g|%{[double]$_.dodgeOk}|measure -Sum).Sum/(($g|%{[double]$_.dodgeOk+[double]$_.tooSlow+[double]$_.missPress}|measure -Sum).Sum)),[math]::Round((($g|%{[double]$_.dmgTaken}|measure -Sum).Sum/(($g|%{[double]$_.dodgeOk+[double]$_.tooSlow+[double]$_.missPress}|measure -Sum).Sum)),1) }
Hd 'P. human shop buyers vs coin when buying; human refusers who never shop'
$runs|?{$_.prof -eq 'human'}|group merchant|%{ "merchant={0} n={1} anyUpg={2} avgUpgSpend={3} finalCoin={4}" -f $_.Name,$_.Count,($_.Group|?{[int]$_.upgQ+[int]$_.upgE+[int]$_.upgP -gt 0}).Count,(avg $_.Group.coinOutUpg),(avg $_.Group.finalCoin) }
Hd 'Q. human: runs that never bought any upgrade - why (points? coin?)'
$runs|?{$_.prof -eq 'human' -and [int]$_.upgQ+[int]$_.upgE+[int]$_.upgP -eq 0}|%{ "seed={0} outcome={1} day={2} plv={3} ptsLeft={4} order={5} patient={6} merchant={7} finalCoin={8}" -f $_.seed,$_.outcome,$_.dayReached,$_.plv,$_.ptsLeft,$_.upgOrder,$_.patient,$_.merchant,$_.finalCoin }
Hd 'R. human: end-of-run unspent points / coin when ptsLeft>0'
$runs|?{$_.prof -eq 'human' -and [int]$_.ptsLeft -gt 0}|%{ "seed={0} ptsLeft={1} order={2} patient={3} bought Q{4} E{5} P{6} coin={7}" -f $_.seed,$_.ptsLeft,$_.upgOrder,$_.patient,$_.upgQ,$_.upgE,$_.upgP,$_.finalCoin }
