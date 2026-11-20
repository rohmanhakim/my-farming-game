using System.Collections.Generic;
using System.Linq;
using Godot;

namespace MyFarmingGame.Autoloads;

/// <summary>Item ids used in Phase-001 (inventory.md, Phase-001 §1). Plain strings so save files
/// and later data files stay readable.</summary>
public static class ItemIds
{
    public const string Berries = "berries";
    public const string Carrot = "carrot";
    public const string Egg = "egg";
    public const string Milk = "milk";
    public const string CarrotSeeds = "carrot_seeds";
    public const string FlowerSeeds = "flower_seeds";
    public const string SunSprout = "sun_sprout";
    /// <summary>Feed is never carried: it goes from the phone order into a feed bin (Task 1.8).</summary>
    public const string ChickenFeed = "chicken_feed";
    public const string Hay = "hay";

    public static readonly string[] All = { Berries, Carrot, Egg, Milk, CarrotSeeds, FlowerSeeds, SunSprout };

    /// <summary>Sprite name of an item's icon (SpriteLibrary), e.g. "item_carrot".</summary>
    public static string SpriteName(string id) => "item_" + id;

    /// <summary>Short name for tooltips and tests (the bag itself is picture-only).</summary>
    public static string DisplayName(string id) => id switch
    {
        Berries => "Wild berries",
        Carrot => "Carrot",
        Egg => "Egg",
        Milk => "Milk can",
        CarrotSeeds => "Carrot seeds",
        FlowerSeeds => "Flower seeds",
        SunSprout => "Sun Sprout",
        ChickenFeed => "Chicken feed",
        Hay => "Hay",
        _ => id,
    };
}

/// <summary>One bag slot: identical items of one quality (1–3 stars) and how many.</summary>
public sealed class ItemStack
{
    public string Id { get; }
    public int Quality { get; }
    public int Count { get; internal set; }

    public ItemStack(string id, int quality, int count)
    {
        Id = id;
        Quality = quality;
        Count = count;
    }
}

/// <summary>
/// Kit's bag (Task 1.4, inventory.md): auto-stacking, no sorting, no weight, no "full". Identical
/// items stack into one slot with a number badge; different qualities stack separately (only ★ in
/// Phase-001). Slots keep the order items first arrived in. State only; <c>UI/BagView</c> draws it.
/// </summary>
public partial class Inventory : Node
{
    public static Inventory Instance { get; private set; }

    public const int MinQuality = 1;
    public const int MaxQuality = 3;

    /// <summary>What a new game starts with (items.md v0.9: the seed bag holds one carrot packet,
    /// 9 seeds, enough for Marlow's 5-carrot order).</summary>
    public static readonly (string Id, int Count)[] StartingItems = { (ItemIds.CarrotSeeds, 9) };

    /// <summary>Emitted after any add or remove.</summary>
    [Signal] public delegate void ChangedEventHandler();

    private readonly List<ItemStack> _stacks = new();

    /// <summary>Non-empty stacks in arrival order.</summary>
    public IReadOnlyList<ItemStack> Stacks => _stacks;

    public Inventory()
    {
        foreach (var (id, count) in StartingItems)
            _stacks.Add(new ItemStack(id, MinQuality, count));
    }

    public override void _EnterTree()
    {
        if (Instance == null && GetParent() == GetTree().Root)
            Instance = this;
        ProcessMode = ProcessModeEnum.Pausable; // project-structure.md §4 process-mode table
    }

    public override void _ExitTree()
    {
        if (Instance == this)
            Instance = null;
    }

    private ItemStack Find(string id, int quality) =>
        _stacks.FirstOrDefault(s => s.Id == id && s.Quality == quality);

    /// <summary>Adds <paramref name="count"/> items. Always fits (no "full"). Returns the new
    /// stack size, or 0 when nothing was added (bad id/count).</summary>
    public int Add(string id, int count = 1, int quality = MinQuality)
    {
        if (string.IsNullOrEmpty(id) || count <= 0)
            return 0;
        quality = Mathf.Clamp(quality, MinQuality, MaxQuality);
        var stack = Find(id, quality);
        if (stack == null)
        {
            stack = new ItemStack(id, quality, 0);
            _stacks.Add(stack);
        }
        stack.Count += count;
        EmitSignal(SignalName.Changed);
        return stack.Count;
    }

    /// <summary>Removes <paramref name="count"/> items, all or nothing. Returns false (and changes
    /// nothing) if Kit has fewer. An emptied stack disappears.</summary>
    public bool Remove(string id, int count = 1, int quality = MinQuality)
    {
        if (count <= 0)
            return false;
        var stack = Find(id, Mathf.Clamp(quality, MinQuality, MaxQuality));
        if (stack == null || stack.Count < count)
            return false;
        stack.Count -= count;
        if (stack.Count == 0)
            _stacks.Remove(stack);
        EmitSignal(SignalName.Changed);
        return true;
    }

    /// <summary>How many of one item and quality Kit has.</summary>
    public int Count(string id, int quality = MinQuality) =>
        Find(id, Mathf.Clamp(quality, MinQuality, MaxQuality))?.Count ?? 0;

    /// <summary>How many of one item Kit has, all qualities together.</summary>
    public int CountAll(string id) => _stacks.Where(s => s.Id == id).Sum(s => s.Count);

    public bool Has(string id, int count = 1, int quality = MinQuality) => Count(id, quality) >= count;

    /// <summary>Empties the bag (save/load restores into an empty bag; tests).</summary>
    public void Clear()
    {
        _stacks.Clear();
        EmitSignal(SignalName.Changed);
    }

    /// <summary>New game: only the starting items (<see cref="StartingItems"/>).</summary>
    public void ResetForNewGame()
    {
        _stacks.Clear();
        foreach (var (id, count) in StartingItems)
            _stacks.Add(new ItemStack(id, MinQuality, count));
        EmitSignal(SignalName.Changed);
    }
}
