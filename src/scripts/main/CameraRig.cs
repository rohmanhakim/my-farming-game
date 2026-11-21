using System;
using Godot;

namespace MyFarmingGame;

/// <summary>Which corner of the map the diagonal camera looks from.</summary>
public enum CameraCorner { SouthEast, SouthWest }

/// <summary>
/// The fixed, angled, diagonal 3D camera (Task 1.14; GDD §9, owner 2026-11-21).
///
/// * Tilted <see cref="Tilt"/>° down (not straight down) and turned <see cref="Turn"/>° from due south
///   towards the <see cref="Corner"/>, so the grid runs diagonally on screen and tiles show as diamonds.
/// * Follows a target (Kit) with exponential smoothing on the physics tick; <see cref="Snap"/> jumps.
/// * The player can't rotate freely or zoom (controls.md §7), but can <b>switch corners</b>
///   (south-east ↔ south-west, owner 2026-11-21) with <c>camera_switch</c>: right stick left/right on any
///   pad, or Q. The turn glides over ≈ 0.4 s. Tilt/turn presets stay debug-only (F9–F12 / 1–4,
///   <c>debug/CameraDebugKeys.cs</c>). Owner pick 2026-11-21: tilt 45°, turn 45°, south-east, perspective.
/// * <see cref="LimitMin"/>/<see cref="LimitMax"/> bound the focus point on the ground (XZ).
/// * <see cref="ScreenToGround"/> turns a screen-space direction (stick, keys) into a ground direction.
/// </summary>
public partial class CameraRig : Node3D
{
    /// <summary>Tilt presets in degrees below the horizon (owner picks one in Task 1.14).</summary>
    public static readonly float[] TiltPresets = { 45f, 52f, 60f };

    /// <summary>Turn presets in degrees from due south (≈ 45°, not necessarily exact — owner 2026-11-21).</summary>
    public static readonly float[] TurnPresets = { 30f, 37f, 45f };

    [Export] public NodePath TargetPath { get; set; }
    [Export] public float Tilt { get; set; } = 45f;   // owner pick 2026-11-21
    [Export] public float Turn { get; set; } = 45f;   // owner pick 2026-11-21
    [Export] public CameraCorner Corner { get; set; } = CameraCorner.SouthEast;
    /// <summary>Distance from the focus point to the camera (perspective), metres.</summary>
    [Export] public float Distance { get; set; } = 13f;
    /// <summary>Vertical field of view (perspective), degrees.</summary>
    [Export] public float Fov { get; set; } = 40f;
    [Export] public bool Orthographic { get; set; }
    /// <summary>Visible height of the view (orthographic), metres.</summary>
    [Export] public float OrthoSize { get; set; } = 9f;
    /// <summary>Follow smoothing; higher = tighter. Matches the 2D camera's feel (speed 5).</summary>
    [Export] public float Smoothing { get; set; } = 5f;
    [Export] public Vector2 LimitMin { get; set; } = new(-10000, -10000);
    [Export] public Vector2 LimitMax { get; set; } = new(10000, 10000);

    public Node3D Target { get; set; }
    public Camera3D Camera { get; private set; }

    /// <summary>The smoothed point on the ground the camera looks at.</summary>
    public Vector3 Focus { get; private set; }

    /// <summary>Signed turn in degrees while gliding to the other corner; null when settled.</summary>
    private float? _gliding;

    /// <summary>How fast the corner switch glides (higher = quicker; ≈ 0.4 s at 12).</summary>
    [Export] public float SwitchSpeed { get; set; } = 12f;

    /// <summary>The input action that switches corners (bound in project.godot).</summary>
    public const string SwitchAction = "camera_switch";

    /// <summary>True while the camera glides to the other corner.</summary>
    public bool IsSwitching => _gliding.HasValue;

    /// <summary>Signed turn the camera is heading to: + = south-east, − = south-west.</summary>
    private float TargetSignedTurn => Turn * (Corner == CameraCorner.SouthEast ? 1f : -1f);

    /// <summary>Switches to the other corner with a short glide (the player's camera switch).</summary>
    public void SwitchCorner()
    {
        float from = _gliding ?? TargetSignedTurn;
        ToggleCorner();
        _gliding = from;
    }

    /// <summary>Stick deflection that counts as a flick (matches the action's deadzone).</summary>
    public const float FlickThreshold = 0.6f;

    private bool _stickFlicked;

    public override void _UnhandledInput(InputEvent e)
    {
        if (!e.IsAction(SwitchAction)) return;
        bool pressed;
        if (e is InputEventJoypadMotion jm)
        {
            // Both stick directions are bound to one action, and Godot matches a motion event to the
            // first binding on that axis whatever its sign (so IsActionPressed is false for one side).
            // Detect the flick here instead: one switch per push past the threshold.
            bool out_ = Mathf.Abs(jm.AxisValue) >= FlickThreshold;
            pressed = out_ && !_stickFlicked;
            _stickFlicked = out_ || (Mathf.Abs(jm.AxisValue) > FlickThreshold * 0.5f && _stickFlicked);
        }
        else
        {
            pressed = e.IsPressed() && !e.IsEcho();
        }
        if (!pressed) return;
        if (!(Autoloads.GameClock.Instance?.IsPlayable ?? true)) return; // not under menus or at night
        SwitchCorner();
        GetViewport().SetInputAsHandled();
    }

    public override void _Ready()
    {
        Camera = GetNodeOrNull<Camera3D>("Camera3D");
        if (Camera == null)
        {
            Camera = new Camera3D { Name = "Camera3D" };
            AddChild(Camera);
        }
        Camera.Current = true;
        if (Target == null && TargetPath != null && !TargetPath.IsEmpty)
            Target = GetNodeOrNull<Node3D>(TargetPath);
        Snap();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Target != null)
        {
            float t = 1f - Mathf.Exp(-Smoothing * (float)delta);
            Focus = Clamp(Focus.Lerp(Ground(Target.GlobalPosition), t));
        }
        if (_gliding is float g)
        {
            float next = Mathf.Lerp(g, TargetSignedTurn, 1f - Mathf.Exp(-SwitchSpeed * (float)delta));
            _gliding = Mathf.Abs(next - TargetSignedTurn) < 0.2f ? null : next;
        }
        ApplyPose();
    }

    /// <summary>Jumps straight to the target with no smoothing (spawn, level change, waking up).</summary>
    public void Snap()
    {
        _gliding = null;
        if (Target != null)
            Focus = Clamp(Ground(Target.GlobalPosition));
        ApplyPose();
    }

    /// <summary>Unit vector on the ground from the focus point towards the camera.</summary>
    public Vector3 BackDirection
    {
        get
        {
            float t = Mathf.DegToRad(_gliding ?? TargetSignedTurn);
            return new Vector3(Mathf.Sin(t), 0, Mathf.Cos(t));
        }
    }

    /// <summary>Ground direction of "up the screen" (the way the camera looks, flattened).</summary>
    public Vector3 ScreenUpOnGround => -BackDirection;

    /// <summary>Ground direction of "right on the screen".</summary>
    public Vector3 ScreenRightOnGround => ScreenUpOnGround.Cross(Vector3.Up);

    /// <summary>Turns a screen-space direction (x right, y down — Input.GetVector) into a ground direction.
    /// Length is kept, so a half-tilted stick stays half.</summary>
    public Vector3 ScreenToGround(Vector2 screen) =>
        ScreenRightOnGround * screen.X - ScreenUpOnGround * screen.Y;

    /// <summary>Places and aims the camera for the current focus, tilt, turn, corner and projection.</summary>
    public void ApplyPose()
    {
        if (Camera == null || !Camera.IsInsideTree()) return;
        float tilt = Mathf.DegToRad(Mathf.Clamp(Tilt, 10f, 89f));
        Vector3 offset = BackDirection * Mathf.Cos(tilt) * Distance + Vector3.Up * Mathf.Sin(tilt) * Distance;
        Camera.GlobalPosition = Focus + offset;
        Camera.LookAt(Focus, Vector3.Up);
        Camera.Projection = Orthographic ? Camera3D.ProjectionType.Orthogonal : Camera3D.ProjectionType.Perspective;
        Camera.Fov = Fov;
        Camera.Size = OrthoSize;
        Camera.Far = Distance * 4f;
    }

    /// <summary>Angle of the camera's view below the horizon, degrees (tests).</summary>
    public float MeasuredTilt
    {
        get
        {
            Vector3 f = -Camera.GlobalBasis.Z;
            return Mathf.RadToDeg(Mathf.Asin(Math.Clamp(-f.Y, -1f, 1f)));
        }
    }

    public void CycleTilt() => Tilt = Next(TiltPresets, Tilt);
    public void CycleTurn() => Turn = Next(TurnPresets, Turn);
    public void ToggleCorner() => Corner = Corner == CameraCorner.SouthEast ? CameraCorner.SouthWest : CameraCorner.SouthEast;
    public void ToggleProjection() => Orthographic = !Orthographic;

    /// <summary>One line describing the current setting (debug label, Notes).</summary>
    public string Describe() =>
        $"tilt {Tilt:0}° · turn {Turn:0}° · {(Corner == CameraCorner.SouthEast ? "south-east" : "south-west")} corner · {(Orthographic ? "orthographic" : "perspective")}";

    private static float Next(float[] presets, float current)
    {
        int i = Array.FindIndex(presets, p => Mathf.IsEqualApprox(p, current));
        return presets[(i + 1) % presets.Length];
    }

    private static Vector3 Ground(Vector3 p) => new(p.X, 0, p.Z);

    private Vector3 Clamp(Vector3 p) => new(
        Mathf.Clamp(p.X, LimitMin.X, Mathf.Max(LimitMin.X, LimitMax.X)), 0,
        Mathf.Clamp(p.Z, LimitMin.Y, Mathf.Max(LimitMin.Y, LimitMax.Y)));
}
