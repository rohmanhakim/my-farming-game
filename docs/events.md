# Events

> **Status:** Brainstorming draft — **updated for the direction reset 2026-11-19**
> **Version:** 0.8
> **Last updated:** 2026-11-19
> **Purpose:** Events during a playthrough, driven by calendar, season, day phase, and the farm's growth. All names tentative; every element is a proposal pending playtesting.

---

## 1. Scope and design intent

Events are invitations, not obligations. Missed events recur or are restated kindly. The only events with consequences are **neglect warnings and losses** (crops, animals, fish), which are always signalled first and never happen while the game is closed.

---

## 2. Core events

| Event | Trigger | What happens | Reward / outcome | Scope |
|---|---|---|---|---|
| **Butterfly Garden task** | Kit accepts Marlow's first request | Plant and water three task flowers | Butterfly net; unlocks find & catch | Prototype |
| **Task restatement** | Talk to Marlow with an unfinished task | Kind restatement; replacement seeds if the flowers were lost | Task stays available | Prototype |
| **Show butterfly to Marlow** | Kit has a caught butterfly | Marlow reacts; ledger page fills | Relationship warmth | Prototype |
| **Release butterfly** | Release in the garden | Butterfly flutters around | Visual joy | Prototype |
| **Order board opens** | After the Butterfly Garden task | Marlow introduces the board and the first small order | Start of the farm economy | Phase-002 |

## 3. Daily / ambient events

Time-of-day triggers are day phases (mechanics.md §2).

| Event | Trigger | What happens | Scope |
|---|---|---|---|
| **New orders** | Dawn | Board refreshes with orders matched to what Kit can produce | Phase-002 |
| **Sprinklers run** | Dawn | Sprinkled beds are watered (splash animation) | Phase-004 |
| **Shipping payout** | Sleep | Money for goods left in the shipping box | Phase-002 |
| **Watering feedback** | Water a bed | Soil darkens, sparkle and sound | Prototype |
| **Crop dry** | Soil dries | Cracked-soil look; growth pauses | Prototype |
| **Crop yellow (warning)** | Dry past grace (by mode) | Leaves droop and yellow | Prototype |
| **Crop lost** | Yellow past grace | Plant disappears; bed empty; small note "*Dry too long. Water every day, or try a sprinkler.*" | Prototype |
| **Butterfly visits** | Grown flowers, day, Spring/Summer | Butterflies appear | Prototype |
| **Fireflies** | Night, Summer | Fireflies over brook and garden | Later |
| **Animal needs care** | Meter reaches warning threshold | Need icon above the animal/pond | Phase-002 |
| **Product ready** | Egg, milk, wool, honey, fish ready | Glow / fluffy / drip / jumping fish | Phase-002 |
| **Animal lost** | Meter empty | Message card with gravestone picture (empty hive for bees), name, tip | Phase-002 |
| **Neighbour trace** | A character is about to be introduced | Picture clue at their place | Phase-002+ |

## 4. Character arrival events

| Arrival | Trigger | Introduces | Tier |
|---|---|---|---|
| **Mr. Marlow** | Day one at the seed shed gate | Mr. Marlow | T1 |
| **The allotment neighbour** | First crop sold or delivered | Allotment neighbour (first veg order) | T2 |
| **The baker** | First eggs collected | Baker (egg and flour orders) | T2 |
| **The brook keeper** | First visit to the brook | Brook keeper (gives fishing rod) | T3 |
| **A friend of Kit's age** | Two neighbour tasks + brook visit | Friend (friendship, festivals) | T3 |
| **Travelling merchant** | First land expansion | Merchant (trades, rarities) | T3 |
| **The Hill Visitor** | Lane restoration reaches Lantern Hill, winter | Hill Visitor | T5 |

Each arrival is preceded by a trace (laundry, birdhouse, footprints, seed packet, unlit lantern).

## 5. Seasonal events, festivals, and contests

| Event | Season | What happens | Scope |
|---|---|---|---|
| **Spring Bloom Festival** | Spring | Flower contest; prettiest-farm judging (farm charm) | Phase-005 |
| **Summer Fair** | Summer | Biggest fish, best jam contests | Phase-005 |
| **Harvest Festival** | Autumn | Biggest pumpkin, best eggs/wool/cheese; bulk catering orders | Phase-005 |
| **Winter Lantern Night** | Winter | Part of lane restoration: relighting the Lantern Hill lantern | Phase-005 |
| **Chicken winter feed** | Winter | Hens eat more, lay no eggs | Phase-002 |
| **Wool thickens** | Autumn | Brushing yields more/better wool | Phase-002 |
| **Beehive sleeps** | Winter | No honey; no loss while asleep | Phase-002 |
| **Merchant visit** | Set days | Merchant at the lane; trades change by season | Phase-004 |

Contests are optional; entries are judged on quality, never on reflexes.

## 6. Relationship and progression milestones

| Milestone | Trigger | What happens |
|---|---|---|
| **First order delivered** | First order | Neighbour thanks Kit; first sticker |
| **Repeat order** | Several orders for one neighbour | Neighbour offers a weekly order |
| **Bigger order** | Relationship grows | Bulk or catering order offered |
| **Recipe gift** | High relationship | Neighbour shares a recipe or blueprint |
| **Ledger page fills** | Completed tasks | Pressed flower or sketch in Marlow's ledger |
| **Tier reached** | Farm growth tier (progression.md §3) | Short celebration; new catalogue items |
| **Lane restoration step** | Community project finished | The lane visibly changes (footbridge, meadow, stile, lantern) |

---

## 7. Related documents

| Document | Relationship |
|---|---|
| [README.md](../README.md) | Centralized hub for the project. |
| [GDD.md](./GDD.md) | Orders, neglect, and ways to earn. |
| [world.md](./world.md) | Butterfly Garden and Lantern Hill. |
| [characters.md](./characters.md) | Characters and introductions. |
| [mechanics.md](./mechanics.md) | Verbs that trigger events. |
| [crops.md](./crops.md) | Crop neglect events. |
| [animals.md](./animals.md) | Care, product, and loss events. |
| [progression.md](./progression.md) | Tiers, contests, restoration. |
| [PHASE.md](./PHASE.md) | Phases named in the Scope column. |

## 8. Change log

- **0.1 — 2026-11-18:** Initial draft. Butterfly Garden core event, daily/ambient day/night events, seasonal events (later), and relationship milestones. No deadlines or penalties.
- **0.2 — 2026-11-18:** Added animal care events (needs care, product ready, naps) and seasonal animal events (chicken winter feed, sheep wool thickens, beehive sleeps). All events recur or are restated kindly — no penalties for missing them.
- **0.3 — 2026-11-18:** Added Two Modes principle and Stage-Gated Events for challenge layers (market day, crafting fair, festivals, Lantern Hill ceremony).
- **0.4 — 2026-11-18:** Added neighbour trace and distant wave ambient events, plus character arrival events for Mr. Marlow, the allotment neighbour, the brook keeper, a child of the lane, and the Hill Visitor — each gated to a stage with picture-only greetings.
- **0.5 — 2026-11-18:** Day/night and seasons confirmed always-on (Gentle Core): stage-gated festivals now activate at Stage 4 with all layers on instead of a Time Awareness layer. Trace list completed to all five traces (added seed packet, lantern). Care and product events include the cow and milk.
- **0.6 — 2026-11-18:** Milestone 0 decision 2 (in-game clock): time-of-day triggers in §3 now explicitly map to the eight named day phases in mechanics.md §2 (stepped 3-hour clock); missed-phase events still recur or restate kindly.
- **0.7 — 2026-11-18:** Ledger rows updated to the world.md §7.4 decision: ledger is **both** relationship UI and collectible, placed **world-only** at the seed-shed table, shipping in Phase-001 Task 1.6; the warmth-driven page rule stays a later event.
- **0.8 — 2026-11-19:** **Direction reset (GDD v0.19).** Gentle Core / stage-gated split removed; scope column now names phases. Added order, sprinkler, shipping-payout, crop dry/yellow/lost, and animal-lost (gravestone card) events; nap event removed. Arrivals tied to farm tiers, with the baker and travelling merchant added and "a child of the lane" reframed as a friend of Kit's age. Festivals and contests made core; Lantern Hill ceremony folded into lane restoration. New relationship milestones (repeat/bigger orders, recipe gifts, tier reached, restoration steps).
