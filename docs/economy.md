# Economy & Balance Sheet

> **Status:** First draft for owner review — new document
> **Version:** 0.16
> **Last updated:** 2026-11-21
> **Purpose:** Prices, income, and costs, plus a balance check showing how long each goal takes. Owner rule (2026-11-19): **what Kit sells ≈ real-world price; what Kit buys ≈ half the real price** (the kid-friendly part). **Real prices are Australian (owner decision 2026-11-20)**: a big-farm country with cheap animals and hay, which keeps hens and sheep worth buying. The full price list is the spreadsheet **[`data/economy.csv`](./data/economy.csv)** (79 items; opens in Excel/Sheets). Most real prices are estimates and must be checked before they are final (§6).

---

## 1. Currency

- **Coins.** **1 coin ≈ A$0.10** (10 coins ≈ A$1), so prices stay whole numbers kids can count. An egg is 6 coins; a pie is 180.
- Shown as a coin icon plus the number. Money never goes below 0; there is no debt.
- **Starting money:** 500 coins (≈ A$50).
- **Starting animals and feed (owner decision):** 5 hens + 1 cow in the old coop and barn, with ≈ 7 days of feed (35 chicken portions + 7 cow hay portions ≈ 210 coins' worth).

## 2. Pricing rules

**Ordering feed from home (owner decision 2026-11-19):** animal feed (grain, hay, fish food) is ordered **by phone from home**: Kit's parents left the **feed seller's number** on a note by the house phone. No shop trip needed. Orders placed **before 18:00** arrive at **Dawn next day**, straight into the **feed bins** in the coop, barn, or pond; orders after 18:00 arrive the day after. No delivery fee. Kit feeds animals by scooping from the bin. **Once a silo or pond auto-feeder exists, orders go straight into it** (owner decision 2026-11-20) and the auto-feeders feed every morning, so Kit only tops up about once a month. **Seeds, saplings, and everything else are still bought in person at Marlow's seed shed** (shop hours).

| Rule | Value |
|---|---|
| **Sell price** (shipping box; the market stall is deferred to a later phase) | ≈ real Australian retail price × 10 coins per A$. **Shipping box (owner 2026-11-21):** whatever is in the box when Kit sleeps sells at this price (with quality, seasonal and demand rules); items can be taken back out until then |
| **Buy price** (seeds, animals, feed, tools, buildings, automation) | ≈ **half** real Australian price (A$ × 5) |
| **Quality** | ★ ×1 · ★★ ×1.25 · ★★★ ×1.5 sell price |
| **Seasonal price** | Out-of-season produce sells at ×1.5 (e.g. greenhouse tomatoes in winter) |
| **Demand price** | Each item sold beyond 20 per day drops its price 2 % (to at most −30 %); recovers overnight |
| **Neighbour orders** *(decided 2026-11-21, owner)* | **Sell price + freshness bonus** (every order is taken at a neighbour's door from Phase-002), counted from the day Kit takes the order: sent in that night's box **+30 %**, next night **+20 %**, the night after **+10 %**; after that the order is simply gone (Kit keeps the goods, no other cost). Switched on with the shipping box in Phase-002; orders are sent through the box, which fills accepted orders first and sells the rest at the box price. Phase-001 has no box: plain sell price (`PriceTable.OrderPremium` 1.0) with its occasional +20 % same-day quick bonus. **Mixed baskets** pay ×1.1 on top of their lines; **daily standing orders** pay the order price each delivered day (owner 2026-11-21, built in Phase-001) |
| **Farmhand wage** | **500 coins/day** per farmhand, hired by the day from T2 (owner decision 2026-11-20). A **game wage**, below half-real (≈ 1,200), so hiring is worth it on a busy field; garden sprites are free |
| **Subscription orders** | Paid at the order price each delivered day |
| **Sell-back** | Buildings and tools sell back for half what Kit paid |

## 3. Key prices (summary — full list in the CSV)

**Kit sells (≈ real)**

| Item | Unit | Coins |
|---|---|---|
| Carrot | 1 | 4 |
| Flower (Sun Sprout) | 1 stem | 25 |
| Tomato | 1 | 9 |
| Pumpkin | 1 | 60 |
| Apple | 1 | 10 |
| Egg | 1 | 6 |
| Milk | 1 can (5 L) | 100 |
| Wool | 1 fleece bundle | 150 |
| Honey | 1 jar | 70 |
| Fish | 1 | 60 |
| Jam | 1 jar | 60 |
| Bread | 1 loaf | 50 |
| Cheese | 1 piece | 60 |
| Apple pie | 1 | 180 |

**Kit buys (≈ half real)**

| Item | Coins |
|---|---|
| Seed packet (9 seeds) | 15–25 (berry plants 135) |
| Laying hen | 175 |
| Sheep | 1,100 |
| Dairy cow | 17,000 |
| Bee colony + hive | 1,750 |
| Feed per animal-day (hen / cow / sheep / pond) | 1 / 25 / 3 / 2 |
| Small coop (5 hens) | 2,000 |
| Small barn (5 stalls) | 140,000 |
| Greenhouse | 30,000 |
| Basic / quality / deluxe sprinkler (hidden pipes included) | 400 / 1,200 / 2,500 |
| Auto-feeder | 400 |

## 4. Production assumptions

| Source | Output |
|---|---|
| Hen | 1 egg/day (rests in Winter) |
| Cow | 4 milk cans/day (≈ 20 L, realistic) |
| Sheep | 1 fleece bundle every 3 days |
| Beehive | 1 honey jar every 3 days, Spring–Autumn |
| 4 × 4 pond (10 fish) | ≈ 2 fish/day once stocked |
| Crops | Growth days in crops.md §3–§5 |

## 5. Balance check

All figures sell the raw product at ★ quality through the shipping box, with bought feed and no order premium. Processing (kitchen, T2), quality, orders, and home-grown feed all shorten paybacks: e.g. a cow whose milk becomes cheese nets ≈ 455/day instead of 375.

### 5.1 Profit per crop tile per day (before quality and orders)

| Crop | Days | Profit per tile per day |
|---|---|---|
| Carrot | 3 | 0.6 |
| Wheat (feed) | 7 | 0.5 |
| Tomato (regrows every 3 days) | 8 + 3 | 3.0 |
| Moon Petal | 6 | 4.6 |
| Pumpkin | 12 | 4.8 |
| Herbs | 6 | 5.5 |
| Sun Sprout | 4 | 5.7 |
| Berries (regrows every 6 days) | 8 + 6 | 8.3 |

### 5.2 Animals — net per day and payback

| Animal setup | Net per day (after feed) | Upfront cost | Payback |
|---|---|---|---|
| Starting 5 hens + 1 cow | 25 + 375 = 400 | 0 (given) | Earns from Day 1 |
| 5 hens in the starting coop (if bought) | 25 | 875 | ≈ 35 days |
| Extra small coop + 5 hens | 25 | 2,875 | ≈ 115 days |
| Sheep (in an existing barn) | 47 | 1,100 | ≈ 23 days |
| Beehive | 23 (3 seasons) | 1,750 | ≈ 76 days |
| 1 cow in the starting barn | 375 | 17,000 | ≈ 45 days |
| Extra small barn + 5 cows | 1,875 | 225,000 | ≈ 120 days |
| Natural pond (free, 4 × 4) + 10 fish | 118 | 80 | < 1 day (after clearing its corner of Field E) |
| New 4 × 4 pond (16 water tiles) + 10 fish | 118 | 4,080 + channel | ≈ 35 days |

### 5.3 Rough timeline (30-day seasons, 17-minute days — GDD §4)

| Goal | When (typical) |
|---|---|
| First order paid | Day 1 (forage) and Day 4 (carrots) |
| Eggs and milk income | From Day 1 (≈ 400 coins/day once feed is bought) |
| Workshop / kitchen | Late Summer – early Autumn |
| Second cow (starting barn) | ≈ Day 40–45 |
| Second barn | Year 2 |
| Sprinklers on most beds | Year 2 |
| Greenhouse, farm shop | Year 2–3 |

## 6. Balance risks (for owner review)

1. **The starting cow earns a lot — kept on purpose (owner decision 2026-11-19).** 4 milk cans a day ≈ 375 coins net from Day 1 is a generous start; crops produce nothing for the first few days, so animals carry the early income. Starting money stays 500.
2. **Animal payback.** Every animal pays back within ≈ 1–4 months (a year is 120 days). **Sheep are the fastest (≈ 23 days)** because Australian sheep and hay are cheap and wool sells well; they may become the obvious first buy. Extra buildings are slower (≈ 4 months), which fits the late game. These paybacks use the shipping-box price; neighbour orders pay +10–30 % more (freshness bonus, plus baskets ×1.1), so a farm that sends orders quickly pays back ≈ 20–25 % sooner.
3. **Flowers and berries out-earn vegetables**, because real cut-flower and berry prices are high. Fine for realism; orders can steer variety by asking for vegetables.
4. **Processing is very profitable** (a pie sells for about 3.3× its ingredients). That matches real bakeries and rewards the kitchen — keep, but watch it doesn't make raw selling pointless.
5. **Season and day length are now set** (30 days, ≈ 15 minutes): a Year-1 is ≈ 120 days ≈ 30 hours of play. Re-check the timeline in the first playtest.
6. **Farmhands first, machines later (decided 2026-11-20).** A farmhand costs 500/day with nothing upfront; machines need their system. Feeding the barn and coop by machine (silo 30,000 + 2 auto-feeders) beats a feeding farmhand after ≈ 62 days; watering a ~150-tile field (4 deluxe sprinklers incl. pipes = 10,000) after ≈ 20 days. So early on Kit hires; machines take over once money allows. The wage is a game price (half-real would be ≈ 1,200/day, more than a field earns).
7. **Hay grass sells below bought hay.** A grown bundle (= 2 portions) sells for 15, but 2 bought cow portions cost 50. Real value would make hay grass the best crop by far. Open: keep it cheap (hay is for feeding, not selling) or redefine the bundle size.
8. Real prices: milk (≈ A$2.10/L retail), hay (A$220–400/t), breeding ewes (A$160–260), sheds (≈ A$80/m² + slab), and hives (2023 prices + inflation) are checked against sources; the rest are estimates and need a check before they are final.

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
- **0.2 — 2026-11-19:** Added farmhand wage (≈ 400 coins/day, proposal) and subscription-order payment rule.
- **0.3 — 2026-11-19:** Paybacks and timeline recomputed for the **free starting coop and barn**: hens ≈ 42 days, first cow ≈ 44 days, first hens ≈ Day 2–4, first cow ≈ mid-Summer.
- **0.4 — 2026-11-19:** Starting 5 hens + 1 cow + 7 days of feed added; timeline updated; new balance risk: the starting cow's milk income from Day 1.
- **0.5 — 2026-11-19:** Owner kept the generous start: full-yield starting cow and 500 starting coins.
- **0.6 — 2026-11-19:** Feed is ordered from home before 18:00, delivered next Dawn, no fee.
- **0.7 — 2026-11-19:** Timeline based on decided 30-day seasons and 15-minute days.
- **0.8 — 2026-11-20:** Fish ponds are now **water tiles** (buildings.md §3.1, owner decision): **250 coins per wide tile** (a 4 × 4 pond = 4,000, down from 10,000; owner agreed the old price was above half-real) and **50 per channel tile**; the natural 4 × 4 pond in Field E is free. Pond payback rows updated (new 4 × 4 pond ≈ 58 days, close to hens and cows). `data/economy.csv` small/large pond rows replaced by water and channel tiles.
- **0.9 — 2026-11-20:** **Prices re-based on Australian prices (owner decision):** 1 coin ≈ A$0.10; sell ≈ real AU retail, buy ≈ half. Every row of `data/economy.csv` re-priced (column now `approx_real_price_aud`; 81 items: hay split into cow and sheep portions, berry canes became berry plants, greenhouse sprinkler upgrade added). Summary tables, crop profits, animal paybacks (cow ≈ 45 days, sheep ≈ 23, hens ≈ 35, extra barn ≈ 120), timeline and balance risks updated; new open risks: farmhand wage and hay grass sell price. Owner compared US, Australia and Japan and chose Australia because hens and sheep pay back best.
- **0.10 — 2026-11-20:** **Farmhands first, machines later (owner decision 2026-11-20):** farmhand wage set to **500/day** (game wage, below half-real), hired by the day from T2; farmhand cottage removed from the price sheet (80 items); balance risk 6 replaced by the farmhand-vs-machine payback (feeding ≈ 62 days, watering ≈ 107 days).
- **0.11 — 2026-11-20:** **Machines simplified (owner decision 2026-11-20):** sprinkler prices include hidden pipes (400 / 1,200 / 2,500; no water source needed); feed orders go straight into the silo / pond-feeder hopper once built; farmhand-vs-sprinkler payback now ≈ 20 days.
- **0.12 — 2026-11-20:** **Windmill / water pump removed (owner decision 2026-11-20):** row dropped from `data/economy.csv` (79 items); flour is milled at the village watermill.
- **0.13 — 2026-11-20:** §5.3 heading corrected to 17-minute days (GDD §4, Harvest Moon pace); the timeline counts days, so no figure changes.
- **0.14 — 2026-11-21:** Order pricing row: mixed baskets ×1.1, same-day quick bonus +20 %, daily standing orders (owner order-variety decision 2026-11-21). The ×1.25 premium is still awaiting the owner's decision.
- **0.15 — 2026-11-21:** **Order premium decided (owner 2026-11-21): none.** Normal orders pay the sell price; only mixed baskets (×1.1) and same-day quick-bonus orders (+20 %) pay extra. The paybacks in §5 were already computed without a premium, so they stand.
- **0.16 — 2026-11-21:** **Shipping box and house-visit orders (owner decisions 2026-11-21).** Sell price now names the **shipping box** (sells what is in it when Kit sleeps; items can be taken back out until then); the **market stall is deferred** to a later phase. Neighbour orders pay the sell price plus a **freshness bonus** (+30 % / +20 % / +10 % by night sent, then the order is gone; owner), switched on in Phase-002 with the box; orders are sent through the box. Balance-check notes say paybacks are box prices.
