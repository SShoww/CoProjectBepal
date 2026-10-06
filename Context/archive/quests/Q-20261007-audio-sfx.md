### 2026-10-07 — Q-20261007-audio-sfx — ใส่ SFX เข้าเกม
- Status: ready-to-close (ผู้ใช้ยืนยันปิด 2026-10-07)
- Owner: Claude Code (Manager) — branch `feature/audio-system`
- Goal: SFX จาก `D:\Downloads\BEPAL SOUNDeffect\BEPAL_Sound_effect` เล่นในเกมตามจุดที่เหมาะสม; build 0 warning + `--autoplay` (ปกติ/`--refuse`) ผ่านโดยไม่ soft-lock
- Files: `BEPAL/Assets/_candidates/sfx/**`, `BEPAL/Bepal_Game/Bepal/Content/Content.mgcb`, `BEPAL/Bepal_Game/Bepal/Content/Sfx/**`, `BEPAL/Bepal_Game/Bepal/Core/Audio.cs`, `BEPAL/Bepal_Game/Bepal/Scenes/**`, `BEPAL/Bepal_Game/Bepal/Game1.cs`, `Context/**`
- Context: [Assets README](../../BEPAL/Assets/_candidates/README.md); ต้นฉบับมี 18 เสียง × 6 ฟอร์แมต (aiff/flac/m4a/mp3/ogg/wav)
- Decision: QtePerfect และ Typewriter เป็นเสียง synth (สร้างตอนเปิดเกมใน `Core/Audio.cs`, ไม่มีไฟล์) — ไฟล์ Perfect เดิมแหลมเจ็บหู (พลังงาน 99% ที่ ~3.8 kHz); ที่เหลือ 17 ไฟล์ใช้ WAV 16-bit PCM (MGCB `WavImporter` + `SoundEffectProcessor`) — SFX สั้น latency ต่ำ ไม่เสี่ยง codec; ขนาดรวมเล็ก
- Validation: `dotnet build` 0 warning/0 error; `--autoplay` และ `--autoplay --refuse` → `RESULT: reached To be continued`; `--shots` ได้ 19 ไฟล์; 18 .xnb ถูก build; รันเกมจริง (ไม่ mute) 12 วินาทีไม่มี error `[audio]` — **ยังไม่ได้ฟังเสียงจริง/ปรับ volume** (autoplay/shots mute ไว้)
- Next action: ปิดแล้ว — ผู้ใช้ฟังเสียงจริงและยืนยันผ่านเมื่อ 2026-10-07. ค้าง: BGM ยังไม่ทำ; `CLAUDE.md` ยังเขียนว่ามีแค่ font เป็น content (ควรอัปเดตเป็น Sfx + `Core/Audio.cs`)
