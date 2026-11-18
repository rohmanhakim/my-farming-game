# Events

> **Status:** Brainstorming draft
> **Version:** 0.5
> **Last updated:** 2026-11-18
> **Purpose:** Events available during one playthrough, adjusted to the scope of the game described in the GDD. Driven by calendar, season, and day/night — never by time pressure or punishment. All names are tentative and can be changed. Every element here is a proposal pending playtesting.

---

## 1. Scope and design intent

Events are gentle milestones, not obligations. There are no deadlines, no missed-event penalties, and no complex calendars the child must read. Events recur or are restated kindly if the child is not present for them.

The game has two modes:

- **Gentle Core (always on):** Core events (Butterfly Garden task), daily/ambient events, and seasonal animal events. No deadlines, no penalties.
- **Challenge Layers (opt-in, parent-controlled):** Seasonal festivals, NPC relationship meters, market-day events, and Lantern Hill ceremony. Only active when a parent turns on the corresponding layer.

---

## 2. Core events

| Event | Trigger | What happens | Reward / outcome | Scope |
|---|---|---|---|---|
| **Butterfly Garden task** | Player accepts Mr. Marlow's first request. | Plant and water three Meadow Bell / Honey Cup / Lace Flower seeds. | Butterfly net; unlocks find & catch verb. | Prototype |
| **Task restatement** | Player speaks to Marlow with an unfinished task. | Marlow kindly restates the request: "Hmm, we didn't finish that last time. Would you like to try it now?" | No penalty; task remains available. | Prototype |
| **Show butterfly to Marlow** | Player has a caught butterfly and speaks to Marlow. | Marlow reacts with delight and notes it in the ledger. | Relationship warmth; ledger page fills (candidate UI). | Prototype |
| **Release butterfly in garden** | Player releases a caught butterfly in the front garden. | Butterfly flutters around the garden; adds life to the space. | Visual joy; no item gain. | Prototype |

---

## 3. Daily / ambient events

| Event | Trigger | What happens | Scope |
|---|---|---|---|
| **Morning greeting** | Player starts a new day / enters the lane in the morning. | Neighbours and Marlow greet the child; lane is bright. | Prototype |
| **Evening settling** | Day transitions to night. | Lane darkens; fireflies appear in season; Marlow may be indoors. | Prototype |
| **Watering sparkle** | Player waters a plant. | Immediate sparkle, sound, or visiting insect feedback. | Prototype |
| **Butterfly visits** | Butterfly Garden flowers are grown (day, spring/summer). | Butterflies appear around the blooms. | Prototype |
| **Fireflies appear** | Night falls (summer season). | Fireflies glow over the brook and garden. | Prototype / later |
| **Animal needs care** | Animal care meter drops to half. | Picture icon (corn, comb, flower, hay) appears above the animal. Child can feed (grain or hay), brush, or plant flowers. | Prototype |
| **Animal product ready** | Egg, wool, honey, or milk is ready to collect. | Egg glows in nest; wool looks fluffy; honey drips from hive; milk bottle full by the cow. | Prototype |
| **Animal naps** | Animal care meter is empty. | Animal rests under a tree / in coop. Mr. Marlow gently suggests feeding. | Prototype |
| **Neighbour trace appears** | A new character is about to be introduced. | A picture-only clue appears at that character's place (laundry on the line, footprints in the mud, a half-painted birdhouse, a seed packet on the gate, an unlit lantern on the hill). No text to read. | Prototype |
| **Distant wave** | Player walks near a place where a future neighbour lives. | A figure waves from a distance, then returns to their task. The lane feels inhabited before any introduction. | Prototype |

---

## 4. Character arrival events

A character's arrival is its own gentle event — one picture-and-voice greeting, then an invitation the child can accept or decline. Nothing is missed forever; a declined invitation simply waits.

| Arrival event | Trigger | What happens | Introduces | Scope |
|---|---|---|---|---|
| **First meeting: Mr. Marlow** | Day one, player reaches the seed shed gate. | Marlow greets the child, offers a seed packet, and names the first task (Butterfly Garden). | Mr. Marlow | Prototype |
| **First meeting: The allotment neighbour** | Player grows their first flower at the shared beds. | A brief wave from the beds; neighbour offers a shared planting tip. Picture-only greeting. | The allotment neighbour | Stage 2 |
| **First meeting: The brook keeper** | Player catches their first creature near the brook. | Keeper appears at the water's edge, smiles, and gestures toward a hidden spot. Picture-only greeting. | The brook keeper | Stage 2 |
| **First meeting: A child of the lane** | Player completes two neighbour tasks and visits the brook. | A peer waves from the lane, then walks alongside the child for a short stretch. No task, just friendship. | A child of the lane | Stage 3 |
| **First meeting: The Hill Visitor** | High warmth with Marlow + winter season, Lantern Hill ceremony layer on. | Visitor arrives at the hill with a story and a seasonal gift. | The Hill Visitor | Stage 4 (challenge layer) |

Each arrival is preceded by a **trace event** (laundry, footprints, birdhouse, seed packet, unlit lantern) so the child senses the neighbour exists before the formal introduction.

---

## 5. Seasonal events (tentative, later unlocks)

| Event | Trigger | What happens | Scope |
|---|---|---|---|
| **Spring bloom day** | Start of spring season. | More flowers bloom; butterflies return in numbers. | Later |
| **Summer evening gathering** | A summer evening at Lantern Hill (deferred). | Neighbours gather; fireflies and lantern light. | Later |
| **Autumn harvest moment** | Start of autumn season. | Leaves fall; harvest-themed decorations available. | Later |
| **Winter lantern lighting** | Winter, high relationship with Marlow (candidate). | The old lantern on Lantern Hill is lit; seasonal visitor attends. | Later — high-relationship event candidate per GDD §3 |
| **Chicken needs winter feed** | Winter season. | Chicken eats more grain; corn icon appears more often. | Prototype |
| **Sheep wool thickens** | Autumn season. | Sheep's wool is extra thick; brushing yields more wool. | Prototype |
| **Beehive sleeps** | Winter season. | Beehive glows dim; no honey production until spring. | Prototype |

---

## 6. Relationship milestones (subtle, optional)

| Milestone | Trigger | What happens | Scope |
|---|---|---|---|
| **First task complete** | Butterfly Garden flowers grown and shown to Marlow. | Marlow gives the butterfly net. | Prototype |
| **Ledger page fills** | Repeated kind interactions and completed tasks (candidate). | A pressed flower or sketch appears behind a ledger page. | Later — candidate relationship UI |

## 7. Stage-Gated Events (challenge layers only)

These events only appear when a parent turns on the corresponding challenge layer. They are additive — Gentle Core events continue to work exactly as before.

| Stage | Event | Trigger | What happens | Active when |
|---|---|---|---|---|
| **Stage 3** | Market day | Child visits the market stand | Sell harvests for Garden Coins; buy seed packets | Garden Coins layer is on |
| **Stage 3** | Crafting fair | Child uses the crafting table | Make compost, scarecrow, or bird feeder | Crafting layer is on |
| **Stage 4** | Spring Bloom Festival | Start of spring | Neighbours gather; more flowers bloom; butterflies return | Stage 4 — all layers on |
| **Stage 4** | Autumn Harvest Moment | Start of autumn | Leaves fall; harvest decorations available; optional goals | Stage 4 — all layers on |
| **Stage 4** | Lantern Hill ceremony | Winter, high relationship with Marlow | Old lantern is lit; seasonal visitor attends | All layers + relationship meter |
| **Stage 5** | Festival prep | Seasonal change | Multi-week preparation for seasonal festival | Full economy layer is on |

Missing a festival or market day simply means waiting for it to return next season. No penalty.

---

## 8. Related documents

| Document | Relationship |
|---|---|
| [README.md](../README.md) | Centralized hub for the project. |
| [GDD.md](./GDD.md) | Want → task → reward loop and relationship meter shape the event design. |
| [world.md](./world.md) | Butterfly Garden task and Lantern Hill events originate from the Bramblewick Lane premise. |
| [characters.md](./characters.md) | Mr. Marlow gives tasks and reacts to events; relationship warmth grows through events. |
| [mechanics.md](./mechanics.md) | Core verbs trigger events (plant, water, catch, feed, brush, collect). |
| [crops.md](./crops.md) | Butterfly Garden flowers are grown for the first core event. |
| [animals.md](./animals.md) | Animal care events (needs care, product ready, naps, seasonal behavior) are listed here. |
| [progression.md](./progression.md) | Stickers and seasonal mysteries reward event-related milestones. |
| [challenge-layers.md](./challenge-layers.md) | Defines optional challenge layers and parent-controlled toggles. |

## 9. Change log

- **0.1 — 2026-11-18:** Initial draft. Butterfly Garden core event, daily/ambient day/night events, seasonal events (later), and relationship milestones. No deadlines or penalties.
- **0.2 — 2026-11-18:** Added animal care events (needs care, product ready, naps) and seasonal animal events (chicken winter feed, sheep wool thickens, beehive sleeps). All events recur or are restated kindly — no penalties for missing them.
- **0.3 — 2026-11-18:** Added Two Modes principle and Stage-Gated Events for challenge layers (market day, crafting fair, festivals, Lantern Hill ceremony).
- **0.4 — 2026-11-18:** Added neighbour trace and distant wave ambient events, plus character arrival events for Mr. Marlow, the allotment neighbour, the brook keeper, a child of the lane, and the Hill Visitor — each gated to a stage with picture-only greetings.
- **0.5 — 2026-11-18:** Day/night and seasons confirmed always-on (Gentle Core): stage-gated festivals now activate at Stage 4 with all layers on instead of a Time Awareness layer. Trace list completed to all five traces (added seed packet, lantern). Care and product events include the cow and milk.
