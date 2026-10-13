---
type: gdd-class-diagram
version: 2.4
date: 2026-10-13
---

# Class Diagram — BePal Architecture (Prototype)

Project: `BEPAL/Bepal_Game/Bepal/` — MonoGame DesktopGL 3.8.4, .NET 8, 1280x720. ภาพส่วนใหญ่ยังเป็น placeholder ที่วาดจากรูปทรง; sprite จริงมีเฉพาะ parallax 5 ชั้น, `fg_jungle`, `player.png` (สัตว์/ศัตรู/NPC โหลดรูปจริงได้ถ้ามีไฟล์ ไม่งั้น fallback เป็น primitive) · เสียงจริง: SFX 17 WAV + BGM 7 OGG + SpriteFont 3 ตัว (สถานะเต็มดู [05-asset-list.md](05-asset-list.md))

## โครงสร้างโฟลเดอร์

| โฟลเดอร์ | หน้าที่ |
| --- | --- |
| `Core/` | `Gfx` (วาดรูปทรง/ข้อความ/สั่นจอ), `Input` (คีย์บอร์ด/เมาส์), `SceneManager` + `Scene` (stack ของหน้าจอ + fade), `Ui` (`Button`, `Slider`, หลอด, สีกลาง `Palette`), `Settings` (volume เพลง/SFX 0..1 เก็บ `%APPDATA%/BePal/settings.txt`; ไม่บันทึกเมื่อ `--autoplay`/`--shots`), `MusicPlayer` + `Bgm` (BGM `Content/Music/*.ogg` loop + fade `Balance.MusicFadeTime`; scene เลือกเพลงผ่าน `Scene.Music`/`MusicOverride`, `SceneManager.WantedMusic`), `Ease` (`Ease.cs`: easing/damp + `Smoothed` สำหรับ movement polish), `Telemetry` + `RunSeed` (`Telemetry.cs`: log เหตุการณ์ต่อ run เป็น JSONL เมื่อใช้ `--telemetry <dir>`, ไม่ทำอะไรถ้าไม่เปิด; `RunSeed` ทำให้ Random ของเกม reproducible ด้วย `--seed`), `Audio` + `Sfx` (เล่น SFX จาก `Content/Sfx/*.wav`, `Audio.Loop`/`EndFrame` สำหรับเสียงวนเช่นฝีเท้า, `minGap` กันเสียงถี่; เงียบเมื่อ `--autoplay`/`--bot`/`--shots` หรือไม่มี audio device) |
| `Model/` | `Pet` (+ `Species` 4 ชนิดรวม Toothless, `Enemy`), `GameState` + `Balance` (**อยู่ในไฟล์เดียว** `GameState.cs`; ตัวเลขทั้งหมดจาก [06-vertical-slice.md](06-vertical-slice.md)), `Items.cs` (`Item` enum 7 ชิ้น, `ItemInfo`, แคตตาล็อก + การ์ดร้าน/กล่อง) |
| `World/` | `Backdrop` (parallax สไปรต์ 5 ชั้น: sky/clouds/hills/mid/near จาก `Content/Sprites/bg_parallax_*.png`, สเกล 4x, โหลดผ่าน `Backdrop.Load` ใน `Gfx.Init`), foreground `fg_jungle` (สเกล 2x, factor 1.3; layer อื่น 0.03/0.10/0.28/0.48/0.70, กลางคืน = tint), `Art` (ตัวละคร placeholder; `Art.PlayerSprite` = รูปเดียวแทน primitive; `Art.LoadOptional` โหลด `pet_<species>`, `enemy_toothless`, `enemy_bigz`, `npc_merchant`, `npc_doctor` จาก `Content/Sprites` ถ้ามี), `SpriteMotion` (`SpriteMotion.cs`: pose หายใจ/เด้ง/เอียง/ยืดหด/พลิกหันจากความเร็ว สำหรับรูปเดียวไม่มี frame animation), `Rain` (VFX ฝน Day 4: static, tick จาก `SceneManager.Update`, วาดผ่านหน้าต่างใน `BaseScene`, `Rain.Flash()` จาก `DayEvents.Storm`) |
| `Scenes/` | หน้าจอทั้งหมด (ด้านล่าง) |
| `DevTools/` | `SceneCatalog` (รายชื่อ scene + ข้อมูลตัวอย่างทั้งหมด 24 รายการ, `SceneCatalog.Find`), `SceneJumpScene` (F2 ในเกม: `W`/`S`/เมาส์ เลือก, `Space`/คลิก กระโดด, `Esc`/`F2` ปิด; เปิดใน Debug build หรือ `--dev` บน Release; `--scene <name>` เปิดตรง — ตรงชื่อเป๊ะก่อน แล้วค่อย substring ไม่สนตัวพิมพ์ ถ้าไม่เจอจะ list ชื่อทั้งหมด), `ShotRunner` (`--shots <dir>` เซฟภาพทุกรายการใน `SceneCatalog.All` ชื่อไฟล์ = ชื่อใน catalog), `AutoPlay` (`--autoplay [--refuse] [--bot <perfect\|sloppy\|upgrade-first\|careless\|caring\|human>] [--seed n]` — `--bot` อย่างเดียวหมายถึง autoplay; log ที่ `%TEMP%/bepal_autoplay[_profile_seed].log`). Telemetry ตั้งค่าแยก: `--telemetry <dir> [--player <name>]` หรือไฟล์ `telemetry.txt` ข้าง exe (บรรทัดแรก = ชื่อ, log ที่ `<exe dir>/data`); ปิดเมื่อใช้ `--shots`/`--scene`; ดู `BEPAL/Docs/Balance/telemetry-spec.md` |

## Scenes

![scene-flow](diagrams/scene-flow.png)

> Draw.io: [diagrams/scene-flow.drawio](diagrams/scene-flow.drawio) (เปิดด้วย VS Code Draw.io Integration)

| Class | Figma Scene |
| --- | --- |
| `MainMenuScene` | 1 |
| `ChooseStarterScene` + `PetPickScene` | 2, 17 |
| `BaseScene` (side-scroller, A/D + Space, `Shift` วิ่ง, `Esc` Pause, `F3` overlay) | 3 |
| `CareSelectScene` | 4 |
| `QteScene` (`Care.Train/Feed/Clean/Heal`) | 5–8 |
| `NotebookScene` | 9–11 |
| `UpgradeScene` | 12 |
| `DoctorScene` | 13 |
| `DayEvents` (Day 2 Toothless · 3 พ่อค้า · 4 Storm · 5 Big Z) + `DialogueScene` / `ChoiceScene` (`Line`, `Option` record อยู่ใน `DialogScenes.cs`) | 14–16, 19–20 |
| `FightScene` | 18 |
| `Wheel` (`Wheel.cs`: เข็มหมุน + `Zone`/`Hit`/`Popups`, ใช้ร่วมใน CareSelect / Qte / Fight; `Evaluate()` → `Hit.Miss/Great/Perfect`) | 4–8, 18 |
| `ChoiceScene` "Paused" (Esc ใน `BaseScene`: Resume / Options / Main Menu) · `OptionsScene` (ปรับ volume เพลง/SFX, เปิดจาก Main Menu และ Pause) | เพิ่มใน slice |
| `ShopScene`, `InventoryScene` (กล่องไอเทมในฐาน, `Kind.Box`; แคตตาล็อกอยู่ `Model/Items.cs`), `NightScene`, `GameOverScene`, `ToBeContinuedScene` | เพิ่มใน slice |

## หลักการ

- **Scene stack:** หน้าต่างซ้อน (`Overlay = true`) วาดทับฉากด้านล่าง แต่มีแค่ scene บนสุดที่ `Update` — เปิดหน้าใหม่ด้วย `M.Push`, ปิดด้วย `M.Remove(this)` แล้วค่อยเรียก callback
- **ตัวเลขอยู่ที่ `Balance` ที่เดียว** — ปรับบาลานซ์ได้โดยไม่ต้องแก้ logic
- **Passive ของสัตว์:** `Pet.Passive` (ข้อความ) + `Pet.PassiveUnlocked` (Lv ≥ `Balance.PassiveLevel` = 2); ผลทำงานใน `FightScene` — Mossling heal 1% MaxHp/attack, Toothless พิษ (ศัตรูรับ +25% / ตี −25%, ไม่ stack), Blinkbun วาร์ปเข็มไป 12 นาฬิกา; Nibbleclaw combo (attack โดนติดกัน +50% ของดาเมจฐานต่อครั้ง รีเซ็ตเมื่อกดพลาด/เปลี่ยนตัว)
- **Lv3:** แสดงข้อความ "learned a new move!" (coming soon) — ยังไม่มีกลไกท่าใหม่
