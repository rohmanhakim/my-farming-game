# Difficulty — One Mode: Realistic but Kid-Friendly

> **Status:** Design draft — **single mode decided 2026-11-19**
> **Version:** 0.7
> **Last updated:** 2026-11-20
> **Purpose:** Defines the game's one difficulty setting and its rules. Earlier versions held parent-controlled challenge layers (v0.1–0.2) and then three modes (v0.3); both are retired. The file name is kept so existing links still work.

## 1. Decision

**There is one mode: realistic but kid-friendly** (owner decision 2026-11-19). No Easy/Normal/Realistic choice, no difficulty menu, no mid-game switching.

- **Realistic:** crops need daily water; animals need daily food; real-world sell prices; neglect has consequences.
- **Kid-friendly:** generous warnings, half-price buying (economy.md), no debt, no sickness or vet, no on-screen death, nothing changes while the game is closed.
- **Challenge comes from the farm, not from a setting.** As in *Harvest Moon*, the farm gets harder because it gets bigger (more crops to water, more animals to feed, more land to clear) and easier again as automation arrives (progression.md).

## 2. The rules (one table)

| Rule | Value | Notes |
|---|---|---|
| **Crop needs water** | Once per in-game day | A watered crop grows one day overnight; rain waters outdoor crops. |
| **Crop: dry day 1** | No growth | Dry-soil look. |
| **Crop: 2 dry days in a row** | **Turns yellow** (warning) | Watering recovers it. |
| **Crop: 3 dry days in a row** | **Gone** | Bed is empty again; short note with a tip. |
| **Animal / fish needs food** | Once per in-game day | Auto-feeders do this later. |
| **Animal: 1 unfed day** | Need icon appears | — |
| **Animal: 2 unfed days in a row** | Meter low, icon flashes; Ada/Marlow mention it | Warning. |
| **Animal: 3 unfed days in a row** | **Gone right away** | Message card with a gravestone picture and a tip. |
| **Sell prices** | ≈ real-world retail | economy.md |
| **Buy prices** | ≈ half of real-world retail | economy.md — the kid-friendly half |
| **Money floor** | 0 | No debt, no loans. |
| **While the game is closed** | Nothing changes | Days only pass during play (sleep or the 21:30 bedtime). |

Days are in-game days (GDD §4 clock). The crop and animal rules use the same "3 days in a row" count on purpose, so it is one rule for players to learn.

## 3. History

| Version | Structure | Why it changed |
|---|---|---|
| 0.1–0.2 | Gentle Core + six parent-toggled challenge layers + five age stages | Audience changed (Kit 17, kid-friendly but not pre-elementary). |
| 0.3 | Three modes: Easy / Normal / Realistic | Owner chose a single mode: realistic but kid-friendly. |
| **0.4** | **One mode** | Current. |

The old layers' content is all core now: money, tool upgrades, crafting, multi-step orders. The old session timer survives only as an optional **break reminder** in Settings.

## 4. Related documents

| Document | Relationship |
|---|---|
| [README.md](../README.md) | Centralized hub for the project. |
| [GDD.md](./GDD.md) | §3 pillars and §4 neglect rules. |
| [economy.md](./economy.md) | Real sell prices, half-price buying. |
| [progression.md](./progression.md) | How challenge rises with the farm. |
| [crops.md](./crops.md) | Day-based growth and the 3-dry-days rule. |
| [animals.md](./animals.md) | Daily feeding and the 3-unfed-days rule. |

## 5. Change log

- **0.1 — 2026-11-18:** Initial draft. Defines challenge layers, stage map, one-click reset, and parent gate.
- **0.2 — 2026-11-18:** Removed the Time Awareness layer (day/night and seasons are always on, Gentle Core). Session Timer clarified as on by default. Fixed "pase menu" typo, numbered the change log, added the README row, and normalised "neighbor" to "neighbour".
- **0.3 — 2026-11-19:** **Restructured into three difficulty modes (Easy / Normal / Realistic)** per the owner's direction reset (GDD v0.19). Gentle Core, the six layers, the five-stage age map, the parent gate, and the one-click reset are retired; coins, upgrades, crafting, and multi-step requests become core systems; the session timer becomes an optional break reminder. Added the reasoning for three modes (§2), a provisional tuning table for neglect, money, and prices (§3), mode-switching rules (§4), and a mapping from each old layer to its new home (§5). File name kept for link stability.
- **0.4 — 2026-11-19:** **Single mode decided (owner):** "realistic but kid-friendly". Easy/Normal/Realistic and mode switching removed. One rules table: daily watering and feeding, **yellow after 2 dry days, gone after 3** (crops), **gone after 3 unfed days** (animals), sell at ≈ real price, buy at ≈ half real price, no debt, nothing while closed.
- **0.5 — 2026-11-20:** Bedtime reference updated to 01:00 (GDD v0.29).
- **0.6 — 2026-11-20:** Bedtime reference corrected to midnight (GDD v0.30).
- **0.7 — 2026-11-20:** Bedtime reference updated to 21:30 (GDD v0.31).
