# Edit Log

ล่าสุด 10 งาน งานละ 3 บรรทัด เขียนผ่าน `tools/context/log-edit.py` เท่านั้น — รายการเก่าอยู่ที่ [archive/edit-log/](archive/edit-log/)

<!-- entries -->
- **2026-11-02 20:38 · Q-20261102-ui-skin** — ใส่ UI ชุดสีพีชใหม่: แถบสถานะผู้เล่นตาม mockup (+Energy เต็ม/ว่าง), dialog, choice, pause, options, ปุ่มเมนูหลัก, สมุด, การ์ดเลือกสัตว์, ป้าย Knock; ไม่มีไฟล์ = วาด nine-patch เดิม
  - Files: BEPAL/Bepal_Game/Bepal/Core/Ui.cs, BEPAL/Bepal_Game/Bepal/Scenes/{BaseScene, DialogScenes, MenuScenes, BaseMenus}.cs, Content/Sprites/ui_*.png (15), Content.mgcb, 05-asset-list.md, CLAUDE.md
  - Validation: build 0/0; shots ดูแล้ว 8 หน้า; autoplay + --refuse reached To be continued; ยังไม่ได้ดูเมนู Pause/เล่นจริง
- **2026-11-02 12:01 · Q-20261102-base-props-sprites** — ใส่ sprite เตียง/เตาไฟ/โต๊ะอัปเกรด/แท่นสมุด/ประตูแดง ในฐานแทน primitive (Art.Prop, fallback เดิม); ย่อประตูเหลือ 205 px ให้สมส่วนกับ player; merge PR #44 และปิด quest ตามคำสั่งผู้ใช้
  - Files: BEPAL/Bepal_Game/Bepal/Content/Sprites/prop_*.png, Content.mgcb, World/Art.cs, World/Backdrop.cs, Scenes/BaseScene.cs, 05-asset-list.md, _candidates/README.md, CLAUDE.md
  - Validation: build 0/0; --shots ฉากฐาน + 04_base_door ดูแล้ว; --autoplay reached To be continued; ยังไม่ได้เล่นจริง
- **2026-11-01 17:02 · Q-20261101-fullscreen-close** — ปิด quest fullscreen (ย้ายไป archive ตามคำสั่งผู้ใช้)
  - Files: Context/quests, Context/archive/quests
  - Validation: merge แล้ว PR #42; ผู้ใช้ทดสอบ F11 แล้ว
- **2026-11-01 17:00 · Q-20261101-fullscreen** — Full screen F11/Alt+Enter ด้วย BoxingViewportAdapter — เกมยังวาด 1280x720 ขยายคงสัดส่วน+แถบดำ, เมาส์แปลงพิกัดกลับ, จำใน settings.txt
  - Files: Game1.cs, Core/Gfx.cs, Core/Input.cs, Core/Settings.cs, CLAUDE.md, 04-class-diagram.md, Context/quests/Q-20261101-fullscreen.md
  - Validation: slnx build 0/0; shots 29 PNG 1280x720; autoplay reached To be continued; ผู้ใช้ทดสอบ F11 แล้วผ่าน
- **2026-11-01 16:45 · Q-20261101-base-toothless-sprites** — แก้หน้าต่างบานที่ 3 โผล่ครึ่งบานข้างประตูขวา: BaseArtPhase 400 -> 250 (หน้าต่างอยู่ x≈655/≈2265, บานที่ 3 พ้น WallRight)
  - Files: Scenes/BaseScene.cs, _candidates/README.md, Context/quests/Q-20261101-base-toothless-sprites.md
  - Validation: build -o scratchpad 0/0 (เกมเปิดค้าง pid 39588 ล็อก bin); shots 04_base_door/05_base_bed_night ok; autoplay reached To be continued
- **2026-10-25 20:05 · Q-20261025-font-size-docs** — Small font ตามจริง = 12 pt (Small.spritefont) แทน 15; CLAUDE.md นับ Sprites เป็น 14 PNG + ระบุ ui_item_* ของ Q-20261025-item-slot-ui
  - Files: CLAUDE.md, BEPAL/Docs/GDD/14-audio-fonts-list.md
  - Validation: เทียบกับ Content/Fonts/Small.spritefont และ ls Content/Sprites; แก้เฉพาะเอกสาร
- **2026-10-25 19:52 · Q-20261025-item-slot-ui** — ใส่ภาพกรอบการ์ดไอเทม + ปุ่ม Buy/Use (normal/hold) ใน Shop/Box; Button รองรับ sprite skin
  - Files: BEPAL/Bepal_Game/Bepal/Content/Sprites/ui_item_*.png, Content.mgcb, Core/Ui.cs, Model/Items.cs, Scenes/BaseMenus.cs, 05-asset-list.md, _candidates/README.md
  - Validation: build 0/0; shots shop/inventory ok; autoplay ทั้ง 2 แบบ reached To be continued; ยังไม่ได้ดูปุ่ม hold ด้วยตา
- **2026-10-24 19:50 · Q-20261024-pet-player-sprites-close** — ปิด quest pet-player-sprites (ย้ายไป archive ตามคำสั่งผู้ใช้)
  - Files: Context/quests, Context/archive/quests
  - Validation: merge แล้ว PR #37; ยังไม่ได้เล่นจริง
- **2026-10-24 19:49 · Q-20261024-pet-player-sprites** — ใส่ sprite สัตว์ 3 ตัว + player ใหม่ (แก้หันผิดทาง) + กรอบ ui_status_panel แทน panel status สัตว์
  - Files: Content/Sprites/*.png, Content/Content.mgcb, World/Art.cs, World/SpriteMotion.cs, Core/Ui.cs, Core/Gfx.cs, Scenes/CareScenes.cs, Scenes/BaseScene.cs, docs
  - Validation: slnx build 0 warn/0 err; --autoplay reached To be continued; --shots 29 PNG; ยังไม่ได้เล่นจริงด้วยมือ
- **2026-10-14 15:09 · Q-20261014-toothless-defeat** — แพ้ Toothless (สัตว์ตายหมด) -> dialogue แล้วกลับฐานไปชุบกับหมอ แทน GameOverScene
  - Files: BEPAL/Bepal_Game/Bepal/Scenes/DayEvents.cs, Context/quests/Q-20261014-toothless-defeat.md
  - Validation: build 0 warn/0 err; autoplay + --refuse reached To be continued; ยังไม่ได้ทดสอบเส้นทางแพ้จริง
