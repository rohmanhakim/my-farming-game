# Items

> **Status:** Brainstorming draft
> **Version:** 0.3
> **Last updated:** 2026-11-18
> **Purpose:** Items the player can obtain, including usable items and additional items, including secrets or event-triggered items, adjusted to the scope of the game described in the GDD. All names are tentative and can be changed. Every element here is a proposal pending playtesting.

---

## 1. Scope and design intent

Items are few, meaningful, and never a source of inventory stress. There is no currency, no sellable stockpile, no sortable rucksack, and no perishable goods. Harvests go straight to the garden shelf.

---

## 2. Always available (starting loadout)

| Item | Type | How obtained | What it does |
|---|---|---|---|
| **Hands** | Action (not an inventory item) | From the start | Plant, harvest, decorate, talk, give, release creatures. |
| **Watering can** | Tool | From the start | Water plants; optional, speeds growth with sparkle feedback. |
| **Seed box** | Tool | From the start | Holds starting seeds (Sun Sprout, Clover Vine, Moon Petal, Carrot Patch — two or three of them). |

---

## 3. Earned through play

| Item | Type | How obtained | What it does |
|---|---|---|---|
| **Butterfly net** | Tool | Reward for the Butterfly Garden task (given by Marlow). | Catch butterflies and fireflies; show to Marlow or release in garden. |
| **Brush** | Tool | Unlocked after caring for the sheep (see progression.md). | Groom the sheep to collect wool. |
| **Basket** | Tool | Unlocked after the first egg collection (see progression.md). | Carry multiple eggs at once. |
| **Trellis/stick** | Tool | Unlocked after growing a tomato (see progression.md). | Supports climbing plants. |
| **Compost bin** | Tool | Unlocked mid-game (see progression.md). | Turns old plants into rich soil. |
| **Garden lantern** (tentative) | Tool | Later reward candidate; winter mystery reward (see progression.md). | Soft light for night walks; decorative. **Distinct from** the unlit lantern on Lantern Hill, which is a world object the player never carries. |

---

## 4. Grown and harvested

| Item | Type | How obtained | Where it goes |
|---|---|---|---|
| **Sun Sprout** | Crop / flower | Grown in garden bed. | Garden shelf automatically. |
| **Clover Vine** | Crop / flower | Grown in garden bed. | Garden shelf automatically. |
| **Moon Petal** | Crop / flower | Grown in garden bed. | Garden shelf automatically. |
| **Carrot Patch** | Crop / vegetable | Grown in garden bed. | Garden shelf automatically. |
| **Meadow Bell / Honey Cup / Lace Flower** | Task flower | Supplied by Marlow; grown for Butterfly Garden task. | Garden shelf automatically after harvest. |
| **Seasonal crops** (Berry Spray, Tomato Vine, Pumpkin Head, Frost Leaf, Herb Pot, Rain Drop) | Crop | Grown later, unlocked through play. | Garden shelf automatically. |

---

## 5. Decorations

| Item | Type | How obtained | Notes |
|---|---|---|---|
| **Garden stake** (tentative) | Decoration | Starting or early reward. | Placed in front garden for personalization. |
| **Potted herb** (tentative) | Decoration | Later reward candidate. | Small terracotta pot; creative ownership. |
| **Seasonal ornament** (tentative) | Decoration | Seasonal event candidate. | Autumn leaves, winter ornaments — optional, not required. |

---

## 6. NPC-held / event-triggered items (not player inventory)

| Item | Held by | How it appears | Notes |
|---|---|---|---|
| **Marlow's flower seeds** | Mr. Marlow | Supplied when the Butterfly Garden task is accepted. | Not in player's seed box until given. |
| **Old meadow sketch** | Mr. Marlow's ledger | Seen during dialogue or ledger browsing. | Visual mystery; not an inventory object. |
| **Pressed flower** | Mr. Marlow's ledger | Appears after a completed task (candidate relationship UI). | Not player-held. |
| **Ledger** | Mr. Marlow | The seed shed's big hand-illustrated book. | Marlow's object; player never carries it. |
| **Unlit lantern** | Lantern Hill post | World object on the hill. | Lit during the winter lantern lighting event candidate. A **world object only** — never the player's Garden lantern tool. |

## 7. Opt-in items (challenge layers only)

These items only appear when a parent turns on the corresponding challenge layer:

| Item | Type | How obtained | What it does |
|---|---|---|---|
| **Garden Coins** | Currency | Earned by selling crops, eggs, wool, honey, or milk at the market stand | Spent at the market stand to buy seeds, decorations, or tool upgrades |
| **Seed packet** | Consumable | Bought at the market stand | Planted in a garden bed to grow a crop |
| **Large watering can** | Tool | Bought or crafted as a tool upgrade | Holds more water; waters multiple plants at once |
| **Hoe** | Tool | Bought or crafted as a tool upgrade | Prepares soil for faster growth |
| **Scythe** | Tool | Bought or crafted as a tool upgrade | Harvests multiple plants at once |
| **Compost** | Crafting output | Made from old plants + soil | Enriches soil for faster crop growth |
| **Scarecrow** | Crafting output | Crafted from wood and cloth | Decorative; protects crops visually |
| **Bird feeder** | Crafting output | Crafted from wood and seeds | Attracts birds to the garden |

## 8. Gentle Core exclusions

The following items are excluded from Gentle Core and never appear unless a parent turns on the corresponding challenge layer:

- Currency / coins / money (may reappear as Garden Coins layer)
- Sellable stockpiles or produce for profit (may reappear as Garden Coins layer)
- Sortable rucksack or loose inventory
- Perishable or spoiling goods
- Crafting materials or recipes (may reappear as Crafting layer)

These items may reappear as opt-in challenge layers managed by the parent (see challenge-layers.md). Gentle Core never includes them.

---

## Related documents

| Document | Relationship |
|---|---|
| [README.md](../README.md) | Centralized hub for the project. |
| [GDD.md](./GDD.md) | No currency, no sellable stockpile, and no sorting come from the GDD scope. |
| [inventory.md](./inventory.md) | Items fill collectathon slots; harvested crops auto-stack with number badges. |
| [mechanics.md](./mechanics.md) | Tools (watering can, butterfly net, brush) live on the fixed tool belt. |
| [crops.md](./crops.md) | Grown crops become harvested items routed to the garden shelf. |
| [events.md](./events.md) | The butterfly net is earned via the Butterfly Garden core event. |
| [challenge-layers.md](./challenge-layers.md) | Defines optional challenge layers and parent-controlled toggles. |

## 9. Change log

- **0.1 — 2026-11-18:** Initial draft. Starting loadout, earned butterfly net, automatic shelf routing, NPC-held items, and exclusions. Day/night and season awareness noted where relevant.
- **0.2 — 2026-11-18:** Added opt-in items for challenge layers (Garden Coins, seed packets, tool upgrades, crafting outputs). Re-framed exclusions as "Gentle Core exclusions."
- **0.3 — 2026-11-18:** Added missing earned tools (Brush, Basket, Trellis/stick, Compost bin), added Carrot Patch and the new seasonal crops (Tomato Vine, Herb Pot), clarified the two lanterns as different items, added milk to the Garden Coins sources, and fixed the duplicate §8 numbering.