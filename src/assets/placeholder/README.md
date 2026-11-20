# Placeholder art (temporary)

Everything in this folder is **temporary placeholder art** and must not ship in a release.

- `kenney/` — selected 16×16 tiles from Kenney's **Tiny Town** and **Tiny Dungeon** packs, **CC0 1.0** (licenses alongside, details in `../LICENSES.md`). Files are renamed `ph_<sprite name>.png`.
- `handmade/` — simple icons drawn for this project in Task 1.3 (`bed`, `clock_sun`, `clock_moon`, `icon_sleep`, `season_spring`) and Task 1.4 (`tool_hands`, `icon_bag`, `item_berries`, `item_carrot`, `item_egg`, `item_milk`, `item_carrot_seeds`). Project-owned, also temporary. From Task 1.5 on, handmade sprites are drawn by `tools/make_placeholders.py` (re-run it to regenerate): soil, carrot stages, roofs, floors, path, overgrown field, gate.
- Code never loads these by path. It asks `SpriteLibrary.Get("<sprite name>")` (`scripts/core/SpriteLibrary.cs`), or uses a `NamedSprite` node with `SpriteName` set.

**Replacing with final art:** drop `<sprite name>.png` (any resolution) into one of `assets/art/{characters,world,ui,items,vfx}/`. The library prefers final art over `ph_` files, and `NamedSprite.DisplaySize` keeps the on-screen size, so no code or scene changes are needed. Then delete the matching `ph_` file.

Sprite names without a placeholder image (`hen`) draw as a flat coloured square until art exists.
