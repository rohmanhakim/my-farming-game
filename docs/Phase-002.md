# Phase-002 — Economy Depth (Box, House Orders, Real Rooms)

> **Status:** **Approved by the owner 2026-11-21** — Task 2.1 done; Task 2.2 next
> **Version:** 0.4
> **Last updated:** 2026-11-21
> **Milestone:** 2 — Economy depth
> **Purpose:** Break Milestone 2 down into session tasks. Each task produces one scoped, playtested artifact that can be created and validated in a single session with AI assistance. Same task shape and rules as Phase-001 (AGENTS.md §3–§4).

## 1. Milestone Overview

**Goal:** turn the slice's board-and-hand-in money loop into the real one: Kit **sells through the shipping box** (picked up at 18:00), **takes orders at neighbours' doors**, sends them through the box for a **freshness bonus**, keeps spare goods in a **storage chest**, and starts **growing the farm** by clearing the back fields. Rain and the first new animal add variety; the house, coop, barn and shops become **real rooms**.

**Decisions this plan builds on (all already in the docs or given by the owner):**

| Topic | Decision | Source |
|---|---|---|
| Shipping box | One sell outlet; items can be taken back out until pickup; normal sell price with seasonal and demand rules | GDD §4, economy.md §2 |
| **Pickup time** | **A fixed 18:00 every day** (owner 2026-11-21, replaces "when Kit sleeps") | owner, this plan |
| House-visit orders | Every order comes from a **sign at an open house's door**, Marlow's included; Kit knocks, the neighbour comes out, Kit takes the order and sends it through the box; only a few houses have a sign each day; some villagers never order | GDD §4, places.md §2.1 |
| Order pay | Sell price + **freshness bonus** +30 % / +20 % / +10 % by the pickup it is sent in (counted from the day it was taken); after the third pickup the order is simply gone (no cost); mixed baskets ×1.1; daily standing orders never expire | GDD Q15, economy.md §2 |
| Neighbours' places | Bakery (after Kit's first eggs), allotments (after the first crop order), general store (after five delivered orders) open in **T1** | progression.md §5, places.md §2.1 |
| Finding orders | **Players spot the door signs themselves**; the noticeboard gives no order hints | owner, places.md |
| Marlow's board | Becomes the **village noticeboard** (help wanted, festivals, projects — no orders) | GDD §4 |
| Orders list | On-screen **Orders button** next to the bag; gamepad **Back/Select** (owner 2026-11-21) | controls.md §2 |
| **Indoors** | **Real rooms you walk into** (owner 2026-11-21; closes GDD open question 16): house, coop, barn, seed shed and the neighbours' places | owner, this plan |
| Bedtime | 21:30 forced bedtime, 04:30 wake-up — unchanged | GDD §4 |
| Rain | ≈ 1 morning in 5 in Spring/Autumn; rain waters every outdoor crop for the day | events.md, GDD §4 |
| Clearing the back fields | Weeds, stones, stumps, logs with hands, scythe, hammer, axe; materials go to the bag | mechanics.md, buildings.md §2, items.md |
| Feeding | Trough with one slot per animal (owner 2026-11-21, built in Phase-001 Task 1.13 fix) | animals.md §3 |

**Scene:** Kit's farm (house, coop, barn, front garden, **Field A**), the lane, **Bramblewick Green's first ring** (Marlow's cottage and seed shed, the noticeboard, bakery, allotments, general store), and the rooms inside them.

**Loop in this milestone:**
1. Kit grows, forages and collects as before; spare goods go into the **shipping box** by the front door and sell at 18:00 (a morning note lists what sold).
2. Neighbours put a **sign at their door**; Kit visits, takes the order, and puts the goods in the box — the 18:00 pickup fills accepted orders first (with the freshness bonus) and sells the rest.
3. The **Orders button** shows what Kit has taken, what is still needed, tonight's bonus and nights left.
4. Kit keeps goods in the **storage chest** instead of selling.
5. Kit buys a **hammer, axe and scythe** and clears **Field A** to farm more land.
6. Some mornings it **rains** and the crops are watered.
7. A **new animal** (sheep or beehive — owner pick) adds a product.
8. **Stickers** mark first moments in a sticker book.

**Explicitly OUT of scope (deferred, not cut):** Kit's market stall (phase not set); barn upgrades, fish pond, kitchen processing, workshop, blueprints, more neighbours beyond the T1 three, co-op split screen → Phase-003; automation, helpers, brook fishing, merchant → Phase-004; festivals, charm, collections → Phase-005; Hazel Wood, Brookside, Lantern Hill areas (they open at later tiers); real 3D art (placeholders keep working; real models drop in by name any time).

## 2. Session Tasks

Tasks are ordered by dependency. After each task the game runs and the new artifact is playtestable. Every task keeps all controls gamepad-complete on any pad, stays inside the performance budget (project-structure.md §16; `tools/perf_check.sh`), registers new state with `SaveGame.Register` (bump `schemaVersion` with a migration), and fills its **Notes & learnings** before the next task starts.

---

### Task 2.1 — Close Phase-001 & Sync the Docs

**Artifact:** Phase-001 retrospective and doc-sync sweep (AGENTS.md §7), with the new owner decisions written back.

**Scope:**
- Retrospective of Phase-001 Notes; exit-criteria checkboxes made truthful; Phase-001 status changed from *In progress* to its completion state (human playtests still listed as pending if not run)
- Write back owner decisions: shipping-box pickup **18:00** (GDD §4, economy.md §2, PHASE.md row), **real rooms** (GDD §4 and open question 16), trough slots (already in animals.md)
- Decide the Phase-001 design guesses listed in Task 1.13 *Advice* with the owner (any tool harvests; ready crops never dry; phone packs; seed choice by packet; first-evening hint) and record them
- Fix doc inconsistencies: progression.md §3 "Where the valley opens" still puts the bakery and general store at **T2**, while the owner decision of 2026-11-21 opens them in **T1**
- README, PHASE.md and project-structure.md brought in line

**Acceptance criteria:**
- No `TBD` left in finished Phase-001 work; every known discrepancy fixed or written down
- The owner confirms Phase-002 can start

**Team Preparation:**
- Owner: answers to the open design guesses; the Phase-001 playtest (or an explicit waiver)

**Tests:**
- **Software:** link check of `docs/`; `tools/verify.sh` still green
- **Human:** None required

**AI assistance:** Prompt for "doc-sync sweep checklist across a game design document set after a milestone."

**Dependencies:** Phase-001 Tasks 1.1–1.20.

**Notes & learnings:**
- **Outcome / status:** 2026-11-21 — **Done** (Human: none required). Docs only; `tools/verify.sh` unchanged and green (768/768 from the last code change, no code touched here). Phase-001 closed for Phase-002 with the child playtest and the Windows frame-rate check still open (owner moved on after playing the 3D build themselves and getting the trough/action fix).
- **What worked well:** Reading the project chat's earlier answers before asking: the owner had already approved most Phase-001 guesses, so only the doc write-back was needed.
- **Problems & fixes:** (1) **Re-asking settled questions annoys the owner** — check the docs *and* the project chat first; the Phase-001 guesses (any tool harvests, ripe crops never dry, phone packs, seed choice by packet) had been accepted earlier. (2) progression.md §3 "Where the valley opens" still put the bakery and general store at T2 although the owner opened them in T1 on 2026-11-21 → fixed. (3) project-structure.md §4 still described the 2D tree → rewritten for 3D.
- **Deviations from scope or plan:** The child playtest was not run before closing Phase-001 (owner's choice); its Task 1.13 status stays pending. Owner decisions written back: shipping box at 18:00 and real rooms (GDD v0.48, economy, mechanics, places, PHASE), trough slots (animals, items, buildings, onboarding), any tool harvests + ripe crops never dry (crops.md v0.12), phone fixed packs and seed packets kept as built, evening hint stays at 16:30, `open_orders` on Back/Select (controls, project-structure), sheep for Task 2.10, rooms entered by walking into the doorway.
- **Handoff to Task 2.2:** Start from `Main.cs`'s `ApplyLevelMarkers` (spawn + camera limits) and implement `load_level` there; level-local state that must survive swaps today lives in level nodes (`FarmGrid` tiles, `AnimalHouse` animals/feed, `BerryBush` picks) and is saved by `SaveBinder` — move it to autoload-backed state or per-level save data before freeing a level. Buildings' indoor fade (`IndoorZone`, `Building.SetKitInside`) is replaced by door props + room levels; keep `OccluderFade` behaviour (`Building._Process`). Schema is v2; Task 2.2 adds v3 (current level + position).
---

### Task 2.2 — Real Rooms (Interior Scenes)

**Artifact:** The house, coop, barn and seed shed are rooms Kit walks into through their door; the outside keeps the buildings as solid shells.

**Scope:**
- Implement the `load_level` contract (project-structure.md §4) for the first time: defer → free the current level → instance the new one → place Kit at a named door marker → camera limits → `LevelChanged`
- Interior levels: `HouseInterior`, `CoopInterior`, `BarnInterior`, `ShedInterior` (bed and phone; trough, bin and hens; trough, bin and cow; ledger and counter), each with door markers back to the farm
- A door prop on each building (A / Space / click at the door, or walking into the doorway) with a short fade (`Transition` layer); the clock stops in every room (GDD §4 "time stops indoors")
- Outside, buildings no longer fade their roof when entered; the **occluder fade** stays so Kit is never hidden behind a building
- Room camera: same fixed diagonal camera, closer framing so the room fills the screen; the corner switch still works
- Save: which level Kit is in and where (schema v3 + migration: an old save puts Kit at the house door outside)
- Systems that live in a level (garden, animal houses, bushes) keep their state across level swaps (state moves to the autoloads or to a per-level save section, so leaving the coop never resets the hens)

**Acceptance criteria:**
- Kit can enter and leave each building with gamepad, keyboard and mouse; sleeping in the house works; the 21:30 bedtime brings Kit to bed from anywhere
- Leaving and re-entering a room keeps everything as it was; save/load inside a room works
- Frame budget held in rooms and outside

**Team Preparation:**
- Owner decided: rooms are entered by walking into the doorway

**Tests:**
- **Software:** port the indoor, sleep, feeding and shop in-game checks to rooms; level-swap checks (state kept, one `LevelChanged`, Kit at the right marker, camera snapped); save/load in each room; v2 → v3 migration
- **Human:** going in and out feels quick and clear; a 6-year-old knows how to leave a room

**AI assistance:** Prompt for "Godot 4.x C# level swapping with a persistent player and camera, door markers, a fade transition and per-level state kept in autoloads."

**Dependencies:** Task 2.1.

**Notes & learnings:**
- **Outcome / status:** _TBD_
- **What worked well:** _TBD_
- **Problems & fixes:** _TBD_
- **Deviations from scope or plan:** _TBD_
- **Handoff to Task 2.3:** _TBD_

---

### Task 2.3 — Bramblewick Green, First Ring

**Artifact:** The lane opens onto the village green: Marlow's cottage and seed shed, the noticeboard, the bakery, the allotments and the general store, with closed places marked kindly.

**Scope:**
- Village level reached at the top of the lane (area transition through `load_level`), laid out from places.md §2.1 (one ring path, every door on it) at 1 tile = 1 m; placeholder buildings and props by model name
- Marlow's cottage and seed shed move here from the farm level; the seed shed interior from Task 2.2 is reached from the village
- Bakery, allotments and general store exteriors: **closed** (a picture of what opens them, places.md rule "gentle locks") until their neighbour arrives at the existing triggers
- Neighbours as NPCs at their places once open (the baker, the allotment neighbour, the shopkeeper — placeholder capsules in clearly different colours and heights)
- Walking time farm door → green ≈ the places.md figure; camera limits per area

**Acceptance criteria:**
- Kit walks farm → lane → green and back; every open door is on the ring; closed places show what opens them
- Existing Marlow, shop, ledger and phone flows still work

**Team Preparation:**
- None (placeholders); real models may drop in by name any time

**Tests:**
- **Software:** area transition checks (spawn markers, state kept); walk routes; port the lane / Marlow / shop checks to the new layout; budget in the busiest village view
- **Human:** a 6-year-old finds the bakery and back home without help

**AI assistance:** Prompt for "Godot 4.x C# area transitions between a farm and a village level with spawn markers."

**Dependencies:** Task 2.2.

**Notes & learnings:**
- **Outcome / status:** _TBD_
- **What worked well:** _TBD_
- **Problems & fixes:** _TBD_
- **Deviations from scope or plan:** _TBD_
- **Handoff to Task 2.4:** _TBD_

---

### Task 2.4 — Shipping Box & Pricing

**Artifact:** A shipping box beside the farmhouse door that sells what is in it at 18:00, with seasonal and demand prices and a morning note.

**Scope:**
- `ShippingBox` prop (places.md §3) and a box screen that moves items between the bag and the box both ways (gamepad-complete; same focus and B-to-close rules as the bag)
- **Pickup at 18:00** every day (owner): sells exactly what is in the box then at the `PriceTable` sell price × quality, ×1.5 out of season, −2 % per item beyond 20 of the same per day (to at most −30 %, recovering overnight; economy.md §2); coins arrive at pickup; items put in after 18:00 wait for the next day's pickup
- Morning note (picture list of what sold and the coins), shown on waking or on the first look at the box
- Save section `shipping` (box contents, today's sold counts) — schema bump + migration
- The Phase-001 hand-in at Marlow keeps working until Task 2.6 switches orders over

**Acceptance criteria:**
- Items taken back out before 18:00 don't sell; pickup pays once; save/load mid-day keeps the box
- Prices match economy.md for quality, season and demand

**Team Preparation:**
- None

**Tests:**
- **Software:** pickup at 18:00 exactly once (also when Kit is indoors or asleep at that moment); take-out before pickup; demand curve over 25 items; season multiplier; save/load mid-day; box screen with pad / keys / mouse
- **Human:** a 6-year-old understands "put it in the box, coins come at 6 pm"

**AI assistance:** Prompt for "Godot 4.x C# shipping box with a daily pickup at a fixed in-game time, demand pricing and a two-way item transfer screen with gamepad focus."

**Dependencies:** Task 2.3.

**Notes & learnings:**
- **Outcome / status:** _TBD_
- **What worked well:** _TBD_
- **Problems & fixes:** _TBD_
- **Deviations from scope or plan:** _TBD_
- **Handoff to Task 2.5:** _TBD_

---

### Task 2.5 — Storage Chest

**Artifact:** A storage chest in the house where Kit keeps goods instead of selling them.

**Scope:**
- `StorageChest` prop in the house room; a chest screen like the box screen (inventory.md §4)
- Never full in Phase-002 (inventory.md: nothing is ever lost because space ran out); goods never spoil
- Save section `storage`

**Acceptance criteria:**
- Items move both ways; the chest survives save/load and room swaps

**Team Preparation:**
- None

**Tests:**
- **Software:** transfers, stacks by quality, save/load, screen controls on every input
- **Human:** None required (covered by the 2.4 box playtest)

**AI assistance:** Prompt for "reusable two-panel item transfer screen in Godot 4.x C# for a chest and a shipping box."

**Dependencies:** Task 2.4.

**Notes & learnings:**
- **Outcome / status:** _TBD_
- **What worked well:** _TBD_
- **Problems & fixes:** _TBD_
- **Deviations from scope or plan:** _TBD_
- **Handoff to Task 2.6:** _TBD_

---

### Task 2.6 — House-Visit Orders & the Freshness Bonus

**Artifact:** Orders come from signs at open houses' doors; Kit takes them from the neighbour and sends them through the box for +30 / +20 / +10 %.

**Scope:**
- `Neighbours` gains "place open"; places open at the existing triggers (first eggs → bakery; first crop order → allotments; five delivered orders → general store); Marlow's cottage is open from Day 1
- Each Dawn only **a few** open houses get an order sign (a basket with a picture note on the doorstep); some villagers never order; **no hints elsewhere** (owner: players spot the signs)
- Knock (A / click at the door) → the neighbour steps out → picture bubble with the order card → take / not now
- **Sent through the box:** at the 18:00 pickup the box fills accepted orders first (oldest first; only orders it can fill completely, so a basket needs every line), pays them at sell price + freshness bonus (+30 % in the pickup on the day the order was taken, +20 % the next, +10 % the one after), then sells the rest; an accepted order still unsent after its third pickup is removed with a kind line, no other effect; daily standing orders never expire and are filled from the box each day
- Box screen marks which accepted orders its contents will fill
- **Marlow's board becomes the village noticeboard** (help wanted, festivals, projects — no orders); board orders accepted in an old save stay valid (migration)
- Guided-start and nudge text updated (onboarding.md steps 7–8: Marlow's carrot order from his door sign; show the box)

**Acceptance criteria:**
- Signs only at open houses; taking, sending, the bonus by pickup day and expiry all follow economy.md §2
- Nothing is ever lost besides the missed money (PHASE.md §7 guardrail)

**Team Preparation:**
- None (neighbour portraits stay placeholders)

**Tests:**
- **Software:** sign generation over 1000 seeds (only open houses, only producible items, a few per day); take → send in the same pickup = +30 %, next = +20 %, then +10 %, then gone with no other effect; baskets need all lines; box fills orders before selling; standing orders daily; old board orders survive the migration; the board shows no orders
- **Human:** a 6-year-old spots a sign, takes the order and understands the bonus without reading

**AI assistance:** Prompt for "order generation at NPC doors with an expiring freshness bonus resolved at a daily pickup, Godot 4.x C#."

**Dependencies:** Tasks 2.3, 2.4.

**Notes & learnings:**
- **Outcome / status:** _TBD_
- **What worked well:** _TBD_
- **Problems & fixes:** _TBD_
- **Deviations from scope or plan:** _TBD_
- **Handoff to Task 2.7:** _TBD_

---

### Task 2.7 — Orders List

**Artifact:** An on-screen Orders button that lists every taken order.

**Scope:**
- HUD Orders button next to the bag; `open_orders` action (Back/Select proposed — any pad) plus the on-screen button for the mouse (controls.md §2, §4)
- Picture cards per taken order: the neighbour's face, have / need counts from bag + box + chest, tonight's bonus, nights-left dots, cancel
- Holds the clock like the bag; replaces the standing-order HUD chip

**Acceptance criteria:**
- Opens and closes with pad, keyboard and mouse; counts update live; cancelling works

**Team Preparation:**
- Owner decided: `open_orders` on Back/Select

**Tests:**
- **Software:** open/close on every input, counts from all three stores, cancel, clock held
- **Human:** the cards are readable at a glance

**AI assistance:** Prompt for "gamepad-navigable card list UI in Godot 4.x C# with live counts."

**Dependencies:** Task 2.6.

**Notes & learnings:**
- **Outcome / status:** _TBD_
- **What worked well:** _TBD_
- **Problems & fixes:** _TBD_
- **Deviations from scope or plan:** _TBD_
- **Handoff to Task 2.8:** _TBD_

---

### Task 2.8 — Rain

**Artifact:** Some mornings it rains and every outdoor crop is watered for the day.

**Scope:**
- Weather rolled at Dawn (≈ 1 in 5 in Spring/Autumn, events.md); shown on the clock dial and on waking
- Rain waters every outdoor crop for that day (GDD §4); greenhouse crops later are not affected
- Rain look: falling streaks (one particle system following the camera), darker sky, puddle tint on paths; never stops play; optional soft sound later
- Save: today's weather

**Acceptance criteria:**
- On a rainy day no watering is needed and crops grow that night; frame budget held while raining

**Team Preparation:**
- None

**Tests:**
- **Software:** forced rain waters every tile once; dry days unaffected; weather saved; rate ≈ 20 % over many days; budget with rain on
- **Human:** a child understands the crops were watered by the rain

**AI assistance:** Prompt for "lightweight rain effect around a fixed top-down camera in Godot 4.x Compatibility renderer."

**Dependencies:** Task 2.1.

**Notes & learnings:**
- **Outcome / status:** _TBD_
- **What worked well:** _TBD_
- **Problems & fixes:** _TBD_
- **Deviations from scope or plan:** _TBD_
- **Handoff to Task 2.9:** _TBD_

---

### Task 2.9 — Clearing Field A

**Artifact:** Kit buys a hammer, an axe and a scythe and clears Field A (behind the house) to farm more land.

**Scope:**
- Field A (12 × 10, weeds and small stones — buildings.md §2) becomes clearable tiles: weeds (hands / scythe), stones (hammer), stumps and logs (axe) as placeholder models; materials (wood, stone, fibre) go to the bag
- Tools sold at the seed shed (items.md: hammer, axe, scythe — "T2"; offered here as soon as Field A opens; prices from economy.csv)
- Cleared tiles become farmable like the front garden (the garden grid generalised to several fields, one MultiMesh per model across all fields)
- The other fields stay overgrown (cleared in later phases); the overgrown tufts disappear tile by tile as Field A is cleared
- Tool belt and Kit's held tool include the new tools; save: field tiles

**Acceptance criteria:**
- Every Field A tile can be cleared with the right tool and then tilled, planted and watered; wrong tool shows the tool needed
- Frame budget held with Field A fully planted

**Team Preparation:**
- None (placeholders)

**Tests:**
- **Software:** clear rules per obstacle and tool; materials added; cleared tiles farm like the garden; save/load of a partly cleared field; budget with the whole field planted
- **Human:** clearing feels satisfying, not a chore; the wrong-tool hint is understood

**AI assistance:** Prompt for "multi-field farm grid with obstacle clearing and MultiMesh drawing in Godot 4.x C#."

**Dependencies:** Task 2.1.

**Notes & learnings:**
- **Outcome / status:** _TBD_
- **What worked well:** _TBD_
- **Problems & fixes:** _TBD_
- **Deviations from scope or plan:** _TBD_
- **Handoff to Task 2.10:** _TBD_

---

### Task 2.10 — A New Animal (Sheep or Beehive)

**Artifact:** The first new species with its own care and product.

**Scope (owner picked sheep, 2026-11-21):**
- **Sheep**: bought at the seed shed, lives in the old barn's free stalls (trough slot, hay), brushed when fluffy for better wool; wool sells and fills orders (animals.md §2)
- ~~Beehive~~ (not chosen; stays for a later phase): a hive placed near flowers (Sun Sprouts), no feeding; honey; leaves after 3 days without flowers in a warm season (empty-hive picture, animals.md §3)
- Same loss rule and kind cards as hens and the cow; orders include the new product once it can be made

**Acceptance criteria:**
- Buy, care, product, loss rule and orders work like the existing animals

**Team Preparation:**
- Owner decided: sheep

**Tests:**
- **Software:** the chosen animal's rules (product timing, brushing or flower check, loss), shop purchase, orders, save/load
- **Human:** the new animal's need is understood without reading

**AI assistance:** Prompt for "adding a new farm animal type reusing an existing animal-house system in Godot 4.x C#."

**Dependencies:** Task 2.6.

**Notes & learnings:**
- **Outcome / status:** _TBD_
- **What worked well:** _TBD_
- **Problems & fixes:** _TBD_
- **Deviations from scope or plan:** _TBD_
- **Handoff to Task 2.11:** _TBD_

---

### Task 2.11 — Sticker Book

**Artifact:** A picture sticker book in the pause menu that fills with first moments.

**Scope:**
- Sticker book page in the pause menu (progression.md §2), cosmetic only — nothing locked behind it
- Phase-002 stickers: First Hello, First Harvest, First Order, New Friend, Green Thumb, Egg Collector (others appear as greyed silhouettes)
- A short sticker pop when one is earned (events.md "first order delivered → first sticker")
- Save section `stickers`

**Acceptance criteria:**
- Each listed sticker is earned once by its action and shown in the book; the book works with pad, keys and mouse

**Team Preparation:**
- None (placeholder sticker pictures)

**Tests:**
- **Software:** each sticker's trigger once; persistence; book navigation on every input
- **Human:** children notice and like the stickers

**AI assistance:** Prompt for "achievement sticker book UI with gamepad navigation in Godot 4.x C#."

**Dependencies:** Task 2.6.

**Notes & learnings:**
- **Outcome / status:** _TBD_
- **What worked well:** _TBD_
- **Problems & fixes:** _TBD_
- **Deviations from scope or plan:** _TBD_
- **Handoff to Task 2.12:** _TBD_

---

### Task 2.12 — Smoothness & HUD Budget

**Artifact:** No first-sight stutter and a cheaper HUD (Phase-001 Task 1.20 follow-ups).

**Scope:**
- Shader warm-up behind the start fade (every placeholder and model material drawn once), or Godot's shader baking for exports
- HUD draw calls down from ≈ 84 (one icon atlas, fewer nested containers, the clock dial drawn in one pass)
- Re-measure with `tools/perf_check.sh` in the farm, the village and a room; update project-structure.md §16

**Acceptance criteria:**
- No frame above 50 ms after the start fade; HUD under the handheld line with room to spare

**Team Preparation:**
- None

**Tests:**
- **Software:** perf check both profiles in three views; no visual change to the HUD (screenshot compare)
- **Human:** None required

**AI assistance:** Prompt for "shader pre-compilation and 2D UI draw-call reduction in Godot 4.x Compatibility renderer."

**Dependencies:** Tasks 2.3, 2.6.

**Notes & learnings:**
- **Outcome / status:** _TBD_
- **What worked well:** _TBD_
- **Problems & fixes:** _TBD_
- **Deviations from scope or plan:** _TBD_
- **Handoff to Task 2.13:** _TBD_

---

### Task 2.13 — Playtest & Iterate

**Artifact:** Validated Milestone 2 ready for the exit review.

**Scope:**
- Playtest with players aged 6+ (and adults), with supervision and consent for under-18s (GDD §14), using an updated `docs/playtest-002.md`
- Iterate on clarity, timing and prices (box, bonus, signs, clearing)

**Acceptance criteria:**
- A new player sells through the box, finds and fills a door order, uses the Orders list and clears part of Field A without adult help
- All tasks 2.1–2.12 complete and integrated

**Team Preparation:**
- Participants, observation sheet, test PC with gamepad

**Tests:**
- **Software:** all automated checks in one pass; `tools/perf_check.sh`; save/load end to end, including an old Phase-001 save
- **Human:** the GDD §14 questions plus the box / sign / bonus questions

**AI assistance:** Prompt for "playtest observation sheet for selling, orders and land clearing in a kid-friendly farming game."

**Dependencies:** All previous tasks.

**Notes & learnings:**
- **Outcome / status:** _TBD_
- **What worked well:** _TBD_
- **Problems & fixes:** _TBD_
- **Deviations from scope or plan:** _TBD_
- **Playtest findings:** _TBD_
- **Advice for Phase-003 planning:** _TBD_

---

## 3. Exit Criteria for Milestone 2

- [ ] All 13 session tasks are done
- [ ] Selling works only through the shipping box, picked up at 18:00, with season and demand prices
- [ ] Every order comes from a door sign; orders sent through the box earn the freshness bonus and expire kindly
- [ ] The house, coop, barn, seed shed and the neighbours' places are real rooms; state survives every room and area change
- [ ] Field A can be cleared and farmed; rain waters crops
- [ ] The new animal works with the same care and loss rules
- [ ] Old saves (Phase-001) load
- [ ] Performance budget met in the farm, the village and a room (project-structure.md §16)

## 4. Open questions for the owner (do not block planning)

1. ~~Sheep or beehive~~ — **decided 2026-11-21 (owner): sheep.**
2. ~~Entering rooms~~ — **decided 2026-11-21 (owner): just walk into the doorway.**
3. ~~`open_orders` on Back/Select~~ — **decided 2026-11-21 (owner): yes.**

## 5. Change log

- **0.1 — 2026-11-21:** First draft for owner approval (owner asked to start Phase 2). Built from PHASE.md §3, the Phase-001 §4.1 carry-forward list and the owner's answers of 2026-11-21: shipping box picked up at a fixed 18:00, players spot door signs themselves (no board hints), real rooms instead of fading roofs, bedtime unchanged, rain and clearing the back fields both in Phase 2. 13 tasks: close Phase-001, real rooms, Bramblewick Green first ring, shipping box and pricing, storage chest, house-visit orders with the freshness bonus, Orders list, rain, clearing Field A, a new animal, sticker book, smoothness and HUD budget, playtest.
- **0.2 — 2026-11-21:** Owner picks: **sheep** for Task 2.10; rooms are entered by **walking into the doorway**.
- **0.3 — 2026-11-21:** **Plan approved by the owner** ("ok can"); `open_orders` on Back/Select decided; Task 2.1 started.
- **0.4 — 2026-11-21:** **Task 2.1 done:** Phase-001 closed, docs synced, owner decisions written back; Notes filled.
