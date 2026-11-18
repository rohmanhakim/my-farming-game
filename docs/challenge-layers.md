# Challenge Layers & Parent Controls

> **Status:** Design draft
> **Version:** 0.2
> **Last updated:** 2026-11-18
> **Purpose:** Defines the optional challenge layers that parents can toggle on or off. Gentle Core is always available.

## 1. Design Philosophy

Challenge layers are **additive, never subtractive**. The Gentle Core (plant, water, harvest, care for animals, decorate) never gets harder, never requires reading, and never introduces currency or time pressure.

Layers are controlled by the parent or caregiver through a grown-up menu. Nothing unlocks automatically based on playtime or age. The parent and child decide together when to turn a layer on — and can turn it off at any time with one click.

## 2. Challenge Layer Table

| Layer | What it adds | Default | Suggested stage |
|---|---|---|---|
| **Words vs. Pictures** | NPC dialogue shows simple sentences with voice-over; picture-only mode stays available | Pictures only | Preschool |
| **Garden Coins** | Earn coins at market stand; buy seed packets and decorations | Off | 2nd–3rd grade |
| **Tool Upgrades** | Larger watering can, hoe, scythe, trellis | Off | 2nd–3rd grade |
| **Crafting** | Compost, scarecrow, bird feeder recipes (picture-based) | Off | 2nd–3rd grade |
| **Multi-step Requests** | Neighbour asks for 2+ items or seasonal discoveries | Off | K–1st grade |
| **Session Timer** | Gentle break reminder after set play time | On (default) | All stages |

Day/night and seasons are **always on** (Gentle Core — see mechanics.md §2–§3), so there is no Time Awareness layer.

## 3. Stage Map

| Stage | Age | Typical active layers | Parent's role |
|---|---|---|---|
| **Stage 1 — My First Garden** | Preschool (3–5) | None (Gentle Core only) | Reads dialogue, presses buttons, narrates |
| **Stage 2 — Garden Helpers** | K–1st (5–7) | Multi-step Requests | Suggests optional goals, reads hard words |
| **Stage 3 — Farm Manager in Training** | 2nd–3rd (7–9) | Garden Coins, Tool Upgrades, Crafting | Runs shop menus, explains prices, helps plan |
| **Stage 4 — Bramblewick Farmer** | 4th–5th (9–11) | All layers + festivals, relationship meters | Co-op neighbour, advisor |
| **Stage 5 — Standard Sim** | 5th grade+ (11+) | Full economy, crafting, expanded world | Optional co-op partner or checks in |

## 4. One-Click Reset

A parent setting returns every layer to Gentle Core instantly. The simple garden is always available, no matter how many layers were previously turned on.

## 5. Parent Gate

The grown-up menu is placed deep in the pause menu so the child cannot accidentally flip layers during play.

## 6. Related documents

| Document | Relationship |
|---|---|
| [README.md](../README.md) | Centralized hub for the project. |
| [GDD.md](./GDD.md) | Two-mode principle and progressive challenge pillar |
| [progression.md](./progression.md) | Stickers, neighbour requests, and stage-based parent role |
| [mechanics.md](./mechanics.md) | Optional verbs (sell, buy, craft, upgrade) for challenge layers |
| [controls.md](./controls.md) | Grown-up menu button mapping and two-gamepad division of labor |
| [inventory.md](./inventory.md) | Opt-in coin and market-stock categories |
| [items.md](./items.md) | Challenge-layer items (coins, seed packets, tool upgrades) |
| [events.md](./events.md) | Stage-gated seasonal festivals and relationship events |

## 7. Change log

- **0.1 — 2026-11-18:** Initial draft. Defines challenge layers, stage map, one-click reset, and parent gate.
- **0.2 — 2026-11-18:** Removed the Time Awareness layer (day/night and seasons are always on, Gentle Core). Session Timer clarified as on by default. Fixed "pase menu" typo, numbered the change log, added the README row, and normalised "neighbor" to "neighbour".