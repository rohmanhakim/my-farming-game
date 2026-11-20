using System;
using Godot;

namespace MyFarmingGame.Player;

/// <summary>Where Kit looks. Kit walks in 8 directions but faces and uses tools in these 4.</summary>
public enum Facing { Down, Up, Left, Right }

/// <summary>
/// Kit's movement (Task 1.2, controls.md §2 and §4).
///
/// * Walks smoothly in 8 directions: left stick / D-pad (any SDL gamepad: Xbox, PlayStation,
///   Switch Pro) or WASD / arrows. Partial stick tilt walks slower.
/// * Faces 4 directions (Harvest Moon / Stardew style). On an exact diagonal the current facing is
///   kept if it is one of the two directions involved, so diagonal walking does not flicker.
/// * Click-to-move (DST style): left-click the ground walks there; holding the button keeps walking
///   toward the cursor. Any stick/key movement cancels the click target.
/// * <see cref="WalkTo"/> is the "walk to it, then act" hook for later tasks: they pass a callback
///   that runs on arrival (for example "use the active tool on this tile").
/// </summary>
public partial class PlayerController : CharacterBody2D
{
    /// <summary>Walking speed in pixels per second (the sprite is 64 px tall). Tune in playtest.</summary>
    [Export] public float MoveSpeed { get; set; } = 160f;

    /// <summary>How fast Kit reaches full speed and stops (px/s²). High = snappy, low = floaty.</summary>
    [Export] public float Acceleration { get; set; } = 1400f;

    /// <summary>Distance at which a click target counts as reached.</summary>
    [Export] public float ArriveDistance { get; set; } = 4f;

    /// <summary>Speed below which Kit counts as standing (idle animation).</summary>
    public const float IdleSpeed = 5f;

    /// <summary>Stick/key input smaller than this is ignored (matches the move_* action deadzones).</summary>
    public const float InputDeadzone = 0.2f;

    /// <summary>Physics frames of no progress toward a click target before giving up (blocked).</summary>
    private const int StuckFrameLimit = 12;

    public Facing Facing { get; private set; } = Facing.Down;

    /// <summary>True while walking toward a clicked point or a <see cref="WalkTo"/> target.</summary>
    public bool HasMoveTarget => _target.HasValue;

    /// <summary>Emitted when the facing changes. Argument is the new <see cref="Player.Facing"/> as int.</summary>
    [Signal] public delegate void FacingChangedEventHandler(int facing);

    private Vector2? _target;
    private Action _onArrived;
    private bool _mouseHeld;
    private int _stuckFrames;
    private AnimationPlayer _anim;

    /// <summary>Test hook: when set, used instead of reading the move_* actions.</summary>
    public Vector2? InputOverride { get; set; }

    public override void _Ready()
    {
        _anim = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
        PlayAnimation();
    }

    public override void _UnhandledInput(InputEvent e)
    {
        // Mouse only: Space and gamepad A also trigger "interact", but they act in place (later tasks).
        if (e is InputEventMouseButton mb && mb.ButtonIndex == MouseButton.Left)
        {
            _mouseHeld = mb.Pressed;
            if (mb.Pressed)
            {
                // Ground click. Object clicks (walk to it, then act) arrive in later tasks: they check
                // what is under the cursor first and call WalkTo with a callback.
                // Use the click's own position (not the polled cursor) so the target is exactly
                // where the click happened, also for injected events.
                WalkTo(GetViewport().GetCanvasTransform().AffineInverse() * mb.Position);
                GetViewport().SetInputAsHandled();
            }
        }
    }

    /// <summary>Walks toward <paramref name="target"/>; runs <paramref name="onArrived"/> on arrival.
    /// Cancelled (callback not run) by stick/key movement or when Kit is blocked.</summary>
    public void WalkTo(Vector2 target, Action onArrived = null)
    {
        if (!target.IsFinite())
            return; // a bad cursor transform must never send Kit (and the camera) to NaN
        _target = target;
        _onArrived = onArrived;
        _stuckFrames = 0;
    }

    public void CancelWalkTo()
    {
        _target = null;
        _onArrived = null;
        _mouseHeld = false;
    }

    public override void _PhysicsProcess(double delta)
    {
        Step((float)delta);
    }

    /// <summary>One movement step. Public so tests can drive it with a fixed delta.</summary>
    public void Step(float delta)
    {
        Vector2 input = InputOverride ?? Input.GetVector("move_left", "move_right", "move_up", "move_down", InputDeadzone);

        if (input != Vector2.Zero)
            CancelWalkTo();
        else if (_mouseHeld && InputOverride == null && GetGlobalMousePosition().IsFinite())
            _target = GetGlobalMousePosition();

        Vector2 desired = input * MoveSpeed;
        if (_target.HasValue)
        {
            Vector2 to = _target.Value - GlobalPosition;
            if (to.Length() <= ArriveDistance)
            {
                Arrive();
                desired = Vector2.Zero;
            }
            else
            {
                // Slow down over the last few pixels so Kit does not overshoot the point.
                float speed = Mathf.Min(MoveSpeed, to.Length() / Math.Max(delta, 0.0001f));
                desired = to.Normalized() * speed;
            }
        }

        Velocity = Velocity.MoveToward(desired, Acceleration * delta);
        if (_target.HasValue && desired != Vector2.Zero)
            Velocity = desired; // click-walks start at full speed so short clicks feel responsive

        if (!Velocity.IsFinite())
        {
            GD.PushError($"[PlayerController] non-finite velocity {Velocity}; stopping Kit");
            Velocity = Vector2.Zero;
            CancelWalkTo();
        }

        Vector2 before = GlobalPosition;
        MoveAndSlide();

        if (_target.HasValue)
        {
            bool progressed = (GlobalPosition - before).Length() > 0.25f;
            _stuckFrames = progressed ? 0 : _stuckFrames + 1;
            if (_stuckFrames >= StuckFrameLimit && !_mouseHeld)
                CancelWalkTo();
        }

        UpdateFacing(desired);
        PlayAnimation();
    }

    private void Arrive()
    {
        var cb = _onArrived;
        _target = null;
        _onArrived = null;
        Velocity = Vector2.Zero;
        cb?.Invoke();
    }

    /// <summary>Picks one of 4 facings from an 8-way direction (keeps the current one on ties).</summary>
    public static Facing FacingFor(Vector2 dir, Facing current)
    {
        if (dir.LengthSquared() < 0.0001f)
            return current;
        float ax = Mathf.Abs(dir.X), ay = Mathf.Abs(dir.Y);
        Facing horizontal = dir.X < 0 ? Facing.Left : Facing.Right;
        Facing vertical = dir.Y < 0 ? Facing.Up : Facing.Down;
        // Within ~10° of a perfect diagonal: keep the current facing if it is one of the two parts.
        if (Mathf.Abs(ax - ay) <= 0.18f * Mathf.Max(ax, ay) && (current == horizontal || current == vertical))
            return current;
        return ax > ay ? horizontal : vertical;
    }

    private void UpdateFacing(Vector2 dir)
    {
        var next = FacingFor(dir, Facing);
        if (next == Facing)
            return;
        Facing = next;
        EmitSignal(SignalName.FacingChanged, (int)next);
    }

    /// <summary>Animation name for the current state, e.g. "walk_left" or "idle_down".</summary>
    public string CurrentAnimationName =>
        (Velocity.Length() > IdleSpeed ? "walk_" : "idle_") + Facing.ToString().ToLowerInvariant();

    private void PlayAnimation()
    {
        if (_anim == null)
            return;
        string name = CurrentAnimationName;
        if (_anim.CurrentAnimation != name && _anim.HasAnimation(name))
            _anim.Play(name);
    }
}
