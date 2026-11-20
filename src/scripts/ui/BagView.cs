using Godot;
using MyFarmingGame.Autoloads;
using MyFarmingGame.Core;

namespace MyFarmingGame.UI;

/// <summary>
/// The simple bag (Task 1.4, inventory.md): a picture grid of auto-stacked items with number
/// badges and the wallet. No sorting, no dragging, no "full".
///
/// Open: gamepad Y (top face button on every pad) or the on-screen bag button.
/// Close: Y again, B / right-click / Esc, or the on-screen close button.
/// While open the clock is held (<c>GameClock.Hold("bag")</c>, agreed in Task 1.3) and
/// <see cref="OpenChanged"/> lets Main freeze Kit. It does not open during the sleep question,
/// the night or the pause menu.
/// </summary>
public partial class BagView : Control
{
    public const string HoldReason = "bag";

    /// <summary>Empty slots shown so the bag never looks full (inventory.md: start ≈ 8).</summary>
    public const int MinSlots = 8;

    public const float SlotSize = 72f;

    /// <summary>Emitted when the bag opens (true) or closes (false).</summary>
    [Signal] public delegate void OpenChangedEventHandler(bool open);

    /// <summary>Optional sources (tests); default to the autoloads.</summary>
    public Inventory Bag { get; set; }
    public Wallet Purse { get; set; }
    public GameClock Clock { get; set; }

    public bool IsOpen { get; private set; }

    private Control _panelRoot;
    private GridContainer _grid;
    private Label _coins;
    private Button _openButton;
    private Button _closeButton;

    public Button OpenButton => _openButton;
    public Button CloseButton => _closeButton;
    public string CoinsText => _coins?.Text ?? "";

    /// <summary>Number of slots currently drawn (filled + empty).</summary>
    public int SlotCount => _grid?.GetChildCount() ?? 0;

    public override void _Ready()
    {
        _panelRoot = GetNode<Control>("%PanelRoot");
        _grid = GetNode<GridContainer>("%Grid");
        _coins = GetNode<Label>("%Coins");
        _openButton = GetNode<Button>("%OpenButton");
        _closeButton = GetNode<Button>("%CloseButton");
        GetNode<TextureRect>("%CoinIcon").Texture = SpriteLibrary.Get("coin");
        GetNode<TextureRect>("%BagIcon").Texture = SpriteLibrary.Get("icon_bag");
        _openButton.Icon = SpriteLibrary.Get("icon_bag");
        _openButton.Pressed += OnOpenButton;
        _closeButton.Pressed += Close;

        Bag ??= Inventory.Instance;
        Purse ??= Wallet.Instance;
        Clock ??= GameClock.Instance;
        if (Bag != null) Bag.Changed += Refresh;
        if (Purse != null) Purse.CoinsChanged += OnCoinsChanged;
        _panelRoot.Visible = false;
        Refresh();
    }

    public override void _ExitTree()
    {
        if (IsOpen)
        {
            // Leaving the tree (quit, scene change): only give the clock back. No focus calls
            // here (Godot errors with "!is_inside_tree()") and no signal (Main is going too).
            IsOpen = false;
            if (Clock != null && IsInstanceValid(Clock)) Clock.Release(HoldReason);
        }
        if (Bag != null && IsInstanceValid(Bag)) Bag.Changed -= Refresh;
        if (Purse != null && IsInstanceValid(Purse)) Purse.CoinsChanged -= OnCoinsChanged;
    }

    private void OnCoinsChanged(int _) => Refresh();

    private void OnOpenButton() { if (IsOpen) Close(); else Open(); }

    /// <summary>True when nothing else is in charge of the screen (sleep question, night, pause).</summary>
    public bool CanOpen => !IsOpen && !GetTree().Paused &&
                           (Clock == null || (!Clock.IsAsking && !Clock.IsSleeping));

    public void Open()
    {
        if (!CanOpen) return;
        IsOpen = true;
        Refresh();
        _panelRoot.Visible = true;
        Clock?.Hold(HoldReason);
        _closeButton.GrabFocus(); // A / Space / Enter on the focused close button also closes
        EmitSignal(SignalName.OpenChanged, true);
    }

    public void Close()
    {
        if (!IsOpen) return;
        IsOpen = false;
        _panelRoot.Visible = false;
        _closeButton.ReleaseFocus();
        if (Clock != null && IsInstanceValid(Clock)) Clock.Release(HoldReason);
        EmitSignal(SignalName.OpenChanged, false);
    }

    public override void _UnhandledInput(InputEvent e)
    {
        if (e.IsActionPressed("open_inventory") && !e.IsEcho())
        {
            if (IsOpen) Close(); else Open();
            GetViewport().SetInputAsHandled();
        }
    }

    // Close keys are read before the GUI: the dimmed backdrop and the panel would otherwise eat
    // a right-click, and Esc/B must work whatever has focus.
    public override void _Input(InputEvent e)
    {
        if (IsOpen && !e.IsEcho() && (e.IsActionPressed("cancel") || e.IsActionPressed("ui_cancel")))
        {
            Close();
            GetViewport().SetInputAsHandled();
        }
    }

    /// <summary>Redraws the coin readout and the grid from the inventory.</summary>
    public void Refresh()
    {
        if (_grid == null) return;
        _coins.Text = (Purse?.Coins ?? 0).ToString("N0", System.Globalization.CultureInfo.InvariantCulture);
        foreach (Node c in _grid.GetChildren())
        {
            _grid.RemoveChild(c);
            c.QueueFree();
        }
        int filled = 0;
        if (Bag != null)
            foreach (var stack in Bag.Stacks)
            {
                _grid.AddChild(MakeSlot(stack));
                filled++;
            }
        for (int i = filled; i < MinSlots; i++)
            _grid.AddChild(MakeSlot(null));
    }

    private static Control MakeSlot(ItemStack stack)
    {
        var box = new StyleBoxFlat
        {
            BgColor = stack != null ? new Color(0.96f, 0.92f, 0.80f) : new Color(0.96f, 0.92f, 0.80f, 0.35f),
            BorderColor = new Color(0.45f, 0.36f, 0.26f),
        };
        box.SetBorderWidthAll(2);
        box.SetCornerRadiusAll(10);
        var slot = new Panel
        {
            Name = stack != null ? $"Item_{stack.Id}_{stack.Quality}" : "Empty",
            CustomMinimumSize = new Vector2(SlotSize, SlotSize),
            MouseFilter = MouseFilterEnum.Pass,
            TooltipText = stack != null ? ItemIds.DisplayName(stack.Id) : "",
        };
        slot.AddThemeStyleboxOverride("panel", box);
        if (stack == null)
            return slot;

        var icon = new TextureRect
        {
            Name = "Icon",
            Texture = SpriteLibrary.Get(ItemIds.SpriteName(stack.Id)),
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
            StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
            MouseFilter = MouseFilterEnum.Ignore,
        };
        icon.SetAnchorsPreset(LayoutPreset.FullRect);
        icon.OffsetLeft = icon.OffsetTop = 10;
        icon.OffsetRight = icon.OffsetBottom = -10;
        slot.AddChild(icon);

        var badge = new Label
        {
            Name = "Count",
            Text = stack.Count.ToString(),
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Bottom,
            MouseFilter = MouseFilterEnum.Ignore,
            LabelSettings = new LabelSettings
            {
                FontSize = 20,
                FontColor = Colors.White,
                OutlineSize = 6,
                OutlineColor = new Color(0.15f, 0.12f, 0.10f),
            },
        };
        badge.SetAnchorsPreset(LayoutPreset.FullRect);
        badge.OffsetRight = -6;
        badge.OffsetBottom = -2;
        slot.AddChild(badge);
        return slot;
    }

    /// <summary>The count badge text shown for an item (tests); null when it has no slot.</summary>
    public string BadgeFor(string id, int quality = Inventory.MinQuality) =>
        _grid?.GetNodeOrNull<Label>($"Item_{id}_{quality}/Count")?.Text;
}
