# World, Setting, and Background Story

> **Status:** Brainstorming draft — story and setting proposal referenced by the GDD
> **Version:** 0.12
> **Last updated:** 2026-11-19
> **Purpose:** A working document for the background-story premise, the place, the lore, and the characters who carry it. It expands the story and setting direction in [GDD.md](./GDD.md); the GDD remains authoritative for gameplay, audience, and project decisions. World details here are proposals until validated through playtesting.

---

## 1. Premise in one paragraph

The player is **Kit, a 17-year-old** from **a city far away** who has recently come to **Bramblewick Lane** because **the family asked Kit to take care of the family's land** — the house, the front garden, and the fields behind it that have grown tangled while nobody was tending them — and, if Kit can, to turn it into a farm that pays its way. The lane itself is a short, quiet lane of small garden homes built into grassy hillsides: friendly but a little faded, with a butterfly meadow that has gone quiet and nobody quite remembering when the last lantern on Lantern Hill was lit.

**The house is a family heirloom** — kept in the family for generations, stood empty for a while, now Kit's to look after. **The parents' jobs keep them in the city**, and they asked Kit to take care of the house and land.

**The ask is trust, not a test:** the family trusted Kit with the house and land. Crops and animals can now be lost to neglect (GDD §4), but the farm itself can never be lost, and the tangled garden is the opening invitation rather than a mess anyone is blamed for. The *trust* belongs to Kit — no adult in the story does Kit's doing (Marlow's rule applies to everyone). **Ada — the parents' old friend — has been taking care of the house while Kit was raised in the city, and helps him when he arrives** ([characters.md](./characters.md) §7). The house is never empty, and Kit's responsibilities are never done for him.

At the end of the lane lives **Mr. Marlow**, an old gardener who has kept the lane's seed shed for longer than anyone can remember. He knows the name of every plant, every bird, and every story on the lane — but his knees no longer let him kneel, and his hands are too stiff for small seeds.

So he does what any wise friend would do: he waits at his gate with a seed packet, a question, and a bit of advice, and lets Kit do the doing.

The story is not about saving the world. It is about a young farmer and an old neighbour putting a small, ordinary, beautiful place back together — one crop, one order, one kind act at a time.

---

## 2. Design intent

The premise exists to serve the GDD, not the other way around. Every element below was chosen to support a specific existing requirement.

| GDD requirement | How the premise serves it |
|---|---|
| Reciprocal request anchor (want → task → reward) | Mr. Marlow holds seeds, tools, and knowledge Kit wants; Kit has young, capable hands he does not. The exchange is *mutually needed*, not a chore assignment. |
| NPC remembers unfinished tasks | He is a permanent neighbour at the end of the lane, never leaves, and is always reachable. |
| Relationship meter | The seed shed's ledger and the lane's own memory respond visibly to how the player has treated people and place. |
| Rewards that unlock new verbs/places | The lane is designed as a series of gentle locks: gate, brook (early exploration reward), stile, hill. |
| Presenter-like interactive storytelling | Mr. Marlow *is* the narrator. He addresses the player, pauses, notices what actually happened, and reacts. No disembodied voice needed. |
| Not a parent/child dynamic | He has no authority over Kit's day, no punishment, no disappointment. He is a neighbour who can be visited or not. |
| Small, readable world | One lane, eight places, no map. **Kit's farm grows** into the fields behind the house; the lane does not. |
| First beat: the first order | Marlow buys wild berries, then carrots — Kit's first coins and the order board. The missing butterflies stay as the lane's quiet mystery and an optional request. |
| A world that feels inhabited, not lonely | Trace objects (laundry, footprints, birdhouse, seed packet, unlit lantern) appear at places before their character is met. The player senses neighbours exist before reading a single line of dialogue. |

---

## 3. The setting

### 3.1 Bramblewick Lane

A curved lane, roughly the length of a short walk, running from the player's home up toward a hill. It is bordered by hedgerows, dry-stone walls, and the occasional wooden gate. Everything is within sight; the lane is the whole shared world. Behind the house lie the family's old fields — **all Kit's from the start**, but overgrown; Kit clears them at their own pace (progression.md §4).

```
        Lantern Hill (oak + old lantern post, lookout)
                  |
              [stile]  <- locked until later
                  |
      Brook ----[footbridge]---- 
                  |
   [ allotment beds / neighbours ]
                  |
  Mr. Marlow's cottage + seed shed
                  |
        --- Bramblewick Lane ---
                  |
     Player's home (round bright door)
       + front garden (starting plot, 2-3 beds)
                  |
     Fields A–E behind the house (all Kit's,
       overgrown, cleared over time)
```

### 3.2 Places

| Place | What it is | Why it exists |
|---|---|---|
| **The player's home** | A small cottage or hillside home with a brightly painted door and a front garden that starts bare or overgrown | Tutorial space, creative-ownership space, the visual "nothing is here yet" hook |
| **Bramblewick Lane** | The curved path itself, with hedgerows, walls, a stile, a gate | The GDD's "small shared or public-feeling space"; a soft world border |
| **Mr. Marlow's cottage + seed shed** | A leaning shed of jars, packets, labels, and a big hand-illustrated ledger | Task hub, seed shop, and the lane's **order board** |
| **The allotment** | A few shared beds tended by other (future) characters | Future NPC home without expanding the map |
| **The brook and footbridge** | A small crossing with flat stones and reeds | Early exploration reward; home for later requests |
| **The market stand** | A roadside table where harvests can be sold and seeds or decorations bought | Where Kit sells produce; Kit's own stall from farm tier T2 (buildings.md) |
| **The stile** | A wooden stile at the lane's upper end, leading toward Lantern Hill | Gentle world gate; reopened during lane restoration |
| **Lantern Hill** | A hill at the lane's end with a broad oak and an old, unlit lantern post | Long-arc destination; relit as the last step of lane restoration (progression.md §6.4) |

### 3.3 Visual character

- **Hobbiton-adjacent, not Hobbiton.** Grass-grown roofs, bright doors, stone walls, picket and picket-adjacent fences, laundry lines, wheelbarrows, terracotta pots, hand-painted signs. Avoid hobbit proportions, pipe-weed, and any design that reads as a copy rather than a homage.
- **Earthy base, bright accents.** Consistent with the GDD palette direction: subdued greens, browns, and stonework, with saturated flowers, doors, seed packets, and characters popping against it.
- **Everything is small and made by hand.** Nothing industrial, nothing looming, no skyline. The largest object in the world is the oak on the hill.

### 3.4 Traces of life: the inhabited lane

Before any neighbour is met, the lane shows that others live here. Trace objects are picture-only — no text, no lore to memorise:

- **Laundry on a line** by the brook → hints at the brook keeper
- **A half-painted birdhouse** leaning on an allotment bed → hints at the allotment neighbour
- **Small footprints** in the mud near the stile → hints at a friend of Kit's age
- **A seed packet** left on the player's gate → hints at Mr. Marlow (already met)
- **An unlit lantern** left on the hill → hints at the seasonal visitor

These traces answer "who lives here?" without a single word of dialogue. The world feels inhabited from day one, so Kit never feels alone — but meets only one neighbour at a time (never more than two per farm tier).

---

## 4. Lore

The lore is deliberately shallow and mostly *felt* rather than told. Players do not need history; they need a place that seems to have one.

### 4.1 The lane remembers

Bramblewick Lane is old in the way hedgerows are old — nobody planted it on purpose, it simply grew. The standing lore rule is:

> **The lane remembers what people do to it, and it answers kindly.**

This is not magic with rules the player must learn. It is a *justification for visible feedback*: gardens that look cared for, neighbours who light up, paths that seem brighter. It lets consequences be environmental and understandable rather than scored (per GDD §3).

**How the device is used (decision — §7.2): A as the base, C as the intent, B capped at two lines.**

- **A — base: pure ambience, never acknowledged.** The lane's memory appears only as *behaviour* — trace objects, light, coincidence, the lane looking brighter where care was given. No character names it, no UI explains it, no lore screen defines it. It is felt, not told.
- **C — intent: left to the player.** Because nothing is stated, a coincidence on screen becomes something to wonder about or talk over with a co-op partner — an invitation, not a prescribed answer.
- **B — capped at two lines.** Mr. Marlow may allude to it **at most twice per playthrough**, always as an open-ended observation and immediately changed subject — e.g. *"Huh. This lane has a way of remembering things, doesn't it?"* Never an explanation, never a rule, never a condition for anything.
- **The one hard rule (soft-magic discipline): the lane's memory may create wonder, but never resolve a problem.** It cannot plant, water, catch, or give for Kit; it cannot be failed, triggered, or optimised. Every reward still passes through Kit's own hands (consistent with §4.4: Marlow names the goal, Kit discovers the method).

### 4.2 The seed shed ledger

Mr. Marlow keeps a **ledger** — a big, hand-illustrated book of seed packets, plant sketches, and notes in a looping hand. It records two things:

1. **What has been promised.** If Kit accepts a task and leaves it unfinished, it stays in the ledger. This is the in-world reason the NPC can kindly restate it later — no menu, no quest log, no shame, just a line in a book he opens when they next meet.
2. **What has grown.** Completed tasks leave a sketch, a pressed flower, or a seed packet behind the page.

The ledger is the relationship UI **and** a collectible object — **decided (§7.4): it is "both", placed world-only.**

- **Where:** it lives open on the seed-shed table and is opened by interacting there (close-up view). It is **not** in the pause menu, so it cannot collide with the Collection Book or the sticker book — three book-shaped icons in one row is exactly the distinguishability risk §7.4 was about.
- **What it shows:** pictures only — sketches, pressed flowers, the seed packet from the first task. No text, no numbers, no hearts. Anyone can see a page filling up at a glance.
- **Behaviour is the real meter:** how Marlow greets you, what he recalls, what he has kept aside — the ledger confirms it, it does not replace it. (Numeric relationship meter stays deferred; see Phase-001.)
- **Validation to run anyway:** a Human playtest must confirm a player *chooses* to open it and understands what the pictures mean; if players never visit the shed, reconsider a pause-menu tab (option ii) later.
- **Prototype scope:** the ledger ships in Phase-001 Task 1.6 as a prop + picture-only close-up + a `Ledger.Note()` API; warmth-driven pages (repeated kind interactions) remain a later rule.

### 4.3 What went quiet

Years ago — nobody says how many — the lane began to slow down. Not dramatically. The butterflies stopped coming to the meadow first. Then the lantern on the hill was not lit one autumn and never relit. Then people simply stopped climbing the hill.

Nobody is sad about it exactly. Everyone is simply *used to it*. Mr. Marlow is the only one who remembers that it used to be otherwise, and his memory of *years and stories* is imperfect — which is the point, and the only place he is ever wrong (§7.3: **a+** — flavour may be wrong, instructions never). **He names the goal; Kit discovers the method.**

This is the engine of the whole story: a gentle, unsolved, entirely non-threatening mystery, revealed in small pieces through ordinary acts of care.

### 4.4 Why Kit

Kit is the one person on the lane who keeps noticing things. Kit asks "why?" and looks under the stile — not braver than anyone else, just younger, quicker, and less used to the way things already are.

This is the thematic core: **the wise neighbour has knowledge but limited physical reach; the young farmer has energy and reach, but is still learning.** Every task in the game joins those two strengths.

### 4.5 What is deliberately left vague

- No origin myth, no map beyond the lane, no named history dates.
- No explanation for *why* things went quiet until much later, if ever. It should resolve as "the lane is awake again," not as an answer to a question.
- No threat, no antagonist, no deadline. The lane did not decline because of anyone's failure.
- No rules about who (or what) the lane's memory is. Whether it is metaphor, weather, or something more is **deliberately left open and will stay open — decided (§7.2)**; it is used as an unnamed literary device only (§4.1).

---

## 5. Characters

### 5.1 Mr. Marlow — the wise friend

| | |
|---|---|
| **Role** | Neighbour, keeper of the seed shed, narrator-voice of the game |
| **Archetype** | A wise, retired gardener: a distinctive hat, patience, a gift kept in a drawer, questions instead of instructions |
| **Age feel** | Elderly, unhurried, a little deaf in one ear, bad knees, excellent memory for plants and poor one for years |
| **Home** | The cottage and seed shed at the end of the lane; always there, always reachable |
| **Voice** | Short sentences, warm, curious, mildly funny. "Ah. Now that's a thing worth looking at." |

**Rules for writing him:**

- **He never has authority over Kit.** No scolding, no disappointment, no "you should have." He reacts to what happened, not to what was expected.
- **He never does the task for Kit.** He names the goal, supplies the seed or tool, and is genuinely surprised by the method.
- **Five layers of what he knows (decided, §7.3):** **What** (goal/reward) — always says it · **Where/who** — always points, never fetches · **How** (the method) — never says it first; only on request or after the hint ladder · **Why** (the lane's memory) — never, capped at two allusions (§4.1) · **Doing it** — never.
- **He offers, never instructs.** Everything about what happens next is an invitation ("Would you like to…, or shall we…?"); declining is always fine and nothing is lost.
- **He speaks to the pair.** In co-play his invitations are phrased for both players, so they decide out loud together.
- **He may be wrong only about flavour** — years, stories, which flower is which — and the world corrects him **in the same beat, on screen**, and he laughs it off: *"No — Lace Flower! Ha, these old eyes. Your eyes are better than mine."* Actionable knowledge is never wrong. (Hint ladder timings: [characters.md](./characters.md) §8.)
- **He always needs Kit for something physical.** He cannot kneel, reach, or carry. Every request has an honest in-story reason.
- **He remembers, kindly.** Unfinished tasks are restated with no penalty: "Hmm, we didn't finish that last time. Would you like to try it now?"
- **He gives, he doesn't award.** Rewards are things he set aside *for* Kit — "I kept this packet for you" — never points handed down.
- **He is not a substitute parent.** He does not manage Kit's day. If a player never visits him, the game still works.

**Visual silhouette (to research):** Explore an original gardener's smock or wool waistcoat, a soft hat with a tucked seed packet or feather, a walking stick, and practical gardening details. Avoid recognizable character designs or signature props from existing works.

### 5.2 Kit — the player character

| | |
|---|---|
| **Role** | The one who notices, reaches, kneels, and does |
| **Identity** | A 17-year-old recent arrival from a city far away, here to take care of the family's land — **decided**; see §7 and characters.md §6 |
| **Defining trait** | Curiosity, not exceptionalism. Not "chosen," just paying attention. |
| **Naming** | **Kit** (locked; characters.md §6) |
| **Customization** | Name, appearance, and door colour are the obvious personalization levers (GDD pillar 4) |

Kit is never shamed or compared to anyone.

### 5.3 The lane itself as a character

The hedge, the brook, the oak, and the unlit lantern behave like a slow, quiet character who responds to care. This is where the GDD's "state of the shared space" carries consequence, rather than any scoreboard.

### 5.4 Future cast (not in the prototype)

Kept here so the setting grows *inward* rather than outward — new faces on the same lane, no new map:

- **The allotment neighbour** — a quieter character with a different kind of task (sharing, taking turns).
- **The brook keeper** — someone or something concerned with the water and the reeds.
- **A friend of Kit's age** — a teen peer, for friendship, festivals, and contests.
- **The baker** and **the travelling merchant** — order and trade partners (characters.md §10).
- **A seasonal visitor** — arrives only for the hill gathering; candidate for the high-relationship event in GDD §3.

---

## 6. How the story carries the first beat

> **2026-11-19:** the opening beat is now **the first order** (GDD §3): berries on Day 1, carrots by Day 4. The Butterfly Garden below remains as an **optional Spring request** later in the game.

The Butterfly Garden request, as it plays in-world:

| Beat | In-world version |
|---|---|
| Player's want | Kit sees a picture of butterflies in Marlow's ledger, or finds an old sketch of the meadow full of them, and wants to see it again. |
| NPC's request | Marlow remembers the flower patch that used to draw them. He has the seed packets; Kit has the knees. |
| Player's choice | Accept, decline, or wander off. The ledger holds the line; he asks again, gently. |
| Reward | The butterfly net — "I kept this in the shed for a long time." |
| Loop back | Butterflies return to the front garden; the meadow question opens. |

Each later reward should follow the same shape: **Marlow has been holding something, Kit's act of care is the reason it is finally given.** Soon after, Marlow opens the order board and the farm economy begins.

---

## 7. Open sub-questions inside this document

> **2026-11-19 note (direction reset, GDD v0.19):** the decisions below stand, with three updates: (a) the player is now **Kit, 17** — wording about a young child, pre-readers, the sofa, or the parent as co-player is historical; (b) the §7.5 rationale "left a 4–6-year-old alone in the house" no longer applies, but **Ada stays** as the family friend and co-op character; (c) §7.7 *"lighting it is never promised"* is **superseded**: relighting the lantern is now the optional final step of **lane restoration** (progression.md §6.4) — still never required.

1. **RESOLVED (2026-11-18, v0.5) — newcomer, and the reason is the family land.** The child comes from **a city far away** and has recently arrived because **the family asked them to take care of the family's land on Bramblewick Lane** (house + front garden + tangled beds). The family moved together; the parent/caregiver lives in the same home ([characters.md](./characters.md) §7) — the *trust* is the child's alone.
   - **Why this framing:** it makes the tangled garden diegetic (nobody was tending it), gives every first meeting a natural introduction ("You must be the new one on the lane"), and hands the child an age-appropriate honour rather than a chore.
   - **How it is shown:** visually only — boxes, a cart, the wild garden, Marlow's one-line greeting. No reading required.
   - **Guardrail:** the ask is an invitation, never a duty with consequences; nothing fails if ignored.
   - **Rejected:** the *local* framing (garden stays unexplained; first meetings aren't introductions) — see the options considered in the session report.
   - **Open thread feeding Q5:** *who* in the family asked, and whether the house is inherited, newly re-opened, or simply "your house" — decided next, with question 5 below.
2. **RESOLVED (2026-11-18, v0.6) — left unnamed, used deliberately as a literary device (A base / C intent / B capped at two lines).**
   - **Decision:** the lane's memory is **never named, never explained, never a mechanic**. It exists as *soft magic* (Sanderson) and as Todorov's *marvellous* — an event that could be ordinary or supernatural, with the narrative refusing to choose.
   - **A (base):** pure ambience — traces, light, coincidence; nobody says the word.
   - **C (intent):** left open for the child/parent to interpret together — a conversation starter, not a stated rule (GDD §10).
   - **B (capped):** Marlow may allude to it **at most twice per playthrough**, as an open-ended observation, never an explanation.
   - **Hard rule:** the memory may create wonder, never solve a problem — no failing, triggering, or optimising it; every reward passes through the child's hands.
   - **Research basis:** soft vs. hard magic systems (Sanderson); Todorov's *fantastique*; works that never name it — *My Neighbor Totoro*, *Winnie-the-Pooh*, *The Magic Faraway Tree*, *Where the Wild Things Are*; pre-reader animism.
   - **Rejected:** naming it as literal magic (creates rules the game must then obey and a lore the child must memorise — guardrail), or dropping it entirely (loses the feedback-as-consequence device of §4.1).
3. **RESOLVED (2026-11-18, v0.7) — (a+): knows everything, says only what's needed, and never decides anything.** Written as a five-layer rule: **What** always said · **Where** always pointed · **How** only on request or after the hint ladder · **Why** never (two allusions max, §4.1) · **Doing never**. Plus: he **offers, never instructs** (invitations, declining is free); he **addresses the pair** so the parent decides out loud with the child; and **(a+)** he may be wrong only about *flavour* (years, stories, names), repaired on screen in the same beat and laughed off — **actionable knowledge is never wrong**, so a child can never follow a bad hint.
   - **Hint ladder (just-in-time, no failure state):** 0 s world feedback → **~15 s** picture bubble points (no words) → leading question (*"Shall we try the seed box?"*) → plain instruction (*"Tap the bed with the seed box"*). The 15 s window exists so **in co-play the parent speaks first**; in solo play it is simply breathing room. One behaviour, two audiences.
   - **Rejected:** (b) freely-wrong mentor — a parent left overruling the game becomes technical support, not a co-explorer; and the GDD's pillar 6 (*repairing a misunderstanding*) is served by the narrow flavour carve-out instead.
   - **Research basis:** Gandalf-as-mentor studies (supplies mission, means, and warnings — never the outcome; success = the hero becomes self-sufficient), Vogler's mentor gift, and the game-writing rule that an NPC hiding *what they want* is an unfair puzzle.
   - **Write-up lives in:** §5.1 rules here, and the full ladder in [characters.md](./characters.md) §8.
4. **RESOLVED (2026-11-18, v0.8) — both, placed world-only (option i).** The ledger is simultaneously the relationship UI and a collectible object: an in-world book on the seed-shed table whose picture-only pages (sketches, pressed flowers, seed packets) *are* the meter — no numbers, no hearts, numeric meter still deferred. **Rejected:** a pause-menu tab (three book icons — ledger, Collection Book, sticker book — in one row fails the "distinguishable at a glance" test) and absorbing the Collection Book (couples two systems that ship in different phases). **Placement:** Phase-001 **Task 1.6** owns the prop, the close-up and a `Ledger.Note()` API; Tasks 1.7/1.8 call it — this closes the gap where events.md called "ledger page fills" Prototype scope but no task built it. **Validate:** Human playtest must confirm a child opens it voluntarily and reads the pictures; revisit option (ii) only if children never visit the shed.
5. **RESOLVED (2026-11-18, v0.9) — family heirloom house; parents busy in the city; child is the caretaker.**
   - **Call 1 — whose house:** a **family heirloom**, kept in the family for generations and stood empty for a while. **The parents' jobs keep them in the city**, and they asked the player to take care of the house themselves. *(Rejected: a relative's house the family mims — ownership felt borrowed; bare "your house" — lost the warmth and made "family land" mean nothing.)*
   - **Call 2 — art: overgrown, never derelict.** The house is plain and *loved* (the bright painted door stays); the garden is wild — weeds, closed gate, seed packet on the latch, boxes and a small cart at the gate. **Art note: no decay, no rubbish, no loneliness** — overgrown means *alive and waiting*, not abandoned (Totoro's rule: the mess is exciting, not sad).
   - **Call 3 — Marlow knows the family, never the player.** He knows the parents, the grandparents, and the family beyond — but the player was **born and raised in the city**, so this is the first time they meet. Effect: every first interaction is a genuine introduction; Marlow can greet warmly from knowledge (*"Your folk kept this garden, once"*) while knowing **nothing about this child** — which is exactly why every choice belongs to the player (ties to §7.3: his knowledge is wide but his authority is zero).
   - **Sub-thread RESOLVED (2026-11-18, v0.10) — day-to-day presence: Ada.** **Ada is the parents' old friend**, who has been taking care of the heirloom house while the child was raised in the city, and **helps him when he arrives**. So: parents busy in the city · the house is never empty · the child's responsibilities are never done for him (Marlow's rule extends to every adult) · Player 2's on-screen avatar is Ada, while the human in the room is usually the child's own parent (keeps GDD §2's "parent is the difficulty dial" true). *(Rejected: parent-commute model and the lone-resident/Totoro model — both left a 4–6-year-old alone in the house, which reads badly to adult gatekeepers.)* See [characters.md](./characters.md) §7.
6. **Is Mr. Marlow the right name and voice?** **RESOLVED — keep Mr. Marlow.** (see docs/characters.md §8 and change log v0.4). Alternatives not used: Ms. Fennel, Mrs. Tansy, Old Marlow, "the keeper of the shed." Herb and gardener naming gives British-countryside flavour without any recognisable source.
7. **RESOLVED (2026-11-18, v0.10) — A now (horizon), C as the direction when a later phase needs an ending.**
   - **Now:** Lantern Hill stays a **horizon** — visible from day one, locked behind the stile, the lantern left unlit as a trace object. Lighting it is **never promised** as a required feature, so no ceremony, gathering, or calendar system enters scope.
   - **Later:** when a phase actually needs an ending, the direction is **(C) a local payoff** — the emotional payoff already exists in the first beat: **the butterflies come back**; the reward is the lane coming back to life. Lantern Hill remains a *place to visit*, not the finale.
   - **Rejected:** (B) committing now to a lantern ceremony — it would drag seasons, calendars, NPC gatherings, and a set-piece into scope, which the GDD excludes from the prototype.

---

## 8. Guardrails

- No antagonist, no threat, no hard deadline. The only decay is from the player's own neglect of crops and animals — always warned first and shown kindly (GDD §4).
- No disappointment from the wise friend, ever.
- No lore the player must read or memorise to proceed.
- No expansion of the lane: grow the cast before the geography. **Kit's farm** may grow into the fields behind the house (progression.md §4).
- Original designs throughout — the direction is a *homage to a mood*, never a reproduction of anyone's characters, assets, or compositions (GDD §9).
- Every element here is a proposal. Marked items must be validated through playtesting before they are treated as decisions.
- The brook is an **early exploration reward**. The market stand opens at farm tier T2; the stile and Lantern Hill reopen through lane restoration. Nothing is parent-gated.

---

## Related documents

| Document | Relationship |
|---|---|
| [README.md](../README.md) | Centralized hub for the project. |
| [GDD.md](./GDD.md) | Authoritative for gameplay, audience, and project decisions; this document expands its setting direction. |
| [characters.md](./characters.md) | Mr. Marlow, the main character, and the parent/caregiver role carry the story in this document. |
| [places.md](./places.md) | The eight places of Bramblewick Lane described here. |
| [crops.md](./crops.md) | Starter carrots for the first order; Butterfly Garden flowers for the optional request. |
| [events.md](./events.md) | Butterfly Garden task and Lantern Hill events originate from this premise. |
| [progression.md](./progression.md) | Relationship milestones reference the ledger and high-relationship events proposed here. |
| [challenge-layers.md](./challenge-layers.md) | The single difficulty mode. |
| [buildings.md](./buildings.md) | What Kit can build on the farm. |

## 9. Change log

- **0.1 — 2026-11-17:** Initial brainstorming draft: Bramblewick Lane premise, Mr. Marlow as the wise-friend narrator, the "lane remembers" lore rule, the seed-shed ledger as a candidate relationship/unfinished-task object, six places, the Butterfly Garden mapped onto the premise, and seven open sub-questions.
- **0.2 — 2026-11-17:** Linked this story and setting proposal from the GDD and clarified the division of authority. Replaced direct character analogies with original archetype descriptions and kept character design open for research.
- **0.3 — 2026-11-18:** Added a Design intent row on "a world that feels inhabited, not lonely" and section 3.4 "Traces of life: the inhabited lane" — trace objects make the lane feel populated before any character is formally introduced.
- **0.4 — 2026-11-18:** Reconciled with places.md: place table now lists all **eight** places (renamed to "Mr. Marlow's cottage + seed shed"; added market stand and stile); recount from six to eight places throughout; added the lantern trace; guardrail clarified — brook is the early exploration reward, market stand and Lantern Hill are parent-unlocked.
- **0.5 — 2026-11-18:** **§7.1 resolved (Milestone 0, story questions):** the child is a **newcomer from a city far away**, asked by the family to take care of the **family's land** on Bramblewick Lane. §1 premise rewritten to match (tangled garden = the unattended family plot; the ask is an invitation, never a duty), §5.2 identity row set to decided, local framing rejected with reasons, open thread recorded for Q5 (who asked / how the house is explained). Research basis: Animal Crossing / Pokémon / My Neighbor Totoro newcomer frames; Stardew's "inherited plot" device without its adult-burnout premise.
- **0.6 — 2026-11-18:** **§7.2 resolved:** "the lane remembers" stays **unnamed** and is codified as a literary device — A base (pure ambience), C intent (child/parent interpret it together), B capped at two allusions by Marlow, plus the hard rule that soft magic may create wonder but never resolve a problem. §4.1 gained the device rules; §4.5's "left open" bullet now points at the decision instead of the open question. Research basis: Sanderson's soft/hard magic, Todorov's *fantastique*, Ghibli/Pooh/Faraway Tree precedents, pre-reader animism.
- **0.7 — 2026-11-18:** **§7.3 resolved — Marlow's knowledge set as "a+".** Five-layer rule (What always · Where always · How on request/after ladder · Why never · Doing never), he **offers rather than instructs**, he **addresses the pair** so the parent decides out loud, and he may be wrong **only about flavour**, repaired on screen in the same beat and laughed off (actionable knowledge never wrong). Hint ladder recorded with the deliberate ~15 s "parent speaks first" window; §4.3 and §5.1 updated to match. Research basis: Gandalf-as-mentor scholarship, Vogler's mentor gift, and the game-writing rule that an NPC hiding what it wants is an unfair puzzle. Full ladder written up in characters.md §8.
- **0.8 — 2026-11-18:** **§7.4 resolved — the ledger is "both", placed world-only (option i).** §4.2 flipped from *proposal* to *decided*: in-world book on the seed-shed table, picture-only pages that are the relationship meter, behaviour-first (numeric meter still deferred), absent from the pause menu to avoid a three-book icon row (also rejecting option iii, merging with the Collection Book). Placed in Phase-001 **Task 1.6** with a `Ledger.Note()` API for Tasks 1.7/1.8 — closing the gap where events.md called "ledger page fills" Prototype scope but no task owned it. Validation: Human playtest must show a child opens it voluntarily.
- **0.9 — 2026-11-18:** **§7.5 resolved (Q5, three calls):** the home is a **family heirloom** whose parents' jobs keep them in the city, with the player asked to take care of the house themselves; art rule **overgrown but never derelict** (no decay, no rubbish, no loneliness — "alive and waiting"); Marlow **knows the whole family but has never met the player** (city-born), so every first meeting is a genuine introduction and every choice is the child's. §1 premise rewritten accordingly; characters.md background rows and places.md home row updated. **⚠️ Open sub-thread recorded:** who is in the house day-to-day — "by himself" vs characters.md §7's co-resident parent; three reconciliation options pending (parent commutes · caregiver at the house · child resident with adults on the lane).
- **0.10 — 2026-11-18:** **§7 sub-thread and §7.7 both resolved — §7 is now fully closed (all 7 items).** *Presence:* **Ada is the parents' old friend** who kept the heirloom house while the child was raised in the city and helps him on arrival; parents stay busy in the city, the house is never empty, and no adult ever does the child's doing (Marlow's rule extended to her — characters.md §7 retitled "Adult character", ⚠️ flag cleared). *Payoff:* Lantern Hill = **horizon now (A)**, with **(C) the local payoff — the butterflies come back — as the direction** when a later phase needs an ending; committing to a lantern ceremony (B) rejected as scope poison. §1 premise updated to name Ada.
- **0.11 — 2026-11-19:** **Direction reset (GDD v0.19).** Premise now names **Kit, 17**, and adds the goal of turning the land into a working farm; the "nothing can be neglected into failure" line replaced (crops/animals can be lost; the farm never can). Map gains the **fields behind the house** (the farm grows, the lane does not). Seed shed hosts the **order board**; market stand is Kit's stall from T2; stile and Lantern Hill reopen through **lane restoration** (§7.7 "never promised" superseded, still optional). Pre-reader/sofa/parent-co-player wording replaced; future cast updated (friend of Kit's age, baker, merchant). §7 gets a reset note; §8 guardrails updated for neglect, farm growth, and no parent gating.
- **0.12 — 2026-11-19:** Owner picks: first beat is now the **first order**; Butterfly Garden kept as an optional Spring request (§2, §6); back fields all owned from the start, cleared at Kit's pace.
