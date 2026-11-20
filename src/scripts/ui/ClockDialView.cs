using Godot;
using MyFarmingGame.Autoloads;
using MyFarmingGame.Core;

namespace MyFarmingGame.UI;

/// <summary>
/// HUD clock (Task 1.3, controls.md §6): a round dial of the 8 day phases with a sun or moon
/// marker that jumps one phase at a time, a digital readout that ticks every 15 minutes ("07:45") and the season icon
/// with the day of the season. Pure view: all state lives in <see cref="GameClock"/>.
/// Icons are loaded by name (clock_sun, clock_moon, season_spring …) so final art swaps in.
/// </summary>
public partial class ClockDialView : Control
{
    /// <summary>Dial radius in pixels; the dial sits centred at the top of this control.</summary>
    [Export] public float Radius { get; set; } = 52f;

    /// <summary>Optional clock to show (tests); defaults to the GameClock autoload.</summary>
    public GameClock Clock { get; set; }

    /// <summary>The phase the dial currently shows (tests compare it with the clock).</summary>
    public DayPhase ShownPhase { get; private set; }

    /// <summary>Sprite name of the marker currently drawn: "clock_sun" or "clock_moon".</summary>
    public string MarkerSprite => GameClock.IsDaylight(ShownPhase) ? "clock_sun" : "clock_moon";

    public string ShownTimeText => _time?.Text ?? "";
    public string ShownDayText => _day?.Text ?? "";

    private static readonly Color[] PhaseColors =
    {
        new(0.98f, 0.74f, 0.78f), // Dawn — soft pink
        new(0.62f, 0.85f, 1.00f), // Morning — bright, clear
        new(0.80f, 0.94f, 1.00f), // Midday — brightest
        new(1.00f, 0.88f, 0.55f), // Afternoon — long light
        new(1.00f, 0.66f, 0.32f), // Evening — gold → amber
        new(0.46f, 0.40f, 0.70f), // Dusk — lane darkens
        new(0.20f, 0.22f, 0.46f), // Night — dark, quiet
        new(0.11f, 0.11f, 0.27f), // Small hours — deepest dark
    };

    private Label _time;
    private Label _day;
    private TextureRect _seasonIcon;

    public override void _Ready()
    {
        _time = GetNode<Label>("%Time");
        _day = GetNode<Label>("%DayLabel");
        _seasonIcon = GetNode<TextureRect>("%SeasonIcon");
        Clock ??= GameClock.Instance;
        if (Clock != null)
        {
            Clock.StepChanged += OnClockChanged;
            Clock.TimeChanged += OnClockChanged;
            Clock.DayStarted += OnClockChanged;
        }
        Refresh();
    }

    public override void _ExitTree()
    {
        if (Clock == null || !IsInstanceValid(Clock)) return;
        Clock.StepChanged -= OnClockChanged;
        Clock.TimeChanged -= OnClockChanged;
        Clock.DayStarted -= OnClockChanged;
    }

    private void OnClockChanged(int _) => Refresh();

    /// <summary>Re-reads the clock and redraws.</summary>
    public void Refresh()
    {
        if (Clock == null)
            return;
        Display(Clock.Phase, Clock.ShownTime, Clock.Season, Clock.DayOfSeason);
    }

    /// <summary>Draws a given state (used by <see cref="Refresh"/> and by tests for every phase).</summary>
    public void Display(DayPhase phase, string time, Season season, int dayOfSeason)
    {
        ShownPhase = phase;
        if (_time != null) _time.Text = time;
        if (_day != null) _day.Text = $"Day {dayOfSeason}";
        if (_seasonIcon != null)
        {
            _seasonIcon.Texture = SpriteLibrary.Get("season_" + season.ToString().ToLowerInvariant());
        }
        QueueRedraw();
    }

    /// <summary>Angle of the middle of a phase's wedge. Dawn rises at the left, midday is at the
    /// top, night is at the bottom (screen angles: 0 = right, growing clockwise).</summary>
    public static float WedgeCenterAngle(DayPhase phase) => Mathf.Pi + ((int)phase + 0.5f) * Mathf.Tau / GameClock.PhaseCount;

    public override void _Draw()
    {
        var center = new Vector2(Size.X / 2f, Radius + 4f);
        float step = Mathf.Tau / GameClock.PhaseCount;

        DrawCircle(center, Radius + 3f, new Color(0.15f, 0.12f, 0.10f, 0.85f));
        for (int i = 0; i < GameClock.PhaseCount; i++)
        {
            float a0 = Mathf.Pi + i * step, a1 = a0 + step;
            var pts = new Vector2[10];
            pts[0] = center;
            for (int k = 0; k <= 8; k++)
            {
                float a = Mathf.Lerp(a0, a1, k / 8f);
                pts[k + 1] = center + Vector2.FromAngle(a) * Radius;
            }
            var col = PhaseColors[i];
            if (i != (int)ShownPhase)
                col = col.Lerp(new Color(0.35f, 0.35f, 0.38f), 0.45f);
            DrawColoredPolygon(pts, col);
        }
        // Thin spokes between phases and a horizon line (day above, night below).
        for (int i = 0; i < GameClock.PhaseCount; i++)
            DrawLine(center, center + Vector2.FromAngle(Mathf.Pi + i * step) * Radius, new Color(0, 0, 0, 0.35f), 1f);
        DrawLine(center + Vector2.Left * Radius, center + Vector2.Right * Radius, new Color(0, 0, 0, 0.6f), 2f);
        DrawArc(center, Radius, 0, Mathf.Tau, 48, new Color(0.15f, 0.12f, 0.10f), 2f);

        var marker = SpriteLibrary.Get(MarkerSprite);
        float iconSize = Radius * 0.62f;
        var pos = center + Vector2.FromAngle(WedgeCenterAngle(ShownPhase)) * Radius * 0.62f;
        DrawTextureRect(marker, new Rect2(pos - Vector2.One * iconSize / 2f, Vector2.One * iconSize), false);
    }
}
