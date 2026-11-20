using Godot;
using MyFarmingGame.Autoloads;

namespace MyFarmingGame.Debugging;

/// <summary>
/// Debug builds only (added by Main when <c>OS.IsDebugBuild()</c>): playtest shortcuts for the
/// day clock so nobody has to wait ~15 minutes to see bedtime.
/// F2 = jump to the next 3-hour step · F3 = toggle 20× clock speed · F4 = jump to 21:20.
/// Raw keys on purpose: these are developer tools, not game controls, so they stay out of the
/// InputMap and need no gamepad binding. Never shipped in a release build.
/// </summary>
public partial class ClockDebugKeys : Node
{
    public const double FastScale = 20.0;

    public override void _UnhandledInput(InputEvent e)
    {
        var clock = GameClock.Instance;
        if (clock == null || e is not InputEventKey { Pressed: true, Echo: false } k)
            return;
        switch (k.PhysicalKeycode)
        {
            case Key.F2:
                if (!clock.IsRunning) return;
                int next = clock.StepIndex + 1;
                if (next * GameClock.MinutesPerStep >= GameClock.PlayableMinutes)
                    clock.SetTime(clock.Day, GameClock.PlayableMinutes - 0.01); // a moment before 21:30
                else
                    clock.SetTime(clock.Day, next * GameClock.MinutesPerStep);
                GD.Print($"[debug] clock → {clock.ShownTime} ({clock.Phase})");
                break;
            case Key.F3:
                clock.TimeScale = clock.TimeScale > 1.0 ? 1.0 : FastScale;
                GD.Print($"[debug] clock speed ×{clock.TimeScale}");
                break;
            case Key.F4:
                if (!clock.IsRunning) return;
                clock.SetTime(clock.Day, GameClock.PlayableMinutes - 10);
                GD.Print("[debug] clock → 21:20 (bedtime in 10 in-game minutes = 10 s)");
                break;
            default:
                return;
        }
        GetViewport().SetInputAsHandled();
    }
}
