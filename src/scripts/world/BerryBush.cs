using Godot;
using MyFarmingGame.Autoloads;
using MyFarmingGame.Core;

namespace MyFarmingGame.World;

/// <summary>
/// A wild berry bush on the lane (Task 1.6, places.md §2.1). Interact → one handful of wild berries
/// into the bag; the bush is bare until it regrows <see cref="RegrowDays"/> days later (counted at
/// the new day's Dawn). Group <see cref="Group"/>; the node name is its save id (Task 1.10).
/// </summary>
public partial class BerryBush : Prop
{
    public const string Group = "berry_bushes";
    public const int RegrowDays = 2;

    public bool HasBerries { get; private set; } = true;
    /// <summary>Day the berries were picked (0 = never).</summary>
    public int PickedOnDay { get; private set; }

    [Signal] public delegate void PickedEventHandler();

    private GameClock _clock;

    public override bool Usable => HasBerries;

    public BerryBush()
    {
        SpriteName = "berry_bush";
        PromptSprite = "item_berries";
        BodySize = new Vector2(40, 20);
        BodyOffset = new Vector2(0, 18);
    }

    public override void _Ready()
    {
        base._Ready();
        AddToGroup(Group);
        _clock = GameClock.Instance;
        if (_clock != null) _clock.DayStarted += OnDayStarted;
        Production.Register(this, () => new[] { (ItemIds.Berries, 1) });
        Refresh();
    }

    public override void _ExitTree()
    {
        if (_clock != null && IsInstanceValid(_clock)) _clock.DayStarted -= OnDayStarted;
        Production.Unregister(this);
    }

    public override void Interact(Node2D player)
    {
        if (!HasBerries) return;
        HasBerries = false;
        PickedOnDay = GameClock.Instance?.Day ?? 1;
        Inventory.Instance?.Add(ItemIds.Berries);
        Refresh();
        EmitSignal(SignalName.Picked);
    }

    private void OnDayStarted(int day) => NewDay(day);

    /// <summary>Regrows once <see cref="RegrowDays"/> days have passed since picking.</summary>
    public void NewDay(int day)
    {
        if (!HasBerries && day - PickedOnDay >= RegrowDays)
        {
            HasBerries = true;
            Refresh();
        }
    }

    /// <summary>Save/load (Task 1.10).</summary>
    public void SetState(bool hasBerries, int pickedOnDay)
    {
        HasBerries = hasBerries;
        PickedOnDay = pickedOnDay;
        Refresh();
    }

    private void Refresh()
    {
        if (Sprite != null) Sprite.SpriteName = HasBerries ? "berry_bush" : "berry_bush_empty";
    }
}
