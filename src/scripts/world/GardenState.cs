using System.Collections.Generic;
using Godot;
using MyFarmingGame.Autoloads;
using MyFarmingGame.Core;

namespace MyFarmingGame.World;

/// <summary>
/// The front garden's tiles and rules, kept alive outside every scene (Phase-002 Task 2.2: Kit
/// sleeps in the house room while the farm scene is not loaded, so the garden still grows, dries
/// and is lost at night). Lives in <see cref="FarmState"/>; <see cref="FarmGrid"/> is its view in the
/// farm scene. Rules (Task 1.5, crops.md §2): hands pull weeds; the hoe tills cleared or weedy ground;
/// the seed bag plants the chosen packet on an empty tilled tile; the can waters tilled soil (a
/// yellow crop recovers); any tool harvests a ready crop; a ready crop never dries. Growth and
/// neglect run on <see cref="GameClock.DayEnded"/> (<see cref="EndDay"/>).
/// </summary>
public partial class GardenState : Node
{
    public int Columns { get; set; } = 8;
    public int Rows { get; set; } = 6;
    /// <summary>Tiles that start tilled (the "3 beds already clear", buildings.md §2).</summary>
    public Godot.Collections.Array<Vector2I> StartingBeds { get; set; } = new() { new(5, 3), new(6, 3), new(7, 3) };
    /// <summary>False for a private garden in rule tests (no clock, no production).</summary>
    public bool ListenToClock { get; set; } = true;

    /// <summary>Task 1.12 nudge "morning after planting": a can icon over every dry bed (the view draws it).</summary>
    public bool WaterHints { get; set; }

    /// <summary>Where the garden's north-west corner is in the farm scene (set by its view).</summary>
    public Vector3 WorldOrigin { get; set; } = new(16, 0, 1);

    [Signal] public delegate void TileChangedEventHandler(Vector2I cell);
    [Signal] public delegate void CropLostEventHandler(Vector2I cell);
    [Signal] public delegate void CropHarvestedEventHandler(Vector2I cell, string item);
    [Signal] public delegate void ToolUsedEventHandler(int action, Vector2I cell);
    /// <summary>Many tiles changed at once (night, reset, load): redraw everything.</summary>
    [Signal] public delegate void ChangedEventHandler();

    private CropTile[] _tiles;
    private GameClock _clock;

    private CropTile[] Tiles
    {
        get
        {
            if (_tiles == null) ResetForNewGame();
            return _tiles;
        }
    }

    public CropTile Get(Vector2I cell) => InBounds(cell) ? Tiles[cell.Y * Columns + cell.X] : null;

    public bool InBounds(Vector2I cell) => cell.X >= 0 && cell.Y >= 0 && cell.X < Columns && cell.Y < Rows;

    public IEnumerable<(Vector2I Cell, CropTile Tile)> AllTiles()
    {
        for (int y = 0; y < Rows; y++)
            for (int x = 0; x < Columns; x++)
                yield return (new Vector2I(x, y), Tiles[y * Columns + x]);
    }

    /// <summary>The centre of a cell in the farm scene (on the ground).</summary>
    public Vector3 CellCenter(Vector2I cell) => WorldOrigin + new Vector3(cell.X + 0.5f, 0, cell.Y + 0.5f);

    /// <summary>True when a farm-scene point lies on a garden tile.</summary>
    public bool ContainsPoint(Vector3 p) =>
        InBounds(new Vector2I(Mathf.FloorToInt(p.X - WorldOrigin.X), Mathf.FloorToInt(p.Z - WorldOrigin.Z)));

    public bool AnyReady
    {
        get
        {
            foreach (var (_, t) in AllTiles()) if (t.IsReady) return true;
            return false;
        }
    }

    public override void _Ready()
    {
        _ = Tiles;
        if (!ListenToClock || GameClock.Instance == null) return;
        _clock = GameClock.Instance;
        _clock.DayEnded += OnDayEnded;
        Production.Register(this, SupplyForOrders);
    }

    public override void _ExitTree()
    {
        if (_clock != null && IsInstanceValid(_clock)) _clock.DayEnded -= OnDayEnded;
        Production.Unregister(this);
    }

    private void OnDayEnded(int day) => EndDay();

    /// <summary>Carrots the garden can supply for an order: one per bed clear of weeds (Task 1.7).</summary>
    private IEnumerable<(string, int)> SupplyForOrders()
    {
        int beds = 0;
        foreach (var (_, t) in AllTiles())
            if (t.Soil != Soil.Weeds) beds++;
        foreach (var r in CropCatalog.Default.Crops)
            if (r is CropDefinition c && Inventory.Instance != null)
                yield return (c.HarvestItem, beds);
    }

    /// <summary>New game: weeds everywhere except the starting beds (tilled, empty).</summary>
    public void ResetForNewGame()
    {
        _tiles = new CropTile[Columns * Rows];
        for (int i = 0; i < _tiles.Length; i++)
            _tiles[i] = new CropTile();
        foreach (var b in StartingBeds)
            if (InBounds(b))
                _tiles[b.Y * Columns + b.X].Soil = Soil.Tilled;
        EmitSignal(SignalName.Changed);
    }

    /// <summary>Uses <paramref name="tool"/> on <paramref name="cell"/> and returns what happened.</summary>
    public TileAction UseTool(Tool tool, Vector2I cell)
    {
        var t = Get(cell);
        if (t == null) return TileAction.Nothing;
        TileAction done = TileAction.Nothing;
        if (t.IsReady)
            done = Harvest(cell, t); // any tool picks a ready crop (kid-friendly)
        else
        {
            switch (tool)
            {
                case Tool.Hands when t.Soil == Soil.Weeds:
                    t.Soil = Soil.Clear;
                    done = TileAction.Cleared;
                    break;
                case Tool.Hoe when t.Soil == Soil.Clear:
                    t.Soil = Soil.Tilled;
                    done = TileAction.Tilled;
                    break;
                case Tool.Hoe when t.Soil == Soil.Weeds:
                    // Owner playtest 2026-11-21: the hoe digs weedy ground too (counts as a weed pulled).
                    t.Soil = Soil.Tilled;
                    EmitSignal(SignalName.ToolUsed, (int)TileAction.Cleared, cell);
                    done = TileAction.Tilled;
                    break;
                case Tool.SeedBag when t.Soil == Soil.Tilled && !t.HasCrop:
                    done = Plant(t);
                    break;
                case Tool.WateringCan when t.Soil == Soil.Tilled:
                    t.Watered = true;
                    t.DryDays = 0; // a yellow crop recovers at once
                    done = TileAction.Watered;
                    break;
            }
        }
        if (done != TileAction.Nothing)
        {
            EmitSignal(SignalName.TileChanged, cell);
            EmitSignal(SignalName.ToolUsed, (int)done, cell);
        }
        return done;
    }

    private static TileAction Plant(CropTile t)
    {
        var inv = Inventory.Instance;
        if (inv == null) return TileAction.Nothing;
        var stacks = new List<ItemStack>(inv.Stacks);
        string chosen = ToolBelt.Instance?.SelectedSeed ?? "";
        stacks.Sort((a, b) => (b.Id == chosen ? 1 : 0) - (a.Id == chosen ? 1 : 0)); // chosen packet first
        foreach (var stack in stacks)
        {
            var crop = CropCatalog.Default.BySeed(stack.Id);
            if (crop == null || !inv.Remove(stack.Id, 1, stack.Quality)) continue;
            t.CropId = crop.Id;
            t.DaysToHarvest = Mathf.Max(1, crop.DaysToHarvest);
            t.DaysGrown = 0;
            t.DryDays = 0;
            return TileAction.Planted;
        }
        return TileAction.Nothing; // no seeds in the bag
    }

    private TileAction Harvest(Vector2I cell, CropTile t)
    {
        var crop = CropCatalog.Default.Get(t.CropId);
        string item = crop?.HarvestItem ?? t.CropId;
        Inventory.Instance?.Add(item, crop?.HarvestCount ?? 1);
        OrderManager.Instance?.NoteProduced(item);
        t.ClearCrop();
        EmitSignal(SignalName.CropHarvested, cell, item);
        return TileAction.Harvested;
    }

    /// <summary>The night's growth and neglect (crops.md §2). Once per <see cref="GameClock.DayEnded"/>.</summary>
    public void EndDay()
    {
        foreach (var (cell, t) in AllTiles())
        {
            if (t.HasCrop && !t.IsReady)
            {
                if (t.Watered)
                {
                    t.DaysGrown++;
                    t.DryDays = 0;
                }
                else if (++t.DryDays >= CropTile.LostAfterDryDays)
                {
                    t.ClearCrop(); // gone; the tile stays tilled
                    EmitSignal(SignalName.CropLost, cell);
                }
            }
            t.Watered = false;
        }
        EmitSignal(SignalName.Changed);
    }

    /// <summary>Replaces every tile (save/load). Missing tiles become weeds.</summary>
    public void SetTiles(IList<CropTile> tiles)
    {
        _tiles = new CropTile[Columns * Rows];
        for (int i = 0; i < _tiles.Length; i++)
            _tiles[i] = i < tiles.Count && tiles[i] != null ? tiles[i] : new CropTile();
        EmitSignal(SignalName.Changed);
    }

    /// <summary>Which tool a tile needs next (null = nothing to do here today).</summary>
    public static string HintFor(CropTile t)
    {
        if (t.Soil != Soil.Tilled) return "tool_hoe";
        if (!t.HasCrop) return "tool_seed_bag";
        if (!t.Watered) return "tool_watering_can";
        return null;
    }

    public static string SoilSprite(CropTile t) => t.Soil switch
    {
        Soil.Weeds => "soil_weeds",
        Soil.Clear => "soil_clear",
        _ => t.Watered ? "soil_wet" : "soil_tilled",
    };

    public static string CropSprite(CropTile t)
    {
        if (!t.HasCrop) return null;
        string prefix = CropCatalog.Default.Get(t.CropId)?.SpritePrefix ?? "crop_" + t.CropId;
        if (t.IsYellow) return prefix + "_yellow";
        return prefix + t.Stage switch { 0 => "_seed", 1 => "_sprout", 2 => "_growing", _ => "_ready" };
    }
}
