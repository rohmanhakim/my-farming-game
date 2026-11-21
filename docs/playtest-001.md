# Playtest 001 — First Playable (Phase-001 Task 1.13)

> **Status:** Ready to run — no sessions held yet. **Run it on the 3D build after Phase-001 Task 1.20** (owner switched to 3D, 2026-11-21); the rows below still apply, with "roof" meaning the faded roof and camera-side wall
> **Version:** 0.3
> **Last updated:** 2026-11-21
> **Purpose:** The checklist for playtesting the Phase-001 vertical slice: how to set up a run, what to try for each task (1.5–1.12), and the observation sheet with the GDD §14 questions. Findings go back into `Phase-001.md` Task 1.13 *Playtest findings*.

---

## 1. Before every session

1. Open `src/` in Godot 4.7 .NET and press **F5**. Untick *Embed Game on Next Play* first (Game tab ⋮ menu); see Task 1.2 Notes.
2. **New game:** the game loads the last save. To start fresh, delete the save:
   `~/Library/Application Support/Godot/app_userdata/Fields & Sunshine/savegame.json` (also `savegame.bak.json`). On Windows: `%APPDATA%\Godot\app_userdata\Fields & Sunshine\`. Settings live next to them in `settings.json`.
3. Plug in a gamepad (Xbox, PlayStation or Switch Pro) **and** keep the mouse handy. Every step below should work on a pad alone.
4. **Debug keys** (debug builds only, for the tester, not the child): F2 = next 3-hour phase, F3 = clock ×20 (press again for normal), F4 = jump to 21:20, F6 = +1 of every item, F7 = +100 coins, F8 = −100 coins.
5. Children play with an adult present and with consent (GDD §14).

## 2. What to try, task by task

| Task | Try this | Watch for |
|---|---|---|
| **1.5 Farm & garden** | Walk around the house, coop, barn and down the path to the barn. Go into the house, coop and barn. In the garden: pull weeds (hands), hoe, plant (seed bag), water (can); A / Space hits the tile with the white square, or click a tile. Water some beds daily, leave one dry for 2 and 3 nights (F4 then sleep). | Does the faded roof read as "inside"? Does the clock stop inside? Does yellow read as "needs water"? Does losing a crop feel fair? |
| **1.6 Lane, Marlow, berries** | Walk through the gate up the lane. Talk to Marlow (A): say Not now (B), then Yes. Pick 3 bushes. Give him the berries (45 coins). Open his ledger in the shed. | Does a 6-year-old understand the bubble (berries ×3 → coins 45) before accepting? Is "No hurry" kind? |
| **1.7 Order board** | After the berries, use the board by the shed (top of the lane, left of the shed door). Take an order, cancel one, hand one in. Sleep and look again: sizes vary, the same item can be wanted twice, some cards are baskets (2–3 items), some show "+N today" (quick bonus). Collect eggs → the baker writes; deliver a carrot order → the allotment neighbour; 5 orders → the shopkeeper. After 4 orders to one neighbour, a daily order (calendar icon) appears: subscribe, deliver once a day (HUD chip gets ✓), stop it any time. | Are the cards readable at a glance? Are baskets, daily orders and the "today" bonus understood? |
| **1.8 Shop & phone** | Before 07:30 use the shed counter (closed sign). At 08:00+ buy carrot and flower seeds. Try a hen (coop full). Spend down to a few coins and try again. In the bag, press the flower packet, then plant. Use the phone at home before and after 18:00 and check the bins next morning. | Are prices and "not enough coins" understandable? Is the closed sign clear? |
| **1.9 Hens & cow** | Choose the feed scoop, use the coop bin, then the cow. Next morning collect eggs and milk. Skip feeding for 1, 2 and 3 days. | Are the hearts and need icons noticed? Is the loss card gentle, not frightening? Is the warning noticed? |
| **1.10 Save/load** | Play a day, quit (pause → Quit), start again. | Everything back as it was? |
| **1.11 Pause & light** | Start / Esc / the pause button. Save, Settings (volume, Hints, Read aloud, Break reminder), Quit. Watch the light from dawn to dusk (F3). | Is dusk dim but readable? Are the menu icons understandable? |
| **1.12 Guided start** | From a new game, follow the hint chip and the arrow: door → weeds → plant → water → Marlow → berries → hand over → board → feed hens and cow → goal. Turn Hints off in Settings and check no hints appear. | Does a 6–8-year-old finish in ≈ 5 minutes without help? Is anything confusing or blocking? |

## 3. Observation sheet (one per player)

Player: ______ Age: ___ Controller / mouse: ______ Date: ______ Observer: ______

| GDD §14 question | Yes / partly / no | What happened (quote the player) |
|---|---|---|
| Starts with little or no explanation (read-aloud placeholder: an adult may read) | | |
| Can tell what can be used (prompts, arrow, highlight square) | | |
| Understands crops need water; reads yellow as a warning | | |
| Losing a crop feels fair and teaches something | | |
| Controls comfortable (gamepad and keyboard + mouse) | | |
| Understands the NPC's want, task and reward before accepting | | |
| First order makes the money loop clear in the first minutes | | |
| Understands the 3-days rule from the warnings alone | | |

Timings: guided start ____ min · first order paid at ____ min · first carrots on day ____.

Exit-criteria checks (Phase-001 §3): completes the guided start and the first two orders · buys seeds · orders feed by phone · keeps the hens and cow fed — **without adult help?** ______

Anything that crashed, froze or looked wrong: ______

## 4. Change log

- **0.1 — 2026-11-21:** New document (Task 1.13): session setup (F5, deleting the save, debug keys), what to try for Tasks 1.5–1.12, and the observation sheet with the GDD §14 questions.
- **0.2 — 2026-11-21:** Order board row updated for the order variety (sizes, baskets, quick bonus, daily orders, new neighbours) and the 45-coin first order (no premium).
- **0.3 — 2026-11-21:** Status: the playtest now runs on the 3D build after Task 1.20 (owner switched to 3D); Task 1.20 updates the rows to the 3D build.
