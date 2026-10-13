---
type: gdd-scope
version: 2.2
date: 2026-10-13
---

# BePal — Scope & Feature List

> **หมายเหตุ:** ปรับให้ตรงกับโค้ดแล้วเมื่อ 2026-10-13 — [06-vertical-slice.md](06-vertical-slice.md) และโค้ดยังเป็นข้อกำหนดหลักของ Prototype
> คอลัมน์ "Status (Prototype)" ท้ายตาราง Feature: ✅ ทำแล้ว · 🔶 ทำแบบต่างจากเดิม (ดูหมายเหตุ) · ⏳ future (ยังไม่อยู่ใน Prototype)

## Unique Selling Point (USP)

1. **Cozy Sci-Fi Sanctuary with Hardcore Survival Peril:** คอนทราสต์ที่ลงตัวระหว่างความอบอุ่นของการเป็น "ผู้ดูแลสัตว์ต่างดาวบนดาวเคราะห์ที่โดดเดี่ยว" กับความตึงเครียดของเกมเพลย์เอาชีวิตรอด สัตว์อวกาศทุกตัวมีระบบความต้องการทางกายภาพที่สมจริง (Stomach, Clean) หากผู้เล่นบริหารจัดการพลาด พวกมันจะเจ็บป่วย โดนลดสเตตัส และอาจตายจากไปตลอดกาล
2. **Discrete Energy Economy (The Lone Caretaker's Budget):** ความท้าทายของการทำงานตัวคนเดียวบนดาวทั้งดวงด้วยแต้ม Energy เริ่มต้นเพียง 3 แต้มต่อวัน (อัปเกรดได้สูงสุด 6) ผู้เล่นต้องตัดสินใจอย่างเด็ดขาดและจัดลำดับความสำคัญ (Prioritize) ว่าจะใช้แรงที่มีจำกัดไปกับการให้อาหาร ทำความสะอาด ฝึกฝน หรือรักษาพยาบาล เพื่อให้ผ่านพ้นไปได้ในแต่ละวัน
3. **10-Attempt Precision Care QTE Engine:** การสื่อสารและดูแลสิ่งมีชีวิตต่างดาวต้องอาศัยทักษะความแม่นยำผ่านวงล้อมินิเกม 10 จังหวะ การกดเข้า Perfect Zone จะให้รางวัลตอบแทนสูง (High Reward) แต่หากพลาด เป้าหมายจะหดเล็กลงหรือหมุนเปลี่ยนทิศ สร้างความท้าทายและสะท้อนถึงความพยศของสัตว์ต่างดาวในทุกๆ แอ็กชัน
4. **The Unpredictable "Red Door" & Dynamic Combat:** "ประตูแดง" ศูนย์กลางของฐานที่นำพามาทั้งมิตรและศัตรูแบบ Endless ผู้เล่นจะไม่มีทางรู้เลยว่าสิ่งที่มาเคาะประตูคือสัตว์ที่ต้องการความช่วยเหลือ พายุอวกาศ หรือพ่อค้าเร่จอมขูดรีด พร้อมระบบต่อสู้ที่ต้องกะจังหวะหลบหลีก (Dodge) และสวนกลับ (Counter-Attack)
5. **High-Stakes Moral Crossroads:** บททดสอบจิตใจของผู้รักสัตว์ ด้วยทางเลือกที่ส่งผลต่อทรัพยากรและความอยู่รอดของฐาน เช่น ข้อเสนอขอซื้อสัตว์เลี้ยงแลกกับเงินก้อนโต 5,000G ผู้เล่นต้องเลือกว่าจะยอมทรยศอุดมการณ์เพื่อนำเงินมาต่อลมหายใจให้สถานพักพิง หรือจะสู้สุดชีวิตเพื่อปกป้องพวกมัน (As built: ตัวเลือก Sell ใช้ไม่ได้เมื่อเหลือสัตว์ตัวเดียว — ไม่มี Game Over จากการขาย)

---

## Feature List & Priorities

| #   | Feature                             | Priority   | รายละเอียด / Acceptance Criteria                                                                                                                                                                            |
| --- | ----------------------------------- | ---------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| 1   | **Starter Pet Selection**           | **Must**   | ผู้เล่นเลือกสัตว์เลี้ยงต่างดาวเริ่มต้น 1 ใน 3 สายพันธุ์ (เช่น ไอ่แดง, ไอ่ซุง, ไอ่เขียว) ที่มีสเตตัสและธาตุเริ่มต้นต่างกัน เพื่อนำมาดูแลในสถานพักพิง                                                         |
| 2   | **Endless 4-Phase Daily Loop**      | **Must**   | 🔶 ลูปเกมแบบไร้จุดจบ (Endless) โดย 1 วันแบ่งเป็น 4 เฟสชัดเจน: Narrative Event $\rightarrow$ Care & QTE $\rightarrow$ Defense/Resolution $\rightarrow$ Progression & Save — **As built:** ฐาน free-roam, End day ที่เตียง, Day 1–5 ตายตัว, ไม่มี Save/เฟสแยก |
| 3   | **Discrete Energy Budget (3–6 AP)** | **Must**   | โควตาพลังงานเริ่มต้น 3 แต้มต่อวัน (อัปเกรดได้สูงสุด 6) กดเข้าหน้า QTE = -1 Energy การใช้คำสั่งดูแลจะหักแต้ม Energy และจะรีเซ็ตกลับเมื่อเข้าสู่วันใหม่                                                                                                  |
| 4   | **4 Core Care Actions & UI**        | **Must**   | ระบบคำสั่ง 4 หมวด: Feed (หิว), Clean (ความสะอาด), Train (เพิ่มเลเวล), Heal (รักษา HP) โดยผู้เล่นต้อง**เดินไปใกล้สัตว์ + `Space`** (As built) และกดกะจังหวะวงล้อเพื่อเลือกคำสั่ง                                                 |
| 5   | **10-Attempt Precision QTE**        | **Must**   | มินิเกม QTE ดูแลสัตว์ 10 ครั้งต่อรอบ ผู้เล่นต้องกด Spacebar เล็งเป้าหมายให้เข้า Perfect Zone หรือ Great Zone เพื่อสะสมหลอด Progress                                                                          |
| 6   | **Pet & Player Progression System** | **Must**   | สัตว์เลี้ยงเลเวลอัปเมื่อหลอด Progress เต็ม (ยิ่งเลเวลสูงหลอดยิ่งเต็มยาก) / ผู้เล่นจะได้รับ Player EXP +1 ทุกครั้งที่กด QTE โดน (Perfect **หรือ** Great) เพื่อนำแต้มไปอัปเกรดฐาน                                              |
| 7   | **Day 1: Tutorial & Safe Day**      | **Must**   | วันแรกเป็นการให้ผู้เล่นคุ้นเคยกับการดูแลสัตว์และระบบ QTE จะยังไม่มีพายุหรือภัยคุกคามใดๆ เพื่อลดความตึงเครียด                                                                                                |
| 8   | **Day 2: Toothless Encounter**      | **Must**   | เหตุการณ์สัตว์ป่ากรดเคาะประตู มีทางเลือกขับไล่ (Chase) หรือฝึกให้เชื่อง (Tame) ผ่านมินิเกมต่อสู้หลบหลีก (Dodge) และสวนกลับ                                                                                  |
| 9   | **Day 3: Merchant Buyout Dilemma**  | **Must**   | พ่อค้าเร่มาเยือน เปิดร้านค้าขายไอเทม และยื่นข้อเสนอซื้อสัตว์เลี้ยงแลก 5,000 coin — ✅ **Sell** (+5,000, สัตว์หาย, ใช้ไม่ได้ถ้าเหลือตัวเดียว) หรือ **Refuse** → เปิดร้านค้าทั้งสองทาง                                                              |
| 10  | **Merchant Boss Fight**             | **Must**   | ⏳ *future — ไม่มีใน Prototype* (Refuse ไปที่ร้านค้าทันที) เดิม: Dodge/Attack ให้โดน 5 ครั้ง                                                                                   |
| 11  | **Gold Economy & Inventory**        | **Must**   | ✅ เงิน coin ซื้อไอเทมจากพ่อค้า → เข้า **กล่อง (Box)** ในฐาน (`InventoryScene`) ใช้ทีหลัง · ถือได้ ≤ 3 ต่อชนิด (`ShopMaxOwned`)                                                                                         |
| 12  | **Equipment / Item Buffs**          | **Should** | ✅ 7 ไอเทม (`Model/Items.cs`): Crab Apple 25 (+18 HP, +20 Stomach) · Caffeine Tonic 40 (+2 Energy) · Sea Tea 18 (Dodge zone +20% Fight ถัดไป) · Cloudy Glasses 30 (กัน 2 hit) · Ballet Shoes 50 (Perfect +15% วันนี้, Stomach −5 คืนนี้) · Torn Notebook 55 (Train +50% วันนี้) · Faded Ribbon 35 (Clean decay −30% คืนนี้) · ไม่มี Exp/ATK potion, Toy Knife                                                                |
| 13  | **Emergency Revive (250G)**         | **Must**   | เมื่อ HP เหลือ 0 สัตว์จะเข้าสู่ภาวะหมดสภาพ ผู้เล่นต้องจ่าย 250G ให้คุณหมอเพื่อชุบชีวิตกลับมา HP = 1 (Figma S13: หน้าหมอแสดงเฉพาะสัตว์ HP = 0 + ยืนยัน "Are you sure?")                                                                                                      |
| 14  | **Daily Summary Report Card**       | **Must**   | ⏳ *future* (As built: `NightScene` fade + dialogue "Morning" สรุป +100 coin / energy restored; ไม่มีเกรดและไม่มี Save) หน้าสรุปผลประจำวันสไตล์ Papers, Please แสดงเกรดการดูแล, โบนัสรายได้, สรุปการหักสเตตัส (Daily Decay) และเซฟเกม                                                                                               |
| 15  | **Survival Log Research Journal**   | **Should** | หน้าต่างสมุดบันทึกข้อมูลสัตว์เลี้ยงต่างดาว และรวบรวมข้อมูลภัยธรรมชาติหรือศัตรูที่เคยเผชิญหน้า ปลดล็อกข้อมูลเมื่อผ่านเหตุการณ์นั้นๆ                                                                          |
| 16  | **Dynamic Audio Feedback & BGM**    | **Should** | ✅ (17 SFX + 7 BGM ตาม scene — ดู [14](14-audio-fonts-list.md)) เสียง SFX แจ้งเตือนการกด QTE (Perfect, Great, Miss, Dodge, Attack) และเพลงประกอบ BGM บรรยากาศ Cozy ในห้องตัดกับเพลงระทึกขวัญตอนเจอศัตรู                                                                      |
| 17  | **Game Over & Endless High Score**  | **Must**   | 🔶 As built: `GameOverScene` แสดง "GAME OVER" + เหตุผล + "Space Choose a new pet" (ไม่มีคะแนน/High Score; จำนวนวันโชว์ใน `ToBeContinuedScene`) · เงื่อนไข: สัตว์ตายหมดใน Fight Toothless, หรือหลัง Night ไม่มีสัตว์เหลือและ coin < 250. หน้าจอสรุปผลเมื่อสัตว์เลี้ยงตายทั้งหมด (Fail State) แสดงสถิติจำนวนวันที่รอดชีวิต (Day Survived) และคะแนนสะสม เพื่อให้ผู้เล่นแข่งทำ High Score                                                               |
| 18  | **Upgrade Station System**          | **Must**   | ✅ ต้องใช้ **ทั้ง coin และ Points** ต่อการซื้อ (QTE 100c · 1/1/1 pts · Energy 150c · 1/2/4 pts · Progress 20c · 1/2/3 pts; สูงสุด 3 level ต่อสาย — ดู [03 §1.3](03-mechanics.md)) ระบบสถานีอัปเกรดฐานที่ให้ผู้เล่นนำแต้ม Skill Points (ที่ได้จากการอัปเลเวลผู้เล่น) และเงินมาใช้อัปเกรดความสามารถถาวร เช่น เพิ่ม Max Energy (สูงสุด 6), ขยายขนาดปุ่ม QTE, หรือเพิ่มตัวคูณหลอด Progress |
| 19  | **Endless Random Event Generato**   | **Must**   | ⏳ *future — Prototype ใช้ event ตายตัว Day 2–5 (`DayEvents.OpenDoor`)* ระบบสุ่มเหตุการณ์รายวันสำหรับ Day 4 เป็นต้นไป ต้องระบุให้โปรแกรมเมอร์รู้ว่าต้องเขียนระบบสุ่ม (RNG) ที่จะดึงข้อมูล ภัยธรรมชาติ (พายุ), ศัตรูบุก, หรือพ่อค้าเร่ มาสร้างเป็น Event ในแต่ละเช้า                 |
| 20  | **Dynamic QTE Modifier**            | **Should** | ⏳ *future — ยังไม่มี hook จากความสกปรก/พายุ* ระบบแทรกแซง QTE ตามสถานการณ์ เช่น สัตว์สกปรกมากเข็มจะหมุนเร็วขึ้น หรือถ้าเจอพายุโซนเป้าหมายจะกะพริบ เพื่อเพิ่มความท้าทาย                                                                                    |
| 21  | **Care Action Select Wheel**        | **Must**   | (Figma S4) คลิ๊กสัตว์ → วงล้อ 4 choice สี กด Spacebar โดน choice ไหนเข้า QTE นั้น กดไม่โดน = ไม่เกิดอะไร, ESC กลับ Base, จอสั่นเมื่อกดโดน                                                                   |
| 22  | **4 QTE Variants**                  | **Must**   | (Figma S5–8) Train = จุดแดงเล็ก 3 จุดพร้อมกัน เข็มเร็ว (3.6) / Feed = กดโดนเปลี่ยนทิศหมุน / Clean = choice หนีเข็ม / Heal = เข็มกะพริบ + choice หด — รายละเอียดใน [08-care-qte §3.2.1](08-care-qte.md)                |
| 23  | **Red Door Random Event**           | **Must**   | 🔶 (Figma S14) ประตูขึ้น "Knock Knock !!" (Day 2+) — **As built: ไม่สุ่ม** Day 2 สัตว์ใหม่ (Toothless) / Day 3 พ่อค้า / Day 4 ภัยธรรมชาติ / Day 5 Big Z                                                                                   |
| 24  | **Tame Fight (multi-pet)**          | **Must**   | (Figma S16–19) Chase/Tame → เลือกสัตว์ไปสู้ → Dodge (Perfect หด) / Attack (Perfect 100%, Great 75%) → สัตว์ตายสลับตัวได้โดย progress คงอยู่ → ชนะได้สัตว์ใหม่ / ตายหมด Game Over                         |
| 25  | **Notebook Unlock Rules**           | **Should** | (Figma S9–11) pet discovery แสดงเฉพาะสัตว์ที่มี, Disaster ดูได้เฉพาะภัยที่เคยผ่าน                                                                                                                          |
| 26  | **Option Screen**                   | **Could**  | ✅ `OptionsScene`: Music / Sound effects volume (`W`/`S` เลือก, `A`/`D` ปรับ, `Esc` กลับ) เปิดจาก Main Menu และเมนู Pause · บันทึกที่ `%APPDATA%/BePal/settings.txt`                                           |
| 27  | **Pause Menu / Box / Shift run**    | **Could**  | ✅ (เพิ่มนอก Figma) `Esc` ในฐาน = Resume / Options / Main Menu · กล่องไอเทมที่ `BoxX` 600 · `Shift` = วิ่ง · Day 5 Big Z → `ToBeContinuedScene` · รายได้: +100/เช้า, ชนะ Toothless +200 |



---

## Out of Scope — สิ่งที่ไม่ทำในเฟสนี้

- **No Real-Time 3D Graphics:** ใช้ภาพกราฟิก 2D (Soft Hand-drawn หรือ Soft Pixel Art ตาม Concept Design ใน Figma) 1280x720 เท่านั้น
- **No Multiplayer / Online Leaderboards:** มุ่งเน้นประสบการณ์เล่นคนเดียว (Single-Player Simulation)
- **No Weapon Inventory / Free Combat:** การต่อสู้ทำผ่านวงล้อจังหวะ QTE สั่งการสัตว์เลี้ยง ไม่มีการควบคุมตัวละครเดินยิงหรือฟันดาบอิสระ

---

## Risks & Mitigation

| ความเสี่ยง                                          | ผลกระทบ                                                                               | แนวทางป้องกันและแก้ไข                                                                                                                                 |
| --------------------------------------------------- | ------------------------------------------------------------------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------- |
| **ความซับซ้อนของ 10-Attempt QTE Math**              | ผู้เล่นรู้สึกยากหรือง่ายเกินไป                                                        | กำหนดเป้าหมายโซนให้ชัดเจน (As built: Care Perfect 0.12 / Great 0.27, Train 0.07 / 0.16 rad) พร้อมทำระบบให้ปรับจูนค่าใน `Balance` (`Model/GameState.cs`) ได้ง่ายตอนทำบาลานซ์เทสต์ (ไม่มีหน้า Config ในเกม)                      |
| **ความลื่นไหลของการสลับหน้าจอ (Screen Transition)** | เกิดสถานะค้างระหว่างฉากฐานหลักและฉากมินิเกม/ต่อสู้                                    | รวมศูนย์การเปลี่ยนหน้าจอไว้ที่เดียว (ออกแบบตอนสร้าง project MonoGame)                                                                               |
| **สมดุลทางเศรษฐกิจ (Gold Economy & Deadlock)**      | ผู้เล่นเงินหมดจนจ่ายค่าชุบชีวิต 250G ไม่ได้ ทำให้เกมจบ (Game Over) เร็วเกินความจำเป็น | ปรับสมดุลให้มี Event หาเงินโผล่มาอย่างเหมาะสม (เช่น รางวัลจากการชนะต่อสู้) และออกแบบให้การ Game Over เป็นเรื่องปกติของแนว Endless                     |
| **ภาระงานด้านภาพสไปรต์สัตว์เลี้ยงหลายอารมณ์**       | กราฟิกเสร็จไม่ทันตามกำหนดเวลา                                                         | ออกแบบโครงสร้าง Sprite Sheet หลักแค่ 4 ท่า (Idle, Happy, Angry, Attack/Teleport) และใช้ Procedural Shake (การสั่นของภาพ) ช่วยเสริมอารมณ์แทนการวาดใหม่ |
| **ความจำเจของลูป Endless (Endless Repetitiveness)** | ผู้เล่นรู้สึกเบื่อกับลูปเดิมๆ หลังจากผ่าน Day 4 ไปแล้ว                                | ออกแบบโครงสร้าง Random Event Pool ให้ยืดหยุ่น เพิ่มความยากแบบ Dynamic Scaling (ศัตรูเลือดเยอะขึ้น, เข็ม QTE หมุนแปลกขึ้น) เมื่อวันลึกขึ้น             |
