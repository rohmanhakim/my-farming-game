using Godot;

namespace MyFarmingGame;

/// <summary>
/// Keeps the shared Camera2D on a target (the player) with a smooth lerp (Task 1.2).
/// The camera node jumps to the target each physics frame; Camera2D's built-in position
/// smoothing lerps what is actually shown. Main owns the camera (project-structure.md §4).
/// Co-op split screen (controls.md §3) replaces this in a later phase.
/// </summary>
public partial class CameraFollow : Camera2D
{
    [Export] public NodePath TargetPath { get; set; }

    public Node2D Target { get; set; }

    public override void _Ready()
    {
        ProcessCallback = Camera2DProcessCallback.Physics; // same tick as the player: no jitter
        PositionSmoothingEnabled = true;
        if (Target == null && TargetPath != null && !TargetPath.IsEmpty)
            Target = GetNodeOrNull<Node2D>(TargetPath);
        SnapToTarget();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Target != null)
            GlobalPosition = Target.GlobalPosition;
    }

    /// <summary>Jumps straight to the target with no lerp (spawn, level change).</summary>
    public void SnapToTarget()
    {
        if (Target == null)
            return;
        GlobalPosition = Target.GlobalPosition;
        ResetSmoothing();
    }
}
