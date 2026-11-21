using System;
using Godot;
using MyFarmingGame.Autoloads;
using MyFarmingGame.Core;

namespace MyFarmingGame.Player;

/// <summary>Where Kit's tools point: one of the 4 grid directions. Up = north (−Z), Down = south (+Z),
/// Left = west (−X), Right = east (+X). Kit walks in any direction but faces and uses tools in these 4.</summary>
public enum Facing { Down, Up, Left, Right }

/// <summary>
/// Kit (Task 1.2; 3D since Tasks 1.14–1.15, controls.md §2 and §4).
///
/// * Walks smoothly in 8 directions <b>relative to the screen</b> under the fixed diagonal camera:
///   left stick / D-pad (any SDL gamepad: Xbox, PlayStation, Switch Pro) or WASD / arrows; up = up
///   the screen. Partial tilt walks slower; diagonals are as fast as straight lines.
/// * The model (child <see cref="NamedModel"/> "Model") turns smoothly towards the walking direction;
///   <see cref="Facing"/> (the tool and interaction front) is the grid direction nearest to the walk
///   and keeps the current one near an exact diagonal (<see cref="FacingFor(Vector2, Facing)"/>).
/// * Click-to-move (DST style): left-click the ground (camera ray onto y = 0) walks there; holding
///   keeps walking towards the cursor; any stick/key input cancels. <see cref="WalkTo"/> is the
///   "walk there, then act" hook.
/// * Interactables (<see cref="IInteractable"/>): <c>interact</c> (A / Space) uses the nearest one in
///   reach; a left-click on one (click areas on physics layer 8) walks to it, then uses it.
/// * Tools on the ground (<see cref="IToolTarget"/>): with nothing to interact with in reach,
///   <c>interact</c> uses the active tool on the tile in front (<see cref="FrontPoint"/>); a click on
///   a tile within <see cref="TileReach"/> works it at once, a farther one walks Kit beside it first.
/// * <see cref="Frozen"/> stops all movement and input (menus, sleeping).
/// Units: metres (1 tile = 1 m). The body origin is Kit's feet; Kit stays at y = 0.
/// </summary>
public partial class PlayerController : CharacterBody3D
{
    /// <summary>Walking speed (the 2D build's 160 px/s ÷ 64). Tune in playtest.</summary>
    [Export] public float MoveSpeed { get; set; } = 2.5f;
    /// <summary>m/s² (the 2D 1400 px/s² ÷ 64).</summary>
    [Export] public float Acceleration { get; set; } = 22f;
    [Export] public float ArriveDistance { get; set; } = 0.06f;
    /// <summary>How fast the model turns to the walking direction (higher = snappier).</summary>
    [Export] public float TurnSpeed { get; set; } = 14f;

    public const float IdleSpeed = 0.08f;
    /// <summary>Stick/key input smaller than this is ignored (matches the move_* action deadzones).</summary>
    public const float InputDeadzone = 0.2f;
    /// <summary>Distance from Kit's feet to the point the tile in front is picked from.</summary>
    public const float FrontReach = 0.7f;
    /// <summary>A clicked tile whose centre is this close to Kit's feet is worked without walking:
    /// the tile Kit stands on and the tiles touching it, diagonals included (2D: 96 px).</summary>
    public const float TileReach = 1.5f;
    /// <summary>How far beside a tile Kit stands to work it (2D: 56 px).</summary>
    public const float StandBeside = 0.875f;
    private const int StuckFrameLimit = 12;

    /// <summary>Group the player joins (interactables use it to notice Kit).</summary>
    public const string Group = "player";

    public Facing Facing { get; private set; } = Facing.Down;

    /// <summary>The look (child "Model", a <see cref="NamedModel"/> "kit"). It turns; the body does not.</summary>
    public NamedModel Model { get; private set; }

    /// <summary>Model yaw in radians (0 = facing −Z / north).</summary>
    public float ModelYaw => Model?.Rotation.Y ?? 0f;

    [Signal] public delegate void FacingChangedEventHandler(int facing);

    /// <summary>Unit ground vector of a grid facing.</summary>
    public static Vector3 FacingVector(Facing f) => f switch
    {
        Facing.Up => Vector3.Forward,   // −Z, north
        Facing.Left => Vector3.Left,     // −X, west
        Facing.Right => Vector3.Right,   // +X, east
        _ => Vector3.Back,               // +Z, south
    };

    /// <summary>Picks one of the 4 facings from a direction on the ground as (x, z) — y of the
    /// Vector2 is z (south positive). Within ~10° of an exact diagonal the current facing is kept if
    /// it is one of the two directions involved, so diagonal walking does not flicker; otherwise an
    /// exact tie picks the north/south one.</summary>
    public static Facing FacingFor(Vector2 dir, Facing current)
    {
        if (dir.LengthSquared() < 0.0001f)
            return current;
        float ax = Mathf.Abs(dir.X), ay = Mathf.Abs(dir.Y);
        Facing horizontal = dir.X < 0 ? Facing.Left : Facing.Right;
        Facing vertical = dir.Y < 0 ? Facing.Up : Facing.Down;
        if (Mathf.Abs(ax - ay) <= 0.18f * Mathf.Max(ax, ay) && (current == horizontal || current == vertical))
            return current;
        return ax > ay ? horizontal : vertical;
    }

    /// <summary>Grid facing for a ground direction (keeps <paramref name="current"/> near a diagonal).</summary>
    public static Facing FacingFor(Vector3 groundDir, Facing current) =>
        FacingFor(new Vector2(groundDir.X, groundDir.Z), current);

    /// <summary>Model yaw (radians) that looks along a ground direction; 0 = −Z (north).</summary>
    public static float YawFor(Vector3 dir) => Mathf.Atan2(-dir.X, -dir.Z);

    /// <summary>Kit's feet on the ground (the body origin sits at the feet).</summary>
    public Vector3 Feet => new(GlobalPosition.X, 0, GlobalPosition.Z);

    /// <summary>The point the tile in front of Kit is picked from.</summary>
    public Vector3 FrontPoint => Feet + FacingVector(Facing) * FrontReach;

    /// <summary>The whole tile in front of Kit (1 tile = 1 m; tile (x, y) covers x..x+1, z = y..y+1).</summary>
    public Vector2I FrontCell => new(Mathf.FloorToInt(FrontPoint.X), Mathf.FloorToInt(FrontPoint.Z));

    /// <summary>Turns Kit to <paramref name="f"/> (after a click-walk to a tile); the model turns too.</summary>
    public void Face(Facing f)
    {
        if (Model != null) Model.Rotation = new Vector3(0, YawFor(FacingVector(f)), 0);
        if (f == Facing) return;
        Facing = f;
        EmitSignal(SignalName.FacingChanged, (int)f);
    }

    private bool _frozen;

    /// <summary>When true Kit ignores all input and stands still (menus, sleeping).</summary>
    public bool Frozen
    {
        get => _frozen;
        set
        {
            _frozen = value;
            if (value) { CancelWalkTo(); Velocity = Vector3.Zero; }
        }
    }

    /// <summary>True while walking toward a clicked point or a <see cref="WalkTo"/> target.</summary>
    public bool HasMoveTarget => _target.HasValue;

    /// <summary>Test hook: used instead of the move_* actions (screen space, like Input.GetVector).</summary>
    public Vector2? InputOverride { get; set; }

    private Vector3? _target;
    private Action _onArrived;
    private bool _mouseHeld;
    private int _stuckFrames;

    public override void _Ready()
    {
        MotionMode = MotionModeEnum.Floating; // top-down: no gravity or floor logic, Kit stays at y = 0
        Model = GetNodeOrNull<NamedModel>("Model");
        if (Model != null) // start looking the way Kit faces
            Model.Rotation = new Vector3(0, YawFor(FacingVector(Facing)), 0);
        AddToGroup(Group);
    }

    /// <summary>The camera rig in use, if any (screen ↔ ground directions).</summary>
    private CameraRig Rig => GetViewport()?.GetCamera3D()?.GetParent() as CameraRig;

    /// <summary>Screen-space direction (x right, y down) → ground direction, using the current camera.</summary>
    public Vector3 ScreenToGround(Vector2 screen) =>
        Rig?.ScreenToGround(screen) ?? new Vector3(screen.X, 0, screen.Y);

    /// <summary>The ground point (y = 0) under a screen position, or null if the ray misses the ground.</summary>
    public Vector3? GroundAt(Vector2 screenPos)
    {
        var cam = GetViewport()?.GetCamera3D();
        if (cam == null) return null;
        Vector3 origin = cam.ProjectRayOrigin(screenPos);
        Vector3 dir = cam.ProjectRayNormal(screenPos);
        if (Mathf.Abs(dir.Y) < 0.0001f) return null;
        float t = -origin.Y / dir.Y;
        if (t < 0) return null;
        Vector3 p = origin + dir * t;
        return p.IsFinite() ? new Vector3(p.X, 0, p.Z) : null;
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
            if (!mb.Pressed) return;
            var clicked = InteractableAt(mb.Position);
            if (clicked != null)
            {
                // Object click: walk to it, then use it. No hold-to-follow.
                _mouseHeld = false;
                WalkTo(clicked.ApproachPoint, () => { if (clicked.CanInteract) clicked.Interact(this); });
            }
            else if (GroundAt(mb.Position) is Vector3 ground)
            {
                if (ToolTargetAt(ground) is IToolTarget target)
                {
                    _mouseHeld = false;
                    WalkToTile(target, ground);
                }
                else
                    WalkTo(ground); // ground click
            }
            GetViewport().SetInputAsHandled();
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
                GetViewport().SetInputAsHandled();
        }
    }

    /// <summary>True when Kit may use tools now (no menu, prompt or night).</summary>
    private static bool ToolsAllowed => GameClock.Instance?.IsPlayable ?? true;

    /// <summary>The tool target (garden) whose tile is under <paramref name="groundPoint"/>, if any.</summary>
    public IToolTarget ToolTargetAt(Vector3 groundPoint)
    {
        foreach (Node n in GetTree().GetNodesInGroup(IToolTarget.Group))
            if (n is IToolTarget t && t.ContainsPoint(groundPoint))
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

    /// <summary>Works the tile under <paramref name="groundPoint"/>: at once when it is within
    /// <see cref="TileReach"/> (turn and use the tool), otherwise walk beside it first.</summary>
    public void WalkToTile(IToolTarget target, Vector3 groundPoint)
    {
        Vector3 center = target.TileCenterAt(groundPoint);
        Vector3 d = Feet - center;
        if (d.Length() <= TileReach)
        {
            CancelWalkTo();
            if (d.LengthSquared() > 0.0003f) Face(FacingFor(-d, Facing));
            if (ToolsAllowed)
                target.UseToolAt(ToolBelt.Instance?.Active ?? Tool.Hands, center);
            return;
        }
        // Stand beside the tile on Kit's side; if that spot is blocked, try the other sides.
        Vector3 first = Mathf.Abs(d.X) > Mathf.Abs(d.Z) ? new Vector3(Mathf.Sign(d.X), 0, 0) : new Vector3(0, 0, d.Z < 0 ? -1 : 1);
        Vector3 side = first;
        foreach (var s in new[] { first, new Vector3(first.Z, 0, first.X), new Vector3(-first.Z, 0, -first.X), -first })
            if (FeetFreeAt(center + s * StandBeside)) { side = s; break; }
        Vector3 approach = center + side * StandBeside;
        Facing face = FacingFor(-side, Facing);
        WalkTo(approach, () =>
        {
            Face(face);
            if (ToolsAllowed)
                target.UseToolAt(ToolBelt.Instance?.Active ?? Tool.Hands, center);
        });
    }

    /// <summary>True when Kit could stand at <paramref name="feetPoint"/> (no solid body there).</summary>
    public bool FeetFreeAt(Vector3 feetPoint)
    {
        var q = new PhysicsShapeQueryParameters3D
        {
            Shape = new CapsuleShape3D { Radius = 0.28f, Height = 1.6f },
            Transform = new Transform3D(Basis.Identity, new Vector3(feetPoint.X, 0.85f, feetPoint.Z)),
            CollideWithBodies = true, CollideWithAreas = false, CollisionMask = CollisionMask,
            Exclude = new Godot.Collections.Array<Rid> { GetRid() },
        };
        return GetWorld3D().DirectSpaceState.IntersectShape(q, 1).Count == 0;
    }

    /// <summary>The interactable whose click area is under the screen position, if any.</summary>
    public IInteractable InteractableAt(Vector2 screenPos)
    {
        var cam = GetViewport()?.GetCamera3D();
        if (cam == null) return null;
        Vector3 from = cam.ProjectRayOrigin(screenPos);
        Vector3 to = from + cam.ProjectRayNormal(screenPos) * cam.Far;
        var q = PhysicsRayQueryParameters3D.Create(from, to, IInteractable.ClickLayer);
        q.CollideWithAreas = true;
        q.CollideWithBodies = false;
        var hit = GetWorld3D().DirectSpaceState.IntersectRay(q);
        if (hit.Count == 0) return null;
        for (Node n = hit["collider"].As<Node>(); n != null; n = n.GetParent())
            if (n is IInteractable it)
                return it;
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
            float d = Feet.DistanceSquaredTo(it.ApproachPoint);
            if (d < bestDist) { bestDist = d; best = it; }
        }
        return best;
    }

    /// <summary>Walks towards <paramref name="target"/> (ground point); runs <paramref name="onArrived"/>
    /// on arrival. Cancelled (callback not run) by stick/key movement or when Kit is blocked.</summary>
    public void WalkTo(Vector3 target, Action onArrived = null)
    {
        if (!target.IsFinite()) return; // a bad cursor ray must never send Kit (and the camera) to NaN
        _target = new Vector3(target.X, 0, target.Z);
        _onArrived = onArrived;
        _stuckFrames = 0;
    }

    public void CancelWalkTo()
    {
        _target = null;
        _onArrived = null;
        _mouseHeld = false;
    }

    public override void _PhysicsProcess(double delta) => Step((float)delta);

    /// <summary>One movement step (public so tests can drive it).</summary>
    public void Step(float delta)
    {
        Vector2 input = InputOverride ?? Input.GetVector("move_left", "move_right", "move_up", "move_down", InputDeadzone);
        if (_frozen) { input = Vector2.Zero; Velocity = Vector3.Zero; }

        if (input != Vector2.Zero)
            CancelWalkTo();
        else if (_mouseHeld && InputOverride == null && GroundAt(GetViewport().GetMousePosition()) is Vector3 cursor)
            _target = cursor;

        Vector3 desired = ScreenToGround(input) * MoveSpeed;
        if (_target.HasValue)
        {
            Vector3 to = _target.Value - Feet;
            if (to.Length() <= ArriveDistance)
            {
                Arrive();
                desired = Vector3.Zero;
            }
            else
            {
                float speed = Mathf.Min(MoveSpeed, to.Length() / Math.Max(delta, 0.0001f));
                desired = to.Normalized() * speed;
            }
        }

        Velocity = Velocity.MoveToward(desired, Acceleration * delta);
        if (_target.HasValue && desired != Vector3.Zero)
            Velocity = desired; // click-walks start at full speed so short clicks feel responsive
        Velocity = new Vector3(Velocity.X, 0, Velocity.Z);

        if (!Velocity.IsFinite())
        {
            GD.PushError($"[PlayerController] non-finite velocity {Velocity}; stopping Kit");
            Velocity = Vector3.Zero;
            CancelWalkTo();
        }

        Vector3 before = GlobalPosition;
        MoveAndSlide();
        GlobalPosition = new Vector3(GlobalPosition.X, 0, GlobalPosition.Z);

        if (_target.HasValue)
        {
            bool progressed = (GlobalPosition - before).Length() > 0.004f;
            _stuckFrames = progressed ? 0 : _stuckFrames + 1;
            if (_stuckFrames >= StuckFrameLimit && !_mouseHeld)
                CancelWalkTo();
        }

        var next = FacingFor(desired, Facing);
        if (next != Facing)
        {
            Facing = next;
            EmitSignal(SignalName.FacingChanged, (int)next);
        }
        TurnModel(desired, delta);
    }

    /// <summary>Turns the model smoothly towards the walking direction (any angle).</summary>
    private void TurnModel(Vector3 dir, float delta)
    {
        if (Model == null || dir.LengthSquared() < 0.0001f) return;
        float yaw = Mathf.LerpAngle(Model.Rotation.Y, YawFor(dir), 1f - Mathf.Exp(-TurnSpeed * delta));
        Model.Rotation = new Vector3(0, yaw, 0);
    }

    private void Arrive()
    {
        var cb = _onArrived;
        _target = null;
        _onArrived = null;
        Velocity = Vector3.Zero;
        cb?.Invoke();
    }

    /// <summary>True while Kit is walking (idle below <see cref="IdleSpeed"/>).</summary>
    public bool IsWalking => new Vector2(Velocity.X, Velocity.Z).Length() > IdleSpeed;

    /// <summary>"walk" or "idle" plus the facing, e.g. "walk_left" (animations come with real models).</summary>
    public string CurrentAnimationName => (IsWalking ? "walk_" : "idle_") + Facing.ToString().ToLowerInvariant();
}
