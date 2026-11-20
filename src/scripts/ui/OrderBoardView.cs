using System.Collections.Generic;
using System.Linq;
using Godot;
using MyFarmingGame.Autoloads;
using MyFarmingGame.Core;
using MyFarmingGame.World;

namespace MyFarmingGame.UI;

/// <summary>
/// The order board screen (Task 1.7): one picture card per open order — item × count, coin reward,
/// a ✓ pin when accepted and ↻ for a repeat order. Pick a card (A / click) to accept it, deliver
/// it, or cancel it in the picture bubble; no deadline, no penalty. B / right-click / Esc / the
/// close button shut the board. While open the clock is held (<see cref="HoldReason"/>).
/// </summary>
public partial class OrderBoardView : Control
{
    public const string Group = "order_board_view";
    public const string HoldReason = "board";
    public const string Portrait = "icon_order_board";

    [Signal] public delegate void OpenChangedEventHandler(bool open);

    public bool IsOpen { get; private set; }
    public IReadOnlyList<Button> Cards => _cards;
    public Button CloseButton { get; private set; }
    /// <summary>Order id per card, in card order.</summary>
    public IReadOnlyList<string> CardOrderIds => _ids;

    private readonly List<Button> _cards = new();
    private readonly List<string> _ids = new();
    private Control _root;
    private HBoxContainer _row;
    private Label _empty;

    public static OrderBoardView Find(SceneTree tree) => tree.GetFirstNodeInGroup(Group) as OrderBoardView;

    public override void _Ready()
    {
        AddToGroup(Group);
        SetAnchorsPreset(LayoutPreset.FullRect);
        MouseFilter = MouseFilterEnum.Ignore;
        _root = new CenterContainer { MouseFilter = MouseFilterEnum.Stop };
        _root.SetAnchorsPreset(LayoutPreset.FullRect);
        AddChild(_root);
        var dim = new ColorRect { Color = new Color(0, 0, 0, 0.35f), MouseFilter = MouseFilterEnum.Ignore };
        dim.SetAnchorsPreset(LayoutPreset.FullRect);
        AddChild(dim);
        MoveChild(dim, 0);
        _root.ZIndex = 1;

        var panel = new PanelContainer();
        panel.AddThemeStyleboxOverride("panel", new StyleBoxFlat
        {
            BgColor = new Color(0.62f, 0.45f, 0.28f), BorderColor = new Color(0.3f, 0.2f, 0.12f),
            BorderWidthLeft = 4, BorderWidthTop = 4, BorderWidthRight = 4, BorderWidthBottom = 4,
            CornerRadiusTopLeft = 12, CornerRadiusTopRight = 12, CornerRadiusBottomLeft = 12, CornerRadiusBottomRight = 12,
            ContentMarginLeft = 20, ContentMarginRight = 20, ContentMarginTop = 14, ContentMarginBottom = 14,
        });
        _root.AddChild(panel);
        var col = new VBoxContainer();
        col.AddThemeConstantOverride("separation", 14);
        panel.AddChild(col);
        col.AddChild(new TextureRect
        {
            Texture = SpriteLibrary.Get(Portrait), CustomMinimumSize = new Vector2(56, 56),
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize, StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
        });
        _row = new HBoxContainer { Alignment = BoxContainer.AlignmentMode.Center };
        _row.AddThemeConstantOverride("separation", 16);
        col.AddChild(_row);
        _empty = new Label { Text = "No orders today", HorizontalAlignment = HorizontalAlignment.Center };
        _empty.AddThemeFontSizeOverride("font_size", 22);
        col.AddChild(_empty);
        CloseButton = new Button { Text = "Close", CustomMinimumSize = new Vector2(140, 52), SizeFlagsHorizontal = SizeFlags.ShrinkCenter };
        CloseButton.AddThemeFontSizeOverride("font_size", 20);
        CloseButton.Pressed += Close;
        col.AddChild(CloseButton);
        Visible = false;
    }

    public override void _ExitTree()
    {
        if (IsOpen) GameClock.Instance?.Release(HoldReason);
    }

    public bool CanOpen => !GetTree().Paused && (GameClock.Instance == null || GameClock.Instance.IsPlayable || IsOpen);

    public void Open()
    {
        if (!IsOpen && !CanOpen) return;
        bool was = IsOpen;
        IsOpen = true;
        Visible = true;
        GameClock.Instance?.Hold(HoldReason);
        Refresh();
        CallDeferred(MethodName.FocusFirst);
        if (!was) EmitSignal(SignalName.OpenChanged, true);
    }

    private void FocusFirst()
    {
        if (!IsOpen) return;
        var b = _cards.Count > 0 ? _cards[0] : CloseButton;
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

    public void Refresh()
    {
        foreach (var c in _cards) { _row.RemoveChild(c); c.QueueFree(); }
        _cards.Clear();
        _ids.Clear();
        var orders = OrderManager.Instance;
        if (orders != null)
            foreach (var o in orders.BoardOrders)
            {
                var card = MakeCard(o);
                var id = o.Id;
                card.Pressed += () => PickCard(id);
                _row.AddChild(card);
                _cards.Add(card);
                _ids.Add(id);
            }
        _empty.Visible = _cards.Count == 0;
        for (int i = 0; i < _cards.Count; i++)
        {
            _cards[i].FocusNeighborBottom = CloseButton.GetPath();
            if (i > 0) _cards[i].FocusNeighborLeft = _cards[i - 1].GetPath();
            if (i < _cards.Count - 1) _cards[i].FocusNeighborRight = _cards[i + 1].GetPath();
        }
        if (_cards.Count > 0) CloseButton.FocusNeighborTop = _cards[0].GetPath();
    }

    /// <summary>Order ids whose cards are lit (Kit can deliver them now).</summary>
    public bool CardLit(string id) => OrderManager.Instance?.CanDeliver(id) ?? false;

    private static Button MakeCard(Order o)
    {
        var card = new Button { CustomMinimumSize = new Vector2(150, 190), FocusMode = FocusModeEnum.All };
        // A card Kit can hand in right now lights up (Task 1.12 "the order card lights up").
        bool ready = OrderManager.Instance?.CanDeliver(o.Id) ?? false;
        card.AddThemeStyleboxOverride("normal", CardStyle(ready ? new Color(1f, 0.93f, 0.55f) : o.State == OrderState.Accepted ? new Color(0.98f, 0.95f, 0.8f) : new Color(1f, 0.99f, 0.95f), false));
        card.AddThemeStyleboxOverride("hover", CardStyle(new Color(1f, 1f, 0.9f), false));
        card.AddThemeStyleboxOverride("focus", CardStyle(new Color(1f, 1f, 0.9f), true));
        card.AddThemeStyleboxOverride("pressed", CardStyle(new Color(0.9f, 0.9f, 0.8f), true));
        var col = new VBoxContainer { MouseFilter = MouseFilterEnum.Ignore, Alignment = BoxContainer.AlignmentMode.Center };
        col.SetAnchorsPreset(LayoutPreset.FullRect);
        card.AddChild(col);
        string badge = (o.Repeat ? "↻ " : "") + (o.State == OrderState.Accepted ? "✓" : "");
        col.AddChild(Lbl(badge, 22));
        col.AddChild(Icon(ItemIds.SpriteName(o.ItemId), 64));
        col.AddChild(Lbl("×" + o.Count, 24));
        var coin = new HBoxContainer { Alignment = BoxContainer.AlignmentMode.Center, MouseFilter = MouseFilterEnum.Ignore };
        coin.AddChild(Icon("coin", 28));
        coin.AddChild(Lbl(o.Reward.ToString(), 22));
        col.AddChild(coin);
        card.TooltipText = $"{ItemIds.DisplayName(o.ItemId)} ×{o.Count} → {o.Reward} coins";
        return card;
    }

    private static StyleBoxFlat CardStyle(Color bg, bool focused) => new()
    {
        BgColor = bg, BorderColor = focused ? new Color(0.95f, 0.6f, 0.1f) : new Color(0.45f, 0.36f, 0.26f),
        BorderWidthLeft = focused ? 5 : 2, BorderWidthTop = focused ? 5 : 2, BorderWidthRight = focused ? 5 : 2, BorderWidthBottom = focused ? 5 : 2,
        CornerRadiusTopLeft = 8, CornerRadiusTopRight = 8, CornerRadiusBottomLeft = 8, CornerRadiusBottomRight = 8,
    };

    private static TextureRect Icon(string sprite, int size) => new()
    {
        Texture = SpriteLibrary.Get(sprite), CustomMinimumSize = new Vector2(size, size), MouseFilter = MouseFilterEnum.Ignore,
        ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize, StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
        SizeFlagsHorizontal = SizeFlags.ShrinkCenter,
    };

    private static Label Lbl(string text, int size)
    {
        var l = new Label { Text = text, HorizontalAlignment = HorizontalAlignment.Center, MouseFilter = MouseFilterEnum.Ignore };
        l.AddThemeFontSizeOverride("font_size", size);
        l.AddThemeColorOverride("font_color", new Color(0.2f, 0.15f, 0.1f));
        return l;
    }

    /// <summary>A card was picked: the bubble offers what can be done with that order.</summary>
    public void PickCard(string id)
    {
        var orders = OrderManager.Instance;
        var o = orders?.Get(id);
        var bubble = PictureBubbleView.Find(GetTree());
        if (o == null || bubble == null) return;
        Close();
        var back = new BubbleButton { Text = "Back", IsCancel = true, OnPress = Open };
        var chips = Marlow.OrderChips(o);
        if (o.State == OrderState.Offered)
        {
            bubble.Show(Portrait, "Take this order?", chips, new[]
            {
                new BubbleButton { Icon = "icon_check", Text = "Yes", OnPress = () => { orders.Accept(id); Ledger.Note(id, ItemIds.SpriteName(o.ItemId), false); Open(); } },
                back,
            });
        }
        else if (orders.CanDeliver(id))
        {
            bubble.Show(Portrait, "Hand these over?", chips, new[]
            {
                new BubbleButton { Icon = ItemIds.SpriteName(o.ItemId), Text = "Give", OnPress = () => Deliver(id) },
                new BubbleButton { Text = "Cancel order", OnPress = () => { orders.Decline(id); Open(); } },
                back,
            });
        }
        else
        {
            int have = Inventory.Instance?.CountAll(o.ItemId) ?? 0;
            bubble.Show(Portrait, "Still to bring. No hurry.", new List<BubbleChip> { new(ItemIds.SpriteName(o.ItemId), $"{have}/{o.Count}") }, new[]
            {
                new BubbleButton { Text = "Cancel order", OnPress = () => { orders.Decline(id); Open(); } },
                back,
            });
        }
    }

    private void Deliver(string id)
    {
        var orders = OrderManager.Instance;
        var o = orders.Get(id);
        if (!orders.Deliver(id)) { Open(); return; }
        Ledger.Note(id, ItemIds.SpriteName(o.ItemId), true);
        PictureBubbleView.Find(GetTree())?.Show(Marlow.Portrait, Marlow.Lines.Thanks(o), new List<BubbleChip> { new("coin", "+" + o.Reward) },
            new[] { new BubbleButton { Icon = "icon_check", Text = "OK", IsCancel = true, OnPress = Open } });
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
