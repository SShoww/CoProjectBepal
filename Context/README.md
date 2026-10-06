# Context — ดัชนี

เลือกอ่านตามหัวข้อ ไม่ต้องอ่านทั้งหมด

| เอกสาร | อ่านเมื่อ |
|---|---|
| [Rules.md](Rules.md) | ทุก session — workflow, ขอบเขต, เกณฑ์จบ, หลาย agent, โน้ตหัวข้อล่าสุด |
| [Status.md](Status.md) | ทุก session — สถานะ repo ปัจจุบันและข้อจำกัด |
| [Agent-Handoff.md](Agent-Handoff.md) | ดู/สร้าง/อัปเดต Quest, รับช่วงงาน |
| [quests/](quests/) | งานที่ยังไม่ปิด (หนึ่งไฟล์ต่องาน) |
| [Gitflow.md](Gitflow.md) | สร้าง branch, เขียน commit message, merge/release/hotfix, pre-merge gates |
| [Edit-Log.md](Edit-Log.md) | เมื่อต้องรู้ว่าใครแก้อะไรล่าสุด — ไม่อ่านอัตโนมัติ |
| [archive/](archive/) | ประวัติ (snapshot) — เปิดเมื่อต้องสืบย้อนเท่านั้น |

## เอกสารเฉพาะหัวข้อ (ของเดิม — เจ้าของหลักของแต่ละเรื่อง)

| เรื่อง | เอกสาร | อ่านเมื่อ |
|---|---|---|
| Build / run / verify, สถาปัตยกรรมโค้ด | [../CLAUDE.md](../CLAUDE.md) | แก้โค้ดเกม |
| Pattern MonoGame ใน repo | [../.claude/skills/monogame/SKILL.md](../.claude/skills/monogame/SKILL.md) | แก้ scene/UI/QTE/balance |
| Scope prototype (ชนะเอกสารอื่น) | [../BEPAL/Docs/GDD/06-vertical-slice.md](../BEPAL/Docs/GDD/06-vertical-slice.md) | ตัดสินว่าอะไรอยู่ใน scope |
| ดัชนี GDD + changelog ความขัดแย้งกับ Figma | [../BEPAL/Docs/GDD/README.md](../BEPAL/Docs/GDD/README.md) | งานออกแบบ/เนื้อหาเกม |
| Class diagram (ต้อง sync เมื่อเพิ่ม/เปลี่ยนชื่อ scene) | [../BEPAL/Docs/GDD/04-class-diagram.md](../BEPAL/Docs/GDD/04-class-diagram.md) | เพิ่ม/เปลี่ยนชื่อ scene หรือโฟลเดอร์ |
| Backlog / sprint ของทีม (งานของคน ไม่ใช่ Quest) | [../BEPAL/Docs/Agile/](../BEPAL/Docs/Agile/) | ถามเรื่องแผน sprint |

## เครื่องมือ
- `python tools/context/log-edit.py --quest Q-ID --files "a, b" --summary "..." --validation "..."` — เขียน [Edit-Log.md](Edit-Log.md) (รายละเอียดใน [Rules.md § Edit Log](Rules.md#edit-log))
