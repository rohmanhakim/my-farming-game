using Godot;

namespace MyFarmingGame.Core;

/// <summary>
/// A one-shot burst of little cubes (Task 1.13 playtest fix, owner 2026-11-21: "the actions are
/// unclear"): a dirt puff when digging or pulling weeds, water drops when watering, seeds when
/// planting, grain when filling a trough slot, a golden pop when harvesting or collecting. Frees
/// itself when done. CPU particles, a few dozen at most, so it costs one draw call while it lives.
/// </summary>
public partial class Puff : CpuParticles3D
{
    public static readonly Color Dirt = new("#8a6038");
    public static readonly Color Water = new("#5aa8f0");
    public static readonly Color Seed = new("#e8d49a");
    public static readonly Color Grain = new("#e3b23c");
    public static readonly Color Gold = new("#ffd966");
    public static readonly Color Leaf = new("#5fa83a");

    /// <summary>Spawns a burst at <paramref name="at"/> (world) under <paramref name="parent"/>.</summary>
    public static Puff At(Node parent, Vector3 at, Color color, int amount = 14, float up = 2.2f)
    {
        if (parent == null || !parent.IsInsideTree()) return null;
        var p = new Puff
        {
            Amount = amount, OneShot = true, Explosiveness = 0.95f, Lifetime = 0.55f,
            Mesh = ModelLibrary.MeshFor("box", new Vector3(0.07f, 0.07f, 0.07f)),
            MaterialOverride = ModelLibrary.MaterialFor(color),
            Direction = Vector3.Up, Spread = 55f, InitialVelocityMin = up * 0.6f, InitialVelocityMax = up,
            Gravity = new Vector3(0, -9f, 0), ScaleAmountMin = 0.6f, ScaleAmountMax = 1.2f,
            CastShadow = GeometryInstance3D.ShadowCastingSetting.Off,
        };
        parent.AddChild(p);
        p.GlobalPosition = at + new Vector3(0, 0.12f, 0);
        p.Emitting = true;
        p.Finished += p.QueueFree;
        return p;
    }
}
