# Edit Log

ล่าสุด 10 งาน งานละ 3 บรรทัด เขียนผ่าน `tools/context/log-edit.py` เท่านั้น — รายการเก่าอยู่ที่ [archive/edit-log/](archive/edit-log/)

<!-- entries -->
- **2026-10-14 13:24 · Q-20261014-custom-fonts** — ใช้ฟอนต์ Animal (title) + Butter Squishy (UI) จาก _candidates แทน Tahoma; เปลี่ยนป้าย [Space] เป็น (Space) เพราะฟอนต์ไม่มี [ ]
  - Files: Content/Fonts/*, Core/Ui.cs, Scenes/{BaseScene, MenuScenes}.cs, CLAUDE.md, 14-audio-fonts-list.md, Status.md
  - Validation: build 0 warning; autoplay + --refuse reached To be continued; shots ตรวจ 6 ภาพ; ยังไม่ได้เล่นจริง; license Butter Squishy ไม่มีบันทึก
- **2026-10-14 12:36 · Q-20261014-ui-polish-extended** — ลดขนาดฟอนต์ (Tahoma, เพิ่ม Title) จัด layout UI แก้เส้นขีดวงล้อ QTE (Gfx.Arc เป็น mesh ไม่ซ้อน) และใช้ MonoGame.Extended NinePatch ทำกรอบมน
  - Files: BEPAL/Bepal_Game/Bepal/Core/{Gfx, Ui}.cs, Scenes/*, Content/Fonts/*, Bepal.csproj, CLAUDE.md, 14-audio-fonts-list.md, Status.md
  - Validation: build 0 warning; autoplay + --refuse reached To be continued; shots ตรวจด้วยตา; ยังไม่ได้เล่นจริง
- **2026-10-14 00:36 · Q-20261013-bgm** — Day 4 เล่น bgm_thunderstorm ทั้งวัน (เดิมเฉพาะหลังเปิดประตู)
  - Files: BEPAL/Bepal_Game/Bepal/Scenes/BaseScene.cs, BEPAL/Docs/GDD/14-audio-fonts-list.md
  - Validation: build 0 warning/0 error; --autoplay -> RESULT: reached To be continued; ยังไม่ได้ฟังด้วยหู
- **2026-10-14 00:11 · Q-20261014-drawio-sync** — แก้ 6 diagram .drawio + PNG ให้ตรงโค้ด (ไม่มีปุ่ม E, Refuse = ร้านค้า, Day 4-5 Storm/Big Z, Options/Box/Shop); WinLose_CoreFlow ตรงอยู่แล้ว
  - Files: BEPAL/Docs/GDD/diagrams/**
  - Validation: render PNG ดูทุกไฟล์, XML parse ผ่าน, แก้เฉพาะเอกสาร ไม่รัน build/autoplay, ยังไม่เปิดใน VS Code จริง
- **2026-10-13 22:46 · Q-20261013-gdd-sync** — ปรับ GDD 01-14, README, telemetry-spec, CLAUDE.md ให้ตรงโค้ด (Day1-5 ตายตัว, ร้าน 7 ไอเทม+Box, Options/BGM, dev tools)
  - Files: BEPAL/Docs/GDD/*.md, BEPAL/Docs/Balance/telemetry-spec.md, CLAUDE.md
  - Validation: dotnet build ผ่าน, แก้เฉพาะเอกสาร, ไม่รัน autoplay, ไม่ได้ตรวจ drawio/00-concept
- **2026-10-13 22:15 · Q-20261013-scene-jump** — เพิ่ม SceneCatalog + --scene <name> + เมนู F2 กระโดดไป scene ใดก็ได้ (dev); ShotRunner ใช้แคตตาล็อกร่วม
  - Files: DevTools/SceneCatalog.cs, DevTools/SceneJumpScene.cs, ShotRunner.cs, Game1.cs, CLAUDE.md, 04-class-diagram.md
  - Validation: build 0/0, --shots 28 PNG, --autoplay reached To be continued; F2 ยังไม่ได้ลองในเกมจริง
- **2026-10-13 20:08 · Q-20261013-validate-ps1** — validate.ps1: พารามิเตอร์ -Raw/-Csv/-Out, รู้จัก starter, ราคา Point ใหม่, I09 ไม่ห้ามร้านหลัง Sell, เพิ่ม I16 ไอเทม, วันที่ invariant
  - Files: BEPAL/Docs/Balance/tools/validate.ps1, BEPAL/Docs/Balance/telemetry-spec.md, Context/Status.md
  - Validation: batch 50 run -> 16/16 PASS; ทดสอบด้านลบจับ I01/I16; ยังไม่ได้ลองกับ data คนจริง
- **2026-10-13 19:45 · Q-20261013-options-menu** — เมนู Options ปรับ volume เพลง/SFX (slider เมาส์/คีย์), เก็บใน %APPDATA%/BePal/settings.txt, เปิดจาก Main Menu และ Pause
  - Files: BEPAL/Bepal_Game/Bepal/Core/Settings.cs, Core/Ui.cs, Core/Audio.cs, Core/MusicPlayer.cs, Scenes/MenuScenes.cs, Scenes/BaseScene.cs, docs
  - Validation: build 0 warn; ทดสอบ input จริงผ่าน harness ชั่วคราว; autoplay ไม่แตะ settings; ยังไม่ได้ฟัง/เปิดจาก Pause ด้วยมือ
- **2026-10-13 19:07 · Q-20261013-context-housekeeping** — ผู้ใช้สั่งปิด Q-20261013 ทั้ง 7 ย้ายไป archive/quests และอัปเดต Status.md ให้ตรง Develop @ a05b7be
  - Files: Context/archive/quests/, Context/Status.md
  - Validation: ตรวจ path ต้นทาง/ปลายทาง (Context/quests -> Context/archive/quests) ไม่มีไฟล์ชน; แก้เอกสารล้วน ไม่ได้รัน build
- **2026-10-13 19:06 · Q-20261013-art-pipeline** — Art.Pet/Enemy/Merchant/Doctor ใช้ texture optional ต่อ asset (null = primitive เดิม) วาดผ่าน SpriteMotion; เขียนขั้นตอนใส่ภาพใน README/asset-list
  - Files: BEPAL/Bepal_Game/Bepal/World/Art.cs, Core/Gfx.cs, Scenes/FightScene.cs, BEPAL/Assets/_candidates/README.md, BEPAL/Docs/GDD/05-asset-list.md
  - Validation: build 0 warn; ไม่มีไฟล์ -> shots+autoplay ผ่าน; ทดสอบด้วย PNG ชั่วคราวที่ไม่ commit
