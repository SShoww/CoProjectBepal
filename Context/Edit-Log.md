# Edit Log

ล่าสุด 10 งาน งานละ 3 บรรทัด เขียนผ่าน `tools/context/log-edit.py` เท่านั้น — รายการเก่าอยู่ที่ [archive/edit-log/](archive/edit-log/)

<!-- entries -->
- **2026-10-07 12:39 · Q-20261007-movement-system** — ระบบ movement: accel/decel + วิ่ง Shift, กล้อง look-ahead/deadzone, overlay/ปุ่ม/bar easing, SpriteMotion สำหรับ sprite รูปเดียว + player.png, แก้สัตว์เลี้ยงกระตุก
  - Files: BEPAL/Bepal_Game/Bepal/Core/**, Scenes/BaseScene.cs, World/Art.cs, World/SpriteMotion.cs, Model/GameState.cs, Content/Sprites/player.png
  - Validation: build 0 warn; autoplay + --refuse ผ่าน; shots 23 ภาพ; ยังไม่ได้เล่นมือเต็ม
- **2026-10-07 10:47 · Q-20261007-fight-dodge-after-hit** — Fight wheel สลับเฟส Attack -> Dodge หลัง Attack โดน; Attack กว้างเท่า Dodge และหดได้; ย้ายตัวเลข Fight ทั้งหมด (zone/damage/enemy/visual) เข้า Balance แบ่งหมวด
  - Files: BEPAL/Bepal_Game/Bepal/Scenes/FightScene.cs, BEPAL/Bepal_Game/Bepal/Model/GameState.cs, BEPAL/Bepal_Game/Bepal/Model/Pet.cs, Context/quests/Q-20261007-fight-dodge-after-hit.md
  - Validation: build ผ่าน; --autoplay ได้ reached To be continued; --shots 08/09 ปกติ; ผู้ใช้เล่นมือแล้วผ่าน; ไม่ได้รัน --autoplay --refuse รอบสุดท้าย
- **2026-10-07 03:48 · Q-20261007-pet-passive** — เพิ่ม passive Lv2: Mossling heal 1%, Toothless พิษ, Blinkbun วาร์ปเข็ม + แสดงในสมุด
  - Files: BEPAL/Bepal_Game/Bepal/Model/Pet.cs, Model/GameState.cs, Scenes/FightScene.cs, Scenes/BaseMenus.cs, DevTools/ShotRunner.cs, BEPAL/Docs/GDD/04-class-diagram.md
  - Validation: build ผ่าน, autoplay ปกติ/refuse ผ่าน, shots 12b/12c ตรวจแล้ว; ยังไม่ได้เล่น fight Lv2 จริง
- **2026-10-07 03:13 · Q-20261007-rain-vfx** — เพิ่ม VFX ฝน Day 4 (Rain.cs): ฝนเบาก่อนเปิดประตู หนัก+ฟ้าแลบหลังเปิดประตู เห็นผ่านหน้าต่างฐาน
  - Files: BEPAL/Bepal_Game/Bepal/World/Rain.cs, BEPAL/Bepal_Game/Bepal/Scenes/BaseScene.cs, BEPAL/Bepal_Game/Bepal/Scenes/DayEvents.cs, BEPAL/Bepal_Game/Bepal/Core/SceneManager.cs, BEPAL/Bepal_Game/Bepal/DevTools/ShotRunner.cs, BEPAL/Docs/GDD/04-class-diagram.md
  - Validation: build 0 warning; shots 17/18 ตรวจแล้ว; autoplay ปกติและ --refuse ผ่าน; ยังไม่ผ่านการเล่นจริง
- **2026-10-07 02:55 · Q-20261007-parallax** — เพิ่ม sprite parallax 5 ชั้น, หน้าต่างใหญ่ 6 บานในบ้าน (ห่างประตู), foreground jungle 2x; อัปเดต CLAUDE.md/monogame skill ว่ามี image assets
  - Files: BEPAL/Bepal_Game/Bepal/World/Backdrop.cs, BEPAL/Bepal_Game/Bepal/Scenes/BaseScene.cs, BEPAL/Bepal_Game/Bepal/Core/Gfx.cs, BEPAL/Bepal_Game/Bepal/Content, CLAUDE.md, .claude/skills/monogame/SKILL.md
  - Validation: build 0 warn, autoplay ปกติ/--refuse ผ่าน, ดู --shots แล้ว; ไม่ผ่าน PR ตามคำสั่งผู้ใช้; ยังไม่ได้เล่นจริง
- **2026-10-07 01:59 · Q-20261007-audio-sfx** — เพิ่ม Audio/Sfx: SFX 17 ไฟล์ WAV + synth (QtePerfect, Typewriter), เคาะประตูวนจนเปิด; ปิด Quest
  - Files: BEPAL/Bepal_Game/Bepal/Core/Audio.cs, BEPAL/Bepal_Game/Bepal/Content/Sfx, BEPAL/Bepal_Game/Bepal/Scenes, BEPAL/Assets/_candidates
  - Validation: build 0 warn, autoplay ปกติ/--refuse ผ่าน; ผู้ใช้ฟังเสียงจริงและยืนยัน 2026-10-07
- **2026-10-06 23:22 · Q-20261006-gitflow-doc** — เพิ่ม Gitflow.md (ปรับคำสั่งให้ตรง repo) และเริ่มใช้: สร้าง/push Develop จาก main, AGENTS.md ห้าม commit ตรงลง main/Develop
  - Files: Context/Gitflow.md, Context/README.md, Context/Status.md, AGENTS.md
  - Validation: ลิงก์ 0 เสีย; ls-remote เห็น Develop=main=0cb2715; ไม่ได้รัน build/gates
- **2026-10-06 22:41 · Q-20261006-ai-setup** — ติดตั้งระบบ AI Setup: ทางเข้า README/AGENTS, Context (Rules/Status/Handoff/quests/archive), Edit Log tool; ไม่แตะโค้ดเกม
  - Files: README.md, AGENTS.md, CLAUDE.md, .gitignore, Context/**, tools/context/log-edit.py
  - Validation: ลิงก์ 0 เสีย, ไม่มี secret, log tool ผ่าน input/ซ้ำ/retention/concurrency/lock; ไม่ได้ build เกม
