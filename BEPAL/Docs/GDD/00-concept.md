---
type: gdd-concept
version: 2.2
date: 2026-10-07
---

# BePal — Game Concept

> **หมายเหตุ:** ปรับให้ตรงกับโค้ดแล้วเมื่อ 2026-10-07 — [06-vertical-slice.md](06-vertical-slice.md) และโค้ดยังเป็นข้อกำหนดหลักของ Prototype

## Elevator Pitch

**“BePal is a hardcore pet-care management simulation where domestic warmth collides with uncanny survival peril: care for abnormal creatures through rigorous time and energy budgeting, survive unexpected environmental disasters and lethal wild incursions, and confront moral dilemmas when shady merchants come knocking.”**

> **สโลแกนแนวคิด:** เกมจำลองการบริหารและดูแลสัตว์เลี้ยงผิดปกติ (Abnormal Pets) ที่ผสมผสานความน่ารักอบอุ่นของสัตว์เลี้ยง (Virtual Pet) เข้ากับความท้าทายและการเอาตัวรอดสุดขั้ว (Hardcore Survival) ทุกการตัดสินใจใช้พลังงานและเงินตรามีผลลัพธ์ถึงชีวิต ปกป้องพวกมันจากภัยพิบัติ สัตว์ป่าดุร้าย และข้อเสนออันดำมืดของพ่อค้าเร่

> **เกมเราคือเกมอะไร (จาก Figma — 01 · Game Overview):** "BePal คือเกมแนว **Endless Survival Management** ที่ผู้เล่นจะได้รับบทเป็นผู้ดูแลสถานพักพิงสัตว์อวกาศเพียงลำพังบนดาวเคราะห์สุดขอบจักรวาล ต้องบริหารพลังงานที่มีจำกัดในแต่ละวันเพื่อป้อนอาหาร ทำความสะอาด และรับมือกับภัยคุกคามปริศนาที่มาเคาะ 'ประตูแดง' หน้าฐาน"

---

## Genre, Platform & Controls

- **Genre:** Hardcore Pet-Care Simulation | Resource Management | QTE Reflex Combat | Narrative Choice
- **Platform:** PC (Windows DesktopGL)
- **Engine:** MonoGame (.NET 8 C# 12)
- **Controls (การควบคุม):**
  - `W` `A` `S` `D` หรือ `Mouse`: เลือกเมนูและโต้ตอบ (Interact) กับสิ่งต่างๆ ภายในห้อง
  - `Spacebar`: กดแอ็กชันวงล้อ QTE, ปัดป้อง/หลบหลีก (Dodge), และยืนยันการเลือก
  - เดินไปที่เตียงแล้วกด `Space`/`Enter`: สิ้นสุดวัน (End Day) ข้ามไปยังช่วงค่ำ/สรุปวัน · เดินไปที่ประตูแล้วกด `Space`: เปิดรับเหตุการณ์หน้าประตู ("Knock Knock !!")
  - `1` `2` `3` `4`: ชอร์ตคัตคำสั่งดูแล (Train, Feed, Clean, Heal)
  - `B` / `S`: เปิดกระเป๋าเก็บไอเทม (Backpack 8 ช่อง) / เปิดร้านค้าพ่อค้าเร่ (Shop)
- **Target Audience:** ผู้เล่นที่ชื่นชอบเกมดูแลสัตว์เลี้ยงสไตล์คลาสสิก (Tamagotchi / Digimon V-Pet) แต่ต้องการความตื่นเต้นระทึกขวัญ ความเสี่ยงสูง (High Stakes) และระบบการต่อสู้หลบหลีกที่ต้องอาศัยทักษะความแม่นยำ

---

## Game Pillars (จาก Figma)

| Pillar | ความหมาย |
| --- | --- |
| **Observation & Understanding** | การสังเกตพฤติกรรมและทำความเข้าใจกฎเกณฑ์เฉพาะตัวของสัตว์คือหัวใจสำคัญ |
| **Cozy yet Dangerous** | บรรยากาศดูปลอดภัยและอบอุ่น ขัดแย้งกับความอันตรายที่ซ่อนอยู่ในตัวสัตว์เลี้ยง |
| **Consequential Interaction** | ทุกการกระทำมีผลลัพธ์ที่ชัดเจนและแตกต่างกันไปตามบริบท |

## Core Pillars (Systems)

1. **Care × Hardcore Peril (การดูแลที่แฝงความตายรอบด้าน):**
   ไม่ใช่แค่การเลี้ยงสัตว์เพื่อความเพลิดเพลิน แต่เป็นสมรภูมิการจัดสรรทรัพยากรเพื่อความอยู่รอด สัตว์เลี้ยงมีค่าสเตตัสความต้องการพื้นฐาน (Stomach, Clean, Health) ที่ลดลงอย่างต่อเนื่อง และสามารถล้มป่วย บาดเจ็บ หรือเสียชีวิตได้จริงหากผู้เล่นละเลย

2. **Discrete Energy & Resource Economy (การบริหารพลังงานและเศรษฐกิจที่จำกัด):**
   ในแต่ละวัน ผู้เล่นได้รับโควตาพลังงานจำกัด (**เริ่มต้น 3 AP อัปเกรดได้สูงสุด 6 AP**) ทุกการกระทำ—ให้อาหาร ทำความสะอาด ฝึกซ้อม หรือรักษาพยาบาล—ล้วนกินพลังงาน การใช้เงิน (Gold) ซื้ออาหาร ยารักษา หรืออุปกรณ์เสริมจากร้านค้าต้องผ่านการคำนวณอย่างรอบคอบ

3. **Precision QTE Mechanics (ความแม่นยำในการตอบสนอง):**
   การดูแลทุกหมวดหมู่และการเผชิญหน้าในสถานการณ์ต่อสู้ควบคุมผ่านระบบ Quick Time Events (QTE) 10 ครั้งต่อรอบ ที่มีทั้ง **Perfect Zone** ($\pm 0.20 \text{ rad}$) และ **Great Zone** ($\pm 0.45 \text{ rad}$) (Figma ใช้คำว่า *Great* แทน *Good*) วงล้อแต่ละแบบมีลูกเล่นต่างกัน (Train/Feed/Clean/Heal) รวมถึงระบบหลบหลีกการโจมตี (Dodge QTE) และการโจมตี (Attack)

4. **Consequential Moral Dilemmas (ทางเลือกและผลลัพธ์เชิงจริยธรรม):**
   เนื้อเรื่องขับเคลื่อนด้วยทางเลือกที่มีน้ำหนักจริง เช่น การตัดสินใจว่าจะยอมเสี่ยงชีวิตฝึกสัตว์ป่าดุร้าย หรือขับไล่มันไป และการเลือกว่าจะยอมขายสัตว์เลี้ยงที่ร่วมทุกข์ร่วมสุขมาเพื่อเงินก้อนโต หรือยืนหยัดปกป้องพวกมันจนนำไปสู่การปะทะกับบอส

---

## Setting & Narrative

→ ดู [11-narrative-world.md](11-narrative-world.md) (ย้ายไปไฟล์แยก 2026-10-07)

---

## Inspiration & Competitor Analysis

→ ดู [12-references-art-direction.md](12-references-art-direction.md) (ย้ายไปไฟล์แยก 2026-10-07)

---

## Art & Visual Direction

→ ดู [12-references-art-direction.md](12-references-art-direction.md) (ย้ายไปไฟล์แยก 2026-10-07)
