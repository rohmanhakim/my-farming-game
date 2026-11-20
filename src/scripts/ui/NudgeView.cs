using Godot;
using MyFarmingGame.Autoloads;
using MyFarmingGame.Core;

namespace MyFarmingGame.UI;

/// <summary>
/// The guided start's hint chip (Task 1.12): a small, non-blocking picture card at the top of the
/// screen with the current step's icon, a few words and progress (e.g. weeds 2/3). Step 10 shows the
/// free-play goal: "Feed lasts 7 days — earn coins for more". Hidden when the guided start is done.
/// </summary>
public partial class NudgeView : Control
{
    public const string Group = "nudge_view";

    private TextureRect _icon;
    private Label _text;
    private PanelContainer _panel;

    public string Text => _text?.Text ?? "";
    public bool Shown => _panel?.Visible ?? false;

    public static NudgeView Find(SceneTree tree) => tree.GetFirstNodeInGroup(Group) as NudgeView;

    public override void _Ready()
    {
        AddToGroup(Group);
        SetAnchorsPreset(LayoutPreset.FullRect);
        MouseFilter = MouseFilterEnum.Ignore;
        var top = new MarginContainer { MouseFilter = MouseFilterEnum.Ignore };
        top.SetAnchorsPreset(LayoutPreset.CenterTop);
        top.GrowHorizontal = GrowDirection.Both;
        top.AddThemeConstantOverride("margin_top", 12);
        AddChild(top);
        _panel = new PanelContainer { MouseFilter = MouseFilterEnum.Ignore };
        _panel.AddThemeStyleboxOverride("panel", new StyleBoxFlat
        {
            BgColor = new Color(1f, 0.98f, 0.9f, 0.9f), BorderColor = new Color(0.95f, 0.7f, 0.1f),
            BorderWidthLeft = 3, BorderWidthTop = 3, BorderWidthRight = 3, BorderWidthBottom = 3,
            CornerRadiusTopLeft = 14, CornerRadiusTopRight = 14, CornerRadiusBottomLeft = 14, CornerRadiusBottomRight = 14,
            ContentMarginLeft = 12, ContentMarginRight = 16, ContentMarginTop = 6, ContentMarginBottom = 6,
        });
        top.AddChild(_panel);
        var row = new HBoxContainer { MouseFilter = MouseFilterEnum.Ignore };
        row.AddThemeConstantOverride("separation", 10);
        _panel.AddChild(row);
        _icon = new TextureRect { CustomMinimumSize = new Vector2(40, 40), ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize, StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered, MouseFilter = MouseFilterEnum.Ignore };
        row.AddChild(_icon);
        _text = new Label { MouseFilter = MouseFilterEnum.Ignore, SizeFlagsVertical = SizeFlags.ShrinkCenter };
        _text.AddThemeFontSizeOverride("font_size", 22);
        _text.AddThemeColorOverride("font_color", new Color(0.2f, 0.15f, 0.1f));
        row.AddChild(_text);
        _panel.Visible = false;
    }

    /// <summary>Icon and words for a step (pictures first, about 5 words).</summary>
    public static (string Icon, string Text) For(OnboardingStep step, Onboarding ob) => step switch
    {
        OnboardingStep.Arrival => ("icon_talk", "Walk to the front door"),
        OnboardingStep.Garden => ("soil_weeds", $"Pull weeds  {Mathf.Min(ob?.WeedsPulled ?? 0, 3)}/3"),
        OnboardingStep.Plant => ("item_carrot_seeds", $"Plant seeds  {Mathf.Min(ob?.Planted ?? 0, 3)}/3"),
        OnboardingStep.Water => ("tool_watering_can", $"Water the beds  {Mathf.Min(ob?.Watered ?? 0, 3)}/3"),
        OnboardingStep.MeetMarlow => ("npc_marlow", "Visit Mr. Marlow up the lane"),
        OnboardingStep.Forage => ("item_berries", $"Pick berries  {Mathf.Min(Inventory.Instance?.CountAll(ItemIds.Berries) ?? 0, 3)}/3"),
        OnboardingStep.FirstOrder => ("item_berries", "Give Mr. Marlow the berries"),
        OnboardingStep.SecondOrder => ("icon_order_board", "Look at the order board"),
        OnboardingStep.FeedAnimals => ("tool_feed_scoop", "Feed the hens and the cow"),
        OnboardingStep.FreePlay => ("item_chicken_feed", "Feed lasts 7 days — earn coins for more"),
        _ => ("", ""),
    };

    public void ShowStep(OnboardingStep step, Onboarding ob)
    {
        if (_panel == null) return;
        var (icon, text) = For(step, ob);
        _panel.Visible = text != "";
        if (!_panel.Visible) return;
        _icon.Texture = SpriteLibrary.Get(icon);
        _text.Text = text;
    }
}
