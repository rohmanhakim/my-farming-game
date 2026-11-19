# Difficulty Modes (formerly Challenge Layers & Parent Controls)

> **Status:** Design draft — **restructured 2026-11-19**
> **Version:** 0.3
> **Last updated:** 2026-11-19
> **Purpose:** Defines the three difficulty modes — **Easy, Normal, Realistic** — that replace the old *Gentle Core + opt-in challenge layers + five-stage map*. The file name is kept so existing links still work.

## 1. Design Philosophy

- **One world, one progression, three tunings.** Every mode has the same crops, animals, buildings, orders, automation, and endgame. Modes only change *numbers*: how long neglect takes, how much money things cost and pay, and how much prices swing.
- **Challenge comes from the farm, not from toggles.** As in *Harvest Moon*, the game gets harder because the farm gets bigger (more beds to water, more animals to feed, bigger orders) — and easier again as automation arrives (progression.md). Modes set how steep that curve feels.
- **Consequences exist in every mode.** Crops yellow then are lost; neglected animals and fish are gone (GDD §4). Easy gives more time and more warning; it never turns consequences off.
- **No parent gate.** The mode lives in normal Settings. There is no grown-up menu, no parent-only toggle, and no stage approval.

## 2. Why three modes

| Considered | Verdict |
|---|---|
| Keep six independent layers | Rejected — layers like *Garden Coins* or *Crafting* are now core systems, so toggling them off would break orders, automation, and progression. A matrix of toggles is also hard to balance and explain. |
| One fixed difficulty | Rejected — the owner wants the game to stay kid-friendly *and* offer a realistic farm sim. |
| Two modes (Relaxed / Realistic) | Possible, but leaves no tuned "intended" middle for most players. |
| **Three modes: Easy / Normal / Realistic** | **Recommended.** One per audience the owner named (kid-friendly, standard, realistic); each explainable in one sentence; one tuning table to balance. |

## 3. Mode Tuning Table (provisional numbers — tune in playtests)

Times are in **play time** (the game is running, not paused, not closed) unless marked in-game days.

| Setting | Easy | **Normal** (default) | Realistic |
|---|---|---|---|
| **Crop: soil stays moist after watering** | 1 in-game day | 1 in-game day | 1 in-game day |
| **Crop: dry → yellow (growth paused before this)** | 2 in-game days dry | 1 in-game day dry | ½ in-game day dry |
| **Crop: yellow → gone** | 2 more in-game days | 1 more in-game day | ½ more in-game day |
| **Animal/fish care meter: full → empty** | ≈ 3 in-game days without care | ≈ 2 in-game days | ≈ 1 in-game day |
| **Warning icon shows at** | ½ meter | ⅓ meter | ¼ meter |
| **Starting money** | High | Medium | Low |
| **Order pay vs. market price** | +50 % | +25 % | +10 % |
| **Seasonal / demand price swing** | Small (±10 %) | Medium (±25 %) | Large (±50 %) |
| **Shop prices (seeds, animals, buildings, land)** | −25 % | base | +25 % |
| **Order-board refresh** | Many small orders | Mixed | Fewer, bigger orders |
| **Break reminder** | On (default) | Off (default) | Off (default) |
| **Loss message** | Gravestone card + "what could help" tip (e.g. "A feeder keeps animals fed") | Gravestone card + tip | Gravestone card only |

- An in-game day ≈ 24 minutes of play (GDD §4 clock). Neglect timers count only while the game runs; nothing advances while it is closed.
- Growth speed (Fast/Medium/Slow, GDD §4) stays a **separate** pre-game setting.
- **⚠️ Open:** these grace times are first guesses. They must be re-checked once growth stage length (≈30–45 s) and day length (≈24 min) are playtested together — a crop can currently grow from seed to harvest well inside one in-game day, so day-based neglect may be too forgiving (GDD §16 Q11).

## 4. Switching modes

- Chosen on the **new-game screen** (default Normal).
- Changeable **any time in Settings**; the change applies from that moment (timers already running are rescaled, not reset).
- **⚠️ Open (GDD §16 Q10):** whether moving *up* to Realistic mid-game should be allowed, or only moving down.

## 5. What happened to the old layers

| Old layer | Now |
|---|---|
| Words vs. Pictures | Retired — text with icons is the standard (GDD §2). |
| Garden Coins | **Core system** — money from orders and other sources (GDD §4). |
| Tool Upgrades | **Core progression** (progression.md §3). |
| Crafting | **Core system** — processing, blueprints, automation (mechanics.md). |
| Multi-step Requests | **Core** — orders and bigger orders. |
| Session Timer | Optional **break reminder** in Settings (on by default in Easy). |
| Five-stage map (preschool → 5th grade+) | Replaced by **farm growth tiers** (progression.md §3), driven by the farm, not by age. |
| One-click reset / parent gate | Removed — a mode switch in Settings replaces both. |

## 6. Related documents

| Document | Relationship |
|---|---|
| [README.md](../README.md) | Centralized hub for the project. |
| [GDD.md](./GDD.md) | §3 *Difficulty* and §4 *Neglect and consequences* are the source of these modes. |
| [progression.md](./progression.md) | Farm growth tiers — how challenge rises naturally with the farm. |
| [mechanics.md](./mechanics.md) | Systems whose numbers the modes tune (growth, neglect, prices). |
| [crops.md](./crops.md) | Yellow → gone crop rule. |
| [animals.md](./animals.md) | Care meters and the loss rule for animals and fish. |
| [buildings.md](./buildings.md) | Building and land costs scaled by mode. |

## 7. Change log

- **0.1 — 2026-11-18:** Initial draft. Defines challenge layers, stage map, one-click reset, and parent gate.
- **0.2 — 2026-11-18:** Removed the Time Awareness layer (day/night and seasons are always on, Gentle Core). Session Timer clarified as on by default. Fixed "pase menu" typo, numbered the change log, added the README row, and normalised "neighbor" to "neighbour".
- **0.3 — 2026-11-19:** **Restructured into three difficulty modes (Easy / Normal / Realistic)** per the owner's direction reset (GDD v0.19). Gentle Core, the six layers, the five-stage age map, the parent gate, and the one-click reset are retired; coins, upgrades, crafting, and multi-step requests become core systems; the session timer becomes an optional break reminder. Added the reasoning for three modes (§2), a provisional tuning table for neglect, money, and prices (§3), mode-switching rules (§4), and a mapping from each old layer to its new home (§5). File name kept for link stability.
