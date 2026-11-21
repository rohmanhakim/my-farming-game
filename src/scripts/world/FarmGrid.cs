using System.Collections.Generic;
using Godot;
using MyFarmingGame.Autoloads;
using MyFarmingGame.Core;
using MyFarmingGame.Player;

namespace MyFarmingGame.World;

/// <summary>What a tool did to a tile (tests, nudges).</summary>
public enum TileAction { Nothing, Cleared, Tilled, Planted, Watered, Harvested }

/// <summary>
/// The front garden (Task 1.5; 3D since Task 1.16; buildings.md §2): <see cref="Columns"/> ×
/// <see cref="Rows"/> tiles of 1 m, the node sitting at its north-west corner. Starts as weeds with <see cref="StartingBeds"/> already tilled ("3 beds already clear").
///
/// Tool rules: hands pull weeds; hoe tills cleared ground; seed bag plants the first seed packet in
/// the bag on an empty tilled tile; watering can waters tilled soil (and recovers a yellow crop);
/// any tool harvests a ready crop into the bag. Growth and neglect run on
/// <see cref="GameClock.DayEnded"/> (<see cref="EndDay"/>), so nothing changes while the game is
/// closed. Draws itself from model names (soil_*, crop_&lt;id&gt;_*) with one MultiMesh per model part
/// (performance budget rule 1), plus a white frame on the tile in front of Kit; hints, sparkles and
/// can icons are screen UI pinned above tiles (<see cref="WorldPin"/>).
/// </summary>
public partial class FarmGrid : Node3D, IToolTarget
{
    /// <summary>Metres per tile.</summary>
    public const float TileSize = 1f;

    [Export] public int Columns { get; set; } = 8;
    [Export] public int Rows { get; set; } = 6;

    /// <summary>Tiles that start tilled (the "3 beds already clear", buildings.md §2).</summary>
    [Export] public Godot.Collections.Array<Vector2I> StartingBeds { get; set; } = new() { new(5, 3), new(6, 3), new(7, 3) };

    /// <summary>When false the grid ignores the clock (rule tests call <see cref="EndDay"/>).</summary>
    [Export] public bool ListenToClock { get; set; } = true;

    [Signal] public delegate void TileChangedEventHandler(Vector2I cell);
    [Signal] public delegate void CropLostEventHandler(Vector2I cell);
    [Signal] public delegate void CropHarvestedEventHandler(Vector2I cell, string item);
    /// <summary>A tool did something (Task 1.12 onboarding counts it). Arguments: <see cref="TileAction"/>, cell.</summary>
    [Signal] public delegate void ToolUsedEventHandler(int action, Vector2I cell);

    private bool _waterHints;

    /// <summary>Task 1.12 nudge "morning after planting": a watering-can icon over every dry bed.</summary>
    public bool WaterHints
    {
        get => _waterHints;
        set { _waterHints = value; QueueRedraw(); }
    }

    private float _sparkle;

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
        BuildDecor();
        _ = Tiles;
        QueueRedraw();
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
    /// <summary>The cell under a world point (its height is ignored).</summary>
    public Vector2I WorldToCell(Vector3 world)
    {
        Vector3 local = new Vector3(world.X, GlobalPosition.Y, world.Z) - GlobalPosition;
        return new Vector2I(Mathf.FloorToInt(local.X / TileSize), Mathf.FloorToInt(local.Z / TileSize));
    }

    /// <summary>The centre of a cell on the ground.</summary>
    public Vector3 CellCenter(Vector2I cell) => GlobalPosition + new Vector3((cell.X + 0.5f) * TileSize, 0, (cell.Y + 0.5f) * TileSize);

    public bool ContainsPoint(Vector3 worldPoint) => InBounds(WorldToCell(worldPoint));

    public Vector3 TileCenterAt(Vector3 worldPoint) => CellCenter(WorldToCell(worldPoint));

    public bool UseToolAt(Tool tool, Vector3 worldPoint) => UseTool(tool, WorldToCell(worldPoint)) != TileAction.Nothing;

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
                case Tool.Hoe when t.Soil == Soil.Weeds:
                    // Owner playtest 2026-11-21: the hoe digs weedy ground too — it turns the weeds
                    // under and tills in one stroke (counts as a weed pulled for the guided start).
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
            ShowActionPuff(done, cell);
            EmitSignal(SignalName.TileChanged, cell);
            EmitSignal(SignalName.ToolUsed, (int)done, cell);
            QueueRedraw();
        }
        else
            ShowHint(cell, HintFor(t)); // a soft puff and the tool this tile needs (hint ladder step 1)
        return done;
    }

    /// <summary>What a tool did, made visible (Task 1.13 playtest fix): dirt, water, seeds or a golden pop.</summary>
    private void ShowActionPuff(TileAction done, Vector2I cell)
    {
        if (!IsInsideTree()) return;
        Color c = done switch
        {
            TileAction.Watered => Puff.Water,
            TileAction.Planted => Puff.Seed,
            TileAction.Harvested => Puff.Gold,
            TileAction.Cleared => Puff.Leaf,
            _ => Puff.Dirt,
        };
        Puff.At(this, CellCenter(cell), c, done == TileAction.Harvested ? 20 : 14);
        LastPuff = done;
    }

    /// <summary>The last action that showed a puff (tests).</summary>
    public TileAction LastPuff { get; private set; } = TileAction.Nothing;

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
        OrderManager.Instance?.NoteProduced(item);
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

    // ---- Wrong-tool feedback -----------------------------------------------------------------
    /// <summary>Seconds the "try this tool" puff stays on a tile.</summary>
    public const float HintSeconds = 0.9f;
    private Vector2I _hintCell;
    private string _hintSprite;
    private float _hintLeft;

    /// <summary>The tool picture shown last after a tool did nothing ("" = none; tests).</summary>
    public string LastHint { get; private set; } = "";

    /// <summary>Which tool a tile needs next (null = nothing to do here today).</summary>
    public static string HintFor(CropTile t)
    {
        if (t.Soil != Soil.Tilled) return "tool_hoe";
        if (!t.HasCrop) return "tool_seed_bag";
        if (!t.Watered) return "tool_watering_can";
        return null;
    }

    private void ShowHint(Vector2I cell, string sprite)
    {
        LastHint = sprite ?? "";
        if (sprite == null) return;
        _hintCell = cell;
        _hintSprite = sprite;
        _hintLeft = HintSeconds;
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
        _highlight = next;
        if (_frame != null)
        {
            _frame.Visible = next.HasValue;
            if (next is Vector2I h) _frame.Position = new Vector3((h.X + 0.5f) * TileSize, 0, (h.Y + 0.5f) * TileSize);
        }
        _sparkle += (float)delta;
        if (_hintLeft > 0) _hintLeft -= (float)delta;
        if (_dirty) Redraw();
        UpdatePins();
    }

    public bool AnyReady
    {
        get
        {
            foreach (var (_, t) in AllTiles()) if (t.IsReady) return true;
            return false;
        }
    }

    /// <summary>The tile highlighted in front of Kit (tests).</summary>
    public Vector2I? HighlightedCell => _highlight;

    // ---- 3D drawing -------------------------------------------------------------------------
    private bool _dirty = true;
    private Node3D _frame;
    private Node3D _tilesRoot;
    private readonly Dictionary<string, List<MultiMeshInstance3D>> _layers = new();
    private WorldPin _hintPin;
    private TextureRect _hintIcon;
    private readonly List<WorldPin> _cellPins = new();

    /// <summary>Marks the tiles for redrawing on the next frame (the 2D API name, kept).</summary>
    public void QueueRedraw() => _dirty = true;

    /// <summary>How many tiles each model name is drawn on right now (tests, budget).</summary>
    public IReadOnlyDictionary<string, int> DrawnCounts => _drawnCounts;
    private readonly Dictionary<string, int> _drawnCounts = new();

    private void BuildDecor()
    {
        _tilesRoot = new Node3D { Name = "Tiles" };
        AddChild(_tilesRoot);
        // White frame on the tile in front of Kit (helps gamepad players see what A will hit).
        _frame = new Node3D { Name = "Highlight", Visible = false };
        var white = new StandardMaterial3D { AlbedoColor = Colors.White, ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded };
        foreach (var (pos, size) in new[]
        {
            (new Vector3(0, 0.08f, -0.45f), new Vector3(0.96f, 0.03f, 0.06f)), (new Vector3(0, 0.08f, 0.45f), new Vector3(0.96f, 0.03f, 0.06f)),
            (new Vector3(-0.45f, 0.08f, 0), new Vector3(0.06f, 0.03f, 0.96f)), (new Vector3(0.45f, 0.08f, 0), new Vector3(0.06f, 0.03f, 0.96f)),
        })
            _frame.AddChild(new MeshInstance3D { Mesh = new BoxMesh { Size = size }, MaterialOverride = white, Position = pos, CastShadow = GeometryInstance3D.ShadowCastingSetting.Off });
        AddChild(_frame);
        // A low wooden border round the garden.
        var wood = ModelLibrary.MaterialFor(new Color(0.42f, 0.30f, 0.18f));
        float w = Columns * TileSize, d = Rows * TileSize;
        foreach (var (pos, size) in new[]
        {
            (new Vector3(w / 2, 0.06f, -0.03f), new Vector3(w + 0.1f, 0.12f, 0.06f)), (new Vector3(w / 2, 0.06f, d + 0.03f), new Vector3(w + 0.1f, 0.12f, 0.06f)),
            (new Vector3(-0.03f, 0.06f, d / 2), new Vector3(0.06f, 0.12f, d)), (new Vector3(w + 0.03f, 0.06f, d / 2), new Vector3(0.06f, 0.12f, d)),
        })
            AddChild(new MeshInstance3D { Name = "Border", Mesh = new BoxMesh { Size = size }, MaterialOverride = wood, Position = pos, CastShadow = GeometryInstance3D.ShadowCastingSetting.Off });
        _hintIcon = new TextureRect
        {
            Size = new Vector2(36, 36), Position = new Vector2(-18, -60), ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
            StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered, MouseFilter = Control.MouseFilterEnum.Ignore,
        };
        _hintPin = new WorldPin { Name = "Hint", Shown = false };
        _hintPin.AddChild(new HintPuff());
        _hintPin.AddChild(_hintIcon);
        AddChild(_hintPin);
    }

    /// <summary>Rebuilds the soil and crop MultiMeshes from the tiles.</summary>
    private void Redraw()
    {
        _dirty = false;
        if (_tilesRoot == null) return;
        var byModel = new Dictionary<string, List<Vector3>>();
        void add(string model, Vector2I cell)
        {
            if (model == null) return;
            if (!byModel.TryGetValue(model, out var l)) byModel[model] = l = new List<Vector3>();
            l.Add(new Vector3((cell.X + 0.5f) * TileSize, 0, (cell.Y + 0.5f) * TileSize));
        }
        foreach (var (cell, t) in AllTiles())
        {
            add(SoilSprite(t), cell);
            add(CropSprite(t), cell);
        }
        _drawnCounts.Clear();
        foreach (var (model, list) in _layers)
            foreach (var mmi in list) mmi.Multimesh.InstanceCount = 0;
        foreach (var (model, cells) in byModel)
        {
            _drawnCounts[model] = cells.Count;
            if (!_layers.TryGetValue(model, out var list))
            {
                list = new List<MultiMeshInstance3D>();
                var parts = ModelLibrary.PlaceholderMeshes(model) ?? new List<(Mesh, Material, Transform3D)>
                    { (ModelLibrary.MeshFor("box", new Vector3(0.9f, 0.1f, 0.9f)), ModelLibrary.MaterialFor(ModelLibrary.FallbackColor), new Transform3D(Basis.Identity, new Vector3(0, 0.05f, 0))) };
                foreach (var (mesh, mat, local) in parts)
                {
                    var mmi = new MultiMeshInstance3D
                    {
                        Name = model, MaterialOverride = mat, CastShadow = GeometryInstance3D.ShadowCastingSetting.Off,
                        Multimesh = new MultiMesh { TransformFormat = MultiMesh.TransformFormatEnum.Transform3D, Mesh = mesh },
                    };
                    mmi.SetMeta("local", local);
                    _tilesRoot.AddChild(mmi);
                    list.Add(mmi);
                }
                _layers[model] = list;
            }
            foreach (var mmi in list)
            {
                var local = mmi.GetMeta("local").AsTransform3D();
                mmi.Multimesh.InstanceCount = cells.Count;
                for (int i = 0; i < cells.Count; i++)
                    mmi.Multimesh.SetInstanceTransform(i, new Transform3D(Basis.Identity, cells[i]) * local);
            }
        }
    }

    /// <summary>Hint puff, ready sparkles and can icons over dry beds (screen UI above tiles).</summary>
    private void UpdatePins()
    {
        if (_hintPin != null)
        {
            bool on = _hintLeft > 0 && _hintSprite != null;
            _hintPin.Shown = on;
            if (on)
            {
                _hintPin.WorldOffset = new Vector3((_hintCell.X + 0.5f) * TileSize, 0.3f, (_hintCell.Y + 0.5f) * TileSize);
                _hintIcon.Texture = SpriteLibrary.Get(_hintSprite);
                _hintPin.Modulate = new Color(1, 1, 1, Mathf.Clamp(_hintLeft / HintSeconds * 1.5f, 0, 1));
            }
        }
        int used = 0;
        foreach (var (cell, t) in AllTiles())
        {
            bool sparkle = t.IsReady;
            bool can = !sparkle && _waterHints && t.HasCrop && !t.Watered;
            if (!sparkle && !can) continue;
            if (used == _cellPins.Count)
            {
                var pin = new WorldPin { Name = "TilePin" };
                pin.AddChild(new TextureRect
                {
                    Size = new Vector2(28, 28), Position = new Vector2(-14, -40), ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
                    StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered, MouseFilter = Control.MouseFilterEnum.Ignore,
                });
                pin.AddChild(new Sparkle());
                AddChild(pin);
                _cellPins.Add(pin);
            }
            var p = _cellPins[used++];
            p.Shown = true;
            p.WorldOffset = new Vector3((cell.X + 0.5f) * TileSize, 0.55f, (cell.Y + 0.5f) * TileSize);
            p.ScreenOffset = new Vector2(0, can ? Mathf.Sin(_sparkle * 4) * 3 : 0);
            var icon = p.GetChild<TextureRect>(0);
            icon.Visible = can;
            if (can) icon.Texture = SpriteLibrary.Get("tool_watering_can");
            var sp = p.GetChild<Sparkle>(1);
            sp.Visible = sparkle;
            sp.Phase = _sparkle * 5 + cell.X;
            sp.QueueRedraw();
        }
        for (int i = used; i < _cellPins.Count; i++) _cellPins[i].Shown = false;
        WaterHintCount = 0;
        foreach (var p in _cellPins) if (p.Shown && p.GetChild<TextureRect>(0).Visible) WaterHintCount++;
    }

    /// <summary>How many watering-can icons show right now (tests).</summary>
    public int WaterHintCount { get; private set; }

    /// <summary>True while the wrong-tool puff shows (tests).</summary>
    public bool HintShowing => _hintPin != null && _hintPin.Shown;

    private partial class HintPuff : Control
    {
        public HintPuff() { MouseFilter = MouseFilterEnum.Ignore; }
        public override void _Draw()
        {
            DrawCircle(new Vector2(-12, 6), 10, new Color(1, 1, 1, 0.5f));
            DrawCircle(new Vector2(10, 2), 8, new Color(1, 1, 1, 0.4f));
        }
    }

    private partial class Sparkle : Control
    {
        public float Phase { get; set; }
        public Sparkle() { MouseFilter = MouseFilterEnum.Ignore; }
        public override void _Draw()
        {
            float a = 0.5f + 0.5f * Mathf.Sin(Phase);
            DrawCircle(new Vector2(18, -20), 4f, new Color(1, 1, 0.7f, a));
            DrawCircle(new Vector2(-16, -10), 3f, new Color(1, 1, 0.7f, 1 - a));
        }
    }
}
