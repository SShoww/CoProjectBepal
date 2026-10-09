# BePal Balance Report (Q-20261009-run-telemetry)

สร้างโดย BALANCE-REPORTER จาก `analysis/run-stats.md`, `analysis/economy-energy.md`, `analysis/upgrades.md`, `data/summary.md`, `data/runs.csv`, `data/validation.md`, `telemetry-spec.md` sec 5 และ `Model/GameState.cs` (`Balance`). ไม่มีการแก้โค้ดเกม. build ที่วัด = `dab9037`.

> **ข้อควรระวังเรื่องข้อมูล:** ข้อมูลทั้งหมดมาจาก **200 BOT runs** (40 seeds x perfect / sloppy / upgrade-first / caring / human) ไม่ใช่ผู้เล่นจริง. **ยังไม่มี telemetry จากคนจริง (mode=play) เลยแม้แต่ run เดียว.** `human` ใน CSV = bot ที่สุ่มพารามิเตอร์ต่อ seed (skill 0.55-1.0, appetite, ลำดับอัปเกรด) ไม่ใช่คน. เวลาทั้งหมดเป็น sim time, bot ไม่ลังเล/ไม่ idle จึงเป็นขอบล่างของเวลาคนจริง. `perfect`/`caring`/`upgrade-first` เป็น route เดียว (1 route/40 runs), seed ซ้ำข้าม profile (ไม่ independent).

## 1. TL;DR

- Slice ผ่านได้เกือบทุก run: to_be_continued 188/200; game over 12/200 (6%) **ทั้งหมดที่ Day 2 Toothless fight ขณะมีสัตว์ตัวเดียว** (sloppy 10/40, human 2/40, อื่น 0).
- Big Z `BigZHp = 9999` ชนะไม่ได้ (0/187 boss fights, dmg สูงสุด 2956) -> Day 5, upgrade, Sea Tea วัดผลต่อ outcome ไม่ได้.
- เศรษฐกิจหลวม: Sell `MerchantOffer = 5000` -> จบด้วย 5750 coin ที่ไม่มี sink; Refuse path รายได้รวม 750 และเหลือ ~49% ไม่ได้ใช้; `ReviveCost = 250` ใช้ 0/200.
- คอขวดของ upgrade คือ **Points (สูงสุด 4)** ไม่ใช่ coin (tree ต้อง 10-16 pts); Energy lv2/3 ไม่มีทางถึง.
- ช่องโหว่โค้ดที่ยังไม่มีข้อมูลยืนยัน: Caffeine Tonic ซื้อซ้ำได้ไม่จำกัด (40c = +2 Energy) -> ทำลาย pacing exp/points ได้.
- Top 5 ที่ควรทำก่อน: (1) cap Tonic, (2) ลดความโหด Toothless (`ToothlessAtk` 15->12), (3) ตัดสินใจเรื่อง Boss/`BigZHp`, (4) ตัดสินใจ `MerchantOffer`/sink/Doctor, (5) `EnergyPoints` 3->2. **ทั้งหมดต้อง re-measure ด้วยคนจริงก่อนปิด.**

## 2. ข้อมูลต่อหนึ่ง run (ต่อ profile)

รูปแบบ = mean / median / p10 / p90 (คำนวณใหม่จาก `data/runs.csv` ตรงกับ `data/summary.md`). n=40/profile.

### 2.1 ภาพรวมต่อ run

| metric | perfect (เพดาน) | caring | upgrade-first | **human** (หลัก) | sloppy (พื้น) |
|---|---|---|---|---|---|
| sim duration (s) | 200.8 / 200.0 / 188.9 / 213.7 | 243.1 / 241.8 / 229.8 / 259.3 | 301.6 / 296.6 / 260.4 / 350.4 | 278.1 / 288.5 / 186.9 / 356.3 | 276.7 / 316.2 / 105.2 / 369.0 |
| presses รวม (care select+QTE+fight) | 169.4 / 168 / 154.9 / 188.3 | 180.5 / 177.5 / 165 / 198.2 | 278.1 / 271 / 231.7 / 334.8 | 127.4 / 139 / 58.7 / 172.8 | 125.1 / 149 / 44 / 157.1 |
| QTE Perfect / Great / Miss (mean) | 83.8 / 16.2 / 0 % | 90.7 / 9.3 / 0 % | 100 / 0 / 0 % | 86.0 / 1.7 / 12.3 % | 41.9 / 38.8 / 19.4 % |
| Fight hit rate (mean) | 96.9% | 96.4% | 96.0% | 83.5% | 73.3% |
| coin earned | 5600 | 5600 | 600 | 1475 (med 400, p90 5600) | 4225 (med 5600, p10 100) |
| coin spent | 0 | 0 | 383 | 233 (med 274, p10 43, p90 367) | 0 |
| coin left | 5750 | 5750 | 367 | 1392 / 382 / 222 / 5450 | 4375 / 5750 / 250 / 5750 |
| energy used รวม | 12 | 12 | 14 | 9.1 / 10 / 4 / 13.1 | 9.75 / 12 / 3 / 12 |
| player level | 5 | 5 | 5 | 3.98 / 4 / 3 / 5 | 3.65 / 4 / 2 / 5 |
| points เหลือ (ค้างไม่ได้ใช้) | 4 | 4 | 1 | 0.35 / 0 / 0 / 1 | 2.65 / 3 / 1 / 4 |
| upgrades ที่ซื้อ | 0 | 0 | 3 (QTE x3) | 2.0 / 2 / 1 / 3 | 0 |
| pet level สูงสุด (mean) | 6.0 | 5.0 | 6.9 | 2.3 | 3.1 |
| สัตว์ตาย (mean) | 1 (boss) | 1 (boss) | 2 (boss) | 1.25 | 1 |
| starve events (mean) | 3 | 0 | 3 | 0.35 | 0.43 |
| bigzDmgDealt mean / median (Hp 9999) | 388 / 375 | 663 / 615 | 1330 / 1214 | 231 / 151 (n=38) | 78 / 71 (n=29) |
| outcome | TBC 40 | TBC 40 | TBC 40 | TBC 38, **game over 2 (5%)** | TBC 30, **game over 10 (25%)** |
| วันที่สัตว์ตายครั้งแรก | D5 (40) | D5 (40) | D5 (40) | D2 (2), D5 (38) | D2 (10), D4 (1, starve), D5 (29) |

หมายเหตุ: ใน human ที่ buy "which": QTE x3 = 18 runs, Energy x1 = 8, QTE x2 = 5, Progress x2 = 3, Progress x1 = 2, QTE x1 = 1, ไม่ซื้อ 3 (นับจาก `runs.csv`). Human level กระจาย L1 1 / L2 1 / L3 8 / L4 18 / L5 12.

### 2.2 อัตรา hit ต่อกิจกรรม (จาก `analysis/run-stats.md`, ตรวจซ้ำ human/sloppy กับ `runs.csv` แล้วตรง)

| กิจกรรม | human (Perfect / Great / Miss) | sloppy | perfect-ceiling bots |
|---|---|---|---|
| Care select (เลือก care) | ok ทุกครั้ง, Perfect 100%; `noenergy` = 0 | gap 6/396 (1.5%) | Perfect 100% |
| QTE Train (n=980 / 2360) | 90.5 / 5.5 / 4.0 % | 54.4 / 28.3 / 17.3 % | perfect 83.8/16.2/0; upgrade-first 100/0/0 |
| QTE Feed (1080 / 990) | 89.2 / 0.4 / 10.5 % | 24.8 / 51.4 / 23.7 % | caring Feed+Clean 100% Perfect |
| QTE Clean (730 / 430) | 83.7 / 0.4 / 15.9 % | 20.9 / 56.3 / 22.8 % | |
| QTE Heal (860 / 120) | 81.4 / 0.7 / 17.9 % | 23.3 / 42.5 / 34.2 % | |
| Fight attack | 58 / 42 / 0 % (n=591) | 23 / 77 / 0 % (n=375) | Great 100% (bot ไม่เคยพลาด attack) |
| Fight dodge Perfect | 100% (n=356) | 100% (n=154) | 100% |
| Fight hurt: too_slow / miss_press | 145 / 135 | 135 / 179 | perfect 187 / 42 |

ใน log `fight_press` ไม่มี attack Miss (ความพลาดอยู่ที่ `fight_hurt` และ `attackMiss` ใน `fight_end`) -> อัตรา attack Miss = 0 เป็นข้อจำกัดของ bot/logging ไม่ใช่ค่าสมดุล.

### 2.3 พลังงานและเงินต่อวัน (mean; ตรวจกับ `runs.csv`)

| profile | energy ใช้ D1 / D2 / D3 / D4 | energy เหลือท้ายวัน D1-D4 | หมายเหตุ |
|---|---|---|---|
| perfect, caring | 3 / 3 / 3 / 3 | 0 ทุกวัน | D5 เหลือ 3 (bot ไม่ care ก่อนบอส) |
| upgrade-first | 3 / 3 / 5 / 3 | 0 ทุกวัน | D3 = Tonic +2 |
| human | 2.05 / 1.95 / 3.18 / 2.21 | ~0.95-1.05 / วัน | appetite ต่ำ (25% ของ seeds) ปล่อยทิ้ง |
| sloppy | 3 / 2.25 / 3 / 3 | D2 มี 0.75 (ตายไปแล้ว 10 run) | |

Coin: Refuse path = StartCoin 150 + `DailyCoin` 100 x4 + `ToothlessReward` 200 = 750; Sell path = 5750. Upgrade+shop+revive ที่ซื้อได้สูงสุดทางทฤษฎี 790 + 83 + 250 = 1123 แต่ด้วย 4 points ซื้อ upgrade ได้จริงสูงสุด ~300 (QQQ).

### 2.4 ตัวอย่าง 1 run: `run_20261009_170340_human_25` (human, seed 25)

ใกล้ค่า median ของ human (286.2 s vs median 288.5 s; presses 142 vs 139). Mossling starter, skill 0.69, appetite 0.71, upgOrder QTE>Energy>Progress, refuse merchant. ตัวเลขทุกตัวจาก `data/raw/run_20261009_170340_human_25.jsonl` (ผลรวมตรง `runs.csv`: finalCoin 385, earned 600, spent 365, energy used 10).

| วัน | เหตุการณ์ | care / QTE (Perfect-Great-Miss) | energy ใช้ / เหลือ | coin (ท้ายวัน ก่อน +100) | อื่น ๆ |
|---|---|---|---|---|---|
| D1 (35.6 s) | ซื้อ QTE lv1 (-100) | Feed 9-0-1, Feed 9-0-1 | 2 / 1 | 50 | player L2, points 0 |
| D2 (133.0 s) | ประตู Toothless -> **Tame**, ไฟต์ชนะ 7 rounds (Mossling Hp100/Atk20; dealt 130, taken 45, 20.7 s), +200, ได้ Toothless; ซื้อ QTE lv2 (-100) | Feed 7-1-2, Clean 8-0-2 | 2 / 1 | 250 | player L3, สัตว์ 2 ตัว |
| D3 (195.9 s) | Merchant -> **Refuse**; Shop: Tonic -40, Crab Apple -25; ซื้อ QTE lv3 (-100) | Train x4: 9-0-1, 10-0-0, 8-1-1, 9-0-1 (progress 90/100/85/90) | **4** (3 + Tonic) / 1 | 185 | player L4 |
| D4 (237.6 s) | Storm | Train 10-0-0, Feed 8-0-2 | 2 / 1 | 285 | exp 29/40 |
| D5 (286.2 s) | Big Z (Mossling Hp 83/88, Atk 19, Clean 5) | - | 0 / 3 | 385 (final) | 13 rounds, dealt 267, taken 190, **สัตว์ตายทั้ง 2 ตัว**, outcome to_be_continued |

ข้อสังเกตจากตัวอย่าง: ใช้ energy ไม่เต็มทุกวัน (เหลือ 1), มี 0 points เหลือ, จบด้วย coin 385 ที่ไม่ได้ใช้ (ใกล้ median human 382), และ Clean ของ Mossling เหลือ 5 ตอนเจอบอส.

## 3. กระทบกันระหว่าง 3 analyses (reconcile)

| # | ประเด็น | ข้อสรุป | หลักฐาน |
|---|---|---|---|
| R1 | "Energy ไม่ใช่คอขวด" (run-stats F7/sec 6) vs "Energy ใช้เต็ม D1-D4 = เพดานจริง" (economy sec 3) | **ถูกทั้งคู่ แต่คนละ profile.** ผู้เล่นที่อยากทำ (perfect/caring/upgrade-first/sloppy ส่วนใหญ่) ใช้ 3/3 ทุกวัน (0/160 วันเหลือ) -> Energy คือเพดาน; human ใช้ 9.1/15 เพราะ `appetite` ของ bot (appetite <0.5 ใช้ 1.27 เหลือ 2.0 ตาม economy). ค่าจริงของคนจริง = **unresolved** (ต้อง mode=play). ผลต่อดีไซน์: ยังไม่ควรเพิ่ม Energy; แต่ Energy เป็น currency ที่มีค่าจริงสำหรับผู้เล่นที่ลงมือ. | `runs.csv` energyUsed_d1..d4: perfect 3.0, human 2.05/1.95/3.18/2.21 |
| R2 | จำนวน boss fights: run-stats ระบุ "0/195" vs upgrades "0/187" | **187 ถูก** (40+40+40+38+29). 195 เป็นตัวเลขผิดใน run-stats (ผลรวมตารางในไฟล์เดียวกันได้ 187). ผลสรุป "ชนะไม่ได้" ไม่เปลี่ยน. | `runs.csv` bigzDmgDealt ไม่ว่าง = 187, max 2956 |
| R3 | run-stats P6: เกณฑ์ dmg ~400 -> "human ~25% / perfect ~50% ผ่าน" | **ตัวเลขนี้ผิด.** จริง: >=400 human 4/38 (10.5%), perfect 17/40 (42.5%), caring 37/40, upgrade-first 40/40, sloppy 0/29. หา threshold ใหม่หลังมีข้อมูลคน (human median 150.5, max 1100). | `runs.csv` bigzDmgDealt |
| R4 | สาเหตุ care Clean/Heal/Feed Miss สูงกว่า Train: run-stats P3 เดาว่า "Train zone กว้างกว่า" | **ไม่ถูก.** ใน `CareScenes.cs` Train zone แคบกว่า (Perfect 0.07 / Great 0.16 x TrainScale) ส่วน care อื่นกว้างกว่า (Perfect 0.12 / Great 0.27). ความยากมาจากกลไก: Clean โซนวิ่งหนี (`CleanZoneSpeed = 4.8`), Heal โซนหดตัว (`HealShrink`), Feed หมุนกลับทิศ. P3 เดิม (ขยาย zone) จึงถูก downgrade เป็น "ปรับกลไก" (ดู A5) และ **ความยากต่างกันของ bot อาจไม่เท่าของคน (conf ต่ำ-กลาง)**. | `CareScenes.cs` L126-128, L166-173 |
| R5 | Revive/Doctor ไม่เคยถูกใช้: run-stats F5 ("ไม่มีตายก่อนบอส") vs economy sec 6 ("GameOver เรียกทันทีไม่ตรวจ coin") | **ถูกทั้งคู่ และเสริมกัน.** ก่อน Day 5 มีสัตว์ตาย 13 ครั้ง = 12 ครั้งเป็น Toothless Day 2 -> `GameOverScene` ทันที (`DayEvents.cs` L52, ไม่ตรวจ `Coin >= ReviveCost` ต่างจาก `MenuScenes.cs` NightScene L162) ทั้งที่ 11/12 มี coin 250 = `ReviveCost` พอดี (อีก 1 มี 150); อีก 1 ครั้งคือ starve D4 ใน `sloppy_30` ซึ่งเกมไปต่อได้ (coin 5650) แล้วไปเจอบอสโดยไม่มีสัตว์เป็น (bot ไม่ revive). Revive 0/200. | game over coin: 250 x11, 150 x1; `sloppy_30.jsonl` |
| R6 | `MerchantOffer`: run-stats P5 "5000 -> ~1500" vs economy P2 "~600-1000" | **unresolved (เป็นเรื่องเจตนาดีไซน์).** ข้อเท็จจริงที่ตรงกัน: sink สูงสุดที่ใช้ได้หลังขาย (ไม่มี shop, 4 pts) ~300 + revive 250 ที่ไม่มีให้ใช้; รายได้ Refuse path ทั้งสไลซ์ = 750. ค่าใดก็ตาม >~600 ยังคง "ไม่มีอะไรให้ซื้อ" ถ้าไม่เพิ่ม sink. ค่าที่ควรทดลองคือ 1000 พร้อมเพิ่ม sink (ดู B2) ไม่ใช่เลือกจาก bot. | economy sec 1-2 |
| R7 | `ProgressPoints` 2->1 (upgrades P1) vs "Progress too strong per cost" (upgrades sec 6, P4 ลดตัวคูณ) | **ขัดกันเอง.** ลด point ของ Progress ให้ซื้อง่ายขึ้นทำให้ upgrade ที่แรงเกินคุ้มขึ้นอีก. ตัดสินใจ: **ไม่แนะนำ `ProgressPoints` 2->1**; ถ้าอยากใช้ 4 pts ให้พอดีให้ดู `QteMax` 3->4 (B5) หลังได้ข้อมูลคน. | upgrades sec 4, 7 |
| R8 | `EnergyPoints` 3->2: upgrades P2 และ economy P3 | **ตรงกัน** -> A3. ต่างกันที่ economy ชี้ว่าแก้แล้วยังเป็นรองกับ Tonic (20c/E vs 150c+3pts). | economy sec 3, upgrades sec 4 |
| R9 | `StarveDamage`: run-stats P4 20->30 vs economy P8 "ต้อง ~35+" | **ไม่ขัดกัน แต่เป็นคนละเป้า.** เลขคณิต: สัตว์ Hp เต็ม 120 starve 3 คืน (D2-D4) -> 30 = -90 (ไม่ตาย), 40 = -120 (ตายพอดี). 30 คือก้าวปลอดภัย (ฆ่าเฉพาะสัตว์ที่ Hp <=90 เช่นหลังไฟต์; `sloppy_30` Hp 21->1->0), 35+ คือถ้าอยากให้การละเลยตายจริง. ต้องตัดสินว่าอยากให้ neglect ตายหรือไม่ (B6). | economy sec 5, `sloppy_30.jsonl` |
| R10 | Toothless: run-stats P1 (ลด HP/ATK), P2 (safety net), economy P4 (ให้ revive เมื่อมี coin) | **ทั้ง 3 แก้ 12 เหตุการณ์เดียวกัน (overlap).** ลำดับที่แนะนำ: ปรับตัวเลข (A2) ก่อน เพราะถูกที่สุด วัดได้ด้วย bot; แล้วค่อยตัดสินเรื่อง safety net/revive (B3-B4) เพราะเป็นการเปลี่ยนกฎ. | 12/12 game over อยู่ Toothless D2, dealt 60-115 / 120, taken 105-120 |
| R11 | Tonic ไม่จำกัด: มีเฉพาะ economy P1 | **ยืนยันจากโค้ด** (`BaseMenus.cs` ShopScene: `_buy[1].Enabled = _gs.Coin >= Items[1].Price` ไม่มี cap/stock, `AddEnergy(2,"tonic")`). **ไม่มีข้อมูล bot ยืนยัน** (bot ซื้อครั้งเดียว 19/19 human, 40/40 upgrade-first). คำนวณ: coin ก่อน merchant 250-550 (med 350) -> 6-13 ขวด = +12 ถึง +26 Energy ใน D3. | economy sec 3 |
| R12 | Sea Tea: upgrades P5 vs economy P5 | **ตรงกัน**: ผลมีแต่ที่ Big Z (ชนะไม่ได้) และร้านเปิดหลัง Toothless; ผูกกับ B1. human tea dodge success 61.7% (n=17) vs 44.2% (n=21) = confounded (Refuse ต้องมีสัตว์ 2 ตัว). | upgrades sec 5 |

ตาราง mapping ID (P1..P8 ของแต่ละไฟล์ -> ข้อในรายงานนี้):

| รายงานนี้ | run-stats | economy | upgrades |
|---|---|---|---|
| A1 Tonic cap | - | P1 | - |
| A2 Toothless | P1 (P2 -> B4) | P4 (-> B3) | - |
| A3 EnergyPoints | - | P3 | P2 |
| A4 StarveDamage | P4 | P8 | - |
| A5 Care mechanics | P3 | P6 (UI Heal) | P3 (Train shrink) |
| B1 Boss | P6 | - | P5 |
| B2 Merchant/sink | P5 | P2, P7 | - |
| B3-B4 Revive/Doctor/safety net | P2, P5 | P4 | - |
| B5 Point economy | - | - | P1, P4 |
| C* tooling | P7 | limitations | P6 |

## 4. Proposals เรียงตาม impact x confidence

หมายเหตุ: ราคาใน `BaseMenus.cs` (shop/upgrade) และค่าใน `CareScenes.cs` ยังไม่ใช่ `Balance` (known deviation) -> ต้องย้ายเข้า `Balance` ก่อนปรับ. ค่าที่เสนอทั้งหมดคือ **ค่าเริ่มทดลอง** ไม่ใช่ค่าที่ optimize แล้ว.

### กลุ่ม A) ปรับตัวเลขได้เลย

| # | constant: ปัจจุบัน -> เสนอ | evidence | conf | risk / side-effects | re-measure |
|---|---|---|---|---|---|
| **A1** | Tonic: เพิ่ม `Balance.TonicMaxPerDay = 1` (หรือ stock 1 ต่อการเข้าร้าน) + ย้ายราคา 40 เข้า `Balance`; ถ้าไม่ cap ให้ขึ้นราคา 40 -> 80 | โค้ด `BaseMenus.cs` ไม่มี cap; coin ก่อน merchant med 350 -> 6-13 ขวด = +12..+26 E; Tonic 20c/E เทียบ Energy upgrade 150c+3pts; bot ซื้อครั้งเดียวจึงไม่เห็นใน telemetry | สูงจากโค้ด / ไม่มี data ผล | เปลี่ยนกฎเล็กน้อย (cap) ไม่ใช่ตัวเลขล้วน; ผู้เล่นที่อยากอัด Energy จะรู้สึกถูกจำกัด; ทำให้ Energy upgrade (A3) มีเหตุผลมากขึ้น | เพิ่ม bot profile `tonic-stack` (C4) วัด playerLevel/points/exp ใน D3-D4 ก่อน/หลัง cap |
| **A2** | `ToothlessAtk` 15 -> 12 (ตัวหลัก); สำรอง `ToothlessHp` 120 -> 105 (run-stats เสนอ 100) | game over 12/200, ทั้งหมดที่ Toothless: dmgTaken 105-120 (= Hp สัตว์ ~100-107), dmgDealt 60-115/120 (8/12 ตีไป >=100). ลด ATK 20% -> dmgTaken ~84-96 สัตว์ที่ Hp ~100 น่ารอด; perfect/caring/upgrade-first ไม่โดนอะไรอยู่แล้ว (taken 0) | กลาง (เลขคณิตจากเหตุการณ์ 12 ครั้ง ยังไม่ได้ simulate; human ตายจริงแค่ 2) | ลดแรงกดดัน Day 2 สำหรับคนเก่ง (ซึ่งไม่โดนอยู่แล้ว); ไม่แก้ปัญหา "สัตว์ตัวเดียว = game over" (B4) | `run-batch.ps1 -Seeds 40 -Profiles sloppy,human` ใหม่; เป้า: game over sloppy 25% -> <=10%, human 5% -> <=2%; perfect ชนะ 40/40 ต้องไม่เปลี่ยน; ดู fight rounds (เดิม 6.6-6.8) |
| **A3** | `EnergyPoints` 3 -> 2 (ตัวเลือก: `MaxEnergyCap` 6 -> 4 เพราะ lv2/3 ถึงไม่ได้) | ซื้อได้เร็วสุด D3 -> ได้ +2 sessions (<=20 exp) ทั้ง slice; Points สูงสุด 4; human point-limited 106/154 snapshot; ผู้ซื้อ Energy ทุกคน (8) เหลือ 1 pt ทิ้ง | กลาง (กลไกชัด, ผลต่อ outcome ไม่มี) | ถ้า cap Tonic (A1) Energy upgrade จะสำคัญขึ้น; ถ้าไม่ cap ก็ยังแพ้ Tonic; D5 ไม่ใช้ energy ใน bot จึงประเมินประโยชน์ D5 ไม่ได้ | human-bot upgOrder Energy-first: ดู energyUsed D2-D4, playerLevel, `pointsLeft`; ต้องมีคนจริงยืนยันว่าอยากซื้อ |
| **A4** | `StarveDamage` 20 -> 30 (35+ ถ้าจะให้ neglect ตาย) | perfect starve 3/3 คืน (-60 Hp) แต่ถึงบอสได้ 40/40; ตายจาก starve 1/200; caring (Feed/Clean) bigz 663 vs perfect 388 (+71%) | ต่ำ-กลาง (route เดียวต่อ profile) | ต้องให้ทีมตัดสินว่าอยากให้ neglect ตายไหม (B6); ถ้า Day 2 สัตว์บาดเจ็บแล้วถูก starve จะตาย (เช่น `sloppy_30`) | `-Profiles perfect,caring,human`: ดู starveEvents, petDeaths_d3/d4, Hp ตอนเข้าบอส |
| **A5** | ปรับกลไก care ที่ยากเกิน: ย้ายเข้า `Balance` แล้วลอง `CleanZoneSpeed` 4.8 -> ~4.0, ลด `HealShrink`; (เดิม run-stats เสนอขยาย zone แต่ zone อื่นกว้างกว่า Train อยู่แล้ว) | human Miss: Heal 17.9%, Clean 15.9%, Feed 10.5% vs Train 4.0%; sloppy Heal 34.2% | ต่ำ-กลาง (ความยากของ bot อาจไม่ตรงกับคน; bot "ไม่ลังเลแต่กดพลาดตาม skill") | เปลี่ยนความรู้สึกของ care ที่คนจริงอาจชอบอยู่แล้ว; Heal ใช้ HP เต็มเกือบตลอด (72/86 ครั้ง hpDelta=0) จึงอาจแก้ที่ UI แทน | รอ telemetry คนจริงก่อน; ดู Miss% ต่อ care ของ human |

ลำดับ A (impact x conf): A1 > A2 > A3 > A4 > A5.

### กลุ่ม B) ต้องการการตัดสินใจดีไซน์จากทีม

| # | ประเด็น / constant | evidence | conf | ตัวเลือก + risk | re-measure |
|---|---|---|---|---|---|
| **B1** | **Big Z `BigZHp = 9999` ชนะไม่ได้ -> Day 5, upgrades, Sea Tea/Apple วัดไม่ได้** | 0/187; dmg max 2956 (30%), human median 150.5 = 1.5% | สูง (วัดได้), กลางสำหรับค่าใหม่ | (a) ชนะ = ทนครบ N rounds / dmg ถึงเกณฑ์ (ตั้งเกณฑ์ใหม่ ไม่ใช่ 400 ตาม R3); (b) ลด `BigZHp` ให้ชนะได้เมื่อ build ดี; (c) คงเดิมเป็น "ยังไม่จบ" แล้วยอมรับว่า Day 5 วัดไม่ได้. ถ้าเปลี่ยน outcome จะแปรตาม skill/สัตว์ที่รอด (ตอนนี้ pet ตายทุก run) | `-Profiles perfect,caring,human,upgrade-first`: ดู % ผ่าน, สัตว์ตาย, bigzDmg ต่อ build |
| **B2** | **`MerchantOffer = 5000` ไม่มี sink** (+ `DailyCoin` ช่วยให้เงินล้น 49% ในทาง Refuse) | final coin 5750 ค้าง 100% ใน perfect/caring, sloppy ส่วนใหญ่; spent 0; human sell 5539 vs refuse 365; shop ไม่เปิดหลัง Sell | สูง (ข้อเท็จจริง) / กลาง-ต่ำ (ค่า) | (a) ลดเหลือ ~1000 (ทดลอง; run-stats 1500, economy 600-1000 = R6); (b) เพิ่ม sink (ราคา upgrade/ร้านที่เปิดหลัง Sell); (c) ปล่อยเป็น "ตัวเลือกเรื่องราว". `DailyCoin` 100 -> 50-75 ไม่เร่งด่วน | ต้องมี counterfactual force-sell/force-refuse (C2) ก่อนวัดว่าใครได้เปรียบ |
| **B3** | **Doctor/Revive ใช้ไม่ได้ใน slice** (`ReviveCost = 250`, revive 0/200; `DayEvents.cs` L52 ไม่ตรวจ coin) | game over 11/12 มี coin 250 พอดีแต่ไม่ได้โอกาส revive; NightScene ตรวจ coin | สูง (ข้อเท็จจริง) | (a) Toothless แพ้ + coin >= `ReviveCost` -> ไปที่ Doctor แทน GameOver (ลด game over ได้ถึง 11/12 ของที่เกิดขึ้น); (b) คงเดิม (Doctor ไว้ใช้หลัง slice). ผลข้างเคียง: ลดน้ำหนักของการตัดสินใจ tame/chase | แก้ AutoPlay ให้ revive จริง (ปัจจุบัน Esc ออก); วัด revives, game over% |
| **B4** | **Day 2 สัตว์ตัวเดียว = game over** (single point of failure) | 12/12 game over อยู่จุดนี้; tame -> human 2/20 แพ้, chase 0/20; ไม่มี fallback | กลาง (human เพียง 2 เหตุการณ์) | (a) ปรับ Toothless (A2); (b) Chase เป็นทางเลือกเริ่มต้นที่ปลอดภัย (ไม่ต้อง fight); (c) safety net ชนะ/แพ้ครั้งแรก -> Hp 1 + Doctor ฟรี (เปลี่ยนกฎ); (d) ให้สัตว์ตัวที่ 2 ตั้งแต่ Day 1 | sloppy/human game over%, สัดส่วน chase vs tame |
| **B5** | **Point budget: ได้ 4 pts, tree ต้อง 10 (ชุดที่ซื้อได้: QQQ / QQ+P / Q+E / P+P); Energy lv2/3 ไม่มีทางถึง; Progress แรงเกินราคา; Sea Tea/Apple ไม่มีเป้า** | upgrades sec 2, 4, 6; QQQ ทิ้ง 1 pt 4/18 | กลาง | `QteMax` 3 -> 4 หรือเพิ่ม points/level; ลดตัวคูณ Progress 0.5 -> 0.35 (ย้าย `TrainGainMultiplier` เข้า `Balance`); ไม่แนะนำ `ProgressPoints` 2->1 (R7). QTE upgrade สุทธิ ~+23% ไม่ใช่ +60% เพราะ pet-level shrink (1.6/1.3) -> ลด shrink 0.15 -> 0.10 ใน `TrainScale` | ต้อง human telemetry (bot Perfect 100% วัด zone ไม่ได้) |
| **B6** | **แรงกดดันการดูแล** (Stomach/Clean มีผลแค่ threshold ATK x0.75 / MaxHp x0.8; starve ฆ่าไม่ได้) | upkeep ~19% ของ energy (2.3E/12E); caring เข้าบอส Hp 100%/ATK 40 vs perfect Hp 75%/ATK 34 | กลาง | ถ้าอยากให้ Feed/Clean มีความหมายจริง: A4 + ปรับ `DailyStomachDecay`/`DailyCleanDecay`; ถ้าไม่ ก็ไม่ต้องแก้ | caring vs perfect vs human: Hp/ATK ตอนเข้าบอส, starveEvents |

### กลุ่ม C) แก้เครื่องมือ/ช่องว่างข้อมูล

| # | งาน | ทำไม | หลักฐาน |
|---|---|---|---|
| C1 | **เก็บ telemetry คนจริง (mode=play)** | ปัจจุบัน 0 run; ทุกข้อเสนอด้านบนยังไม่ validated | validation.md bot-ness flags (gap ระหว่าง QTE median 0.38 s, tSim/tReal = 60 เสมอ) |
| C2 | เพิ่ม flag force-sell / force-refuse ใน `run-batch.ps1` + AutoPlay (seed เดียวกัน) | sell vs refuse ปนกับความต่างของ bot (perfect/caring/sloppy ขายเสมอ, upgrade-first ปฏิเสธเสมอ) แยกผลไม่ได้ | economy sec 2, 8 |
| C3 | bot ที่ "พลาด" attack/dodge จริง + log attack Miss ใน `fight_press` | attack Miss = 0 ทุกที่, dodge Perfect 100%; `too_slow` เป็นสาเหตุเสีย Hp 80%+ แม้ bot เก่ง (อาจเป็นข้อจำกัด bot) | run-stats F8, sec 3 |
| C4 | bot profile `tonic-stack` (ซื้อ Tonic ซ้ำจน coin หมด) | วัดผล A1 ได้; ตอนนี้ไม่มี data | economy R11 |
| C5 | bot ที่ใช้ Energy วัน 5 + ลังเล/idle (pacing คน) + fixed-route A/B ต่อ upgrade (QTE0 vs x3, +-Tea, Refuse เท่ากัน) | d5 energy ไม่เคยถูกใช้ (0/190); route เดียวต่อ profile ทำให้ confound | economy sec 0, upgrades P6/sec 8 |
| C6 | AutoPlay revive จริงที่ Doctor; เพิ่ม ShotRunner entry สำหรับ GameOver/Night/PetPick ถ้าแก้ B3/B4 | AutoPlay เข้า Doctor แล้ว Esc เสมอ | CLAUDE.md dev tools coupling |
| C7 | ย้ายค่าใน `BaseMenus.cs` (ราคา upgrade/shop), `CareScenes.cs` (zone/speed), `TrainGainMultiplier`, `0.15f` TrainScale เข้า `Balance` | ต้องทำก่อน A1/A5/B5 | CLAUDE.md known deviations |
| C8 | อัปเดต `validate.ps1` ให้รับ run จริง (outcome `quit`, mode=play, to_be_continued ไม่ต้องครบ day_end) และแยก profile key | invariants เขียนสำหรับ bot; real run profile ชื่อ `human` ชนกับ bot `human` ใน `Aggregate` (group by profile) | `Aggregate/Program.cs` L69,L123 |

## 5. ขั้นตอนต่อไป: เก็บข้อมูลผู้เล่นจริง

**ตรวจสอบที่ทำใน report นี้** ดูส่วนท้าย. ด้านล่างคือคำสั่งที่ใช้ได้จริง (รันจาก repo root `D:\GitHub\CoProjectBepal`, PowerShell 7).

1. **ให้ playtester รันเกมพร้อม telemetry** (ไม่มี `--autoplay` -> mode=play, profile=`human`):
   ```powershell
   dotnet run --project BEPAL/Bepal_Game/Bepal -- --telemetry C:\temp\bepal_tel
   ```
   หรือ build แจกเป็นโฟลเดอร์ แล้วให้รัน `Bepal.exe --telemetry telemetry` (ไฟล์เขียนเมื่อเข้า base ครั้งแรก; ปิดเกมกลางคัน = `outcome=quit` ถูกเขียนใน `OnExiting`/`ProcessExit`).
2. ให้ผู้เล่นส่ง `run_<yyyyMMdd_HHmmss>_human_<seed>.jsonl` ทุกไฟล์ในโฟลเดอร์ (1 ไฟล์/run) พร้อมระบุว่าเล่นกี่ครั้ง/เคยเล่นเกมแนวนี้ไหม (ข้อมูลที่ telemetry ไม่เก็บ).
3. วางไฟล์ใน **โฟลเดอร์แยก** เพื่อไม่ปนกับ bot data (profile `human` ชนกัน):
   ```powershell
   New-Item -ItemType Directory -Force BEPAL/Docs/Balance-play/data/raw | Out-Null
   Copy-Item <ไฟล์ที่ได้รับ>\*.jsonl BEPAL/Docs/Balance-play/data/raw/
   Copy-Item BEPAL/Docs/Balance/tools BEPAL/Docs/Balance-play/tools -Recurse -Exclude bin,obj
   ```
4. Aggregate -> `runs.csv` + `summary.md`:
   ```powershell
   dotnet run --project BEPAL/Docs/Balance-play/tools/Aggregate -v q -- BEPAL/Docs/Balance-play/data/raw BEPAL/Docs/Balance-play/data
   ```
5. ตรวจความสมบูรณ์ + วิเคราะห์ (สคริปต์อ่าน `../data/raw` ของตัวเอง; ผลเขียนลง `Balance-play/data` และ `Balance-play/analysis`):
   ```powershell
   New-Item -ItemType Directory -Force BEPAL/Docs/Balance-play/analysis | Out-Null
   pwsh -File BEPAL/Docs/Balance-play/tools/validate.ps1          # ดู C8: อาจ FAIL บางข้อกับ run quit/ไม่ครบ
   pwsh -File BEPAL/Docs/Balance-play/tools/run-stats.ps1 > BEPAL/Docs/Balance-play/analysis/run-stats-data.txt
   pwsh -File BEPAL/Docs/Balance-play/tools/economy-energy.ps1  > BEPAL/Docs/Balance-play/analysis/economy-energy-data.txt
   pwsh -File BEPAL/Docs/Balance-play/tools/economy-energy2.ps1 > BEPAL/Docs/Balance-play/analysis/economy-energy-data2.txt
   pwsh -File BEPAL/Docs/Balance-play/tools/upgrades.ps1
   pwsh -File BEPAL/Docs/Balance-play/tools/upgrades-report.ps1
   ```
   (สคริปต์ run-stats/economy/upgrades อ้าง profile ทั้ง 5 ชื่อ; กับข้อมูลคนจริงให้ดู profile `human` เท่านั้น และถือเป็น sample ใหม่แยกจาก bot.)
6. **วัดซ้ำหลังแก้ค่า (bot):**
   ```powershell
   pwsh BEPAL/Docs/Balance/tools/run-batch.ps1 -Seeds 40 -Profiles perfect,sloppy,upgrade-first,caring,human -OutDir BEPAL/Docs/Balance/data/raw_v2
   ```
   (เก็บใน dir ใหม่เพื่อเทียบกับ `data/raw`; Aggregate ท้าย run-batch จะเขียน `runs.csv`/`summary.md` ที่ parent ของ OutDir -> ระวังทับไฟล์เดิม ให้ใช้ `-NoAggregate` แล้วรัน Aggregate เองตามข้อ 4 ไปยัง out dir อื่น).
7. เมื่อมีคนจริง >= 15-20 runs: เทียบ Miss% ต่อ care, เวลา QTE, % game over Day 2, energy ที่ใช้, การเลือก tame/chase และ sell/refuse, ซื้ออะไรเมื่อไร แล้วค่อยปรับ A2-A5.

## 6. สิ่งที่ตรวจเอง (spot-check)

คำนวณใหม่ด้วย PowerShell จาก `data/runs.csv` และ `data/raw/*.jsonl` และอ่านโค้ด:

1. mean/median/p10/p90 ของ `tSimSec`, `pressesTotal`, `finalCoin`, `energyUsedTotal`, `playerLevel`, `pointsLeft` ทั้ง 5 profile = ตรง `summary.md` (human `tSimSec` 278.12/288.51/186.85/356.28).
2. Outcome: perfect/caring/upgrade-first TBC 40; human TBC 38 + game over 2; sloppy TBC 30 + game over 10 (deathDay 2 = 10, 4 = 1 starve); human deathDay 2 = 2.
3. Boss fights = **187** ไม่ใช่ 195; bigzDmgDealt max 2956; ชนะ 0 (แก้ R2).
4. เกณฑ์ boss 400 dmg: human 4/38, perfect 17/40, caring 37/40 (ไม่ตรงกับ "~25% / ~50%" ของ run-stats P6 -> R3).
5. Game over 12 run: finalCoin 250 x11, 150 x1; ทั้ง 12 ที่ Day 2; sloppy 10 รายมี dmgTaken 105-120.
6. QTE ต่อ care human/sloppy (Train 980/2360 ... Heal 860/120) ตรง run-stats ทุกช่อง (เช่น human Heal 81.4/0.7/17.9).
7. Energy ต่อวัน: human 2.05/1.95/3.18/2.21, perfect 3/3/3/3, upgrade-first 3/3/5/3 ตรง economy.
8. Merchant human: sell 8 / refuse 30 / n/a 2; finalCoin sell 5538.75 vs refuse 365.5; bigz dmg 176.1 vs 246.0 ตรง economy; human upgrade combos (Q3 18, E1 8, Q2 5, P2 3, P1 2, Q1 1, none 3) ตรง upgrades sec 3.
9. Raw jsonl `human_25`: coin chain 150 + 600 - 365 = 385 = `finalCoin`; energy used 2/2/4/2/0 = 10; ตรง `run_summary`. Raw `sloppy_30`: สัตว์ตาย starve D4 ขณะ coin 5650 แล้วเกมไปต่อถึงบอส (ยืนยัน R5).
10. โค้ด: `DayEvents.cs` L52 `GameOverScene` ไม่ตรวจ coin vs `MenuScenes.cs` L162 ตรวจ `Coin < Balance.ReviveCost`; `BaseMenus.cs` ShopScene Tonic ไม่มี cap/stock; `PlayerMaxExp = 10 + (PlayerLevel - 1) * 10` (`GameState.cs` L156); `CareScenes.cs` zone Train 0.07/0.16 vs อื่น 0.12/0.27; ค่า `Balance` (`MerchantOffer` 5000, `ReviveCost` 250, `StarveDamage` 20, `ToothlessHp/Atk` 120/15, `BigZHp` 9999, `EnergyPoints` 3 ฯลฯ) ตรงกับที่อ้าง.
