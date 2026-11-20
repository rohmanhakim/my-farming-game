# Animals & Fish Pond

> **Status:** Design draft — **updated for the direction reset 2026-11-19**
> **Version:** 0.7
> **Last updated:** 2026-11-20
> **Purpose:** Farm animals, the fish pond, and the lane's wild creatures. Care is real responsibility: well-cared animals give better products; neglected animals are gone. Kid-friendly presentation — no sickness, no vet, no on-screen death.

---

## 1. Scope and Design Intent

- **Four farm species** (chicken, sheep, cow, beehive) plus **pond fish**. **The limit is the housing** (owner decision 2026-11-19): a barn with 5 stalls holds 5 cows/sheep, a small coop holds 5 hens, a 4 × 4 pond holds 10 fish (buildings.md §3.1). More animals need bigger buildings, which need money and cleared land.
- **One care need per species**, shown as a picture icon. Simple to learn, real enough to matter.
- **Realistic consequence, kid-friendly presentation** (GDD pillar 2): if an animal goes **3 days in a row without food**, it is **gone right away**. A short message card appears with a **picture of a small gravestone** and the animal's name. Nothing is shown dying; there is no sickness or vet system.
- **Nothing happens while the game is closed.** Days only pass during play.
- **Automation is the long-term answer:** auto-feeders (fed from the silo) and helpers keep meters full on a big farm.

---

## 2. Animal Roster

| Animal | Care need | Need icon | Produces | Housing | Season behaviour |
|---|---|---|---|---|---|
| **Chicken** | Feed grain | Corn kernel | Egg | Coop | Lays Spring–Autumn; rests in Winter (no eggs) but still eats — needs more grain in Winter |
| **Sheep** | Feed hay + brush when fluffy | Hay bale / comb | Wool | Barn | Wool grows faster in Summer, thickens in Autumn |
| **Cow** | Feed hay | Hay bale | Milk | Barn | 4 milk cans (≈ 20 L) daily year-round; grazes in pasture Spring/Summer |
| **Beehive** | Flowers nearby | Flower petal | Honey | Hive / apiary | Active Spring–Autumn; sleeps in Winter (no loss while asleep) |
| **Pond fish** | Feed fish food | Fish-food flake | Fish | Fish pond | Grow faster in Summer; slow in Winter; some species only stock in certain seasons |

Breeds (speckled hen, black sheep, highland cow, rare bee strains, rare fish) are a long-term collection (progression.md §6.2).

---

## 3. Daily Care and the Loss Rule

Animals need **food once per in-game day** (the same daily rhythm as watering crops). Each animal (and each pond, as a group) shows a heart meter with no numbers.

| Unfed days in a row | What the player sees | Effect |
|---|---|---|
| **0** (fed today) | Happy animal, full heart | Produces; quality can rise |
| **1** | Need icon above the animal; heart ⅔ | No product tomorrow |
| **2** | Icon flashes, heart ⅓; Ada or Marlow mentions it | Warning |
| **3** | — | **The animal is gone.** Message card: gravestone picture + "*Bramble the hen is gone. Hens need food every day.*" plus a tip ("An auto-feeder can help"). |

- **Feeding resets the count to 0.** Brushing (sheep) and flowers nearby (bees) raise quality but are not survival needs.
- **Beehives** need no feeding; with no flowers within 5 tiles for 3 days in a warm season the colony leaves (empty-hive picture instead of a gravestone). Asleep in Winter: no loss.
- **Fish:** one feed per day feeds the whole pond; 3 unfed days → the pond's fish are gone (one card). Overfeeding is not punished.
- **After a loss:** nothing else is taken away; the building stays and a new animal can be bought.
- The same **3-days-in-a-row** count is used for crops (crops.md §2) so there is one rule to learn.

---

## 4. Quality Products

- Products come in **1–3 stars**. Quality rises with **consistent care** (no missed feeding days), brushing, good feed (silo hay vs. bought), and nearby flowers for bees.
- Higher quality sells for more, scores better in contests, and some orders ask for it ("a 3-star cheese for the festival").
- Quality never drops below 1 star; neglect only stops it rising — the real cost of neglect is the loss rule.

---

## 5. Visual Design

| UI element | Implementation |
|---|---|
| **Need icons** | Corn kernel, hay bale, comb, petal, fish-food flake — bounce above the animal/pond |
| **Care meter** | Heart bar (full / ⅔ / ⅓), no numbers; also in the care menu (controls.md §2) |
| **Product ready** | Egg glows in nest; wool looks fluffy; honey drips; milk pail full; fish jump in the pond |
| **Loss card** | Small, gentle gravestone illustration (or empty hive / still pond), animal name, one-line tip; dismissed with one button |

---

## 6. Fish Pond

- **Build:** the farm's pond is made of **water tiles** (buildings.md §3.1). The natural 4 × 4 pond in Field E is free once cleared (T3); Kit grows it, or digs new ponds linked to it by channels, tile by tile. Touching water merges into one pond.
- **Stock:** buy fingerlings from the travelling merchant or the shop, or release fish caught at the brook or Mill Pond.
- **Capacity:** about 2 fish per 3 tiles of water at least 2 tiles wide (4 × 4 = 10, 6 × 6 = 24, 8 × 8 = 42); 1-wide channels hold no fish.
- **Feed:** fish food (bought, or crafted from crops) — once a day for the whole connected pond.
- **Harvest:** when fish are grown, collect them with a net or fishing rod; the pond restocks from remaining fish over time.
- **Use:** sell, deliver in orders (a fishmonger neighbour, festival catering), or process (smoked fish, fish pie).
- **Output:** ≈ 2 fish per day from a full 4 × 4 pond (economy.md §4).
- **Automation:** a **pond auto-feeder** feeds it daily from its own hopper (≈ 30 days of fish food, filled straight from phone orders); a helper can harvest.
- **Limits:** money for water tiles and stock early; cleared land later (every water tile uses a farm tile).
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
- Anything advancing while the game is closed.
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
| [challenge-layers.md](./challenge-layers.md) | The single mode's 3-days rule. |
| [economy.md](./economy.md) | Animal, feed, and product prices; paybacks. |
| [progression.md](./progression.md) | Breeds, quality stickers, contests. |
| [events.md](./events.md) | Care, product, loss, and seasonal events. |

## 10. Change Log

- **0.1 — 2026-11-18:** Initial draft. Butterfly and firefly as collectibles; ambient wild animals; pets and livestock deferred; no beasts of burden. Day/night and season awareness added.
- **0.2 — 2026-11-18:** Three care animals added (chicken, sheep, beehive) with gentle care meter, no death/neglect penalty, seasonal behavior, and collectathon inventory integration.
- **0.3 — 2026-11-18:** Added the **cow** (fourth care animal, produces milk) and a new §6 covering ambient/collectible creatures (butterflies, firefly, frog), making change log 0.1 truthful. Seasonal behavior aligned with similar farming games: chicken lays spring–autumn and rests in winter; beehive active spring through autumn. Exclusions reworded for the initial four.
- **0.4 — 2026-11-19:** **Direction reset (GDD v0.19).** Nap state removed: an animal whose meter empties is **gone right away**, shown only as a short message card with a **gravestone picture** (empty hive for bees); no sickness or vet. 4-animal cap removed — numbers limited by money and housing/land. Sheep now also eat hay. Added **quality products (1–3 stars)**, **§6 Fish Pond** (stock, feed, harvest; pond auto-feeder; money then land limit), brook fishing, breeds as a collection, and automation (auto-feeders, helpers) as the long-term answer to neglect. Products now sold and used in orders and processing.
- **0.5 — 2026-11-19:** **Owner decisions:** animal numbers capped by **housing capacity** (5-stall barn = 5 animals; coop 5/10 hens; pond 10/25 fish). Meter-drain-by-mode replaced by the single-mode **daily feeding rule: 3 unfed days in a row → gone** (warnings on days 1–2), matching the crops' 3-dry-days rule. Pond capacity, output, and land footprint added; economy.md linked.
- **0.6 — 2026-11-20:** §6 Fish Pond rewritten for **water tiles** (buildings.md §3.1): free natural pond in Field E, ponds grow tile by tile and merge, ≈ 2 fish per 3 wide tiles, one feed per connected pond; fish can also come from Mill Pond (places.md v0.10).
- **0.7 — 2026-11-20:** Pond auto-feeder has its own 30-day hopper filled by phone orders (owner decision: machines are set-and-forget).
