# Agent Handoff — Active Quest Board

Board = โฟลเดอร์ [quests/](quests/) หนึ่งไฟล์ต่อหนึ่งงานที่ยังไม่ปิด (`quests/Q-ID.md`) ไม่มีไฟล์ Board รวม
กติกาหลาย agent และเกณฑ์จบอยู่ที่ [Rules.md](Rules.md)

## ดู Board (อ่านแค่หัวข้อ สถานะ เจ้าของ)
```sh
rg -N "^### |^- (Status|Owner):" Context/quests
```
แล้วเปิดรายละเอียดเฉพาะ Quest ที่เกี่ยวข้องหรือ Files ทับกับงานของคุณ

## สถานะ
| Status | ความหมาย |
|---|---|
| `active` | กำลังทำงาน |
| `waiting` | รอข้อมูล การตัดสินใจ หรือการรับช่วง |
| `blocked` | ไปต่อไม่ได้ — ต้องระบุ blocker และ next action |
| `ready-to-close` | ผ่านเกณฑ์และบันทึก Log แล้ว รอผู้ใช้ยืนยันปิด |

ปิดแล้ว → ย้ายไฟล์ไป [archive/quests/](archive/quests/) (หลังผู้ใช้ยืนยัน)

## ตั้ง ID
`Q-YYYYMMDD-short-name` (วันที่สร้าง + ชื่อสั้นภาษาอังกฤษ kebab-case) ตรวจก่อนว่าไม่ชนกับ `quests/` และ `archive/quests/`

## Template (กระชับ 8–12 บรรทัด หลักฐานยาวเก็บแยกแล้วลิงก์)
```markdown
### YYYY-MM-DD — Q-ID — ชื่องาน
- Status: active
- Owner: ชื่อ agent / chat หรือ thread ID ที่ทราบจริง
- Goal: ผลลัพธ์และเกณฑ์ตรวจรับ
- Files: path ที่รับผิดชอบอย่างชัดเจน ไม่ใช้คำว่า all
- Context: ลิงก์เอกสารหรือหลักฐานที่จำเป็น
- Validation: ผลตรวจจริง หรือระบุว่ายังไม่ได้ตรวจ
- Next action: ขั้นตอนถัดไปและสิ่งที่ต้องรอ
```

## รับช่วงงาน
1. ผู้ใช้สั่งให้รับช่วง Quest ใด → อ่านไฟล์ Quest ล่าสุด
2. แก้เฉพาะบรรทัด `Owner` (และ `Status` ถ้าเปลี่ยน) พร้อมระบุ "รับช่วงจาก ... ตามคำสั่งผู้ใช้ YYYY-MM-DD" ใน Next action
3. ทำต่อจาก Next action — ไม่อ้างผลตรวจเดิมเป็นผลใหม่
