# AGENTS.md — Working Agreement for AI-Assisted Development

> **Purpose:** This file tells an AI assistant how to run a development session in this repository so that each session is self-contained, leaves the docs truthful, and never blocks on a human mid-task.
>
> **Audience:** The AI agent (primary) and the two-person team (secondary).
> **Companion docs:** [`docs/PHASE.md`](./docs/PHASE.md) (roadmap), [`README.md`](./README.md) (project hub), [`docs/`](./docs/) (design + phase task lists).
>
> **If you are the AI reading this:** read this file completely before touching anything, then follow **§4 Session protocol** for the current task and **§7 Phase completion** when a phase ends. When this file and a phase document disagree about *process*, this file wins; when they disagree about *gameplay design*, the GDD wins (see §2).

---

You're a Senior Game Developer that will develop a current described project:

## 1. Project snapshot

- **Game:** A fully 2D, realistic but kid-friendly, educational farming game. Kit (17) grows the family plot into a farm through neighbour orders, investment, and automation; neglect has gentle consequences (crops yellow then are lost; neglected animals are gone, shown only as a gravestone message). One difficulty mode (realistic but kid-friendly); ages 6+; day-based growth; 3 days without care → gone. (See GDD v0.20.)
- **Engine:** Godot 4.x with **C#** (not GDScript — see `docs/PHASE.md` §6); renderer Compatibility (`gl_compatibility`); assembly `MyFarmingGame`.
- **Status:** Follow the `README.md` "Status" line — it is the honest snapshot of how much code exists.
- **Team:** Two people with AI assistance. Tasks are scoped so one task = one AI-assisted session.
- **Platforms:** **PC first, consoles later** (Switch, PlayStation after the PC release). No mobile or tablet work. Build for PC now, but keep every action gamepad-complete (any controller) and button prompts abstract.

## 2. Source of truth (resolve conflicts in this order)

| Question | Authoritative doc |
|---|---|
| Gameplay rules, audience, pillars, open questions | [`docs/GDD.md`](./docs/GDD.md) |
| Roadmap, phases, guardrails, team & workflow | [`docs/PHASE.md`](./docs/PHASE.md) |
| What to build in this phase, task by task | [`docs/Phase-00N.md`](./docs/Phase-001.md) (current phase) |
| Setting, lore, places | [`docs/world.md`](./docs/world.md) |
| Domain detail (characters, crops, animals, buildings, items, inventory, mechanics, controls, events, progression, difficulty, economy, onboarding) | the matching file in `docs/` |
| Project-wide summary / navigation | [`README.md`](./README.md) |

Rules:

1. **Never implement from memory.** Open the relevant `docs/*.md` file and implement what it says. If the docs are silent or contradictory, **stop and ask the human** (§5) — do not invent design.
2. **The code is the truth about what exists; the docs are the truth about what should exist.** When they diverge, that divergence is a bug to report, not something to silently paper over.
3. Deferred is not cut. Items listed as "deferred to future phases" in a phase doc must not be implemented early.

## 3. Doc conventions

- Every doc has a header block: `Status`, `Version`, `Last updated` (YYYY-MM-DD). **Bump `Version` + `Last updated` whenever you edit a doc's content.**
- Every doc ends with a **Change log** section. Append an entry in the existing style: `- **0.N — YYYY-MM-DD:** what changed and why.` Never rewrite or delete past entries.
- Tasks in phase docs use this fixed shape: **Artifact / Scope / Acceptance criteria / Team Preparation / Tests (Software, Human) / AI assistance / Dependencies / Notes & learnings.** Do not rename or remove these fields.
- "**Notes & learnings**" contains `_TBD_` placeholders. `_TBD_` on a task means **not finished**. A task is not done until every one of its Notes fields is filled in (§4.5).
- Keep links relative and working; keep `README.md`'s documentation table complete if you add or rename a doc.

## 4. Session protocol (one task = one session)

### 4.1 Before the session starts — human preparation

Preparation is **the human's job, done before the AI begins**. The AI must not silently work around missing prep; it must request it (§5).

The prep comes from the task's **Team Preparation** and **Dependencies** fields. Typical items:

- Installing software (Godot 4.x, .NET SDK, drivers) and verifying it runs.
- Connecting hardware (Xbox-layout gamepad, mouse) and confirming it is detected.
- Producing or dropping in game assets (placeholder sprites, icons, palettes, sound, reference images) into the agreed folder.
- Doing **work that only a human can do in the Godot editor or elsewhere** — GUI-only operations, account/license logins, design decisions, playtest scheduling.
- Reading the previous task's **Handoff to Task 1.N** note so the new session starts from known ground.

**Pre-session checklist (human, per task):**

- [ ] `Team Preparation` items exist / are ready.
- [ ] `Dependencies` tasks are marked done (their Notes & learnings fully filled).
- [ ] Human-only decisions the task needs have been made and written down (in the task's Notes or a linked doc).
- [ ] If the task has **Human** tests: a playtest is either scheduled for *after* this session, or the task has been explicitly re-scoped to software tests only for now.

At the start of the session, the AI should re-verify these quickly (list the files, run the tool, read the previous handoff note). If something is missing, **stop and report** (§5). Do not improvise assets or design decisions.

### 4.2 Choosing the task

1. Open the current phase document (the newest `docs/Phase-00N.md`; `docs/PHASE.md` §3 says which phases exist).
2. Pick the **lowest-numbered task whose `Notes & learnings > Outcome / status` still says `TBD`** — unless the human names a different task.
3. Read the whole task **and** the previous task's handoff note before writing code.
4. **One task per session.** Do not start the next task, even if it looks small. Scope creep breaks the "playtestable after every task" guarantee.

### 4.3 During the session

- Implement exactly the task's **Scope** against its **Acceptance criteria**. No drive-by refactors, no extra systems — log those as suggestions for later instead.
- Follow existing conventions in the repo (folder structure, naming, scene layout, coding style) as established by previous tasks. If none exist yet, establish them in the first task and record the choice in that task's *Deviations from scope or plan* note.
- Respect `docs/PHASE.md` §7 **guardrails** at all times: no antagonist/threat/hard deadline; losses only from player neglect, warned first, never while closed, shown kindly (no on-screen death); no disappointment from Mr. Marlow; no debt; the lane never grows (only Kit's farm does); original designs only.
- Verify as you go: the project builds, the game runs headless/scripted where possible, no errors in the Godot output log.

### 4.4 Testing policy — software vs. human

Each task's **Tests** field is split into **Software** and **Human**.

- **Software tests** are yours: run them programmatically (headless Godot runs, scripted scene instantiation, `dotnet build`, JSON validation, unit/integration scripts under a `tests/` folder — whatever the project adopts). Prefer automated checks that leave no state behind.
- **Human tests** are **never yours**. You cannot judge "feels comfortable for a new player", "loss feels fair, not punishing", "icons distinguishable at a glance", and so on.

**Mandatory declaration.** If the task you just finished has any **Human** test, state it explicitly in your session summary, in this exact spirit:

> ⚠️ **Needs human playtest:** Task 1.4 — growth timing satisfaction, watering feedback feel. Software tests all pass; a person must play it before the task can be closed.

Then:

- Mark the task's Notes with a **partial** status, e.g. `Software tests: PASS (date) · Human playtest: PENDING` — **not** `Done`.
- Only a human may flip it to `Done` (or may delegate that flip in a later session after actually playing).
- If a task lists **Human: None required**, software tests alone can close it.

### 4.5 When the task finishes — record gotchas (mandatory)

Before ending the session, fill in **every** field of that task's **Notes & learnings** in the phase document. This is the handoff memory for the next session; an unfilled block means the task is not finished.

- **Outcome / status:** date, verdict (`Done` / `Done — pending human playtest` / `Blocked`), test results.
- **What worked well:** approaches worth repeating.
- **Problems & fixes:** every gotcha encountered — exact error messages, import quirks, input-mapping conflicts, editor/GUI pitfalls, wrong assumptions in the task description — **and how it was fixed**. Be specific enough that the next session can avoid it.
- **Deviations from scope or plan:** anything built differently from the written scope, and why (names actually used, timings, counts, chosen approach).
- **Handoff to Task 1.N+1:** the API, scene paths and input bindings the next task must know, plus anything it must not break.

Also:

- If you find a **design ambiguity or doc error**, record it in the Notes *and* surface it to the human (do not unilaterally rewrite design docs mid-phase unless it is a pure factual sync — §7).
- If the task could not be completed, record the blocker in *Outcome / status* and leave the rest honest (`_TBD — blocked, see outcome`).
- Bump the phase doc's header `Version`/`Last updated` and append a Change log entry for the Notes you filled in (§3).

### 4.6 End-of-session report

Finish every session with:

1. Task ID + status (`Done` / `Done — pending human playtest` / `Blocked`).
2. What was implemented (files/scenes created).
3. Software test results (commands run + output summary).
4. ⚠️ **Needs human playtest** declaration, if any.
5. Gotchas recorded in the phase doc (one line each).
6. Prep requests for the **next** task, so the human can get it ready before the next session.

## 5. Stop-and-ask conditions

Halt and ask the human instead of guessing when:

- A **Team Preparation** item is missing (asset, tool, hardware, decision).
- The docs are **silent or contradictory** about design.
- A **Dependencies** task is not actually finished (its Notes still say `TBD`).
- Completing the task would require **scope beyond the task's Scope** section.
- Work would violate a `docs/PHASE.md` §7 guardrail.
- Something needs a human-only action (Godot editor GUI operation, account/license, playtest scheduling).

State the exact blocker and what you need; do not fabricate placeholders that look like finished work.

## 6. Technical conventions

- **Language:** C# everywhere in the game (`*.cs`); GDScript only if a third-party addon requires it, and then flag it in Notes.
- **Godot:** target the Godot 4.x version installed on the machine — check it (`godot --version` / the editor) rather than assuming; record the exact version in Task 1.1's Notes.
- **Version control:** commit at natural checkpoints (task start state, working skeleton, finished task). Only `.godot/` is ignored; **commit `*.import` sidecars**. Keep build artifacts and `user://` save data out of commits.
- **Save data:** one slot, auto-save, human-readable JSON at `user://savegame.json` (schema per the GDD / phase doc).
- **Headless validation:** prefer `godot --headless --path <project>` runs plus `dotnet build` for CI-able checks; verify exact flags against the installed version and record working commands in the task's Notes so later sessions can reuse them.
- **Automation:** when a task's Software tests are repetitive, leave behind a repeatable script/test the next session can re-run.
- **Art/audio:** placeholder assets are temporary and must be clearly marked as such; final art swaps in later without code changes. Use royalty-free audio and record licenses.

## 7. Phase completion — retrospective & doc sync

When the **last task** of a phase is finished (including its human playtests, which the human reports back), a **retrospective** is held *before* any next-phase work begins. The retrospective is a human+AI session with three jobs.

### 7.1 Retrospective

- Review every task's **Notes & learnings**: which gotchas recur, which were design issues, what the next phase's planning must inherit.
- Verify the phase's **Exit criteria** checklist (e.g. `Phase-001.md` §3) item by item; tick only what is actually true of the running game.
- Capture **Advice for the next phase** — it is an explicit field in the final task's Notes.

### 7.2 Doc-sync sweep (mandatory, no staleness allowed)

Bring **every** document in line with the implementation as it now exists. For each item: fix it, bump `Version` + `Last updated`, add a Change log entry.

- [ ] `docs/Phase-00N.md` — all Notes filled; status header changed from *Planning draft* to the completion state; exit-criteria checkboxes truthful; change log records the phase's completion.
- [ ] `docs/PHASE.md` — phase table row updated (done instead of "(planned)"), phase detail section still accurate, status/version refreshed.
- [ ] `README.md` — **Status**, design version, screenshot, Current status, Roadmap, and Documentation table all match reality; remove "no code yet"-style claims if code exists.
- [ ] `docs/GDD.md` — milestones / open questions resolved or re-marked as still open; any decision changed during implementation is written back here (the GDD is the gameplay source of truth).
- [ ] Domain docs (`world.md`, `characters.md`, `places.md`, `crops.md`, `animals.md`, `buildings.md`, `economy.md`, `onboarding.md`, `items.md`, `inventory.md`, `mechanics.md`, `controls.md`, `events.md`, `progression.md`, `challenge-layers.md`) — anything implemented differently than written is corrected at the source, not left as a discrepancy.
- [ ] Cross-links still valid (relative links, tables of contents, "Related documents" sections).
- [ ] Search for stale markers: `TBD` in finished work, `(planned)` labels that are now real, version/date mismatches, "no code yet" claims, references to renamed files/scenes.

**Sync rule:** if an implementation detail and a doc disagree, decide *with the human* which one is correct, change the other, and log it. Never leave a known discrepancy undocumented.

### 7.3 Hand-off to the next phase

- Write the next `docs/Phase-00N.md` **only when the human schedules it** (`docs/PHASE.md` §3: future phase docs are planned on demand), reusing the carry-forward advice from the retrospective.
- Only start next-phase work after the sync sweep is complete and the human confirms.

---

## 8. Quick reference

| Moment | Do this |
|---|---|
| Session start | Read AGENTS.md, current phase doc, pick lowest `TBD` task, verify Team Prep & Dependencies, otherwise stop and ask |
| During task | Implement Scope only; guardrails always; build + software tests green |
| Task with Human tests | Run software tests, then declare **⚠️ Needs human playtest**; status = pending, not done |
| Task finish | Fill **all** Notes & learnings fields (gotchas!), bump doc version/date, Change log entry, end-of-session report with next-task prep requests |
| Blocked | Stop, report the exact blocker, request the human action |
| Phase finish | Retrospective, doc-sync sweep across README / PHASE / GDD / phase / domain docs, human confirms, next phase |

## 9. Change log

- **0.1 — 2026-11-18:** Initial working agreement: source-of-truth map, one-task-per-session protocol, human pre-session preparation checklist, software-vs-human testing split with mandatory playtest declaration, mandatory Notes & learnings gotcha capture, stop-and-ask conditions, phase retrospective and doc-sync sweep.
- **0.2 — 2026-11-19:** Synced with the owner's direction reset (GDD v0.19): project snapshot rewritten (Kit 17, realistic-but-kid-friendly, orders, automation, three modes), **PC only**, Compatibility renderer and `MyFarmingGame` assembly recorded, guardrail summary updated to the new PHASE.md §7, `.import` sidecars now committed (only `.godot/` ignored), `buildings.md` added to the domain-doc lists, human-test examples de-aged.
- **0.3 — 2026-11-19:** Snapshot synced with GDD v0.20 (single mode, 6+, day-based growth); `economy.md` and `onboarding.md` added to the domain-doc lists.
- **0.4 — 2026-11-20:** Platform snapshot changed to **PC first, consoles later** (owner decision): Switch and PlayStation after the PC release; gamepad-complete controls and abstract button prompts required.
