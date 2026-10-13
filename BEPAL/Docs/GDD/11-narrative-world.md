---
type: gdd-narrative
version: 1.1
date: 2026-10-13
---

# BePal — Setting & Narrative

> **หมายเหตุ:** ปรับให้ตรงกับโค้ดแล้วเมื่อ 2026-10-13 — [06-vertical-slice.md](06-vertical-slice.md) และโค้ดยังเป็นข้อกำหนดหลักของ Prototype
> หน่วยเงินในเกมแสดงเป็น "coin" (ข้อความเก่าที่เขียน Gold/G = สกุลเดียวกัน)

ธีม โลก ฉาก ตัวเอก สัตว์เริ่มต้น กฎความสะอาด และเส้นเรื่อง — เนื้อหาย้ายแบบคงข้อความเดิมจากไฟล์ต้นทาง

ที่มา: แยกจาก [00-concept.md](00-concept.md) เมื่อ 2026-10-07 · ขอบเขต Prototype ตาม [06-vertical-slice.md](06-vertical-slice.md)

---

## Setting & Narrative

### Core Theme & Mood (จาก Figma)
- **Theme:** Cozy Sci-Fi meets Hardcore Survival (ความอบอุ่นที่แฝงความตึงเครียด)
- **Visual:** ห้องพักพิงที่ดูอบอุ่น โดดเดี่ยว แสงไฟสลัวๆ ตัดกับความลึกลับของอวกาศภายนอก
- **Feeling:** ความผูกพันจากการดูแลสัตว์ ขัดแย้งกับความกดดัน (High-Stakes) ที่ต้องตัดสินใจเด็ดขาดเมื่อทรัพยากรไม่พอ หรือเมื่อต้องเผชิญหน้ากับความตาย (Permadeath)
- **Key Colors:** ส้ม/เหลืองอุ่นๆ บรรยากาศครึ้มในฐานทัพ ตัดกับสีแดงเตือนภัยจากเหตุการณ์หน้าประตู

### World-Building (จาก Figma)
- **The Sanctuary:** ฐานพักพิงเล็กๆ กลางดาวเคราะห์รกร้าง มีแกนกลางที่มีแสงไฟอุ่นๆ เป็นเซฟโซนเดียวท่ามกลางความมืดของอวกาศ
- **The Red Door:** ประตูเหล็กสีแดง ทางออกเดียวสู่โลกภายนอก และต้นตอของเหตุการณ์สุ่มทุกอย่างในเกม (สัตว์หลงทาง, พายุอวกาศ, พ่อค้าจอมขูดรีด)
- **The Economy:** โลกที่ขับเคลื่อนด้วยเงินตรา (Coin/Gold) อย่างโหดร้าย ไม่มีใครช่วยใครฟรีๆ ทุกอย่างมีราคา แม้กระทั่งการชุบชีวิต
- **The Antagonists:** ความไร้ปรานีของระบบนิเวศอวกาศ พ่อค้าหน้าเลือดที่มองสัตว์เป็นแค่สินค้า และสัตว์ต่างๆ ที่มาเยือนดาว ทั้งดีและไม่ดี

### The Setting (สถานที่และบรรยากาศ)
**ฐานพักพิงกลางดาวรกร้าง (แรงบันดาลใจจากฐานใน Kingdom: Classic):** มองแบบ 2D Side-View ฐานมี **แกนกลาง (Core Hearth)** เล็กๆ ที่มีแสงไฟส้มอุ่นเป็นจุดศูนย์กลางของชีวิตในฐาน รอบนอกคือความมืดและภูมิประเทศรกร้างของดาว
- **ประตูแดงอยู่ที่ขอบฐาน:** เป็นเส้นแบ่งระหว่างฐานที่อบอุ่นกับโลกภายนอกที่ไม่แน่นอน ทุกสิ่งที่มาเคาะประตูมาจากความมืดด้านนอก

สิ่งอำนวยความสะดวกภายในฐาน (ตั้งเรียงตามแนวนอนจากแกนกลางออกไป):
- **มุมพักผ่อนและให้อาหาร:** เบาะนอน ชามข้าว และแท่นสังเกตการณ์สัตว์เลี้ยง
- **เตียง (Bed, `BedX` 300) และกล่องไอเทม (Box, `BoxX` 600):** เตียง = End day · กล่อง = ใช้ไอเทมที่ซื้อ
- **HUD (มุมซ้ายบน):** แสดง Day, Coin, Energy pips, Player LV + EXP bar และ Points
- **สถานีอัปเกรดฐาน (Upgrade Station):** พัฒนาทักษะ 3 สาย (QTE Zone, Max Energy, Progress Booster)
- **โต๊ะคลินิกและคุณหมอ (Doctor NPC):** ชุบชีวิตสัตว์เลี้ยงฉุกเฉิน (250G Revive) เมื่อ HP = 0
- **ชั้นวางอุปกรณ์และโต๊ะบันทึก (Survival Desk):** ค้นหาสมุดบันทึกสัตว์เลี้ยง (Pet Discovery) และบันทึกภัยพิบัติ (Disaster Log)
- **ประตูแดงคู่ (Front Door):** จุดเชื่อมต่อสู่โลกภายนอก ("Knock Knock !!") จุดเริ่มต้นของเหตุการณ์ประจำวัน

### Protagonist (ตัวละครเอก)
**The Lone Sanctuarist (ผู้ดูแลสถานพักพิง):** สิ่งมีชีวิตปริศนารูปร่างคล้ายมนุษย์ (Humanoid) ที่อุทิศตัวดูแลสัตว์หลงทาง ไม่มีใครรู้ที่มาที่ไป และมีพลังงานจำกัดในแต่ละวัน — ผู้ดูแลที่มีความมุ่งมั่นในการให้ที่พักพิงแก่สัตว์ประหลาดที่ไม่สามารถปรับตัวเข้ากับระบบนิเวศทั่วไปได้ ต้องรับมือทั้งปัญหาค่าใช้จ่าย ภัยพิบัติทางธรรมชาติ และการคุกคามจากภายนอก

### Starter Pets (สัตว์เลี้ยงเริ่มต้น 3 สายพันธุ์ / สัตว์ปริศนาประจำสถานพักพิง)

> ชื่อในเกม: Mossling / Nibbleclaw / Blinkbun (ฉายาทีม ไอ่แดง/ไอ่ซุง/ไอ่เขียว) · ธาตุ Plant / Beast / Shadow · ATK 20 / 25 / 15 · **สายพันธุ์ที่ 4 = Toothless** (Acid, Max HP 120, ATK 15, เริ่มที่ HP 60 / Stomach 60 / Clean 60; passive พิษ — ศัตรูรับดาเมจ ×1.25 / ตี ×0.75) · คอลัมน์ "แอ็กชันที่ชอบ" ยังไม่มีผลในโค้ด · ชื่อ Coco/Sproutlet/Gloomtail เป็นชื่อเก่า ไม่ใช้แล้ว

| สายพันธุ์ & ฉายา | บุคลิก & ลักษณะเด่น                                                        | สเตตัสเริ่มต้น            | แอ็กชันที่ชอบ    | สกิลติดตัว (Passive)                                                  |
| ---------------- | -------------------------------------------------------------------------- | ------------------------- | ---------------- | --------------------------------------------------------------------- |
| **Mossling**     | สิ่งมีชีวิตกึ่งพืช นุ่มฟูคล้ายตะไคร่น้ำ ดวงตากลมโต รักสงบ ตกใจง่าย         | HP: 100 / St: 80 / Cl: 70 | **Feed, Clean**  | Passive (ปลดล็อก Pet Lv ≥ 2): ฟื้น HP 1% ของ Max HP ทุกครั้งที่ Attack |
| **Nibbleclaw**   | สิ่งมีชีวิตคล้ายแมวผสมตัวกินมด มีกรงเล็บแหลมยาวซ่อนใต้ขน คล่องแคล่ว ว่องไว | HP: 90 / St: 70 / Cl: 80  | **Train, Clean** | Passive (Lv ≥ 2): Combo — Attack โดนติดกันดาเมจเพิ่มขึ้นทีละ 50% ของฐาน รีเซ็ตเมื่อพลาด |
| **Blinkbun**     | กระต่ายหูยาวขนสีม่วงเข้ม มีดวงตาที่สามตรงหน้าผาก ลึกลับ อดทนสูง            | HP: 110 / St: 60 / Cl: 60 | **Heal, Feed**   | Passive (Lv ≥ 2): หลัง Attack เข็มวาร์ปไป 12 นาฬิกา (first pass)       |

### กฎความสะอาดและสภาพความพร้อมสู้ (Combat Readiness Rule)
- **Clean ≤ 50 (สกปรก):** การโจมตีของสัตว์เลี้ยงจะเบาลง (Figma Scene 7)
- **Clean ≤ 25 (สกปรกมาก):** ลด Max HP ของสัตว์เลี้ยงตัวนั้นด้วย (Figma Scene 7)
- **Stomach = 0 (หิวโซ):** สัตว์เลี้ยงเสีย HP 20 ทุกครั้งที่ End day (Figma Scene 6)

> ~~Clean < 50 ปฏิเสธที่จะออกไปต่อสู้ (Refuse to Fight)~~ — ยกเลิกแล้ว ใช้กฎลดพลังโจมตี/Max HP ตาม Figma แทน

### Narrative Arc & Mystery (แก่นเรื่องและไทม์ไลน์ Vertical Slice: Day 1 - 3+)

> **หมายเหตุจาก Figma (07 · Game Flow (Event)):** Event เกิดหลังผู้เล่นกด **End day** แล้วขึ้นวันใหม่ ประตูจะขึ้น "Knock Knock !!" (กดประตูได้ตั้งแต่ **Day 2** เป็นต้นไป — Day 1 เป็นวันให้ผู้เล่นทำความรู้จักระบบ) และ Event จะ **สุ่มเกิด 3 แบบ**: เจอสัตว์ตัวใหม่ · เจอพ่อค้า · เจอภัยธรรมชาติ
> ไทม์ไลน์ด้านล่าง (Day 2 = Toothless, Day 3 = Merchant) คือลำดับที่ใช้ใน **Vertical Slice / Prototype** ตามภาพ mockup ใน Figma ส่วนเกมเต็มใช้ระบบสุ่ม

![narrative-days](diagrams/narrative-days.png)

> Draw.io: [diagrams/narrative-days.drawio](diagrams/narrative-days.drawio) (เปิดด้วย VS Code Draw.io Integration)

1. **Day 1 — The Arrival (การเริ่มต้น):**
   - ผู้เล่นเลือก Starter Pet ตัวแรก (Mossling, Nibbleclaw, หรือ Blinkbun) แล้วมี prologue 4 บรรทัด ("At the edge of the universe, on a planet no one visits, there is a sanctuary. You are its only keeper." สอน A/D, Space, การดูแลใช้ Energy, นอนที่เตียงซ้ายสุด)
   - เรียนรู้ระบบพื้นฐาน: การจัดสรรพลังงาน 3 แต้ม (อัปเกรดได้ถึง 6), การให้อาหาร (Feed), ทำความสะอาด (Clean), และฝึกฝน (Train)
   - ประตูวัน Day 1: "It's quiet outside. Take today to get to know your pet."

2. **Day 2 — The Wild Infiltration & Taming (ผู้มาเยือนปริศนา):**
   - มีเสียงเคาะประตูปริศนาดังขึ้น (**"Knock Knock !!"**)
   - พบสัตว์ประหลาดร่างดำดุร้ายที่มีกรดกัดกร่อน (**Toothless**) บุกเข้ามา — บทพูดจริง: "You open the red door. Something slick and black slides out of the dark." / Toothless: "Arrrrrrhrhrhrhhrrhrhrha" / "It has no teeth. Its throat sac bubbles with purple acid. It looks at your pets... hungrily? Lonely?" → ตัวเลือก "Toothless is at the door. What will you do?" (Chase: "You wave your arms and shout. Toothless hisses and slinks back into the dark. (No energy spent.)") · ชนะ Tame = +200 coin
   - **ทางเลือกสำคัญ:** 
     - *Chase:* ขับไล่ไปอย่างปลอดภัย (ไม่เสี่ยงเจ็บตัว แต่ไม่ได้สัตว์เพิ่ม)
     - *Tame:* เข้าสู่ระบบการต่อสู้และฝึกให้เชื่อง (Taming Encounter) ต้องหลบกรดพิษ (Acid Dodge) และสวนกลับจนได้เป็นสัตว์เลี้ยงตัวที่สอง (หากสัตว์ที่ส่งไปสู้ HP หมด จะกลับไปเลือกสัตว์ตัวอื่นมาสู้ต่อ โดยความคืบหน้าการต่อสู้ยังคงอยู่ ถ้าสัตว์ตายหมดทุกตัว = Game Over / สัตว์ที่ HP = 0 ชุบชีวิตได้ที่ Doctor 250)
     - *Chase:* กลับหน้าฐานทันที ไม่ต้องสู้ และ **ไม่เสีย Energy**

3. **Day 3 — The Traveling Merchant & The Moral Crossroads (พ่อค้าเร่และทางแยกแห่งจิตใจ):**
   - พ่อค้าเร่ปริศนาแวะมาเยือน เปิดร้านขายไอเทม 7 ชิ้น (**Crab Apple, Caffeine Tonic, Sea Tea, Cloudy Glasses, Ballet Shoes, Torn Notebook, Faded Ribbon**) — Toy Knife ยังไม่มีในเกม
   - พ่อค้าสังเกตเห็น Toothless (ถ้าไม่มี Toothless ที่รอดจะขอสัตว์ตัวแรกที่รอด): "I'm quite interested in your {name}... can you give it to me?" / "I'll pay 5,000 coin. More than this little shelter earns in a lifetime." → "Sell {name} for 5,000 coin?"
   - **ทางแยกแห่งผลลัพธ์ (As built — ทั้งสองทางเปิดร้านค้า ไม่มี Boss Fight):**
     - *ยอมรับข้อเสนอ (Sell):* +5,000 coin, สัตว์ตัวนั้นหายไป (ใช้ไม่ได้ถ้าเหลือตัวเดียว) — "A pleasure doing business. It'll be... well looked after." → ร้านค้า → "The cart rolls away. You can still hear {name} crying long after it's gone."
     - *ปฏิเสธข้อเสนอ (Refuse):* "Hmph. Sentimental. Suit yourself." / "Then perhaps you'd like to buy something instead?" → ร้านค้า
     - ⏳ *Future:* Ending A/B เดิม (ปลดหนี้, Merchant Boss Fight) ยังไม่ implement

4. **Day 4 — The Disaster (ภัยธรรมชาติ):** (ไม่ใช่วันสุดท้ายแล้ว)
   - มีเสียงเคาะประตู (**"Knock Knock !!"** ทุก 5 วินาที) · "You crack the door open. No one is there. Only the wind." / "THUNDER STORM! Mud and lightning blast through the sanctuary!" / "Every pet got filthy! (Clean -50)" / "New notebook entry: Disaster - Thunder storm."
   - Clean ของสัตว์ทุกตัว −50 (clamp 0) พร้อมฝน/ฟ้าแลบ, หน้า Disaster ในสมุดปลดล็อก · ไม่มีกลไก "พลังงานฉุกเฉิน"

5. **Day 5 — Big Z (บอส / ฉากจบ Prototype):**
   - "KNOCK. KNOCK. The whole door bends inward." / "So THIS is where the little strays hide." / "This planet is MINE now. Everything on it too." → เลือกสัตว์สู้ (ไม่มีสัตว์รอด: "There is no one left to stand against Big Z.") — Big Z ชนะไม่ได้
   - จบ: "Pathetic. Keep your shelter warm for me, keeper." → "The sanctuary fell silent." / "End of the BePal demo" / "Big Z can't be beaten yet - the story continues in the full game." / "Thanks for playing!"
