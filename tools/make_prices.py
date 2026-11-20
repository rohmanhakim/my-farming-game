#!/usr/bin/env python3
"""Generates src/data/prices.json from docs/data/economy.csv (the price source of truth).

Usage (from the repository root):  python3 tools/make_prices.py
Only items the game uses are exported, keyed by the game's item id (ITEMS below). Re-run after
editing economy.csv; TestRunner checks that the JSON still matches the CSV.
Added in Phase-001 Task 1.6.
"""
import csv
import json
import os

ROOT = os.path.join(os.path.dirname(__file__), "..")
CSV = os.path.join(ROOT, "docs", "data", "economy.csv")
OUT = os.path.join(ROOT, "src", "data", "prices.json")

# game item id -> economy.csv "item" column
ITEMS = {
    "berries": "Wild berries (forage)",
    "carrot": "Carrot",
    "sun_sprout": "Sun Sprout (flower)",
    "egg": "Egg",
    "milk": "Milk",
    "carrot_seeds": "Carrot seeds",
    "flower_seeds": "Flower seeds",
    "hen": "Laying hen",
    "chicken_feed": "Chicken feed",
    "hay": "Hay (cow portion)",
}


def main():
    with open(CSV, newline="", encoding="utf-8") as f:
        rows = {r["item"]: r for r in csv.DictReader(f)}
    out = {}
    for item_id, name in ITEMS.items():
        r = rows[name]
        out[item_id] = {"name": name, "unit": r["unit"], "direction": r["player_direction"], "coins": int(r["price_coins"])}
    with open(OUT, "w", encoding="utf-8") as f:
        json.dump({"source": "docs/data/economy.csv", "items": out}, f, indent=2)
        f.write("\n")
    print(f"wrote {len(out)} prices to {os.path.normpath(OUT)}")


if __name__ == "__main__":
    main()
