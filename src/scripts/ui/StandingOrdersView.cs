using Godot;
using MyFarmingGame.Autoloads;
using MyFarmingGame.Core;

namespace MyFarmingGame.UI;

/// <summary>
/// The daily-order reminder (GDD §4 "subscription orders", owner request 2026-11-21): for each daily
/// standing order Kit is subscribed to, a small picture chip on the HUD (item × count) that gets a ✓
/// once it is delivered today. Hidden when Kit has no daily orders.
/// </summary>
public partial class StandingOrdersView : VBoxContainer
{
    private double _t;

    /// <summary>Chips shown (tests).</summary>
    public int Shown => GetChildCount();

    public override void _Ready()
    {
        MouseFilter = MouseFilterEnum.Ignore;
        Refresh();
    }

    public override void _Process(double delta)
    {
        _t += delta;
        if (_t < 0.5) return;
        _t = 0;
        Refresh();
    }

    public void Refresh()
    {
        foreach (Node c in GetChildren()) { RemoveChild(c); c.QueueFree(); }
        var om = OrderManager.Instance;
        if (om == null) return;
        int today = GameClock.Instance?.Day ?? 1;
        foreach (var o in om.Subscriptions)
        {
            var row = new HBoxContainer { MouseFilter = MouseFilterEnum.Ignore };
            row.AddChild(Icon("icon_daily", 26));
            row.AddChild(Icon(ItemIds.SpriteName(o.ItemId), 30));
            bool done = o.DeliveredOnDay == today;
            var l = new Label { Text = "×" + o.Count + (done ? "  ✓" : ""), MouseFilter = MouseFilterEnum.Ignore };
            l.AddThemeFontSizeOverride("font_size", 20);
            l.AddThemeColorOverride("font_color", done ? new Color(0.6f, 1f, 0.6f) : Colors.White);
            l.AddThemeColorOverride("font_outline_color", new Color(0.1f, 0.08f, 0.05f));
            l.AddThemeConstantOverride("outline_size", 6);
            row.AddChild(l);
            AddChild(row);
        }
    }

    private static TextureRect Icon(string sprite, int size) => new()
    {
        Texture = SpriteLibrary.Get(sprite), CustomMinimumSize = new Vector2(size, size), MouseFilter = MouseFilterEnum.Ignore,
        ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize, StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
    };
}
