# PHASE.md — Development Roadmap: Prototype to Production

> **Status:** Planning draft — **re-planned for the direction reset 2026-11-19**
> **Version:** 0.10  
> **Last updated:** 2026-11-21  
> **Purpose:** High-level overview of development from the first playable prototype to a final, production-ready game. Each phase document expands this roadmap — `docs/Phase-001.md` exists today; Phase-002 through Release are **planned** documents to be written when their phase is scheduled.

## 1. Vision

A **3D, realistic but kid-friendly, educational** farming game for **PC**, seen through a fixed, angled top-down camera with a cartoony, colourful look (owner decision 2026-11-21, replaces "fully 2D"). Kit, a 17-year-old, grows a tangled family plot into a working farm by filling neighbours' orders, investing in animals, buildings, and land, and finally **automating** the farm — the key selling point. Neglect has gentle but real consequences; challenge rises naturally with the farm (money first, land last). There is **one difficulty mode: realistic but kid-friendly**. Audience: **ages 6+**.

## 2. Guiding Principles (apply to all phases)

- **One mode:** realistic but kid-friendly; challenge comes from the farm growing (challenge-layers.md).
- **A fixed valley, a growing farm:** the world is Bramblewick Vale — Kit's farm plus five areas around a village hub (places.md). All of Kit's land (992 tiles) is owned from the start but overgrown and cleared over time. Each village building and outer area is built in the phase whose features need it, not before.
- **Consequences, kindly shown:** crops yellow then are lost; neglected animals and fish are gone (gravestone message card). No sickness, vet, gore, or on-screen death. Nothing changes while the game is closed.
- **Optional economy:** orders and every other way to earn are optional; no debt, no deadline penalties. An unsent neighbour order may simply expire (owner 2026-11-21), but nothing is ever lost besides the missed money.
- **Automation is the payoff:** unlocked by crafting or buying, once the farm is big enough to need it.
- **Presenter-like storytelling:** characters address the player, pause for an answer, and respond to what happened.
- **PC first, consoles later.** PC release first; Nintendo Switch and PlayStation planned after it (decided 2026-11-20). No mobile or tablet versions. Until console work is scheduled, every task keeps controls gamepad-complete and button prompts abstract (no hard-coded Xbox letters).

## 3. Development Phases

| Phase | Document | Goal | Platform |
|-------|----------|------|----------|
| **Phase 001** | `docs/Phase-001.md` | **First playable (vertical slice):** day clock, clearing, day-based growth and neglect, foraging, first orders, order board, seed shop, coop + hens with the loss rule, versioned save, short onboarding — built in 2D (Tasks 1.1–1.13), then **moved to 3D** with placeholder primitive meshes (Tasks 1.14–1.20, owner 2026-11-21) | PC |
| **Phase 002** | `docs/Phase-002.md` (**draft for owner approval**, 2026-11-21) | Economy depth: **real rooms** for buildings (owner 2026-11-21), Bramblewick Green's first ring, **shipping box** (take items back out until the fixed **18:00** pickup, owner 2026-11-21), seasonal/demand pricing, the order freshness bonus over the box (+30/+20/+10 %, then the order is gone), the storage chest, **house-visit orders** (sign at the door, talk to take the order, send it through the box; every order incl. Marlow's; the baker's, allotment neighbour's and shopkeeper's places open in T1 when they arrive), Marlow's board → village noticeboard, the on-screen Orders list, rain, clearing the back fields (scythe/hammer/axe), sheep or beehive, stickers | PC |
| **Phase 003** | `docs/Phase-003.md` (planned) | Farm growth: barn (cow, sheep), **fish pond**, kitchen processing (jam, bread, cheese, pie), workshop & blueprints, more neighbours, **local co-op with dynamic split screen** | PC |
| **Phase 004** | `docs/Phase-004.md` (planned) | **Automation**: sprinklers, auto-feeders, pond auto-feeder, helpers, silo, greenhouse; fishing at the brook, travelling merchant, optional Butterfly Garden request | PC |
| **Phase 005** | `docs/Phase-005.md` (planned) | Long-term goals: festivals & contests, quality products, farm charm & decorations, collections, hydroponics, farm shop, lane restoration, showcase, mastery | PC |
| *Deferred, phase not set* | — | **Kit's market stall** (owner 2026-11-21: one way to sell at a time, the shipping box first; if the stall returns it is "set prices and leave it") | PC |
| **Release** | `docs/Release.md` (planned) | Production-ready, polished, localised, age-rated PC release | PC (Windows first; macOS/Linux considered); consoles (Switch, PlayStation) after the PC release |

> **⚠️ Re-plan note:** the previous plan sequenced phases by the child's age stage (preschool → 5th grade) and introduced tablets from Phase-003. Both are retired. The table above is a **proposal** following the farm-growth tiers in progression.md §3; confirm before Phase-002 is written.

## 4. Phase 001 — First Playable (in detail)

**Goal:** a vertical slice of the money loop: Kit clears and plants, waters daily, forages berries for Marlow's first order, grows carrots for the second, saves up for a coop and hens, feeds them daily, and fills egg orders — with crops and hens lost after 3 days of neglect.

**Scope:** Front garden, the lane's berry bushes, Mr. Marlow's seed shed (order board + shop). Single-player. Spring only. See docs/Phase-001.md for the tasks and the deferred list: Tasks 1.1–1.13 built the slice in 2D; **Tasks 1.14–1.20 move it to 3D** (fixed angled top-down camera, primitive placeholder meshes) keeping all game logic, then the Task 1.13 playtest runs on the 3D build.

**Exit criteria:** a new player (6+) finishes the ≈ 5-minute guided start and the first two orders without help, buys hens, keeps them fed, and understands the 3-days rule from the warnings.

## 5. How Phase Documents Are Structured

Each `docs/Phase-00N.md` breaks its milestone into **session tasks**. One task = one scoped, playtested artifact that can be created and validated in a single working session with AI assistance. Tasks are ordered by dependency and designed so the game remains playable and coherent after every task.

## 6. Team & Workflow

- **Team:** Two people, with AI assistance.
- **Engine:** Godot 4.x with **C#**; renderer **Compatibility (`gl_compatibility`)**; assembly **`MyFarmingGame`**.
- **Art:** **3D, cartoony and colourful** (owner 2026-11-21). Placeholder primitive meshes with flat colours for prototypes; real low-poly models (Kenney CC0, Blender, AI 3D — GDD Q17) swapped in later by name without code changes. Every phase stays inside the handheld performance budget (project-structure.md §16) so the Switch version needs no art redo.
- **Audio:** Free royalty-free assets; licenses recorded.
- **Save format:** One save slot, auto-save, human-readable JSON for debugging.

## 7. Guardrails (never change across phases)

- No antagonist, threat, or hard deadline. Orders can expire (owner 2026-11-21), but an expired order costs nothing besides the missed money: no fee, no relationship loss, Kit keeps the goods. The only losses come from the player's own neglect, are always warned first, and never happen while the game is closed.
- Losses are shown kindly: crops yellow then disappear; an animal loss is a short message with a gravestone picture. No on-screen death, sickness, vet, or gore.
- No disappointment or scolding from Mr. Marlow, ever.
- No debt, loans, or real-money purchases.
- No lore the player must read or memorise to proceed.
- The valley is fixed: five areas plus Kit's farm (places.md). New areas open by care and restoration, never by a timer, and no further areas are added. Kit's farm grows only by clearing and building on its own land.
- Original designs throughout — homage to a mood, never reproduction of existing characters/assets.

---

## 8. Change log

- **0.1 — 2026-11-18:** Initial roadmap draft: five phases plus release, Phase-001 detail, phase-document structure, team & workflow, guardrails.
- **0.2 — 2026-11-18:** Audience reworded (mainly pre-elementary–1st grade with a parent; older kids accommodated by challenge layers). Engine set to Godot 4.x with **C#**. Places recounted to eight. Phase-001 scope annotated with target future phases; future phase docs marked "(planned)"; Phase-003 goal drops Time Awareness (always on, Gentle Core); "PC first, tablet later" reconciled with the Phase-003 tablet row.
- **0.3 — 2026-11-19:** **Re-planned for the direction reset (GDD v0.19).** Vision rewritten (Kit 17, realistic but kid-friendly, educational, orders, automation, three difficulty modes). Audience column and age-stage sequencing removed; phases now follow farm growth (economy → land & buildings → automation → long-term goals), flagged as a proposal. **PC only** — tablet/multiplatform rows removed. Phase-001 now includes crop neglect (yellow → gone) at Normal timing. Renderer/assembly recorded in §6. Guardrails rewritten: "no decay caused by the player" and "no currency/no punishment" replaced by warned, kindly-shown, neglect-only losses; no debt; lane fixed but farm may grow.
- **0.4 — 2026-11-19:** Owner picks: **single difficulty mode**; audience 6+; land owned from the start and cleared; **Phase-001 re-scoped to a vertical slice** (orders + chickens) and Phase-002–004 rows rebalanced (co-op split screen in 003; Butterfly Garden optional in 004).
- **0.5 — 2026-11-20:** Platform changed from "PC only" to **PC first, consoles later** (Switch, PlayStation after the PC release; owner decision). Vision bullet and Release row updated.
- **0.6 — 2026-11-20:** World map redesigned as **Bramblewick Vale** (owner decision, places.md v0.10): the "lane does not grow" principle and guardrail replaced by "the valley is fixed — five areas plus Kit's farm"; village buildings and outer areas arrive in the phase whose features need them.
- **0.7 — 2026-11-20:** Windmill/pump dropped from Phase 004 (owner decision: sprinklers need no water source; flour comes from the village watermill).
- **0.8 — 2026-11-21:** **Selling and house-visit orders (owner decisions 2026-11-21).** Phase 002 builds the **shipping box** first and **house-visit orders** as neighbours' places open, plus the order **freshness bonus** over the box (orders can expire; guardrail reworded: nothing lost besides the missed money); **Kit's market stall deferred** (phase not set).
- **0.9 — 2026-11-21:** **Switch to 3D (owner decision 2026-11-21):** vision and §6 art updated (fixed angled top-down 3D camera, cartoony colourful look, primitive placeholders, handheld performance budget); Phase 001 row and §4 add the 3D migration (Tasks 1.14–1.20) before the playtest.
- **0.10 — 2026-11-21:** Phase 002 row: `docs/Phase-002.md` drafted for owner approval; shipping box picked up at 18:00; real rooms and the village's first ring added (owner 2026-11-21).
