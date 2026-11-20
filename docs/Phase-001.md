# Phase-001 — First Playable (Vertical Slice)

> **Status:** In progress — Tasks 1.1–1.4 done; Task 1.5 built, pending human playtest (2026-11-21)  
> **Version:** 0.31  
> **Last updated:** 2026-11-21  
> **Milestone:** 1 — First playable  
> **Purpose:** Break Milestone 1 down into session tasks. Each task produces one scoped, playtested artifact that can be created and validated in a single session with AI assistance.

## 1. Milestone Overview

**Goal (re-scoped 2026-11-19):** a **vertical slice of the money loop** — produce → order → money → invest — in one connected area. Owner decision: narrow the first playable to include orders and chickens.

**Scene:** Kit's house (bed, phone) and front garden with the old coop → the old barn behind the house → the farm gate → the lane (berry bushes) → Mr. Marlow's seed shed (order board, shop).

**Where this sits on the map (places.md v0.10, Bramblewick Vale, 2026-11-20):** the slice covers the farm's centre column from places.md §3 — the house with the front garden and old coop facing the gate, the cross path, and the old barn at the back edge of Field A (behind the house, ground around it already clear) — plus the lane up to Mr. Marlow's cottage and seed shed (≈ 20 tiles from the front door). Fields A–E stay overgrown and are not walkable yet. The village green, the brook, the wood, and the hill are built in later phases; the slice needs no area transitions.

**Indoors (GDD §4 "time stops indoors", owner decision 2026-11-20):** in this phase the house, coop, barn and seed shed are entered **on the same map** — walking into a building's footprint fades its roof out to show the inside, and the clock stops while Kit is inside. There are **no separate interior scenes** (fade to a room map) in Phase-001; whether later phases need them is an open question for Phase-002 planning (the kitchen upgrade is the first likely case).

**Loop in this milestone:**
1. Kit clears weeds, plants starter carrots, and waters them daily.
2. Marlow's first order: 3 handfuls of wild berries (forage) → coins; the order board opens.
3. Marlow's second order: 5 carrots (3 days of watering) → coins.
4. Crops grow one day per watered night; left dry they turn yellow (2 days) and are gone (3 days).
5. Kit starts with **5 hens and 1 cow** in the family's old coop and barn, plus ≈ 7 days of feed; Kit orders more feed **by phone from home** and buys seeds at the seed shed (half real price, economy.md §2).
6. Kit feeds the animals daily and collects eggs and milk; egg and milk orders appear and repeat; an animal unfed for 3 days is gone (gravestone message card).
7. Everything saves and loads.

**Explicitly OUT of scope (deferred, not cut):** market stall, shipping box, demand pricing, inventory grid UI beyond the simple bag, stickers, clearing the back fields, other animals → **Phase-002**; barn, fish pond, processing, workshop, co-op split screen → **Phase-003**; automation, brook fishing, merchant, the Butterfly Garden request → **Phase-004**; festivals, charm, endgame → **Phase-005** (docs/PHASE.md §3).

## 2. Session Tasks

Tasks are ordered by dependency. After each task, the game should be runnable and the new artifact playtestable.

Each task includes **Team Preparation**, **Tests** (Software / Human), and a **Notes & learnings** block to fill in before the next task starts. An unfilled block means the task isn't finished.

---

### Task 1.1 — Project Foundation

**Artifact:** Godot 4.x project with version control, scene tree structure, and placeholder art pipeline.

**Scope:**
- Initialize the Godot 4.x C# project in `src/`
- `.gitignore`: ignore **only** `.godot/`; **commit** `*.import` sidecars (root `.gitignore` already does this)
- Verify — do not change — the 2026-11-19 settings: renderer **Compatibility** (`gl_compatibility`, no `Mobile` tag) and assembly **`MyFarmingGame`**
- Create root scenes: `Main.tscn`, `Player.tscn`, `UI.tscn`
- Build `Main.tscn` to the project-structure.md §4 **scene-architecture standard**: `Systems` / `World{Level, Entities, Effects}` / `Audio` + HUD(10) / Pause(20) / Transition(30) / Debug(40) CanvasLayers, layer constants in `scripts/core/UILayers.cs`, and explicit process modes (autoloads set theirs when they land); create empty `src/shaders/` + `src/debug/` folders
- Placeholder art (colored rectangles) for player, NPC, beds, crops, hen, coop, tools
- Confirm the project runs and displays a scene

**Acceptance criteria:**
- Godot project opens without errors
- `Main.tscn` runs and shows a colored background with a placeholder player sprite
- `Main.tscn` tree, CanvasLayer numbers, and process modes match project-structure.md §4
- `git status` shows no `.godot/` files; `*.import` sidecars are tracked
- Renderer is Compatibility; `dotnet build src/MyFarmingGame.csproj` succeeds

**Team Preparation:**
- Godot 4.x **.NET** installed and on `PATH` (project-structure.md §14.1)
- Team agrees on folder structure and naming (project-structure.md)

**Tests:**
- **Software:** project opens headless without errors; `Main.tscn` instantiates; placeholder sprite visible; `.gitignore` check as above; `dotnet build` passes
- **Human:** None required

**AI assistance:** Prompt for "Godot 4.x C# project structure for a 2D top-down farming game, Compatibility renderer, basic scene tree."

**Dependencies:** None.

**Notes & learnings:**
- **Outcome / status:** 2026-11-20 — **Done** (Human: none required); rebuilt on 2026-11-20 to the v0.15 scene-architecture standard. Software tests: PASS — `dotnet build src/MyFarmingGame.csproj` 0 warnings/0 errors; `godot --headless --path src --import` exit 0; `godot --headless --path src --quit-after 120` exit 0, no errors; `res://tests/TestRunner.tscn` 62/62 checks (incl. Main tree, process modes, CanvasLayer numbers); `git status` shows no `src/.godot/` files and the `*.import` sidecars as normal files to commit. A windowed `--write-movie` frame confirms the green background and the placeholder player in the centre. All checks bundled in `tools/verify.sh`.
- **What worked well:** Godot 4.7.2 .NET found at `/Applications/Godot_mono.app/Contents/MacOS/Godot` (not on `PATH`); every check runs headless from the CLI, no editor GUI needed. Hand-written `.csproj` + `.tscn` files load cleanly. `SpriteLibrary` + `NamedSprite` decouple art from scenes: a scene names a sprite (`"player"`), and the library resolves final art → `ph_` placeholder → flat coloured square. `--write-movie <file>.png --quit-after 5` gives a quick screenshot for visual checks.
- **Problems & fixes:** (1) Only the **.NET 11** runtime is installed but Godot.NET.Sdk 4.7.2 is used with `net8.0` → added `<RollForward>LatestMajor</RollForward>` to the `.csproj`, so the game runs on .NET 11; teammates with .NET 8 still work. (2) `kenney.nl` returns **403** to command-line downloads → used Kenney's own CC0 uploads on OpenGameArt (same zips, license files included). (3) `timeout` is not available on macOS — run Godot directly with `--quit-after`. (4) Godot 4.7 writes `*.cs.uid` sidecars next to scripts — commit them like `*.import`. (5) **Level art covered the player:** `World/Level` is a plain `Node` (per the standard), so a level's `Node2D` is a separate top-level canvas item drawn *after* `World/Entities` — the player was invisible while every node-level test still passed. Fix: level ground art gets a negative `z_index` (`PlaceholderLevel/Background` = −10) and a test checks it; Task 1.5's TileMapLayers need the same. Always confirm visuals with a `--write-movie` frame. (6) No `.sln` yet: `dotnet build` on the `.csproj` is enough; the editor creates the `.sln` the first time it is opened — commit it then.
- **Deviations from scope or plan:** (a) Placeholder art is **Kenney CC0 pixel tiles** (owner request via the project chat), not plain coloured rectangles; they live in `src/assets/placeholder/kenney/ph_<name>.png` (a dedicated placeholder folder instead of `ph_` files inside `assets/art/…`, project-structure.md §9 updated). Licences in `src/assets/LICENSES.md`. No hen in either pack, so `hen` and `tool_hands` use the coloured-square fallback. (b) Texture filter defaults to **Nearest** (crisp pixel placeholders); revisit when final non-pixel art lands. (c) Window 1280×720. (d) The Phase-001 InputMap actions from project-structure.md §7 were bound now (move_*, interact, cancel, cycle_tool_next/prev, pause) so 1.2–1.11 share one registry; gamepad "LB+RB" was mapped as **RB/X = next, LB = prev** because InputMap cannot express a chord. No code reads input yet. (e) Scene-architecture standard: `scripts/main/Main.cs` (new `scripts/main/` folder) sets CanvasLayer numbers from `scripts/core/UILayers.cs` and moves the player to the level's `PlayerSpawn`; process modes are set in `Main.tscn`. The coloured background moved into a temporary level, `scenes/world/PlaceholderLevel.tscn` (Background + `PlayerSpawn`). `UI.tscn`'s root is now a full-rect `Control` instanced into the `HUD` layer. `load_level` is **not** implemented yet (no task needs a second level). Empty `src/shaders/` and `src/debug/` hold a `.gitkeep`. (f) Added `scripts/core/SpriteLibrary.cs`, `scripts/core/NamedSprite.cs`, `tests/TestRunner.{tscn,cs}`, `tools/verify.sh`.
- **Handoff to Task 1.2:** Godot **4.7.2.stable.mono**; run checks with `tools/verify.sh` (set `GODOT=` if the binary is elsewhere). Scenes: `res://scenes/main/Main.tscn` built to project-structure.md §4 — `Main` (Node, Always, script `Main.cs`) → `Systems`, `World` (Node2D, Pausable) → `Level/PlaceholderLevel`, `Entities/Player` (unique `%Player`) + `Entities/Camera2D` (static, not yet following), `Effects`; `Audio` (Always); `HUD` (10, Pausable) → `UI` (Control from `res://scenes/ui/UI.tscn`); `Pause` (20, WhenPaused); `Transition` (30, Always); `Debug` (40, Always). `res://scenes/player/Player.tscn`: root `Player` CharacterBody2D, motion_mode floating, child `Sprite` = NamedSprite `"player"` 64×64, `CollisionShape2D` 40×24 at feet. Task 1.2 adds `scripts/player/PlayerController.cs` to the Player root and an AnimationPlayer; keep the `Sprite` child as a NamedSprite (or keep using `SpriteLibrary`) so art swaps stay code-free. Input actions already exist — `interact` includes left mouse, so click-to-move must decide ground-click vs object-click in code. Keep `TestRunner` green and extend it for movement checks.

---

### Task 1.2 — Player Movement (Gamepad + Keyboard/Mouse)

**Artifact:** Controllable player character.

**Scope:**
- `Player.tscn` with Sprite2D, AnimationPlayer, CollisionShape2D
- C# controller: left stick/D-pad and WASD/arrows; click-to-move; left-click a target = walk to it, then act (controls.md §4)
- Fixed angled top-down camera (Camera2D) following the player
- Idle + walk animations (placeholder, 4 directions); no run button

**Acceptance criteria:**
- Player moves smoothly with gamepad, keyboard, and mouse click
- Camera follows with smooth lerp
- Animations face the movement direction

**Team Preparation:**
- Xbox-layout gamepad and mouse available; placeholder player sprite

**Tests:**
- **Software:** each input moves the player; camera follows; sprite direction matches movement
- **Human:** controls feel comfortable for a 6-year-old and an adult; movement speed feels right

**AI assistance:** Prompt for "Godot 4.x C# 2D top-down controller with gamepad, WASD, and click-to-move."

**Dependencies:** Task 1.1.

**Notes & learnings:**
- **Outcome / status:** 2026-11-20 — **Done** (owner playtest 2026-11-20: "perfect" after the checkerboard ground was added; real-gamepad check not reported separately). Software tests: PASS — `tools/verify.sh`: `dotnet build` 0 warnings/0 errors, import + 120-frame boot clean, `TestRunner` **107/107** (45 new movement checks: WASD, arrows, W+D diagonal, left stick full and half tilt, D-pad, `WalkTo` arrival callback, injected left-click on the ground, key input cancelling a click target, facing + animation per direction, camera trailing then settling). A `--write-movie` frame confirms Kit drawn in the centre of the view. · Human playtest: **PASS** by the owner (movement, speed, camera feel). Still worth a quick check with a real Xbox/PlayStation/Switch Pro pad and a 6-year-old when available.
- **What worked well:** Driving the real input path in tests: `Input.ParseInputEvent` with `InputEventKey` / `InputEventJoypadMotion` / `InputEventJoypadButton` (device 0) works headless with no pad connected, and awaiting `SceneTree.PhysicsFrame` lets the controller and camera run for real. Camera2D's built-in position smoothing (camera node snaps to the player each physics frame; smoothing speed 5) gives the lerp with no custom maths. Animations are authored in `Player.tscn` (not built in code), so final sprite-sheet frames replace the placeholder tracks in the editor.
- **Problems & fixes:** (1) **Mouse position is not updated in headless runs** — `GetGlobalMousePosition()` stayed at the window origin, so clicks walked to the wrong place in tests. Fix: the click target is taken from the event itself (`GetViewport().GetCanvasTransform().AffineInverse() * mb.Position`); only hold-to-follow polls the cursor. (2) `Viewport.PushInput(ev)` rescales positions by the headless window stretch (visible rect reads 1280×1280) → pass `inLocalCoords: true` in tests. (3) `AnimationPlayer.CurrentAnimation` is a `StringName`: `.StartsWith(...)` does not compile (CS0411) → call `.ToString()` first. (4) Chained moves in tests leak ~7 frames of momentum from acceleration → each test move waits 15 physics frames first. (5) The Task 1.1 `move_*` deadzone of 0.5 would make a half-tilted stick do nothing → lowered to 0.2. (6) **Blank grey screen when run from the editor:** with the editor's *Embed Game on Next Play* on (Game tab ⋮ menu), the game drew nothing on the owner's Mac (M1, Compatibility renderer) and logged no errors; the same files ran fine as a separate window. Fix: untick *Embed Game on Next Play* (or run `Godot --path src` from Terminal). Debug builds now print one `[Main] 1s state:` line (player/camera position, viewport, level) to diagnose blank-screen reports, and `PlayerController` ignores non-finite click targets/velocity so Kit and the camera can never jump to NaN.
- **Deviations from scope or plan:** (a) **Owner decision: 8-way walking, 4-way facing** (Harvest Moon / Stardew style); on an exact diagonal (within ~10°) the current facing is kept if it is one of the two directions, so it doesn't flicker. Diagonals are normalised (same speed as straight). (b) Speed **160 px/s** (≈ 2.5 sprite heights/s), acceleration 1400 px/s² (≈ 0.11 s to full speed); both `[Export]` on `PlayerController` for playtest tuning. Click-walks start at full speed and ease into the target; arrival within 4 px; a blocked click-walk gives up after 12 frames without progress. (c) **Placeholder animations:** the Kenney sprite has one frame, so walk = a 3 px bob (0.4 s loop), left = horizontal flip, up = a slight blue-grey tint (back view stand-in); all 8 clips exist by name. (d) **Gamepad:** owner wants Switch and PlayStation later (PC first). No new bindings were needed: SDL button indices are position-based, so the Task 1.1 map already works on Xbox, PlayStation and Switch Pro pads. Docs updated to "PC first, consoles later" (GDD, PHASE, README, AGENTS, controls.md). (e) Camera follow lives in a new `scripts/main/CameraFollow.cs` on `World/Entities/Camera2D` (Main still owns the camera; `Main._Ready` snaps it after spawning the player). (f) Owner playtest feedback: a solid green field gave no sense of movement while the camera follows, so `PlaceholderLevel/Background` now uses a **placeholder checkerboard shader** (`res://shaders/placeholder_checker.gdshader`, two greens, 64 px cells = one tile); Task 1.5's TileMapLayer ground replaces it. Object clicks (walk to, then act) are not wired yet — there are no interactables; the `WalkTo(target, onArrived)` hook is ready.
- **Handoff to Task 1.3:** Controller: `res://scripts/player/PlayerController.cs` (namespace `MyFarmingGame.Player`) on the `Player` root (`%Player` in Main). API: `Facing` (`Down/Up/Left/Right`) + `FacingChanged(int)` signal, `WalkTo(Vector2, Action onArrived = null)`, `CancelWalkTo()`, `HasMoveTarget`, `CurrentAnimationName`, `MoveSpeed`/`Acceleration` exports, `InputOverride` (tests only). Inputs read: `move_*` (deadzone 0.2) and raw left mouse in `_UnhandledInput` (ground click; UI controls eat their own clicks first). `interact` (A / Space / LMB) and `cancel` are **not** read yet — Task 1.4+ act on the tile in front of `Facing`, and the bed (1.3) should be reached by an object click → `WalkTo(bed, Sleep)`. Camera: `World/Entities/Camera2D` with `CameraFollow` (`Target`, `SnapToTarget()` — call it after any teleport such as waking up at home). Animations: `idle_*`/`walk_*` × 4 in `Player.tscn`'s AnimationPlayer. Button prompts must stay per-action (no hard-coded "A") so console icons can be added later. The placeholder Background is 2560 px wide, so walking ~1280 px from spawn shows its edge until Task 1.5's level.

---

### Task 1.3 — Day Clock, Calendar & Sleep

**Artifact:** The in-game day: stepped clock, HUD dial, sleep, and the 21:30 bedtime. Moved early because growth and feeding are now day-based.

**Scope:**
- `GameClock` autoload: 8 phases × 3 in-game hours, stepped (GDD §4); step length as a named constant (3 min per 3-hour step = 60 s per in-game hour, Harvest Moon pace → 17-minute playable day 04:30 → 21:30, GDD §4 v0.33)
- Day counter + season (Spring only in this phase; season length constant = 30 days)
- HUD clock dial (sun/moon marker + digital readout; readout ticks every 15 in-game minutes — owner 2026-11-20)
- Sleep at Kit's bed → day rollover, clock to the **04:30** wake-up; **automatic bedtime at 21:30** (Kit goes home, no penalty; owner reset 2026-11-20, was 06:00 / 02:00)
- `DayEnded` / `DayStarted` signals other systems subscribe to
- Clock stops in the pause menu and never advances while the game is closed

**Acceptance criteria:**
- Clock steps through all 8 phases; HUD dial matches
- Sleeping or reaching 21:30 starts the next day at 04:30
- Signals fire exactly once per rollover

**Team Preparation:**
- Clock dial placeholder icons; bed placeholder

**Tests:**
- **Software:** time-scaled run steps through phases; sleep and 21:30 both roll the day once; pause stops time; restart does not advance time
- **Human:** player understands the dial and the bed without explanation

**AI assistance:** Prompt for "Godot 4.x C# stepped day clock autoload with day rollover signals."

**Dependencies:** Task 1.2.

**Notes & learnings:**
- **Outcome / status:** 2026-11-20 — **Done** (owner playtest 2026-11-20: approved). Software tests: PASS — `tools/verify.sh`: `dotnet build` 0 warnings/0 errors, import + 120-frame boot clean (no errors in the log), `TestRunner` **187/187** (75 new: readout ticks every 15 in-game minutes = 15 s and not before; 04:30 → 18:00 = 810 s; design constants; fresh clock = Spring Day 1 04:30; time-scaled run 04:30→07:30→…→19:30 with one `StepChanged` per step in order; 21:30 bedtime fires once, starts sleep, time frozen while asleep; `DayEnded`/`DayStarted` exactly once per rollover for both bed and bedtime, even when `BeginSleep`/`CompleteRollover`/`Wake` are called twice; bed prompt stops the clock and "not yet" restarts it; 100 s hitch counts as 0.25 s; held clock stands still; calendar 30 → Spring 30, 31 → Summer 1, 121 → Spring 1 year 2; restarted clock starts fresh; autoload registered and Pausable; dial follows the clock and draws all 8 phases with sun by day / moon by night; tree pause stops the clock and unpause resumes it; in the real Main scene: bed prompt only when near, Space and gamepad A open "Go to sleep?", Kit freezes, B / "Not yet" closes it, clicking the bed walks Kit there and asks, "Sleep" → Day 2 04:30 with the HUD updated, 21:30 far from home → Kit wakes beside the bed on Day 3 at 04:30 with the camera snapped; button-prompt glyphs per pad family). A `--write-movie` frame check confirmed the dial, the bed prompt and the question panel draw (temporary capture scene deleted afterwards). · Human playtest: **PASS** by the owner (F5, unembedded). Still worth a check with a real gamepad and a 6-year-old when available.
- **What worked well:** Keeping the clock's rules separate from visuals: `GameClock.Tick(delta)` is public and tests make private clocks (`new GameClock()` added with `SetProcess(false)`) so a full day runs in seven calls with no waiting. Time runs continuously underneath and only the *display* is stepped, which keeps the bedtime and 45 s per in-game hour exact. One sleep flow for both bed and bedtime (`BeginSleep` → `CompleteRollover` → `Wake`) with guards makes "exactly once" a property of the clock, not of the callers. Named handler methods plus `-=` in `_ExitTree` for every subscription to the autoload, because tests instance `Main.tscn` several times. A throwaway GDScript scene + `--write-movie` is a quick way to look at a UI state for one frame without adding test code.
- **Problems & fixes:** (1) `DisplayServer.KeyboardGetKeycodeFromPhysical` logs `ERROR: Not supported by this display server` in headless runs → `ButtonPrompt.KeyLabel` uses the physical keycode when `DisplayServer.GetName() == "headless"`. (2) The "Sleep" button ignores presses for 200 ms after opening so the A/Space press that opened the question cannot answer it; at first "Not yet"/B had the same guard and a fast headless test closed nothing → only "Sleep" is guarded now ("Not yet" never loses anything). (3) The bedtime is not a step boundary: steps are counted from the 04:30 wake-up (04:30, 07:30 … 19:30), so the dial stays on "19:30" (Dusk) until 21:30 arrives 1 min 30 s of real time later. `ShownTime` therefore formats hours *and* minutes (`GameClock.FormatTime`). (4) **Night (22:30–01:30) and Small hours (01:30–04:30) are never played** because bedtime comes first; the dial keeps their wedges and tests draw them via `ClockDialView.Display`. Night-only content (seed shed lit, Moon Petal) needs a new time — raised with the owner. (5) A huge frame delta (window dragged, breakpoint) would skip time → each frame counts at most 0.25 s (`GameClock.MaxFrameSeconds`). (6) A test lambda that checked every `SleepStarted` reason failed once a later bed sleep fired the same signal → record reasons in a list and check the first.
- **Deviations from scope or plan:** (a) **Step length 180 s** (3 min, 60 s per in-game hour — Harvest Moon pace, owner 2026-11-20 after trying 135 s; `GameClock.RealSecondsPerStep`), playable day 04:30 → 21:30 = 17 in-game hours = 17 real minutes (GDD §4 v0.33); season length 30 (`SeasonLengthDays`). Seasons roll over generally (day 31 = Summer 1) though Phase-001 only has Spring content and a spring icon; nothing reads the season yet. (b) **"Go to sleep?" question** (Sleep / Not yet) when using the bed, so a stray A press can't end the day by accident; it follows controls.md §6 ("clear visual button") and the rule that the clock stops while any menu is open. **Owner decision 2026-11-20: keep the question.** (c) **Interaction groundwork** (needed to reach the bed, per the Task 1.2 handoff): `scripts/core/IInteractable.cs` (group `interactables`, click areas on physics layer 8) and `PlayerController` now reads `interact` — Space / A uses the nearest interactable in reach; a left-click on one walks Kit there, then uses it; any other click is still a ground click. `PlayerController.Frozen` stops Kit during the question and the night. (d) **Button prompts** per action (`scripts/ui/ButtonPrompt.cs`): keyboard key cap, or the gamepad's bottom face button drawn as Xbox "A", PlayStation cross, or Nintendo "B", chosen from the last device used and the pad's name. (e) **Placeholder art drawn for this task** (the two Kenney packs in use have no bed, sun or moon): `assets/placeholder/handmade/ph_bed.png`, `ph_clock_sun.png`, `ph_clock_moon.png`, `ph_icon_sleep.png`, `ph_season_spring.png`; `SpriteLibrary` now searches `placeholder/{kenney,handmade}`. Final art drops in by name. (f) The bed stands in `PlaceholderLevel` at (-320, -224); "Kit goes home" at 21:30 means Kit wakes at the bed's `WakeSpot` (fade, no walking). Task 1.5's real level should move the bed into Kit's house. (g) Night card: 0.8 s fade to a night colour with the sleep icon, 1.2 s season icon + "Day N", 0.8 s fade in (`DayTransitionView` exports). (h) HUD clock in the top-right corner: 8-wedge dial (dawn rises on the left, noon at the top, night at the bottom; current wedge bright, others dimmed; marker jumps per phase), a readout that **ticks every 15 in-game minutes** ("07:45", owner 2026-11-20; `GameClock.ReadoutMinutes`, signal `TimeChanged(int minuteOfDay)`), spring icon + "Day N". (i) **Debug builds only:** F2 = next step, F3 = 20× clock speed, F4 = jump to 21:20 (`src/debug/ClockDebugKeys.cs`, added by `Main`); raw keys on purpose, developer tools only. (j) **Owner decisions 2026-11-20 (after the build):** day reset to **wake-up 04:30, forced bedtime 21:30** (after trying 01:00 and midnight) and marked decided (GDD v0.31; `GameClock.DayStartMinute` / `BedtimeMinute`; the GDD/mechanics phase tables now start at 04:30); Small hours stays a dial-only phase; keep the "Go to sleep?" question.
- **Handoff to Task 1.4:** Clock: autoload `GameClock` (`res://scripts/autoloads/GameClock.cs`, namespace `MyFarmingGame.Autoloads`), use `GameClock.Instance`. Day logic belongs on **`DayEnded(int day)`** (fires once while the screen is dark, before the day number changes: growth, watering/hunger counts, shipping) and **`DayStarted(int day)`** (new day at 04:30: deliveries, orders). Also `TimeChanged(int minuteOfDay)` (every 15 in-game minutes), `StepChanged(int phase)`, `BedtimeReached`, `SleepAsked`, `SleepAskCancelled`, `SleepStarted(int reason)`, `WokeUp`. Subscribe from C#: `GameClock.Instance.DayEnded += OnDayEnded;` with a **named method**, and `-=` it in `_ExitTree`. Read `Day`, `DayOfSeason`, `Season`, `Year`, `Phase`, `ShownTime`, `MinutesIntoDay`, `IsRunning`. A menu (the Bag) must stop time: `GameClock.Instance.Hold("bag")` / `Release("bag")`, and set `PlayerController.Frozen`. Save/load (Task 1.10) saves `Day` + `MinutesIntoDay` and restores with `SetTime(day, minutes)`; new game = `ResetForNewGame()`. Interaction: anything Kit can use implements `IInteractable`, joins group `interactables`, has an `Area2D` on layer 8 for clicks (see `Bed.tscn`); `interact` currently does nothing when no interactable is in reach — Task 1.4/1.5 adds "use the active tool on the tile in front of `Facing`" as the fallback in `PlayerController._UnhandledInput`. Use `ButtonPrompt` (with `Action = "..."`) for any on-screen button hint; never hard-code a letter. Tests: `TestRunner` uses private clocks for rule checks and the autoload for in-game checks; call `GameClock.Instance.ResetForNewGame()` before and after.

---

### Task 1.4 — Tool Belt & Simple Bag

**Artifact:** On-screen tool belt and a simple auto-stacking bag with a wallet readout.

**Scope:**
- `ToolBelt` autoload + `ToolBelt.tscn`: slots for hands, hoe, watering can, seed bag, feed scoop (scoop appears after buying hens)
- Cycle: gamepad X / LB+RB, mouse wheel; active slot highlighted
- `Inventory` autoload: auto-stacking items (berries, carrots, eggs, seeds); simple bag panel (Y / on-screen button)
- `Wallet` autoload: coins, never below 0; HUD coin icon + number; starting 500 coins (economy.md)

**Acceptance criteria:**
- Cycling changes the active tool; active tool persists
- Adding the same item stacks with a number badge
- Wallet can add and spend, never goes below 0

**Team Preparation:**
- Placeholder icons for tools, items, coin

**Tests:**
- **Software:** cycling works with both input schemes; stacking; wallet floor at 0
- **Human:** icons distinguishable at a glance

**AI assistance:** Prompt for "Godot 4.x C# tool belt, auto-stacking inventory, and wallet autoloads."

**Dependencies:** Task 1.2.

**Notes & learnings:**
- **Outcome / status:** 2026-11-20 — **Done** (owner playtest 2026-11-20: approved). Software tests: PASS — `tools/verify.sh`: `dotnet build` 0 warnings/0 errors, import + 120-frame boot clean, `TestRunner` **275/275** (88 new. Rules on private instances: belt order hands → hoe → can → seed bag → scoop, Next/Prev wrap both ways, one `ActiveToolChanged` per change, unowned tools skipped and unselectable, removing the active tool falls back to hands, hands can't be removed; new bag holds the 9 starting carrot seeds; bag stacks the same item into one slot, separate slots in arrival order, quality stacks separately, bad adds ignored, remove is all-or-nothing, emptied stacks disappear, 1000 items fit; wallet starts at 500, earns, spends, refuses overspending and negative amounts, 500 random earns/spends never go below 0; the three autoloads are registered and Pausable. In the real Main scene: 5 belt slots with icons, hands highlighted, HUD shows 500; gamepad X and RB = next, LB = previous with wrap; mouse wheel down/up over the world and over the belt; a fast wheel burst moves one slot; clicking a slot selects it and is not a ground click; gamepad Y opens the bag (clock held, Kit frozen, close button focused), badges "3" carrots + "3" berries in 8 slots, wallet 545 in bag and HUD, a carrot added while open → badge "4" in the same slot, cycling ignored while open; Y, B, right-click, Esc, the close button and A on it all close it and the clock resumes; the on-screen bag button opens it; it won't open during "Go to sleep?"; gamepad A answers "Sleep" after the guard; after sleeping the active tool, bag and wallet are unchanged; prompt labels LB/RB, L1/R1, Switch X, "Wheel"). A `--write-movie` frame confirmed the belt, wallet, bag button and an open bag with badges draw (temporary capture scene deleted). · Human playtest: **PASS** by the owner (F5). Still worth a check with a real gamepad and a 6-year-old (icons at a glance; the can and scoop placeholders look alike) when available.
- **What worked well:** The Task 1.3 pattern again: autoload state with private test instances (`new ToolBelt()` etc.; only the root child sets `Instance`), views that only subscribe with named handlers and unsubscribe in `_ExitTree`. Holding the clock with `GameClock.Hold("bag")` needed no clock changes. Building the belt slots and bag slots in code from the autoload state (scenes hold only the layout) keeps them right when tools or items change. A pure-Python PNG writer (zlib + struct, no PIL on this Mac) draws the placeholder icons.
- **Problems & fixes:** (1) **Gamepad A/B did nothing on focused menu buttons.** Godot 4.7's built-in `ui_accept` is only Enter / Keypad Enter / Space and `ui_cancel` only Esc, so the Task 1.3 "Go to sleep?" buttons could not be confirmed with a gamepad (only keyboard/mouse). Fix: `ui_accept` and `ui_cancel` are now overridden in `project.godot` with gamepad A (button 0) and B (button 1) added, keys as physical keycodes; a new test confirms A answers "Sleep". A button only fires on a release whose press it also saw, so the A press that opens a menu can't also click its focused button. (2) The bag's dimmed backdrop (a `ColorRect`, mouse Stop) ate right-clicks before `_UnhandledInput` → the bag reads its close inputs in `_Input` while open. (3) The mouse wheel over a belt slot is consumed by the GUI (in Godot 4 even `MOUSE_FILTER_PASS` marks it handled) → slots forward wheel events from `GuiInput`. (4) One wheel flick can send several ticks → at most one tool change per 120 ms from the wheel (`ToolBeltView.WheelRepeatMsec`). (5) `Container`s reset a child's `Scale`, so "raise the active slot" with Scale did nothing → the active slot uses a light background and a thick white outline instead. (6) `ERROR: Condition "!is_inside_tree()"` when the game quit with the bag open (`ReleaseFocus()` from `_ExitTree`) → `_ExitTree` only releases the clock hold. (7) `ToString("N0")` follows the Mac's locale (Indonesian would print "1.000") → invariant culture, "1,750". (8) Test-only: `Input.ParseInputEvent` with only `PhysicalKeycode` set does not match an action bound by `keycode` (Godot's default Esc) → project bindings stay physical.
- **Deviations from scope or plan:** (a) **Feed scoop on the belt from Day 1** (items.md v0.9): Kit starts with hens and a cow, so the starting belt has all five tools (`ToolBelt.StartingTools`); `Give`/`Take` exist for later tools. **Starting bag = one carrot packet, 9 seeds** (items.md v0.9, `Inventory.StartingItems`). (b) **Inputs:** bag = new action `open_inventory` (gamepad Y, the top face button on every pad; was "deferred" in project-structure.md §7) plus an on-screen bag button bottom-right; **no keyboard key** (owner decision 2026-11-20: not needed). Close = Y, B, right-click, Esc, or the Close button (focused, so A/Space/Enter press it). `cycle_tool_next` binding order changed to RB, X, wheel down so the belt prompt shows RB (X still works); `cycle_tool_prev` = LB, wheel up. The LB+RB chord remains "LB = previous, RB = next". (c) **Belt layout:** bottom centre, 72 px picture slots, LB prompt on the left and RB on the right (keyboard/mouse shows "Wheel"); slot tooltips name the tool; clicking a slot selects it (controls.md §4 "on-screen belt buttons"). (d) **Bag:** centred panel with the bag icon, coin readout, a 4-column grid with at least 8 slots (empty ones drawn faintly, so it never looks full; inventory.md "start ≈ 8"), count badge bottom-right of each slot. No categories, quality stars or item actions yet; only ★ items exist in Phase-001. Items: `berries`, `carrot`, `egg`, `milk`, `carrot_seeds` (`ItemIds`). (e) **Wallet HUD** top-left (coin + amount, `WalletHud.tscn`); the bag shows it too. New game: 9 carrot seeds in the bag and 500 coins. (f) `ButtonPrompt` additions: shoulder buttons drawn as a pill (LB/RB, L1/R1, L/R), "Wheel" for wheel bindings, `HideWithoutKey` (hides a keyboard prompt for actions without a key). (g) **Placeholder art drawn for this task** (`assets/placeholder/handmade/`): `ph_tool_hands`, `ph_icon_bag`, `ph_item_berries`, `ph_item_carrot`, `ph_item_egg`, `ph_item_milk`, `ph_item_carrot_seeds` (32×32). The Kenney watering can and feed scoop tiles look much alike; that is for the human icon test. (h) **Debug builds only:** F6 = +1 of every item, F7 = +100 coins, F8 = spend 100 (`src/debug/BagDebugKeys.cs`, added by `Main`). (i) `Main` freezes Kit while the bag is open (`BagView.OpenChanged`).
- **Handoff to Task 1.5:** Autoloads (namespace `MyFarmingGame.Autoloads`, all Pausable): **`ToolBelt.Instance`** — `Active` (`Tool.Hands/Hoe/WateringCan/SeedBag/FeedScoop`), `ActiveToolChanged(int tool)`, `ToolsChanged`, `Tools`, `Owns`, `Select`, `Next`, `Prev`, `Give`, `Take`, `ResetForNewGame`, `SpriteName(tool)`. **`Inventory.Instance`** — `Add(id, count = 1, quality = 1)` (returns the new stack size; always fits), `Remove(id, count = 1, quality = 1)` (all or nothing, returns bool), `Count`, `CountAll`, `Has`, `Stacks` (`ItemStack{Id, Quality, Count}` in arrival order), `Changed`, `Clear`, `ResetForNewGame` (back to `StartingItems` = 9 carrot seeds); ids in `ItemIds` (`berries`, `carrot`, `egg`, `milk`, `carrot_seeds`), icon name `ItemIds.SpriteName(id)` = `item_<id>`. **`Wallet.Instance`** — `Coins`, `Earn(n)`, `Spend(n)` (false and nothing spent if Kit can't afford it), `CanAfford`, `SetCoins` (save/load, clamped at 0), `CoinsChanged(int)`, `StartingCoins = 500`, `ResetForNewGame`. Scenes: `HUD/UI/ToolBelt` (`ToolBeltView`), `HUD/UI/Bag` (`BagView`: `Open`, `Close`, `IsOpen`, `OpenChanged(bool)`, hold reason `"bag"`), `HUD/UI/WalletHud` (`WalletView`). Task 1.5 adds "use `ToolBelt.Instance.Active` on the tile in front of `Facing`" as the fallback when `interact` finds no interactable (`PlayerController._UnhandledInput`); harvesting calls `Inventory.Instance.Add(ItemIds.Carrot)` and planting `Remove(ItemIds.CarrotSeeds)`. The 9 starting carrot seeds are already in the bag on a new game. Tool cycling and the bag are ignored while the clock is stopped by a menu or the night; keep any new menu on `GameClock.Hold(...)` so this stays true. Save/load (1.10): belt owned tools + active tool, inventory stacks, wallet coins.

---

### Task 1.5 — Clearing, Planting & Day-Based Growth

**Artifact:** The real farm level for the slice, with front-garden tiles that can be cleared, tilled, planted, watered, grown over days, and lost to neglect.

**Scope:**
- **Farm level** replacing `PlaceholderLevel`, laid out per places.md §3 (centre column only, §1 above): house with the front garden and old coop facing the gate, the cross path, the old barn behind the house with cleared ground around it, the gate to the lane; overgrown fields shown but blocked. Coop and barn are placed as props here (animals come in Task 1.9)
- **Bed moved into Kit's house** (Task 1.3 handoff); the forced-bedtime wake spot follows it
- **Indoors** (§1, GDD §4): an indoor zone per building footprint — Kit inside → roof fades out and `GameClock` stops; outside → roof back, clock runs. Built for the house here; Tasks 1.6, 1.8 and 1.9 reuse it for the seed shed, coop and barn
- Front garden 8 × 6 tiles (buildings.md §2): 3 beds clear, the rest weeds (cleared by hand). No stones in the front garden (stones start in Field A)
- Starting seeds: **one carrot seed packet (9 seeds)** in the bag (owner 2026-11-20; items.md §2), enough for the 5-carrot order
- Clear weeds (hands) → till (hoe) → plant (seed bag) → water (can)
- **Day-based growth:** on `DayEnded`, each crop watered that day grows one day; carrots take 3 days (crops.md)
- **Neglect:** 2 dry days in a row → yellow sprite; 3 → crop removed, tile stays tilled; watering resets the count
- Harvest ready carrots → Inventory
- Rain: not in this phase (deferred to Phase-002)

**Acceptance criteria:**
- Full cycle clear → till → plant → water → grow → harvest works
- Unwatered crops yellow after 2 days and disappear after 3; watering at yellow recovers
- Nothing changes while the game is closed
- Kit can walk from the house to the barn and to the gate; the bed is in the house; the clock stops inside the house and runs again outside

**Team Preparation:**
- Placeholder sprites: weeds, tilled soil, dry soil, wet soil, carrot stages, yellow carrot, house (walls + separate roof), old coop, old barn, gate, path, overgrown-field edge

**Tests:**
- **Software:** scripted day rollovers verify growth only on watered days, yellow at 2, removal at 3, reset on watering; entering/leaving the house stops/restarts the clock; Kit starts with 9 carrot seeds
- **Human:** player reads yellow as "needs water"; losing a crop feels fair; the faded roof reads as "inside"

**AI assistance:** Prompt for "Godot 4.x C# farm tile grid with till/plant/water and day-based growth with a dry-days counter, plus an Area2D indoor zone that fades a roof and pauses a game clock."

**Dependencies:** Tasks 1.3, 1.4.

**Notes & learnings:**
- **Outcome / status:** 2026-11-21 — **Done — pending human playtest.** Software tests: PASS — `tools/verify.sh`: `dotnet build` 0 warnings/0 errors, import + 120-frame boot clean, `TestRunner` **348/348** (new: crops.tres carrot = 3 days; Kit starts with 9 carrot seeds; garden 8×6 with 3 tilled beds and 45 weed tiles; hoe/seeds refused on weeds, hands clear, hoe tills, seed bag plants and uses one seed, no double planting; growth only on watered nights; 1 dry day green, 2 yellow, watering at yellow recovers at once; 3 dry days → crop gone, tile stays tilled, one `CropLost`; a ready carrot never dries; any tool harvests into the bag; no seeds → nothing; `CropTile` dict round-trip; indoors stops a clock but keeps it playable. In the real Main scene: FarmLevel with house, 4×3 coop, 6×5 barn and front garden; bed inside the house; Kit starts outside with the clock running; inside the house/coop/barn the clock stops and the house roof fades out, outside it comes back; Kit walks door → gate (gate shut), around the house → barn → into the barn; overgrown Field A blocks; gamepad A / Space with hands, hoe, seed bag and can clear → till → plant → water the highlighted tile in front; a mouse click on a weed tile walks Kit there and clears it; a night grows the watered carrot through `DayEnded`; Kit wakes in the house). Existing Task 1.3 sleep tests updated for the bed now being indoors. A `--write-movie` frame confirmed the garden, beds, path, coop and house roofs draw below/above Kit as intended. · Human playtest: **PENDING** (yellow reads as "needs water"; losing a crop feels fair; the faded roof reads as "inside").
- **What worked well:** Building the farm from a small layout table in `FarmLevel.cs` (fields, paths, gate in tiles) so the drawing (`FarmGround`) and the blocking bodies can never disagree. One reusable `Building` node (floor, walls with a door gap, roof, `IndoorZone`) for house, coop and barn; Tasks 1.6/1.8/1.9 reuse it. Garden rules on a plain `FarmGrid` node with `ListenToClock = false` for rule tests (no autoload needed). A repeatable placeholder generator (`tools/make_placeholders.py`, pure Python) replaces one-off icon scripts.
- **Problems & fixes:** (1) `GD0102: The type of the exported member ... StartingBeds is not supported` — C# arrays of `Vector2I` can't be exported → `Godot.Collections.Array<Vector2I>`. (2) **The indoor clock stop broke everything that checked `GameClock.IsRunning`** to mean "no menu open" (the bed, tool cycling, debug keys) — the bed is indoors, so it could never be used. Fix: new `GameClock.IsPlayable` (no menu, prompt or night) for "may Kit act?", and `IsRunning` = playable and not indoors; indoors is tracked separately (`EnterIndoors`/`ExitIndoors`, signal `IndoorsChanged`), not as a menu `Hold`. Any new "can Kit do this?" check should use `IsPlayable`. (3) A test check placed after the night ran while Kit was already back in the house → indoor-dependent checks must run right after moving Kit, before sleeping. (4) The old sleep tests teleported Kit to spots that are now walls or overgrown field → they now use `PlayerSpawn` and an in-house offset.
- **Deviations from scope or plan:** (a) **Layout** (tiles, 64 px, farm top-left = level origin, 50 × 32): Field D (0,0,16×12), Field C (34,0,16×12), front garden (16,1, 8×6), path gate → door (24,0, 2×8), house 8×4 at (21,8) door on top, old coop 4×3 at (28,2) door at the bottom, cross path (0,12, 50×2), path to the barn (24,14, 2×11), **Field A drawn in two halves** (20–24 and 26–32) because the 2-tile path to the barn runs through it, old barn 6×5 at (22,25) door on top with cleared ground around it, Field B (32,14), Field E (0,14, 20×16), animal yard left clear. **The gate is shut** in Task 1.5 (`FarmLevel.GateClosed`); Task 1.6 opens it onto the lane. (b) **"3 beds already clear" = 3 tilled 1×1 beds** (buildings.md "Tilled bed 1×1"; onboarding step 3 plants straight into them) at garden cells (5,3), (6,3), (7,3), next to the path. Clearing order for the other 45 tiles: weeds → hands → clear → hoe → tilled. (c) **Any tool harvests a ready crop**, and **a ready crop never dries out** (docs are silent; kinder for 6+, and a finished crop has nothing left to grow). (d) The seed bag plants the first seed packet in the bag that `crops.tres` knows (only carrots exist now). Watering cans have unlimited water (no refill in the docs for Phase-001). (e) No `IndoorZone.tscn`: `Building` creates its `IndoorZone` in code over the floor; the roof fades to alpha 0 in 0.25 s. (f) Crop data in `data/crops.tres` (`CropCatalog` + `CropDefinition` resources in `scripts/core/`). (g) New sprites (handmade placeholders): `soil_weeds/clear/tilled/wet`, `crop_carrot_seed/sprout/growing/ready/yellow`, `roof_house/coop/barn`, `floor_wood/straw`, `ground_path/overgrown`, `gate`. `PlaceholderLevel.tscn` deleted. (h) A white square highlights the garden tile in front of Kit (helps gamepad players see what A will hit).
- **Handoff to Task 1.6:** Level `res://scenes/world/FarmLevel.tscn` (`FarmLevel.cs` layout constants: `Farm`, `OvergrownFields`, `Paths`, `Gate`; `GateClosed` export → set false and add the lane north of the gate, y < 0; the Background already covers up to y = −2560). Buildings: `Building` (`SizeTiles`, `Door` Top/Bottom, `ZoneName`, `FloorSprite`, `RoofSprite`, `DoorOutside`, `DoorInside`, `Interior`, `Zone`, `Roof`) — the seed shed is one more `Building` with `ZoneName = "shed"`. Clock: `GameClock.IsPlayable` (can Kit act) vs `IsRunning` (time moves); `EnterIndoors/ExitIndoors`, `IsIndoors`, `IndoorsChanged(bool)`. Conversations must still `Hold(...)` the clock (stops it outdoors too). Garden: `FarmGrid` at `FarmLevel/FrontGarden` — `Get(cell)`, `AllTiles()`, `UseTool(tool, cell)` → `TileAction`, `EndDay()`, `ResetForNewGame()`, `SetTiles(list)`, signals `TileChanged`, `CropLost`, `CropHarvested(cell, item)`; tile data `CropTile` (`Soil`, `Watered`, `CropId`, `DaysGrown`, `DaysToHarvest`, `DryDays`; `ToDict/FromDict` for Task 1.10). Player: `PlayerController.FeetOffset`, `FrontPoint`, `Face(f)`, `UseToolInFront()`, `WalkToTile`; anything else Kit uses tools on implements `IToolTarget` (group `tool_targets`). Interactables still win over tiles when in reach.

---

### Task 1.6 — Mr. Marlow, Foraging & the First Order

**Artifact:** Marlow at the seed shed with picture-bubble dialogue, berry bushes on the lane, and the first order paid.

**Scope:**
- `Marlow.tscn` with picture bubble (icon + one short line; read-aloud hook for later)
- Berry bushes on the lane: pick → berries in Inventory (regrow after 2 days)
- First order: "3 handfuls of wild berries" → deliver → coins (economy.md price ×1.25 order premium, pending approval — use a named constant)
- Decline/back with B / right-click; Marlow restates unfinished orders kindly
- Ledger prop at the shed (picture pages; `Ledger.Note()` API) — kept from v0.7
- The lane from the farm gate to Marlow's cottage and seed shed (≈ 20 tiles, places.md §2.1); the seed shed uses the Task 1.5 indoor zone (clock stops inside); the clock also stops while a conversation is open (GDD §4)

**Acceptance criteria:**
- Accept, decline, and deliver all work; coins added once
- Declining leaves the order available

**Team Preparation:**
- Placeholder Marlow sprite, berry bush, order card icons

**Tests:**
- **Software:** order states (offered → accepted → delivered → paid); payment exactly once
- **Human:** a 6-year-old understands the request and the reward from the bubble

**AI assistance:** Prompt for "Godot 4.x C# NPC picture-bubble dialogue with a simple order offer/accept/deliver flow."

**Dependencies:** Tasks 1.4, 1.5.

**Notes & learnings:**
- **Outcome / status:** _TBD_
- **What worked well:** _TBD_
- **Problems & fixes:** _TBD_
- **Deviations from scope or plan:** _TBD_
- **Handoff to Task 1.7:** _TBD — order data shape_

---

### Task 1.7 — Order Board & Repeat Orders

**Artifact:** The order board at the seed shed with orders matched to what Kit can produce.

**Scope:**
- `OrderManager` autoload: generates orders each Dawn from what Kit can currently produce (unlocked crops, owned hens and cow — eggs and milk), at quantities the farm can supply
- Board UI: picture cards (item icon × count, coin reward)
- Accept / decline / cancel any time, no penalty, no deadline
- Second scripted order: Marlow's "5 carrots"
- Repeat orders: after 3 delivered orders from one neighbour, that neighbour offers a repeating order; quantities grow slowly

**Acceptance criteria:**
- Never generates an order for something Kit cannot produce
- Repeat and slightly bigger orders appear after deliveries

**Team Preparation:**
- Board placeholder; order card layout

**Tests:**
- **Software:** generator only uses producible items (unit test over many seeds); repeat logic
- **Human:** cards are readable at a glance

**AI assistance:** Prompt for "Godot 4.x C# order generator constrained by player capabilities."

**Dependencies:** Task 1.6.

**Notes & learnings:**
- **Outcome / status:** _TBD_
- **What worked well:** _TBD_
- **Problems & fixes:** _TBD_
- **Deviations from scope or plan:** _TBD_
- **Handoff to Task 1.8:** _TBD_

---

### Task 1.8 — Seed Shop & Buying

**Artifact:** Buying seeds and extra hens at the seed shed and ordering feed by phone from home, at half real price (economy.md); the old coop and barn hold their starting animals and feed.

**Scope:**
- Shop UI at the seed shed: carrot and flower seed packets (20 each, packet of 9), laying hen (**175**, only while the coop has room)
- **Shop hours:** the seed shed is open **07:30–16:30** (opens with Morning, closes as Evening begins — GDD §4 phase table); outside hours Marlow's picture bubble shows a closed sign and the opening time. The shop screen stops the clock (GDD §4)
- **Phone at home** (feed seller's number left by Kit's parents on a note; inside the house, so the clock is already stopped): chicken feed (1 per portion), hay (25 per cow portion); orders before 18:00 are delivered to the coop/barn feed bins at the next Dawn (after 18:00 → the day after); no fee
- The **old coop** (4 × 3, 5 hens) and **old barn** (6 × 5, 5 stalls, 1 cow) start with ≈ 7 days of feed in their bins (35 chicken portions + 7 hay portions); **no buildings or cows are bought in Phase-001** (Phase-002)
- Prices read from `docs/data/economy.csv` (or a `.tres` generated from it)
- Coop capacity 5 hens; buying more is blocked with a friendly "coop is full" icon

**Acceptance criteria:**
- Buying deducts coins and adds items or a hen; can't buy without enough coins
- Capacity enforced (coop full at start, so hens can only be bought after a loss)
- The shop only sells inside its hours; the phone works any time Kit is home

**Team Preparation:**
- Placeholder shop panel, phone and feed-seller note, feed-bin icons, closed sign

**Tests:**
- **Software:** purchases, wallet floor, capacity cap, prices match `economy.csv` (hen 175, seeds 20, chicken feed 1, cow hay 25); shop closed before 07:30 and from 16:30; feed order before/after 18:00 arrives on the right Dawn into the right bin
- **Human:** prices and "not enough coins" are understandable; the closed sign is clear

**AI assistance:** Prompt for "Godot 4.x C# shop reading prices from CSV, with opening hours and a delayed phone delivery into feed bins."

**Dependencies:** Tasks 1.4, 1.5.

**Notes & learnings:**
- **Outcome / status:** _TBD_
- **What worked well:** _TBD_
- **Problems & fixes:** _TBD_
- **Deviations from scope or plan:** _TBD_
- **Handoff to Task 1.9:** _TBD_

---

### Task 1.9 — Hens, Cow, Eggs, Milk & the Loss Rule

**Artifact:** The starting 5 hens and 1 cow: daily feeding, eggs and milk, and loss after 3 unfed days.

**Scope:**
- Hen and cow entities in the old coop and barn; feed with the feed scoop from the building's feed bin (grain for hens, hay for the cow), once per day
- Coop and barn use the Task 1.5 indoor zone (clock stops inside)
- Fed today → next morning 1 egg per hen, 4 milk cans from the cow (★ quality only in this phase)
- **Heart meter** per animal, no numbers (animals.md §3): full when fed today, ⅔ at 1 unfed day, ⅓ at 2
- The same rule for **every animal, hens and the cow**: 1 unfed day → need icon (no product tomorrow); 2 → flashing icon + an Ada or Marlow line (Ada's line is a picture bubble without her sprite until Task 1.12 adds her); 3 → the animal is gone + **message card with gravestone picture**, its name, and a tip, dismissed with one button
- Egg and milk orders become available from Day 1 (Task 1.7 generator)

**Acceptance criteria:**
- Feeding resets the count; eggs and milk appear only after fed days
- Loss card appears once per lost animal (hen or cow); no on-screen death; nothing else is removed

**Team Preparation:**
- Placeholder hen, cow, egg, milk can, feed scoop, feed bins, heart meter, gravestone card illustration

**Tests:**
- **Software:** scripted rollovers verify egg and milk production, the heart meter, and the 3-day loss for a hen and for the cow; card shown once per animal
- **Human:** the loss card is gentle, not frightening, for a 6-year-old; the warning is noticed

**AI assistance:** Prompt for "Godot 4.x C# animal with daily feeding counter and production."

**Dependencies:** Tasks 1.3, 1.8.

**Notes & learnings:**
- **Outcome / status:** _TBD_
- **What worked well:** _TBD_
- **Problems & fixes:** _TBD_
- **Deviations from scope or plan:** _TBD_
- **Handoff to Task 1.10:** _TBD — what state must be saved_

---

### Task 1.10 — Save / Load (Versioned)

**Artifact:** One-slot auto-save with schema versioning.

**Scope:**
- `SaveGame` autoload; JSON at `user://savegame.json` with top-level `schemaVersion: 1`
- Sections owned by each system (`clock`, `farm`, `inventory`, `wallet`, `orders`, `animals`, `player`, plus `feed` — bin contents and phone orders not yet delivered —, `forage` — berry-bush regrowth —, `ledger`, `onboarding` — guided-start step and shown nudges) via a registration API
- Auto-save at every day rollover and after purchases/deliveries; manual save from pause menu
- Safe write (temp file + rename); previous save kept as `savegame.bak.json`; fall back to it if parsing fails
- A no-op migration chain ready for `v1 → v2`

**Acceptance criteria:**
- Restart restores everything (position, day, tiles, crops, dry counts, inventory, coins, orders, hens and cow, hunger counts, feed bins, pending feed deliveries, berry bushes, ledger pages; onboarding progress once Task 1.12 registers it)
- Corrupt main file → backup loads

**Team Preparation:**
- Agreed save sections (GDD §13)

**Tests:**
- **Software:** round-trip equality; 50× save/load without drift; corrupt-file fallback; migration hook runs
- **Human:** None required

**AI assistance:** Prompt for "Godot 4.x C# versioned JSON save with section registration, atomic write, and backup."

**Dependencies:** Tasks 1.3–1.9. (Onboarding, Task 1.12, registers its section when it is built.)

**Notes & learnings:**
- **Outcome / status:** _TBD_
- **What worked well:** _TBD_
- **Problems & fixes:** _TBD_
- **Deviations from scope or plan:** _TBD — final schema_
- **Handoff to Task 1.11:** _TBD — how new systems register save sections_

---

### Task 1.11 — Pause Menu & Day/Night Visuals

**Artifact:** Pause menu (resume, save, settings, quit) and the lighting shift across day phases.

**Scope:**
- `PauseMenu.tscn`: Resume, Save, Settings (volume, nudges on/off, read-aloud placeholder, break reminder on/off — off by default, GDD §4; the reminder interval is still to be agreed, so only the toggle is built), Quit; calendar (season, day) and the day/night indicator matching the HUD dial (controls.md)
- Lighting tint per day phase from `GameClock`
- Pause stops time
- **Deferred (not in this phase):** the Collection Book and sticker book pages (Phase-002, stickers), and the valley picture map (places.md §5; owner 2026-11-20 — the slice is one area)

**Acceptance criteria:**
- Opens with Start/Esc; every button works; tint follows the phase

**Team Preparation:**
- Menu icons; day and night palettes

**Tests:**
- **Software:** buttons; pause stops clock; tint changes per phase
- **Human:** night is dim but readable; menu icons understandable

**AI assistance:** Prompt for "Godot 4.x C# pause menu and CanvasModulate day/night tint."

**Dependencies:** Tasks 1.3, 1.10.

**Notes & learnings:**
- **Outcome / status:** _TBD_
- **What worked well:** _TBD_
- **Problems & fixes:** _TBD_
- **Deviations from scope or plan:** _TBD_
- **Handoff to Task 1.12:** _TBD_

---

### Task 1.12 — Onboarding Flow

**Artifact:** The ≈ 5-minute guided start and Day 1–7 nudges from [onboarding.md](./onboarding.md).

**Scope:**
- Steps 1–10 of onboarding.md §2 as a small step machine (step 10 = the "feed lasts 7 days — earn coins for more" goal icon)
- **Ada** as a simple solo-play NPC at the house (characters.md §7): waves on arrival, shows the hens and cow (step 9), speaks her nudges in a picture bubble; she never does Kit's tasks. Co-play Ada stays Phase-003
- One-time nudges from onboarding.md §3 that apply to this phase: first evening (sleep), morning after planting (watering), yellow crop, lost crop, carrots ready, feed running low, first morning (collect eggs and milk), first eggs and milk, hens unfed for 2 days. The rain nudge waits for rain (Phase-002)
- Nudges toggle in Settings; the onboarding step and shown nudges register a save section with Task 1.10's API

**Acceptance criteria:**
- Guided start completes in ≈ 5 minutes and never blocks free play
- Each nudge shows once, also across save/load

**Team Preparation:**
- Pointer/arrow icon; short lines for each step; placeholder Ada sprite and the arrival cart prop

**Tests:**
- **Software:** step machine advances on the right actions; nudges fire once; toggle works
- **Human:** a 6–8-year-old finishes the start without help

**AI assistance:** Prompt for "Godot 4.x C# lightweight tutorial step machine with one-time contextual hints."

**Dependencies:** Tasks 1.5–1.11.

**Notes & learnings:**
- **Outcome / status:** _TBD_
- **What worked well:** _TBD_
- **Problems & fixes:** _TBD_
- **Deviations from scope or plan:** _TBD_
- **Handoff to Task 1.13:** _TBD_

---

### Task 1.13 — Playtest & Iterate

**Artifact:** Validated first playable ready for the Milestone 1 exit review.

**Scope:**
- Playtest with players aged 6+ (and adults), with supervision and consent for under-18s (GDD §14)
- Answer the GDD §14 playtest questions; iterate on clarity, timing, and prices

**Acceptance criteria:**
- A new player completes the guided start and the first two orders, buys seeds, orders feed by phone, and keeps the hens and cow fed without adult help
- All tasks 1.1–1.12 complete and integrated

**Team Preparation:**
- Participants, observation sheet, test PC with gamepad
- Fallback: team members simulate and mark findings "needs target-player validation"

**Tests:**
- **Software:** all automated checks from Tasks 1.1–1.12 in one pass; stable frame rate; no runtime errors; save/load end-to-end
- **Human:** all GDD §14 questions answered

**AI assistance:** Prompt for "playtest observation sheet for a kid-friendly farming game, including the GDD §14 questions."

**Dependencies:** All previous tasks.

**Notes & learnings:**
- **Outcome / status:** _TBD_
- **What worked well:** _TBD_
- **Problems & fixes:** _TBD_
- **Playtest findings (GDD §14 answers):** _TBD_
- **Advice for Phase-002 planning:** _TBD_

---

## 3. Exit Criteria for Milestone 1

- [ ] All 13 session tasks are done
- [ ] A new player (6+) completes the guided start and the first two orders with little or no explanation
- [ ] Crops grow by day, yellow at 2 dry days, and are lost at 3; hens and the cow are lost at 3 unfed days with the gentle card
- [ ] Starting hens and cow work (feed, eggs, milk, loss rule); buying seeds at the shed and ordering feed by phone work at economy.md prices
- [ ] The clock stops indoors (house, coop, barn, seed shed) and in every menu
- [ ] Game saves and loads correctly (versioned)
- [ ] Stable frame rate on target PC hardware
- [ ] Placeholder art clearly marked as temporary
- [ ] Notes recorded for Phase-002 planning

---

## 4. What Comes Next (Milestone 2 Preview)

Phase-002 (planned — docs/PHASE.md §3): market stall and shipping box with seasonal and demand pricing, rain, clearing the back fields with hammer/axe/scythe, more neighbours and the baker, sheep or beehive, stickers.

---

## 5. Change log

- **0.1–0.2 — 2026-11-18:** Earlier planning iterations (task breakdown, tests, dependencies).
- **0.3 — 2026-11-18:** GDScript → **C#** throughout (engine decision, docs/PHASE.md §6). Out-of-scope list annotated with the target future phase for each deferred item (product prototype = source of truth). Milestone 2 preview updated for the four care animals; forward reference to Phase-002 marked "(planned)"; day/night & seasons noted as always-on Gentle Core.
- **0.4 — 2026-11-18:** Added a **Notes & learnings** fill-in block to every task (1.1–1.12): outcome, what worked, problems & fixes, deviations, and a task-specific handoff question for the next task — to be completed before starting the next task. Task format list in §2 updated accordingly.
- **0.5 — 2026-11-18:** Task 1.4 growth timing aligned with the Milestone 0 decision (GDD §4, v0.16): ~30–45 s/stage is the **Medium** default of a named Fast/Medium/Slow setting, and growth is **Paused** while the game is closed (sleep grants no extra stage). The pre-game options screen itself is explicitly **not** added to Task 1.4 scope — see the session report for the pending placement question.
- **0.6 — 2026-11-18:** ⚠️ **Reconciliation pending (design vs. phase doc):** Milestone 0 decision 2 (GDD §4 v0.17, mechanics.md §2) defines an 8-phase stepped clock — 3-hour jumps, ≈3 min of play per step, reset to Dawn on sleep — plus an **always-on HUD clock dial**, neither of which Task 1.10/1.11 currently mentions. Task 1.11's undefined "cycle duration" is now effectively 8 steps ≈ 24 min per in-game day. **Not edited into task scope yet:** the human must place the HUD clock (extend Task 1.11 vs. add a task) before these tasks are worked on.
- **0.7 — 2026-11-18:** **Task 1.6 gains the ledger** per world.md §7.4 decision ("both", world-only placement): prop + picture-only close-up + `Ledger.Note()` API for Tasks 1.7/1.8, explicitly absent from the pause menu. Scope, acceptance criteria, team prep (placeholder pages) and software tests updated. This closes the gap where events.md marked "ledger page fills" as Prototype scope but no task owned it.
- **0.8 — 2026-11-18:** **Gap A closed** (controls decision session): Task 1.6 gains the explicit **decline/back input** — gamepad B or right-click — so "no punishment for declining" is actually operable; acceptance criteria and software tests updated. Help tooltip (Back/Select) formally deferred out of Phase-001.
- **0.9 — 2026-11-19:** **Direction reset (GDD v0.19).** Core loop: watering now **required**; plants grow only while moist and, left dry, **turn yellow then are gone** (Normal timing; mode selector deferred). Task 1.4 scope/acceptance/tests/prep rewritten accordingly (no more "plants never wither"; yellow sprite and dry-soil look added). Task 1.6/1.7: Marlow gives replacement seeds for lost task flowers. Task 1.9 saves moisture/dryness timers. **Task 1.1 fixed:** `.gitignore` ignores only `.godot/`, `*.import` sidecars are committed (resolves project-structure.md §14.4); verify — don't change — Compatibility renderer and `MyFarmingGame` assembly. Out-of-scope list and Milestone 2 preview re-mapped to the new phase plan. "Child" wording replaced with player/Kit; playtest participants tied to the pending age-band decision.
- **0.10 — 2026-11-19:** **Re-scoped to a vertical slice (owner decision).** Milestone 1 is now the money loop: clearing, day-based growth with the 3-dry-days rule, foraging, Marlow's first orders, order board with repeat orders, seed shop (half real price), small coop + hens with daily feeding and the 3-unfed-days loss card, versioned save, pause menu, and a ≈ 5-minute onboarding flow. Task list rebuilt (13 tasks): clock moved to 1.3 because growth is day-based; tool belt + bag + wallet (1.4); clearing/planting/growth (1.5); Marlow & first order (1.6); order board (1.7); shop (1.8); chickens (1.9); versioned save (1.10); pause & day/night (1.11); onboarding (1.12); playtest (1.13). Butterfly Garden, garden shelf, and play-time growth removed from this phase. All Notes & learnings blocks reset to _TBD_ (no task had started).
- **0.11 — 2026-11-19:** Old coop pre-placed at game start (owner decision); Task 1.8 now sells seeds and hens only — no coop purchase or placement in this phase.
- **0.12 — 2026-11-19:** Game starts with **5 hens and 1 cow** plus ≈ 7 days of feed (owner decision): Task 1.8 sells seeds, feed, and replacement hens; Task 1.9 covers hens and the cow (eggs, milk); exit criteria updated.
- **0.13 — 2026-11-19:** Task 1.8 adds ordering feed by phone from home (18:00 cut-off, Dawn delivery into feed bins).
- **0.14 — 2026-11-19:** Task 1.3 uses the decided clock (≈ 2 min 15 s per step, 15-minute day) and 30-day seasons.
- **0.15 — 2026-11-20:** Task 1.1 synced with project-structure.md v0.4 (scene-architecture standard agreed by the owner): `Main.tscn` must be built to the §4 layer/process-mode tree (HUD 10 / Pause 20 / Transition 30 / Debug 40, `UILayers` constants, explicit process modes incl. autoloads), `src/shaders/` + `src/debug/` created; new acceptance criterion added. No other task changed.
- **0.16 — 2026-11-20:** **Task 1.1 done** (built to the v0.15 scene-architecture standard) — Notes & learnings filled (Godot 4.7.2 .NET, RollForward for .NET 11, Kenney CC0 placeholders in `src/assets/placeholder/`, InputMap registry bound early, verified headless commands, `tools/verify.sh`); status header moved from Planning draft to In progress.
- **0.17 — 2026-11-20:** **Task 1.2 built — pending human playtest.** Notes & learnings filled: 8-way walking with 4-way facing (owner decision), 160 px/s, click-to-move with `WalkTo` hook, CameraFollow with smoothing, placeholder idle/walk clips, move deadzone 0.2, headless mouse/test gotchas, any-controller gamepad note (PC first, consoles later). Gotcha added: editor game embedding shows a blank grey screen on macOS + Compatibility; run unembedded. Placeholder checkerboard ground added after owner feedback.
- **0.18 — 2026-11-20:** **Task 1.2 Done** — owner playtested and approved movement, speed and camera.
- **0.19 — 2026-11-20:** **Task 1.3 built — pending human playtest.** Notes & learnings filled: 135 s steps (15-minute day), continuous time under a stepped display, 02:00 bedtime 1.5 min into the Night step, Small hours unreachable (raised with the owner), "Go to sleep?" question, interaction groundwork (`IInteractable`, Space / A / click), per-pad button prompts, handmade placeholder icons, debug clock keys, headless keyboard-layout gotcha.
- **0.20 — 2026-11-20:** Owner decisions on Task 1.3: forced bedtime **moved from 02:00 to 01:00** (scope, acceptance, tests and Notes updated; day now 14 min 15 s), Small hours accepted as dial-only, "Go to sleep?" question kept.
- **0.21 — 2026-11-20:** Owner corrected the forced bedtime to **midnight (00:00)**; Task 1.3 scope, acceptance, tests and Notes updated (day now 13 min 30 s; Night is dial-only too).
- **0.22 — 2026-11-20:** Owner reset the Task 1.3 day: **wake-up 04:30, forced bedtime 21:30** (steps counted from 04:30; day 12 min 45 s). Scope, acceptance, tests and Notes updated.
- **0.23 — 2026-11-20:** Task 1.3: HUD time now ticks every 15 in-game minutes (owner); `TimeChanged` signal and 5 new tests; Notes updated.
- **0.24 — 2026-11-20:** Task 1.3 clock speed set to **Harvest Moon pace** (60 s per in-game hour; day 17 min) by the owner.
- **0.25 — 2026-11-20:** §1 notes where the slice sits on the new Bramblewick Vale map (places.md v0.10): farm front garden + the lane to Marlow's shed; no scope change.
- **0.26 — 2026-11-20:** Feed price in the phone-order note updated to the Australian-based economy (economy.md v0.9): hay 25 per cow portion.
- **0.27 — 2026-11-20:** **Task 1.3 Done** — owner playtested and approved the day clock, bed and bedtime.
- **0.28 — 2026-11-20:** **Tasks 1.5–1.13 checked against the design docs** (owner: "docs is source of truth"). §1: feed is phone-ordered from home (economy.md §2), the slice includes the house and the old barn behind it (places.md §3), and a new **Indoors** note — buildings are entered on the same map, the roof fades and the clock stops (GDD §4); no separate interior scenes in Phase-001 (open question for Phase-002). Task 1.5 now builds the real farm level, moves the bed into the house, adds the indoor zone, drops front-garden stones (buildings.md §2) and starts Kit with one carrot packet (9 seeds, owner). 1.6 adds the lane and the shed's indoor zone. 1.7 adds the cow (milk orders). 1.8: hen **175** (was 125, economy.csv), no buildings bought, shop hours 07:30–16:30, starting feed bins. 1.9: heart meter, loss rule for the cow too, Ada or Marlow warning. 1.10: save sections for feed, forage, ledger, onboarding. 1.11: break-reminder toggle; Collection Book, stickers and the picture map deferred (owner). 1.12: all 10 onboarding steps, Ada as a simple NPC, missing nudges added. 1.13 and exit criteria: "buys hens" (impossible with a full coop) → buys seeds and orders feed; cow loss and indoor clock stop added.
- **0.29 — 2026-11-20:** **Task 1.4 built — pending human playtest.** Notes & learnings filled: `ToolBelt`, `Inventory`, `Wallet` autoloads; belt (RB/X next, LB previous, wheel, click a slot), bag (Y or on-screen button; clock held, Kit frozen), HUD wallet (500 start, never below 0); feed scoop owned from Day 1 and 9 starting carrot seeds (items.md v0.9; owner confirmed); no keyboard key for the bag (owner); handmade item icons; gotcha fixed: gamepad A/B now confirm/cancel focused menu buttons (`ui_accept`/`ui_cancel`), which also fixes the Task 1.3 sleep question on a gamepad.
- **0.30 — 2026-11-20:** **Task 1.4 Done** — owner playtested and approved the tool belt, bag and wallet.
- **0.31 — 2026-11-21:** **Task 1.5 built — pending human playtest.** Notes & learnings filled: FarmLevel (house with bed, front garden, old coop, old barn, cross path, shut gate, overgrown fields), `Building` + `IndoorZone` (roof fades, clock stops indoors; `GameClock.IsPlayable` vs `IsRunning`), `FarmGrid` (clear → till → plant → water, day-based growth, yellow at 2 dry days, lost at 3), `crops.tres`; choices: 3 tilled 1×1 starting beds, any tool harvests, ready crops never dry out, Field A split by the barn path.
