---
type: gdd-mechanics
version: 2.3
date: 2026-10-13
---

# BePal — Mechanic Design & Systems Specification

> **หมายเหตุ:** ปรับให้ตรงกับโค้ดแล้วเมื่อ 2026-10-13 — [06-vertical-slice.md](06-vertical-slice.md) และโค้ดยังเป็นข้อกำหนดหลักของ Prototype
> ส่วนที่เป็นแนวคิด Endless (สุ่ม event, Save, Report Card, Phase 1–4) **ยังไม่อยู่ใน Prototype** — Prototype ใช้ Day 1–5 แบบกำหนดตายตัว (`Balance.LastDay` = 5) และฐานเป็นฉาก side-scroller เดินอิสระ (A/D + Space) ไม่ใช่ห้องที่คลิกเมาส์

## State Machine Diagram

![state-machine](diagrams/state-machine.png)

> Draw.io: [diagrams/state-machine.drawio](diagrams/state-machine.drawio) (เปิดด้วย VS Code Draw.io Integration)

---
## Section 0: Game Architecture & The Endless Loop

### 0.1 Story & Setting Overview (เรื่องราวและฉากหลัง)

- **The Lone Sanctuarist:** ผู้เล่นรับบทเป็นสิ่งมีชีวิตสายพันธุ์ Humanoid ลึกลับที่อาศัยอยู่เพียงลำพังบนดาวเคราะห์อันห่างไกล โดยเปลี่ยนดาวทั้งดวงให้เป็น "สถานพักพิงสัตว์อวกาศ" (Alien Pet Sanctuary)
- **The Red Door Encounters:** "ประตูแดง" ศูนย์กลางของฐานทำหน้าที่ต้อนรับสิ่งมีชีวิตแปลกประหลาดที่หลงทาง บาดเจ็บ หรือบุกรุกเข้ามา
- **Beyond the Pets:** ภัยคุกคามที่ไม่แน่นอน ทั้งสิ่งมีชีวิตทรงภูมิปัญญาจอมขูดรีด พ่อค้าเร่ หรือภัยพิบัติทางธรรมชาติระดับจักรวาล
### 0.2 Unique Selling Point (USP)

- **Cozy Sci-Fi Sanctuary with Hardcore Survival Peril:** บรรยากาศอบอุ่นที่ซ่อนความตึงเครียดของการเอาชีวิตรอด สัตว์อวกาศมีระบบความต้องการสมจริง (Stomach, Clean) หากพลาดอาจเจ็บป่วยและตายได้
- **Discrete Energy Economy (The Lone Caretaker's Budget):** บีบคั้นการตัดสินใจด้วยแต้ม Energy เริ่มต้นเพียง 3 แต้มต่อวัน (อัปเกรดได้สูงสุด 6) บังคับให้ผู้เล่นต้องจัดลำดับความสำคัญอย่างเด็ดขาด
- **10-Attempt Precision Care QTE Engine:** มินิเกมวงล้อความแม่นยำ 10 จังหวะ ผูกผลลัพธ์เข้ากับระบบ Progression โดยให้รางวัลระดับ High Reward เฉพาะการกดเข้า Perfect Zone
- **The Unpredictable "Red Door" & Dynamic Combat:** สุ่มเจอศัตรูและเหตุการณ์แบบ Endless พร้อมระบบต่อสู้กะจังหวะ Dodge หลบหลีก และ Counter-Attack สวนกลับ
- **High-Stakes Moral Crossroads:** ทางเลือกเดิมพันสูง เช่น การยอมขายสัตว์เลี้ยงแลกเงิน 5,000G ที่อาจนำไปสู่ความล่มสลายและ Game Over หากโลภจนไม่เหลือสัตว์เลี้ยงในฐาน

### 0.3 The Endless Core Loop (โครงสร้างลูปประจำวัน)
_(สามารถนำโค้ด stateDiagram-v2 ที่ร่างไว้มาแปะในส่วนนี้เพื่อให้ทีมเห็น Flow chart ทั้งหมด)_

- **Phase 1: Morning Event** — เหตุการณ์หน้าประตู (Prototype: กำหนดตายตัว Day 2 Toothless · Day 3 พ่อค้า · Day 4 พายุ · Day 5 Big Z; แนวคิดเดิม: สุ่ม)
- **Phase 2: Care & QTE** — บริหาร Energy (เริ่ม 3 AP อัปเกรดสูงสุด 6) เพื่อเข้าวงล้อดูแลสัตว์ (Feed, Clean, Train, Heal)
- **Phase 3: Defense & Resolution** — ตัดสินใจรับมือเหตุการณ์ประจำวัน หรือเข้าสู่ฉาก Combat QTE
- **Phase 4: Progression & Save** — คำนวณหักสเตตัสรายวัน (Decay), ตรวจสอบการเจ็บป่วย, อัปเลเวล และข้ามเข้าสู่วันใหม่

## Section 1: The Sanctuary (Base Systems & UI)
### 1.1 Base Room HUD Layout & Hitboxes
ห้องพักพิงหลัก (Habitat Base Room) เป็นหน้าจอหลักสำหรับการบริหารจัดการ โดยมีองค์ประกอบการโต้ตอบหลักบนหน้าจอดังนี้:

**Top Bar (แถบสถานะผู้ดูแล):**
- - ตัวนับวัน [ Day (ตามจำนวนวันที่รอดชีวิต) ] บ่งบอกถึงความก้าวหน้าในโหมด Endless
- หลอดพลังงาน Energy Pips (เริ่ม 3 ดวง อัปเกรดได้สูงสุด 6 ดวง) สำหรับใช้ทำกิจกรรม
- จำนวนเงินคงเหลือ [ GOLD: 150 G ]
- หลอดเลเวลผู้เล่น [ PLAYER LV. 1 ] พร้อมแถบ EXP (หลอดจะเพิ่มขึ้น Great +1 / Perfect +2 ทุกครั้งที่กด QTE โดน)
**Center Habitat Stage (พื้นที่พักพิงสัตว์เลี้ยง):**
- แสดง Sprite สัตว์เลี้ยงปัจจุบัน (เช่น ไอ่แดง, ไอ่ซุง, ไอ่เขียว, Toothless) เดินไปมาหรือมีแอนิเมชัน Idle ในห้อง
**Pet Status Interaction (การเช็กสเตตัสสัตว์เลี้ยง):**
- เมื่อตรวจสอบสัตว์เลี้ยง จะแสดงหลอดสเตตัสหลัก 3 อย่าง: **HP** (พลังชีวิต), **Stomach** (ความอิ่ม), และ **Clean** (ความสะอาด) พร้อมแสดง **Level / Progress Bar** ของสัตว์เลี้ยงตัวนั้นๆ
**Command Interaction (การสั่งการดูแลสัตว์เลี้ยง):**
- ผู้เล่นต้อง **เดินไปใกล้ตัวสัตว์เลี้ยงแล้วกด `Space`** (As built; แนวคิดเดิม: คลิกซ้าย) เพื่อเปิดหน้าต่างวงล้อคำสั่ง (Action Selection Wheel) · เลือกคำสั่งใช้ 1 Energy; Heal ใช้ไม่ได้เมื่อ HP เต็ม
- เข็มจะหมุนไปรอบวงล้อที่มี 4 คำสั่ง ผู้เล่นต้องกด Spacebar เล็งให้ตรงเพื่อเลือก:
	- **[ FEED ]** — ให้อาหาร (ใช้ 1 AP / หิวเพิ่ม)
	- **[ CLEAN ]** — ทำความสะอาด (ใช้ 1 AP / ความสะอาดเพิ่ม)
	- **[ TRAIN ]** — ฝึกซ้อม (ใช้ 1 AP / เพิ่มหลอด Progress เพื่ออัปเลเวล)
	- **[ HEAL ]** — รักษาพยาบาล (ใช้ 1 AP / ฟื้นฟู HP)
**Quick-Access UI Buttons (ปุ่มลัดหน้าจอหลัก):**
- **กล่องไอเทม (Box):** (As built — แทนปุ่ม BAG/คีย์ B) เดินไปที่กล่อง (`BoxX` 600) + `Space` เปิด `InventoryScene` เพื่อใช้ไอเทมที่ซื้อจากร้าน (ดู [10-economy-items.md](10-economy-items.md))
- **[ END DAY ] (เดินไปที่เตียง + `Space`/`Enter`):** สิ้นสุดวัน เพื่อข้ามเวลาไปยังช่วงประมวลผล (Phase 4) ทันที — ใช้ไม่ได้ถ้ายังมี event หน้าประตูค้างอยู่ (`DoorEventPending`)
- **ควบคุมอื่นๆ:** `Shift` = วิ่ง · `Esc` = เมนู Pause (Resume / Options / Main Menu) · `F3` = overlay dev

### 1.2 Environment Objects (วัตถุโต้ตอบในฐาน)
ผู้เล่นเดินไปที่จุดโต้ตอบแล้วกด `Space` (As built: ตำแหน่ง X ของจุดต่างๆ — เตียง 300 · กล่อง 600 · Upgrade 900 · Hearth 1900 · หมอ 2650 · สมุด 3100 · ประตู 3640; โลกกว้าง 3840):

- **ประตูแดงคู่ (Front Door):** จุดเปิดรับเหตุการณ์และแขกที่มาเคาะประตู ("Knock Knock !!" เสียงเคาะซ้ำทุก 5 วินาที) — Prototype: Event กำหนดตายตัวตามวัน (แนวคิดเดิม: สุ่ม)
- **NPC คุณหมอ (Doctor Clinic):** บริการกู้ชีพสัตว์เลี้ยงฉุกเฉิน ผู้เล่นสามารถจ่ายเงิน 250 Gold เมื่อสัตว์เลี้ยง HP = 0 เพื่อชุบชีวิตให้ฟื้นกลับมามี HP = 1
- **สถานีอัปเกรดฐาน (Upgrade Station / ไอคอนรูปเฟือง):** ใช้แต้ม Skill Points (ที่ได้จากการอัปเลเวลผู้เล่น) และเงิน Gold เพื่ออัปเกรด 3 สาย:
	- **สาย QTE:** ทำให้กด QTE ง่ายขึ้น (As built: ขยาย zone ของ QTE Train ×(1 + 0.2 × level) เท่านั้น ไม่ลดความเร็วเข็ม)
	- **สาย Progress Booster:** เพิ่มประสิทธิภาพการดูแล ทำให้หลอด Progress เพิ่มเยอะขึ้นเมื่อกดสำเร็จ (As built: Train Perfect/Great = 10/5 × (1 + 0.5 × level) เช่น Lv1 → 15/7.5, Lv3 → 25/12.5)
	- **สาย Energy:** เพิ่มขีดจำกัดพลังงานรายวันให้ผู้เล่น (Max Energy อัปได้สูงสุดไม่เกิน 6 AP)
- **โต๊ะค้นคว้า (Survival Desk / ไอคอนสมุด):** สำหรับเปิดสมุดบันทึก เพื่อดูคำอธิบายของสัตว์เลี้ยง (ข้อมูลธาตุต่างๆ) และรวบรวมบันทึกภัยพิบัติหรือศัตรูที่เคยเผชิญหน้า

> **ประตู (Figma Scene 3):** คลิ๊กประตูเพื่อต้อนรับ Event ได้ตั้งแต่ **Day 2** ขึ้นไป (Day 1 เป็นวันให้ผู้เล่นทำความรู้จักระบบ) — ปุ่ม **End day** กดได้ทุกเมื่อ

### 1.3 Upgrade Station — ค่าใช้จ่ายและผลลัพธ์ (Figma Scene 12)

- **Points:** เริ่มต้น 0 และได้เพิ่มทุกครั้งที่ผู้เล่น Level up (Player EXP bar เต็ม)
- ต้องมี **Coin และ Points** ครบตามที่กำหนดจึงจะอัปเกรดได้ (กดปุ่ม `+`) — ESC กลับ Base

| สาย | ค่าใช้จ่าย (REQ) | ผลเมื่ออัปเกรด |
| --- | --- | --- |
| **QTE** | 100 Coin · 1 / 1 / 1 Point (Lv 1 / 2 / 3) | choice บนวงล้อ **QTE Train** ใหญ่ขึ้น ×(1 + 0.2 × level) (ถ่วงกับ level สัตว์ที่ทำให้ choice เล็กลง) |
| **Energy** | 150 Coin · 1 / 2 / 4 Points | Energy +1 ต่อวัน (และ +1 ทันทีที่ซื้อ) สูงสุด 3 level (เพดาน 6) |
| **Progress Bar** | 20 Coin · 1 / 2 / 3 Points | progress ของ Train +50% ต่อ level (สูงสุด 3 level) |

> ค่า Point ต่อ level อยู่ใน `Balance.QtePointCost/EnergyPointCost/ProgressPointCost` — ตัวเลขยืนยันแล้ว ดู [06 §3.3](06-vertical-slice.md)

### 1.4 Notebook (สมุด) — กฎการปลดล็อก (Figma Scene 9–11)

- สมุดหลักมี 2 หัวข้อ: **pet discovery** และ **Disaster** (hover มีกรอบ, ESC กลับ Base)
- **pet discovery:** แสดงเฉพาะสัตว์ที่ผู้เล่นมีอยู่ (เช่น มี 2 ใน 4 ตัว จะขึ้นข้อมูล 2 ตัว) — Day 1 จะมีแค่ตัวแรก แล้วค่อยๆ เพิ่มตามที่รับเลี้ยง หน้าข้อมูลแสดง LV, Stomach, Clean, Health, ไอคอนธาตุ และคำอธิบายสัตว์
- **Disaster:** กดเข้าได้เฉพาะเมื่อเคยผ่าน Event ภัยธรรมชาตินั้นแล้ว (เช่น Thunder storm) แสดงรูปและคำอธิบายภัย
- เปลี่ยนหน้าด้วย `A`/`D` หรือลูกศร · `Esc` ในหน้าย่อยกลับสมุดหลัก (ไม่ใช่ Base) · หน้าสัตว์แสดง LV, ATK, Health, Stomach, Clean, ธาตุ, คำอธิบาย และ Passive (หรือ "Passive - unlocks at Level 2")

### 1.5 Doctor (Figma Scene 13)

- ถ้าไม่มีสัตว์ HP = 0 → แสดง Doctor Background กับคำว่า "Choose pet" เท่านั้น
- ถ้ามี → แสดงการ์ดสัตว์เรียงแถว + ราคาชุบชีวิต (**250**) (As built: ไม่มีลูกศร)
- คลิ๊กที่สัตว์ → "Are you sure? Revive {name} for 250 coin." (เงินไม่พอ = "You don't have enough coin." และปุ่ม Yes ใช้ไม่ได้) **Yes** = Doctor พูด "{name} is breathing again..." แล้วกลับ Base พร้อมสัตว์ HP = 1 / **No** = กลับหน้า Choose pet ของหมอ — ESC กลับ Base
- ถ้าไม่มีสัตว์ตาย: "All your pets are alive. Come back if that changes."

---

## Section 2: Pet Stats, Progression & Decay Systems

→ ดู [07-pet-stats.md](07-pet-stats.md) (ย้ายไปไฟล์แยก 2026-10-07)

---

## Section 3: Care QTE Mini-Game & Gimmicks

→ ดู [08-care-qte.md](08-care-qte.md) (ย้ายไปไฟล์แยก 2026-10-07)

---

## Section 4: The Red Door Encounters & Combat Mechanics

→ ดู [09-combat-encounters.md](09-combat-encounters.md) (ย้ายไปไฟล์แยก 2026-10-07)

---

## Section 5: Sanctuary Economy & Resources

→ ดู [10-economy-items.md](10-economy-items.md) (ย้ายไปไฟล์แยก 2026-10-07)
