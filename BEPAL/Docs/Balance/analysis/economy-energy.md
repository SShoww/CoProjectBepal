# Economy + Energy analysis (Q-20261009-run-telemetry)

Analyst: ECONOMY-ENERGY-ANALYST. ข้อมูล: `data/raw/*.jsonl` 200 runs (BOT ทั้งหมด: perfect, sloppy, upgrade-first, caring, human x 40 seeds). Invariants I01-I15 ผ่านหมด.
สคริปต์: `tools/economy-energy.ps1`, `tools/economy-energy2.ps1` -> ผลดิบ `analysis/economy-energy-data.txt`, `analysis/economy-energy-data2.txt`.

## 0. ข้อควรระวัง (อ่านก่อน)
- ทุก run เป็น bot. `human` = bot ที่สุ่มพารามิเตอร์ต่อ seed (ไม่ใช่คนจริง) จึงใช้เป็น "การกระจายหลัก" แต่ยังเป็นพฤติกรรมสังเคราะห์.
- `perfect`/`caring`/`sloppy` ขาย Toothless 100% (sloppy 30 + 10 run ตายก่อน), `upgrade-first` ปฏิเสธ 100%. human: sell 8 / refuse 30 / ไม่ถึง merchant 2. => ความต่าง sell vs refuse ปนกับความต่างของ bot (confounded).
- ทุก profile ไม่เคยใช้พลังงานวัน 5 (d5 used = 0 ใน 190/190 run ที่ถึง boss) = พฤติกรรม bot ไม่ใช่ข้อมูลผู้เล่น.
- Boss Big Z HP 9999 ชนะไม่ได้ => "ผลกับ boss" วัดได้เฉพาะ `bigzDmgDealt`.
- ในโปรโตไทป์ 4 คืน: `day_end` ของวัน d คือ "ก่อน" +100 ของคืนนั้น.

## 1. Coin: รับ/จ่าย (ต่อ run, ค่าเฉลี่ย)
| profile | daily | Toothless | merchant | upgrade | shop (apple/tea/tonic) | revive | final | หมายเหตุ |
|---|---:|---:|---:|---:|---|---:|---:|---|
| human | 385 | 90 | 1000 | 196.5 | 10 / 7.7 / 19 | 0 | 1392 (med 382) | sell 8: 5539, refuse 30: 366 |
| perfect | 400 | 200 | 5000 | 0 | 0 | 0 | 5750 | ไม่ซื้อ, points ค้าง 4 |
| sloppy | 325 | 150 | 3750 | 0 | 0 | 0 | 4375 | 10/40 game over d2 |
| upgrade-first | 400 | 200 | 0 | 300 | 25 / 18 / 40 | 0 | 367 | refuse, ซื้อ QTE x3 + shop ครบ |
| caring | 400 | 200 | 5000 | 0 | 0 | 0 | 5750 | |

รายได้ตลอดสไลซ์ (Balance): path Refuse = StartCoin 150 + DailyCoin 100 x4 + ToothlessReward 200 = **750**; path Sell = **5750**.
ที่ใช้ได้จริงใน d1-d4: upgrade รวมสูงสุด 790 coin (QTE 3x100, Energy 3x150, Progress 2x20) + shop ที่ซื้อได้ครั้งเดียว 83 (25+40+18) + revive 250 (ไม่เคยถูกใช้).

### เงินตึงหรือหลวม? (human + upgrade-first)
- เงินตึงเฉพาะ d1: coin ตอนจบ d1 ของ upgrade-first = 50 (150-100 ซื้อ QTE ตั้งแต่ d1), human med 50 (mean 93.5). ตอนจบ d2: human med 250, upgrade-first 150.
- ก่อนถึง merchant (d3 เช้า): human refuse med 350 (150..550), upgrade-first 250. ร้านค้ามี 3 ของรวม 83 => ซื้อได้หมดทุก run, ไม่มีของที่ "แพงเกินซื้อ".
- ตอนจบ (refuse): human 366, upgrade-first 367 = **49% ของรายได้ 750 ไม่ได้ใช้**. human refuse 24/30 run จบด้วย coin >= 250 (พอ revive แต่ไม่เคยมีสัตว์ตายให้ revive).
- snapshot ท้ายวัน (human, upgrade ที่ยังไม่ max): QTE 126 ครั้ง: ซื้อได้ทันที 37, **coin ไม่พอ 5**, **points ไม่พอ 55**, ไม่พอทั้งคู่ 29. Energy 154: point-limited 106, ทั้งคู่ไม่พอ 48, coin-limited 0. Progress 150: point-limited 138, ซื้อได้ 12. => **ตัวบีบคือ Points ไม่ใช่ Coin** (conf: สูง, profile: human + upgrade-first + sloppy; ดู C ใน data.txt).
- coin-limited พบจริงแค่ upgrade-first d1 (QTE ที่ 2 ต้องรอ +100 คืนแรก) -> สั้นมาก.

### Points คือเพดานจริง
- exp ต่อ Energy = 10 (perfect). ต้องใช้ exp สะสม L2=10, L3=30, L4=60, **L5=100**, L6=150. 12 Energy (d1-d4) x 10 = 120 exp => **จบ d4 ที่ L5 เสมอ (perfect/caring/upgrade-first: playerLevel 5, exp 20)** => Points สูงสุด 4. (human reach L5 ใช้ 12.8 Energy เฉลี่ย, L4 = 9.3, L3 = 5.0.)
- รายการซื้อทั้งหมดต้อง 16 points (QTE 3, Energy 9, Progress 4) => ซื้อได้ <= 25% ของ tree. upgrade-first ใช้ 3 points (QTE x3) เหลือ 1.
- Energy upgrade (3 pts + 150c) ได้ผลสั้น: ซื้อ d3 = +1 ทันทีและ +1 วัน d4 (d5 ไม่ได้ใช้) = **+2 Energy (~20 exp)**; ซื้อ d4 = ~+1-2 (human seeds 9/16/29 ซื้อ d4: d4 ใช้ 3/4 ไม่เต็มด้วยซ้ำ). human ซื้อ 8 ใน 40 run (d3 x5, d4 x3), 5 ใน 8 run ใช้ energy เต็ม max=4 ในวัน d3/d4.

## 2. Merchant: Sell 5000 vs Refuse
| | Sell | Refuse |
|---|---|---|
| coin | +5000 (final 5539-5750) | 0 (final 366) |
| สัตว์ | Toothless หาย (petsFinal 1) | มีสัตว์ 2 ตัว (human 1.3, upgrade-first 2.0) |
| ร้านค้า | ไม่เปิด (DayEvents.cs L93 เปิดเฉพาะ Refuse) | เปิด 1 ครั้ง: 83 coin ซื้อครบ |
| sink หลังขาย | **ไม่มีเลย**: upgrade ติด points (เหลือ 4 pts ใน perfect/caring), shop ปิด, revive ไม่มีสัตว์ตาย | |
| bigzDmgDealt (boss, ชนะไม่ได้) | perfect 388, caring 663, sloppy 78, human(sell) 176 | upgrade-first 1330, human(refuse) 246 |
- 5000 = 6.7x รายได้ทั้งสไลซ์ของ path Refuse (750) และ 4.5x ของ sink สูงสุดรวมทุกอย่าง (790+83+250 = 1123); **ซื้ออะไรไม่ได้เพิ่ม** => ราคา 5000 ไม่มีผลทางเศรษฐกิจในสไลซ์ ตัวแปรเดียวคือ "ได้เงิน vs เสียสัตว์ตัวที่ 2 + ร้านค้า".
- สิ่งที่เสียจากการขาย = สัตว์ตัวที่ 2 (boss damage ต่ำกว่า: human sell 176 vs refuse 246 ค่าเฉลี่ย แต่ median 154 vs 146 -> n เล็ก 8 vs 30 ไม่สรุปได้; upgrade-first 1330 vs perfect 388 ต่างมากแต่ confound ด้วย QTE upgrade + 2 pets + tea).
- ข้อสรุป: Sell ถูก dominate ในเชิง "ใช้เงิน" แต่ไม่ถูก dominate ในเชิงเรื่องราว; ถ้าตั้งใจให้เป็น dilemma ราคา 5000 ไม่ทำให้มัน tempting จริง (ไม่มีอะไรให้ซื้อ).

## 3. Energy
เงื่อนไข: BaseEnergy 3, ไม่มี event `noenergy`; `gap` 6 ครั้งไม่เสีย energy.

| profile | ใช้ต่อวัน d1-d4 (ใช้/สูงสุด) | วันที่เหลือ energy > 0 | หมายเหตุ |
|---|---|---|---|
| perfect, caring | 3.0/3 ทุกวัน | **0/160 วัน** | energy คือเพดานจริง |
| upgrade-first | 3,3,5,3 (d3 มี Tonic +2) | 0/160 | |
| sloppy | 3, 2.2, 3, 3 | 10 วัน (d2 ตายแล้ว/เกมจบ) | |
| human | 2.0, 1.9, 3.2, 2.2 (max 3.0-3.2) | d1 28/40, d2 29/40, d3 27/38, d4 29/38 | เหลือเฉลี่ย ~1 E/วัน |
- human แยกตาม bot appetite: appetite >= 0.5 (30 run, 117 วัน) ใช้ 2.72 เหลือ 0.63, เหลือ>0 63% ของวัน; appetite < 0.5 (10 run, 37 วัน) ใช้ 1.27 เหลือ 2.00 (เหลือ 100% ของวัน). => "energy ทิ้ง" ของ human เป็นพารามิเตอร์ bot (ขี้เกียจ) ไม่ใช่ปัญหา economy; ผู้เล่นที่อยากทำ จะใช้หมดทุกวัน (profile อื่น 4 แบบยืนยัน).
- Tonic: วันที่ซื้อ ใช้ 4.05 vs 2.13 (human), 5.0 vs 3.0 (upgrade-first) = บอทใช้ +2 ได้จริงครบ. ผลต่อ level human: tonic 4.05 vs ไม่ซื้อ 4.09 (ไม่ต่าง, confound กับ appetite).
- **Tonic ไม่จำกัดจำนวน**: `BaseMenus.cs` L290/L306 เปิดปุ่มเมื่อ `Coin >= 40` ซ้ำได้ไม่จำกัด + บวก Energy ตรงๆ (ไม่ clamp). bot ซื้อครั้งเดียว (19/19, 40/40) จึงไม่ปรากฏใน telemetry แต่ผู้เล่นมี coin ก่อน merchant 250-550 (med 350) => ซื้อได้ 6-13 ขวด = +12 ถึง +26 Energy ใน d3 (= +120-260 exp, เกิน L5 ได้ทันที, ทำให้ Points และ stat เกินแผน). (conf: สูงจากโค้ด; ไม่มีข้อมูล bot ยืนยันพฤติกรรมนี้.)
- Tonic 40c/2E = 20c/E vs Energy upgrade 150c+3pts ได้ ~2E => Tonic คุ้มกว่าหลายเท่า, Energy upgrade แทบไม่คุ้ม.
- วัน 5: ทุก run เหลือ 3 E ตอนเจอ boss (bot ไม่ care ก่อนเปิดประตู).

## 4. Energy -> ผลตอบแทน (ต่อ 1 Energy = 1 QTE 10 ครั้ง)
| care | exp | ผลหลัก (perfect-level: caring/perfect) | ผล human (hit จริง) | ต้นทางเสีย |
|---|---:|---|---|---|
| Train | 10 (human 9.6) | progress +92..100 (human 99; ~1 pet level ที่ L1, 100+(L-1)x50 ต่อเลเวล) | 99 | Stomach -10 (-5 ถ้าติดศูนย์) |
| Feed | 10 (human 8.95) | Stomach +57.5 (caring) | +25.5 | - |
| Clean | 10 (human 8.4) | Clean +60 (caring) | +30.2 | Stomach -5 |
| Heal | 10 (human 8.2) | HP สูงสุด +60 แต่เฉพาะเมื่อ HP ไม่เต็ม | avg 5.3, **72/86 ครั้ง hpDelta = 0** | Stomach -5 |
- exp เท่ากันทุก care (ไม่มี care ไหนให้ exp เกิน) => Player level ไม่แยกกลยุทธ์ มีแต่ miss เท่านั้นที่ลดลง.
- Train คือเพียงตัวที่โตสัตว์ (+10 HP +5 ATK ต่อ pet level) => ราคาต่อ pet level ~1.0 E (L1) -> ~1.5 E (L2) -> ~2 E (L3) ...
- Heal: HP เต็มเกือบตลอด (Toothless fight_start 100% ทุก profile) -> Heal มีค่าเฉพาะหลัง fight หรือ starve; bot human ใช้ Heal 86 ครั้ง (24% ของ care energy) 84% ไร้ผล (conf: กลาง; เพราะน้ำหนัก care ของ bot สุ่ม ไม่ใช่การตัดสินใจของคน).

## 5. Upkeep: Stomach/Clean vs HP
- ค่าลดต่อคืน: Stomach -20 (DailyStomachDecay), Clean -15 (DailyCleanDecay), Starve -20 HP เฉพาะ Stomach 0.
- ต้นทุนเป็น Energy: 1 Feed perfect ~ +60 Stomach, 1 Clean perfect ~ +60 Clean => ต่อคืน 0.33 + 0.25 = **0.58 E (19% ของ 3 E)**; 4 คืน d1-d4 = 80 Stomach + 60 Clean ~ **2.3 E (19% ของ 12 E)** + Train เผา Stomach 10/ครั้ง. caring ใช้จริง 4 E (Feed 2, Clean 2 = 33% ของ budget; Train 8).
- ความเสี่ยงต่อ HP: starve = 120 ครั้ง/40 run ใน perfect และ 119 ใน upgrade-first (3 คืนทุก run: d2, d3, d4 -> -60 HP/ตัว) แต่ **starve ฆ่าสัตว์ได้ 1 ครั้งใน 200 run** (sloppy d4, สัตว์ HP ต่ำจาก Toothless fight). caring: starve 0.
- ผลจริงของ upkeep คือ threshold: Clean <= 50 -> ATK x0.75, Clean <= 25 -> MaxHp x0.8. ตอนเข้า boss: perfect/upgrade-first Clean เฉลี่ย 28, 50% ของ fight_start อยู่ <= 25; caring Clean 68, 0% <= 50.
  - ตอน boss: perfect HP 75% ของ max, ATK 34; caring HP 100%, ATK 40; bigzDmg **caring 663 vs perfect 388 (+71%) แม้ Train น้อยกว่า 4 ครั้ง (8 vs 12)**. (profile: perfect vs caring เท่านั้น, deterministic bot; ยืนยันทิศทางว่า upkeep 33% ของ energy คุ้มใน boss.)
  - Toothless (d2) ไม่ได้รับผล upkeep (ผ่านคืนเดียว: Clean ~55-65, HP เต็ม) => upkeep เริ่มมีผลตั้งแต่ d3 เท่านั้น.
- human: stomach=0 ท้ายวัน 7%, Clean <= 50 29% (<= 25: 13%), starve 14 ครั้ง/10 run, ไม่ตาย.

## 6. Revive / ตายก่อน boss / Doctor
- pet_death: boss d5 ทุก run ที่ถึง d5 (perfect 40, caring 40, upgrade-first 80, human 48, sloppy 29); **ก่อน d5 มี 13 ครั้ง = 12 ครั้งเป็นการตายใน Toothless fight d2 ที่จบเป็น game over (sloppy 10/40 = 25%, human 2/40 = 5%) + 1 ครั้ง sloppy starve d4**. revive = **0/200**, starve ที่ตายในช่วง d2-d4 = 1/200.
- game over 12 run: coin ตอนตาย = **250 ใน 11 run** (150 ใน 1) = `ReviveCost` พอดี แต่ `DayEvents.cs` L52 เรียก GameOver ทันทีไม่ตรวจ coin (ต่างจาก NightScene L162 ที่ตรวจ `Coin < ReviveCost`) => revive ไม่เคยถึงมือ.
- ยืนยันที่ spec §5: starvation -20/คืน ทำให้ Doctor 250 coin ไม่มี use case ในสไลซ์ (หลัง Toothless ชนะ ไม่มีสัตว์ตายจนถึง boss).

## 7. ข้อเสนอ (เรียงตาม impact)
| # | ข้อเสนอ | Constant | evidence | conf | impact |
|---|---|---|---|---|---|
| P1 | จำกัด Caffeine Tonic (ปัจจุบันซื้อซ้ำได้ไม่จำกัด): เพิ่ม `Balance.TonicMaxPerDay = 1` หรือขึ้นราคา; ตอนนี้ 40c ใน `BaseMenus.cs` `Items[1]` | `Items[1].Price` (40) -> เช่น 80 + cap 1/วัน | โค้ด L290/306; coin ก่อน merchant med 350 (human), 250 (upgrade-first); bot ซื้อครั้งเดียว (ไม่มีข้อมูลซ้ำ) | สูง (โค้ด) / ไม่มีข้อมูล bot | สูง: เหนือ L5 ได้ใน d3, ทำลาย pacing exp/points |
| P2 | MerchantOffer ให้มีน้ำหนักจริง (ตอนนี้ไม่มี sink ใช้เงิน 5000) | `MerchantOffer` 5000 -> ~600-1000 (หรือเพิ่ม sink) | perfect/caring final 5750 ค้าง 100%; sink สูงสุดที่ซื้อได้ 790+83; points จำกัด 4 | กลาง (ขึ้นกับเจตนา GDD) | กลาง: Sell vs Refuse กลายเป็น trade-off จริง |
| P3 | Energy upgrade แพงเกินผล: ลด points หรือ coin | `EnergyPoints` 3 -> 2 (หรือ `EnergyCoin` 150 -> 100 ไม่แก้ปัญหา points) | ได้ ~+2 E (<= 20 exp) ต่อ 3 pts จาก 4 pts ทั้งหมด; Tonic ได้ 2E ที่ 40c; human ซื้อ 8/40, point-limited 106/154 snapshot | กลาง | กลาง |
| P4 | GameOver จาก Toothless ควรเปิดทาง revive เมื่อ coin >= ReviveCost (กรณี coin = 250 พอดี 11/12) | `ReviveCost` 250 (คงไว้) + logic `DayEvents.cs` L52 | game over 12 run, coin 250 = ReviveCost; revive 0/200 | กลาง-สูง (ข้อเท็จจริง), ทางเลือกคือ design | กลาง: ลด game over 5-25% (bot) |
| P5 | Sea Tea ไร้ประโยชน์ในสไลซ์: ร้านเปิดหลัง Toothless แล้ว (d3) ถัดไปมีแต่ boss ที่ชนะไม่ได้ | `Items[2]` (18) หรือย้าย/ปล่อย Tea ให้ใช้ได้ก่อน Toothless | human 17 ซื้อ, upgrade-first 40/40 -> ใช้ใน boss เท่านั้น (seaTea=true ที่ Big Z) | กลาง | ต่ำ |
| P6 | Heal ควรบอก/ปิดเมื่อ HP เต็ม | (UI, ไม่ใช่ constant) | 72/86 Heal human hpDelta=0 | ต่ำ-กลาง (bot สุ่ม) | ต่ำ |
| P7 | DailyCoin อาจลดได้ (เงินหลวม): Refuse path 750 vs ต้องใช้จริง ~220-300+83 | `DailyCoin` 100 -> 50-75 (ไม่แนะนำเร่งด่วน) | จบ refuse เหลือ 49% | กลาง-ต่ำ | ต่ำ |
| P8 | ถ้าต้องการให้ upkeep เป็นการตัดสินใจ (ตอนนี้เป็นแค่ ATK/HP threshold): ไม่ต้องแก้ถ้าชอบ; DailyStomachDecay 20 / DailyCleanDecay 15 ให้ upkeep ~19% ของ energy | `DailyStomachDecay`, `DailyCleanDecay`, `StarveDamage` | caring +71% bigzDmg แต่ starve ไม่ฆ่า (1/200) | กลาง | กลาง: ถ้าอยากให้ starvation มีน้ำหนัก ต้อง StarveDamage 20 -> ~35+ |

## 8. Limitations
- ไม่มีข้อมูลคนจริง; bot `human` ไม่เล่น Day 5 energy, ไม่ซื้อ Tonic ซ้ำ, สุ่มน้ำหนัก care.
- ไม่ได้ทดสอบ counterfactual (ไม่มี run ปฏิเสธพร้อม bot เดียวกันกับขาย). ต้องการ: run `human` seed เดียวกัน force-sell vs force-refuse (ต้อง flag ใน run-batch).
- ราคา shop/upgrade ยังอยู่ใน `BaseMenus.cs` (known deviation) ไม่ใช่ `Balance`.
