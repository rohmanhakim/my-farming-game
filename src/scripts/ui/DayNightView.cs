using Godot;
using MyFarmingGame.Autoloads;

namespace MyFarmingGame.UI;

/// <summary>
/// The world's light across the day (Task 1.11): a CanvasModulate tint per day phase from
/// <see cref="GameClock"/>, eased over <see cref="FadeSeconds"/> when the phase changes. Night is dim
/// but readable (never below ≈ 50 % brightness). Lives in Main/World/Effects; only the world is
/// tinted, never the HUD.
/// </summary>
public partial class DayNightView : CanvasModulate
{
    [Export] public float FadeSeconds { get; set; } = 1.5f;

    private GameClock _clock;
    private Tween _tween;

    /// <summary>The colour for <paramref name="phase"/> (warm dawn, white day, golden evening, blue night).</summary>
    public static Color TintFor(DayPhase phase) => phase switch
    {
        DayPhase.Dawn => new Color(1.00f, 0.90f, 0.82f),
        DayPhase.Morning => new Color(1.00f, 0.98f, 0.95f),
        DayPhase.Midday => Colors.White,
        DayPhase.Afternoon => new Color(1.00f, 0.97f, 0.90f),
        DayPhase.Evening => new Color(1.00f, 0.84f, 0.68f),
        DayPhase.Dusk => new Color(0.74f, 0.70f, 0.88f),
        DayPhase.Night => new Color(0.52f, 0.56f, 0.80f),
        _ => new Color(0.50f, 0.54f, 0.78f),
    };

    /// <summary>The colour being faded to (tests).</summary>
    public Color Target { get; private set; } = Colors.White;

    public override void _Ready()
    {
        _clock = GameClock.Instance;
        if (_clock == null) return;
        _clock.StepChanged += OnStep;
        Target = TintFor(_clock.Phase);
        Color = Target;
    }

    public override void _ExitTree()
    {
        if (_clock != null && IsInstanceValid(_clock)) _clock.StepChanged -= OnStep;
    }

    private void OnStep(int phase)
    {
        Target = TintFor((DayPhase)phase);
        _tween?.Kill();
        if (!IsInsideTree()) { Color = Target; return; }
        _tween = CreateTween();
        _tween.TweenProperty(this, "color", Target, FadeSeconds);
    }
}
