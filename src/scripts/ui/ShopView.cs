using System;
using System.Collections.Generic;
using Godot;
using MyFarmingGame.Autoloads;
using MyFarmingGame.Core;

namespace MyFarmingGame.UI;

/// <summary>One thing for sale (Task 1.8): picture × count and a price in coins. <see cref="Blocked"/>
/// returns a picture explaining why it can't be bought right now (e.g. "coop is full"), or null.</summary>
public sealed class ShopEntry
{
    public string Id { get; init; } = "";
    public string Sprite { get; init; } = "";
    public int Count { get; init; } = 1;
    public int Price { get; init; }
    public Func<string> Blocked { get; init; }
    public Action OnBuy { get; init; }
}

/// <summary>
/// The buying screen (Task 1.8) used by the seed-shed counter and the phone at home: one card per
/// entry (picture, ×count, coin price) with a big Buy button. Buying spends coins first and can never
/// go below zero; a blocked entry shows its reason picture instead of buying, and an unaffordable
/// one shakes the coin. While open the clock is held (<see cref="HoldReason"/>). B / right-click /
/// Esc / Close leave.
/// </summary>
public partial class ShopView : Control
{
    public const string Group = "shop_view";
    public const string HoldReason = "shop";

    [Signal] public delegate void OpenChangedEventHandler(bool open);
    /// <summary>A purchase happened. Arguments: entry id, price.</summary>
    [Signal] public delegate void BoughtEventHandler(string id, int price);

    public bool IsOpen { get; private set; }
    public string Title { get; private set; } = "";
    public IReadOnlyList<Button> BuyButtons => _buy;
    public Button CloseButton { get; private set; }
    /// <summary>What the last refused buy showed: "" (bought), "coin" (not enough coins) or the block picture.</summary>
    public string LastRefusal { get; private set; } = "";

    private readonly List<ShopEntry> _entries = new();
    private readonly List<Button> _buy = new();
    private TextureRect _titleIcon;
    private Label _note;
    private HBoxContainer _row;
    private Label _coins;

    public static ShopView Find(SceneTree tree) => tree.GetFirstNodeInGroup(Group) as ShopView;

    public override void _Ready()
    {
        AddToGroup(Group);
        SetAnchorsPreset(LayoutPreset.FullRect);
        MouseFilter = MouseFilterEnum.Ignore;
        var dim = new ColorRect { Color = new Color(0, 0, 0, 0.35f), MouseFilter = MouseFilterEnum.Stop };
        dim.SetAnchorsPreset(LayoutPreset.FullRect);
        AddChild(dim);
        var center = new CenterContainer { MouseFilter = MouseFilterEnum.Ignore };
        center.SetAnchorsPreset(LayoutPreset.FullRect);
        AddChild(center);
        var panel = new PanelContainer();
        panel.AddThemeStyleboxOverride("panel", new StyleBoxFlat
        {
            BgColor = new Color(0.96f, 0.92f, 0.8f), BorderColor = new Color(0.35f, 0.26f, 0.18f),
            BorderWidthLeft = 4, BorderWidthTop = 4, BorderWidthRight = 4, BorderWidthBottom = 4,
            CornerRadiusTopLeft = 14, CornerRadiusTopRight = 14, CornerRadiusBottomLeft = 14, CornerRadiusBottomRight = 14,
            ContentMarginLeft = 20, ContentMarginRight = 20, ContentMarginTop = 14, ContentMarginBottom = 14,
        });
        center.AddChild(panel);
        var col = new VBoxContainer();
        col.AddThemeConstantOverride("separation", 12);
        panel.AddChild(col);
        var top = new HBoxContainer { Alignment = BoxContainer.AlignmentMode.Center };
        top.AddThemeConstantOverride("separation", 24);
        _titleIcon = new TextureRect { CustomMinimumSize = new Vector2(56, 56), ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize, StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered };
        top.AddChild(_titleIcon);
        top.AddChild(new TextureRect { Texture = SpriteLibrary.Get("coin"), CustomMinimumSize = new Vector2(32, 32), ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize, StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered, SizeFlagsVertical = SizeFlags.ShrinkCenter });
        _coins = Lbl("", 24);
        top.AddChild(_coins);
        col.AddChild(top);
        _note = Lbl("", 18);
        _note.AutowrapMode = TextServer.AutowrapMode.WordSmart;
        col.AddChild(_note);
        _row = new HBoxContainer { Alignment = BoxContainer.AlignmentMode.Center };
        _row.AddThemeConstantOverride("separation", 14);
        col.AddChild(_row);
        CloseButton = new Button { Text = "Close", CustomMinimumSize = new Vector2(140, 52), SizeFlagsHorizontal = SizeFlags.ShrinkCenter };
        CloseButton.AddThemeFontSizeOverride("font_size", 20);
        CloseButton.Pressed += Close;
        col.AddChild(CloseButton);
        Visible = false;
        if (Wallet.Instance != null) Wallet.Instance.CoinsChanged += OnCoins;
    }

    public override void _ExitTree()
    {
        if (IsOpen) GameClock.Instance?.Release(HoldReason);
        if (Wallet.Instance != null && IsInstanceValid(Wallet.Instance)) Wallet.Instance.CoinsChanged -= OnCoins;
    }

    private void OnCoins(int c) { if (_coins != null) _coins.Text = c.ToString("N0", System.Globalization.CultureInfo.InvariantCulture); }

    public bool CanOpen => !GetTree().Paused && (GameClock.Instance == null || GameClock.Instance.IsPlayable);

    /// <summary>Opens the screen with <paramref name="entries"/>. <paramref name="note"/> is one short line.</summary>
    public bool Open(string titleSprite, IList<ShopEntry> entries, string note = "")
    {
        if (IsOpen || !CanOpen) return false;
        Title = titleSprite;
        _titleIcon.Texture = SpriteLibrary.Get(titleSprite);
        _note.Text = note ?? "";
        _note.Visible = _note.Text != "";
        _entries.Clear();
        _entries.AddRange(entries);
        LastRefusal = "";
        OnCoins(Wallet.Instance?.Coins ?? 0);
        Rebuild();
        IsOpen = true;
        Visible = true;
        GameClock.Instance?.Hold(HoldReason);
        CallDeferred(MethodName.FocusFirst);
        EmitSignal(SignalName.OpenChanged, true);
        return true;
    }

    private void FocusFirst()
    {
        if (!IsOpen) return;
        var b = _buy.Count > 0 ? _buy[0] : CloseButton;
        if (b.IsInsideTree()) b.GrabFocus();
    }

    public void Close()
    {
        if (!IsOpen) return;
        IsOpen = false;
        Visible = false;
        GameClock.Instance?.Release(HoldReason);
        EmitSignal(SignalName.OpenChanged, false);
    }

    private void Rebuild()
    {
        foreach (Node c in _row.GetChildren()) { _row.RemoveChild(c); c.QueueFree(); }
        _buy.Clear();
        for (int i = 0; i < _entries.Count; i++)
        {
            var e = _entries[i];
            var card = new VBoxContainer { Alignment = BoxContainer.AlignmentMode.Center, CustomMinimumSize = new Vector2(140, 0) };
            card.AddChild(new TextureRect
            {
                Texture = SpriteLibrary.Get(e.Sprite), CustomMinimumSize = new Vector2(64, 64), SizeFlagsHorizontal = SizeFlags.ShrinkCenter,
                ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize, StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
            });
            card.AddChild(Lbl("×" + e.Count, 22));
            string block = e.Blocked?.Invoke();
            var buy = new Button
            {
                Text = e.Price.ToString(), Icon = SpriteLibrary.Get(block ?? "coin"), ExpandIcon = true,
                CustomMinimumSize = new Vector2(130, 56), FocusMode = FocusModeEnum.All,
                TooltipText = $"{ItemIds.DisplayName(e.Id)} ×{e.Count}: {e.Price} coins",
            };
            buy.AddThemeFontSizeOverride("font_size", 22);
            buy.AddThemeConstantOverride("icon_max_width", 32);
            if (block != null) buy.Modulate = new Color(1, 1, 1, 0.6f);
            int index = i;
            buy.Pressed += () => Buy(index);
            card.AddChild(buy);
            _row.AddChild(card);
            _buy.Add(buy);
        }
    }

    /// <summary>Buys entry <paramref name="index"/>. Returns true when it was bought.</summary>
    public bool Buy(int index)
    {
        if (!IsOpen || index < 0 || index >= _entries.Count) return false;
        var e = _entries[index];
        string block = e.Blocked?.Invoke();
        if (block != null) { Refuse(index, block); return false; }
        if (Wallet.Instance == null || !Wallet.Instance.Spend(e.Price)) { Refuse(index, "coin"); return false; }
        e.OnBuy?.Invoke();
        LastRefusal = "";
        EmitSignal(SignalName.Bought, e.Id, e.Price);
        int focus = index;
        Rebuild();
        if (focus < _buy.Count) _buy[focus].GrabFocus();
        return true;
    }

    private void Refuse(int index, string picture)
    {
        LastRefusal = picture;
        if (index >= _buy.Count) return;
        var b = _buy[index];
        b.Icon = SpriteLibrary.Get(picture);
        var t = CreateTween();
        var x = b.Position.X;
        t.TweenProperty(b, "position:x", x + 8, 0.05);
        t.TweenProperty(b, "position:x", x - 8, 0.08);
        t.TweenProperty(b, "position:x", x, 0.05);
    }

    private static Label Lbl(string text, int size)
    {
        var l = new Label { Text = text, HorizontalAlignment = HorizontalAlignment.Center };
        l.AddThemeFontSizeOverride("font_size", size);
        l.AddThemeColorOverride("font_color", new Color(0.2f, 0.15f, 0.1f));
        return l;
    }

    public override void _Input(InputEvent e)
    {
        if (IsOpen && !e.IsEcho() && (e.IsActionPressed("cancel") || e.IsActionPressed("ui_cancel")))
        {
            Close();
            GetViewport().SetInputAsHandled();
        }
    }
}
