# Project My Farming Game

> **Working title:** TBD
> **Status:** 🟡 Design draft (direction reset 2026-11-19) · 🟡 Godot project initialised in `src/` (4.7, C#, Compatibility renderer, assembly `MyFarmingGame`) — no gameplay code yet
> **Design version:** GDD v0.19 · World v0.11 (both 2026-11-19)
> **Last README update:** 2026-11-19 (🔄 direction reset: Kit is 17, neighbour orders, realistic-but-kid-friendly neglect, Harvest Moon-style growth, automation, three difficulty modes, PC only · 🆕 `docs/buildings.md`)

![](docs/assets/game-screenshoot-small.jpeg)

A fully 2D, **realistic but kid-friendly, educational** farming game for **PC**. You play **Kit, a 17-year-old** who has come from the city to look after the family's tangled land on Bramblewick Lane. Grow crops, raise animals, stock a fish pond, and fill **orders from the neighbours** — eggs, fruit, pies, seasonal baskets, bulk orders — to earn money and grow the plot into a working farm.

Like *Harvest Moon*, the challenge rises as the farm grows: at first money is the limit, in the end only land is. Neglect has real but gentle consequences — dry crops turn yellow and are lost, neglected animals are gone (shown only as a short message with a gravestone picture; no sickness, no vet, nothing graphic). Then comes the payoff and the key selling point: **automation**. Craft or buy sprinklers, feeders, and helpers, and watch the farm run itself — leaving time for decorations, collections, contests, and restoring the lane.

Every way to earn is optional: orders, market sales, contests, foraging and fishing, trading, crafting. The player decides how much money to chase. Difficulty is one of three modes — **Easy, Normal, Realistic**.

---

## Contents

- [Why this project](#why-this-project)
- [Design pillars](#design-pillars)
- [The core loop](#the-core-loop)
- [Documentation](#documentation)
- [Current status](#current-status)
- [Roadmap](#roadmap)
- [Technical direction](#technical-direction)
- [How this README gets updated](#how-this-readme-gets-updated)
- [Documentation hub](#documentation-hub)

---

## Why this project

Farming games are wonderful at teaching patience and responsibility, but most either never let anything go wrong or bury players in systems. This one aims for the middle: a real (if simple) farm where plants need water, animals need feeding, and money has to be earned — presented kindly enough for kids, and always teaching something true about where food comes from.

The differentiator is twofold: an interactive, presenter-like story (characters address the player, pause, and respond to what happened), and a farm that grows from hand-tending to **automation** as its big payoff.

## Design pillars

1. **Easy to understand** — see what to do; learn controls by trying them.
2. **Realistic but kid-friendly** — needs and consequences are real; losses are shown gently (yellow crops, a gravestone card), never graphically. No sickness or vet.
3. **Educational** — seasons, plant and animal needs, where food comes from, simple money sense.
4. **Grow your farm (Harvest Moon-style)** — money is the early limit, land the final one; no fixed bed or animal caps.
5. **Automation is the payoff** — sprinklers, feeders, helpers, unlocked by crafting or buying.
6. **Many ways to earn, none mandatory** — orders are the main income; everything else is optional; no debt.
7. **Small, satisfying activities** — short actions, visible results.
8. **Creative ownership** — lay out and decorate the farm.
9. **Warm illustrated world** — non-pixel-art European comics style.
10. **Practice everyday social skills** — greeting, keeping promises, delivering what was asked, trading fairly.

## The core loop

1. **Plan** what to grow, raise, or build with the money and land you have.
2. **Work the farm** — plant, water, harvest, feed, tend the pond.
3. **Earn** — fill neighbour orders, or sell, trade, forage, compete.
4. **Invest** — seeds, animals, buildings, tools, land, automation.
5. **Grow** — bigger farm, bigger orders, new neighbours, until land is the limit.

The first beat is still **The Butterfly Garden**: plant and water three flowers for Mr. Marlow and earn a butterfly net. Right after, Marlow opens the order board and the farm economy begins.

## Documentation

The [Game Design Document](./docs/GDD.md) is the **single source of truth** for gameplay, audience, and project decisions. This README is the project-wide summary and navigation hub. Every document in `docs/` carries its own status header and change log.

## Current status

- [x] Initial brief captured
- [x] Engine selected — **Godot 4.x with C#**
- [x] Milestone 0 decisions (growth timing, clock, story, controls) — growth amended 2026-11-19 (watering now required)
- [x] **Direction reset (2026-11-19):** Kit 17, orders, neglect consequences, farm growth, automation, three modes, PC only
- [x] Godot project config: Compatibility renderer, `MyFarmingGame` assembly, `.import` files committed, only `.godot/` ignored
- [ ] **Owner decision needed:** target age band (GDD §16 Q9)
- [ ] Repository scaffolding (Phase-001 Task 1.1)
- [ ] First playable prototype
- [ ] First playtest

No game code yet. The current milestone is **Phase-001, the interaction prototype** (`docs/Phase-001.md`).

## Roadmap

Full plan: [`docs/PHASE.md`](./docs/PHASE.md).

| Phase | Goal |
|---|---|
| **001** | Interaction prototype: plant, water, grow, crop neglect (yellow → gone), Butterfly Garden task, save |
| **002** | Farm economy: money, order board, shop, selling, inventory, chickens & animal loss rule, difficulty modes |
| **003** | Farm growth: land expansion, barn, beehive, fish pond, processing, workshop, co-op |
| **004** | Automation: sprinklers, feeders, helpers, greenhouse; foraging, fishing, merchant |
| **005** | Long-term goals: festivals, contests, farm charm, collections, hydroponics, farm shop, lane restoration |
| **Release** | Polished, localised, age-rated PC release |

**Not planned:** mobile/tablet versions, online multiplayer, combat, sickness/vet systems, debt, real-money purchases.

## Technical direction

- **Platform:** **PC only** (Windows first; macOS/Linux considered later).
- **Engine:** Godot 4.x with **C#**; renderer **Compatibility (`gl_compatibility`)**; assembly **`MyFarmingGame`**.
- **Version control:** `*.import` sidecars are committed; only `src/.godot/` is ignored.
- The first prototype answers movement, planting, watering, growth/neglect feedback, and saving **before** content production starts.

## How this README gets updated

- **Bump `Last README update`** every time you touch this file (`YYYY-MM-DD`).
- **Tick checkboxes** as things land; add new items rather than rewriting sections.
- **Mirror the GDD version** in the header; each doc's change log is the source of truth for that doc.
- **When the working title is chosen**, replace "Project My Farming Game" in the header and the `Working title` line.
- Keep this file an honest snapshot: if something is speculative, label it as a proposal.

---

## Documentation hub

| Document | What it covers |
|---|---|
| [Game Design Document (`docs/GDD.md`)](./docs/GDD.md) | Authoritative source: audience, pillars, core loop, orders, ways to earn, neglect, growth, clock, automation, open questions. |
| [World, Setting & Background Story (`docs/world.md`)](./docs/world.md) | Bramblewick Lane, Kit's arrival, Mr. Marlow, the ledger, the lane's quiet mystery and its restoration. |
| [Characters (`docs/characters.md`)](./docs/characters.md) | Kit (17), Ada, Mr. Marlow, future cast (baker, merchant, friend of Kit's age…). |
| [Places (`docs/places.md`)](./docs/places.md) | The eight places on the lane plus Kit's growing farm. |
| [Crops (`docs/crops.md`)](./docs/crops.md) | Crops, orchard trees, watering-based growth, yellow → gone rule. |
| [Animals & Fish Pond (`docs/animals.md`)](./docs/animals.md) | Chicken, sheep, cow, beehive, fish pond; care meters, loss rule, quality products. |
| [Buildings (`docs/buildings.md`)](./docs/buildings.md) | 🆕 Everything Kit can build: coop, barn, greenhouse, hydroponics, sprinklers, windmill, farm shop… with cost, footprint, unlock, education angle. |
| [Items (`docs/items.md`)](./docs/items.md) | Tools, produce, processed goods, blueprints, automation items, decorations, rare items. |
| [Inventory System (`docs/inventory.md`)](./docs/inventory.md) | Auto-stacking picture slots, quality stacks, wallet, storage chest, Collection Book. |
| [Mechanics (`docs/mechanics.md`)](./docs/mechanics.md) | Verbs and systems: farming, orders, selling, crafting, building, automation. |
| [Controls (`docs/controls.md`)](./docs/controls.md) | PC gamepad and keyboard + mouse mappings; local co-op seat rule. |
| [Events (`docs/events.md`)](./docs/events.md) | Daily, order, neglect, seasonal, festival, merchant, and restoration events. |
| [Progression (`docs/progression.md`)](./docs/progression.md) | Farm growth tiers, land expansion, orders as progression, and long-term goals (charm, collections, trophies, restoration, mastery). |
| [Difficulty Modes (`docs/challenge-layers.md`)](./docs/challenge-layers.md) | Easy / Normal / Realistic tuning (replaces the old challenge layers). |
| [Roadmap (`docs/PHASE.md`)](./docs/PHASE.md) | Phases from prototype to release; guardrails. |
| [Phase-001 (`docs/Phase-001.md`)](./docs/Phase-001.md) | Current phase task list. |
| [Project Structure (`docs/project-structure.md`)](./docs/project-structure.md) | Technical contract: folders, naming, InputMap/autoload registries, version control, verification commands. |

### How the docs fit together

- **[docs/GDD.md](./docs/GDD.md)** — authoritative for gameplay, audience, and project decisions.
- **[docs/world.md](./docs/world.md)** — expands the setting; the GDD wins on gameplay.
- **[docs/project-structure.md](./docs/project-structure.md)** — technical contract; never overrides the GDD or a phase task.
- **Everything else** — expands the GDD in one domain.

---

*License: TBD.*
