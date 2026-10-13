### 2026-10-13 — Q-20261013-scene-jump — ระบบกระโดดไป scene ใดก็ได้ไว้ test (--scene, F2)
- Status: closed
- Owner: Claude Code (ตามคำสั่งผู้ใช้ 2026-10-13)
- Goal: เปิด/ย้ายไปแต่ละ scene ได้ทันทีโดยไม่ต้องเล่นผ่าน Day 1–N
- Files: BEPAL/Bepal_Game/Bepal/DevTools/SceneCatalog.cs, BEPAL/Bepal_Game/Bepal/DevTools/SceneJumpScene.cs, BEPAL/Bepal_Game/Bepal/DevTools/ShotRunner.cs, BEPAL/Bepal_Game/Bepal/Game1.cs, CLAUDE.md, BEPAL/Docs/GDD/04-class-diagram.md
- Context: ย้ายรายการ setup ของ ShotRunner ไป SceneCatalog (ชื่อ shot เดิม); `--scene <name>` (substring ได้, ไม่ตรง = พิมพ์รายชื่อ, ข้าม telemetry); F2 เปิดเมนูกระโดดเฉพาะ Debug build หรือ `--dev`/`--scene`; scene ใหม่เพิ่มบรรทัดเดียวใน SceneCatalog
- Validation (2026-10-13): build 0 error/0 warning; `--shots` ได้ PNG 28 ไฟล์ ชื่อเดิม; `--autoplay` -> reached To be continued; `--scene zzz` พิมพ์รายชื่อ. ยังไม่ได้เปิดเกมจริงกด F2 / `--scene fight` ดูด้วยตา
- Next action: ผู้ใช้ลองกด F2 และ `--scene` ในเกมจริง แล้วยืนยันปิด
