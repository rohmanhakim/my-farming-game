using Godot;
using MyFarmingGame.Autoloads;

namespace MyFarmingGame.UI;

/// <summary>
/// The world's light across the day (Task 1.11; 3D since Task 1.19). Per day phase from
/// <see cref="GameClock"/> it sets the sun (<see cref="Sun"/>: colour, brightness, height in the sky)
/// and the sky/ambient light of <see cref="Environment"/>, blended over <see cref="FadeSeconds"/>
/// when the phase changes; windows (nodes in group <see cref="WindowGroup"/>) glow from evening on.
/// Night is dim but readable: the light never drops below ≈ 50 % (<see cref="MinBrightness"/>).
/// Only the world is lit this way, never the HUD. Lives in Main/World/Effects.
/// </summary>
public partial class DayNightView : Node
{
    [Export] public float FadeSeconds { get; set; } = 1.5f;
    [Export] public NodePath SunPath { get; set; }
    [Export] public NodePath EnvironmentPath { get; set; }

    /// <summary>Nodes (MeshInstance3D with their own material) that glow at night: lit windows.</summary>
    public const string WindowGroup = "night_windows";

    /// <summary>Night never gets darker than this share of midday light (2D rule "≥ 50 % bright").</summary>
    public const float MinBrightness = 0.5f;

    public DirectionalLight3D Sun { get; private set; }
    public WorldEnvironment Environment { get; private set; }

    private GameClock _clock;
    private Tween _tween;
    private float _blend = 1f;
    private Light _from, _to;

    /// <summary>One phase's light.</summary>
    public readonly record struct Light(Color Tint, float SunEnergy, float SunPitch, float Ambient, Color Sky, float WindowGlow);

    /// <summary>The colour for <paramref name="phase"/> (warm dawn, white day, golden evening, blue night).
    /// Applied to the sun and the ambient light.</summary>
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

    /// <summary>Full light settings for a phase (sun lower and dimmer towards evening; no sun at night,
    /// a soft blue "moonlight" instead).</summary>
    public static Light LightFor(DayPhase phase)
    {
        Color tint = TintFor(phase);
        return phase switch
        {
            DayPhase.Dawn => new Light(tint, 0.75f, -20f, 0.45f, new Color(0.95f, 0.78f, 0.70f), 0.3f),
            DayPhase.Morning => new Light(tint, 1.0f, -40f, 0.42f, new Color(0.68f, 0.84f, 0.96f), 0f),
            DayPhase.Midday => new Light(tint, 1.1f, -60f, 0.40f, new Color(0.62f, 0.82f, 0.95f), 0f),
            DayPhase.Afternoon => new Light(tint, 1.0f, -45f, 0.42f, new Color(0.66f, 0.82f, 0.93f), 0f),
            DayPhase.Evening => new Light(tint, 0.8f, -22f, 0.45f, new Color(0.98f, 0.72f, 0.52f), 0.7f),
            DayPhase.Dusk => new Light(tint, 0.6f, -12f, 0.50f, new Color(0.45f, 0.42f, 0.66f), 1f),
            _ => new Light(tint, 0.6f, -50f, 0.8f, new Color(0.16f, 0.18f, 0.32f), 1f),
        };
    }

    /// <summary>Overall brightness of a phase's light relative to midday (tests; must stay ≥ <see cref="MinBrightness"/>).</summary>
    public static float BrightnessOf(Light l)
    {
        var mid = LightFor(DayPhase.Midday);
        float lum(Color c) => 0.3f * c.R + 0.59f * c.G + 0.11f * c.B;
        float now = l.SunEnergy * lum(l.Tint) + l.Ambient * lum(l.Tint);
        float day = mid.SunEnergy + mid.Ambient;
        return now / day;
    }

    /// <summary>The light being blended to (tests).</summary>
    public Light Target => _to;

    /// <summary>The colour being faded to (tests; same as <see cref="Light.Tint"/> of <see cref="Target"/>).</summary>
    public Color TargetTint => _to.Tint;

    /// <summary>True once the blend to <see cref="Target"/> is finished.</summary>
    public bool Settled => _blend >= 1f;

    public override void _Ready()
    {
        Sun = GetNodeOrNull<DirectionalLight3D>(SunPath);
        Environment = GetNodeOrNull<WorldEnvironment>(EnvironmentPath);
        _clock = GameClock.Instance;
        _to = LightFor(_clock?.Phase ?? DayPhase.Midday);
        _from = _to;
        Apply(1f);
        if (_clock != null) _clock.StepChanged += OnStep;
    }

    public override void _ExitTree()
    {
        if (_clock != null && IsInstanceValid(_clock)) _clock.StepChanged -= OnStep;
    }

    private void OnStep(int phase)
    {
        _from = Current;
        _to = LightFor((DayPhase)phase);
        _tween?.Kill();
        if (!IsInsideTree() || FadeSeconds <= 0) { _blend = 1f; Apply(1f); return; }
        _blend = 0f;
        _tween = CreateTween();
        _tween.TweenMethod(Callable.From<float>(Apply), 0f, 1f, FadeSeconds);
    }

    /// <summary>The light shown right now.</summary>
    public Light Current { get; private set; }

    private void Apply(float t)
    {
        _blend = t;
        Current = new Light(
            _from.Tint.Lerp(_to.Tint, t), Mathf.Lerp(_from.SunEnergy, _to.SunEnergy, t),
            Mathf.Lerp(_from.SunPitch, _to.SunPitch, t), Mathf.Lerp(_from.Ambient, _to.Ambient, t),
            _from.Sky.Lerp(_to.Sky, t), Mathf.Lerp(_from.WindowGlow, _to.WindowGlow, t));
        if (Sun != null)
        {
            Sun.LightColor = Current.Tint;
            Sun.LightEnergy = Current.SunEnergy;
            Sun.RotationDegrees = new Vector3(Current.SunPitch, Sun.RotationDegrees.Y, 0);
        }
        var env = Environment?.Environment;
        if (env != null)
        {
            env.AmbientLightColor = Current.Tint;
            env.AmbientLightEnergy = Current.Ambient;
            env.BackgroundColor = Current.Sky;
        }
        if (IsInsideTree())
            foreach (Node n in GetTree().GetNodesInGroup(WindowGroup))
                if (n is GeometryInstance3D g && g.MaterialOverride is StandardMaterial3D m)
                {
                    m.EmissionEnabled = Current.WindowGlow > 0.01f;
                    m.EmissionEnergyMultiplier = Current.WindowGlow * 1.5f;
                }
    }
}
