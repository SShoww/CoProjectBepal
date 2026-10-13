---
type: gdd-economy
version: 1.1
date: 2026-10-13
---

# BePal — Sanctuary Economy & Items

> **หมายเหตุ:** ปรับให้ตรงกับโค้ดแล้วเมื่อ 2026-10-07 — [06-vertical-slice.md](06-vertical-slice.md) และโค้ดยังเป็นข้อกำหนดหลักของ Prototype

ระบบเงินและแคตตาล็อกไอเทม — เนื้อหาย้ายแบบคงข้อความเดิมจากไฟล์ต้นทาง

ที่มา: แยกจาก [03-mechanics.md](03-mechanics.md) เมื่อ 2026-10-07 · ขอบเขต Prototype ตาม [06-vertical-slice.md](06-vertical-slice.md)

---

## Section 5: Sanctuary Economy & Resources

### 5.1 Currency Economy Model (ระบบการไหลเวียนของเงิน)
- **Starting Wallet:** เริ่มต้นเกม Day 1 ด้วยเงิน 150 Gold
- **Daily Subsidy:** รับเงินสนับสนุน +100 Gold ทุกเช้าของวันใหม่
- **Care Performance Bonus:** *(ยังไม่ implement — ไม่มีระบบเกรด/Report Card ใน Prototype)* โบนัส 10–50 Gold ตามเกรดการดูแล (S–C)
- **Endless Event Rewards:** *(ยังไม่ implement — ไม่มี RNG Events)* แหล่งรายได้จริงใน Prototype: Daily +100 · ชนะ Toothless +200 (`Balance.ToothlessReward`) · ขายสัตว์ให้พ่อค้า Day 3 +5,000 (ขายตัวสุดท้ายไม่ได้)
- หน่วยเงินในเกมแสดงเป็น "coin" (เอกสารเดิมใช้ Gold/G)
- **ราคา Upgrade:** QTE 100 · Energy 150 · Progress 20 coin (+ Points ดู [03 §1.3](03-mechanics.md))
- **Emergency Revive:** ค่าบริการกู้ชีพของคุณหมอ 250 Gold หากเงินไม่พอจ่ายและไม่มีสัตว์เลี้ยงตัวอื่นเหลืออยู่ จะเข้าสู่สถานะ Game Over ทันที

### 5.2 Item Catalog (สินค้าและไอเทมใช้งาน)

> As built (2026-10-13): ซื้อแล้วเข้า inventory (Box ในฐาน) ใช้ทีหลังผ่าน `InventoryScene`; ถือได้ ≤ 3 ต่อชนิด; ร้านเปิดหลัง Sell/Refuse ของพ่อค้า Day 3 รายละเอียด effect ที่ implement แล้ว (Crab Apple, Tonic, Sea Tea, Cloudy Glasses, Ballet Shoes, Torn Notebook, Faded Ribbon) ดู [06-vertical-slice.md §3.4](06-vertical-slice.md) — Toy Knife ยังไม่ทำ; Sea Tea ใน build นี้ = Fight ครั้งถัดไป (ไม่ใช่ 1 วัน)

| รายการสินค้า       | ราคา | ประเภท         | คุณสมบัติและการใช้งาน                                                                                |
| ------------------ | ---- | -------------- | ---------------------------------------------------------------------------------------------------- |
| **Crab Apple**     | 25 G | อาหารพิเศษ     | ฟื้นฟู +18 HP และเพิ่ม +20 Stomach ทันทีโดยไม่เสีย Energy (AP)                                       |
| **Sea Tea**        | 18 G | เครื่องดื่มบัฟ | ขยายความกว้างของ Dodge Zone ขึ้น +20% (0.32 → 0.38) ใน **Fight ครั้งถัดไป** (ไม่ใช่ 1 วัน)           |
| **Cloudy Glasses** | 30 G | ไอเทมใช้งาน    | มอบโล่ป้องกัน (Invulnerability Shield) ให้สัตว์เลี้ยง 2 ครั้ง ในฉากต่อสู้รอบถัดไป                    |
| **Torn Notebook**  | 55 G | ตำราวิจัย      | บูสต์หลอด Progress จากการ Train +50% เป็นเวลา 1 วัน (ย้ายอัปเกรดแบบถาวรไปไว้ที่ Upgrade Station แทน) |
| **Caffeine Tonic** | 40 G | ยาชูกำลัง      | ฟื้นฟูแต้มพลังงานให้ผู้เล่น +2 AP ทันทีในวันนั้น แต่ไม่เกิน Max Energy (ซื้อได้เสมอ แต่ **Use** ไม่ได้ตอน Energy เต็ม)                                                     |
| **Ballet Shoes**   | 50 G | ของเล่นบัฟ     | หน้าต่าง Perfect Zone กว้างขึ้น +15% เป็นเวลา 1 วัน (แลกกับ Stomach ลดเร็วขึ้น +5)                   |
| **Toy Knife**      | 45 G | ของเล่นบัฟ     | *(ยังไม่ implement — ไม่มีใน `Items.cs`)* เพิ่มพลังโจมตีสวนกลับ (Counter Damage) ในการต่อสู้รอบถัดไป +35% |
| **Faded Ribbon**   | 35 G | ของประดับบัฟ   | อัตราความสะอาดลดลงช้าลง (Clean Decay -30%) เป็นเวลา 1 วัน                                            |
