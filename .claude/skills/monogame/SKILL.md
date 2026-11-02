---
name: monogame
description: Work on the BePal MonoGame (DesktopGL, .NET 8) prototype — adding or changing scenes, UI, QTE/wheel mechanics, balance numbers, fonts/content, and verifying changes with the build, --shots and --autoplay. Use whenever editing code under BEPAL/Bepal_Game/Bepal/ or when asked about MonoGame patterns in this repo.
---

# MonoGame workflow for BePal

Project root for code: `BEPAL/Bepal_Game/Bepal/` (MonoGame DesktopGL 3.8.4, .NET 8, `Nullable` enabled, 1280x720). Run all commands from there.

## Before changing anything
1. Read `BEPAL/Docs/GDD/06-vertical-slice.md` — it defines prototype scope (Day 1–5) and overrides other GDD files.
2. Look at a neighbouring scene/file and match its style (naming, comment density, how it uses `Gfx`, `Palette`, `Input`).

## Conventions (do not break)
- **Almost no image assets.** Draw with `Gfx` primitives (rects, ellipses, arcs, glow, text, shake) and `World/Art.cs`. Exception: the parallax/foreground PNGs in `Content/Sprites/` (`World/Backdrop.cs`, pixel art, `PointClamp` batch). New textures go in `Content/Sprites/` and must be registered in `Content.mgcb` with `TextureImporter`/`TextureProcessor`. Other content: SpriteFonts `Fonts/Main|Big|Small` and the SFX. Colors come from `Palette` (`Core/Ui.cs`).
- **Tuning numbers live in `Balance`** (`Model/GameState.cs`), mirroring 06-vertical-slice.md. Never hard-code gameplay numbers in scenes.
- **Scenes** go through `Core/SceneManager.cs`: only the top scene `Update`s; `Overlay => true` scenes draw over those beneath. Open with `M.Push(scene)`; close with `M.Remove(this)` then call the callback; full-screen changes use `M.Reset(scene, overlays...)`. Push/Pop/Remove call `Input.Consume()` — don't re-add your own consume to avoid key leaks.
- **Wheel QTE** (`Scenes/Wheel.cs`) is shared by care select, care QTEs and fights. Angle 0 = top, clockwise, zones in radians; `Evaluate()` → `Hit.Miss/Great/Perfect`. Reuse it rather than writing a new timing mechanic.
- **Dev-tools coupling:** `DevTools/AutoPlay` matches scene types and public members (`Wheel`, `ChoiceScene.Prompt` prefixes like `"Toothless is at"`/`"Sell"`, `BaseScene.Near/PlayerX/PetX`, `GameState.DoorEventPending`); `ShotRunner` constructs scenes directly. If you rename a scene, prompt text or those members, update `DevTools/` in the same change.
- **Docs sync:** when adding/renaming scenes or folders, update `BEPAL/Docs/GDD/04-class-diagram.md`.

## Adding a new scene (checklist)
1. Create the scene class next to similar ones in `Scenes/`; implement `Update`/`Draw`, set `Overlay` if it should sit on top.
2. Open it via `M.Push` / `M.Reset`; on exit use `M.Remove(this)` then the callback.
3. Add it to `ShotRunner` so `--shots` renders it.
4. If the bot must pass through it, teach `AutoPlay` how to drive it (otherwise it soft-locks → `TIMEOUT`).
5. Update `04-class-diagram.md`.

## Adding fonts/content
Edit `Content/Content.mgcb` with `dotnet mgcb-editor` (tool is restored on first build). Keep font names `Main|Big|Small` stable; check Thai glyph coverage if text may be Thai.

## Verification (there is no unit-test project)
```sh
dotnet build
dotnet run -- --shots <dir>     # inspect the PNGs after any UI change
dotnet run -- --autoplay        # want: RESULT: reached To be continued (TIMEOUT = soft-lock)
dotnet run -- --autoplay --refuse   # Day 3 refuse-merchant + shop path
```
The autoplay log is written to `%TEMP%/bepal_autoplay.log`. Run both autoplay variants after touching flow, scenes, QTE or day events. Report honestly if a check fails or was skipped.

## Common MonoGame pitfalls
- Don't allocate (new textures, lists, strings) per frame in `Draw`/`Update`; cache and reuse.
- Use `gameTime.ElapsedGameTime` for timing, not frame counts — autoplay runs 60 simulated frames per real frame, so frame-based logic behaves differently there.
- Edge-detect input (previous vs current state); held keys otherwise fire every frame.
- Always `SpriteBatch.End()` what you `Begin()`; mismatched batches throw at runtime, not build time.
- Dispose of `Texture2D`/`RenderTarget2D` you create manually.
