using Godot;
using MyFarmingGame.Autoloads;
using MyFarmingGame.Player;

namespace MyFarmingGame.World;

/// <summary>
/// A building's inside (Task 1.5, GDD §4 "time stops indoors", Phase-001 §1 Indoors): while Kit
/// stands in this area the roof fades out and <see cref="GameClock"/> stops; outside, the roof comes
/// back and the clock runs. Created by <see cref="Building"/> over its floor; any Area2D shape works.
/// </summary>
public partial class IndoorZone : Area2D
{
    /// <summary>Name passed to <see cref="GameClock.EnterIndoors"/> ("house", "coop", …).</summary>
    [Export] public string ZoneName { get; set; } = "house";

    /// <summary>The roof drawn over the building; faded out while Kit is inside.</summary>
    public CanvasItem Roof { get; set; }

    /// <summary>Seconds the roof takes to fade.</summary>
    [Export] public float FadeSeconds { get; set; } = 0.25f;

    [Signal] public delegate void PlayerInsideChangedEventHandler(bool inside);

    public bool PlayerInside { get; private set; }

    private Tween _tween;

    public override void _Ready()
    {
        CollisionLayer = 0;
        CollisionMask = 1; // Kit's body layer
        Monitorable = false;
        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;
    }

    public override void _ExitTree()
    {
        if (PlayerInside)
            GameClock.Instance?.ExitIndoors(ZoneName);
        PlayerInside = false;
    }

    private void OnBodyEntered(Node2D body)
    {
        if (!body.IsInGroup(PlayerController.Group) || PlayerInside) return;
        PlayerInside = true;
        GameClock.Instance?.EnterIndoors(ZoneName);
        FadeRoof(0f);
        EmitSignal(SignalName.PlayerInsideChanged, true);
    }

    private void OnBodyExited(Node2D body)
    {
        if (!body.IsInGroup(PlayerController.Group) || !PlayerInside) return;
        PlayerInside = false;
        GameClock.Instance?.ExitIndoors(ZoneName);
        FadeRoof(1f);
        EmitSignal(SignalName.PlayerInsideChanged, false);
    }

    private void FadeRoof(float alpha)
    {
        if (Roof == null) return;
        _tween?.Kill();
        _tween = CreateTween();
        _tween.TweenProperty(Roof, "modulate:a", alpha, FadeSeconds);
    }
}
