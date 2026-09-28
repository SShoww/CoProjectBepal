---
type: gdd-vertical-slice
version: 1.0
date: 2026-09-28
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
| **3** | **พ่อค้า** | ขอซื้อ Toothless (ถ้าไม่มี Toothless จะขอซื้อ starter แทน) — **ขาย** = ได้ **5,000 coin** และสัตว์ตัวนั้นหายไป (ขายตัวสุดท้ายไม่ได้) / **ปฏิเสธ** = ไม่เกิดอะไร → เปิด **ร้านค้า** |
| **4** | **Thunderstorm** | ข้อความแจ้งพายุ → Clean ของสัตว์ทุกตัว **−50** (ไม่ต่ำกว่า 0) + ปลดล็อกหน้า Disaster ในสมุด |
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
| **Toothless** (ศัตรู → สัตว์เลี้ยง) | — | 120 | 15 | 60 / 60 (หลัง Tame) |
| **Big Z** (บอส) | — | 9999 | 40 | — |

> Passive ของสัตว์ **ไม่อยู่ใน Prototype** (เตรียมช่องในโค้ดไว้ — รอคุยกันเรื่อง Skill ของสัตว์)

### 3.2 ทรัพยากร

| รายการ | ค่า |
| --- | --- |
| Energy เริ่มต้น | 3 AP / วัน (กดเข้า QTE = −1) |
| Coin เริ่มต้น | 150 |
| รายได้ | +100 ทุกเช้า · ชนะ Toothless +200 · ขายสัตว์ให้พ่อค้า +5,000 |
| หมอชุบชีวิต | **250 coin** → HP = 1 |
| Player EXP | ได้ **+1 ทุกครั้งที่กด QTE โดน** (Perfect หรือ Great) — EXP เต็ม = +1 Point (สูตร Max EXP ตาม [03-mechanics](03-mechanics.md) §3.3) |

### 3.3 Upgrade (ตาม Figma)

| สาย | ราคา | ผล |
| --- | --- | --- |
| QTE | 100 coin · 1 Point | choice ของ QTE Train ใหญ่ขึ้น |
| Energy | 150 coin · 3 Points | Energy +1 ต่อวัน |
| Progress Bar | 20 coin · 2 Points | progress ต่อครั้งของ Train +10 → +15 |

### 3.4 ร้านค้า (Day 3 หลังปฏิเสธพ่อค้า — ซื้อแล้วใช้ทันที ไม่มีกระเป๋า)

| ไอเทม | ราคา | ผล |
| --- | --- | --- |
| Crab Apple | 25 | สัตว์ 1 ตัว: +18 HP, +20 Stomach |
| Caffeine Tonic | 40 | Energy +2 วันนี้ |
| Sea Tea | 18 | Dodge zone กว้างขึ้น +20% ใน Fight ครั้งถัดไป |

### 3.5 กฎสเตตัส (ตาม Figma)

- เริ่มวันใหม่: Stomach −20, Clean −15
- Stomach = 0 → −20 HP ทุก End day
- Clean ≤ 50 → ATK ลดลง (×0.75) · Clean ≤ 25 → Max HP ลดลง (×0.8)

---

## 4. Scenes ใน Prototype

ใช้ Scene 1–20 ตาม [01-core-loop.md](01-core-loop.md) โดยเปลี่ยนแปลงดังนี้:
- **Scene 3 (Base):** เปลี่ยนเป็นฉาก side-scrolling เดินได้ (ดูข้อ 2)
- **Scene 4–8 (Care + QTE):** ตาม Figma — สีวงล้อ Train แดง / Feed เหลือง / Clean ฟ้า / Heal เขียว
- **Scene 18 (Fight):** Dodge (Perfect อย่างเดียว หดสั้นลง) / Attack (Perfect 100% / Great 75% ATK) สลับสัตว์ได้เมื่อสัตว์ตาย ตายหมด = Game Over (ยกเว้น Big Z → To be continued)
- **เพิ่ม:** หน้าร้านค้า (Day 3), หน้าข้อความ Thunderstorm (Day 4), หน้า "To be continued..." (Day 5)
- ภาษาในเกม: **อังกฤษทั้งหมด**

---

## 5. Out of Scope (Prototype)

Save/Load · Daily Summary Report Card · Passive ของสัตว์ · Burn / Infected · ไอเทมอีก 5 ชิ้นและกระเป๋า · Endless random event · Option (ปุ่มมีแต่ยังไม่ทำงาน)

## 6. ลำดับการตัดถ้าเวลาไม่พอ

1. สมุด (Scene 9–11) → 2. Upgrade → 3. ร้านค้า → 4. หมอ

**ห้ามตัด:** core loop · QTE 4 แบบ · Fight · Day 1–5 จนถึงฉากจบ Big Z
