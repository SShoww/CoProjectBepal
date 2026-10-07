---
type: asset-list
version: 1.0
date: 2026-10-07
project: BePal
---

# BePal — Audio & Fonts List

> **หมายเหตุ:** ปรับให้ตรงกับโค้ดแล้วเมื่อ 2026-10-07 — [06-vertical-slice.md](06-vertical-slice.md) และโค้ดยังเป็นข้อกำหนดหลักของ Prototype

รายการเสียง (SFX/BGM) และฟอนต์ — เนื้อหาย้ายแบบคงข้อความเดิมจากไฟล์ต้นทาง

ที่มา: แยกจาก [05-asset-list.md](05-asset-list.md) เมื่อ 2026-10-07 · ขอบเขต Prototype ตาม [06-vertical-slice.md](06-vertical-slice.md)

---

## 🔊 2. รายการเสียงสำหรับ ซุง & ภูมิ (Audio)

### 2.1 Sound Effects (SFX — WAV 44.1kHz 16-bit PCM)

> **ชื่อไฟล์จริง:** คอลัมน์ท้ายตารางคือไฟล์ที่ใช้ใน `Content/Sfx/` (17 WAV, ชื่อตาม `Sfx` ใน `Core/Audio.cs`) — `—` = ยังไม่มีไฟล์/ไม่ได้ใช้ · `qte_perfect` กับ `typewriter` สร้างเสียงตอนรันเกม (ไม่มีไฟล์)

| รหัส Audio | ชื่อไฟล์ | บริบทการเล่น | คำอธิบายอารมณ์เสียง | Priority | ไฟล์จริงใน `Content/Sfx/` |
| --- | --- | --- | --- | --- | --- |
| **SFX-01** | `sfx/sfx_qte_perfect.wav` | กด Space โดน Perfect | ปิ๊งแก้วใสคมชัด น่าพึงพอใจ | P0 | (synth ตอนรัน: `Sfx.QtePerfect`) |
| **SFX-02** | `sfx/sfx_qte_great.wav` | กด Space โดน Great | เคาะไม้ทุ้มปานกลาง | P0 | `sfx_qte_great.wav` |
| **SFX-03** | `sfx/sfx_qte_miss.wav` | กดพลาด | Buzzer ทึบสั้นๆ | P0 | `sfx_qte_miss.wav` |
| **SFX-04** | `sfx/sfx_wheel_select.wav` | เลือก choice บนวงล้อ Care สำเร็จ (จอสั่น) | ตุ้บหนักแน่น | P1 | `sfx_qte_select.wav` |
| **SFX-05** | `sfx/sfx_footstep.wav` | ผู้เล่นเดินในฐาน (สุ่ม pitch) | ก้าวเท้าเบาๆ บนพื้นดิน/ไม้ | P1 | `sfx_world_footstep.wav` |
| **SFX-06** | `sfx/sfx_interact.wav` | กด `Space` ใช้ของในฐาน | ป๊อปเบาๆ | P1 | `sfx_world_interact.wav` |
| **SFX-07** | `sfx/sfx_ui_click.wav` | คลิกปุ่ม UI / ESC | คลิกนุ่ม | P0 | `sfx_ui_click.wav` |
| **SFX-08** | `sfx/sfx_knock.wav` | "Knock Knock !!" ที่ประตูแดง | เคาะประตูเหล็กสองครั้ง ก้องๆ | P0 | `sfx_event_door_knock.wav` |
| **SFX-09** | `sfx/sfx_door_open.wav` | เปิดประตูแดง | ประตูเหล็กหนักเลื่อนเปิด | P1 | `sfx_event_door_open.wav` |
| **SFX-10** | `sfx/sfx_sleep.wav` | End day ที่เตียง (เข้ากลางคืน) | ระฆังลมเบาๆ / เสียงผ้าห่ม | P1 | `sfx_event_end_day.wav` |
| **SFX-11** | `sfx/sfx_dodge_success.wav` | Dodge สำเร็จ | ลมวูบ (Whoosh) / Metallic Parry | P0 | `sfx_fight_dodge.wav` |
| **SFX-12** | `sfx/sfx_attack_hit.wav` | Attack โดนศัตรู | กระแทกหนักแน่น | P0 | `sfx_fight_player_attack.wav` |
| **SFX-13** | `sfx/sfx_pet_hurt.wav` | สัตว์เราโดนโจมตี | ร้องเจ็บสั้นๆ แปลกๆ (uncanny) | P0 | `sfx_fight_pet_hurt.wav` |
| **SFX-14** | `sfx/sfx_acid_spit.wav` | Toothless ใช้ท่า Acid | ของเหลวเดือดสาดกระเซ็น (Sizzle Splat) | P1 | — |
| **SFX-15** | `sfx/sfx_bigz_roar.wav` | Big Z ปรากฏ / โจมตี | คำรามต่ำ ทุ้มหนัก | P1 | `sfx_fight_boss_roar.wav` |
| **SFX-16** | `sfx/sfx_coin.wav` | ได้/จ่าย coin (รายได้เช้า, ร้าน, หมอ, ขายสัตว์) | เหรียญกระทบกัน | P0 | `sfx_ui_coin.wav` |
| **SFX-17** | `sfx/sfx_level_up.wav` | Player LV up (+1 Point) | ไล่โน้ตขึ้นสดใส | P1 | `sfx_ui_levelup.wav` |
| **SFX-18** | `sfx/sfx_new_pet.wav` | "You got new pet !!!" | Jingle สั้นเฉลิมฉลอง | P1 | `sfx_event_new_pet.wav` |
| **SFX-19** | `sfx/sfx_thunder.wav` | หน้าข้อความพายุ Day 4 | ฟ้าผ่าเปรี้ยง + ฝน | P1 | `sfx_event_storm.wav` |
| **SFX-20** | `sfx/sfx_eat.wav`, `sfx_bubble.wav` | QTE Feed (เคี้ยวกรุบกรอบ) / QTE Clean (หยดน้ำ/สบู่) | ตาม 00-concept | Later | — |
| **SFX-21** | `sfx/sfx_typewriter_key.wav` | ตัวอักษรขึ้นใน dialogue | แป้นพิมพ์ดีดนุ่มๆ | Later | (synth ตอนรัน: `Sfx.Typewriter`) |
| **SFX-22** | `sfx/sfx_coin_gatling.wav`, `sfx_cane_strike.wav` | Merchant Boss Fight | — | Later | — |

### 2.2 Background Music (BGM — OGG Looping)

| รหัส Audio | ชื่อไฟล์ | บริบทของฉาก | สไตล์และอารมณ์ดนตรี | Priority |
| --- | --- | --- | --- | --- |
| **BGM-01** | `music/bgm_menu.ogg` | Main Menu / เลือกสัตว์ | เงียบ ลึกลับ อบอุ่น | P1 |
| **BGM-02** | `music/bgm_base_cozy.ogg` | ฐาน (Day 1–5) + Care/QTE | อะคูสติกกีตาร์ + เปียโนไฟฟ้า Lo-Fi สบายๆ แต่มีโน้ตแปลกแทรก | P0 |
| **BGM-03** | `music/bgm_fight.ogg` | Fight กับ Toothless (Day 2) | ตึงเครียด จังหวะเร็ว | P0 |
| **BGM-04** | `music/bgm_merchant.ogg` | พ่อค้า + ร้านค้า (Day 3) | แจ๊ซหม่นๆ เจ้าเล่ห์ | P1 |
| **BGM-05** | `music/bgm_thunderstorm.ogg` | หน้าพายุ (Day 4) | ambient ฝนตก ฟ้าร้อง | P1 |
| **BGM-06** | `music/bgm_boss_bigz.ogg` | Fight กับ Big Z (Day 5) | หนักหน่วง สิ้นหวัง | P1 |
| **BGM-07** | `music/bgm_to_be_continued.ogg` | หน้า "To be continued..." | เปียโนเดี่ยว ค้างคา | P1 |
| **BGM-08** | `music/bgm_ending_bittersweet.ogg`, `bgm_ending_heroic.ogg` | ฉากจบ A/B ของ Merchant arc (เกมเต็ม) | — | Later |

> MonoGame เล่น `Song` จาก `.ogg` / `.mp3` ได้ แต่แนะนำ `.ogg` สำหรับ DesktopGL

---

## 🔤 3. ฟอนต์สำหรับ ภูมิ

ข้อความในเกมเป็นภาษาอังกฤษทั้งหมด — ไม่ต้องใช้ฟอนต์ไทย · ต้องเป็นฟอนต์ที่ **อนุญาตให้ใช้ในเกมได้** (เช่น Google Fonts / OFL)

| รหัส Font | ไฟล์ | บริบทการใช้งาน | สไตล์ฟอนต์ | ปัจจุบันในโค้ด | Priority |
| --- | --- | --- | --- | --- | --- |
| **FNT-01** | `Fonts/fnt_title_display.ttf` → `Big.spritefont` (44 pt) | โลโก้, หัวข้อหน้าจอ, "Knock Knock !!", "To be continued...", Perfect/Great/Miss | ตัวหนา กึ่งลึกลับ มีเอกลักษณ์ | `Segoe UI` 44 | P1 |
| **FNT-02** | `Fonts/fnt_ui_cozy.ttf` → `Main.spritefont` (18 pt) + `Small.spritefont` (14 pt) | Dialogue, ปุ่ม, HUD, คำอธิบายไอเทม | Sans-serif มน อบอุ่น อ่านง่าย | `Segoe UI` 18 / 14 | P1 |
