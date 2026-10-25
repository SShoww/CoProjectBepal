# Edit Log

ล่าสุด 10 งาน งานละ 3 บรรทัด เขียนผ่าน `tools/context/log-edit.py` เท่านั้น — รายการเก่าอยู่ที่ [archive/edit-log/](archive/edit-log/)

<!-- entries -->
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
- **2026-10-14 13:54 · Q-20261014-ui-font-patrick-hand** — ปิด quest ui-font-patrick-hand (ย้ายไป archive ตามคำสั่งผู้ใช้)
  - Files: Context/quests, Context/archive/quests
  - Validation: merge แล้ว PR #32; ยังไม่ได้เล่นจริง
- **2026-10-14 13:54 · Q-20261014-custom-fonts** — ปิด quest custom-fonts (ย้ายไป archive ตามคำสั่งผู้ใช้)
  - Files: Context/quests, Context/archive/quests
  - Validation: merge แล้ว PR #31; ยังไม่ได้เล่นจริง
- **2026-10-14 13:54 · Q-20261014-ui-polish-extended** — ปิด quest ui-polish-extended (ย้ายไป archive ตามคำสั่งผู้ใช้)
  - Files: Context/quests, Context/archive/quests
  - Validation: merge แล้ว PR #30; ยังไม่ได้เล่นจริง
- **2026-10-14 00:36 · Q-20261013-bgm** — Day 4 เล่น bgm_thunderstorm ทั้งวัน (เดิมเฉพาะหลังเปิดประตู)
  - Files: BEPAL/Bepal_Game/Bepal/Scenes/BaseScene.cs, BEPAL/Docs/GDD/14-audio-fonts-list.md
  - Validation: build 0 warning/0 error; --autoplay -> RESULT: reached To be continued; ยังไม่ได้ฟังด้วยหู
- **2026-10-14 00:11 · Q-20261014-drawio-sync** — แก้ 6 diagram .drawio + PNG ให้ตรงโค้ด (ไม่มีปุ่ม E, Refuse = ร้านค้า, Day 4-5 Storm/Big Z, Options/Box/Shop); WinLose_CoreFlow ตรงอยู่แล้ว
  - Files: BEPAL/Docs/GDD/diagrams/**
  - Validation: render PNG ดูทุกไฟล์, XML parse ผ่าน, แก้เฉพาะเอกสาร ไม่รัน build/autoplay, ยังไม่เปิดใน VS Code จริง
