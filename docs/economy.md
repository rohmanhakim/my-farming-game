# Economy & Balance Sheet

> **Status:** First draft for owner review — new document
> **Version:** 0.1
> **Last updated:** 2026-11-19
> **Purpose:** Prices, income, and costs, plus a balance check showing how long each goal takes. Owner rule (2026-11-19): **what Kit sells ≈ real-world price; what Kit buys ≈ half the real price** (the kid-friendly part). The full price list is the spreadsheet **[`data/economy.csv`](./data/economy.csv)** (79 items; opens in Excel/Sheets). All prices are approximate US retail and must be checked before they are final.

---

## 1. Currency

- **Coins.** **1 coin ≈ US$0.10** (10 coins ≈ $1), so prices stay whole numbers kids can count. An egg is 4 coins; a pie is 150.
- Shown as a coin icon plus the number. Money never goes below 0; there is no debt.
- **Starting money:** 500 coins (≈ $50).

## 2. Pricing rules

| Rule | Value |
|---|---|
| **Sell price** (market stall, shipping box) | ≈ real retail price × 10 coins per dollar |
| **Buy price** (seeds, animals, feed, tools, buildings, automation) | ≈ **half** real retail price |
| **Quality** | ★ ×1 · ★★ ×1.25 · ★★★ ×1.5 sell price |
| **Seasonal price** | Out-of-season produce sells at ×1.5 (e.g. greenhouse tomatoes in winter) |
| **Demand price** | Each item sold beyond 20 per day drops its price 2 % (to at most −30 %); recovers overnight |
| **Neighbour orders** *(proposal — needs approval)* | Pay **×1.25** the sell price ("farm-fresh, delivered"); bulk orders ×1.1 |
| **Sell-back** | Buildings and tools sell back for half what Kit paid |

## 3. Key prices (summary — full list in the CSV)

**Kit sells (≈ real)**

| Item | Unit | Coins |
|---|---|---|
| Carrot | 1 | 3 |
| Flower (Sun Sprout) | 1 stem | 15 |
| Tomato | 1 | 6 |
| Pumpkin | 1 | 50 |
| Apple | 1 | 8 |
| Egg | 1 | 4 |
| Milk | 1 can (5 L) | 60 |
| Wool | 1 fleece bundle | 120 |
| Honey | 1 jar | 60 |
| Fish | 1 | 40 |
| Jam | 1 jar | 50 |
| Bread | 1 loaf | 40 |
| Cheese | 1 piece | 50 |
| Apple pie | 1 | 150 |

**Kit buys (≈ half real)**

| Item | Coins |
|---|---|
| Seed packet (9 seeds) | 10–40 |
| Laying hen | 125 |
| Sheep | 1,500 |
| Dairy cow | 10,000 |
| Bee colony + hive | 2,000 |
| Feed per animal-day (hen / cow or sheep / pond) | 1 / 15 / 10 |
| Small coop (5 hens) | 750 |
| Small barn (5 stalls) | 75,000 |
| Greenhouse | 20,000 |
| Basic / quality / deluxe sprinkler | 100 / 300 / 750 |
| Auto-feeder | 300 |

## 4. Production assumptions

| Source | Output |
|---|---|
| Hen | 1 egg/day (rests in Winter) |
| Cow | 4 milk cans/day (≈ 20 L, realistic) |
| Sheep | 1 fleece bundle every 3 days |
| Beehive | 1 honey jar every 3 days, Spring–Autumn |
| Small pond (10 fish) | ≈ 2 fish/day once stocked |
| Crops | Growth days in crops.md §3–§5 |

## 5. Balance check

### 5.1 Profit per crop tile per day (before quality and orders)

| Crop | Days | Profit per tile per day |
|---|---|---|
| Carrot | 3 | 0.4 |
| Wheat (feed) | 7 | 0.4 |
| Tomato (regrows every 3 days) | 8 + 3 | 2.0 |
| Moon Petal | 6 | 3.1 |
| Sun Sprout | 4 | 3.3 |
| Herbs | 6 | 3.9 |
| Pumpkin | 12 | 4.0 |
| Berries (regrows every 6 days) | 8 + 6 | 6.7 |

### 5.2 Animals — net per day and payback

| Animal setup | Net per day (after feed) | Upfront cost | Payback |
|---|---|---|---|
| Small coop + 5 hens | 15 | 1,375 | **≈ 92 days** (≈ 69 with order premium) |
| Sheep (in an existing barn) | 25 | 1,500 | 60 days |
| Beehive | 20 (3 seasons) | 2,000 | ≈ 100 days |
| Small barn + 5 cows | 1,125 | 125,000 | ≈ 111 days |
| Small pond + 10 fish | 70 | 10,050 | ≈ 144 days |

### 5.3 Rough timeline (one 28-day season = proposed, GDD §16)

| Goal | When (typical) |
|---|---|
| First order paid | Day 1 (forage) and Day 4 (carrots) |
| Small coop + 2 hens | ≈ Day 12–15 (Spring) |
| Workshop / kitchen | Late Summer |
| First barn and cow | ≈ end of Year 1 |
| Sprinklers on most beds | Year 2 |
| Greenhouse, farm shop | Year 2–3 |

## 6. Balance risks (for owner review)

1. **Real-price animals pay back slowly.** Chickens take about 3 months of game time to pay for themselves, which is realistic but may feel slow. **Recommendation:** approve the **order premium (×1.25)** and let early neighbours mostly order eggs, so chickens feel worthwhile through orders.
2. **Flowers and berries out-earn vegetables**, because real cut-flower and berry prices are high. Fine for realism; orders can steer variety by asking for vegetables.
3. **Processing is very profitable** (a pie sells for about 3.5× its ingredients). That matches real bakeries and rewards the kitchen — keep, but watch it doesn't make raw selling pointless.
4. **Season length** (28 days proposed) and **day length** (GDD §4) change every timeline above. Lock them before tuning further.
5. All real prices are estimates and need a quick check against a real price source before they are final.

## 7. Related documents

| Document | Relationship |
|---|---|
| [README.md](../README.md) | Centralized hub for the project. |
| [GDD.md](./GDD.md) | Ways to earn; one-mode rules. |
| [data/economy.csv](./data/economy.csv) | Full price sheet. |
| [buildings.md](./buildings.md) | Building costs and capacities. |
| [crops.md](./crops.md) | Growth days. |
| [animals.md](./animals.md) | Production rates. |
| [challenge-layers.md](./challenge-layers.md) | The single difficulty mode. |

## 8. Change log

- **0.1 — 2026-11-19:** New document (owner request). Coins at ≈ $0.10; sell ≈ real retail, buy ≈ half real; quality, seasonal, demand, and sell-back rules; proposed ×1.25 order premium. 79-item price sheet in `data/economy.csv`. Production assumptions, profit per tile, animal paybacks, a Year-1 timeline, and five balance risks.
