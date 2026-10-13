### 2026-10-13 — Q-20261013-options-menu — เมนู Options ปรับ volume
- Status: ready-to-close
- Owner: Claude Code (ตามคำสั่งผู้ใช้ 2026-10-13)
- Goal: เมนู Options (Main Menu + Pause) ปรับ volume เพลง/SFX ด้วยเมาส์ลากหรือคีย์บอร์ด และจำค่าข้ามเซสชัน
- Files: BEPAL/Bepal_Game/Bepal/Core/{Settings,Ui,Input,Audio,MusicPlayer}.cs, Game1.cs, Model/GameState.cs, Scenes/{MenuScenes,BaseScene}.cs, DevTools/ShotRunner.cs, CLAUDE.md, BEPAL/Docs/GDD/{04-class-diagram,06-vertical-slice,14-audio-fonts-list}.md, Context/Status.md
- Context: `Settings.Music/Sfx` (0..1) คูณเข้ากับ `Balance.MusicVolume` และ volume ของแต่ละ SFX; เก็บที่ `%APPDATA%/BePal/settings.txt`; `Settings.Persist=false` เมื่อ --autoplay/--shots (ไม่อ่าน/เขียนไฟล์); Pause เมนูเปิดซ้ำหลังปิด Options
- Validation (2026-10-13): build 0 warning; harness ชั่วคราว (ลบแล้ว) ยิง input จริง: ลาก slider เพลงไป 0.5, Down+Left x2 -> SFX 0.9, Esc ปิดแล้วบันทึกไฟล์ (music=0.5 sfx=0.9), รันใหม่โหลดค่าได้, ปรับต่อเป็น 0.8; `--autoplay` ไม่แตะไฟล์ settings และ -> reached To be continued; shots 01_menu/19_options ตรวจแล้ว. ยังไม่ได้เปิด Options จาก Pause ด้วยมือ/ฟังเสียงว่า volume เปลี่ยนจริง (ตรวจแค่ค่า)
- Next action: ผู้ใช้ลองเปิดจาก Pause + ฟัง; ยืนยันปิด (รวม Q-20261013-bgm ที่ยังค้าง ready-to-close)
