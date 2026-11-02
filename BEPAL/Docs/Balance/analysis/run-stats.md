# Run stats analysis (Q-20261009-run-telemetry)

ที่มา: `data/raw/*.jsonl` 200 runs (bot ทั้งหมด, mode=autoplay, 40 seeds x 5 profiles) สร้างตัวเลขด้วย `tools/run-stats.ps1` (รันซ้ำได้: `pwsh -File tools/run-stats.ps1`). validation: ผ่านครบ 15 invariants.

**ข้อควรระวัง (จาก validation.md / spec sec 5):** ทุก run เป็น bot ไม่ใช่คน. เวลา = sim time เท่านั้น. `perfect`/`caring`/`upgrade-first` เป็น route เดียว (1 distinct route/40 runs) จึงแปรผันเฉพาะ wheel RNG. `human` = bot ที่สุ่ม skill 0.55-1.0, jitter, appetite, ลำดับอัปเกรด (40 routes) ใช้เป็นการกระจายหลัก; `perfect` = เพดาน; `sloppy` = พื้น (low skill, ไม่มี bot_params). bot กด Perfect ได้เกือบ 100% / ไม่ลังเล จึงไม่แทนความช้าของคนจริง. กด attack ของ bot ได้ผล Great 100% (perfect/caring/upgrade-first) = พฤติกรรม bot ไม่ใช่ค่าสมดุล.

## 1. ผลลัพธ์ (outcome) ต่อ profile

| profile | n | to_be_continued | game_over | หมายเหตุ |
|---|---:|---:|---:|---|
| perfect | 40 | 40 | 0 | เพดาน |
| caring | 40 | 40 | 0 | |
| upgrade-first | 40 | 40 | 0 | |
| human (หลัก) | 40 | 38 | 2 (5%) | |
| sloppy (พื้น) | 40 | 30 | 10 (25%) | |

- "ชนะ" = ถึง `to_be_continued` ได้เสมอเมื่อรอดถึง Day 5. Big Z มี Hp 9999 (`Balance.BigZHp`) -> **ชนะบอสไม่ได้ 0/195 fights** (ตั้งใจ; fight จบเมื่อสัตว์ตาย). ดังนั้นความ "ยาก" วัดได้ที่ Day 2 Toothless เท่านั้น.
- game over ทั้ง 12 ครั้งเกิดที่ **Day 2, FightScene (Toothless), ขณะมีสัตว์ตัวเดียว** (ไม่มี pet_added, ยังไม่ผ่านเหตุการณ์ใดที่จะได้ตัวที่ 2). ไม่มี game over ที่ Day 3-5 เลย; ไม่มีการ revive (0).

## 2. ระยะเวลา (sim s)

| profile | mean | median | p10 | p90 | min | max |
|---|---:|---:|---:|---:|---:|---:|
| perfect | 200.8 | 200.0 | 188.9 | 213.7 | 186.2 | 222.7 |
| caring | 243.1 | 241.8 | 229.8 | 259.3 | 223.7 | 271.6 |
| upgrade-first | 301.6 | 296.6 | 260.4 | 350.4 | 242.4 | 389.7 |
| human | 278.1 | 288.5 | 186.9 | 356.3 | 94.8 | 481.3 |
| sloppy | 276.7 | 316.2 | 105.2 | 369.0 | 94.0 | 419.5 |

human ที่ถึง Day 5 (n=38) ใช้ราว 3-6 นาที sim (median ~4.8 นาที); bot ไม่เดินช้า/ไม่ลังเลจึงเป็น **ขอบล่าง** ของเวลาคนจริง (ยังไม่มีข้อมูล mode=play).

### เวลาต่อวัน (mean sim s) และสัดส่วนกิจกรรม

| profile | D1 | D2 | D3 | D4 | D5 | QTE+care select | Fight | อื่นๆ (เดิน/บทสนทนา/เมนู) |
|---|---:|---:|---:|---:|---:|---:|---:|---:|
| perfect | 22.7 | 55.6 | 40.3 | 40.0 | 42.3 | 56.0 (28%) | 43.5 (22%) | 101.3 (50%) |
| caring | 22.7 | 60.2 | 55.6 | 51.0 | 53.6 | 84.4 (35%) | 55.0 (23%) | 103.7 (43%) |
| upgrade-first | 28.3 | 61.1 | 50.9 | 40.2 | 121.1 | 65.7 (22%) | 122.4 (41%) | 113.5 (38%) |
| human | 41.8 | 67.4 | 75.3* | 59.7* | 42.9* | 129.2 (46%) | 39.3 (14%) | 109.6 (39%) |
| sloppy | 53.0 | 94.0 | 74.4* | 71.2* | 27.4* | 147.0 (53%) | 45.4 (16%) | 84.3 (30%) |

\* เฉลี่ยเฉพาะ run ที่ถึงวันนั้น (human 38, sloppy 30). QTE = เวลาจาก care_select ok ถึง qte_end.
- ยิ่ง skill ต่ำ ยิ่งใช้เวลากับ QTE มาก (เพราะ Miss ต้องรอ needle วนอีกรอบ): perfect 56 s vs sloppy 147 s (x2.6). Day 5 ยาวใน upgrade-first (121 s) เพราะบอสทนนานขึ้น (ดู sec 5).
- "อื่นๆ" ~100 s ค่อนข้างคงที่ = โครง base/hub + บทสนทนา.

## 3. จำนวนการกด และอัตรา hit

presses/run (care select + QTE + fight): perfect 169, caring 181, upgrade-first 278, human 127 (median 139, p10 59, p90 173), sloppy 125.

**Care select (กดเลือกการดูแล)** — ok ทุกครั้ง, Perfect 100% ทุก profile (bot) ยกเว้น sloppy มี `gap` 6/396 (1.5%); ไม่มี noenergy เลย. เลือกการดูแล (human): Feed 108, Heal 86, Train 98, Clean 73 ; (sloppy) Train 236 / Feed 99 / Clean 43 / Heal 12.

**QTE ต่อชนิดการดูแล (10 กด/ครั้ง)**

| profile | care | n | Perfect | Great | Miss |
|---|---|---:|---:|---:|---:|
| human | Train | 980 | 90.5% | 5.5% | 4.0% |
| human | Feed | 1080 | 89.2% | 0.4% | 10.5% |
| human | Clean | 730 | 83.7% | 0.4% | 15.9% |
| human | Heal | 860 | 81.4% | 0.7% | 17.9% |
| sloppy | Train | 2360 | 54.4% | 28.3% | 17.3% |
| sloppy | Feed | 990 | 24.8% | 51.4% | 23.7% |
| sloppy | Clean | 430 | 20.9% | 56.3% | 22.8% |
| sloppy | Heal | 120 | 23.3% | 42.5% | 34.2% |
| perfect | Train | 4800 | 83.8% | 16.2% | 0% |
| caring | Train / Feed+Clean | 3200 / 1600 | 86.0% / 100% | 14.0% / 0% | 0% |
| upgrade-first | Train | 5600 | 100% | 0% | 0% |

- สรุป QTE ทั้งหมด (22,750 กด): Perfect 82.6%, Great 12.1%, Miss 5.3%.
- **Train ง่ายกว่า Clean/Heal/Feed ใน human (Miss 4% vs 10-18%)** และ sloppy ก็คล้ายกัน (Train Miss 17% vs Heal 34%). ต้นเหตุน่าจะเป็นขนาด zone ของ Train กว้างกว่า/ความเร็ว wheel (ค่าอยู่ใน `CareScenes.cs` ไม่ใช่ `Balance` - known deviation). Perfect 100% ของ upgrade-first = QteUpgrade ทำให้ Perfect zone กว้างพอจน bot ไม่พลาดเลย (QteZoneMultiplier 1+0.2*lv; 3 upgrades -> Great 0%).

**Fight (กดต่อ fight)**

| profile | attack n | attack P/G/M | dodge n | dodge P | hurt: too_slow | hurt: miss_press |
|---|---:|---|---:|---:|---:|---:|
| human | 591 | 58% / 42% / 0 | 356 | 100% | 145 | 135 |
| sloppy | 375 | 23% / 77% / 0 | 154 | 100% | 135 | 179 |
| perfect | 861 | 0 / 100% / 0 | 592 | 100% | 187 | 42 |
| caring | 1124 | 0 / 100% / 0 | 750 | 100% | 268 | 66 |
| upgrade-first | 2631 | 0 / 100% / 0 | 2151 | 100% | 258 | 182 |

(ใน log fight_press Miss ไม่ถูกนับเป็น attack Miss; ความพลาดอยู่ที่ `fight_hurt` cause=`miss_press`/`too_slow`, และ `attackMiss` ใน fight_end.) too_slow คือสาเหตุเสียเลือดหลักในทุก profile ที่ bot เก่ง (perfect 187 vs miss_press 42 = 82%; caring 80%) => บอส/ศัตรูโจมตีเร็วกว่าที่ bot จะทัน แม้ bot ไม่ผิดพลาด (ข้อสังเกต: อาจเป็นข้อจำกัดของ bot ในการ dodge; ยืนยันด้วยคนจริงก่อนเปลี่ยนค่า).

## 4. ที่ตาย / game over และสิ่งที่สัมพันธ์

- ตำแหน่ง: 12/12 game over = Day 2 Toothless fight, สัตว์ตัวเดียว (human 2, sloppy 10). Day 3-5: 0.
- ตัวเลขใน fight ที่แพ้ (12 ครั้ง): damage taken 105-120 (Hp สัตว์เริ่มต้น ~100-120), rounds 4-7, ให้ dmg ไป 60-115 จาก Hp 120 ของ Toothless (ขาดอีก ~5-60).
- **Door choice Day 2 เป็นตัวกำหนดความเสี่ยง**: ไม่ "tame" -> ไม่มี fight. human: chase 20 / tame 20 -> game over 2/20 ของ fight (10%) และ 0/20 ของ chase. sloppy: tame 40/40 -> game over 25%.
- **Skill vs outcome (human, n=40; terciles ตาม bot_params.skill)**

| tercile | skill | n | game over | Toothless fought / won | QTE Miss เฉลี่ย | player lv | dmg ใส่บอส | energy ที่ใช้ |
|---|---|---:|---:|---|---:|---:|---:|---:|
| ต่ำ | 0.56-0.70 | 13 | 2 | 8 / 6 | 19% | 3.5 | 139 | 7.9 |
| กลาง | 0.71-0.84 | 14 | 0 | 5 / 5 | 10% | 4.1 | 211 | 9.2 |
| สูง | 0.85-0.99 | 13 | 0 | 7 / 7 | 8% | 4.3 | 331 | 10.2 |

  game over ทั้ง 2 ของ human อยู่ใน skill ต่ำสุด (0.60, 0.65), ทั้งคู่เลือก tame ตอน Day 2 ด้วยสัตว์ตัวเดียว. n เล็กมาก (2 เหตุการณ์) -> ยืนยันเชิงทิศทางเท่านั้น (ตรงกับ sloppy: skill ~0 -> 25%). Skill ยังเพิ่ม: level, energy ที่ใช้ (ต่ำ 7.9 -> สูง 10.2 จาก 15), dmg บอส (x2.4).
- Pet death ที่ไม่ใช่ game over: ทุก run ที่ถึง Day 5 เสียสัตว์ 1 ตัว (upgrade-first 2) ที่บอส = ตายตามดีไซน์.
- ความหิว: perfect (Train อย่างเดียว) ได้ starve 3 ครั้ง (-60 Hp รวม, 20/คืน) แต่ไม่มีตายก่อนบอส; เข้าบอสด้วย Hp 90/120, Clean 0 (ATK ลดจาก Clean ต่ำ). caring เข้าบอสด้วย Hp 140. human: starve เฉลี่ย 0.35/run.

## 5. Fight stats

**Toothless (Day 2, Hp 120, tame เท่านั้น)**

| profile | n | ชนะ | rounds | dmg ใส่ (mean) | dmg โดน (mean) | atkMiss | missPress | too_slow | เวลา (s) | สัตว์เริ่ม Hp/Atk |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---|
| perfect / caring / upgrade-first | 40 each | 100% | 6 | 132 | 0 | 0 | 0 | 0 | 15.0 | 120 / 30 |
| human | 20 | 18 (90%) | 6.8 (med 7) | 124.6 | 38.3 | 1.0 | 1.7 | 0.85 | 23.2 | 102.5 / 21.3 |
| sloppy | 40 | 30 (75%) | 6.6 (med 7) | 120.5 | 78.4 | 2.3 | 3.3 | 1.9 | 35.1 | 106.8 / 23.4 |

Toothless ถูกฆ่าใน ~6 rounds ถ้าตีโดนทุกที่. ผู้เล่นเก่ง (ATK 30) ไม่โดนเลย; ผู้เล่น ATK ~21-23 ใช้ 7 rounds และเสีย Hp ~38-78 (ทั้ง Hp สัตว์ ~100-107) => ระยะขอบแคบมากที่ skill ต่ำ.

**Big Z (Day 5, Hp 9999, Atk 20)** — ไม่มีใครชนะ (ตั้งใจ)

| profile | n | rounds (mean) | dmg ใส่ mean / median / max | dmg โดน | dodgeOk | too_slow | เวลา (s) | Hp สัตว์ตอนเริ่ม | สัตว์ตาย |
|---|---:|---:|---|---:|---:|---:|---:|---:|---:|
| sloppy | 29 | 3.8 | 78 / 71 / 204 | 72 | 1.2 | 2.0 | 14.1 | 61 (min 11) | 1.0 |
| human | 38 | 12.0 (med 9.5) | 231 / 151 / 1100 | 119 | 7.1 | 3.4 | 29.1 | 87 (min 40) | 1.26 |
| perfect | 40 | 15.5 | 388 / 375 / 825 | 115 | 9.8 | 4.7 | 28.5 | 90 | 1.0 |
| caring | 40 | 22.1 | 663 / 615 / 1230 | 167 | 13.8 | 6.7 | 40.0 | 140 | 1.0 |
| upgrade-first | 40 | 59.8 | 1330 / 1214 / 2956 | 218 | 48.8 | 6.5 | 107.3 | 107 (+Sea Tea 40/40) | 2.0 |

บอสถูกทำลายสูงสุด 2956/9999 (30%) แม้ใช้ upgrade-first; ค่ากลางของ human = 1.5-2.3% ของ Hp บอส. บอสไม่ใช่ตัวทดสอบความสมดุลเลย (outcome TBC แน่นอน) — ที่ต่างคือ "นานแค่ไหน/ใส่ได้เท่าไร".

## 6. เศรษฐกิจ/พลังงาน (ข้อสังเกตประกอบ)

- เหรียญ: ขายสัตว์ให้ Merchant ได้ 5000 -> perfect/caring/sloppy จบด้วย 5750 เหรียญที่ **ไม่มีที่ใช้** (spent 0). human: median final coin 382 (ไม่ขาย), 8/38 ขาย (coin 5450+). Revive 250 เหรียญไม่เคยถูกใช้ (revives 0).
- พลังงาน: human ใช้ 9.1 (median 10), perfect/caring ใช้ 12 (เหลือ 3 รวม), upgrade-first 14. Skill ต่ำ/appetite ต่ำปล่อยพลังงานทิ้ง -> พลังงานไม่ใช่ข้อจำกัดหลัก; ข้อจำกัดคือเวลา/ความตั้งใจของ bot.
- Exp: PlayerMaxExp 10+(L-1)*10; level 5 ที่ 12 ครั้ง QTE; ผู้เล่นที่ Miss มากขึ้นช้ากว่า (human lv 3.98 vs 5).

## 7. Findings

| # | finding | evidence (profile, n) | confidence |
|---|---|---|---|
| F1 | ความเสี่ยง fail จริงมีจุดเดียว: Toothless Day 2 ด้วยสัตว์ตัวเดียว. game over 5% (human), 25% (sloppy), 0% (skill สูง) | human n=40, sloppy n=40, perfect/caring/upgrade-first n=120 | กลาง (human เพียง 2 เหตุการณ์) |
| F2 | การเลือก tame/chase คือการตัดสินใจเสี่ยง: ถ้า tame ด้วย skill ต่ำ ไม่มี fallback (ตายแล้ว game over ทันที) | human tame 20 -> 2 แพ้; chase 20 -> 0 | กลาง |
| F3 | Big Z (Hp 9999) ชนะไม่ได้ (0/195); ฝั่ง "สัตว์ตายที่บอส" ทุก run -> ปลายเกม outcome ไม่ขึ้นกับ skill | ทุก profile | สูง |
| F4 | skill ต่ำ ~2.6x เวลา QTE และ Miss 18-34% ใน Clean/Heal; Train ง่ายสุด (Miss 4% human) -> care ไม่สมดุลกัน | human n=3650 กด, sloppy n=3900 | กลาง-สูง (ระวัง bot) |
| F5 | การขาย pet 5000 ทำให้เหรียญล้นและไร้ sink; ReviveCost 250 ไม่เคยถูกใช้ เพราะไม่มีการตายก่อนบอส (ไม่ใช่ game over) | perfect/caring/sloppy final 5750; revives 0 | สูง |
| F6 | การละเลยการให้อาหารทำให้เข้าบอสอ่อนลง (Hp 90 vs 140; dmg บอส 388 vs 663) แต่ไม่ทำให้ตาย ก่อนบอส -> แรงจูงใจดูแลต่ำ | perfect vs caring (1 route แต่ละ n=40) | กลาง (ผล cosmetic เพราะบอสชนะไม่ได้) |
| F7 | พลังงานไม่คอขวด: human ใช้ 9.1/15 | human n=40 | กลาง |
| F8 | too_slow เป็นสาเหตุเสีย Hp หลักแม้ bot เก่ง (80%+) | perfect, caring | ต่ำ-กลาง (อาจเป็นข้อจำกัด bot) |

## 8. Proposals (ยังไม่แก้โค้ด; ค่าอยู่ใน `Model/GameState.cs` `Balance`)

| # | proposal | evidence | confidence | expected impact | constant |
|---|---|---|---|---|---|
| P1 | ลด Toothless ให้อภัยขึ้น: `ToothlessHp` 120 -> 100 (หรือ `ToothlessAtk` 15 -> 12) เพื่อให้ผู้เล่น skill ต่ำที่มีสัตว์ตัวเดียวพอรอด | game over sloppy 25% / human 10% ของ fight; ตอนแพ้ขาด dmg ใส่อีก ~5-60 จาก 120, taken 105-120 ~= Hp สัตว์ | กลาง (ใช้ sloppy เป็นหลัก; ตรวจซ้ำด้วยคนจริง) | คาดว่า game over Day 2 ของ sloppy ลดจาก 25% เหลือ ~10%; human ~5% -> ~2% ; Toothless จะ 5 rounds แทน 6 | `ToothlessHp`, `ToothlessAtk` |
| P2 | หรือเพิ่ม safety net: ให้ Toothless fight แพ้ไม่ใช่ game over ในครั้งแรก (ฟื้นด้วย Hp 1 + Doctor ฟรี) -- เป็นการเปลี่ยนกฎ ไม่ใช่ค่าเดียว | game over ทั้ง 12 อยู่ Day 2 เท่านั้น | ต่ำ-กลาง | game over -> ~0; แต่ลดน้ำหนักของ tame/chase | (ไม่มีค่าที่ตรงใน Balance; ต้องแก้ DayEvents/GameState) |
| P3 | ทำให้ Clean/Heal/Feed ไม่ยากกว่า Train: ขยาย zone ของ care ที่ไม่ใช่ Train (ปัจจุบัน Miss 10-18% vs 4%) หรือชดเชย reward | human/sloppy ต่อ care (รายละเอียด sec 3) | กลาง (ค่าอยู่ใน `CareScenes.cs` ไม่ใช่ Balance -- ต้องย้ายเข้า `Balance` ก่อน) | Miss ของ Clean/Heal ลดเหลือ ~6-8% ใน human; ลดเวลา QTE ~ 20% | (ใหม่) `Balance.CareZoneHalfWidth*` ย้ายมาจาก `CareScenes.cs` |
| P4 | เพิ่มแรงกดดันการดูแล: `StarveDamage` 20 -> 30 (ไม่ฆ่าก่อน Day 5 ถ้าไม่ละเลยเกิน 3 คืน; ที่ 20 perfect เสียแค่ 60 Hp และถึงบอสได้ 100%) | perfect (Train only) ถึงบอส 40/40 ด้วย Hp 90, starve ไม่ตายสักครั้ง | ต่ำ-กลาง (ทดสอบ route เดียว; ต้องดูว่าอยากให้การละเลยตายได้หรือไม่) | perfect จะเข้าบอสด้วย Hp ~60/120 และ pet ที่ไม่เคยให้อาหารเสี่ยงตายคืน Day 4; ช่วยให้ Feed มีความหมาย | `StarveDamage` (และ `DailyStomachDecay` 20 เป็นตัวเลื่อนวันที่เริ่มอดอยาก) |
| P5 | แก้เศรษฐกิจ: ลด `MerchantOffer` 5000 -> ~1500 หรือเพิ่ม sink ที่คุ้ม (ราคาอัปเกรด/ร้านค้า) เพื่อให้ขายแล้วไม่ล้นเหรียญ; ReviveCost 250 ใช้ไม่ได้จนกว่าจะมีตายก่อนบอส | final coin 5750 ไม่ใช้ใน 120 runs; revives 0 | สูง (ข้อเท็จจริง) / กลาง (ค่าที่เหมาะ) | ตัวเลือก sell/refuse ที่ Day 3 จะเป็นการแลกเปลี่ยนจริง (ตอนนี้ขาย = ได้เหรียญเปล่า, ปฏิเสธ = ได้ pet) | `MerchantOffer`, `ReviveCost` |
| P6 | ถ้าต้องการให้บอสวัดผลได้ใน slice: ให้ "ชนะ" = ทนครบ N rounds หรือทำ dmg ถึงเกณฑ์ (human mean 231, perfect 388, upgrade-first 1330, max 2956) แทน Hp 9999 | บอส 0/195 ชนะ; ใส่ได้ 0.8%-30% | สูง (วัดได้), กลาง (เป็นการเปลี่ยนดีไซน์) | outcome ของวันสุดท้ายแปรตาม skill; ตั้งเกณฑ์ ~400 ให้ human ~25% / perfect ~50% ผ่าน | `BigZHp` (หรือเพิ่ม `BigZSurviveRounds`) |
| P7 | เก็บ telemetry เพิ่ม: run จริง mode=play (ไม่มีเลย), เวลาที่ผู้เล่นใช้ในแต่ละ scene, fight attack Miss แยก (ตอนนี้ไม่มี attack Miss ใน fight_press) | ข้อจำกัดข้อมูล bot | สูง | ปิดช่องว่างระหว่าง bot กับคน ก่อนปรับ P1/P3/P4 | (ไม่มี) |

หมายเหตุลำดับความสำคัญ: P5 (เศรษฐกิจ) และ P3 (care zones) มั่นใจที่สุดว่าเห็นผลจริง; P1/P4 ควร re-run `run-batch.ps1` หลังแก้ แล้วดู game over ของ `sloppy` + `human` อีกรอบ; P6 ต้องให้ทีมตัดสินใจดีไซน์.
