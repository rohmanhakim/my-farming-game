# Crops

> **Status:** Brainstorming draft
> **Version:** 0.10
> **Last updated:** 2026-11-20
> **Purpose:** Crops in the game, their seasons, growth days, uses, and the watering/neglect rule. All names are tentative; every element is a proposal pending playtesting. Prices live in [economy.md](./economy.md).

---

## 1. Scope and design intent

Crops are the heart of the farm economy: they fill orders, sell at the market, feed animals, and become jam, bread, and pies.

- **Day-based growth, like *Harvest Moon*** (owner decision 2026-11-19): every crop takes **at least one in-game day**; most take several.
- **Crops need water every day.** **2 dry days in a row → yellow; 3 dry days in a row → gone** (challenge-layers.md §2). Watering a yellow crop recovers it.
- **Season-aware:** each crop sprouts only in its season(s); out-of-season seeds don't sprout (no loss). The **greenhouse** and **hydroponics** grow anything year-round.
- **Harvested goods don't spoil.**
- **No fixed bed limit** — any cleared tile can be tilled; money, clearing, and finally land are the limits (buildings.md §2).

---

## 2. Growth and neglect model

- **A crop grows one day each night it was watered** that day (or rained on). Sleeping — or the automatic 21:30 bedtime — ends the day (GDD §4).
- **Dry day:** no growth; cracked-soil look.
- **2 dry days in a row:** leaves droop and turn yellow — the warning.
- **3 dry days in a row:** the crop is gone; the tile stays tilled; a short note gives a tip ("Water every day, or try a sprinkler").
- **Rain:** rainy days water every outdoor crop (≈ 1 day in 5 in Spring/Autumn, fewer in Summer; never in the greenhouse).
- **While the game is closed:** nothing changes — days only pass during play.
- **Regrowing crops** (tomato, berries, hay grass) keep producing after their first harvest if watered.
- **Quality (★–★★★):** compost and never missing a watering day raise it.
- **Starting garden:** 3 cleared beds in the front garden; the rest must be cleared first (progression.md §4).

---

## 3. Starting seeds

| Name | Season | Days to harvest | Uses |
|---|---|---|---|
| **Carrot** | Spring / Summer | 3 | First orders, sell, animal treat |
| **Sun Sprout** (flower) | Spring / Summer | 4 | Sell, flower orders, decoration |
| **Clover Vine** | Spring / Autumn | 5 | Bee food, animal feed |
| **Moon Petal** (flower) | Summer / Autumn | 6 | Gift, sell; glows at night |

## 4. Flowers for the Butterfly Garden request (optional, later)

The Butterfly Garden is now an **optional later request** from Marlow (the first beat is a first order — GDD §3). He supplies the seeds.

| Name | Season | Days |
|---|---|---|
| **Meadow Bell** | Spring | 4 |
| **Honey Cup** | Spring | 4 |
| **Lace Flower** | Spring | 4 |

## 5. Farm crops (seed shed or merchant)

| Name | Season | Days | Regrows | Main uses | Unlock |
|---|---|---|---|---|---|
| **Wheat** | Summer / Autumn | 7 | — | Flour → bread; chicken feed | T2 |
| **Hay grass** | Spring–Autumn | 4 | every 4 days | Cow/sheep feed (silo) | T2 |
| **Tomato** | Summer | 8 | every 3 days | Orders, sauces; needs trellis | T2 |
| **Berries** | Summer | 8 | every 6 days | Jam, pie, bee food | T2 |
| **Herbs** | All (indoor / greenhouse) | 6 | — | Orders (Marlow), cooking | T2 |
| **Pumpkin** | Autumn | 12 | — | Pie, contests | T3 |
| **Frost Leaf** | Winter | 5 | — | Winter orders | T3 |
| **Rain Drop** | Spring / Autumn | 6 | — | Sell, gift | T3 |

## 6. Orchard trees

A sapling needs **one full season** of watering to mature, then fruits every year in its season (buildings.md: 2 × 2 tiles per tree). A mature tree is never lost (established trees survive dry spells — realistic) but gives no fruit that season if it is never watered. Young saplings follow the normal 3-dry-days rule.

| Tree | Fruit season | Harvest | Uses |
|---|---|---|---|
| **Apple** | Autumn | every 3 days | Pies, jam |
| **Pear** | Autumn | every 3 days | Jam, sell |
| **Cherry** | Summer | every 3 days | Jam, pie |
| **Plum** | Summer | every 3 days | Jam |

## 7. Rare crops (collection)

Golden pumpkin, giant variants, merchant-only seeds — progression.md §6.2.

## 8. Crop uses

| Use | Description |
|---|---|
| **Orders** | Delivered to neighbours (main income). |
| **Sell** | Market stall, shipping box, farm shop. |
| **Process** | Kitchen and the village watermill: jam, flour, bread, pie. |
| **Feed** | Hay and wheat → silo → feeders. |
| **Decoration / gift** | Flower borders; gifts. |

---

## Related documents

| Document | Relationship |
|---|---|
| [README.md](../README.md) | Centralized hub for the project. |
| [GDD.md](./GDD.md) | Growth model, neglect rule, ways to earn. |
| [economy.md](./economy.md) | Seed and crop prices; profit per tile. |
| [mechanics.md](./mechanics.md) | Plant, water, harvest, process verbs. |
| [buildings.md](./buildings.md) | Beds, greenhouse, hydroponics, orchard, sprinklers, silo. |
| [items.md](./items.md) | Harvested and processed goods. |
| [challenge-layers.md](./challenge-layers.md) | The 3-dry-days rule. |
| [events.md](./events.md) | Crop and rain events. |

## 9. Change log

- **0.1 — 2026-11-18:** Initial draft. Small seasonal crop list with no withering, no economy, and automatic shelf routing. Butterfly Garden flowers supplied by NPC task.
- **0.2 — 2026-11-18:** Added Carrot Patch to starting seeds; added Tomato Vine, Herb Pot. Clover Vine and Berry Spray noted as bee food. Winter indoor crops (Frost Leaf, Herb Pot) keep gardening alive in cold months.
- **0.3 — 2026-11-18:** Reconciled tables with change log 0.2 (Carrot Patch, Tomato Vine, Herb Pot now appear in the tables; bee-food notes added to Clover Vine and Berry Spray). Added the season-persistence rule: grown plants keep blooming after their season, so butterfly visits in spring and summer are consistent.
- **0.4 — 2026-11-18:** Growth model §2 updated to the Milestone 0 decision (GDD §4, v0.16): stage timing default **Medium ≈30–45 s** (Fast/Slow presets for the options screen), growth paused while the game is closed by default, optional positive-only Gentle catch-up (+1 stage max, never past bloom), both set before starting a new game. Replaced the old “closed-game growth is TBD” line.
- **0.5 — 2026-11-19:** **Direction reset (GDD v0.19).** No-withering rule replaced: crops **need water**, dry soil pauses growth, neglected crops **turn yellow then are gone** (grace per mode). Economy uses added (orders, selling, processing, feed). 2–3-bed limit is now only the starting garden. Added farm crops (wheat, hay grass), **orchard trees**, crop quality, greenhouse year-round growing, rare crops as a collection, and unlock tiers.
- **0.6 — 2026-11-19:** **Day-based growth (owner decision):** play-time stages (≈30–45 s, Fast/Medium/Slow, Gentle catch-up) replaced by **days to harvest, minimum 1 day**, growing one day per watered night, Harvest Moon-style. Neglect fixed to **2 dry days → yellow, 3 → gone** (single mode). Added **rain** (waters outdoor crops), regrow intervals, sapling maturing rule, and days for every crop. Butterfly Garden flowers moved to an optional later request. "Carrot Patch / Herb Pot / Tomato Vine / Berry Spray" shortened to Carrot / Herbs / Tomato / Berries.
- **0.7 — 2026-11-20:** Bedtime reference updated to 01:00 (GDD v0.29).
- **0.8 — 2026-11-20:** Bedtime reference corrected to midnight (GDD v0.30).
- **0.9 — 2026-11-20:** Bedtime reference updated to 21:30 (GDD v0.31).
- **0.10 — 2026-11-20:** **Windmill / water pump removed (owner decision 2026-11-20):** flour is milled at the village watermill.
