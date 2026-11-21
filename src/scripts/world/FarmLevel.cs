using Godot;
using MyFarmingGame.Core;

namespace MyFarmingGame.World;

/// <summary>
/// Kit's farm and the lane to Marlow's seed shed for the Phase-001 slice (Task 1.5; 3D since Task 1.15;
/// places.md §3, Phase-001 §1). 1 tile = 1 m; the farm is 50 × 32 tiles with its north-west corner
/// at the level origin (tile (x, y) covers x..x+1 on X and y..y+1 on Z; north = −Z) and the gate in
/// the middle of the north fence. Only the centre column is walkable in Phase-001: the house with
/// the front garden and old coop facing the gate, the cross path, and the old barn behind the house
/// with cleared ground around it. Fields A–E are drawn overgrown and blocked.
///
/// The scene holds the buildings, bed, front garden, props and markers; this script builds the
/// ground (grass, paths, overgrown fields and hedges as one MultiMesh of tufts, fence) and the
/// blocking bodies from the layout tables below (the same tables the 2D build used).
/// </summary>
public partial class FarmLevel : Node3D
{
    /// <summary>Pixels per tile in the 2D build (kept for the save migration and old notes).</summary>
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

    /// <summary>Fence thickness in metres (2D: 10 px).</summary>
    public const float FenceThickness = 0.15f;
    /// <summary>Height of invisible blockers (taller than Kit).</summary>
    public const float BlockHeight = 2.5f;

    /// <summary>Where a tile rect lies on the ground: (x, z, width, depth) in metres.</summary>
    public static Rect2 ToGround(Rect2I r) => new(r.Position.X, r.Position.Y, r.Size.X, r.Size.Y);

    private static readonly Color GrassColor = new("#8fbf5a");
    private static readonly Color PathColor = new("#d8c08a");
    private static readonly Color FieldColor = new("#6f8f3e");
    private static readonly Color HedgeColor = new("#3f6b2c");

    public override void _Ready()
    {
        BuildGround();
        var blockers = new StaticBody3D { Name = "Blockers" };
        AddChild(blockers);
        foreach (var f in OvergrownFields)
            AddBlock(blockers, ToGround(f));
        foreach (var h in Hedges)
            AddBlock(blockers, ToGround(h));
        foreach (var e in ClearingEdges)
            AddBlock(blockers, ToGround(e));

        // Fence around the farm with a gap for the gate.
        Rect2 farm = ToGround(Farm), gate = ToGround(Gate);
        float t = FenceThickness;
        var fence = new[]
        {
            new Rect2(farm.Position.X, farm.Position.Y - t, gate.Position.X - farm.Position.X, t),
            new Rect2(gate.End.X, farm.Position.Y - t, farm.End.X - gate.End.X, t),
            new Rect2(farm.Position.X, farm.End.Y, farm.Size.X, t),
            new Rect2(farm.Position.X - t, farm.Position.Y - t, t, farm.Size.Y + 2 * t),
            new Rect2(farm.End.X, farm.Position.Y - t, t, farm.Size.Y + 2 * t),
        };
        foreach (var r in fence)
        {
            AddBlock(blockers, r);
            AddBox("Fence", r, 0.08f, 0.8f, new Color("#7a5534")); // the top rail
        }
        if (GateClosed)
            AddBlock(blockers, new Rect2(gate.Position.X, farm.Position.Y - t, gate.Size.X, t));
    }

    /// <summary>Grass everywhere, paths on top, overgrown fields and hedges as tufts (one MultiMesh).</summary>
    private void BuildGround()
    {
        AddPlane("Grass", new Rect2(-20, -40, 90, 82), 0f, GrassColor);
        foreach (var f in OvergrownFields)
            AddPlane("Field", ToGround(f), 0.004f, FieldColor);
        foreach (var p in Paths)
            AddPlane("Path", ToGround(p), 0.008f, PathColor);
        foreach (var p in LanePaths)
            AddPlane("Path", ToGround(p), 0.008f, PathColor);
        foreach (var h in Hedges)
            AddBox("Hedge", ToGround(h), 0f, 1.1f, HedgeColor);

        // One tuft per overgrown tile, slightly jittered so it reads as wild, not as a grid.
        var cells = new System.Collections.Generic.List<Vector3>();
        var rng = new RandomNumberGenerator { Seed = 1234 };
        foreach (var f in OvergrownFields)
            for (int x = f.Position.X; x < f.End.X; x++)
                for (int y = f.Position.Y; y < f.End.Y; y++)
                    cells.Add(new Vector3(x + 0.5f + rng.RandfRange(-0.25f, 0.25f), 0, y + 0.5f + rng.RandfRange(-0.25f, 0.25f)));
        Tufts = MakeMultiMesh("Tufts", "grass_tuft", cells, rng);
    }

    /// <summary>The overgrown-field tufts (tests, budget).</summary>
    public MultiMeshInstance3D Tufts { get; private set; }

    private MultiMeshInstance3D MakeMultiMesh(string name, string model, System.Collections.Generic.List<Vector3> at, RandomNumberGenerator rng)
    {
        var parts = ModelLibrary.PlaceholderMeshes(model);
        var mesh = parts?[0].Mesh ?? new BoxMesh();
        var mm = new MultiMesh { TransformFormat = MultiMesh.TransformFormatEnum.Transform3D, Mesh = mesh, InstanceCount = at.Count };
        var local = parts?[0].Local ?? Transform3D.Identity;
        for (int i = 0; i < at.Count; i++)
        {
            float s = rng.RandfRange(0.8f, 1.25f);
            var b = Basis.FromEuler(new Vector3(0, rng.RandfRange(0, Mathf.Tau), 0)).Scaled(new Vector3(s, s, s));
            mm.SetInstanceTransform(i, new Transform3D(b, at[i]) * local);
        }
        // Tufts cast no shadow: hundreds of tiny shadows cost a pass and read as noise (budget §16).
        var node = new MultiMeshInstance3D { Name = name, Multimesh = mm, MaterialOverride = parts?[0].Material, CastShadow = GeometryInstance3D.ShadowCastingSetting.Off };
        AddChild(node);
        return node;
    }

    private void AddPlane(string name, Rect2 r, float y, Color c) => AddChild(new MeshInstance3D
    {
        Name = name,
        Mesh = new PlaneMesh { Size = r.Size },
        MaterialOverride = ModelLibrary.MaterialFor(c),
        Position = new Vector3(r.GetCenter().X, y, r.GetCenter().Y),
        CastShadow = GeometryInstance3D.ShadowCastingSetting.Off,
    });

    private void AddBox(string name, Rect2 r, float y, float h, Color c) => AddChild(new MeshInstance3D
    {
        CastShadow = name == "Fence" ? GeometryInstance3D.ShadowCastingSetting.Off : GeometryInstance3D.ShadowCastingSetting.On,
        Name = name,
        Mesh = new BoxMesh { Size = new Vector3(r.Size.X, h, r.Size.Y) },
        MaterialOverride = ModelLibrary.MaterialFor(c),
        Position = new Vector3(r.GetCenter().X, y + h / 2f, r.GetCenter().Y),
    });

    private static void AddBlock(StaticBody3D body, Rect2 r) =>
        body.AddChild(new CollisionShape3D
        {
            Shape = new BoxShape3D { Size = new Vector3(r.Size.X, BlockHeight, r.Size.Y) },
            Position = new Vector3(r.GetCenter().X, BlockHeight / 2f, r.GetCenter().Y),
        });
}
