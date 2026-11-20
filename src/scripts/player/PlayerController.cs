using System;
using Godot;
using MyFarmingGame.Autoloads;
using MyFarmingGame.Core;

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
/// * Interactables (Task 1.3, <see cref="IInteractable"/>): <c>interact</c> (A / Space) uses the
///   nearest one in reach; left-clicking one walks to it, then uses it.
/// * Tools on the ground (Task 1.5, <see cref="IToolTarget"/>): with nothing to interact with in
///   reach, <c>interact</c> uses the active tool on the tile in front of Kit (<see cref="FrontPoint"/>);
///   left-clicking a tile walks Kit beside it, turns Kit to face it and uses the tool there.
/// * <see cref="Frozen"/> stops all movement and input (prompts, sleeping).
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

    /// <summary>Kit's feet relative to the body origin (the collision box sits at the feet).</summary>
    public static readonly Vector2 FeetOffset = new(0, 20);

    /// <summary>How far in front of Kit's feet the "tile in front" is picked.</summary>
    public const float FrontReach = 44f;

    /// <summary>Unit vector of a facing.</summary>
    public static Vector2 FacingVector(Facing f) => f switch
    {
        Facing.Up => Vector2.Up,
        Facing.Left => Vector2.Left,
        Facing.Right => Vector2.Right,
        _ => Vector2.Down,
    };

    /// <summary>The point the tile in front of Kit is picked from (world coordinates).</summary>
    public Vector2 FrontPoint => GlobalPosition + FeetOffset + FacingVector(Facing) * FrontReach;

    /// <summary>Turns Kit to <paramref name="f"/> (after a click-walk to a tile).</summary>
    public void Face(Facing f)
    {
        if (f == Facing) return;
        Facing = f;
        EmitSignal(SignalName.FacingChanged, (int)f);
        PlayAnimation();
    }

    /// <summary>Group the player joins (interactables use it to notice Kit).</summary>
    public const string Group = "player";

    private bool _frozen;

    /// <summary>When true Kit ignores all input and stands still (menus, sleeping).</summary>
    public bool Frozen
    {
        get => _frozen;
        set
        {
            _frozen = value;
            if (value)
            {
                CancelWalkTo();
                Velocity = Vector2.Zero;
            }
        }
    }

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
        AddToGroup(Group);
        PlayAnimation();
    }

    public override void _UnhandledInput(InputEvent e)
    {
        if (_frozen)
        {
            if (e is InputEventMouseButton { ButtonIndex: MouseButton.Left, Pressed: false })
                _mouseHeld = false;
            return;
        }
        if (e is InputEventMouseButton mb && mb.ButtonIndex == MouseButton.Left)
        {
            _mouseHeld = mb.Pressed;
            if (mb.Pressed)
            {
                // Use the click's own position (not the polled cursor) so the target is exactly
                // where the click happened, also for injected events.
                Vector2 world = GetViewport().GetCanvasTransform().AffineInverse() * mb.Position;
                var clicked = InteractableAt(world);
                if (clicked != null)
                {
                    // Object click: walk to it, then use it. No hold-to-follow.
                    _mouseHeld = false;
                    WalkTo(clicked.ApproachPoint, () => { if (clicked.CanInteract) clicked.Interact(this); });
                }
                else if (ToolTargetAt(world) is IToolTarget target)
                {
                    // Tile click: walk beside it, face it, use the active tool on it.
                    _mouseHeld = false;
                    WalkToTile(target, world);
                }
                else
                {
                    WalkTo(world); // ground click
                }
                GetViewport().SetInputAsHandled();
            }
            return;
        }
        // Space / gamepad A (the mouse part of "interact" is handled above).
        if (e.IsActionPressed("interact") && !e.IsEcho())
        {
            var near = NearestInReach();
            if (near != null)
            {
                CancelWalkTo();
                near.Interact(this);
                GetViewport().SetInputAsHandled();
            }
            else if (UseToolInFront())
            {
                GetViewport().SetInputAsHandled();
            }
        }
    }

    /// <summary>True when Kit may use tools now (no menu, prompt or night).</summary>
    private static bool ToolsAllowed => GameClock.Instance?.IsPlayable ?? true;

    /// <summary>The tool target (garden) whose tile is under <paramref name="worldPoint"/>, if any.</summary>
    public IToolTarget ToolTargetAt(Vector2 worldPoint)
    {
        foreach (Node n in GetTree().GetNodesInGroup(IToolTarget.Group))
            if (n is IToolTarget t && t.ContainsPoint(worldPoint))
                return t;
        return null;
    }

    /// <summary>Uses the active tool on the tile in front of Kit. Returns true if a tile was there.</summary>
    public bool UseToolInFront()
    {
        if (!ToolsAllowed) return false;
        var target = ToolTargetAt(FrontPoint);
        if (target == null) return false;
        CancelWalkTo();
        target.UseToolAt(ToolBelt.Instance?.Active ?? Tool.Hands, FrontPoint);
        return true;
    }

    /// <summary>Walks next to the tile under <paramref name="worldPoint"/>, then faces it and uses the
    /// active tool on it (the mouse "walk to it, then act" for tiles).</summary>
    public void WalkToTile(IToolTarget target, Vector2 worldPoint)
    {
        Vector2 center = target.TileCenterAt(worldPoint);
        Vector2 feet = GlobalPosition + FeetOffset;
        Vector2 d = feet - center;
        Vector2 side = Mathf.Abs(d.X) > Mathf.Abs(d.Y) ? new Vector2(Mathf.Sign(d.X), 0) : new Vector2(0, d.Y < 0 ? -1 : 1);
        Vector2 approach = center + side * 56f - FeetOffset;
        Facing face = FacingFor(-side, Facing);
        WalkTo(approach, () =>
        {
            Face(face);
            if (ToolsAllowed)
                target.UseToolAt(ToolBelt.Instance?.Active ?? Tool.Hands, center);
        });
    }

    /// <summary>The interactable whose click area is under <paramref name="worldPoint"/>, if any.</summary>
    public IInteractable InteractableAt(Vector2 worldPoint)
    {
        var query = new PhysicsPointQueryParameters2D
        {
            Position = worldPoint,
            CollideWithAreas = true,
            CollideWithBodies = false,
            CollisionMask = IInteractable.ClickLayer,
        };
        foreach (var hit in GetWorld2D().DirectSpaceState.IntersectPoint(query, 8))
        {
            for (Node n = hit["collider"].As<Node>(); n != null; n = n.GetParent())
                if (n is IInteractable it)
                    return it;
        }
        return null;
    }

    /// <summary>The closest usable interactable Kit can reach from here, if any.</summary>
    public IInteractable NearestInReach()
    {
        IInteractable best = null;
        float bestDist = float.MaxValue;
        foreach (Node n in GetTree().GetNodesInGroup(IInteractable.Group))
        {
            if (n is not IInteractable it || !it.CanInteract || !it.IsInReach(this))
                continue;
            float d = GlobalPosition.DistanceSquaredTo(it.ApproachPoint);
            if (d < bestDist) { bestDist = d; best = it; }
        }
        return best;
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
        if (_frozen)
        {
            input = Vector2.Zero;
            Velocity = Vector2.Zero;
        }

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
