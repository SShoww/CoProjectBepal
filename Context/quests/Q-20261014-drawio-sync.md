### 2026-10-14 — Q-20261014-drawio-sync — อัปเดต diagrams/*.drawio ให้ตรงโค้ด
- Status: ready-to-close
- Owner: Claude Code (ตามคำสั่งผู้ใช้ 2026-10-14)
- Goal: diagram .drawio + PNG ใน GDD ตรงพฤติกรรมโค้ดปัจจุบัน (ไม่มีปุ่ม E, Day 3 Refuse = ร้านค้า ไม่มี Merchant Boss, Day 4 Storm / Day 5 Big Z, Options/Box/Shop); ฟีเจอร์ที่ยังไม่ทำติดป้าย future
- Files: BEPAL/Docs/GDD/diagrams/**, Context/quests/Q-20261014-drawio-sync.md, Context/Edit-Log.md
- Context: ต่อจาก Q-20261013-gdd-sync (ข้าม drawio); ground truth = 06-vertical-slice, 13 As built, 04-class-diagram; BePal_WinLose_CoreFlow.drawio ตรวจแล้วตรงโค้ดอยู่แล้ว ไม่แก้
- Validation (2026-10-14): แก้ 6 .drawio + render PNG ใหม่ (Edge headless + draw.io viewer) และดูภาพทีละไฟล์; XML parse ผ่าน; แก้เฉพาะเอกสาร ไม่ build/autoplay; ยังไม่ได้เปิดใน VS Code Draw.io จริง; scene-flow/scene-breakdown เส้นหนาแน่นบางจุด (label ชิดเส้น)
- Next action: ผู้ใช้ยืนยันปิด Quest; ดูใน VS Code จริงถ้าอยากจัดวางเพิ่ม
