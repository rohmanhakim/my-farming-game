# Game Design Document

> **Status:** Brainstorming draft
> **Version:** 0.18
> **Last updated:** 2026-11-18
> **Purpose:** A living document for shaping the game together. The concept and audience below come from the initial brief; unconfirmed details are proposals, not commitments.

## 1. High Concept

A gentle, fully 2D life-and-garden game for young children. Players care for a small garden, explore beyond home, and help friendly characters with everyday tasks. Farming, exploration, and community interactions work together as parts of one cozy world. The presentation uses illustrated, non-pixel-art graphics.

The game takes inspiration from the welcoming farming and community feel of *Stardew Valley*, while aiming for a much smaller scope and an experience designed specifically for young children from pre-elementary through first grade, playing with a parent — while accommodating older children (up to fifth grade) through optional challenge layers defined in this design.

**Working title:** TBD

## 2. Audience and Play Context

- **Primary audience:** Children from **pre-elementary through first grade**, playing with a parent or caregiver. The game accommodates older children (through fifth grade and beyond) through the optional challenge layers defined below. The whole experience grows with the child: younger players (pre-elementary–1st grade) use Gentle Core only; older players (2nd–5th grade+) can opt into challenge layers managed by a parent or caregiver.
- **Intended play modes:** A child can play alone or with a parent. Parent-child play is designed for collaboration on one device.
- **Parent/caregiver role:** The parent acts as the "difficulty dial" — managing which challenge layers are active through a grown-up menu, helping with reading and menus, and deciding with the child when the game is ready to grow. On PC with separate gamepads, parent and child collaborate in real time; on touchscreen they take turns.
- **Working literacy assumption:** Essential actions and feedback never depend on fluent reading. Text is always paired with pictures, icons, and voice-over. Reading ability varies widely across the target age range, so the game must be playable by a non-reader in Gentle Core.
- **Likely considerations:** Players may be new to games, and some may benefit from visual cues, short text, or spoken support even if reading is an intended part of the experience.
- **Prototype platform:** PC.
- **Longer-term platform goal:** Multiplatform, with iPad and Android tablets as the priority touchscreen devices.

Text can be part of the experience, but essential actions and feedback should not depend on fluent reading.

## 3. Design Goals

### Design pillars

1. **Easy to understand:** A child can see what to do and learn the controls by trying them.
2. **Kind and low-pressure:** Mistakes are safe; the game does not punish a child for taking a break or doing things in an unexpected order.
3. **Small, satisfying activities:** Short actions produce visible, cheerful results.
4. **Creative ownership:** Players can personalize a garden or space without needing to optimize it.
5. **Warm illustrated world:** Expressive characters, clear shapes, and inviting environments support the experience.
6. **Practice everyday social skills:** Through ordinary interactions, players can explore communication, cooperation, boundaries, and caring for shared spaces.
7. **Collectathon inventory:** Picture-based slots, auto-stacking, and a collection book give kids the joy of collecting without inventory stress. No currency, no sorting, no "full" penalty.
8. **Gentle animal care:** Caring for farm animals teaches responsibility through positive care loops. Animals nap if ignored — they never die, run away, or become permanently unhappy.
9. **Progressive challenge (opt-in):** Optional stickers, multi-step requests, and seasonal mysteries grow with the child's skill when a parent turns them on. Nothing is ever locked behind them; the core loop always remains available.
10. **The parent is the difficulty dial:** Challenge layers are turned on or off by a parent or caregiver through a grown-up menu. The game never auto-scales based on age or playtime. Gentle Core remains available at every stage, no matter how many layers are active.

### Intended emotional experience

Players should feel curious, capable, welcomed, and proud of what they have made. Social situations should invite reflection without making children feel judged or ashamed.

### Social learning approach

The game should let players practice interactions that can help them navigate life beyond home, such as greeting someone, asking before taking or using something, taking turns, offering help, sharing, and responding when another character has a different need or preference.

Some situations may offer simple choices and show understandable consequences. Consequences should be proportionate, connected to what happened, and focused on how people or the shared environment respond. When possible, players should be able to repair a misunderstanding or try a kinder approach. Avoid rigid good/bad scoring, shame, or presenting complex moral questions as having one simplistic answer.

### Two Modes: Gentle Core + Opt-in Challenge Layers

The game offers two modes that share one world:

**Gentle Core (always on):** Plant, water, harvest, care for animals, decorate. No death, no currency, no reading required, no time pressure, no penalties. A child can play forever here.

**Challenge Layers (opt-in, parent-controlled):** Garden coins, tool upgrades, crafting, multi-step requests, festivals, and deeper systems. Each layer is toggled by the parent in a grown-up menu. Layers are additive — turning one on never removes Gentle Core features.

Layers progress through five stages:

- **Stage 1 — My First Garden** (preschool): Gentle Core only
- **Stage 2 — Garden Helpers** (K–1st): Multi-step requests (day/night and seasons are always on — Gentle Core)
- **Stage 3 — Farm Manager in Training** (2nd–3rd): Garden coins, tool upgrades, crafting
- **Stage 4 — Bramblewick Farmer** (4th–5th): Festivals, relationship meters, expanded world
- **Stage 5 — Standard Sim** (5th grade+): Full economy, expanded systems

The parent can return to Gentle Core at any time with one click. See [challenge-layers.md](./challenge-layers.md) for the full layer definitions and stage map.

### Social interaction anchor: reciprocal requests

The shared pattern is that the player wants something and helps another person to get it. The specific request and task can differ each time and should fit the character, situation, and desired reward.

1. The player expresses interest in or asks for something.
2. An NPC explains a clear task that would help them and what the player can receive for doing it.
3. The player chooses whether to accept and completes the task, or leaves it unfinished.
4. On completion, the NPC responds and gives the promised reward. Rewards should vary with the task and may include the thing the player wanted.

This is a social exchange, not a hidden test: the player should understand the request and expected reward before agreeing. Tasks should be achievable, with outcomes based on the player's actions rather than confusing rules or arbitrary timers.

If a task is left unfinished, the player does not receive its promised reward yet. When they next speak with that NPC, the NPC can remember and kindly restate the request, for example: "Hmm, we didn't finish that task last time. Would you like to try it now?" Taking a break or closing the game is not itself a failure. The player should be able to accept the task again or decline without shame or loss of unrelated progress.

### First want-task-reward beat: The Butterfly Garden

**Status:** Proposed answer to open question 1; validate through playtesting.

The first request should teach the reciprocal-task pattern using actions the player already knows, then reward the player with a new ability that opens another kind of play.

| Beat | Proposal |
|---|---|
| Player's want | The player sees butterflies in a picture bubble and wants them to visit the garden. The desire is communicated visually before any reading is needed. |
| NPC's request | The NPC offers flower seeds and asks the player to plant and water three flowers so butterflies will come. |
| Player's choice | The player may accept, decline, or leave the task unfinished. The NPC remembers and kindly offers it again later. |
| Reward | The NPC gives the player a butterfly net, unlocking a new activity: finding and catching friendly butterflies or bugs. |
| Loop back | The player can show a discovered butterfly to the NPC or release it in the garden, where butterflies add life to the space. |

The request uses already-known verbs (plant and water), states the expected reward before acceptance, and makes the desired outcome visible. The net adds a new verb rather than simply increasing a currency or item count.

#### Starting loadout and item handling

- **Hands:** Available from the start for planting, harvesting, decorating, talking, and general interaction; these are actions, not inventory items.
- **Watering can:** Starts with the player so the initial garden loop is usable.
- **Seed box:** Starts with two or three basic seeds. Flower seeds for the first task are supplied by the NPC when the player accepts it.
- **Shovel or trowel:** Not part of the initial loadout; reserve digging as a possible later ability or reward.
- **Future tools:** A bug net, ladder, sprinkler, or similar tool should be introduced only when it opens a distinct new verb or place.

A loose, sortable inventory is avoided. The tool belt holds a small set of large icon slots for the watering can, seed box, and earned tools. Harvests and animal products route automatically to collectathon inventory slots (see inventory.md) rather than accumulating in a large player inventory. There is no currency, no sorting, and no "full" penalty — overflow goes to the Overflow Nest at Mr. Marlow's shed.

#### Design-pattern references

These games were reviewed as references for pacing the first task and reward, not as templates to reproduce:

| Reference | Broad pattern considered |
|---|---|
| *Stardew Valley* | A simple introduction to a character precedes a later gift that unlocks fishing. |
| *Animal Crossing: New Horizons* | Short, explicit requests and workshops introduce tools and new capabilities. |
| *Cozy Grove* | Small, untimed help requests connect to character stories and gradual world expansion. |
| *Ooblets* | Stated goals and rewards can reopen places or systems. |
| *Garden Story* | Villager requests can improve a community and unlock useful rewards. |
| *Spiritfarer* | Helping characters can lead to new facilities and mechanics. |

The shared principle is to begin with tools for verbs the player already has, then reward them with tools or access that introduce new verbs or places.

#### Later alternatives

- **Ladder to a lookout:** Help grow and harvest vegetables for an NPC's picnic, then receive a ladder that opens a treehouse lookout. Keep this as a possible reward when a second area is introduced.
- **Garden helper:** A creature or sprinkler waters a bed. Defer this because it automates an existing action instead of opening a new activity.

### NPC relationship meter (proposal)

Track a separate social relationship value for each NPC. Keep it always visible in a subtle way in that NPC's dialogue box. Change it only in response to specific tasks or dialogue events; it does not change passively over time. The meter represents the history of that relationship, not a moral grade.

At higher relationship levels, an NPC may offer something special during selected events. Rewards could include unique items that cannot be obtained through ordinary gameplay, access to a special place, special dialogue, or a memorable event in itself. A seasonal celebration such as New Year is one possible occasion. The exact events, thresholds, and reward mix are TBD.

## 4. Core Experience

### Proposed core loop

1. Decide what the player would like to do, find, or unlock.
2. Explore, care for the garden, or talk with a character to find a way forward.
3. Help someone by completing a clear task in exchange for an agreed reward.
4. See an immediate response and receive the reward; some rewards introduce a new activity, area, or mechanic.
5. Enjoy the new possibility, continue exploring, or choose another goal.

This loop is a starting hypothesis and should be tested before expanding the feature set.

### Progression and novelty

Rewards should not be limited to currency or more items. Early rewards should introduce new possibilities that change how the player can interact with the game, such as a new activity, tool, place, or mechanic. Introduce these one at a time with clear, playful explanation. Avoid making progression depend on collecting and managing a large inventory.

### Gardening without chores

Gardening is a playful option, not a maintenance obligation. Crops should not die, spoil, or create a backlog that pressures the child to return and keep everything in order. Keep planting, growth, and harvesting simple, with useful feedback and rewards that do not require managing large quantities of produce or supplies.

### Plant growth model (decision — Milestone 0, 2026-11-18)

**Design principle:** The garden always moves forward, and the player's care makes it sparkle.

- Plants advance through growth stages while the game is being played, so the child can see progress and connect it to activity in the garden.
- Growth is measured in **accumulated play time**, not in in-game days, and it advances **only while the game is being played**.
- Watering is optional for plant survival and growth. It speeds up growth and gives cheerful immediate feedback, but plants continue to grow without it. A specific NPC task may ask the player to try watering as an activity; this must never imply that plants will suffer if left unwatered.
- Plants never wither, die, or become worse because the child stopped playing.
- Keep the initial garden small (for example, two or three beds) so tending it remains play rather than field maintenance.
- Give watering an immediate response—such as a sparkle, sound, or visiting insect—even when a full growth-stage change takes longer.

**Decided settings — defaults chosen for the prototype, adjustable on the options screen before starting a new game:**

| Setting | Options | **Default** |
|---|---|---|
| **Growth speed** (time per growth stage) | Fast ≈ 15–20 s · **Medium ≈ 30–45 s** · Slow ≈ 60–90 s | **Medium** — seed → bloom ≈ 2 minutes of play |
| **Growth while the game is closed** | **Paused** · Gentle catch-up | **Paused** — nothing grows while closed |

- **Stage advances tick live while playing,** so the child watches the plant change; ending the day (sleep/rest) does **not** add an extra stage, so growth is never double-counted.
- **Gentle catch-up** (the optional setting) grants **at most one stage** when the game is next opened and **never skips a plant past bloom**. It can only move a plant forward — there is no penalty, debt, or missed-day state, ever.
- **Rationale:** the default follows the genre norm — *Stardew Valley*, *Harvest Moon*, and *Ooblets* all grow only during play (Stardew's ≈14-minute in-game day and its 4-day parsnip scale to a child's play beat, not to a stopwatch); only *Animal Crossing* grows on the real-world clock, which requires owning a clock this design does not need. The catch-up option is the positive-only exception the earlier proposal asked for, kept off by default so the simplest configuration has zero schedule.
- **Options screen, not in-game tuning:** both settings are chosen by the grown-up on the options screen **before starting a new game** (they may be changed only by starting a new game, so a mid-game change can never surprise the child). Neither setting may ever surface as a warning, a reminder, or an obligation.
- **Prototype validation:** Phase-001 ships and tests the **Medium / Paused** defaults. Preset values are provisional and may be re-tuned after the first playtest.

For the Butterfly Garden task, planting and watering flowers should create visible progress in-session. The arrival of butterflies can be a short, clearly signaled event rather than a long idle wait.

### In-day clock and day phases (decision — Milestone 0, 2026-11-18)

**The game has a clock, shown in the HUD, but it moves in coarse steps — never minute by minute.**

- **8 day phases, 3 in-game hours each** (24 hours = 8 × 3). Each phase owns a fixed clock range, so the child always sees where in the day they are without reading a schedule.
- **Stepped, not continuous:** the clock jumps **one 3-hour step at a time**, and the phase changes with it. It never ticks by the minute.
- **Step rate:** one step ≈ **3 minutes of play** → a full in-game day ≈ **24 minutes of play** (tunable constant; may become a Gentle/Long day option later).
- **Runs only while playing:** the clock stops when the pause menu is open and does not advance while the game is closed — consistent with the growth decision above.
- **Sleep/rest** ends the day: the calendar day increments and the clock resets to **Dawn (06:00)**. Sleeping early, late, or never carries no penalty.

| # | Phase | Clock | Light | What is true in this phase |
|---|---|---|---|---|
| 1 | **Dawn** | 06:00–09:00 | First light, soft pink | Morning greeting; the lane wakes |
| 2 | **Morning** | 09:00–12:00 | Bright, clear | Mr. Marlow at the gate; butterflies visit grown flowers |
| 3 | **Midday** | 12:00–15:00 | Highest, brightest | Neighbours out and about |
| 4 | **Afternoon** | 15:00–18:00 | Long light | Distant wave from a future neighbour; brook keeper on warm afternoons; Marlow's late-afternoon presence |
| 5 | **Evening** | 18:00–21:00 | Gold → amber | Evening settling begins; lane softens |
| 6 | **Dusk** | 21:00–00:00 | Lane darkens | Fireflies appear in season; Marlow may be indoors |
| 7 | **Night** | 00:00–03:00 | Dark, quiet palette | Seed shed lit; Moon Petal glows; garden lantern useful |
| 8 | **Small hours** | 03:00–06:00 | Deepest dark | Quiet only — nothing is lost, missed, or closed |

**HUD clock:** a small dial in the corner of the screen — an arc of daylight with a sun/moon marker that **jumps one step at a time**, plus the current phase's light tint. A small digital readout ("09:00") may sit beside it for the grown-up; **no essential action ever depends on reading it.**

**What the clock does and does not drive:**

- **Drives:** lighting/day-night transition, NPC presence schedules ("morning", "late afternoon" become phase numbers), time-of-day events (morning greeting, evening settling, butterflies by day, fireflies at night), night-only visuals, the calendar readout.
- **Never drives:** plant growth (accumulated play time, §4 above), animal care, penalties, deadlines, or anything that can be missed. Events tied to a phase recur or are restated kindly.

### Session shape

**Proposal:** Make it easy to have a satisfying short play session, while allowing children to continue when they want. There should be no required daily check-in or penalty for leaving.

**Target session length:** TBD; validate through playtesting.

### Parent-child co-play

The child can play alone, and no activity should require a parent to be present. When playing together on PC, the child and parent use separate gamepads and collaborate in real time. On a touchscreen device, they can take turns on the same screen. Tasks should work in solo play and remain flexible enough for either real-time collaboration or touch turn-taking, without making simultaneous input essential. Activities should create opportunities to talk, make choices, and solve small problems together. Online play is not required.

## 5. Player Activities

### MVP proposal

> **Scope note:** This MVP describes the **Phase-001 interaction prototype** — a vertical slice of the core loop (see PHASE.md). The full **product prototype** scope, which is the source of truth for what the finished prototype contains (collectathon inventory, care animals, sticker book), is defined in §15 and delivered across later phases.

- Play solo or collaborate with a parent on the same device, discussing choices and solving small problems.
- Move around one small home or garden area.
- Interact with garden beds.
- Choose from two or three seeds.
- Plant and water crops using simple controls.
- See crops change as they grow.
- Harvest crops.
- Complete the proposed Butterfly Garden task: plant and water three flowers, then receive a butterfly net that unlocks a new activity.
- Show or release butterflies in the garden as a simple way to connect the new activity back to the world and NPC.
- Use a fixed tool belt with large icon slots; route harvests to a garden shelf rather than a loose inventory.
- Use a small set of decorations to personalize the space.
- Save progress locally.

**Starting loadout:** The player begins with hands, a watering can, and a seed box holding two or three basic seeds. The first accepted NPC task supplies its own flower seeds. Tools that unlock new verbs are earned through play.

### Possible later activities

- A second small area to explore.
- More plants, characters, and decorations.
- Simple giving or sharing interactions.
- More structured parent-child cooperative activities that also work in solo play.
- More ways to express creativity, such as arranging a garden or choosing colors.

These are ideas, not promised features.

## 6. Rules and Difficulty

### Starting principles

- Do not use crop death, debt, lost belongings, shame, or harsh failure states in the initial design.
- Animals never die, run away, or become permanently unhappy from neglect. They nap and wait for care.
- Do not require fast reactions, precise timing, or reading-heavy instructions.
- Let children experiment; make important actions reversible where practical.
- Give clear feedback when an action succeeds, is unavailable, or needs another step.
- Keep any resource or inventory system small and understandable. Collectathon slots auto-stack; no manual sorting.
- Make gardening forgiving and optional; do not make crops a recurring chore. Plants grow without watering; watering speeds growth with cheerful feedback.
- Use early rewards to introduce new ways to play, not only to increase item counts.
- Show social consequences through character responses and the state of the shared space, rather than abstract punishment or moral scores.
- Let players revisit a social interaction, make amends, or try another response where practical.
- Make task expectations and outcomes clear; do not make a child responsible for an outcome they could not understand or reasonably control.
- Do not take away unrelated progress or make the player lose something already earned because a task remains unfinished.
- Seasons affect which crops grow and how animals behave, but missing a season is never a failure — it simply returns next year.
- Optional stickers, requests, and mysteries grow with the child's skill when a parent turns them on. Nothing is locked behind them; Gentle Core always remains available.

### Open design questions

- **Decided (v0.16):** in-session growth timing and closed-game growth policy — Medium (≈30–45 s per stage) and no growth while closed, both adjustable on the pre-game options screen (see §4 plant growth model).
- **Decided (v0.17):** yes — a coarse, HUD-visible, phase-based in-day clock: 8 phases × 3 in-game hours, advanced in 3-hour steps (≈3 minutes of play per step), running only while playing. It affects atmosphere, NPC presence, and event availability **only** — never growth, never penalties (see §4 in-day clock and day phases).
- What, if anything, should be earned or unlocked?

## 7. World and Characters

### Initial scope proposal

- One cozy home or garden hub.
- One small nearby area, if needed to make exploration part of the first prototype.
- One friendly character with a clear, achievable responsibility task.

The world should be readable at a glance and small enough that children can navigate it without a map. Include a small shared or public-feeling space, such as a path or community garden, to support interactions outside the home.

**Setting and background-story proposal:** See [world.md](./world.md). It introduces Bramblewick Lane, Mr. Marlow, and the mystery of the quieting lane. Specific character designs and open world details remain proposals for research and playtesting.

## 8. Controls and Interface

### Control goals

- Keep the number of buttons and actions low.
- Make the primary interaction consistent throughout the game.
- Use large, easy-to-select targets if touch is supported.
- Simple two-button inputs and short holds are acceptable (e.g. LB+RB, hold-Start for the grown-up menu); avoid rapid or memory-heavy button sequences.
- Offer a clear way to pause and reach settings.

### Proposed interaction model

Movement plus one primary interaction action. Seed selection and other choices should use a simple visual menu.

For the PC prototype, offer two selectable control schemes: gamepad and fully mouse-controlled play. Parent-child PC co-play uses two separate gamepads. Design the all-mouse interactions around direct pointing and clicking so they can later map naturally to touchscreen input without changing the underlying activity. The gamepad scheme should use a small, consistent set of buttons and feel familiar to players of farming games on Nintendo Switch, simplified for this game's smaller scope.

Prioritize iPad and Android tablets for touchscreen adaptation. Touch co-play uses turn-taking, while all tasks remain playable solo.

**Exact PC mappings — decided (v0.18; full tables in [controls.md](./controls.md) §2 and §4):**

- **Gamepad:** left stick/D-pad move · A interact · B cancel · X or LB+RB cycle tool belt · Start pause · (Y inventory, RS care menu, Start-hold grown-up menu in later phases).
- **Keyboard + mouse, Don't Starve Together-style:** WASD/arrow keys *or* left-click-to-move · **left-click a target = walk to it, then act** · Space = quick action on the nearest interactable · right-click = cancel · mouse wheel = cycle the tool belt · Esc = pause. Deliberately not carried over from DST: combat keys, camera rotation, Ctrl/Alt/Shift modifiers, number hotbar, map.
- **Seat rule:** keyboard + mouse is **locked to one player**; the other seat must use a gamepad. **If no gamepad is connected, co-op cannot start** — the game runs single-player.

**Still TBD:** tablet layout, and how flexible tasks support solo, two-gamepad, and touch turn-taking play (Phase-003+).

## 9. Art Direction

- Fully 2D presentation; no pixel-art style.
- Colorful, cartoony European children's-book and comics-inspired illustration, with clear inked outlines, expressive faces, lively poses, and readable scenery.
- Friendly and expressive, but not chibi; avoid exaggerated chibi proportions.
- Keep shapes and scenes clear rather than realistic or visually busy.
- **Palette direction:** A hybrid of lively, saturated, high-contrast garden colors and more earthy, subdued environmental tones. Use bright accents to make plants, characters, and interactive objects pop against a slightly moodier world.
- **Camera direction:** An angled top-down / three-quarter view that shows the ground plane clearly while using overlap and simple depth cues to make the world easy to read.
- Use clear silhouettes and strong contrast between interactive objects and backgrounds.
- Make crops and characters expressive enough to communicate state without text.
- Build a consistent visual language for things children can interact with.

Develop an original visual identity. Use broad color and camera-direction references only; do not copy any particular game's distinctive character designs, assets, settings, compositions, or exact visual style. Original character design needs further research before it is defined.

**Palette refinement, camera implementation, animation complexity, production pipeline, and original character research:** TBD.

## 10. Story, Dialogue, Audio, and Text

- Include a background story that gives the world and its characters context; it need not interrupt the core activities.
- Use simple, child-friendly dialogue with short sentences and familiar vocabulary.
- Use an interactive, presenter-like storytelling approach: characters can address the player, ask a clear question, leave a pause for the child to answer or act, and respond to what the player does.
- Use repetition, visual clues, and small mysteries or problems to help children follow along.
- The story should support the game's focus on responsibility and everyday social learning, while inviting parent-child conversation rather than prescribing a single moral answer.
- This guided, interactive story woven through a cozy life-and-garden game is a key point of difference.

- Use gentle music and sound effects to reinforce actions and progress.
- Avoid startling sounds and provide independent volume controls where practical.
- Make important feedback understandable without relying on sound alone.
- Keep essential instructions short and pair them with visuals.
- Consider optional spoken prompts if they improve accessibility and fit the project scope.

The proposed premise and cast are documented in [world.md](./world.md). Dialogue presentation, voice, localization, music style, and text-reading support remain TBD.

## 11. Accessibility and Child-Friendly UX

Design and testing should consider:

- Players who are not yet fluent readers.
- Large text and clear icons.
- Forgiving interaction and movement.
- Adjustable sound and music.
- Visual feedback for actions that also have audio feedback.
- A calm experience without time pressure or punitive reminders.
- Caregiver-friendly access to settings and information.

Specific accessibility requirements should be refined alongside platform and playtest decisions.

## 12. Safety, Privacy, and Caregiver Considerations

- Avoid collecting personal information unless there is a clear, necessary reason.
- Do not include open chat or user-generated public content in the initial scope.
- Make purchases, online features, and data practices explicit to caregivers if they are ever introduced.
- Establish an appropriate child-privacy and age-rating review before release.

Online features and monetization are not currently in scope.

## 13. Technical Direction

The prototype targets PC first. The longer-term goal is multiplatform support, including touchscreen devices.

**Engine and implementation approach:** TBD. Evaluate candidates for 2D workflow, PC prototyping, future touch input, and eventual multiplatform deployment.

The first technical prototype should answer the basic interaction questions—movement, planting, watering, growth feedback, and saving—before substantial content production begins.

Potential engine candidates can be evaluated against the chosen platform, the team's experience, accessibility needs, and the desired art workflow.

### Development approach

- **Project type:** Passion project with no predetermined overall content or feature cap.
- **Core team:** Two people, with AI assistance.
- **Art and audio resources:** Free royalty-free assets may be used. Check and record each asset's license, attribution requirements, and suitability for the intended distribution before use.

The project can grow over time, but work should still be prioritized in small, testable milestones so the core experience remains playable and coherent.

## 14. Prototype and Validation Plan

### First prototype

Build one small playable scene with:

- A controllable character.
- One garden bed, basic seeds, and the flower seeds supplied for the first task.
- Planting and watering interactions.
- A visible growth or progress response.
- The NPC's picture-bubble want and clear request to plant and water three flowers.
- The butterfly net as the task reward, unlocking a simple butterfly-finding interaction.
- A garden shelf that receives harvests automatically.

Use placeholder art if needed. The prototype is for validating the activity and controls, not for demonstrating final visual quality.

### Playtesting questions

- Can a child start playing with little or no spoken explanation?
- Can they tell what objects can be interacted with?
- Do they understand how to plant, water, and finish the activity?
- Are controls comfortable for the target input device?
- Do they enjoy the activity enough to repeat or explore?
- Can they understand the NPC's visible want, the task, and the promised reward before accepting?
- Does earning the butterfly net feel like a meaningful new way to play?
- Can the child understand the character's feelings or needs from dialogue, visuals, or voice?
- Does the outcome of a social choice feel understandable and fair, without feeling punitive?
- Does the child water plants because they want to, rather than because they think they must?
- When a plant grows without being watered, does the child still feel ownership and understand what happened?
- After a break, can the child read the plant's state as positive progress without needing an adult to explain it?
- Do caregivers find setup and settings understandable?

Playtests with children should be conducted with appropriate adult supervision and consent.

## 15. Initial Prototype Scope

These are not ruled out for the passion project; they are deferred from the initial prototype so the core experience can be tested first:

- Large maps or many locations.
- Complex crafting, economy systems, or sellable stockpiles.
- Detailed real-time calendars (seasons affect crops and animals in simple, visible ways).
- Combat or dangerous failure.
- Online multiplayer and public social systems.
- Long, reading-heavy dialogue or exposition.

**In product-prototype scope:** Collectathon inventory slots, four care animals (chicken, sheep, cow, beehive), gentle care meter with no death penalty, seasonal crop and animal behavior, optional sticker book, and Dora/Blues Clues-style picture dialogue with Mr. Marlow. (These are delivered across phases — Phase-001 is the interaction prototype only; see PHASE.md.)

## 16. Open Questions for Brainstorming

1. **RESOLVED (2026-11-18, v0.16):** growth timing and closed-game policy decided — see §4 plant growth model. Defaults: **Medium ≈30–45 s per growth stage**, **no growth while the game is closed**; Fast/Slow presets and an optional positive-only “Gentle catch-up” are selectable on the options screen before starting a new game. *Former question: how short should in-session growth stages be, and should plants receive bounded growth while the game is closed?*
2. Which tasks or dialogue events should change an NPC's relationship, and by how much?
3. Which special events, places, dialogue, or unique items should high relationships unlock?
4. How should a parent and child share tasks while collaborating with two gamepads or taking turns on a tablet?
5. **RESOLVED for PC (2026-11-18, v0.18):** mappings locked in [controls.md](./controls.md) §2/§4 — gamepad table as written, plus a **Don't Starve Together-style keyboard + mouse scheme** (WASD or click-to-move, left-click = walk-to-then-act, Space = quick action, right-click = cancel, wheel = belt, Esc = pause) and the **seat rule** (keyboard + mouse belongs to one player; a second player needs a gamepad, otherwise co-op does not start). *Still open:* the **tablet layout** half of this question — Phase-003.
6. What research and visual exploration are needed to create original characters that fit the non-chibi European cartoon-comics direction?
7. Which open story and setting questions in [world.md](./world.md) should be decided next?
8. What grown-up gesture or question best protects the challenge-layer menu so a child cannot accidentally flip layers during play?

## Related documents

| Document | Relationship |
|---|---|
| [README.md](../README.md) | Centralized hub; mirrors GDD version numbers in its header. |
| [world.md](./world.md) | Expands the GDD's setting, story, and places; the GDD remains authoritative for gameplay. |
| [characters.md](./characters.md) | Main character, parent, Mr. Marlow NPC, and future cast — aligned with GDD design pillars. |
| [places.md](./places.md) | The eight places on Bramblewick Lane, mapped to GDD requirements. |
| [crops.md](./crops.md) | Crop list, growth model, and Butterfly Garden flowers referenced by GDD tasks. |
| [animals.md](./animals.md) | Gentle care animals supporting GDD pillar 8 (gentle animal care). |
| [items.md](./items.md) | Starting loadout, earned items, and NPC-held items referenced throughout the GDD. |
| [inventory.md](./inventory.md) | Collectathon slots implementing GDD pillar 7 (collectathon inventory). |
| [mechanics.md](./mechanics.md) | Core verbs and systems aligned with GDD scope and audience. |
| [controls.md](./controls.md) | Input mappings; GDD open question Q5 covers gamepad/mouse mappings. |
| [events.md](./events.md) | Core/daily/seasonal events driving the GDD's want → task → reward loop. |
| [progression.md](./progression.md) | Stickers, optional requests, and tools supporting GDD pillar 9 (progressive challenge). |
| [challenge-layers.md](./challenge-layers.md) | Defines optional challenge layers, parent-controlled toggles, and the five-stage growth map. |

## 17. Change Log

- **0.1 — 2026-11-17:** Initial brainstorming draft based on the brief: a simpler farming-life game for young children with fully 2D, non-pixel-art graphics.
- **0.2 — 2026-11-17:** Set PC as the prototype platform with future multiplatform and touchscreen goals. Added everyday social learning and gentle, repairable consequences as design direction; recorded the reading and speaking assumption for validation.
- **0.3 — 2026-11-17:** Confirmed a blend of gardening, exploration, and helping characters. Established taking responsibility through a clear character-given task as the first social interaction; left rewards and incomplete-task consequences open.
- **0.4 — 2026-11-17:** Defined parent-child co-play on one device as the intended play mode. Recorded the two-person, AI-assisted passion-project context and free royalty-free asset approach; clarified that prototype scope limits are not permanent project limits.
- **0.5 — 2026-11-17:** Specified selectable gamepad and fully mouse-controlled PC schemes, with mouse interaction intended to inform future touch controls. Set a simple, non-chibi European children's-book/comics art direction and clarified that parent-child co-play is real-time collaboration.
- **0.6 — 2026-11-17:** Prioritized iPad and Android tablets, specified separate gamepads for PC co-play and touch turn-taking, and made solo play a complete option. Added a background story and simple, interactive dialogue as a differentiator; refined the art direction toward an original, colorful European cartoon-comics look.
- **0.7 — 2026-11-17:** Defined the social interaction anchor as helping an NPC to get something the player wants. Added NPC memory for unfinished tasks and a proposed per-NPC relationship meter with possible event bonuses.
- **0.8 — 2026-11-17:** Added early mechanic-unlocking rewards, a subtly visible relationship meter that changes only through tasks or dialogue events, and special high-relationship events or unique rewards. Clarified that gardening should be low-pressure and avoid crop-maintenance and inventory chores.
- **0.9 — 2026-11-17:** Proposed the Butterfly Garden as the first want-task-reward beat: plant and water three flowers to earn a butterfly net. Added the starting loadout, fixed tool belt, garden-shelf storage, design-pattern references, and later reward alternatives; marked the proposal for playtesting.
- **0.10 — 2026-11-17:** Proposed that plants grow during play and watering optionally accelerates growth with immediate positive feedback. Added small-garden and no-withering guardrails, optional bounded away-time growth, and playtest questions about whether watering feels fun rather than obligatory.
- **0.11 — 2026-11-17:** Set a hybrid bright-and-earthy palette direction and an angled top-down/three-quarter camera direction. Left original character design for further research and emphasized a distinct visual identity.
- **0.12 — 2026-11-17:** Linked the Bramblewick Lane and Mr. Marlow story proposal in [world.md](./world.md), resolving the background-story premise question while keeping its open details subject to research and playtesting.
- **0.13 — 2026-11-18:** Expanded scope: collectathon inventory (picture-based, auto-stacking, no currency), three care animals (chicken, sheep, beehive) with gentle care meter and no death penalty, seasons affecting crops and animal behavior, progressive sticker system, and Dora/Blues Clues-style picture dialogue with parent guidance role.
- **0.14 — 2026-11-18:** Added Two Modes principle (Gentle Core always on; challenge layers opt-in and parent-controlled), expanded audience to preschool through fifth grade, added pillar 10 (the parent is the difficulty dial), and introduced challenge-layers.md as the source of truth for toggles and stages.
- **0.15 — 2026-11-18:** Audience clarified: primary audience is pre-elementary through first grade playing with a parent, with older children accommodated through the optional challenge layers. §5 MVP vs §15 product-prototype scope disambiguated (product prototype is the source of truth; Phase-001 is the interaction prototype). Time Awareness removed as a layer (day/night and seasons always on, Gentle Core); control-combo rule relaxed (simple two-button inputs and short holds allowed); four care animals (cow added); places recounted to eight; engine recorded as Godot with C# (see PHASE.md).
- **0.16 — 2026-11-18:** **Milestone 0 decision 1 (growth timing):** §4 plant growth model moved from proposal to decision. Growth runs on accumulated play time, live during play, never on an in-game day and never while closed. Presets set (Fast ≈15–20 s / **Medium ≈30–45 s** / Slow ≈60–90 s per stage) with **Medium** as default; growth-while-closed set to **Paused** by default with an optional positive-only “Gentle catch-up” (max +1 stage, never past bloom). Both are options-screen choices made **before starting a new game**, per genre comparison (Stardew Valley / Harvest Moon / Ooblets grow only during play; Animal Crossing is the real-clock exception). Open question §16.1 resolved; §6 growth question closed, clock question narrowed.
- **0.17 — 2026-11-18:** **Milestone 0 decision 2 (in-game clock):** added §4 "In-day clock and day phases" — the game *does* have a clock, shown in the HUD as a stepped dial: **8 day phases × 3 in-game hours**, jumping one 3-hour step at a time (≈3 min of play per step, ≈24-minute in-game day), running only while playing, resetting to Dawn on sleep. Phase table maps every phase to its light, its NPC presence, and its events (morning greeting → small hours). Explicitly excluded from driving growth, care, or penalties. §6 clock question closed; this answers GDD §6/README "decide whether there is any in-game clock" (Milestone 0 item 2).
- **0.18 — 2026-11-18:** **Milestone 0 decision 3 (control mappings):** §8 records the decided PC mappings — gamepad table unchanged, keyboard + mouse redefined as a **Don't Starve Together-style** scheme (dual movement, one-click walk-to-then-act, Space quick action), plus the **seat rule** that keyboard + mouse is locked to one player and co-op needs a gamepad for the second seat. DST's combat/camera/modifier/hotbar keys explicitly rejected. §16 Q5 resolved for PC; tablet layout left open for Phase-003. Full tables live in controls.md v0.6.
