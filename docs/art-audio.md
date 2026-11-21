# Art & Audio Brief

> **Status:** First draft for owner review — new document
> **Version:** 0.4
> **Last updated:** 2026-11-21
> **Purpose:** The look and sound of the game, with reference games the owner named (Harvest Moon, Story of Seasons, Stardew Valley). References are for **mood, proportion, and readability only** — never copy characters, assets, melodies, or exact styles (GDD §9). The GDD remains authoritative.

---

## 1. Owner direction (2026-11-19)

- Visual feel of **Harvest Moon / Story of Seasons / Stardew Valley**, but **not too chibi**.
- **Primary art references (owner pick 2026-11-19): _Story of Seasons: Pioneers of Olive Town_ and _Harvest Moon: Home Sweet Home_.** The other games below are secondary.
- Music **like Harvest Moon**: fun, cheerful, seasonal.
- ~~Still: fully 2D, illustrated (non-pixel), angled top-down camera, European-comics-flavoured line work.~~ **Replaced 2026-11-21 (owner): 3D, cartoony and colourful, fixed angled top-down camera, turned diagonally (corner view)** — mood like *Find Your Words* (3D, top-down, cartoony, colourful). See §1b and GDD §9.

## 1b. 3D look and placeholder models (owner decision 2026-11-21)

- **Shapes:** rounded, slightly chunky, simple; big readable silhouettes from a top-down diagonal camera (models are seen from a corner, so two sides of every building show) (tops of heads, hats, roofs and crop leaves matter more than faces at play distance — faces show in dialogue portraits).
- **Shading:** soft toon-style light with flat, saturated colours; one warm sun and a soft sky light. Outline or no outline: open (GDD §16 Q17).
- **Placeholders (Phase-001 Tasks 1.14–1.20):** Godot's built-in primitive meshes with flat colours, defined in one table by name, for example:

| Name | Placeholder | Colour idea |
|---|---|---|
| `kit` | capsule ≈ 1.7 m + small box "nose" showing the facing | red-orange top |
| `marlow`, `ada` | capsules in other colours and heights (Marlow shorter) | green, purple |
| `house`, `coop`, `barn`, `seed_shed` | box walls + prism roof (sloped), door gap | cream walls, red / brown / blue roofs |
| `crop_carrot_<stage>`, `crop_carrot_yellow` | cylinders/cones growing taller by stage; yellow tint when dry | green → orange top |
| `soil_weeds/clear/tilled/wet` | thin tiles in different browns/greens | — |
| `hen`, `cow` | sphere; box + smaller box head | white, black-and-white |
| `berry_bush`, `bed`, `phone`, `ledger`, `order_board`, `feed_bin`, `gate` | boxes / spheres / cylinders | bright accents on usable things |

- **Real models later** swap in by the same name with no code change: glTF (`.glb`) files in `src/assets/models/` (project-structure.md §9). Sources to consider: Kenney CC0 3D packs (e.g. the Nature Kit; check each pack fits the style), models made by the team in Blender, AI-generated 3D (check the licence and clean up topology). Keep one consistent style whatever the source.
- **Budget:** low-poly (Kit ≈ 3–5k triangles, props ≤ 1–2k, a building ≤ 3k), one small texture atlas or plain colours, so the Switch version runs smoothly (project-structure.md §16).

## 2. Visual references (links to official or press pages)

| Game | Look at | Take from it | Avoid |
|---|---|---|---|
| **Story of Seasons: A Wonderful Life** (2023) — [Marvelous page](https://marvelousgames.com/games/story-of-seasons-a-wonderful-life), [artwork gallery](https://www.rpgfan.com/gallery/story-of-seasons-a-wonderful-life-artwork/) | Character art and proportions | Taller, calmer proportions than classic Harvest Moon; warm, soft palette; adults look like adults | Realistic 3D detail (we are cartoony and simpler) |
| ⭐ **Primary — Story of Seasons: Pioneers of Olive Town** — [screenshot gallery](https://www.rpgfan.com/gallery/story-of-seasons-pioneers-of-olive-town-screenshots/), [character news](https://www.rpgsite.net/news/10664-new-story-of-seasons-pioneers-of-olive-town-screenshots-showcase-character-customization-residents-babies-and-more) | Farm layout, buildings, machines | Clear farm buildings; machines that read as "automation"; character customization screen | Very large chibi heads on some characters |
| ⭐ **Primary — Harvest Moon: Home Sweet Home** — [first screenshots](https://gematsu.com/2024/06/harvest-moon-home-sweet-home-first-details-screenshots) | Village and farm mood | Cosy village life, bright UI icons | Chibi proportions |
| **Stardew Valley** — [Steam page (screenshots)](https://store.steampowered.com/app/413150/Stardew_Valley/), [RPGFan gallery](https://www.rpgfan.com/gallery/stardew-valley-screenshots/) | Readability and UI | Every interactive thing reads at a glance; seasonal palette shifts; dialogue portraits with clear expressions | Pixel art (we are illustrated, non-pixel) |

Galleries on third-party sites may change; take screenshots for the team's private mood board only (do not ship or publish them).

## 3. Character proportions — "not too chibi"

| Style | Head-to-body | Use |
|---|---|---|
| Classic Harvest Moon / chibi | 2–3 heads tall | ❌ too chibi |
| **Our target** | **About 5 heads tall for Kit, 5–6 for adults** — close to the two primary references, never 2–3; Marlow slightly shorter, stooped | ✅ friendly, readable, a teen who looks 17 |
| Realistic | 7½–8 heads | ❌ too stiff for a kid-friendly game |

- Slightly larger heads and hands than real life for expression; clear silhouettes; outlines optional in 3D (GDD §16 Q17).
- Faces show feelings at a glance (happy, tired, surprised) — needed for 6+ players.
- Animals are rounded and friendly but recognisably real breeds (educational).
- The **loss card** (gravestone picture) is soft: small stone, flowers, warm light — never dark or scary.

## 4. Palette and world

- Earthy base (greens, browns, stone) with bright accents on crops, doors, animals, and anything you can use (GDD §9).
- Each season has its own tint: Spring fresh green/pink, Summer saturated, Autumn amber/red, Winter blue-white with warm windows.
- Overgrown land looks **wild, not dirty** (world.md §7.5 rule).

## 5. Music direction — "fun, like Harvest Moon"

- **Mood:** cheerful, bouncy, a little silly; easy to hum; never tense.
- **Instruments:** acoustic guitar, ukulele, accordion, tin whistle / recorder, fiddle, glockenspiel, marimba, light hand percussion, plucked bass.
- **Structure:**
  - One **daytime theme per season** (≈ 100–130 BPM), looping 2–3 minutes.
  - **Evening/night** variations: slower, softer, fewer instruments.
  - Short themes for the **village/lane**, **seed shed**, **festivals** (bigger, celebratory), and **rainy days** (gentle).
  - **Jingles:** order delivered (coins), new day, first harvest, tier reached.
  - **Loss card:** a soft, short chime — not a sad drone.
- **Listening references (mood only):** the seasonal themes of *Harvest Moon: Back to Nature* / *Friends of Mineral Town*, *Story of Seasons: A Wonderful Life*, and the spring theme of *Stardew Valley*. Never reuse melodies or arrangements.

## 6. Sound effects

Soft, satisfying, never startling: watering splash, crop pop on harvest, hen cluck, cow moo, coin chime, door creak, footsteps per surface. Every important sound has a matching visual cue (GDD §11).

## 7. Still needed from the owner

1. How 3D models will be made: free CC0 packs (Kenney), Blender by the team, AI-generated 3D, commissioned, or a mix (affects style consistency and licences; GDD §16 Q17).
2. Music source: commissioned composer, royalty-free library, or the team.
3. Kit's look: hair, clothes, colours (he is a 17-year-old boy; appearance stays customizable).

## 8. Related documents

| Document | Relationship |
|---|---|
| [README.md](../README.md) | Centralized hub for the project. |
| [GDD.md](./GDD.md) | §9 Art direction, §10 Audio, §11 Accessibility. |
| [characters.md](./characters.md) | Kit, Ada, Marlow silhouettes. |
| [world.md](./world.md) | Visual character of the lane. |

## 9. Change log

- **0.1 — 2026-11-19:** New document (owner request). Reference games with links (Story of Seasons: A Wonderful Life and Pioneers of Olive Town, Harvest Moon: Home Sweet Home, Stardew Valley) and what to take/avoid from each; "not too chibi" proportion spec (Kit 5½–6 heads); palette; Harvest Moon-style fun music direction (instruments, seasonal themes, jingles); SFX; open inputs.
- **0.2 — 2026-11-19:** Owner picked **Pioneers of Olive Town** and **Harvest Moon: Home Sweet Home** as the primary art references; proportion target moved to ≈ 5 heads (Kit) / 5–6 (adults) to sit close to them while staying non-chibi.
- **0.3 — 2026-11-21:** **Switch to 3D (owner decision 2026-11-21):** fixed angled top-down camera, cartoony colourful 3D (mood: *Find Your Words*); new §1b with shapes, shading, the placeholder primitive table, real-model sources (Kenney CC0, Blender, AI 3D) and a low-poly budget; 2D-only wording updated; outlines left open (GDD Q17).
- **0.4 — 2026-11-21:** Camera is diagonal (corner view, owner 2026-11-21): buildings show two sides, so both need detail.
