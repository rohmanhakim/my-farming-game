using Godot;

namespace MyFarmingGame.UI;

/// <summary>
/// Shows the button for an InputMap action (controls.md: prompts are per action, never a
/// hard-coded letter). Follows the device used last: keyboard key cap, or the gamepad's own face
/// button — Xbox letters (A), PlayStation shapes (✕, drawn, no font needed) or Nintendo letters
/// (B is the bottom button). Godot's SDL indices are position-based, so the same binding is the
/// same physical button on every pad; only the drawing changes. Final button art can replace the
/// drawing later without touching callers.
/// </summary>
[GlobalClass]
public partial class ButtonPrompt : Control
{
    public enum PadFamily { Xbox, PlayStation, Nintendo }

    private string _action = "interact";

    /// <summary>InputMap action whose button is shown.</summary>
    [Export]
    public string Action
    {
        get => _action;
        set { _action = value; QueueRedraw(); }
    }

    /// <summary>When true and the keyboard/mouse is in use, a prompt for an action with no keyboard
    /// key draws nothing (the on-screen button next to it is the mouse way in).</summary>
    [Export] public bool HideWithoutKey { get; set; }

    /// <summary>Shared by all prompts: true after gamepad input, false after keyboard/mouse.</summary>
    public static bool UsingGamepad { get; set; } = Input.GetConnectedJoypads().Count > 0;

    /// <summary>Joypad that sent the last gamepad input (decides the family).</summary>
    public static int LastPadDevice { get; set; }

    public override void _Ready()
    {
        CustomMinimumSize = new Vector2(30, 30);
        MouseFilter = MouseFilterEnum.Ignore;
    }

    public override void _Input(InputEvent e)
    {
        bool pad = e is InputEventJoypadButton || (e is InputEventJoypadMotion m && Mathf.Abs(m.AxisValue) > 0.5f);
        bool keys = e is InputEventKey || e is InputEventMouseButton;
        if (pad) LastPadDevice = e.Device;
        if ((pad && !UsingGamepad) || (keys && UsingGamepad))
            UsingGamepad = pad;
        if (pad || keys)
            QueueRedraw();
    }

    /// <summary>Which face-button drawing a controller name gets.</summary>
    public static PadFamily FamilyFor(string joyName)
    {
        string n = (joyName ?? "").ToLowerInvariant();
        if (n.Contains("playstation") || n.Contains("ps4") || n.Contains("ps5") || n.Contains("ps3") ||
            n.Contains("dualsense") || n.Contains("dualshock") || n.Contains("sony"))
            return PadFamily.PlayStation;
        if (n.Contains("nintendo") || n.Contains("switch") || n.Contains("joy-con") || n.Contains("pro controller"))
            return PadFamily.Nintendo;
        return PadFamily.Xbox;
    }

    /// <summary>Text on the button for a face button index (South=0, East=1, West=2, North=3).
    /// PlayStation returns the shape's name; the shape itself is drawn.</summary>
    public static string FaceLabel(PadFamily family, JoyButton b) => family switch
    {
        PadFamily.PlayStation => b switch { JoyButton.A => "cross", JoyButton.B => "circle", JoyButton.X => "square", JoyButton.Y => "triangle", JoyButton.LeftShoulder => "L1", JoyButton.RightShoulder => "R1", _ => b.ToString() },
        PadFamily.Nintendo => b switch { JoyButton.A => "B", JoyButton.B => "A", JoyButton.X => "Y", JoyButton.Y => "X", JoyButton.LeftShoulder => "L", JoyButton.RightShoulder => "R", _ => b.ToString() },
        _ => b switch { JoyButton.A => "A", JoyButton.B => "B", JoyButton.X => "X", JoyButton.Y => "Y", JoyButton.LeftShoulder => "LB", JoyButton.RightShoulder => "RB", _ => b.ToString() },
    };

    /// <summary>True for the shoulder buttons, which are drawn as a pill instead of a circle.</summary>
    public static bool IsShoulder(JoyButton b) => b is JoyButton.LeftShoulder or JoyButton.RightShoulder;

    /// <summary>True if the action has a keyboard key.</summary>
    public static bool HasKey(string action)
    {
        if (!InputMap.HasAction(action)) return false;
        foreach (var ev in InputMap.ActionGetEvents(action))
            if (ev is InputEventKey)
                return true;
        return false;
    }

    /// <summary>Keyboard key name for the action, e.g. "Space"; "Wheel" for a mouse-wheel binding;
    /// "Click" otherwise.</summary>
    public static string KeyLabel(string action)
    {
        if (!InputMap.HasAction(action)) return "?";
        foreach (var ev in InputMap.ActionGetEvents(action))
            if (ev is InputEventKey k)
            {
                // Show the key as labelled on this keyboard layout (headless has no layout: use as is).
                var code = k.PhysicalKeycode == Key.None ? k.Keycode
                    : DisplayServer.GetName() == "headless" ? k.PhysicalKeycode
                    : DisplayServer.KeyboardGetKeycodeFromPhysical(k.PhysicalKeycode);
                return OS.GetKeycodeString(code);
            }
        foreach (var ev in InputMap.ActionGetEvents(action))
            if (ev is InputEventMouseButton { ButtonIndex: MouseButton.WheelUp or MouseButton.WheelDown })
                return "Wheel";
        return "Click";
    }

    public static JoyButton? PadButton(string action)
    {
        if (!InputMap.HasAction(action)) return null;
        foreach (var ev in InputMap.ActionGetEvents(action))
            if (ev is InputEventJoypadButton jb)
                return jb.ButtonIndex;
        return null;
    }

    /// <summary>What this prompt shows right now (tests).</summary>
    public string CurrentLabel
    {
        get
        {
            var btn = PadButton(Action);
            if (UsingGamepad && btn.HasValue)
                return FaceLabel(FamilyFor(Input.GetJoyName(LastPadDevice)), btn.Value);
            return KeyLabel(Action);
        }
    }

    private void SetMinWidth(float w)
    {
        if (!Mathf.IsEqualApprox(CustomMinimumSize.X, w))
            CallDeferred(Control.MethodName.SetCustomMinimumSize, new Vector2(w, CustomMinimumSize.Y));
    }

    public override void _Draw()
    {
        var font = GetThemeDefaultFont();
        var dark = new Color(0.15f, 0.12f, 0.10f);
        var btn = PadButton(Action);
        if (!(UsingGamepad && btn.HasValue))
        {
            if (HideWithoutKey && !HasKey(Action))
            {
                SetMinWidth(0);
                return;
            }
            // Key cap: rounded box with the key name.
            string text = KeyLabel(Action);
            int fs = 14;
            float w = Mathf.Max(Size.Y, font.GetStringSize(text, HorizontalAlignment.Left, -1, fs).X + 14);
            var box = new StyleBoxFlat { BgColor = new Color(0.96f, 0.95f, 0.9f), BorderColor = dark };
            box.SetBorderWidthAll(2); box.SetCornerRadiusAll(6);
            DrawStyleBox(box, new Rect2(0, 0, w, Size.Y));
            DrawString(font, new Vector2(0, Size.Y / 2 + fs * 0.35f), text, HorizontalAlignment.Center, w, fs, dark);
            SetMinWidth(w);
            return;
        }

        var family = FamilyFor(Input.GetJoyName(LastPadDevice));
        if (IsShoulder(btn.Value))
        {
            // Shoulder button: a dark pill with LB / RB, L1 / R1 or L / R.
            string sl = FaceLabel(family, btn.Value);
            float pw = Mathf.Max(Size.Y * 1.4f, font.GetStringSize(sl, HorizontalAlignment.Left, -1, 14).X + 14);
            var pill = new StyleBoxFlat { BgColor = new Color(0.25f, 0.25f, 0.28f), BorderColor = dark };
            pill.SetBorderWidthAll(2); pill.SetCornerRadiusAll((int)(Size.Y / 2));
            DrawStyleBox(pill, new Rect2(0, 0, pw, Size.Y));
            DrawString(font, new Vector2(0, Size.Y / 2 + 14 * 0.35f), sl, HorizontalAlignment.Center, pw, 14, Colors.White);
            SetMinWidth(pw);
            return;
        }
        SetMinWidth(Size.Y);
        var c = Size / 2f;
        float r = Mathf.Min(Size.X, Size.Y) / 2f - 1f;
        string label = FaceLabel(family, btn.Value);
        Color fill = family switch
        {
            PadFamily.Xbox => btn.Value switch
            {
                JoyButton.A => new Color(0.36f, 0.72f, 0.30f), JoyButton.B => new Color(0.85f, 0.28f, 0.25f),
                JoyButton.X => new Color(0.25f, 0.50f, 0.90f), JoyButton.Y => new Color(0.95f, 0.78f, 0.20f),
                _ => new Color(0.4f, 0.4f, 0.4f),
            },
            PadFamily.Nintendo => new Color(0.30f, 0.30f, 0.32f),
            _ => new Color(0.18f, 0.18f, 0.22f),
        };
        DrawCircle(c, r + 1f, dark);
        DrawCircle(c, r - 1f, fill);
        if (family == PadFamily.PlayStation)
        {
            float s = r * 0.45f;
            switch (label)
            {
                case "cross":
                    var blue = new Color(0.55f, 0.70f, 1f);
                    DrawLine(c + new Vector2(-s, -s), c + new Vector2(s, s), blue, 3f);
                    DrawLine(c + new Vector2(-s, s), c + new Vector2(s, -s), blue, 3f);
                    break;
                case "circle":
                    DrawArc(c, s, 0, Mathf.Tau, 24, new Color(1f, 0.45f, 0.45f), 3f);
                    break;
                case "square":
                    DrawRect(new Rect2(c - Vector2.One * s, Vector2.One * 2 * s), new Color(1f, 0.55f, 0.85f), false, 3f);
                    break;
                default:
                    DrawPolyline(new[] { c + new Vector2(0, -s), c + new Vector2(s, s * 0.8f), c + new Vector2(-s, s * 0.8f), c + new Vector2(0, -s) },
                        new Color(0.4f, 0.9f, 0.75f), 3f);
                    break;
            }
            return;
        }
        int size = 16;
        DrawString(font, new Vector2(0, c.Y + size * 0.36f), label, HorizontalAlignment.Center, Size.X, size, Colors.White);
    }
}
