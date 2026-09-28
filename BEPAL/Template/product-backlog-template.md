<!-- Template เต็มไฟล์สำหรับสร้าง docs/agile/01-product-backlog.md -->
<!-- ดึง User Story และ Technical Task ทั้งหมดของโปรเจกต์มาใส่ที่นี่ -->

# Product Backlog

> รวม User Story ทั้งหมดของโปรเจกต์ — ยังไม่ได้แปลว่าต้องทำใน Sprint นี้ทั้งหมด
> โปรเจกต์นี้แบ่งงานตลอดเทอมเป็น **3 Sprint** (Sprint 1-3, สิ้นสุดก่อน 12 ต.ค. 2026) — Sprint ไหนหยิบ Story ไปทำ ให้ใส่เลข Sprint นั้น (1-3) ลงคอลัมน์ `Sprint`

## Must Have (MVP)

| # | User Story | Acceptance Criteria | Estimate (SP) | Sprint |
|---|---|---|---|---|
| 1 | As a player, I want to jump, so that I can avoid obstacles | กดปุ่มกระโดดแล้วตัวละครลอยขึ้นแล้วตกลงพื้นเดิม ไม่ทะลุพื้น | 5 | 1 |
| 2 | [As a ..., I want ..., so that ...] | [เงื่อนไขที่นับว่า Story นี้ "เสร็จจริง"] | [SP] | [เลข sprint หรือ —] |

## Should Have

| # | User Story | Acceptance Criteria | Estimate (SP) | Sprint |
|---|---|---|---|---|
| 1 | As a player, I want to see my remaining lives, so that I know how close I am to game over | จำนวนชีวิตแสดงบนจอตลอดเวลา ลดลงทันทีที่โดนโจมตี | 2 | — |
| 2 | [As a ..., I want ..., so that ...] | [เงื่อนไขที่นับว่า Story นี้ "เสร็จจริง"] | [SP] | — |

## Nice to Have (Could Have)

| # | User Story | Acceptance Criteria | Estimate (SP) | Sprint |
|---|---|---|---|---|
| 1 | As a designer, I want enemy spawn rate stored in a data file, so that I can tune difficulty without recompiling | ปรับค่า spawn rate ในไฟล์ data แล้วรันเกมใหม่ ค่าที่เปลี่ยนมีผลทันทีโดยไม่ต้อง build ใหม่ | 3 | — |
| 2 | [As a ..., I want ..., so that ...] | [เงื่อนไขที่นับว่า Story นี้ "เสร็จจริง"] | [SP] | — |

---

## Story Point Estimation Reference
ใช้ Modified Fibonacci: `1, 2, 3, 5, 8, 13, 20`
- **1 SP:** งานเล็กมาก ไม่เกิน 1-2 ชั่วโมง เช่น เพิ่ม UI text, ปรับตัวเลข balance
- **2 SP:** งานเล็ก ครึ่งวัน เช่น เพิ่ม sound effect, สร้าง prefab ง่ายๆ
- **3 SP:** งานขนาดกลาง 1 วัน เช่น ทำ mechanics ย่อย 1 ชิ้น, ออกแบบ UI 1 หน้าจอ
- **5 SP:** งานขนาดใหญ่ 2-3 วัน เช่น Core mechanic 1 ระบบ, หน้าจอพร้อม logic
- **8 SP:** งานใหญ่มาก ต้องแบ่งเป็น task ย่อย หรือทำทั้งสัปดาห์
- **13+ SP:** ใหญ่เกินไปสำหรับ 1 Story — **ต้องแตกเป็นหลาย Story ก่อนนำเข้า Sprint**
