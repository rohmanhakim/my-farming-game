using Godot;

namespace MyFarmingGame.World;

/// <summary>
/// The guided start's pointer (Task 1.12): a bouncing arrow above the current step's target in the
/// world (the front door, a weed, a bed, Marlow, a berry bush, a feed bin). Drawn above Kit and
/// roofs. <see cref="Point"/>(null) hides it.
/// </summary>
public partial class GuideArrow : Node2D
{
    private float _t;
    public Vector2? Target { get; private set; }

    public override void _Ready()
    {
        ZIndex = 50;
        Visible = false;
    }

    public void Point(Vector2? target)
    {
        Target = target;
        Visible = target.HasValue;
        if (target.HasValue) GlobalPosition = target.Value;
    }

    public override void _Process(double delta)
    {
        if (!Visible) return;
        _t += (float)delta;
        QueueRedraw();
    }

    public override void _Draw()
    {
        float y = -64 + Mathf.Sin(_t * 4f) * 8f;
        var pts = new[] { new Vector2(-16, y - 22), new Vector2(16, y - 22), new Vector2(0, y) };
        DrawColoredPolygon(pts, new Color(1f, 0.82f, 0.2f));
        DrawPolyline(new[] { pts[0], pts[1], pts[2], pts[0] }, new Color(0.35f, 0.22f, 0.05f), 3f);
        DrawRect(new Rect2(-6, y - 40, 12, 18), new Color(1f, 0.82f, 0.2f));
    }
}
