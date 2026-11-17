# World, Setting, and Background Story

> **Status:** Brainstorming draft — story and setting proposal referenced by the GDD
> **Version:** 0.2
> **Last updated:** 2026-11-17
> **Purpose:** A working document for the background-story premise, the place, the lore, and the characters who carry it. It expands the story and setting direction in [GDD.md](./GDD.md); the GDD remains authoritative for gameplay, audience, and project decisions. World details here are proposals until validated through playtesting.

---

## 1. Premise in one paragraph

The player is a child who lives on **Bramblewick Lane**, a short, quiet lane of small garden homes built into grassy hillsides. The lane is friendly but a little faded: the gardens are tangled, the butterfly meadow has gone quiet, and nobody quite remembers when the last lantern on Lantern Hill was lit.

At the end of the lane lives **Mr. Marlow**, an old gardener who has kept the lane's seed shed for longer than anyone can remember. He knows the name of every plant, every bird, and every story on the lane — but his knees no longer let him kneel, and his hands are too stiff for small seeds.

So he does what any wise friend would do: he waits at his gate with a seed packet, a question, and a bit of advice, and lets the child do the doing.

The story is not about saving the world. It is about a child and an old neighbour putting a small, ordinary, beautiful place back together, one kind act at a time.

---

## 2. Design intent

The premise exists to serve the GDD, not the other way around. Every element below was chosen to support a specific existing requirement.

| GDD requirement | How the premise serves it |
|---|---|
| Reciprocal request anchor (want → task → reward) | Mr. Marlow holds seeds, tools, and knowledge the child wants; the child has small, capable hands he does not. The exchange is *mutually needed*, not a chore assignment. |
| NPC remembers unfinished tasks | He is a permanent neighbour at the end of the lane, never leaves, and is always reachable. |
| Relationship meter | The seed shed's ledger and the lane's own memory respond visibly to how the player has treated people and place. |
| Rewards that unlock new verbs/places | The lane is designed as a series of gentle locks: gate, stile, brook, orchard, hill. |
| Presenter-like interactive storytelling | Mr. Marlow *is* the narrator. He addresses the player, pauses, notices what actually happened, and reacts. No disembodied voice needed. |
| Not a parent/child dynamic | He has no authority over the child's day, no punishment, no disappointment. He is a neighbour who can be visited or not. |
| Small, readable world | One lane, six places, no map. |
| First beat: The Butterfly Garden | The missing butterflies are the first mystery and the first task. |

---

## 3. The setting

### 3.1 Bramblewick Lane

A curved lane, roughly the length of a short walk, running from the player's home up toward a hill. It is bordered by hedgerows, dry-stone walls, and the occasional wooden gate. Everything is within sight; the lane is the whole world for now.

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
       + front garden (2-3 beds)
```

### 3.2 Places

| Place | What it is | Why it exists |
|---|---|---|
| **The player's home** | A small cottage or hillside home with a round, brightly painted door and a front garden that starts bare or overgrown | Tutorial space, creative-ownership space, the visual "nothing is here yet" hook |
| **Bramblewick Lane** | The curved path itself, with hedgerows, walls, a stile, a gate | The GDD's "small shared or public-feeling space"; a soft world border |
| **Marlow's seed shed** | A leaning shed of jars, packets, labels, and a big hand-illustrated ledger | Task hub, reward source, non-currency economy |
| **The allotment** | A few shared beds tended by other (future) characters | Future NPC home without expanding the map |
| **The brook and footbridge** | A small crossing with flat stones and reeds | Early exploration reward; home for later requests |
| **Lantern Hill** | A hill at the lane's end with a broad oak and an old, unlit lantern post | Long-arc payoff destination; matches the GDD's deferred "ladder to a lookout" reward |

### 3.3 Visual character

- **Hobbiton-adjacent, not Hobbiton.** Grass-grown roofs, round bright doors, stone walls, picket and picket-adjacent fences, laundry lines, wheelbarrows, terracotta pots, hand-painted signs. Avoid hobbit proportions, pipe-weed, and any design that reads as a copy rather than a homage.
- **Earthy base, bright accents.** Consistent with the GDD palette direction: subdued greens, browns, and stonework, with saturated flowers, doors, seed packets, and characters popping against it.
- **Everything is small and made by hand.** Nothing industrial, nothing looming, no skyline. The largest object in the world is the oak on the hill.

---

## 4. Lore

The lore is deliberately shallow and mostly *felt* rather than told. Children do not need history; they need a place that seems to have one.

### 4.1 The lane remembers

Bramblewick Lane is old in the way hedgerows are old — nobody planted it on purpose, it simply grew. The standing lore rule is:

> **The lane remembers what people do to it, and it answers kindly.**

This is not magic with rules the child must learn. It is a *justification for visible feedback*: gardens that look cared for, neighbours who light up, paths that seem brighter. It lets consequences be environmental and understandable rather than scored (per GDD §3).

### 4.2 The seed shed ledger

Mr. Marlow keeps a **ledger** — a big, hand-illustrated book of seed packets, plant sketches, and notes in a looping hand. It records two things:

1. **What has been promised.** If the child accepts a task and leaves it unfinished, it stays in the ledger. This is the in-world reason the NPC can kindly restate it later — no menu, no quest log, no shame, just a line in a book he opens when they next meet.
2. **What has grown.** Completed tasks leave a sketch, a pressed flower, or a seed packet behind the page.

The ledger is a *candidate interface object* for the relationship meter: a child who cannot read yet can still see a page filling up. **Status: proposal; validate before building.**

### 4.3 What went quiet

Years ago — nobody says how many — the lane began to slow down. Not dramatically. The butterflies stopped coming to the meadow first. Then the lantern on the hill was not lit one autumn and never relit. Then people simply stopped climbing the hill.

Nobody is sad about it exactly. Everyone is simply *used to it*. Mr. Marlow is the only one who remembers that it used to be otherwise, and he remembers imperfectly — which is the point. **He names the goal; the child discovers the method.**

This is the engine of the whole story: a gentle, unsolved, entirely non-threatening mystery, revealed in small pieces through ordinary acts of care.

### 4.4 Why the child

The child is the one person on the lane who keeps noticing things. They ask "why?" They look under the stile. They are not braver than anyone else — they are just smaller, quicker, and less used to the way things already are.

This is the thematic core: **the wise neighbour has knowledge but limited physical reach; the young player has curiosity and reach, but is still learning.** Every task in the game joins those two strengths.

### 4.5 What is deliberately left vague

- No origin myth, no map beyond the lane, no named history dates.
- No explanation for *why* things went quiet until much later, if ever. It should resolve as "the lane is awake again," not as an answer to a question.
- No threat, no antagonist, no deadline. The lane did not decline because of anyone's failure.
- No rules about who (or what) the lane's memory is. Whether it is metaphor, weather, or something more is left open — see §7.

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

- **He never has authority over the child.** No scolding, no disappointment, no "you should have." He reacts to what happened, not to what was expected.
- **He never does the task for the child.** He names the goal, supplies the seed or tool, and is genuinely surprised by the method.
- **He always needs the child for something physical.** He cannot kneel, reach, or carry. Every request has an honest in-story reason.
- **He remembers, kindly.** Unfinished tasks are restated with no penalty: "Hmm, we didn't finish that last time. Would you like to try it now?"
- **He gives, he doesn't award.** Rewards are things he set aside *for* the child — "I kept this packet for you" — never points handed down.
- **He is not a substitute parent.** He does not manage meals, bedtime, or behaviour. If a player never visits him, the game still works.

**Visual silhouette (to research):** Explore an original gardener's smock or wool waistcoat, a soft hat with a tucked seed packet or feather, a walking stick, and practical gardening details. Avoid recognizable character designs or signature props from existing works.

### 5.2 The child — the player character

| | |
|---|---|
| **Role** | The one who notices, reaches, kneels, and does |
| **Identity** | A local child, or a recent arrival — either works; see §7 |
| **Defining trait** | Curiosity, not exceptionalism. Not "chosen," just paying attention. |
| **Naming** | Short, single-syllable, word-shape readable for pre-readers: **Kit, Wren, Tolly, Moss, Nell, Junie** |
| **Customization** | Name, appearance, and door colour are the obvious personalization levers (GDD pillar 4) |

The child is never blamed, never behind, and never compared to anyone. There is no "the other children on the lane" to measure against.

### 5.3 The lane itself as a character

The hedge, the brook, the oak, and the unlit lantern behave like a slow, quiet character who responds to care. This is where the GDD's "state of the shared space" carries consequence, rather than any scoreboard.

### 5.4 Future cast (not in the prototype)

Kept here so the setting grows *inward* rather than outward — new faces on the same lane, no new map:

- **The allotment neighbour** — a quieter character with a different kind of task (sharing, taking turns).
- **The brook keeper** — someone or something concerned with the water and the reeds.
- **A child of the lane** — a peer, for tasks that are about friendship rather than help.
- **A seasonal visitor** — arrives only for the hill gathering; candidate for the high-relationship event in GDD §3.

---

## 6. How the story carries the first beat

The Butterfly Garden task is Chapter One of this premise, not a tutorial bolted on:

| Beat | In-world version |
|---|---|
| Player's want | The child sees a picture of butterflies in Marlow's ledger, or finds an old sketch of the meadow full of them, and wants to see that. |
| NPC's request | Marlow remembers the flower patch that used to draw them. He has the seed packets; the child has the knees. |
| Player's choice | Accept, decline, or wander off. The ledger holds the line; he asks again, gently. |
| Reward | The butterfly net — "I kept this in the shed for a long time." |
| Loop back | Butterflies return to the front garden; the meadow question opens. |

Each later reward should follow the same shape: **Marlow has been holding something, the child's act of care is the reason it is finally given.**

---

## 7. Open sub-questions inside this document

1. **Is the child a newcomer or a local?** Newcomer explains the overgrown garden and gives a natural "show me the lane" tour; a local explains why everyone already knows them and makes the lane feel lived-in. **Recommend testing the newcomer framing first** because it makes the bare garden diegetic rather than unexplained.
2. **Is the lane's memory literal magic, metaphor, or weather?** Keeping it unresolved protects the gentle tone; naming it early may create rules the game must then obey. **Recommend: leave it unnamed.**
3. **How much does Marlow know?** He should know enough to point and not enough to solve. Defining exactly what he does *not* know is a writing task for the dialogue pass.
4. **Does the ledger become the relationship UI, a collectible, both, or neither?** Depends on whether a picture-book object survives contact with the fixed-tool-belt UI. Validate early — it is the one element here with interface implications.
5. **How does the child's home get explained?** Inherited, newly moved-in, or simply "your house" — affects only the opening minute, but affects art (overgrown vs. plain) and first dialogue.
6. **Is Mr. Marlow the right name and voice?** Alternatives: **Ms. Fennel**, **Mrs. Tansy**, **Old Marlow**, or a role-first "the keeper of the shed." Herb and gardener naming gives British-countryside flavour without any recognisable source.
7. **Does the eventual payoff stay on Lantern Hill?** It matches the GDD's deferred lookout reward, but the ceremony/lantern idea touches seasons and calendars, which are explicitly out of prototype scope. Keep as horizon, not mechanic.

---

## 8. Guardrails

- No antagonist, no threat, no deadline, no decay caused by the player.
- No disappointment from the wise friend, ever.
- No lore the child must read or memorise to proceed.
- No expansion by map-size: grow the cast before the geography.
- Original designs throughout — the direction is a *homage to a mood*, never a reproduction of anyone's characters, assets, or compositions (GDD §9).
- Every element here is a proposal. Marked items must be validated through playtesting before they are treated as decisions.

---

## 9. Change log

- **0.1 — 2026-11-17:** Initial brainstorming draft: Bramblewick Lane premise, Mr. Marlow as the wise-friend narrator, the "lane remembers" lore rule, the seed-shed ledger as a candidate relationship/unfinished-task object, six places, the Butterfly Garden mapped onto the premise, and seven open sub-questions.
- **0.2 — 2026-11-17:** Linked this story and setting proposal from the GDD and clarified the division of authority. Replaced direct character analogies with original archetype descriptions and kept character design open for research.