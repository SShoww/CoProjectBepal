# Upgrade analysis: parses data/raw/*.jsonl, writes analysis/upgrades-data/*.csv and prints tables.
param([string]$Root = (Join-Path $PSScriptRoot '..'))
$raw = Join-Path $Root 'data/raw'
$out = Join-Path $Root 'analysis/upgrades-data'; New-Item -ItemType Directory -Force $out | Out-Null
$runs=@(); $buys=@(); $presses=@(); $fights=@(); $lvl=@(); $shop=@(); $qends=@(); $days=@()
foreach($f in Get-ChildItem $raw -Filter *.jsonl){
  $ev = Get-Content $f.FullName | ForEach-Object { $_ | ConvertFrom-Json }
  $st = $ev[0]; $prof=$st.profile; $seed=$st.seed
  $bp = $ev | ? type -eq 'bot_params' | select -First 1
  $sum = $ev | ? type -eq 'run_summary' | select -Last 1
  $q=0;$e=0;$p=0
  foreach($x in $ev){
    switch($x.type){
      'upgrade_buy' { $buys += [pscustomobject]@{prof=$prof;seed=$seed;day=$x.day;t=$x.t;name=$x.name;lvl=$x.newLevel;coinCost=$x.coinCost;pts=$x.pointCost;coinAfter=$x.coinAfter;ptsAfter=$x.pointsAfter}
        switch($x.name){'QTE'{$q=$x.newLevel}'Energy'{$e=$x.newLevel}'Progress'{$p=$x.newLevel}} }
      'qte_press' { $presses += [pscustomobject]@{prof=$prof;seed=$seed;day=$x.day;care=$x.care;hit=$x.hit;upgQ=$q;skill=$bp.skill} }
      'qte_end' { $qends += [pscustomobject]@{prof=$prof;seed=$seed;day=$x.day;care=$x.care;perfect=$x.perfect;great=$x.great;miss=$x.miss;prog=$x.progressDelta;petLvl=$x.petLevelUps;upgP=$p;upgQ=$q} }
      'exp' { if($x.levelUp){ $lvl += [pscustomobject]@{prof=$prof;seed=$seed;day=$x.day;t=$x.t;newLevel=$x.level+0;points=$x.points} } }
      'shop_buy' { $shop += [pscustomobject]@{prof=$prof;seed=$seed;day=$x.day;item=$x.item;price=$x.price} }
      'fight_start' { $fs=$x }
      'fight_end' { $fights += [pscustomobject]@{prof=$prof;seed=$seed;day=$x.day;enemy=$x.enemy;won=$x.won;dmgDealt=$x.dmgDealt;dmgTaken=$x.dmgTaken;dodgeOk=$x.dodgeOk;tooSlow=$x.tooSlow;missPress=$x.missPress;atkHits=$x.attackHits;petsLost=$x.petsLost;tea=$fs.seaTea;dodgeStart=$fs.dodgeStart;upgQ=$q;upgE=$e;upgP=$p;petAtk=$fs.petAtk} }
      'day_end' { $days += [pscustomobject]@{prof=$prof;seed=$seed;day=$x.day;coin=$x.coin;energyLeft=$x.energyLeft;used=$x.energyUsedToday;maxE=$x.maxEnergy;pts=$x.points;plv=$x.playerLevel} }
    }
  }
  $runs += [pscustomobject]@{prof=$prof;seed=$seed;skill=$bp.skill;upgOrder=$bp.upgOrder;patient=$bp.patient;appetite=$bp.appetite;outcome=$sum.outcome;dayReached=$sum.dayReached;upgQ=$sum.upgQte;upgE=$sum.upgEnergy;upgP=$sum.upgProgress;ptsLeft=$sum.pointsLeft;plv=$sum.playerLevel;coinOutUpg=$sum.coinOut_upgrade;merchant=$sum.merchantChoice;eUsed=$sum.energyUsedTotal;bigz=$sum.bigzDmgDealt;petLvlMax=$sum.petLvlMax;finalCoin=$sum.finalCoin}
}
$runs|Export-Csv "$out/runs.csv" -NoTypeInformation; $buys|Export-Csv "$out/buys.csv" -NoTypeInformation
$presses|Export-Csv "$out/presses.csv" -NoTypeInformation; $fights|Export-Csv "$out/fights.csv" -NoTypeInformation
$lvl|Export-Csv "$out/levelups.csv" -NoTypeInformation; $shop|Export-Csv "$out/shop.csv" -NoTypeInformation
$qends|Export-Csv "$out/qends.csv" -NoTypeInformation; $days|Export-Csv "$out/days.csv" -NoTypeInformation
"done: runs=$($runs.Count) buys=$($buys.Count) presses=$($presses.Count) fights=$($fights.Count)"
