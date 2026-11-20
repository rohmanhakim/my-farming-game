using System.Collections.Generic;
using Godot;
using MyFarmingGame.Autoloads;
using MyFarmingGame.Core;
using MyFarmingGame.Player;

namespace MyFarmingGame.World;

/// <summary>What a tool did to a tile (tests, nudges).</summary>
public enum TileAction { Nothing, Cleared, Tilled, Planted, Watered, Harvested }

/// <summary>
/// The front garden (Task 1.5, buildings.md §2): <see cref="Columns"/> × <see cref="Rows"/> tiles of
/// 64 px. Starts as weeds with <see cref="StartingBeds"/> already tilled ("3 beds already clear").
///
/// Tool rules: hands pull weeds; hoe tills cleared ground; seed bag plants the first seed packet in
/// the bag on an empty tilled tile; watering can waters tilled soil (and recovers a yellow crop);
/// any tool harvests a ready crop into the bag. Growth and neglect run on
/// <see cref="GameClock.DayEnded"/> (<see cref="EndDay"/>), so nothing changes while the game is
/// closed. Draws itself from SpriteLibrary names (soil_*, crop_&lt;id&gt;_*), plus a highlight on the
/// tile in front of Kit.
/// </summary>
public partial class FarmGrid : Node2D, IToolTarget
{
    public const int TileSize = 64;

    [Export] public int Columns { get; set; } = 8;
    [Export] public int Rows { get; set; } = 6;

    /// <summary>Tiles that start tilled (the "3 beds already clear", buildings.md §2).</summary>
    [Export] public Godot.Collections.Array<Vector2I> StartingBeds { get; set; } = new() { new(5, 3), new(6, 3), new(7, 3) };

    /// <summary>When false the grid ignores the clock (rule tests call <see cref="EndDay"/>).</summary>
    [Export] public bool ListenToClock { get; set; } = true;

    [Signal] public delegate void TileChangedEventHandler(Vector2I cell);
    [Signal] public delegate void CropLostEventHandler(Vector2I cell);
    [Signal] public delegate void CropHarvestedEventHandler(Vector2I cell, string item);

    private CropTile[] _tiles;
    private GameClock _clock;
    private Vector2I? _highlight;

    public CropTile Get(Vector2I cell) => InBounds(cell) ? Tiles[cell.Y * Columns + cell.X] : null;

    public bool InBounds(Vector2I cell) => cell.X >= 0 && cell.Y >= 0 && cell.X < Columns && cell.Y < Rows;

    private CropTile[] Tiles
    {
        get
        {
            if (_tiles == null) ResetForNewGame();
            return _tiles;
        }
    }

    /// <summary>All tiles with their cells, row by row.</summary>
    public IEnumerable<(Vector2I Cell, CropTile Tile)> AllTiles()
    {
        for (int y = 0; y < Rows; y++)
            for (int x = 0; x < Columns; x++)
                yield return (new Vector2I(x, y), Tiles[y * Columns + x]);
    }

    public override void _Ready()
    {
        AddToGroup(IToolTarget.Group);
        ZIndex = -5; // below Kit, above the ground (Task 1.1 draw-order gotcha)
        _ = Tiles;
        if (ListenToClock && GameClock.Instance != null)
        {
            _clock = GameClock.Instance;
            _clock.DayEnded += OnDayEnded;
            Production.Register(this, SupplyForOrders);
        }
    }

    /// <summary>Carrots the garden can supply for an order: one per bed that is clear of weeds
    /// (at least the 3 starting beds), Task 1.7.</summary>
    private IEnumerable<(string, int)> SupplyForOrders()
    {
        int beds = 0;
        foreach (var (_, t) in AllTiles())
            if (t.Soil != Soil.Weeds) beds++;
        foreach (var r in CropCatalog.Default.Crops)
            if (r is CropDefinition c && Inventory.Instance != null)
                yield return (c.HarvestItem, beds);
    }

    public override void _ExitTree()
    {
        if (_clock != null && IsInstanceValid(_clock))
            _clock.DayEnded -= OnDayEnded;
        Production.Unregister(this);
    }

    private void OnDayEnded(int day) => EndDay();

    /// <summary>New game: weeds everywhere except the starting beds (tilled, empty).</summary>
    public void ResetForNewGame()
    {
        _tiles = new CropTile[Columns * Rows];
        for (int i = 0; i < _tiles.Length; i++)
            _tiles[i] = new CropTile();
        foreach (var b in StartingBeds)
            if (InBounds(b))
                _tiles[b.Y * Columns + b.X].Soil = Soil.Tilled;
        QueueRedraw();
    }

    // ---- Coordinates ----------------------------------------------------------------------
    public Vector2I WorldToCell(Vector2 world)
    {
        Vector2 local = ToLocal(world);
        return new Vector2I(Mathf.FloorToInt(local.X / TileSize), Mathf.FloorToInt(local.Y / TileSize));
    }

    public Vector2 CellCenter(Vector2I cell) => ToGlobal(new Vector2((cell.X + 0.5f) * TileSize, (cell.Y + 0.5f) * TileSize));

    public bool ContainsPoint(Vector2 worldPoint) => InBounds(WorldToCell(worldPoint));

    public Vector2 TileCenterAt(Vector2 worldPoint) => CellCenter(WorldToCell(worldPoint));

    public bool UseToolAt(Tool tool, Vector2 worldPoint) => UseTool(tool, WorldToCell(worldPoint)) != TileAction.Nothing;

    // ---- Rules ----------------------------------------------------------------------------
    /// <summary>Uses <paramref name="tool"/> on <paramref name="cell"/> and returns what happened.</summary>
    public TileAction UseTool(Tool tool, Vector2I cell)
    {
        var t = Get(cell);
        if (t == null)
            return TileAction.Nothing;
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
            QueueRedraw();
        }
        return done;
    }

    private static TileAction Plant(CropTile t)
    {
        var inv = Inventory.Instance;
        if (inv == null)
            return TileAction.Nothing;
        var stacks = new List<ItemStack>(inv.Stacks);
        string chosen = ToolBelt.Instance?.SelectedSeed ?? "";
        stacks.Sort((a, b) => (b.Id == chosen ? 1 : 0) - (a.Id == chosen ? 1 : 0)); // chosen packet first
        foreach (var stack in stacks)
        {
            var crop = CropCatalog.Default.BySeed(stack.Id);
            if (crop == null || !inv.Remove(stack.Id, 1, stack.Quality))
                continue;
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
        t.ClearCrop();
        EmitSignal(SignalName.CropHarvested, cell, item);
        return TileAction.Harvested;
    }

    /// <summary>The night's growth and neglect (crops.md §2). Called once per <see cref="GameClock.DayEnded"/>.</summary>
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
        QueueRedraw();
    }

    /// <summary>Replaces every tile (save/load). Missing tiles become weeds.</summary>
    public void SetTiles(IList<CropTile> tiles)
    {
        _tiles = new CropTile[Columns * Rows];
        for (int i = 0; i < _tiles.Length; i++)
            _tiles[i] = i < tiles.Count && tiles[i] != null ? tiles[i] : new CropTile();
        QueueRedraw();
    }

    // ---- Drawing --------------------------------------------------------------------------
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

    public override void _Process(double delta)
    {
        Vector2I? next = null;
        var player = GetTree().GetFirstNodeInGroup(PlayerController.Group) as PlayerController;
        if (player != null && (GameClock.Instance?.IsPlayable ?? true) && !player.Frozen)
        {
            var cell = WorldToCell(player.FrontPoint);
            if (InBounds(cell)) next = cell;
        }
        if (next != _highlight)
        {
            _highlight = next;
            QueueRedraw();
        }
    }

    /// <summary>The tile highlighted in front of Kit (tests).</summary>
    public Vector2I? HighlightedCell => _highlight;

    public override void _Draw()
    {
        foreach (var (cell, t) in AllTiles())
        {
            var rect = new Rect2(cell.X * TileSize, cell.Y * TileSize, TileSize, TileSize);
            DrawTextureRect(SpriteLibrary.Get(SoilSprite(t)), rect, false);
            string crop = CropSprite(t);
            if (crop != null)
                DrawTextureRect(SpriteLibrary.Get(crop), rect, false);
        }
        DrawRect(new Rect2(0, 0, Columns * TileSize, Rows * TileSize), new Color(0.35f, 0.25f, 0.15f), false, 3f);
        if (_highlight is Vector2I h)
            DrawRect(new Rect2(h.X * TileSize + 2, h.Y * TileSize + 2, TileSize - 4, TileSize - 4), new Color(1, 1, 1, 0.9f), false, 3f);
    }
}
