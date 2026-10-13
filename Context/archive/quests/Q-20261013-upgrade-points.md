### 2026-10-13 — Q-20261013-upgrade-points — ราคา Point ต่อเลเวล + ทำให้ชัดว่าต้องใช้ Point
- Status: ready-to-close
- Owner: Claude Code (ตามคำสั่งผู้ใช้ 2026-10-13)
- Goal: QTE 1/1/1, Energy 1/2/4, Progress 1/2/3 point (Progress max 2->3); Upgrade scene/HUD/ข้อความแรกบอกที่มาและวิธีใช้ Point
- Files: BEPAL/Bepal_Game/Bepal/Model/GameState.cs, BEPAL/Bepal_Game/Bepal/Scenes/BaseMenus.cs, BEPAL/Bepal_Game/Bepal/Scenes/BaseScene.cs, BEPAL/Bepal_Game/Bepal/Scenes/CareScenes.cs, BEPAL/Bepal_Game/Bepal/DevTools/AutoPlay.cs, BEPAL/Bepal_Game/Bepal/DevTools/ShotRunner.cs, BEPAL/Docs/Balance/tools/validate.ps1, BEPAL/Docs/Balance/telemetry-spec.md, BEPAL/Docs/GDD/06-vertical-slice.md
- Context: `Balance.PointCost(row, level)` แทน `*Points`; `GameState.PointHintShown`; telemetry `pointCost` = ราคาของเลเวลที่ซื้อ; ไม่แก้ doc วิเคราะห์เก่าใน Balance/analysis (เป็นประวัติ)
- Validation (2026-10-13): build 0 warning; `--autoplay`/`--refuse` -> reached To be continued; shots 10_upgrade/10b_upgrade_poor/03_base ตรวจแล้ว; run-batch 10 seeds x 5 profile: 47 to_be_continued + 3 game_over (sloppy x2 เท่า baseline, human seed 3 day 2 ใหม่ — human bot สุ่ม ไม่ใช่ soft-lock), human ซื้อ Energy 4/10 run, upgrade_buy 61 ครั้ง cost ตรงตารางทุกครั้ง. ไม่ได้รัน validate.ps1 (ฝัง path data/raw, C10) แต่แก้ตารางราคาในนั้นแล้ว ยังไม่ได้ทดสอบ; ไม่ได้ดูข้อความ hint แรกด้วยตา
- Next action: ผู้ใช้ยืนยันปิด; ถัดไป Q-20261013-inventory-items; ควรเก็บ telemetry คนจริงเพิ่มเพื่อดูว่าซื้อ Energy มากขึ้นไหม
