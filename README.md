# Project My Farming Game

> **Working title:** TBD
> **Status:** 🟡 Design draft — no code yet
> **Design version:** GDD v0.12 · World v0.2 (both 2026-11-17)
> **Last README update:** 2026-11-17

![](docs/assets/game-screenshoot-small.jpeg)

A gentle, fully 2D life-and-garden game for young children (pre-elementary through first grade). The player cares for a small garden, explores a quiet lane, and helps friendly neighbours with ordinary tasks — farming, exploration, and everyday social learning as parts of one cozy world.

Inspired by the welcoming feel of games like *Stardew Valley*, but deliberately much smaller in scope and designed specifically for children: no crop death, no shame, no time pressure, no reading required to act.

---

## Contents

- [Project My Farming Game](#project-my-farming-game)
  - [Contents](#contents)
  - [Why this project](#why-this-project)
  - [Design pillars](#design-pillars)
  - [The core loop](#the-core-loop)
  - [Documentation](#documentation)
  - [Current status](#current-status)
  - [Roadmap](#roadmap)
    - [Milestone 0 — Design decisions](#milestone-0--design-decisions)
    - [Milestone 1 — Interaction prototype](#milestone-1--interaction-prototype)
    - [Milestone 2 — Validation](#milestone-2--validation)
    - [Later (ideas, not promises)](#later-ideas-not-promises)
  - [Open questions](#open-questions)
  - [Technical direction](#technical-direction)
  - [Team and constraints](#team-and-constraints)
  - [How this README gets updated](#how-this-readme-gets-updated)

---

## Why this project

Most farming-life games assume a player who can already read fluently, manage an inventory, and keep a daily schedule. This one starts from the opposite end: a player who is just learning to read, who may be new to games, and who should be able to pick it up, do something satisfying, and put it down without penalty.

The differentiator is an interactive, presenter-like story woven through the gameplay — characters address the player, pause for an answer, and respond to what actually happened.

## Design pillars

1. **Easy to understand** — a child can see what to do and learn the controls by trying them.
2. **Kind and low-pressure** — mistakes are safe; nothing punishes a break or an unusual order of play.
3. **Small, satisfying activities** — short actions produce visible, cheerful results.
4. **Creative ownership** — personalise a garden without needing to optimise it.
5. **Warm illustrated world** — expressive characters, clear shapes, non-pixel-art children's-book style.
6. **Practice everyday social skills** — greeting, asking, taking turns, offering help, repairing a misunderstanding.

## The core loop

1. Decide what you'd like to do, find, or unlock.
2. Explore, tend the garden, or talk to a character to find a way forward.
3. Help someone with a clear task in exchange for an agreed reward.
4. See an immediate response; some rewards open a new activity, tool, or place.
5. Enjoy the new possibility — or pick another goal.

The first beat is **The Butterfly Garden**: plant and water three flowers for Mr. Marlow and earn a butterfly net, which unlocks bug-finding as a brand-new verb.

## Documentation

Everything currently lives in `docs/`. The GDD is authoritative for gameplay, audience, and project decisions.

| Document | What's in it | Status |
|---|---|---|
| [`docs/GDD.md`](docs/GDD.md) | Audience, pillars, core loop, rules, controls, art direction, accessibility, safety, prototype plan, change log | Draft v0.12 |
| [`docs/world.md`](docs/world.md) | Bramblewick Lane, Mr. Marlow, the seed-shed ledger, lore rules, cast, open story questions | Draft v0.2 |

Both documents carry their own status header, open-question list, and change log. Read the change log before editing so the version history stays coherent.

## Current status

Where the project actually is right now:

- [x] Initial brief captured
- [x] Design pillars and audience defined
- [x] Core loop and first reward beat proposed (Butterfly Garden)
- [x] Rules/difficulty guardrails written (no crop death, no shame, no timers)
- [x] Art direction narrowed (angled top-down, earthy base + bright accents, non-chibi, non-pixel)
- [x] Background-story premise written (Bramblewick Lane, Mr. Marlow)
- [ ] Engine selected
- [ ] Repository scaffolding (project folder, build running)
- [ ] First playable prototype
- [ ] First playtest with a child
- [ ] Decisions made on the open questions below

Nothing is implemented yet. The current milestone is **decisions, not code**: pick an engine, then build the smallest scene that answers the interaction questions.

## Roadmap

### Milestone 0 — Design decisions
- [ ] Choose engine (see [Technical direction](#technical-direction))
- [ ] Decide in-session growth timing and closed-game growth policy
- [ ] Decide whether there is any in-game clock
- [ ] Resolve the open story questions in `docs/world.md` §7 that affect the first minute of play
- [ ] Record chosen PC control mappings

### Milestone 1 — Interaction prototype
One small playable scene, placeholder art, for validating feel rather than visuals:
- [ ] Controllable character (gamepad + full-mouse schemes)
- [ ] One garden bed, basic seeds, and the task's flower seeds
- [ ] Plant and water interactions
- [ ] Visible growth/progress feedback
- [ ] NPC picture-bubble want and clear task request
- [ ] Butterfly net reward → simple butterfly-finding interaction
- [ ] Garden shelf that receives harvests automatically
- [ ] Local save

### Milestone 2 — Validation
- [ ] Playtest questions from `docs/GDD.md` §14 answered with adult-supervised play
- [ ] Decide: does the loop hold up before adding features?
- [ ] Confirm fixed tool belt and shelf feel right (no loose inventory)

### Later (ideas, not promises)
- Second small area (brook, footbridge, allotment)
- More plants, characters, decorations
- Giving/sharing interactions and structured parent-child activities
- Touchscreen adaptation (iPad / Android tablets first)
- Relationship meter and high-relationship events

Explicitly deferred from the prototype: large maps, crafting/economy depth, seasons and calendars, combat, online multiplayer, long reading-heavy dialogue.

## Open questions

From `docs/GDD.md` §16 — brainstorming, not decided:

1. How short should in-session growth stages be, and should plants get bounded growth while the game is closed?
2. Which tasks or dialogue events change an NPC's relationship, and by how much?
3. What should high relationships unlock?
4. How should a parent and child split tasks across two gamepads or tablet turn-taking?
5. Which PC gamepad and mouse mappings should the prototype use?
6. What research is needed for original, non-chibi European-comics character design?
7. Which open story questions in `docs/world.md` should be decided next?

## Technical direction

**Prototype platform:** PC. **Longer-term goal:** multiplatform, iPad and Android tablets prioritised for touch.

**Engine:** TBD. Evaluate candidates against:
- 2D workflow and art pipeline (illustrated, non-pixel assets)
- Speed of PC prototyping
- Future touch input without rewriting activities
- Eventual multiplatform deployment
- The team's existing experience

The first technical prototype must answer movement, planting, watering, growth feedback, and saving **before** substantial content production starts.

## Team and constraints

- **Team:** Two people, with AI assistance. Passion project, no fixed content or feature cap.
- **Process:** Small, testable milestones so the core experience stays playable and coherent.
- **Assets:** Free royalty-free resources may be used — record each asset's license, attribution requirement, and suitability before use.
- **Safety:** No personal-data collection, no open chat or user-generated public content. Child-privacy and age-rating review required before any release. Online features and monetisation are out of scope.

## How this README gets updated

This file is meant to be edited as the project develops. Suggested conventions:

- **Bump `Last README update`** every time you touch this file, using `YYYY-MM-DD`.
- **Tick checkboxes** as things land; add new items rather than rewriting sections.
- **Move work from "Current status" → "Roadmap" → "Done"** as it happens.
- **Bump the design version numbers** only in `docs/GDD.md` and `docs/world.md` (their change logs are the source of truth); mirror the versions in the header here.
- **When the working title is chosen**, replace "Project My Farm" in the header and the `Working title` line.
- **When the engine is chosen**, fill in [Technical direction](#technical-direction) and note the decision in the GDD change log.
- Keep this file an honest snapshot: if something is speculative, label it as a proposal.

---

*License: TBD.*