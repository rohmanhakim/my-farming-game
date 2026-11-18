# Mechanics

> **Status:** Brainstorming draft
> **Version:** 0.7
> **Last updated:** 2026-11-18
> **Purpose:** Recommended player verbs and mechanics adjusted to the scope of the game described in the GDD. Every element here is a proposal pending playtesting. The GDD remains authoritative for gameplay, audience, and project decisions.

---

## 1. Scope and design intent

The mechanics below are deliberately small. The game is aimed mainly at children from pre-elementary through first grade, playing with a parent or caregiver, while accommodating older children (up to fifth grade) through optional challenge layers. Some players are new to games and not yet fluent readers. Essential actions and feedback do not depend on reading, fast reactions, or precise timing.

The game has two modes:

- **Gentle Core (always on):** The mechanics listed in sections 2–7. No punishment, no currency, no reading required, no time pressure. The core loop never gets harder.
- **Challenge Layers (opt-in, parent-controlled):** Optional verbs and systems toggled by a parent in a grown-up menu (see challenge-layers.md). These include sell, buy, craft, and upgrade verbs, plus market coins and crafting outputs. They never remove Gentle Core features.

- **One lane, one garden:** All mechanics serve Bramblewick Lane and the player's front garden. There is no large map to navigate.
- **No punishment:** Crops never wither, nothing is lost, and no task carries a deadline or failure state. Animals nap if ignored — they never die or run away.
- **Collectathon inventory:** Picture-based slots with auto-stacking. No loose sortable inventory, no currency, no "full" penalty. Overflow goes to the Overflow Nest at Mr. Marlow's shed.
- **Time passes gently:** A day/night cycle and a seasonal calendar exist for atmosphere and light gating, but never to pressure the child. Seasons affect which crops grow and how animals behave.

---

## 2. Day and night cycle

A day/night cycle runs through play. It is cosmetic and gently gating — never punitive.

**Clock model (Milestone 0 decision 2, GDD §4):** the day is divided into **8 phases of 3 in-game hours** each, and the clock **jumps one 3-hour step at a time** — it never ticks minute by minute. One step ≈ **3 minutes of play** (≈24-minute in-game day; tunable). The clock runs only while playing, stops when the pause menu is open, does not advance while the game is closed, and resets to Dawn when the child sleeps.

| # | Phase | Clock | What is true |
|---|---|---|---|
| 1 | Dawn | 06:00–09:00 | Morning greeting; the lane wakes |
| 2 | Morning | 09:00–12:00 | Marlow at the gate; butterflies visit grown flowers |
| 3 | Midday | 12:00–15:00 | Brightest; neighbours out and about |
| 4 | Afternoon | 15:00–18:00 | Distant wave; brook keeper on warm afternoons; Marlow's late-afternoon presence |
| 5 | Evening | 18:00–21:00 | Evening settling begins; gold light |
| 6 | Dusk | 21:00–00:00 | Lane darkens; fireflies appear in season; Marlow may be indoors |
| 7 | Night | 00:00–03:00 | Quiet dark palette; seed shed lit; Moon Petal glows |
| 8 | Small hours | 03:00–06:00 | Deepest dark; quiet only — nothing lost or missed |

- **HUD clock:** a corner dial (arc of daylight + sun/moon marker) jumps one step at a time, with the phase's light tint over the scene; an optional digital readout is for the grown-up. No essential action depends on reading it.
- **Day (phases 1–5):** Butterflies visit flowers; most neighbours are out and about; the lane is bright.
- **Night (phases 6–8):** Fireflies appear in season; the lane takes on a quieter, darker palette; some neighbours are indoors.
- **Sleep/rest:** The child can end the day by resting at home. Sleep advances the calendar by one day and returns the clock to Dawn (06:00). There is no penalty for sleeping early, late, or not at all.
- **No time pressure:** The clock does not cause crop death, missed events, or lost progress. Events that occur at a specific time of day will recur or be restated kindly. The clock never counts down to anything.

---

## 3. Calendar and seasons

The game uses a seasonal calendar (exact season length TBD). Seasons affect which crops grow, which creatures appear, and which events are available.

- **Spring:** Butterfly Garden flowers bloom; butterflies return.
- **Summer:** Warm-weather crops and creatures; long days.
- **Autumn:** Harvest-themed crops and decorations; falling leaves.
- **Winter:** Few outdoor crops; the lane rests; indoor and shelf-based activities remain available.

Plants grow only in their appropriate season. Out-of-season seeds simply do not sprout — no penalty, no loss.

---

## 4. Core verbs (player actions)

| Verb | What it does | Notes |
|---|---|---|
| **Move** | Walk the character around the lane and garden. | Left stick / D-pad or tap-to-move. No run button required. |
| **Interact** | Talk to NPCs, open the seed shed, use garden beds, pick up/release creatures. | One primary action button. Contextual; no reading required. NPCs appear at scheduled times/places; a declined greeting is always restated kindly. |
| **Plant** | Place a seed in an empty garden bed. | Uses the seed box tool belt slot. |
| **Water** | Pour water on a planted seed or growing plant. | Optional. Speeds growth and gives sparkle/sound feedback. Plants grow without it. |
| **Harvest** | Gather a grown flower or crop. | Automatic; the item goes straight to the garden shelf. No inventory to manage. |
| **Find & catch** | Use the butterfly net to catch a butterfly or firefly. | Unlocked by the Butterfly Garden task. Can show to Marlow or release in the garden. |
| **Sleep / rest** | End the current day and advance to the next. | Available at home. No penalty. |
| **Decorate** | Place a small decoration in the front garden. | Creative ownership. No optimization required. |
| **Talk / give** | Greet an NPC or show them a caught creature or animal product. | Drives the want → task → reward loop. |
| **Feed** | Scatter food for an animal (e.g. grain for the chicken, hay for the cow). | Picture icon appears above the animal when hungry. Part of animal care responsibility. |
| **Brush** | Groom an animal when its wool or coat is fluffy (e.g. the sheep). | Picture icon appears above the animal when brushing is needed. Collects wool. |
| **Collect** | Gather an animal product (egg from nest, honey from hive, wool from sheep, milk from cow). | Product goes straight to collectathon inventory slot. |

## 4.5 Optional verbs (challenge layers only)

These verbs are inactive in Gentle Core. A parent turns them on in the grown-up menu. They are additive — Gentle Core verbs continue to work exactly as before.

| Verb | What it does | Notes |
|---|---|---|
| **Sell** | Sell harvested crops, eggs, wool, honey, or milk at the market stand for Garden Coins. | Only active when Garden Coins layer is on. No debt or bankruptcy possible. |
| **Buy** | Purchase seed packets, decorations, or tool upgrades with Garden Coins. | Only active when Garden Coins layer is on. Parent can set spending limits. |
| **Craft** | Combine items (e.g. old plants + soil = compost) to make compost, scarecrow, or bird feeder. | Recipes shown in pictures. Only active when Crafting layer is on. |
| **Upgrade** | Improve a tool (e.g. watering can holds more water; hoe works faster). | Only active when Tool Upgrades layer is on. |
| **Toggle settings** | Open the grown-up menu to change challenge layers, session timer, or parent gate. | Parent-only interaction. Protected by grown-up gesture or question. |

---

## 5. Garden growth model

- Plants advance through visible growth stages while the game is being played, on **accumulated play time**.
- **Stage timing (Milestone 0 decision, GDD §4):** default **Medium ≈ 30–45 s per stage** (seed → bloom ≈ 2 min); **Fast ≈ 15–20 s** and **Slow ≈ 60–90 s** presets available.
- Watering is optional: it accelerates growth and gives cheerful immediate feedback, but plants continue to grow without it.
- Plants never wither, die, or become worse because the child stopped playing.
- **Nothing grows while the game is closed** (default). The optional **Gentle catch-up** setting adds at most one stage on the next launch and never skips a plant past bloom — positive only, never a penalty.
- Growth settings are chosen on the options screen **before starting a new game**; sleep/rest does not grant an extra growth stage.
- Once grown, plants keep blooming into later seasons (they only need their own season to *sprout*), so spring flowers still draw butterflies in summer.
- The initial garden is small (two or three beds) so tending remains play, not field maintenance.

---

## 6. Tool belt and storage

- **Fixed tool belt:** Large icon slots for the watering can, seed box, and earned tools (e.g. butterfly net, brush, basket). New tool icons appear as they are earned; old tools never disappear. No loose, sortable inventory.
- **Collectathon inventory slots:** Picture-based categories (Seeds, Tools, Animal Products, Harvested Crops, Decorations, Caught Creatures). Items auto-stack with number badges. Overflow goes to the Overflow Nest at Mr. Marlow's shed.
- **Garden shelf:** Harvests are routed here automatically for viewing. The child can see what they have grown without managing quantities.
- **Hands:** Available from the start for planting, harvesting, decorating, talking, and general interaction — these are actions, not inventory items.
- **Care menu:** A dedicated control (right-stick click / on-screen care icon — see controls.md) opens a picture-only care view showing each animal's heart meter and current need. No reading required.

---

## 7. Parent-child co-play mechanics

- The child and parent can play together on PC using two separate gamepads, collaborating in real time.
- On touchscreen devices they take turns on the same screen.
- Solo play is complete; no activity requires a parent.
- Simultaneous input is never essential. Activities create opportunities to talk, make choices, and solve small problems together.

---

## 8. Gentle Core exclusions

The following mechanics are excluded from Gentle Core and never appear unless a parent turns on the corresponding challenge layer:

- Crop death, withering, spoilage, or rot.
- Animal death, running away, or permanent unhappiness from neglect.
- Debt, currency, selling, or economy depth. (May reappear as the Garden Coins layer.)
- Stamina, fatigue, or energy bars.
- Crafting, complex inventory, or manual sorting. (May reappear as the Crafting layer.)
- Combat, danger, or failure states.
- Time pressure, deadlines, or punishments for taking breaks.
- Reading required to perform essential actions.
- Real-time animal need decay while the game is closed.

These items may reappear as opt-in challenge layers managed by the parent (see challenge-layers.md). Gentle Core never includes them.

---

## Related documents

| Document | Relationship |
|---|---|
| [README.md](../README.md) | Centralized hub for the project. |
| [GDD.md](./GDD.md) | Authoritative for gameplay, audience, and project decisions; mechanics align with its scope. |
| [controls.md](./controls.md) | Core verbs map to gamepad, mouse, and touchscreen inputs. |
| [inventory.md](./inventory.md) | Tool belt slots and collectathon categories store items produced by verbs. |
| [crops.md](./crops.md) | Plant, water, and harvest verbs drive the crop growth model. |
| [animals.md](./animals.md) | Feed, brush, and collect verbs implement animal care. |
| [events.md](./events.md) | Verbs trigger core, daily, ambient, and seasonal events. |
| [challenge-layers.md](./challenge-layers.md) | Defines optional challenge layers and parent-controlled toggles. |

## 9. Change log

- **0.1 — 2026-11-18:** Initial draft. Added day/night cycle, sleep/rest, seasonal calendar, and growth model aligned with the GDD's low-pressure, no-inventory scope.
- **0.2 — 2026-11-18:** Added Feed, Brush, Collect verbs for animal care; collectathon inventory slots with auto-stacking and Overflow Nest; seasonal animal behavior; explicit exclusion of real-time animal decay while closed.
- **0.3 — 2026-11-18:** Added Two Modes principle (Gentle Core always on; challenge layers opt-in). Added optional verbs (sell, buy, craft, upgrade, toggle settings) for challenge layers. Re-framed excluded mechanics as "Gentle Core exclusions" with note that they may reappear as opt-in layers.
- **0.4 — 2026-11-18:** Added NPC scheduled presence and kind restatement of declined greetings to the Interact verb notes.
- **0.5 — 2026-11-18:** Audience reworded (mainly pre-elementary–1st grade with a parent; older kids accommodated by challenge layers). Added season-persistence rule for grown plants (butterfly consistency), the care menu (dedicated control), hay/milk to Feed and Collect verbs, and merged the split Gentle Core exclusion list.
- **0.6 — 2026-11-18:** §5 garden growth model synced to Milestone 0 decision 1 (GDD §4): stage timing Medium ≈30–45 s with Fast/Slow presets, nothing grows while the game is closed (optional positive-only Gentle catch-up), settings chosen before a new game; sleep grants no extra stage.
- **0.7 — 2026-11-18:** §2 day/night cycle rewritten to Milestone 0 decision 2 (GDD §4 v0.17): **8 day phases × 3 in-game hours**, clock stepped in 3-hour jumps (≈3 min play per step), running only while playing, reset to Dawn on sleep, plus a stepped HUD dial. Phase table maps presence/events to phases; the clock explicitly never drives growth, care, or penalties.