### 2026-11-02 — Q-20261102-base-props-sprites — ใส่ sprite ของในฐาน (เตียง เตาไฟ แท่นสมุด ประตูแดง โต๊ะอัปเกรด)
- Status: closed (ผู้ใช้สั่งปิด 2026-11-02, merge แล้ว PR #44)
- Owner: Claude Code (ตามคำสั่งผู้ใช้ 2026-11-02)
- Goal: นำ `BEPAL/Assets/_candidates/sprites/env/{bed,core_heart,notebook_desk,red_door,upgrade_station}.png` เข้าเกมแทน primitive ใน `BaseScene.DrawObjects`/เตาไฟ; ไม่มีไฟล์ = วาด primitive เหมือนเดิม
- Files: BEPAL/Assets/_candidates/sprites/env/{bed,core_heart,notebook_desk,red_door,upgrade_station}.png, BEPAL/Bepal_Game/Bepal/Content/Sprites/prop_*.png, BEPAL/Bepal_Game/Bepal/Content/Content.mgcb, BEPAL/Bepal_Game/Bepal/World/Art.cs, BEPAL/Bepal_Game/Bepal/World/Backdrop.cs, BEPAL/Bepal_Game/Bepal/Scenes/BaseScene.cs, BEPAL/Docs/GDD/05-asset-list.md, BEPAL/Assets/_candidates/README.md, CLAUDE.md, Context/quests/Q-20261102-base-props-sprites.md
- Context: ภาพต้นฉบับ 2048x2048 → crop กล่องทึบ + ย่อด้านยาวสุด 512 px (Lanczos) → `Content/Sprites/prop_{bed,hearth,notebook,door,upgrade}.png`
- Validation (2026-11-02): build 0 warning/0 error; `--shots` 03_base/04_base_door/05_base_bed_night/13c_base_box ดูแล้ว ของวางบนพื้นถูกที่ ไม่ทับกัน กลางคืนมืดตามผนัง; ประตูลดจาก 270 → 205 px (บานประตู ≈1.3 เท่าของ player 140 px ตามคำขอผู้ใช้) และ glow/"Knock Knock" ขยับตามความสูงประตู ตรวจ 04_base_door แล้ว; `--autoplay` -> RESULT: reached To be continued
- Next action: -
