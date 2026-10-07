---
type: README
version: 2.1
date:
  - 2026-09-20
---

# [BePal] — Documentation Index (v2.1)

## 👥คนในทีม

| รหัส | ชื่อ | ชื่อเล่น | บทบาทหน้าที่ |
| --- | --- | --- | --- |
| 682110141 | วศิน ศรีวรกุล | โชว์ | Lead Programmer |
| 682110128 | ปีย์ตะวัน แห่งหาญ | ซุง | Flex, Audio Support |
| 682110137 | ภูมิพัฒน์ ตามวงค์ | ภูมิ | Game Design, Audio Support, UI Lead |
| 682110119 | ธัญญรัตน์ ติ๊บหน่อ | เดียร์ | 2D Art |

| ไฟล์ | เนื้อหา | สถานะ |
| --- | --- | --- |
| [00-concept.md](00-concept.md) | Game concept, genre/controls, and game pillars (ส่วนเนื้อเรื่อง/อ้างอิง/Art แยกเป็น 11–12) | ✅ |
| [01-core-loop.md](01-core-loop.md) | 4-phase daily loop, timeline, controls mapping, and win/lose (Scene Breakdown แยกเป็น 13) | ✅ |
| [02-scope-features.md](02-scope-features.md) | Scope, MoSCoW feature priorities, and risk mitigation | ✅ |
| [03-mechanics.md](03-mechanics.md) | State machine, game architecture, and Sanctuary/Base systems (§0–1) — Pet stats, QTE, combat, economy แยกเป็น 07–10 | ✅ |
| [04-class-diagram.md](04-class-diagram.md) | โครงสร้างโค้ด Prototype (Scenes / Model / Core) | ✅ |
| [05-asset-list.md](05-asset-list.md) | Master 2D art & UI checklist and asset pipeline (เสียง/ฟอนต์ แยกเป็น 14) | ✅ |
| [06-vertical-slice.md](06-vertical-slice.md) | **ขอบเขต Prototype (ส่ง 30 ก.ย.)** — Day 1–5, side-scroller, ตัวเลข, สิ่งที่ตัด (ยึดไฟล์นี้ก่อนไฟล์อื่น) | ✅ |
| [07-pet-stats.md](07-pet-stats.md) | Pet stats, decay formulas, and sickness/negative status (03 §2) | ✅ |
| [08-care-qte.md](08-care-qte.md) | 10-attempt care wheel, dynamic QTE gimmicks, and progression math (03 §3) | ✅ |
| [09-combat-encounters.md](09-combat-encounters.md) | Combat readiness rule, Red Door encounters, and endless combat (03 §4) | ✅ |
| [10-economy-items.md](10-economy-items.md) | Currency economy model and item catalog (03 §5) | ✅ |
| [11-narrative-world.md](11-narrative-world.md) | Theme, world-building, setting, protagonist, starter pets, and narrative arc (00 Setting & Narrative) | ✅ |
| [12-references-art-direction.md](12-references-art-direction.md) | Game references, competitor analysis, and art & visual direction (00) | ✅ |
| [13-scene-breakdown.md](13-scene-breakdown.md) | Scene 1–20 breakdown from Figma plus extra GDD v2.0 scenes (01) | ✅ |
| [14-audio-fonts-list.md](14-audio-fonts-list.md) | Audio SFX/BGM list (with real files) and font checklist (05 §2–3) | ✅ |

**แหล่งข้อมูลออกแบบ:** [Figma — Bepal (White Board)](https://www.figma.com/design/akrHzNmTSxOSfPKBU0V5kk/Bepal?node-id=0-1) — 01 Game Overview · 02 Brainstorm · 03 Concept Design · 04 Early Prototype (v1) · 05 Game Loop · 06 Game Flow Scenes 1–13 · 07 Game Flow (Event) Scenes 14–20 · 08 Original Canva Board

---

## 📝 Changelog v2.1 (2026-09-27) — Sync จาก Figma

| ไฟล์ | สิ่งที่อัปเดต |
| --- | --- |
| `00-concept.md` | เพิ่ม pitch "Endless Survival Management", Game Pillars 3 ข้อ, Theme & Mood, World-Building (Sanctuary / Red Door / Economy / Antagonists), ตาราง Competitor Analysis (Kingdom, This War of Mine, Sheltered), Art Direction = Soft Hand-drawn + Uncanny, แก้กฎ Clean/Stomach, Energy เริ่ม 3 AP |
| `01-core-loop.md` | เพิ่ม Core Loop ตาม Figma, Scene Breakdown 1–20 (UI / Programmer / Art ต่อ scene) + flow diagram, สเปกการต่อสู้ Chase/Tame/Dodge/Attack, เงื่อนไข Game Over ใหม่, Controls ใช้ Perfect/Great |
| `02-scope-features.md` | Energy 3–6 AP, เพิ่มฟีเจอร์ #21–26 (Care wheel, 4 QTE variants, Red Door random event, Tame fight, Notebook unlock, Option) |
| `03-mechanics.md` | เพิ่ม §1.3 Upgrade cost table, §1.4 Notebook, §1.5 Doctor, §3.2.1 สเปก QTE 4 แบบ, ผลของ Level up (Lv.3 ปลดท่าใหม่), สเปกการต่อสู้ Figma, Starving = -20 HP/End day, Good → Great |
| `05-asset-list.md` | เพิ่ม §1.8 รายการ Art 20 รายการจาก Figma, SFX-03 → `sfx_qte_great.wav`, แจ้งเตือนสี choice ไม่ตรงกัน |

**จุดที่ Figma กับ GDD เดิมขัดกัน (เลือกตาม Figma แล้ว):** Stomach = 0 เสีย 20 HP/วัน (เดิม 25%) · Clean ต่ำไม่ได้ "ปฏิเสธการสู้" แต่ลด ATK/Max HP · Energy เริ่มต้น 3 (เดิม 6) · Event สุ่ม 3 แบบหลัง End day (เดิมกำหนดตายตัวรายวัน — ยังใช้ลำดับ Day 2/3 สำหรับ Vertical Slice) · สัตว์ตายระหว่างสู้สลับตัวได้ ตายหมด = Game Over

**ยังรอข้อมูลใน Figma:** หน้า Option, หน้า Disaster event, Scene 19–20 (Programmer/Art ยังเป็นข้อความทดสอบ), Lore & Concept for art (Starting Point / Narrative Goal / Art Keywords ยังว่าง)

---

## 🏷️ Naming Convention

**Asset:** [05-asset-list.md](05-asset-list.md) เป็นเจ้าของกฎการตั้งชื่อ asset (ตารางภาพเต็ม + ชื่อไฟล์จริง; ตาราง SFX/BGM/ฟอนต์อยู่ใน [14-audio-fonts-list.md](14-audio-fonts-list.md)) · ขั้นตอน pipeline ดู [_candidates/README.md](../../Assets/_candidates/README.md) — หัวข้อ Asset Naming Convention ด้านล่างซ้ำกัน ให้ยึด 05

| Prefix | ประเภท |
| --- | --- |
| `spr_` | Sprite / Texture |
| `sfx_` | Sound Effect |
| `bgm_` | Background Music |
| `fnt_` | Font |
| `dat_` | Data / Config |

**เอกสาร:** ไฟล์ใน `BEPAL/Docs/GDD/` เรียงลำดับด้วย prefix ตัวเลข 2 หลัก (`00-`, `01-`, ..., `06-`) สอดคล้องกับโครงสร้างมาตรฐานของโปรเจกต์อย่างเคร่งครัด

---

## Asset Naming Convention

| Prefix | ประเภท | ตัวอย่าง |
| --- | --- | --- |
| `spr_` | Sprite / Texture | `spr_coco_idle.png`, `spr_merchant_boss.png` |
| `sfx_` | Sound Effect | `sfx_qte_perfect.wav`, `sfx_acid_spit.wav` |
| `bgm_` | Background Music | `bgm_base_shelter.mp3`, `bgm_merchant_boss.mp3` |
| `fnt_` | Font | `fnt_title_horror.ttf`, `fnt_ui_cozy.ttf` |
| `dat_` | Data / Config | `dat_pet_species.json`, `dat_shop_items.json` |

---

## 📁 ใครดูแลส่วนไหน

| คนในทีม | รับผิดชอบ | โฟลเดอร์ staging / Source |
| --- | --- | --- |
| เดียร์ | 2D Art, Sprites & Environment | `BEPAL/Assets/_candidates/sprites/` |
| ภูมิ | Game Design, Mechanics, Numbers Balance & QTE Math, Narrative Scripts, UI Lead & Audio Support | `BEPAL/Assets/_candidates/fonts/`, `data/` |
| ซุง | Flex & Audio Support (SFX/BGM) | `BEPAL/Assets/_candidates/sfx/`, `music/` |
| โชว์ | Lead Gameplay Code, Architecture & Automated QA | `BEPAL/Bepal_Game/Bepal/` |

---

## Executive Summary & Comparison Matrix: GDD v1 vs GDD v2

> **หมายเหตุ (v1 history):** ตารางนี้เป็นประวัติการเปรียบเทียบ v1 → v2 — ตัวเลขเก่าในนี้ (3-Day slice, 6 AP/วัน, Perfect/Good/Miss, ร้านค้า 5 ชนิด + กระเป๋า 8 ช่อง) **ไม่ใช่ขอบเขต Prototype** ขอบเขตและตัวเลขจริงอยู่ที่ [06-vertical-slice.md](06-vertical-slice.md): Day 1–5, Energy เริ่ม 3 สูงสุด 6, ร้านค้า 3 ชนิด, ไม่มี inventory

เอกสารชุด **GDD v2.0** สังเคราะห์จากชุดข้อมูลนำเสนอ 64 สไลด์ โดยยกระดับจากเอกสารต้นแบบเดิม (GDD v1) อย่างเป็นรูปธรรม:

| มิติการออกแบบ | Legacy GDD (v1) | New GDD (v2) | ประโยชน์และผลลัพธ์ที่ยกระดับ |
| --- | --- | --- | --- |
| **ขอบเขตการเล่น (Scope)** | ลูป 5 วันแบบหลวมๆ วนรับสัตว์แปลกหน้าประตู | **3-Day High-Density Vertical Slice** ที่มีโครงเรื่องเข้มข้น มีจุดเริ่มต้น จุดวิกฤต และไคลแมกซ์ชัดเจน | Pacing กระชับ สนุก ตื่นเต้น เหมาะแก่การนำเสนอและทดสอบ Alpha/Demo |
| **ระบบสเตตัสสัตว์เลี้ยง** | มีเพียง Health 3 แต้ม และ Satisfaction Bar 3 แต้ม | **สเตตัสเสมือนจริง 4 มิติ:** Health (0–100), Stomach (0–100), Clean (0–100), และ Level/EXP | มอบความรู้สึกของ Virtual Pet สมจริง ผูกพันและต้องใส่ใจดูแลรอบด้าน |
| **ทรัพยากรและการบริหาร** | เล่นได้เรื่อยๆ ไม่จำกัดครั้งจนกว่าเลือดจะหมด | **Discrete Energy Budget (เริ่ม 3 AP/วัน อัปเกรดสูงสุด 6)** จัดสรรการกระทำอย่างมีกลยุทธ์ | เกิดการวางแผน Resource Management ที่ท้าทาย ทุกแอ็กชันมีความหมาย |
| **ระบบมินิเกม Care QTE** | หมุนเข็มสุ่มเลือก 1 ใน 4 ช่อง (Feed, Play, Pet, Observe) | **10-Attempt Session** แยกตามหมวดหมู่ (Feed, Clean, Train, Heal) พร้อมคำนวณ Perfect/Good/Miss | ควบคุมจังหวะได้แม่นยำ ท้าทายฝีมือผู้เล่น และมีระบบสะสมคะแนนสตรีค |
| **สูตรเวลา (Day Progress)** | เวลาไม่เดินหน้าตามผลลัพธ์ | **Day Progress Scaling:** สำเร็จ $+10\%$, พลาด $+15\%$ (เวลาเร่งเร็วขึ้นเมื่อพลาด) | เพิ่มความกดดันทางอารมณ์และสะท้อนความตื่นตระหนกได้อย่างแยบยล |
| **ระบบการต่อสู้ (Combat)** | มีเพียง Dodge QTE รับการโจมตีแบบตั้งรับ | **ระบบต่อสู้เต็มรูปแบบ:** มี Boss HP, Telegraphs, Dodge Zones, และ Counter-Attack | มีความตื่นเต้นแบบเกมแอ็กชัน สามารถสยบสัตว์และต่อสู้ป้องกันบ้านได้ |
| **เนื้อเรื่องและทางเลือก** | รับกล่องเปิดดูสัตว์ ไม่มีการตัดสินใจเชิงจริยธรรม | **Moral Dilemmas:** ทางเลือกขับไล่หรือสยบ Toothless, ทางเลือกขายสัตว์เลี้ยง 5,000G หรือสู้บอส | เพิ่มคุณค่าการเล่นซ้ำ (Replayability) และสร้างผลกระทบทางอารมณ์ |
| **ระบบเศรษฐกิจและไอเทม** | ไม่มีระบบเงินและร้านค้า | **ระบบเศรษฐกิจ Gold สมบูรณ์แบบ:** เงินสนับสนุน, ค่ารักษา, ร้านค้า 5 ชนิด, กระเป๋า 8 ช่อง และอุปกรณ์สวมใส่ | เพิ่มความลึกในการวางแผนการเงินและความหลากหลายของบิลด์สัตว์เลี้ยง |

---

## Guidelines for Developers & Designers

1. **สำหรับ Game Design & UI Lead (ภูมิ):**
   - ใช้ค่าตัวเลขและสูตรคำนวณใน `03-mechanics.md`, `07-pet-stats.md`, `08-care-qte.md`, `09-combat-encounters.md` และ `10-economy-items.md` เป็นฐานในการ Balance ตัวเลข
   - สามารถขยายเนื้อเรื่องไปยัง Days 4–5 ได้ในอนาคตโดยใช้สถาปัตยกรรมลูป 4 เฟสใน `01-core-loop.md`
   - กำกับทิศทาง UI (HUD, Dialogue Frames, ฟอนต์) และตรวจงาน UI ร่วมกับเดียร์
2. **สำหรับ Lead Programmers (โชว์):**
   - โค้ดอยู่ที่ `BEPAL/Bepal_Game/Bepal/` ดูโครงสร้างใน `04-class-diagram.md` และขอบเขตใน `06-vertical-slice.md`
   - ใช้ Scene Breakdown ใน `13-scene-breakdown.md` และสเปกใน `03-mechanics.md` / `07`–`10` เป็นแนวทางเขียนโค้ด
3. **สำหรับ Flex & Audio Support (ซุง):**
   - ติดตามบทสนทนาและทางเลือกเนื้อเรื่องใน `13-scene-breakdown.md`, `11-narrative-world.md` และ `09-combat-encounters.md`
   - จัดหาและตรวจสอบไฟล์เสียงตามรายการใน `14-audio-fonts-list.md`
4. **สำหรับ 2D Artists (เดียร์):**
   - ยึดสัดส่วนความละเอียดและรายการภาพตามที่ระบุใน `05-asset-list.md`
   - ออกแบบชุดสไปรต์สัตว์เลี้ยงให้มีอารมณ์ Idle, Happy, Angry, Attack/Teleport และ Hurt เพื่อรองรับ Status Effects
