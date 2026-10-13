### 2026-10-13 — Q-20261013-context-housekeeping — ปิด 5 quest ค้าง + อัปเดต Status
- Status: ready-to-close
- Owner: Claude Code (ตามคำสั่งผู้ใช้ 2026-10-13)
- Goal: ย้าย 5 quest `ready-to-close` ไป `archive/quests/`; เขียน `Context/Status.md` ใหม่ให้ตรง `Develop` @ `29a9060`
- Files: Context/quests/**, Context/archive/quests/**, Context/Status.md
- Context: แผนวิเคราะห์จุดอ่อน 2026-10-13 (อยู่ใน Status.md)
- Validation: ตรวจ path ต้นทาง/ปลายทางแล้ว (`git mv` ภายใน Context/quests → Context/archive/quests)
- Next action: ผู้ใช้ยืนยันปิด (ย้ายไป archive); ถัดไป Q-20261013-balance-migration
