# Crops

> **Status:** Brainstorming draft
> **Version:** 0.4
> **Last updated:** 2026-11-18
> **Purpose:** Crops available in the game, adjusted to the scope of the game described in the GDD. All names are tentative and can be changed. Every element here is a proposal pending playtesting.

---

## 1. Scope and design intent

The crop list is intentionally tiny. There is no economy, no sell pressure, and no crop death. Plants exist to give the child visible, cheerful progress and to supply the Butterfly Garden task and seasonal decoration.

- **No withering:** Plants never die, even in winter or if unwatered.
- **Blooms persist after their season:** A grown plant keeps blooming into later seasons — it only needs its own season to *sprout*. A flower grown in spring still draws butterflies in summer (mirroring how similar games treat seasonal butterflies).
- **No inventory backlog:** Harvests go straight to the garden shelf.
- **Season-aware:** Each crop grows in one or more seasons. Out-of-season planting simply does not sprout — no penalty.

---

## 2. Growth model (summary)

- Plants advance through visible growth stages in-session, on **accumulated play time** (no in-game-day requirement).
- **Stage timing (GDD §4 decision, Milestone 0):** default **Medium ≈ 30–45 s per stage** — seed → sprout → bloom in ≈ 2 minutes of play. **Fast ≈ 15–20 s** and **Slow ≈ 60–90 s** presets exist for the options screen.
- Watering is optional and speeds growth with sparkle/sound feedback.
- Plants grow without watering; watering never feels obligatory.
- **Growth while the game is closed:** **paused by default** — nothing grows while the game is off. The optional **Gentle catch-up** setting adds at most one stage when the game is next opened and never pushes a plant past bloom (positive-only; no penalty either way).
- Both settings are chosen on the options screen **before starting a new game** and are not changed mid-game.
- Initial garden size: two or three beds.

---

## 3. Starting loadout seeds

The player begins with a seed box holding two or three basic seeds (Sun Sprout, Clover Vine, Moon Petal, Carrot Patch — two or three of these). These are available from the start and grow in the starting season.

| Tentative name | Season | Growth speed | Use | Notes |
|---|---|---|---|---|
| **Sun Sprout** | Spring / Summer | Fast | Decoration, shelf | Bright yellow bloom; good first plant. |
| **Clover Vine** | Spring / Autumn | Medium | Decoration, shelf, bee food | Trailing green vine; soft texture. Plant near the beehive to feed the bees. |
| **Moon Petal** | Summer / Autumn | Medium | Decoration, shelf, gift | Pale bloom; glows faintly at night. |
| **Carrot Patch** | Spring / Summer | Fast | Shelf, decoration | Familiar first vegetable; kids recognize it instantly. |

---

## 4. Butterfly Garden flowers

These are the three flowers for Mr. Marlow's first task. They are supplied by the NPC when the task is accepted and do not come from the starting seed box.

| Tentative name | Season | Role | Notes |
|---|---|---|---|
| **Meadow Bell** | Spring | Butterfly Garden task | Attracts butterflies when grown. Supplied by Marlow. |
| **Honey Cup** | Spring | Butterfly Garden task | Attracts butterflies when grown. Supplied by Marlow. |
| **Lace Flower** | Spring | Butterfly Garden task | Attracts butterflies when grown. Supplied by Marlow. |

---

## 5. Seasonal crops (tentative, later unlocks)

Additional crops may be unlocked later through play. All are tentative names.

| Tentative name | Season | Growth speed | Use | Unlock |
|---|---|---|---|---|
| **Berry Spray** | Summer | Slow | Shelf, decoration, bee food | Later reward or task. Plant near the beehive to feed the bees. |
| **Tomato Vine** | Summer | Medium | Shelf, decoration | Supports the trellis/stick tool unlock. |
| **Pumpkin Head** | Autumn | Slow | Shelf, decoration | Later reward or task. |
| **Frost Leaf** | Winter | Medium | Shelf, decoration | Indoor/winter crop candidate. |
| **Herb Pot** | Winter | Medium | Shelf, gift | Indoor windowsill crop; keeps gardening alive in cold months. |
| **Rain Drop** | Spring / Autumn | Medium | Shelf, gift | Later reward or task. |

Winter has few or no outdoor crops, consistent with the lane resting. Indoor or shelf-based winter activities remain available.

---

## 6. Crop uses

| Use | Description |
|---|---|
| **Decoration** | Placed in the front garden for creative ownership. |
| **Shelf** | Sent automatically to the garden shelf; viewable but not managed. |
| **Gift** | Shown or given to an NPC as part of a social task (optional). |

---

## 7. Unlock conditions

- **Starting seeds:** Available at start of game.
- **Butterfly Garden flowers:** Supplied by Mr. Marlow when the player accepts the first task.
- **Later seasonal crops:** Unlocked through tasks, relationship milestones, or exploration (deferred from prototype).

---

## Related documents

| Document | Relationship |
|---|---|
| [README.md](../README.md) | Centralized hub for the project. |
| [GDD.md](./GDD.md) | No-withering, no-economy, small-garden guardrails come from the GDD scope. |
| [mechanics.md](./mechanics.md) | Plant, water, and harvest verbs drive the crop growth model. |
| [items.md](./items.md) | Grown crops route to the garden shelf as items. |
| [inventory.md](./inventory.md) | Harvested crops fill collectathon inventory slots. |
| [world.md](./world.md) | Butterfly Garden flowers are the first story task on Bramblewick Lane. |
| [events.md](./events.md) | Spring bloom day and seasonal crop events are listed here. |

## 8. Change log

- **0.1 — 2026-11-18:** Initial draft. Small seasonal crop list with no withering, no economy, and automatic shelf routing. Butterfly Garden flowers supplied by NPC task.
- **0.2 — 2026-11-18:** Added Carrot Patch to starting seeds; added Tomato Vine, Herb Pot. Clover Vine and Berry Spray noted as bee food. Winter indoor crops (Frost Leaf, Herb Pot) keep gardening alive in cold months.
- **0.3 — 2026-11-18:** Reconciled tables with change log 0.2 (Carrot Patch, Tomato Vine, Herb Pot now appear in the tables; bee-food notes added to Clover Vine and Berry Spray). Added the season-persistence rule: grown plants keep blooming after their season, so butterfly visits in spring and summer are consistent.
- **0.4 — 2026-11-18:** Growth model §2 updated to the Milestone 0 decision (GDD §4, v0.16): stage timing default **Medium ≈30–45 s** (Fast/Slow presets for the options screen), growth paused while the game is closed by default, optional positive-only Gentle catch-up (+1 stage max, never past bloom), both set before starting a new game. Replaced the old “closed-game growth is TBD” line.