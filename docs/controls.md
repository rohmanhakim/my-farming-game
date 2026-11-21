# Controls

> **Status:** Brainstorming draft
> **Version:** 0.16
> **Last updated:** 2026-11-21
> **Purpose:** Control schemes (gamepad and keyboard + mouse) for this kind of game, adjusted to the scope of the game described in the GDD. **PC first, consoles (Switch, PlayStation) later; no touch/mobile.** Tables use XBOX button names, but any modern controller works by position (see §2 *Any controller*). All mappings are tentative (see GDD open question Q5) and subject to playtesting.

---

## 1. Control goals (from GDD §8)

- Keep the number of buttons and actions low.
- Make the primary interaction consistent throughout the game.
- Use large, easy-to-select on-screen targets for mouse play.
- Simple two-button inputs and short holds are acceptable (e.g. LB+RB to cycle the tool belt); avoid rapid, memory-heavy button sequences.
- Offer a clear way to pause and reach settings.
- Essential actions are icon-led; any text is short.

---

## 2. XBOX-layout gamepad (PC prototype)

| Action | XBOX button | Notes |
|---|---|---|
| Move character / move menu cursor | Left Stick or D-Pad | Walking only; no run required. The D-pad is reserved for movement only (menus use dedicated buttons). |
| Interact / use selected tool / confirm | A | Talk to NPCs, plant, water, harvest, feed, deliver orders, open shops/order board, place buildings, catch/release creatures. |
| Cancel / back | B | Exit menus, decline a task, put tool away. |
| Open / cycle fixed tool belt | X / LB + RB | Cycles watering can → seed box → earned tools (e.g. butterfly net, brush, basket). Large icon slots; no sortable inventory. (LB+RB is a simple two-button input, allowed per the control goals.) |
| Open collectathon inventory | Y | Opens the collectathon slot grid. Picture-based, no reading required. (The Collection Book lives in the pause menu.) **Phase-001 (Task 1.4):** Y opens the simple bag (auto-stacked items + wallet); Y or B closes it. |
| Open care menu | Right Stick (RS) click, or on-screen care icon | Dedicated button — the D-pad stays reserved for movement. Shows care meters for every animal and the fish pond, and which need attention. Picture icons. |
| Pause menu / calendar & settings | Start | Opens pause: day/night indicator, season calendar, Collection Book, sticker book, settings (volume, read-aloud, nudges, break reminder), save, quit. |
| Open / close the Orders list *(owner 2026-11-21, Phase-002)* | Back (Select) — View on Xbox, Create/touchpad on PlayStation, − on Switch *(proposed)* | Same as the on-screen Orders button on the HUD: picture cards of every taken order (GDD §4). B or Back closes it. |
| Help tooltip / inspect | *(needs a new button — proposed: hold LT)* | Shows a contextual hint or spoken/visual cue for the highlighted object. **Deferred — no Phase-001 task builds it** (the hint ladder in characters.md §8 covers help for now). Back/Select moved to the Orders list (2026-11-21). |
| Switch camera corner *(owner 2026-11-21, 3D)* | Right Stick flick left or right | Swaps the diagonal camera between the south-east and south-west corners with a short glide; one switch per flick; not while a menu is open. Same position on every pad. |
| Reserved (unassigned in prototype) | LT / RT | Held for future tools or camera options if needed. |

---

### Movement feel (decided 2026-11-20, built in Task 1.2)

- Kit **walks in 8 directions** with smooth starts and stops, but **faces and uses tools in 4 directions** (up, down, left, right), Harvest Moon / Stardew style. On a diagonal Kit keeps facing the way they already faced, so the sprite doesn't flicker. **In 3D (owner 2026-11-21, Phase-001 Tasks 1.14–1.20):** stick, D-pad and keys move Kit relative to the screen (up = up the screen) under the fixed **diagonal** camera (owner 2026-11-21: the grid runs diagonally on screen). Kit's model turns smoothly towards the walking direction, but the tool / interaction front stays one of the 4 grid directions so the tile in front is always a whole tile: the grid direction nearest to the walking direction; straight up / down / left / right on screen lies between two, and then Kit keeps the current front (proposal, playtested in Phase-001 Task 1.14). The highlighted tile shows what A will hit.
- A half-tilted stick walks slower; diagonals are as fast as straight lines.

### Any controller (decided 2026-11-20)

- Buttons are mapped **by position**, not by printed letter: "A" means the bottom face button (Cross on PlayStation, B on a Switch Pro Controller), "B" the right face button, and so on. Godot's SDL controller database makes Xbox, PlayStation and Switch Pro controllers behave the same.
- **Every action must work on a gamepad** (nothing keyboard-only), so console versions need no new scheme.
- **Button prompts are abstract:** UI asks for the prompt of an action (`interact`, `cancel`, …), never a hard-coded "A". A later task draws per-controller icons (Xbox / PlayStation / Switch).
- **Menus (built in Task 1.4):** A presses the focused menu button and B goes back, on every pad (Godot's own menu actions have no gamepad buttons, so the project adds them).
- **Open (for the console phase):** Nintendo's own convention puts confirm on the right face button; decide then whether Switch builds swap `interact`/`cancel`.

## 3. Local co-op (PC)

- **Player 1 (Kit):** Full control scheme as above.
- **Player 2 (Ada):** Identical scheme; real-time collaboration.
- Simultaneous input is never essential. Either player can move, interact, or use tools independently.
- Tasks work in solo play and in co-play.
- **Camera (decided 2026-11-19): dynamic split screen.** One shared camera while both players are on screen together; when they move apart the view splits vertically, and it merges back smoothly when they meet. Both players share one house, farm, map, wallet, and order board.

**Seat rule (decided): keyboard + mouse is locked to one player.** At most **one** seat may use keyboard + mouse; **the other seat must use a gamepad.** If no gamepad is connected, **co-op cannot be started** — the game runs single-player with keyboard + mouse. Reason: a keyboard and a mouse are two separate devices, so two people cannot share that seat without fighting over the same hands; a gamepad is self-contained, which is why two gamepads remain the real co-play configuration (GDD §8).

---

## 4. Keyboard + mouse scheme (PC) — **Don't Starve Together-style** (decided)

Two ways to do everything, available at the same time, exactly as DST does it: **walk with WASD *or* click-to-move**, and **one left-click both approaches and acts** (Kit walks to the target, then does it). The care menu opens from an on-screen care icon (same picture view as the gamepad RS binding).

| Action | Keyboard + mouse |
|---|---|
| Move character | **WASD / arrow keys**, *or* **left-click the ground** to walk there (hold LMB to keep walking toward the cursor) |
| Interact / use tool / talk / plant / water | **Left-click the object or character** — Kit walks over, then acts |
| Quick action (nearest thing in front of Kit) | **Space** — DST's auto-action, minus combat |
| Cancel / back (decline a bubble, exit a menu) | **Right-click**, or a visible on-screen cancel button |
| Cycle tool belt | **Mouse wheel** or dedicated on-screen belt buttons (click a slot) |
| Open / close the bag | **On-screen bag button** (bottom right); close with right-click, Esc or the Close button. No keyboard key (owner decision 2026-11-20). |
| Open / close the Orders list *(Phase-002)* | **On-screen Orders button** next to the bag button; close like the bag. No keyboard key, matching the bag. |
| Pause / calendar & settings | **ESC** key or on-screen pause button |
| Switch camera corner *(owner 2026-11-21, 3D)* | **Q** |

**Deliberately not carried over from DST:** combat keys (F), camera rotation (Q/E — our camera is fixed), all Ctrl/Alt/Shift modifiers (control goals forbid memory-heavy combos), number hotbar (tool belt is cycled instead; revisit if the belt grows long), map key (the valley map is a page in the pause menu — places.md §5 — so it needs no key of its own).

**Seat rule:** see §3 — keyboard + mouse belongs to **one** player only; a second player requires a gamepad, otherwise co-op does not start.

---

## 5. Touchscreen — removed

**No touch (decided 2026-11-19; platform updated 2026-11-20 to PC first, consoles later).** The former iPad/Android touch mapping and touch turn-taking are dropped; no mobile or tablet version is planned.

---

## 6. Calendar, season, day/night, and clock UI

- **HUD clock (always visible while playing):** a small corner dial — an arc of daylight with a sun/moon marker — whose marker **jumps one 3-hour phase at a time** (8 day phases; GDD §4), with a digital readout that **ticks every 15 in-game minutes** (owner 2026-11-20). It is icon-based and readable at a glance. A small digital readout beside it ("09:00") gives the exact time.
- The **Start** pause menu shows the current season, day, and time of day visually (icons, not text-dependent), matching the HUD dial.
- The clock stops while any menu is open and does not advance while the game is closed; sleeping returns it to Dawn.
- Sleep/rest at home is prompted through a clear visual button; sleeping advances the day gently.
- No simultaneous button combos are required to check time or season.

---

## 7. Excluded from controls

- Rapid or memory-heavy button sequences and quick-time events. (Simple two-button inputs like LB+RB and short holds are fine.)
- Free camera rotation, zoom or any other camera control (the 3D camera is fixed at an angled top-down view per GDD §9; it follows Kit). The only exception is the **corner switch** (south-east ↔ south-west; owner 2026-11-21).
- Inventory sorting or weight management.
- Voice commands (optional spoken prompts may be added for accessibility, but not required to play).
- Care or growth timers that run while the game is closed.

---

## Related documents

| Document | Relationship |
|---|---|
| [README.md](../README.md) | Centralized hub for the project; technical direction covers engine and platform evaluation. |
| [GDD.md](./GDD.md) | Open question Q5 covers gamepad/mouse mappings; design pillars shape control goals. |
| [mechanics.md](./mechanics.md) | Core verbs (move, interact, plant, etc.) map to the buttons listed here. |
| [inventory.md](./inventory.md) | Y button opens collectathon inventory; RS click opens the care menu; Collection Book is in the pause menu. |
| [challenge-layers.md](./challenge-layers.md) | The single difficulty mode (no difficulty setting). |

## 8. Change log

- **0.1 — 2026-11-18:** Initial draft. XBOX layout with fixed tool belt cycling, pause/calendar access, two-gamepad co-play, mouse scheme, and touchscreen mapping. Day/night and season UI noted. Exact mappings TBD per GDD open question Q5.
- **0.2 — 2026-11-18:** Added collectathon inventory open (Y button), care menu (D-Pad Up), and exclusion of real-time caretaking timers.
- **0.3 — 2026-11-18:** Added grown-up menu button mapping (Start hold 3 seconds) for parent-controlled challenge layers.
- **0.4 — 2026-11-18:** Care menu moved from D-Pad Up (conflicted with D-pad movement) to a dedicated RS click / on-screen icon. Y opens the grid only; Collection Book lives in the pause menu. Combo rules relaxed: simple two-button inputs and short holds are allowed.
- **0.5 — 2026-11-18:** §6 retitled and extended for Milestone 0 decision 2 (in-game clock): added the **always-on HUD clock dial** (8 day phases, stepped 3-hour jumps, icon-based, adult digital readout), pause/close behaviour, and sleep resetting to Dawn; pause calendar now mirrors the dial.
- **0.6 — 2026-11-18:** **Milestone 0 (control mappings) decided.** §4 rewritten as the **Don't Starve Together-style keyboard + mouse scheme**: WASD/arrow *or* click-to-move, left-click = walk-to-then-act, Space = quick action on the nearest interactable, right-click = cancel, wheel = belt, Esc = pause; DST's combat keys, camera rotation, modifiers, hotbar and map deliberately dropped. New **seat rule** in §3 and §4: **keyboard + mouse is locked to one player — a second player must use a gamepad, otherwise co-op cannot start.** Back/Select help tooltip marked **deferred** (no Phase-001 task builds it). Gamepad table (§2) unchanged and confirmed.
- **0.7 — 2026-11-19:** **PC only (direction reset, GDD v0.19).** §5 touchscreen mapping and touch turn-taking removed; touch wording dropped from §1 and §4. **Grown-up menu (Start-hold) removed** — challenge layers are retired and the difficulty mode lives in normal Settings. Interact row extended for feed/deliver/shops/place; care menu now covers the fish pond; Player 2 named Ada.
- **0.8 — 2026-11-19:** Co-op camera decided: **dynamic split screen** that merges when players are close; shared house/farm/map. Difficulty setting removed from the pause menu (single mode); read-aloud and nudges toggles added.
- **0.9 — 2026-11-20:** Platform is now **PC first, consoles (Switch, PlayStation) later**. §2 adds *Movement feel* (8-way walking, 4-way facing, built in Task 1.2) and *Any controller* (position-based buttons for Xbox/PlayStation/Switch Pro, gamepad-complete actions, abstract button prompts, open Switch confirm-button question).
- **0.10 — 2026-11-20:** HUD clock readout ticks every 15 in-game minutes (owner, GDD v0.32); the dial marker still jumps per 3-hour phase.
- **0.11 — 2026-11-20:** The map-key note now points to the pause-menu picture map of Bramblewick Vale (places.md v0.10); no new binding.
- **0.12 — 2026-11-20:** Task 1.4 built: Y / on-screen bag button opens the simple bag (no keyboard key, owner decision); gamepad A/B confirm and go back on focused menu buttons on every pad; belt slots are clickable.
- **0.13 — 2026-11-21:** **Orders list (owner decision 2026-11-21):** new on-screen **Orders button** next to the bag (Phase-002); gamepad **Back/Select** opens it (proposed). The deferred help tooltip loses Back/Select (proposed: hold LT).
- **0.14 — 2026-11-21:** **3D switch (owner decision 2026-11-21, GDD v0.45):** movement is camera-relative under the fixed 3D camera, Kit's model turns 8-way while the tool front stays 4-way; mouse clicks pick through a camera ray (objects first, then the ground); still no camera control of any kind. No binding changes.
- **0.15 — 2026-11-21:** **Diagonal camera (owner decision 2026-11-21):** movement stays screen-relative; the 4-way tool front follows the grid (diagonal on screen) with a tie rule for straight screen directions. No binding changes.
- **0.16 — 2026-11-21:** **Camera corner switch (owner 2026-11-21):** right stick flick left/right (any pad) or Q swaps the 3D camera between the south-east and south-west corners; free rotation and zoom stay excluded.
