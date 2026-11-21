using System.Collections.Generic;
using System.Linq;
using Godot;
using MyFarmingGame.Core;
using MyFarmingGame.World;

namespace MyFarmingGame.Autoloads;

/// <summary>
/// The farm's state that must live on while its scene is not loaded (Phase-002 Task 2.2, owner
/// 2026-11-21: a separate scene for each room). Kit sleeps in the house room, buys in the shed room
/// and walks between scenes, but the garden must still grow and dry at night, the hens and the cow
/// must still count hungry days, feed must still arrive at Dawn, and the lane's bushes must still
/// regrow. So this autoload holds:
///
/// * <see cref="Garden"/> — the front garden's tiles and rules (<see cref="GardenState"/>);
/// * the herds by kind — the coop's hens and the barn's cow (<see cref="HerdState"/>);
/// * the berry bushes by name (<see cref="Bush"/>), regrown at Dawn here.
///
/// Scenes only show this state: <see cref="FarmGrid"/>, <see cref="AnimalHouse"/> and
/// <see cref="BerryBush"/> bind to it when they enter the tree. Saved by Main's SaveBinder.
/// </summary>
public partial class FarmState : Node
{
    public static FarmState Instance { get; private set; }

    public GardenState Garden { get; private set; }
    private readonly Dictionary<string, HerdState> _herds = new();

    /// <summary>A berry bush's state (Task 1.6): berries on it, and the day they were picked.</summary>
    public sealed class BushRecord
    {
        public bool HasBerries = true;
        public int PickedOnDay;
    }

    private readonly Dictionary<string, BushRecord> _bushes = new();
    public IReadOnlyDictionary<string, BushRecord> Bushes => _bushes;

    private GameClock _clock;

    public override void _EnterTree()
    {
        if (Instance == null && GetParent() == GetTree().Root) Instance = this;
        ProcessMode = ProcessModeEnum.Pausable;
    }

    public override void _ExitTree()
    {
        if (_clock != null && IsInstanceValid(_clock)) _clock.DayStarted -= OnDayStarted;
        Production.Unregister(this);
        if (Instance == this) Instance = null;
    }

    public override void _Ready()
    {
        Garden = new GardenState { Name = "Garden" };
        AddChild(Garden);
        AddHerd(new HerdState { Name = "Hens", Kind = "hen", Capacity = 5, FeedItem = ItemIds.ChickenFeed, StartingFeed = 35,
            StartingNames = new[] { "Bramble", "Clover", "Pip", "Hazel", "Daisy" } });
        AddHerd(new HerdState { Name = "Cows", Kind = "cow", Capacity = 5, FeedItem = ItemIds.Hay, StartingFeed = 7,
            StartingNames = new[] { "Maple" } });
        _clock = GameClock.Instance;
        if (_clock != null) _clock.DayStarted += OnDayStarted;
        // Each bush on the lane supplies one handful of berries for orders (Task 1.7).
        Production.Register(this, () => new[] { (ItemIds.Berries, _bushes.Count == 0 ? 5 : _bushes.Count) });
    }

    private void AddHerd(HerdState h)
    {
        AddChild(h);
        _herds[h.Kind] = h;
    }

    /// <summary>The herd of a kind ("hen" / "cow"), or null.</summary>
    public HerdState Herd(string kind) => _herds.TryGetValue(kind ?? "", out var h) ? h : null;

    public IEnumerable<HerdState> Herds => _herds.Values;

    /// <summary>The record of the bush named <paramref name="name"/> (created on first use).</summary>
    public BushRecord Bush(string name)
    {
        if (!_bushes.TryGetValue(name, out var r)) _bushes[name] = r = new BushRecord();
        return r;
    }

    /// <summary>Dawn: bushes picked at least <see cref="BerryBush.RegrowDays"/> days ago regrow.</summary>
    private void OnDayStarted(int day)
    {
        foreach (var r in _bushes.Values)
            if (!r.HasBerries && day - r.PickedOnDay >= BerryBush.RegrowDays)
                r.HasBerries = true;
    }

    /// <summary>New game: fresh garden, starting animals and full bushes.</summary>
    public void ResetForNewGame()
    {
        Garden?.ResetForNewGame();
        foreach (var h in _herds.Values) h.ResetForNewGame();
        foreach (var r in _bushes.Values) { r.HasBerries = true; r.PickedOnDay = 0; }
    }
}
