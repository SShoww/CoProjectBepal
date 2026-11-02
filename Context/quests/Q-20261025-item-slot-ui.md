### 2026-10-25 — Q-20261025-item-slot-ui — ใส่ภาพกรอบการ์ดไอเทม + ปุ่ม (Shop/Inventory)
- Status: ready-to-close
- Owner: Claude Code (ตามคำสั่งผู้ใช้ 2026-10-25)
- Goal: นำ `BEPAL/Assets/_candidates/sprites/ui/Item/` (slot_frame, slot_frame_Button, slot_frame_Button_hold) เข้าเกม เป็นกรอบการ์ดและปุ่ม Buy/Use ของ `ItemCards`/`ItemTrack`
- Files: BEPAL/Bepal_Game/Bepal/Content/Sprites/ui_item_*.png, BEPAL/Bepal_Game/Bepal/Content/Content.mgcb, BEPAL/Bepal_Game/Bepal/Core/Ui.cs, BEPAL/Bepal_Game/Bepal/Scenes/BaseMenus.cs, BEPAL/Bepal_Game/Bepal/Model/Items.cs, BEPAL/Docs/GDD/05-asset-list.md, BEPAL/Assets/_candidates/README.md, Context/quests/Q-20261025-item-slot-ui.md
- Context: ภาพ 270x230 = `Items.CardW/CardH`; ปุ่มในภาพอยู่ที่ (25,183) 220x40; ไม่แตะ CLAUDE.md / 14-audio-fonts-list.md (มีงานค้างของคนอื่น)
- Validation (2026-10-25): `dotnet build -o <scratchpad>` 0 warning/0 error (build ปกติล็อกเพราะ `Bepal.exe` pid 45928 เปิดค้าง); `--shots` 13_shop/13b_inventory ดูแล้ว กรอบ+ปุ่มขึ้นถูกตำแหน่ง ตัวอักษรอ่านได้; `--autoplay` และ `--autoplay --refuse` -> RESULT: reached To be continued. ยังไม่ได้ดูภาพปุ่ม hold ด้วยตา (ต้องกดเมาส์ค้างในเกมจริง) · Re-run ก่อน ship (2026-10-25): build -o scratchpad 0/0, autoplay + --refuse reached To be continued, shots 13_shop/13b_inventory ok
- Next action: ผู้ใช้ลองกด Buy/Use ค้างในเกม; ยืนยันปิด quest; CLAUDE.md อัปเดตแล้ว (14 PNG + ui_item_*)
