using Godot;
using MyFarmingGame.Core;

namespace MyFarmingGame.World;

/// <summary>
/// The guided start's pointer (Task 1.12; 3D since Task 1.17): a bouncing arrow above the current
/// step's target in the world (the front door, a weed, a bed, Marlow, a berry bush, a feed bin),
/// drawn as screen UI so it is never hidden by a roof. <see cref="Point"/>(null) hides it.
/// </summary>
public partial class GuideArrow : Node3D
{
    /// <summary>How high above the target the arrow tip floats (metres).</summary>
    public const float Height = 1.4f;

    public Vector3? Target { get; private set; }

    private WorldPin _pin;
    private ArrowShape _shape;
    private float _t;

    public override void _Ready()
    {
        _shape = new ArrowShape();
        _pin = new WorldPin { Name = "Pin", WorldOffset = new Vector3(0, Height, 0), Shown = false };
        _pin.AddChild(_shape);
        AddChild(_pin);
        Visible = false;
    }

    public void Point(Vector3? target)
    {
        Target = target;
        Visible = target.HasValue;
        if (_pin != null) _pin.Shown = target.HasValue;
        if (target.HasValue) GlobalPosition = new Vector3(target.Value.X, 0, target.Value.Z);
    }

    /// <summary>True while the arrow is on screen (tests).</summary>
    public bool Showing => _pin != null && _pin.Shown && _pin.Visible;

    public override void _Process(double delta)
    {
        if (!Visible) return;
        _t += (float)delta;
        _pin.ScreenOffset = new Vector2(0, Mathf.Sin(_t * 4f) * 8f);
    }

    private partial class ArrowShape : Control
    {
        public ArrowShape() { MouseFilter = MouseFilterEnum.Ignore; }

        public override void _Draw()
        {
            var pts = new[] { new Vector2(-16, -22), new Vector2(16, -22), new Vector2(0, 0) };
            DrawColoredPolygon(pts, new Color(1f, 0.82f, 0.2f));
            DrawPolyline(new[] { pts[0], pts[1], pts[2], pts[0] }, new Color(0.35f, 0.22f, 0.05f), 3f);
            DrawRect(new Rect2(-6, -40, 12, 18), new Color(1f, 0.82f, 0.2f));
        }
    }
}
