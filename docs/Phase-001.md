# Phase-001 — Interaction Prototype

> **Status:** Planning draft  
> **Version:** 0.4  
> **Milestone:** 1 — Interaction Prototype  
> **Purpose:** Break Milestone 1 down into session tasks. Each task produces one scoped, playtested artifact that can be created and validated in a single session with AI assistance.

## 1. Milestone Overview

**Goal:** A vertical slice of the core loop (Want → Task → Reward) in one connected scene.

**Scene:** Player's front garden → path → Mr. Marlow's seed shed gate.

**Core loop in this milestone:**
1. Mr. Marlow offers the Butterfly Garden task (plant and water 3 flowers).
2. Child plants seeds in a garden bed.
3. Child waters plants (optional, speeds growth).
4. Flowers grow through visible stages (seed → sprout → bloom).
5. Child harvests flowers → they route automatically to the garden shelf.
6. Child shows flowers to Mr. Marlow → receives the butterfly net.
7. Butterfly net unlocks simple catch/release butterfly interaction.

**Explicitly OUT of scope (deferred to future phases, not cut):** Animals, collectathon inventory grid, sticker book → **Phase-002**; challenge layers, grown-up menu, multi-gamepad co-play → **Phase-003**; market stand → **Phase-004**; stile, Lantern Hill, brook, allotment beds, seasons beyond spring, numeric relationship meter → **later phases**. The product-prototype scope (GDD §15) remains the source of truth for the finished game.

## 2. Session Tasks

Tasks are ordered by dependency. After each task, the game should be runnable and the new artifact playtestable.

Each task includes:
- **Team Preparation** — what the team needs to have ready before starting (assets, hardware, decisions).
- **Tests** — split into **Software tests** (verifiable by running the game or scripts, no human required) and **Human playtests** (require a person, ideally a child in the target age range, to play and give feedback).
- **Notes & learnings** — an empty fill-in block. **When the task is done, fill it in before starting the next task**: what worked, what broke and how it was fixed, how the plan deviated, and concrete advice for the next task (and for Phase-002 planning). An unfilled block means the task isn't finished.

---

### Task 1.1 — Project Foundation

**Artifact:** Godot 4.x project with version control, scene tree structure, and placeholder art pipeline.

**Scope:**
- Initialize Godot 4.x project
- Set up `.gitignore` for Godot
- Create root scene structure: `Main.tscn`, `Player.tscn`, `UI.tscn`
- Create placeholder art assets (colored rectangles/simple sprites for player, NPC, garden bed, flowers, tools)
- Confirm project runs and displays a blank scene

**Acceptance criteria:**
- Godot project opens without errors
- `Main.tscn` runs and shows a colored background with a placeholder player sprite
- `.gitignore` excludes `.godot/`, `*.import`, build artifacts

**Team Preparation:**
- Godot 4.x installed on development machine
- Git repository initialized (or access to existing repo)
- Team agrees on project folder structure and naming conventions

**Tests:**
- **Software:** Project opens in Godot without errors; `Main.tscn` instantiates and runs; placeholder sprite visible on screen; `.gitignore` correctly excludes Godot artifacts
- **Human:** None required

**AI assistance:** Prompt for "Godot 4.x C# project structure for a 2D top-down farming game, including .gitignore and basic scene tree."

**Dependencies:** None.

**Notes & learnings:**
- **Outcome / status:** _TBD — fill in when done (date done, final verdict)_
- **What worked well:** _TBD_
- **Problems & fixes:** _TBD — Godot/C# setup errors, import quirks, .gitignore misses_
- **Deviations from scope or plan:** _TBD — folder structure or naming conventions actually adopted_
- **Handoff to Task 1.2:** _TBD — where scenes/scripts ended up, which Godot version, anything the movement task must not break_

---

### Task 1.2 — Player Movement (Gamepad + Mouse)

**Artifact:** Controllable player character that walks with gamepad and mouse input.

**Scope:**
- `Player.tscn` with Sprite2D, AnimationPlayer, CollisionShape2D
- C# controller: left stick/D-pad move, click-to-move (mouse)
- Fixed angled top-down/3/4 camera (Camera2D, no rotation)
- Idle + walk animations (placeholder, 4 directions)
- Walk only; no run button

**Acceptance criteria:**
- Player moves smoothly with gamepad left stick
- Player moves to mouse click position on ground
- Camera follows player with smooth lerp
- Animations flip correctly by direction
- No diagonal movement required (8-way or 4-way acceptable)

**Team Preparation:**
- Xbox-layout gamepad controller connected and working
- Mouse available for click-to-move testing
- Placeholder player sprite (colored rectangle acceptable)

**Tests:**
- **Software:** Gamepad left stick input moves player; mouse click on ground moves player to that position; Camera2D follows player with lerp; sprite flips by movement direction
- **Human:** Controls feel comfortable for the target input device (gamepad and mouse); no accidental inputs; movement speed feels right for a young child

**AI assistance:** Prompt for "Godot 4.x C# 2D top-down player controller with gamepad and mouse click-to-move, fixed camera follow."

**Dependencies:** Task 1.1.

**Notes & learnings:**
- **Outcome / status:** _TBD — fill in when done (date done, playtest verdict)_
- **What worked well:** _TBD_
- **Problems & fixes:** _TBD — input mapping issues, camera lerp tuning, animation flip bugs_
- **Deviations from scope or plan:** _TBD — 4-way vs 8-way chosen, actual move speed, click-to-move gotchas_
- **Handoff to Task 1.3:** _TBD — which inputs are already bound (gamepad X is claimed by tool cycling), where the player controller lives, any pause/menu input conflicts_

---

### Task 1.3 — Fixed Tool Belt

**Artifact:** On-screen tool belt with 2 slots (watering can, seed box) that can be cycled and visually indicates the active tool.

**Scope:**
- `ToolBelt.tscn` UI node at bottom of screen
- 2 icon slots: Watering Can (active by default), Seed Box
- Cycle input: gamepad X / mouse scroll / on-screen buttons
- Active slot highlighted visually
- Tool selection stored in a singleton/autoload `ToolBelt.cs`

**Acceptance criteria:**
- Tool belt visible at bottom of screen
- Cycling changes the highlighted slot
- Active tool persists between scenes
- Placeholder icons clearly distinguishable

**Team Preparation:**
- Two placeholder tool icons (distinct colored rectangles acceptable — one for watering can, one for seed box)

**Tests:**
- **Software:** Tool belt visible at bottom of screen; cycling input changes highlighted slot; active tool persists across scene reloads; correct tool icon shown as active
- **Human:** Placeholder icons are clearly distinguishable from each other at a glance; cycling feels intuitive with gamepad and mouse

**AI assistance:** Prompt for "Godot 4.x C# fixed tool belt UI with 2 slots, cycling input, and active slot highlight."

**Dependencies:** Task 1.2 (player exists to hold tools).

**Notes & learnings:**
- **Outcome / status:** _TBD — fill in when done (date done, playtest verdict)_
- **What worked well:** _TBD_
- **Problems & fixes:** _TBD — autoload/persistence quirks, input conflicts with movement, UI anchoring_
- **Deviations from scope or plan:** _TBD — cycling input actually used, icon style chosen_
- **Handoff to Task 1.4:** _TBD — how the active tool is queried by other systems (API other tasks should use), how to add a 3rd slot later for the butterfly net_

---

### Task 1.4 — Garden Beds & Planting

**Artifact:** Interactive garden beds where the player can plant seeds and see them grow through stages.

**Scope:**
- `GardenBed.tscn` with empty, planted, and harvestable visual states
- 2–3 beds in the front garden scene
- Plant action: with Seed Box active, interact on empty bed → plant seed
- Growth stages: Seed → Sprout → Bloom (3 stages)
- Each stage has a visible sprite change
- Growth timer: ~30–45 seconds of accumulated play time per stage
- Watering (with Watering Can active) optionally advances one stage immediately with sparkle/sound feedback
- Plants never wither

**Acceptance criteria:**
- Player can plant a seed in an empty bed
- Plant visually advances through seed → sprout → bloom
- Watering a plant triggers immediate growth stage advance + sparkle particles
- Plants do not die or wither
- Bed returns to empty planting state after harvest

**Team Preparation:**
- Placeholder flower sprites for 3 growth stages (seed, sprout, bloom — distinct colors acceptable)
- Placeholder sparkle particle effect for watering feedback

**Tests:**
- **Software:** Planting a seed changes bed to planted state; growth timer advances stage after ~30–45 seconds accumulated play time; watering triggers immediate stage advance + sparkle particles; plants never wither; bed returns to empty after harvest
- **Human:** Growth timing feels satisfying (not too fast, not too slow); watering feels rewarding; child understands plants grow without needing adult explanation

**AI assistance:** Prompt for "Godot 4.x C# garden bed with planting, 3 growth stages, timer-based growth, and watering acceleration."

**Dependencies:** Task 1.2, Task 1.3.

**Notes & learnings:**
- **Outcome / status:** _TBD — fill in when done (date done, playtest verdict)_
- **What worked well:** _TBD_
- **Problems & fixes:** _TBD — growth timer accuracy, watering double-trigger, state reset bugs_
- **Deviations from scope or plan:** _TBD — actual growth timing, number of beds, sparkle feedback approach_
- **Handoff to Task 1.5:** _TBD — how bed states are represented (the harvest task must read/clear them), signals or events already available for state changes_

---

### Task 1.5 — Harvest & Garden Shelf

**Artifact:** Harvesting mature plants and automatic routing to the garden shelf.

**Scope:**
- Harvest action: interact on bloom-stage bed → remove plant, add flower to shelf
- `GardenShelf.tscn` UI node showing collected flowers (visual gallery, not managed)
- Harvested flowers appear as icons on the shelf automatically
- No sorting, no inventory grid, no currency

**Acceptance criteria:**
- Player can harvest a bloom-stage flower
- Flower disappears from bed and appears on garden shelf
- Bed returns to empty planting state
- Shelf displays harvested flowers visually

**Team Preparation:**
- Placeholder shelf UI slots (empty + filled states)
- Placeholder harvested flower icon

**Tests:**
- **Software:** Harvesting a bloom-stage flower removes it from the bed and adds it to the shelf; bed returns to empty planting state; shelf displays collected flowers
- **Human:** Shelf display is clear and understandable without reading; harvesting feels satisfying

**AI assistance:** Prompt for "Godot 4.x C# harvest system and garden shelf UI that auto-displays harvested items without sorting."

**Dependencies:** Task 1.4.

**Notes & learnings:**
- **Outcome / status:** _TBD — fill in when done (date done, playtest verdict)_
- **What worked well:** _TBD_
- **Problems & fixes:** _TBD — bed state not clearing, shelf icons not updating, double-counting_
- **Deviations from scope or plan:** _TBD — shelf layout chosen, how many flowers it can show_
- **Handoff to Task 1.6:** _TBD — where harvested flowers are stored (Marlow task counts on reading this), any event worth subscribing to_

---

### Task 1.6 — Mr. Marlow NPC & Picture-Bubble Dialogue

**Artifact:** Mr. Marlow at the seed shed gate with picture-bubble dialogue offering the Butterfly Garden task.

**Scope:**
- `Marlow.tscn` NPC at shed gate
- Idle + talk gesture animations (placeholder)
- Dialogue bubble UI: picture icon + short text, no reading required
- Dialogue states: Greeting → Task Offer → Task Active (restatement) → Task Complete → Reward Given
- Marlow supplies 3 Butterfly Garden flower seeds when task is accepted
- Marlow kindly restates unfinished task if player talks to him again
- No numeric relationship meter (dialogue state only)

**Acceptance criteria:**
- Player can walk to Marlow and interact
- Dialogue bubble appears with picture + text
- Accepting the task gives the player 3 flower seeds in the seed box
- Talking to Marlow with an unfinished task restates it kindly
- No punishment for declining or walking away

**Team Preparation:**
- Placeholder Mr. Marlow sprite (colored rectangle acceptable)
- Placeholder dialogue bubble UI
- Picture icons for the task (flower icon, net icon)

**Tests:**
- **Software:** Dialogue states transition correctly (Greeting → Task Offer → Task Active → Task Complete → Reward Given); accepting task gives 3 flower seeds; Marlow restates unfinished task kindly; no punishment for declining
- **Human:** Picture bubble is understandable without reading; child understands what Marlow wants and what the reward is; declining or walking away doesn't feel punitive

**AI assistance:** Prompt for "Godot 4.x C# NPC dialogue system with picture bubbles, task offer/accept/restate states, and item delivery."

**Dependencies:** Task 1.2, Task 1.3 (seed box tool), Task 1.4 (garden beds).

**Notes & learnings:**
- **Outcome / status:** _TBD — fill in when done (date done, playtest verdict)_
- **What worked well:** _TBD_
- **Problems & fixes:** _TBD — interaction/interactable detection issues, bubble positioning, seed delivery bugs_
- **Deviations from scope or plan:** _TBD — dialogue flow actually implemented, bubble size/legibility findings from playtest_
- **Handoff to Task 1.7:** _TBD — how dialogue decides which state to show (Task 1.7 moves that logic into the TaskManager), where the interact prompt lives_

---

### Task 1.7 — Butterfly Garden Task State Machine

**Artifact:** Global task state tracking the Butterfly Garden task from offer to completion.

**Scope:**
- `TaskManager.cs` autoload/singleton
- Task states: `NotOffered`, `Offered`, `Active`, `Completed`, `Rewarded`
- Tracks: flowers planted (0/3), flowers watered (optional counter), flowers shown to Marlow
- Completes when player has harvested 3 flowers and interacts with Marlow
- Triggers reward: butterfly net granted

**Acceptance criteria:**
- Task state persists across save/load
- Task advances correctly: offered → active → completed → rewarded
- Marlow dialogue reflects current task state
- Reward (butterfly net) is granted exactly once

**Team Preparation:**
- None specific (uses assets from previous tasks)

**Tests:**
- **Software:** Task state machine transitions correctly through all 5 states; state persists across save/load cycles; Marlow dialogue reflects current task state; butterfly net is granted exactly once
- **Human:** None required

**AI assistance:** Prompt for "Godot 4.x C# task state machine autoload with offer/active/completed/rewarded states and persistence."

**Dependencies:** Task 1.6.

**Notes & learnings:**
- **Outcome / status:** _TBD — fill in when done (date done)_
- **What worked well:** _TBD_
- **Problems & fixes:** _TBD — state not persisting, reward granted twice, dialogue showing wrong state_
- **Deviations from scope or plan:** _TBD — how counters are stored, how save hooks are wired (Task 1.9 must serialize this)_
- **Handoff to Task 1.8:** _TBD — how the reward-granted event is signalled (the net slot must unlock exactly once), how other systems should query task state_

---

### Task 1.8 — Butterfly Net & Catch/Release

**Artifact:** Butterfly net tool (3rd tool belt slot) with simple catch/release butterfly interaction in the garden.

**Scope:**
- Butterfly net appears as 3rd tool belt slot after Task 1.7 completion
- Butterflies spawn around bloomed flowers (visual only, simple movement)
- Catch action: with net active, interact on butterfly → "caught" state
- Release action: interact in garden → butterfly flutters around garden, then disappears
- No inventory slot for caught butterflies (cosmetic only in this milestone)

**Acceptance criteria:**
- Butterfly net appears in tool belt after completing Marlow's task
- Butterflies appear around flowers during the day
- Player can catch a butterfly (visual feedback)
- Player can release a butterfly in the garden (visual flutter, then fade)

**Team Preparation:**
- Butterfly sprite (placeholder: small animated colored shape)
- Butterfly net icon (placeholder: distinct icon for 3rd tool belt slot)
- Simple flutter path behavior reference (what "catch" and "release" should look like)

**Tests:**
- **Software:** Net appears as 3rd belt slot only after Task 1.7 completes; butterflies spawn near bloom-stage beds during daytime; interact with net catches butterfly (state change, visual feedback); interact in garden releases butterfly (flutter animation, then fade); no inventory entry created for caught butterflies
- **Human:** Child understands they can catch a butterfly; catch action feels fun and not frustrating; release moment feels delightful; butterfly-net icon is distinguishable from other tools

**AI assistance:** Prompt for "Godot 4.x C# butterfly catch/release mechanic with simple butterfly AI and net tool."

**Dependencies:** Task 1.7.

**Notes & learnings:**
- **Outcome / status:** _TBD — fill in when done (date done, playtest verdict)_
- **What worked well:** _TBD_
- **Problems & fixes:** _TBD — butterfly AI/steering, catch hit detection, tool belt slot unlock timing_
- **Deviations from scope or plan:** _TBD — spawn rules chosen, catch/release feel findings from playtest_
- **Handoff to Task 1.9:** _TBD — which state (net slot, butterflies caught/released if any) must be saved, and which is purely cosmetic_

---

### Task 1.9 — Save / Load System

**Artifact:** One-slot auto-save system that persists game state.

**Scope:**
- `SaveGame.cs` autoload/singleton
- Save data: player position, day, season, tool belt slots, garden bed states, task state, shelf contents
- Auto-save on: day change, task state change, harvest, menu pause
- Manual save/load from pause menu
- Save file: human-readable JSON at `user://savegame.json`

**Acceptance criteria:**
- Game saves automatically on task progress and day change
- Game loads correctly after restart (position, beds, task state preserved)
- JSON file is readable and editable for debugging
- No corruption on repeated save/load cycles

**Team Preparation:**
- Agreed save data schema (which fields to persist — reference GDD save expectations)
- Decision on save file location (`user://savegame.json`)

**Tests:**
- **Software:** Auto-save triggers on day change, task state change, and harvest; `savegame.json` written to `user://` and is valid, human-readable JSON; full game restart restores player position, day, season, tool belt slots, bed states, task state, and shelf contents; repeated save/load cycles (50×) produce no corruption; save file contains every field in the schema
- **Human:** None required

**AI assistance:** Prompt for "Godot 4.x C# JSON save/load singleton with auto-save triggers and one save slot."

**Dependencies:** Task 1.2 through Task 1.8 (all systems that need persistence).

**Notes & learnings:**
- **Outcome / status:** _TBD — fill in when done (date done)_
- **What worked well:** _TBD_
- **Problems & fixes:** _TBD — serialization pitfalls, corrupt/reload edge cases, missed auto-save trigger_
- **Deviations from scope or plan:** _TBD — final save schema fields, actual auto-save triggers_
- **Handoff to Task 1.10:** _TBD — how the Save button should invoke save (API), where save hooks live so new systems can register, versioning strategy for schema changes_

---

### Task 1.10 — Pause Menu & Calendar UI

**Artifact:** Pause menu showing season, day, and time of day visually; settings and quit.

**Scope:**
- `PauseMenu.tscn` overlay
- Calendar UI: season icon (Spring), day number, day/night indicator (sun/moon)
- Buttons: Resume, Save, Settings, Quit
- Visual only (icons, not text-dependent)
- Sleep/rest button at home (advances day by one, no penalty)

**Acceptance criteria:**
- Pause menu opens with Start/ESC
- Calendar shows current season, day, and time visually
- Save button triggers manual save
- Quit button exits to desktop
- Sleep button at home advances the day

**Team Preparation:**
- Icon set for calendar: Spring icon, day number display, sun/moon day-night indicator
- Button labels/icons: Resume, Save, Settings, Quit
- Sleep/rest button icon for the home location

**Tests:**
- **Software:** Pause menu opens on Start/ESC and closes on Resume; calendar displays current season (Spring), day number, and day/night indicator; Save button triggers manual save; Quit button exits to desktop; sleep button advances day counter by one; pause blocks game-time progression while open
- **Human:** Calendar is readable at a glance without reading text; child can find pause button; icons are distinguishable; child understands what sleep button does

**AI assistance:** Prompt for "Godot 4.x C# pause menu with calendar UI showing season, day, and time of day using icons."

**Dependencies:** Task 1.9 (save system), Task 1.2 (pause input).

**Notes & learnings:**
- **Outcome / status:** _TBD — fill in when done (date done, playtest verdict)_
- **What worked well:** _TBD_
- **Problems & fixes:** _TBD — pause input conflicts, tree-pause vs. processing UI, sleep triggering save/day change_
- **Deviations from scope or plan:** _TBD — icon set chosen, which buttons actually shipped, sleep flow_
- **Handoff to Task 1.11:** _TBD — where the day/night indicator gets its value (Task 1.11 must drive it), how pause stops/passes game time_

---

### Task 1.11 — Day / Night Cycle (Cosmetic)

**Artifact:** Visual day/night transition that changes lighting palette.

**Scope:**
- Day/night cycle runs through play (cosmetic only, no gameplay gating)
- Lighting/color shift: bright day palette → darker night palette
- Fireflies may appear at night in season (visual ambient, no interaction)
- No time pressure, no crop death, no penalties

**Acceptance criteria:**
- Lighting shifts smoothly between day and night
- No gameplay mechanics are blocked by time of day
- Player can play at any time without penalty

**Team Preparation:**
- Day palette and night palette (color values or reference image)
- Firefly particle placeholder (simple glowing dot animation)

**Tests:**
- **Software:** Lighting/color shifts smoothly between day and night over the cycle duration; no gameplay mechanic is gated by time of day; fireflies render only during night; no performance drop during transition
- **Human:** Transition is not jarring or frightening for a child; night palette is dim but still readable; child feels no time pressure to hurry

**AI assistance:** Prompt for "Godot 4.x 2D day/night cycle with ColorRect lighting shift and ambient firefly particles."

**Dependencies:** Task 1.10 (calendar shows day/night indicator).

**Notes & learnings:**
- **Outcome / status:** _TBD — fill in when done (date done, playtest verdict)_
- **What worked well:** _TBD_
- **Problems & fixes:** _TBD — color modulation vs. Light2D choice, UI readability under night palette, performance_
- **Deviations from scope or plan:** _TBD — palettes chosen, cycle duration, fireflies included or not_
- **Handoff to Task 1.12:** _TBD — the actual cycle duration to expect during a playtest, anything that hurt readability for a child_

---

### Task 1.12 — Playtest & Iterate

**Artifact:** Validated prototype ready for Milestone 1 exit review.

**Scope:**
- Playtest with target age group (or simulate child user testing)
- Validate against GDD §14 playtest questions:
  - Can a child start playing with little or no spoken explanation?
  - Can they tell what objects can be interacted with?
  - Do they understand how to plant, water, and finish?
  - Are controls comfortable?
  - Do they understand the NPC's visible want, task, and reward?
  - Does earning the butterfly net feel meaningful?
- Iterate on controls, timing, and clarity based on feedback

**Acceptance criteria:**
- A new player can complete the Butterfly Garden task without adult explanation
- Controls feel comfortable for gamepad and mouse
- Growth timing feels satisfying, not too fast or slow
- All tasks 1.1–1.11 are complete and integrated

**Team Preparation:**
- Playtest participants: children in the target age range (preschool–1st grade), with appropriate adult supervision and consent per GDD §14
- Playtest observation sheet based on GDD §14 playtest questions
- Game build installed on test PC; gamepad connected; quiet play space
- Fallback plan: if child participants are unavailable, two team members simulate child players and mark all findings as "needs child validation"

**Tests:**
- **Software:** Run through all automated checks from Tasks 1.1–1.11 in one pass; frame rate stable on target PC; no runtime errors in Godot console; save/load works end-to-end
- **Human:** All GDD §14 playtest questions answered — child starts without explanation, identifies interactable objects, plants/waters/finishes, understands Marlow's want and reward, finds butterfly net meaningful, controls comfortable, watering feels voluntary, plant growth readable after a break

**AI assistance:** Prompt for "playtest observation sheet for children's farming game prototype, including the GDD §14 validation questions."

**Dependencies:** All previous tasks.

**Notes & learnings:**
- **Outcome / status:** _TBD — fill in when done (date done, milestone verdict)_
- **What worked well:** _TBD_
- **Problems & fixes:** _TBD — every issue found during playtest and how it was resolved_
- **Playtest findings (GDD §14 answers):** _TBD — record each question's answer, and mark anything validated only by team simulation as "needs child validation"_
- **Advice for Phase-002 planning:** _TBD — carry-forward lessons (tool belt extension, task/dialogue patterns, save schema growth, placeholder art that needs real art)_

---

## 3. Exit Criteria for Milestone 1

The milestone is complete when:

- [ ] All 12 session tasks are done
- [ ] A child can complete the Butterfly Garden task (plant 3 flowers, water, grow, harvest, show to Marlow, receive net) with little or no spoken explanation
- [ ] Game saves and loads correctly
- [ ] Game runs at stable frame rate on target PC hardware
- [ ] Placeholder art is clearly marked as temporary
- [ ] Task list and playtest notes are recorded for Milestone 2 planning

---

## 4. What Comes Next (Milestone 2 Preview)

After Milestone 1, the next phase document (Phase-002, planned) will cover:

- Animals (chicken, sheep, cow, beehive) with gentle care meters
- Collectathon inventory grid with auto-stacking and Overflow Nest
- Sticker book and Collection Book
- Second place on the lane (brook or allotment beds)
- Challenge layers (multi-step requests)

---

## 5. Change log

- **0.1–0.2 — 2026-11-18:** Earlier planning iterations (task breakdown, tests, dependencies).
- **0.3 — 2026-11-18:** GDScript → **C#** throughout (engine decision, PHASE.md §6). Out-of-scope list annotated with the target future phase for each deferred item (product prototype = source of truth). Milestone 2 preview updated for the four care animals; forward reference to Phase-002 marked "(planned)"; day/night & seasons noted as always-on Gentle Core.
- **0.4 — 2026-11-18:** Added a **Notes & learnings** fill-in block to every task (1.1–1.12): outcome, what worked, problems & fixes, deviations, and a task-specific handoff question for the next task — to be completed before starting the next task. Task format list in §2 updated accordingly.
