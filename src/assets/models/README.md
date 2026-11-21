# Real 3D models (by name)

Drop a real model here and it replaces the placeholder primitive with the **same name**, with no code
or scene change (project-structure.md §9, art-audio.md §1b).

- `characters/` — Kit, Marlow, Ada, animals (`kit.glb`, `marlow.glb`, …)
- `world/` — buildings, props, ground pieces (`house.glb`, `berry_bush.glb`, …)
- `items/` — things Kit holds or that lie on the ground

Rules:

- glTF binary (`.glb`) or a Godot scene (`.tscn`) named exactly like the placeholder entry in
  `res://data/placeholder_models.json`.
- 1 unit = 1 m; the origin sits at the **feet / ground centre**; the model faces **−Z** (north).
- Low-poly: Kit ≈ 3–5k triangles, props ≤ 1–2k, a building ≤ 3k (project-structure.md §16).
- Add a row to `src/assets/LICENSES.md` the moment a file lands (Kenney CC0, team-made, or AI-generated
  with its licence terms).

Empty until real models arrive (Phase-001 uses primitives only).
