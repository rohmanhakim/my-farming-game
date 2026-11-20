using System.Collections.Generic;
using Godot;
using MyFarmingGame.Autoloads;
using MyFarmingGame.Core;

namespace MyFarmingGame.UI;

/// <summary>
/// HUD tool belt (Task 1.4, controls.md §2/§4): large picture slots along the bottom of the
/// screen, the active one raised and outlined. Pure view plus input; state lives in
/// <see cref="ToolBelt"/>.
///
/// Cycling: gamepad RB / X = next, LB = previous (position-based, so the same on Xbox, PlayStation
/// and Switch Pro pads); mouse wheel down = next, up = previous; clicking a slot picks it.
/// Ignored while the clock is stopped by a menu, the sleep question or the night (bag open,
/// "Go to sleep?", fade), so a stray press never changes tools behind a menu.
/// </summary>
public partial class ToolBeltView : Control
{
    /// <summary>A fast mouse wheel (or a free-spinning one) sends many ticks; at most one tool
    /// change per this many milliseconds from the wheel.</summary>
    public const ulong WheelRepeatMsec = 120;

    public const float SlotSize = 72f;

    /// <summary>Optional belt (tests); defaults to the ToolBelt autoload.</summary>
    public ToolBelt Belt { get; set; }

    /// <summary>Optional clock (tests); defaults to the GameClock autoload.</summary>
    public GameClock Clock { get; set; }

    private HBoxContainer _slots;
    private readonly Dictionary<Tool, Button> _buttons = new();
    private ulong _lastWheel;
    private StyleBoxFlat _normal, _active, _hover;

    /// <summary>Tool whose slot is drawn as active (tests).</summary>
    public Tool? ShownActive { get; private set; }

    public IReadOnlyDictionary<Tool, Button> SlotButtons => _buttons;

    public override void _Ready()
    {
        _slots = GetNode<HBoxContainer>("%Slots");
        _normal = SlotStyle(new Color(0.16f, 0.13f, 0.10f, 0.78f), new Color(0.55f, 0.48f, 0.38f), 2);
        _hover = SlotStyle(new Color(0.24f, 0.20f, 0.15f, 0.85f), new Color(0.80f, 0.72f, 0.55f), 2);
        _active = SlotStyle(new Color(0.98f, 0.90f, 0.62f, 0.96f), new Color(1f, 1f, 1f), 4);
        Belt ??= ToolBelt.Instance;
        Clock ??= GameClock.Instance;
        if (Belt != null)
        {
            Belt.ActiveToolChanged += OnActiveChanged;
            Belt.ToolsChanged += Rebuild;
        }
        Rebuild();
    }

    public override void _ExitTree()
    {
        if (Belt == null || !IsInstanceValid(Belt)) return;
        Belt.ActiveToolChanged -= OnActiveChanged;
        Belt.ToolsChanged -= Rebuild;
    }

    private static StyleBoxFlat SlotStyle(Color bg, Color border, int width)
    {
        var s = new StyleBoxFlat { BgColor = bg, BorderColor = border };
        s.SetBorderWidthAll(width);
        s.SetCornerRadiusAll(12);
        s.SetContentMarginAll(8);
        return s;
    }

    private void Rebuild()
    {
        foreach (var b in _buttons.Values)
            b.QueueFree();
        _buttons.Clear();
        if (Belt == null) return;
        foreach (var tool in Belt.Tools)
        {
            var t = tool;
            var btn = new Button
            {
                Name = $"Slot{tool}",
                CustomMinimumSize = new Vector2(SlotSize, SlotSize),
                FocusMode = FocusModeEnum.None, // the D-pad is for walking; slots are picked by cycling
                Icon = SpriteLibrary.Get(ToolBelt.SpriteName(tool)),
                ExpandIcon = true,
                IconAlignment = HorizontalAlignment.Center,
                TooltipText = ToolBelt.DisplayName(tool),
            };
            btn.Pressed += () => Belt?.Select(t);
            btn.GuiInput += OnSlotGuiInput;
            _slots.AddChild(btn);
            _buttons[tool] = btn;
        }
        OnActiveChanged((int)Belt.Active);
    }

    private void OnActiveChanged(int tool)
    {
        ShownActive = (Tool)tool;
        foreach (var (t, b) in _buttons)
        {
            bool on = t == ShownActive;
            b.AddThemeStyleboxOverride("normal", on ? _active : _normal);
            b.AddThemeStyleboxOverride("hover", on ? _active : _hover);
            b.AddThemeStyleboxOverride("pressed", _active);
            // Containers reset Scale, so the active slot stands out by its light background and
            // thick white outline; the others are dark and slightly faded.
            b.Modulate = on ? Colors.White : new Color(1f, 1f, 1f, 0.9f);
        }
    }

    /// <summary>True when cycling is allowed (no menu, question or night in the way).</summary>
    public bool CanCycle => Belt != null && (Clock == null || Clock.IsRunning) && !GetTree().Paused;

    public override void _UnhandledInput(InputEvent e)
    {
        if (HandleCycle(e))
            GetViewport().SetInputAsHandled();
    }

    // The wheel over a slot button is eaten by the GUI before _UnhandledInput, so slots forward it.
    private void OnSlotGuiInput(InputEvent e)
    {
        if (e is InputEventMouseButton { ButtonIndex: MouseButton.WheelUp or MouseButton.WheelDown } && HandleCycle(e))
            AcceptEvent();
    }

    private bool HandleCycle(InputEvent e)
    {
        bool next = e.IsActionPressed("cycle_tool_next") && !e.IsEcho();
        bool prev = e.IsActionPressed("cycle_tool_prev") && !e.IsEcho();
        if (!next && !prev) return false;
        if (!CanCycle) return true; // swallow it: nothing behind a menu should react either
        if (e is InputEventMouseButton)
        {
            ulong now = Time.GetTicksMsec();
            if (_lastWheel != 0 && now - _lastWheel < WheelRepeatMsec) return true;
            _lastWheel = now;
        }
        if (next) Belt.Next(); else Belt.Prev();
        return true;
    }

    /// <summary>Tests: forget the last wheel tick so the next one counts.</summary>
    public void ResetWheelRepeat() => _lastWheel = 0;
}
