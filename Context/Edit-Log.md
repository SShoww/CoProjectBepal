# Edit Log

ล่าสุด 10 งาน งานละ 3 บรรทัด เขียนผ่าน `tools/context/log-edit.py` เท่านั้น — รายการเก่าอยู่ที่ [archive/edit-log/](archive/edit-log/)

<!-- entries -->
- **2026-10-13 18:02 · Q-20261013-balance-migration** — ย้ายตัวเลข care QTE, ราคา/ผล shop, PlayerMaxExp และตัวคูณ upgrade เข้า Balance ค่าเดิมทุกตัว
  - Files: BEPAL/Bepal_Game/Bepal/Model/GameState.cs, BEPAL/Bepal_Game/Bepal/Scenes/CareScenes.cs, BEPAL/Bepal_Game/Bepal/Scenes/BaseMenus.cs, CLAUDE.md
  - Validation: build 0 warn; batch 50 run ก่อน/หลัง event stream เหมือนกัน; autoplay --refuse ผ่าน
- **2026-10-13 17:54 · Q-20261013-context-housekeeping** — ย้าย 5 quest ready-to-close ไป archive และเขียน Status.md ใหม่ให้ตรง Develop @ 29a9060 พร้อมจุดอ่อนและการตัดสินใจของผู้ใช้
  - Files: Context/Status.md, Context/quests/Q-20261013-context-housekeeping.md, Context/archive/quests/
  - Validation: ตรวจ path ย้ายแล้ว ไม่ได้รัน build (แก้เฉพาะเอกสาร)
- **2026-10-12 20:02 · Q-20261012-human-telemetry** — วิเคราะห์ telemetry คนจริง 8 runs (Aggregate+validate, balance-report §7, ซ่อม Boom_121, แถว QTE not missed ใน players.md); jsonl ดิบ gitignore เพราะมีชื่อผู้เล่น ไม่ push
  - Files: .gitignore, BEPAL/Docs/Balance/analysis/human, BEPAL/Docs/Balance/balance-report.md, BEPAL/Docs/Balance/tools/Aggregate/Program.cs
  - Validation: validate 8/8 ผ่านหลังซ่อม; Aggregate build/รันผ่าน; ไม่ได้แก้โค้ดเกมใน branch นี้ จึงไม่ได้รัน build/autoplay ของเกม
- **2026-10-12 19:54 · Q-20261012-telemetry-restart** — C9: Telemetry.RunStart รองรับเริ่มเกมใหม่ในเซสชันเดียว (ปิด run เก่าด้วย quit, reset state, ไฟล์ใหม่ต่อเกม) เพื่อแก้บั๊ก day/summary ผิดแบบ Boom_121
  - Files: BEPAL/Bepal_Game/Bepal/Core/Telemetry.cs, BEPAL/Docs/Balance/telemetry-spec.md, BEPAL/Docs/Balance/balance-report.md
  - Validation: build 0/0; --autoplay และ --refuse reached To be continued; harness ชั่วคราวผ่าน; ยังไม่ได้ทดสอบกด Main Menu ใน GUI จริง
- **2026-10-10 00:23 · Q-20261009-run-telemetry** — Run telemetry: per-run JSONL + bot profiles + balance report (round 1); round 2: --player, Gregorian run ids, build hash baked into exe, telemetry.txt auto-enable for tester exe, Aggregate players.md, Energy clamped to MaxEnergy (Tonic disabled at full Energy)
  - Files: BEPAL/Bepal_Game/Bepal/{Core/Telemetry.cs, Game1.cs, Bepal.csproj, Model/GameState.cs, Scenes/BaseMenus.cs, DevTools/AutoPlay.cs}, BEPAL/Docs/Balance/{telemetry-spec.md, tools/Aggregate/Program.cs}, BEPAL/Docs/GDD/{06-vertical-slice, 10-economy-items}.md, CLAUDE.md, Context/quests/Q-20261009-run-telemetry.md
  - Validation: build slnx 0/0; --autoplay and --autoplay --refuse reach To be continued; --shots 23 PNG; 200-run batch + validate.ps1 0 fail (round 1); 20 human-bot runs 0 energy over max; published exe + telemetry.txt writes data/*.jsonl with build hash
- **2026-10-07 22:37 · Q-20261007-drawio-diagrams** — สร้าง .drawio 6 ไฟล์แทน mermaid GDD (เก็บ mermaid เดิม + ลิงก์)
  - Files: BEPAL/Docs/GDD/diagrams/*.drawio, BEPAL/Docs/GDD/01-core-loop.md, 03-mechanics.md, 04-class-diagram.md, 11-narrative-world.md, 13-scene-breakdown.md
  - Validation: Reviewer render/เทียบ XML ทุก edge PASS 6/6; ยังไม่ได้เปิดใน VS Code GUI
- **2026-10-07 15:39 · Q-20261007-docs-restructure** — แยกหัวข้อ GDD ออกเป็นไฟล์ใหม่ 07-14 (pet-stats, care-qte, combat-encounters, economy-items, narrative-world, references-art-direction, scene-breakdown, audio-fonts-list) ย้ายข้อความคงเดิม + re-point ลิงก์
  - Files: BEPAL/Docs/GDD/07..14-*.md (ใหม่), 00, 01, 03, 05 GDD, GDD README, CLAUDE.md
  - Validation: link check 0 เสีย (Manager รันซ้ำ), บล็อกที่ย้ายไม่เหลือในไฟล์ต้นทาง, ไม่แก้ตัวเลข
- **2026-10-07 12:39 · Q-20261007-movement-system** — ระบบ movement: accel/decel + วิ่ง Shift, กล้อง look-ahead/deadzone, overlay/ปุ่ม/bar easing, SpriteMotion สำหรับ sprite รูปเดียว + player.png, แก้สัตว์เลี้ยงกระตุก
  - Files: BEPAL/Bepal_Game/Bepal/Core/**, Scenes/BaseScene.cs, World/Art.cs, World/SpriteMotion.cs, Model/GameState.cs, Content/Sprites/player.png
  - Validation: build 0 warn; autoplay + --refuse ผ่าน; shots 23 ภาพ; ยังไม่ได้เล่นมือเต็ม
- **2026-10-07 10:47 · Q-20261007-fight-dodge-after-hit** — Fight wheel สลับเฟส Attack -> Dodge หลัง Attack โดน; Attack กว้างเท่า Dodge และหดได้; ย้ายตัวเลข Fight ทั้งหมด (zone/damage/enemy/visual) เข้า Balance แบ่งหมวด
  - Files: BEPAL/Bepal_Game/Bepal/Scenes/FightScene.cs, BEPAL/Bepal_Game/Bepal/Model/GameState.cs, BEPAL/Bepal_Game/Bepal/Model/Pet.cs, Context/quests/Q-20261007-fight-dodge-after-hit.md
  - Validation: build ผ่าน; --autoplay ได้ reached To be continued; --shots 08/09 ปกติ; ผู้ใช้เล่นมือแล้วผ่าน; ไม่ได้รัน --autoplay --refuse รอบสุดท้าย
- **2026-10-07 03:48 · Q-20261007-pet-passive** — เพิ่ม passive Lv2: Mossling heal 1%, Toothless พิษ, Blinkbun วาร์ปเข็ม + แสดงในสมุด
  - Files: BEPAL/Bepal_Game/Bepal/Model/Pet.cs, Model/GameState.cs, Scenes/FightScene.cs, Scenes/BaseMenus.cs, DevTools/ShotRunner.cs, BEPAL/Docs/GDD/04-class-diagram.md
  - Validation: build ผ่าน, autoplay ปกติ/refuse ผ่าน, shots 12b/12c ตรวจแล้ว; ยังไม่ได้เล่น fight Lv2 จริง
