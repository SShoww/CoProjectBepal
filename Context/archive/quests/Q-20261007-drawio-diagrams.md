### 2026-10-07 — Q-20261007-drawio-diagrams — แปลง Mermaid ใน GDD เป็น Draw.io
- Status: ready-to-close
- Owner: Claude Code (Manager) — Researcher / Coder / Reviewer เป็น sub-agent ตามคำสั่งผู้ใช้ 2026-10-07
- Goal: สร้าง .drawio (VS Code Draw.io Integration) แทน mermaid flowchart/state ของ GDD 6 บล็อก · ตรวจรับ: Reviewer export PNG เทียบ mermaid เดิม = PASS (node ครบ, ทิศ edge, สี, label ไทย) · Manager ไม่แก้ .drawio เอง, วนแก้เกิน 5 รอบ → รายงานผู้ใช้
  - Scope (ผู้ใช้ยืนยัน): 01-core-loop (2), 03-mechanics, 04-class-diagram, 11-narrative-world, 13-scene-breakdown · ไม่รวม gantt/asset pipeline
  - Reference = mermaid เดิม · ผู้ใช้สั่งภายหลัง: ลบ mermaid ออกจาก md เหลือ PNG + ลิงก์ + รูป PNG (diagrams/*.png render จาก .drawio) ไป .drawio
- Files: BEPAL/Docs/GDD/diagrams/**, BEPAL/Docs/GDD/01-core-loop.md, 03-mechanics.md, 04-class-diagram.md, 11-narrative-world.md, 13-scene-breakdown.md (ลบ mermaid, ใส่ PNG + ลิงก์; mermaid ต้นฉบับสำรองที่ diagrams/mermaid-source.md)
- Context: [BePal_WinLose_CoreFlow.drawio](../../BEPAL/Docs/GDD/BePal_WinLose_CoreFlow.drawio) (ไฟล์ drawio เดิม ไม่แตะ) · branch `feature/drawio-diagrams`
- Validation: Reviewer 3 รอบ (Coder แก้ 2 รอบ) → 6 ไฟล์ PASS: node/edge/label ตรง mermaid ทีละเส้น, ทิศ, style เดียวกัน, ไม่มี edge ทะลุ node (render Edge headless + draw.io viewer). ยังไม่ได้เปิดใน VS Code GUI จริง; scene-breakdown/state-machine มี label ชิดเส้นเล็กน้อย; PNG render ด้วย Edge headless + draw.io viewer (state-machine.png มีที่ว่างล่างเกิน)
- Next action: ผู้ใช้เปิด .drawio ใน VS Code ดูจริง แล้วยืนยันปิด Quest + commit/ship (ยังไม่ได้ commit)
