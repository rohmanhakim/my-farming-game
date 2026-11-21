using System.Collections.Generic;
using System.Linq;
using Godot;
using MyFarmingGame.Autoloads;
using MyFarmingGame.Core;
using MyFarmingGame.World;
using GDict = Godot.Collections.Dictionary;
using GArray = Godot.Collections.Array;

namespace MyFarmingGame;

/// <summary>
/// Connects the game's state to <see cref="SaveGame"/> (Task 1.10). Lives in Main/Systems. Registers
/// one save section per owner — clock, inventory (bag + tool belt), wallet, orders, feed (bins +
/// phone orders on the way), animals, farm (garden tiles), forage (berry bushes), ledger, player —
/// loads the save once the level is ready, and auto-saves after every night (when Kit wakes),
/// every purchase and every delivered order. Onboarding (Task 1.12) registers its own section.
/// </summary>
public partial class SaveBinder : Node
{
    public static readonly string[] SectionNames =
        { "clock", "inventory", "wallet", "orders", "feed", "animals", "farm", "forage", "ledger", "player" };

    [Export] public NodePath PlayerPath { get; set; }
    [Export] public NodePath CameraPath { get; set; }
    [Export] public NodePath LevelPath { get; set; }
    [Export] public NodePath ShopPath { get; set; }

    private SaveGame _save;
    private Node3D _player;
    private CameraRig _camera;
    private Node _level;
    private UI.ShopView _shop;

    /// <summary>True when the last start-up restored a save.</summary>
    public bool LoadedAtStart { get; private set; }

    public override void _Ready()
    {
        _save = SaveGame.Instance;
        _player = GetNodeOrNull<Node3D>(PlayerPath);
        _camera = GetNodeOrNull<CameraRig>(CameraPath);
        _level = GetNodeOrNull(LevelPath);
        _shop = GetNodeOrNull<UI.ShopView>(ShopPath);
        if (_save == null) return;
        RegisterAll();
        if (GameClock.Instance != null) GameClock.Instance.WokeUp += AutoSave;
        if (OrderManager.Instance != null) OrderManager.Instance.OrderPaid += OnOrderPaid;
        if (_shop != null) _shop.Bought += OnBought;
    }

    /// <summary>Called by Main after the level and player are placed: restore the save, if any.</summary>
    public void LoadAtStart()
    {
        LoadedAtStart = _save?.Enabled == true && _save.Load();
        if (LoadedAtStart) _camera?.Snap();
    }

    public override void _ExitTree()
    {
        if (_save != null && IsInstanceValid(_save))
            foreach (var s in SectionNames) _save.Unregister(s);
        if (GameClock.Instance != null) GameClock.Instance.WokeUp -= AutoSave;
        if (OrderManager.Instance != null) OrderManager.Instance.OrderPaid -= OnOrderPaid;
        if (_shop != null && IsInstanceValid(_shop)) _shop.Bought -= OnBought;
    }

    private void AutoSave() => _save?.Save();
    private void OnOrderPaid(string id, int coins) => AutoSave();
    private void OnBought(string id, int price) => AutoSave();

    private IEnumerable<T> InLevel<T>(string group) where T : Node =>
        GetTree().GetNodesInGroup(group).OfType<T>().Where(n => _level != null && _level.IsAncestorOf(n));

    private void RegisterAll()
    {
        _save.Register("clock", SaveClock, LoadClock);
        _save.Register("inventory", SaveInventory, LoadInventory);
        _save.Register("wallet", () => new GDict { { "coins", Wallet.Instance?.Coins ?? 0 } },
            d => Wallet.Instance?.SetCoins(d["coins"].AsInt32()));
        _save.Register("orders", SaveOrders, LoadOrders);
        _save.Register("feed", SaveFeed, LoadFeed);
        _save.Register("animals", SaveAnimals, LoadAnimals);
        _save.Register("farm", SaveFarm, LoadFarm);
        _save.Register("forage", SaveForage, LoadForage);
        _save.Register("ledger", SaveLedger, LoadLedger);
        _save.Register("player", SavePlayer, LoadPlayer);
    }

    // ---- clock -------------------------------------------------------------------------------
    private GDict SaveClock() => new() { { "day", GameClock.Instance?.Day ?? 1 }, { "minutes", GameClock.Instance?.MinutesIntoDay ?? 0 } };
    private void LoadClock(GDict d) => GameClock.Instance?.SetTime(d["day"].AsInt32(), d["minutes"].AsDouble());

    // ---- inventory + tool belt ---------------------------------------------------------------
    private GDict SaveInventory()
    {
        var stacks = new GArray();
        foreach (var s in Inventory.Instance?.Stacks ?? new List<ItemStack>())
            stacks.Add(new GDict { { "id", s.Id }, { "quality", s.Quality }, { "count", s.Count } });
        var tools = new GArray();
        foreach (var t in ToolBelt.Instance?.Tools ?? new List<Tool>())
            tools.Add(t.ToString());
        return new GDict
        {
            { "stacks", stacks }, { "tools", tools },
            { "activeTool", (ToolBelt.Instance?.Active ?? Tool.Hands).ToString() },
            { "seed", ToolBelt.Instance?.SelectedSeed ?? "" },
        };
    }

    private void LoadInventory(GDict d)
    {
        var inv = Inventory.Instance;
        if (inv != null)
        {
            inv.Clear();
            foreach (var v in d["stacks"].AsGodotArray())
            {
                var s = v.AsGodotDictionary();
                inv.Add(s["id"].AsString(), s["count"].AsInt32(), s["quality"].AsInt32());
            }
        }
        var belt = ToolBelt.Instance;
        if (belt == null) return;
        var owned = d["tools"].AsGodotArray().Select(v => System.Enum.TryParse<Tool>(v.AsString(), out var t) ? t : Tool.Hands).ToList();
        System.Enum.TryParse<Tool>(d["activeTool"].AsString(), out var active);
        belt.Restore(owned, active, d.TryGetValue("seed", out var seed) ? seed.AsString() : "");
    }

    // ---- orders ------------------------------------------------------------------------------
    private GDict SaveOrders()
    {
        var om = OrderManager.Instance;
        var list = new GArray();
        var delivered = new GDict();
        if (om != null)
        {
            foreach (var o in om.Orders) list.Add(o.ToDict());
            foreach (var (k, v) in om.Delivered) delivered[k] = v;
        }
        var neighbours = new GArray();
        var milestones = new GArray();
        if (om != null)
        {
            foreach (var n in om.ActiveNeighbours.OrderBy(n => n)) neighbours.Add(n);
            foreach (var m in om.Milestones.OrderBy(m => m)) milestones.Add(m);
        }
        return new GDict
        {
            { "orders", list }, { "boardOpen", om?.BoardOpen ?? false }, { "delivered", delivered }, { "seed", om?.Seed ?? 1 },
            { "neighbours", neighbours }, { "milestones", milestones },
        };
    }

    private void LoadOrders(GDict d)
    {
        var orders = d["orders"].AsGodotArray().Select(v => Order.FromDict(v.AsGodotDictionary())).ToList();
        var delivered = new Dictionary<string, int>();
        foreach (var (k, v) in d["delivered"].AsGodotDictionary()) delivered[k.AsString()] = v.AsInt32();
        var neighbours = d.TryGetValue("neighbours", out var nb) ? nb.AsGodotArray().Select(v => v.AsString()).ToList() : new List<string>();
        var milestones = d.TryGetValue("milestones", out var ms) ? ms.AsGodotArray().Select(v => v.AsString()).ToList() : new List<string>();
        OrderManager.Instance?.Restore(orders, d["boardOpen"].AsBool(), delivered, d["seed"].AsInt32(), neighbours, milestones);
    }

    // ---- feed (bins + deliveries) and animals ------------------------------------------------
    private GDict SaveFeed()
    {
        var bins = new GDict();
        foreach (var h in Herds) bins[h.Kind] = h.Feed;
        var pending = new GArray();
        foreach (var p in FeedStore.Pending)
            pending.Add(new GDict { { "item", p.Item }, { "amount", p.Amount }, { "day", p.DeliverOnDay } });
        return new GDict { { "bins", bins }, { "pending", pending } };
    }

    private void LoadFeed(GDict d)
    {
        FeedStore.Restore(d["pending"].AsGodotArray().Select(v =>
        {
            var p = v.AsGodotDictionary();
            return new FeedDelivery { Item = p["item"].AsString(), Amount = p["amount"].AsInt32(), DeliverOnDay = p["day"].AsInt32() };
        }).ToList());
        var bins = d["bins"].AsGodotDictionary();
        foreach (var h in Herds)
            if (bins.TryGetValue(h.Kind, out var n))
                h.Restore(n.AsInt32(), h.Animals.ToList());
    }

    private GDict SaveAnimals()
    {
        var d = new GDict();
        foreach (var h in Herds)
        {
            var list = new GArray();
            foreach (var a in h.Animals) list.Add(a.ToDict());
            d[h.Kind] = list;
        }
        return d;
    }

    private void LoadAnimals(GDict d)
    {
        foreach (var h in Herds)
            if (d.TryGetValue(h.Kind, out var list))
                h.Restore(h.Feed, list.AsGodotArray().Select(v => AnimalData.FromDict(v.AsGodotDictionary())).ToList());
    }

    // ---- farm, forage, ledger, player --------------------------------------------------------
    // Phase-002 Task 2.2: the garden, herds and bushes live in FarmState, not in a scene.
    private static GardenState Garden => FarmState.Instance?.Garden;
    private static IEnumerable<HerdState> Herds => FarmState.Instance?.Herds ?? Enumerable.Empty<HerdState>();

    private GDict SaveFarm()
    {
        var g = Garden;
        var tiles = new GArray();
        if (g != null) foreach (var (_, t) in g.AllTiles()) tiles.Add(t.ToDict());
        return new GDict { { "columns", g?.Columns ?? 0 }, { "rows", g?.Rows ?? 0 }, { "tiles", tiles } };
    }

    private void LoadFarm(GDict d) =>
        Garden?.SetTiles(d["tiles"].AsGodotArray().Select(v => CropTile.FromDict(v.AsGodotDictionary())).ToList());

    private GDict SaveForage()
    {
        var bushes = new GDict();
        foreach (var (name, r) in FarmState.Instance?.Bushes ?? new Dictionary<string, FarmState.BushRecord>())
            bushes[name] = new GDict { { "berries", r.HasBerries }, { "pickedOnDay", r.PickedOnDay } };
        return new GDict { { "bushes", bushes } };
    }

    private void LoadForage(GDict d)
    {
        var bushes = d["bushes"].AsGodotDictionary();
        foreach (var (k, v) in bushes)
        {
            var s = v.AsGodotDictionary();
            var r = FarmState.Instance?.Bush(k.AsString());
            if (r == null) continue;
            r.HasBerries = s["berries"].AsBool();
            r.PickedOnDay = s["pickedOnDay"].AsInt32();
        }
        foreach (var b in InLevel<BerryBush>(BerryBush.Group)) b.SetState(b.HasBerries, b.PickedOnDay); // redraw
    }

    private GDict SaveLedger()
    {
        var pages = new GArray();
        foreach (var p in Ledger.Pages)
            pages.Add(new GDict { { "key", p.Key }, { "sprite", p.Sprite }, { "done", p.Done } });
        return new GDict { { "pages", pages } };
    }

    private void LoadLedger(GDict d) => Ledger.Restore(d["pages"].AsGodotArray().Select(v =>
    {
        var p = v.AsGodotDictionary();
        return new LedgerPage { Key = p["key"].AsString(), Sprite = p["sprite"].AsString(), Done = p["done"].AsBool() };
    }).ToList());

    private GDict SavePlayer()
    {
        // Metres on the ground plane since schemaVersion 2 (3D, Task 1.20); v1 stored pixels as x / y.
        var pos = _player?.GlobalPosition ?? Vector3.Zero;
        var facing = (_player as Player.PlayerController)?.Facing ?? Player.Facing.Down;
        // Phase-002 Task 2.2 (schema 3): which scene Kit is in (rooms are separate scenes).
        string level = (GetParent()?.GetParent() as Main)?.CurrentLevelPath ?? "";
        return new GDict { { "x", pos.X }, { "z", pos.Z }, { "facing", facing.ToString() }, { "level", level } };
    }

    private void LoadPlayer(GDict d)
    {
        if (_player == null) return;
        if (d.TryGetValue("level", out var lv) && lv.AsString() is { Length: > 0 } level && GetParent()?.GetParent() is Main main)
            main.LoadLevelNow(level);
        _player.GlobalPosition = new Vector3((float)d["x"].AsDouble(), 0, (float)d["z"].AsDouble());
        if (_player is Player.PlayerController pc && System.Enum.TryParse<Player.Facing>(d["facing"].AsString(), out var f))
            pc.Face(f);
        _camera?.Snap();
    }
}
