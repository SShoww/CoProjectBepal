### 2026-10-24 — Q-20261024-pet-player-sprites — ใส่ sprite สัตว์ 3 ตัว + ผู้เล่นใหม่ + กรอบ status สัตว์เข้าเกม
- Status: closed (ผู้ใช้สั่งปิด 2026-10-24, merge แล้ว PR #37)
- Owner: Claude Code (ตามคำสั่งผู้ใช้ 2026-10-24)
- Goal: ใช้ภาพจาก `BEPAL/Assets/_candidates/sprites/pet/` และ `player/` ในเกมแทน primitive / player pixel-art เดิม
- Files: BEPAL/Bepal_Game/Bepal/Content/Sprites/{pet_mossling,pet_nibbleclaw,pet_blinkbun,player,ui_status_panel}.png, Core/Ui.cs, Core/Gfx.cs, Scenes/CareScenes.cs, Content/Content.mgcb, World/Art.cs, World/SpriteMotion.cs, Scenes/BaseScene.cs, CLAUDE.md, BEPAL/Docs/GDD/{04-class-diagram,05-asset-list}.md, BEPAL/Assets/_candidates/README.md
- Context: ภาพต้นฉบับเป็นภาพวาด (ไม่ใช่ pixel art) canvas ใหญ่ → crop, กลับด้านให้หันขวา (Mossling/Blinkbun; player ไม่กลับ — แก้ 2026-10-24 หลังผู้ใช้แจ้งว่าเดินขวาแต่หันซ้าย), ย่อ 256 px, mipmaps + `LinearClamp`; player สูง 140 px (เดิม 99), `PlayerSpriteSrc` หา opaque box ตอนโหลด; Toothless ยังไม่มีภาพ
- Validation (2026-10-24): build 0 error; --shots 29 รูป (ดู choose/base/fight แล้ว); --autoplay reached To be continued. ยังไม่ได้เล่นจริงด้วยมือ, ความสูง player 140 / สัดส่วนสัตว์ทุกตัวเท่ากัน 90 เป็นค่าเดา
- Status panel (2026-10-24): `ui_status_panel.png` แทน `Ui.Panel` ใน `PetStats` (ฐาน + CareSelect + QTE), ขนาด 264x221, ตัวหนังสือสีน้ำตาลเข้ม; build 0 error, --shots ดู base/care แล้ว, --autoplay reached To be continued
- Gates (2026-10-24 19:49, clean worktree @ 09bab0b): slnx build 0 warn/0 err; --autoplay reached To be continued; --shots 29 PNG
- Next action: ผู้ใช้ดูในเกมแล้วปรับขนาด (Art.PlayerSpriteHeight / PetSpriteHeight); ยืนยันปิด
