### 2026-10-07 — Q-20261007-docs-restructure — อัปเดตและจัดโครงสร้าง Document ให้ตรงโค้ดล่าสุด
- Status: ready-to-close
- Owner: Claude Code (Manager) — Code Analyst, Document Analyst, Document Edit เป็น sub-agent ตามคำสั่งผู้ใช้ 2026-10-07
- Goal: เอกสารตรงกับโค้ดที่ `Develop` @ `64b74d5` · ตรวจรับ: ทุกรายการที่ Manager อนุมัติถูกแก้, ลิงก์ในเอกสารไม่เสีย, ไม่เปลี่ยนโค้ดเกม
- Scope IN: `CLAUDE.md`, `README.md`, `Context/*.md` (ยกเว้น Edit-Log/archive), `BEPAL/Docs/GDD/**`, `BEPAL/Docs/Agile/**` (เฉพาะส่วนที่ขัดกับโค้ด/ล้าสมัย)
- Scope OUT: โค้ดใน `BEPAL/Bepal_Game/**`, เนื้อหาการออกแบบ (GDD intent) ที่ไม่เกี่ยวกับโค้ด, `Context/archive/**`, `.claude/skills/**`
- Files: CLAUDE.md, README.md, Context/Status.md, Context/README.md, BEPAL/Docs/GDD/**, BEPAL/Docs/Agile/**, BEPAL/Assets/_candidates/README.md
- Context: [CLAUDE.md](../../CLAUDE.md), [Gitflow.md](../Gitflow.md) · branch `feature/docs-restructure` · ทับกับ Q-20261007-movement-system (Files: 04-class-diagram.md) — Quest นั้น ready-to-close และ merge แล้ว (PR #6)
- Validation: 2026-10-07 (รอบ 2: อัปเดตตามโค้ด BigZ ATK 20, Toothless pet Hp 60, Passive implemented, Storm clamp 0 รวมสัตว์ตาย, Caffeine ไม่ clamp, Doctor 250, Energy 3/6, End day = เตียง+Space; Manager ตรวจ Storm/Caffeine กับ DayEvents.cs/BaseMenus.cs แล้ว; link check 0 เสีย) รอบ 1: link check 12 ไฟล์ที่แก้ = 0 ลิงก์เสีย; git diff มีเฉพาะไฟล์ที่อนุมัติ (12 ไฟล์ + quest); 00–03 เพิ่มแค่ banner ไม่แก้ตัวเลข; ไม่แตะโค้ด. ยังไม่ได้ commit/PR
- Next action: รอผู้ใช้ยืนยัน ship-branch (commit + PR เข้า Develop) แล้วปิด Quest. ยังไม่แก้ (นอกรายการ/ต้องตัดสินแยก): 03-mechanics มีเนื้อหา design intent ที่ต่างจากโค้ด (Acid Leak Clean −20 Day 2, zone width ±0.20/±0.45 rad, Pet Lv3 ATK +10%, Burn, เกรด S–C), D-06 refactor เข้า Balance, Agile (sprint-plan-02, ปิด Sprint 1)
