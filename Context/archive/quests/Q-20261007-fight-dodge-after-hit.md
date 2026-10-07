### 2026-10-07 — Q-20261007-fight-dodge-after-hit — Fight QTE: Dodge เกิดหลัง Attack โดน
- Status: ready-to-close  (ผู้ใช้เล่นทดสอบและสั่งปิด + commit/push/merge 2026-10-07)
- Owner: Claude Code (Sonnet 5.5) session 5ef5afc1 — branch `feature/fight-dodge-after-hit`
- Goal: ใน Fight ตอนนี้ Dodge กับ Attack อยู่บน Wheel พร้อมกันตลอด → เปลี่ยนเป็นสลับเฟส: เริ่มด้วย Attack อย่างเดียว; Attack โดน (Perfect/Great) แล้วศัตรูยังไม่ตาย → Attack หาย แล้ว Dodge โผล่ (หดลงเหมือนเดิม); Dodge สำเร็จ = หลบ, Dodge พลาด/ช้าเกิน = สัตว์โดนตี; จบเฟส Dodge → กลับเฟส Attack
- Decisions (ตั้งเอง รอผู้ใช้แก้ได้): กด Attack พลาด (Miss) = สัตว์โดนตีทันทีและอยู่เฟส Attack เดิม (ไม่เปิด Dodge); ศัตรูตายจาก Attack = ไม่เปิด Dodge; สลับสัตว์หลังตาย = เริ่มเฟส Attack
- Files: `BEPAL/Bepal_Game/Bepal/Scenes/FightScene.cs`, `BEPAL/Bepal_Game/Bepal/Model/GameState.cs` (Balance), `BEPAL/Bepal_Game/Bepal/Model/Pet.cs` (Enemy อ่านค่าจาก Balance)
- Context: [06-vertical-slice.md](../../BEPAL/Docs/GDD/06-vertical-slice.md) บรรทัด Scene 18 (Fight) — GDD ไม่ได้ระบุว่าอยู่พร้อมกัน; AutoPlay กด Space เมื่อ `Wheel.Evaluate().hit != Miss` จึงควรใช้ได้เลย
- Validation: dotnet build ผ่าน (0 warn/0 error); --autoplay = RESULT: reached To be continued; --shots 08_fight, 09_fight_bigz ถูกต้อง; ผู้ใช้เล่นมือแล้วผ่าน. เพิ่มระหว่างทาง: Attack กว้างเท่า Dodge และหดได้, ย้ายตัวเลข Fight (zone, damage, enemy, visual) เข้า Balance เป็นหมวด
- Next action: ผู้ใช้ลองเล่น Fight จริง แล้วยืนยันปิด (ย้ายไป archive/quests/) และตัดสินพฤติกรรม Miss ใน Decisions
