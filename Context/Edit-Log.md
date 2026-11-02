# Edit Log

ล่าสุด 10 งาน งานละ 3 บรรทัด เขียนผ่าน `tools/context/log-edit.py` เท่านั้น — รายการเก่าอยู่ที่ [archive/edit-log/](archive/edit-log/)

<!-- entries -->
- **2026-11-02 22:16 · Q-20261102-doctor-bigz-sprites** — ใส่ภาพหมอ (ตัดจาก Docter.png) + มุมหมอ (ป้าย/ม่าน/เตียง) + Big Z เข้าเกม
  - Files: Content/Sprites/npc_doctor|enemy_bigz|prop_doctor.png, Content.mgcb, World/Art.cs, Scenes/BaseScene.cs, CLAUDE.md, _candidates/README.md, 05-asset-list.md
  - Validation: build 0/0; shots 04/09/11 ok; autoplay reached To be continued; Big Z intro portrait ยังไม่ได้ดู
- **2026-11-02 21:27 · Q-20261102-upgrade-ui-skin** — หน้า Upgrade ใช้กรอบ peach (ui_options) + การ์ดแถว ui_item_slot + ปุ่ม ui_options_button แบบ nine-slice (Ui.SkinSliced/NineSlice, Button.SkinCorner) และใช้ ui_energy_full/empty เป็น pip ระดับอัปเกรด
  - Files: BEPAL/Bepal_Game/Bepal/Core/Ui.cs, BEPAL/Bepal_Game/Bepal/Scenes/BaseMenus.cs, CLAUDE.md, Context/quests/Q-20261102-upgrade-ui-skin.md
  - Validation: build 0/0; --shots 10_upgrade/10b ดูแล้ว; --autoplay reached To be continued; upgrade-first bot ซื้อ 6 ครั้ง; ยังไม่ได้เล่นจริง
- **2026-11-02 21:21 · Q-20261102-pet-outline-icon** — ไอคอน exe/หน้าต่างจาก YourStory_Bepal_icon.png; outline สี #60DFFF กระพริบรอบ sprite สัตว์เมื่อ player เข้าใกล้และเมื่อ hover การ์ดเลือกสัตว์
  - Files: BEPAL/Bepal_Game/Bepal/Icon.ico, BEPAL/Bepal_Game/Bepal/Icon.bmp, BEPAL/Bepal_Game/Bepal/World/Art.cs, BEPAL/Bepal_Game/Bepal/World/SpriteMotion.cs, CLAUDE.md
  - Validation: build slnx 0/0; --shots 02_choose (บังคับ highlight ชั่วคราว) เห็น outline ฟ้า; exe icon ตรวจด้วย ExtractAssociatedIcon; --autoplay reached To be continued (หลังเปลี่ยนสี)
- **2026-11-02 21:04 · Q-20261102-ui-skin** — ใส่ UI สีพีชชุดใหม่: แถบสถานะผู้เล่น 2/3 ของ mockup (+Energy เต็ม/ว่าง), dialog, choice, pause, options, ปุ่มเมนูหลัก, สมุด, การ์ดเลือกสัตว์, ป้าย Knock; merge PR #46 และปิด quest ตามคำสั่งผู้ใช้
  - Files: Core/Ui.cs, Scenes/{BaseScene, DialogScenes, MenuScenes, BaseMenus}.cs, Content/Sprites/ui_*.png (15), Content.mgcb, 05-asset-list.md, CLAUDE.md
  - Validation: build slnx 0/0; --autoplay + --refuse reached To be continued; shots ดูแล้ว; ยังไม่ได้ดูเมนู Pause/เล่นจริง
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
