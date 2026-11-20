using System.Collections.Generic;
using System.Linq;
using Godot;
using MyFarmingGame.Core;

namespace MyFarmingGame.Autoloads;

/// <summary>
/// Neighbour orders (autoload <c>OrderManager</c>). Task 1.6: holds Marlow's scripted first order
/// (3 handfuls of wild berries) and the accept / decline / deliver flow — payment happens exactly
/// once. Task 1.7 adds the order board, daily generation and repeat orders.
/// </summary>
public partial class OrderManager : Node
{
    public static OrderManager Instance { get; private set; }

    public const string FirstOrderId = "marlow_berries";
    public const int FirstOrderCount = 3;

    /// <summary>An order changed state. Argument: order id.</summary>
    [Signal] public delegate void OrderChangedEventHandler(string id);
    /// <summary>An order was delivered and paid. Arguments: order id, coins.</summary>
    [Signal] public delegate void OrderPaidEventHandler(string id, int coins);
    /// <summary>The order board opened (after the first order is paid).</summary>
    [Signal] public delegate void BoardOpenedEventHandler();

    private readonly List<Order> _orders = new();

    /// <summary>Every order still offered, accepted, or paid (paid ones stay for the ledger/tests).</summary>
    public IReadOnlyList<Order> Orders => _orders;

    /// <summary>True once the first order is paid (the board appears at the shed).</summary>
    public bool BoardOpen { get; private set; }

    public OrderManager() => SeedNewGame();

    public override void _EnterTree()
    {
        if (Instance == null && GetParent() == GetTree().Root)
            Instance = this;
        ProcessMode = ProcessModeEnum.Pausable; // project-structure.md §4
    }

    public override void _ExitTree()
    {
        if (Instance == this)
            Instance = null;
    }

    private void SeedNewGame()
    {
        _orders.Clear();
        BoardOpen = false;
        _orders.Add(Order.Make(FirstOrderId, "marlow", "berries", FirstOrderCount));
    }

    public void ResetForNewGame()
    {
        SeedNewGame();
        EmitSignal(SignalName.OrderChanged, FirstOrderId);
    }

    public Order Get(string id) => _orders.FirstOrDefault(o => o.Id == id);

    /// <summary>Open orders (not yet paid) from one neighbour, oldest first.</summary>
    public IEnumerable<Order> OpenFrom(string giver) => _orders.Where(o => o.Giver == giver && o.State != OrderState.Paid);

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
        Wallet.Instance?.Earn(o.Reward);
        EmitSignal(SignalName.OrderChanged, id);
        EmitSignal(SignalName.OrderPaid, id, o.Reward);
        if (id == FirstOrderId && !BoardOpen)
        {
            BoardOpen = true;
            EmitSignal(SignalName.BoardOpened);
        }
        return true;
    }

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

    /// <summary>Save/load (Task 1.10).</summary>
    public void Restore(IEnumerable<Order> orders, bool boardOpen)
    {
        _orders.Clear();
        _orders.AddRange(orders);
        BoardOpen = boardOpen;
    }
}
