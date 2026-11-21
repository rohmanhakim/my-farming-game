using System.Linq;
using System.Threading.Tasks;
using Godot;
using MyFarmingGame.Autoloads;
using MyFarmingGame.Core;
using MyFarmingGame.UI;

namespace MyFarmingGame.Tests;

/// <summary>Task 1.11: pause menu (Start / Esc / button), settings, day/night tint.</summary>
public partial class TestRunner
{
    private async Task CheckPauseAndLight()
    {
        ResetTask14State();
        var clock = GameClock.Instance;
        var main = await NewMain();
        var menu = main.GetNodeOrNull<PauseMenuView>("Pause/PauseMenu");
        var light = main.GetNodeOrNull<DayNightView>("World/Effects/DayNight");
        var button = main.GetNodeOrNull<PauseButton>("HUD/UI/PauseButton");
        Check(menu != null && light != null && button != null, "Pause/PauseMenu, World/Effects/DayNight and the HUD pause button exist");
        if (menu == null || light == null || button == null) { main.QueueFree(); return; }
        menu.AllowQuit = false;
        GameSettings.ResetToDefaults();
        await Frames(3);

        // Start (gamepad) opens it and pauses the game; the clock stands still.
        clock.SetTime(1, 200);
        await Tap(JoyButton.Start);
        Check(menu.IsOpen && GetTree().Paused && menu.Visible, "gamepad Start opens the pause menu and pauses the game");
        Check(menu.ResumeButton.HasFocus(), "Resume has gamepad focus");
        Check(menu.DayText == "Day 1", $"the menu shows the calendar ({menu.DayText})");
        double m = clock.MinutesIntoDay;
        await IdleFrames(20);
        Check(clock.MinutesIntoDay == m, "time stands still while paused");
        await Tap(JoyButton.B);
        Check(!menu.IsOpen && !GetTree().Paused, "B resumes");

        // Esc and the on-screen button.
        await Press(Godot.Key.Escape);
        Check(menu.IsOpen && GetTree().Paused, "Esc opens the pause menu");
        await Press(Godot.Key.Escape);
        Check(!menu.IsOpen && !GetTree().Paused, "Esc closes it again");
        button.EmitSignal(BaseButton.SignalName.Pressed);
        await IdleFrames(2);
        Check(menu.IsOpen, "the on-screen pause button opens it");
        menu.ResumeButton.EmitSignal(BaseButton.SignalName.Pressed);
        await IdleFrames(2);
        Check(!menu.IsOpen && !GetTree().Paused, "Resume closes it");

        // Save button (test save file), settings, quit.
        var save = SaveGame.Instance;
        save.SavePath = TestSavePath;
        save.Enabled = true;
        menu.Open();
        menu.SaveButton.EmitSignal(BaseButton.SignalName.Pressed);
        Check(FileAccess.FileExists(TestSavePath) && menu.SavedLabel.Visible, "Save writes the game and shows 'Saved ✓'");
        save.Enabled = false;
        save.DeleteSave();
        save.SavePath = SaveGame.DefaultPath;
        menu.SettingsButton.EmitSignal(BaseButton.SignalName.Pressed);
        await IdleFrames(2);
        Check(menu.SettingsOpen, "Settings opens the settings page");
        float vol = GameSettings.Volume;
        menu.VolumeDown.EmitSignal(BaseButton.SignalName.Pressed);
        Check(Mathf.IsEqualApprox(GameSettings.Volume, vol - GameSettings.VolumeStep), $"volume − lowers the volume ({GameSettings.Volume:0.00})");
        menu.VolumeUp.EmitSignal(BaseButton.SignalName.Pressed);
        int bus = AudioServer.GetBusIndex("Master");
        Check(Mathf.IsEqualApprox(GameSettings.Volume, vol) && Mathf.IsEqualApprox(AudioServer.GetBusVolumeDb(bus), Mathf.LinearToDb(vol)), "volume + raises it; the Master bus follows");
        Check(GameSettings.Nudges && !GameSettings.ReadAloud && !GameSettings.BreakReminder, "defaults: hints on, read aloud off, break reminder off (GDD §4)");
        menu.NudgesToggle.ButtonPressed = false;
        menu.BreakToggle.ButtonPressed = true;
        menu.ReadAloudToggle.ButtonPressed = true;
        Check(!GameSettings.Nudges && GameSettings.BreakReminder && GameSettings.ReadAloud, "the three toggles change the settings");
        await Tap(JoyButton.B);
        Check(menu.IsOpen && !menu.SettingsOpen, "B in Settings goes back to the menu");
        menu.QuitButton.EmitSignal(BaseButton.SignalName.Pressed);
        Check(!menu.IsOpen && !GetTree().Paused, "Quit (test mode) closes the menu");
        GameSettings.ResetToDefaults();
        Check(GameSettings.Nudges && !GameSettings.BreakReminder, "settings reset");

        // Not during the sleep question.
        clock.AskToSleep();
        await Tap(JoyButton.Start);
        Check(!menu.IsOpen, "the pause menu doesn't open over 'Go to sleep?'");
        clock.CancelAsk();

        // Day/night light follows the phase (3D since Task 1.19: sun colour, brightness and height, sky, ambient).
        light.FadeSeconds = 0.01f;
        var sun = main.GetNode<DirectionalLight3D>("World/Effects/Sun");
        var env = main.GetNode<WorldEnvironment>("World/Effects/WorldEnvironment").Environment;
        var seen = new System.Collections.Generic.List<float>();
        Color prev = Colors.Black;
        bool differs = true;
        foreach (DayPhase phase in System.Enum.GetValues(typeof(DayPhase)))
        {
            if (phase > DayPhase.Dusk) break; // night phases are never reached before bedtime (dial only)
            clock.SetTime(1, (int)phase * GameClock.MinutesPerStep + 1);
            await Frames(3);
            await WaitUntil(() => light.Settled, 60);
            var want = DayNightView.LightFor(phase);
            differs &= light.TargetTint == DayNightView.TintFor(phase) && sun.LightColor.IsEqualApprox(want.Tint)
                       && Mathf.IsEqualApprox(sun.LightEnergy, want.SunEnergy) && env.AmbientLightColor.IsEqualApprox(want.Tint)
                       && env.BackgroundColor.IsEqualApprox(want.Sky) && light.TargetTint != prev;
            seen.Add(sun.RotationDegrees.X);
            prev = light.TargetTint;
        }
        Check(differs, "the sun, sky and ambient light change per phase and reach each phase's values");
        Check(seen.Count >= 5 && seen.Min() < -50 && seen.Max() > -25, $"the sun climbs to midday and sinks towards dusk ({string.Join(", ", seen.Select(v => v.ToString("0")))})");
        foreach (DayPhase phase in System.Enum.GetValues(typeof(DayPhase)))
            Check(DayNightView.BrightnessOf(DayNightView.LightFor(phase)) >= DayNightView.MinBrightness,
                $"{phase} light is at least half as bright as midday ({DayNightView.BrightnessOf(DayNightView.LightFor(phase)):0.00})");
        var windows = GetTree().GetNodesInGroup(DayNightView.WindowGroup);
        clock.SetTime(1, (int)DayPhase.Dusk * GameClock.MinutesPerStep + 1);
        await WaitUntil(() => light.Settled, 60);
        Check(windows.Count >= 3 && windows.Cast<Node>().All(w => w is GeometryInstance3D { MaterialOverride: StandardMaterial3D { EmissionEnabled: true } }),
            $"windows of the house, shed and cottage glow at dusk ({windows.Count})");
        clock.SetTime(1, (int)DayPhase.Midday * GameClock.MinutesPerStep + 1);
        await WaitUntil(() => light.Settled, 60);
        Check(windows.Cast<Node>().All(w => w is GeometryInstance3D { MaterialOverride: StandardMaterial3D { EmissionEnabled: false } }),
            "…and stop glowing by day");
        Check(main.FindChildren("*", "Light3D", true, false).Count == 1, "budget: one light in the world (windows glow, they are not lights)");
        Check(DayNightView.TintFor(DayPhase.Midday) == Colors.White, "midday is untinted");
        var night = DayNightView.TintFor(DayPhase.Night);
        Check(night.V >= 0.5f && night.V < 0.9f && DayNightView.TintFor(DayPhase.Dusk).V < 1f, "night is dim but readable (≥ 50 % bright)");

        main.QueueFree();
        await Frames(2);
        ResetTask14State();
    }
}
