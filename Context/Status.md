# Status — บริบทปัจจุบัน

ไม่ใช่ backlog (งานของทีมอยู่ที่ [BEPAL/Docs/Agile/](../BEPAL/Docs/Agile/), งาน AI อยู่ที่ [quests/](quests/))
ข้อมูลในไฟล์นี้เป็นสิ่งที่ตรวจ ณ วันที่ระบุ — ยืนยันใหม่ก่อนอ้างเป็นสถานะ runtime ปัจจุบัน

**ตรวจล่าสุด:** 2026-10-07 โดย Claude Code (Q-20261007-docs-restructure) · แหล่ง: `git log`, code report (อ่านโค้ดบน `Develop` @ `64b74d5`)

## ปัจจุบัน
- ใช้ Gitflow ([Gitflow.md](Gitflow.md)): `Develop` รวม PR ถึง #7 แล้ว; branch ปัจจุบัน `feature/docs-restructure`
- อยู่ใน Sprint 2 (2026-10-05 → 2026-10-18): Art/Audio จริง + Balance + Passive — Sprint 1 (prototype Day 1–5) กำหนดส่ง 2026-09-30 (ยังไม่ได้ยืนยันผลส่ง)
- สถานะตามโค้ดจริง (as-built):
  - Art: parallax 5 ชั้น + `fg_jungle` + `player.png` (sprite ผู้เล่น); สัตว์เลี้ยง/ศัตรู/หมอ/พ่อค้ายังเป็น primitive
  - Audio: SFX 17 ไฟล์ใน `Content/Sfx/` เสร็จ (ยังไม่มีเพลง)
  - Movement system เสร็จ (Q-20261007-movement-system ready-to-close): วิ่ง, กล้อง/UI easing, กำแพง, pause menu, F3 overlay
  - ฝน Day 4 (`World/Rain.cs`)
  - Passive สัตว์เลี้ยง: first pass รอ balance; Toothless balance อยู่ใน `Balance` (`ToothlessDodgeShrink`, `ToothlessNeedleSpeed`)

## ข้อจำกัด / ข้อควรระวัง
- ภาพ/เสียง/movement ล่าสุดยังไม่ผ่านการเล่นจริงของผู้ใช้ (ตรวจด้วย build + `--shots` + `--autoplay` เท่านั้น)
- Figma MCP มี rate limit — อ่าน Figma ผ่าน layers panel ในเบราว์เซอร์แทนเมื่อติด limit
