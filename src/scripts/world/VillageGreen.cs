using System.Collections.Generic;
using Godot;
using MyFarmingGame.Core;

namespace MyFarmingGame.World;

/// <summary>
/// Bramblewick Green, first ring (Phase-002 Task 2.3; places.md §2.1): the village reached from the
/// top of the lane. 64 × 44 tiles (1 tile = 1 m, north = −Z). One ring path around the green passes
/// every door; the duck pond sits in the middle. This task opens the bakery (north-west), the
/// general store (north-east), the allotments (west) and an empty market stand (east); the other
/// buildings and the exits west, north and east come in later phases. The lane enters at the
/// south edge. Ground, paths, pond, hedges and the allotment beds are baked from the tables below.
/// </summary>
public partial class VillageGreen : Node3D
{
    public static readonly Rect2I Area = new(0, 0, 64, 44);

    /// <summary>The ring path (2 tiles wide) and the lane in from the south.</summary>
    public static readonly Rect2I[] Paths =
    {
        new(10, 8, 44, 2), new(10, 34, 44, 2), new(10, 8, 2, 28), new(52, 8, 2, 28),
        new(31, 36, 2, 8),
        new(16, 6, 2, 2), new(45, 6, 2, 2), new(9, 20, 1, 2), new(54, 19, 2, 2),
    };

    /// <summary>The duck pond (blocked).</summary>
    public static readonly Rect2I Pond = new(26, 18, 12, 8);

    /// <summary>The allotments: a fenced plot west of the ring, gate on the east side.</summary>
    public static readonly Rect2I Allotments = new(1, 14, 8, 14);

    /// <summary>Gap in the south hedge where the lane comes in.</summary>
    public static readonly Rect2I LaneGap = new(31, 43, 2, 1);

    private static readonly Color Grass = new("#9cc865");
    private static readonly Color Lawn = new("#a9d470");
    private static readonly Color Path = new("#dcc590");
    private static readonly Color Water = new("#5fa8d8");
    private static readonly Color Hedge = new("#3f6b2c");
    private static readonly Color Soil = new("#8a6038");
    private static readonly Color Fence = new("#7a5534");

    public override void _Ready()
    {
        var flat = new List<(Vector3, Vector3, Color)>();
        var solid = new List<(Vector3, Vector3, Color)>();
        void Flat(Rect2 r, float y, Color c) => flat.Add((new Vector3(r.GetCenter().X, y - 0.01f, r.GetCenter().Y), new Vector3(r.Size.X, 0.02f, r.Size.Y), c));
        void Box(Rect2 r, float h, Color c) => solid.Add((new Vector3(r.GetCenter().X, h / 2f, r.GetCenter().Y), new Vector3(r.Size.X, h, r.Size.Y), c));

        Flat(new Rect2(-20, -20, 104, 84), 0f, Grass);
        Flat(new Rect2(12, 10, 40, 24), 0.003f, Lawn);
        foreach (var p in Paths) Flat(R(p), 0.008f, Path);
        Flat(R(Pond), 0.012f, Water);
        Flat(R(Allotments), 0.005f, Soil);
        for (int i = 0; i < 4; i++) Box(new Rect2(2, 15 + i * 3.3f, 6, 1.6f), 0.18f, new Color("#5a8a3a")); // planted beds

        var blockers = new StaticBody3D { Name = "Blockers" };
        AddChild(blockers);
        void Block(Rect2 r) => blockers.AddChild(new CollisionShape3D
        {
            Shape = new BoxShape3D { Size = new Vector3(r.Size.X, FarmLevel.BlockHeight, r.Size.Y) },
            Position = new Vector3(r.GetCenter().X, FarmLevel.BlockHeight / 2f, r.GetCenter().Y),
        });

        // Hedges all round (exits west, north and east open in later phases), with the lane gap south.
        Rect2 a = R(Area);
        var hedges = new[]
        {
            new Rect2(a.Position.X - 1, a.Position.Y - 1, a.Size.X + 2, 1),
            new Rect2(a.Position.X - 1, a.End.Y, LaneGap.Position.X - a.Position.X + 1, 1),
            new Rect2(LaneGap.End.X, a.End.Y, a.End.X - LaneGap.End.X + 1, 1),
            new Rect2(a.Position.X - 1, a.Position.Y, 1, a.Size.Y),
            new Rect2(a.End.X, a.Position.Y, 1, a.Size.Y),
            // The lane between hedges.
            new Rect2(29, 36, 2, 8), new Rect2(33, 36, 2, 8),
        };
        foreach (var h in hedges) { Block(h); Box(h, 1.1f, Hedge); }
        Block(R(Pond));
        // Allotment fence with a gate on the east side (z 20..22).
        Rect2 al = R(Allotments);
        var fence = new[]
        {
            new Rect2(al.Position.X, al.Position.Y - 0.1f, al.Size.X, 0.15f), new Rect2(al.Position.X, al.End.Y, al.Size.X, 0.15f),
            new Rect2(al.Position.X - 0.1f, al.Position.Y, 0.15f, al.Size.Y),
            new Rect2(al.End.X, al.Position.Y, 0.15f, 20 - al.Position.Y), new Rect2(al.End.X, 22, 0.15f, al.End.Y - 22),
        };
        foreach (var f in fence) { Block(f); Box(f, 0.8f, Fence); }

        AddChild(new MeshInstance3D { Name = "Ground", Mesh = ModelLibrary.BakeBoxes(flat), MaterialOverride = ModelLibrary.VertexColorMaterial, CastShadow = GeometryInstance3D.ShadowCastingSetting.Off });
        AddChild(new MeshInstance3D { Name = "Hedges", Mesh = ModelLibrary.BakeBoxes(solid), MaterialOverride = ModelLibrary.VertexColorMaterial });
    }

    private static Rect2 R(Rect2I r) => new(r.Position.X, r.Position.Y, r.Size.X, r.Size.Y);
}
