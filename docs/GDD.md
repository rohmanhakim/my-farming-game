# Game Design Document

> **Status:** Brainstorming draft
> **Version:** 0.12
> **Last updated:** 2026-11-17
> **Purpose:** A living document for shaping the game together. The concept and audience below come from the initial brief; unconfirmed details are proposals, not commitments.

## 1. High Concept

A gentle, fully 2D life-and-garden game for young children. Players care for a small garden, explore beyond home, and help friendly characters with everyday tasks. Farming, exploration, and community interactions work together as parts of one cozy world. The presentation uses illustrated, non-pixel-art graphics.

The game takes inspiration from the welcoming farming and community feel of *Stardew Valley*, while aiming for a much smaller scope and an experience designed specifically for children from pre-elementary through first grade.

**Working title:** TBD

## 2. Audience and Play Context

- **Primary audience:** Children from pre-elementary through first grade.
- **Intended play modes:** A child can play alone or with a parent. Parent-child play is designed for collaboration on one device.
- **Parent/caregiver role:** Collaborate with the child in real time when using separate PC gamepads; touchscreen co-play can use turn-taking.
- **Working literacy assumption:** The intended player can talk and is beginning to read or can read simple text. This needs validation because reading and language abilities vary across the target age range.
- **Likely considerations:** Players may be new to games, and some may benefit from visual cues, short text, or spoken support even if reading is an intended part of the experience.
- **Prototype platform:** PC.
- **Longer-term platform goal:** Multiplatform, with iPad and Android tablets as the priority touchscreen devices.

Text can be part of the experience, but essential actions and feedback should not depend on fluent reading.

## 3. Design Goals

### Design pillars

1. **Easy to understand:** A child can see what to do and learn the controls by trying them.
2. **Kind and low-pressure:** Mistakes are safe; the game does not punish a child for taking a break or doing things in an unexpected order.
3. **Small, satisfying activities:** Short actions produce visible, cheerful results.
4. **Creative ownership:** Players can personalize a garden or space without needing to optimize it.
5. **Warm illustrated world:** Expressive characters, clear shapes, and inviting environments support the experience.
6. **Practice everyday social skills:** Through ordinary interactions, players can explore communication, cooperation, boundaries, and caring for shared spaces.

### Intended emotional experience

Players should feel curious, capable, welcomed, and proud of what they have made. Social situations should invite reflection without making children feel judged or ashamed.

### Social learning approach

The game should let players practice interactions that can help them navigate life beyond home, such as greeting someone, asking before taking or using something, taking turns, offering help, sharing, and responding when another character has a different need or preference.

Some situations may offer simple choices and show understandable consequences. Consequences should be proportionate, connected to what happened, and focused on how people or the shared environment respond. When possible, players should be able to repair a misunderstanding or try a kinder approach. Avoid rigid good/bad scoring, shame, or presenting complex moral questions as having one simplistic answer.

### Social interaction anchor: reciprocal requests

The shared pattern is that the player wants something and helps another person to get it. The specific request and task can differ each time and should fit the character, situation, and desired reward.

1. The player expresses interest in or asks for something.
2. An NPC explains a clear task that would help them and what the player can receive for doing it.
3. The player chooses whether to accept and completes the task, or leaves it unfinished.
4. On completion, the NPC responds and gives the promised reward. Rewards should vary with the task and may include the thing the player wanted.

This is a social exchange, not a hidden test: the player should understand the request and expected reward before agreeing. Tasks should be achievable, with outcomes based on the player's actions rather than confusing rules or arbitrary timers.

If a task is left unfinished, the player does not receive its promised reward yet. When they next speak with that NPC, the NPC can remember and kindly restate the request, for example: "Hmm, we didn't finish that task last time. Would you like to try it now?" Taking a break or closing the game is not itself a failure. The player should be able to accept the task again or decline without shame or loss of unrelated progress.

### First want-task-reward beat: The Butterfly Garden

**Status:** Proposed answer to open question 1; validate through playtesting.

The first request should teach the reciprocal-task pattern using actions the player already knows, then reward the player with a new ability that opens another kind of play.

| Beat | Proposal |
|---|---|
| Player's want | The player sees butterflies in a picture bubble and wants them to visit the garden. The desire is communicated visually before any reading is needed. |
| NPC's request | The NPC offers flower seeds and asks the player to plant and water three flowers so butterflies will come. |
| Player's choice | The player may accept, decline, or leave the task unfinished. The NPC remembers and kindly offers it again later. |
| Reward | The NPC gives the player a butterfly net, unlocking a new activity: finding and catching friendly butterflies or bugs. |
| Loop back | The player can show a discovered butterfly to the NPC or release it in the garden, where butterflies add life to the space. |

The request uses already-known verbs (plant and water), states the expected reward before acceptance, and makes the desired outcome visible. The net adds a new verb rather than simply increasing a currency or item count.

#### Starting loadout and item handling

- **Hands:** Available from the start for planting, harvesting, decorating, talking, and general interaction; these are actions, not inventory items.
- **Watering can:** Starts with the player so the initial garden loop is usable.
- **Seed box:** Starts with two or three basic seeds. Flower seeds for the first task are supplied by the NPC when the player accepts it.
- **Shovel or trowel:** Not part of the initial loadout; reserve digging as a possible later ability or reward.
- **Future tools:** A bug net, ladder, sprinkler, or similar tool should be introduced only when it opens a distinct new verb or place.

Avoid a loose, sortable inventory. The proposal is a fixed tool belt with large icon slots for the watering can, seed box, and one earned tool. Harvests go automatically to a garden shelf rather than accumulating in a large player inventory.

#### Design-pattern references

These games were reviewed as references for pacing the first task and reward, not as templates to reproduce:

| Reference | Broad pattern considered |
|---|---|
| *Stardew Valley* | A simple introduction to a character precedes a later gift that unlocks fishing. |
| *Animal Crossing: New Horizons* | Short, explicit requests and workshops introduce tools and new capabilities. |
| *Cozy Grove* | Small, untimed help requests connect to character stories and gradual world expansion. |
| *Ooblets* | Stated goals and rewards can reopen places or systems. |
| *Garden Story* | Villager requests can improve a community and unlock useful rewards. |
| *Spiritfarer* | Helping characters can lead to new facilities and mechanics. |

The shared principle is to begin with tools for verbs the player already has, then reward them with tools or access that introduce new verbs or places.

#### Later alternatives

- **Ladder to a lookout:** Help grow and harvest vegetables for an NPC's picnic, then receive a ladder that opens a treehouse lookout. Keep this as a possible reward when a second area is introduced.
- **Garden helper:** A creature or sprinkler waters a bed. Defer this because it automates an existing action instead of opening a new activity.

### NPC relationship meter (proposal)

Track a separate social relationship value for each NPC. Keep it always visible in a subtle way in that NPC's dialogue box. Change it only in response to specific tasks or dialogue events; it does not change passively over time. The meter represents the history of that relationship, not a moral grade.

At higher relationship levels, an NPC may offer something special during selected events. Rewards could include unique items that cannot be obtained through ordinary gameplay, access to a special place, special dialogue, or a memorable event in itself. A seasonal celebration such as New Year is one possible occasion. The exact events, thresholds, and reward mix are TBD.

## 4. Core Experience

### Proposed core loop

1. Decide what the player would like to do, find, or unlock.
2. Explore, care for the garden, or talk with a character to find a way forward.
3. Help someone by completing a clear task in exchange for an agreed reward.
4. See an immediate response and receive the reward; some rewards introduce a new activity, area, or mechanic.
5. Enjoy the new possibility, continue exploring, or choose another goal.

This loop is a starting hypothesis and should be tested before expanding the feature set.

### Progression and novelty

Rewards should not be limited to currency or more items. Early rewards should introduce new possibilities that change how the player can interact with the game, such as a new activity, tool, place, or mechanic. Introduce these one at a time with clear, playful explanation. Avoid making progression depend on collecting and managing a large inventory.

### Gardening without chores

Gardening is a playful option, not a maintenance obligation. Crops should not die, spoil, or create a backlog that pressures the child to return and keep everything in order. Keep planting, growth, and harvesting simple, with useful feedback and rewards that do not require managing large quantities of produce or supplies.

### Plant growth model (proposal)

**Design principle:** The garden always moves forward, and the player's care makes it sparkle.

- Plants advance through growth stages while the game is being played, so the child can see progress and connect it to activity in the garden.
- Watering is optional for plant survival and growth. It speeds up growth and gives cheerful immediate feedback, but plants continue to grow without it. A specific NPC task may ask the player to try watering as an activity; this must never imply that plants will suffer if left unwatered.
- Plant stages and feedback should be quick enough to make progress visible during a play session. Exact timing is TBD.
- Plants never wither, die, or become worse because the child stopped playing.
- Consider a small, bounded amount of positive growth while the game is closed, so returning after a break can bring a pleasant surprise without creating a real-time schedule or daily obligation.
- Keep the initial garden small (for example, two or three beds) so tending it remains play rather than field maintenance.
- Give watering an immediate response—such as a sparkle, sound, or visiting insect—even when a full growth-stage change takes longer.

For the Butterfly Garden task, planting and watering flowers should create visible progress in-session. The arrival of butterflies can be a short, clearly signaled event rather than a long idle wait.

**Prototype question:** Test whether plants should pause completely while the game is closed or receive bounded catch-up growth.

### Session shape

**Proposal:** Make it easy to have a satisfying short play session, while allowing children to continue when they want. There should be no required daily check-in or penalty for leaving.

**Target session length:** TBD; validate through playtesting.

### Parent-child co-play

The child can play alone, and no activity should require a parent to be present. When playing together on PC, the child and parent use separate gamepads and collaborate in real time. On a touchscreen device, they can take turns on the same screen. Tasks should work in solo play and remain flexible enough for either real-time collaboration or touch turn-taking, without making simultaneous input essential. Activities should create opportunities to talk, make choices, and solve small problems together. Online play is not required.

## 5. Player Activities

### MVP proposal

- Play solo or collaborate with a parent on the same device, discussing choices and solving small problems.
- Move around one small home or garden area.
- Interact with garden beds.
- Choose from two or three seeds.
- Plant and water crops using simple controls.
- See crops change as they grow.
- Harvest crops.
- Complete the proposed Butterfly Garden task: plant and water three flowers, then receive a butterfly net that unlocks a new activity.
- Show or release butterflies in the garden as a simple way to connect the new activity back to the world and NPC.
- Use a fixed tool belt with large icon slots; route harvests to a garden shelf rather than a loose inventory.
- Use a small set of decorations to personalize the space.
- Save progress locally.

**Starting loadout:** The player begins with hands, a watering can, and a seed box holding two or three basic seeds. The first accepted NPC task supplies its own flower seeds. Tools that unlock new verbs are earned through play.

### Possible later activities

- A second small area to explore.
- More plants, characters, and decorations.
- Simple giving or sharing interactions.
- More structured parent-child cooperative activities that also work in solo play.
- More ways to express creativity, such as arranging a garden or choosing colors.

These are ideas, not promised features.

## 6. Rules and Difficulty

### Starting principles

- Do not use crop death, debt, lost belongings, shame, or harsh failure states in the initial design.
- Do not require fast reactions, precise timing, or reading-heavy instructions.
- Let children experiment; make important actions reversible where practical.
- Give clear feedback when an action succeeds, is unavailable, or needs another step.
- Keep any resource or inventory system small and understandable.
- Make gardening forgiving and optional; do not make crops a recurring chore or require players to keep large quantities of items organized.
- Use early rewards to introduce new ways to play, not only to increase item counts.
- Show social consequences through character responses and the state of the shared space, rather than abstract punishment or moral scores.
- Let players revisit a social interaction, make amends, or try another response where practical.
- Make task expectations and outcomes clear; do not make a child responsible for an outcome they could not understand or reasonably control.
- Do not take away unrelated progress or make the player lose something already earned because a task remains unfinished.

### Open design questions

- How short should in-session growth stages be, and should there be bounded growth while the game is closed?
- Is there any in-game clock, and if so, does it affect play?
- What, if anything, should be earned or unlocked?

## 7. World and Characters

### Initial scope proposal

- One cozy home or garden hub.
- One small nearby area, if needed to make exploration part of the first prototype.
- One friendly character with a clear, achievable responsibility task.

The world should be readable at a glance and small enough that children can navigate it without a map. Include a small shared or public-feeling space, such as a path or community garden, to support interactions outside the home.

**Setting and background-story proposal:** See [world.md](./world.md). It introduces Bramblewick Lane, Mr. Marlow, and the mystery of the quieting lane. Specific character designs and open world details remain proposals for research and playtesting.

## 8. Controls and Interface

### Control goals

- Keep the number of buttons and actions low.
- Make the primary interaction consistent throughout the game.
- Use large, easy-to-select targets if touch is supported.
- Avoid requiring simultaneous or complex button combinations.
- Offer a clear way to pause and reach settings.

### Proposed interaction model

Movement plus one primary interaction action. Seed selection and other choices should use a simple visual menu.

For the PC prototype, offer two selectable control schemes: gamepad and fully mouse-controlled play. Parent-child PC co-play uses two separate gamepads. Design the all-mouse interactions around direct pointing and clicking so they can later map naturally to touchscreen input without changing the underlying activity. The gamepad scheme should use a small, consistent set of buttons and feel familiar to players of farming games on Nintendo Switch, simplified for this game's smaller scope.

Prioritize iPad and Android tablets for touchscreen adaptation. Touch co-play uses turn-taking, while all tasks remain playable solo.

**Exact PC mappings, tablet layout, and how flexible tasks support solo, two-gamepad, and touch turn-taking play:** TBD.

## 9. Art Direction

- Fully 2D presentation; no pixel-art style.
- Colorful, cartoony European children's-book and comics-inspired illustration, with clear inked outlines, expressive faces, lively poses, and readable scenery.
- Friendly and expressive, but not chibi; avoid exaggerated chibi proportions.
- Keep shapes and scenes clear rather than realistic or visually busy.
- **Palette direction:** A hybrid of lively, saturated, high-contrast garden colors and more earthy, subdued environmental tones. Use bright accents to make plants, characters, and interactive objects pop against a slightly moodier world.
- **Camera direction:** An angled top-down / three-quarter view that shows the ground plane clearly while using overlap and simple depth cues to make the world easy to read.
- Use clear silhouettes and strong contrast between interactive objects and backgrounds.
- Make crops and characters expressive enough to communicate state without text.
- Build a consistent visual language for things children can interact with.

Develop an original visual identity. Use broad color and camera-direction references only; do not copy any particular game's distinctive character designs, assets, settings, compositions, or exact visual style. Original character design needs further research before it is defined.

**Palette refinement, camera implementation, animation complexity, production pipeline, and original character research:** TBD.

## 10. Story, Dialogue, Audio, and Text

- Include a background story that gives the world and its characters context; it need not interrupt the core activities.
- Use simple, child-friendly dialogue with short sentences and familiar vocabulary.
- Use an interactive, presenter-like storytelling approach: characters can address the player, ask a clear question, leave a pause for the child to answer or act, and respond to what the player does.
- Use repetition, visual clues, and small mysteries or problems to help children follow along.
- The story should support the game's focus on responsibility and everyday social learning, while inviting parent-child conversation rather than prescribing a single moral answer.
- This guided, interactive story woven through a cozy life-and-garden game is a key point of difference.

- Use gentle music and sound effects to reinforce actions and progress.
- Avoid startling sounds and provide independent volume controls where practical.
- Make important feedback understandable without relying on sound alone.
- Keep essential instructions short and pair them with visuals.
- Consider optional spoken prompts if they improve accessibility and fit the project scope.

The proposed premise and cast are documented in [world.md](./world.md). Dialogue presentation, voice, localization, music style, and text-reading support remain TBD.

## 11. Accessibility and Child-Friendly UX

Design and testing should consider:

- Players who are not yet fluent readers.
- Large text and clear icons.
- Forgiving interaction and movement.
- Adjustable sound and music.
- Visual feedback for actions that also have audio feedback.
- A calm experience without time pressure or punitive reminders.
- Caregiver-friendly access to settings and information.

Specific accessibility requirements should be refined alongside platform and playtest decisions.

## 12. Safety, Privacy, and Caregiver Considerations

- Avoid collecting personal information unless there is a clear, necessary reason.
- Do not include open chat or user-generated public content in the initial scope.
- Make purchases, online features, and data practices explicit to caregivers if they are ever introduced.
- Establish an appropriate child-privacy and age-rating review before release.

Online features and monetization are not currently in scope.

## 13. Technical Direction

The prototype targets PC first. The longer-term goal is multiplatform support, including touchscreen devices.

**Engine and implementation approach:** TBD. Evaluate candidates for 2D workflow, PC prototyping, future touch input, and eventual multiplatform deployment.

The first technical prototype should answer the basic interaction questions—movement, planting, watering, growth feedback, and saving—before substantial content production begins.

Potential engine candidates can be evaluated against the chosen platform, the team's experience, accessibility needs, and the desired art workflow.

### Development approach

- **Project type:** Passion project with no predetermined overall content or feature cap.
- **Core team:** Two people, with AI assistance.
- **Art and audio resources:** Free royalty-free assets may be used. Check and record each asset's license, attribution requirements, and suitability for the intended distribution before use.

The project can grow over time, but work should still be prioritized in small, testable milestones so the core experience remains playable and coherent.

## 14. Prototype and Validation Plan

### First prototype

Build one small playable scene with:

- A controllable character.
- One garden bed, basic seeds, and the flower seeds supplied for the first task.
- Planting and watering interactions.
- A visible growth or progress response.
- The NPC's picture-bubble want and clear request to plant and water three flowers.
- The butterfly net as the task reward, unlocking a simple butterfly-finding interaction.
- A garden shelf that receives harvests automatically.

Use placeholder art if needed. The prototype is for validating the activity and controls, not for demonstrating final visual quality.

### Playtesting questions

- Can a child start playing with little or no spoken explanation?
- Can they tell what objects can be interacted with?
- Do they understand how to plant, water, and finish the activity?
- Are controls comfortable for the target input device?
- Do they enjoy the activity enough to repeat or explore?
- Can they understand the NPC's visible want, the task, and the promised reward before accepting?
- Does earning the butterfly net feel like a meaningful new way to play?
- Can the child understand the character's feelings or needs from dialogue, visuals, or voice?
- Does the outcome of a social choice feel understandable and fair, without feeling punitive?
- Does the child water plants because they want to, rather than because they think they must?
- When a plant grows without being watered, does the child still feel ownership and understand what happened?
- After a break, can the child read the plant's state as positive progress without needing an adult to explain it?
- Do caregivers find setup and settings understandable?

Playtests with children should be conducted with appropriate adult supervision and consent.

## 15. Initial Prototype Scope

These are not ruled out for the passion project; they are deferred from the initial prototype so the core experience can be tested first:

- Large maps or many locations.
- Complex crafting, inventory, or economy systems.
- Seasons and detailed calendars.
- Combat or dangerous failure.
- Online multiplayer and public social systems.
- Long, reading-heavy dialogue or exposition.

## 16. Open Questions for Brainstorming

1. How short should in-session growth stages be, and should plants receive bounded growth while the game is closed?
2. Which tasks or dialogue events should change an NPC's relationship, and by how much?
3. Which special events, places, dialogue, or unique items should high relationships unlock?
4. How should a parent and child share tasks while collaborating with two gamepads or taking turns on a tablet?
5. Which PC gamepad and mouse mappings should the prototype use, and what tablet layout works best?
6. What research and visual exploration are needed to create original characters that fit the non-chibi European cartoon-comics direction?
7. Which open story and setting questions in [world.md](./world.md) should be decided next?

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
