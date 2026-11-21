# Events

> **Status:** Brainstorming draft — **updated for the direction reset 2026-11-19**
> **Version:** 0.12
> **Last updated:** 2026-11-21
> **Purpose:** Events during a playthrough, driven by calendar, season, day phase, and the farm's growth. All names tentative; every element is a proposal pending playtesting.

---

## 1. Scope and design intent

Events are invitations, not obligations. Missed events recur or are restated kindly. The only events with consequences are **neglect warnings and losses** (crops, animals, fish), which are always signalled first and never happen while the game is closed.

---

## 2. Core events

| Event | Trigger | What happens | Reward / outcome | Scope |
|---|---|---|---|---|
| **First order: wild berries** | Day 1, Kit meets Marlow | Forage 3 handfuls of berries on the lane | Coins; the order board opens | Phase-001 |
| **Second order: carrots** | After the first order | Grow and deliver 5 carrots | Coins | Phase-001 |
| **Butterfly Garden request** *(optional)* | Spring, after the first orders | Plant and water three task flowers | Butterfly net; unlocks find & catch | Phase-004 |
| **Task restatement** | Talk to Marlow with an unfinished task | Kind restatement; replacement seeds if the flowers were lost | Task stays available | Prototype |
| **Show butterfly to Marlow** | Kit has a caught butterfly | Marlow reacts; ledger page fills | Relationship warmth | Prototype |
| **Release butterfly** | Release in the garden | Butterfly flutters around | Visual joy | Prototype |

## 3. Daily / ambient events

Time-of-day triggers are day phases (mechanics.md §2).

| Event | Trigger | What happens | Scope |
|---|---|---|---|
| **New orders** | Dawn | Board refreshes with orders matched to what Kit can produce | Phase-001 |
| **Order signs** *(owner 2026-11-21)* | Dawn | Only **a few** open houses that want something (Marlow's cottage included) put a **basket with a picture note** at the door; Kit spots them while walking round | Phase-002 |
| **Rain** | Some mornings (≈ 1 in 5 in Spring/Autumn) | Rain waters every outdoor crop for the day | Phase-002 |
| **Sprinklers run** | Dawn | Sprinkled beds are watered (splash animation) | Phase-004 |
| **Shipping payout** | Sleep | Money for whatever is in the shipping box at that moment (items taken out before sleeping don't sell): it fills accepted orders first (sell price + freshness bonus), then the rest sells; any accepted order on its third night unsent is simply gone, with a kind line from the neighbour; a morning note shows which orders went out and what sold | Phase-002 |
| **Watering feedback** | Water a bed | Soil darkens, sparkle and sound | Prototype |
| **Crop dry** | Soil dries | Cracked-soil look; growth pauses | Prototype |
| **Crop yellow (warning)** | 2 dry days in a row | Leaves droop and yellow | Prototype |
| **Crop lost** | 3 dry days in a row | Plant disappears; bed empty; small note "*Dry too long. Water every day, or try a sprinkler.*" | Prototype |
| **Butterfly visits** | Grown flowers, day, Spring/Summer | Butterflies appear | Prototype |
| **Fireflies** | Night, Summer | Fireflies over brook and garden | Later |
| **Animal needs care** | 1–2 unfed days | Need icon above the animal/pond | Phase-001 (hens) |
| **Product ready** | Egg, milk, wool, honey, fish ready | Glow / fluffy / drip / jumping fish | Phase-001 (hens) |
| **Animal lost** | 3 unfed days in a row | Message card with gravestone picture (empty hive for bees), name, tip | Phase-001 (hens) |
| **Neighbour trace** | A character is about to be introduced | Picture clue at their place | Phase-002+ |

## 4. Character arrival events

| Arrival | Trigger | Introduces | Tier |
|---|---|---|---|
| **Mr. Marlow** | Day one at the seed shed gate | Mr. Marlow | T1 |
| **The allotment neighbour** | First crop order delivered | Allotment neighbour (first veg order); the allotments open (owner 2026-11-21) | T1 |
| **The baker** | First eggs collected | Baker (egg and flour orders); the bakery opens (owner 2026-11-21) | T1 |
| **The shopkeeper** | Five orders delivered (characters.md §10) | Shopkeeper (general store); the store opens (owner 2026-11-21) | T1 |
| **The brook keeper** | First visit to the far bank once the footbridge is repaired | Brook keeper (gives fishing rod) | T3 |
| **The smith** | First debris Kit's tools can't clear | Smith (tool upgrades) | T3 |
| **A friend of Kit's age** | Two neighbour tasks + brook visit | Friend (friendship, festivals) | T3 |
| **Travelling merchant** | Field A cleared | Merchant (trades, rarities) | T3 |
| **The carpenter** | First T4 building wanted | Carpenter (late buildings, house upgrades) | T4 |
| **The village nurse** | Clinic opens (T4) with a healthy-food order sign | Nurse (clinic; healthy-food orders) | T4 |
| **The librarian** | Ten forage or fish finds in the Collection Book | Librarian (picture books) | T4 |
| **The innkeeper** | Lane restoration reaches the stile | Innkeeper (catering orders) | T5 |
| **The Hill Visitor** | Lane restoration reaches Lantern Hill, winter | Hill Visitor | T5 |

Each arrival is preceded by a trace (characters.md §3). At most three new characters per tier; the merchant is a visitor and does not count. Where everyone lives: places.md §2.

## 5. Seasonal events, festivals, and contests

| Event | Season | What happens | Scope |
|---|---|---|---|
| **Spring Bloom Festival** | Spring | Flower contest on the green; prettiest-farm judging (farm charm) | Phase-005 |
| **Summer Fair** | Summer | Biggest fish, best jam contests, on the green and at the village hall | Phase-005 |
| **Harvest Festival** | Autumn | Biggest pumpkin, best eggs/wool/cheese in the village hall; bulk catering orders (inn) | Phase-005 |
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
| **Subscription offered** | Many happy deliveries to one neighbour | Neighbour offers a standing daily order (item, amount, time); Kit may subscribe |
| **Subscription reminder** | Each morning a subscription is due | HUD reminder (Orders list from Phase-002) until sent; missed day = unpaid only |
| **Bigger order** | Relationship grows | Bulk or catering order offered |
| **Recipe gift** | High relationship | Neighbour shares a recipe or blueprint |
| **Ledger page fills** | Completed tasks | Pressed flower or sketch in Marlow's ledger |
| **Tier reached** | Farm growth tier (progression.md §3) | Short celebration; new catalogue items |
| **Lane restoration step** | Community project finished (planned at the village hall) | The valley visibly changes and an area opens: footbridge → far bank (T3), watermill (T4), stile → Lantern Hill, relit lantern (T5) |
| **Area opens** | Fallen log cleared, or a restoration step | Short fade to the newly opened area; the pause-menu map clears its fog (places.md §5) |

---

## 7. Related documents

| Document | Relationship |
|---|---|
| [README.md](../README.md) | Centralized hub for the project. |
| [GDD.md](./GDD.md) | Orders, neglect, and ways to earn. |
| [world.md](./world.md) | First beat, Butterfly Garden, and Lantern Hill. |
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
- **0.9 — 2026-11-19:** Owner picks: first core events are now the **first orders** (berries, carrots, Phase-001); Butterfly Garden optional and later; crop yellow/lost triggers set to 2/3 dry days (single mode); merchant arrives when Field A is cleared.
- **0.10 — 2026-11-19:** Added subscription-offered and daily subscription-reminder events.
- **0.11 — 2026-11-20:** **Bramblewick Vale (places.md v0.10).** Arrivals added for the new villagers (shopkeeper T2, smith T3, carpenter/nurse/librarian T4, innkeeper T5); brook keeper now met on the far bank after the footbridge repair; festivals placed on the green, at the village hall, and at the inn; restoration steps now open areas; new "Area opens" event clears the pause-map fog.
- **0.12 — 2026-11-21:** **Selling and house-visit orders (owner decisions 2026-11-21):** new Dawn **order signs** event (only a few open houses each day); baker, allotment neighbour and shopkeeper arrive in T1 and their places open then (shopkeeper trigger synced to five delivered orders, characters.md); shipping payout sells only what is in the box at sleep, with a morning note.
