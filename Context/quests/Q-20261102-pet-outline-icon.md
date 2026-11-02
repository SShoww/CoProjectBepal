### 2026-11-02 — Q-20261102-pet-outline-icon — ไอคอน exe + outline สัตว์ตอนเข้าใกล้/ตอนเลือก
- Status: ready-to-close
- Owner: Claude Code (ตามคำสั่งผู้ใช้ 2026-11-02)
- Goal: ใช้ `BEPAL/Assets/_candidates/YourStory_Bepal_icon.png` เป็นไอคอน exe/หน้าต่าง; วาด outline รอบ sprite สัตว์เมื่อ player เข้าใกล้ (BaseScene) และเมื่อ hover การ์ดเลือกสัตว์ (PetPickScene, Doctor)
- Files: BEPAL/Assets/_candidates/YourStory_Bepal_icon.png, BEPAL/Bepal_Game/Bepal/Icon.ico, BEPAL/Bepal_Game/Bepal/Icon.bmp, BEPAL/Bepal_Game/Bepal/World/Art.cs, BEPAL/Bepal_Game/Bepal/World/SpriteMotion.cs, CLAUDE.md, Context/quests/Q-20261102-pet-outline-icon.md
- Context: outline = silhouette texture (alpha-only, premultiplied, box-filter mipmaps) ของ `pet_*` วาด 12 ครั้งรอบ sprite สี #60DFFF (ผู้ใช้เลือก 2026-11-02) หนา 4 px กระพริบเบา ๆ; ใช้ flag `highlight` เดิมของ `Art.Pet` (BaseScene `_near`, PetPickScene/Doctor hover) จึงไม่ต้องแก้ scene; primitive ยังใช้ glow เดิม; สีครีมแรกกลืนกับการ์ดพีช จึงเปลี่ยนเป็นเหลือง แล้วผู้ใช้ให้ใช้ #60DFFF
- Validation (2026-11-02): build 0 warning/0 error; `--shots` (บังคับ highlight ชั่วคราวแล้ว revert) 02_choose + 03_base เห็น outline ชัด; ดึงไอคอนจาก `bin/Debug/net8.0/Bepal.exe` ได้ภาพใหม่; `--autoplay` -> RESULT: reached To be continued (รันใหม่หลังเปลี่ยนสี #60DFFF; build slnx 0/0)
- Next action: ผู้ใช้ลองเดินเข้าใกล้สัตว์/hover การ์ดในเกมจริง (ไอคอน taskbar/หน้าต่างดูตอนรัน exe); ยืนยันปิด quest
