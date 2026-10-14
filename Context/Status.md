# Status — บริบทปัจจุบัน

ไม่ใช่ backlog (งานของทีมอยู่ที่ [BEPAL/Docs/Agile/](../BEPAL/Docs/Agile/), งาน AI อยู่ที่ [quests/](quests/))
ข้อมูลในไฟล์นี้เป็นสิ่งที่ตรวจ ณ วันที่ระบุ — ยืนยันใหม่ก่อนอ้างเป็นสถานะ runtime ปัจจุบัน

**ตรวจล่าสุด:** 2026-10-13 โดย Claude Code (ปิด 7 quest Q-20261013-*) · แหล่ง: `git log` บน `Develop` @ `a05b7be` (PR #13–#19), รายงานผลตรวจในแต่ละ quest ที่ [archive/quests/](archive/quests/)

## ปัจจุบัน
- ใช้ Gitflow ([Gitflow.md](Gitflow.md)): `Develop` รวม PR ถึง #19 แล้ว; Sprint 2 (2026-10-05 → 2026-10-18); **final deadline 2026-11-04**
- as-built:
  - Art: parallax 5 ชั้น + `fg_jungle` + `player.png`; สัตว์/ศัตรู/หมอ/พ่อค้ารองรับ sprite จริงแล้ว (ท่อพร้อม, `Art.LoadOptional`) แต่ยังไม่มีไฟล์ภาพ จึงวาด primitive (M16/M17 ค้างที่ตัวภาพ)
  - Audio: SFX 17 ไฟล์เสร็จ; BGM 7 ไฟล์เข้าเกมแล้ว (`Content/Music/`, `MusicPlayer`, เลือกเพลงตาม scene) และมีเมนู Options ปรับ volume เพลง/SFX (Main Menu + Pause); ฟอนต์เป็น `Tahoma` (ฟอนต์ระบบ Windows, สิทธิ์แจกจ่ายยังไม่ชัด — S8; เปลี่ยนจาก Segoe UI เพราะตัวอักษรเอียงทั้งเกม)
  - ทำแล้ว 2026-10-13: ตัวเลข care/shop/EXP ย้ายเข้า `Balance`; Energy 0 เปิด care ไม่ได้ + Heal จางเมื่อ HP เต็ม; หน้าจบบอกว่าเป็น demo; Point cost ต่อเลเวล (Energy 1/2/4, Progress 1/2/3 max 3) + UI อธิบาย Point; ร้านเปิดหลัง Sell/Refuse, ของเข้า Box (`InventoryScene`), ไอเทมใหม่ 4 ชิ้น (เพดาน 3 ชิ้นต่อชนิด `Balance.ShopMaxOwned` — ผู้ใช้ยังไม่ได้ confirm)

## ค้าง / รอบถัดไป
- ฟอนต์ OFL; release build (`dotnet publish -c Release -r win-x64 --self-contained`)
- `validate.ps1` แก้แล้ว (C10): รับ `-Raw/-Csv/-Out`, รู้จัก `starter`, ราคา Point ใหม่ และมี invariant I16 สำหรับไอเทม — รันผ่าน 16/16 กับ batch 50 run หลังข้อ 6 (ยังไม่ได้ลองกับ data คนจริง เพราะไม่มีไฟล์ดิบในเครื่อง); ไม่มี CI; ShotRunner ไม่มี shot GameOver/Night/PetPick; มี `--scene <name>` และ F2 กระโดดไป scene (SceneCatalog)
- เก็บ telemetry คนจริงเพิ่ม (ปัจจุบัน 8 runs ก่อนแก้ข้อ 3/5/6; เป้า 15–20) เพื่อดูว่าซื้อ Energy/ใช้ของมากขึ้นไหม; `sprint-plan-02.md` ยังไม่มี
- ท่า attack/hurt/happy ของ sprite ยังไม่ต่อโค้ด; ยังไม่ได้ดู popup "No energy", dialogue Sell→ร้าน, hint Point แรก ด้วยตา

## ข้อจำกัด / ข้อควรระวัง
- ภาพ/เสียง/movement ล่าสุดยังไม่ผ่านการเล่นจริงของผู้ใช้ (ตรวจด้วย build + `--shots` + `--autoplay` เท่านั้น)
- Figma MCP มี rate limit — อ่าน Figma ผ่าน layers panel ในเบราว์เซอร์แทนเมื่อติด limit
