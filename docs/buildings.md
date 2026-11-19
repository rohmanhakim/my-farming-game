# Buildings & Farm Structures

> **Status:** Brainstorming draft
> **Version:** 0.2
> **Last updated:** 2026-11-19
> **Purpose:** Everything Kit can build on the farm — cost, footprint, capacity, when it unlocks, what it teaches, and how it fits the land. Sized against the farm map below so the land is a real, countable limit. Costs come from [economy.md](./economy.md) (≈ half real price). All numbers are proposals pending playtesting.

---

## 1. Design intent

- **The land is all Kit's from day one, but overgrown.** It must be cleared before it can be used (progression.md §4). Early on, **money** limits what Kit builds; later, **cleared land** and finally **total land** do.
- **Capacity comes from the building, not a global cap.** A barn with 5 stalls holds 5 cows; a bigger barn holds more but needs more land (owner decision 2026-11-19).
- **Each building teaches something real** (GDD pillar 3) and opens an activity or saves work.
- **No upkeep bills, no breakdowns.** Some buildings use materials the farm produces (feed, compost).
- Bought at **Marlow's seed shed / builder's catalogue**, or built from a **blueprint + materials** at the workshop.

## 2. The farm map (land budget)

One tile = one crop square (about 1 m × 1 m in-world). All areas are owned from the start.

| Area | Size (tiles) | Tiles | Starts as |
|---|---|---|---|
| **Front garden** | 8 × 6 | 48 | 3 beds already clear; the rest weeds |
| **Field A** (behind the house) | 12 × 10 | 120 | Weeds, small stones |
| **Field B** | 12 × 10 | 120 | Weeds, stones, a few stumps |
| **Field C** | 16 × 12 | 192 | Stumps, stones, tall grass |
| **Field D** | 16 × 12 | 192 | Big stones, logs |
| **Field E** (by the brook) | 20 × 16 | 320 | Boulders, big logs; a natural pond spot |
| **Total** | | **992** | |

Paths between buildings are needed for walking; plan on roughly 15 % of the land for paths. **A fully built farm has room for about 850 usable tiles** — enough to feel big, small enough that choices matter (§5).

## 3. Catalogue

Cost tiers: see economy.md for exact numbers (coins). Footprint in tiles (width × height).

| Building | What it does | Cost (coins) | Footprint | Capacity | Unlock | What it teaches | Automation / land tie-in |
|---|---|---|---|---|---|---|---|
| **Tilled bed** | Plant one crop | free (hoe) | 1 × 1 | 1 crop | Start | Soil and seeds | Sprinklers water beds in range; crops are the biggest land user. |
| **Compost bin** | Scraps → compost (better crop quality) | 300 | 1 × 1 | — | T1 | Recycling, soil health | A helper can empty it. |
| **Small coop** | Houses chickens | 750 | 4 × 3 = 12 | **5 hens** | T1 (after first order) | What hens need | Fits one auto-feeder. |
| **Big coop** (upgrade) | More hens | +1,250 | 6 × 4 = 24 | **10 hens** | T2 | Scaling up | — |
| **Beehive** | Honey; pollinates crops within 5 tiles | 2,000 | 1 × 1 | 1 colony | T2 | Pollination | Up to 8 hives = an apiary (8 tiles). No feeding; needs flowers nearby. |
| **Workshop** | Craft tools, fences, decorations, sprinklers, feeders | 15,000 | 4 × 3 = 12 | — | T2 | Making and repairing | Required to craft automation. |
| **Kitchen** (house upgrade) | Jam, bread, cheese, pie, smoked fish | 25,000 | 0 (in house) | — | T2 | Where food comes from | Processed goods earn more per tile. |
| **Market stall** | Sell to passers-by on the lane | 1,500 | 0 (on the lane) | — | T2 | Pricing, demand | A helper can stock it. |
| **Small barn** | Houses cows and sheep | 75,000 | 6 × 5 = 30 | **5 stalls** | T3 | Large animals' needs | Fits auto-feeders. |
| **Big barn** (upgrade) | More stalls | +75,000 | 8 × 6 = 48 | **10 stalls** | T3 | Scaling up | — |
| **Small fish pond** | Stock, feed, harvest fish | 10,000 | 4 × 4 = 16 | **10 fish** | T3 | Water ecosystems | Pond auto-feeder fixture. Free in Field E's pond spot after clearing (no dig cost). |
| **Large fish pond** | More fish | 30,000 | 6 × 6 = 36 | **25 fish** | T3 | — | — |
| **Fruit tree (orchard)** | Fruit every season for years | 150 per sapling | 2 × 2 = 4 per tree | 1 tree | T3 | Perennial vs annual crops | Sprinklers water trees. |
| **Storage silo** | Stores feed (hay, grain) | 25,000 | 3 × 3 = 9 | 240 portions | T3 | Planning for winter | **Required for auto-feeders.** |
| **Sprinkler** (basic / quality / deluxe) | Waters beds every morning | 100 / 300 / 750 (or craft) | 1 × 1 | 8 / 24 / 48 tiles around it | T4 (one basic sprinkler earlier as a reward) | Irrigation | Needs a water source within 10 tiles (well, pond, pump). |
| **Auto-feeder** | Feeds a coop or barn daily from the silo | 300 (or craft) | 0 (inside) | 1 building | T4 | Saving work | Core automation. |
| **Pond auto-feeder** | Feeds a pond daily | 500 (or craft) | 0 (on pond) | 1 pond | T4 | — | Core automation. |
| **Windmill / water pump** | Water source for sprinklers anywhere; mills flour; powers hydroponics | 15,000 | 3 × 3 = 9 | Waters 1 field | T4 | Renewable energy | Extends sprinkler reach to dry fields. |
| **Greenhouse** | Any crop, any season | 20,000 | 7 × 6 = 42 | **24 crop tiles inside** | T4 | Climate control | Built-in sprinkler upgrade (+3,000). |
| **Hydroponics bay** | Soil-free racks: grows 25 % faster, needs no watering, stacks 2 high | 7,500 | 4 × 3 = 12 | **24 crop slots** (12 tiles × 2 levels) | T5 | Water efficiency | **Needs power** (windmill within range). Best food per tile when land is full. |
| **Farmhand cottage** | Houses a helper (harvests, collects, delivers) | 50,000 | 4 × 4 = 16 | 1 helper, covers 1 field | T4 | Teamwork | Core automation. |
| **Farm shop** | Customers come to the farm; standing orders | 100,000 | 5 × 4 = 20 | — | T5 | Running a business | A helper can run it. |
| **Showcase** | Displays collections and trophies | 30,000 | 5 × 4 = 20 | — | T5 | — | progression.md §6. |
| **Decorations, paths, fences** | Beauty and layout | 75–1,500 | 0–1 | — | T1+ | Design | Raise farm charm; compete with crops for land. |

## 4. Example: a fully built farm fits the land

| Use | Tiles |
|---|---|
| Paths (~15 %) | 150 |
| Big coop × 2 (20 hens) | 48 |
| Big barn × 2 (20 large animals) | 96 |
| Large fish pond | 36 |
| Greenhouse | 42 |
| Hydroponics bay × 2 | 24 |
| Orchard, 12 trees | 48 |
| Workshop, silo, windmill, farmhand cottage, farm shop, showcase | 86 |
| Apiary (8 hives) | 8 |
| Decorations | ~40 |
| **Left for outdoor crop beds** | **~414** |
| **Total** | **992** |

If a player wants three barns, a second greenhouse, and a big orchard, something else has to give. That trade-off is the late game.

## 5. Placement rules

- Every building except the kitchen and market stall uses farm tiles, and can only be placed on **cleared** land.
- Buildings can be **moved** for free and **sold back** for half what Kit paid.
- When land is full, growth means **better use of land**: hydroponics, greenhouse, orchards, processing, layout.

## 6. Open questions

1. Final prices and footprints after the first economy playtest (economy.md §6).
2. Should Field E's natural pond spot be a free small pond?
3. Should buildings ever need repairs? (Current answer: no.)

## 7. Related documents

| Document | Relationship |
|---|---|
| [README.md](../README.md) | Centralized hub for the project. |
| [GDD.md](./GDD.md) | Pillars (educational, grow your farm, automation). |
| [economy.md](./economy.md) | Exact prices. |
| [progression.md](./progression.md) | Tiers and land clearing. |
| [animals.md](./animals.md) | Housing capacity = animal limit. |
| [crops.md](./crops.md) | Beds, greenhouse, hydroponics, orchard. |
| [mechanics.md](./mechanics.md) | Build, clear, automation verbs. |
| [places.md](./places.md) | Where the fields are. |

## 8. Change log

- **0.1 — 2026-11-19:** New document (owner request). Catalogue of 18 farm structures — bed, compost bin, coop, beehive/apiary, workshop, kitchen, market stall, silo, barn, fish pond, orchard, sprinklers, windmill/water pump, greenhouse, hydroponics, farm shop, helper post, decorations — each with cost tier, footprint, unlock tier, education angle, and automation/land tie-in. Placement rules (move free, sell back at half, land as the final limit).
- **0.2 — 2026-11-19:** **Aligned with the land (owner request).** Added **§2 farm map**: front garden + five fields, **992 tiles, all owned from the start but overgrown**. Every building now has an exact footprint and a **capacity** (5-stall barn = 5 animals; coop, pond, greenhouse, hydroponics likewise); costs in coins from economy.md (≈ half real price). Added §4 worked example showing a full farm fits the land with ~414 crop tiles left. "Land bought" replaced by "land cleared". Hydroponics now needs no watering; showcase added.
