# Mechanics

> **Status:** Brainstorming draft — **updated for the direction reset 2026-11-19**
> **Version:** 0.8
> **Last updated:** 2026-11-19
> **Purpose:** Player verbs and systems. Every element is a proposal pending playtesting. The GDD remains authoritative for gameplay, audience, and project decisions.

---

## 1. Scope and design intent

The game is a realistic but kid-friendly, educational farm sim for PC (GDD §1–§3). Mechanics stay small and readable; challenge comes from the farm growing, and automation relieves it.

- **One lane, a growing farm:** Bramblewick Lane stays small; Kit's farm grows into the fields behind the house.
- **Consequences, kindly shown:** dry crops yellow then are lost; neglected animals and fish are gone (gravestone message card). Nothing changes while the game is closed; every loss is warned first.
- **Money is core:** earned mainly through neighbour orders, plus optional alternatives (GDD §4 *Ways to earn*). No debt.
- **Difficulty modes** (Easy / Normal / Realistic, challenge-layers.md) tune the numbers only.

---

## 2. Day and night cycle

**Clock model (Milestone 0 decision 2, GDD §4):** 8 phases × 3 in-game hours; the clock jumps one 3-hour step at a time (≈3 minutes of play per step, ≈24-minute in-game day). Runs only while playing; stops in the pause menu and while closed; resets to Dawn on sleep.

| # | Phase | Clock | What is true |
|---|---|---|---|
| 1 | Dawn | 06:00–09:00 | New orders on the board; sprinklers run |
| 2 | Morning | 09:00–12:00 | Marlow at the gate; seed shed open |
| 3 | Midday | 12:00–15:00 | Neighbours out; market busiest |
| 4 | Afternoon | 15:00–18:00 | Brook keeper; travelling merchant on visit days |
| 5 | Evening | 18:00–21:00 | Shops close; shipping box collected at day's end |
| 6 | Dusk | 21:00–00:00 | Fireflies in season |
| 7 | Night | 00:00–03:00 | Seed shed lit; Moon Petal glows |
| 8 | Small hours | 03:00–06:00 | Quiet |

- **HUD clock:** a corner dial with sun/moon marker plus a digital readout.
- **Sleep/rest:** ends the day, pays out the shipping box, resets the clock to Dawn. No penalty for when you sleep.
- The clock drives light, presence, shop hours, order refresh, and shipping payout. Growth and care meters run on play time (§5, animals.md §3).

---

## 3. Calendar and seasons

Seasons decide what sprouts, which fish and creatures appear, festival dates, and **seasonal prices** (out-of-season goods pay more). Season length TBD.

- **Spring:** Butterfly Garden flowers; butterflies return.
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
| **Harvest** | Gather a ready crop or fruit. | Goes to inventory (garden shelf in Phase-001). |
| **Feed** | Fill an animal's or the pond's meter. | Grain, hay, fish food. |
| **Brush** | Groom the sheep. | Raises wool quality. |
| **Collect** | Take eggs, milk, wool, honey, fish. | Quality 1–3 stars. |
| **Find & catch** | Net butterflies/fireflies. | Unlocked by the Butterfly Garden task; always released. |
| **Fish** | Cast a rod at the brook or pond. | Simple timing-free catch (no reflex test). |
| **Forage** | Pick wild berries, herbs, mushrooms. | Along the lane; seasonal. |
| **Sleep / rest** | End the day. | At home. |
| **Talk / give / deliver** | Greet, gift, or **deliver an order**. | Drives orders and relationships. |

## 5. Economy and building verbs (core)

| Verb | What it does | Notes |
|---|---|---|
| **Take order** | Accept an order from the board or a neighbour. | Optional; no deadline penalty; cancel any time. |
| **Sell** | Sell at the market stall, shipping box, or farm shop. | Seasonal and demand pricing (swing by mode). |
| **Buy** | Seeds, animals, tools, buildings, land, blueprints, automation. | Money never goes below zero. |
| **Trade** | Swap goods with the travelling merchant. | Rare seeds, breeds, decorations. |
| **Process** | Turn produce into jam, bread, cheese, pie, smoked fish. | Kitchen/mill; worth more than raw goods. |
| **Craft** | Make tools, fences, decorations, sprinklers, feeders from blueprints. | Workshop; recipes shown in pictures + short text. |
| **Build / place / move** | Place buildings, beds, decorations; move them freely. | Land footprint is the late-game limit (buildings.md). |
| **Upgrade** | Improve tools or buildings. | Bigger watering can, bigger coop, better sprinklers. |
| **Expand land** | Buy or clear a new field. | progression.md §4. |
| **Enter contest** | Submit a product to a festival contest. | Optional; ribbons and prizes. |
| **Decorate** | Place decorations to raise farm charm. | progression.md §6.1. |

---

## 6. Garden growth and neglect model

- Growth on **accumulated play time while the soil is moist**: default **Medium ≈ 30–45 s per stage**; Fast/Slow presets on the new-game screen.
- One watering keeps soil moist ≈ one in-game day; sprinklers water each Dawn.
- **Dry → yellow → gone**, with grace times set by mode (challenge-layers.md §3). Watering at yellow recovers the crop.
- Nothing grows or dries while the game is closed; the optional Gentle catch-up setting adds at most one stage on next launch.
- Sleep does not grant an extra stage.
- Grown crops remain harvestable after their season ends.

---

## 7. Automation (core, key selling point)

| Automation | Replaces | Unlock | Notes |
|---|---|---|---|
| **Sprinklers** (basic / quality / deluxe) | Watering | Blueprint (craft) or buy; T4 (basic earlier via a reward) | Needs a water source in range (well, pond, pump). |
| **Auto-feeder** | Feeding coop/barn animals | Craft or buy; T4 | Draws feed from the silo. |
| **Pond auto-feeder** | Feeding fish | Craft or buy; T4 | Draws fish food from storage. |
| **Helpers** | Harvesting, collecting, brushing, delivering, stall-keeping | Helper post; T4 | Each covers an area; friendly characters or creatures. |
| **Windmill / water pump** | Water reach, milling, power | T4 | Powers hydroponics. |

Automation never removes manual verbs. It is unlocked by **crafting or buying** at a point in progression, and every unit uses land.

---

## 8. Tool belt and storage

- **Tool belt:** large icon slots for watering can, seed box, and earned tools (net, brush, rod, hoe, scythe…). Old tools never disappear.
- **Inventory:** picture-based, auto-stacking slots by category (inventory.md); items stack by quality.
- **Storage chest / silo:** overflow and feed storage; no "full" penalty.
- **Wallet:** money shown on the HUD.
- **Care menu:** a dedicated control (controls.md §2) shows each animal's and pond's meter.
- **Garden shelf:** the Phase-001 stand-in for the inventory.

---

## 9. Co-play

- Two players on one PC in real time (controls.md §3); solo play is complete.
- Proposed: shared farm, shared wallet, either player can deliver orders (GDD §16 Q4 open).

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
| [challenge-layers.md](./challenge-layers.md) | Difficulty modes. |

## 12. Change log

- **0.1 — 2026-11-18:** Initial draft. Added day/night cycle, sleep/rest, seasonal calendar, and growth model aligned with the GDD's low-pressure, no-inventory scope.
- **0.2 — 2026-11-18:** Added Feed, Brush, Collect verbs for animal care; collectathon inventory slots with auto-stacking and Overflow Nest; seasonal animal behavior; explicit exclusion of real-time animal decay while closed.
- **0.3 — 2026-11-18:** Added Two Modes principle (Gentle Core always on; challenge layers opt-in). Added optional verbs (sell, buy, craft, upgrade, toggle settings) for challenge layers. Re-framed excluded mechanics as "Gentle Core exclusions" with note that they may reappear as opt-in layers.
- **0.4 — 2026-11-18:** Added NPC scheduled presence and kind restatement of declined greetings to the Interact verb notes.
- **0.5 — 2026-11-18:** Audience reworded (mainly pre-elementary–1st grade with a parent; older kids accommodated by challenge layers). Added season-persistence rule for grown plants (butterfly consistency), the care menu (dedicated control), hay/milk to Feed and Collect verbs, and merged the split Gentle Core exclusion list.
- **0.6 — 2026-11-18:** §5 garden growth model synced to Milestone 0 decision 1 (GDD §4): stage timing Medium ≈30–45 s with Fast/Slow presets, nothing grows while the game is closed (optional positive-only Gentle catch-up), settings chosen before a new game; sleep grants no extra stage.
- **0.7 — 2026-11-18:** §2 day/night cycle rewritten to Milestone 0 decision 2 (GDD §4 v0.17): **8 day phases × 3 in-game hours**, clock stepped in 3-hour jumps (≈3 min play per step), running only while playing, reset to Dawn on sleep, plus a stepped HUD dial. Phase table maps presence/events to phases; the clock explicitly never drives growth, care, or penalties.
- **0.8 — 2026-11-19:** **Direction reset (GDD v0.19).** Two-modes/challenge-layer framing replaced by three difficulty modes. Watering now required; neglect rule (dry → yellow → gone; animals gone with gravestone card). Sell/buy/craft/upgrade promoted to **core economy and building verbs** with new verbs: take order, trade, process, build/place/move, expand land, enter contest, fish, forage. New **§7 Automation** (sprinklers, auto-feeders, pond auto-feeder, helpers, windmill/pump). Clock phases now drive order refresh, shop hours, and shipping payout. Touchscreen and turn-taking co-play removed (PC only). Exclusions rewritten; §11 points to the endgame in progression.md.
