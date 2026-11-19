# Animals & Fish Pond

> **Status:** Design draft — **updated for the direction reset 2026-11-19**
> **Version:** 0.4
> **Last updated:** 2026-11-19
> **Purpose:** Farm animals, the fish pond, and the lane's wild creatures. Care is real responsibility: well-cared animals give better products; neglected animals are gone. Kid-friendly presentation — no sickness, no vet, no on-screen death.

---

## 1. Scope and Design Intent

- **Four farm species** (chicken, sheep, cow, beehive) plus **pond fish**. There is **no fixed cap** on how many — the number is limited by money (buying animals and housing) and later by land (coop, barn, pond footprints; buildings.md).
- **One care need per species**, shown as a picture icon. Simple to learn, real enough to matter.
- **Realistic consequence, kid-friendly presentation** (GDD pillar 2): if an animal's care meter empties, it is **gone right away**. A short message card appears with a **picture of a small gravestone** and the animal's name. Nothing is shown dying; there is no sickness or vet system.
- **Nothing happens while the game is closed.** Meters drain only during play.
- **Automation is the long-term answer:** auto-feeders (fed from the silo) and helpers keep meters full on a big farm.

---

## 2. Animal Roster

| Animal | Care need | Need icon | Produces | Housing | Season behaviour |
|---|---|---|---|---|---|
| **Chicken** | Feed grain | Corn kernel | Egg | Coop | Lays Spring–Autumn; rests in Winter (no eggs) but still eats — needs more grain in Winter |
| **Sheep** | Feed hay + brush when fluffy | Hay bale / comb | Wool | Barn | Wool grows faster in Summer, thickens in Autumn |
| **Cow** | Feed hay | Hay bale | Milk | Barn | Milk daily year-round; grazes in pasture Spring/Summer |
| **Beehive** | Flowers nearby | Flower petal | Honey | Hive / apiary | Active Spring–Autumn; sleeps in Winter (no loss while asleep) |
| **Pond fish** | Feed fish food | Fish-food flake | Fish | Fish pond | Grow faster in Summer; slow in Winter; some species only stock in certain seasons |

Breeds (speckled hen, black sheep, highland cow, rare bee strains, rare fish) are a long-term collection (progression.md §6.2).

---

## 3. Care Meter and Loss Rule

Each animal (and each pond, as a group) has **one** heart-shaped meter, no numbers.

| Meter | What the player sees | Effect |
|---|---|---|
| **Full** | Happy animal | Produces; quality rises over time |
| **Low** (threshold by mode) | Need icon bounces above the animal; Marlow or Ada may mention it | Production slows; quality stops rising |
| **Empty** | — | **The animal is gone.** Message card: gravestone picture + "*Bramble the hen is gone. Hens need grain every day.*" (tip shown in Easy and Normal). |

- **Refilling:** one feed / brush / flower visit restores the meter fully.
- **Drain rate** depends on difficulty mode (challenge-layers.md §3: ≈3 / 2 / 1 in-game days from full to empty).
- **Beehives** do not have a loss state in Winter while asleep; in other seasons, no flowers nearby → the hive leaves (same message card, a picture of an empty hive instead of a gravestone).
- **Fish:** the pond meter covers all its fish. Empty → the pond's fish are gone (one message card for the pond). Overfeeding is not punished (no added complexity).
- **After a loss:** nothing else is taken away; the housing stays, and a new animal can be bought.

---

## 4. Quality Products

- Products come in **1–3 stars**. Quality rises with **consistent care** (meter kept above low), brushing, good feed (silo hay vs. bought), and nearby flowers for bees.
- Higher quality sells for more, scores better in contests, and some orders ask for it ("a 3-star cheese for the festival").
- Quality never drops below 1 star; neglect only stops it rising — the real cost of neglect is the loss rule.

---

## 5. Visual Design

| UI element | Implementation |
|---|---|
| **Need icons** | Corn kernel, hay bale, comb, petal, fish-food flake — bounce above the animal/pond |
| **Care meter** | Heart bar, no numbers; also visible in the care menu (controls.md §2) |
| **Product ready** | Egg glows in nest; wool looks fluffy; honey drips; milk pail full; fish jump in the pond |
| **Loss card** | Small, gentle gravestone illustration (or empty hive / still pond), animal name, one-line tip; dismissed with one button |

---

## 6. Fish Pond

- **Build:** a fish pond structure (buildings.md; footprint 4, bigger ponds 6/9) from T3.
- **Stock:** buy fingerlings from the travelling merchant or the shop, or release fish caught at the brook.
- **Feed:** fish food (bought, or crafted from crops) — one feed fills the pond meter.
- **Harvest:** when fish are grown, collect them with a net or fishing rod; the pond restocks from remaining fish over time.
- **Use:** sell, deliver in orders (a fishmonger neighbour, festival catering), or process (smoked fish, fish pie).
- **Automation:** a **pond auto-feeder** keeps the meter full; a helper can harvest.
- **Limits:** money for the pond and stock early; land (pond footprint) later.
- **Education:** water ecosystems, how fish farms work, feeding regularly.

---

## 7. Integration with Inventory and Orders

| Product | Inventory slot | Uses |
|---|---|---|
| Egg | Animal Products (stacks by quality) | Orders, sell, baking |
| Wool | Animal Products | Orders, sell, crafting (decorations) |
| Honey | Animal Products | Orders, sell, jam/pie recipes |
| Milk | Animal Products | Orders, sell, cheese, baking |
| Fish | Animal Products | Orders, sell, smoked fish, fish pie |

---

## 8. Wild Creatures (not farm animals)

| Creature | Where | Interaction |
|---|---|---|
| **Butterflies** | Garden, lane, meadow (Spring/Summer) | Catch with the butterfly net; show to Marlow or release |
| **Fireflies** | Brook and garden at night (Summer) | Catch and release |
| **Frog** | Brook | Ambient only |
| **Brook fish** | Brook (fishing) | Catch with a rod; keep, sell, or release into Kit's pond |

Butterflies and fireflies are always released — never kept or sold.

---

## 9. Explicitly Excluded

- Sickness, injury, vet visits, medicine.
- On-screen death or any graphic depiction of loss.
- Needs or meters draining while the game is closed.
- Breeding/genetics simulation (breeds are bought or won, not bred) — may be revisited later.
- Pets (dogs, cats) and working animals — deferred, not ruled out.

---

## Related documents

| Document | Relationship |
|---|---|
| [README.md](../README.md) | Centralized hub for the project. |
| [GDD.md](./GDD.md) | Pillar 2 (realistic but kid-friendly) and §4 neglect rules. |
| [buildings.md](./buildings.md) | Coop, barn, apiary, fish pond, silo, auto-feeders. |
| [mechanics.md](./mechanics.md) | Feed, brush, collect, fish, stock verbs. |
| [inventory.md](./inventory.md) | Animal products by quality. |
| [challenge-layers.md](./challenge-layers.md) | Drain rates and warning thresholds per mode. |
| [progression.md](./progression.md) | Breeds, quality stickers, contests. |
| [events.md](./events.md) | Care, product, loss, and seasonal events. |

## 10. Change Log

- **0.1 — 2026-11-18:** Initial draft. Butterfly and firefly as collectibles; ambient wild animals; pets and livestock deferred; no beasts of burden. Day/night and season awareness added.
- **0.2 — 2026-11-18:** Three care animals added (chicken, sheep, beehive) with gentle care meter, no death/neglect penalty, seasonal behavior, and collectathon inventory integration.
- **0.3 — 2026-11-18:** Added the **cow** (fourth care animal, produces milk) and a new §6 covering ambient/collectible creatures (butterflies, firefly, frog), making change log 0.1 truthful. Seasonal behavior aligned with similar farming games: chicken lays spring–autumn and rests in winter; beehive active spring through autumn. Exclusions reworded for the initial four.
- **0.4 — 2026-11-19:** **Direction reset (GDD v0.19).** Nap state removed: an animal whose meter empties is **gone right away**, shown only as a short message card with a **gravestone picture** (empty hive for bees); no sickness or vet. 4-animal cap removed — numbers limited by money and housing/land. Sheep now also eat hay. Added **quality products (1–3 stars)**, **§6 Fish Pond** (stock, feed, harvest; pond auto-feeder; money then land limit), brook fishing, breeds as a collection, and automation (auto-feeders, helpers) as the long-term answer to neglect. Products now sold and used in orders and processing.
