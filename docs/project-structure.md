# Project Structure

> **Status:** Proposal for team agreement — input to Phase-001 Task 1.1 ("Team agrees on project folder structure and naming conventions")
> **Version:** 0.33
> **Last updated:** 2026-11-21
> **Purpose:** The repository layout, Godot folder structure, naming rules, input/autoload registries, asset pipeline, and verification commands that every development session follows when adding code or assets under `src/`. It is a *technical* contract only — it never overrides a gameplay decision.

---

## 1. Precedence — what wins when documents disagree

| Question | Authoritative doc |
|---|---|
| Gameplay, audience, design rules | [`GDD.md`](./GDD.md) |
| What to build this phase, task by task | [`Phase-001.md`](./Phase-001.md) |
| Control mappings (what a button *means*) | [`controls.md`](./controls.md) |
| **Where a file lives, what it is called, how it is verified** | **this document** |
| How the AI runs a session | [`../AGENTS.md`](../AGENTS.md) |

If this document and a phase task disagree about *structure*, this document wins and the phase task's Notes record the deviation. If they disagree about *scope or gameplay*, the phase task / GDD wins and this document is updated.

This document is **not** a substitute for Task 1.1: it supplies the agreement that Task 1.1 then implements (project init, `.gitignore`, first three scenes, placeholder art, first green run).

---

## 2. Toolchain baseline (verified 2026-11-19, Task 1.1)

| Item | State | Needed for |
|---|---|---|
| Godot project | `src/project.godot`, features `4.7` + `GL Compatibility`, renderer `gl_compatibility`, assembly **`MyFarmingGame`** (set 2026-11-19) | — |
| Godot **.NET** editor binary | ✅ **4.7.2.stable.mono** at `/Applications/Godot_mono.app/Contents/MacOS/Godot` (not on `PATH`; `tools/verify.sh` finds it, or set `GODOT=`) | everything |
| .NET SDK | ✅ `11.0.100` (runtime 11.0.0 only). `.csproj` targets `net8.0` with `RollForward=LatestMajor`, so it runs on .NET 8–11 | `dotnet build` |
| `.csproj` / `.sln` / `*.cs` | ✅ `src/MyFarmingGame.csproj` (`Godot.NET.Sdk/4.7.2`); `.sln` is created by the editor on first open — commit it then | `dotnet build` (§14.2)
| Git | ✅ repo initialised; `src/` committed in `9d49b65` | version control |
| AI skills | ✅ 17 Godot/game-dev skills installed, see §12 | every session |

**Nothing in this table may be papered over with a fake binary or stub project.** Missing toolchain = stop and request it (AGENTS.md §5).

---

## 3. Repository layout

```text
my-farming-game/
├── AGENTS.md              # how an AI session runs (process)
├── README.md              # project hub + documentation table
├── docs/                  # ALL design documents (never code)
│   ├── GDD.md, world.md, characters.md, places.md, crops.md, animals.md,
│   │   items.md, inventory.md, mechanics.md, controls.md, events.md,
│   │   progression.md, challenge-layers.md
│   ├── PHASE.md           # roadmap
│   ├── Phase-001.md       # current phase task list
│   ├── project-structure.md   # THIS document
│   └── assets/            # README screenshots and design images (map/bramblewick-vale.svg) — NOT game assets
├── src/                   # the Godot project root; res:// == src/
├── .agents/skills/        # installed AI skills (gitignored, restorable)
├── skills-lock.json       # pinned skill versions (committed)
├── tools/                 # repo-level helper scripts — verify.sh (Task 1.1)
└── .gitignore             # repo-wide ignores
```

Rules:

- **Design docs never live in `src/`; game files never live in `docs/`.**
- `res://` always means `src/` — never write `res://src/...`.
- Helper shell scripts that are not part of the game go in `tools/`, not in `src/`.

---

## 4. Godot project structure (`src/`)

```text
src/
├── project.godot              # Godot config: window, autoloads (§8), InputMap (§7)
├── MyFarmingGame.csproj, .sln # .sln added 2026-11-21 (C# export needs it) — committed
│                              # main scene: res://scenes/main/Main.tscn (set in Task 1.1)
├── export_presets.cfg         # macOS preset (2026-11-21, §10)
│
├── scenes/                    # every .tscn, grouped by where it is used (3D since Phase-001 Tasks 1.14–1.18)
│   ├── main/
│   │   └── Main.tscn          # root: Systems (SleepSequence, SaveBinder, OnboardingDirector) · World (Node3D: Level, Entities = Player + CameraRig, Effects = WorldEnvironment + Sun + DayNight) · Audio · HUD/Pause/Transition/Debug CanvasLayers
│   ├── player/
│   │   └── Player.tscn        # Kit: CharacterBody3D + capsule collider + NamedModel "kit" (child "Model"; the held tool "Hand" is added in code)
│   ├── world/                 # the level and everything Kit walks up to (3D)
│   │   ├── FarmLevel.tscn     # farm + lane + seed shed: ground/fields/fence built by FarmLevel.cs, Buildings (house, coop, barn, shed, cottage), FrontGarden (FarmGrid), props, markers (PlayerSpawn, CameraLimitMin/Max)
│   │   ├── Bed.tscn, Marlow.tscn, Ada.tscn, Ledger.tscn, BerryBush.tscn, OrderBoard.tscn, ShopCounter.tscn, Phone.tscn
│   │   │                      # one-node Prop scenes (StaticBody3D + script; the Prop base builds look, body, click area, prompt)
│   │   ├── Hen.tscn, Cow.tscn # FarmAnimal props spawned by AnimalHouse (feed bin and trough slots are built in code)
│   │   └── Tree.tscn          # KayKit tree template (real model)
│   └── ui/                    # everything drawn on the UI CanvasLayers (unchanged by the 3D switch)
│       ├── UI.tscn            # HUD root: ClockDial, SleepPrompt, WalletHud, ToolBelt, Bag, PictureBubble, OrderBoard, Shop, Nudge, StandingOrders, CareNotices, pause button
│       ├── ClockDial.tscn, SleepPrompt.tscn, DayTransition.tscn, ToolBelt.tscn, Bag.tscn, WalletHud.tscn
│       ├── OrderBoard.tscn, Shop.tscn, PictureBubble.tscn, Nudge.tscn
│       └── PauseMenu.tscn     # instanced into Main/Pause
│
├── scripts/                   # every .cs, grouped by the layer it belongs to
│   ├── autoloads/             # GameClock, ToolBelt, Inventory, Wallet, OrderManager, SaveGame (schema v2), Onboarding — §8
│   ├── player/
│   │   └── PlayerController.cs   # Kit: screen-relative 8-way walk, 4-way grid Facing, click-to-move by camera ray, WalkTo, tool use, held tool + use motion
│   ├── world/
│   │   ├── FarmLevel.cs       # layout tables (tiles = metres) → baked ground/fence/hedge meshes, tuft MultiMesh, blockers
│   │   ├── Building.cs (+ Fader), IndoorZone.cs   # walls, floor, roof model, window; roof/camera-side wall fade inside and when hiding Kit
│   │   ├── FarmGrid.cs, CropTile.cs   # garden rules + MultiMesh tiles, front-tile frame, hint/sparkle pins, action puffs
│   │   ├── Prop.cs            # 3D base for usable things (IInteractable, NamedModel look, click Area3D layer 8, WorldPin prompt)
│   │   ├── Bed, Marlow, Ada, Ledger, BerryBush, OrderBoard, ShopCounter, Phone, FeedBin, TroughSlot (.cs)
│   │   ├── AnimalHouse.cs, FarmAnimal.cs (+ HeartBar), Hen.cs, Cow.cs   # animals, feed bin, trough slots, loss rule
│   │   └── GuideArrow.cs      # guided-start arrow (WorldPin)
│   ├── ui/                    # views (state lives in the autoloads); DayNightView.cs drives the 3D sun/sky/windows
│   ├── main/
│   │   ├── Main.cs            # coordinator: layers, spawn + camera limits from markers, menus freeze Kit, debug keys
│   │   ├── CameraRig.cs       # fixed diagonal camera (45° / 45° / south-east), corner switch, presets
│   │   └── SleepSequence.cs, SaveBinder.cs, OnboardingDirector.cs
│   └── core/                  # shared helpers and data
│       ├── UILayers.cs, IInteractable.cs, IToolTarget.cs
│       ├── ModelLibrary.cs, NamedModel.cs   # 3D load-by-name; placeholders baked into one vertex-coloured mesh (§9, §16)
│       ├── WorldPin.cs, Puff.cs             # screen UI pinned to 3D points; one-shot action puffs
│       ├── SpriteLibrary.cs                 # 2D pictures by name (HUD, bubbles, pins)
│       └── CropDefinition.cs, CropCatalog.cs, PriceTable.cs, Order.cs, Neighbours.cs, GameSettings.cs, Production.cs, FeedStore.cs
│
├── assets/                    # ALL game art/audio/fonts, imported by Godot
│   ├── art/
│   │   ├── characters/        # Kit, Marlow
│   │   ├── world/             # garden, lane, shed, beds
│   │   ├── ui/                # tool icons, bubbles, shelf, menu
│   │   ├── items/             # seeds, flowers, net, ledger pages
│   │   └── vfx/               # sparkle, firefly, butterfly frames
│   ├── audio/
│   │   ├── music/
│   │   └── sfx/
│   ├── fonts/
│   ├── tiles/                 # TileSet sources for garden/lane (Task 1.4+)
│   ├── placeholder/           # Task 1.1 — TEMPORARY art only (README inside)
│   │   ├── kenney/            # ph_<sprite name>.png from Kenney CC0 packs + their License.txt
│   │   └── handmade/          # Task 1.3 — ph_ icons drawn for this project (bed, clock_sun/moon, icon_sleep, season_spring)
│   └── LICENSES.md            # every third-party asset (placeholders included): source + license (§9)
│
├── data/                      # data files loaded at runtime: .tres / .json
│   ├── crops.tres             # CropCatalog (carrot, Sun Sprout); crops.md is the source
│   ├── prices.json            # generated from docs/data/economy.csv by tools/make_prices.py
│   └── placeholder_models.json  # placeholder 3D models by name (§9)
├── shaders/                   # .gdshader files — snake_case.gdshader (§6)
├── debug/                     # debug-only scenes/scripts — never needed by the game in release
│   ├── ClockDebugKeys.cs, BagDebugKeys.cs, CameraDebugKeys.cs   # F2–F4 clock, F6–F8 bag/wallet, 1–4 / F9–F12 camera presets
│   └── PerfCheck.tscn + PerfCheck.cs                              # windowed frame budget (tools/perf_check.sh)
├── tests/                     # headless, repeatable, leave no state behind (§11)
│   ├── TestRunner.tscn, TestRunner.cs   # groups `rules` and `game`
│   └── FarmTests, LaneTests, OrderTests, ShopTests, AnimalTests, SaveTests, PauseTests, OnboardingTests, Tests3D (.cs)
├── addons/                    # third-party plugins (GDScript-only addons flagged in Notes)
└── .godot/                    # generated — gitignored, never committed
```

> **3D since Phase-001 Tasks 1.14–1.18 (owner decision 2026-11-21).** The tree above is the 3D project. Deleted at the 1.15 cut-over: `CameraFollow.cs`, `FarmGround.cs`, `NamedSprite.cs`, `TestGround3D`, `Main3D`/`Player3D` (renamed), `scenes/ui/DayNightOverlay.tscn`, `shaders/placeholder_checker.gdshader`.

> **Configured in Task 1.1:** `application/run/main_scene` = `res://scenes/main/Main.tscn`; window 1280×720; default canvas texture filter Nearest (for the pixel placeholders — revisit with final art). Godot 4.7 writes `*.cs.uid` sidecars next to scripts — commit them.

### Structural rules (apply to every task)

1. **Type-first folders:** scenes go in `scenes/<context>/`, scripts in `scripts/<layer>/`. Do not scatter loose `.tscn`/`.cs` at `src/` root (generated project files excepted).
2. **One primary node class per scene**, and the scene file name equals its root node name (`FarmGrid.tscn` → root node `FarmGrid`).
3. **Script file name equals the C# class name** (`FarmGrid.cs` → `class FarmGrid : Node3D`).
4. **Logic lives in `.cs`, layout lives in `.tscn`.** Never hard-code a node path that a scene rename would break without also updating the script; prefer `GetNode<T>("%UniqueName")` with owner-unique names for cross-scene lookups.
5. **Scenes never reference each other directly.** Cross-scene communication goes through signals / the `EventBus` (§8) or an autoload API (§8). A scene may only be *instanced* by its parent context (`Main.tscn` instances world + UI).
6. **Autoloads are reserved** for genuinely cross-scene, session-persistent state — exactly the list in §8. Anything else is a normal node.
7. **Input is declared in the InputMap** (§7), never as raw keycodes/mouse buttons in script.
8. **Save data is written only by `SaveGame.cs`** to `user://savegame.json` (one slot, auto-save, human-readable JSON). No other script touches `user://`.
9. **New folder or new autoload = update this document in the same session.**
10. **Scene architecture is fixed:** `Main.tscn`'s tree, layer numbers, process modes, and level swapping follow the standard below. A new layer or a process-mode exception = update that table in the same session.

### Scene architecture standard (Main ↔ Level) — agreed 2026-11-20

Every task that creates or touches `Main.tscn` or a level scene follows this layout. `Main` is the single coordinator: nothing else reparents the player, camera, or UI layers.

```text
Main (Node)                            process_mode = Always
├── Systems (Node)                     # in-tree systems that are not autoloads (§8)
├── World (Node3D)                     process_mode = Pausable
│   ├── Level (Node)                   # exactly one level instance lives here (load_level, below)
│   ├── Entities (Node3D)              # Player, CameraRig (Camera3D) — survive level swaps
│   └── Effects (Node3D)               # WorldEnvironment, Sun, DayNight, guide arrow — not owned by a level
├── Audio (Node)                       # music + SFX players; buses Master / Music / SFX
├── HUD (CanvasLayer, layer 10)        process_mode = Pausable
├── Pause (CanvasLayer, layer 20)      process_mode = WhenPaused
├── Transition (CanvasLayer, layer 30) process_mode = Always
└── Debug (CanvasLayer, layer 40)      process_mode = Always
```

**Ownership rules**

1. **Main owns** the layer stack, the player, the camera, `Audio`, and scene transitions.
2. **A level scene owns only level-local data:** geometry (`TileMapLayer`s), placed interactables, environment, and `Marker2D`s (`PlayerSpawn`, `CameraLimits` — PascalCase). A level **never** contains the player, camera, or UI — it exposes markers instead.
3. **World is a plain `Node3D`, not a CanvasLayer** (2D until Phase-001 Task 1.15). CanvasLayers are reserved for UI stacking. Layer numbers come from `scripts/core/UILayers.cs` — never hand-type a `layer = N` anywhere else.
4. **UI roots:** `UI.tscn` instances into `HUD`; `PauseMenu.tscn` into `Pause`; fades/wipes into `Transition`; debug tooling into `Debug` (debug-only files live in `src/debug/`).

**Layer constants** (`scripts/core/UILayers.cs`)

| Constant | Value | Contents |
|---|---|---|
| `Hud` | 10 | clock dial, tool belt, bag, picture bubbles, nudges |
| `PauseMenu` | 20 | pause menu |
| `Transition` | 30 | day fades, scene wipes |
| `Debug` | 40 | debug overlays |

World is the 3D scene; pinned UI (`WorldPin`) draws on the world canvas below the HUD layers.

> **3D (since the Task 1.15 cut-over):** `World`, `World/Entities` and `World/Effects` are `Node3D`; the `Camera3D` lives in `World/Entities/CameraRig`; the sun and `WorldEnvironment` live in `World/Effects`; levels expose `Marker3D`s (`PlayerSpawn`, `CameraLimits` as two corner markers). **Units: 1 tile = 1 m; the ground is the XZ plane at y = 0; map north is −Z.** The camera is diagonal (turned roughly 45°, exact value picked in Task 1.14; owner 2026-11-21), so screen-up is not map north: input is turned by the camera's yaw (`CameraRig` exposes it) before moving Kit, and the tool front snaps to the nearest grid axis. A 2D tile `(x, y)` maps to the 3D tile centre `(x + 0.5, 0, y + 0.5)`. UI CanvasLayers and their numbers do not change. The 2D draw-order gotcha below no longer applies in 3D (depth sorting is automatic).

> **2D-era note (Task 1.1, no longer applies in 3D):** because `World/Level` is a plain `Node`, a level's root `Node2D` is a separate top-level canvas item drawn after `World/Entities` and will cover the player. Give level ground art (backgrounds, ground `TileMapLayer`s) a negative `z_index`; `TestRunner` checks this for the placeholder level.

**Process modes — set these explicitly**

A node's default is `Inherit`, and **autoloads inherit from the root (i.e. `Always`)** — so without explicit settings, `GetTree().Paused = true` freezes nothing that matters and the clock keeps ticking during pause. Set the mode in the node's `_Ready()` (`ProcessMode = ProcessMode.Pausable;`) or in the scene file:

| Node / autoload | Process mode | Why |
|---|---|---|
| `Main` | Always | coordinator survives pause |
| `World` (→ Level, Entities, Effects) | Pausable | the world freezes on pause |
| `HUD` | Pausable | clock/HUD freeze with the world |
| `Pause` | WhenPaused | only runs while paused |
| `Transition`, `Debug`, `Audio` | Always | fades and music never break |
| Autoloads `GameClock`, `OrderManager`, `Onboarding` | Pausable | day-driven logic must stop with the game |
| Autoloads `ToolBelt`, `Inventory`, `Wallet` | Pausable (state is event-driven; belt-and-braces) | matters only if `_Process` is ever used |
| Autoloads `SaveGame`, `EventBus` | Always | saving and event routing must work during pause/transitions |

**`load_level` contract** — implemented once in `Main`; the only way levels change:

1. Defer the swap (`CallDeferred`) — never free or reparent inside a physics/input callback.
2. `queue_free()` the current child of `World/Level` **only** — player, camera, entities, audio, and UI stay.
3. Instantiate the new level scene and add it under `World/Level`.
4. Read the level's `PlayerSpawn` marker → move the player; apply `CameraLimitMin/Max` to the shared `CameraRig`.
5. Emit `LevelChanged(newLevelName)` so UI/save code reacts. Outside code calls `load_level` or listens for the signal — it never reaches into `Main` directly.

---

## 5. Task → file map (Phase-001)

Quick lookup so a session knows where its artifact belongs. Scope still comes from `Phase-001.md`.

| Task | Creates (under `src/`) |
|---|---|
| 1.1 Project Foundation | `scenes/main/Main.tscn`, `scenes/player/Player.tscn`, `scenes/ui/UI.tscn`, placeholder art (§9) |
| 1.2 Player Movement | `scripts/player/PlayerController.cs`, InputMap `move_*`, `interact`, `cancel` (§7) |
| 1.3 Day Clock & Sleep | `scripts/autoloads/GameClock.cs`, `scenes/ui/ClockDial.tscn` + `SleepPrompt.tscn` + `DayTransition.tscn`, `scripts/ui/{ClockDialView,SleepPromptView,DayTransitionView,ButtonPrompt}.cs`, `scenes/world/Bed.tscn` + `scripts/world/Bed.cs`, `scripts/core/IInteractable.cs`, `scripts/main/SleepSequence.cs`, `debug/ClockDebugKeys.cs` ✅ built |
| 1.4 Tool Belt, Bag & Wallet | `scripts/autoloads/ToolBelt.cs` + `Inventory.cs` + `Wallet.cs`, `scenes/ui/{ToolBelt,Bag,WalletHud}.tscn`, `scripts/ui/{ToolBeltView,BagView,WalletView}.cs`, `debug/BagDebugKeys.cs`, actions `cycle_tool_*`, `open_inventory`, `ui_accept`/`ui_cancel` pad buttons ✅ built |
| 1.5 Farm Level, Clearing, Planting & Growth | `scenes/world/FarmLevel.tscn`, `scripts/world/{FarmLevel,FarmGround,FarmGrid,CropTile,Building,IndoorZone}.cs`, `scripts/core/{IToolTarget,CropDefinition,CropCatalog}.cs`, `data/crops.tres`, `tools/make_placeholders.py`; Bed moved into the house; no separate `FarmGrid.tscn`/`IndoorZone.tscn` (FarmGrid is a node in FarmLevel, Building creates its IndoorZone) ✅ built |
| 1.6 Marlow & First Order | `scenes/world/{Marlow,Ledger,BerryBush}.tscn`, `scripts/world/{Prop,Marlow,Ledger,BerryBush}.cs`, `scenes/ui/PictureBubble.tscn` + `scripts/ui/PictureBubbleView.cs`, `scripts/autoloads/OrderManager.cs` (moved up from 1.7), `scripts/core/{Order,PriceTable}.cs`, `data/prices.json` + `tools/make_prices.py`; lane, seed shed and cottage added to `FarmLevel` ✅ built |
| 1.7 Order Board | `scenes/world/OrderBoard.tscn` + `scripts/world/OrderBoard.cs`, `scenes/ui/OrderBoard.tscn` + `scripts/ui/OrderBoardView.cs`, `scripts/core/Production.cs`; generator + repeats in `OrderManager` ✅ built |
| 1.8 Seed Shop & Phone | `scenes/ui/Shop.tscn` + `scripts/ui/ShopView.cs`, `scenes/world/{ShopCounter,Phone}.tscn` + `scripts/world/{ShopCounter,Phone,AnimalHouse,FeedBin}.cs`, `scripts/core/FeedStore.cs`; prices already from `data/prices.json` (Task 1.6); Sun Sprout in `crops.tres` ✅ built |
| 1.9 Hens, Cow & Loss Rule | `scenes/world/{Hen,Cow}.tscn`, `scripts/world/{FarmAnimal,Hen,Cow}.cs`, feeding/products/loss in `AnimalHouse.cs`, `scripts/ui/CareNoticeView.cs` (node `HUD/UI/CareNotices`; the loss card is shown in the picture bubble — no separate `LossCard.tscn`) ✅ built |
| 1.10 Save / Load | `scripts/autoloads/SaveGame.cs` (versioned, section registration), `scripts/main/SaveBinder.cs` ✅ built |
| 1.11 Pause & Day/Night | `scenes/ui/PauseMenu.tscn` + `scripts/ui/{PauseMenuView,PauseButton}.cs`, `scenes/ui/DayNightOverlay.tscn` + `scripts/ui/DayNightView.cs`, `scripts/core/GameSettings.cs`, action `pause` (bound since 1.1) ✅ built |
| 1.12 Onboarding | `scripts/autoloads/Onboarding.cs`, `scripts/main/OnboardingDirector.cs`, `scenes/ui/Nudge.tscn` + `scripts/ui/NudgeView.cs`, `scenes/world/Ada.tscn` + `scripts/world/{Ada,GuideArrow}.cs` ✅ built |
| 1.13 Playtest & Iterate | fixes only — no new top-level folders |
| 1.14 3D Foundation | `scenes/main/Main3D.tscn` + `scripts/main/Main3D.cs` (became `Main` in 1.15), `scenes/player/Player3D.tscn`, `scripts/player/PlayerController3D.cs` (became `Player`/`PlayerController`), `scripts/main/CameraRig.cs`, `debug/CameraDebugKeys.cs`, `scripts/core/{ModelLibrary,NamedModel}.cs`, `data/placeholder_models.json`, `assets/models/` + README, `scenes/world/TestGround3D.tscn` + `scripts/world/TestGround3D.cs`, `tests/Tests3D.cs`; `TestRunner` groups; `tools/verify.sh` boots Main3D ✅ built |
| 1.15 Farm Level & Buildings in 3D | the cut-over: `Main.tscn`/`Main.cs`, `Player.tscn`/`PlayerController.cs` (3D), `FarmLevel.tscn`/`FarmLevel.cs`, `Building.cs` (+ `Fader`), `IndoorZone.cs`, `Bed` as a Prop, `SleepSequence`/`SaveBinder` in 3D, `SaveGame` schemaVersion 2 (moved forward from 1.20), roof placeholders ✅ built |
| 1.16 Garden Tiles in 3D | `FarmGrid.cs` as `Node3D` with MultiMesh tiles (no separate view class), soil/crop placeholders ✅ built |
| 1.17 Props, NPCs & Prompts in 3D | `Prop.cs` (3D base), `scripts/core/WorldPin.cs`, `GuideArrow.cs`, 3D prop scenes (same names), prop placeholders ✅ built |
| 1.18 Hens & Cow in 3D | `FarmAnimal.cs` (+ `HeartBar`), `AnimalHouse.cs` in metres, hen/cow placeholders ✅ built |
| 1.19 Day/Night Lighting in 3D | `scripts/ui/DayNightView.cs` drives the `Sun` and `WorldEnvironment` (no separate scene) ✅ built |
| 1.20 Save Migration & Performance Check (the cut-over and migration landed in 1.15) | `debug/PerfCheck.tscn` + `PerfCheck.cs`, `tools/perf_check.sh`, `ModelLibrary.BakedMesh`/`BakeBoxes` ✅ built |

---

## 6. Naming conventions

| Thing | Convention | Example |
|---|---|---|
| Folders (`src/`) | `lowercase`, no spaces | `scenes/world/` |
| Scene files | `PascalCase.tscn` | `FarmGrid.tscn` |
| C# files / classes | `PascalCase`, file == class | `PlayerController.cs` |
| Node names in the tree | `PascalCase`, role-based | `FarmGrid`, `ActiveSlotHighlight` |
| Autoload singletons | `PascalCase`, no suffix | `TaskManager` (not `TaskManagerSystem`) |
| InputMap actions | `snake_case`, verb_noun | `cycle_tool_next` |
| Signals / C# events | past tense | `[Signal] delegate void FlowerHarvestedEventHandler(...)` |
| Exported properties | `[Export] public` + descriptive noun | `[Export] public float StageSeconds` |
| Resource files (`.tres`) | `snake_case.tres` | `crops.tres` |
| Shader files | `snake_case.gdshader` | `water_shimmer.gdshader` |
| Art/audio files | `snake_case.<ext>`, placeholder prefix `ph_` (§9) | `ph_watering_can.png` |
| Save file | fixed path | `user://savegame.json` |

C# style: follow the existing `.editorconfig` in `src/` (UTF-8, LF). Godot C# classes that own nodes must be `partial` (source generators) — see the `godot-csharp` skill.

---

## 7. InputMap registry (canonical action names)

Defined once in `project.godot → [input]`, bound for **both** gamepad and keyboard/mouse per [`controls.md`](./controls.md) §2/§4. Scripts may only reference these names.

| Action | Gamepad | Keyboard / mouse | Phase |
|---|---|---|---|
| `move_left` / `move_right` / `move_up` / `move_down` | Left stick + D-pad | A/D/W/S + arrows | 1.2 |
| `interact` | A | left-click on object, Space (`quick_action` = same handler, nearest target) | 1.2 |
| `cancel` | B | right-click | bind by 1.6 (decline/back); earlier is fine |
| `cycle_tool_next` / `cycle_tool_prev` | RB or X / LB | mouse wheel down/up, or click a belt slot | 1.4 |
| `pause` | Start | Esc | 1.11 |
| `camera_switch` | Right stick flick left or right (axis 2, deadzone 0.6) | Q | 1.14 (owner 2026-11-21) |
| `open_inventory` | Y | on-screen bag button (no key) | 1.4 (simple bag) |
| `open_care_menu` | RS click | on-screen icon only | deferred (Phase-002) |
| `open_orders` | Back/Select *(decided 2026-11-21, owner)* | on-screen Orders button (no key) | Phase-002 Task 2.7 |
| `help_inspect` | *(new button needed; proposed hold LT)* | — | **deferred, explicitly out of Phase-001** |

**Bound in Task 1.1** (all Phase-001 rows above except the deferred ones). Keyboard uses physical keycodes. `cycle_tool_next` = X, RB, wheel down; `cycle_tool_prev` = LB, wheel up (InputMap cannot express the LB+RB chord). `interact` includes left mouse, so click-to-move (Task 1.2) must split ground clicks from object clicks in code.

**Task 1.2:** `move_*` deadzones lowered from 0.5 to **0.2** so a half-tilted stick walks slowly instead of not at all; `PlayerController` reads them with `Input.GetVector(..., 0.2)`. Ground left-clicks are handled in `PlayerController._UnhandledInput` (UI eats its own clicks first). Gamepad bindings use Godot's SDL button indices, which are **position-based**, so the same map works for Xbox, PlayStation and Switch Pro controllers (controls.md §2 *Any controller*). UI must show button prompts per action, never a hard-coded letter.

**Task 1.3:** `interact` is now read: **Space / gamepad A** (bottom face button on every pad) uses the nearest `IInteractable` in reach; a **left-click on an interactable's click area** (physics layer 8) walks Kit there, then uses it — any other left-click is a ground click. In the "Go to sleep?" prompt, Godot's built-in `ui_*` actions drive the buttons (D-pad/stick/arrows move focus, A/Space/Enter confirm, B/Esc/right-click close via `ui_cancel` + `cancel`). Button prompts come from `scripts/ui/ButtonPrompt.cs` (key cap, or Xbox letters / PlayStation shapes / Nintendo letters, picked from the last device used). Debug-only F2/F3/F4 clock keys are raw keys in `src/debug/` on purpose (developer tools, not game controls).

**Task 1.4:** `open_inventory` bound to gamepad **Y** (top face button on every pad); the mouse uses the on-screen bag button; there is no keyboard key (owner decision 2026-11-20). `cycle_tool_next` events reordered to RB, X, wheel down so prompts show RB. **Godot's built-in `ui_accept` / `ui_cancel` have no gamepad buttons** (only Enter/Kp Enter/Space and Esc), so they are overridden in `project.godot` with **A** and **B** added (keys as physical keycodes) — every focused menu button now works on any pad. The bag closes on Y, B, right-click, Esc or its Close button (read in `_Input` while open, before the GUI). Debug-only F6/F7/F8 bag/wallet keys are raw keys in `src/debug/`.

**Task 1.11:** `pause` (Start / Esc) now opens the pause menu; an on-screen pause button does the same for the mouse. Inside the menu B / Esc / Start close it (B / Esc first leave Settings).

**Task 1.6:** no new actions. In a picture bubble the focused button takes A / Space / Enter / click; B, right-click and Esc press its cancel button ("Not now"). Props and Marlow use `interact` like the bed.

**Task 1.5:** no new actions. `interact` (A / Space) with no interactable in reach uses the active tool on the garden tile in front of Kit; a left-click on a garden tile walks Kit beside it, then uses the tool. Interactables win over tiles.

Rules: D-pad stays reserved for movement (`controls.md` §2); no action may require reading; every new action is added to this table in the same session.

---

## 8. Autoload registry (`project.godot → [autoload]`)

| Autoload | File | Added in | Responsibility | Must not break |
|---|---|---|---|---|
| `GameClock` ✅ | `scripts/autoloads/GameClock.cs` | Task 1.3 | stepped clock (8 phases), day/season, 04:30 wake-up, sleep and 21:30 bedtime, `DayEnded`/`DayStarted` signals | day rollover drives growth and hunger counts — fire exactly once |
| `ToolBelt` ✅ | `scripts/autoloads/ToolBelt.cs` | Task 1.4 | active tool + owned tools (hands … feed scoop), cycling | other systems read the active tool through it |
| `Inventory` ✅ | `scripts/autoloads/Inventory.cs` | Task 1.4 | auto-stacking items (per id + quality), `ItemIds` | never "full"; `Remove` is all or nothing |
| `Wallet` ✅ | `scripts/autoloads/Wallet.cs` | Task 1.4 | coins (500 start); never below 0 | `Spend` refuses instead of going negative |
| `OrderManager` ✅ | `scripts/autoloads/OrderManager.cs` | Task 1.6 (board in 1.7) | order generation (producible items only), accept/deliver, repeat orders | never offers what Kit cannot produce |
| `SaveGame` ✅ | `scripts/autoloads/SaveGame.cs` | Task 1.10 | versioned JSON save, section registration, backup | nothing else writes `user://` |
| `Onboarding` ✅ | `scripts/autoloads/Onboarding.cs` | Task 1.12 | guided start + one-time nudges | never blocks play |
| `EventBus` *(proposed)* | `scripts/autoloads/EventBus.cs` | when first needed | decoupled game events | keep tiny |

Adding an autoload is a structural decision that must be recorded here plus in the task's *Deviations* note. Do not create autoloads for convenience. Each autoload's **process mode** is fixed by the §4 process-mode table — set it in the same session the autoload lands.

---

## 9. Asset & placeholder pipeline

- **Game assets live only in `src/assets/`** so Godot imports them (`*.import` sidecars are generated next to the source file).
- **Placeholders first, always.** Prefer in-scene `ColorRect` / `PlaceholderTexture2D` over image files. When an image file *is* the placeholder, its file name starts with `ph_` and it is marked temporary — final art swaps in with **no code changes** (same node paths, same region sizes where practical).
- **Licensing:** every asset that is not `ph_` gets a row in `src/assets/LICENSES.md` (name, source, license, date). Royalty-free only; record it at the moment the file lands, not later.
- **Art style guardrail:** cartoony, colourful 3D (GDD §9, since 2026-11-21; the 2D build's sprites were non-pixel-art placeholders); original designs only (docs/PHASE.md §7). Never copy existing characters/assets.
- **Placeholder folder (decided in Task 1.1):** image placeholders live in `src/assets/placeholder/<source>/ph_<sprite name>.png` (today: `kenney/`, Kenney CC0 Tiny Town + Tiny Dungeon; and since Task 1.3 `handmade/`, simple icons drawn for this project). They are licensed in `LICENSES.md` too, because they are third-party files.
- **3D models (from Task 1.14):** world objects use a `NamedModel` node (`ModelName`); code calls `ModelLibrary.Instantiate("<name>")`. Lookup order: `assets/models/{characters,world,items}/<name>.glb` (or `.tscn`) → a **placeholder primitive** from `data/placeholder_models.json` (shape `box` / `capsule` / `cylinder` / `sphere` / `prism`, size in metres, flat colour, offset; several parts per name allowed, e.g. walls + roof) → a magenta box. Dropping a real model in with the same name replaces the placeholder with no code or scene change. A real model's origin sits at its **feet / ground centre**, faces **−Z** (north), 1 unit = 1 m. Materials are cached by colour so equal colours share one material (§16). Every model file gets a `LICENSES.md` row (Kenney CC0, team-made in Blender, or AI-generated with its licence terms).
- **Load 2D pictures by name, never by path** (HUD, bubbles, cards, pinned icons; the 2D world's `NamedSprite` node was removed at the 3D cut-over): code calls `SpriteLibrary.Get("<name>")`. Lookup order: `assets/art/{characters,world,ui,items,vfx}/<name>.png` → `assets/placeholder/{kenney,handmade}/ph_<name>.png` → coloured square. Dropping final art in with the same name replaces the placeholder with no code or scene change.

---

## 10. Version control

**Decided 2026-11-19:** only Godot's generated cache is ignored. The root `.gitignore` already contains it, so no `src/.gitignore` is needed:

```gitignore
src/.godot/
```

No `android/` entry (PC first, consoles later; no mobile). **First export 2026-11-21 (owner request):** `build/` is now in `.gitignore`; `src/export_presets.cfg` has a **macOS** preset (universal x86_64 + arm64, ad-hoc signed, release, `tests/*` excluded) writing `build/macos/Fields & Sunshine.zip`; `src/MyFarmingGame.sln` exists (C# export needs it, with the Debug / ExportDebug / ExportRelease configurations); `rendering/textures/vram_compression/import_etc2_astc=true` (required for arm64 / universal Mac exports). Export templates: Godot 4.7.2 mono, only `macos.zip` installed in `~/Library/Application Support/Godot/export_templates/4.7.2.stable.mono/`. **Export with `tools/export_mac.sh`** → `build/macos/Fields & Sunshine (Mac).zip`: Godot 4.7.2 writes the "&" of the game name unescaped into the Info.plist, which breaks the signature ("cannot be opened / move to trash" on other Macs); the script escapes it, ad-hoc signs the whole bundle again, verifies it and zips with `ditto` (owner confirmed the fixed app opens on another Mac, 2026-11-21). On the other Mac: `xattr -cr` the app, then right-click → Open. Web export is not possible for C# projects in Godot 4.

Root `.gitignore` (repo-wide, current):

```gitignore
.unipi/*
src/.godot/
```

> ⚠️ `.agents/skills/` is **committed** today, although §3 and the earlier draft of this section described it as gitignored/restorable. Left as is; decide separately whether to keep committing it.

- **Commit:** scenes, scripts, `project.godot`, `*.import` sidecars (decided 2026-11-19), `export_presets.cfg`, `skills-lock.json`, `src/assets/**`, `src/assets/LICENSES.md`.
- **Never commit:** `.godot/`, `user://` save data, build artifacts, editor cache, real credentials.
- Commit at natural checkpoints: task start state → working skeleton → finished task (AGENTS.md §6).

---

## 11. Verification commands (software tests)

> ✅ **Verified 2026-11-19 (Task 1.1)** with Godot 4.7.2 .NET on macOS. `godot` = the binary in §2. All of these run in one go with **`tools/verify.sh`** (exit 0 = green). macOS has no `timeout`; rely on `--quit-after`.

```bash
# from the repository root
godot --headless --path src --import                  # first import / after adding assets
godot --headless --path src --quit-after 120          # main scene boots and runs 120 frames, exit 0
godot --path src --write-movie /tmp/shot.png --quit-after 5   # windowed: writes frame PNGs for a visual check
dotnet build src/MyFarmingGame.csproj                # C# compiles (needs §14.2)
godot --headless --path src res://tests/TestRunner.tscn          # exit code 0 = all checks pass
godot --headless --path src res://tests/TestRunner.tscn -- --group=rules          # only some groups (Task 1.14)
tools/perf_check.sh                                    # windowed frame budget, pc + handheld profiles (Task 1.20)
```

**Test groups (Task 1.14; 2D group removed at the 1.15 cut-over):** `rules` (no scene: data, rules, maths) and `game` (the real 3D `Main.tscn`). No argument = both, in the original order. `tools/verify.sh rules` passes the list through. Headless runs draw nothing, so the §16 draw-call check only bites in a windowed run (Task 1.20 adds a windowed measurement).

**Test layout:** `src/tests/TestRunner.tscn` + `TestRunner.cs` — a `Node` that runs small checks (scene instantiates, action exists, state machine transitions, save round-trip), prints each failure, and calls `GetTree().Quit(failCount == 0 ? 0 : 1)` so CI/headless runs go red on failure. Adapt the pattern from the `godot-gdscript-headless-testing` skill to C#; never leave a test scene in `Main.tscn`'s path.

**Session verification minimum:** the project builds, `Main.tscn` runs headless with no errors in the output log, and any test added this session passes — before the session ends.

---

## 12. Installed AI skills (project-level)

Installed with `npx skills` from the repository root; files in `.agents/skills/`, versions pinned in `skills-lock.json`. Pi discovers them from the working directory up to the repo root, so **any session started inside this repo can load them**.

Restore after cloning: `npx skills experimental_install` · Update: `npx skills update -p`

| Skill | Load when |
|---|---|
| `godot-csharp` | any `.cs` in a Godot node, build/export failure, `[Export]`/`[Signal]` questions |
| `godot-nodes-scenes` | instancing scenes, broken `res://` paths, "Node not found", adding an autoload |
| `godot-signals-groups` | wiring events, handler fires twice/never, event-bus design |
| `godot-best-practices` | general Godot project/scene/design hygiene questions |
| `godot-2d-movement` | Task 1.2 — CharacterBody2D controller, camera follow |
| `input-systems` | Tasks 1.2/1.3 — InputMap actions, gamepad bindings, rebinds |
| `godot-ui-control` + `game-ui-ux` | Tasks 1.3/1.5/1.10 — anchors, containers, gamepad focus, HUD layout |
| `godot-resources` | Tasks 1.5/1.8 — crop and price `.tres` data |
| `godot-tilemap` | Task 1.4+ — garden/lane TileSet, `TileMapLayer` |
| `godot-animation` | Tasks 1.2/1.6/1.11 — AnimationPlayer, tweens, sprite flips |
| `godot-physics` | collision layers and interaction areas (2D-era examples; the project is 3D now) |
| `godot-audio` | first SFX/music wiring, buses, silent/cut-off sound debugging |
| `dialogue-systems` | Task 1.6 — picture-bubble/state-flow structure (engine-neutral reference) |
| `save-systems` | Task 1.9 — JSON save schema, atomic write, versioned migration |
| `godot-gdscript-headless-testing` | §11 — headless runner / CI exit-code pattern (adapt to C#) |
| `godot-export` | first PC export / `export_presets.cfg` (later phase) |

**3D note (2026-11-21):** after Task 1.20, `godot-2d-movement` and `godot-tilemap` no longer apply; the other skills do. No 3D-specific skill is installed yet — installing one (3D character controller, camera, `MultiMeshInstance3D`) before Task 1.14 is optional team prep.

**Skills are advisory.** They run with agent permissions: review before trusting, and they never override `AGENTS.md`, the GDD, or a phase task.

---

## 13. Session checklist (how a development session follows this doc)

1. Read `AGENTS.md` → current phase task (`docs/Phase-001.md`) → **this document**.
2. Confirm §2/§14 prerequisites; if the toolchain is missing, stop and request it (AGENTS.md §5).
3. Place every new file per §4 and the §5 task map; follow §6 naming.
4. Register new input actions in §7 and any autoload in §8 — in the same session.
5. Mark placeholders and license new assets per §9; update `.gitignore` per §10 if needed.
6. Run §11 verification; fix errors before finishing.
7. Load the matching skill from §12 when the task matches one.
8. Fill in **all** `Notes & learnings` fields of the task, bump this doc's version/date if structure changed, append a Change log entry.

---

## 14. Open decisions & blockers (need a human)

1. ~~Godot binary missing~~ — **RESOLVED 2026-11-19:** Godot 4.7.2 .NET installed in `/Applications` (not on `PATH`; see §2). Original note: Two extracted mono builds sit in `~/Downloads/` (`4.7.2` and `4.6.1`); `src/project.godot` declares feature `4.7`. *Need:* install **one** Godot **.NET/mono** build matching `4.7`, make it runnable as `godot`, then record the exact version + verified headless commands in §11 and Task 1.1's Notes. Until then no software test can run.
2. ~~No `.csproj`~~ — **RESOLVED 2026-11-19:** `src/MyFarmingGame.csproj` written in Task 1.1 and builds with SDK 11 (`RollForward=LatestMajor`). Original note: Godot generates them on first C# build (editor or `godot --build`). *Need:* confirm the generated project builds with SDK `10.0.401` (Godot 4.7 targets an earlier `net` TFM — install the matching targeting pack if the build fails), and commit the generated files.
3. ~~Assembly name contains a space~~ — **RESOLVED 2026-11-19:** renamed to `MyFarmingGame` in `project.godot`.
4. ~~`.import` discrepancy~~ — **RESOLVED 2026-11-19:** commit `*.import` sidecars, ignore only `.godot/`; Phase-001 Task 1.1 synced (v0.9).
5. ~~Renderer preset~~ — **RESOLVED 2026-11-19:** PC only; renderer switched to **Compatibility** (`gl_compatibility`, also for the `.mobile` override), `Mobile` feature tag and the `d3d12` driver line removed. The `3d/physics_engine="Jolt Physics"` entry is 3D-only and harmless.

---

## 15. Related documents

- [`Phase-001.md`](./Phase-001.md) — the tasks this structure serves (Task 1.1 implements §4, §6, §10, §11).
- [`controls.md`](./controls.md) — the control mappings behind §7.
- [`mechanics.md`](./mechanics.md) — verbs and day/season rules behind §8.
- [`GDD.md`](./GDD.md) — gameplay source of truth (this doc never overrides it).
- [`../README.md`](../README.md) — project hub and documentation table.
- [`../AGENTS.md`](../AGENTS.md) — session protocol, testing policy, doc conventions.

---

## 16. Performance budget (3D, Switch later) — proposal 2026-11-21

The game ships on PC first, but Switch and PlayStation follow (GDD §2). Every 3D task stays inside this budget so the console versions need no redo. The original Switch is the low bar. Numbers started as a proposal; `tools/perf_check.sh` (Task 1.20) measures them windowed for both profiles.

**Measured 2026-11-21 (Task 1.20, owner's M1 MacBook Air, Compatibility, busiest view: full garden, house, coop, fields, midday):** ≈ 11.4 ms per frame (≈ 88 fps uncapped), **242 draw calls** (3D world ≈ 158, HUD ≈ 84, shadows ≈ 23), ≈ 67k triangles, both profiles. Known issue: one ≈ 145 ms first-sight shader-compile hitch per run (warm-up / shader baking to do). Next target: the HUD's draw calls.

| Item | PC target | Handheld profile (Switch-like) |
|---|---|---|
| Frame rate / resolution | 60 fps at 1080p on integrated graphics (M1 Mac, Intel Iris Xe) | 30 fps at 720p (tested on the Mac with the low profile until a devkit exists) |
| Draw calls per frame (HUD included) | ≤ 300 (aim ≤ 150) | ≤ 250 (revised from 150 in Task 1.20: the HUD alone is ≈ 84) |
| Triangles on screen | ≤ 150k (aim ≤ 100k) | ≤ 100k |
| Shadows | one `DirectionalLight3D` with shadows, 2 splits, ≈ 30 m distance | 1 split, ≈ 20 m, lower resolution |
| Other lights | ≤ 4 small unshadowed lights on screen at night; prefer glowing (emissive) windows to lights | same |
| Anti-aliasing / effects | MSAA 2×; no SSAO / SSR / SDFGI / volumetric fog / glow (Compatibility lacks most anyway) | MSAA off or FXAA |
| Models | Kit ≈ 3–5k triangles, props ≤ 1–2k, a building ≤ 3k (art-audio.md §1b) | same models |
| Textures | ≤ 1024², atlases or plain colours | same |

Rules:

1. **Repeated things are one `MultiMeshInstance3D`** (soil tiles, crops per stage, weeds, overgrown grass, fence posts), never one node per tile — the farm grows to 992 tiles.
2. **Share materials:** one material per colour or model, never a unique material per instance; `ModelLibrary` caches them.
3. **Transparency only while fading:** roofs and walls fade, then are hidden; no permanently transparent meshes.
4. **Simple physics:** box/capsule colliders only; Kit is the only `CharacterBody3D`; animals move by setting positions inside their house.
5. **Placeholders are baked:** `ModelLibrary` merges each placeholder's parts into one vertex-coloured mesh with one shared material; static scenery is baked with `ModelLibrary.BakeBoxes`. Small props set `Prop.CastsShadow = false`.
6. **Measure, don't guess:** read `Performance.Monitor.RenderTotalDrawCallsInFrame` and `RenderTotalPrimitivesInFrame` in the busiest view (the whole front garden planted, all animals in view) in a repeatable check; a task that breaks the budget records why in its Notes.
7. Renderer stays **Compatibility** for PC; confirm renderer support with the console porting partner before any console work (GDD §13).

---

## 17. Change log

- **0.1 — 2026-11-18:** Initial structure plan: repository and `src/` layout, task→file map for Phase-001, naming conventions, InputMap and autoload registries, placeholder/licence asset pipeline, version-control rules, intended headless verification commands, the 17 installed Godot/game-dev skills with when-to-load guidance, session checklist, and five open decisions/blockers (missing Godot binary, missing `.csproj`, spaced assembly name, `*.import` conflict, mobile renderer preset).
- **0.2 — 2026-11-19:** Owner decisions applied (PC only): §14 items 3–5 **resolved** — assembly renamed `MyFarmingGame`, `.import` sidecars committed with only `.godot/` ignored, renderer switched to Compatibility. §2, §4, §10, §11 updated to match; `android/` ignore and the `grown_up_menu` input action removed; art guardrail wording updated. Flagged that `.agents/skills/` is committed despite being described as gitignored.
- **0.3 — 2026-11-19:** Phase-001 re-scoped to a vertical slice: §5 task→file map rebuilt for the 13 new tasks; §8 autoload registry updated (GameClock first, ToolBelt/Inventory/Wallet, OrderManager, versioned SaveGame, Onboarding; TaskManager removed). Note: §4's scene/script tree still lists some Phase-001 v0.9 file names (GardenBed, GardenShelf, Butterfly) — treat §5 as authoritative until Task 1.1 settles the tree.
- **0.4 — 2026-11-20:** **Scene-architecture standard agreed (owner decision).** A feature-first `source/{core,data,gameplay,levels,shaders,ui,debug}` proposal was reviewed and **rejected in favour of keeping the type-first `src/` layout**; only its new folders were absorbed. New §4 subsections: `Main ↔ Level` ownership tree (Systems / World{Level, Entities, Effects} / Audio + HUD 10 / Pause 20 / Transition 30 / Debug 40 CanvasLayers), named layer constants `scripts/core/UILayers.cs`, an explicit **process-mode table including autoloads** (records the pause gotcha: autoloads default to `Always` and would keep the clock ticking while paused), and the `load_level` contract (defer → free level only → instance → spawn from marker → `LevelChanged`). Added `src/shaders/` + `src/debug/` to the §4 tree, structural rule 10, a §6 naming row for `.gdshader`, and a §8 pointer to the process-mode table. `aspect="expand"` deliberately unchanged.
- **0.5 — 2026-11-20:** Task 1.1 implemented (incl. the scene-architecture tree and `UILayers.cs`): §2 toolchain verified (Godot 4.7.2 .NET, SDK 11 with RollForward), §4 adds `assets/placeholder/`, `SpriteLibrary`/`NamedSprite`, main scene configured, `.cs.uid` sidecars; §7 bindings recorded; §9 placeholder folder and load-by-name rule; §11 commands verified + `tools/verify.sh`; §14 items 1–2 resolved.
- **0.6 — 2026-11-20:** Task 1.2: §4 tree adds `scripts/main/CameraFollow.cs` and describes `Player.tscn`/`PlayerController.cs`; §7 records the 0.2 move deadzone, where ground clicks are handled, and position-based gamepad buttons (any controller, abstract prompts). §10 wording: PC first, consoles later. Also: `src/shaders/placeholder_checker.gdshader` (temporary checkerboard ground for `PlaceholderLevel`, replaced by tiles in Task 1.5).
- **0.7 — 2026-11-20:** Task 1.3: `GameClock` autoload registered (Pausable). §4 tree adds `Bed.tscn`/`Bed.cs`, `ClockDial`/`SleepPrompt`/`DayTransition` scenes and views, `ButtonPrompt.cs`, `IInteractable.cs`, `SleepSequence.cs` (Main/Systems), `debug/ClockDebugKeys.cs`, and the `assets/placeholder/handmade/` folder (§9 lookup order now searches it). §5 Task 1.3 row lists the real files; the stale "ToolBeltView — Task 1.3" label corrected to 1.4. §7 records how `interact` (Space / A / click on an interactable) and the prompt's `ui_*` navigation work, and the debug-only raw keys.
- **0.8 — 2026-11-20:** Bedtime references updated to 01:00 (owner decision, GDD v0.29); debug F4 now jumps to 00:50.
- **0.9 — 2026-11-20:** Bedtime references corrected to midnight (GDD v0.30); debug F4 jumps to 23:50.
- **0.10 — 2026-11-20:** Clock references updated to the 04:30 wake-up and 21:30 bedtime (GDD v0.31); debug F4 jumps to 21:20.
- **0.11 — 2026-11-20:** `GameClock`/`ClockDial` descriptions: readout ticks every 15 in-game minutes (`TimeChanged` signal).
- **0.12 — 2026-11-20:** `docs/assets/` now also holds design images: `map/bramblewick-vale.svg` (the to-scale valley map, places.md v0.10).
- **0.13 — 2026-11-20:** Task 1.4: `ToolBelt`, `Inventory`, `Wallet` autoloads registered (Pausable). §4 tree adds `Bag.tscn`, `WalletHud.tscn`, `BagView`/`WalletView`, `debug/BagDebugKeys.cs`; `ToolBelt.tscn` relabelled Task 1.4. §5 row lists the real files. §7: `open_inventory` bound (Y + on-screen button, no key by owner decision), `cycle_tool_*` row updated, and `ui_accept`/`ui_cancel` overridden with gamepad A/B (Godot's defaults have no pad buttons).
- **0.14 — 2026-11-20:** Synced with Phase-001 v0.28 (Tasks 1.5–1.13 checked against the docs): §4 tree drops the stale v0.9 names — `GardenBed` (now `FarmGrid`, Task 1.5), `GardenShelf`, `Butterfly` (the Butterfly Garden is an optional Phase-004 request), and `GrowthTiming.cs`/`growth_timing.tres` (play-time growth presets, retired by day-based growth) — and adds `FarmLevel`, `IndoorZone`, `BerryBush`, `Phone`, `Hen`/`Cow`, `Ada`, `crops.tres`, `prices.tres`. Pause menu and `pause` action labelled Task 1.11 (was 1.10). §5 rows for 1.5, 1.8, 1.9 and 1.12 updated; naming examples use `FarmGrid`/`crops.tres`.
- **0.15 — 2026-11-21:** Task 1.5 built: tree and file map updated (FarmLevel, FarmGround, FarmGrid, CropTile, Building, IndoorZone, IToolTarget, CropDefinition/CropCatalog, `data/crops.tres`, `tools/make_placeholders.py`); PlaceholderLevel removed; no separate FarmGrid/IndoorZone scenes; §7 note on tool use with `interact`.
- **0.16 — 2026-11-21:** Task 1.6 built: Prop base, Marlow, BerryBush, Ledger, PictureBubble, `OrderManager` autoload landed in 1.6 (board/generator still 1.7), `Order` + `PriceTable`, `data/prices.json` generated by `tools/make_prices.py` instead of `prices.tres`.
- **0.17 — 2026-11-21:** Task 1.7 built: OrderBoard prop + board screen, `Production` supply registry; file map updated.
- **0.18 — 2026-11-21:** Task 1.8 built: Shop screen, ShopCounter, Phone, AnimalHouse + FeedBin, FeedStore; file map updated.
- **0.19 — 2026-11-21:** Task 1.9 built: FarmAnimal/Hen/Cow, CareNoticeView (loss card in the picture bubble); file map updated.
- **0.20 — 2026-11-21:** Task 1.10 built: SaveGame autoload (Always) and SaveBinder in Main/Systems; file map and autoload registry updated.
- **0.21 — 2026-11-21:** Task 1.11 built: PauseMenu (Main/Pause), PauseButton, DayNightOverlay (Main/World/Effects), GameSettings; settings file written by SaveGame.
- **0.22 — 2026-11-21:** Task 1.12 built: Onboarding autoload, OnboardingDirector, NudgeView, Ada, GuideArrow; file map and autoload registry updated.
- **0.23 — 2026-11-21:** Owner order-variety follow-up: `core/Neighbours.cs`, `ui/StandingOrdersView.cs` (HUD/UI/StandingOrders); `OrderPremium` now 1.0.
- **0.24 — 2026-11-21:** InputMap registry: `open_orders` (Back/Select proposed, on-screen Orders button) added for Phase-002; `help_inspect` loses Back/Select (controls.md v0.13). Docs-only.
- **0.25 — 2026-11-21:** **Switch to 3D planned (owner decision 2026-11-21, GDD v0.45).** §4 lists the planned 3D files for Phase-001 Tasks 1.14–1.20 (parallel `Main3D.tscn` until the 1.20 cut-over, `Player3D`, `CameraRig`, `ModelLibrary`/`NamedModel` + `data/placeholder_models.json`, `FarmLayout`, `FarmLevel3D`, `Building3D`, `FarmGridView3D`, `Prop3D`, `FarmAnimal3D`, `DayNight3D`, `assets/models/`) and the 2D files deleted at cut-over; scene architecture records 3D units (1 tile = 1 m, XZ ground, north = −Z) and node types; §5 task map rows 1.14–1.20; §9 3D load-by-name pipeline and model conventions; §12 3D note; new **§16 performance budget** (Switch later); change log renumbered to §17. Docs-only.
- **0.26 — 2026-11-21:** Scene architecture note: diagonal camera (owner 2026-11-21) — map north stays −Z, input turned by the camera yaw, tool front snaps to grid axes. Docs-only.
- **0.27 — 2026-11-21:** Task 1.14 built: §4 marks the 3D foundation files built (`Main3D` + `Main3D.cs`, `Player3D`, `CameraRig`, `CameraDebugKeys`, `ModelLibrary`/`NamedModel`, `placeholder_models.json`, `TestGround3D`, `Tests3D.cs`); §5 row 1.14; §11 test groups and the Main3D boot command.
- **0.28 — 2026-11-21:** InputMap registry: `camera_switch` (right stick flick / Q) for the owner's camera corner switch (Task 1.14). Gotcha recorded in Phase-001 Task 1.14: one action bound to both stick directions matches either sign, so `CameraRig` detects the flick itself.
- **0.29 — 2026-11-21:** Tasks 1.15–1.18 built and the **cut-over to 3D done in Task 1.15** (not 1.20): §4 lists the 3D files and the deleted 2D ones, the scene-architecture 3D note is now permanent, §5 rows 1.15–1.18 (and 1.20 renamed), §9 notes `NamedSprite` is gone (SpriteLibrary stays for 2D pictures), §11 test groups `rules` + `game`; `tools/verify.sh` boots the 3D `Main.tscn`.
- **0.30 — 2026-11-21:** Task 1.20: §16 records the measured values and revises the handheld draw-call line to 250 (HUD ≈ 84), adds the baking rule; §11 `tools/perf_check.sh`; §5 row 1.20 built.
- **0.31 — 2026-11-21:** §10: first export (macOS universal app) — `build/` ignored, macOS preset, `.sln`, ETC2/ASTC import setting, export command; no web export for C#.
- **0.32 — 2026-11-21:** §10: `tools/export_mac.sh` (plist fix, re-sign, verify, ditto zip) after the first export would not open on another Mac.
- **0.33 — 2026-11-21:** **Phase-002 Task 2.1 doc sync:** §4 tree rewritten for the 3D project (scenes, scripts, data, debug, tests), 2D-era wording removed from the structural rules, scene architecture and `load_level` contract; §5 rows 1.14 and 1.19 corrected; §7 `open_orders` decided (Back/Select).
