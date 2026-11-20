# Game Design Document

> **Status:** Brainstorming draft — **direction reset 2026-11-19** (see v0.19–v0.20 change log)
> **Version:** 0.42
> **Last updated:** 2026-11-21
> **Purpose:** A living document for shaping the game together. Confirmed owner decisions are marked **decided**; everything else is a proposal, not a commitment.

## 1. High Concept

A fully 2D, **realistic but kid-friendly, educational** farming-life game for PC. The player is **Kit, a 17-year-old** who has come from the city to look after the family's land on Bramblewick Lane. Kit grows crops, raises animals, stocks a fish pond, fills **orders from the neighbours**, and slowly grows a small, tangled plot into a working farm — the way a *Harvest Moon* farm grows: the challenge rises naturally as the farm gets bigger. **Automation** (sprinklers, feeders, helpers), unlocked later by crafting or buying it, is the game's **key selling point**, followed by **decoration** (owner decision 2026-11-19): first the farm runs itself, then Kit makes it beautiful.

Farming, community, and exploration are parts of one warm world. Neglect has real but gentle consequences — crops turn yellow and are lost, neglected animals are gone — always shown without anything graphic. There is **one difficulty: realistic but kid-friendly**. The presentation uses illustrated, non-pixel-art graphics.

The game takes inspiration from the welcoming farming and community feel of *Stardew Valley* and the farm-growth arc of *Harvest Moon*, with a smaller scope and a teaching-friendly tone.

**Working title:** **Fields & Sunshine** (owner pick 2026-11-19; chosen after trying Seeds & Sunshine and Greens & Sunshine; a formal trademark check is still needed before release). The setting stays Bramblewick Lane; the title does not need to match it.

## 2. Audience and Play Context

- **Audience (decided 2026-11-19): ages 6 and up**, plus families and teens. Kid-friendly and educational; no longer aimed at pre-elementary children.
  - **What 6+ requires of the design:** icon-first UI (every price, order, and need shows a picture plus a small number); sentences of about 10 words or fewer; numbers kept small (counts up to about 20 in orders; coins shown as icon + number); optional **read-aloud** for dialogue and order cards; no reflex or timing tests; losses shown without anything frightening.
  - **Rating target:** an all-ages / 6+ rating (e.g. ESRB E, PEGI 3–7) — no violence; animal loss is shown only as a gravestone message card. Formal rating review before release (§12).
- **Play modes:** Solo, or **local co-op** on one PC: one house, one farm, one map, shared wallet and orders, with a **dynamic split screen** that merges into one view when the two players are close together (§4 *Co-play*).
- **Second player:** Plays **Ada**, the family friend (characters.md §7).
- **Reading:** helpful but not required for core actions; short text always comes with icons, and key text can be read aloud.
- **Platform:** **PC first; consoles later** (decided 2026-11-20, replaces "PC only" from 2026-11-19). The game ships on PC first; **Nintendo Switch and PlayStation** versions are planned after that. No tablet, mobile, or touchscreen versions are planned.

## 3. Design Goals

### Design pillars (decided 2026-11-19)

1. **Easy to understand:** A player can see what to do and learn the controls by trying them.
2. **Realistic but kid-friendly:** Plants need water, animals need care, money must be earned. Neglect has consequences — crops yellow and are lost; neglected animals are gone — but nothing is shown graphically: an animal loss is a short message with a picture of a gravestone. No sickness, no vet, no gore.
3. **Educational:** Players learn real things through play — seasons and what grows when, what animals need, where food comes from (milk → cheese, wheat → bread, fruit → jam/pie), and simple money sense (earn, budget, save up, invest in tools).
4. **Grow your farm (Harvest Moon-style):** Challenge rises naturally as the farm grows. All land is Kit's from day one but overgrown and must be cleared. Early limits are **money**, then **cleared land**; the final limit is **total land**. Animal numbers are limited by **housing** (a 5-stall barn holds 5 cows).
5. **Automation is the payoff:** Sprinklers, feeders, and helpers are unlocked by crafting or buying them at the right point in progression, turning chores into a farm that runs itself.
6. **Many ways to earn, none mandatory:** Neighbour orders are the main income; market sales, contests, foraging and fishing, trading, crafting, and friendship rewards are alternatives. The player chooses how much money to chase — there is no pressure and no debt (§4 *Ways to earn and acquire*).
7. **Small, satisfying activities:** Short actions produce visible, cheerful results.
8. **Decoration — the second selling point** (decided 2026-11-19): once automation frees up time, players lay out and decorate the farm (buy, craft, or win decorations; place anywhere), raising **farm charm** that brings visitors and wins contests (progression.md §6.1). Never required to optimise.
9. **Warm illustrated world:** Expressive characters, clear shapes, and inviting environments.
10. **Practice everyday social skills:** Through ordinary interactions — greeting, keeping a promise, delivering what was asked, negotiating a trade — players explore communication, reliability, and caring for shared spaces.

### Learning goals (decided 2026-11-19)

The *educational* pillar, made concrete. Every system should serve at least one goal.

| # | A player learns that… | Taught by |
|---|---|---|
| 1 | Plants need water every day, the right season, and good soil | Daily watering, 3-dry-days rule, seasons, compost |
| 2 | Animals depend on us for food and shelter every day | Daily feeding, housing capacity, 3-unfed-days rule |
| 3 | Food comes from farms and is changed into other food | Milk → cheese, wheat → flour → bread, fruit → jam/pie |
| 4 | Money is earned, spent, and saved up for a goal | Orders, prices, saving for hens, a cow, or a bigger barn |
| 5 | Prices change with the season and how much is for sale | Seasonal and demand pricing |
| 6 | Keeping a promise matters, and you choose what to promise | Accepting and delivering orders (optional) |
| 7 | Space and time must be planned | Clearing land, building footprints, days to harvest |
| 8 | Tools and machines save work | Tool upgrades, sprinklers, feeders, helpers |
| 9 | Waste can be reused | Compost, scraps, cleared debris → materials |
| 10 | Nature works together | Bees pollinate crops; rain waters fields; ponds and brooks |

### Intended emotional experience

Players should feel curious, capable, responsible, and proud of the farm they built. Losses should feel like a lesson that can be acted on ("I'll build a feeder next time"), never like shame.

### Social learning approach

The game lets players practise interactions that help beyond home: greeting someone, asking before taking, taking turns, offering help, keeping an agreement, and responding when someone has a different need. Consequences are proportionate, connected to what happened, and focused on how people or the shared space respond. When possible, players can repair a misunderstanding or try again. Avoid rigid good/bad scoring or shame.

### Difficulty: one mode (decided 2026-11-19)

There is **one mode: realistic but kid-friendly** — no difficulty choice and no mode switching. Realistic: daily watering and feeding, real sell prices, losses from neglect. Kid-friendly: two warning days before any loss, buying at about half the real price, no debt, nothing changes while the game is closed. Challenge comes from the farm growing, not from a setting. Rules table: [challenge-layers.md](./challenge-layers.md).

### Social interaction anchor: reciprocal requests and orders

The shared pattern is that the player wants something and helps another person to get it.

1. The player expresses interest in or asks for something.
2. An NPC explains a clear task, or posts an **order**, and what the player receives for it (money, an item, a recipe, a blueprint).
3. The player chooses whether to accept and completes it, or leaves it.
4. On completion, the NPC responds and gives the promised reward.

Requests and orders are **optional and have no deadline penalty**. An unaccepted order simply waits or is replaced by a new one; an accepted order stays open until it is delivered or the player cancels it, with no loss. NPCs remember kindly: "We didn't finish that last time. Would you like to try it now?"

### First want-task-reward beat: The First Order (decided 2026-11-19)

The first beat teaches the game's real loop — produce → order → money → invest — instead of a butterfly net.

| Beat | Design |
|---|---|
| Player's want | Kit starts with 5 hens, a cow, and about a week of feed — but the feed will run out and the garden is bare. Kit wants coins for feed and seeds (and, one day, more animals). |
| NPC's request (quick) | Marlow: "Bring me 3 handfuls of wild berries from the lane." Foraging is possible right away, so the **first order is paid within minutes** (coins + the order board opens). |
| NPC's request (farming) | Marlow then orders **5 carrots**, which Kit grows from the starter seeds (3 days, watered daily). |
| Player's choice | Accept, decline, or leave it. Marlow restates kindly; if the carrots are lost to neglect he simply waits for the next batch. |
| Reward | Coins for feed and seeds and the **order board**, where neighbours post repeat orders. |
| Loop back | Eggs and milk → egg and milk orders from Marlow and the baker → repeat, subscription, and bigger orders. |

**The Butterfly Garden** stays as an **optional later request** in Spring (plant and water three flowers → butterfly net → catching butterflies). It is no longer the opening.

#### Starting loadout and item handling

- **Hands:** Planting, harvesting, decorating, talking, and general interaction.
- **Watering can:** Starts with the player. Watering is required for growth, once per day.
- **Old family hoe:** For tilling cleared ground.
- **Seed box:** Starts with two or three basic seeds; more are bought at the seed shed.
- **Purse:** 500 coins (economy.md §1).
- **Starting animals:** 5 hens (old coop) and 1 cow (old barn), with ≈ 7 days of feed (35 chicken-feed portions, 7 hay portions) already in their feed bins.
- **Ordering feed from home (owner decision 2026-11-19):** animal feed (grain, hay, fish food) is ordered **by phone from home**: Kit's parents left the **feed seller's number** on a note by the house phone (they kept the farm running this way). No shop trip needed. Orders placed **before 18:00** arrive at **Dawn next day**, straight into the **feed bins** in the coop, barn, or pond; orders after 18:00 arrive the day after. No delivery fee. Kit feeds animals by scooping from the bin. **Seeds, saplings, and everything else are still bought in person at Marlow's seed shed** (shop hours).
- **Future tools:** Introduced when they open a new verb or make a chore faster (scythe, hammer, axe, brush, fishing rod, net…).

The tool belt holds a small set of large icon slots. Harvests and products go to picture-based, auto-stacking inventory slots (inventory.md). Overflow goes to a storage chest; there is no sorting and no "full" penalty.

#### Design-pattern references

| Reference | Broad pattern considered |
|---|---|
| *Harvest Moon* | Small plot → bigger farm; money gates early growth, land gates late growth; buildings and tools as investments. |
| *Stardew Valley* | Shipping, community requests, crafting sprinklers as a mid-game milestone. |
| *Animal Crossing: New Horizons* | Short, explicit requests; workshops introduce tools. |
| *Cozy Grove* | Small, untimed help requests connected to character stories. |
| *Spiritfarer* | Helping characters leads to new facilities and mechanics. |

These are references for pacing, never templates to reproduce.

#### Later alternatives

- **Ladder to a lookout:** Help prepare a neighbour's picnic, receive a ladder that opens a lookout.
- ~~Garden helper (deferred because it automates)~~ — **reversed 2026-11-19:** automation is now the key selling point (§4 *Automation*).

### NPC relationship meter (proposal)

Track a separate relationship value per NPC, changed by delivered orders, completed tasks, and dialogue events — never passively. Higher relationships bring **repeat and bigger orders**, recipes, blueprints, and special events. It is the history of the relationship, not a moral grade.

## 4. Core Experience

### Core loop (decided direction)

1. **Plan:** Decide what to grow, raise, or build, within the money and land Kit has.
2. **Work the farm:** Plant, water, harvest; feed and care for animals; tend the fish pond.
3. **Earn:** Fill neighbour orders (main income) or use any other way to earn (§4 *Ways to earn and acquire*).
4. **Invest:** Spend on seeds, animals, buildings, tools, land, and — later — automation.
5. **Grow:** A bigger farm brings bigger orders, new neighbours, and new challenges, until the land itself is the limit.

### Neighbour orders — the main income (decided 2026-11-19)

- Neighbours order farm output: **eggs, milk, fruit, vegetables, fish, pies, jam, seasonal baskets, and bulk orders**.
- **Orders only ask for what Kit can actually produce** right now — an order generator draws from crops, animals, and recipes the player has unlocked, at quantities their farm size can supply.
- **Optional, no pressure:** orders have no deadline penalty; ignoring them costs nothing but the missed money.
- **Variety:** different neighbours have different tastes (Marlow likes herbs and honey; a baker wants eggs and wheat; a family wants a weekly basket).
- **Growth:** satisfied neighbours send **repeat orders** and, as the relationship grows, **bigger orders** (bulk, mixed baskets, festival catering).
- **Variety from the start (owner decision 2026-11-21):** even on a small farm the board mixes **varied sizes** (the same item can be wanted twice in different amounts), occasional **mixed baskets** (2–3 items, a small basket bonus), **daily standing orders** once a neighbour knows Kit, and **quick-bonus orders** that pay a little extra if delivered the same day — a bonus only, never a penalty, so "no deadline penalty" still holds. The **baker, the allotment neighbour and the shopkeeper** start posting on Marlow's board during the first tier, one at a time after Kit's first eggs, first delivered crop order and fifth delivered order (characters.md §10); their village buildings still open at T2.
- **Subscription orders (owner request 2026-11-19):** a satisfied neighbour can offer a **standing daily order** — e.g. "6 eggs every morning by 10:00" or "a milk can every day". Kit chooses to subscribe; a **reminder** shows on the HUD and the order board each morning until it is delivered; Kit can **cancel any time** with no penalty. A missed day simply isn't paid (no fine, no relationship loss); a helper can take over the daily delivery later.
- **Where:** the **order board** at the seed shed, plus hand-delivered requests when talking to a neighbour. Delivery is by hand or, later, via a delivery helper.

### Ways to earn and acquire (decided 2026-11-19 — all optional)

Orders are the main income, but **none of these is mandatory**: the player decides how much money they want and how they want to get it.

| Way | What it gives | Notes |
|---|---|---|
| **Neighbour orders** | Money, relationship, repeat/bigger orders | Main income (above). |
| **Market stall / shipping box** | Money | Sell any produce. **Seasonal pricing** (out-of-season goods pay more) and **demand pricing** (selling lots of one thing lowers its price for a while). Prices in economy.md. |
| **Town board & festival contests** | Money, ribbons, unique items | Board posts one-off jobs (e.g. "fix the fence"); seasonal festivals hold produce and animal contests judged on quality. |
| **Foraging, fishing & exploring** | Wild goods to use, sell, or deliver | Berries, mushrooms, herbs along the lane; fishing at the brook; finds while exploring. |
| **Travelling merchants** | Trades, rare seeds, rare items | Visit on certain days; trade goods for goods, or buy rarities. |
| **Crafting / processing** | Higher-value goods | Turn produce into **jam, cheese, bread, pies, honey jars, smoked fish** at the kitchen/workshop — worth more than the raw item. |
| **Shop purchases** | Seeds, animals, tools, **blueprints**, **automation** | The seed shed and the market sell what Kit needs to grow; blueprints unlock building and crafting recipes. |
| **Friendship rewards** | Gifts, **recipes**, blueprints | Good relationships unlock recipes and helper blueprints, not just items. |
| **Quality animal products** | Better prices and contest entries | Well-cared animals give higher-quality eggs, milk, wool, honey (shown as 1–3 stars) that sell for more. |

**No debt, no loans, no bankruptcy.** Money can reach zero but never below; a broke player can always forage, fish, or fill a small order to restart.

### Neglect and consequences (decided 2026-11-19)

One rule for everything that lives on the farm: **3 days in a row without care → gone, with 2 warning days first.**

- **Crops:** need water once per in-game day. Dry day 1 → no growth. **2 dry days in a row → yellow** (warning). **3 dry days in a row → gone** (tile stays tilled). Watering resets the count. Rain counts as watering.
- **Animals and fish:** need food once per day. 1 unfed day → need icon. 2 → flashing icon, Ada/Marlow mention it. **3 unfed days in a row → gone right away** — no sickness, no vet. The game shows a **short message with a picture of a gravestone** and a tip; nothing is shown on screen.
- **Never while the game is closed:** days only pass during play.
- **Never a surprise:** every loss has two visible warning days.
- **Recoverable:** a lost crop or animal can be replaced; nothing else is taken away. Automation (sprinklers, feeders) is the long-term answer.

### Plant growth model (decided 2026-11-19 — replaces the Milestone 0 play-time model)

**Day-based, like *Harvest Moon*.** Every crop takes **at least one in-game day**; most take 3–12 days (crops.md §3–§5).

- A crop grows **one day overnight** if it was watered (or rained on) that day.
- Regrowing crops (tomato, berries, hay grass) produce again every few days.
- Nothing grows while the game is closed.
- *Superseded:* the Milestone 0 decision (v0.16) of play-time stages (Fast/Medium/Slow ≈ 15–90 s per stage, Gentle catch-up) is retired — it made crops ready in about 2 minutes, too fast for an economy and for the neglect rule.

### In-day clock and day phases (decision — Milestone 0, 2026-11-18)

**The game has a clock, shown in the HUD. Its readout ticks every 15 in-game minutes (owner 2026-11-20); the day phases below move in 3-hour steps.**

- **8 day phases, 3 in-game hours each** (24 hours = 8 × 3).
- **Readout every 15 in-game minutes** (15 real seconds per tick); the **phase** (light, presence, dial marker) changes one 3-hour step at a time.
- **Day length (decided 2026-11-19, reset 2026-11-20): 17 real minutes** for the playable day, 04:30 wake-up → 21:30 bedtime (17 in-game hours). The 3-hour steps are counted from the wake-up: 04:30, 07:30, 10:30, 13:30, 16:30, 19:30. That is **60 real seconds per in-game hour — Harvest Moon pace** (owner 2026-11-20), so one 3-hour step = **3 min** and 04:30 → 18:00 takes 13 min 30 s. ~5 real minutes of chores (watering, feeding) ≈ 5 in-game hours, done by ≈ 09:30. (Stardew Valley ≈ 42 s per in-game hour.)
- **Runs only while playing:** stops in the pause menu and while the game is closed.
- **When time stops (decided 2026-11-20, owner):** time stops while any menu or screen is open — pause menu, bag, the "Go to sleep?" question, a conversation, a shop screen. Time **also stops indoors** (houses, seed shed, barn, coop) so younger players can take their time (owner 2026-11-20, kid tolerance; replaces the earlier "keeps running indoors"). Known trade-off: indoor work (shopping, chatting, processing) costs no time, so shop hours and timed orders only bite outdoors. **In Phase-001, buildings are entered on the same map** (owner 2026-11-20): walking into a building's footprint fades its roof out and stops the clock; there are no separate interior scenes yet (open question 16).
- **Sleep/rest** ends the day: the calendar day increments and the clock resets to **Dawn (04:30, owner 2026-11-20)**.
- **Forced bedtime at 21:30 (decided 2026-11-20, owner):** if Kit is still up at 21:30, Kit heads home and sleeps automatically — no penalty. Without this, a player who never sleeps would freeze the calendar now that growth is day-based.
- **Night (22:30–01:30) and Small hours (01:30–04:30)** are never played: bedtime comes two hours into Dusk. They exist only as wedges on the HUD dial. Night-only content in the table below (seed shed lit, Moon Petal glow) needs a new home — open question.
- **Seasons (decided 2026-11-19): 30 days each, like *Harvest Moon*** — 4 seasons, a **120-day year**.

| # | Phase | Clock | Light | What is true in this phase |
|---|---|---|---|---|
| 1 | **Dawn** | 04:30–07:30 | First light, soft pink | Morning greeting; new orders appear on the board |
| 2 | **Morning** | 07:30–10:30 | Bright, clear | Mr. Marlow at the gate; seed shed open; butterflies visit flowers |
| 3 | **Midday** | 10:30–13:30 | Highest, brightest | Neighbours out and about; market stall busiest |
| 4 | **Afternoon** | 13:30–16:30 | Long light | Brook keeper on warm afternoons; travelling merchant on visiting days |
| 5 | **Evening** | 16:30–19:30 | Gold → amber | Shops close; shipping box collected at the end of the day |
| 6 | **Dusk** | 19:30–22:30 | Lane darkens | Fireflies in season; Marlow may be indoors |
| 7 | **Night** | 22:30–01:30 | Dark, quiet palette | Seed shed lit; Moon Petal glows |
| 8 | **Small hours** | 01:30–04:30 | Deepest dark | Quiet only |

**HUD clock:** a small dial with a sun/moon marker that jumps one phase at a time, plus a digital readout that ticks every 15 minutes ("07:45").

**What the clock drives:** lighting, NPC presence, shop hours, order-board refresh, shipping-box payout, time-of-day events. **Day rollover** (sleep or 21:30 bedtime) drives crop growth, watering/feeding counts, and the 3-days rule. Missed time-of-day events recur.

### Automation (decided 2026-11-19 — key selling point #1; decoration is #2)

- **What:** **Sprinklers** (water an area of beds), **feeders** (keep animals' food meter topped up), **pond auto-feeders**, and **helpers** (a farmhand or creature who harvests, collects, or delivers).
- **How it is unlocked:** by **crafting** it from a blueprint or **buying** it, once the player reaches the right point in progression (progression.md §3). Blueprints come from the shop, friendships, and contests.
- **Why it matters:** automation is the answer to neglect at scale and the moment the farm "comes alive". It is gated by money and progression, then by land (each sprinkler, feeder, or helper still needs space and upkeep materials).
- **Hired helpers (owner request 2026-11-19), alongside machines:** two kinds, both optional:
  - **Farmhands** — villagers hired **by the day** from **T2** (a "help wanted" card on the order board), like *Harvest Moon*: they arrive in the morning and go home in the evening, so there is **no farmhand building**. **500 coins/day** (economy.md). Each covers one job (water, feed, harvest, collect, deliver subscription orders, clear land) in one area. If the wallet can't pay, they take the day off — never debt.
  - **Garden sprites** — small original helper creatures (inspired by the *idea* of Harvest Moon's harvest sprites, not their design) befriended through gifts and friendship; they work for free but only a few jobs per day and like to be thanked.
  - **Farmhands first, machines later** (owner decision 2026-11-20) — like a real farm: people cost nothing upfront but a wage every day; machines are an investment that is cheaper over time (auto-feeders need the silo; sprinklers include hidden pipes and need no water source). Early on Kit hires; machines take over once money allows (sprinklers pay back against a farmhand in ≈ 3 weeks, feeders in ≈ 2 months, economy.md §6). **Machines are set-and-forget:** sprinklers just need their coverage area; auto-feeders draw from the silo or their own hopper, which phone orders fill directly, so Kit tops up about once a month. Machines only water and feed; harvesting, collecting, and delivering still need people or sprites.
- Automation never removes the manual verbs; players can always water and feed by hand.

### Buildings (decided direction 2026-11-19)

**The farm starts with an old coop holding 5 hens and an old barn (5 stalls) holding 1 cow, plus about a week of feed** — family buildings and animals (owner decision 2026-11-19, like *Harvest Moon*). Kit buys more animals and feed as the farm grows. More and bigger buildings come later. Kit can build more coops, barns, a fish pond, beehives/apiary, an orchard, a workshop, a kitchen, a storage silo, compost bins, sprinklers/irrigation, a **greenhouse** (any crop, any season), a **hydroponics** bay (faster, water-efficient), a market stall, and a farm shop. Each has a cost, a land footprint, an unlock tier, and an education angle — full catalogue in [buildings.md](./buildings.md). Money gates them early; cleared land, then total land (992 tiles), gates them late. Capacity comes from the building (a 5-stall barn holds 5 animals).

### After the farm is running (all optional)

When the land is full and automation runs the chores, long-term goals remain: **decorations and farm charm** (beauty that brings visitors and wins the prettiest-farm contest), **collections** (rare crops, fish, breeds, recipes), **festival trophies**, **restoring the lane** (footbridge, meadow, stile, Lantern Hill), a **showcase/museum**, **mastery** goals, and rare **prestige** items. Details in [progression.md](./progression.md) §6.

### Session shape

Short sessions are satisfying; long sessions are possible. There is no required daily check-in and nothing advances while the game is closed. An optional **break reminder** lives in Settings (off by default). Onboarding is kept short — see [onboarding.md](./onboarding.md).

### Co-play (decided 2026-11-19)

- **One house, one farm, one map**, shared wallet, inventory storage, and order board. Either player can deliver an order.
- **Dynamic split screen:** when the players are close, one shared camera; when they move apart, the screen splits (vertically) and merges back smoothly when they meet again.
- Solo play is complete; tasks never require two players. Online play is not planned.

## 5. Player Activities

### First playable (Phase-001 vertical slice, decided 2026-11-19)

> **Scope note:** Phase-001 is now a **vertical slice of the money loop** (docs/Phase-001.md).

- Move around the front garden, the lane, and Marlow's seed shed.
- Day clock, sleep, and the 21:30 bedtime.
- Clear weeds and stones; till; plant; water daily; see crops grow over days, yellow after 2 dry days, gone after 3.
- Forage wild berries; Marlow's first order (berries) and second order (carrots); coins.
- Order board with simple repeat orders matched to what Kit can produce.
- Start with 5 hens and 1 cow in the old coop and barn, plus a week of feed; buy seeds, feed, and more hens at the seed shed (half real price).
- Feed hens daily, collect eggs, deliver egg orders; the hen-loss message card.
- Save and load (versioned JSON).

### Later activities (product scope)

- Market stall and shipping box.
- More animals (sheep, cow, beehive), fish pond, and buildings.
- Clearing the back fields and farm layout.
- Kitchen/workshop processing and crafting; blueprints.
- Automation: sprinklers, feeders, pond feeders, helpers.
- Fishing, travelling merchants, festivals and contests, the optional Butterfly Garden request.
- More neighbours, relationships, recipes.

## 6. Rules and Difficulty

### Starting principles

- Consequences are real but kid-friendly: crops yellow then disappear; neglected animals and fish are gone, shown only as a short message with a gravestone picture.
- No sickness, injury, vet, gore, or on-screen death.
- No debt, loans, or bankruptcy; money never goes below zero.
- Nothing changes while the game is closed.
- Every loss is warned visibly for two days first, and is replaceable with money.
- Orders and requests are optional with no deadline penalty.
- Do not require fast reactions or precise timing.
- Keep inventory small and understandable: auto-stacking picture slots, no manual sorting.
- Early rewards should introduce new ways to play, not only bigger numbers.
- Show social consequences through character responses, not moral scores.
- Do not take away unrelated progress because a task is unfinished.
- Seasons decide what grows; out-of-season seeds do not sprout (no loss).
- The farm's challenge grows with the farm: money first, then cleared land, then total land (progression.md).

### Difficulty

**One mode: realistic but kid-friendly** (§3; rules in challenge-layers.md).

### Open design questions

- **Superseded (v0.20):** play-time growth (v0.16) replaced by day-based growth (§4).
- **Decided (v0.17):** coarse, HUD-visible, phase-based in-day clock (§4).
- **Decided (v0.19):** what is earned or unlocked — money from orders and other optional sources, spent on seeds, animals, buildings, land, tools, and automation.

## 7. World and Characters

- **Bramblewick Vale** (owner decision 2026-11-20, replaces "one lane, eight places"): a small valley with **Kit's farm** at the bottom, the lane leading up to **Bramblewick Green** (the village hub: Marlow's seed shed, bakery, general store, village hall, smithy, clinic, carpenter, library, inn, market stand, allotments), and three outer areas — **Brookside & Mill Pond**, **Hazel Wood**, **Lantern Hill & Old Quarry**. About 7,400 tiles (≈ 33 screens); the farthest spot is ≈ 36 s from the front door. Areas open at farm tiers and through lane restoration; the valley itself is fixed. Kit's farm grows by clearing and building on its own fields. Sizes, layout, unlocks, and the to-scale map: [places.md](./places.md).
- A **picture map** in the pause menu shows the valley, Kit's position, and where today's orders go (places.md §5).
- **Nobody gets sick:** the village clinic is a place for healthy-food orders and lessons, not treatment (pillar 2 unchanged).
- A cast introduced one at a time — at most three new characters per farm tier (characters.md) — each with their own orders and tastes.

**Setting and background story:** [world.md](./world.md). Characters: [characters.md](./characters.md).

## 8. Controls and Interface

### Control goals

- Keep the number of buttons and actions low.
- Make the primary interaction consistent throughout the game.
- Simple two-button inputs and short holds are acceptable; avoid rapid or memory-heavy sequences.
- Offer a clear way to pause and reach settings.

### Interaction model

Movement plus one primary interaction action; simple visual menus for seeds, orders, and shops. **PC first:** two selectable schemes — gamepad (any modern controller: Xbox, PlayStation, Switch Pro), and keyboard + mouse. Every action works on a gamepad, so later console versions need no new control scheme.

**Exact PC mappings — decided (v0.18; full tables in [controls.md](./controls.md) §2 and §4):**

- **Gamepad:** left stick/D-pad move · A interact · B cancel · X or LB+RB cycle tool belt · Start pause · (Y inventory, RS care menu in later phases).
- **Keyboard + mouse, Don't Starve Together-style:** WASD/arrows *or* left-click-to-move · left-click a target = walk to it, then act · Space = quick action · right-click = cancel · mouse wheel = tool belt · Esc = pause.
- **Seat rule:** keyboard + mouse is locked to one player; the other seat must use a gamepad. No gamepad → no co-op.

## 9. Art Direction

- Fully 2D presentation; no pixel-art style.
- Colorful, cartoony European comics-inspired illustration, with clear inked outlines, expressive faces, lively poses, and readable scenery. Characters are drawn with **teen and adult proportions** (Kit is 17), friendly and expressive, not chibi.
- **Kid-friendly consequences:** a yellowing crop reads clearly as "needs water" (drooping, yellow-brown leaves) without looking rotten; an animal loss is shown only as a small, gentle gravestone illustration in a message card.
- **Palette:** lively, saturated garden colors over earthy, subdued environmental tones.
- **Camera:** angled top-down / three-quarter view.
- Clear silhouettes; strong contrast between interactive objects and backgrounds.

Develop an original visual identity. Do not copy any particular game's characters, assets, settings, or exact visual style.

**Primary references Pioneers of Olive Town and Harvest Moon: Home Sweet Home; proportions (≈ 5 heads tall — not chibi), palette, and music direction:** [art-audio.md](./art-audio.md). Camera implementation and animation complexity: TBD.

## 10. Story, Dialogue, Audio, and Text

- A background story gives the world context without interrupting farming.
- Simple, friendly dialogue with short sentences; text is always paired with icons.
- Presenter-like storytelling: characters address the player, ask a question, and respond to what the player did.
- The story supports responsibility, learning, and everyday social skills.
- Gentle music and sound; no startling sounds; independent volume controls.
- Important feedback never relies on sound alone.

The premise and cast are in [world.md](./world.md). Voice, localisation, and music style: TBD.

## 11. Accessibility and Kid-Friendly UX

- Large text and clear icons; short sentences.
- Forgiving interaction and movement.
- Visible warnings before every loss.
- Adjustable sound and music; visual feedback for audio cues.
- Optional read-aloud for dialogue and order cards; optional break reminder.
- Settings that a grown-up can find and understand.

## 12. Safety, Privacy, and Caregiver Considerations

- No personal data collection; no open chat or user-generated public content.
- No real-money purchases; in-game money only.
- Establish an appropriate child-privacy and age-rating review before release (depends on §16 Q9).

## 13. Technical Direction

- **Platform:** **PC first; consoles later** (owner decision 2026-11-20): Windows first, macOS/Linux considered; **Nintendo Switch and PlayStation planned after the PC release**. No mobile or tablet. Console ports need platform SDK access and Godot console-porting support (third-party porting partners); not scheduled yet.
- **Engine:** Godot 4.x with **C#** (docs/PHASE.md §6). Renderer: **Compatibility (`gl_compatibility`)** — the lightest 2D-friendly renderer with the widest PC hardware support. Assembly name: **`MyFarmingGame`**.
- The first playable (Phase-001) answers movement, clearing, planting, day-based growth and neglect, orders, buying, chickens, and saving before content production begins.

### Save system (decided direction 2026-11-19)

- One slot, auto-save at sleep and on key events, human-readable JSON at `user://savegame.json`.
- **Versioned from day one:** a top-level `schemaVersion` integer; on load, migrations run in order (`v1 → v2 → …`) so old saves keep working as systems are added.
- **Safe writes:** write to a temp file, then rename; keep the previous save as `savegame.bak.json` and fall back to it if the main file fails to parse.
- Each system owns its own section (`clock`, `farm`, `inventory`, `wallet`, `orders`, `animals`…), registered with the save autoload so new systems plug in without touching old ones (project-structure.md §8).

### Development approach

- **Project type:** Passion project with no predetermined content cap.
- **Core team:** Two people, with AI assistance.
- **Art and audio:** Free royalty-free assets allowed; licenses recorded.

## 14. Prototype and Validation Plan

### First playable

Phase-001 vertical slice (§5): clearing, planting, day-based growth and neglect, the first orders, feeding the starting hens and cow, eggs and milk, buying feed and seeds, save/load. Placeholder art.

### Playtesting questions

- Can a 6-year-old start with little or no explanation (with read-aloud on)?
- Can they tell what objects can be interacted with?
- Do they understand that crops need water, and read the yellow stage as a warning?
- Does losing a crop feel fair and teach something, rather than feel punishing?
- Are controls comfortable for gamepad and keyboard + mouse?
- Do they understand the NPC's want, the task, and the reward before accepting?
- Does the first order (berries → coins) make the money loop clear within the first few minutes?
- Do kids understand the 3-days rule from the warnings alone?
- (Later phases) Do orders feel like an invitation rather than a to-do list? Does automation feel like a reward?

Playtests with children are conducted with appropriate adult supervision and consent.

## 15. Product Scope

**In product scope (delivered across phases — docs/PHASE.md):** neighbour orders and money; market stall and shipping box with seasonal/demand pricing; four animal species (chicken, sheep, cow, beehive) with care meters and quality products; fish pond; buildings; clearing the overgrown fields; kitchen/workshop processing; blueprints and crafting; automation (sprinklers, feeders, pond auto-feeders, helpers); foraging and fishing; travelling merchants; festivals and contests; relationships, recipes; long-term goals (charm, collections, lane restoration); sticker/achievement book.

**Not planned:** mobile/tablet versions; online multiplayer; combat; sickness/vet systems; debt or loans; real-money purchases.

## 16. Open Questions for Brainstorming

1. **SUPERSEDED (v0.20):** play-time growth replaced by day-based growth (§4).
2. Which orders and dialogue events change an NPC's relationship, and by how much?
3. Which recipes, blueprints, and special events should high relationships unlock?
4. **RESOLVED (v0.20):** co-op shares one house, farm, map, wallet, and orders, with a merging split screen (§4).
5. **RESOLVED for PC (v0.18):** control mappings in controls.md. (Tablet half dropped — PC first, consoles later; no tablet.)
6. What research is needed to design original teen/adult characters in the European comics direction?
7. Which open story questions in [world.md](./world.md) should be decided next?
8. ~~Grown-up gesture to protect the challenge-layer menu~~ — **closed (v0.19):** layers retired; difficulty mode lives in normal Settings.
9. **RESOLVED (v0.20):** target age band **6+** (§2).
10. **CLOSED (v0.20):** no difficulty modes, so no mode switching.
11. **RESOLVED (v0.20):** day-based growth, Harvest Moon-style (§4).
12. **RESOLVED (v0.20):** the first beat is a first order; the Butterfly Garden is optional and later (§3).
13. **RESOLVED (v0.26, reset v0.29–v0.33):** day length 17 real minutes (04:30 → 21:30, 60 s per in-game hour).
14. **RESOLVED (v0.26):** 30-day seasons, 120-day year (Harvest Moon).
15. **✅ DECIDED (2026-11-21, owner) — order premium: none.** Normal orders pay the plain sell price; only mixed baskets (×1.1) and same-day quick-bonus orders (+20 %) pay extra (economy.md §3).
16. **⚠️ OPEN — interior scenes.** Phase-001 enters buildings on the same map (roof fades, §4). Do later buildings need separate interior scenes (a fade to a room map)? Decide in Phase-002 planning; the kitchen upgrade is the first likely case.

## Related documents

| Document | Relationship |
|---|---|
| [README.md](../README.md) | Centralized hub; mirrors GDD version numbers in its header. |
| [world.md](./world.md) | Setting, story, and places; the GDD remains authoritative for gameplay. |
| [characters.md](./characters.md) | Kit (17), Ada, Mr. Marlow, and future cast. |
| [places.md](./places.md) | Bramblewick Vale: areas, sizes, farm layout, unlocks, and the to-scale map. |
| [crops.md](./crops.md) | Crop list, watering-based growth, yellowing and loss. |
| [animals.md](./animals.md) | Animals and fish pond, care meters, loss rule, quality products. |
| [items.md](./items.md) | Tools, products, processed goods, automation items. |
| [inventory.md](./inventory.md) | Picture-based auto-stacking inventory, wallet, storage chest. |
| [mechanics.md](./mechanics.md) | Verbs and systems: farming, orders, selling, crafting, automation. |
| [controls.md](./controls.md) | PC input mappings. |
| [events.md](./events.md) | Daily, seasonal, order, contest, and merchant events. |
| [progression.md](./progression.md) | Farm growth tiers: money → buildings → automation → land. |
| [buildings.md](./buildings.md) | Farm map (992 tiles) and everything Kit can build. |
| [economy.md](./economy.md) | Prices (sell ≈ real, buy ≈ half) and balance check; full sheet in data/economy.csv. |
| [onboarding.md](./onboarding.md) | The short guided start. |
| [art-audio.md](./art-audio.md) | Art and music brief: references, proportions, music direction. |
| [challenge-layers.md](./challenge-layers.md) | The single difficulty mode and its rules. |

## 17. Change Log

- **0.1 — 2026-11-17:** Initial brainstorming draft based on the brief: a simpler farming-life game for young children with fully 2D, non-pixel-art graphics.
- **0.2 — 2026-11-17:** Set PC as the prototype platform with future multiplatform and touchscreen goals. Added everyday social learning and gentle, repairable consequences as design direction; recorded the reading and speaking assumption for validation.
- **0.3 — 2026-11-17:** Confirmed a blend of gardening, exploration, and helping characters. Established taking responsibility through a clear character-given task as the first social interaction; left rewards and incomplete-task consequences open.
- **0.4 — 2026-11-17:** Defined parent-child co-play on one device as the intended play mode. Recorded the two-person, AI-assisted passion-project context and free royalty-free asset approach; clarified that prototype scope limits are not permanent project limits.
- **0.5 — 2026-11-17:** Specified selectable gamepad and fully mouse-controlled PC schemes, with mouse interaction intended to inform future touch controls. Set a simple, non-chibi European children's-book/comics art direction and clarified that parent-child co-play is real-time collaboration.
- **0.6 — 2026-11-17:** Prioritized iPad and Android tablets, specified separate gamepads for PC co-play and touch turn-taking, and made solo play a complete option. Added a background story and simple, interactive dialogue as a differentiator; refined the art direction toward an original, colorful European cartoon-comics look.
- **0.7 — 2026-11-17:** Defined the social interaction anchor as helping an NPC to get something the player wants. Added NPC memory for unfinished tasks and a proposed per-NPC relationship meter with possible event bonuses.
- **0.8 — 2026-11-17:** Added early mechanic-unlocking rewards, a subtly visible relationship meter that changes only through tasks or dialogue events, and special high-relationship events or unique rewards. Clarified that gardening should be low-pressure and avoid crop-maintenance and inventory chores.
- **0.9 — 2026-11-17:** Proposed the Butterfly Garden as the first want-task-reward beat: plant and water three flowers to earn a butterfly net. Added the starting loadout, fixed tool belt, garden-shelf storage, design-pattern references, and later reward alternatives; marked the proposal for playtesting.
- **0.10 — 2026-11-17:** Proposed that plants grow during play and watering optionally accelerates growth with immediate positive feedback. Added small-garden and no-withering guardrails, optional bounded away-time growth, and playtest questions about whether watering feels fun rather than obligatory.
- **0.11 — 2026-11-17:** Set a hybrid bright-and-earthy palette direction and an angled top-down/three-quarter camera direction. Left original character design for further research and emphasized a distinct visual identity.
- **0.12 — 2026-11-17:** Linked the Bramblewick Lane and Mr. Marlow story proposal in [world.md](./world.md), resolving the background-story premise question while keeping its open details subject to research and playtesting.
- **0.13 — 2026-11-18:** Expanded scope: collectathon inventory (picture-based, auto-stacking, no currency), three care animals (chicken, sheep, beehive) with gentle care meter and no death penalty, seasons affecting crops and animal behavior, progressive sticker system, and Dora/Blues Clues-style picture dialogue with parent guidance role.
- **0.14 — 2026-11-18:** Added Two Modes principle (Gentle Core always on; challenge layers opt-in and parent-controlled), expanded audience to preschool through fifth grade, added pillar 10 (the parent is the difficulty dial), and introduced challenge-layers.md as the source of truth for toggles and stages.
- **0.15 — 2026-11-18:** Audience clarified: primary audience is pre-elementary through first grade playing with a parent, with older children accommodated through the optional challenge layers. §5 MVP vs §15 product-prototype scope disambiguated (product prototype is the source of truth; Phase-001 is the interaction prototype). Time Awareness removed as a layer (day/night and seasons always on, Gentle Core); control-combo rule relaxed (simple two-button inputs and short holds allowed); four care animals (cow added); places recounted to eight; engine recorded as Godot with C# (see docs/PHASE.md).
- **0.16 — 2026-11-18:** **Milestone 0 decision 1 (growth timing):** §4 plant growth model moved from proposal to decision. Growth runs on accumulated play time, live during play, never on an in-game day and never while closed. Presets set (Fast ≈15–20 s / **Medium ≈30–45 s** / Slow ≈60–90 s per stage) with **Medium** as default; growth-while-closed set to **Paused** by default with an optional positive-only “Gentle catch-up” (max +1 stage, never past bloom). Both are options-screen choices made **before starting a new game**, per genre comparison (Stardew Valley / Harvest Moon / Ooblets grow only during play; Animal Crossing is the real-clock exception). Open question §16.1 resolved; §6 growth question closed, clock question narrowed.
- **0.17 — 2026-11-18:** **Milestone 0 decision 2 (in-game clock):** added §4 "In-day clock and day phases" — the game *does* have a clock, shown in the HUD as a stepped dial: **8 day phases × 3 in-game hours**, jumping one 3-hour step at a time (≈3 min of play per step, ≈24-minute in-game day), running only while playing, resetting to Dawn on sleep. Phase table maps every phase to its light, its NPC presence, and its events (morning greeting → small hours). Explicitly excluded from driving growth, care, or penalties. §6 clock question closed; this answers GDD §6/README "decide whether there is any in-game clock" (Milestone 0 item 2).
- **0.18 — 2026-11-18:** **Milestone 0 decision 3 (control mappings):** §8 records the decided PC mappings — gamepad table unchanged, keyboard + mouse redefined as a **Don't Starve Together-style** scheme (dual movement, one-click walk-to-then-act, Space quick action), plus the **seat rule** that keyboard + mouse is locked to one player and co-op needs a gamepad for the second seat. DST's combat/camera/modifier/hotbar keys explicitly rejected. §16 Q5 resolved for PC; tablet layout left open for Phase-003. Full tables live in controls.md v0.6.
- **0.19 — 2026-11-19:** **Direction reset (owner decisions).** (1) **Rewards:** neighbour **orders** for farm output are the main income — varied, matched to what Kit can produce, optional, growing into repeat and bigger orders. (2) **Kit is 17**; audience is kid-friendly and educational but no longer pre-elementary — **exact age band flagged as open (§16 Q9)**; parent-as-difficulty-dial retired. (3) **Pillars rewritten** around *realistic but kid-friendly* and *educational*; **neglect has consequences** — crops yellow then are lost; neglected animals/fish are gone immediately, shown only as a short message with a gravestone picture; no sickness or vet; nothing changes while closed. Growth now **requires watering** (Milestone 0 growth decision amended). (4) **Harvest Moon-style growth:** money is the early limit, land size the final one; fixed 2–3 bed and 4-animal caps removed. Gentle Core + challenge layers + five stages **collapsed into three difficulty modes: Easy / Normal / Realistic**. (5) **Automation** (sprinklers, feeders, pond feeders, helpers) is the key selling point, unlocked by crafting or buying; the old "defer the garden helper" note reversed. (6) **PC only** — tablet/touch plans removed; renderer Compatibility, assembly `MyFarmingGame`. Added **§4 Ways to earn and acquire** (market/shipping box with seasonal and demand pricing, board and contests, foraging/fishing, travelling merchants, processing, shop blueprints/automation, friendship recipes, quality products — all optional, no debt) and the **fish pond**. Added **Buildings** (greenhouse, hydroponics, barn, coop, silo, windmill, farm shop… — new [buildings.md](./buildings.md)) and **After the farm is running** (decorations and farm charm, collections, trophies, lane restoration, showcase, mastery, prestige items). New open questions Q9–Q12.
- **0.20 — 2026-11-19:** **Owner picks from the design review.** **One difficulty mode** (realistic but kid-friendly) replaces Easy/Normal/Realistic. **Age band 6+** with design requirements (icon-first, short text, read-aloud, small numbers) and rating target. **Land all unlocked but overgrown**, cleared at the player's pace; animal limits = **housing capacity**. **Day-based growth** (min 1 day) replaces the Milestone 0 play-time model; one neglect rule — **3 days in a row without water/food → gone**, 2 warning days. **First beat = first order** (berries, then carrots); Butterfly Garden optional and later. Added **learning goals** (10), **co-op** rules (shared farm, merging split screen), **save versioning**, Phase-001 as a **vertical slice** (orders + chickens), 02:00 bedtime proposal, economy.md and onboarding.md links. Open: day length (Q13), season length (Q14), order premium (Q15).
- **0.21 — 2026-11-19:** Owner requests confirmed in the docs: **subscription orders** (standing daily deliveries at a set time, reminder, cancel any time, missed day = just unpaid) in §4 *Neighbour orders*; **hired helpers alongside machines** in §4 *Automation* — paid farmhands (daily wage, day off if unpaid, never debt) and befriended original garden sprites.
- **0.22 — 2026-11-19:** **Selling points ordered (owner):** automation first, **decoration second** — high concept and pillar 8 updated (decoration + farm charm).
- **0.23 — 2026-11-19:** **Starting buildings (owner decision):** the farm starts with an **old empty coop (5 hens) and old empty barn (5 stalls)**, Harvest Moon-style; Kit buys only the animals at first and can add more/bigger buildings later. First-beat want/reward, learning goal 4, buildings note, and first playable updated.
- **0.24 — 2026-11-19:** **Starting animals (owner decision):** the old coop holds **5 hens** and the old barn **1 cow**, with ≈ 7 days of feed; 500 starting coins kept. First beat's want is now coins for feed and seeds; first playable updated.
- **0.25 — 2026-11-19:** Feed is **ordered from home** (by phone — the parents left the feed seller's number) before 18:00 and delivered to the feed bins at Dawn next day; no fee.
- **0.26 — 2026-11-19:** **Day length ≈ 15 real minutes** (06:00 → 02:00; 45 s per in-game hour; one 3-hour step ≈ 2 min 15 s) and **30-day seasons / 120-day year** like Harvest Moon (owner decisions). Q13–Q14 resolved. **Kit is a boy** (characters.md).
- **0.27 — 2026-11-19:** Working title set to **Fields & Sunshine**; art primary references (Pioneers of Olive Town, Harvest Moon: Home Sweet Home) and ≈ 5-head proportions linked from §9.
- **0.28 — 2026-11-20:** **Platform: PC first, consoles later** (owner decision): Nintendo Switch and PlayStation planned after the PC release; still no mobile/tablet. §2, §8, §13 and Q5 updated; gamepad scheme must cover every action (any modern controller).
- **0.29 — 2026-11-20:** **Forced bedtime decided and moved to 01:00** (owner, during the Task 1.3 playtest review; was "02:00 (proposal)"). Playable day 06:00 → 01:00 = 19 in-game hours = 14 min 15 s at the unchanged 45 s per in-game hour. Small hours noted as dial-only.
- **0.30 — 2026-11-20:** Owner corrected the forced bedtime to **midnight (00:00)** (replaces v0.29's 01:00). Playable day = 18 in-game hours = 6 steps = 13 min 30 s at the unchanged 45 s per in-game hour. Night and Small hours are now dial-only; Night-only content (seed shed lit, Moon Petal) flagged as needing a new time.
- **0.31 — 2026-11-20:** Owner reset the day: **wake-up 04:30, forced bedtime 21:30** (replaces v0.30's midnight). Phase clock times shifted so the 3-hour steps start at the wake-up (Dawn 04:30–07:30 … Small hours 01:30–04:30). Playable day 17 in-game hours = 12 min 45 s at 45 s per in-game hour. Night/Small hours still dial-only; Night-only content still needs a new time.
- **0.32 — 2026-11-20:** Owner: the HUD time **ticks every 15 in-game minutes** (was 3-hour jumps only); phases stay 3-hour steps. Reference: 04:30 → 18:00 = 10 min 7.5 s of play.
- **0.33 — 2026-11-20:** Owner: **Harvest Moon pace, 60 real seconds per in-game hour** (was 45 s). Day 04:30 → 21:30 = 17 real minutes; 15-minute readout tick = 15 s; 3-hour phase = 3 min. Menus (e.g. the bag) stop the clock.
- **0.34 — 2026-11-20:** Owner rule: time stops for menus, conversations and shop screens, but keeps running indoors.
- **0.35 — 2026-11-20:** Owner changed the rule: **time stops indoors** too (kid tolerance), on top of menus, conversations and shop screens.
- **0.36 — 2026-11-20:** **World map redesigned as Bramblewick Vale (owner decision).** §7 now describes a hub-and-spokes valley (Kit's farm, Bramblewick Green village, Brookside & Mill Pond, Hazel Wood, Lantern Hill & Old Quarry) instead of one lane with eight places; adds the pause-menu picture map, up to three new characters per tier, and a clinic without sickness. Details in places.md v0.10.
- **0.37 — 2026-11-20:** **Farmhands first, machines later (owner decision 2026-11-20):** farmhands are hired by the day from T2 at 500 coins/day and come and go like *Harvest Moon*; the farmhand cottage is removed. Machines become the later investment.
- **0.38 — 2026-11-20:** **Machines simplified (owner decision 2026-11-20):** sprinklers need no water source; auto-feeders run for weeks from the silo or a pond hopper filled by phone orders.
- **0.39 — 2026-11-20:** **Windmill / water pump removed (owner decision 2026-11-20):** not needed for water (sprinklers have hidden pipes) and flour comes from the village watermill; hydroponics needs no power.
- **0.40 — 2026-11-20:** §4 *When time stops* notes that Phase-001 enters buildings on the same map (roof fades, clock stops) with no separate interior scenes (owner); new open question 16 on interior scenes for Phase-002.
- **0.41 — 2026-11-21:** **Order variety (owner decision 2026-11-21):** §4 *Neighbour orders* adds varied sizes, occasional mixed baskets, daily standing orders and same-day quick bonuses (bonus only, never a penalty) from the first tier, and the baker, allotment neighbour and shopkeeper posting on Marlow's board in T1.
- **0.42 — 2026-11-21:** **Q15 decided (owner 2026-11-21):** no order premium; only baskets and quick-bonus orders pay extra.
