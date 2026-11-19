# Crops

> **Status:** Brainstorming draft — **updated for the direction reset 2026-11-19**
> **Version:** 0.5
> **Last updated:** 2026-11-19
> **Purpose:** Crops in the game, their seasons, uses, and the watering/neglect rule. All names are tentative; every element is a proposal pending playtesting.

---

## 1. Scope and design intent

Crops are the heart of the farm economy: they fill orders, sell at the market, feed animals, and become jam, bread, and pies.

- **Crops need water** (GDD §4). Dry soil pauses growth; a crop left dry **turns yellow**, then is **gone** (bed empty again). Watering a yellow crop recovers it.
- **Season-aware:** each crop sprouts only in its season(s). Out-of-season seeds don't sprout — no loss. A grown crop is harvestable after its season ends (kept from v0.3). The **greenhouse** grows anything year-round (buildings.md).
- **Harvested goods don't spoil** (no added complexity).
- **No fixed bed limit** — beds are bought; money is the early limit, land the final one.

---

## 2. Growth and neglect model (summary)

- Growth runs on **accumulated play time while the soil is moist** (GDD §4). Default **Medium ≈ 30–45 s per stage**; Fast/Slow presets on the new-game screen. Nothing grows or dries while the game is closed.
- **Watering** moistens the soil for about one in-game day; sprinklers water automatically each morning.
- **Neglect stages:** moist (growing) → dry (paused, cracked-soil look) → **yellow** (drooping, yellow-brown leaves — the warning) → **gone**. Grace times per mode in challenge-layers.md §3.
- **Quality:** compost and consistent watering raise crop quality (1–3 stars).
- **Starting garden:** 2–3 beds in the front garden — a starting point, not a cap.

---

## 3. Starting seeds

| Tentative name | Season | Growth | Uses |
|---|---|---|---|
| **Sun Sprout** | Spring / Summer | Fast | Sell, decoration, flower orders |
| **Clover Vine** | Spring / Autumn | Medium | Bee food, animal feed |
| **Moon Petal** | Summer / Autumn | Medium | Gift, sell; glows at night |
| **Carrot Patch** | Spring / Summer | Fast | Orders, sell, animal treat |

## 4. Butterfly Garden flowers

Supplied by Mr. Marlow for the first task (replacement seeds given kindly if they are lost).

| Name | Season | Notes |
|---|---|---|
| **Meadow Bell** | Spring | Attracts butterflies |
| **Honey Cup** | Spring | Attracts butterflies; bee favourite |
| **Lace Flower** | Spring | Attracts butterflies |

## 5. Farm crops (bought at the seed shed or from merchants)

| Tentative name | Season | Growth | Main uses | Unlock |
|---|---|---|---|---|
| **Wheat** | Summer / Autumn | Medium | Flour → bread; animal feed (silo) | T2 |
| **Hay grass** | Spring–Autumn | Fast | Animal feed (silo) | T2 |
| **Tomato Vine** | Summer | Medium | Orders, sauces; needs trellis | T2 |
| **Berry Spray** | Summer | Slow | Jam, pie, bee food | T2 |
| **Pumpkin Head** | Autumn | Slow | Pie, contests (biggest pumpkin) | T3 |
| **Herb Pot** | All (indoor / greenhouse) | Medium | Orders (Marlow), cooking | T2 |
| **Frost Leaf** | Winter | Medium | Winter orders | T3 |
| **Rain Drop** | Spring / Autumn | Medium | Sell, gift | T3 |

## 6. Orchard trees

Planted once, harvested every season for years (buildings.md *Orchard*). Trees are never lost to neglect once mature (realistic: established trees survive dry spells) but give no fruit that season if never watered.

| Tree | Fruit season | Uses |
|---|---|---|
| **Apple** | Autumn | Pies, jam, juice |
| **Pear** | Autumn | Jam, sell |
| **Cherry** | Summer | Jam, pie |
| **Plum** | Summer | Jam |

## 7. Rare crops (collection)

Golden pumpkin, giant variants, merchant-only seeds — see progression.md §6.2.

## 8. Crop uses

| Use | Description |
|---|---|
| **Orders** | Delivered to neighbours (main income). |
| **Sell** | Market stall, shipping box, farm shop (seasonal & demand pricing). |
| **Process** | Kitchen/mill: jam, bread, pie, sauce. |
| **Feed** | Hay and wheat stored in the silo for auto-feeders. |
| **Decoration / gift** | Flowers in borders; gifts to neighbours. |
| **Garden shelf** | Phase-001 display of harvests (becomes the inventory in later phases). |

---

## Related documents

| Document | Relationship |
|---|---|
| [README.md](../README.md) | Centralized hub for the project. |
| [GDD.md](./GDD.md) | Growth model, neglect rule, ways to earn. |
| [mechanics.md](./mechanics.md) | Plant, water, harvest, process verbs. |
| [buildings.md](./buildings.md) | Beds, greenhouse, hydroponics, orchard, sprinklers, silo. |
| [items.md](./items.md) | Harvested and processed goods. |
| [inventory.md](./inventory.md) | Harvested crop slots. |
| [challenge-layers.md](./challenge-layers.md) | Neglect grace times per mode. |
| [world.md](./world.md) | Butterfly Garden flowers are the first story task. |
| [events.md](./events.md) | Seasonal crop and contest events. |

## 9. Change log

- **0.1 — 2026-11-18:** Initial draft. Small seasonal crop list with no withering, no economy, and automatic shelf routing. Butterfly Garden flowers supplied by NPC task.
- **0.2 — 2026-11-18:** Added Carrot Patch to starting seeds; added Tomato Vine, Herb Pot. Clover Vine and Berry Spray noted as bee food. Winter indoor crops (Frost Leaf, Herb Pot) keep gardening alive in cold months.
- **0.3 — 2026-11-18:** Reconciled tables with change log 0.2 (Carrot Patch, Tomato Vine, Herb Pot now appear in the tables; bee-food notes added to Clover Vine and Berry Spray). Added the season-persistence rule: grown plants keep blooming after their season, so butterfly visits in spring and summer are consistent.
- **0.4 — 2026-11-18:** Growth model §2 updated to the Milestone 0 decision (GDD §4, v0.16): stage timing default **Medium ≈30–45 s** (Fast/Slow presets for the options screen), growth paused while the game is closed by default, optional positive-only Gentle catch-up (+1 stage max, never past bloom), both set before starting a new game. Replaced the old “closed-game growth is TBD” line.
- **0.5 — 2026-11-19:** **Direction reset (GDD v0.19).** No-withering rule replaced: crops **need water**, dry soil pauses growth, neglected crops **turn yellow then are gone** (grace per mode). Economy uses added (orders, selling, processing, feed). 2–3-bed limit is now only the starting garden. Added farm crops (wheat, hay grass), **orchard trees**, crop quality, greenhouse year-round growing, rare crops as a collection, and unlock tiers.
