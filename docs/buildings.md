# Buildings & Farm Structures

> **Status:** Brainstorming draft — new document
> **Version:** 0.1
> **Last updated:** 2026-11-19
> **Purpose:** Catalogue of everything Kit can build on the farm: cost, land footprint, unlock tier, what it teaches, and how it ties to automation and the land-size limit. All names, costs, and sizes are proposals pending playtesting. The GDD remains authoritative for gameplay.

---

## 1. Design intent

- **Buildings are the farm's investments.** Early on, money decides what Kit can build; later, **land** decides it — every structure has a footprint, and the farm's total land is the final limit (GDD pillar 4).
- **Each building teaches something real** (GDD pillar 3) and either opens a new activity or makes an old one easier.
- **Automation lives in buildings and fixtures** (GDD §4 *Automation*): sprinklers, feeders, pumps, and helpers are built or placed, so they compete for land like everything else.
- **No upkeep bills, no breakdowns** (no added complexity). Some buildings consume materials (feed, compost) that the farm itself produces.
- Buildings are bought from **Marlow's seed shed / the builder's catalogue** with money, or built from a **blueprint + materials** at the workshop.

## 2. Units

- **Cost tiers** (scaled ±25 % by difficulty mode, challenge-layers.md §3): 💰 cheap · 💰💰 moderate · 💰💰💰 expensive · 💰💰💰💰 major investment. Exact prices are set during economy balancing.
- **Footprint** is in **farm tiles**; one garden bed = 1 tile. The starting plot is about **12 tiles**; each land expansion adds a field (progression.md §4).
- **Unlock tier** refers to the farm growth tiers in progression.md §3: **T1 Small plot · T2 Growing farm · T3 Working farm · T4 Automated farm · T5 Full land**.

## 3. Catalogue

| Building | What it does | Cost | Footprint | Unlock | What it teaches | Automation & land tie-in |
|---|---|---|---|---|---|---|
| **Garden bed** | Plant one crop. | 💰 | 1 | T1 (start: 2–3 beds; more bought freely) | Soil, seeds, watering | Watered by sprinklers; the most land-hungry thing on a farm. |
| **Compost bin** | Turns crop scraps and old plants into compost (faster growth, better quality). | 💰 | 1 | T1 | Recycling, soil health, nothing is waste | Feeds sprinklered beds; a helper can empty it later. |
| **Chicken coop** | Houses chickens (eggs). | 💰💰 | 4 | T2 | What hens need: food, shelter, laying seasons | Accepts an auto-feeder; capacity upgrades cost money, not land. |
| **Beehive / apiary** | One hive = honey; an apiary (several hives) = more honey and pollination bonus to nearby crops. | 💰💰 (hive) · 💰💰💰 (apiary) | 1 per hive | T2 | Pollination, why bees matter to food | Flowers and orchards nearby raise honey quality; no feeding needed. |
| **Workshop / crafting station** | Crafts tools, fences, decorations, and **automation from blueprints**. | 💰💰 | 2 | T2 | Making and repairing instead of buying | Required to craft sprinklers, feeders, and helper posts. |
| **Kitchen (house upgrade)** | Processes produce into jam, bread, cheese, pies, smoked fish. | 💰💰 | 0 (inside the house) | T2 | Where food comes from: milk → cheese, wheat → bread | A helper can run batches later; processed goods are worth more per tile of land. |
| **Market stall** | Kit's own stall on the lane; sell to passers-by at market price. | 💰💰 | 0 (on the lane, not the farm) | T2 | Pricing, supply and demand | A delivery helper can stock it. |
| **Storage silo** | Stores animal feed grown on the farm (wheat/hay); feeders draw from it. | 💰💰 | 2 | T3 | Planning for winter; storing food | **Required for auto-feeders**; grow feed → silo → feeders. |
| **Barn** | Houses cows and sheep (milk, wool). | 💰💰💰 | 6 | T3 | Larger animals' needs: hay, space, grooming | Accepts auto-feeders and a brushing helper; capacity upgrades. |
| **Fish pond** | Stock, feed, and harvest fish (animals.md §6). | 💰💰💰 | 4 (bigger ponds 6/9) | T3 | Water ecosystems, feeding without overfeeding | **Pond auto-feeder** fixture; size grows with land. |
| **Orchard (fruit trees)** | Fruit trees: plant once, harvest every season for years. | 💰💰 per tree | 1 per tree (trees can't share tiles) | T3 | Perennial vs annual crops; patience pays | Sprinklers cover trees; fruit feeds jam/pie orders. |
| **Irrigation / sprinklers** | Waters every bed in range each morning. Basic (3×3), Quality (5×5), Deluxe (7×7). | 💰💰 → 💰💰💰 (or blueprint + materials) | 1 each | T4 (basic sprinkler can be earned early via a blueprint reward) | Irrigation, saving time and water | **Core automation.** Needs a water source in range (well, pond, or water pump). |
| **Windmill / water pump** | Pumps water for sprinklers and powers the mill (grain → flour) and hydroponics. | 💰💰💰 | 2 | T4 | Renewable energy, how water reaches fields | Extends sprinkler range across the farm; powers hydroponics. |
| **Greenhouse** | Grows any crop **in any season**; crops inside never need protecting from the season change. | 💰💰💰💰 | 9 (holds 6 beds) | T4 | Climate control, growing seasons | Has built-in sprinklers once upgraded; best value per tile for off-season orders. |
| **Hydroponics bay** | Soil-free racks inside the greenhouse or barn: **faster growth, uses less water**, stacked vertically. | 💰💰💰💰 | 2 (holds 6 crop slots stacked) | T5 | Water efficiency, modern farming | **Needs power (windmill) and an upgrade**; the answer when land runs out — more food per tile. |
| **Farm shop** | A shop on the farm itself: customers come to Kit; sells at a small premium and takes standing orders. | 💰💰💰💰 | 4 | T5 | Running a small business | A shopkeeper helper runs it; turns surplus into steady income. |
| **Helper post / farmhand cottage** | Hires or houses a **helper** (harvests, collects, delivers, brushes). | 💰💰💰 | 2 | T4 | Teamwork, delegating | **Core automation**; each helper covers an area, so placement matters. |
| **Decorations, paths, fences** | Beauty and layout (progression.md §6). | 💰 → 💰💰💰 | 0–1 | T1 onward | Design and planning | Raise **farm charm** (progression.md §6); paths can't hold crops, so charm competes with production for land. |

## 4. Land and placement rules

- Every building except the kitchen and market stall uses farm tiles; land is bought or cleared in fields (progression.md §4).
- Buildings can be **moved** for free (layout is creative ownership, GDD pillar 8) and **sold back** for half their price.
- When the land is full, the only ways to grow are **better use of land** (hydroponics, greenhouse, orchards, processing) — this is the late-game puzzle.

## 5. Open questions

1. Exact prices, footprints, and starting plot size — set during economy balancing.
2. Should hydroponics be one tier earlier for players who prefer modern farming?
3. Should buildings ever need repairs? (Current answer: no — no added complexity.)

## 6. Related documents

| Document | Relationship |
|---|---|
| [README.md](../README.md) | Centralized hub for the project. |
| [GDD.md](./GDD.md) | Pillars 3–5 (educational, grow your farm, automation). |
| [progression.md](./progression.md) | Farm growth tiers and land expansion that unlock these buildings. |
| [animals.md](./animals.md) | Coop, barn, apiary, and fish pond residents. |
| [crops.md](./crops.md) | Beds, orchard trees, greenhouse and hydroponics crops. |
| [mechanics.md](./mechanics.md) | Build, craft, process, and automation verbs. |
| [items.md](./items.md) | Blueprints, materials, automation items. |
| [places.md](./places.md) | Where the farm and the market stall sit on the lane. |
| [challenge-layers.md](./challenge-layers.md) | Mode-based price scaling. |

## 7. Change log

- **0.1 — 2026-11-19:** New document (owner request). Catalogue of 18 farm structures — bed, compost bin, coop, beehive/apiary, workshop, kitchen, market stall, silo, barn, fish pond, orchard, sprinklers, windmill/water pump, greenhouse, hydroponics, farm shop, helper post, decorations — each with cost tier, footprint, unlock tier, education angle, and automation/land tie-in. Placement rules (move free, sell back at half, land as the final limit).
