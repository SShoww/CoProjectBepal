### 2026-10-12 — Q-20261012-telemetry-restart — Telemetry: เริ่มเกมใหม่ในเซสชันเดียว (C9)
- Status: ready-to-close
- Owner: Claude Code (ตามคำสั่งผู้ใช้ 2026-10-12)
- Goal: กด Main Menu แล้วเริ่มเกมใหม่ (หรือเริ่มใหม่หลัง game over / to be continued) ต้องได้ 1 ไฟล์ต่อ 1 เกม โดย `day`/summary อ่านจาก GameState ของเกมนั้น ไม่ผูกกับเกมแรก (บั๊กของ Boom_121)
- Files: BEPAL/Bepal_Game/Bepal/Core/Telemetry.cs, BEPAL/Docs/Balance/telemetry-spec.md, BEPAL/Docs/Balance/balance-report.md (แก้ C9 เท่านั้น)
- Context: [balance-report.md](../../BEPAL/Docs/Balance/balance-report.md) §7.1/§7.5 C9; ต่อจาก Q-20261012-human-telemetry (ไม่แตะไฟล์ของ Quest นั้น)
- Validation (2026-10-12): `dotnet build Bepal_Game.slnx` 0 warning/0 error; `--autoplay` และ `--autoplay --refuse` -> reached To be continued; `--autoplay --bot human --seed 7 --telemetry` -> 1 ไฟล์ 1 run_summary; harness ชั่วคราว (นอก repo) เรียก RunStart 4 เกมติดกัน: เกมที่ไม่มี event ถูกทิ้ง, เกมกลางทางปิดด้วย quit dayReached=2 ถูกต้อง, เริ่มใหม่หลัง game_over ได้ไฟล์ใหม่, ชื่อซ้ำได้ suffix -2/-3. ยังไม่ได้ทดสอบด้วยการกด Main Menu ในเกมจริง (GUI)
- Next action: ผู้ใช้ยืนยันปิด; ทดสอบจริงด้วยการเล่น -> Esc -> Main Menu -> เริ่มใหม่ แล้วดูว่าได้ไฟล์แยก; ตัดสินเรื่อง push ข้อมูลผู้เล่น (ดู Q-20261012-human-telemetry)
