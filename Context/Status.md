# Status — บริบทปัจจุบัน

ไม่ใช่ backlog (งานของทีมอยู่ที่ [BEPAL/Docs/Agile/](../BEPAL/Docs/Agile/), งาน AI อยู่ที่ [quests/](quests/))
ข้อมูลในไฟล์นี้เป็นสิ่งที่ตรวจ ณ วันที่ระบุ — ยืนยันใหม่ก่อนอ้างเป็นสถานะ runtime ปัจจุบัน

**ตรวจล่าสุด:** 2026-10-13 โดย Claude Code (Q-20261013-context-housekeeping) · แหล่ง: `git log`, อ่านโค้ดบน `Develop` @ `29a9060`, telemetry [balance-report.md](../BEPAL/Docs/Balance/balance-report.md) §7 (ยังไม่ได้รัน build/autoplay ใน session นี้)

## ปัจจุบัน
- ใช้ Gitflow ([Gitflow.md](Gitflow.md)): `Develop` รวม PR ถึง #12 แล้ว
- Sprint 2 (2026-10-05 → 2026-10-18); **final deadline 2026-11-04**
- as-built:
  - Art: parallax 5 ชั้น + `fg_jungle` + `player.png`; สัตว์เลี้ยง/ศัตรู/หมอ/พ่อค้ายังเป็น primitive (M16/M17 ค้าง)
  - Audio: SFX 17 ไฟล์เสร็จ; BGM 7 ไฟล์ (ogg) อยู่ใน `BEPAL/Assets/_candidates/music/` แล้ว แต่**ยังไม่เข้า Content และยังไม่มีระบบเล่นเพลง**; ฟอนต์ยังเป็น `Segoe UI` (สิทธิ์แจกจ่ายไม่ชัด — S8)
  - Movement, ฝน Day 4, passive สัตว์เลี้ยง first pass, telemetry (bot + คนจริง 8 runs, เป้า 15–20) เสร็จแล้ว

## จุดอ่อนที่รู้ (จากการวิเคราะห์ 2026-10-13)
- Design: บอสชนะไม่ได้แต่ผู้เล่นไม่รู้ว่าเป็นจุดจบ demo; Sell ได้ 5000 coin แต่ไม่มีที่ใช้; ไม่มีใครซื้อ Energy upgrade (ไม่รู้ว่าต้องใช้ point / point แพง); Train miss สูงสุดในคนจริง
- UX: เปิด care ได้ตอน Energy = 0 (`noenergy` 21 ครั้ง/8 runs); Heal เลือกได้ตอน HP เต็ม
- Tech debt: ตัวเลข care/shop/upgrade ยังไม่อยู่ใน `Balance`; `validate.ps1` ฝัง path (C10); ไม่มี CI/release build; ShotRunner ไม่มี shot GameOver/Night/PetPick

## การตัดสินใจของผู้ใช้ (2026-10-13)
- บอสต้องแพ้ (demo) แต่ต้องบอกผู้เล่นว่า demo มีถึงแค่นี้
- Merchant เปิดร้านทั้งทาง Sell และ Refuse; ของซื้อเข้า inventory (กล่องในฐาน) ใช้ทีหลังผ่าน UI การ์ด; เพิ่ม Cloudy Glasses, Ballet Shoes, Torn Notebook, Faded Ribbon
- ราคา point: QTE 1 ทุกเลเวล; Energy 1→2→4; Progress 1→2→3 (Progress max 2→3) + ทำให้ชัดว่าต้องใช้ point และได้จากไหน
- ลำดับ: housekeeping → balance migration → energy UX → demo ending → upgrade points → inventory/items → art pipeline; BGM/ฟอนต์/release build ไว้รอบหลัง

## ข้อจำกัด / ข้อควรระวัง
- ภาพ/เสียง/movement ล่าสุดยังไม่ผ่านการเล่นจริงของผู้ใช้ (ตรวจด้วย build + `--shots` + `--autoplay` เท่านั้น)
- Figma MCP มี rate limit — อ่าน Figma ผ่าน layers panel ในเบราว์เซอร์แทนเมื่อติด limit
