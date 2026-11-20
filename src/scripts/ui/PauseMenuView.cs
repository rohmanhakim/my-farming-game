using Godot;
using MyFarmingGame.Autoloads;
using MyFarmingGame.Core;

namespace MyFarmingGame.UI;

/// <summary>
/// The pause menu (Task 1.11, controls.md §2/§6): Start / Esc / the on-screen pause button open it and
/// pause the whole game (<c>GetTree().Paused</c>, so the clock stops). It shows the calendar
/// (season icon + "Day N") and the same day/night dial as the HUD, and the buttons Resume, Save,
/// Settings and Quit. Settings: volume (− / +), nudges, read aloud (placeholder), break reminder
/// (off by default). B / Esc / Start close it (Settings first goes back to the main page).
/// The opener (<see cref="PauseButton"/>, in the HUD) runs while the game runs; this view lives on
/// the Pause layer (process mode WhenPaused).
/// </summary>
public partial class PauseMenuView : Control
{
    public const string Group = "pause_menu";

    [Signal] public delegate void OpenChangedEventHandler(bool open);

    public bool IsOpen { get; private set; }
    public bool SettingsOpen => _settings?.Visible ?? false;

    public Button ResumeButton { get; private set; }
    public Button SaveButton { get; private set; }
    public Button SettingsButton { get; private set; }
    public Button QuitButton { get; private set; }
    public Button VolumeDown { get; private set; }
    public Button VolumeUp { get; private set; }
    public CheckButton NudgesToggle { get; private set; }
    public CheckButton ReadAloudToggle { get; private set; }
    public CheckButton BreakToggle { get; private set; }
    public Button BackButton { get; private set; }
    public Label SavedLabel { get; private set; }
    public string DayText => _day?.Text ?? "";

    /// <summary>Tests: when false Quit only closes the menu instead of quitting the app.</summary>
    public bool AllowQuit { get; set; } = true;

    private Control _main;
    private Control _settings;
    private Label _day;
    private TextureRect _season;
    private ProgressBar _volume;

    public static PauseMenuView Find(SceneTree tree) => tree.GetFirstNodeInGroup(Group) as PauseMenuView;

    public override void _Ready()
    {
        AddToGroup(Group);
        SetAnchorsPreset(LayoutPreset.FullRect);
        var dim = new ColorRect { Color = new Color(0.05f, 0.05f, 0.12f, 0.6f), MouseFilter = MouseFilterEnum.Stop };
        dim.SetAnchorsPreset(LayoutPreset.FullRect);
        AddChild(dim);
        var center = new CenterContainer();
        center.SetAnchorsPreset(LayoutPreset.FullRect);
        AddChild(center);
        var panel = new PanelContainer();
        panel.AddThemeStyleboxOverride("panel", new StyleBoxFlat
        {
            BgColor = new Color(0.13f, 0.15f, 0.3f, 0.96f), BorderColor = new Color(0.95f, 0.92f, 0.75f),
            BorderWidthLeft = 3, BorderWidthTop = 3, BorderWidthRight = 3, BorderWidthBottom = 3,
            CornerRadiusTopLeft = 16, CornerRadiusTopRight = 16, CornerRadiusBottomLeft = 16, CornerRadiusBottomRight = 16,
            ContentMarginLeft = 28, ContentMarginRight = 28, ContentMarginTop = 20, ContentMarginBottom = 20,
        });
        center.AddChild(panel);
        var col = new VBoxContainer();
        col.AddThemeConstantOverride("separation", 14);
        panel.AddChild(col);

        // Calendar: season icon + Day N, and the day/night dial (same as the HUD).
        var cal = new HBoxContainer { Alignment = BoxContainer.AlignmentMode.Center };
        cal.AddThemeConstantOverride("separation", 16);
        _season = new TextureRect { CustomMinimumSize = new Vector2(48, 48), ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize, StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered, SizeFlagsVertical = SizeFlags.ShrinkCenter };
        cal.AddChild(_season);
        _day = Lbl("", 28);
        cal.AddChild(_day);
        var dial = ResourceLoader.Load<PackedScene>("res://scenes/ui/ClockDial.tscn").Instantiate<Control>();
        var dialBox = new Control { CustomMinimumSize = new Vector2(150, 196) }; // the dial scene includes its own readout and day line
        dialBox.AddChild(dial);
        dial.SetAnchorsPreset(LayoutPreset.TopLeft);
        dial.Position = Vector2.Zero;
        cal.AddChild(dialBox);
        col.AddChild(cal);

        _main = new VBoxContainer();
        _main.AddThemeConstantOverride("separation", 10);
        col.AddChild(_main);
        ResumeButton = Big("Resume", "icon_check", Resume);
        SaveButton = Big("Save", "icon_ledger", SaveNow);
        SettingsButton = Big("Settings", "icon_clock", () => ShowSettings(true));
        QuitButton = Big("Quit", "icon_sleep", Quit);
        foreach (var b in new[] { ResumeButton, SaveButton, SettingsButton, QuitButton }) _main.AddChild(b);
        SavedLabel = Lbl("Saved ✓", 20);
        SavedLabel.Visible = false;
        _main.AddChild(SavedLabel);

        _settings = new VBoxContainer { Visible = false };
        _settings.AddThemeConstantOverride("separation", 10);
        col.AddChild(_settings);
        var vol = new HBoxContainer { Alignment = BoxContainer.AlignmentMode.Center };
        vol.AddThemeConstantOverride("separation", 10);
        vol.AddChild(Lbl("🔊", 26));
        VolumeDown = Small("−", () => { GameSettings.SetVolume(GameSettings.Volume - GameSettings.VolumeStep); SaveGame.Instance?.SaveSettings(); });
        _volume = new ProgressBar { MinValue = 0, MaxValue = 1, Step = 0.01, CustomMinimumSize = new Vector2(200, 28), ShowPercentage = false, SizeFlagsVertical = SizeFlags.ShrinkCenter };
        VolumeUp = Small("+", () => { GameSettings.SetVolume(GameSettings.Volume + GameSettings.VolumeStep); SaveGame.Instance?.SaveSettings(); });
        vol.AddChild(VolumeDown);
        vol.AddChild(_volume);
        vol.AddChild(VolumeUp);
        _settings.AddChild(vol);
        NudgesToggle = Toggle("Hints", nameof(GameSettings.Nudges));
        ReadAloudToggle = Toggle("Read aloud", nameof(GameSettings.ReadAloud));
        BreakToggle = Toggle("Break reminder", nameof(GameSettings.BreakReminder));
        BackButton = Big("Back", "", () => ShowSettings(false));
        _settings.AddChild(BackButton);

        GameSettings.Changed += RefreshSettings;
        Visible = false;
    }

    public override void _ExitTree()
    {
        GameSettings.Changed -= RefreshSettings;
        if (IsOpen && IsInsideTree()) GetTree().Paused = false;
    }

    private Button Big(string text, string icon, System.Action onPress)
    {
        var b = new Button { Text = text, CustomMinimumSize = new Vector2(300, 56), FocusMode = FocusModeEnum.All };
        if (!string.IsNullOrEmpty(icon)) { b.Icon = SpriteLibrary.Get(icon); b.ExpandIcon = true; b.AddThemeConstantOverride("icon_max_width", 36); }
        b.AddThemeFontSizeOverride("font_size", 24);
        b.Pressed += () => onPress();
        return b;
    }

    private Button Small(string text, System.Action onPress)
    {
        var b = new Button { Text = text, CustomMinimumSize = new Vector2(56, 52), FocusMode = FocusModeEnum.All };
        b.AddThemeFontSizeOverride("font_size", 28);
        b.Pressed += () => onPress();
        return b;
    }

    private CheckButton Toggle(string text, string key)
    {
        var t = new CheckButton { Text = text, CustomMinimumSize = new Vector2(300, 52), FocusMode = FocusModeEnum.All };
        t.AddThemeFontSizeOverride("font_size", 22);
        t.Toggled += on => { GameSettings.Set(key, on); SaveGame.Instance?.SaveSettings(); };
        _settings.AddChild(t);
        return t;
    }

    private static Label Lbl(string text, int size)
    {
        var l = new Label { Text = text, HorizontalAlignment = HorizontalAlignment.Center, SizeFlagsVertical = SizeFlags.ShrinkCenter };
        l.AddThemeFontSizeOverride("font_size", size);
        l.AddThemeColorOverride("font_color", new Color(0.97f, 0.95f, 0.85f));
        return l;
    }

    private void RefreshSettings()
    {
        if (_volume == null) return;
        _volume.Value = GameSettings.Volume;
        NudgesToggle.SetPressedNoSignal(GameSettings.Nudges);
        ReadAloudToggle.SetPressedNoSignal(GameSettings.ReadAloud);
        BreakToggle.SetPressedNoSignal(GameSettings.BreakReminder);
    }

    /// <summary>True when the game can be paused now (not during the night or the sleep question).</summary>
    public bool CanOpen => !IsOpen && (GameClock.Instance == null || (!GameClock.Instance.IsSleeping && !GameClock.Instance.IsAsking));

    public void Open()
    {
        if (!CanOpen) return;
        IsOpen = true;
        var clock = GameClock.Instance;
        _day.Text = clock != null ? $"Day {clock.DayOfSeason}" : "";
        _season.Texture = SpriteLibrary.Get("season_" + (clock?.Season ?? Season.Spring).ToString().ToLowerInvariant());
        SavedLabel.Visible = false;
        ShowSettings(false);
        RefreshSettings();
        Visible = true;
        GetTree().Paused = true;
        CallDeferred(MethodName.FocusFirst);
        EmitSignal(SignalName.OpenChanged, true);
    }

    private void FocusFirst()
    {
        var b = SettingsOpen ? (Control)VolumeUp : ResumeButton;
        if (IsOpen && b.IsInsideTree()) b.GrabFocus();
    }

    public void Resume()
    {
        if (!IsOpen) return;
        IsOpen = false;
        Visible = false;
        GetTree().Paused = false;
        EmitSignal(SignalName.OpenChanged, false);
    }

    private void ShowSettings(bool on)
    {
        _settings.Visible = on;
        _main.Visible = !on;
        CallDeferred(MethodName.FocusFirst);
    }

    /// <summary>Saves now (manual save, Task 1.10) and shows "Saved ✓".</summary>
    public void SaveNow()
    {
        bool ok = SaveGame.Instance?.Save() ?? false;
        SavedLabel.Text = ok ? "Saved ✓" : "Not saved";
        SavedLabel.Visible = true;
    }

    private void Quit()
    {
        SaveGame.Instance?.Save();
        Resume();
        if (AllowQuit) GetTree().Quit();
    }

    public override void _Input(InputEvent e)
    {
        if (!IsOpen || e.IsEcho()) return;
        if (e.IsActionPressed("pause") || e.IsActionPressed("cancel") || e.IsActionPressed("ui_cancel"))
        {
            if (SettingsOpen && !e.IsActionPressed("pause")) ShowSettings(false);
            else Resume();
            GetViewport().SetInputAsHandled();
        }
    }
}
