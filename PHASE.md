# PHASE.md — Development Roadmap: Prototype to Production

> **Status:** Planning draft  
> **Version:** 0.2  
> **Last updated:** 2026-11-18  
> **Purpose:** High-level overview of development from the first playable prototype to a final, production-ready game. Each phase document expands this roadmap — `docs/Phase-001.md` exists today; Phase-002 through Release are **planned** documents to be written when their phase is scheduled.

## 1. Vision

A gentle, fully 2D life-and-garden game for young children — mainly pre-elementary through first grade, playing with a parent — that grows with the child (older children up to fifth grade are accommodated through optional challenge layers). The game never punishes, never requires reading to act, and always keeps the Gentle Core available. Parents act as the "difficulty dial" through optional challenge layers.

## 2. Guiding Principles (apply to all phases)

- **Two Modes:** Gentle Core is always on and never gets harder. Challenge layers are opt-in, parent-controlled, and additive.
- **Small surface area:** One lane, eight places, small cast. Grow cast before geography.
- **No punishment:** No crop/animal death, no currency, no time pressure, no reading required.
- **Presenter-like storytelling:** Characters address the player, pause for an answer, and respond to what happened.
- **Collectathon inventory:** Picture-based, auto-stacking, no "full" penalty.
- **PC first, tablet later:** Early phases target PC; iPad/Android tablets are introduced from Phase 003 onward as the longer-term touchscreen goal.

## 3. Development Phases

| Phase | Document | Goal | Platform | Audience |
|-------|----------|------|----------|----------|
| **Phase 001** | `docs/Phase-001.md` | Interaction prototype: vertical slice of the core loop | PC | Preschool (Gentle Core only) |
| **Phase 002** | `docs/Phase-002.md` (planned) | Animal care, collectathon inventory, second place | PC | Preschool – K/1st |
| **Phase 003** | `docs/Phase-003.md` (planned) | Challenge layers (multi-step requests), co-play | PC + tablet | K/1st – 2nd/3rd |
| **Phase 004** | `docs/Phase-004.md` (planned) | Garden Coins, tool upgrades, crafting, market stand | PC + tablet | 2nd/3rd – 4th/5th |
| **Phase 005** | `docs/Phase-005.md` (planned) | Full seasonal festivals, relationship meters, expanded world | Multiplatform | 4th/5th – 5th grade+ |
| **Release** | `docs/Release.md` (planned) | Production-ready, polished, localized, age-rated | Multiplatform | All stages |

## 4. Phase 001 — Interaction Prototype (in detail)

**Goal:** One small, playable scene that validates the core loop: Mr. Marlow asks → child plants and waters 3 flowers → flowers grow → child harvests and shows them → Mr. Marlow gives the butterfly net → child can catch/release butterflies.

**Scope:** Player's front garden + Mr. Marlow's seed shed gate. Single-player. Spring season only. No animals, no inventory grid, no stickers, no challenge layers, no currency — **deferred to future phases, not cut**: animals, inventory, and stickers → Phase-002; challenge layers & co-play → Phase-003; market stand & currency → Phase-004; stile, Lantern Hill, brook, allotment, relationship meter → later phases (see §3).

**Exit criteria:** A child can start the game with little or no spoken explanation, complete the Butterfly Garden task, earn the net, and understand what to do next without adult help.

## 5. How Phase Documents Are Structured

Each `docs/Phase-00N.md` breaks its milestone into **session tasks**. One task = one scoped, playtested artifact that can be created and validated in a single working session with AI assistance. Tasks are ordered by dependency and designed so the game remains playable and coherent after every task.

## 6. Team & Workflow

- **Team:** Two people, with AI assistance.
- **Engine:** Godot 4.x with **C#**.
- **Art:** Placeholder art for prototypes; final European children's-book/comics art swapped in later without code changes.
- **Audio:** Free royalty-free assets; licenses recorded.
- **Save format:** One save slot, auto-save, human-readable JSON for debugging.

## 7. Guardrails (never change across phases)

- No antagonist, threat, deadline, or decay caused by the player.
- No disappointment from Mr. Marlow, ever.
- No lore the child must read or memorize to proceed.
- No expansion by map-size: grow the cast before the geography.
- Original designs throughout — homage to a mood, never reproduction of existing characters/assets.

---

## 8. Change log

- **0.1 — 2026-11-18:** Initial roadmap draft: five phases plus release, Phase-001 detail, phase-document structure, team & workflow, guardrails.
- **0.2 — 2026-11-18:** Audience reworded (mainly pre-elementary–1st grade with a parent; older kids accommodated by challenge layers). Engine set to Godot 4.x with **C#**. Places recounted to eight. Phase-001 scope annotated with target future phases; future phase docs marked "(planned)"; Phase-003 goal drops Time Awareness (always on, Gentle Core); "PC first, tablet later" reconciled with the Phase-003 tablet row.