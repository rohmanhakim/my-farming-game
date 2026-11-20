using System.Collections.Generic;
using System.Linq;

namespace MyFarmingGame.Core;

/// <summary>A phone feed order on its way (Task 1.8).</summary>
public sealed class FeedDelivery
{
    public string Item { get; init; } = "";
    public int Amount { get; init; }
    public int DeliverOnDay { get; init; }
}

/// <summary>
/// Feed ordered by phone and not yet delivered (Task 1.8, economy.md §2): ordered before 18:00 it
/// arrives at the next Dawn, later it arrives the day after; no fee. Each feed bin takes its own
/// deliveries at Dawn (<see cref="TakeDue"/>). Static state so it survives level changes; saved by
/// Task 1.10 in the <c>feed</c> section.
/// </summary>
public static class FeedStore
{
    /// <summary>Orders placed at or after this minute of the day arrive a day later (18:00).</summary>
    public const int CutoffMinute = 18 * 60;

    private static readonly List<FeedDelivery> _pending = new();

    public static IReadOnlyList<FeedDelivery> Pending => _pending;

    public static int DeliveryDay(int day, int minuteOfDay) => minuteOfDay < CutoffMinute ? day + 1 : day + 2;

    /// <summary>Places a phone order; returns the day it arrives.</summary>
    public static int Order(string item, int amount, int day, int minuteOfDay)
    {
        int arrive = DeliveryDay(day, minuteOfDay);
        _pending.Add(new FeedDelivery { Item = item, Amount = amount, DeliverOnDay = arrive });
        return arrive;
    }

    /// <summary>Removes and returns how much <paramref name="item"/> is due by <paramref name="day"/>.</summary>
    public static int TakeDue(string item, int day)
    {
        int sum = _pending.Where(d => d.Item == item && d.DeliverOnDay <= day).Sum(d => d.Amount);
        _pending.RemoveAll(d => d.Item == item && d.DeliverOnDay <= day);
        return sum;
    }

    public static int PendingAmount(string item) => _pending.Where(d => d.Item == item).Sum(d => d.Amount);

    public static void Reset() => _pending.Clear();

    public static void Restore(IEnumerable<FeedDelivery> pending)
    {
        _pending.Clear();
        _pending.AddRange(pending);
    }
}
