### 2026-10-13 — Q-20261013-balance-migration — ย้ายตัวเลข care/shop/EXP เข้า Balance
- Status: ready-to-close
- Owner: Claude Code (ตามคำสั่งผู้ใช้ 2026-10-13)
- Goal: ตัวเลขใน CareScenes/ShopScene/GameState อยู่ใน `Balance` โดยพฤติกรรมเท่าเดิมทุกตัว
- Files: BEPAL/Bepal_Game/Bepal/Model/GameState.cs, BEPAL/Bepal_Game/Bepal/Scenes/CareScenes.cs, BEPAL/Bepal_Game/Bepal/Scenes/BaseMenus.cs, CLAUDE.md
- Context: Known deviations ใน CLAUDE.md (ลบส่วน care/shop/PlayerMaxExp ออกแล้ว; เหลือ MaxProgress, Pet.cs thresholds, NightScene, Toothless Hp 60)
- Validation (2026-10-13): `dotnet build` 0 warning/0 error; `run-batch.ps1 -Seeds 10` (5 profile = 50 run) ก่อน/หลัง: event stream ทุก run เหมือนกันทุกฟิลด์ (ยกเว้น runId/เวลาจริง/build); `--autoplay --refuse` -> reached To be continued. ไม่ได้รัน `--shots` (ไม่แตะ UI)
- Next action: ผู้ใช้ยืนยันปิด; ถัดไป Q-20261013-energy-ux
