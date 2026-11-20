using System;
using System.Collections.Generic;

namespace MyFarmingGame.Core;

/// <summary>
/// What Kit's farm can produce right now (Task 1.7, GDD §4 "orders only ask for what Kit can
/// actually produce"). Systems register a supply function while they exist: the front garden
/// (carrots), berry bushes (wild berries), and, from Task 1.9, hens (eggs) and the cow (milk).
/// <see cref="Supply"/> sums them: item id → how many Kit could deliver in an order.
/// </summary>
public static class Production
{
    private static readonly Dictionary<object, Func<IEnumerable<(string Item, int Amount)>>> Sources = new();

    public static void Register(object owner, Func<IEnumerable<(string Item, int Amount)>> source) => Sources[owner] = source;

    public static void Unregister(object owner) => Sources.Remove(owner);

    /// <summary>Item id → amount the farm can supply (only items with amount &gt; 0).</summary>
    public static Dictionary<string, int> Supply()
    {
        var sum = new Dictionary<string, int>();
        foreach (var source in Sources.Values)
            foreach (var (item, amount) in source())
                if (amount > 0)
                    sum[item] = sum.TryGetValue(item, out var a) ? a + amount : amount;
        return sum;
    }
}
