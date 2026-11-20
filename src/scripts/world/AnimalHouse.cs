using System.Collections.Generic;
using System.Linq;
using Godot;
using MyFarmingGame.Autoloads;
using MyFarmingGame.Core;

namespace MyFarmingGame.World;

/// <summary>One farm animal's data (Task 1.8 placement; Task 1.9 adds feeding and products).</summary>
public sealed class AnimalData
{
    public string Name { get; set; } = "";
    public string Kind { get; set; } = "hen";
    /// <summary>Unfed days in a row (Task 1.9).</summary>
    public int UnfedDays { get; set; }
    /// <summary>Fed today (Task 1.9).</summary>
    public bool FedToday { get; set; }
}

/// <summary>
/// The animals and feed bin of one building (Task 1.8): the old coop (5 hens, chicken feed) or the
/// old barn (stalls for 5, 1 cow, hay). Lives inside its <see cref="Building"/>. Holds the
/// <see cref="Feed"/> portions in the bin (starts with ≈ 7 days), takes phone deliveries for its
/// feed at Dawn (<see cref="FeedStore"/>), and enforces <see cref="Capacity"/>. Group
/// <see cref="Group"/>; find one with <see cref="For"/>.
/// </summary>
public partial class AnimalHouse : Node2D
{
    public const string Group = "animal_houses";

    /// <summary>"hen" or "cow".</summary>
    [Export] public string Kind { get; set; } = "hen";
    [Export] public int Capacity { get; set; } = 5;
    /// <summary>Feed item the bin holds ("chicken_feed" / "hay").</summary>
    [Export] public string FeedItem { get; set; } = ItemIds.ChickenFeed;
    [Export] public int StartingFeed { get; set; } = 35;
    [Export] public string[] StartingNames { get; set; } = { "Bramble", "Clover", "Pip", "Hazel", "Daisy" };
    /// <summary>Where the feed bin stands, relative to this node.</summary>
    [Export] public Vector2 BinOffset { get; set; } = Vector2.Zero;

    /// <summary>Names for animals bought later, used in order.</summary>
    public static readonly string[] SpareNames = { "Poppy", "Fern", "Willow", "Juniper", "Sorrel", "Bluebell", "Nutmeg", "Tansy" };

    [Signal] public delegate void FeedChangedEventHandler(int portions);
    [Signal] public delegate void AnimalsChangedEventHandler();

    public int Feed { get; private set; }
    private readonly List<AnimalData> _animals = new();
    public IReadOnlyList<AnimalData> Animals => _animals;
    public bool HasRoom => _animals.Count < Capacity;
    public FeedBin Bin { get; private set; }

    private GameClock _clock;
    private bool _started;

    public static AnimalHouse For(SceneTree tree, string kind) =>
        tree.GetNodesInGroup(Group).OfType<AnimalHouse>().FirstOrDefault(h => h.Kind == kind);

    public override void _Ready()
    {
        AddToGroup(Group);
        if (!_started) ResetForNewGame();
        Bin = new FeedBin { Name = "FeedBin", Position = BinOffset, House = this };
        AddChild(Bin);
        _clock = GameClock.Instance;
        if (_clock != null) _clock.DayStarted += OnDayStarted;
    }

    public override void _ExitTree()
    {
        if (_clock != null && IsInstanceValid(_clock)) _clock.DayStarted -= OnDayStarted;
    }

    public void ResetForNewGame()
    {
        _started = true;
        Feed = StartingFeed;
        _animals.Clear();
        foreach (var n in StartingNames)
            _animals.Add(new AnimalData { Name = n, Kind = Kind });
        EmitSignal(SignalName.FeedChanged, Feed);
        EmitSignal(SignalName.AnimalsChanged);
    }

    private void OnDayStarted(int day) => ReceiveDeliveries(day);

    /// <summary>Dawn: phone orders due today go into the bin.</summary>
    public int ReceiveDeliveries(int day)
    {
        int n = FeedStore.TakeDue(FeedItem, day);
        if (n > 0) AddFeed(n);
        return n;
    }

    public void AddFeed(int portions)
    {
        if (portions <= 0) return;
        Feed += portions;
        EmitSignal(SignalName.FeedChanged, Feed);
    }

    /// <summary>Takes feed from the bin; false (nothing taken) if there isn't enough.</summary>
    public bool TakeFeed(int portions)
    {
        if (portions <= 0 || Feed < portions) return false;
        Feed -= portions;
        EmitSignal(SignalName.FeedChanged, Feed);
        return true;
    }

    /// <summary>Adds an animal if there is room. Returns it, or null when the house is full.</summary>
    public AnimalData AddAnimal(string name = null)
    {
        if (!HasRoom) return null;
        name ??= SpareNames.FirstOrDefault(n => _animals.All(a => a.Name != n)) ?? $"{Kind} {_animals.Count + 1}";
        var a = new AnimalData { Name = name, Kind = Kind };
        _animals.Add(a);
        EmitSignal(SignalName.AnimalsChanged);
        return a;
    }

    public bool RemoveAnimal(AnimalData a)
    {
        if (!_animals.Remove(a)) return false;
        EmitSignal(SignalName.AnimalsChanged);
        return true;
    }

    /// <summary>Save/load (Task 1.10).</summary>
    public void Restore(int feed, IEnumerable<AnimalData> animals)
    {
        _started = true;
        Feed = Mathf.Max(0, feed);
        _animals.Clear();
        _animals.AddRange(animals);
        EmitSignal(SignalName.FeedChanged, Feed);
        EmitSignal(SignalName.AnimalsChanged);
    }
}
