using Godot;
using MyFarmingGame.Core;

namespace MyFarmingGame.World;

/// <summary>
/// Kit's farm and the lane to Marlow's seed shed for the Phase-001 slice (Task 1.5; places.md §3, Phase-001 §1). Tile = 64 px; the farm
/// is 50 × 32 tiles with its top-left corner at the level origin and the gate in the middle of the
/// top fence. Only the centre column is walkable in Phase-001: the house with the front garden and
/// old coop facing the gate, the cross path, and the old barn behind the house with cleared ground
/// around it. Fields A–E are drawn overgrown and blocked.
///
/// The scene holds the buildings, bed, front garden and markers; this script draws the ground
/// (paths, overgrown fields, fence) and builds the blocking bodies from the layout below.
/// </summary>
public partial class FarmLevel : Node2D
{
    public const int Tile = 64;

    /// <summary>The whole farm in tiles.</summary>
    public static readonly Rect2I Farm = new(0, 0, 50, 32);

    /// <summary>Overgrown fields (blocked in Phase-001), in tiles. Field A is split by the path to the barn.</summary>
    public static readonly Rect2I[] OvergrownFields =
    {
        new(0, 0, 16, 12),   // Field D
        new(34, 0, 16, 12),  // Field C
        new(0, 14, 20, 16),  // Field E (by the brook)
        new(20, 14, 4, 10),  // Field A, west of the path
        new(26, 14, 6, 10),  // Field A, east of the path
        new(32, 14, 12, 10), // Field B
    };

    /// <summary>The 2-tile cross path and its arms, in tiles.</summary>
    public static readonly Rect2I[] Paths =
    {
        new(24, 0, 2, 8),    // gate → front door
        new(0, 12, 50, 2),   // cross path
        new(24, 14, 2, 11),  // → old barn
    };

    /// <summary>The lane north of the gate to Marlow's seed shed (Task 1.6, places.md §2.1), in tiles.</summary>
    public static readonly Rect2I[] LanePaths =
    {
        new(24, -19, 2, 19),
    };

    /// <summary>Hedges either side of the lane (drawn overgrown, blocked), in tiles.</summary>
    public static readonly Rect2I[] Hedges =
    {
        new(14, -19, 8, 19),
        new(28, -19, 8, 19),
    };

    /// <summary>Edges of the clearing around the seed shed and cottage (blocked, not drawn).</summary>
    public static readonly Rect2I[] ClearingEdges =
    {
        new(15, -28, 21, 1),
        new(15, -28, 1, 9),
        new(35, -28, 1, 9),
    };

    /// <summary>The gate in the top fence, in tiles.</summary>
    public static readonly Rect2I Gate = new(24, 0, 2, 1);

    /// <summary>True to shut the gate (the lane is open from Task 1.6).</summary>
    [Export] public bool GateClosed { get; set; }

    public const float FenceThickness = 10f;

    public static Rect2 ToPx(Rect2I r) => new(r.Position.X * Tile, r.Position.Y * Tile, r.Size.X * Tile, r.Size.Y * Tile);

    public override void _Ready()
    {
        var blockers = new StaticBody2D { Name = "Blockers" };
        AddChild(blockers);
        foreach (var f in OvergrownFields)
            AddBlock(blockers, ToPx(f));
        foreach (var h in Hedges)
            AddBlock(blockers, ToPx(h));
        foreach (var e in ClearingEdges)
            AddBlock(blockers, ToPx(e));

        // Fence around the farm with a gap for the gate.
        Rect2 farm = ToPx(Farm), gate = ToPx(Gate);
        float t = FenceThickness;
        AddBlock(blockers, new Rect2(farm.Position.X, farm.Position.Y - t, gate.Position.X - farm.Position.X, t));
        AddBlock(blockers, new Rect2(gate.End.X, farm.Position.Y - t, farm.End.X - gate.End.X, t));
        AddBlock(blockers, new Rect2(farm.Position.X, farm.End.Y, farm.Size.X, t));
        AddBlock(blockers, new Rect2(farm.Position.X - t, farm.Position.Y - t, t, farm.Size.Y + 2 * t));
        AddBlock(blockers, new Rect2(farm.End.X, farm.Position.Y - t, t, farm.Size.Y + 2 * t));
        if (GateClosed)
            AddBlock(blockers, new Rect2(gate.Position.X, farm.Position.Y - t, gate.Size.X, t));
    }

    private static void AddBlock(StaticBody2D body, Rect2 r) =>
        body.AddChild(new CollisionShape2D { Shape = new RectangleShape2D { Size = r.Size }, Position = r.GetCenter() });
}
