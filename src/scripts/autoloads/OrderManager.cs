using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MyFarmingGame.Core;

namespace MyFarmingGame.Autoloads;

/// <summary>
/// Neighbour orders (autoload <c>OrderManager</c>; Tasks 1.6–1.7, GDD §4 "Neighbour orders",
/// order variety per the owner, 2026-11-21).
///
/// * Marlow's scripted first order (3 handfuls of wild berries) opens the board; he then pins
///   the scripted "5 carrots". Payment always happens exactly once per delivery.
/// * Every Dawn the board is topped up to <see cref="MaxBoardOffers"/> open orders, drawn only from
///   what Kit can produce now (<see cref="Production.Supply"/>), never more than the farm supplies:
///   <b>varied sizes</b> (1 … a little above the usual size), now and then a <b>mixed basket</b>
///   (2–3 items, basket bonus), a <b>quick bonus</b> (extra coins if delivered today — never a
///   penalty), and the same item may appear twice in different sizes.
/// * <b>Neighbours</b> (<see cref="Neighbours"/>): the baker, the allotment neighbour and the
///   shopkeeper start posting, one at a time, after Kit's first eggs, first delivered crop order and
///   5 delivered orders. Each orders what they like.
/// * After <see cref="RepeatAfterDeliveries"/> deliveries a neighbour offers a <b>repeat</b> order;
///   after <see cref="StandingAfterDeliveries"/> one who likes eggs or milk offers a <b>daily
///   standing order</b> — subscribe, deliver once a day, cancel any time; a missed day isn't paid.
/// * No deadlines and no penalties: an unaccepted order waits; cancelling returns it to offered.
/// </summary>
public partial class OrderManager : Node
{
    public static OrderManager Instance { get; private set; }

    public const string FirstOrderId = "marlow_berries";
    public const int FirstOrderCount = 3;
    public const string SecondOrderId = "marlow_carrots";
    public const int SecondOrderCount = 5;

    /// <summary>Open (offered or accepted) orders the board shows at most.</summary>
    public const int MaxBoardOffers = 5;
    /// <summary>Deliveries to one neighbour before they offer a repeat order.</summary>
    public const int RepeatAfterDeliveries = 3;
    /// <summary>Deliveries to one neighbour before they offer a daily standing order.</summary>
    public const int StandingAfterDeliveries = 4;
    /// <summary>Order size limit at farm tier T1 (progression.md §3: 1–6 items).</summary>
    public const int MaxOrderCount = 6;
    /// <summary>Share of new orders that are mixed baskets (rare at T1).</summary>
    public const double BasketChance = 0.2;
    /// <summary>Share of new orders with a quick bonus.</summary>
    public const double QuickChance = 0.25;

    [Signal] public delegate void OrderChangedEventHandler(string id);
    [Signal] public delegate void OrderPaidEventHandler(string id, int coins);
    [Signal] public delegate void BoardOpenedEventHandler();
    /// <summary>A neighbour pinned their first order. Argument: neighbour id.</summary>
    [Signal] public delegate void NeighbourIntroducedEventHandler(string id);

    private readonly List<Order> _orders = new();
    private readonly Dictionary<string, int> _delivered = new();
    private readonly HashSet<string> _neighbours = new();
    private readonly HashSet<string> _milestones = new();
    private GameClock _clock;

    public IReadOnlyList<Order> Orders => _orders;
    public bool BoardOpen { get; private set; }
    public int Seed { get; set; } = 1;
    public IReadOnlyDictionary<string, int> Delivered => _delivered;
    /// <summary>Neighbours who post orders (Marlow from the start).</summary>
    public IReadOnlyCollection<string> ActiveNeighbours => _neighbours;
    /// <summary>Things that happened once and may bring a neighbour ("produced:egg", "delivered:carrot").</summary>
    public IReadOnlyCollection<string> Milestones => _milestones;

    public int DeliveredBy(string giver) => _delivered.TryGetValue(giver, out var n) ? n : 0;
    public int TotalDelivered => _delivered.Values.Sum();
    private int Today => TestDay ?? _clock?.Day ?? 1;

    /// <summary>Tests: the day used when there is no clock (private instances).</summary>
    public int? TestDay { get; set; }

    public OrderManager() => SeedNewGame();

    public override void _EnterTree()
    {
        if (Instance == null && GetParent() == GetTree().Root)
            Instance = this;
        ProcessMode = ProcessModeEnum.Pausable; // project-structure.md §4
    }

    public override void _Ready()
    {
        if (Instance != this) return;
        _clock = GameClock.Instance;
        if (_clock != null) _clock.DayStarted += OnDayStarted;
    }

    public override void _ExitTree()
    {
        if (_clock != null && IsInstanceValid(_clock)) _clock.DayStarted -= OnDayStarted;
        if (Instance == this)
            Instance = null;
    }

    private void SeedNewGame()
    {
        _orders.Clear();
        _delivered.Clear();
        _neighbours.Clear();
        _milestones.Clear();
        _neighbours.Add(Neighbours.Marlow);
        BoardOpen = false;
        Seed = 1;
        _orders.Add(Order.Make(FirstOrderId, Neighbours.Marlow, ItemIds.Berries, FirstOrderCount));
    }

    public void ResetForNewGame()
    {
        SeedNewGame();
        EmitSignal(SignalName.OrderChanged, FirstOrderId);
    }

    public Order Get(string id) => _orders.FirstOrDefault(o => o.Id == id);

    public IEnumerable<Order> OpenFrom(string giver) => _orders.Where(o => o.Giver == giver && o.State != OrderState.Paid);

    /// <summary>What the board shows: every open order (offered, accepted or subscribed).</summary>
    public IEnumerable<Order> BoardOrders => _orders.Where(o => o.State != OrderState.Paid);

    /// <summary>Daily standing orders Kit is subscribed to.</summary>
    public IEnumerable<Order> Subscriptions => _orders.Where(o => o.Standing && o.State == OrderState.Accepted);

    public void Add(Order o)
    {
        if (o == null || Get(o.Id) != null) return;
        _orders.Add(o);
        EmitSignal(SignalName.OrderChanged, o.Id);
    }

    public bool Accept(string id)
    {
        var o = Get(id);
        if (o == null || o.State != OrderState.Offered) return false;
        o.State = OrderState.Accepted;
        EmitSignal(SignalName.OrderChanged, id);
        return true;
    }

    /// <summary>Declines or cancels an order (or a daily subscription): back to offered, no penalty.</summary>
    public bool Decline(string id)
    {
        var o = Get(id);
        if (o == null || o.State == OrderState.Paid) return false;
        if (o.State == OrderState.Accepted)
        {
            o.State = OrderState.Offered;
            EmitSignal(SignalName.OrderChanged, id);
        }
        return true;
    }

    /// <summary>True when Kit has every line in the bag (and, for a standing order, hasn't delivered today).</summary>
    public bool CanDeliver(string id)
    {
        var o = Get(id);
        if (o == null || o.State != OrderState.Accepted) return false;
        if (o.Standing && o.DeliveredOnDay == Today) return false;
        return o.Lines.All(l => (Inventory.Instance?.CountAll(l.Item) ?? 0) >= l.Count);
    }

    /// <summary>Hands the items over and pays (plus a quick bonus in time) — exactly once per
    /// delivery. A standing order stays subscribed for tomorrow.</summary>
    public bool Deliver(string id)
    {
        if (!CanDeliver(id)) return false;
        var o = Get(id);
        foreach (var l in o.Lines)
            if (!RemoveAnyQuality(l.Item, l.Count)) return false;
        int pay = o.Reward + (o.BonusOpen(Today) ? o.Bonus : 0);
        if (o.Standing) o.DeliveredOnDay = Today;
        else o.State = OrderState.Paid;
        _delivered[o.Giver] = DeliveredBy(o.Giver) + 1;
        if (o.Repeat) o.TimesDelivered++;
        foreach (var l in o.Lines)
            if (l.Item is ItemIds.Carrot or ItemIds.SunSprout) _milestones.Add("delivered:crop");
        Wallet.Instance?.Earn(pay);
        EmitSignal(SignalName.OrderChanged, id);
        EmitSignal(SignalName.OrderPaid, id, pay);
        if (id == FirstOrderId && !BoardOpen)
        {
            BoardOpen = true;
            Add(Order.Make(SecondOrderId, Neighbours.Marlow, ItemIds.Carrot, SecondOrderCount, Today));
            EmitSignal(SignalName.BoardOpened);
        }
        OfferRepeatIfEarned(o);
        return true;
    }

    /// <summary>Kit produced <paramref name="item"/> (collected eggs, harvested a crop): may bring a neighbour.</summary>
    public void NoteProduced(string item) => _milestones.Add("produced:" + item);

    private void OfferRepeatIfEarned(Order delivered)
    {
        string id = RepeatId(delivered.Giver);
        if (delivered.Repeat || delivered.Standing || delivered.IsBasket || Get(id) != null || DeliveredBy(delivered.Giver) < RepeatAfterDeliveries) return;
        var r = Order.Make(id, delivered.Giver, delivered.ItemId, Math.Min(delivered.Count, MaxOrderCount), Today);
        r.Repeat = true;
        Add(r);
    }

    public static string RepeatId(string giver) => "repeat_" + giver;
    public static string StandingId(string giver) => "standing_" + giver;

    private static bool RemoveAnyQuality(string item, int count)
    {
        var inv = Inventory.Instance;
        if (inv == null || inv.CountAll(item) < count) return false;
        for (int q = Inventory.MinQuality; q <= Inventory.MaxQuality && count > 0; q++)
        {
            int take = Mathf.Min(count, inv.Count(item, q));
            if (take > 0 && inv.Remove(item, take, q)) count -= take;
        }
        return count == 0;
    }

    // ---- Daily board -------------------------------------------------------------------------
    private void OnDayStarted(int day) => OnDawn(day, Production.Supply());

    /// <summary>Dawn: new neighbours, paid repeat orders come back (a little bigger), finished
    /// one-off orders are cleared, standing orders appear, and the board is topped up.</summary>
    public void OnDawn(int day, IReadOnlyDictionary<string, int> supply)
    {
        if (!BoardOpen) return;
        IntroduceNextNeighbour(day, supply);
        foreach (var r in _orders.Where(o => o.Repeat && o.State == OrderState.Paid).ToList())
        {
            int baseCount = r.BaseCount > 0 ? r.BaseCount : r.Count;
            r.BaseCount = baseCount;
            if (!supply.TryGetValue(r.ItemId, out var s) || s <= 0) continue; // Kit can't make it now: wait
            int cap = Math.Min(s, MaxOrderCount);
            r.Count = Math.Clamp(baseCount + r.TimesDelivered / 2, 1, Math.Max(cap, 1));
            r.Reprice();
            r.State = OrderState.Offered;
            r.DayOffered = day;
            EmitSignal(SignalName.OrderChanged, r.Id);
        }
        _orders.RemoveAll(o => o.State == OrderState.Paid && !o.Repeat && o.Id != FirstOrderId && o.Id != SecondOrderId);
        OfferStandingOrders(day, supply);

        var rng = new Random(unchecked(Seed * 7919 + day * 104729));
        int n = 0;
        while (BoardOrders.Count() < MaxBoardOffers)
        {
            var o = GenerateOne(day, supply, rng, $"gen_{day}_{n++}", allowSameItem: true);
            if (o == null) break;
            Add(o);
        }
    }

    /// <summary>One new neighbour at a time, each after a visible action (characters.md §10).</summary>
    private void IntroduceNextNeighbour(int day, IReadOnlyDictionary<string, int> supply)
    {
        string next = null;
        if (!_neighbours.Contains(Neighbours.Baker) && _milestones.Contains("produced:" + ItemIds.Egg))
            next = Neighbours.Baker;
        else if (!_neighbours.Contains(Neighbours.Allotment) && _milestones.Contains("delivered:crop"))
            next = Neighbours.Allotment;
        else if (!_neighbours.Contains(Neighbours.Shopkeeper) && TotalDelivered >= Neighbours.ShopkeeperAfterDeliveries)
            next = Neighbours.Shopkeeper;
        if (next == null) return;
        var nb = Neighbours.Get(next);
        var item = supply.Keys.Where(nb.LikesItem).Where(PriceTable.Has).OrderBy(k => k, StringComparer.Ordinal).FirstOrDefault();
        if (item == null) return; // nothing they like yet: wait
        _neighbours.Add(next);
        var first = Order.Make($"hello_{next}", next, item, Math.Clamp(BaseCount(item), 1, Math.Min(supply[item], MaxOrderCount)), day);
        Add(first);
        EmitSignal(SignalName.NeighbourIntroduced, next);
    }

    private void OfferStandingOrders(int day, IReadOnlyDictionary<string, int> supply)
    {
        foreach (var giver in _neighbours)
        {
            if (Get(StandingId(giver)) != null || DeliveredBy(giver) < StandingAfterDeliveries) continue;
            var nb = Neighbours.Get(giver);
            var item = new[] { ItemIds.Egg, ItemIds.Milk }.FirstOrDefault(i => nb.LikesItem(i) && supply.TryGetValue(i, out var s) && s > 0);
            if (item == null) continue;
            int count = Math.Clamp(item == ItemIds.Milk ? 2 : 4, 1, supply[item]);
            var o = Order.Make(StandingId(giver), giver, item, count, day);
            o.Standing = true;
            Add(o);
        }
    }

    /// <summary>Usual order size per item (sizes vary around it), small numbers for 6+.</summary>
    public static int BaseCount(string item) => item switch
    {
        ItemIds.Carrot => 3,
        ItemIds.Berries => 2,
        ItemIds.Egg => 3,
        ItemIds.Milk => 1,
        _ => 2,
    };

    /// <summary>One new order for something Kit can produce (null if nothing fits). Picks a
    /// neighbour who likes something producible, prefers items not on the board, varies the size
    /// (1 … usual + 1 + deliveries/3, capped by supply and <see cref="MaxOrderCount"/>), and sometimes
    /// makes a mixed basket or adds a quick bonus. Never the exact same item and size twice.</summary>
    public Order GenerateOne(int day, IReadOnlyDictionary<string, int> supply, Random rng, string id, bool allowSameItem = true)
    {
        var producible = supply.Where(kv => kv.Value > 0 && PriceTable.Has(kv.Key)).Select(kv => kv.Key).OrderBy(k => k, StringComparer.Ordinal).ToList();
        if (producible.Count == 0) return null;
        var givers = Neighbours.Of(_neighbours).Where(n => producible.Any(n.LikesItem)).ToList();
        if (givers.Count == 0) return null;
        var giver = givers[rng.Next(givers.Count)];
        var items = producible.Where(giver.LikesItem).ToList();
        var fresh = items.Where(i => BoardOrders.All(o => o.Lines.All(l => l.Item != i))).ToList();
        if (fresh.Count == 0 && !allowSameItem) return null;
        var pool = fresh.Count > 0 ? fresh : items;
        string item = pool[rng.Next(pool.Count)];
        int cap = Math.Min(supply[item], MaxOrderCount);
        int hi = Math.Clamp(BaseCount(item) + 1 + TotalDelivered / 3, 1, cap);
        int count = rng.Next(1, hi + 1);
        // Same item already on the board: a different size, or nothing.
        var taken = BoardOrders.Where(o => !o.IsBasket && o.ItemId == item).Select(o => o.Count).ToHashSet();
        if (taken.Contains(count))
        {
            var other = Enumerable.Range(1, hi).Where(c => !taken.Contains(c)).ToList();
            if (other.Count == 0) return null;
            count = other[rng.Next(other.Count)];
        }
        var o = Order.Make(id, giver.Id, item, count, day);
        if (items.Count >= 2 && rng.NextDouble() < BasketChance)
        {
            var second = items.Where(i => i != item).ToList();
            string item2 = second[rng.Next(second.Count)];
            o.Extra.Add(new OrderLine(item2, rng.Next(1, Math.Min(3, Math.Min(supply[item2], MaxOrderCount)) + 1)));
            o.Reprice();
        }
        if (rng.NextDouble() < QuickChance)
            o.BonusUntilDay = day;
        return o;
    }

    /// <summary>Adds a daily order for <paramref name="item"/> now (Task 1.12 nudge "first eggs and
    /// milk": egg and milk orders appear on the board). Returns it, or null if it isn't priced.</summary>
    public Order AddDailyOrder(string item, int day)
    {
        if (!BoardOpen || !PriceTable.Has(item)) return null;
        int n = 0;
        while (Get($"gen_{day}_{item}_{n}") != null) n++;
        string giver = _neighbours.Contains(Neighbours.Baker) && item is ItemIds.Egg or ItemIds.Milk ? Neighbours.Baker : Neighbours.Marlow;
        var o = Order.Make($"gen_{day}_{item}_{n}", giver, item, Math.Clamp(BaseCount(item) + TotalDelivered / 3, 1, MaxOrderCount), day);
        Add(o);
        return o;
    }

    /// <summary>Save/load (Task 1.10).</summary>
    public void Restore(IEnumerable<Order> orders, bool boardOpen, IDictionary<string, int> delivered = null, int seed = 1,
        IEnumerable<string> neighbours = null, IEnumerable<string> milestones = null)
    {
        _orders.Clear();
        _orders.AddRange(orders);
        BoardOpen = boardOpen;
        _delivered.Clear();
        if (delivered != null)
            foreach (var (k, v) in delivered) _delivered[k] = v;
        Seed = seed;
        _neighbours.Clear();
        _neighbours.Add(Neighbours.Marlow);
        if (neighbours != null) foreach (var n in neighbours) _neighbours.Add(n);
        _milestones.Clear();
        if (milestones != null) foreach (var m in milestones) _milestones.Add(m);
    }
}
