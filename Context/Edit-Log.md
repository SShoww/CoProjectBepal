# Edit Log

ล่าสุด 10 งาน งานละ 3 บรรทัด เขียนผ่าน `tools/context/log-edit.py` เท่านั้น — รายการเก่าอยู่ที่ [archive/edit-log/](archive/edit-log/)

<!-- entries -->
- **2026-10-07 01:59 · Q-20261007-audio-sfx** — เพิ่ม Audio/Sfx: SFX 17 ไฟล์ WAV + synth (QtePerfect, Typewriter), เคาะประตูวนจนเปิด; ปิด Quest
  - Files: BEPAL/Bepal_Game/Bepal/Core/Audio.cs, BEPAL/Bepal_Game/Bepal/Content/Sfx, BEPAL/Bepal_Game/Bepal/Scenes, BEPAL/Assets/_candidates
  - Validation: build 0 warn, autoplay ปกติ/--refuse ผ่าน; ผู้ใช้ฟังเสียงจริงและยืนยัน 2026-10-07
- **2026-10-06 23:22 · Q-20261006-gitflow-doc** — เพิ่ม Gitflow.md (ปรับคำสั่งให้ตรง repo) และเริ่มใช้: สร้าง/push Develop จาก main, AGENTS.md ห้าม commit ตรงลง main/Develop
  - Files: Context/Gitflow.md, Context/README.md, Context/Status.md, AGENTS.md
  - Validation: ลิงก์ 0 เสีย; ls-remote เห็น Develop=main=0cb2715; ไม่ได้รัน build/gates
- **2026-10-06 22:41 · Q-20261006-ai-setup** — ติดตั้งระบบ AI Setup: ทางเข้า README/AGENTS, Context (Rules/Status/Handoff/quests/archive), Edit Log tool; ไม่แตะโค้ดเกม
  - Files: README.md, AGENTS.md, CLAUDE.md, .gitignore, Context/**, tools/context/log-edit.py
  - Validation: ลิงก์ 0 เสีย, ไม่มี secret, log tool ผ่าน input/ซ้ำ/retention/concurrency/lock; ไม่ได้ build เกม
