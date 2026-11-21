using Godot;

namespace MyFarmingGame.Core;

/// <summary>
/// A small piece of 2D UI pinned to a point in the 3D world (Task 1.15–1.17): button prompts,
/// heart bars, need icons, the feed-bin count, the guide arrow, garden hints. Each frame it moves to
/// where the camera shows <see cref="Anchor"/> + <see cref="WorldOffset"/>, plus
/// <see cref="ScreenOffset"/> pixels, and hides when that point is behind the camera. Being screen
/// UI, it keeps one readable size at any camera angle and is never hidden behind a roof.
///
/// Add it as a child of the 3D node it belongs to (a Control under a Node3D draws on the world's
/// canvas, below the HUD layers). Its own children are the UI to show, laid out around (0, 0).
/// </summary>
public partial class WorldPin : Control
{
    /// <summary>The 3D node to follow (defaults to the parent).</summary>
    public Node3D Anchor { get; set; }

    /// <summary>Offset from the anchor in metres (usually up: (0, height, 0)).</summary>
    public Vector3 WorldOffset { get; set; }

    /// <summary>Extra offset on screen in pixels after projecting.</summary>
    public Vector2 ScreenOffset { get; set; }

    /// <summary>Set false to hide the pin without touching its children.</summary>
    public bool Shown { get; set; } = true;

    /// <summary>Where the pin is on screen this frame (tests).</summary>
    public Vector2 ScreenPosition { get; private set; }

    public override void _Ready()
    {
        MouseFilter = MouseFilterEnum.Ignore;
        Anchor ??= GetParent() as Node3D;
        TopLevel = true;
        Follow();
    }

    public override void _Process(double delta) => Follow();

    /// <summary>Moves to the anchor's screen position now (also called by tests).</summary>
    public void Follow()
    {
        var cam = GetViewport()?.GetCamera3D();
        if (!Shown || Anchor == null || !IsInstanceValid(Anchor) || !Anchor.IsVisibleInTree() || cam == null)
        {
            Visible = false;
            return;
        }
        Vector3 p = Anchor.GlobalPosition + WorldOffset;
        if (cam.IsPositionBehind(p))
        {
            Visible = false;
            return;
        }
        ScreenPosition = cam.UnprojectPosition(p) + ScreenOffset;
        Position = ScreenPosition;
        Visible = true;
    }
}
