# Animals

> **Status:** Design draft
> **Version:** 0.3
> **Last updated:** 2026-11-18
> **Purpose:** Four gentle animals on Bramblewick Lane, plus the lane's ambient creatures. Care is responsibility without punishment — animals nap if ignored, never die or run away.

---

## 1. Scope and Design Intent

Four animals maximum. Each is a character with a name, a distinct look, and one care need. Kids learn that animals need attention, but neglect means a nap — never death, sadness, or abandonment.

**No livestock management chores:** No feeding schedules that reset every few minutes. No real-time decay while the game is closed. Animals wait patiently.

---

## 2. Animal Roster

| Animal | Care need (1 max) | Visual cue | Produces (collectathon) | Season behavior |
|---|---|---|---|---|
| **Chicken** (name: TBD) | Feed grain | Corn kernel icon above head | Egg (stackable) | Lays daily in Spring–Autumn; rests in Winter (no eggs) but still eats — needs extra grain in Winter |
| **Sheep** (name: TBD) | Brush when fluffy | Comb icon above head | Wool bundle (stackable) | Wool grows faster in Summer; thickens in Autumn; sleeps in Winter |
| **Cow** (name: TBD) | Feed hay | Hay bale icon above head | Milk bottle (stackable) | Produces milk daily year-round (as in similar farming games); grazes in pasture in Spring/Summer, gets extra hay in Autumn/Winter |
| **Beehive** | Plant flowers nearby | Flower petal icon above head | Honey jar (stackable) | Active Spring, Summer, and Autumn (bees forage across the warm seasons); sleeps in Winter |

---

## 3. Care Meter (Tamagotchi-Inspired, Gentle)

Each animal has **one** visible heart-shaped meter:

- **Full heart:** Animal is happy; produces its item (egg, wool, honey)
- **Half heart:** Animal slows production; a picture icon appears above its head
- **Empty heart:** Animal naps under a tree / in its coop. No production. No sadness. No running away.

**Refilling:** One feed (grain or hay), brush, or flower visit restores the heart fully.

**Neglect rule:** If the game is closed for a week, the animal waits exactly where it was. The heart does not decay in real time. It only empties gently during play sessions if ignored. When the child returns, Mr. Marlow says:

> *"Oh, your chicken is napping. She's waiting for a snack. Would you like to feed her?"*

---

## 4. Visual Design for Pre-Readers

| UI element | Implementation |
|---|---|
| **Hunger icon** | Large corn kernel floating above the chicken (animated bounce) |
| **Brush icon** | Comb icon above the sheep |
| **Flower icon** | Petal icon above the beehive |
| **Hay icon** | Hay bale icon above the cow |
| **Care meter** | Heart-shaped bar, no numbers — full / half / empty colors |
| **Production ready** | Egg glows in nest; wool looks fluffy; honey drips from hive; milk bottle full by the cow |
| **Marlow prompt** | Picture bubble: animal icon + need icon + question mark |

All icons are clickable/tappable. The parent reads: *"The chicken is showing a corn picture. That means she wants food."*

---

## 5. Integration with Collectathon Inventory

| Animal product | Inventory slot | Collection book entry | Use |
|---|---|---|---|
| Egg | Stackable (egg with number badge) | "Egg — collected 0/50" | Gift to Marlow; decoration |
| Wool | Stackable (wool bundle) | "Wool — collected 0/20" | Trade for decorations; gift |
| Honey | Stackable (honey jar) | "Honey — collected 0/10" | Gift; winter feed for bees |
| Milk | Stackable (milk bottle) | "Milk — collected 0/30" | Gift; kitchen decoration |

If a stack reaches its cap, new items go to the Overflow Nest at Marlow's shed (see inventory.md).

---

## 6. Ambient and Collectible Creatures

Alongside the four care animals, the lane is alive with small creatures (mirroring similar games, where butterflies and fireflies are ambient collectibles rather than cared-for animals):

| Creature | Where | How it appears | Interaction |
|---|---|---|---|
| **Butterflies** | Garden, lane, meadow | Visit grown flowers in Spring/Summer (grown blooms persist, so spring flowers still draw them in summer) | Catch with the butterfly net; show to Marlow or release in the garden |
| **Fireflies** | Over the brook and garden | Glow at night in season (Summer) | Catch with the butterfly net; release again |
| **Frog** | Brook and footbridge | Sits on flat stones by day | Ambient only — observe, no capture |

Caught creatures fill the **Caught Creatures** inventory category (catch and release only — they are never harmed, kept, or sold).

**Deferred (not in the current scope):** pets (dogs, cats) and livestock beyond the initial four (goats, pigs, horses); beasts of burden or working animals. These may be revisited in a later phase without changing the care rules above.

---

## 7. Explicitly Excluded

- Animals beyond the initial four (goats, pigs, horses, and so on)
- Beasts of burden or working animals
- Animal death, running away, or permanent unhappiness
- Real-time hunger decay while game is closed
- Animal products sold for currency
- Barns, coops, or buildings to maintain
- Feeding schedules more frequent than once per play session

---

## Related documents

| Document | Relationship |
|---|---|
| [README.md](../README.md) | Centralized hub for the project. |
| [GDD.md](./GDD.md) | Pillar 8 (gentle animal care) defines the no-death, no-neglect-penalty approach. |
| [mechanics.md](./mechanics.md) | Feed, brush, and collect verbs implement animal care. |
| [inventory.md](./inventory.md) | Animal products (egg, wool, honey) fill collectathon slots; overflow goes to Marlow's shed. |
| [items.md](./items.md) | Animal products are items shown or given to NPCs. |
| [places.md](./places.md) | Animals live at specific places on Bramblewick Lane. |
| [events.md](./events.md) | Animal care events (needs care, product ready, naps, seasonal behavior) are listed here. |

## 8. Change Log

- **0.1 — 2026-11-18:** Initial draft. Butterfly and firefly as collectibles; ambient wild animals; pets and livestock deferred; no beasts of burden. Day/night and season awareness added.
- **0.2 — 2026-11-18:** Three care animals added (chicken, sheep, beehive) with gentle care meter, no death/neglect penalty, seasonal behavior, and collectathon inventory integration.
- **0.3 — 2026-11-18:** Added the **cow** (fourth care animal, produces milk) and a new §6 covering ambient/collectible creatures (butterflies, firefly, frog), making change log 0.1 truthful. Seasonal behavior aligned with similar farming games: chicken lays spring–autumn and rests in winter; beehive active spring through autumn. Exclusions reworded for the initial four.