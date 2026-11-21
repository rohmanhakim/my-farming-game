# Mechanics

> **Status:** Brainstorming draft — **updated for the direction reset 2026-11-19**
> **Version:** 0.23
> **Last updated:** 2026-11-21
> **Purpose:** Player verbs and systems. Every element is a proposal pending playtesting. The GDD remains authoritative for gameplay, audience, and project decisions.

---

## 1. Scope and design intent

The game is a realistic but kid-friendly, educational farm sim for PC (GDD §1–§3). Mechanics stay small and readable; challenge comes from the farm growing, and automation relieves it.

- **One lane, a growing farm:** Bramblewick Lane stays small; Kit's farm grows into the fields behind the house.
- **Consequences, kindly shown:** dry crops yellow then are lost; neglected animals and fish are gone (gravestone message card). Nothing changes while the game is closed; every loss is warned first.
- **Money is core:** earned mainly through neighbour orders, plus optional alternatives (GDD §4 *Ways to earn*). No debt.
- **One difficulty mode:** realistic but kid-friendly (challenge-layers.md).
- **All land is Kit's but overgrown;** clearing it is part of play (progression.md §4).
- **Day-based:** crops grow and animals need food per in-game day; **3 days in a row without care → gone**, with 2 warning days.

---

## 2. Day and night cycle

**Clock model (Milestone 0 decision 2, GDD §4):** 8 phases × 3 in-game hours; the phase changes one 3-hour step at a time (3 min of play per step, 60 s per in-game hour — Harvest Moon pace) and the HUD time ticks every 15 in-game minutes (15 s; the playable day 04:30 → 21:30 lasts 17 real minutes; wake-up 04:30, forced bedtime 21:30). Runs only while playing; stops in the pause menu and while closed; resets to Dawn on sleep.

| # | Phase | Clock | What is true |
|---|---|---|---|
| 1 | Dawn | 04:30–07:30 | Order signs on a few neighbours' doors (Phase-001: new orders on the board); sprinklers run |
| 2 | Morning | 07:30–10:30 | Marlow at the gate; seed shed open |
| 3 | Midday | 10:30–13:30 | Neighbours out; market busiest |
| 4 | Afternoon | 13:30–16:30 | Brook keeper; travelling merchant on visit days |
| 5 | Evening | 16:30–19:30 | Shops close; the shipping box is picked up when Kit sleeps |
| 6 | Dusk | 19:30–22:30 | Fireflies in season |
| 7 | Night | 22:30–01:30 | Seed shed lit; Moon Petal glows |
| 8 | Small hours | 01:30–04:30 | Quiet |

- **Time stops** while a menu, conversation or shop screen is open, and **also indoors** (owner 2026-11-20, kid tolerance, GDD §4).
- **HUD clock:** a corner dial with sun/moon marker (per phase) plus a digital readout ticking every 15 in-game minutes.
- **Sleep/rest:** ends the day, picks up and pays out the shipping box (only what is in it at that moment), resets the clock to Dawn. No penalty for when you sleep.
- The clock drives light, presence, shop hours, order refresh, and shipping payout. Day rollover (sleep or the 21:30 bedtime) advances crop growth and the watering/feeding counts (§6, animals.md §3).

---

## 3. Calendar and seasons

Seasons decide what sprouts, which fish and creatures appear, festival dates, and **seasonal prices** (out-of-season goods pay more). **Each season is 30 days** (120-day year, like Harvest Moon).

- **Spring:** First crops; butterflies return (optional Butterfly Garden request).
- **Summer:** Warm crops; fish grow fast; fireflies.
- **Autumn:** Harvest crops, orchard fruit; harvest festival.
- **Winter:** Few outdoor crops; greenhouse, kitchen, and workshop shine; hens rest.

---

## 4. Core verbs

| Verb | What it does | Notes |
|---|---|---|
| **Move** | Walk around the farm and lane. | WASD / left stick / click-to-move. |
| **Interact** | Talk, use beds, open shops/boards, pick up. | One primary action. |
| **Plant** | Put a seed in an empty bed. | Seeds bought or given. |
| **Water** | Moisten a bed's soil. | **Required for growth**; recovers a yellow crop. |
| **Harvest** | Gather a ready crop or fruit. | Goes to the inventory. |
| **Feed** | Fill an animal's or the pond's meter. | Grain, hay, fish food. |
| **Brush** | Groom the sheep. | Raises wool quality. |
| **Collect** | Take eggs, milk, wool, honey, fish. | Quality 1–3 stars. |
| **Find & catch** | Net butterflies/fireflies. | Unlocked by the optional Butterfly Garden request; always released. |
| **Fish** | Cast a rod at the brook or pond. | Simple timing-free catch (no reflex test). |
| **Forage** | Pick wild berries, herbs, mushrooms. | Along the lane; seasonal. |
| **Sleep / rest** | End the day. | At home. |
| **Talk / give / deliver** | Greet, gift, or **deliver an order**. | Drives orders and relationships. |

## 5. Economy and building verbs (core)

| Verb | What it does | Notes |
|---|---|---|
| **Take order** | Accept an order **at a neighbour's house** (from Phase-002; Phase-001 uses Marlow's board): spot the sign (basket with a picture note) while walking round, knock, talk to the neighbour, take the order (owner 2026-11-21). | Optional; cancel any time. **Send** it by putting the goods in the shipping box; at pickup the box fills accepted orders first. Freshness bonus +30 % / +20 % / +10 % by the night it goes out; unsent after three nights it is simply gone, nothing else lost (economy.md §2). |
| **Subscribe / cancel** | Accept a standing daily order (e.g. 6 eggs every day; sent through the shipping box from Phase-002) or cancel it. | Daily HUD reminder; missed day = unpaid, no penalty; a helper can deliver it. |
| **Hire / dismiss** | Hire a farmhand for a job and area, or let them go. | Daily wage from the wallet; unpaid = day off, never debt. |
| **Sell** | Put goods in the **shipping box** beside the house (owner 2026-11-21). Kit can take them back out any time before pickup; whatever is in the box when Kit sleeps fills accepted orders first, then the rest sells. Goods Kit keeps stay in the bag or the **storage chest** at the house and never spoil. Later: the farm shop (T5). The **market stall is deferred** to a later phase (if it returns: set prices and leave it). | Normal sell price with quality, seasonal and demand pricing (economy.md). A morning note shows what sold. |
| **Buy** | Seeds, animals, tools, buildings, blueprints, automation (at the seed shed). | Money never goes below zero. |
| **Order feed** | Phone the feed seller from home (the parents left the number) for grain, hay, or fish food. | Before 18:00 → delivered to the feed bins at Dawn next day; later → the day after; no fee. |
| **Trade** | Swap goods with the travelling merchant. | Rare seeds, breeds, decorations. |
| **Process** | Turn produce into jam, bread, cheese, pie, smoked fish. | Kitchen/mill; worth more than raw goods. |
| **Craft** | Make tools, fences, decorations, sprinklers, feeders from blueprints. | Workshop; recipes shown in pictures + short text. |
| **Build / place / move** | Place buildings, beds, decorations; move them freely. | Land footprint is the late-game limit (buildings.md). |
| **Upgrade** | Improve tools or buildings. | Bigger watering can, bigger coop, better sprinklers. |
| **Clear land** | Remove weeds, stones, stumps, logs with hands, scythe, hammer, axe. | All land is owned; clearing gives materials (progression.md §4). |
| **Enter contest** | Submit a product to a festival contest. | Optional; ribbons and prizes. |
| **Decorate** | Place decorations to raise farm charm. | progression.md §6.1. |

---

## 6. Garden growth and neglect model

- **Day-based (Harvest Moon-style):** a crop grows one day overnight if it was watered (or rained on) that day; every crop takes at least 1 day (crops.md).
- **Neglect:** 2 dry days in a row → yellow; 3 → gone (tile stays tilled). Watering resets the count.
- Sprinklers water each Dawn. Nothing changes while the game is closed.
- Grown crops remain harvestable after their season ends.

---

## 7. Automation (core, key selling point)

| Automation | Replaces | Unlock | Notes |
|---|---|---|---|
| **Sprinklers** (basic / quality / deluxe) | Watering | Blueprint (craft) or buy; T4 (basic earlier via a reward) | No water source needed (hidden pipes); shows its coverage before placing. |
| **Auto-feeder** | Feeding coop/barn animals | Craft or buy; T4 | Draws feed from the silo; phone orders and harvests fill the silo; low-feed warning. |
| **Pond auto-feeder** | Feeding fish | Craft or buy; T4 | Draws fish food from storage. |
| **Helpers** — hired farmhands (daily wage) or befriended garden sprites (free, few jobs/day) | Watering, feeding, harvesting, collecting, brushing, clearing, delivering subscription orders, filling the shipping box (stall-keeping if the market stall returns) | Farmhands: "help wanted" card on the order board, by the day, T2. Sprites: friendship, T4 | Each covers an area; friendly characters or creatures. |

Automation never removes manual verbs. It is unlocked by **crafting or buying** at a point in progression, and every unit uses land.

---

## 8. Tool belt and storage

- **Tool belt:** large icon slots for watering can, seed box, and earned tools (net, brush, rod, hoe, scythe…). Old tools never disappear.
- **Inventory:** picture-based, auto-stacking slots by category (inventory.md); items stack by quality.
- **Storage chest / silo:** overflow and feed storage; no "full" penalty.
- **Wallet:** money shown on the HUD.
- **Care menu:** a dedicated control (controls.md §2) shows each animal's and pond's meter.

---

## 9. Co-play

- **One house, one farm, one map**; shared wallet, storage, and orders; either player can deliver.
- **Dynamic split screen:** one camera when the players are close; splits when they move apart; merges again when they meet (controls.md §3).
- Solo play is complete.

---

## 10. Exclusions

- Sickness, injury, vet, medicine; on-screen death.
- Debt, loans, bankruptcy.
- Stamina/energy bars.
- Combat or danger.
- Needs, growth, or dryness advancing while the game is closed.
- Reflex or timing tests (fishing included).
- Spoilage of harvested goods.
- Building repairs/breakdowns.

---

## 11. After the farm is running

Optional long-term systems — decoration and farm charm, collections, contests, lane restoration, showcase, mastery, prestige items — are defined in progression.md §6.

---

## Related documents

| Document | Relationship |
|---|---|
| [README.md](../README.md) | Centralized hub for the project. |
| [GDD.md](./GDD.md) | Authoritative for gameplay; mechanics align with its scope. |
| [controls.md](./controls.md) | Verbs mapped to PC inputs. |
| [inventory.md](./inventory.md) | Storage, wallet, quality stacks. |
| [crops.md](./crops.md) | Growth and neglect. |
| [animals.md](./animals.md) | Feed, brush, collect, fish pond. |
| [buildings.md](./buildings.md) | Build, place, automation fixtures. |
| [events.md](./events.md) | Verbs trigger events. |
| [progression.md](./progression.md) | Tiers, land, endgame. |
| [challenge-layers.md](./challenge-layers.md) | The single difficulty mode. |
| [economy.md](./economy.md) | Prices. |

## 12. Change log

- **0.1 — 2026-11-18:** Initial draft. Added day/night cycle, sleep/rest, seasonal calendar, and growth model aligned with the GDD's low-pressure, no-inventory scope.
- **0.2 — 2026-11-18:** Added Feed, Brush, Collect verbs for animal care; collectathon inventory slots with auto-stacking and Overflow Nest; seasonal animal behavior; explicit exclusion of real-time animal decay while closed.
- **0.3 — 2026-11-18:** Added Two Modes principle (Gentle Core always on; challenge layers opt-in). Added optional verbs (sell, buy, craft, upgrade, toggle settings) for challenge layers. Re-framed excluded mechanics as "Gentle Core exclusions" with note that they may reappear as opt-in layers.
- **0.4 — 2026-11-18:** Added NPC scheduled presence and kind restatement of declined greetings to the Interact verb notes.
- **0.5 — 2026-11-18:** Audience reworded (mainly pre-elementary–1st grade with a parent; older kids accommodated by challenge layers). Added season-persistence rule for grown plants (butterfly consistency), the care menu (dedicated control), hay/milk to Feed and Collect verbs, and merged the split Gentle Core exclusion list.
- **0.6 — 2026-11-18:** §5 garden growth model synced to Milestone 0 decision 1 (GDD §4): stage timing Medium ≈30–45 s with Fast/Slow presets, nothing grows while the game is closed (optional positive-only Gentle catch-up), settings chosen before a new game; sleep grants no extra stage.
- **0.7 — 2026-11-18:** §2 day/night cycle rewritten to Milestone 0 decision 2 (GDD §4 v0.17): **8 day phases × 3 in-game hours**, clock stepped in 3-hour jumps (≈3 min play per step), running only while playing, reset to Dawn on sleep, plus a stepped HUD dial. Phase table maps presence/events to phases; the clock explicitly never drives growth, care, or penalties.
- **0.8 — 2026-11-19:** **Direction reset (GDD v0.19).** Two-modes/challenge-layer framing replaced by three difficulty modes. Watering now required; neglect rule (dry → yellow → gone; animals gone with gravestone card). Sell/buy/craft/upgrade promoted to **core economy and building verbs** with new verbs: take order, trade, process, build/place/move, expand land, enter contest, fish, forage. New **§7 Automation** (sprinklers, auto-feeders, pond auto-feeder, helpers, windmill/pump). Clock phases now drive order refresh, shop hours, and shipping payout. Touchscreen and turn-taking co-play removed (PC only). Exclusions rewritten; §11 points to the endgame in progression.md.
- **0.9 — 2026-11-19:** Owner picks: single difficulty mode; **day-based growth** and the 3-days neglect rule (§1, §6); 'Expand land' verb replaced by **Clear land** (all land owned, overgrown); garden shelf removed; Butterfly Garden now optional; co-play decided (shared farm, dynamic split screen).
- **0.10 — 2026-11-19:** Added **subscribe/cancel** (standing daily orders with reminder) and **hire/dismiss** verbs; helpers now explicitly hired farmhands (daily wage) or befriended garden sprites.
- **0.11 — 2026-11-19:** Added **Order feed** (phone the feed seller from home, 18:00 cut-off, Dawn delivery to feed bins); Buy no longer lists land.
- **0.12 — 2026-11-19:** Clock step ≈ 2 min 15 s (≈ 15-minute playable day); seasons 30 days.
- **0.13 — 2026-11-20:** Forced bedtime moved to **01:00** (owner decision, GDD v0.29).
- **0.14 — 2026-11-20:** Forced bedtime corrected to **midnight (00:00)** (owner, GDD v0.30).
- **0.15 — 2026-11-20:** Day reset by the owner: wake-up 04:30, forced bedtime 21:30; phase table times shifted to steps starting at 04:30 (GDD v0.31).
- **0.16 — 2026-11-20:** HUD time ticks every 15 in-game minutes (owner, GDD v0.32).
- **0.17 — 2026-11-20:** Harvest Moon pace: 60 real seconds per in-game hour (owner, GDD v0.33).
- **0.18 — 2026-11-20:** When time stops: menus, conversations, shop screens; never just for being indoors (owner, GDD v0.34).
- **0.19 — 2026-11-20:** Time also stops indoors (owner, GDD v0.35).
- **0.20 — 2026-11-20:** **Farmhands first, machines later (owner decision 2026-11-20):** farmhands hired by the day from the order board at T2 (no cottage); sprites stay T4.
- **0.21 — 2026-11-20:** **Machines simplified (owner decision 2026-11-20):** sprinklers need no water source; silo filled by phone orders and harvests; windmill no longer extends water reach.
- **0.22 — 2026-11-20:** **Windmill / water pump removed (owner decision 2026-11-20):** row dropped; flour is milled at the village watermill.
- **0.23 — 2026-11-21:** **Selling and house-visit orders (owner decisions 2026-11-21).** Sell verb: the **shipping box** first (take items back out until pickup when Kit sleeps; only what is in it then sells); the **market stall deferred**. Take-order verb: orders also come from **neighbours' houses** (spot the sign, knock, talk, take the order) and are **sent through the shipping box** with a freshness bonus (+30/+20/+10 %, then simply gone). Day phases, sleep and helper jobs updated. Every order comes from an open house's door (Marlow included); the board holds no orders from Phase-002.
