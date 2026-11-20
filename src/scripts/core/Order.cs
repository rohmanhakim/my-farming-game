using Godot;

namespace MyFarmingGame.Core;

/// <summary>Where an order is (Task 1.6): offered → accepted → paid. Declining puts it back to
/// offered; nothing is ever lost or late.</summary>
public enum OrderState { Offered, Accepted, Paid }

/// <summary>
/// One neighbour order (Task 1.6/1.7, economy.md §3): "<see cref="Count"/> × <see cref="ItemId"/>
/// for <see cref="Reward"/> coins". No deadline, no penalty. Saved by Task 1.10 via
/// <see cref="ToDict"/> / <see cref="FromDict"/>.
/// </summary>
public sealed class Order
{
    public string Id { get; set; } = "";
    public string Giver { get; set; } = "marlow";
    public string ItemId { get; set; } = "";
    public int Count { get; set; } = 1;
    public int Reward { get; set; }
    public OrderState State { get; set; } = OrderState.Offered;
    /// <summary>True for a repeat order (Task 1.7).</summary>
    public bool Repeat { get; set; }
    /// <summary>Day it was offered (Task 1.7 board refresh).</summary>
    public int DayOffered { get; set; } = 1;

    public static Order Make(string id, string giver, string item, int count, int dayOffered = 1) => new()
    {
        Id = id, Giver = giver, ItemId = item, Count = count,
        Reward = PriceTable.OrderReward(item, count), DayOffered = dayOffered,
    };

    public Godot.Collections.Dictionary ToDict() => new()
    {
        { "id", Id }, { "giver", Giver }, { "item", ItemId }, { "count", Count }, { "reward", Reward },
        { "state", State.ToString() }, { "repeat", Repeat }, { "dayOffered", DayOffered },
    };

    public static Order FromDict(Godot.Collections.Dictionary d)
    {
        var o = new Order
        {
            Id = d.TryGetValue("id", out var id) ? id.AsString() : "",
            Giver = d.TryGetValue("giver", out var g) ? g.AsString() : "marlow",
            ItemId = d.TryGetValue("item", out var i) ? i.AsString() : "",
            Count = d.TryGetValue("count", out var c) ? c.AsInt32() : 1,
            Reward = d.TryGetValue("reward", out var r) ? r.AsInt32() : 0,
            Repeat = d.TryGetValue("repeat", out var rp) && rp.AsBool(),
            DayOffered = d.TryGetValue("dayOffered", out var day) ? day.AsInt32() : 1,
        };
        if (d.TryGetValue("state", out var s) && System.Enum.TryParse<OrderState>(s.AsString(), out var st))
            o.State = st;
        return o;
    }
}
