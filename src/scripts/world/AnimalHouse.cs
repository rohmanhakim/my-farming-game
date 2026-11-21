using System.Collections.Generic;
using System.Linq;
using Godot;
using MyFarmingGame.Autoloads;
using MyFarmingGame.Core;

namespace MyFarmingGame.World;

/// <summary>
/// One farm animal (Tasks 1.8–1.9, animals.md §3): fed today?, unfed days in a row, and products
/// waiting to be collected. Rules live in <see cref="AnimalHouse.EndDay"/>.
/// </summary>
public sealed class AnimalData
{
    public string Name { get; set; } = "";
    public string Kind { get; set; } = "hen";
    /// <summary>Unfed days in a row.</summary>
    public int UnfedDays { get; set; }
    /// <summary>Fed today.</summary>
    public bool FedToday { get; set; }
    /// <summary>Products ready to collect (eggs / milk cans).</summary>
    public int ProductReady { get; set; }

    public const int LostAfterUnfedDays = 3;

    /// <summary>Heart meter in thirds (animals.md §3): 3 = full (fed today, or no day missed yet),
    /// 2 = ⅔ after 1 unfed day, 1 = ⅓ after 2.</summary>
    public int Hearts => FedToday ? 3 : Mathf.Clamp(3 - UnfedDays, 1, 3);

    /// <summary>Need icon above the animal: it missed yesterday's feed and isn't fed yet today.</summary>
    public bool ShowsNeed => !FedToday && UnfedDays >= 1;

    /// <summary>The need icon flashes (2 unfed days: the warning day).</summary>
    public bool NeedFlashes => !FedToday && UnfedDays >= 2;

    public Godot.Collections.Dictionary ToDict() => new()
    {
        { "name", Name }, { "kind", Kind }, { "unfedDays", UnfedDays }, { "fedToday", FedToday }, { "product", ProductReady },
    };

    public static AnimalData FromDict(Godot.Collections.Dictionary d) => new()
    {
        Name = d.TryGetValue("name", out var n) ? n.AsString() : "",
        Kind = d.TryGetValue("kind", out var k) ? k.AsString() : "hen",
        UnfedDays = d.TryGetValue("unfedDays", out var u) ? u.AsInt32() : 0,
        FedToday = d.TryGetValue("fedToday", out var f) && f.AsBool(),
        ProductReady = d.TryGetValue("product", out var p) ? p.AsInt32() : 0,
    };
}

/// <summary>
/// The animals and feed bin of one building (Tasks 1.8–1.9, animals.md §3): the old coop (5 hens,
/// chicken feed, 1 egg each) or the old barn (stalls for 5, 1 cow, hay, 4 milk cans).
///
/// * Feed bin: <see cref="Feed"/> portions (starts with ≈ 7 days), filled by phone deliveries at
///   Dawn (<see cref="FeedStore"/>). A <b>trough with one slot per animal</b> (<see cref="Slots"/>,
///   owner 2026-11-21): with the feed scoop, filling a slot (or using its animal) takes one portion
///   from the bin and feeds that animal (<see cref="FeedOne"/>); the food shows until the night.
///   <see cref="FeedAll"/> stays for rules and tests. Once per day each.
/// * Night (<see cref="GameClock.DayEnded"/> → <see cref="EndDay"/>): a fed animal resets its count
///   and has its product ready next morning; an unfed one counts a day; at 3 unfed days in a row it
///   is gone (<see cref="AnimalLost"/>; the UI shows the gentle card). Nothing else is removed.
/// * Dawn: an animal at 2 unfed days raises <see cref="HungryWarning"/> (Marlow/Ada mention it).
/// * Registers its products with <see cref="Production"/> so egg and milk orders appear.
/// Spawns one <see cref="FarmAnimal"/> per animal at <see cref="Spots"/>.
/// </summary>
public partial class AnimalHouse : Node3D
{
    public const string Group = "animal_houses";

    /// <summary>"hen" or "cow".</summary>
    [Export] public string Kind { get; set; } = "hen";
    [Export] public int Capacity { get; set; } = 5;
    /// <summary>Feed item the bin holds ("chicken_feed" / "hay").</summary>
    [Export] public string FeedItem { get; set; } = ItemIds.ChickenFeed;
    [Export] public int StartingFeed { get; set; } = 35;
    [Export] public string[] StartingNames { get; set; } = { "Bramble", "Clover", "Pip", "Hazel", "Daisy" };
    /// <summary>Where the feed bin stands, relative to this node (metres; this node sits at the
    /// building's north-west corner).</summary>
    [Export] public Vector3 BinOffset { get; set; } = Vector3.Zero;
    /// <summary>Where each animal stands (one per capacity slot), relative to this node (metres).</summary>
    [Export] public Vector3[] Spots { get; set; } = { new(0.625f, 0, 1.1f), new(1.5f, 0, 1.1f), new(2.35f, 0, 1.1f), new(0.625f, 0, 2.1f), new(1.5f, 0, 2.1f) };
    /// <summary>Where each trough slot stands (one per capacity slot), relative to this node (metres).</summary>
    [Export] public Vector3[] SlotSpots { get; set; } = System.Array.Empty<Vector3>();
    /// <summary>Which way Kit stands from a slot to fill it (on the ground; into the room).</summary>
    [Export] public Vector3 SlotApproach { get; set; } = new(0, 0, 0.6f);
    /// <summary>When false the house ignores the clock (rule tests call <see cref="EndDay"/>).</summary>
    [Export] public bool ListenToClock { get; set; } = true;

    /// <summary>Names for animals bought later, used in order.</summary>
    public static readonly string[] SpareNames = { "Poppy", "Fern", "Willow", "Juniper", "Sorrel", "Bluebell", "Nutmeg", "Tansy" };

    [Signal] public delegate void FeedChangedEventHandler(int portions);
    [Signal] public delegate void AnimalsChangedEventHandler();
    /// <summary>An animal was lost to 3 unfed days. Arguments: name, kind.</summary>
    [Signal] public delegate void AnimalLostEventHandler(string name, string kind);
    /// <summary>Dawn: at least one animal here has gone 2 days unfed. Argument: kind.</summary>
    [Signal] public delegate void HungryWarningEventHandler(string kind);
    /// <summary>Products were collected. Arguments: item, count.</summary>
    [Signal] public delegate void CollectedEventHandler(string item, int count);

    public int Feed { get; private set; }
    private readonly List<AnimalData> _animals = new();
    public IReadOnlyList<AnimalData> Animals => _animals;
    public bool HasRoom => _animals.Count < Capacity;
    public FeedBin Bin { get; private set; }
    private readonly List<TroughSlot> _slots = new();
    /// <summary>The trough slots; slot i feeds animal i.</summary>
    public IReadOnlyList<TroughSlot> Slots => _slots;
    private readonly List<FarmAnimal> _entities = new();
    public IReadOnlyList<FarmAnimal> Entities => _entities;

    /// <summary>Product item of this house's animals: egg (hen) or milk (cow).</summary>
    public string ProductItem => Kind == "cow" ? ItemIds.Milk : ItemIds.Egg;
    /// <summary>Products per fed animal per day: 1 egg per hen, 4 milk cans per cow (animals.md §2).</summary>
    public int ProductPerDay => Kind == "cow" ? 4 : 1;
    /// <summary>Uncollected products pile up to this many days' worth.</summary>
    public const int MaxStoredDays = 3;

    private GameClock _clock;
    private bool _started;

    public static AnimalHouse For(SceneTree tree, string kind) =>
        tree.GetNodesInGroup(Group).OfType<AnimalHouse>().FirstOrDefault(h => h.Kind == kind);

    public static string DisplayKind(string kind) => kind == "cow" ? "cow" : "hen";

    public override void _Ready()
    {
        AddToGroup(Group);
        if (!_started) ResetForNewGame();
        Bin = new FeedBin { Name = "FeedBin", Position = BinOffset, House = this };
        AddChild(Bin);
        for (int i = 0; SlotSpots != null && i < SlotSpots.Length; i++)
        {
            var slot = new TroughSlot { Name = "Slot" + (i + 1), House = this, Index = i, Position = SlotSpots[i], ApproachOffset = SlotApproach };
            AddChild(slot);
            _slots.Add(slot);
        }
        RebuildEntities();
        if (!ListenToClock) return;
        _clock = GameClock.Instance;
        if (_clock != null)
        {
            _clock.DayStarted += OnDayStarted;
            _clock.DayEnded += OnDayEnded;
        }
        Production.Register(this, () => new[] { (ProductItem, _animals.Count * ProductPerDay) });
    }

    public override void _ExitTree()
    {
        if (_clock != null && IsInstanceValid(_clock))
        {
            _clock.DayStarted -= OnDayStarted;
            _clock.DayEnded -= OnDayEnded;
        }
        Production.Unregister(this);
    }

    public void ResetForNewGame()
    {
        _started = true;
        Feed = StartingFeed;
        _animals.Clear();
        foreach (var n in StartingNames)
            _animals.Add(new AnimalData { Name = n, Kind = Kind });
        EmitSignal(SignalName.FeedChanged, Feed);
        Changed();
    }

    private void Changed()
    {
        if (IsInsideTree()) RebuildEntities();
        EmitSignal(SignalName.AnimalsChanged);
    }

    private void RebuildEntities()
    {
        if (_entities.Count == _animals.Count && _entities.Select(e => e.Data).SequenceEqual(_animals))
            return; // same animals: their props redraw themselves
        foreach (var e in _entities) { RemoveChild(e); e.QueueFree(); }
        _entities.Clear();
        for (int i = 0; i < _animals.Count; i++)
        {
            var e = Kind == "cow" ? (FarmAnimal)new Cow() : new Hen();
            e.Name = "Animal_" + _animals[i].Name;
            e.Data = _animals[i];
            e.House = this;
            e.Position = Spots != null && i < Spots.Length ? Spots[i] : new Vector3(0.6f + 0.8f * i, 0, 1.5f);
            AddChild(e);
            _entities.Add(e);
        }
    }

    private void OnDayStarted(int day) => Dawn(day);

    private void OnDayEnded(int day) => EndDay();

    /// <summary>Dawn: phone orders due today go into the bin; warn about animals on their second
    /// hungry day.</summary>
    public void Dawn(int day)
    {
        ReceiveDeliveries(day);
        if (_animals.Any(a => a.UnfedDays == 2))
            EmitSignal(SignalName.HungryWarning, Kind);
    }

    /// <summary>Phone orders due by <paramref name="day"/> go into the bin.</summary>
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

    /// <summary>Feeds one animal one portion from the bin (once per day). True if it was fed now.</summary>
    public bool FeedOne(AnimalData a)
    {
        if (a == null || a.FedToday || !_animals.Contains(a) || !TakeFeed(1)) return false;
        a.FedToday = true;
        EmitSignal(SignalName.AnimalsChanged);
        return true;
    }

    /// <summary>The feed scoop at the bin: every hungry animal gets one portion while the bin lasts.
    /// Returns how many were fed.</summary>
    public int FeedAll()
    {
        int fed = 0;
        foreach (var a in _animals)
            if (FeedOne(a)) fed++;
        return fed;
    }

    public int Hungry => _animals.Count(a => !a.FedToday);

    /// <summary>Collects one animal's products into the bag. Returns how many.</summary>
    public int Collect(AnimalData a)
    {
        if (a == null || a.ProductReady <= 0) return 0;
        int n = a.ProductReady;
        a.ProductReady = 0;
        Inventory.Instance?.Add(ProductItem, n);
        var who = _entities.FirstOrDefault(e => e.Data == a);
        if (who != null) Puff.At(who, who.GlobalPosition + new Vector3(0, 0.3f, 0), Puff.Gold, 16);
        OrderManager.Instance?.NoteProduced(ProductItem);
        EmitSignal(SignalName.Collected, ProductItem, n);
        EmitSignal(SignalName.AnimalsChanged);
        return n;
    }

    /// <summary>The night (animals.md §3): fed → count reset, product ready tomorrow; unfed →
    /// +1 day; 3 unfed days in a row → gone (one <see cref="AnimalLost"/> each).</summary>
    public void EndDay()
    {
        var lost = new List<AnimalData>();
        foreach (var a in _animals)
        {
            if (a.FedToday)
            {
                a.UnfedDays = 0;
                a.ProductReady = Mathf.Min(a.ProductReady + ProductPerDay, ProductPerDay * MaxStoredDays);
            }
            else if (++a.UnfedDays >= AnimalData.LostAfterUnfedDays)
                lost.Add(a);
            a.FedToday = false;
        }
        foreach (var a in lost)
            _animals.Remove(a);
        Changed();
        foreach (var a in lost)
            EmitSignal(SignalName.AnimalLost, a.Name, a.Kind);
    }

    /// <summary>Adds an animal if there is room. Returns it, or null when the house is full.</summary>
    public AnimalData AddAnimal(string name = null)
    {
        if (!HasRoom) return null;
        name ??= SpareNames.FirstOrDefault(n => _animals.All(a => a.Name != n)) ?? $"{Kind} {_animals.Count + 1}";
        var a = new AnimalData { Name = name, Kind = Kind };
        _animals.Add(a);
        Changed();
        return a;
    }

    public bool RemoveAnimal(AnimalData a)
    {
        if (!_animals.Remove(a)) return false;
        Changed();
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
        Changed();
    }
}
