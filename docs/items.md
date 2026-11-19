# Items

> **Status:** Brainstorming draft — **updated for the direction reset 2026-11-19**
> **Version:** 0.5
> **Last updated:** 2026-11-19
> **Purpose:** Items the player can obtain: tools, produce, processed goods, building and automation items, decorations, and rare items. All names are tentative; every element is a proposal pending playtesting.

---

## 1. Scope and design intent

Items are readable at a glance and stored in picture-based, auto-stacking slots (inventory.md). Money is core. Harvested goods do not spoil.

---

## 2. Starting loadout

| Item | Type | What it does |
|---|---|---|
| **Hands** | Action | Plant, harvest, talk, give, place. |
| **Watering can** | Tool | Water beds (required for growth). |
| **Seed box** | Tool | Two or three starting seeds. |
| **Purse** | Wallet | 500 coins (economy.md). |
| **Old family hoe** | Tool | Till cleared ground. |

## 3. Tools

| Item | How obtained | What it does |
|---|---|---|
| **Butterfly net** | Optional Butterfly Garden request (Marlow) | Catch/release butterflies and fireflies. |
| **Hammer** | Shop, T2 | Break stones when clearing land. |
| **Axe** | Shop, T2 | Clear stumps and logs. |
| **Scythe** | Shop, T2 | Cut hay grass; harvest several crops at once. |
| **Brush** | Shop or reward, T2 | Groom sheep (wool quality). |
| **Fishing rod** | Brook keeper / shop, T2 | Fish at the brook and pond. |
| **Basket** | After first eggs | Carry many eggs. |
| **Trellis/stick** | After first tomato | Support climbing plants. |
| **Upgraded tools** | Workshop / shop, T3+ | Bigger can, faster hoe, golden variants (prestige). |
| **Garden lantern** | Winter mystery | Light for night walks; distinct from Lantern Hill's unlit lantern. |

## 4. Produce (harvested / collected)

Starting crops, farm crops, orchard fruit, rare crops (crops.md); eggs, milk, wool, honey, fish (animals.md); forage finds (berries, herbs, mushrooms). All stack by **quality (1–3 stars)**.

## 5. Processed goods

| Item | Made from | Where |
|---|---|---|
| **Jam** | Berries / orchard fruit + honey | Kitchen |
| **Bread** | Flour (wheat via mill) + egg | Kitchen |
| **Cheese** | Milk | Kitchen |
| **Pie** | Fruit or pumpkin + flour + egg | Kitchen |
| **Smoked fish** | Fish | Kitchen |
| **Flour** | Wheat | Windmill |
| **Fish food** | Crops | Workshop |
| **Compost** | Crop scraps | Compost bin |

Recipes are bought, given by friends, or won (GDD §4).

## 6. Building, crafting, and automation items

| Item | How obtained | Use |
|---|---|---|
| **Blueprints** | Shop, friendship, contests | Unlock crafting/building recipes |
| **Wood, stone, fibre** | Clearing land, foraging, shop | Building and crafting materials |
| **Sprinkler** (basic/quality/deluxe) | Craft or buy | Auto-watering |
| **Auto-feeder / pond auto-feeder** | Craft or buy | Auto-feeding |
| **Helper contract** | Helper post | Hire a helper |
| **Animal feed** (grain, hay) | Grow → silo, or buy | Feeding |

## 7. Decorations

Fences, paths, flower borders, benches, statues, lanterns, scarecrows, signposts, seasonal ornaments, painted doors — bought, crafted, or won. They raise **farm charm** (progression.md §6.1).

## 8. Rare and collectible items

Rare seeds, rare breeds, legendary fish, contest ribbons and trophies, fossils/shells from exploring, prestige tools — tracked in the Collection Book and shown in the showcase (progression.md §6).

## 9. NPC-held / world items (not player inventory)

| Item | Held by | Notes |
|---|---|---|
| **Marlow's flower seeds** | Mr. Marlow | Given with the optional Butterfly Garden request. |
| **Ledger** | Mr. Marlow | Picture pages recording promises and completed tasks; never carried. |
| **Old meadow sketch / pressed flowers** | Ledger | Visual mystery and memories. |
| **Unlit lantern** | Lantern Hill | World object; relit during lane restoration (progression.md §6.4). |
| **Order board** | Seed shed | Lists open neighbour orders. |

## 10. Exclusions

- Spoiling or perishable goods.
- Loose sortable rucksack or weight limits.
- Real-money items.

---

## Related documents

| Document | Relationship |
|---|---|
| [README.md](../README.md) | Centralized hub for the project. |
| [GDD.md](./GDD.md) | Ways to earn and acquire; automation. |
| [inventory.md](./inventory.md) | Slot categories and storage. |
| [mechanics.md](./mechanics.md) | Verbs that use these items. |
| [crops.md](./crops.md) | Crops and fruit. |
| [animals.md](./animals.md) | Animal products and fish. |
| [buildings.md](./buildings.md) | Buildings and automation fixtures. |
| [progression.md](./progression.md) | Decorations, collections, prestige items. |

## 11. Change log

- **0.1 — 2026-11-18:** Initial draft. Starting loadout, earned butterfly net, automatic shelf routing, NPC-held items, and exclusions. Day/night and season awareness noted where relevant.
- **0.2 — 2026-11-18:** Added opt-in items for challenge layers (Garden Coins, seed packets, tool upgrades, crafting outputs). Re-framed exclusions as "Gentle Core exclusions."
- **0.3 — 2026-11-18:** Added missing earned tools (Brush, Basket, Trellis/stick, Compost bin), added Carrot Patch and the new seasonal crops (Tomato Vine, Herb Pot), clarified the two lanterns as different items, added milk to the Garden Coins sources, and fixed the duplicate §8 numbering.
- **0.4 — 2026-11-19:** **Direction reset (GDD v0.19).** Opt-in/Gentle-Core split removed: money, seed packets, tool upgrades, and crafting outputs are core. Added purse, hoe/scythe/fishing rod, quality stacks, **processed goods** (jam, bread, cheese, pie, smoked fish, flour, fish food, compost), **blueprints, materials, and automation items** (sprinklers, feeders, helper contracts, feed), decorations for farm charm, rare/collectible items, and the order board. Compost bin moved to buildings.md.
- **0.5 — 2026-11-19:** Purse fixed at 500 coins (single mode, economy.md); old family hoe in the starting loadout; Butterfly Garden marked optional.
