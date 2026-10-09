# Upgrade analysis (Q-20261009-run-telemetry)

ข้อมูล: 200 BOT runs (40 seed x perfect/sloppy/upgrade-first/caring/human), build dab9037. Scripts: `tools/upgrades.ps1` (parse jsonl -> `analysis/upgrades-data/*.csv`) + `tools/upgrades-report.ps1` (ตาราง -> `upgrades-data/report.txt`). ตัวเลขด้านล่างมาจาก report.txt.

**ข้อควรระวัง:** ทุกอย่างเป็น bot ไม่ใช่คนจริง. มีแค่ `human` (priority สุ่ม) กับ `upgrade-first` ที่ซื้อ upgrade; `perfect/caring/sloppy` ซื้อ 0/120 runs. `upgrade-first` ซื้อ QTE x3 อย่างเดียวและ Refuse พ่อค้าเสมอ (confound กับจำนวนสัตว์ 2 ตัว). `human` n=40 แต่กลุ่มย่อยเล็ก (Energy n=8, Progress n=5, Progress x2 n=3).

## 1. กฎของ upgrade (จาก code)

| Upgrade | Coin | Point | Max | ผล | Balance const |
|---|---:|---:|---:|---|---|
| QTE | 100 | 1 | 3 | Train zone x(1+0.2L) **เฉพาะ Train** (Perfect 0.07 rad, Great 0.16 rad; หารด้วย 1+0.15*(min(petLv,3)-1)) | `QteCoin/QtePoints/QteMax` |
| Energy | 150 | 3 | 3 (cap 6) | MaxEnergy +1 และ +1 Energy ทันที | `EnergyCoin/EnergyPoints`, `MaxEnergyCap` |
| Progress | 20 | 2 | 2 | Train progress/press x(1+0.5L): Perfect 10->15->20 | `ProgressCoin/ProgressPoints/ProgressMax` (ตัวคูณ 0.5 hard-code ใน `GameState.TrainGainMultiplier`) |
| Sea Tea 18c | | | | Dodge start 0.32 -> 0.38 ไฟต์ถัดไป | `DodgeStartSeaTea` |
| Tonic 40c | | | | +2 Energy วันนั้น | hard-code ใน BaseMenus.cs |
| Crab Apple 25c | | | | +18 HP +20 Stomach 1 ตัว | hard-code ใน BaseMenus.cs |

ราคารวม upgrade ทั้งหมด = 490c / **10 point**. ร้านเข้าได้ครั้งเดียว (Day 3 หลัง Refuse พ่อค้า), ซื้อครบ 3 ชิ้น = 83c.

## 2. Budget: Point คือคอขวด ไม่ใช่ Coin

- Point = PlayerLevel-1; EXP 1/press สำเร็จ, level k->k+1 ต้อง 10k exp. Energy 3/วัน = 30 press/วัน => `perfect/caring`: L3 ปลาย Day1 (2 pt), L4 ปลาย Day2 (3 pt), L5 ปลาย Day4 (**4 pt**). L6 ต้อง 150 exp; **plv สูงสุดที่เห็น = 5** (138/200 runs ถึง L5, ไม่มี L6) => point สูงสุดจริง = **4** เทียบราคารวม 10.
- Coin (ไม่ขายพ่อค้า): 150 เริ่ม + Toothless 200 (Day2) + 100/วัน = 450 หลัง Day 2, ~750 Day 5 > 490. Coin ขาดแค่ Day 1-2 (upgrade-first: Q1 Day1 เหลือ 50, Q2+Q3 Day2). ถ้าขายพ่อค้า +5000 (perfect/caring จบ 5750c ไม่มีที่ใช้) coin ไม่มีความหมาย.
- ชุดที่ซื้อได้ใน 4 pt: QQQ (เหลือ 1 pt ทิ้ง) | QQ+P | Q+E | P+P. **Energy lv2-3 (6/9 pt) ไม่มีทางถึง; QTE x3 + Energy + Progress พร้อมกันเป็นไปไม่ได้.**
- หลักฐาน (human): ซื้อ Q x3 แล้วเหลือ 1 pt ทิ้ง 4/18 runs; ผู้ซื้อ Energy ทุกตัว (8) เหลือ 1 pt (bot `patient` ไม่ fallback ซื้อ Q ด้วย — artefact บางส่วน). 2 runs (seed 27,30; Energy-first+patient) จบ plv 3 ไม่เคยถึง 3 pt -> Energy ต้อง L4 = 60 exp.

## 3. Pick rate / timing

| | human (n=40) | upgrade-first (n=40) |
|---|---|---|
| ซื้ออะไรก็ตาม | 37/40 (3 ไม่ซื้อ: ตาย Day2 1, รอ Energy ไม่ถึง L4 2) | 40/40 |
| QTE >=1 / x3 | 24 / 18 | 40 / 40 |
| Energy | 8 (20%) | 0 |
| Progress >=1 / x2 | 5 / 3 | 0 |

- จังหวะ: QTE1 ซื้อ Day1 (human 22/24; upgrade-first 40/40), Q2 Day2, Q3 Day3-4 (human) / Day2 (upgrade-first). **Energy ซื้อ Day3-4 เท่านั้น** (5 ที่ Day3, 3 ที่ Day4) -> ได้ประโยชน์ <= 2 session. Progress ซื้อ Day1 (3) / Day3 (2) สำหรับ lv1.
- pick rate ของ human เป็นผลของ `upgOrder` ที่สุ่ม ไม่ใช่ความชอบของผู้เล่น -> **อย่าตีความเป็น preference**. บอกได้แค่ความเป็นไปได้: QTE ซื้อได้เร็ว/ง่ายสุด, Energy ช้าสุด/ถึงยากสุด (Energy-first+patient สำเร็จ 8/10).

## 4. ผลของแต่ละ upgrade

### QTE zone (+20% x3): ดูดี แต่วัดไม่ได้ และผลสุทธิถูกกลบ
- Train Perfect rate: upgrade-first = **100% ที่ upgQ 0/1/2/3** (n=400/800/1200/3200) -> ceiling ของ bot วัดไม่ได้. human: 86.8% (upg0, n=500) vs 98.1/92.4/92.7% (upg1/2/3, n=160/170/150) แต่ control ที่ไม่ได้ขยาย zone (Clean/Feed/Heal) แกว่ง 84.7/86.1/87.1/82.8% ตาม upg -> noise ราว +-3-4pp. แยกตาม skill: tercile ต่ำ/กลาง +11.7/+9.6pp (n 140-220, seed 4-9) แต่ tercile สูง +0.4pp; paired within-run ทำได้แค่ n=6 และ "ก่อนซื้อ" มี 10 press/run -> ใช้ไม่ได้. **Evidence ต่ำ, confidence ต่ำ** (human).
- Geometry (คำนวณ ไม่ใช่ bot): Perfect half-width 0.07 -> 0.112 rad ที่ x3 แต่ pet Lv3+ หารด้วย 1.30 => x3 บน pet Lv3+ = 1.23x ของ zone Lv1 เท่านั้น (pet ใน upgrade-first/perfect ถึง Lv6-7 ภายใน Day 3-4). **upgrade ส่วนใหญ่แค่ชดเชย penalty ของ pet level, สุทธิ ~+23% ไม่ใช่ +60%.**
- EXP ไม่เปลี่ยน (1/press สำเร็จ) -> ช่วยเฉพาะเมื่อลด miss. คนจริง miss มากกว่า bot (human bot Miss 12%, sloppy 17%) จึงน่าจะมีค่ากับคนจริง -> **ต้อง playtest คนจริง**.
- Fight: QTE ไม่เกี่ยวกับ wheel ของ fight (code) ไม่มีผลต่อไฟต์.

### Energy (+1 max, 150c/3pt): looks good but doesn't help ใน slice
- Care ใช้ได้ Day 1-4 เท่านั้น (`energyUsed_d5`=0 ใน perfect/upgrade-first). ซื้อเร็วสุด Day3 (pt=3 หลัง Day2) ได้ +1 ทันที + 1 Day4 = **+2 session (+20 exp, ~+200 progress)**. Tonic ให้ +2 Energy ที่ Day3 ในราคา 40c/0 pt (upgrade-first Day3 ใช้ 5 ต่อ max 3 ยืนยัน). Energy lv1 แพง 3.75x Tonic + กิน 75% ของ point ทั้งหมด แต่ได้เท่ากัน.
- human ที่ซื้อ Energy: energyUsed 11.9 vs 8.4 (n=8 vs 32) แต่ skill 0.82 vs 0.76, plv 4.62 vs 3.81 (ต้อง L4 ถึงซื้อได้ = selection bias) -> ไม่ใช่ causal. BigZ dmg 186 vs 228 (ไม่ดีขึ้น, n เล็ก).
- human เหลือ Energy ~0.95/วัน โดยไม่ใช้ (appetite ต่ำ 25% ของ seed) -> Energy upgrade ไร้ค่ากับคนที่ใช้ไม่หมด; perfect/caring/sloppy/upgrade-first ใช้ 3/3 ทุกวัน.
- **Never reachable: Energy lv2, lv3** (ต้อง 6/9 pt > 4). `MaxEnergyCap=6` ไม่มีวันถึง.

### Progress (+50% x2, 20c/2pt): ถูกและแรง แต่ถูกจำกัดด้วย point; ไม่กระทบ outcome
- กลไกยืนยันจาก data: human P1 progress/session = 128.6 vs 94.5 (7.4 Perfect*15 + 2.3 Great*7.5 = 128); n=14 sessions. P2 = gain x2 (Perfect 20): 120 press ~2400 progress => pet Lv8-9 (`MaxProgress=100+50(L-1)`: สะสม Lv7=1350, Lv9=2200) เทียบ Lv6 (perfect), 6.9 (upgrade-first).
- human ที่ซื้อ P: petLvlMax 3.6 vs 2.1 แต่เทรน 4.3 vs 2.2 session -> confound, n=5, อย่าสรุป.
- Pet Lv มีผลแค่ Atk +5/Lv, Hp +10/Lv. Toothless ชนะ 40/40 (perfect/caring/upgrade-first), BigZ HP 9999 **ชนะ 0/187 ไฟต์ (by design)** -> pet level ที่สูงขึ้นไม่เปลี่ยนผลแพ้/ชนะใน slice. UI "+10 -> +15" ไม่บอกว่า lv2 = +20.

### Fight / game over
- game over 12/200 ทั้งหมดตาย **Day 2 (Toothless)**: sloppy 10 (ไม่ซื้อ upgrade), human 2 (seed 24 plv1, seed 35 plv2 ซื้อแค่ Q1). ณ จุดนั้นซื้อได้แค่ QTE1-2 ที่ไม่แตะ fight => **upgrade ไม่มีผลต่อ failure point เดียวของ slice (โดยโครงสร้าง)**.
- BigZ dmg: upgrade-first 1330 (atkHits 59.8) vs perfect 388 (15.5) vs caring 663 — ต่างเพราะ upgrade-first Refuse (2 pet) + Sea Tea + Lv6.9 ไม่ใช่ QTE; **confound สมบูรณ์ ห้ามอ้างเป็นผล upgrade**.

## 5. ร้านค้า (ซื้อทั้งหมด Day 3)

| item | human ซื้อ | upgrade-first ซื้อ |
|---|---:|---:|
| Tonic | 19 | 40 |
| Crab Apple | 16 | 40 |
| Sea Tea | 17 | 40 |

- ร้านเปิดเฉพาะเมื่อ Refuse (30/40 human); ขายพ่อค้า = ไม่มีร้าน (coin 5750 ใช้ไม่ได้).
- Sea Tea (วัดได้เฉพาะ BigZ): dodge success (ok/(ok+tooSlow+missPress)) human tea 61.7% (n=17) vs ไม่มี 44.2% (n=21); dmgTaken/dodge 7.7 vs 10.9. confound: tea ต้องผ่านการ Refuse (pet 2 ตัว) + skill/shop probability. upgrade-first (tea ทั้งหมด) 81.6% vs perfect 63.1% / caring 62.2% (bot ต่างชนิด). **แยกผล Sea Tea ไม่ได้; ทิศทางดีขึ้นแต่ confidence ต่ำ**; ผลต่อ outcome = 0 เพราะ BigZ unwinnable.
- Crab Apple: ไม่มีสัตว์ตายจากหิวใน slice (starve 20 HP/คืน) -> แทบไม่มีเป้าหมาย (ไม่ได้ตรวจ HP effect ละเอียด).

## 6. จัดประเภท

| รายการ | ประเภท | หลักฐาน |
|---|---|---|
| QTE | looks good, impact ยืนยันไม่ได้ (pet-level penalty กลบ ~x1.23) | upgrade-first ceiling 100%; control noise +-3pp |
| Energy | **looks good but doesn't help** (+2 session ทั้ง slice = Tonic 40c) + lv2/3 never reachable | Sec 2, 4 |
| Progress | **too strong per cost** แต่ไม่มีผลต่อ outcome; point เป็นตัวจำกัด | +36% (data), x2 (math) |
| Sea Tea / Apple | nobody needs (outcome ไม่เปลี่ยน/ไม่มีเป้า) | BigZ unwinnable; ไม่มี starve death |
| Point budget | 4 vs 10 -> ต้องเลือก; QQQ ทิ้ง 1 pt | Sec 2 |

## 7. ข้อเสนอ

| # | ข้อเสนอ | evidence | confidence | impact | const |
|---|---|---|---|---|---|
| P1 | ใช้ 4 pt ให้พอดี: `ProgressPoints` 2->1 (QQQ+P=4) หรือ `QteMax` 3->4 | QQQ ทิ้ง 1 pt 4/18; ผู้ซื้อ E ทิ้ง 1 pt 8/8 | กลาง | กลาง | `Balance.ProgressPoints` / `QteMax` |
| P2 | Energy ซื้อได้ Day2: `EnergyPoints` 3->2 (และ/หรือ `EnergyCoin` 150->100) หรือลด `MaxEnergyCap` เป็น 4 (เลิก lv2-3) | ซื้อได้เร็วสุด Day3 ให้ +2 session = Tonic | กลาง-สูง (กลไก) | กลาง | `Balance.EnergyPoints`, `EnergyCoin`, `MaxEnergyCap` |
| P3 | ลด pet-level penalty ของ Train zone (0.15->0.10) ให้ x3 สุทธิ ~+40% | 1.6/1.3=1.23 (คำนวณ) | กลาง (ต้อง playtest คนจริง) | กลาง | `0.15f` ใน `CareScenes.TrainScale` -> ย้ายเข้า `Balance` (เช่น `TrainLevelShrink`); `QteZoneMultiplier` 0.2 |
| P4 | ลด/ระบุ Progress: ตัวคูณ 0.5->0.35 หรือเขียน +20 ใน UI | P2 = x2 gain, pet Lv8-9 | ต่ำ-กลาง | ต่ำ (ไม่กระทบ outcome) | `0.5f` ใน `GameState.TrainGainMultiplier` -> ย้ายเข้า `Balance` |
| P5 | ให้ Sea Tea/Apple มีความหมาย: ทำให้ผล fight ผูกกับ level/อุปกรณ์ (ตอนนี้ BigZ 9999 HP unwinnable) หรือเปิดร้านก่อน Toothless (Day 2) | game over 12/12 ที่ Day2 ก่อนมีร้าน | สูง (โครงสร้าง) | สูงต่อ design | `Balance.BigZHp`, ลำดับ `DayEvents` |
| P6 | เก็บ data เพิ่ม: A/B upgrade-first ต่อ item (QTE0 vs x3, +-Tea, Refuse เท่ากัน) + bot ที่ hit window ผันตามความกว้าง zone | confound ทุกจุด | - | - | - |

## 8. ข้อจำกัด
- bot ไม่มี hesitation; Perfect 100% ของ upgrade-first วัด zone ไม่ได้.
- seed ซ้ำข้าม profile ไม่ independent; human กลุ่มย่อยเล็ก.
- ไม่มี run ที่ Refuse โดยไม่ซื้อ upgrade ในแบบเดียวกับ upgrade-first จึงแยกผล "Refuse" ออกจาก upgrade ไม่ได้.
