### 2026-10-14 — Q-20261014-custom-fonts — ใช้ฟอนต์ใน _candidates/fonts (Animal, Butter Squishy)
- Status: closed (ผู้ใช้ยืนยัน 2026-10-14)
- Owner: Claude Code (ตามที่ผู้ใช้วางฟอนต์ไว้ใน _candidates/fonts และยืนยันว่าเป็นงานรายวิชาไม่ใช้เชิงพาณิชย์)
- Goal: แทน Tahoma ด้วยฟอนต์ FNT-01 (`fnt_title_display.otf`) และ FNT-02 (`fnt_ui_cozy.otf`) ตาม 14-audio-fonts-list.md
- Files: BEPAL/Bepal_Game/Bepal/Content/Fonts/**, BEPAL/Bepal_Game/Bepal/Core/Ui.cs, BEPAL/Bepal_Game/Bepal/Scenes/{BaseScene,MenuScenes}.cs, CLAUDE.md, BEPAL/Docs/GDD/14-audio-fonts-list.md, Context/Status.md, Context/quests/Q-20261014-custom-fonts.md
- Context: license ของฟอนต์คือ Personal Use Only ขัดกับ README ของ _candidates (ต้อง OFL/ใช้เชิงพาณิชย์ได้) — ผู้ใช้ยอมรับเพราะเป็นงานรายวิชา; ผู้ใช้เปลี่ยนไฟล์ fnt_ui_cozy.otf เป็น Butter Squishy (Khurasan) เวลา 13:10 — license ไม่มีบันทึก (Befonts-License.txt ยังเป็นของ Animal/Baby Animal เดิม); ขาดอักขระ [ ] ^ { } ~ จึงเปลี่ยน "[Space]"→"(Space)" ใน BaseScene.cs และ "[ Space ] to Enter"→"Press Space to Enter" ใน MenuScenes.cs; ขนาด Big 34 / Main 17 / Small 14 / Title 68; Content Builder แคชฟอนต์เก่า ต้อง touch .spritefont เพื่อบังคับ build ใหม่; Button ที่ label 1 ตัวอักษร (+) ใช้ Big
- Validation (2026-10-14): build 0 warning; `--shots` 28 PNG ดู 01,03,04,07,10,13 (ฟอนต์ใหม่ขึ้น, อ่านง่ายกว่าชุดแรก, ไม่มี ? ในข้อความที่เห็น); `--autoplay` รันซ้ำหลังแก้ล่าสุด (ดู Edit Log); ยังไม่ได้เล่นจริง/ดูครบทุกฉาก
- Next action: — (ปิดแล้ว; merge ผ่าน PR แล้ว ยังไม่ได้เล่นจริงโดยผู้ใช้ตามที่ระบุใน Validation)
