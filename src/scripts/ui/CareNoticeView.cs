using System.Collections.Generic;
using System.Linq;
using Godot;
using MyFarmingGame.Autoloads;
using MyFarmingGame.Core;
using MyFarmingGame.World;

namespace MyFarmingGame.UI;

/// <summary>
/// Gentle animal-care messages (Task 1.9, animals.md §3 and §5), shown in the picture bubble when
/// Kit wakes up (or at once if Kit is already playing):
/// * <b>Warning</b> — an animal has gone 2 days unfed: a neighbour mentions it (Marlow until Ada
///   arrives in Task 1.12; <see cref="WarningPortrait"/>).
/// * <b>Loss card</b> — one per lost animal: a small gravestone picture, the animal's name, one
///   line and a tip, dismissed with one button. Nothing is shown dying.
/// </summary>
public partial class CareNoticeView : Node
{
    public const string Group = "care_notices";

    /// <summary>Who mentions hungry animals (Ada from Task 1.12).</summary>
    public static string WarningPortrait { get; set; } = "npc_marlow";

    private readonly Queue<(string Portrait, string Line, BubbleChip[] Chips)> _queue = new();
    private readonly List<HerdState> _houses = new();
    private GameClock _clock;

    /// <summary>Loss cards shown so far (tests).</summary>
    public int LossCardsShown { get; private set; }
    public int WarningsShown { get; private set; }
    public int Pending => _queue.Count;

    public override void _Ready()
    {
        AddToGroup(Group);
        // The herds live in FarmState (Phase-002 Task 2.2), so warnings and loss cards arrive even
        // while the coop's and barn's rooms are not loaded.
        foreach (var h in FarmState.Instance?.Herds ?? System.Linq.Enumerable.Empty<HerdState>())
        {
            h.AnimalLost += OnLost;
            h.HungryWarning += OnHungry;
            _houses.Add(h);
        }
        _clock = GameClock.Instance;
        if (_clock != null) _clock.WokeUp += ShowNext;
        if (OrderManager.Instance != null) OrderManager.Instance.NeighbourIntroduced += OnNeighbour;
    }

    /// <summary>A new neighbour pinned their first order (owner order-variety request 2026-11-21).</summary>
    private void OnNeighbour(string id)
    {
        var n = Neighbours.Get(id);
        _queue.Enqueue((n.Portrait, n.Hello, new[] { new BubbleChip("icon_order_board", "") }));
        NeighboursIntroduced++;
        if (_clock == null || !_clock.IsSleeping) ShowNext();
    }

    public int NeighboursIntroduced { get; private set; }

    public override void _ExitTree()
    {
        foreach (var h in _houses.Where(IsInstanceValid))
        {
            h.AnimalLost -= OnLost;
            h.HungryWarning -= OnHungry;
        }
        if (_clock != null && IsInstanceValid(_clock)) _clock.WokeUp -= ShowNext;
        if (OrderManager.Instance != null && IsInstanceValid(OrderManager.Instance)) OrderManager.Instance.NeighbourIntroduced -= OnNeighbour;
    }

    public static string LossLine(string name, string kind) => kind == "cow"
        ? $"{name} the cow is gone. Cows need food every day."
        : $"{name} the hen is gone. Hens need food every day.";

    public const string LossTip = "Tip: scoop feed every day, and order more by phone.";

    public static string WarningLine(string kind) => kind == "cow"
        ? "Your cow looks hungry. There's hay in the barn's bin."
        : "Your hens look hungry. There's grain in the coop's bin.";

    private void OnLost(string name, string kind)
    {
        _queue.Enqueue(("", LossLine(name, kind) + " " + LossTip,
            new[] { new BubbleChip("gravestone", ""), new BubbleChip(kind == "cow" ? "cow" : "hen", name, true) }));
        LossCardsShown++;
        if (_clock == null || !_clock.IsSleeping) ShowNext();
    }

    private void OnHungry(string kind)
    {
        _queue.Enqueue((WarningPortrait, WarningLine(kind),
            new[] { new BubbleChip(kind == "cow" ? "cow" : "hen", ""), new BubbleChip(ItemIds.SpriteName(kind == "cow" ? ItemIds.Hay : ItemIds.ChickenFeed), "") }));
        WarningsShown++;
        if (_clock == null || !_clock.IsSleeping) ShowNext();
    }

    /// <summary>Shows the next queued message (each one's OK shows the one after).</summary>
    public void ShowNext()
    {
        var bubble = PictureBubbleView.Find(GetTree());
        if (bubble == null || _queue.Count == 0 || bubble.IsOpen) return;
        var (portrait, line, chips) = _queue.Dequeue();
        if (!bubble.Show(portrait, line, chips, new[] { new BubbleButton { Icon = "icon_check", Text = "OK", IsCancel = true, OnPress = ShowNext } }))
            return;
    }
}
