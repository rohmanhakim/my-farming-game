# Phase-001 — First Playable (Vertical Slice)

> **Status:** In progress — Task 1.1 done (2026-11-20)  
> **Version:** 0.16  
> **Last updated:** 2026-11-20  
> **Milestone:** 1 — First playable  
> **Purpose:** Break Milestone 1 down into session tasks. Each task produces one scoped, playtested artifact that can be created and validated in a single session with AI assistance.

## 1. Milestone Overview

**Goal (re-scoped 2026-11-19):** a **vertical slice of the money loop** — produce → order → money → invest — in one connected area. Owner decision: narrow the first playable to include orders and chickens.

**Scene:** Kit's front garden → the lane (berry bushes) → Mr. Marlow's seed shed (order board, shop).

**Loop in this milestone:**
1. Kit clears weeds, plants starter carrots, and waters them daily.
2. Marlow's first order: 3 handfuls of wild berries (forage) → coins; the order board opens.
3. Marlow's second order: 5 carrots (3 days of watering) → coins.
4. Crops grow one day per watered night; left dry they turn yellow (2 days) and are gone (3 days).
5. Kit starts with **5 hens and 1 cow** in the family's old coop and barn, plus ≈ 7 days of feed; Kit buys more feed and seeds at the seed shed (half real price).
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
- **Outcome / status:** _TBD_
- **What worked well:** _TBD_
- **Problems & fixes:** _TBD — input mapping issues, camera lerp tuning_
- **Deviations from scope or plan:** _TBD — 4-way vs 8-way, move speed_
- **Handoff to Task 1.3:** _TBD — bound inputs, where the controller lives_

---

### Task 1.3 — Day Clock, Calendar & Sleep

**Artifact:** The in-game day: stepped clock, HUD dial, sleep, and the 02:00 bedtime. Moved early because growth and feeding are now day-based.

**Scope:**
- `GameClock` autoload: 8 phases × 3 in-game hours, stepped (GDD §4); step length as a named constant (≈ 2 min 15 s per 3-hour step → ≈ 15-minute playable day, GDD §4)
- Day counter + season (Spring only in this phase; season length constant = 30 days)
- HUD clock dial (sun/moon marker + digital readout)
- Sleep at Kit's bed → day rollover, clock to 06:00; **automatic bedtime at 02:00** (Kit goes home, no penalty)
- `DayEnded` / `DayStarted` signals other systems subscribe to
- Clock stops in the pause menu and never advances while the game is closed

**Acceptance criteria:**
- Clock steps through all 8 phases; HUD dial matches
- Sleeping or reaching 02:00 starts the next day at 06:00
- Signals fire exactly once per rollover

**Team Preparation:**
- Clock dial placeholder icons; bed placeholder

**Tests:**
- **Software:** time-scaled run steps through phases; sleep and 02:00 both roll the day once; pause stops time; restart does not advance time
- **Human:** player understands the dial and the bed without explanation

**AI assistance:** Prompt for "Godot 4.x C# stepped day clock autoload with day rollover signals."

**Dependencies:** Task 1.2.

**Notes & learnings:**
- **Outcome / status:** _TBD_
- **What worked well:** _TBD_
- **Problems & fixes:** _TBD_
- **Deviations from scope or plan:** _TBD — actual step length_
- **Handoff to Task 1.4:** _TBD — signal names and how to subscribe_

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
- **Outcome / status:** _TBD_
- **What worked well:** _TBD_
- **Problems & fixes:** _TBD_
- **Deviations from scope or plan:** _TBD_
- **Handoff to Task 1.5:** _TBD — APIs for active tool, AddItem/RemoveItem, Spend/Earn_

---

### Task 1.5 — Clearing, Planting & Day-Based Growth

**Artifact:** Front-garden tiles that can be cleared, tilled, planted, watered, grown over days, and lost to neglect.

**Scope:**
- Front garden 8 × 6 tiles (buildings.md §2): 3 beds clear, the rest weeds (hands) and small stones (hammer deferred — in this phase stones are decorative and block tiles)
- Clear weeds (hands) → till (hoe) → plant (seed bag) → water (can)
- **Day-based growth:** on `DayEnded`, each crop watered that day grows one day; carrots take 3 days (crops.md)
- **Neglect:** 2 dry days in a row → yellow sprite; 3 → crop removed, tile stays tilled; watering resets the count
- Harvest ready carrots → Inventory
- Rain: not in this phase (deferred to Phase-002)

**Acceptance criteria:**
- Full cycle clear → till → plant → water → grow → harvest works
- Unwatered crops yellow after 2 days and disappear after 3; watering at yellow recovers
- Nothing changes while the game is closed

**Team Preparation:**
- Placeholder sprites: weeds, stone, tilled soil, dry soil, wet soil, carrot stages, yellow carrot

**Tests:**
- **Software:** scripted day rollovers verify growth only on watered days, yellow at 2, removal at 3, reset on watering
- **Human:** player reads yellow as "needs water"; losing a crop feels fair

**AI assistance:** Prompt for "Godot 4.x C# farm tile grid with till/plant/water and day-based growth with a dry-days counter."

**Dependencies:** Tasks 1.3, 1.4.

**Notes & learnings:**
- **Outcome / status:** _TBD_
- **What worked well:** _TBD_
- **Problems & fixes:** _TBD_
- **Deviations from scope or plan:** _TBD_
- **Handoff to Task 1.6:** _TBD — tile/crop data shape (save task needs it)_

---

### Task 1.6 — Mr. Marlow, Foraging & the First Order

**Artifact:** Marlow at the seed shed with picture-bubble dialogue, berry bushes on the lane, and the first order paid.

**Scope:**
- `Marlow.tscn` with picture bubble (icon + one short line; read-aloud hook for later)
- Berry bushes on the lane: pick → berries in Inventory (regrow after 2 days)
- First order: "3 handfuls of wild berries" → deliver → coins (economy.md price ×1.25 order premium, pending approval — use a named constant)
- Decline/back with B / right-click; Marlow restates unfinished orders kindly
- Ledger prop at the shed (picture pages; `Ledger.Note()` API) — kept from v0.7

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
- `OrderManager` autoload: generates orders each Dawn from what Kit can currently produce (unlocked crops, owned hens), at quantities the farm can supply
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

**Artifact:** Buying at the seed shed: seeds, feed, and extra hens, at half real price (economy.md); the old coop and barn are pre-placed with their starting animals.

**Scope:**
- Shop UI at the seed shed: carrot and flower seed packets, laying hen (125, only while the coop has room)
- **Phone at home** (feed seller's number left by Kit's parents on a note): chicken feed (1 per portion), hay (15 per portion); orders before 18:00 are delivered to the coop/barn feed bins at the next Dawn (after 18:00 → the day after); no fee
- The **old coop** (4 × 3, 5 hens) and **old barn** (6 × 5, 5 stalls, 1 cow) are pre-placed at game start with ≈ 7 days of feed; buying cows and extra buildings is Phase-002
- Prices read from `data/economy.csv` (or a `.tres` generated from it)
- Coop capacity 5 hens; buying more is blocked with a friendly "coop is full" icon

**Acceptance criteria:**
- Buying deducts coins and adds items/buildings; can't buy without enough coins
- Capacity enforced (coop full at start, so hens can only be bought after a loss)

**Team Preparation:**
- Placeholder shop panel; old-coop sprite

**Tests:**
- **Software:** purchases, wallet floor, capacity cap; feed order before/after 18:00 arrives on the right Dawn into the right bin
- **Human:** prices and "not enough coins" are understandable

**AI assistance:** Prompt for "Godot 4.x C# shop reading prices from CSV, with grid placement of a building."

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
- Hen and cow entities in the old coop and barn; feed with the feed scoop (grain for hens, hay for the cow), once per day
- Fed today → next morning 1 egg per hen, 4 milk cans from the cow (★ quality only in this phase)
- 1 unfed day → need icon; 2 → flashing icon + Ada line; 3 → hen gone + **message card with gravestone picture**, name, and tip
- Egg and milk orders become available from Day 1 (Task 1.7 generator)

**Acceptance criteria:**
- Feeding resets the count; eggs appear only after fed days
- Loss card appears once; no on-screen death; nothing else is removed

**Team Preparation:**
- Placeholder hen, cow, egg, milk can, feed scoop, gravestone card illustration

**Tests:**
- **Software:** scripted rollovers verify egg production and the 3-day loss; card shown once
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
- Sections owned by each system (`clock`, `farm`, `inventory`, `wallet`, `orders`, `animals`, `player`) via a registration API
- Auto-save at every day rollover and after purchases/deliveries; manual save from pause menu
- Safe write (temp file + rename); previous save kept as `savegame.bak.json`; fall back to it if parsing fails
- A no-op migration chain ready for `v1 → v2`

**Acceptance criteria:**
- Restart restores everything (position, day, tiles, crops, dry counts, inventory, coins, orders, hens, hunger counts)
- Corrupt main file → backup loads

**Team Preparation:**
- Agreed save sections (GDD §13)

**Tests:**
- **Software:** round-trip equality; 50× save/load without drift; corrupt-file fallback; migration hook runs
- **Human:** None required

**AI assistance:** Prompt for "Godot 4.x C# versioned JSON save with section registration, atomic write, and backup."

**Dependencies:** Tasks 1.3–1.9.

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
- `PauseMenu.tscn`: Resume, Save, Settings (volume, nudges on/off, read-aloud placeholder), Quit; calendar (season, day)
- Lighting tint per day phase from `GameClock`
- Pause stops time

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
- Steps 1–9 of onboarding.md §2 as a small step machine
- One-time nudges from onboarding.md §3 that apply to this phase (sleep, watering, yellow, lost crop, feed low, first eggs/milk, hungry animals)
- Nudges toggle in Settings

**Acceptance criteria:**
- Guided start completes in ≈ 5 minutes and never blocks free play
- Each nudge shows once

**Team Preparation:**
- Pointer/arrow icon; short lines for each step

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
- A new player completes the guided start, the first two orders, buys hens, and keeps them fed without adult help
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
- [ ] Crops grow by day, yellow at 2 dry days, and are lost at 3; hens are lost at 3 unfed days with the gentle card
- [ ] Starting hens and cow work (feed, eggs, milk, loss rule); buying seeds and feed works at economy.md prices
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
