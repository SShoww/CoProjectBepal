---
type: agile-sprint-plan
version: 1.0
date: 2026-10-18
project: BePal
---

# Sprint 2 Plan

**Sprint Goal:** ใส่ Art / Audio จริง + Balance + Passive ให้ prototype Day 1–5 ดูและฟังเป็นเกมจริง
**ระยะเวลา:** 2026-10-05 — 2026-10-18
**Team:** โชว์ (Lead Programmer) · ภูมิ (Game Design, UI Lead) · ซุง (Flex, Audio Support) · เดียร์ (2D Art)

> สรุปย้อนหลัง ณ 2026-10-18 จาก `git log` บน `Develop` (PR #1–#36) และ Quest ใน `Context/archive/quests/` + `Context/quests/`

---

## Sprint Backlog

| # | User Story | รับผิดชอบ | MoSCoW | Estimate (SP) | Status |
|---|---|---|---|---|---|
| M16 | Real sprites for the 3 starter pets | เดียร์ (ภาพ) · โชว์ (ท่อโค้ด) | Must Have | 5 | ❌ Blocked — ท่อโค้ดพร้อม ยังไม่มีไฟล์ภาพ |
| M17 | Real sprites for Toothless, Big Z, Merchant, Doctor, Keeper | เดียร์ (ภาพ) · โชว์ (ท่อโค้ด) | Must Have | 5 | 🔄 In Progress — มีแค่ Keeper (`player.png`) |
| M18 | Painted parallax layers for the base | เดียร์ · โชว์ | Must Have | 5 | ✅ Done |
| S4 | Sound effects for QTE and fights | ซุง · โชว์ | Should Have | 3 | ✅ Done |
| S5 | Background music for the base and fights | ซุง · โชว์ | Should Have | 3 | ✅ Done |
| S6 | Playtest and rebalance the numbers | ภูมิ · โชว์ | Should Have | 3 | ✅ Done (ข้อมูลยังน้อย — ดูงานค้าง) |
| S7 | Define each pet's passive skill | ภูมิ · โชว์ | Should Have | 5 | ✅ Done |
| S8 | Cozy UI font and framed UI panels | ภูมิ · โชว์ | Should Have | 3 | ✅ Done (ฟอนต์ title ยังเป็น Personal Use) |
| C1 | Option menu with volume *(ดึงมาจาก Sprint 3)* | โชว์ | Could Have | 2 | ✅ Done |

**Velocity:** 22 / 32 SP ตามแผน (+2 SP จาก C1 ที่ดึงมาก่อน = 24 SP) — ค้าง M16 + M17 (10 SP) เพราะรอภาพ

## Status Legend
- 🔲 Todo
- 🔄 In Progress
- ✅ Done
- ❌ Blocked

---

## Tasks

### Story M16–M17 — Sprite สัตว์ / ศัตรู / NPC
- [x] ท่อโหลด sprite (`Art.LoadOptional`): วาง PNG + ลง `Content.mgcb` แล้วใช้ภาพแทน primitive ทันที ไม่มีไฟล์ = วาด primitive เหมือนเดิม  [owner:: โชว์]  [estimate:: 2h]  [status:: done]
- [x] Keeper sprite จริง (`player.png`) + `SpriteMotion` ขยับตัว  [owner:: เดียร์, โชว์]  [estimate:: 3h]  [status:: done]
- [ ] ภาพ Idle / Happy / Hurt ของ Mossling, Nibbleclaw, Blinkbun  [owner:: เดียร์]  [estimate:: 8h]  [status:: todo]
- [ ] ภาพ Toothless, Big Z (Idle + ท่าโจมตี), Merchant, Doctor  [owner:: เดียร์]  [estimate:: 8h]  [status:: todo]
- [ ] ต่อท่า attack / hurt / happy เข้าโค้ด + ปรับ height constants ใน `Art.cs` ตามภาพจริง  [owner:: โชว์]  [estimate:: 3h]  [status:: todo]

### Story M18 — Parallax
- [x] Pixel-art parallax 5 ชั้น (`bg_parallax_*`, 320x180 วาด 4x) + `fg_jungle` ใน batch `PointClamp`  [owner:: เดียร์, โชว์]  [estimate:: 4h]  [status:: done]
- [x] VFX ฝน + ฟ้าแลบ Day 4 ผ่านหน้าต่างฐาน (`World/Rain.cs`)  [owner:: โชว์]  [estimate:: 2h]  [status:: done]

### Story S4–S5 — Audio
- [x] SFX 17 ไฟล์ + เสียง synth (`Typewriter`, `QtePerfect`) เล่นตามจังหวะ QTE / fight / ประตู  [owner:: ซุง, โชว์]  [estimate:: 4h]  [status:: done]
- [x] BGM 7 เพลง (menu, ฐาน, fight, boss, พ่อค้า/ร้าน, พายุ Day 4, จบ) loop + fade เลือกตาม scene  [owner:: ซุง, โชว์]  [estimate:: 4h]  [status:: done]

### Story S6 — Playtest + Balance
- [x] Telemetry ต่อ run (`--telemetry`, bot 5 profile, `run-batch.ps1`) + Aggregate  [owner:: โชว์]  [estimate:: 5h]  [status:: done]
- [x] เก็บ telemetry ผู้เล่นจริง 8 คน + วิเคราะห์ใน `BEPAL/Docs/Balance/balance-report.md`  [owner:: ภูมิ, โชว์]  [estimate:: 4h]  [status:: done]
- [x] ย้ายตัวเลข care / shop / EXP / Pet เข้า `Balance`  [owner:: โชว์]  [estimate:: 2h]  [status:: done]
- [x] ปรับตามผล: ราคา Point ต่อเลเวล (Energy 1/2/4, Progress 1/2/3 max 3), QTE EXP, Energy 0 เปิด care ไม่ได้, Heal จางเมื่อ HP เต็ม  [owner:: ภูมิ, โชว์]  [estimate:: 3h]  [status:: done]
- [x] แก้ `validate.ps1` (C10) + telemetry เริ่มเกมใหม่ในเซสชันเดียว (C9)  [owner:: โชว์]  [estimate:: 2h]  [status:: done]
- [ ] เก็บ telemetry คนจริงรอบใหม่ให้ถึง 15–20 runs (หลังแก้ Point / Energy / ไอเทม)  [owner:: ภูมิ]  [estimate:: 3h]  [status:: todo]

### Story S7 — Pet Passive (ปลดล็อก LV 2)
- [x] Mossling heal 1% MaxHp ต่อ hit · Nibbleclaw combo · Blinkbun วาร์ปเข็มนำหน้า Dodge (จูน lead time 10-14/10-16) · Toothless พิษ  [owner:: ภูมิ, โชว์]  [estimate:: 5h]  [status:: done]
- [x] Fight: Dodge เกิดหลัง Attack โดน (สลับเฟส)  [owner:: โชว์]  [estimate:: 2h]  [status:: done]

### Story S8 — UI font + panels
- [x] ฟอนต์ title Animal (`fnt_title_display.otf`) + UI Patrick Hand SC (OFL)  [owner:: ภูมิ, โชว์]  [estimate:: 3h]  [status:: done]
- [x] กรอบ panel / ปุ่มแบบ nine-patch (MonoGame.Extended) + จัดขนาดฟอนต์ / ตำแหน่ง UI / เส้นในโซน QTE  [owner:: โชว์]  [estimate:: 4h]  [status:: done]
- [ ] เปลี่ยนฟอนต์ title เป็น license OFL ก่อนเผยแพร่นอกรายวิชา  [owner:: ภูมิ]  [estimate:: 1h]  [status:: todo]

### Story C1 — Options menu
- [x] หน้า Options ปรับ volume เพลง / SFX (Main Menu + Pause) จำค่าใน `%APPDATA%/BePal/settings.txt`  [owner:: โชว์]  [estimate:: 2h]  [status:: done]

### งานนอกแผน (ทำเพิ่มใน Sprint นี้)
- [x] Movement ลื่นขึ้น (Shift วิ่ง, ease, กล้อง) + แก้ตำแหน่งผู้เล่น  [owner:: โชว์]  [status:: done]
- [x] ร้านเปิดทั้ง Sell / Refuse, Box + `InventoryScene`, ไอเทมใหม่ 4 ชิ้น, shop/inventory แบบ carousel  [owner:: โชว์]  [status:: done]
- [x] แพ้ Toothless แล้วกลับฐานไปชุบกับหมอ (ไม่ Game Over)  [owner:: โชว์]  [status:: done]
- [x] หน้าจบบอกชัดว่าเป็น demo  [owner:: โชว์]  [status:: done]
- [x] Dev tools: `--scene <name>` + F2 กระโดดไป scene (`SceneCatalog`)  [owner:: โชว์]  [status:: done]
- [x] ระบบ AI workflow (`AGENTS.md`, `Context/`, Quest, Edit Log) + Gitflow (`Develop`, PR)  [owner:: โชว์]  [status:: done]
- [x] Sync GDD กับโค้ด + แปลง diagram เป็น `.drawio`  [owner:: โชว์]  [status:: done]

---

## ไม่ได้ทำ / ยกไป Sprint 3

| งาน | เหตุผล | ไป Sprint 3 |
|---|---|---|
| M16 sprite สัตว์ 3 ตัว | ยังไม่มีไฟล์ภาพ (ท่อโค้ดพร้อมแล้ว) | ✅ Must |
| M17 sprite Toothless / Big Z / Merchant / Doctor | ยังไม่มีไฟล์ภาพ (มีแค่ Keeper) | ✅ Must — ถ้าไม่ทันตัด Merchant / Doctor ก่อน |
| ท่า attack / hurt / happy ในโค้ด | รอภาพ M16–M17 | ✅ |
| Telemetry คนจริง 15–20 runs | มี 8 runs จาก build ก่อนแก้ Point / Energy / ไอเทม | ✅ |
| ฟอนต์ title OFL | Animal ใช้ได้เฉพาะงานรายวิชา | ✅ ก่อนเผยแพร่ |
| Release build (`dotnet publish -c Release -r win-x64 --self-contained`) | ยังไม่ได้ทำ | ✅ |
| ผู้ใช้เล่นจริงเพื่อยืนยันภาพ / เสียง / UI ล่าสุด | ตรวจด้วย build + `--shots` + `--autoplay` เท่านั้น | ✅ |
| ShotRunner ยังไม่มี shot GameOver / Night / PetPick | ไม่อยู่ในแผน | — |

---

## Daily Notes

### 2026-10-07
**ทำแล้ว:** ตั้ง AI workflow + Gitflow (10-06), SFX 17 ไฟล์, parallax pixel-art, ฝน Day 4, Passive 4 ตัว, Dodge หลัง Attack, movement system, จัดเอกสารใหม่ + `.drawio`
**Blocked:** ยังไม่มี sprite สัตว์ / ศัตรู

### 2026-10-12
**ทำแล้ว:** telemetry ต่อ run + bot 5 profile (10-10), วิเคราะห์ผู้เล่นจริง 8 คน, แก้ telemetry เริ่มเกมใหม่
**Blocked:** ข้อมูลคนจริงยังน้อย (n = 8)

### 2026-10-13
**ทำแล้ว:** BGM 7 เพลง (ซุง), Options menu, ย้ายตัวเลขเข้า `Balance`, Energy UX, หน้าจบ demo, ราคา Point, Box / ไอเทมใหม่, ท่อ sprite, `--scene` / F2, `validate.ps1`, sync GDD
**Blocked:** รอภาพจากเดียร์

### 2026-10-14 – 2026-10-16
**ทำแล้ว:** ฟอนต์ Animal + Patrick Hand SC, UI polish + nine-patch panel, shop / inventory carousel, แพ้ Toothless ไปหาหมอ, sync `.drawio`, จูน Blinkbun lead time
**Blocked:** M16 / M17 ยังไม่มีภาพ

---

## Retrospective (สั้น)
- **ไปได้ดี:** Audio, Passive, Balance tooling, UI ครบตามแผน และดึง C1 จาก Sprint 3 มาทำได้
- **ต้องปรับ:** Art เป็นคอขวด (เหมือนที่คาดไว้ใน Sprint Backlog) — Sprint 3 ควรล็อกรายการภาพขั้นต่ำและวันส่งภาพกับเดียร์ตั้งแต่ต้น Sprint
- **ความเสี่ยง:** final deadline 2026-11-04 — งานภาพ + เล่นจริงยืนยัน + release build ต้องจบใน Sprint 3

---

## Links
- [[00-concept|GDD Concept]]
- [[06-vertical-slice|GDD 06 — Vertical Slice]]
- [[01-product-backlog|Product Backlog]]
- [[02-sprint-backlog|Sprint Backlog]]
- [[sprint-plan-01|Sprint 1 Plan]]
- [[balance-report|Balance Report]]
