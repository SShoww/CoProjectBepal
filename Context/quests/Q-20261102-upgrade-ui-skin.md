### 2026-11-02 — Q-20261102-upgrade-ui-skin — ใส่กรอบ UI ชุด peach ให้หน้า Upgrade
- Status: ready-to-close
- Owner: Claude Code (ตามคำสั่งผู้ใช้ 2026-11-02)
- Goal: หน้า Upgrade ใช้ภาพกรอบเดียวกับหน้าอื่น (panel `ui_options`, การ์ดแถว `ui_item_slot`, ปุ่ม `ui_options_button`) และใช้ `ui_energy_full/empty` แทนช่องสี่เหลี่ยมแสดงจำนวนอัปเกรด
- Files: BEPAL/Bepal_Game/Bepal/Core/Ui.cs, BEPAL/Bepal_Game/Bepal/Scenes/BaseMenus.cs, CLAUDE.md, Context/quests/Q-20261102-upgrade-ui-skin.md
- Context: ภาพ panel/ปุ่มมีขนาดไม่ตรงกล่องของ Upgrade (720x520, ปุ่ม +) จึงวาดแบบ nine-slice (`Ui.SkinSliced`, `Button.SkinCorner`) แทนการยืดทั้งภาพ; ไม่มีภาพ = nine-patch เดิม
- Validation (2026-11-02): build 0 warning/0 error; `--shots` 10_upgrade + 10b_upgrade_poor ดูแล้ว (panel peach, การ์ดแถวน้ำตาล, pip = energy icon, ปุ่ม + skin, ปุ่ม disabled มืดลง); `--autoplay` -> RESULT: reached To be continued; `--bot upgrade-first --seed 1` ซื้ออัปเกรด 6 ครั้งผ่าน `PlusRect` ใหม่, outcome to_be_continued; แก้ตาม feedback: ปุ่มที่มีตัวอักษรตัวเดียว ("+") จัดกลางตาม ink ของ glyph (`Button.Draw`) แทน line box ที่ทำให้ + ต่ำ — `--shots` 10b ดูแล้ว + อยู่กลางปุ่ม
- Next action: ผู้ใช้ดูหน้า Upgrade ในเกมจริง (pip ว่างสีแทนบนการ์ดน้ำตาล contrast ต่ำ — ถ้าไม่ชอบเปลี่ยนสีการ์ดได้); ยืนยันปิด quest
