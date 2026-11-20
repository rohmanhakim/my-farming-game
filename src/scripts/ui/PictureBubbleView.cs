using System;
using System.Collections.Generic;
using Godot;
using MyFarmingGame.Autoloads;
using MyFarmingGame.Core;

namespace MyFarmingGame.UI;

/// <summary>One picture in a bubble: an icon with a short label ("×3", "56"), optionally faded.</summary>
public readonly record struct BubbleChip(string Sprite, string Label, bool Faded = false);

/// <summary>A bubble button: picture first, a short word second. <see cref="IsCancel"/> buttons
/// are also pressed by B / right-click / Esc.</summary>
public sealed class BubbleButton
{
    public string Icon { get; init; } = "";
    public string Text { get; init; } = "";
    public Action OnPress { get; init; }
    public bool IsCancel { get; init; }

    public static BubbleButton Close(string icon = "icon_check", string text = "OK") => new() { Icon = icon, Text = text, IsCancel = true };
}

/// <summary>
/// The picture bubble (Task 1.6, GDD "picture bubbles"): a speaker portrait, a row of pictures
/// (item × count → coins), one short line, and big buttons. Used for Marlow's conversations and
/// the ledger close-up; later tasks reuse it. While open the clock is held
/// (<see cref="HoldReason"/>, GDD §4: time stops in a conversation) and Main freezes Kit.
/// Gamepad: D-pad/stick moves focus, A presses, B = the cancel button. Mouse: click, right-click
/// = cancel. <see cref="ReadAloudRequested"/> is the read-aloud hook (no voice yet).
/// </summary>
public partial class PictureBubbleView : Control
{
    public const string Group = "picture_bubble";
    public const string HoldReason = "talk";
    public const ulong OpenGuardMsec = 200;

    [Signal] public delegate void OpenChangedEventHandler(bool open);
    [Signal] public delegate void ReadAloudRequestedEventHandler(string line);

    public bool IsOpen { get; private set; }
    public string Line { get; private set; } = "";
    public IReadOnlyList<BubbleChip> Chips { get; private set; } = Array.Empty<BubbleChip>();
    public IReadOnlyList<Button> Buttons => _buttons;
    public string Portrait { get; private set; } = "";

    private readonly List<Button> _buttons = new();
    private readonly List<BubbleButton> _defs = new();
    private Control _panelRoot;
    private TextureRect _portrait;
    private HBoxContainer _chipRow;
    private Label _line;
    private HBoxContainer _buttonRow;
    private ulong _openedAt;

    public static PictureBubbleView Find(SceneTree tree) => tree.GetFirstNodeInGroup(Group) as PictureBubbleView;

    public override void _Ready()
    {
        AddToGroup(Group);
        SetAnchorsPreset(LayoutPreset.FullRect);
        MouseFilter = MouseFilterEnum.Ignore;

        // Bottom-centre, above the tool belt: a full-rect column pushed to the bottom.
        var column = new VBoxContainer { MouseFilter = MouseFilterEnum.Ignore, Alignment = BoxContainer.AlignmentMode.End };
        column.SetAnchorsPreset(LayoutPreset.FullRect);
        AddChild(column);
        _panelRoot = new MarginContainer { MouseFilter = MouseFilterEnum.Ignore, SizeFlagsHorizontal = SizeFlags.ShrinkCenter };
        _panelRoot.AddThemeConstantOverride("margin_bottom", 110);
        _panelRoot.CustomMinimumSize = new Vector2(660, 0);
        column.AddChild(_panelRoot);

        var panel = new PanelContainer { CustomMinimumSize = new Vector2(660, 0) };
        panel.AddThemeStyleboxOverride("panel", new StyleBoxFlat
        {
            BgColor = new Color(1f, 0.98f, 0.92f, 0.97f), BorderColor = new Color(0.25f, 0.18f, 0.12f),
            BorderWidthLeft = 3, BorderWidthTop = 3, BorderWidthRight = 3, BorderWidthBottom = 3,
            CornerRadiusTopLeft = 18, CornerRadiusTopRight = 18, CornerRadiusBottomLeft = 18, CornerRadiusBottomRight = 18,
            ContentMarginLeft = 16, ContentMarginRight = 16, ContentMarginTop = 12, ContentMarginBottom = 12,
        });
        _panelRoot.AddChild(panel);
        var row = new HBoxContainer();
        row.AddThemeConstantOverride("separation", 16);
        panel.AddChild(row);
        _portrait = new TextureRect
        {
            CustomMinimumSize = new Vector2(88, 88), ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
            StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered, SizeFlagsVertical = SizeFlags.ShrinkCenter,
        };
        row.AddChild(_portrait);
        var col = new VBoxContainer { SizeFlagsHorizontal = SizeFlags.ExpandFill };
        col.AddThemeConstantOverride("separation", 10);
        row.AddChild(col);
        _chipRow = new HBoxContainer { Alignment = BoxContainer.AlignmentMode.Center };
        _chipRow.AddThemeConstantOverride("separation", 14);
        col.AddChild(_chipRow);
        _line = new Label
        {
            AutowrapMode = TextServer.AutowrapMode.WordSmart, HorizontalAlignment = HorizontalAlignment.Center,
            LabelSettings = new LabelSettings { FontSize = 20, FontColor = new Color(0.2f, 0.15f, 0.1f) },
        };
        col.AddChild(_line);
        _buttonRow = new HBoxContainer { Alignment = BoxContainer.AlignmentMode.Center };
        _buttonRow.AddThemeConstantOverride("separation", 18);
        col.AddChild(_buttonRow);
        _panelRoot.Visible = false;
    }

    public override void _ExitTree()
    {
        if (IsOpen) GameClock.Instance?.Release(HoldReason);
    }

    /// <summary>True when nothing else is in charge of the screen.</summary>
    public bool CanOpen => !GetTree().Paused && (GameClock.Instance == null || (!GameClock.Instance.IsAsking && !GameClock.Instance.IsSleeping));

    /// <summary>Shows (or replaces) the bubble. Returns false if it can't open now.</summary>
    public bool Show(string portrait, string line, IList<BubbleChip> chips, IList<BubbleButton> buttons)
    {
        if (!IsOpen && !CanOpen) return false;
        Portrait = portrait ?? "";
        _portrait.Texture = string.IsNullOrEmpty(portrait) ? null : SpriteLibrary.Get(portrait);
        _portrait.Visible = _portrait.Texture != null;
        Line = line ?? "";
        _line.Text = Line;
        _line.Visible = Line != "";
        Chips = new List<BubbleChip>(chips ?? Array.Empty<BubbleChip>());
        foreach (Node c in _chipRow.GetChildren()) { _chipRow.RemoveChild(c); c.QueueFree(); }
        foreach (var chip in Chips) _chipRow.AddChild(MakeChip(chip));
        _chipRow.Visible = Chips.Count > 0;

        foreach (var b in _buttons) { _buttonRow.RemoveChild(b); b.QueueFree(); }
        _buttons.Clear();
        _defs.Clear();
        foreach (var def in buttons ?? Array.Empty<BubbleButton>())
        {
            var btn = new Button
            {
                Text = def.Text, CustomMinimumSize = new Vector2(150, 56),
                Icon = string.IsNullOrEmpty(def.Icon) ? null : SpriteLibrary.Get(def.Icon), ExpandIcon = true,
                FocusMode = FocusModeEnum.All,
            };
            btn.AddThemeFontSizeOverride("font_size", 20);
            btn.AddThemeConstantOverride("icon_max_width", 36);
            var captured = def;
            btn.Pressed += () => Press(captured, false);
            _buttonRow.AddChild(btn);
            _buttons.Add(btn);
            _defs.Add(def);
        }
        if (_defs.Count == 0)
        {
            var close = BubbleButton.Close();
            return Show(portrait, line, chips, new[] { close });
        }

        bool wasOpen = IsOpen;
        IsOpen = true;
        _panelRoot.Visible = true;
        _openedAt = Time.GetTicksMsec();
        GameClock.Instance?.Hold(HoldReason);
        _buttons[0].CallDeferred(Control.MethodName.GrabFocus);
        if (!wasOpen) EmitSignal(SignalName.OpenChanged, true);
        EmitSignal(SignalName.ReadAloudRequested, Line);
        return true;
    }

    private static Control MakeChip(BubbleChip chip)
    {
        var box = new VBoxContainer { Alignment = BoxContainer.AlignmentMode.Center };
        if (!string.IsNullOrEmpty(chip.Sprite))
            box.AddChild(new TextureRect
            {
                Texture = SpriteLibrary.Get(chip.Sprite), CustomMinimumSize = new Vector2(56, 56),
                ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize, StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
                Modulate = chip.Faded ? new Color(1, 1, 1, 0.45f) : Colors.White,
            });
        if (!string.IsNullOrEmpty(chip.Label))
            box.AddChild(new Label
            {
                Text = chip.Label, HorizontalAlignment = HorizontalAlignment.Center,
                LabelSettings = new LabelSettings { FontSize = chip.Sprite == "" ? 34 : 22, FontColor = new Color(0.2f, 0.15f, 0.1f) },
            });
        return box;
    }

    /// <summary>Presses button <paramref name="index"/> (tests; skips the open guard).</summary>
    public void PressButton(int index)
    {
        if (index >= 0 && index < _defs.Count) Press(_defs[index], true);
    }

    /// <summary>Presses the cancel button (B / right-click / Esc), or just closes.</summary>
    public void PressCancel()
    {
        var def = _defs.Find(d => d.IsCancel);
        if (def != null) Press(def, true); else Close();
    }

    private void Press(BubbleButton def, bool force)
    {
        if (!IsOpen) return;
        if (!force && !def.IsCancel && Time.GetTicksMsec() - _openedAt < OpenGuardMsec) return;
        Close();
        def.OnPress?.Invoke();
    }

    public void Close()
    {
        if (!IsOpen) return;
        IsOpen = false;
        _panelRoot.Visible = false;
        foreach (var b in _buttons) b.ReleaseFocus();
        GameClock.Instance?.Release(HoldReason);
        EmitSignal(SignalName.OpenChanged, false);
    }

    public override void _Input(InputEvent e)
    {
        if (IsOpen && !e.IsEcho() && (e.IsActionPressed("cancel") || e.IsActionPressed("ui_cancel")))
        {
            PressCancel();
            GetViewport().SetInputAsHandled();
        }
    }
}
