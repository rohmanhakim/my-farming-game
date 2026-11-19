# Places

> **Status:** Brainstorming draft
> **Version:** 0.7
> **Last updated:** 2026-11-19
> **Purpose:** Places available in the game that the player can explore, adjusted to the scope of the game described in the GDD. One lane, eight places, no map — plus Kit's farm, which grows into the fields behind the house. All names are tentative and can be changed. Every element here is a proposal pending playtesting.

---

## 1. Scope and design intent

The lane is readable at a glance and small enough to navigate without a map. Places are gentle locks rather than open-world gates: the stile and Lantern Hill reopen through optional **lane restoration** (progression.md §6.4). The brook is an early exploration reward; the market stand becomes Kit's stall at farm tier T2. **The farm is the part that grows:** the fields behind the house are all Kit's from the start but overgrown, and are cleared at the player's pace (progression.md §4); the lane itself never expands.

---

## 2. Place table

| Place | What it is | Why it exists | Interactive objects | Unlock state | Time / season variants |
|---|---|---|---|---|---|
| **The player's home** | A **family heirloom** cottage with a brightly painted door and a wild front garden (starting plot, 2–3 beds) — **overgrown but never derelict**: loved, alive, waiting. | Tutorial space, creative-ownership space, the visual "nothing is here yet" hook. | Garden beds, decoration spots, sleep/rest point, seed box, storage chest, shipping box, kitchen (house upgrade); seed packet left on the gate (trace for Mr. Marlow); boxes and a small cart at the gate (the arrival). | Available from start. | Door/window light shows day vs night; garden looks different per season; Marlow's gate greeting in the morning. |
| **Bramblewick Lane** | The curved path itself, with hedgerows, dry-stone walls, a stile, and a gate. | The GDD's "small shared or public-feeling space"; a soft world border. | Path edges, hedgerows, gate; small footprints in the mud near the stile (trace for a friend of Kit's age). | Available from start. | Lighting changes day/night; ambient creatures appear by season; a distant wave from a future neighbour in the afternoon. |
| **Mr. Marlow's cottage + seed shed** | A leaning shed of jars, packets, labels, and a big hand-illustrated ledger. | Task hub, seed shop, **order board**. | Marlow, seed packets for sale, order board, ledger, gift drawer, building catalogue. | Available from start. | Marlow present at the gate in the morning and late afternoon; shed lit at night; kindly restates unfinished tasks if visited. |
| **The allotment beds** | A few shared beds tended by other (future) characters. | Future NPC home without expanding the map. | Shared planting beds; half-painted birdhouse leaning on a bed (trace for the allotment neighbour). | Available from start. | Seasonal planting visuals; the allotment neighbour tends the beds on sunny afternoons after introduction (T2). |
| **The brook and footbridge** | A small crossing with flat stones and reeds. | Early exploration reward; home for later requests. | Footbridge, reeds, frog, **fishing spots**, forage (berries, herbs); laundry on a line by the water (trace for the brook keeper). | Earned early through exploration (early exploration reward). | Frog visible by day; fireflies over water at night in season; the brook keeper appears at the water's edge on warm afternoons after introduction (T3); footbridge repair is a lane-restoration step. |
| **The market stand** | A roadside table where Kit sells produce (seasonal and demand pricing) and the **travelling merchant** sets up on visit days. | Alternative income to orders; Kit's own stall from T2. | Market table, price board, merchant's cart. | Visible from start; Kit's stall from T2 (buildings.md). | Busiest at Midday; items and prices change by season. |
| **The stile** | A wooden stile at the lane's upper end, leading toward Lantern Hill. | Gentle world gate; locked until later. | Stile latch. | Reopened through lane restoration (progression.md §6.4). | n/a |
| **Lantern Hill** | A hill at the lane's end with a broad oak and an old, unlit lantern post. | Long-arc payoff destination; matches the GDD's deferred "ladder to a lookout" reward. | Oak tree, unlit lantern post (trace for the seasonal visitor), lookout view. | Optional last step of lane restoration. | Day view vs night view; relit lantern at Winter Lantern Night; the Hill Visitor arrives (T5). |

---

### 2.1 Kit's farm (grows over time)

| Area | What it is | Unlock |
|---|---|---|
| **Front garden** | Starting plot, 8 × 6 = 48 tiles (3 beds clear) | Start |
| **Fields A–E behind the house** | Overgrown family fields (120–320 tiles each, buildings.md §2); hold buildings, beds, orchard, fish pond (buildings.md) | **Owned from the start**, overgrown; cleared at the player's pace (progression.md §4); all five cleared = T5 Full land |

## 3. Visual character (from world.md §3.3)

- **Hobbiton-adjacent, not Hobbiton:** Grass-grown roofs, bright doors, stone walls, picket fences, laundry lines, wheelbarrows, terracotta pots, hand-painted signs. Avoid hobbit proportions, pipe-weed, and direct copies.
- **Earthy base, bright accents:** Subdued greens, browns, and stonework; saturated flowers, doors, seed packets, and characters pop against it.
- **Everything is small and made by hand:** Nothing industrial, nothing looming. The largest object is the oak on Lantern Hill.

---

## Related documents

| Document | Relationship |
|---|---|
| [README.md](../README.md) | Centralized hub for the project. |
| [GDD.md](./GDD.md) | Scope and audience; places serve the gentle-lock world design. |
| [world.md](./world.md) | Premise and setting; the lane map and place purposes originate here (§3). All eight places are kept in sync between the two documents. |
| [characters.md](./characters.md) | Mr. Marlow lives at the cottage + seed shed; Kit and Ada live at the player's home. |
| [crops.md](./crops.md) | Garden beds at the player's home hold crops; seasonal planting visuals appear by place. |
| [animals.md](./animals.md) | Animals live at specific places on the lane (coop, beehive, etc.). |
| [events.md](./events.md) | Daily and seasonal events occur at places on the lane. |
| [buildings.md](./buildings.md) | What can be built on the farm's fields. |
| [progression.md](./progression.md) | Clearing land and lane restoration. |

## 4. Change log

- **0.1 — 2026-11-18:** Initial draft. Six places aligned with world.md, plus day/night and season variant notes. Stile and Lantern Hill marked as locked/deferred long-arc content.
- **0.2 — 2026-11-18:** Added the Market Stand as a parent-unlocked place for the Garden Coins challenge layer. Noted that new places are parent-unlocked.
- **0.3 — 2026-11-18:** Added trace objects (seed packet, footprints, birdhouse, laundry) and per-place presence schedules so each place feels inhabited before its character is formally introduced.
- **0.4 — 2026-11-18:** Recounted to **eight places** (stile and market stand were already listed here; world.md now matches). Brook unlock changed to "earned early through exploration" (early exploration reward). Added the unlit-lantern trace to Lantern Hill.
- **0.5 — 2026-11-18:** Player's home updated per world.md §7.5: **family heirloom** cottage, **overgrown-never-derelict** art rule (no decay, no rubbish, no loneliness), arrival props (boxes + cart at the gate) added as interactive/trace objects.
- **0.6 — 2026-11-19:** **Direction reset (GDD v0.19).** Parent-unlocked places removed. Added **§2.1 Kit's farm** (front garden + five fields behind the house, bought/cleared over time). Seed shed hosts the order board and building catalogue; market stand is the selling place and merchant spot, Kit's stall from T2; brook gains fishing and forage; stile and Lantern Hill reopen via lane restoration. Stage references replaced with farm tiers.
- **0.7 — 2026-11-19:** Back fields are owned from the start and cleared, not bought (owner decision); sizes in buildings.md §2.
