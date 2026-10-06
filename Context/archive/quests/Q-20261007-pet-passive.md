### 2026-10-07 — Q-20261007-pet-passive — Pet Passive (unlock Lv2) ตาม Figma screenshots
- Status: closed (ผู้ใช้สั่งปิด 2026-10-07)
- Owner: Claude Code (session 5de13596)
- Goal: Mossling heal 1% MaxHp ต่อ attack hit, Toothless ใส่พิษ (ศัตรูรับดาเมจ+ / ตีเบาลง, ไม่ stack), Blinkbun วาร์ปเข็มไป 12 นาฬิกา; ปลดล็อกที่ Lv2; ข้อมูลแสดงในสมุด (Notebook)
- Files: BEPAL/Bepal_Game/Bepal/Model/Pet.cs, BEPAL/Bepal_Game/Bepal/Model/GameState.cs, BEPAL/Bepal_Game/Bepal/Scenes/FightScene.cs, BEPAL/Bepal_Game/Bepal/Scenes/BaseMenus.cs, BEPAL/Docs/GDD/04-class-diagram.md
- Context: 06-vertical-slice.md (passive เดิมนอก prototype, Sprint 2 รวม S7); ตัวเลขพิษเป็นค่าเริ่มต้น "ลองแบบนี้ไปก่อนค่อย balance"; Nibbleclaw ยังไม่มี passive
- Validation: dotnet build 0 warn; --autoplay + --autoplay --refuse = RESULT: reached To be continued; --shots 12b/12c ดูหน้า passive ถูกต้อง (Lv1 ล็อก, Lv2 แสดง). ยังไม่ได้ทดสอบ effect ใน fight ด้วยมือ (autoplay ใช้ pet Lv1)
- Next action: ผู้ใช้เล่นทดสอบ + ปรับ Balance.Poison*/MosslingHealPct; ตัดสินใจ passive ของ Nibbleclaw; ยืนยันปิด Quest
