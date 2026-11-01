### 2026-11-01 — Q-20261101-fullscreen — Full screen โดยไม่เปลี่ยนขนาดเกม (BoxingViewportAdapter)
- Status: ready-to-close
- Owner: Claude Code (ตามคำสั่งผู้ใช้ 2026-11-01)
- Goal: เพิ่ม `using MonoGame.Extended;` + `using MonoGame.Extended.ViewportAdapters;` และให้เกมเต็มจอได้ โดยเกมยังวาดที่ 1280x720 (ขยายคงสัดส่วน + แถบดำ)
- Files: BEPAL/Bepal_Game/Bepal/Game1.cs, BEPAL/Bepal_Game/Bepal/Core/Gfx.cs, BEPAL/Bepal_Game/Bepal/Core/Input.cs, BEPAL/Bepal_Game/Bepal/Core/Settings.cs, CLAUDE.md, BEPAL/Docs/GDD/04-class-diagram.md, Context/quests/Q-20261101-fullscreen.md
- Context: `Gfx.Viewport` = `BoxingViewportAdapter` (ไม่สร้างตอน `--shots`/`--autoplay`); scale matrix รวมใน `Gfx.ViewMatrix` (ทุก `Gfx.Begin` + `Gfx.Arc`); `Input.Mouse` แปลงกลับเป็นพิกัดเกมด้วย `PointToScreen`; F11 / Alt+Enter = สลับ full screen แบบ borderless (`HardwareModeSwitch=false`); จำใน settings.txt (`fullscreen=1`); หน้าต่างลากขยายได้
- Validation (2026-11-01): build ผ่าน; `--shots` 29 PNG ขนาด 1280x720; `--autoplay` -> RESULT: reached To be continued; slnx build 0 warn/0 err; ผู้ใช้ทดสอบ F11 ในเกมจริงแล้วผ่าน (2026-11-01)
- Next action: merge เข้า Develop แล้ว รอผู้ใช้ยืนยันปิด quest
