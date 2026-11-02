# วิธีทำให้หน้าตาเกม BePal ดีขึ้น (ตอนนี้ควรลงมือที่อะไรก่อน)

> **TL;DR**
> - จุดที่ทำให้เกมดู "ไม่ลงตัว" ที่สุดตอนนี้คือ **สไตล์ไม่ตรงกัน**: ฉากหลัง/ผู้เล่นเป็น pixel art คม แต่สัตว์/ศัตรู/หมอ/พ่อค้า/เฟอร์นิเจอร์ในฐานเป็นวงรีขอบนุ่มที่วาดจาก primitive (เห็นชัดใน `03_base`, `08_fight`, `09_fight_bigz`, `11_doctor`) — ทางแก้จริงคือ **sprite pixel art ที่ตรงสไตล์** ซึ่งโค้ดรองรับแล้ว (`World/Art.cs` บรรทัด 25-38) ขาดแค่ไฟล์ภาพ และต้อง **ตัดสินใจสไตล์ให้ตรงกันก่อน** เพราะเอกสาร asset ยังระบุ 280x360 แบบ hand-drawn
> - **Quick win ที่เห็นผลทันทีโดยไม่ต้องมี asset:** (1) เลิกใช้ Segoe UI ที่ตัวหนังสือออกมาเอียงทั้งเกม (ดูหัวข้อฟอนต์: ตัวประมวลผลฟอนต์บน Windows ไม่ตรวจ style จึงอาจหยิบหน้าฟอนต์ Italic) แล้วใส่ไฟล์ .ttf ที่มี license ชัดไว้ข้าง `.spritefont` (2) particle เล็กๆ เขียนเอง ~120 บรรทัด (3) vignette + color grading แบบ SpriteBatch (ไม่ต้องใช้ shader) (4) ปัดค่า shake เป็นจำนวนเต็มเพื่อไม่ให้ตัวอักษร/pixel art เบลอตอนจอสั่น
> - **ไม่แนะนำ** ให้ย้ายทั้งเกมไปวาดที่ 320x180 แล้วขยาย: UI/ฟอนต์ทั้งหมดอยู่ที่พิกัด 1280x720 และ SpriteFont 18/14 pt จะใช้ไม่ได้ที่ความละเอียดต่ำ — ใช้ `RenderTarget2D` เฉพาะเมื่อจะทำ post-process (grading/bloom/transition) เท่านั้น
> - Shader (`.fx` ผ่าน `mgfxc`) ทำได้บน DesktopGL แต่จำกัดที่ `vs_3_0/ps_3_0` (MojoShader → GLSL) และ **ยังไม่ได้ทดสอบว่า build chain ของทีม (mgcb 3.8.4 + Content.Builder.Task 3.8.6) คอมไพล์ .fx ผ่านไหม** — เป็นงานระดับ "medium" ที่ควรทำหลัง quick win
> - ทุกข้อสรุปเชิงตัวเลข (จำนวนบรรทัด/วัน) เป็น **ประมาณการของผู้วิจัย** ไม่ใช่ข้อเท็จจริงจากแหล่งอ้างอิง

วันที่: 2026-10-14 · ผู้ขอ: ทีม BePal · ขอบเขต: อ่านโค้ดอย่างเดียว (รัน `dotnet run -- --shots` ไปยัง temp นอกรีโป ไม่ได้แก้โค้ด/csproj/ไม่ได้ commit) · ต่อจาก [monogame-extended-visuals.md](monogame-extended-visuals.md) (ไม่ทำซ้ำเรื่อง MonoGame.Extended)

วิธีอ่านเอกสารนี้: `[n]` = ข้อเท็จจริงจากแหล่งอ้างอิงในตารางด้านล่าง · **"สังเกต"** = สิ่งที่เห็นในโค้ด (ระบุ file:line) หรือภาพ shots (ระบุชื่อ PNG) · **"ความเห็น"** = ดุลยพินิจของผู้วิจัย

---

## Sources

| # | URL | คืออะไร | ระดับ |
|---|-----|---------|-------|
| [1] | https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.SpriteBatch.html | API `SpriteBatch`: `Begin(...)` ค่า default (blend = AlphaBlend, sampler = LinearClamp, transformMatrix = Identity), overload ของ `Draw`/`DrawString` | Primary |
| [2] | https://docs.monogame.net/articles/tutorials/building_2d_games/18_texture_sampling/index.html | บทเรียนทางการเรื่อง Texture sampling: Point vs Linear, `PointClamp` สำหรับ pixel art, ต้อง `End`/`Begin` ใหม่เมื่อเปลี่ยน sampler | Primary |
| [3] | https://raw.githubusercontent.com/MonoGame/MonoGame/develop/MonoGame.Framework/Graphics/States/BlendState.cs | ซอร์ส `BlendState`: ค่า factor ของ `Additive` / `AlphaBlend` / `NonPremultiplied` / `Opaque` (branch `develop`) | Primary |
| [4] | https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.BlendState.html | API `BlendState` (static fields + properties เช่น `ColorSourceBlend`, `ColorDestinationBlend`, `ColorBlendFunction`) | Primary |
| [5] | https://docs.monogame.net/articles/getting_to_know/howto/graphics/HowTo_Create_a_RenderTarget.html และ https://docs.monogame.net/articles/getting_to_know/whatis/graphics/WhatIs_Render_Target.html | วิธีสร้าง/ใช้ `RenderTarget2D` (`SetRenderTarget`, ต้อง reset เป็น `null`, เป็น `Texture2D` ย่อย) — ได้ข้อความจากผลค้นหาของเว็บ docs (ยังไม่ได้เปิดหน้า WhatIs โดยตรง เพราะ URL ที่เดาครั้งแรกเป็น 404) | Primary (ผ่านผลค้นหา) |
| [6] | https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.RenderTarget2D.html | constructor ของ `RenderTarget2D` (มี overload รับ `SurfaceFormat`, `DepthFormat`, `RenderTargetUsage`) — หน้านี้ไม่อธิบายสมาชิกของ `RenderTargetUsage` | Primary |
| [7] | https://docs.monogame.net/articles/getting_to_know/whatis/graphics/WhatIs_BackBuffer.html | Back buffer: บน Windows ขนาดตาม `ClientBounds` โดย default; เป็น render target; ไม่กล่าวถึง `PreferredBackBuffer*`/resize | Primary |
| [8] | https://docs.monogame.net/articles/getting_started/content_pipeline/custom_effects.html | เขียน custom effect (.fx): OpenGL แปลผ่าน MojoShader รองรับ `vs_2_0/ps_2_0` และ `vs_3_0/ps_3_0`; macro `#if OPENGL`; ข้อควรระวัง (ค่า default ของ parameter ใช้ไม่ได้บน GL, parameter ที่ไม่ใช้ถูกตัด, ไม่รองรับ preshader) | Primary |
| [9] | https://docs.monogame.net/articles/getting_started/tools/mgfxc.html | `mgfxc` คืออะไร, `/Profile:` = DirectX_11 / OpenGL ฯลฯ, ติดตั้งผ่าน `dotnet tool install -g dotnet-mgfxc` — หน้านี้ไม่ระบุว่า profile OpenGL คอมไพล์บน OS ไหนได้บ้าง | Primary |
| [10] | https://docs.monogame.net/articles/tutorials/advanced/2d_shaders/05_transition_effect/index.html | ซีรีส์ 2D Shaders ทางการ บทที่ 5: เพิ่ม effect ผ่าน MGCB Editor ("Sprite Effect"), โครง HLSL พร้อม `#if OPENGL`, ตั้งค่า parameter จาก C#, ส่ง `effect:` เข้า `SpriteBatch.Begin` | Primary |
| [11] | https://docs.monogame.net/articles/tutorials/advanced/2d_shaders/08_light_effect/index.html | ซีรีส์ 2D Shaders บทที่ 8: 2D lighting แบบ deferred (color buffer + normal buffer + light buffer, `BlendState.Additive`, composite) | Primary |
| [12] | https://docs.monogame.net/articles/tutorials/building_2d_games/16_working_with_spritefonts/index.html | `.spritefont`: `FontName`/`Size`/`Style`/`CharacterRegions`; อบเป็น atlas ตอน build; ชื่อฟอนต์ระบบใช้ได้เฉพาะเครื่องที่ build ติดตั้งฟอนต์นั้น — ให้ copy .ttf/.otf ไว้ข้างไฟล์; ให้ตรวจ license (เช่น SIL OFL) | Primary |
| [13] | https://raw.githubusercontent.com/MonoGame/MonoGame/develop/MonoGame.Framework.Content.Pipeline/Processors/FontDescriptionProcessor.cs | ซอร์ส `FontDescriptionProcessor` (`PremultiplyAlpha` default true; บน Windows หาฟอนต์จาก registry ด้วย prefix ชื่อ และ **การเช็ก style ถูกปิดไว้**) (branch `develop` — อาจต่างจาก 3.8.4/3.8.6 เล็กน้อย) | Primary |
| [14] | https://docs.monogame.net/api/Microsoft.Xna.Framework.Content.Pipeline.Processors.TextureProcessor.html | API `TextureProcessor` มี flag `PremultiplyAlpha` | Primary |
| [15] | https://learn.microsoft.com/en-us/windows/win32/direct3dhlsl/dx-graphics-hlsl-smoothstep | HLSL `smoothstep(min,max,x)` (SM2+) | Primary |
| [16] | https://learn.microsoft.com/en-us/windows/win32/direct3dhlsl/dx-graphics-hlsl-tex2d | HLSL `tex2D(s,t)` (pixel shader, SM1-3; SM4 ต้องใช้ legacy compile option) | Primary |
| [17] | https://learn.microsoft.com/en-us/windows/win32/direct3dhlsl/dx-graphics-hlsl-lerp | HLSL `lerp(x,y,s)` (SM2+) | Primary |
| [18] | https://www.aseprite.org/docs/cli/ | Aseprite CLI: `--sheet`, `--data`, `--format json-array`, `--sheet-type`, `--sheet-pack`, `--list-tags`, `--trim`, `--scale` | Primary |
| [19] | https://www.aseprite.org/faq/ | Aseprite: ใช้ EULA (ไม่ใช่ GPL แล้ว), ซื้อได้หลายช่องทาง, ไม่พบข้อความเรื่อง free trial, มี license พิเศษสำหรับสถาบันการศึกษา | Primary |
| [20] | https://www.nuget.org/packages/MonoGame.Aseprite | MonoGame.Aseprite 6.4.1 (2026-10-08), MIT, net8.0, dependency = `AsepriteDotNet >= 1.9.1`, ระบุว่า "Updated for MonoGame 3.8.4" | Primary |
| [21] | https://github.com/AristurtleDev/monogame-aseprite | repo ของ [20]: MIT, อ่านไฟล์ `.aseprite` ตรง; หน้า repo ไม่ระบุเวอร์ชัน MonoGame ที่รองรับ/ไม่ระบุ DesktopGL | Primary |
| [22] | https://www.nuget.org/packages/FontStashSharp.MonoGame และ https://github.com/FontStashSharp/FontStashSharp | FontStashSharp.MonoGame 1.6.1 (2026-09-16), Zlib, net8.0/netstandard2.0, โหลด TTF/BMFont ตอน runtime, มี stroked/blurry text; ไม่ระบุเวอร์ชัน MonoGame ที่ต้องใช้ | Primary |
| [23] | https://kenney.nl/support | Kenney: asset เป็น CC0, ใช้เชิงพาณิชย์ได้, ไม่บังคับให้เครดิต | Primary |
| [R] | ไฟล์ในรีโป: `CLAUDE.md`, `Game1.cs`, `Core/Gfx.cs`, `Core/Ui.cs`, `Core/SceneManager.cs`, `World/Art.cs`, `World/Backdrop.cs`, `World/Rain.cs`, `World/SpriteMotion.cs`, `Scenes/BaseScene.cs`, `Scenes/Wheel.cs`, `DevTools/ShotRunner.cs`, `Content/Fonts/*.spritefont`, `Content/Content.mgcb`, `Bepal.csproj`, `.config/dotnet-tools.json`, `Docs/GDD/05-asset-list.md`, `12-references-art-direction.md`, `14-audio-fonts-list.md`, `Assets/_candidates/README.md` และภาพ shots 29 รูป (ดูหัวข้อถัดไป) | สภาพปัจจุบันของเกม | Primary (ของเราเอง) |

ข้อจำกัดของการเข้าถึง: เครื่องมือดึงหน้าเว็บสรุปผ่านโมเดลขนาดเล็ก ไม่ใช่การอ่านทุกบรรทัด; หน้า docs ของ MonoGame ที่ผมลอง 2 URL (SamplerState และ RenderTarget ใต้ `getting_to_know/whatis/graphics/`) ให้ 404 จึงใช้บทเรียน [2] และผลค้นหา [5] แทน

---

## สภาพหน้าตาปัจจุบัน (audit)

วิธีตรวจ: อ่านโค้ด + `dotnet run -- --shots <temp>` สำเร็จ (exit 0) ได้ PNG 29 รูป (เปิดดู 11 รูป: `01_menu`, `03_base`, `05_base_bed_night`, `07_qte_Train`, `08_fight`, `09_fight_bigz`, `11_doctor`, `12_notebook`, `13_shop`, `14_dialogue`, `16_tbc`, `17_rain_day4`); `git status` หลังรันไม่เปลี่ยน

### จุดที่ทำได้ดีอยู่แล้ว (สังเกต)
- ฉากหลัง parallax 5 ชั้น pixel art + ฉากหน้าใบไม้ ให้ความลึกดี (`03_base`, `17_rain_day4`), วาดด้วย `PointClamp` แยก batch (`World/Backdrop.cs:55,72`) — ตรงกับคำแนะนำของ docs [2]
- ผู้เล่นเป็น pixel sprite จริง วาดด้วย `PointClamp` ที่สเกล 3 เท่าพอดี (`Art.cs:163-164`: src สูง 33 px → วาดสูง 99 px)
- มีแสงแบบราคาถูกอยู่แล้ว: แสงเตา additive (`BaseScene.cs:337-338`), overlay กลางคืน (`BaseScene.cs:377`), ฝน + ฟ้าแลบ additive (`Rain.cs:64-88`) — ภาพ `05_base_bed_night` และ `17_rain_day4` อ่านบรรยากาศออกชัด
- มี juice พื้นฐาน: screen shake (`Gfx.cs:56-71`), ข้อความ Perfect/Great ลอย (`Wheel.cs:105-139`), ปุ่ม hover ยก/เรืองเล็กน้อย (`Ui.cs:53-63`), ผู้เล่น squash/stretch (`SpriteMotion.cs`)

### จุดอ่อน (แต่ละข้อมีหลักฐาน)

| # | จุดอ่อน | หลักฐาน |
|---|---|---|
| W1 | **สไตล์ไม่ตรงกัน:** สัตว์/ศัตรู/หมอ/พ่อค้า/เตียง/กล่อง/สถานี Upgrade เป็นวงรี–สี่เหลี่ยมเรียบขอบเนียน ขณะที่ฉากหลังและผู้เล่นเป็น pixel คม หมอเป็น "แคปซูลขาว+กากบาท" | `Art.cs:85-158, 198-222`; `03_base`, `08_fight`, `09_fight_bigz` (Big Z = วงกลมแดงตาโต), `11_doctor`, `13_shop` (พ่อค้าเป็นเงาดำ) |
| W2 | **มาตราส่วน pixel ไม่เท่ากัน:** ฉากหลัง 4 เท่า, ฉากหน้า 2 เท่า, ผู้เล่น 3 เท่า ในเฟรมเดียวกัน | `Backdrop.cs:14,27`, `Art.cs:163-164` — ความเห็น: อาจจงใจ แต่ความละเอียดพิกเซลของผู้เล่นละเอียดกว่าฉากหลังเห็นได้ใน `03_base` |
| W3 | **ฟอนต์เอียงทั้งเกมทั้งที่ตั้งไว้ `Bold`/`Regular`** (เมนู ปุ่ม HUD แม้ `Small` = Regular) และช่องไฟบางจุดแปลก เช่น "Health 120 /120" | `Content/Fonts/*.spritefont` (`<Style>` = Bold/Bold/Regular, ฟอนต์ `Segoe UI`); ภาพ `01_menu`, `08_fight`, `16_tbc`; ที่มาที่เป็นไปได้ดูหัวข้อ (e) |
| W4 | **ขอบข้อความ/shake:** ข้อความเป็น `SpriteFont` ที่วาดด้วย `LinearClamp` และมีเงา offset 2 px; `Gfx.Text` ปัดพิกัดเป็นจำนวนเต็มแต่ shake matrix บวก offset ทศนิยมทับอีกที | `Gfx.cs:143-149` (`MathF.Round`), `Gfx.cs:68,78` (offset ทศนิยมเข้า matrix) — ความเห็น: ตอนจอสั่นตัวอักษรน่าจะเบลอ/สั่นระดับ sub-pixel (ไม่ได้บันทึกวิดีโอยืนยัน) |
| W5 | **UI เป็นกล่อง 2 สี + ขอบ 2 px ทุกหน้า**: ไม่มีมุม/เงา/ลายไม้ตามที่ GDD ระบุ ("กรอบไม้", 9-slice) ส่วนสมุดเป็นสี่เหลี่ยมครีม | `Ui.cs:123-127`; `12_notebook`, `13_shop`, `14_dialogue` (กล่องบทสนทนาใหญ่ว่างครึ่งล่าง); `05-asset-list.md` UI-05/HUD-08 = Not Started |
| W6 | **ไม่มี particle ทั่วไป:** มีแค่ข้อความลอย + glow; ไม่มีประกาย/ฝุ่น/เหรียญ/ควัน | ค้นโค้ด: ไม่มีคลาส particle; `Wheel.cs:105-139` เป็นข้อความอย่างเดียว |
| W7 | **ไม่มี post-process/ภาพรวมโทนสี:** ไม่มี `Effect`/`RenderTarget2D` ในเกมจริง (มีเฉพาะใน `ShotRunner.cs:38` เพื่อบันทึกภาพ); หญ้าสีเขียวนีออนในฉากหลังเด่นกว่าโทน "อบอุ่น ครีม–น้ำตาล–ส้มโคม" ที่ GDD 12 ระบุ | `Game1.cs:150-156` (`Draw` วาดตรงลง back buffer); `03_base`; `12-references-art-direction.md` หัวข้อ Color Palette — "โทนไม่ตรง" เป็นความเห็น |
| W8 | **ตัวละครแบบ sprite จะเสียจุดขาย "ตาจ้องตามผู้เล่น":** เส้นทาง sprite คืนค่าก่อนวาดตา | `Art.cs:83` (`return` ก่อน `Eye(...)`), เทียบ `05-asset-list.md` §1.2 ที่บอกว่า "ถ้าวาดตาแยก layer โค้ดจะขยับตาให้" แต่โค้ดยังไม่ทำ |
| W9 | **ข้อความล้นกรอบในร้านค้า:** คำอธิบาย "Ballet Shoes" ยาวชนปุ่ม Buy (บรรทัด "tonight" ถูกปุ่มทับ) | `13_shop` — บั๊กเลย์เอาต์ ไม่ใช่เรื่องสไตล์ แต่ทำให้ดูไม่เรียบร้อย |
| W10 | **หน้าจอ fixed 1280x720 ไม่มี letterbox/scale** | `Game1.cs:25-31` (`PreferredBackBufferWidth/Height`), `Gfx.cs:11-12` |

---

## (a) ความสอดคล้องของ art direction: แทน primitive ด้วย sprite

**ข้อเท็จจริงจากแหล่ง**
- `SpriteBatch.Draw` มี overload รับ `sourceRectangle`, `origin`, `scale`, `SpriteEffects` ครบ จึงเลือกวาดเฟรมจาก sheet ได้โดยไม่ต้องมีไลบรารีเสริม [1]
- docs ทางการแนะนำ `PointClamp` สำหรับ pixel art; Point = เลือกพิกเซลที่ใกล้ที่สุด ภาพขยายจึงคม [2]
- Aseprite ส่งออก sheet + JSON ได้จาก CLI (`--sheet`, `--data`, `--format json-array`, `--list-tags` จะใส่ tag ลง `meta.frameTags`) [18]

**สังเกต (ในรีโป)**
- สายท่อ sprite พร้อมแล้ว: `Art.LoadOptional` โหลด `pet_<species>`, `enemy_toothless`, `enemy_bigz`, `npc_merchant`, `npc_doctor` ถ้ามีไฟล์ และใช้ `OpaqueBounds` หาขอบเท้าอัตโนมัติ (`Art.cs:25-54`); ความสูงตอนวาดกำหนดเป็นค่าคงที่ (`Art.cs:23`: pet 90, doctor 125, merchant 220, toothless 150, Big Z 230); ภาพนิ่งเดียวต่อตัว ขยับด้วย idle breath/bob จาก `SpriteMotion`/`Pose` (`Art.cs:58-66`)
- ทุกการวาด sprite ผ่าน `SpriteMotion.Draw` ที่ `PointClamp` (`SpriteMotion.cs:68-78`)
- **ความขัดแย้งในเอกสาร:** `05-asset-list.md` §1.2 ระบุ sprite สัตว์ **280x360 px**, สไตล์ "Soft Hand-drawn + Uncanny" และ `12-references-art-direction.md` ระบุว่า "As built" = Soft Pixel Art (`PointClamp`) ส่วน `_candidates/README.md` บอกว่ารับ PNG "ขนาดอะไรก็ได้"

**ความเห็น / ผลที่ตามมา**
- ถ้าส่งภาพวาดมือ 280x360 เข้า pipeline ปัจจุบัน มันจะถูกย่อเหลือสูง 90 px ด้วย `PointClamp` (เลือกพิกเซลใกล้สุดตาม [2]) — คาดว่าได้ขอบหยักและรายละเอียดหาย (**คาดการณ์** ยังไม่ได้ทดลองด้วยภาพจริง) ทางออกมีสองทาง: (1) วาดสัตว์เป็น pixel art ความละเอียดต่ำที่สเกลเป็นจำนวนเต็ม (เช่น 3 หรือ 4 เท่า ให้เท่าผู้เล่น/ฉากหลัง) หรือ (2) ถ้าจะใช้ภาพละเอียดจริงๆ ต้องวาดผ่าน `LinearClamp` และยอมรับว่าสไตล์ผสม → แนะนำทาง (1) เพราะตรงกับ "as built"
- ภาพ sprite เดียวต่อตัวไม่มีตาแยก → เสีย cue "ตามองเรา" (W8) ทางแก้ถูกที่สุด: ขอให้ศิลปินส่ง **ภาพลำตัวไม่มีตา + ภาพตา (ขาว/รูม่านตา) แยก** แล้วให้ `Art.TryDrawSprite` วาดตาทับด้วย `Eye(...)` ที่มีอยู่ (`Art.cs:11-15`) — ประมาณ 30-40 บรรทัด
- การทำ animation: ของที่มีอยู่ (`SpriteMotion` สำหรับภาพนิ่ง) เพียงพอสำหรับ idle ของสัตว์ ถ้าอยากมีท่าเดิน/โจมตี/เจ็บ (PET-02..04 ฯลฯ ใน `05-asset-list.md`) ให้เพิ่ม sprite sheet แนวนอนเฟรมขนาดเท่ากัน + ตัวเล่นเฟรมเอง (ประมาณ 40-60 บรรทัด, ใช้ `Draw` + `sourceRectangle` ตาม [1]) — ไม่จำเป็นต้องมีไลบรารี
- **ไลบรารีอ่าน Aseprite:** `MonoGame.Aseprite` 6.4.1 (MIT, net8.0, อัปเดต 2026-10-08) อ่านไฟล์ `.aseprite` ตรงได้ [20][21] แต่หน้า NuGet ระบุเพียง "Updated for MonoGame 3.8.4" และไม่มีหน้าใดยืนยัน DesktopGL หรือ 3.8.6 → **ยังไม่ยืนยันความเข้ากัน, ไม่แนะนำให้เพิ่ม dependency ตอนนี้**; ใช้ CLI ส่งออก PNG + JSON [18] แล้วอ่านเอง ปลอดภัยกว่า
- **plug-in:** `World/Art.cs` (ขอบเขตเดิม) + ค่าคงที่ความสูง + `Content.mgcb` (บล็อก `TextureImporter`/`TextureProcessor`/`PremultiplyAlpha=True` เหมือน `player.png`); ความเสี่ยงต่อ AutoPlay/ShotRunner/SceneCatalog: **ต่ำ** (ไม่เปลี่ยนชนิด scene หรือ member ที่ DevTools อ้าง); ตัว `--shots` ช่วยตรวจ

---

## (b) Pixel-perfect pipeline: วาดลง RenderTarget ความละเอียดต่ำแล้วขยาย

**ข้อเท็จจริงจากแหล่ง**
- ใช้ `GraphicsDevice.SetRenderTarget(rt)` แล้ววาดตามปกติ จากนั้นต้อง `SetRenderTarget(null)` เพื่อกลับไปวาดลงจอ; `RenderTarget2D` เป็น `Texture2D` จึงส่งให้ `Draw` ได้เลย; render target ต้องสร้างใหม่เมื่อ device reset [5]
- `RenderTarget2D` มี constructor ที่เลือก `SurfaceFormat`/`DepthFormat`/`RenderTargetUsage` ได้ [6]
- back buffer บน Windows ตามขนาด `ClientBounds` โดย default และเป็น render target ตัวหนึ่ง [7]
- `SpriteBatch.Begin` default sampler = `LinearClamp` [1]; การเปลี่ยน sampler ต้อง `End`/`Begin` ใหม่ [2]

**สังเกต:** เกมวาดตรงลง back buffer 1280x720 (`Game1.cs:150-156`); ฉากหลังและ sprite ใช้ `PointClamp` แยก batch ส่วนที่เหลือ (รวมตัวอักษร) ใช้ `LinearClamp` global (`Gfx.cs:77`); `ShotRunner.cs:38-47` มี render-target 1280x720 อยู่แล้วเพื่อบันทึก PNG

**ความเห็น (trade-off)**
| ทางเลือก | ได้ | เสีย |
|---|---|---|
| คงเดิม (วาด 1280x720 ตรง, `PointClamp` เฉพาะ sprite) | ไม่เสี่ยงอะไร; ตัวอักษรคม | pixel หลายสเกลปนกัน (W2) |
| ทั้งเกมที่ 320x180 แล้ว scale 4 เท่า | pixel เท่ากันทุกชิ้นโดยอัตโนมัติ; shake เป็นก้อนพิกเซล | **UI/ฟอนต์ทั้งหมดอยู่พิกัด 1280x720 และ SpriteFont 14-44 pt อ่านไม่ออกที่ 320x180**; ต้องเขียน UI ใหม่ทั้งชุด → **ไม่แนะนำ** |
| วาดเฉพาะ "โลก" (backdrop+ตัวละคร) ลง RT ความละเอียดต่ำ (เช่น 640x360) แล้วขยาย 2 เท่า ส่วน UI/ตัวอักษรวาดทับที่ 1280x720 | pixel โลกเท่ากัน + มี RT สำหรับ post-process ด้วย | ต้องแตก batch ใน `SceneManager`/`Scene.Draw`; ตำแหน่งตัวละคร/ข้อความที่ผูกกับโลก (ชื่อสัตว์ หัวบาร์) ต้องแปลงพิกัด; **ปานกลาง-เสี่ยง** |
| วาดทั้งเกมที่ 1280x720 ลง RT ปกติ แล้ววาด RT ลง back buffer ด้วย effect (grading/vignette) | ได้จุดเสียบ shader จุดเดียว (ดูข้อ c); เปิดทางสู่ letterbox/fullscreen | ต้อง `SetRenderTarget` ใน `Game1.Draw` และเลิก `Clear` ตรงๆ; ต้องดูแล `ShotRunner` (ซ้อน RT) |

- เรื่อง shake: offset ทศนิยมใน matrix ทำให้ตัวอักษรไม่ตรงกริดพิกเซล (W4) — **quick win:** ปัด `_shakeOffset` เป็นจำนวนเต็มใน `Gfx.UpdateShake` (บรรทัด 68) 2 บรรทัด; ถ้าไปใช้ RT ความละเอียดต่ำ shake ต้องคิดเป็นหน่วยพิกเซลของ RT
- **plug-in:** แนะนำอย่างน้อยทางสุดท้าย (RT เต็มจอ → back buffer) เมื่อจะทำ post-process: แก้ `Game1.Draw` (~15 บรรทัด) + helper ใน `Gfx` (~25 บรรทัด); ความเสี่ยงต่อ DevTools: `AutoPlay` ไม่วาดอะไรอยู่แล้ว (ข้าม), `ShotRunner` ใช้ `SceneManager.Draw` ตรง (ไม่ผ่าน `Game1.Draw`) จึงไม่ได้ภาพที่ผ่าน effect → ต้องเพิ่มทางผ่านเดียวกัน ไม่งั้น shots ไม่ตรงกับของจริง; ถ้าจะใช้ letterbox ต้องแปลง `Input.Mouse` (คลิกปุ่ม) ด้วย — **ผูกกับ `Core/Input.cs`** (ยังไม่ได้อ่านโค้ด `Input` ละเอียด)

---

## (c) แสง/บรรยากาศ: vignette, grading, ฟ้าแลบ, bloom

**ข้อเท็จจริงจากแหล่ง**
- `BlendState.Additive` = source factor `SourceAlpha`, destination `One`; `AlphaBlend` = source `One`, destination `InverseSourceAlpha` (**premultiplied**); `NonPremultiplied` = `SourceAlpha`/`InverseSourceAlpha` [3][4]
- `BlendState` มี property `ColorSourceBlend`/`ColorDestinationBlend`/`ColorBlendFunction` ให้ประกอบ blend เองได้ [4]
- `TextureProcessor` มี `PremultiplyAlpha` [14]; `Content.mgcb` ของเราตั้ง `PremultiplyAlpha=True` ทั้ง texture และฟอนต์ [R]
- Effect (.fx) บน DesktopGL: OpenGL แปลผ่าน MojoShader รองรับถึง `vs_3_0/ps_3_0`; ใช้ `#if OPENGL` เลือก shader model; parameter ที่ไม่ใช้ถูกตัดทิ้ง, ค่า default ของ parameter ใช้ไม่ได้บน GL (ต้องตั้งจาก C#), ไม่รองรับ preshader [8]
- เพิ่ม effect ผ่าน MGCB Editor ("Sprite Effect") และส่ง `effect:` เข้า `SpriteBatch.Begin` [10][1]; docs ทางการมีตัวอย่าง effect แบบ transition [10] และ lighting แบบ deferred (RT 3 ใบ + additive) [11]
- HLSL ที่ใช้ได้ที่ SM2/SM3: `lerp` [17], `smoothstep` [15], `tex2D` (เฉพาะ pixel shader) [16]
- `mgfxc` ใช้ profile `DirectX_11` หรือ `OpenGL` [9] — **หน้านี้ไม่ระบุ** ว่า profile OpenGL คอมไพล์บน OS ไหนได้

**สังเกต:** แสงแบบ SpriteBatch มีแล้ว (ดูหัวข้อ audit); `Gfx.Soft` สร้างด้วยสูตร `(1-d)^2` และคูณ `Color.White * a` (`Gfx.cs:48`) เป็น premultiplied ถูกต้องสำหรับ `AlphaBlend`

**ความเห็น**
1. **ระดับ SpriteBatch ล้วน (ไม่ต้อง shader, quick win):**
   - *Vignette:* สร้าง texture เต็มจอ 1 ใบตอนเริ่มเกม (เหมือน `MakeCircle` แต่กลางโปร่งใส ขอบดำ, ขนาด 256x144 ขยายเต็มจอด้วย `LinearClamp` เพื่อให้ไล่สีนุ่ม) แล้ววาดด้วย `AlphaBlend` ท้าย `SceneManager.Draw` (~30 บรรทัดใน `Gfx.cs` + 1 บรรทัดเรียก)
   - *โทนสีรายวัน/ฝน/กลางคืน:* ใช้ `Gfx.Rect` เต็มจอสีโปร่งแสง (วิธีเดียวกับ `Rain.cs:64` และ `BaseScene.cs:377`) เพิ่มค่าต่อวันใน `Balance`/scene — แต่ **ข้อจำกัด:** overlay แบบนี้ "ลด contrast" ไม่ใช่ grading จริง
   - *ฟ้าแลบ:* มีแล้ว (`Rain.cs:77-88`); ถ้าอยากให้เด่นขึ้น เพิ่มแฟลชวูบเดียวที่ "ดับ ฉับ" + เงาตัวละคร (ต้องมีเงาแยก ซึ่งมีอยู่ที่ `Art.cs:172`)
   - *Glow:* additive ด้วย `Soft` ที่ premultiplied อยู่แล้ว ภายใต้ `Additive` (source = `SourceAlpha` [3]) จะถูกคูณ alpha ซ้ำ → ขอบ glow ตกเร็วกว่าที่ตั้งใจ (**ข้อสรุปจากสูตรใน [3] + `Gfx.cs:48`**, ไม่ได้วัดภาพ) ถ้าจะให้ glow นุ่มกว่านี้ ให้ทดลองสร้าง `Soft` อีกใบสูตร `(1-d)` เฉยๆ สำหรับ additive
2. **ระดับ shader (medium):** วาดเกมทั้งเฟรมลง `RenderTarget2D` แล้ววาดลงจอด้วย `Begin(effect: ...)` ผ่าน `.fx` ที่ทำ vignette + grading (LUT-less: `lerp` สีเข้าโทนอบอุ่น + `smoothstep` ทำ vignette) — ใช้ได้ที่ `ps_3_0` ตาม [8]; **bloom จริง** ต้องหลายพาส (threshold → blur → add) ใช้ RT เพิ่ม 2 ใบ — มากกว่า 3 วัน และไม่คุ้มสำหรับ scope นี้ (ความเห็น)
   - โครง `.fx` ต้องตามรูปแบบใน [10] (`#if OPENGL` เลือก `vs_3_0/ps_3_0`, `SpriteTexture`/`sampler_state`, `technique` ที่มี `pass` และ `compile`) — ผมไม่ได้ใส่โค้ดตัวอย่างเต็มเพราะ [10] ตัดส่วน vertex shader/technique ออกจากบทความ จึงไม่มีตัวอย่างที่ยืนยันได้ครบ ให้ copy จากโปรเจกต์ตัวอย่างทางการ (MonoGame.Samples branch 3.8.5, `Tutorials/2dShaders` ตามที่ [10]-ซีรีส์ ระบุ)
   - **ความเสี่ยงที่ยังไม่ยืนยัน:** (i) ทีมใช้ `dotnet-mgcb` 3.8.4 (`.config/dotnet-tools.json`) กับ `MonoGame.Content.Builder.Task` `3.8.*` (restore ได้ 3.8.6 ตามรายงานก่อนหน้า) — ยังไม่ได้ทดลองว่า `EffectProcessor` คอมไพล์บนเครื่องทีมผ่านไหม; แนะนำเริ่มด้วย `.fx` ที่ pass-through 10 บรรทัด + `--shots` ก่อนทำจริง (ii) `ShotRunner` ต้องวาดผ่านเส้นทางเดียวกัน
3. **Lighting แบบ 2D light map** (แสงเตา/โคม ตัดกับความมืด): เทคนิคใน [11] ใช้ RT 3 ใบ (+ normal map) เกินความจำเป็น; รูปแบบย่อ (RT เดียว = light map ทำงานจาก `Gfx.Glow` additive แล้ว multiply กับฉาก) ทำได้ด้วย `BlendState` ที่ตั้ง `ColorSourceBlend`/`ColorDestinationBlend` เอง [4] — ไม่มีตัวอย่างทางการสำหรับ multiply แบบนี้ในแหล่งที่อ่าน ถือเป็น "ความเห็น" และต้องทดลอง

**plug-in:** `Gfx.cs` (+ ฟังก์ชัน vignette), `SceneManager.Draw` (1 บรรทัด), `Balance` (ค่าความเข้ม), และสำหรับ shader: `Content/effects/*.fx` + `Content.mgcb` + `Game1.Draw`; ความเสี่ยงต่อ DevTools: vignette/overlay **ต่ำ** (ผ่าน `SceneManager.Draw` เลยโผล่ใน `--shots`), shader **กลาง** (ต้องแก้ `ShotRunner` ให้ใช้ทางเดียวกัน)

---

## (d) Particle และ juice

**ข้อเท็จจริงจากแหล่ง**
- `SpriteBatch.Draw` overload ที่มี `position, sourceRectangle, color, rotation, origin, scale(Vector2/float), effects, layerDepth` ใช้วาด quad หมุน/ขยาย/ย้อมสีได้ในครั้งเดียว [1]
- `BlendState.Additive` สำหรับแสง/ประกาย [3][4]

**สังเกต:** มี texture พร้อมใช้ (`Pixel`, `Disc`, `Soft` ใน `Gfx.cs:14-16`), `Gfx.BeginAdditive/EndAdditive` (`Gfx.cs:81-91`), ฝนเป็นฟังก์ชันของเวลา ไม่สร้าง object (`Rain.cs:8,60-83`); จุดที่ควรมี burst: ผล QTE (`CareScenes.cs:213-248`), โดนตี/Dodge (`FightScene.cs:80-103,183-197`), เหรียญ (`Audio` มี `Sfx.UiCoin` แต่ไม่มีภาพประกอบ)

**ความเห็น (ออกแบบเล็กๆ ไม่ต้องมีแพ็กเกจ)**
- คลาส `Particles` แบบ static (struct array ขนาดคงที่ เช่น 512 ตัว: ตำแหน่ง/ความเร็ว/อายุ/สี/ขนาด/ชนิดหนึ่งในสาม: `Soft`+additive = ประกาย, `Disc`+alpha = ฝุ่น/เศษ, `Pixel` = เม็ดเล็ก) + `Particles.Burst(pos, kind, color, count)` + `Update(dt)` เรียกจาก `SceneManager.Update` (เหมือน `Rain.Tick`, `SceneManager.cs:92`) + `Draw` สองช่วง (alpha แล้ว additive) — ประมาณ 100-140 บรรทัด ไฟล์ใหม่ `World/Particles.cs`
- **ข้อควรระวังต่อ AutoPlay/telemetry:** ถ้า particle ใช้ `System.Random` ตัวเดียวกับที่ผูกกับ `RunSeed` ผลการเล่นของบอทอาจเปลี่ยน → ใช้ `Random` แยกอินสแตนซ์ (เหมือน `Rain.cs:20`) เพื่อไม่กระทบ determinism ของ `--seed`; และใน `--autoplay` ตัว `SceneManager.Update` ถูกเรียก 60 ครั้ง/เฟรม (`Game1.cs`) จึงควรข้ามการ spawn ตอน `Audio.Muted`/โหมดบอทถ้าต้องการลดต้นทุน (ไม่จำเป็น ถ้าอาร์เรย์ขนาดคงที่)
- Juice อื่นที่ถูกและเห็นผล: hit-stop (หยุด `Update` ของ fight 2-4 เฟรมตอน Perfect — แก้ใน `FightScene.Update` เพียงตัวนับ, ระวังไม่ให้ `AutoPlay` timing เปลี่ยนเกินไป เพราะบอทใช้เวลาจำลอง), squash/stretch ของสัตว์ตอนโดนตี (ใช้ `Pose.Scale` ที่มีอยู่ `SpriteMotion.cs:8-14` ผ่าน `TryDrawSprite`), เลขดาเมจลอยขึ้น (ขยาย `Popups` ที่มี), เหรียญบินเข้า HUD
- ใช้ `Ease` ที่มีอยู่ (`Core/Ease.cs`) ไม่ต้องเพิ่ม tween

**plug-in:** ไฟล์ใหม่ `World/Particles.cs`, เรียกจาก `SceneManager` (2 บรรทัด) และจุด burst 5-6 จุดใน `CareScenes.cs`/`FightScene.cs`/`BaseScene.cs`; ความเสี่ยงต่อ DevTools **ต่ำ** (ไม่เปลี่ยน member ที่ AutoPlay อ้าง); ประมาณ 0.5-1 วัน

---

## (e) ขัดเกลา UI: panel, ฟอนต์, ปุ่ม, transition

**ข้อเท็จจริงจากแหล่ง**
- `.spritefont` เป็น XML (`FontName`, `Size`, `Style`, `CharacterRegions`) และอบเป็น atlas ตอน build ภายใต้ pipeline [12]; ถ้าระบุชื่อฟอนต์ระบบ ใช้ได้เฉพาะเครื่องที่ติดตั้งฟอนต์นั้น — docs แนะนำให้ copy ไฟล์ .ttf/.otf ไว้โฟลเดอร์เดียวกับ `.spritefont` แล้วอ้างชื่อไฟล์เต็ม และตรวจ license (เช่น SIL OFL) [12]
- `FontDescriptionProcessor` ฝั่ง Windows ค้นหาฟอนต์จาก registry ด้วยชื่อขึ้นต้น และฟังก์ชันตรวจ style ถูก comment ปิด (`MatchFont` คืน true เสมอ) จึงไม่ยืนยันว่าได้หน้า Regular/Bold ตามที่ขอ — การเป็นตัวเอียงขึ้นกับว่าไฟล์ที่หาเจอเป็นหน้าเอียงหรือไม่ [13] (ซอร์ส branch `develop`; ไม่ได้ตรวจ tag 3.8.4/3.8.6)
- `PremultiplyAlpha` default = true ของ processor ฟอนต์ [13] ตรงกับ `Content.mgcb` ของเรา
- ตัวอักษรไทย: **ไม่จำเป็น** — `14-audio-fonts-list.md` §3 ระบุว่าข้อความในเกมเป็นภาษาอังกฤษทั้งหมด และ `CharacterRegions` ของทั้งสามฟอนต์ครอบ ASCII 32-126 เท่านั้น (`Content/Fonts/*.spritefont`)

**สังเกต:** ตัวอักษรเอียงทั้งเกม (W3) ทั้งที่ `Small` ตั้ง `Regular`; `FNT-01/02` ใน `14-audio-fonts-list.md` วางแผนเปลี่ยนเป็นฟอนต์ .ttf เฉพาะ (P1) อยู่แล้ว และ `Segoe UI` ระบุเป็นของชั่วคราว; `Ui.Panel` = `Rect` + `Outline` 2 px (`Ui.cs:123-127`), `Button` มี hover tween (ยก 1-2 px + ผสมสี) (`Ui.cs:53-63`); `SceneManager` มี fade ดำและ overlay scale-in (`SceneManager.cs:122-145`)

**ความเห็น**
1. **ฟอนต์ (quick win):** เลือก 1-2 ฟอนต์ที่ license อนุญาต (ตรวจหน้า license ของฟอนต์นั้นเอง — ผมไม่ได้เลือกชื่อให้เพราะยังไม่ได้ตรวจหน้า license ของฟอนต์ใดๆ), วาง .ttf ข้าง `.spritefont` แล้วเปลี่ยน `<FontName>` เป็นชื่อไฟล์ตาม [12] → ได้ทั้ง "หน้าตาตรงที่ตั้งใจ" และ "build ซ้ำได้ทุกเครื่อง" (ปัญหาใหม่ที่ [13] ชี้: ผลลัพธ์ปัจจุบันขึ้นกับฟอนต์ที่ติดตั้งในเครื่องคนสั่ง build); ฟอนต์พิกเซลควรใช้ขนาดที่เป็นจำนวนเต็มตามที่ตั้งใจออกแบบ (docs [12] บอกว่าฟอนต์พิกเซลดูดีที่ขนาดเล็ก) และ `LinearClamp` อาจทำให้เบลอ → วาดข้อความด้วย `PointClamp` เมื่อใช้ฟอนต์พิกเซล (ต้องแก้ `Gfx.Begin` ที่ใช้ sampler เดียวทั้ง batch; **ปานกลาง** เพราะต้องแยก batch ข้อความ)
   - **FontStashSharp** (Zlib, 1.6.1, net8.0 [22]) โหลด TTF ตอน runtime ได้ แต่หน้าอ้างอิงไม่ยืนยัน MonoGame เวอร์ชันที่ใช้/ DesktopGL → **ไม่แนะนำ** เพราะ SpriteFont ที่ฝัง .ttf ก็แก้โจทย์เราแล้วและไม่เพิ่ม dependency
2. **Panel/กรอบ:** ถ้ามีกรอบ PNG (UI-05, HUD-08) ให้ทำ 9-slice เอง: วาด 9 ชิ้นด้วย `Draw` + `sourceRectangle` [1] ประมาณ 30 บรรทัดใน `Ui.cs` (เปลี่ยน `Ui.Panel`/`Button.Draw`/`Ui.Bar` จุดเดียวเปลี่ยนทั้งเกม) — **ก่อนมี asset** ยังขัดเกลาด้วยโค้ดได้: เงา offset ใต้ panel, เส้นไฮไลต์ด้านในสีอ่อน 1 px, มุมตัดทแยง 2 px (วาดด้วย `Gfx.Rect` ไม่กี่ชิ้น), พื้นหลัง panel ไล่สีเล็กน้อย — ความเห็นว่าเห็นผลในหน้า `12_notebook`/`13_shop`/`14_dialogue` ทันที (ไม่ได้ทดสอบ)
3. **ข้อความล้นในร้านค้า (W9):** แก้เลย์เอาต์ (ลดข้อความ/ขยายการ์ด/ใช้ `Gfx.Wrap` + วัดความสูงก่อนวางปุ่ม) — ไม่ใช่งานภาพ แต่เห็นชัด
4. **Transition:** มี fade ดำแล้ว; การเปลี่ยนฉากแบบ wipe/iris ทำได้ด้วย shader ตามบทเรียนทางการ [10] (ใช้ค่า `Progress` + `smoothstep` [15]) หรือด้วยสี่เหลี่ยม `Gfx.Rect` หลายชิ้นโดยไม่ต้องใช้ shader — ลำดับความสำคัญต่ำ

**plug-in:** `Content/Fonts/*.spritefont` (+ไฟล์ .ttf ข้างๆ), `Ui.cs` (`Panel`/`Button`/`Bar`), `Scenes/BaseMenus.cs` (shop layout); ความเสี่ยงต่อ DevTools: **ต่ำ** ถ้าไม่เปลี่ยนข้อความ prompt (AutoPlay จับ prefix ของ `ChoiceScene.Prompt` เช่น `"Toothless is at"`/`"Sell"`) และไม่เปลี่ยน `BuyRect`/`UseRect` (`ShopScene.BuyRect`, `InventoryScene.UseRect` ถูก AutoPlay อ้าง ดู `CLAUDE.md`)

---

## (f) Workflow เอา asset เข้าเกม และข้อควรระวัง license

**ข้อเท็จจริงจากรีโป ([R])**
- ขั้นตอน: วางใน `Assets/_candidates/` → ทีมรีวิว → copy ไป `Content/Sprites/` → ลงทะเบียน `Content.mgcb` (`TextureImporter`/`TextureProcessor`, `PremultiplyAlpha=True`) → `dotnet build` → ตรวจด้วย `--shots` (`_candidates/README.md`)
- ชื่อไฟล์ที่โค้ดรอ: `pet_mossling|nibbleclaw|blinkbun|toothless`, `enemy_toothless`, `enemy_bigz`, `npc_merchant`, `npc_doctor` (ภาพนิ่งเดียวหันขวา; ถ้าไม่มีไฟล์ใช้ primitive ต่อ) — ตารางใน `_candidates/README.md`
- **นโยบาย AI/placeholder:** ค้นเอกสารที่ระบุ (`AGENTS.md`, `Context/Rules.md`, `05-asset-list.md`, `14-audio-fonts-list.md`) ไม่พบข้อความเรื่องการใช้ภาพสร้างด้วย AI; พบเฉพาะนโยบาย "placeholder ได้ถ้า asset ไม่ทัน" (`05-asset-list.md` Priority P0) และข้อกำหนดฟอนต์ "SIL OFL หรือ CC0/Free for commercial use" (`_candidates/README.md`) → **รีโปยังไม่มีนโยบายภาพ AI — ต้องให้ทีมตัดสินใจเอง**
- **ที่มาของ parallax:** `bg_parallax_*` คัดลอกมาจาก `_candidates/sprites/Hexagon/hexagon_*.png` (`_candidates/README.md`) แต่ในโฟลเดอร์นั้น **ไม่มีไฟล์ license/ที่มา** และ docs ไม่ระบุผู้วาด/license — **ควรตรวจและบันทึก** ก่อนเผยแพร่เกม (ผมตรวจไม่ได้จากรีโป)

**ข้อเท็จจริงจากแหล่ง**
- Aseprite: ซื้อได้หลายช่องทาง; EULA อนุญาตคอมไพล์ซอร์สใช้ส่วนตัวรวมถึงสร้าง asset เชิงพาณิชย์ แต่ห้ามแจกจ่ายตัวที่คอมไพล์ให้คนอื่น; บริษัทต้องมีหนึ่ง license ต่อนักพัฒนา, มี license พิเศษให้สถาบันการศึกษา; หน้า FAQ ไม่มีข้อความเรื่อง free trial [19] — ถ้าทีมใช้ในรายวิชา ควรตรวจสิทธิ์การศึกษากับ aseprite.org
- CLI ส่งออก sheet+JSON ตัวอย่างคำสั่งตาม [18] (`aseprite -b file.ase --trim --sheet sheet.png --data sheet.json --format json-array --sheet-pack`)
- Kenney: asset เป็น CC0 ใช้เชิงพาณิชย์ได้ ไม่บังคับให้เครดิต [23] — เหมาะเป็น **placeholder UI/ไอคอน** เท่านั้น เพราะไม่ใช่สไตล์ BePal; **ผมไม่ได้ตรวจชุดย่อยรายชุด** ว่าแต่ละชุดใช้ CC0 เหมือนกันหมด (ต้องเปิดหน้าของชุดนั้นดู)

**ความเห็น**
- ลำดับงานศิลป์ที่คุ้มสุด: (1) สัตว์ 4 ตัวภาพ idle (4 ไฟล์) + ตาแยก, (2) หมอ + พ่อค้า + Big Z, (3) กรอบ UI 9-slice 1 ชุด (panel + ปุ่ม + แถบ HUD) — แล้วค่อยพิจารณา animation/เฟอร์นิเจอร์ในฐาน (ENV-07..13) ซึ่งตอนนี้ยังเป็นสี่เหลี่ยมทั้งหมด (`03_base`, `05_base_bed_night`)
- กำหนด "สเปกพิกเซล" ให้ศิลปินก่อนวาด: สเกลแสดงผลเป็นจำนวนเต็ม (เช่น 3 เท่าเท่าผู้เล่น), ความสูง sprite ต้นฉบับ (เช่น สัตว์ ~30 px, เท่ากับ 90 px ที่ 3 เท่า ตามค่าคงที่ `Art.cs:23`), พาเลตต์ร่วมกับฉากหลัง (ดึงสีจาก `bg_parallax_*` ด้วยเครื่องมือเลือกสีของโปรแกรมวาด) — เป็นข้อเสนอ ไม่ใช่กฎเดิมของโปรเจกต์

---

## ตารางจัดลำดับความสำคัญ

ป้าย: **QW** = quick win (<1 วัน) · **M** = medium (1-3 วัน) · **BIG** = big (ต้องมี asset ใหม่) · เวลาเป็นประมาณการของผู้วิจัยสำหรับทีมนักศึกษา

| # | รายการ | ผลต่อภาพ | ความพยายาม | ความเสี่ยง | ไฟล์ที่แตะ |
|---|---|---|---|---|---|
| 1 | แก้ฟอนต์: ใส่ .ttf ที่ license ชัดข้าง `.spritefont` เลิกพึ่ง Segoe UI (แก้ตัวเอียง, build ซ้ำได้) | สูง (ทุกหน้า) | **QW** (ติดที่เลือกฟอนต์/ตรวจ license) | ต่ำ — ต้องตรวจว่าความกว้างตัวอักษรเปลี่ยนแล้ว layout (เช่น ร้านค้า) ไม่แตก | `Content/Fonts/*.spritefont`, ไฟล์ .ttf |
| 2 | ปัด shake offset เป็นจำนวนเต็ม + แก้ข้อความล้นในร้านค้า (W4, W9) | กลาง | **QW** | ต่ำ | `Core/Gfx.cs`, `Scenes/BaseMenus.cs` |
| 3 | Particle เล็ก (ประกาย QTE Perfect, เศษตอนโดนตี, เหรียญ, ฝุ่นเดิน) | สูง (ความรู้สึก) | **QW**–**M** | ต่ำ (ใช้ `Random` แยก) | ใหม่ `World/Particles.cs` + `SceneManager`, `CareScenes`, `FightScene` |
| 4 | Vignette + overlay โทนสีรายวัน/ฝนด้วย SpriteBatch | กลาง | **QW** | ต่ำ | `Core/Gfx.cs`, `Core/SceneManager.cs`, `Model/GameState.cs` (`Balance`) |
| 5 | ขัดเกลา `Ui.Panel`/`Button`/`Bar` ด้วยโค้ด (เงา ไฮไลต์ มุมตัด) | กลาง | **QW**–**M** | ต่ำ (ถ้าไม่แตะ `BuyRect`/`UseRect`/prompt) | `Core/Ui.cs` |
| 6 | **ตัดสินใจสไตล์/สเปกพิกเซลของสัตว์/NPC** (pixel เท่ากับผู้เล่น?) แล้วส่ง sprite ชุดแรก (4 สัตว์ + หมอ + พ่อค้า + Big Z) พร้อม **ตาแยก layer** และให้ `Art.TryDrawSprite` วาดตาทับ | **สูงที่สุด** (แก้ W1, W8) | **BIG** (+~0.5 วันโค้ด) | กลาง — ขัดกับ `05-asset-list.md` (280x360, hand-drawn) ต้องแก้เอกสารให้ตรง; ต้องจัดการ style ที่ผสมกับของ Figma | `Content/Sprites/*.png`, `Content.mgcb`, `World/Art.cs`, `05-asset-list.md` |
| 7 | กรอบ UI 9-slice จาก PNG (กล่องบทสนทนา ปุ่ม แถบ HUD) | สูง | **BIG** (+~0.5 วันโค้ด) | ต่ำ | `Core/Ui.cs`, `Content/Sprites/ui_*.png` |
| 8 | RT เต็มจอ + shader (`.fx`) สำหรับ grading/vignette/transition | กลาง-สูง | **M** (+ทดสอบ pipeline) | กลาง — ยังไม่ยืนยันว่า mgcb 3.8.4/3.8.6 คอมไพล์ `.fx` บนเครื่องทีม; `ShotRunner` ต้องผ่านทางเดียวกัน | `Game1.cs`, `Core/Gfx.cs`, `DevTools/ShotRunner.cs`, `Content/effects/*.fx`, `Content.mgcb` |
| 9 | เฟอร์นิเจอร์/ฐาน (เตียง กล่อง สถานี Upgrade เตา ประตู) เป็นภาพ | สูง | **BIG** | ต่ำ-กลาง (ต้องปรับ `BaseScene.DrawStructure`; ตำแหน่ง interaction ที่ AutoPlay อ้างต้องไม่ขยับ) | `Scenes/BaseScene.cs`, `Content/Sprites/` |
| 10 | Animation แบบ sprite sheet (เดิน/โจมตี/เจ็บ) | กลาง-สูง | **BIG** | ต่ำ | ตัวเล่นเฟรมใหม่ใน `World/` + `Art.cs` |
| 11 | RT ความละเอียดต่ำเฉพาะโลก + ขยายจำนวนเต็ม | กลาง | **M**–**BIG** | **สูง** (แตก batch ใน scene, แปลงพิกัด) — ไม่แนะนำตอนนี้ | `Scenes/*`, `Core/*` |
| 12 | Bloom หลายพาส / light map เต็มรูปแบบ | กลาง | **M**–**BIG** | กลาง | RT + `.fx` หลายตัว — ไม่แนะนำสำหรับ scope Day 1–5 |
| 13 | Letterbox / fullscreen / resize | ต่ำต่อภาพ | **M** | กลาง (ต้องแปลง `Input.Mouse`) | `Game1.cs`, `Core/Input.cs` |

ลำดับแนะนำ: 1 → 2 → 4 → 3 → 5 (ทั้งหมดไม่ต้องรอ asset) แล้วขนานกับงานศิลป์ข้อ 6 → 7 → 9; ข้อ 8 ทำหลังสุดเมื่อเนื้อหาภาพนิ่งแล้ว

---

## ความเสี่ยง / ความขัดแย้งกับเอกสาร

- **สไตล์:** `05-asset-list.md` (v3.2, ปรับ 2026-10-13) ยังระบุ "Soft Hand-drawn + Uncanny" และสัตว์ 280x360 ส่วน `12-references-art-direction.md` v1.1 ระบุ "As built = Soft Pixel Art" และ README ระบุให้ยึด Figma เมื่อขัดกัน — ต้องให้ทีม/เดียร์ (2D Art ตามเอกสาร) ตัดสินใจเป็นข้อความเดียวก่อนจ้างงานศิลป์ ไม่งั้นข้อ 6 จะพลาดทั้งชุด
- **ภาพ AI:** ไม่มีนโยบายในรีโป (ดูหัวข้อ f); อย่าสร้างนโยบายเอง ให้ทีมตัดสินใจ
- **สี:** GDD 12 เน้น "สว่าง โทนอบอุ่น (ครีม น้ำตาล เขียวมิ้นต์ ส้มโคม)" และ "หลีกเลี่ยงเส้นคมและสีสดจัดเกินไป" — grading ที่ทำให้หญ้าเขียวนีออนนุ่มลงสอดคล้องกับเอกสาร; แต่การปรับ palette ของฉากหลัง (`bg_parallax_*`) ต้องกลับไปที่ผู้วาดต้นฉบับ/ตรวจ license ก่อน (ดูข้อสงสัยเรื่องที่มา)
- **ตาจ้อง:** cue "uncanny: eyes track the player" (`Art.cs:8`, GDD 12 "มันกำลังมองเรา") หายถ้าใช้ sprite ภาพเดียว — ต้องมีตาแยก (W8)
- **ภาษา:** ไม่ต้องใส่ glyph ไทยในเกม (`14-audio-fonts-list.md` §3, `05-asset-list.md`: ข้อความในภาพเป็นอังกฤษ) — ถ้าอนาคตเปลี่ยน ต้องขยาย `CharacterRegions` (ฟอนต์ทั้งสามครอบแค่ ASCII)
- **DevTools coupling (`CLAUDE.md`):** AutoPlay อ้างชนิด scene, prefix ของ `ChoiceScene.Prompt`, `ShopScene.BuyRect`, `InventoryScene.UseRect`, ตำแหน่ง `BaseScene.*X`; งานภาพที่ไม่แตะสิ่งเหล่านี้ปลอดภัย; งานที่เปลี่ยนเส้นทางวาด (ข้อ 8, 11, 13) ต้องดู `ShotRunner`/`SceneCatalog` ด้วย; `GDD/04-class-diagram.md` ต้องอัปเดตถ้าเพิ่มโฟลเดอร์/คลาสใหม่ (เช่น `Particles`)
- **Scope:** `06-vertical-slice.md` กำหนด slice Day 1–5 และกำหนดส่ง 2026-09-30 (ผ่านไปแล้ว ตามรายงานก่อนหน้า) — งานภาพทั้งหมดในเอกสารนี้ไม่ขัดกับ scope แต่ไม่มีข้อไหนเป็น P0 ใหม่; อย่าให้รายการ BIG เลื่อนงานเนื้อหาที่ GDD ระบุ
- **Hexagon pack:** ที่มา/license ของ parallax ไม่ถูกบันทึกในรีโป (ดูหัวข้อ f)

---

## สิ่งที่ยังยืนยันไม่ได้ / คำถามเปิด

- **ที่มาของตัวอักษรเอียง:** [13] อธิบายว่าบน Windows ตัวหาฟอนต์ไม่ตรวจ style จึงอาจได้หน้าเอียง — ผมสังเกตว่าภาพเอียงจริง แต่ **ไม่ได้ตรวจ registry เครื่องนี้ว่า "Segoe UI Italic/Bold Italic" ถูกเลือกจริงหรือไม่** และซอร์สที่อ่านเป็น branch `develop` ไม่ใช่ tag 3.8.4/3.8.6 — ควรลองเปลี่ยนเป็นไฟล์ .ttf แล้ว `--shots` เทียบ
- **mgfxc/EffectProcessor บนเครื่องทีม:** ไม่ได้ทดลองคอมไพล์ `.fx` และหน้า [9] ไม่ระบุว่า profile OpenGL ทำงานบน OS ใด; ต้องทดลอง `.fx` pass-through ก่อนวางแผนงาน shader
- **ความเข้ากันของไลบรารีบุคคลที่สาม:** `MonoGame.Aseprite` [20][21] และ `FontStashSharp.MonoGame` [22] — หน้าอ้างอิงไม่ระบุเวอร์ชัน MonoGame ที่ต้องใช้/DesktopGL (ระบุเพียง "Updated for MonoGame 3.8.4" สำหรับ Aseprite) จึงไม่แนะนำให้เพิ่มจนกว่าจะลอง build จริง; ไม่ได้ประเมิน `Apos.Shapes` (ไม่แนะนำเพราะยังไม่ได้ตรวจ)
- **ผลของ `PointClamp` ตอนย่อภาพวาดมือ 280x360 → 90 px:** เป็นการคาดจากนิยามของ Point filter [2] ไม่ได้ทดลองกับไฟล์จริง (ยังไม่มีไฟล์สัตว์)
- **Blur ของตัวอักษรตอน shake:** อนุมานจากโค้ด (`Gfx.cs:68,78,146`) ไม่ได้ตรวจด้วยการรันจริงหรือวิดีโอ
- **สูตร glow:** ข้อสรุปว่า `Soft` ผ่าน `Additive` ตกเร็วเกินคาดมาจากสูตร [3] + `Gfx.cs:48` ไม่ได้วัดความต่างของภาพ
- **RenderTargetUsage:** หน้า API [6] ไม่อธิบายสมาชิกของ enum จึงยังไม่ได้อ้างพฤติกรรม DiscardContents/PreserveContents — ต้องอ่านหน้าของ enum ก่อนใช้ RT ข้ามเฟรม
- **ไม่ได้อ่านโค้ด `Core/Input.cs` ละเอียด** (เกี่ยวข้องกับการแปลงตำแหน่งเมาส์ถ้าทำ letterbox)
- **จำนวนบรรทัด/วัน:** ทั้งหมดเป็นประมาณการ ไม่ได้ prototype
- **ที่มา/license ของ Hexagon pack:** ตรวจจากรีโปไม่ได้
- **หน้า MonoGame docs ที่ URL แรกให้ 404** (SamplerState ใต้ `getting_to_know/whatis/graphics/`, RenderTarget ชื่อไฟล์ผิด) — ข้อมูลเรื่อง sampler มาจากบทเรียน [2] และเรื่อง RT มาจากผลค้นหา [5] (หน้า HowTo/WhatIs ที่ผลค้นหาชี้) ซึ่งผมไม่ได้เปิดอ่านเต็ม
