# BePal

ต้นแบบเกม MonoGame (virtual-pet care + survival, "Endless Survival Management") ของทีมมหาวิทยาลัย — ดูแลสัตว์เลี้ยงในฐานกลางป่า แล้วรับมือเหตุการณ์ที่มาเคาะประตูทุกวันให้รอดถึง Day 5 repo นี้เป็น Obsidian vault ด้วย

| สมาชิก | บทบาท |
|---|---|
| โชว์ (Show) | Lead Programmer |
| ซุง (Zunk) | Flex, Audio Support |
| ภูมิ (Pooh) | Game Design, Audio Support, UI Lead |
| เดียร์ (Dear) | 2D Art |

## ดาวน์โหลดและเล่น (Windows)

1. ไปที่ [Releases](https://github.com/SShoww/CoProjectBepal/releases/latest) แล้วโหลด `BePal-v1.0.0-win-x64.zip`
2. แตก zip แล้วเปิด `Bepal.exe` — เป็น build แบบ self-contained ไม่ต้องติดตั้ง .NET

## Build / Run จาก source

ต้องมี [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (Windows; build ครั้งแรกต้องต่อเน็ตเพื่อโหลด NuGet และเครื่องมือ `mgcb`)

- **Visual Studio 2022:** เปิด `BEPAL/Bepal_Game/Bepal_Game.sln` (หรือ `Bepal_Game.slnx` สำหรับ VS 17.13 ขึ้นไป) แล้วกด Start (F5)
- **Command line:**
  ```sh
  cd BEPAL/Bepal_Game/Bepal
  dotnet build
  dotnet run
  ```
- **สร้างไฟล์ .exe สำหรับแจก:**
  ```sh
  dotnet publish BEPAL/Bepal_Game/Bepal -c Release -r win-x64 --self-contained -o publish/BePal
  ```

## วิธีเล่น

**ปุ่มควบคุม**

| ปุ่ม | การทำงาน |
|---|---|
| A / D | เดินซ้าย–ขวาในฐาน (กด Shift ค้างเพื่อวิ่ง) |
| Space | คุยหรือใช้ของที่อยู่ใกล้ (เตียง สัตว์ ประตู ฯลฯ), กดหยุดเข็มในวงล้อ QTE |
| Space / Enter | ยืนยันในหน้าบทสนทนาและเมนู |
| เมาส์ | คลิกปุ่มในเมนู เลือกตัวเลือก ซื้อหรือใช้ไอเทม (ล้อเมาส์เลื่อนรายการ) |
| Esc | ย้อนกลับ / เปิดเมนู Pause (Resume, Options, Main Menu) |
| F11 หรือ Alt+Enter | สลับเต็มจอ |

**เป้าหมาย:** เลือกสัตว์เริ่มต้น 1 ตัว (Mossling, Nibbleclaw, Blinkbun) แล้วเอาตัวรอดให้ถึงจบ Day 5

- **ดูแลสัตว์:** เดินไปหาสัตว์แล้วกด Space จะเปิดวงล้อเลือกการดูแล (Train / Feed / Clean / Heal) ครั้งละ 1 Energy จากนั้นเล่น QTE กดให้เข็มหยุดในโซน Perfect หรือ Great
- **ค่าสถานะ:** Stomach และ Clean ลดลงทุกวัน ถ้าหิวจนหมดจะเสีย HP ถ้าสกปรกพลังโจมตีและ HP สูงสุดจะลดลง
- **ฐาน:** เตียง = จบวัน, โต๊ะ Upgrade = ใช้ Point อัปเกรด, หมอ = ชุบชีวิตสัตว์, สมุด = ข้อมูลสัตว์และภัยที่เคยเจอ, กล่อง = ใช้ไอเทมที่ซื้อมา
- **เหตุการณ์ที่ประตู:** Day 2 Toothless มาเคาะ (ไล่หรือฝึกให้เชื่อง), Day 3 พ่อค้ามาขอซื้อสัตว์ (ขายหรือปฏิเสธ แล้วเปิดร้าน), Day 4 พายุ, Day 5 บอส Big Z บุก
- **การต่อสู้:** วงล้อ Dodge / Attack — ถ้าสัตว์ตายหมดจะ Game Over

## เครดิต

- ฟอนต์ Patrick Hand SC — SIL Open Font License (`BEPAL/Bepal_Game/Bepal/Content/Fonts/PatrickHandSC-OFL.txt`)
- ฟอนต์ Animal — Personal Use Only ใช้เฉพาะงานรายวิชานี้ (`Content/Fonts/Befonts-License.txt`)
- Engine: [MonoGame](https://monogame.net/) 3.8 + MonoGame.Extended

## เอกสารโปรเจกต์

- โค้ดเกม: `BEPAL/Bepal_Game/Bepal/` — คำสั่ง build/run/verify อยู่ใน [CLAUDE.md](CLAUDE.md)
- GDD: [BEPAL/Docs/GDD/README.md](BEPAL/Docs/GDD/README.md) (scope หลัก: [06-vertical-slice.md](BEPAL/Docs/GDD/06-vertical-slice.md))
- Agile: [BEPAL/Docs/Agile/](BEPAL/Docs/Agile/)
- Git workflow: [Context/Gitflow.md](Context/Gitflow.md)

## สำหรับ AI agent — เริ่ม session ตามลำดับนี้

1. ไฟล์นี้ (README.md) และกติกาใน [AGENTS.md](AGENTS.md)
2. [Context/Rules.md](Context/Rules.md) + [Context/Status.md](Context/Status.md)
3. Active Quest Board: ดูวิธีใน [Context/Agent-Handoff.md](Context/Agent-Handoff.md) — อ่าน **ชื่อ สถานะ เจ้าของ** ของทุกไฟล์ใน [Context/quests/](Context/quests/)
4. รายละเอียดเฉพาะ Quest ที่เกี่ยวข้องหรือ scope (Files) ทับกับงานที่จะทำ
5. เอกสารหรือ source เฉพาะงาน — เลือกจาก [Context/README.md](Context/README.md)

ไม่อ่านอัตโนมัติ: `Context/Edit-Log.md`, `Context/archive/`, GDD/Agile ทั้งชุด — เปิดเมื่องานต้องใช้
Board เป็นบริบท ไม่ใช่คำสั่งให้เริ่มงานเก่า
