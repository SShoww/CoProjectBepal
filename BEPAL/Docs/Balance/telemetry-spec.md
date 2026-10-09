# Telemetry Spec (Q-20261009-run-telemetry)

ฟอร์แมต: JSONL (1 event/บรรทัด) flat fields เท่านั้น แปลงเป็น CSV ได้ตรง ๆ. ไฟล์: `telemetry/<runId>.events.jsonl` + `telemetry/runs.csv` (1 แถว/run).

## 1. ID scheme
| field | type | note |
|---|---|---|
| runId | string | `yyyyMMddTHHmmss-<profile>-<rand4>` |
| profile | string | `human:<name>` / `bot:default` / `bot:refuse` / `bot:<custom>` |
| seed | int | ค่า seed ของ `Random` (ถ้าโค้ดยังไม่มี seed -> -1; ต้องเพิ่ม seed ให้ Wheel/SpawnZone ถ้าอยาก replay) |
| build | string | git short hash |
| mode | string | `play` / `autoplay` (autoplay = 60 sim frames/real frame) |
Ordering fields ทุก event: `seq` (int, เรียงใน run), `tReal` (float s, Stopwatch), `tSim` (float s, ผลรวม dt ที่ scene ใช้), `day` (int), `scene` (string).

## 2. Events (ชื่อ -> fields -> จุดที่ emit)
- `run_start`: starter(Species), startCoin, startEnergy -> `ChooseStarter` สร้าง `new GameState` (MenuScenes.cs) 
- `care_select`: pet, result(`ok|gap|noenergy`), care(Train/Feed/Clean/Heal|""), hit(Perfect/Great/Miss) , energyAfter -> CareScenes.cs CareSelectScene.Update (~L61-70) ทุกครั้งที่กด Confirm
- `qte_press`: pet, care, idx(1..10), hit, expGain(0/1), levelUps, ... -> QteScene.Update (~L100 หลัง Evaluate) / Apply
- `qte_end`: pet, care, perfect, great, miss, attempts, progressDelta, stomachDelta, cleanDelta, hpDelta, petLevelUps, playerLevelUp(bool) -> QteScene.Close()
- `fight_start`: enemy, pet, petHp, petMaxHp, petAtk, seaTea(bool), dodgeStart -> FightScene ctor
- `fight_press`: kind(`attack|dodge|miss`), hit, dmgDealt, combo, petHp, enemyHp, poisoned -> FightScene.Update (ที่ branch `zone==_dodge` / `zone==_attack` / else)
- `fight_hurt`: cause(`too_slow|miss_press`), dmgTaken, petHp, pet -> FightScene.EnemyHits() (ต้องส่ง cause เข้า)
- `fight_swap`: fromPet, toPet -> PetFell callback
- `fight_end`: enemy, won(bool), rounds(=#attack presses สำเร็จ), attackHits, attackMiss, dodgeOk, tooSlow, missPress, dmgDealt, dmgTaken, petsLost, durationReal -> ก่อน `_onEnd(_won)`
- `coin`: delta(int), source/sink(`start|daily|toothless_reward|merchant_sale|upgrade|revive|shop_apple|shop_tonic|shop_tea`), coinAfter -> ทุกจุดที่แก้ Coin (ดู audit) ผ่าน helper `GameState.AddCoin(int,string)`
- `energy`: delta, reason(`care|daily_restore|tonic|upgrade`), energyAfter, maxEnergy
- `exp`: delta, reason, level, exp, points, levelUp(bool) -> GameState.AddExp
- `upgrade_buy`: name(QTE/Energy/Progress), newLevel, coinCost, pointCost, coinAfter, pointsAfter -> UpgradeScene (BaseMenus.cs L42-51)
- `shop_buy`: item, price, target -> ShopScene (L277-303)
- `pet_death`: pet, cause(`starve|fight|boss`), hpBefore, deathCountTotal -> จุดที่ Hp ลดถึง 0 (AdvanceDay L~starve, FightScene.EnemyHits)
- `pet_revive`: pet, cost, reviveCountTotal -> DoctorScene L111
- `starve`: pet, hpBefore, hpAfter, died -> GameState.AdvanceDay
- `pet_added`/`pet_removed`: pet, via(`toothless_tame|merchant_sale`)
- `day_end`: day, coin, energyLeft(ก่อนนอน), energyUsedToday, maxEnergy, points, playerLevel, playerExp, upgQte, upgEnergy, upgProgress, alive(int), dead(int) + snapshot `pet_snapshot` ต่อสัตว์ -> NightScene ก่อน/หลัง `AdvanceDay` (ก่อน = ปิดวัน, หลัง = เช้าวันใหม่)
- `pet_snapshot`: pet, species, hp, maxHp, stomach, clean, progress, maxProgress, level, atk, dead -> ทุก `day_end` + `run_end` + `fight_start`
- `door_event`: day, event(`toothless|merchant|storm|bigz`), choice(`chase|tame|sell|refuse|fight|none`) -> DayEvents.cs (Option callbacks)
- `run_end`: outcome(`to_be_continued|game_over|quit|timeout`), dayReached, deathScene, ... -> ToBeContinuedScene ctor / GameOverScene ctor / quit (MainMenu `Game1.Exit`, Esc->Main Menu ใน Paused ChoiceScene) / AutoPlay TIMEOUT

## 3. Run summary row (runs.csv) — 1 แถว/run
runId,profile,seed,build,mode,outcome,dayReached,deathDay,deathScene,tRealSec,tSimSec,startCoin,finalCoin,coinEarnedTotal,coinSpentTotal,coin_daily,coin_toothless,coin_merchant,spent_upgrade,spent_revive,spent_shop,energyUsedTotal,energyLeftSumByDay(d1..d5 แยกคอลัมน์ energyUsed_d1..d5, energyLeft_d1..d5),careSelectPresses,careSelectOk,careSelectGap,qteP_<care>,qteG_<care>,qteM_<care> (x4 care),fightAtkP,fightAtkG,fightDodgeOk,fightTooSlow,fightMissPress,fightDmgDealt,fightDmgTaken,fightRounds,toothlessWon,bigzDmgDealt,upgQte,upgEnergy,upgProgress,pointsSpent,pointsLeft,playerLevel,playerExp,petDeaths,revives,starveEvents,merchantChoice(sell|refuse|n/a),petsFinal,petLvlMax,finalHpSum,finalStomachAvg,finalCleanAvg

## 4. Invariants (data-validator)
1. start `startCoin` + Σ`coin.delta` == `finalCoin`; coinEarned − coinSpent == finalCoin − startCoin; ทุก `coin.coinAfter` ต่อเนื่องกับ event ก่อนหน้า; coin ≥ 0 เสมอ.
2. ต่อวัน: energyUsed ≤ maxEnergy + 2×(tonic ที่ซื้อวันนั้น); energyLeft ≥ 0; energyUsed_d == #care_select(ok) ของวัน; `energy.energyAfter` ∈ [0, maxEnergy+2*tonics].
3. qte: perfect+great+miss == 10 (`Balance.Attempts`) ต่อ `qte_end`; count(`qte_press`)==10; Σqte presses == Σ(care_select ok) (ยกเว้นโดนตัดกลางคัน).
4. care_select: ok count == Σ energy(care) deltas; gap/noenergy ไม่ลด energy.
5. exp: Σ`exp.delta` (reason=qte) == #(qte_press hit≠Miss); points == playerLevel−1 − pointsSpent; pointsSpent == Σupgrade_buy.pointCost; exp < PlayerMaxExp.
6. upgrades: upgQte≤3, upgProgress≤2, upgEnergy≤3; maxEnergy==min(3+upgEnergy,6); cost ต่อครั้งตรง Balance (100c/1p, 150c/3p, 20c/2p).
7. fight: attackHits+attackMiss+dodgeOk+tooSlow(non-press)... presses = attackP+attackG+dodgeOk+missPress; dmgDealt == Σ fight_press.dmgDealt; won=true -> enemyHp==0; Toothless dmgDealt ≥ 120; petsLost ≤ #pets; dmgTaken == Σ hurt (ก่อน clamp Hp≥0 อาจน้อยกว่า, ใช้ min).
8. pet: 0≤stomach,clean≤100; hp≤maxHp; progress<maxProgress; level ไม่ลดลง; death -> revive ก่อนจะ care ได้; reviveCount*250 == spent_revive; petDeaths ≥ revives.
9. coin sources: daily == 100×(dayReached−1) (ถ้าไม่ตายก่อน); toothless ∈{0,200}; merchant ∈{0,5000}; merchant sell -> petsFinal ลด 1 และ shop_* == 0 ในวันนั้น.
10. เวลา: tRealSec>0; `seq` ต่อเนื่อง; day ไม่ลดลง; events เรียงตาม tSim; outcome เป็นค่าใดค่าหนึ่ง และมี `run_end` เพียง 1 ต่อ run; to_be_continued -> dayReached==5.
11. day_end: day_end ครบ 1..(dayReached−1) (+วัน 5 ไม่มี day_end เพราะจบที่ BigZ).

## 5. As built (Coder, deviations from sections 1-4)
- File: one `run_<yyyyMMdd_HHmmss>_<profile>[-refuse]_<seed>.jsonl` per process (no separate events/runs.csv from the game); `runId` = that name. Every row: `runId, seq, t (sim seconds = sum of dt since run_start), tReal, type, day, scene` + fields. `tSim` is therefore called `t`.
- Final row `run_summary` (flat; counters built inside `Telemetry`, e.g. `qteP_/qteG_/qteM_<care>`, `coinIn_<source>`, `coinOut_<sink>`, `petDeaths_d<N>`, `energyUsed_d<N>`, `energyLeft_d<N>`, `deathDay` = first pet death, -1 if none). `runs.csv` + `summary.md` are produced by `tools/Aggregate` from these rows (adds `pressesTotal`, `qtePerfectRate`, ...).
- `fight_end.durationReal` -> `durationSim`. `fight_hurt` has both `dmgTaken` (nominal) and `hpLost` (after clamp). `coin` uses `reason` for source/sink (no `start` event; use `run_start.startCoin`). `day_end` is emitted once (before `AdvanceDay`); the morning's `coin`/`energy`/`starve` events follow it with the new `day`. `mode` = `play`/`autoplay`; `profile` = bot profile or `human`.
- `run_end` outcomes: `to_be_continued | game_over | quit | timeout`; `quit` is also written by `Game1.OnExiting` / `ProcessExit` if the run was still open. Nothing is written if the player never reached the base (no `run_start`).
- Seed: `RunSeed` seeds `Wheel.Rng` and `BaseScene._rng` (`--seed`); `Gfx`/`Rain` randoms (visual only) are not seeded. Bot noise uses its own `Random(seed)`.
- Bot profiles: `perfect` (baseline), `sloppy`, `upgrade-first` (implies --refuse), `careless`, plus `caring`. NOTE: `perfect` presses the care wheel as soon as it is on any zone and the needle starts on Train, so it already only trains; `careless` is currently identical to `perfect`. `caring` (Feed/Clean/Heal by need) is the profile that actually exercises care.
- Bot profile `human` (round 2): every seed draws its own parameters (own `Random(seed*104729+3)`, independent of play RNG): hit skill 0.55-1.0 (chance to press a zone pass), `perfectWait` (wait for Perfect vs take an early Great), stray-press rate, reaction jitter 0-24 extra cooldown frames, care appetite (25% of seeds are low 0.15-0.5 => neglected care/early bed), care weights Train/Feed/Clean/Heal (20% chance each is 0), random upgrade priority (QTE/Energy/Progress, `patient` = save for the top item vs buy first affordable), per-item shop purchase probability, Toothless chase/tame chance, merchant sell/refuse (50%; forced refuse when only 1 pet because Sell is disabled), Doctor revive whenever a pet is dead and coin >= ReviveCost. Parameters are logged once as event `bot_params` right after `run_start`. Note: mode=`autoplay`, profile=`human` here is a bot; real play is mode=`play`.
- `careless` is kept selectable but is identical to `perfect` (see above) and is removed from the `run-batch.ps1` default profile list (defaults: perfect, sloppy, upgrade-first, caring, human).
- AutoPlay fix: with a single pet the merchant's Sell option is disabled; bots now pick Refuse then (previously `human` seeds with 1 pet soft-locked there).
- Structural finding: starvation deals only 20 HP per night once Stomach hits 0, so no pet can starve to death inside the 4 nights before the boss, and with <= 1 pet before Day 2 a Toothless-fight death is game over: revives are unreachable by design in the Day 1-5 slice (only boss-fight deaths happen).
- `--player <name>` (2026-10-09): tester name for real play. Sanitized to letters/digits/`-`/`_`, added to the file name (`run_<ts>_human-<name>_<seed>.jsonl`) and as field `player` on `run_start` + `run_summary` (so `runs.csv` gets a `player` column). Empty when not given. Run-id timestamp now uses the invariant (Gregorian) calendar; before, a Thai locale produced Buddhist-era years (`2569…`).
- `build` (2026-10-09): baked into the assembly at build time (`Bepal.csproj` target `SetSourceRevisionId` -> InformationalVersion `+<hash>`), so a published exe on a machine without git still logs the hash; runtime `git rev-parse` is only the fallback. Tester build: `dotnet publish BEPAL/Bepal_Game/Bepal -c Release -r win-x64 --self-contained -o <out>` (~83 MB, no .NET needed), then `Bepal.exe --telemetry data --player <name>`. Hash = HEAD at build time; uncommitted changes are not marked, so publish from a commit.
- `players.md` (2026-10-09): `tools/Aggregate` also writes `<out>/players.md` when the folder has real-play runs (`mode=play` or a `player` name): one section per player, one column per run, no averaging. `runs.csv` gains a `starter` column (from `run_start`). Bot-only batches do not get this file.
- `telemetry.txt` (2026-10-09): if this file sits next to `Bepal.exe` and no `--telemetry` is given, telemetry turns on by itself (plain double-click, no command line) and writes to `<exe dir>/data/`. Player name = first line that is not blank and not `#...` (`--player` still wins). Ignored by `--shots`. Tester kit: publish (see above), add `telemetry.txt` with the tester's name, zip, send; the tester sends back the `data` folder. Do not put the exe under `Program Files` (not writable).
