# Inventory System

> **Status:** Design draft — **updated for the direction reset 2026-11-19**
> **Version:** 0.3
> **Last updated:** 2026-11-19
> **Purpose:** Picture-based, auto-stacking inventory with a wallet and storage. Easy to read, no sorting, no "full" penalty.

## 1. Design Intent

The inventory should feel like a well-labelled pantry, not a logistics puzzle. Money is now core (GDD §4), but inventory management is still not where the challenge lives: there is no sorting, no weight limit, and no spoilage.

## 2. Slot Categories

| Category | Icon | What it holds |
|---|---|---|
| **Seeds** | 🌱 | Seed packets, saplings, fingerlings |
| **Tools** | 🛠️ | Watering can, net, brush, rod, hoe, scythe… |
| **Animal Products** | 🥚 | Eggs, milk, wool, honey, fish |
| **Harvest** | 🥕 | Crops, fruit, forage finds |
| **Processed Goods** | 🫙 | Jam, bread, cheese, pie, smoked fish, flour |
| **Materials** | 🪵 | Wood, stone, fibre, feed, compost |
| **Decorations** | 🪴 | Fences, paths, ornaments, trophies to place |
| **Caught Creatures** | 🦋 | Butterflies, fireflies (always released) |

Slot counts per category: TBD during UI design (start ≈ 8, upgradable with a bigger backpack/storage).

## 3. Auto-Stacking and Quality

Identical items stack into one slot with a number badge; items of **different quality (1–3 stars)** stack separately. No manual sorting or dragging.

## 4. Storage — No "Full" Penalty

If a category is full, new items go to the **storage chest** at the house (the old Overflow Nest at Marlow's shed is replaced, because storage is now the player's own). Feed (grain, hay) goes to the **silo** once built. Nothing is ever lost because space ran out.

## 5. Wallet

Money is shown on the HUD and in the inventory screen. It never goes below zero (no debt).

## 6. Collection Book

A gallery in the pause menu showing every discoverable crop, fish, breed, recipe, forage find, and rare item. Unfound entries are silhouettes. It is a trophy case, not a required checklist (progression.md §6.2).

## 7. Exclusions

- Sorting, dragging, weight limits.
- Spoilage.
- "Full inventory" blocking play.

## 8. Co-play

On one PC, either player can open the inventory with their own controller; the wallet and storage are shared (proposal, GDD §16 Q4).

## Related documents

| Document | Relationship |
|---|---|
| [README.md](../README.md) | Centralized hub for the project. |
| [GDD.md](./GDD.md) | Economy and ways to earn. |
| [items.md](./items.md) | Items in each category. |
| [mechanics.md](./mechanics.md) | Verbs that fill and empty slots. |
| [animals.md](./animals.md) | Quality products. |
| [buildings.md](./buildings.md) | Storage silo. |
| [progression.md](./progression.md) | Collections and sticker book. |

---

## 9. Change log

- **0.1 — 2026-11-18:** Initial draft. Slot categories, auto-stacking, Overflow Nest, Collection Book, opt-in categories, Gentle Core exclusions, parent role.
- **0.2 — 2026-11-18:** Fixed duplicate section numbering (two §4s), added the missing change log, added milk to Animal Products, and noted that the Collection Book lives in the pause menu (not on Y).
- **0.3 — 2026-11-19:** **Direction reset (GDD v0.19).** Wallet made core (no debt); opt-in coin/market categories and Gentle Core exclusions removed. Added Processed Goods and Materials categories, fish in Animal Products, **quality stacks**, storage chest at the house (replaces the Overflow Nest at Marlow's shed) and silo for feed. Parent role replaced by a co-play note; tablet turn-taking removed (PC only).
