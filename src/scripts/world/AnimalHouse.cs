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
/// The animals and feed of one building, kept alive outside every scene (Phase-002 Task 2.2: the
/// coop and barn are separate room scenes now, so this state lives in the <see cref="FarmState"/>
/// autoload and keeps counting nights while Kit is elsewhere). Tasks 1.8–1.9, animals.md §3:
///
/// * Feed bin: <see cref="Feed"/> portions, filled by phone deliveries at Dawn (<see cref="FeedStore"/>).
///   One trough slot per animal (owner 2026-11-21): <see cref="FeedOne"/> feeds one animal from the bin.
/// * Night (<see cref="EndDay"/>): fed → count reset, product ready next morning; unfed → +1 day;
///   3 unfed days in a row → gone (<see cref="AnimalLost"/>). Dawn: 2 unfed days → <see cref="HungryWarning"/>.
/// * Registers its products with <see cref="Production"/> when it listens to the clock.
/// Rule tests make a private herd with <see cref="ListenToClock"/> false.
/// </summary>
public partial class HerdState : Node
{
    public const string Group = "herds";

    public string Kind { get; set; } = "hen";
    public int Capacity { get; set; } = 5;
    public string FeedItem { get; set; } = ItemIds.ChickenFeed;
    public int StartingFeed { get; set; } = 35;
    public string[] StartingNames { get; set; } = { "Bramble", "Clover", "Pip", "Hazel", "Daisy" };
    public bool ListenToClock { get; set; } = true;

    [Signal] public delegate void FeedChangedEventHandler(int portions);
    [Signal] public delegate void AnimalsChangedEventHandler();
    [Signal] public delegate void AnimalLostEventHandler(string name, string kind);
    [Signal] public delegate void HungryWarningEventHandler(string kind);
    [Signal] public delegate void CollectedEventHandler(string item, int count);

    public int Feed { get; private set; }
    private readonly List<AnimalData> _animals = new();
    public IReadOnlyList<AnimalData> Animals => _animals;
    public bool HasRoom => _animals.Count < Capacity;
    public string ProductItem => Kind == "cow" ? ItemIds.Milk : ItemIds.Egg;
    public int ProductPerDay => Kind == "cow" ? 4 : 1;
    public int Hungry => _animals.Count(a => !a.FedToday);

    private GameClock _clock;
    private bool _started;

    public override void _Ready()
    {
        if (!_started) ResetForNewGame();
        if (!ListenToClock) return;
        AddToGroup(Group);
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

    private void OnDayStarted(int day) => Dawn(day);
    private void OnDayEnded(int day) => EndDay();

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

    public void Dawn(int day)
    {
        ReceiveDeliveries(day);
        if (_animals.Any(a => a.UnfedDays == 2))
            EmitSignal(SignalName.HungryWarning, Kind);
    }

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

    public bool TakeFeed(int portions)
    {
        if (portions <= 0 || Feed < portions) return false;
        Feed -= portions;
        EmitSignal(SignalName.FeedChanged, Feed);
        return true;
    }

    public bool FeedOne(AnimalData a)
    {
        if (a == null || a.FedToday || !_animals.Contains(a) || !TakeFeed(1)) return false;
        a.FedToday = true;
        EmitSignal(SignalName.AnimalsChanged);
        return true;
    }

    public int FeedAll()
    {
        int fed = 0;
        foreach (var a in _animals)
            if (FeedOne(a)) fed++;
        return fed;
    }

    public int Collect(AnimalData a)
    {
        if (a == null || a.ProductReady <= 0) return 0;
        int n = a.ProductReady;
        a.ProductReady = 0;
        Inventory.Instance?.Add(ProductItem, n);
        OrderManager.Instance?.NoteProduced(ProductItem);
        EmitSignal(SignalName.Collected, ProductItem, n);
        EmitSignal(SignalName.AnimalsChanged);
        return n;
    }

    public void EndDay()
    {
        var lost = new List<AnimalData>();
        foreach (var a in _animals)
        {
            if (a.FedToday)
            {
                a.UnfedDays = 0;
                a.ProductReady = Mathf.Min(a.ProductReady + ProductPerDay, ProductPerDay * AnimalHouse.MaxStoredDays);
            }
            else if (++a.UnfedDays >= AnimalData.LostAfterUnfedDays)
                lost.Add(a);
            a.FedToday = false;
        }
        foreach (var a in lost)
            _animals.Remove(a);
        EmitSignal(SignalName.AnimalsChanged);
        foreach (var a in lost)
            EmitSignal(SignalName.AnimalLost, a.Name, a.Kind);
    }

    public AnimalData AddAnimal(string name = null)
    {
        if (!HasRoom) return null;
        name ??= AnimalHouse.SpareNames.FirstOrDefault(n => _animals.All(a => a.Name != n)) ?? $"{Kind} {_animals.Count + 1}";
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

    public void Restore(int feed, IEnumerable<AnimalData> animals)
    {
        _started = true;
        Feed = Mathf.Max(0, feed);
        var copy = animals.ToList();
        _animals.Clear();
        _animals.AddRange(copy);
        EmitSignal(SignalName.FeedChanged, Feed);
        EmitSignal(SignalName.AnimalsChanged);
    }
}

/// <summary>
/// The coop or barn's animals as seen in a room (Tasks 1.8–1.9; a view since Phase-002 Task 2.2).
/// Binds to the persistent <see cref="HerdState"/> of its <see cref="Kind"/> in <see cref="FarmState"/>
/// (or makes a private one when <see cref="ListenToClock"/> is false, for rule tests), spawns one
/// <see cref="FarmAnimal"/> per animal at <see cref="Spots"/>, the feed bin and one
/// <see cref="TroughSlot"/> per <see cref="SlotSpots"/>, and passes the herd's rules and signals through.
/// </summary>
public partial class AnimalHouse : Node3D
{
    public const string Group = "animal_houses";

    /// <summary>"hen" or "cow".</summary>
    [Export] public string Kind { get; set; } = "hen";
    // Private-herd settings (rule tests); the real coop and barn take theirs from FarmState.
    [Export] public int Capacity { get; set; } = 5;
    [Export] public string FeedItem { get; set; } = ItemIds.ChickenFeed;
    [Export] public int StartingFeed { get; set; } = 35;
    [Export] public string[] StartingNames { get; set; } = { "Bramble", "Clover", "Pip", "Hazel", "Daisy" };
    /// <summary>Where the feed bin stands, relative to this node (metres).</summary>
    [Export] public Vector3 BinOffset { get; set; } = Vector3.Zero;
    /// <summary>Where each animal stands (one per capacity slot), relative to this node (metres).</summary>
    [Export] public Vector3[] Spots { get; set; } = { new(0.625f, 0, 1.1f), new(1.5f, 0, 1.1f), new(2.35f, 0, 1.1f), new(0.625f, 0, 2.1f), new(1.5f, 0, 2.1f) };
    /// <summary>Where each trough slot stands, relative to this node (metres).</summary>
    [Export] public Vector3[] SlotSpots { get; set; } = System.Array.Empty<Vector3>();
    /// <summary>Which way Kit stands from a slot to fill it (on the ground; into the room).</summary>
    [Export] public Vector3 SlotApproach { get; set; } = new(0, 0, 0.6f);
    /// <summary>True = bind to the shared herd in FarmState (the game); false = a private herd (rule tests).</summary>
    [Export] public bool ListenToClock { get; set; } = true;

    public static readonly string[] SpareNames = { "Poppy", "Fern", "Willow", "Juniper", "Sorrel", "Bluebell", "Nutmeg", "Tansy" };
    public const int MaxStoredDays = 3;

    [Signal] public delegate void FeedChangedEventHandler(int portions);
    [Signal] public delegate void AnimalsChangedEventHandler();
    [Signal] public delegate void AnimalLostEventHandler(string name, string kind);
    [Signal] public delegate void HungryWarningEventHandler(string kind);
    [Signal] public delegate void CollectedEventHandler(string item, int count);

    /// <summary>The herd this view shows (shared or private).</summary>
    public HerdState State { get; private set; }

    public int Feed => State?.Feed ?? 0;
    public IReadOnlyList<AnimalData> Animals => State?.Animals ?? (IReadOnlyList<AnimalData>)System.Array.Empty<AnimalData>();
    public bool HasRoom => State?.HasRoom ?? false;
    public string ProductItem => Kind == "cow" ? ItemIds.Milk : ItemIds.Egg;
    public int ProductPerDay => Kind == "cow" ? 4 : 1;
    public int Hungry => State?.Hungry ?? 0;

    public FeedBin Bin { get; private set; }
    private readonly List<TroughSlot> _slots = new();
    public IReadOnlyList<TroughSlot> Slots => _slots;
    private readonly List<FarmAnimal> _entities = new();
    public IReadOnlyList<FarmAnimal> Entities => _entities;

    public static AnimalHouse For(SceneTree tree, string kind) =>
        tree.GetNodesInGroup(Group).OfType<AnimalHouse>().FirstOrDefault(h => h.Kind == kind);

    public static string DisplayKind(string kind) => kind == "cow" ? "cow" : "hen";

    /// <summary>Picks the shared herd (or makes the private one) — before children need it.</summary>
    private void Bind()
    {
        if (State != null) return;
        if (ListenToClock && FarmState.Instance != null)
            State = FarmState.Instance.Herd(Kind);
        if (State == null)
        {
            State = new HerdState
            {
                Name = "Herd", Kind = Kind, Capacity = Capacity, FeedItem = FeedItem, StartingFeed = StartingFeed,
                StartingNames = StartingNames, ListenToClock = false,
            };
            AddChild(State);
        }
        else
        {
            FeedItem = State.FeedItem;
            Capacity = State.Capacity;
        }
        State.FeedChanged += OnFeedChanged;
        State.AnimalsChanged += OnAnimalsChanged;
        State.AnimalLost += OnLost;
        State.HungryWarning += OnHungry;
        State.Collected += OnCollected;
    }

    public override void _Ready()
    {
        Bind();
        AddToGroup(Group);
        Bin = new FeedBin { Name = "FeedBin", Position = BinOffset, House = this };
        AddChild(Bin);
        for (int i = 0; SlotSpots != null && i < SlotSpots.Length; i++)
        {
            var slot = new TroughSlot { Name = "Slot" + (i + 1), House = this, Index = i, Position = SlotSpots[i], ApproachOffset = SlotApproach };
            AddChild(slot);
            _slots.Add(slot);
        }
        RebuildEntities();
    }

    public override void _ExitTree()
    {
        if (State == null || !IsInstanceValid(State)) return;
        State.FeedChanged -= OnFeedChanged;
        State.AnimalsChanged -= OnAnimalsChanged;
        State.AnimalLost -= OnLost;
        State.HungryWarning -= OnHungry;
        State.Collected -= OnCollected;
        if (State.GetParent() == this) return; // private herd: freed with us
        State = null; // let a re-entering view bind again
    }

    private void OnFeedChanged(int n) => EmitSignal(SignalName.FeedChanged, n);
    private void OnAnimalsChanged()
    {
        if (IsInsideTree()) RebuildEntities();
        EmitSignal(SignalName.AnimalsChanged);
    }
    private void OnLost(string name, string kind) => EmitSignal(SignalName.AnimalLost, name, kind);
    private void OnHungry(string kind) => EmitSignal(SignalName.HungryWarning, kind);
    private void OnCollected(string item, int n) => EmitSignal(SignalName.Collected, item, n);

    private void RebuildEntities()
    {
        if (_entities.Count == Animals.Count && _entities.Select(e => e.Data).SequenceEqual(Animals))
            return;
        foreach (var e in _entities) { RemoveChild(e); e.QueueFree(); }
        _entities.Clear();
        for (int i = 0; i < Animals.Count; i++)
        {
            var e = Kind == "cow" ? (FarmAnimal)new Cow() : new Hen();
            e.Name = "Animal_" + Animals[i].Name;
            e.Data = Animals[i];
            e.House = this;
            e.Position = Spots != null && i < Spots.Length ? Spots[i] : new Vector3(0.6f + 0.8f * i, 0, 1.5f);
            AddChild(e);
            _entities.Add(e);
        }
    }

    // ---- the herd's rules, passed through ------------------------------------------------------
    public void ResetForNewGame() => State?.ResetForNewGame();
    public void Dawn(int day) => State?.Dawn(day);
    public int ReceiveDeliveries(int day) => State?.ReceiveDeliveries(day) ?? 0;
    public void AddFeed(int portions) => State?.AddFeed(portions);
    public bool TakeFeed(int portions) => State?.TakeFeed(portions) ?? false;
    public bool FeedOne(AnimalData a) => State?.FeedOne(a) ?? false;
    public int FeedAll() => State?.FeedAll() ?? 0;
    public void EndDay() => State?.EndDay();
    public AnimalData AddAnimal(string name = null) => State?.AddAnimal(name);
    public bool RemoveAnimal(AnimalData a) => State?.RemoveAnimal(a) ?? false;
    public void Restore(int feed, IEnumerable<AnimalData> animals) => State?.Restore(feed, animals);

    /// <summary>Collects one animal's products into the bag, with a golden pop. Returns how many.</summary>
    public int Collect(AnimalData a)
    {
        int n = State?.Collect(a) ?? 0;
        if (n > 0)
        {
            var who = _entities.FirstOrDefault(e => e.Data == a);
            if (who != null) Puff.At(who, who.GlobalPosition + new Vector3(0, 0.3f, 0), Puff.Gold, 16);
        }
        return n;
    }
}
