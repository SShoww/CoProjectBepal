### 2026-11-01 — Q-20261101-base-toothless-sprites — ใส่ภาพผนังฐาน (bg_base) + Toothless เข้าเกม
- Status: ready-to-close
- Owner: Claude Code (ตามคำสั่งผู้ใช้ 2026-11-01)
- Goal: นำ `BEPAL/Assets/_candidates/sprites/env/bg_base.png` และ `pet/spr_toothless_idle.png` เข้าเกม; bg_base ย่อให้สูงเท่าผนัง placeholder เดิม (ย่อตามสัดส่วน + ต่อซ้ำ ตามที่ผู้ใช้เลือก)
- Files: BEPAL/Assets/_candidates/sprites/env/bg_base.png, BEPAL/Assets/_candidates/sprites/pet/spr_toothless_idle.png, BEPAL/Bepal_Game/Bepal/Content/Sprites/bg_base.png, BEPAL/Bepal_Game/Bepal/Content/Sprites/pet_toothless.png, BEPAL/Bepal_Game/Bepal/Content/Content.mgcb, BEPAL/Bepal_Game/Bepal/World/Backdrop.cs, BEPAL/Bepal_Game/Bepal/World/Art.cs, BEPAL/Bepal_Game/Bepal/Scenes/BaseScene.cs, BEPAL/Docs/GDD/05-asset-list.md, BEPAL/Assets/_candidates/README.md, CLAUDE.md, Context/quests/Q-20261101-base-toothless-sprites.md
- Context: `Backdrop.DrawBaseInterior` ย่อ x0.42 ให้ผนัง (แถว 0..596 ของภาพ) อยู่ y 350–600, ต่อซ้ำโดยกลับด้านทุกแผ่นที่สอง, เลื่อน `BaseScene.BaseArtPhase` = 250 ให้หน้าต่างไม่ทับ Upgrade และบานที่ 3 ไม่โผล่ครึ่งบานข้างประตู (2026-11-01 แก้จาก 400); `pet_toothless` ใช้กับศัตรู Toothless ด้วยเมื่อไม่มี `enemy_toothless`
- Validation (2026-11-01): build 0 warning/0 error; `--shots` 03_base/04_base_door/05_base_bed_night/08_fight + 17_rain_day4 ดูแล้วถูกต้อง; `--autoplay` -> RESULT: reached To be continued (re-run หลังแก้ล่าสุด บน branch นี้)
- Next action: ผู้ใช้ดูฉากฐานในเกมจริง; ยืนยันปิด quest
