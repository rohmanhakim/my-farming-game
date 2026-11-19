# PHASE.md — Development Roadmap: Prototype to Production

> **Status:** Planning draft — **re-planned for the direction reset 2026-11-19**
> **Version:** 0.3  
> **Last updated:** 2026-11-19  
> **Purpose:** High-level overview of development from the first playable prototype to a final, production-ready game. Each phase document expands this roadmap — `docs/Phase-001.md` exists today; Phase-002 through Release are **planned** documents to be written when their phase is scheduled.

## 1. Vision

A fully 2D, **realistic but kid-friendly, educational** farming game for **PC**. Kit, a 17-year-old, grows a tangled family plot into a working farm by filling neighbours' orders, investing in animals, buildings, and land, and finally **automating** the farm — the key selling point. Neglect has gentle but real consequences; challenge rises naturally with the farm (money first, land last). Difficulty is one of three modes: **Easy, Normal, Realistic**.

## 2. Guiding Principles (apply to all phases)

- **One world, three modes:** every mode has the same content; modes only tune numbers (challenge-layers.md).
- **The farm grows, the lane doesn't:** Kit's land expands into fields behind the house; the shared lane stays small. Grow the cast before the geography.
- **Consequences, kindly shown:** crops yellow then are lost; neglected animals and fish are gone (gravestone message card). No sickness, vet, gore, or on-screen death. Nothing changes while the game is closed.
- **Optional economy:** orders and every other way to earn are optional; no debt, no deadline penalties.
- **Automation is the payoff:** unlocked by crafting or buying, once the farm is big enough to need it.
- **Presenter-like storytelling:** characters address the player, pause for an answer, and respond to what happened.
- **PC only.** No mobile or tablet versions.

## 3. Development Phases

| Phase | Document | Goal | Platform |
|-------|----------|------|----------|
| **Phase 001** | `docs/Phase-001.md` | Interaction prototype: planting, watering, growth and crop neglect (yellow → gone), Butterfly Garden task, save | PC |
| **Phase 002** | `docs/Phase-002.md` (planned) | Farm economy: money, **order board and neighbour orders**, seed shop, shipping box/market, inventory + storage, chickens & coop with the **animal loss rule**, difficulty-mode selector | PC |
| **Phase 003** | `docs/Phase-003.md` (planned) | Farm growth: **land expansion**, barn (cow, sheep), beehive, **fish pond**, kitchen processing (jam, bread, cheese, pie), workshop & blueprints, more neighbours, local co-op | PC |
| **Phase 004** | `docs/Phase-004.md` (planned) | **Automation**: sprinklers, auto-feeders, pond auto-feeder, helpers, silo, windmill/pump, greenhouse; foraging, fishing at the brook, travelling merchant | PC |
| **Phase 005** | `docs/Phase-005.md` (planned) | Long-term goals: festivals & contests, quality products, farm charm & decorations, collections, hydroponics, farm shop, lane restoration, showcase, mastery | PC |
| **Release** | `docs/Release.md` (planned) | Production-ready, polished, localised, age-rated PC release | PC (Windows first; macOS/Linux considered) |

> **⚠️ Re-plan note:** the previous plan sequenced phases by the child's age stage (preschool → 5th grade) and introduced tablets from Phase-003. Both are retired. The table above is a **proposal** following the farm-growth tiers in progression.md §3; confirm before Phase-002 is written.

## 4. Phase 001 — Interaction Prototype (in detail)

**Goal:** One small, playable scene that validates the core interaction loop: Mr. Marlow asks → Kit plants and waters 3 flowers → flowers grow while watered (and visibly yellow, then are lost, if left dry) → Kit harvests and shows them → Mr. Marlow gives the butterfly net → Kit can catch/release butterflies.

**Scope:** Front garden + Mr. Marlow's seed shed gate. Single-player. Spring season only. Normal-mode neglect timing (no mode selector yet). **Deferred to future phases, not cut:** money, orders, shop, animals, inventory grid, stickers → Phase-002; land, buildings, fish pond, processing, co-op → Phase-003; automation → Phase-004; festivals, charm, endgame → Phase-005.

**Exit criteria:** A new player can start with little or no explanation, understands that crops need water and that yellow means "water me now", completes the Butterfly Garden task, earns the net, and understands what to do next without help.

## 5. How Phase Documents Are Structured

Each `docs/Phase-00N.md` breaks its milestone into **session tasks**. One task = one scoped, playtested artifact that can be created and validated in a single working session with AI assistance. Tasks are ordered by dependency and designed so the game remains playable and coherent after every task.

## 6. Team & Workflow

- **Team:** Two people, with AI assistance.
- **Engine:** Godot 4.x with **C#**; renderer **Compatibility (`gl_compatibility`)**; assembly **`MyFarmingGame`**.
- **Art:** Placeholder art for prototypes; final European comics-style art swapped in later without code changes.
- **Audio:** Free royalty-free assets; licenses recorded.
- **Save format:** One save slot, auto-save, human-readable JSON for debugging.

## 7. Guardrails (never change across phases)

- No antagonist, threat, or hard deadline. The only losses come from the player's own neglect, are always warned first, and never happen while the game is closed.
- Losses are shown kindly: crops yellow then disappear; an animal loss is a short message with a gravestone picture. No on-screen death, sickness, vet, or gore.
- No disappointment or scolding from Mr. Marlow, ever.
- No debt, loans, or real-money purchases.
- No lore the player must read or memorise to proceed.
- The lane does not grow; only Kit's farm does. Grow the cast before the geography.
- Original designs throughout — homage to a mood, never reproduction of existing characters/assets.

---

## 8. Change log

- **0.1 — 2026-11-18:** Initial roadmap draft: five phases plus release, Phase-001 detail, phase-document structure, team & workflow, guardrails.
- **0.2 — 2026-11-18:** Audience reworded (mainly pre-elementary–1st grade with a parent; older kids accommodated by challenge layers). Engine set to Godot 4.x with **C#**. Places recounted to eight. Phase-001 scope annotated with target future phases; future phase docs marked "(planned)"; Phase-003 goal drops Time Awareness (always on, Gentle Core); "PC first, tablet later" reconciled with the Phase-003 tablet row.
- **0.3 — 2026-11-19:** **Re-planned for the direction reset (GDD v0.19).** Vision rewritten (Kit 17, realistic but kid-friendly, educational, orders, automation, three difficulty modes). Audience column and age-stage sequencing removed; phases now follow farm growth (economy → land & buildings → automation → long-term goals), flagged as a proposal. **PC only** — tablet/multiplatform rows removed. Phase-001 now includes crop neglect (yellow → gone) at Normal timing. Renderer/assembly recorded in §6. Guardrails rewritten: "no decay caused by the player" and "no currency/no punishment" replaced by warned, kindly-shown, neglect-only losses; no debt; lane fixed but farm may grow.
