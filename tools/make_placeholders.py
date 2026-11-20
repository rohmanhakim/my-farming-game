#!/usr/bin/env python3
"""Draws the project's handmade placeholder sprites (TEMPORARY art, project-owned).

Usage (from the repository root):  python3 tools/make_placeholders.py
Writes src/assets/placeholder/handmade/ph_<name>.png for every sprite in SPRITES below.
Pure standard library (zlib + struct): no PIL needed. Re-running overwrites the files with
identical output. Final art replaces these by name (assets/placeholder/README.md).

Added in Phase-001 Task 1.5 (earlier handmade icons were drawn by one-off scripts and are not
regenerated here). Later tasks append their sprites to SPRITES.
"""
import os
import struct
import zlib

OUT = os.path.join(os.path.dirname(__file__), "..", "src", "assets", "placeholder", "handmade")


class Canvas:
    def __init__(self, w, h, bg=(0, 0, 0, 0)):
        self.w, self.h = w, h
        self.px = [[bg for _ in range(w)] for _ in range(h)]

    def set(self, x, y, c):
        if 0 <= x < self.w and 0 <= y < self.h:
            if len(c) == 3:
                c = (*c, 255)
            self.px[y][x] = c

    def rect(self, x0, y0, w, h, c):
        for y in range(y0, y0 + h):
            for x in range(x0, x0 + w):
                self.set(x, y, c)

    def outline(self, x0, y0, w, h, c):
        for x in range(x0, x0 + w):
            self.set(x, y0, c)
            self.set(x, y0 + h - 1, c)
        for y in range(y0, y0 + h):
            self.set(x0, y, c)
            self.set(x0 + w - 1, y, c)

    def disc(self, cx, cy, r, c):
        for y in range(int(cy - r - 1), int(cy + r + 2)):
            for x in range(int(cx - r - 1), int(cx + r + 2)):
                if (x - cx) ** 2 + (y - cy) ** 2 <= r * r:
                    self.set(x, y, c)

    def line(self, x0, y0, x1, y1, c):
        n = max(abs(x1 - x0), abs(y1 - y0), 1)
        for i in range(n + 1):
            self.set(round(x0 + (x1 - x0) * i / n), round(y0 + (y1 - y0) * i / n), c)

    def png(self):
        raw = b"".join(b"\x00" + b"".join(bytes(p) for p in row) for row in self.px)

        def chunk(tag, data):
            return struct.pack(">I", len(data)) + tag + data + struct.pack(">I", zlib.crc32(tag + data) & 0xFFFFFFFF)

        return (b"\x89PNG\r\n\x1a\n" + chunk(b"IHDR", struct.pack(">IIBBBBB", self.w, self.h, 8, 6, 0, 0, 0))
                + chunk(b"IDAT", zlib.compress(raw, 9)) + chunk(b"IEND", b""))


def rng(seed):
    """Tiny deterministic random generator so output is identical on every run."""
    state = [seed * 2654435761 % 2**32 or 1]

    def nxt(n):
        state[0] = (state[0] * 1103515245 + 12345) % 2**31
        return state[0] % n
    return nxt


# ---- palette -----------------------------------------------------------------------------
GRASS = (122, 178, 92)
GRASS_D = (88, 140, 66)
WEED = (70, 120, 50)
WEED_L = (110, 160, 60)
DIRT_DRY = (176, 132, 88)
DIRT_DRY_D = (150, 108, 70)
DIRT_WET = (110, 74, 48)
DIRT_WET_D = (86, 56, 36)
LEAF = (70, 160, 70)
LEAF_D = (46, 120, 50)
YELLOW = (214, 196, 80)
YELLOW_D = (170, 150, 60)
ORANGE = (238, 132, 40)
INK = (40, 32, 28)


def soil(c, wet):
    base, dark = (DIRT_WET, DIRT_WET_D) if wet else (DIRT_DRY, DIRT_DRY_D)
    c.rect(1, 1, 30, 30, base)
    for y in (6, 13, 20, 27):
        c.rect(3, y, 26, 2, dark)
    if not wet:  # cracks on dry soil
        c.line(8, 9, 12, 11, DIRT_DRY_D)
        c.line(20, 16, 24, 18, DIRT_DRY_D)
    c.outline(0, 0, 32, 32, dark)


def soil_weeds():
    c = Canvas(32, 32)
    c.rect(0, 0, 32, 32, GRASS)
    r = rng(3)
    for _ in range(9):
        x, y = 3 + r(26), 6 + r(23)
        for dx in (-2, 0, 2):
            c.line(x, y, x + dx, y - 4 - r(3), WEED if dx else WEED_L)
    return c


def soil_clear():
    c = Canvas(32, 32)
    c.rect(0, 0, 32, 32, GRASS)
    c.rect(2, 2, 28, 28, (150, 150, 90))
    r = rng(5)
    for _ in range(14):
        c.set(3 + r(26), 3 + r(26), DIRT_DRY_D)
    return c


def soil_tilled():
    c = Canvas(32, 32)
    soil(c, False)
    return c


def soil_wet():
    c = Canvas(32, 32)
    soil(c, True)
    return c


def crop(stage, yellow=False):
    c = Canvas(32, 32)
    leaf, leaf_d = (YELLOW, YELLOW_D) if yellow else (LEAF, LEAF_D)
    if stage == 0:  # seed mound
        c.rect(12, 20, 8, 4, DIRT_DRY_D)
        c.set(14, 21, (240, 220, 160))
        c.set(17, 21, (240, 220, 160))
        return c
    h = {1: 6, 2: 11, 3: 15}[stage]
    base = 26
    if stage == 3:
        c.rect(13, base - 3, 6, 4, ORANGE)  # carrot shoulder above the soil
    for dx, lean in ((-1, -4), (0, 0), (1, 4)):
        top = base - h + (3 if yellow and dx else 0)
        c.line(16 + dx, base - 2, 16 + dx + lean, top, leaf_d)
        c.line(16 + dx + 1, base - 2, 16 + dx + lean + 1, top + 1, leaf)
        if yellow:  # drooping tips
            c.line(16 + dx + lean, top, 16 + dx + lean * 2, top + 4, leaf_d)
    return c


def roof(color, dark, w, h, ridge_vertical=False):
    c = Canvas(w, h)
    c.rect(0, 0, w, h, color)
    for y in range(3, h, 6):
        c.rect(1, y, w - 2, 1, dark)
    if ridge_vertical:
        c.rect(w // 2 - 1, 0, 2, h, dark)
    else:
        c.rect(0, h // 2 - 1, w, 2, dark)
    c.outline(0, 0, w, h, INK)
    return c


def barn_roof():
    c = roof((150, 52, 44), (110, 36, 32), 48, 40)
    c.line(6, 6, 41, 33, (235, 225, 210))
    c.line(41, 6, 6, 33, (235, 225, 210))
    return c


def floor_wood():
    c = Canvas(32, 32)
    c.rect(0, 0, 32, 32, (196, 160, 112))
    for y in range(0, 32, 8):
        c.rect(0, y, 32, 1, (150, 116, 78))
    for y, x in ((0, 10), (8, 24), (16, 6), (24, 18)):
        c.rect(x, y, 1, 8, (150, 116, 78))
    return c


def floor_straw():
    c = Canvas(32, 32)
    c.rect(0, 0, 32, 32, (214, 190, 120))
    r = rng(11)
    for _ in range(30):
        x, y = r(32), r(32)
        c.line(x, y, x + 3, y + 1, (180, 150, 80))
    return c


def ground_path():
    c = Canvas(32, 32)
    c.rect(0, 0, 32, 32, (206, 182, 132))
    r = rng(7)
    for _ in range(10):
        c.rect(r(30), r(30), 2, 2, (172, 150, 108))
    return c


def ground_overgrown():
    c = Canvas(32, 32)
    c.rect(0, 0, 32, 32, (66, 108, 54))
    r = rng(9)
    for _ in range(18):
        x, y = r(32), 4 + r(28)
        c.line(x, y, x + r(3) - 1, y - 4 - r(4), (46, 84, 40))
    for _ in range(3):
        c.disc(r(28) + 2, r(28) + 2, 2, (120, 116, 110))  # small stones
    return c


def gate():
    c = Canvas(64, 32)
    post = (110, 76, 46)
    c.rect(0, 0, 6, 32, post)
    c.rect(58, 0, 6, 32, post)
    for y in (8, 20):
        c.rect(6, y, 52, 4, (160, 118, 72))
    for x in (16, 30, 44):
        c.rect(x, 6, 3, 20, (160, 118, 72))
    c.line(6, 26, 57, 8, (130, 92, 56))
    return c


def bush(berries):
    c = Canvas(32, 32)
    c.disc(16, 18, 12, (60, 120, 56))
    c.disc(10, 14, 7, (74, 140, 64))
    c.disc(22, 13, 7, (74, 140, 64))
    if berries:
        for x, y in ((9, 12), (15, 9), (22, 11), (12, 20), (19, 18), (25, 20), (16, 25)):
            c.disc(x, y, 1.6, (150, 40, 90))
            c.set(x, y - 1, (220, 120, 170))
    c.rect(14, 29, 4, 3, (100, 70, 40))
    return c


def gate_open():
    c = Canvas(64, 32)
    post = (110, 76, 46)
    c.rect(0, 0, 6, 32, post)
    c.rect(58, 0, 6, 32, post)
    c.rect(6, 4, 4, 24, (160, 118, 72))  # gate swung back against the post
    return c


def icon_talk():
    c = Canvas(32, 32)
    c.disc(16, 13, 11, (250, 248, 240))
    c.line(9, 20, 6, 28, (250, 248, 240))
    c.line(10, 20, 7, 28, (250, 248, 240))
    c.line(11, 21, 7, 28, (250, 248, 240))
    for x in (10, 16, 22):
        c.disc(x, 13, 1.6, INK)
    return c


def icon_ledger():
    c = Canvas(32, 32)
    c.rect(3, 6, 26, 20, (120, 70, 40))
    c.rect(5, 7, 10, 17, (246, 238, 214))
    c.rect(17, 7, 10, 17, (246, 238, 214))
    c.rect(15, 6, 2, 20, (90, 50, 30))
    c.disc(10, 13, 3, (200, 80, 120))
    c.line(10, 16, 10, 21, LEAF_D)
    for y in (11, 14, 17, 20):
        c.line(19, y, 25, y, (170, 160, 140))
    return c


def icon_check():
    c = Canvas(32, 32)
    c.disc(16, 16, 13, (80, 170, 90))
    for d in (0, 1, 2):
        c.line(9, 16 + d, 14, 21 + d, (255, 255, 255))
        c.line(14, 21 + d, 23, 10 + d, (255, 255, 255))
    return c


def icon_order_board():
    c = Canvas(32, 32)
    c.rect(4, 4, 24, 20, (150, 108, 66))
    c.rect(6, 6, 20, 16, (196, 160, 112))
    c.rect(8, 8, 7, 9, (250, 248, 240))
    c.rect(17, 9, 7, 9, (250, 248, 240))
    c.disc(11, 8, 1, (210, 60, 60))
    c.disc(20, 9, 1, (210, 60, 60))
    c.rect(7, 24, 3, 7, (110, 76, 46))
    c.rect(22, 24, 3, 7, (110, 76, 46))
    return c


def marlow():
    """Mr. Marlow: wide soft hat with a seed packet, white beard, gardening smock, walking stick."""
    c = Canvas(32, 32)
    skin, smock, hat = (236, 196, 160), (120, 140, 90), (96, 120, 70)
    c.rect(12, 9, 9, 8, skin)                    # face
    c.rect(6, 6, 21, 3, hat)                     # brim
    c.rect(10, 1, 13, 6, hat)                    # crown
    c.rect(19, 2, 3, 4, (240, 200, 90))          # tucked seed packet
    c.set(14, 12, INK); c.set(18, 12, INK)       # eyes
    c.rect(12, 14, 9, 4, (240, 240, 236))        # beard
    c.rect(10, 17, 13, 11, smock)                # smock
    c.rect(10, 22, 13, 1, (96, 112, 72))
    c.rect(12, 28, 3, 3, (90, 70, 50)); c.rect(18, 28, 3, 3, (90, 70, 50))
    c.line(25, 14, 25, 31, (130, 92, 56))        # walking stick
    c.rect(23, 18, 3, 2, skin)                   # hand on the stick
    c.outline(10, 1, 13, 6, INK)
    return c


SPRITES = {
    # Task 1.5 — farm level, front garden, buildings
    "soil_weeds": soil_weeds,
    "soil_clear": soil_clear,
    "soil_tilled": soil_tilled,
    "soil_wet": soil_wet,
    "crop_carrot_seed": lambda: crop(0),
    "crop_carrot_sprout": lambda: crop(1),
    "crop_carrot_growing": lambda: crop(2),
    "crop_carrot_ready": lambda: crop(3),
    "crop_carrot_yellow": lambda: crop(2, yellow=True),
    "roof_house": lambda: roof((168, 84, 60), (128, 60, 44), 64, 32),
    "roof_coop": lambda: roof((214, 150, 70), (170, 110, 50), 32, 24),
    "roof_barn": barn_roof,
    "floor_wood": floor_wood,
    "floor_straw": floor_straw,
    "ground_path": ground_path,
    "ground_overgrown": ground_overgrown,
    "gate": gate,
    # Task 1.6 — lane, Marlow, berries, ledger, bubbles
    "berry_bush": lambda: bush(True),
    "berry_bush_empty": lambda: bush(False),
    "gate_open": gate_open,
    "roof_shed": lambda: roof((96, 130, 90), (70, 100, 66), 48, 32),
    "roof_cottage": lambda: roof((120, 96, 150), (90, 70, 116), 40, 32),
    "icon_talk": icon_talk,
    "icon_ledger": icon_ledger,
    "icon_check": icon_check,
    "icon_order_board": icon_order_board,
    "npc_marlow": marlow,
}


def main():
    os.makedirs(OUT, exist_ok=True)
    for name, make in SPRITES.items():
        with open(os.path.join(OUT, f"ph_{name}.png"), "wb") as f:
            f.write(make().png())
    print(f"wrote {len(SPRITES)} placeholder sprites to {os.path.normpath(OUT)}")


if __name__ == "__main__":
    main()
