### 2026-10-14 — Q-20261014-toothless-defeat — แพ้ Toothless แล้วกลับฐานไปชุบกับหมอ (ไม่ Game Over)
- Status: ready-to-close
- Owner: Claude Code (ตามคำสั่งผู้ใช้ 2026-10-14)
- Goal: เมื่อสัตว์ทุกตัวตายตอนสู้ Toothless (Day 2, Tame) ให้กลับ BaseScene แล้วไปชุบกับ Doctor แทน GameOverScene
- Files: BEPAL/Bepal_Game/Bepal/Scenes/DayEvents.cs
- Context: FightScene ถอดตัวเองก่อนเรียก callback → ไม่ต้อง Reset; NightScene ยัง GameOver เมื่อสัตว์ตายหมดและ coin < ReviveCost ตอนจบวัน (กัน soft-lock)
- Validation (2026-10-14): build 0 warning/0 error; `--autoplay` และ `--autoplay --refuse` -> reached To be continued. ยังไม่ได้ทดสอบเส้นทางแพ้ Toothless ด้วยมือ/บอทจริง
- Next action: ผู้ใช้ลองแพ้ Toothless แล้วเดินไปหา Doctor; ยืนยันปิด quest
