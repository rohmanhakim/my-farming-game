# Inventory System

> **Status:** Design draft
> **Version:** 0.2
> **Last updated:** 2026-11-18
> **Purpose:** Collectathon-style inventory for young children. Picture-based, auto-stacking, no currency, no "full" penalty.

## 1. Design Intent

Kids love collecting things. The inventory should feel like a sticker book or treasure chest, not a logistics puzzle. There is no currency, no selling, no sorting, and no weight limit. Harvests and animal products go straight into picture slots.

## 2. Slot Categories

| Category | Icon | What it holds | Max slots |
|---|---|---|---|
| **Seeds** | 🌱 | Seed packets (carrot, tomato, butterfly flowers, etc.) | 6 |
| **Tools** | 🛠️ | Watering can, butterfly net, brush, basket, trellis | 6 |
| **Animal Products** | 🥚 | Eggs, wool bundles, honey jars, milk bottles | 6 |
| **Harvested Crops** | 🥕 | Vegetables, flowers, fruit from garden beds | 6 |
| **Decorations** |� | Garden stakes, potted herbs, seasonal ornaments | 6 |
| **Caught Creatures** | 🦋 | Butterflies, fireflies (catch and release only) | 6 |

## 3. Auto-Stacking

Identical items stack automatically into one slot with a number badge (e.g., 5 eggs = one slot showing "5"). No manual sorting, dragging, or organizing is required. The **Collection Book** (section 6) lives in the pause menu, not on the Y button.

## 4. No "Full" Penalty

If a category reaches its slot limit, new items go to the **Overflow Nest** at Mr. Marlow's shed. The Overflow Nest is always visible and accessible. A parent can help the child decide what to gift to Marlow or keep.

## 5. Opt-in categories (challenge layers only)

These categories only appear when a parent turns on the corresponding challenge layer:

| Category | Icon | What it holds | Active when |
|---|---|---|---|
| **Coins / Wallet** | 🪙 | Garden Coins earned at the market stand | Garden Coins layer is on |
| **Market Stock** | 🏪 | Items listed for sale (seed packets, decorations, tool upgrades) | Garden Coins layer is on |

## 6. Collection Book

A gallery in the pause menu showing every discoverable item. Uncollected items appear as silhouettes until found. Collected items show in full color. No penalties for missing items; the book is a trophy case, not a checklist the child must complete.

## 7. Gentle Core exclusions

The following are excluded from Gentle Core and never appear unless a parent turns on the corresponding challenge layer:

- Currency / coins / money (may reappear as the Garden Coins layer)
- Sellable stockpiles or produce for profit (may reappear as the Garden Coins layer)
- Sortable rucksack or loose inventory
- Perishable or spoiling goods
- Crafting materials or recipes (may reappear as the Crafting layer)
- Inventory management puzzles
- "Full inventory" blocking gameplay

These items may reappear as opt-in challenge layers managed by the parent (see challenge-layers.md). Gentle Core never includes them.

## 8. Parent Role

On PC with two gamepads, the parent can open the inventory on their controller to help the child decide what to keep or gift. On tablet, inventory is accessed during turn-taking. Essential actions never require opening the inventory. When the Garden Coins layer is on, the parent can also help manage coins and market stock.

## Related documents

| Document | Relationship |
|---|---|
| [README.md](../README.md) | Centralized hub for the project. |
| [GDD.md](./GDD.md) | Pillar 7 (collectathon inventory) defines the no-currency, no-full-penalty approach. |
| [mechanics.md](./mechanics.md) | Fixed tool belt and garden shelf work alongside collectathon slots. |
| [items.md](./items.md) | Items populate the collectathon slot categories. |
| [animals.md](./animals.md) | Animal products (egg, wool, honey) fill animal product slots; overflow goes to Marlow's shed. |
| [crops.md](./crops.md) | Harvested crops fill harvested crop slots and route to the garden shelf. |
| [places.md](./places.md) | The Overflow Nest is located at Mr. Marlow's shed. |
| [challenge-layers.md](./challenge-layers.md) | Defines optional challenge layers and parent-controlled toggles. |

---

## 9. Change log

- **0.1 — 2026-11-18:** Initial draft. Slot categories, auto-stacking, Overflow Nest, Collection Book, opt-in categories, Gentle Core exclusions, parent role.
- **0.2 — 2026-11-18:** Fixed duplicate section numbering (two §4s), added the missing change log, added milk to Animal Products, and noted that the Collection Book lives in the pause menu (not on Y).