using Godot;
using MyFarmingGame.Autoloads;
using MyFarmingGame.Core;

namespace MyFarmingGame.UI;

/// <summary>
/// "Go to sleep?" prompt shown when Kit uses the bed (Task 1.3, controls.md §6: a clear visual
/// button). Gamepad: D-pad/stick moves focus, A confirms, B closes. Keyboard: arrows, Space/Enter,
/// Esc. Mouse: click. The clock is stopped while it is open (GameClock.IsAsking).
/// </summary>
public partial class SleepPromptView : Control
{
    /// <summary>"Sleep" presses arriving this soon after opening are ignored, so the button press
    /// that opened the question can never also answer it.</summary>
    public const ulong OpenGuardMsec = 200;

    /// <summary>Optional clock (tests); defaults to the GameClock autoload.</summary>
    public GameClock Clock { get; set; }

    private Button _yes;
    private Button _no;
    private ulong _openedAt;

    public Button YesButton => _yes;
    public Button NoButton => _no;

    public override void _Ready()
    {
        _yes = GetNode<Button>("%Yes");
        _no = GetNode<Button>("%No");
        var sleepIcon = SpriteLibrary.Get("icon_sleep");
        _yes.Icon = sleepIcon;
        GetNode<TextureRect>("%BigIcon").Texture = sleepIcon;
        _yes.Pressed += Confirm;
        _no.Pressed += Decline;
        Visible = false;
        Clock ??= GameClock.Instance;
        if (Clock != null)
        {
            Clock.SleepAsked += Open;
            Clock.SleepAskCancelled += Close;
            Clock.SleepStarted += OnSleepStarted;
        }
    }

    public override void _ExitTree()
    {
        if (Clock == null || !IsInstanceValid(Clock)) return;
        Clock.SleepAsked -= Open;
        Clock.SleepAskCancelled -= Close;
        Clock.SleepStarted -= OnSleepStarted;
    }

    private void OnSleepStarted(int _) => Close();

    public void Open()
    {
        Visible = true;
        _openedAt = Time.GetTicksMsec();
        _yes.GrabFocus();
    }

    public void Close()
    {
        if (!Visible) return;
        Visible = false;
        _yes.ReleaseFocus();
        _no.ReleaseFocus();
    }

    private bool Guarded => Time.GetTicksMsec() - _openedAt < OpenGuardMsec;

    /// <summary>Sleep now. Public for tests (skips the open guard).</summary>
    public void Confirm(bool force) { if (force || !Guarded) Clock?.BeginSleep(SleepReason.Bed); }
    private void Confirm() => Confirm(false);

    /// <summary>Not yet: close and let time run (no guard needed: it never loses anything).</summary>
    public void Decline() => Clock?.CancelAsk();

    public override void _UnhandledInput(InputEvent e)
    {
        if (!Visible) return;
        if (e.IsActionPressed("ui_cancel") || e.IsActionPressed("cancel"))
        {
            Decline();
            GetViewport().SetInputAsHandled();
        }
    }
}
