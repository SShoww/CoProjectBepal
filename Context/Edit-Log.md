# Edit Log

ล่าสุด 10 งาน งานละ 3 บรรทัด เขียนผ่าน `tools/context/log-edit.py` เท่านั้น — รายการเก่าอยู่ที่ [archive/edit-log/](archive/edit-log/)

<!-- entries -->
- **2026-10-13 19:45 · Q-20261013-options-menu** — เมนู Options ปรับ volume เพลง/SFX (slider เมาส์/คีย์), เก็บใน %APPDATA%/BePal/settings.txt, เปิดจาก Main Menu และ Pause
  - Files: BEPAL/Bepal_Game/Bepal/Core/Settings.cs, Core/Ui.cs, Core/Audio.cs, Core/MusicPlayer.cs, Scenes/MenuScenes.cs, Scenes/BaseScene.cs, docs
  - Validation: build 0 warn; ทดสอบ input จริงผ่าน harness ชั่วคราว; autoplay ไม่แตะ settings; ยังไม่ได้ฟัง/เปิดจาก Pause ด้วยมือ
- **2026-10-13 19:21 · Q-20261013-bgm** — เชื่อม BGM 7 ไฟล์ผ่าน MusicPlayer: เลือกเพลงตาม Scene.Music, loop + fade, เงียบใน autoplay/shots
  - Files: BEPAL/Bepal_Game/Bepal/Core/MusicPlayer.cs, Core/SceneManager.cs, Game1.cs, Scenes/*, Content/Content.mgcb, Content/Music/, docs
  - Validation: build 0 warn; รันจริงเห็น state=Playing; ลำดับเพลงจาก autoplay ถูกต้อง; ยังไม่ได้ฟังด้วยหู
- **2026-10-13 19:07 · Q-20261013-context-housekeeping** — ผู้ใช้สั่งปิด Q-20261013 ทั้ง 7 ย้ายไป archive/quests และอัปเดต Status.md ให้ตรง Develop @ a05b7be
  - Files: Context/archive/quests/, Context/Status.md
  - Validation: ตรวจ path ต้นทาง/ปลายทาง (Context/quests -> Context/archive/quests) ไม่มีไฟล์ชน; แก้เอกสารล้วน ไม่ได้รัน build
- **2026-10-13 19:06 · Q-20261013-art-pipeline** — Art.Pet/Enemy/Merchant/Doctor ใช้ texture optional ต่อ asset (null = primitive เดิม) วาดผ่าน SpriteMotion; เขียนขั้นตอนใส่ภาพใน README/asset-list
  - Files: BEPAL/Bepal_Game/Bepal/World/Art.cs, Core/Gfx.cs, Scenes/FightScene.cs, BEPAL/Assets/_candidates/README.md, BEPAL/Docs/GDD/05-asset-list.md
  - Validation: build 0 warn; ไม่มีไฟล์ -> shots+autoplay ผ่าน; ทดสอบด้วย PNG ชั่วคราวที่ไม่ commit
- **2026-10-13 18:48 · Q-20261013-inventory-items** — ร้านเปิดหลัง Sell/Refuse, ของเข้า Box ใช้ผ่าน InventoryScene, เพิ่ม Cloudy Glasses/Ballet Shoes/Torn Notebook/Faded Ribbon, bot ซื้อ/ใช้ไอเทม
  - Files: BEPAL/Bepal_Game/Bepal/Model/Items.cs, Model/GameState.cs, Scenes/BaseMenus.cs, Scenes/BaseScene.cs, Scenes/FightScene.cs, Scenes/DayEvents.cs, DevTools/AutoPlay.cs, GDD docs, CLAUDE.md
  - Validation: build 0 warn; autoplay+refuse ผ่าน; batch 50 run ไม่ timeout item_use ครบ 7 ชนิด; validate.ps1 ไม่ได้รัน
- **2026-10-13 18:29 · Q-20261013-upgrade-points** — ราคา Point ต่อเลเวล (Energy 1/2/4, Progress 1/2/3 max 3, QTE 1/1/1) พร้อมข้อความ Upgrade/HUD/hint แรกให้ชัดว่าต้องใช้และได้ Point จากไหน
  - Files: BEPAL/Bepal_Game/Bepal/Model/GameState.cs, BEPAL/Bepal_Game/Bepal/Scenes/BaseMenus.cs, BEPAL/Bepal_Game/Bepal/Scenes/BaseScene.cs, BEPAL/Bepal_Game/Bepal/Scenes/CareScenes.cs, BEPAL/Bepal_Game/Bepal/DevTools/AutoPlay.cs, BEPAL/Docs/Balance/tools/validate.ps1, BEPAL/Docs/GDD/06-vertical-slice.md
  - Validation: build 0 warn; autoplay+refuse ผ่าน; batch 50 run ไม่ soft-lock, upgrade_buy cost ตรงตาราง; validate.ps1 ยังไม่ได้รัน
- **2026-10-13 18:11 · Q-20261013-demo-ending** — ToBeContinuedScene เปลี่ยนข้อความเป็นจุดจบ demo (End of the BePal demo / Big Z can't be beaten yet / Thanks for playing)
  - Files: BEPAL/Bepal_Game/Bepal/Scenes/MenuScenes.cs
  - Validation: build 0 warn; autoplay+refuse ผ่าน; shot 16_tbc ตรวจแล้ว
- **2026-10-13 18:10 · Q-20261013-energy-ux** — Energy 0 ไม่เปิด care (แจ้งให้ไปนอน); Heal จางและเป็น gap เมื่อ HP เต็ม; bot ไม่เลือก Heal ตอน HP เต็ม
  - Files: BEPAL/Bepal_Game/Bepal/Scenes/BaseScene.cs, BEPAL/Bepal_Game/Bepal/Scenes/CareScenes.cs, BEPAL/Bepal_Game/Bepal/Scenes/Wheel.cs, BEPAL/Bepal_Game/Bepal/DevTools/AutoPlay.cs
  - Validation: build 0 warn; autoplay+refuse ผ่าน; batch 50 run ผลเท่า baseline; shots ตรวจแล้ว
- **2026-10-13 18:02 · Q-20261013-balance-migration** — ย้ายตัวเลข care QTE, ราคา/ผล shop, PlayerMaxExp และตัวคูณ upgrade เข้า Balance ค่าเดิมทุกตัว
  - Files: BEPAL/Bepal_Game/Bepal/Model/GameState.cs, BEPAL/Bepal_Game/Bepal/Scenes/CareScenes.cs, BEPAL/Bepal_Game/Bepal/Scenes/BaseMenus.cs, CLAUDE.md
  - Validation: build 0 warn; batch 50 run ก่อน/หลัง event stream เหมือนกัน; autoplay --refuse ผ่าน
- **2026-10-12 20:02 · Q-20261012-human-telemetry** — วิเคราะห์ telemetry คนจริง 8 runs (Aggregate+validate, balance-report §7, ซ่อม Boom_121, แถว QTE not missed ใน players.md); jsonl ดิบ gitignore เพราะมีชื่อผู้เล่น ไม่ push
  - Files: .gitignore, BEPAL/Docs/Balance/analysis/human, BEPAL/Docs/Balance/balance-report.md, BEPAL/Docs/Balance/tools/Aggregate/Program.cs
  - Validation: validate 8/8 ผ่านหลังซ่อม; Aggregate build/รันผ่าน; ไม่ได้แก้โค้ดเกมใน branch นี้ จึงไม่ได้รัน build/autoplay ของเกม
