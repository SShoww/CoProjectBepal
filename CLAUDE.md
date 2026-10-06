# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

**AI workflow first:** follow [AGENTS.md](AGENTS.md) (language, token rules, multi-agent) and start every session from [README.md](README.md) → `Context/`. This file covers the game code only.

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

Folders under `BEPAL/Bepal_Game/Bepal/` (namespace `Bepal` everywhere, no sub-namespaces):

- `Core/` — `Gfx`, `Input`, `SceneManager` (+ abstract `Scene`), `Ui` (`Palette`, `Button`, `Ui` helpers)
- `Model/` — `GameState` + `Balance`, `Pet` (+ `Species`, `Enemy`)
- `Scenes/` — `MenuScenes` (MainMenu, ChooseStarter, GameOver, ToBeContinued, Night), `BaseScene`, `BaseMenus` (Upgrade, Doctor, Notebook, Shop), `CareScenes` (CareSelect, Qte), `FightScene`, `DialogScenes` (Dialogue, Choice, PetPick), `DayEvents`, `Wheel`
- `World/` — `Art` (procedural characters/merchant/enemies), `Backdrop` (base background)
- `DevTools/` — `AutoPlay`, `ShotRunner`; `Content/Fonts/` — the three SpriteFonts

- **Entry:** `Game1` sets up `Gfx`, starts `MainMenuScene`, and switches into `ShotRunner`/`AutoPlay` mode based on CLI args (`--shots <dir>`, `--autoplay`, `--refuse`). In `--autoplay` it runs 60 simulated frames (fixed 1/60 s dt) per real frame with injected input (`Input.Inject`), so frame-count timing differs from normal play.
- **Scene stack (`Core/SceneManager.cs`):** only the top scene `Update`s; scenes with `Overlay => true` draw on top of the scenes beneath them. Open with `M.Push(scene)`; close with `M.Remove(this)` then invoke the callback; full-screen changes use `M.Reset(scene, overlays...)` (fade). `Push`/`Pop`/`Remove` call `Input.Consume()` so key presses don't leak into the next scene.
- **No image assets:** everything is drawn from primitives via `Gfx` (rects, ellipses, arcs, glow, text, screen shake) and `World/Art.cs` (placeholder characters). `Gfx.Init` generates its own `Pixel`/`Disc`/`Soft` textures at runtime. Only content is three SpriteFonts (`Fonts/Main|Big|Small` → `Gfx.Font|Big|Small`). Colors live in `Palette` (`Core/Ui.cs`). Input goes through the static `Input` class (`Confirm` = Space/Enter, `Back` = Esc, `Pressed`/`Click` are edge-triggered).
- **Model:** `GameState` holds day/coin/energy/player level+exp/skill points/pets/upgrades (`QteUpgrade`, `EnergyUpgrade`, `ProgressUpgrade`) and `AdvanceDay()` (end-of-day starvation, then next-morning decay). `Pet` stats are Hp/Stomach/Clean/Progress/Level; low Clean lowers MaxHp/Atk. `Pet.Create(Species)` and `Enemy.Toothless()/BigZ()` hold per-creature numbers. **All tuning numbers go in `Balance` (`Model/GameState.cs`)**, mirroring 06-vertical-slice.md — don't hard-code numbers in scenes.
- **Base hub:** `BaseScene` is a side-scroller (A/D + Space) over a 3840-wide world with interaction spots at fixed X positions (`BedX`, `UpgradeX`, `HearthX`, `DoctorX`, `BookX`, `DoorX`; enum `BaseScene.Kind { Bed, Upgrade, Doctor, Book, Door, Pet }`; pets wander and are found via `PetX(pet)`). Bed → end day (`NightScene` drives `BaseScene.Night`); Upgrade → `UpgradeScene`; Doctor → `DoctorScene` (revive); Book → `NotebookScene`; pets → `CareSelectScene` → `QteScene` (`Care.Train/Feed/Clean/Heal`, 1 Energy each); door → `DayEvents.OpenDoor`, which scripts fixed events per day (2 Toothless, 3 Merchant → `ShopScene` if refused, 4 Storm, 5 BigZ boss → `ToBeContinuedScene`) using `DialogueScene`/`ChoiceScene`/`PetPickScene`/`FightScene`. All pets dead → `GameOverScene`.
- **Wheel QTE (`Scenes/Wheel.cs`):** shared rotating-needle mechanic used by care select, care QTEs and fights; `Zone` has Perfect/Great half-widths in radians, angle 0 = top, clockwise. `Evaluate()` returns `Hit.Miss/Great/Perfect`.
- **Dev tools coupling:** `AutoPlay` pattern-matches on scene types (`MainMenuScene`, `DialogueScene`, `ChoiceScene`, `PetPickScene`, `ShopScene`, `DoctorScene`, `CareSelectScene`, `QteScene`, `FightScene`, `BaseScene`, `ToBeContinuedScene`, `GameOverScene`) and public members (`Wheel`, `ChoiceScene.Prompt` prefixes like `"Toothless is at"`/`"Sell"`, `BaseScene.Near/PlayerX/PetX/State`, `BaseScene.DoorX/DoctorX/BedX`, `GameState.DoorEventPending`). It ends with `RESULT: reached To be continued`, `RESULT: game over`, or `RESULT: TIMEOUT` after 30 simulated minutes. `ShotRunner` constructs scenes directly (constructor signatures, `BaseScene.StartX/Night`) and has one entry per screen — add a shot when adding a scene. Renaming scenes, prompt text, or these members requires updating `DevTools/`.
