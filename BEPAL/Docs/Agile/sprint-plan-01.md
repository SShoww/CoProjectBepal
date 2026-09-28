---
type: agile-sprint-plan
version: 1.0
date: 2026-09-28
project: BePal
---

# Sprint 1 Plan

**Sprint Goal:** Prototype เล่นได้ครบ Day 1–5 ตั้งแต่เลือกสัตว์จนถึงฉาก Big Z "To be continued..." ให้ทันเดโมวันพุธ 30 ก.ย.
**ระยะเวลา:** 2026-09-21 — 2026-10-04
**Team:** โชว์ (Lead Programmer) · ภูมิ (Game Design, UI Lead) · ซุง (Flex, Audio Support) · เดียร์ (2D Art)

---

## Sprint Backlog

| # | User Story | รับผิดชอบ | MoSCoW | Estimate (SP) | Status |
|---|---|---|---|---|---|
| M1 | Choose 1 of 3 starter pets | โชว์ | Must Have | 3 | ✅ Done |
| M2 | Walk around the sanctuary with A/D and interact with Space | โชว์ | Must Have | 5 | ✅ Done |
| M3 | Pick a care action from a spinning wheel | โชว์ | Must Have | 3 | ✅ Done |
| M4 | Train QTE | โชว์ | Must Have | 3 | ✅ Done |
| M5 | Feed, Clean and Heal QTEs | โชว์ | Must Have | 5 | ✅ Done |
| M6 | Daily Stomach / Clean decay | โชว์ | Must Have | 3 | ✅ Done |
| M7 | Energy budget + end the day at the bed | โชว์ | Must Have | 3 | ✅ Done |
| M8 | Player EXP from QTE hits | โชว์ | Must Have | 2 | ✅ Done |
| M9 | Toothless on Day 2 (Chase / Tame) | โชว์ | Must Have | 3 | ✅ Done |
| M10 | Dodge / Attack fight wheel | โชว์ | Must Have | 8 | ✅ Done |
| M11 | Merchant on Day 3 (Sell / Refuse + shop) | โชว์ | Must Have | 5 | ✅ Done |
| M12 | Thunderstorm on Day 4 | โชว์ | Must Have | 2 | ✅ Done |
| M13 | Big Z on Day 5 + To be continued | โชว์ | Must Have | 3 | ✅ Done |
| M14 | Doctor revive (250 coin) | โชว์ | Must Have | 2 | ✅ Done |
| M15 | Game Over when every pet falls | โชว์ | Must Have | 2 | ✅ Done |
| S1 | Notebook (pet discovery / Disaster) | โชว์ | Should Have | 3 | ✅ Done |
| S2 | Upgrade station | โชว์ | Should Have | 3 | ✅ Done |
| S3 | Day/night sky + parallax | โชว์ | Should Have | 3 | ✅ Done |

**Velocity:** 61 / 61 SP เสร็จ (โค้ดทั้งหมดยังใช้ภาพ placeholder)

## Status Legend
- 🔲 Todo
- 🔄 In Progress
- ✅ Done
- ❌ Blocked

---

## Tasks

### Design & Docs — GDD
- [x] Sync GDD จาก Figma (v2.1)  [owner:: ภูมิ]  [estimate:: 3h]  [status:: done]
- [x] สรุปขอบเขต Vertical Slice ลง GDD 06  [owner:: ภูมิ]  [estimate:: 2h]  [status:: done]
- [x] โครงสร้างโค้ดลง GDD 04  [owner:: โชว์]  [estimate:: 1h]  [status:: done]

### Story M1–M2 — Starter pet + Base
- [x] หน้า Choose your pet (hover กรอบเรืองแสง)  [owner:: โชว์]  [estimate:: 2h]  [status:: done]
- [x] ฐาน side-scrolling กว้าง 3840 px + กล้องตามตัว  [owner:: โชว์]  [estimate:: 4h]  [status:: done]
- [x] ป้าย `[Space]` เหนือของที่อยู่ใกล้ + HUD (Day, Coin, Energy, Player LV)  [owner:: โชว์]  [estimate:: 2h]  [status:: done]

### Story M3–M5 — Care wheel + QTE
- [x] วงล้อเลือก Care 4 สี + จอสั่นเมื่อกดโดน  [owner:: โชว์]  [estimate:: 2h]  [status:: done]
- [x] QTE Train / Feed / Clean / Heal ตาม Figma Scene 5–8  [owner:: โชว์]  [estimate:: 6h]  [status:: done]
- [x] ข้อความ Perfect / Great / Miss  [owner:: โชว์]  [estimate:: 1h]  [status:: done]

### Story M6–M8 — Stats, Energy, EXP
- [x] Daily decay + Stomach / Clean penalties  [owner:: โชว์]  [estimate:: 2h]  [status:: done]
- [x] เตียง End day + ฉากกลางคืน + สรุปผลตอนเช้า  [owner:: โชว์]  [estimate:: 2h]  [status:: done]
- [x] Player EXP / Points  [owner:: โชว์]  [estimate:: 1h]  [status:: done]

### Story M9–M15 — Day events + Fight
- [x] Dialogue / Choice / Pet pick UI  [owner:: โชว์]  [estimate:: 3h]  [status:: done]
- [x] Fight wheel Dodge / Attack + สลับสัตว์  [owner:: โชว์]  [estimate:: 6h]  [status:: done]
- [x] Event Day 2–5 (Toothless, Merchant + Shop, Thunderstorm, Big Z)  [owner:: โชว์]  [estimate:: 4h]  [status:: done]
- [x] Doctor, Game Over, To be continued  [owner:: โชว์]  [estimate:: 2h]  [status:: done]
- [x] บอททดสอบ `--autoplay` เล่นถึงฉากจบได้ทุกเส้นทาง  [owner:: โชว์]  [estimate:: 2h]  [status:: done]

### Story S1–S3 — Notebook, Upgrade, Parallax
- [x] Notebook + Upgrade station  [owner:: โชว์]  [estimate:: 3h]  [status:: done]
- [x] Parallax 5 layer + กลางวัน/กลางคืน  [owner:: โชว์]  [estimate:: 3h]  [status:: done]

### Demo Prep — ก่อนพุธ 30 ก.ย.
- [ ] ทุกคนลองเล่นครบ 5 วัน จดจุดที่ยาก/งง  [owner:: ทุกคน]  [estimate:: 1h]  [status:: todo]
- [ ] ปรับตัวเลขใน `Balance` ตามผลลองเล่น  [owner:: ภูมิ]  [estimate:: 2h]  [status:: todo]
- [ ] เริ่มสเก็ตช์สัตว์ 3 ตัวแรก (เตรียม Sprint 2)  [owner:: เดียร์]  [estimate:: 4h]  [status:: todo]
- [ ] หา SFX ชุดแรก (Perfect / Miss / Knock)  [owner:: ซุง]  [estimate:: 2h]  [status:: todo]
- [ ] เตรียมสคริปต์เดโม (เส้นทางที่จะโชว์)  [owner:: ภูมิ]  [estimate:: 1h]  [status:: todo]

---

## Daily Notes

### 2026-09-28
**เมื่อวาน:** Sync GDD จาก Figma, ตัดสินขอบเขต Vertical Slice (Day 1–5, side-scroller แบบ Kingdom)
**วันนี้:** สร้างเกมใน `BEPAL/Bepal_Game/Bepal/` ครบทุก Story ของ Sprint 1 ทดสอบด้วยบอทแล้วเล่นถึงฉากจบได้ทุกเส้นทาง
**Blocked:** ยังไม่มี Art / Audio จริง (ใช้ placeholder) · Passive ของสัตว์รอคุยออกแบบ

---

## Links
- [[00-concept|GDD Concept]]
- [[06-vertical-slice|GDD 06 — Vertical Slice]]
- [[01-product-backlog|Product Backlog]]
- [[02-sprint-backlog|Sprint Backlog]]
