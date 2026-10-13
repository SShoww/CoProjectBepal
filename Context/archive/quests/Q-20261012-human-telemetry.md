### 2026-10-12 — Q-20261012-human-telemetry — วิเคราะห์ telemetry ผู้เล่นจริง 8 run
- Status: ready-to-close
- Owner: Claude Code (ตามคำสั่งผู้ใช้ 2026-10-12)
- Goal: นำ raw telemetry คนจริง (mode=play, build 15ac821) เข้า repo -> Aggregate + validate -> เทียบกับ bot ใน balance-report -> อัปเดตรายงาน (ไม่แก้ Balance/โค้ดเกม)
- Files: BEPAL/Docs/Balance/data-human/** และ data-human-raw/** (เฉพาะในเครื่อง, gitignore), .gitignore, BEPAL/Docs/Balance/analysis/human/**, BEPAL/Docs/Balance/tools/Aggregate/Program.cs, BEPAL/Docs/Balance/balance-report.md
- Context: [balance-report.md](../../BEPAL/Docs/Balance/balance-report.md), [telemetry-spec.md](../../BEPAL/Docs/Balance/telemetry-spec.md) sec 5; ต่อจาก Q-20261009-run-telemetry (ready-to-close, ไม่แตะ)
- Validation (2026-10-12): Aggregate -> 8 runs (analysis/human/); validate.ps1 (สำเนา path ใหม่) Boom_121 fail 8 invariant จาก bug telemetry (`_gs` ผูก GameState แรก) -> ซ่อมไฟล์ด้วยสคริปต์ (ต้นฉบับใน data-human-raw/) แล้ว validate ใหม่ 8/8 ผ่าน 15 invariant; ไม่ได้ build/รันเกม; ไม่ได้แก้โค้ดเกม/Balance. สาเหตุ Boom ยืนยันโดยผู้ใช้: กด Main Menu แล้วเริ่มใหม่
- Next action: ผู้ใช้ยืนยันปิด; งานต่อ (นอก scope นี้) ดู balance-report.md §7.5 C9-C11: แก้ Telemetry.RunStart, แก้ validate.ps1, ถามผู้เล่น, เก็บ run เพิ่มให้ถึง 15-20
- Decision (2026-10-12): ไฟล์ jsonl ดิบมีชื่อผู้เล่น -> ไม่ commit/push (ผู้ใช้สั่ง ignore); ผลวิเคราะห์ analysis/human/ ยังมีชื่อผู้เล่น
