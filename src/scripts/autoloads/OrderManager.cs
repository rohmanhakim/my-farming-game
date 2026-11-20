using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MyFarmingGame.Core;

namespace MyFarmingGame.Autoloads;

/// <summary>
/// Neighbour orders (autoload <c>OrderManager</c>, Tasks 1.6–1.7, GDD §4 "Neighbour orders").
///
/// * Task 1.6: Marlow's scripted first order (3 handfuls of wild berries) and the accept / decline /
///   deliver flow — payment happens exactly once. Paying it opens the order board.
/// * Task 1.7: when the board opens Marlow pins the scripted second order (5 carrots). Every Dawn
///   (<see cref="GameClock.DayStarted"/>) the board is topped up to <see cref="MaxBoardOffers"/>
///   open orders, drawn only from what Kit can produce now (<see cref="Production.Supply"/>) and
///   never more than the farm can supply. Orders grow slowly with deliveries. After
///   <see cref="RepeatAfterDeliveries"/> delivered orders a neighbour offers a <b>repeat order</b>
///   that comes back each Dawn after it is paid, a little bigger every second time.
/// * No deadlines, no penalties: an unaccepted order waits; cancelling returns it to offered.
/// </summary>
public partial class OrderManager : Node
{
    public static OrderManager Instance { get; private set; }

    public const string FirstOrderId = "marlow_berries";
    public const int FirstOrderCount = 3;
    public const string SecondOrderId = "marlow_carrots";
    public const int SecondOrderCount = 5;

    /// <summary>Open (offered or accepted) orders the board shows at most. The board never shows
    /// two orders for the same item, so with only carrots and berries it holds 2.</summary>
    public const int MaxBoardOffers = 3;
    /// <summary>Deliveries to one neighbour before they offer a repeat order.</summary>
    public const int RepeatAfterDeliveries = 3;
    /// <summary>Order size limit at farm tier T1 (progression.md §3: 1–6 items).</summary>
    public const int MaxOrderCount = 6;

    /// <summary>An order changed state. Argument: order id.</summary>
    [Signal] public delegate void OrderChangedEventHandler(string id);
    /// <summary>An order was delivered and paid. Arguments: order id, coins.</summary>
    [Signal] public delegate void OrderPaidEventHandler(string id, int coins);
    /// <summary>The order board opened (after the first order is paid).</summary>
    [Signal] public delegate void BoardOpenedEventHandler();

    private readonly List<Order> _orders = new();
    private readonly Dictionary<string, int> _delivered = new();
    private GameClock _clock;

    /// <summary>Every order still offered, accepted, or paid.</summary>
    public IReadOnlyList<Order> Orders => _orders;

    /// <summary>True once the first order is paid (the board appears at the shed).</summary>
    public bool BoardOpen { get; private set; }

    /// <summary>Random seed for the daily orders (saved, so a reload gives the same board).</summary>
    public int Seed { get; set; } = 1;

    /// <summary>Delivered (paid) orders per neighbour.</summary>
    public IReadOnlyDictionary<string, int> Delivered => _delivered;

    public int DeliveredBy(string giver) => _delivered.TryGetValue(giver, out var n) ? n : 0;

    public int TotalDelivered => _delivered.Values.Sum();

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
        BoardOpen = false;
        Seed = 1;
        _orders.Add(Order.Make(FirstOrderId, "marlow", ItemIds.Berries, FirstOrderCount));
    }

    public void ResetForNewGame()
    {
        SeedNewGame();
        EmitSignal(SignalName.OrderChanged, FirstOrderId);
    }

    public Order Get(string id) => _orders.FirstOrDefault(o => o.Id == id);

    /// <summary>Open orders (not yet paid) from one neighbour, oldest first.</summary>
    public IEnumerable<Order> OpenFrom(string giver) => _orders.Where(o => o.Giver == giver && o.State != OrderState.Paid);

    /// <summary>What the board shows: every open order (offered or accepted).</summary>
    public IEnumerable<Order> BoardOrders => _orders.Where(o => o.State != OrderState.Paid);

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

    /// <summary>Declines or cancels an order: it goes back to offered, no penalty.</summary>
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

    public bool CanDeliver(string id)
    {
        var o = Get(id);
        return o != null && o.State == OrderState.Accepted && (Inventory.Instance?.CountAll(o.ItemId) ?? 0) >= o.Count;
    }

    /// <summary>Hands the items over and pays — exactly once. Returns false (nothing changes) when
    /// the order isn't accepted or Kit doesn't have enough.</summary>
    public bool Deliver(string id)
    {
        if (!CanDeliver(id)) return false;
        var o = Get(id);
        if (!RemoveAnyQuality(o.ItemId, o.Count)) return false;
        o.State = OrderState.Paid;
        _delivered[o.Giver] = DeliveredBy(o.Giver) + 1;
        if (o.Repeat) o.TimesDelivered++;
        Wallet.Instance?.Earn(o.Reward);
        EmitSignal(SignalName.OrderChanged, id);
        EmitSignal(SignalName.OrderPaid, id, o.Reward);
        if (id == FirstOrderId && !BoardOpen)
        {
            BoardOpen = true;
            // Marlow pins the farming order (GDD §3: "then orders 5 carrots").
            Add(Order.Make(SecondOrderId, "marlow", ItemIds.Carrot, SecondOrderCount, _clock?.Day ?? 1));
            EmitSignal(SignalName.BoardOpened);
        }
        OfferRepeatIfEarned(o);
        return true;
    }

    private void OfferRepeatIfEarned(Order delivered)
    {
        string id = RepeatId(delivered.Giver);
        if (delivered.Repeat || Get(id) != null || DeliveredBy(delivered.Giver) < RepeatAfterDeliveries) return;
        var r = Order.Make(id, delivered.Giver, delivered.ItemId, Math.Min(delivered.Count, MaxOrderCount), _clock?.Day ?? 1);
        r.Repeat = true;
        Add(r);
    }

    public static string RepeatId(string giver) => "repeat_" + giver;

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

    // ---- Daily board (Task 1.7) -------------------------------------------------------------
    private void OnDayStarted(int day) => OnDawn(day, Production.Supply());

    /// <summary>Dawn: paid repeat orders come back (a little bigger), finished one-off orders are
    /// cleared away, and the board is topped up from what Kit can produce.</summary>
    public void OnDawn(int day, IReadOnlyDictionary<string, int> supply)
    {
        if (!BoardOpen) return;
        foreach (var r in _orders.Where(o => o.Repeat && o.State == OrderState.Paid).ToList())
        {
            int baseCount = r.BaseCount > 0 ? r.BaseCount : r.Count;
            r.BaseCount = baseCount;
            if (!supply.TryGetValue(r.ItemId, out var s) || s <= 0) continue; // Kit can't make it now: wait
            int cap = Math.Min(s, MaxOrderCount);
            r.Count = Math.Clamp(baseCount + r.TimesDelivered / 2, 1, Math.Max(cap, 1));
            r.Reward = PriceTable.OrderReward(r.ItemId, r.Count);
            r.State = OrderState.Offered;
            r.DayOffered = day;
            EmitSignal(SignalName.OrderChanged, r.Id);
        }
        _orders.RemoveAll(o => o.State == OrderState.Paid && !o.Repeat && o.Id != FirstOrderId && o.Id != SecondOrderId);

        var rng = new Random(unchecked(Seed * 7919 + day * 104729));
        int n = 0;
        while (BoardOrders.Count() < MaxBoardOffers)
        {
            var o = GenerateOne(day, supply, rng, $"gen_{day}_{n++}", allowSameItem: false);
            if (o == null) break;
            Add(o);
        }
    }

    /// <summary>Base order size per item (before growth), small numbers for 6+.</summary>
    public static int BaseCount(string item) => item switch
    {
        ItemIds.Carrot => 3,
        ItemIds.Berries => 2,
        ItemIds.Egg => 3,
        ItemIds.Milk => 1,
        _ => 2,
    };

    /// <summary>One new order for something Kit can produce (null if nothing is producible).
    /// Prefers items not already on the board. Size grows by one per 3 delivered orders, capped by
    /// the farm's supply and <see cref="MaxOrderCount"/>.</summary>
    public Order GenerateOne(int day, IReadOnlyDictionary<string, int> supply, Random rng, string id, bool allowSameItem = true)
    {
        var items = supply.Where(kv => kv.Value > 0 && PriceTable.Has(kv.Key)).Select(kv => kv.Key).OrderBy(k => k, StringComparer.Ordinal).ToList();
        if (items.Count == 0) return null;
        var fresh = items.Where(i => BoardOrders.All(o => o.ItemId != i)).ToList();
        if (fresh.Count == 0 && !allowSameItem) return null;
        var pool = fresh.Count > 0 ? fresh : items;
        string item = pool[rng.Next(pool.Count)];
        int count = Math.Clamp(BaseCount(item) + TotalDelivered / 3, 1, Math.Min(supply[item], MaxOrderCount));
        return Order.Make(id, "marlow", item, count, day);
    }

    /// <summary>Save/load (Task 1.10).</summary>
    public void Restore(IEnumerable<Order> orders, bool boardOpen, IDictionary<string, int> delivered = null, int seed = 1)
    {
        _orders.Clear();
        _orders.AddRange(orders);
        BoardOpen = boardOpen;
        _delivered.Clear();
        if (delivered != null)
            foreach (var (k, v) in delivered) _delivered[k] = v;
        Seed = seed;
    }
}
