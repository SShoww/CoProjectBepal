# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

BePal — a university team's MonoGame prototype (virtual-pet care + survival, "Endless Survival Management"). The repo is also an Obsidian vault (`.obsidian/`); design docs are Markdown, mostly in Thai.

- Game code: `BEPAL/Bepal_Game/Bepal/` (MonoGame DesktopGL 3.8.4, .NET 8, C# with `Nullable` enabled, 1280x720)
- Design docs: `BEPAL/Docs/GDD/` (index in `README.md`), Agile docs in `BEPAL/Docs/Agile/`, templates in `BEPAL/Template/`
- **Scope authority:** `BEPAL/Docs/GDD/06-vertical-slice.md` defines the prototype (Day 1–5 slice) and overrides the other GDD files. Code structure is documented in `04-class-diagram.md` — keep it in sync when adding/renaming scenes or folders.
- The Figma board (linked in the GDD README) is the upstream design source; where it conflicts with older GDD text, Figma wins (conflicts listed in the README changelog).

## Commands

Run from `BEPAL/Bepal_Game/Bepal/` (the first build runs `dotnet tool restore` automatically to fetch `mgcb`):

```sh
dotnet build
dotnet run
dotnet run -- --shots <dir>          # render every screen to <dir>/*.png, then exit (layout check without clicking)
dotnet run -- --autoplay             # bot plays menu -> Day 5, writes %TEMP%/bepal_autoplay.log (prints path), then exits
dotnet run -- --autoplay --refuse    # same, but refuses the Day 3 merchant and visits the shop
dotnet mgcb-editor                   # edit Content/Content.mgcb
```

There is no unit-test project. Verification = build, then `--autoplay` (look for `RESULT: reached To be continued`; `TIMEOUT` means a soft-lock) and `--shots` (inspect PNGs) after UI changes.

## Architecture

- **Entry:** `Game1` sets up `Gfx`, starts `MainMenuScene`, and switches into `ShotRunner`/`AutoPlay` mode based on CLI args. `AutoPlay` runs 60 simulated frames per real frame with injected input.
- **Scene stack (`Core/SceneManager.cs`):** only the top scene `Update`s; scenes with `Overlay => true` draw on top of the scenes beneath them. Open with `M.Push(scene)`; close with `M.Remove(this)` then invoke the callback; full-screen changes use `M.Reset(scene, overlays...)` (fade). `Push`/`Pop`/`Remove` call `Input.Consume()` so key presses don't leak into the next scene.
- **No image assets:** everything is drawn from primitives via `Gfx` (rects, ellipses, arcs, glow, text, screen shake) and `World/Art.cs` (placeholder characters). Only content is three SpriteFonts (`Fonts/Main|Big|Small`). Colors live in `Palette` (`Core/Ui.cs`).
- **Model:** `GameState` holds day/coin/energy/player level/pets/upgrades and `AdvanceDay()` (end-of-day decay/starvation). **All tuning numbers go in `Balance` (`Model/GameState.cs`)**, mirroring 06-vertical-slice.md — don't hard-code numbers in scenes.
- **Base hub:** `BaseScene` is a side-scroller (A/D + Space) with interaction spots at fixed X positions (`BedX`, `DoorX`, …, enum `BaseScene.Kind`). Bed → end day; pets → `CareSelectScene` → `QteScene` (`Care.Train/Feed/Clean/Heal`); door → `DayEvents.OpenDoor`, which scripts fixed events per day (2 Toothless, 3 Merchant, 4 Storm, 5 BigZ boss) using `DialogueScene`/`ChoiceScene`/`FightScene`.
- **Wheel QTE (`Scenes/Wheel.cs`):** shared rotating-needle mechanic used by care select, care QTEs and fights; `Zone` has Perfect/Great half-widths in radians, angle 0 = top, clockwise. `Evaluate()` returns `Hit.Miss/Great/Perfect`.
- **Dev tools coupling:** `AutoPlay` pattern-matches on scene types and public fields (`Wheel`, `ChoiceScene.Prompt` prefixes like `"Toothless is at"`/`"Sell"`, `BaseScene.Near/PlayerX/PetX`, `GameState.DoorEventPending`), and `ShotRunner` constructs scenes directly. Renaming scenes, prompt text, or these members requires updating `DevTools/`.
