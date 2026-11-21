# Buildings & Farm Structures

> **Status:** Brainstorming draft
> **Version:** 0.11
> **Last updated:** 2026-11-21
> **Purpose:** Everything Kit can build on the farm — cost, footprint, capacity, when it unlocks, what it teaches, and how it fits the land. Sized against the farm map below so the land is a real, countable limit. Costs come from [economy.md](./economy.md) (≈ half the real Australian price). All numbers are proposals pending playtesting.

---

## 1. Design intent

- **The land is all Kit's from day one, but overgrown.** It must be cleared before it can be used (progression.md §4). Early on, **money** limits what Kit builds; later, **cleared land** and finally **total land** do.
- **Capacity comes from the building, not a global cap.** A barn with 5 stalls holds 5 cows; a bigger barn holds more but needs more land (owner decision 2026-11-19).
- **Each building teaches something real** (GDD pillar 3) and opens an activity or saves work.
- **No upkeep bills, no breakdowns.** Some buildings use materials the farm produces (feed, compost).
- Bought from **Marlow's building catalogue** (early buildings) or, from T4, from **the carpenter** in the village (greenhouse, hydroponics, farm shop, showcase, house upgrades) — or built from a **blueprint + materials** at the workshop.

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

**Layout (places.md §3, 2026-11-20):** the farm is **50 × 32 tiles**. Top row: Field D, the house with the front garden and old coop facing the gate, Field C. Bottom row: Field E along the brook (with the natural pond), Field A behind the house with the old barn at its back edge, Field B. A 2-tile cross path joins every field to the front door, and a 12 × 6 **animal yard** beside the old barn is clear from the start (it fits a second barn and a big coop side by side). Field sizes and places are fixed; what goes inside them is up to the player.

Paths between buildings are needed for walking; plan on roughly 15 % of the land for paths. **A fully built farm has room for about 850 usable tiles** — enough to feel big, small enough that choices matter (§5).

## 3. Catalogue

Cost tiers: see economy.md for exact numbers (coins). Footprint in tiles (width × height).

| Building | What it does | Cost (coins) | Footprint | Capacity | Unlock | What it teaches | Automation / land tie-in |
|---|---|---|---|---|---|---|---|
| **Tilled bed** | Plant one crop | free (hoe) | 1 × 1 | 1 crop | Start | Soil and seeds | Sprinklers water beds in range; crops are the biggest land user. |
| **Compost bin** | Scraps → compost (better crop quality) | 450 | 1 × 1 | — | T1 | Recycling, soil health | A helper can empty it. |
| **Small coop** | Houses chickens. **One old coop is on the farm from the start** (front garden, already cleared); extra coops cost | 2,000 | 4 × 3 = 12 | **5 hens** | T1 (after first order) | What hens need | Fits one auto-feeder. |
| **Big coop** (upgrade) | More hens | +2,500 | 6 × 4 = 24 | **10 hens** | T2 | Scaling up | — |
| **Beehive** | Honey; pollinates crops within 5 tiles | 1,750 | 1 × 1 | 1 colony | T2 | Pollination | Up to 8 hives = an apiary (8 tiles). No feeding; needs flowers nearby. |
| **Workshop** | Craft tools, fences, decorations, sprinklers, feeders | 30,000 | 4 × 3 = 12 | — | T2 | Making and repairing | Required to craft automation. |
| **Kitchen** (house upgrade) | Jam, bread, cheese, pie, smoked fish | 40,000 | 0 (in house) | — | T2 | Where food comes from | Processed goods earn more per tile. |
| **Shipping box** *(owner 2026-11-21)* | Sell anything: put goods in, take them out again until pickup; what is in it when Kit sleeps sells at the normal price | free — the family's old box, on the farm from the start *(proposed)* | 1 × 1 (beside the front door) | any amount | Start (built in Phase-002) | Choosing what to sell and what to keep | A farmhand can fill it. |
| **Market stall** — *deferred to a later phase (owner 2026-11-21)* | Set prices and leave it; passers-by buy while Kit does other things | 2,500 | 0 (on the green) | — | later phase | Pricing, demand | A helper can stock it. |
| **Small barn** | Houses cows and sheep. **One old barn is on the farm from the start** (edge of Field A, ground around it cleared); extra barns cost | 140,000 | 6 × 5 = 30 | **5 stalls** | T3 | Large animals' needs | Fits auto-feeders. |
| **Big barn** (upgrade) | More stalls | +100,000 | 8 × 6 = 48 | **10 stalls** | T3 | Scaling up | — |
| **Fish pond (water tiles)** | Stock, feed, harvest fish | **250 per tile** (wide water) | any shape, at least 2 tiles wide to hold fish | **≈ 2 fish per 3 tiles** (4 × 4 = 10, 6 × 6 = 24, 8 × 8 = 42) | T3 | Water ecosystems | Must connect to the farm's water (§3.1). Pond auto-feeder covers the whole connected pond. |
| **Water channel** | Joins a new pond to the farm's water | **50 per tile** | 1 tile wide | no fish | T3 | Irrigation | Must connect to the farm's water (§3.1). A plank bridge is placed free where it crosses a path. |
| **Fruit tree (orchard)** | Fruit every season for years | 200 per sapling | 2 × 2 = 4 per tree | 1 tree | T3 | Perennial vs annual crops | Sprinklers water trees. |
| **Storage silo** | Stores feed (hay, grain); phone-ordered feed and harvested hay/wheat go straight in | 30,000 | 3 × 3 = 9 | 240 portions (≈ 7 weeks for 5 cows) | T3 | Planning for winter | **Required for auto-feeders.** Low-feed warning icon. |
| **Sprinkler** (basic / quality / deluxe) | Waters beds every morning; hidden pipes included | 400 / 1,200 / 2,500 (or craft) | 1 × 1 | 8 / 24 / 48 tiles around it | T4 (one basic sprinkler earlier as a reward) | Irrigation | **No water source needed** — works anywhere; shows its coverage before placing. |
| **Auto-feeder** | Feeds a coop or barn daily from the silo | 400 (or craft) | 0 (inside) | 1 building | T4 | Saving work | Core automation. |
| **Pond auto-feeder** | Feeds a pond daily from its own hopper; phone-ordered fish food goes straight in | 750 (or craft) | 0 (on pond) | 1 pond, hopper holds 30 days | T4 | — | Core automation. |
| **Greenhouse** | Any crop, any season | 30,000 | 7 × 6 = 42 | **24 crop tiles inside** | T4 | Climate control | Built-in sprinkler upgrade (+3,500). |
| **Hydroponics bay** | Soil-free racks: grows 25 % faster, needs no watering, stacks 2 high | 10,000 | 4 × 3 = 12 | **24 crop slots** (12 tiles × 2 levels) | T5 | Water efficiency | Best food per tile when land is full. |
| **Farm shop** | Customers come to the farm; standing orders | 200,000 | 5 × 4 = 20 | — | T5 | Running a business | A helper can run it. |
| **Showcase** | Displays collections and trophies | 40,000 | 5 × 4 = 20 | — | T5 | — | progression.md §6. |
| **Decorations, paths, fences** | Beauty and layout | 60–1,000 | 0–1 | — | T1+ | Design | Raise farm charm; compete with crops for land. |

### 3.1 Water tiles: ponds and channels (owner decision 2026-11-20)

- **The source is the natural pond** in Field E, fed by the brook: free, **4 × 4 (10 fish)**, ready once its corner of Field E is cleared (T3).
- **Water is dug tile by tile** in build mode, on cleared land. Every new water tile must **touch existing water** — the natural pond or anything already joined to it. A preview line turns blue when a new pond is connected.
- **Touching water merges.** Two ponds joined by water become **one pond**: shared fish, one feed bin, one pond auto-feeder. So the natural pond can grow in place, and new ponds anywhere on the farm are fed by a channel.
- **Fish need room:** only water at least 2 tiles wide holds fish, at about 2 fish per 3 tiles. 1-tile-wide **channels** only carry water and are cheap.
- Water tiles use farm land like any building. They can be filled back in (sold back for half).

## 4. Example: a fully built farm fits the land

| Use | Tiles |
|---|---|
| Paths (~15 %) | 150 |
| Big coop × 2 (20 hens) | 48 |
| Big barn × 2 (20 large animals) | 96 |
| Fish pond, 6 × 6 water (24 fish) | 36 |
| Greenhouse | 42 |
| Hydroponics bay × 2 | 24 |
| Orchard, 12 trees | 48 |
| Workshop, silo, farm shop, showcase | 61 |
| Apiary (8 hives) | 8 |
| Decorations | ~40 |
| **Left for outdoor crop beds** | **~439** |
| **Total** | **992** |

If a player wants three barns, a second greenhouse, and a big orchard, something else has to give. That trade-off is the late game.

## 5. Placement rules

- Every building except the kitchen and market stall uses farm tiles (the shipping box takes 1 path tile by the door), and can only be placed on **cleared** land.
- **Feed bins:** every coop, barn, and pond has a built-in feed bin; home-ordered feed is delivered into it at Dawn (economy.md §2). Once a **silo** (coop/barn feed) or a **pond auto-feeder** (fish food) exists, orders go there instead and the auto-feeders draw from it, so Kit only tops up about once a month.
- **Starting buildings (owner decision 2026-11-19):** the old coop (with 5 hens) and old barn (with 1 cow) are free and usable as they are; they can be upgraded (big coop / big barn) or moved like any other building.
- Buildings can be **moved** for free and **sold back** for half what Kit paid.
- When land is full, growth means **better use of land**: hydroponics, greenhouse, orchards, processing, layout.

## 6. Open questions

1. Final prices and footprints after the first economy playtest (economy.md §6).
2. **Resolved (2026-11-20):** Field E's natural pond is a free 4 × 4 pond fed by the brook; it and any new pond grow tile by tile (§3.1).
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
- **0.3 — 2026-11-19:** Farm now **starts with an old empty coop and barn** (free, usable as is); Kit buys animals first and pays only for extra or bigger buildings.
- **0.4 — 2026-11-19:** Starting coop holds 5 hens and starting barn 1 cow.
- **0.5 — 2026-11-19:** Feed bins in coop/barn/pond receive home-ordered feed.
- **0.6 — 2026-11-20:** **Bramblewick Vale (places.md v0.10).** §2 records the farm layout (50 × 32, house centre-top, animal yard by the old barn). **Ponds are now water tiles (§3.1, owner decision):** the free natural 4 × 4 pond in Field E is the source; water is dug tile by tile (250 coins per wide tile, 50 per 1-wide channel), must touch existing water, and merges into one pond; ≈ 2 fish per 3 wide tiles; any water tile feeds sprinklers. Small/large fish pond rows replaced; open question 2 resolved. From T4 the village carpenter sells the late buildings; Marlow keeps the early catalogue.
- **0.7 — 2026-11-20:** Costs re-based on **Australian prices** (economy.md v0.9, owner decision): e.g. extra coop 2,000, beehive 1,750, workshop 30,000, kitchen 40,000, small barn 140,000, windmill 50,000, greenhouse 30,000, farmhand cottage 125,000, farm shop 200,000; sprinklers 125 / 400 / 900. Pond and channel tiles unchanged (250 / 50).
- **0.8 — 2026-11-20:** **Farmhands first, machines later (owner decision 2026-11-20):** **farmhand cottage removed** — farmhands are villagers hired by the day (GDD §4). §4 example: 16 more tiles for crops (~430).
- **0.9 — 2026-11-20:** **Machines simplified (owner decision 2026-11-20):** sprinklers need **no water source** (hidden pipes, priced in: 400 / 1,200 / 2,500); water tiles and the windmill no longer feed sprinklers (windmill mills flour and powers hydroponics). Silo and pond auto-feeder hopper are filled straight from phone orders and harvests; auto-feeders last weeks, with a low-feed warning.
- **0.10 — 2026-11-20:** **Windmill / water pump removed (owner decision 2026-11-20):** sprinklers no longer need it, flour comes from the restored village watermill (T4), and hydroponics needs no power. §4 example: 9 more crop tiles (~439).
- **0.11 — 2026-11-21:** **Selling (owner decision 2026-11-21):** added the **shipping box** (free family box by the front door, proposed; built in Phase-002); the **market stall is deferred** to a later phase and, if it returns, works as "set prices and leave it".
