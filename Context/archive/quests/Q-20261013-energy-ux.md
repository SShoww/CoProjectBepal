### 2026-10-13 — Q-20261013-energy-ux — Energy 0 เปิด care ไม่ได้ + Heal ตอน HP เต็ม
- Status: ready-to-close
- Owner: Claude Code (ตามคำสั่งผู้ใช้ 2026-10-13)
- Goal: Energy <= 0 แล้วเดินไปหา pet ไม่เปิด CareSelect แต่แสดงข้อความ; HP เต็ม -> zone Heal จาง + label "Heal (HP full)" กดแล้วนับเป็น gap (ไม่หัก Energy)
- Files: BEPAL/Bepal_Game/Bepal/Scenes/BaseScene.cs, BEPAL/Bepal_Game/Bepal/Scenes/CareScenes.cs, BEPAL/Bepal_Game/Bepal/Scenes/Wheel.cs, BEPAL/Bepal_Game/Bepal/Model/GameState.cs, BEPAL/Bepal_Game/Bepal/DevTools/AutoPlay.cs, BEPAL/Bepal_Game/Bepal/DevTools/ShotRunner.cs
- Context: telemetry `care_select`/`noenergy` ยังถูกส่ง แต่จาก BaseScene; เพิ่ม `Zone.Disabled` (Wheel.Evaluate ข้าม) + `Balance.DisabledZoneAlpha`; เพิ่ม shot `06b_care_hurt`
- Validation (2026-10-13): build 0 warning; `--autoplay` และ `--refuse` -> reached To be continued; `--shots` ดู 06_care (Heal จาง) / 06b_care_hurt (Heal ปกติ) แล้ว; run-batch 10 seeds x 5 profile: 48 to_be_continued + 2 game_over (sloppy, เท่า baseline), noenergy = 0. พบ soft-lock ของ human bot (random target = Heal ตอน HP เต็ม) แล้วแก้ใน AutoPlay. ยังไม่ได้ดู popup "No energy" ด้วยตา (ไม่มี shot)
- Next action: ผู้ใช้ยืนยันปิด; ถัดไป Q-20261013-demo-ending
