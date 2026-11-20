using System.Collections.Generic;
using System.Linq;
using Godot;

namespace MyFarmingGame.Core;

/// <summary>Where an order is (Task 1.6): offered → accepted → paid. Declining puts it back to
/// offered; nothing is ever lost or late. A daily standing order stays accepted (subscribed) and
/// is delivered once per day.</summary>
public enum OrderState { Offered, Accepted, Paid }

/// <summary>One item line of an order: "<see cref="Count"/> × <see cref="Item"/>".</summary>
public sealed class OrderLine
{
    public string Item { get; set; } = "";
    public int Count { get; set; } = 1;
    public OrderLine() { }
    public OrderLine(string item, int count) { Item = item; Count = count; }
}

/// <summary>
/// One neighbour order (Tasks 1.6–1.7, economy.md §3, owner variety request 2026-11-21).
/// The first line is <see cref="ItemId"/> × <see cref="Count"/>; a <b>mixed basket</b> adds
/// <see cref="Extra"/> lines and pays a basket bonus. A <b>standing</b> order is delivered once a
/// day while Kit is subscribed. A <b>quick bonus</b> order pays extra if delivered by the end of
/// <see cref="BonusUntilDay"/> — never less after that (no deadline penalty, GDD §4).
/// Saved by Task 1.10 via <see cref="ToDict"/> / <see cref="FromDict"/>.
/// </summary>
public sealed class Order
{
    /// <summary>Mixed baskets pay this × the sum of their lines (economy.md "bulk ×1.1").</summary>
    public const float BasketBonus = 1.1f;
    /// <summary>Quick-bonus orders add this share of the reward when delivered in time.</summary>
    public const float QuickBonus = 0.2f;

    public string Id { get; set; } = "";
    public string Giver { get; set; } = "marlow";
    public string ItemId { get; set; } = "";
    public int Count { get; set; } = 1;
    /// <summary>More item lines (mixed basket).</summary>
    public List<OrderLine> Extra { get; set; } = new();
    public int Reward { get; set; }
    public OrderState State { get; set; } = OrderState.Offered;
    /// <summary>True for a repeat order (Task 1.7).</summary>
    public bool Repeat { get; set; }
    /// <summary>True for a daily standing order (GDD §4 "subscription orders").</summary>
    public bool Standing { get; set; }
    /// <summary>Standing orders: the day it was last delivered (0 = not yet).</summary>
    public int DeliveredOnDay { get; set; }
    /// <summary>Quick bonus: delivered on or before this day pays <see cref="Bonus"/> extra (0 = none).</summary>
    public int BonusUntilDay { get; set; }
    /// <summary>Day it was offered (Task 1.7 board refresh).</summary>
    public int DayOffered { get; set; } = 1;
    /// <summary>Repeat orders: how often delivered (it grows every second time).</summary>
    public int TimesDelivered { get; set; }
    /// <summary>Repeat orders: the size it started at.</summary>
    public int BaseCount { get; set; }

    /// <summary>Every item line, first line first.</summary>
    public IEnumerable<OrderLine> Lines => new[] { new OrderLine(ItemId, Count) }.Concat(Extra);

    public bool IsBasket => Extra.Count > 0;

    /// <summary>The quick-bonus coins (0 for orders without one).</summary>
    public int Bonus => BonusUntilDay > 0 ? Mathf.RoundToInt(Reward * QuickBonus) : 0;

    /// <summary>True when delivering on <paramref name="day"/> earns the quick bonus.</summary>
    public bool BonusOpen(int day) => BonusUntilDay > 0 && day <= BonusUntilDay;

    /// <summary>Recomputes <see cref="Reward"/> from the lines (order premium, basket bonus).</summary>
    public void Reprice()
    {
        int sum = Lines.Sum(l => PriceTable.OrderReward(l.Item, l.Count));
        Reward = IsBasket ? Mathf.RoundToInt(sum * BasketBonus) : sum;
    }

    public static Order Make(string id, string giver, string item, int count, int dayOffered = 1)
    {
        var o = new Order { Id = id, Giver = giver, ItemId = item, Count = count, DayOffered = dayOffered };
        o.Reprice();
        return o;
    }

    public Godot.Collections.Dictionary ToDict()
    {
        var extra = new Godot.Collections.Array();
        foreach (var l in Extra) extra.Add(new Godot.Collections.Dictionary { { "item", l.Item }, { "count", l.Count } });
        return new()
        {
            { "id", Id }, { "giver", Giver }, { "item", ItemId }, { "count", Count }, { "extra", extra }, { "reward", Reward },
            { "state", State.ToString() }, { "repeat", Repeat }, { "standing", Standing }, { "deliveredOnDay", DeliveredOnDay },
            { "bonusUntilDay", BonusUntilDay }, { "dayOffered", DayOffered }, { "timesDelivered", TimesDelivered }, { "baseCount", BaseCount },
        };
    }

    public static Order FromDict(Godot.Collections.Dictionary d)
    {
        int I(string k, int def = 0) => d.TryGetValue(k, out var v) ? v.AsInt32() : def;
        var o = new Order
        {
            Id = d.TryGetValue("id", out var id) ? id.AsString() : "",
            Giver = d.TryGetValue("giver", out var g) ? g.AsString() : "marlow",
            ItemId = d.TryGetValue("item", out var i) ? i.AsString() : "",
            Count = I("count", 1),
            Reward = I("reward"),
            Repeat = d.TryGetValue("repeat", out var rp) && rp.AsBool(),
            Standing = d.TryGetValue("standing", out var st) && st.AsBool(),
            DeliveredOnDay = I("deliveredOnDay"),
            BonusUntilDay = I("bonusUntilDay"),
            DayOffered = I("dayOffered", 1),
            TimesDelivered = I("timesDelivered"),
            BaseCount = I("baseCount"),
        };
        if (d.TryGetValue("extra", out var ex))
            foreach (var v in ex.AsGodotArray())
            {
                var l = v.AsGodotDictionary();
                o.Extra.Add(new OrderLine(l["item"].AsString(), l["count"].AsInt32()));
            }
        if (d.TryGetValue("state", out var s) && System.Enum.TryParse<OrderState>(s.AsString(), out var state))
            o.State = state;
        return o;
    }
}
