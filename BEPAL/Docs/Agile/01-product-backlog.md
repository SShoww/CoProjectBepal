---
type: agile-backlog
version: 4.0
date: 2026-09-28
project: BePal
---

# Product Backlog — BePal

> รวม User Story ทั้งหมดของโปรเจกต์ — ยังไม่ได้แปลว่าต้องทำใน Sprint นี้ทั้งหมด
> แบ่งงานเป็น **3 Sprint** (สิ้นสุด **4 พ.ย. 2026**) — Sprint ไหนหยิบ Story ไปทำ ให้ใส่เลข Sprint (1-3) ลงคอลัมน์ `Sprint`
> ขอบเขตและตัวเลขอ้างอิง [[06-vertical-slice|GDD 06 — Vertical Slice]]
>
> - **โชว์ (Show):** Lead Programmer
> - **ซุง (Zunk):** Flex, Audio Support
> - **ภูมิ (Pooh):** Game Design, Audio Support, UI Lead
> - **เดียร์ (Dear):** 2D Art

## Must Have (MVP)

| # | User Story | Acceptance Criteria | Estimate (SP) | Sprint |
|---|---|---|---|---|
| 1 | As a player, I want to choose 1 of 3 starter pets, so that I start with a companion I like | หน้า Choose your pet แสดง Mossling / Nibbleclaw / Blinkbun พร้อม Element, HP, ATK — hover มีกรอบเรืองแสง คลิกแล้วเข้าฐานพร้อมสัตว์ตัวนั้น | 3 | 1 |
| 2 | As a player, I want to walk around the sanctuary with A/D and interact with Space, so that the base feels like a place I live in | เดินซ้าย-ขวาได้ กล้องตามตัว เข้าใกล้ของ (เตียง, Upgrade, หมอ, สมุด, ประตู, สัตว์) แล้วขึ้นป้าย `[Space]` กด Space แล้วเปิดหน้านั้น | 5 | 1 |
| 3 | As a player, I want to pick a care action from a spinning wheel, so that choosing care feels like a skill check | วงล้อ 4 สี Train แดง / Feed เหลือง / Clean ฟ้า / Heal เขียว กด Space โดน choice ไหนเข้า QTE นั้นและหัก 1 Energy กดโดนช่องว่างไม่เกิดอะไร ESC กลับฐาน | 3 | 1 |
| 4 | As a player, I want the Train QTE, so that my pet levels up and gets stronger | จุดแดงเล็กหลายจุด เข็มเร็ว กดได้ 10 ครั้ง Perfect +10 / Great +5 Progress หลอดเต็มแล้วเลเวลขึ้น (HP +10, ATK +5) จุดเล็กลงตามเลเวลจนถึง LV 3 | 3 | 1 |
| 5 | As a player, I want Feed, Clean and Heal QTEs with their own twist, so that each care action feels different | Feed กดโดนแล้วเข็มกลับทิศ / Clean จุดวิ่งหนีเข็ม / Heal เข็มกะพริบและจุดหด — ทุกแบบกด 10 ครั้งแล้วกลับฐาน และเพิ่ม Stomach / Clean / HP ตามลำดับ | 5 | 1 |
| 6 | As a player, I want my pets' Stomach and Clean to drop every day, so that I must keep caring for them | เริ่มวันใหม่ Stomach −20, Clean −15 · Stomach = 0 เสีย 20 HP ตอน End day · Clean ≤ 50 ATK ลด · Clean ≤ 25 Max HP ลด | 3 | 1 |
| 7 | As a player, I want a limited Energy budget and to end the day at my bed, so that every care choice is a trade-off | เริ่ม 3 Energy ต่อวัน เดินไปเตียงกด Space แล้วยืนยันเพื่อจบวัน (ถ้ามี Event ค้างที่ประตูจะนอนไม่ได้) ฟ้ามืดลงแล้วขึ้นวันใหม่พร้อมสรุปผล | 3 | 1 |
| 8 | As a player, I want to earn Player EXP from QTE hits, so that I get Points to spend on upgrades | กดโดน QTE (Perfect/Great) ได้ +1 EXP ทุกครั้ง EXP เต็มแล้ว Player LV +1 และได้ +1 Point แสดงบน HUD | 2 | 1 |
| 9 | As a player, I want Toothless to knock on Day 2 and let me choose Chase or Tame, so that I can grow my team | ประตูขึ้น "Knock Knock !!" Chase = กลับฐาน ไม่เสีย Energy / Tame = เลือกสัตว์ไปสู้ ชนะแล้วได้ Toothless + 200 coin | 3 | 1 |
| 10 | As a player, I want a Dodge/Attack fight wheel, so that fights test my timing | Dodge = Perfect อย่างเดียวและหดลงเรื่อยๆ ไม่ทัน/กดพลาด = โดนตี · Attack Perfect 100% / Great 75% ATK · สัตว์ตายแล้วเลือกตัวใหม่ได้ HP ศัตรูไม่รีเซ็ต · ตายหมด = แพ้ | 8 | 1 |
| 11 | As a player, I want the merchant on Day 3 to offer to buy my pet, so that I face a moral choice | Sell = ได้ 5,000 coin และสัตว์หายไป (ขายตัวสุดท้ายไม่ได้) / Refuse = เปิดร้านขาย Crab Apple, Caffeine Tonic, Sea Tea ซื้อแล้วใช้ทันที | 5 | 1 |
| 12 | As a player, I want a thunderstorm on Day 4, so that I have to react to disasters | สัตว์ทุกตัว Clean −50 (ไม่ต่ำกว่า 0) และปลดล็อกหน้า Disaster ในสมุด | 2 | 1 |
| 13 | As a player, I want Big Z to invade on Day 5, so that the slice ends on a cliffhanger | Fight กับ Big Z (HP 9999, ATK 40) แพ้แน่นอน แล้วขึ้นหน้า "To be continued..." กลับเมนูหลัก | 3 | 1 |
| 14 | As a player, I want the Doctor to revive fallen pets, so that a loss isn't always permanent | หน้าหมอแสดงเฉพาะสัตว์ HP = 0 ราคา 250 coin ยืนยัน "Are you sure?" แล้ว HP = 1 | 2 | 1 |
| 15 | As a player, I want a Game Over when every pet falls in a fight, so that my choices have stakes | สัตว์ตายหมดตอนสู้ Toothless → Game Over → กลับหน้าเลือกสัตว์เริ่มต้น | 2 | 1 |
| 16 | As a player, I want real sprites for the 3 starter pets, so that they look cute but uncanny | สไปรต์ Idle / Happy / Hurt ครบ 3 ตัว ตาม Art Direction (Soft Hand-drawn + Uncanny) ใส่ในเกมแทน placeholder | 5 | 2 |
| 17 | As a player, I want real sprites for Toothless, Big Z, the Merchant, the Doctor and the Keeper, so that every character reads clearly | สไปรต์ครบ 5 ตัว (Idle + ท่าโจมตีของศัตรู) ใส่ในเกมแทน placeholder | 5 | 2 |
| 18 | As a player, I want painted parallax layers for the base, so that the world feels warm and lonely like Kingdom | ภาพ layer ท้องฟ้า / ภูเขาไกล / ภูเขาใกล้ / ผนังฐาน / พื้น ขนาดรองรับฐานกว้าง 3840 px ใส่แทนรูปทรง | 5 | 2 |

## Should Have

| # | User Story | Acceptance Criteria | Estimate (SP) | Sprint |
|---|---|---|---|---|
| 1 | As a player, I want a notebook of my pets and past disasters, so that I can study them | Pet discovery แสดงเฉพาะสัตว์ที่มี (LV, Stomach, Clean, Health, ธาตุ, คำอธิบาย) · Disaster เปิดได้หลังเจอภัยนั้นแล้ว | 3 | 1 |
| 2 | As a player, I want an Upgrade station, so that I can make QTE, Energy and Train progress better | QTE 100 coin · 1 pt / Energy 150 · 3 / Progress 20 · 2 ปุ่ม + กดไม่ได้เมื่อ coin/points ไม่พอหรือเต็มแล้ว | 3 | 1 |
| 3 | As a player, I want a day/night sky and parallax, so that ending the day feels meaningful | ท้องฟ้ามืดตอน End day แล้วสว่างตอนเช้า layer เลื่อนคนละความเร็วตามกล้อง | 3 | 1 |
| 4 | As a player, I want sound effects for QTE and fights, so that hits feel satisfying | SFX Perfect / Great / Miss / Dodge / Attack / Knock / Acid เล่นตรงจังหวะ | 3 | 2 |
| 5 | As a player, I want background music for the base and fights, so that the mood shifts from cozy to tense | BGM ฐาน (cozy Lo-Fi) และ BGM ต่อสู้ วนลูปได้ และเปลี่ยนเพลงตอนเข้า-ออกการต่อสู้ | 3 | 2 |
| 6 | As a designer, I want to playtest and rebalance the numbers, so that 5 days feel hard but fair | เล่นจริงอย่างน้อย 3 คน จดผล แล้วปรับค่าใน `Balance` (Energy, decay, ราคา, ความเร็วเข็ม) | 3 | 2 |
| 7 | As a designer, I want to define each pet's passive skill, so that picking a starter matters | สรุป Passive 3 ตัวลง GDD พร้อมตัวเลข แล้วใส่ในโค้ดผ่านช่อง `Pet.Passive` | 5 | 2 |
| 8 | As a player, I want a cozy UI font and framed UI panels, so that the UI matches the art | ฟอนต์ UI + กรอบ panel / ปุ่ม / dialogue box ตาม Mood Board ใส่แทนของ placeholder | 3 | 2 |

## Nice to Have (Could Have)

| # | User Story | Acceptance Criteria | Estimate (SP) | Sprint |
|---|---|---|---|---|
| 1 | As a player, I want an Option menu with volume, so that I can adjust the sound | ปุ่ม Option ในเมนูเปิดหน้าปรับเสียง BGM / SFX และค่าคงอยู่จนปิดเกม | 2 | 3 |
| 2 | As a player, I want a pet to learn a new attack at LV 3, so that training pays off in fights | ถึง LV 3 แล้วปลดท่าใหม่ ใช้ในหน้า Fight ได้ (ดาเมจหรือ effect ต่างจาก Attack ปกติ) | 3 | 3 |
| 3 | As a player, I want a daily summary report card, so that I can see how well I did each day | หน้าสรุปตอนจบวันแสดงเกรด S–F, สเตตัสที่ลด และ coin ที่ได้ | 3 | 3 |
| 4 | As a player, I want to save and load my run, so that I can continue later | บันทึกอัตโนมัติตอนจบวัน เมนูหลักมีปุ่ม Continue | 5 | — |
| 5 | As a player, I want random events after Day 5, so that the game keeps going (Endless) | Day 6+ สุ่ม Event จาก pool (สัตว์ใหม่ / พ่อค้า / ภัยพิบัติ) ความยากเพิ่มตามจำนวนวัน | 8 | — |
| 6 | As a player, I want more shop items and a bag, so that I can plan my resources | ไอเทมอีก 5 ชิ้นจาก GDD 03 + กระเป๋า 8 ช่องใช้ของทีหลังได้ | 5 | — |

---

## Story Point Estimation Reference
ใช้ Modified Fibonacci: `1, 2, 3, 5, 8, 13, 20`
- **1 SP:** งานเล็กมาก ไม่เกิน 1-2 ชั่วโมง เช่น เพิ่ม UI text, ปรับตัวเลข balance
- **2 SP:** งานเล็ก ครึ่งวัน เช่น เพิ่ม sound effect, สร้าง prefab ง่ายๆ
- **3 SP:** งานขนาดกลาง 1 วัน เช่น ทำ mechanics ย่อย 1 ชิ้น, ออกแบบ UI 1 หน้าจอ
- **5 SP:** งานขนาดใหญ่ 2-3 วัน เช่น Core mechanic 1 ระบบ, หน้าจอพร้อม logic
- **8 SP:** งานใหญ่มาก ต้องแบ่งเป็น task ย่อย หรือทำทั้งสัปดาห์
- **13+ SP:** ใหญ่เกินไปสำหรับ 1 Story — **ต้องแตกเป็นหลาย Story ก่อนนำเข้า Sprint**

## Links
- [[02-sprint-backlog|Sprint Backlog]]
- [[sprint-plan-01|Sprint 1 Plan]]
- [[06-vertical-slice|GDD 06 — Vertical Slice]]
