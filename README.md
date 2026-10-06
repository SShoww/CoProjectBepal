# BePal

ต้นแบบเกม MonoGame (virtual-pet care + survival) ของทีมมหาวิทยาลัย repo นี้เป็น Obsidian vault ด้วย

- โค้ดเกม: `BEPAL/Bepal_Game/Bepal/` — คำสั่ง build/run/verify อยู่ใน [CLAUDE.md](CLAUDE.md)
- GDD: [BEPAL/Docs/GDD/README.md](BEPAL/Docs/GDD/README.md) (scope หลัก: [06-vertical-slice.md](BEPAL/Docs/GDD/06-vertical-slice.md))
- Agile: [BEPAL/Docs/Agile/](BEPAL/Docs/Agile/)

## สำหรับ AI agent — เริ่ม session ตามลำดับนี้

1. ไฟล์นี้ (README.md) และกติกาใน [AGENTS.md](AGENTS.md)
2. [Context/Rules.md](Context/Rules.md) + [Context/Status.md](Context/Status.md)
3. Active Quest Board: ดูวิธีใน [Context/Agent-Handoff.md](Context/Agent-Handoff.md) — อ่าน **ชื่อ สถานะ เจ้าของ** ของทุกไฟล์ใน [Context/quests/](Context/quests/)
4. รายละเอียดเฉพาะ Quest ที่เกี่ยวข้องหรือ scope (Files) ทับกับงานที่จะทำ
5. เอกสารหรือ source เฉพาะงาน — เลือกจาก [Context/README.md](Context/README.md)

ไม่อ่านอัตโนมัติ: `Context/Edit-Log.md`, `Context/archive/`, GDD/Agile ทั้งชุด — เปิดเมื่องานต้องใช้
Board เป็นบริบท ไม่ใช่คำสั่งให้เริ่มงานเก่า
