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
    private readonly List<AnimalHouse> _houses = new();
    private GameClock _clock;

    /// <summary>Loss cards shown so far (tests).</summary>
    public int LossCardsShown { get; private set; }
    public int WarningsShown { get; private set; }
    public int Pending => _queue.Count;

    public override void _Ready()
    {
        AddToGroup(Group);
        foreach (var h in GetTree().GetNodesInGroup(AnimalHouse.Group).OfType<AnimalHouse>())
        {
            h.AnimalLost += OnLost;
            h.HungryWarning += OnHungry;
            _houses.Add(h);
        }
        _clock = GameClock.Instance;
        if (_clock != null) _clock.WokeUp += ShowNext;
    }

    public override void _ExitTree()
    {
        foreach (var h in _houses.Where(IsInstanceValid))
        {
            h.AnimalLost -= OnLost;
            h.HungryWarning -= OnHungry;
        }
        if (_clock != null && IsInstanceValid(_clock)) _clock.WokeUp -= ShowNext;
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
