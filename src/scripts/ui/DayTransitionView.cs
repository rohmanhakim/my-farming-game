using System.Threading.Tasks;
using Godot;
using MyFarmingGame.Autoloads;
using MyFarmingGame.Core;

namespace MyFarmingGame.UI;

/// <summary>
/// Full-screen fade used for sleeping (Task 1.3): fades to a night colour with the sleep icon,
/// then shows the new day (season icon + "Day N") and fades back in. Lives on the Transition
/// layer (process mode Always). Durations are exports for playtest tuning.
/// </summary>
public partial class DayTransitionView : Control
{
    [Export] public float FadeSeconds { get; set; } = 0.8f;
    [Export] public float HoldSeconds { get; set; } = 1.2f;

    private ColorRect _shade;
    private TextureRect _icon;
    private Label _label;

    public bool IsCovering => Visible && _shade.Color.A > 0.99f;

    public override void _Ready()
    {
        _shade = GetNode<ColorRect>("Shade");
        _icon = GetNode<TextureRect>("%Icon");
        _label = GetNode<Label>("%DayText");
        Visible = false;
    }

    /// <summary>Fades the screen out to night with the sleep icon.</summary>
    public async Task FadeOut()
    {
        Visible = true;
        _icon.Texture = SpriteLibrary.Get("icon_sleep");
        _label.Text = "";
        _shade.Color = _shade.Color with { A = 0 };
        _icon.Modulate = _label.Modulate = Colors.Transparent;
        var t = CreateTween().SetParallel();
        t.TweenProperty(_shade, "color:a", 1.0, FadeSeconds);
        t.TweenProperty(_icon, "modulate:a", 1.0, FadeSeconds);
        await ToSignal(t, Tween.SignalName.Finished);
    }

    /// <summary>Seconds for the quick door fade (Phase-002 Task 2.2).</summary>
    [Export] public float DoorFadeSeconds { get; set; } = 0.15f;

    /// <summary>A quick plain fade to black for walking through a door (no icon, no text).</summary>
    public async Task Cover()
    {
        Visible = true;
        _label.Text = "";
        _icon.Modulate = _label.Modulate = Colors.Transparent;
        var t = CreateTween();
        t.TweenProperty(_shade, "color:a", 1.0, DoorFadeSeconds);
        await ToSignal(t, Tween.SignalName.Finished);
    }

    /// <summary>Fades the quick door cover away again.</summary>
    public async Task Uncover()
    {
        var t = CreateTween();
        t.TweenProperty(_shade, "color:a", 0.0, DoorFadeSeconds);
        await ToSignal(t, Tween.SignalName.Finished);
        Visible = false;
    }

    /// <summary>Shows the new day, holds it briefly, then fades the game back in.</summary>
    public async Task ShowDayAndFadeIn(Season season, int dayOfSeason)
    {
        _icon.Texture = SpriteLibrary.Get("season_" + season.ToString().ToLowerInvariant());
        _label.Text = $"Day {dayOfSeason}";
        _label.Modulate = Colors.White;
        await ToSignal(GetTree().CreateTimer(HoldSeconds, processAlways: true), SceneTreeTimer.SignalName.Timeout);
        var t = CreateTween().SetParallel();
        t.TweenProperty(_shade, "color:a", 0.0, FadeSeconds);
        t.TweenProperty(_icon, "modulate:a", 0.0, FadeSeconds);
        t.TweenProperty(_label, "modulate:a", 0.0, FadeSeconds);
        await ToSignal(t, Tween.SignalName.Finished);
        Visible = false;
    }
}
