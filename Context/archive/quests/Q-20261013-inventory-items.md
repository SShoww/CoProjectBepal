### 2026-10-13 — Q-20261013-inventory-items — Shop ทั้งสองทาง + Box/Inventory + ไอเทมใหม่ 4 ชิ้น
- Status: ready-to-close
- Owner: Claude Code (ตามคำสั่งผู้ใช้ 2026-10-13)
- Goal: ร้านเปิดทั้ง Sell/Refuse; ของเข้า Box ในฐาน (`Kind.Box`, X=600) ใช้ผ่าน InventoryScene; เพิ่ม Cloudy Glasses, Ballet Shoes, Torn Notebook, Faded Ribbon
- Files: BEPAL/Bepal_Game/Bepal/Model/Items.cs, Model/GameState.cs, Scenes/BaseMenus.cs, Scenes/BaseScene.cs, Scenes/CareScenes.cs, Scenes/FightScene.cs, Scenes/DayEvents.cs, DevTools/AutoPlay.cs, DevTools/ShotRunner.cs, BEPAL/Docs/Balance/telemetry-spec.md, BEPAL/Docs/GDD/{04-class-diagram,06-vertical-slice,10-economy-items}.md, CLAUDE.md
- Context: ค่าทั้งหมดใน `Balance` (ราคา/โบนัส); `Items.Apply/UseBlock` เป็นจุดเดียวของกฎการใช้; Sea Tea ยังเป็น "Fight ครั้งถัดไป"; เพิ่ม `Balance.ShopMaxOwned = 3` (ตัดสินใจเองกันกักตุน Tonic หลัง Sell 5000 coin — ผู้ใช้ยังไม่ได้ confirm)
- Validation (2026-10-13): build 0 warning; `--autoplay`/`--refuse` -> reached To be continued; shots 13_shop/13b_inventory/13c_base_box ตรวจแล้ว; run-batch 10 seeds x 5 profile: 47 to_be_continued + 3 game_over, ไม่มี timeout, `item_use` ครบทั้ง 7 ชนิด (109 ครั้ง), Aggregate รันได้ ไม่ crash. ไม่ได้รัน validate.ps1 (ฝัง path C10; ไม่รู้จัก event item_use); ไม่ได้ดู dialogue Sell->shop->cart rolls away ด้วยตา; ไม่ได้เล่นเองเพื่อดูผล Ballet/Notebook/Ribbon/Glasses ในเกมจริง; หัวข้อ "Merchant" ทับ HUD เล็กน้อยใน shop
- Next action: ผู้ใช้ยืนยันปิด + confirm เพดาน 3 ชิ้น; ถัดไป Q-20261013-art-pipeline; เก็บ telemetry คนจริงเพิ่ม
