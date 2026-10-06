# Status — บริบทปัจจุบัน

ไม่ใช่ backlog (งานของทีมอยู่ที่ [BEPAL/Docs/Agile/](../BEPAL/Docs/Agile/), งาน AI อยู่ที่ [quests/](quests/))
ข้อมูลในไฟล์นี้เป็นสิ่งที่ตรวจ ณ วันที่ระบุ — ยืนยันใหม่ก่อนอ้างเป็นสถานะ runtime ปัจจุบัน

**ตรวจล่าสุด:** 2026-10-06 โดย Claude Code (Q-20261006-ai-setup) · แหล่ง: `git status`, `git log -5`, [02-sprint-backlog.md](../BEPAL/Docs/Agile/02-sprint-backlog.md)

## ปัจจุบัน
- ใช้ Gitflow ([Gitflow.md](Gitflow.md)) ตั้งแต่ 2026-10-06: `main` = `Develop` = `0cb2715`; งาน AI Setup + Gitflow + Toothless balance อยู่ใน branch `ai-setup` → [PR #1](https://github.com/SShoww/CoProjectBepal/pull/1) เข้า `Develop` (gates ผ่าน 2026-10-06, รอ review)
- อยู่ใน Sprint 2 (2026-10-05 → 2026-10-18): Art/Audio จริง + Balance + Passive — Sprint 1 (prototype Day 1–5) ส่งแล้วตามแผน 2026-09-30 (ยังไม่ได้ยืนยันผลส่ง)
- ระบบ AI Setup (README/AGENTS/Context/tools) ติดตั้ง 2026-10-06

## ข้อจำกัด / ข้อควรระวัง
- Toothless balance ใน `Model/Pet.cs` (DodgeShrink 0.05, NeedleSpeed 2) commit ตามคำสั่งผู้ใช้ 2026-10-06 — build 0 warning + `--autoplay` (ปกติ/`--refuse`) ผ่าน 2026-10-06
- ไม่มี unit-test project: verification ของเกม = build + `--autoplay` + `--shots` ([CLAUDE.md](../CLAUDE.md))
- `.gitignore` ignore `*.log`, `[Ll]og/`, `.obsidian/` — Edit Log ใช้ `.md` จึงไม่โดน ignore
- Figma MCP มี rate limit — อ่าน Figma ผ่าน layers panel ในเบราว์เซอร์แทนเมื่อติด limit
