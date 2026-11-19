# Project My Farming Game

> **Working title:** TBD
> **Status:** 🟡 Design draft (direction reset 2026-11-19) · 🟡 Godot project initialised in `src/` (4.7, C#, Compatibility renderer, assembly `MyFarmingGame`) — no gameplay code yet
> **Design version:** GDD v0.20 · World v0.12 (both 2026-11-19)
> **Last README update:** 2026-11-19 (🔄 owner picks: one mode, ages 6+, day-based growth, land owned but overgrown, first order as the opening, Phase-001 vertical slice · 🆕 `docs/economy.md` + price sheet, `docs/onboarding.md`)

![](docs/assets/game-screenshoot-small.jpeg)

A fully 2D, **realistic but kid-friendly, educational** farming game for **PC**. You play **Kit, a 17-year-old** who has come from the city to look after the family's tangled land on Bramblewick Lane. Grow crops, raise animals, stock a fish pond, and fill **orders from the neighbours** — eggs, fruit, pies, seasonal baskets, bulk orders — to earn money and grow the plot into a working farm.

Like *Harvest Moon*, crops take days to grow and the challenge rises as the farm grows. All the land is Kit's from day one, but overgrown: money is the first limit, clearing the next, and in the end only land is. Neglect has real but gentle consequences — **3 days in a row without water or food** and a crop or animal is gone (crops turn yellow first; an animal loss is only a short message with a gravestone picture; no sickness, no vet, nothing graphic). Then comes the payoff and the key selling point: **automation**. Craft or buy sprinklers, feeders, and helpers, and watch the farm run itself — leaving time for decorations, collections, contests, and restoring the lane.

Every way to earn is optional: orders, market sales, contests, foraging and fishing, trading, crafting. The player decides how much money to chase. There is **one difficulty: realistic but kid-friendly** — real sell prices, half-price buying. For **ages 6 and up**.

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
4. **Grow your farm (Harvest Moon-style)** — money first, then clearing, finally land; animals limited by housing (5 stalls = 5 cows).
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

The first beat is **the first order**: on Day 1 Mr. Marlow buys wild berries Kit forages on the lane, then orders carrots Kit grows — the first coins and the order board. The Butterfly Garden is now an optional later request.

## Documentation

The [Game Design Document](./docs/GDD.md) is the **single source of truth** for gameplay, audience, and project decisions. This README is the project-wide summary and navigation hub. Every document in `docs/` carries its own status header and change log.

## Current status

- [x] Initial brief captured
- [x] Engine selected — **Godot 4.x with C#**
- [x] Milestone 0 decisions (growth timing, clock, story, controls) — growth amended 2026-11-19 (watering now required)
- [x] **Direction reset (2026-11-19):** Kit 17, orders, neglect consequences, farm growth, automation, PC only
- [x] **Owner picks (2026-11-19):** one mode, ages 6+, day-based growth, land owned but overgrown, housing caps, first order, co-op split screen, economy sheet, short onboarding, Phase-001 vertical slice
- [x] Godot project config: Compatibility renderer, `MyFarmingGame` assembly, `.import` files committed, only `.godot/` ignored
- [ ] **Owner decisions open:** day length, season length, order premium (GDD §16 Q13–Q15); art/audio brief inputs
- [ ] Repository scaffolding (Phase-001 Task 1.1)
- [ ] First playable prototype
- [ ] First playtest

No game code yet. The current milestone is **Phase-001, the first playable vertical slice** (`docs/Phase-001.md`).

## Roadmap

Full plan: [`docs/PHASE.md`](./docs/PHASE.md).

| Phase | Goal |
|---|---|
| **001** | First playable: clock, clearing, day-based growth & neglect, first orders, order board, shop, coop + hens, versioned save, onboarding |
| **002** | Economy depth: market & shipping, pricing, rain, clearing back fields, more neighbours, sheep or bees |
| **003** | Farm growth: barn, fish pond, processing, workshop, co-op split screen |
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
| [Buildings (`docs/buildings.md`)](./docs/buildings.md) | Farm map (992 tiles) and everything Kit can build, with cost, footprint, capacity, unlock, education angle. |
| [Items (`docs/items.md`)](./docs/items.md) | Tools, produce, processed goods, blueprints, automation items, decorations, rare items. |
| [Inventory System (`docs/inventory.md`)](./docs/inventory.md) | Auto-stacking picture slots, quality stacks, wallet, storage chest, Collection Book. |
| [Mechanics (`docs/mechanics.md`)](./docs/mechanics.md) | Verbs and systems: farming, orders, selling, crafting, building, automation. |
| [Controls (`docs/controls.md`)](./docs/controls.md) | PC gamepad and keyboard + mouse mappings; local co-op seat rule. |
| [Events (`docs/events.md`)](./docs/events.md) | Daily, order, neglect, seasonal, festival, merchant, and restoration events. |
| [Progression (`docs/progression.md`)](./docs/progression.md) | Farm growth tiers, clearing the land, orders as progression, and long-term goals (charm, collections, trophies, restoration, mastery). |
| [Difficulty (`docs/challenge-layers.md`)](./docs/challenge-layers.md) | The single mode — realistic but kid-friendly — and its rules. |
| [Economy (`docs/economy.md`)](./docs/economy.md) | 🆕 Prices (sell ≈ real, buy ≈ half), balance check; full sheet in [`docs/data/economy.csv`](./docs/data/economy.csv). |
| [Onboarding (`docs/onboarding.md`)](./docs/onboarding.md) | 🆕 The ≈ 5-minute guided start and Day 1–7 nudges. |
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
