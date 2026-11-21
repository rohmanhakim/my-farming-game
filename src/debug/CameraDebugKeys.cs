using Godot;

namespace MyFarmingGame.Debugging;

/// <summary>
/// Debug builds only (added by <see cref="Main"/>): lets the owner compare the camera presets
/// (Phase-001 Task 1.14) and pick one. F9 or 1 = tilt 45° / 52° / 60° · F10 or 2 = turn 30° / 37° / 45° ·
/// F11 or 3 = south-east / south-west corner · F12 or 4 = perspective / orthographic (F-keys need fn on Mac laptops). A label in the top-right
/// corner shows the current setting. Raw keys on purpose (developer tools, not game controls — the
/// player never moves the camera), so they need no gamepad binding. Never shipped in a release build.
/// </summary>
public partial class CameraDebugKeys : Node
{
    public CameraRig Rig { get; set; }
    public Label Label { get; private set; }

    public override void _Ready()
    {
        Label = new Label
        {
            Name = "CameraLabel",
            HorizontalAlignment = HorizontalAlignment.Right,
            AnchorLeft = 1, AnchorRight = 1, OffsetLeft = -560, OffsetRight = -12, OffsetTop = 210, OffsetBottom = 258,
        };
        Label.AddThemeFontSizeOverride("font_size", 16);
        Label.AddThemeColorOverride("font_outline_color", new Color(0, 0, 0));
        Label.AddThemeConstantOverride("outline_size", 4);
        AddChild(Label);
        Refresh();
    }

    public override void _UnhandledInput(InputEvent e)
    {
        if (Rig == null || e is not InputEventKey { Pressed: true, Echo: false } k) return;
        switch (k.PhysicalKeycode)
        {
            // F-keys are media keys on Mac laptops (fn needed), so 1–4 do the same.
            case Key.F9: case Key.Key1: Rig.CycleTilt(); break;
            case Key.F10: case Key.Key2: Rig.CycleTurn(); break;
            case Key.F11: case Key.Key3: Rig.ToggleCorner(); break;
            case Key.F12: case Key.Key4: Rig.ToggleProjection(); break;
            default: return;
        }
        Rig.Snap();
        Refresh();
        GD.Print($"[debug] camera → {Rig.Describe()}");
        GetViewport().SetInputAsHandled();
    }

    private void Refresh()
    {
        if (Label != null && Rig != null)
            Label.Text = $"Camera: {Rig.Describe()}\n1 or F9 tilt · 2 or F10 turn · 3 or F11 corner · 4 or F12 projection";
    }
}
