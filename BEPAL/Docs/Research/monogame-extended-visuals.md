# MonoGame.Extended 6.1.1 จะทำให้หน้าตาเกม BePal ดีขึ้นไหม

> **TL;DR:** ไม่ — MonoGame.Extended เป็น "กล่องเครื่องมือช่วยเขียนโค้ด" ไม่ใช่ตัวอัปเกรดภาพ (ไม่มี shader/lighting/bloom/AA ในซอร์ส) หน้าตาเกมจะดีขึ้นจริงได้จาก **asset จริง + particle + shader/lighting ของ MonoGame core** ส่วน Extended ช่วยได้เฉพาะจุด (Particles/SpriteSheet) และแทบไม่คุ้มเทียบกับต้นทุนที่ต้องแก้ DevTools

วันที่: 2026-10-14 · ผู้ขอ: ทีม BePal · ขอบเขตการอ่านโค้ด: read-only (ไม่ได้แก้ csproj/โค้ดเกม)

หมายเหตุวิธีอ่านเอกสารนี้: ข้อความที่มี `[n]` คือ **ข้อเท็จจริงจากแหล่งอ้างอิง**; หัวข้อ "ข้อสรุป/inference" คือความเห็นของผู้วิจัยเกี่ยวกับเกมนี้

---

## ที่มา / Sources

| # | URL | คืออะไร | ระดับ |
|---|-----|---------|-------|
| [1] | https://github.com/craftworkgames/MonoGame.Extended | README ของ repo (MIT, แจก NuGet, ลิงก์ docs) | Primary |
| [2] | https://www.nuget.org/packages/MonoGame.Extended/6.1.1 | หน้า NuGet ของแพ็กเกจหลัก (net8.0, MIT, อัปเดต 2026-08-18, "No dependencies") | Primary |
| [3] | https://www.nuget.org/packages/MonoGame.Extended.Content.Pipeline/6.1.1 | หน้า NuGet ของ pipeline (net8.0; deps: Autofac >= 5.2.0, MonoGame.Extended >= 6.1.1) | Primary |
| [4] | https://api.github.com/repos/craftworkgames/MonoGame.Extended/releases | Release notes 5.5.0 ถึง 6.1.1 | Primary |
| [5] | https://raw.githubusercontent.com/craftworkgames/MonoGame.Extended/HEAD/Directory.Build.props | root props: `net8.0`, `MonoGameVersion` = `3.8.5`, `Version` = `6.1.1` | Primary |
| [6] | https://raw.githubusercontent.com/craftworkgames/MonoGame.Extended/HEAD/source/Directory.Build.props | อ้างอิง `MonoGame.Framework.Native` `$(MonoGameVersion)` แบบ `PrivateAssets=All` (+ ทางเลือก FNA/KNI) | Primary |
| [7] | https://api.github.com/repos/craftworkgames/MonoGame.Extended/contents/source/MonoGame.Extended?ref=v6.1.1 | รายการไดเรกทอรี/ไฟล์ของซอร์สที่ tag v6.1.1 (60 รายการ) | Primary |
| [8] | .../contents/source/MonoGame.Extended/Graphics (และ `/Graphics/Effects`, `/Graphics/Effects/Resources`, `/Shapes?ref=v6.1.1`, `/Tweening`, `/Screens`, `/Screens/Transitions`, `/VectorDraw`, `/Particles`, `/Particles/Modifiers`) | รายการไฟล์ย่อย (host เดียวกับ [7]; Shapes ตรวจที่ tag v6.1.1, ที่เหลือตรวจที่ HEAD/`develop`) | Primary |
| [9] | https://raw.githubusercontent.com/craftworkgames/MonoGame.Extended/HEAD/source/MonoGame.Extended/Graphics/SpriteBatch.Extensions.cs | extension ของ SpriteBatch (มีแต่ `Draw` overloads: NinePatch/Sprite/Texture2DRegion) | Primary |
| [10] | .../HEAD/source/MonoGame.Extended/VectorDraw/PrimitiveDrawing.cs | API วาดรูปทรง (namespace `MonoGame.Extended.VectorDraw`) | Primary |
| [11] | .../HEAD/source/MonoGame.Extended/Shapes/Polygon.cs | `Polygon` = เรขาคณิตล้วน ไม่มีโค้ดวาด | Primary |
| [12] | .../HEAD/source/MonoGame.Extended/Screens/ScreenManager.cs | พฤติกรรม ScreenManager / Transition | Primary |
| [13] | .../v6.1.1/source/MonoGame.Extended/Graphics/AnimatedSprite.cs | `AnimatedSprite` เล่นจาก `SpriteSheet` | Primary |
| [14] | .../v6.1.1/source/MonoGame.Extended.Content.Pipeline/MonoGame.Extended.Content.Pipeline.csproj และ `readme.txt` | pipeline อ้าง `MonoGame.Framework.Content.Pipeline` `$(MonoGameVersion)`; readme บอกว่าเวอร์ชันแพ็กเกจกับ pipeline ต้องตรงกันทุกประการ | Primary |
| [15] | https://www.monogameextended.net/docs/about/introduction/ | หน้าแรกของ docs (รายการ feature + สถานะเอกสาร) | Primary (เอกสารทางการ) |
| [16] | https://www.monogameextended.net/docs/getting-started/installation-monogame/ | ขั้นตอนติดตั้ง (ระบุ 6.0.0; Pipeline optional) | Primary |
| [17] | https://www.monogameextended.net/blog/ และ /blog/version-6/ | รายการบล็อก + เนื้อหา 6.0.0 (breaking changes) | Primary |
| [18] | https://www.monogameextended.net/docs/features/particles/quick_start/ | Particle quick start | Primary |
| [19] | https://www.monogameextended.net/docs/features/camera/ | Camera / viewport adapter / ไม่มี shake | Primary |
| [20] | https://www.monogameextended.net/docs/features/tweening/ | Tweening (ป้าย "not up to date for 6.0.0") | Primary |
| [21] | https://www.monogameextended.net/blog/ai-contributions-and-the-future-of-monogame-extended/ | โพสต์ 2026-08-10 เรื่องนโยบาย PR/การดูแลโปรเจกต์ | Primary |
| [R] | ไฟล์ในรีโป BePal: `CLAUDE.md`, `BEPAL/Bepal_Game/Bepal/Bepal.csproj`, `.config/dotnet-tools.json`, `obj/project.assets.json`, `Core/Gfx.cs`, `Core/Ease.cs`, `World/Rain.cs`, `World/Backdrop.cs`, `World/SpriteMotion.cs`, `Core/SceneManager.cs`, `Docs/GDD/05-asset-list.md`, `12-references-art-direction.md` | สภาพปัจจุบันของเกม | Primary (โค้ดเราเอง) |

ข้อจำกัดของการเข้าถึง: เครื่องมือที่ใช้ดึงหน้าเว็บสรุปเนื้อหาผ่านโมเดลขนาดเล็ก จึงไม่ใช่การอ่านซอร์สแบบ byte-by-byte ทุกบรรทัด; การค้นโค้ดทั้ง repo ผ่าน GitHub search API ถูกปฏิเสธ (HTTP 401) ดังนั้นข้อความ "ไม่มี X" ด้านล่างอิงจาก **รายการไดเรกทอรีระดับบนและไฟล์ที่ระบุชื่อ** ไม่ใช่การ grep ทั้งโค้ด

---

## 1. สภาพปัจจุบันของเกม (ข้อเท็จจริงจากโค้ด BePal [R])

- วาดเกือบทุกอย่างจาก primitive ผ่าน `Gfx` (`Rect`, `Ellipse`, `Glow`, `Line`, `Arc`) บน texture ที่สร้างตอน runtime (`Pixel`, `Disc` ขอบนุ่ม 1px, `Soft` radial gradient) — `Core/Gfx.cs` บรรทัด 39-53 และ 94-139
- มี `Glow` + `BeginAdditive` สำหรับแสงแบบ additive อยู่แล้ว (`Gfx.cs` บรรทัด 80-91) ใช้กับแสงฟ้าแลบใน `World/Rain.cs`
- ฝน = 280 เส้นที่เป็นฟังก์ชันของเวลา ไม่สร้าง object ต่อเฟรม (`Rain.cs` บรรทัด 11-83)
- Screen shake เป็นของตัวเอง (`Gfx.Shake/UpdateShake`, `Gfx.cs` บรรทัด 55-71) ผ่านเมทริกซ์ใน `Gfx.Begin`
- Easing/ smoothing เป็นของตัวเอง (`Core/Ease.cs`: `OutCubic`, `InOutSine`, `OutBack`, `Smoothstep`, `Damp`, `Smoothed`)
- ฉากหลัง parallax 5 ชั้น 320x180 วาด 4x ด้วย `PointClamp` + ฉากหน้า `fg_jungle` (`World/Backdrop.cs`) — ใน 7 PNG เท่านั้น
- ตัวผู้เล่นเป็นรูปนิ่งภาพเดียว ขยับด้วย `SpriteMotion` (squash/stretch/lean แบบ procedural); เอกสาร asset ระบุว่า sprite sheet ผู้เล่น (PLR-01/02) ยัง "Not Started" (`05-asset-list.md` บรรทัด 55-56)
- Scene stack + fade + overlay-open animation เป็นของตัวเอง (`Core/SceneManager.cs`); มี Wheel QTE, Button/Slider เอง
- ไม่มีการใช้ `Effect`/shader/`RenderTarget2D` ในโค้ดเกมเลย (grep `Effect|Shader|RenderTarget` ได้เฉพาะ `SoundEffect`/`SpriteEffects`)
- แพ็กเกจ: `MonoGame.Framework.DesktopGL` `3.8.*` และ `MonoGame.Content.Builder.Task` `3.8.*`; ที่ restore จริงตอนนี้คือ **3.8.6** (`obj/project.assets.json`) ส่วน `dotnet-tools.json` ตรึง mgcb ไว้ที่ **3.8.4** (CLAUDE.md ก็เขียน 3.8.4 — เวอร์ชันที่ใช้จริงไม่ตรงกัน แต่ไม่เกี่ยวกับคำถามนี้)

---

## 2. MonoGame.Extended 6.1.1 มีอะไรบ้าง (ข้อเท็จจริงจากแหล่ง)

### 2.1 ภาพรวมและสถานะ
- เป็นชุดยูทิลิตี้/ไลบรารี "ไม่ใช่ engine หรือ framework"; ใบอนุญาต MIT; เจ้าของแพ็กเกจ `craftworkgames` [1][2][15]
- แพ็กเกจแจกเป็น `net8.0` เดียว, หน้า NuGet ระบุ "No dependencies", อัปเดตล่าสุด 2026-08-18 [2]
- ไทม์ไลน์: 6.0.0 เมื่อ 2026-05-02, 6.0.1 เมื่อ 2026-06-30, 6.1.0 เมื่อ 2026-07-23, 6.1.1 เมื่อ 2026-08-18 [4]. 6.1.1 = แก้ type ซ้ำของ `TiledXmlParser`, อัปเดต KNI, ทำ LDtk/Ogmo ให้รองรับ Native AOT, ตรวจแพลตฟอร์มผ่าน `PlatformInfo.GraphicsBackend` [4]
- โครงการยังอยู่ในช่วงเอกสารไม่ทันโค้ด: หน้าแรก docs บอกว่า docs "กำลังปรับให้ตรง V5"; หมวด Screen Management, Tilemap, Tweening, Serialization ถูกป้าย "ต้องอัปเดต" [15]; หน้า Tweening ระบุว่าไม่ตรงกับ 6.0.0 [20]; หน้าติดตั้งใช้ 6.0.0 แต่มีกล่องเตือนว่า stable คือ 4.0.0 (เนื้อหาขัดกันเอง) [16]
- นโยบายผู้ดูแล (2026-08-10): ไม่รับ PR ที่ implement ด้วย AI [21] — ไม่กระทบเราโดยตรง เพียงบอกว่าโครงการมีผู้ดูแลน้อยคน

### 2.2 โมดูล (ดูจากไดเรกทอรีที่ tag v6.1.1 [7] และไฟล์ย่อย [8])

| โมดูล / namespace | มีอะไร | แหล่ง |
|---|---|---|
| `Graphics` | `Sprite`, `AnimatedSprite`, `SpriteSheet`, `SpriteSheetAnimation(+Builder/Frame)`, `Texture2DAtlas`, `Texture2DRegion`, `NinePatch`, `FlipFlags`, `RenderTarget2DExtensions`, `GraphicsDevice.Extensions`, `SpriteBatch.Extensions` (มีเฉพาะ `Draw` overloads) และ `Effects/` = `DefaultEffect`, `MatrixChainEffect` | [8][9][13] |
| `Shapes` | `Polygon`, `Polyline` = **เรขาคณิตล้วน ไม่มีโค้ดวาด** | [8][11] |
| `VectorDraw` | `PrimitiveBatch` + `PrimitiveDrawing`: `DrawCircle`/`DrawSolidCircle`, `DrawArc`, `DrawEllipse`, `DrawSegment`, `DrawPolygon`, `DrawRectangle` ฯลฯ — มีพารามิเตอร์ `sides` ของ arc/ellipse; ค่าคงที่ `CircleSegments` = 32 (16 บนบางแพลตฟอร์ม) ไม่มีการกล่าวถึง anti-aliasing (มีแต่ alpha-blend) | [10] |
| `Particles` | `ParticleEffect`, `ParticleEmitter`, `ParticleBuffer`, `Profiles` (เช่น Spray, Circle), `Modifiers` (Age, Drag, LinearGravity, OpacityFastFade, Rotation, Velocity, VelocityColor, Vortex), `Interpolators`, `ParticleEffectSerializer`; วาดผ่าน `SpriteBatch.Draw(particleEffect)`; ต้องใช้ `Texture2DRegion` | [8][18] |
| `Tweening` | `Tweener`, `Tween`, `LinearTween`, `ColorTween`, `EasingFunctions` | [8][20] |
| `Screens` | `ScreenManager`, `Screen`, `GameScreen`, `Transitions/` = `FadeTransition`, `ExpandTransition`, `Transition` | [8][12] |
| กล้อง | `OrthographicCamera`, `Camera<T>`, `ViewportAdapters/` (เช่น `BoxingViewportAdapter`); docs บอกชัดว่า **ไม่มี shake ในตัว** (ให้สุ่มออฟเซ็ตเอง) | [7][19] |
| `BitmapFonts` | รองรับ BMFont/Hiero | [15] |
| `Input` | extended keyboard/mouse/gamepad + `InputListener` | [15] |
| `Collisions`, `Collision2D`, `Bounding*2D` | ระบบชนที่ปรับใหม่ใน 6.0.0 | [7][17] |
| `ECS` | Entity Component System (จำกัด component 32 → 256 ใน 6.0.0) | [7][17] |
| `Tilemaps` | ระบบใหม่ใน 6.0.0 รองรับ Tiled/LDtk/Ogmo; **namespace `MonoGame.Extended.Tiled` ถูกลบแล้ว** | [7][17] |
| อื่นๆ | `Collections`, `Timers`, `Serialization`, `HslColor`, `FramesPerSecondCounter`, `Math` | [7] |
| Content Pipeline | importer/processor สำหรับ Animations, BitmapFonts, Json, Particles, TextureAtlases, Tilemaps (LDtk/Ogmo/Tiled) | [3][8] |

สิ่งที่ **ไม่พบ** (ระดับไดเรกทอรีบนที่ tag v6.1.1 [7]): ไม่มีโฟลเดอร์ `Gui`, `Shader`, `Lighting`, `Bloom`; รายการไฟล์ใน `Graphics/Effects/` มีเพียง `DefaultEffect`, `EffectResource`, `ITextureEffect`, `MatrixChainEffect` + resource ของ `DefaultEffect` (.fx/.mgfxo/.fxb/.knifxo) และ `Macros.fxh`/`Structures.fxh` [8] — เป็น effect พื้นฐานสำหรับให้ไลบรารีเองวาดได้ ไม่ใช่เอฟเฟกต์ภาพให้ผู้ใช้เลือก (ข้อสรุปจากชื่อไฟล์ ไม่ได้อ่านโค้ด .fx ทั้งหมด)

การเปลี่ยนแปลง 4.x/5.x → 6.x ที่ยืนยันได้: ลบ `MonoGame.Extended.Tiled` (ใช้ระบบ Tilemaps ใหม่), ลบ `BitVector32` ใน ECS (ใช้ `ComponentBits`), ลบ flow ชนแบบ `IShapeF` แบบเดิม และ "legacy shape helper" บางตัว [17]; ใน 5.x ประกาศ obsolete แล้วจะลบใน 6.0.0 ได้แก่ Pitch ของกล้อง, `ParticleEffectReader/Writer` (แทนด้วย `ParticleEffectSerializer`) [17]; `ScreenManager.LoadScreen()` obsolete แล้วให้ใช้ Show/Close/Replace [17][12]. **สถานะ Gui:** ผมไม่เห็น Gui ใน tag v6.1.1 [7] แต่ไม่ได้พบเอกสารที่บอกว่า "ย้ายไปไหน/ถูกถอด" จึงสรุปแค่ว่า "ไม่มีใน 6.1.1" (ไม่ยืนยันประวัติ). **Stable vs preview:** 6.0.0, 6.0.1, 6.1.0, 6.1.1 เป็น release ปกติ [4]; ส่วนที่เอกสารยังไม่ตามทัน (Tweening, Screens, Tilemap) ควรถือว่าเสี่ยงต่อการใช้งานจริงมากกว่า

### 2.3 ตรงกับเกมเราตรงไหนบ้าง (ภาพรวมก่อนลงรายละเอียดข้อ 4)
`Ease`/`Smoothed` ↔ `Tweening`; `Gfx.Shake` ↔ (Extended ไม่มี shake); `Gfx.Rect/Ellipse/Arc/Line` ↔ `VectorDraw.PrimitiveDrawing`; `Rain.cs` ↔ `Particles`; `SceneManager` ↔ `Screens`; `SpriteMotion` ↔ `AnimatedSprite`/`SpriteSheet` (คนละแนวทาง); Slider/Button ↔ (ไม่มี Gui); SpriteFont ↔ `BitmapFonts`.

---

## 3. ความเข้ากันได้กับโปรเจกต์ (ข้อเท็จจริง + ช่องว่างที่ยังไม่ยืนยัน)

ข้อเท็จจริง:
- ไลบรารีคอมไพล์ด้วย `net8.0` เทียบกับ `MonoGameVersion` = **3.8.5** [5] โดยอ้าง `MonoGame.Framework.Native` แบบ `PrivateAssets=All` [6] จึงไม่ถ่ายทอด dependency ไปที่ผู้ใช้ (ตรงกับหน้า NuGet ที่บอก "No dependencies" [2]) — ผู้ใช้ต้องมี MonoGame เองอยู่แล้ว
- เอกสารทางการ **ไม่ระบุ** เวอร์ชัน MonoGame ขั้นต่ำสำหรับ 6.1.1 [15][16]; มีเพียงร่องรอย 3.8.5 ใน props [5] และใน PR ของ 6.1.0 [4]. บล็อกเดือน 2026-01 บันทึกปัญหา import `.tmx` กับ ContentBuilder 3.8.5 [17] (เกี่ยวกับ Tiled ซึ่งเราไม่ใช้)
- `MonoGame.Extended.Content.Pipeline` **ไม่จำเป็น** ถ้าไม่ใช้ importer ของ Extended (เช่น particle/atlas/bitmap font จากไฟล์ pipeline) — ตัวหน้า docs ระบุว่า optional และโหลดจากดิสก์ตอน runtime ได้ [16]. ถ้าใช้ ต้องใส่ DLL ใน `Content.mgcb`, เวอร์ชันต้อง **ตรงกันทุกประการ** กับแพ็กเกจหลัก [14] และมันดึง `Autofac >= 5.2.0` [3]; pipeline อ้าง `MonoGame.Framework.Content.Pipeline` `$(MonoGameVersion)` (= 3.8.5) [14] ขณะที่โปรเจกต์เราตรึง mgcb tool ที่ 3.8.4 และ restore Content.Builder.Task ได้ 3.8.6 [R] — **สามเวอร์ชันนี้ยังไม่เคยทดสอบด้วยกัน**

ช่องว่าง (ยังไม่ยืนยัน — ต้องลองจริง): (1) assembly ที่ build กับ 3.8.5 `Native` จะทำงานกับ `MonoGame.Framework.DesktopGL` 3.8.6 ได้ไหม (คาดว่าได้ถ้าไม่เรียก API ที่เพิ่ม/เปลี่ยนใน 3.8.5+ แต่ไม่มีหลักฐาน); (2) ข้อความ "6.1.0 adds native backend support" [4] หมายความว่าอย่างไรต่อผู้ใช้ DesktopGL; (3) pipeline 3.8.5 vs mgcb 3.8.4

---

## 4. มีฟีเจอร์ด้านคุณภาพภาพ (shader/lighting/bloom/AA) ไหม

คำตอบจากหลักฐาน: **ไม่มี**
- ไม่มีโฟลเดอร์ Bloom/Lighting/Shader ที่ระดับบนของซอร์ส v6.1.1 [7]
- `Graphics/Effects` มีเฉพาะ `DefaultEffect`/`MatrixChainEffect` ที่ใช้ภายใน [8]
- `PrimitiveDrawing` ไม่มีการกล่าวถึง anti-aliasing [10]; `SpriteBatch.Extensions.cs` ไม่มีเมธอดวาดรูปทรงเลย (เป็น `Draw` ของ NinePatch/Sprite/TextureRegion เท่านั้น) [9] — ดังนั้นตัว `DrawCircle(this SpriteBatch ...)` แบบที่เคยมีใน 3.x-5.x **ไม่พบใน 6.1.1** (6.0.0 ระบุว่าลบ "legacy shape helper" บางส่วน [17] แต่ไม่ได้ระบุชื่อ จึงห้ามสรุปว่าเป็นตัวนี้โดยตรง)
- ข้อจำกัด: ตรวจจากรายการไฟล์และไฟล์ที่ระบุชื่อ ไม่ใช่ grep ทั้งโค้ด

สิ่งที่ต้องใช้แทน (MonoGame core — ไม่ใช่ Extended):
- **Bloom / glow จริง / blur / vignette / CRT / color grading:** `RenderTarget2D` วาดฉากลงเท็กซ์เจอร์ก่อน แล้ววาดผ่าน `Effect` (HLSL คอมไพล์เป็น `.fx` ผ่าน MGCB/`mgfxc`) ด้วย `SpriteBatch.Begin(effect: ...)` หลายพาส
- **แสงแบบ 2D (เช่น ไฟหน้าเตาผิง/โคมในกลางคืน):** ทำได้ด้วย `RenderTarget2D` ที่ทำ "light map" (วาด `Soft` texture แบบ additive ลงไป) แล้ว multiply กับฉาก — ใช้เทคนิคที่เกมมีของอยู่แล้ว (`Gfx.Glow`, `BeginAdditive`) แค่เพิ่ม render target
- **Pixel-perfect pixel art:** วาดทั้งเกมลง `RenderTarget2D` ความละเอียดต่ำแล้วขยายด้วย `PointClamp` (ปัจจุบันเกมวาด backdrop ที่ 4x ตรงๆ อยู่แล้ว)

---

## 5. ตารางฟีเจอร์ → จุดใน BePal → ข้อสรุป (inference ของผู้วิจัย)

| ฟีเจอร์ Extended | จุดใน BePal | ข้อสรุป | เหตุผลสั้นๆ |
|---|---|---|---|
| `Particles` (`ParticleEffect`/`Emitter`/Modifiers) | ฝน/ฟ้าแลบ (`World/Rain.cs`), ประกายตอน QTE Perfect/Great, ประกายตี/ชน (`FightScene`), ควัน/ฝุ่น, ผงในห้อง | **เห็นผลดีขึ้นได้ (แต่จำกัด)** | ปัจจุบันไม่มี particle ทั่วไป (ฝนเป็นเส้น 280 เส้นแบบเขียนเอง) การมีระบบ emitter+modifier (gravity, drag, fade, vortex) ทำให้เพิ่มเอฟเฟกต์ได้เร็วและหลากหลาย; แต่ความงามขึ้นกับ texture ของ particle ที่เราต้องวาด/สร้างเอง (Quick Start ใช้พิกเซลขาว 1x1) [18]; blend mode แบบ additive ไม่ได้กล่าวไว้ในหน้านั้น — ต้องตรวจเอง (เราจัดการ blend ผ่าน `Gfx.BeginAdditive` ได้) |
| `Tweening` (`Tweener`, `EasingFunctions`) | `Core/Ease.cs`, bar `Smoothed`, overlay-open (`SceneManager`) | **ซ้ำกับที่มี / สะดวกโค้ดอย่างเดียว** | มี easing ที่ใช้อยู่ครบ (OutCubic/OutBack/InOutSine/Damp); docs ส่วนนี้ล้าสมัยกว่า 6.0.0 [20]; ไม่เปลี่ยนภาพ |
| `VectorDraw.PrimitiveDrawing` / `Shapes` | `Core/Gfx.cs` (`Rect/Ellipse/Arc/Line`) | **ซ้ำ และอาจแย่กว่า** | ของเราใช้ texture `Disc` ขอบนุ่มผ่าน SpriteBatch ซึ่งอยู่ batch เดียวกับทุกอย่างและรองรับ shake/overlay เมทริกซ์; `PrimitiveBatch` เป็น batch แยก ต้องจัดการ Begin/End สลับ; ไม่มี AA ตามหลักฐาน [10]; `Shapes` ไม่มีโค้ดวาด [11] |
| `OrthographicCamera` + `BoxingViewportAdapter` | `World` 3840 px ของ `BaseScene`, `Backdrop.Draw(camX)`, `Gfx.Shake` | **ไม่เปลี่ยนภาพ / ซ้ำ** | Camera ให้แค่ view matrix, zoom, rotation, `WorldToScreen`; ไม่มี shake [19]; parallax ของเราคำนวณเองต่อชั้น (`Backdrop`, ค่า factor) — ถ้าย้ายก็แค่เปลี่ยนวิธีเขียน; Boxing/letterbox มีประโยชน์เฉพาะถ้าต้องการรองรับจอ/ปรับขนาดหน้าต่าง ซึ่งเกมตอนนี้ล็อก 1280x720 |
| `SpriteSheet` / `AnimatedSprite` / `Texture2DAtlas` | แทน `Art.cs` (ตัวละครแบบ procedural), `Art.PlayerSprite` + `SpriteMotion` | **ดีขึ้นเมื่อมี asset จริงแล้วเท่านั้น (ความสะดวก)** | ตัวเฟรมเอฟเฟกต์ภาพมาจากภาพวาด ไม่ใช่ไลบรารี; ตอนนี้ PLR-01/02 ยัง Not Started [R]; `AnimatedSprite` เล่นจาก `SpriteSheet` ที่โหลดผ่าน pipeline/ไฟล์ [13] — ถ้าเรามีแค่ sprite sheet แนวนอนเฟรมเท่ากัน เขียนตัวเล่นเฟรมเองได้ในไม่กี่สิบบรรทัด |
| `NinePatch` | กรอบ UI/กล่องบทสนทนา (`Core/Ui.cs`, `Button`) | **เห็นผลได้ถ้ามีกรอบ PNG จริง** | ตอนนี้กรอบวาดจาก `Rect/Outline` ไม่มีรูป; NinePatch ทำให้ขอบ/มุมของกรอบที่วาดมือสวยคงที่ทุกขนาด [9] — ต้องมีงานศิลป์ก่อน |
| `Screens` (`ScreenManager`, `FadeTransition`) | `Core/SceneManager.cs` + `Scene` abstract; `AutoPlay`/`ShotRunner`/`SceneCatalog` | **ซ้ำ + เสี่ยงทำของพัง** | เรามี stack/overlay/fade ที่ DevTools พึ่งพาโดยตรง (pattern-match ชนิด scene, `M.Push/Remove/Reset`); ของ Extended มี stack + transition เหมือนกัน (ทำงาน bottom-to-top, transition ทีละอันเดียว) [12] และ docs ยังป้าย "ต้องอัปเดต" [15] — การย้ายไม่ให้ภาพใหม่ แต่ต้องแก้ DevTools |
| `BitmapFonts` | `Content/Fonts/Main\|Big\|Small` (SpriteFont) | **ไม่เปลี่ยนภาพ / ไม่เหมาะ** | SpriteFont ที่ใช้อยู่ทำงานได้; BitmapFont ต้องการ BMFont/Hiero สร้างไฟล์ ถ้าอยากได้ฟอนต์พิกเซลสวยก็เป็นเรื่องฟอนต์/asset ไม่ใช่ไลบรารี (หมายเหตุ: ยังไม่ได้ตรวจการรองรับตัวอักษรไทยของ BitmapFont — ยังไม่ยืนยัน) |
| `Collisions`, `ECS`, `Tilemaps`, `Input`, `Collections`, `Timers` | ไม่มีส่วนที่ตรงใน BePal (ชนแบบ wheel/QTE, ไม่มี tilemap) | **ไม่เกี่ยวข้อง** | — |
| Shader / Bloom / Lighting / AA | ทั้งเกม | **Extended ไม่มี** | ดูข้อ 4 |

---

## 6. ต้นทุนและความเสี่ยง (inference ผสมข้อเท็จจริง)

- **Dependency เพิ่ม:** แพ็กเกจหลัก (ไม่มี transitive dependency ตาม [2]) + ถ้าใช้ pipeline เพิ่ม `Autofac` และต้องล็อกเวอร์ชันให้ตรงกัน [3][14]
- **เวอร์ชัน MonoGame:** โครงการเรา `3.8.*` (ตอนนี้ได้ 3.8.6) ส่วน Extended build กับ 3.8.5 [5] — ไม่มีหลักฐานว่าเข้ากันได้/ไม่ได้ จึงต้องมีขั้นตอนทดลอง build + `--autoplay` + `--shots` ก่อนตัดสินใจ; เนื่องจาก `3.8.*` ลอยตัว การออก MonoGame 3.8.x ใหม่ในอนาคตอาจทำให้พังโดยที่เราไม่ได้แตะโค้ด
- **การผูกกับ DevTools:** `AutoPlay`/`ShotRunner`/`SceneCatalog` จับชนิด scene ของเราโดยตรง (CLAUDE.md) ดังนั้นห้ามย้ายไป Extended `ScreenManager` โดยไม่เขียน DevTools ใหม่
- **ความน่าเชื่อถือของ docs:** หลายหมวดป้าย "ต้องอัปเดต" และเอกสารติดตั้งอ้างเวอร์ชันขัดกัน [15][16][20] — เวลาเจอปัญหาต้องอ่านซอร์สเอง
- **ขอบเขตงาน:** prototype slice (กำหนดส่ง 2026-09-30) ผ่านไปแล้ว; งานที่คุ้มกว่าคือเนื้อหา/ศิลป์ ไม่ใช่เปลี่ยนโครงไลบรารี
- **ผลต่อการ build ที่ทีมใช้:** `Bepal.csproj` มี target `RestoreDotnetTools` ที่รัน `dotnet tool restore` ทุก build; เพิ่ม package pipeline อาจต้องแก้ `Content.mgcb` ด้วย (ไม่เจอปัญหา install ที่บันทึกไว้ใน release notes [4] แต่ไม่ได้อ่าน issue tracker — ยังไม่ยืนยัน)

---

## 7. คำแนะนำ

**ข้ามไปก่อน (skip) สำหรับเป้าหมาย "ให้หน้าตาดีขึ้น"** — Extended ไม่ใช่ตัวทำให้ภาพดีขึ้น และส่วนที่ซ้ำกับของเรา (Tween/Shapes/Camera/Screens) ให้ประโยชน์เป็นศูนย์ต่อภาพ

ถ้าจะใช้บางส่วนจริง: **adopt partially เฉพาะ `MonoGame.Extended` (ไม่เอา Content.Pipeline) แล้วใช้ `Particles` อย่างเดียว** ทดลองใน branch แยกตามลำดับ
1. เพิ่ม package เดียว, build + `--autoplay` + `--shots` เพื่อยืนยันความเข้ากันกับ 3.8.6 (ช่องว่างข้อ 3)
2. ใช้ `ParticleEffect` กับจุดเดียวก่อน (เช่น ประกาย QTE Perfect) แล้วเทียบกับการเขียน emitter เล็กๆ เอง (ประมาณร้อยบรรทัด) — ถ้าผลต่างน้อย ให้เขียนเองและไม่ต้องมี dependency
3. อย่าย้าย `Ease`, `Gfx`, `SceneManager`, กล้อง/shake

**สิ่งที่ทำให้หน้าตาดีขึ้นมากที่สุด (เรียงตามผลต่อภาพ — เป็นความเห็นของผู้วิจัย):**
1. **Asset จริง** — sprite sheet ผู้เล่น/สัตว์เลี้ยง/ศัตรู (ตอนนี้ตัวละครส่วนใหญ่เป็นรูปทรงจาก primitive; รายการอยู่ใน `05-asset-list.md`) และกรอบ UI
2. **Particles/เอฟเฟกต์** (ฝุ่น, ประกาย, ควัน, ผงในห้อง) — เขียนเองหรือใช้ Extended Particles ก็ได้
3. **แสง + Post-processing ด้วย MonoGame core:** `RenderTarget2D` + HLSL (`mgfxc`) ทำ light map สำหรับกลางคืน/เตาผิง, bloom เบาๆ, vignette — เป็นสิ่งที่ Extended ไม่มีให้
4. ความสม่ำเสมอของสไตล์ pixel-art (ลง `RenderTarget2D` ความละเอียดต่ำ + `PointClamp` ถ้าต้องการ pixel-perfect)

---

## 8. สิ่งที่ยังยืนยันไม่ได้ / คำถามเปิด

- ความเข้ากันได้ในทางปฏิบัติของ `MonoGame.Extended` 6.1.1 (build กับ 3.8.5 `Native`) กับ `MonoGame.Framework.DesktopGL` 3.8.6 ของโปรเจกต์ — ยังไม่ได้ลอง build จริง (ตามข้อกำหนดงานนี้ห้ามแก้ csproj)
- Pipeline 3.8.5 vs `dotnet-mgcb` 3.8.4 ที่ตรึงไว้ — ยังไม่ทดสอบ
- ไม่ได้อ่านซอร์ส `ParticleEmitter`/`PrimitiveBatch` ทั้งไฟล์: จึงไม่ยืนยันเรื่อง blend mode additive ของ particle, การวาดรูปแบบ batch ร่วมกับ `SpriteBatch`, และ allocation ต่อเฟรม
- ไม่ได้ตรวจ GitHub issues/Discussions (เพื่อหาปัญหาติดตั้ง/ความเข้ากันกับ MonoGame 3.8.6) และ GitHub search API ใช้ไม่ได้ (401) — ข้อความ "ไม่มี X" จึงอิงรายการไฟล์ ไม่ใช่ grep เต็ม repo
- สถานะ/ประวัติของโมดูล `Gui` ในซีรีส์ 4.x/5.x → ไม่ยืนยัน (รู้เพียงว่าไม่อยู่ใน v6.1.1)
- ความสามารถของ `BitmapFont` กับตัวอักษรไทย — ไม่ได้ตรวจ
- ซอร์สส่วนที่อ้างด้วย HEAD (ไม่ใช่ tag v6.1.1): `ScreenManager`, `PrimitiveDrawing`, `SpriteBatch.Extensions`, `Particles` — HEAD คือ branch `develop` อาจต่างจาก 6.1.1 เล็กน้อย
- เครื่องมือดึงเว็บสรุปผ่านโมเดลย่อย — ตัวเลข/ชื่อสำคัญควรเปิด URL ตรวจซ้ำก่อนตัดสินใจจริง
