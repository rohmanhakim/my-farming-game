using Godot;
using MyFarmingGame.Core;

namespace MyFarmingGame.World;

/// <summary>
/// Draws the farm's ground features from <see cref="FarmLevel"/>'s layout (Task 1.5): paths,
/// overgrown fields and the fence. Sprites by name (ground_path, ground_overgrown, gate), tiled at
/// 64 px. Sits below Kit (negative z_index, Task 1.1 draw-order gotcha).
/// </summary>
public partial class FarmGround : Node2D
{
    private static readonly Color FenceColor = new(0.45f, 0.31f, 0.19f);

    public override void _Ready()
    {
        TextureRepeat = TextureRepeatEnum.Enabled;
        QueueRedraw();
    }

    public override void _Draw()
    {
        // Textures are 32-px placeholders drawn at 64 px: draw at half scale ×2.
        DrawSetTransform(Vector2.Zero, 0, new Vector2(2, 2));
        var path = SpriteLibrary.Get("ground_path");
        var overgrown = SpriteLibrary.Get("ground_overgrown");
        foreach (var f in FarmLevel.OvergrownFields)
            DrawTextureRect(overgrown, Half(FarmLevel.ToPx(f)), true);
        foreach (var h in FarmLevel.Hedges)
            DrawTextureRect(overgrown, Half(FarmLevel.ToPx(h)), true);
        foreach (var p in FarmLevel.Paths)
            DrawTextureRect(path, Half(FarmLevel.ToPx(p)), true);
        foreach (var p in FarmLevel.LanePaths)
            DrawTextureRect(path, Half(FarmLevel.ToPx(p)), true);
        DrawSetTransform(Vector2.Zero, 0, Vector2.One);

        Rect2 farm = FarmLevel.ToPx(FarmLevel.Farm), gate = FarmLevel.ToPx(FarmLevel.Gate);
        float t = FarmLevel.FenceThickness;
        DrawRect(new Rect2(farm.Position.X, -t, gate.Position.X - farm.Position.X, t), FenceColor);
        DrawRect(new Rect2(gate.End.X, -t, farm.End.X - gate.End.X, t), FenceColor);
        DrawRect(new Rect2(farm.Position.X, farm.End.Y, farm.Size.X, t), FenceColor);
        DrawRect(new Rect2(farm.Position.X - t, -t, t, farm.Size.Y + 2 * t), FenceColor);
        DrawRect(new Rect2(farm.End.X, -t, t, farm.Size.Y + 2 * t), FenceColor);
    }

    private static Rect2 Half(Rect2 r) => new(r.Position / 2, r.Size / 2);
}
