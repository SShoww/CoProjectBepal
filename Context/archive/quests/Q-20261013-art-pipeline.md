### 2026-10-13 — Q-20261013-art-pipeline — เตรียมท่อใส่ sprite สัตว์/ศัตรู/NPC
- Status: ready-to-close
- Owner: Claude Code (ตามคำสั่งผู้ใช้ 2026-10-13)
- Goal: ใส่ PNG ใน Content/Sprites + ลงทะเบียน mgcb แล้วเกมใช้ภาพจริงแทน primitive ทันที; ไม่มีไฟล์ = วาดเหมือนเดิม
- Files: BEPAL/Bepal_Game/Bepal/World/Art.cs, Core/Gfx.cs, Scenes/FightScene.cs, BEPAL/Assets/_candidates/README.md, BEPAL/Docs/GDD/05-asset-list.md
- Context: ชื่อไฟล์ `pet_<species>`, `enemy_toothless`, `enemy_bigz`, `npc_merchant`, `npc_doctor`; โหลดใน `Art.LoadOptional` (ContentLoadException -> ข้าม); หา opaque bounds อัตโนมัติ; หันขวา mirror ตาม lookAt; ใช้เฉพาะ idle; hurt flash = ย้อมแดง (`Art.Pet` มี param `flash`)
- Validation (2026-10-13): build 0 warning; ไม่มีไฟล์ sprite -> `--shots` 27 รูป + `--autoplay`/`--refuse` ผ่านเหมือนเดิม; ทดสอบด้วย PNG ชั่วคราว 8 ไฟล์ + mgcb ชั่วคราว (ลบ/revert แล้ว ไม่ได้ commit) -> ดู 08_fight/09_fight_bigz/11_doctor: feet ตรง, mirror ถูกทิศ, padding ไม่กระทบ. ไม่ได้ทดสอบ sprite จริงของทีม, ไม่ได้ดูท่า flash/dead/highlight กับ sprite ด้วยตา, ไม่ได้ดูใน BaseScene/Notebook
- Next action: ผู้ใช้ยืนยันปิด; เมื่อ Dear ส่งภาพจริง ปรับ height constants ใน Art.cs ตามสัดส่วน; ท่า attack/hurt/happy ค่อยต่อรอบหลัง
