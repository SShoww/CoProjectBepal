---
type: gdd-vertical-slice
version: 1.1
date: 2026-10-13
deadline: 2026-09-30
---

# BePal — Vertical Slice (Prototype)

ขอบเขตของ Prototype ที่ต้องส่ง **วันพุธ 30 ก.ย. 2026** — ถ้าข้อมูลในไฟล์นี้ขัดกับไฟล์ 00–05 **ให้ยึดไฟล์นี้** สำหรับ Prototype

---

## 1. Timeline (Day 1–5, fixed — ไม่มีการสุ่ม)

| Day | Event ที่ประตู | รายละเอียด |
| --- | --- | --- |
| **1** | — (ประตูกดไม่ได้) | ให้ผู้เล่นเรียนรู้ระบบดูแลสัตว์ |
| **2** | **Toothless บุก** | Pet encounter → **Chase** (กลับฐาน ไม่สู้ ไม่เสีย Energy) หรือ **Tame** → เลือกสัตว์ไปสู้ → Fight → ชนะ = ได้ Toothless + **200 coin** |
| **3** | **พ่อค้า** | ขอซื้อ Toothless (ถ้าไม่มี Toothless จะขอซื้อ starter แทน) — **ขาย** = ได้ **5,000 coin** และสัตว์ตัวนั้นหายไป (ขายตัวสุดท้ายไม่ได้) แล้วเปิด **ร้านค้า** ต่อ / **ปฏิเสธ** = เปิด **ร้านค้า** เลย (เปลี่ยน 2026-10-13: เดิมร้านเปิดเฉพาะทางปฏิเสธ) |
| **4** | **Thunderstorm** | ข้อความแจ้งพายุ → Clean ของสัตว์ทุกตัว **−50** (`Balance.StormCleanLoss`, clamp ที่ 0) **รวมสัตว์ที่ตายแล้ว (HP = 0) ด้วย** (โค้ดไม่ข้ามตัวที่ตาย) + ปลดล็อกหน้า Disaster ในสมุด |
| **5** | **Big Z บุก** | เข้าหน้า Fight ปกติ (HP 9999) แพ้แน่นอน → หน้า **"To be continued..."** → กลับ Main Menu |

---

## 2. Controls & Base Layout (สไตล์ Kingdom: Classic)

| ปุ่ม | หน้าที่ |
| --- | --- |
| `A` / `D` | เดินตัวผู้เล่น (The Lone Sanctuarist) ซ้าย–ขวา กล้องเลื่อนตาม |
| `Space` | ใช้ของที่อยู่ใกล้ (ขึ้นป้าย `[Space]` เหนือของ) / กด QTE / ยืนยัน |
| `ESC` | ปิดหน้าต่าง กลับฐาน |
| Mouse | กดปุ่ม UI เท่านั้น (เมนู, ปุ่มในร้าน, Upgrade, Choice) |

- **End day:** เดินไปที่ **เตียง** แล้วกด `Space` (ไม่ใช้ปุ่ม `E` แล้ว)
- **ความกว้างฐาน:** ประมาณ 3 หน้าจอ (3840 px) ประตูอยู่ขวาสุด
- **As built (พฤติกรรมในเกมตอนนี้ — ไม่ใช่การเปลี่ยน scope):**
  - `Shift` = วิ่ง (560 px/s เทียบกับเดิน 330 px/s) · `F3` = overlay ตำแหน่ง (เครื่องมือ dev)
  - `ESC` ในฐาน = เปิดเมนู Pause (Resume / Options / Main Menu) · `ESC` ในหน้าต่างอื่น (Upgrade, Doctor, Notebook, Shop) = ปิดหน้าต่างตามเดิม
  - กำแพงฐาน: กำแพงแข็งซ้าย/ขวา + กำแพงนิ่ม (ชะลอก่อนถึงขอบ)
  - Day 4 มีฝน/ฟ้าแลบ · มีเสียงเคาะประตูทุก 5 วินาทีขณะมี event หน้าประตูค้างอยู่ · เตียง (End day) ใช้ไม่ได้จนกว่าจะจัดการ event นั้น

```
[ขอบมืด] — เตียง (End day) — Upgrade — [Core Hearth + สัตว์เดินไปมา] — หมอ — สมุด — [ประตูแดง]
```

- **Parallax:** 4–5 layer (ท้องฟ้า/ดาว, ภูเขาไกล, ภูเขาใกล้, ฐาน, พื้น) วาดด้วยรูปทรงง่ายๆ สีตาม Mood Board (ส้มอุ่นในฐาน / ม่วง-น้ำเงินด้านนอก)
- **กลางวัน–กลางคืน:** ท้องฟ้าเปลี่ยนเป็นกลางคืนตอน End day ก่อนเข้าวันใหม่
- ภาพอื่นๆ เป็น placeholder (รูปทรง + ข้อความ) จนกว่า Art จะเสร็จ

---

## 3. Numbers

### 3.1 สัตว์

| ตัว (ชื่อในเกม) | ชื่อทีม | HP | ATK | Stomach / Clean เริ่ม |
| --- | --- | --- | --- | --- |
| **Mossling** | ไอ่แดง | 100 | 20 | 80 / 70 |
| **Nibbleclaw** | ไอ่ซุง | 90 | 25 | 70 / 80 |
| **Blinkbun** | ไอ่เขียว | 110 | 15 | 60 / 60 |
| **Toothless** (ศัตรู → สัตว์เลี้ยง) | — | ศัตรู (Fight Day 2) HP 120 / ATK 15 (`Balance.ToothlessHp`) · สัตว์เลี้ยงหลัง Tame: Max HP ฐาน 120 แต่เริ่มที่ **HP 60** / ATK 15 | 60 / 60 (หลัง Tame) |
| **Big Z** (บอส) | — | 9999 | 20 | — |

> **Passive ของสัตว์ — implemented (first pass, ยังต้อง balance):** ปลดล็อกที่ Pet Level ≥ `Balance.PassiveLevel` (= 2) · Mossling: ฟื้น 1% Max HP ต่อการ Attack · Nibbleclaw: combo Attack โดนติดกัน ดาเมจ ×(1 + combo × 0.5) รีเซ็ตเมื่อพลาด · Blinkbun: หลัง Attack เข็มวาร์ปให้ไปถึงกลาง Dodge ในอีก 0.75 วิ (`Balance.BlinkbunLeadTime`) · Toothless: พิษ — ศัตรูรับดาเมจ ×1.25 / ตี ×0.75 (ไม่ stack)
> ท่าโจมตีใหม่เมื่อ Pet Lv 3 ("learned a new move! (coming soon)") **ยังไม่มี mechanic** — pending

### 3.2 ทรัพยากร

| รายการ | ค่า |
| --- | --- |
| Energy เริ่มต้น | 3 AP / วัน (`Balance.BaseEnergy`, กดเข้า QTE = −1) · สูงสุด 6 (`MaxEnergyCap`) · MaxEnergy = min(3 + EnergyUpgrade, 6) · เริ่มวันใหม่รีเซ็ตเป็น MaxEnergy |
| Coin เริ่มต้น | 150 |
| รายได้ | +100 ทุกเช้า · ชนะ Toothless +200 · ขายสัตว์ให้พ่อค้า +5,000 |
| หมอชุบชีวิต | **250 coin** → HP = 1 |
| Player EXP | ได้ **Great +1 / Perfect +2 ทุกครั้งที่กด QTE โดน** — EXP เต็ม = +1 Point (สูตร Max EXP ตาม [08-care-qte](08-care-qte.md) §3.3) |

### 3.3 Upgrade (ตาม Figma)

| สาย | ราคา | ผล |
| --- | --- | --- |
| QTE | 100 coin · 1 / 1 / 1 Point (เลเวล 1 / 2 / 3) | choice ของ QTE Train ใหญ่ขึ้น |
| Energy | 150 coin · 1 / 2 / 4 Points | Energy +1 ต่อวัน (และ +1 Energy ทันทีที่ซื้อ) |
| Progress Bar | 20 coin · 1 / 2 / 3 Points | progress ต่อครั้งของ Train +50% ต่อเลเวล (`TrainGainMultiplier` = 1 + 0.5 × level, max 3 level) |

> ราคา Point ต่อเลเวลอยู่ใน `Balance.QtePointCost/EnergyPointCost/ProgressPointCost` (ตัดสินใจ 2026-10-13: Energy เดิม 3 Points แพงจนไม่มีใครซื้อ)
> Upgrade scene บอกที่มาของ Point ("Points come from Player level-ups") และเหตุผลที่กดไม่ได้ ("Need N more points"); ครั้งแรกที่ได้ Point มีข้อความชี้ไป Upgrade station

> QTE: ขนาด zone ×(1 + 0.2 × level) สูงสุด 3 level

### 3.4 ร้านค้า + กล่องไอเทม (Day 3 หลัง Sell หรือ Refuse)

ซื้อแล้วของเข้า **กล่อง (Box)** ในฐาน (`BaseScene.Kind.Box`, `BoxX` = 600 ระหว่างเตียงกับ Upgrade) ใช้ทีหลังผ่าน `InventoryScene` (UI การ์ดแบบเดียวกับร้าน, ปุ่ม Use) ถือได้ไม่เกิน `Balance.ShopMaxOwned` = 3 ต่อชนิด (กันกักตุน Tonic ด้วยเงิน 5,000)

| ไอเทม | ราคา | ผล (ตอนกด Use) |
| --- | --- | --- |
| Crab Apple | 25 | เลือกสัตว์ 1 ตัว: +18 HP, +20 Stomach |
| Caffeine Tonic | 40 | Energy +2 แต่ไม่เกิน MaxEnergy (Use ไม่ได้ตอน Energy เต็ม) |
| Sea Tea | 18 | Dodge zone กว้างขึ้น +20% ใน Fight ครั้งถัดไป |
| Cloudy Glasses | 30 | กันดาเมจ 2 ครั้งใน Fight ครั้งถัดไป (ที่เหลือหมดเมื่อจบ Fight) |
| Ballet Shoes | 50 | Perfect zone ของ care QTE +15% วันนี้; คืนนี้ Stomach ลดเพิ่ม 5 |
| Torn Notebook | 55 | Progress จาก Train +50% วันนี้ (บวกกับ upgrade แบบ additive) |
| Faded Ribbon | 35 | Clean ลดน้อยลง 30% คืนนี้ |

> บัฟรายวัน (Ballet/Notebook/Ribbon) หมดตอน End day (`GameState.AdvanceDay`); ใช้ซ้ำตอนยังมีผลไม่ได้

### 3.5 กฎสเตตัส (ตาม Figma)

- เริ่มวันใหม่: Stomach −20, Clean −15
- Stomach = 0 → −20 HP ทุก End day
- Clean ≤ 50 → ATK ลดลง (×0.75) · Clean ≤ 25 → Max HP ลดลง (×0.8)

### 3.6 Care / QTE และสูตรเลเวล (As built)

- QTE ทุกแบบ `Attempts` = 10 ครั้ง · ทุกครั้งที่กดโดน Player EXP Great +1 / Perfect +2
- ต่อครั้ง Perfect / Great: Train Progress +10 / +5 (× `TrainGainMultiplier`) · Feed Stomach +6 / +3 · Clean +6 / +3 · Heal HP +6 / +3
- Stomach ที่เผาผลาญตอนเริ่ม QTE: Train −10 · Clean/Heal −5 · Feed 0
- Max HP = (BaseMaxHp + (Lv−1)×10) × (Clean ≤ 25 ? 0.8 : 1) · ATK = round((BaseAtk + (Lv−1)×5) × (Clean ≤ 50 ? 0.75 : 1))
- MaxProgress = 100 + (Lv−1)×50 · PlayerMaxExp = 10 + (Lv−1)×10
- Level-up ของสัตว์: +10 Max HP, +5 ATK (`HpPerLevel`, `AtkPerLevel`)

**ความเร็วเข็ม / ขนาด zone (half-width เป็น radian, `Balance`):**

| รายการ | ค่า |
| --- | --- |
| เข็ม Care select / ฐาน | 2.2 · zone Perfect 0.62 (ไม่มี Great) |
| เข็ม QTE ปกติ | 2.4 (`QteBaseSpeed`) · Train **3.6** |
| Zone Perfect / Great | Train 0.07 / 0.16 (หด −15% ต่อ Pet Level จนถึง Lv 3) · Feed/Clean/Heal 0.12 / 0.27 |
| Feed | Perfect = เข็มเร็วขึ้น ×1.15 (สูงสุด 6) และกลับทิศ · Miss = รีเซ็ตเป็น 2.4 |
| Clean | zone วิ่งหนี 4.8 rad/s |
| Heal | เข็มจาง + zone หดและเกิดใหม่ |
| Train | มี dot 3 จุดพร้อมกัน (โดนแล้วเกิดใหม่) |
| Ballet Shoes | Perfect zone ×1.15 ทุก care QTE |

**Fight (`FightScene`):** เริ่มด้วย Attack (เขียว) → ตีโดนแต่ไม่ตาย = สลับเป็น Dodge (เหลือง, Perfect เท่านั้น) → Dodge สำเร็จ = กลับ Attack · พลาดหรือ "Too slow" (zone หายที่ 0.012) = ศัตรูตี · Dodge เริ่ม 0.32 (Sea Tea 0.38) แล้วหดตามศัตรู · Attack หดครึ่งเดียวของ Dodge (Perfect ต่ำสุด 0.06, Great 0.16) · เข็มศัตรู Toothless 2.4 / Big Z 3.4 (DodgeShrink 0.10 / 0.26) · ดาเมจสัตว์ = ATK × (Perfect 1.0 / Great 0.75) · Toothless ATK 15 HP 120, Big Z ATK 20 HP 9999

---

## 4. Scenes ใน Prototype

ใช้ Scene 1–20 ตาม [13-scene-breakdown.md](13-scene-breakdown.md) โดยเปลี่ยนแปลงดังนี้:
- **Scene 3 (Base):** เปลี่ยนเป็นฉาก side-scrolling เดินได้ (ดูข้อ 2)
- **Scene 4–8 (Care + QTE):** ตาม Figma — สีวงล้อ Train แดง / Feed เหลือง / Clean ฟ้า / Heal เขียว
- **Scene 18 (Fight):** Dodge (Perfect อย่างเดียว หดสั้นลง) / Attack (Perfect 100% / Great 75% ATK) สลับสัตว์ได้เมื่อสัตว์ตาย ตายหมด = Game Over (ยกเว้น Big Z → To be continued)
- **เพิ่ม:** หน้าร้านค้า (Day 3), หน้าข้อความ Thunderstorm (Day 4), หน้า "To be continued..." (Day 5)
- ภาษาในเกม: **อังกฤษทั้งหมด**

---

## 5. Out of Scope (Prototype)

Save/Load · Daily Summary Report Card · Burn / Infected · Toy Knife (ไอเทมชิ้นที่ 8 ใน Figma/10-economy) · Endless random event · ท่าโจมตีใหม่ของสัตว์ Lv 3 · Combo Streak

> เปลี่ยน 2026-10-13: ไอเทม 7 ชิ้น + กล่อง (§3.4) และหน้า Options (Music/SFX volume, เปิดจาก Main Menu และ Pause) **ทำแล้ว** จึงเอาออกจากรายการนี้

## 6. ลำดับการตัดถ้าเวลาไม่พอ

1. สมุด (Scene 9–11) → 2. Upgrade → 3. ร้านค้า → 4. หมอ

**ห้ามตัด:** core loop · QTE 4 แบบ · Fight · Day 1–5 จนถึงฉากจบ Big Z
