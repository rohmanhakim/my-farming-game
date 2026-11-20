using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using MyFarmingGame.Autoloads;
using MyFarmingGame.Core;
using MyFarmingGame.Player;
using MyFarmingGame.UI;
using MyFarmingGame.World;

namespace MyFarmingGame.Tests;

/// <summary>
/// Headless smoke tests. Run: godot --headless --path src res://tests/TestRunner.tscn
/// Exit code 0 = all checks passed, 1 = at least one failure (each failure is printed).
/// </summary>
public partial class TestRunner : Node
{
    private int _failures;
    private int _checks;

    public override async void _Ready()
    {
        CheckProjectSettings();
        CheckInputMap();
        CheckSprites();
        CheckMainScene();
        CheckFacingRules();
        await CheckMovement();
        CheckClockRules();
        CheckClockDial();
        await CheckClockPause();
        await CheckSleepInGame();

        GD.Print($"[TestRunner] {_checks - _failures}/{_checks} checks passed");
        GetTree().Quit(_failures == 0 ? 0 : 1);
    }

    private void Check(bool ok, string what)
    {
        _checks++;
        if (ok) { GD.Print($"  PASS  {what}"); return; }
        _failures++;
        GD.PrintErr($"  FAIL  {what}");
    }

    private void CheckProjectSettings()
    {
        Check((string)ProjectSettings.GetSetting("rendering/renderer/rendering_method") == "gl_compatibility",
            "renderer is gl_compatibility");
        Check((string)ProjectSettings.GetSetting("dotnet/project/assembly_name") == "MyFarmingGame",
            "assembly name is MyFarmingGame");
        Check((string)ProjectSettings.GetSetting("application/run/main_scene") == "res://scenes/main/Main.tscn",
            "main scene is res://scenes/main/Main.tscn");
    }

    private void CheckInputMap()
    {
        string[] actions =
        {
            "move_left", "move_right", "move_up", "move_down",
            "interact", "cancel", "cycle_tool_next", "cycle_tool_prev", "pause",
        };
        foreach (var a in actions)
            Check(InputMap.HasAction(a) && InputMap.ActionGetEvents(a).Count > 0, $"input action '{a}' bound");
    }

    private void CheckSprites()
    {
        var expectedFallback = new HashSet<string> { "hen", "tool_hands" };
        foreach (var name in SpriteLibrary.KnownNames)
        {
            var tex = SpriteLibrary.Get(name, out var source);
            bool ok = tex != null && (expectedFallback.Contains(name) || source != SpriteLibrary.Source.Fallback);
            Check(ok, $"sprite '{name}' resolves ({source})");
        }
    }

    private void CheckNode<T>(Node main, string path, ProcessModeEnum mode) where T : Node
    {
        var node = main.GetNodeOrNull(path);
        Check(node is T, $"Main/{path} exists as {typeof(T).Name}");
        if (node != null)
            Check(node.ProcessMode == mode, $"Main/{path} process mode is {mode}");
    }

    private void CheckLayer(Node main, string path, int layer)
    {
        var cl = main.GetNodeOrNull<CanvasLayer>(path);
        Check(cl != null && cl.Layer == layer, $"{path} CanvasLayer is layer {layer}");
    }

    private void CheckMainScene()
    {
        var packed = ResourceLoader.Load<PackedScene>("res://scenes/main/Main.tscn");
        Check(packed != null, "Main.tscn loads");
        if (packed == null) return;

        var main = packed.Instantiate();
        AddChild(main);

        // Scene-architecture standard (project-structure.md §4): node types and process modes.
        CheckNode<Node>(main, ".", ProcessModeEnum.Always);
        CheckNode<Node>(main, "Systems", ProcessModeEnum.Inherit);
        CheckNode<Node2D>(main, "World", ProcessModeEnum.Pausable);
        CheckNode<Node>(main, "World/Level", ProcessModeEnum.Inherit);
        CheckNode<Node2D>(main, "World/Entities", ProcessModeEnum.Inherit);
        CheckNode<Node2D>(main, "World/Effects", ProcessModeEnum.Inherit);
        CheckNode<Node>(main, "Audio", ProcessModeEnum.Always);
        CheckNode<CanvasLayer>(main, "HUD", ProcessModeEnum.Pausable);
        CheckNode<CanvasLayer>(main, "Pause", ProcessModeEnum.WhenPaused);
        CheckNode<CanvasLayer>(main, "Transition", ProcessModeEnum.Always);
        CheckNode<CanvasLayer>(main, "Debug", ProcessModeEnum.Always);
        Check(main.GetNode("World") is not CanvasLayer, "World is a plain Node2D, not a CanvasLayer");

        CheckLayer(main, "HUD", UILayers.Hud);
        CheckLayer(main, "Pause", UILayers.PauseMenu);
        CheckLayer(main, "Transition", UILayers.Transition);
        CheckLayer(main, "Debug", UILayers.Debug);

        Check(main.GetNode("World/Level").GetChildCount() == 1, "exactly one level under World/Level");
        var level = main.GetNode("World/Level").GetChildOrNull<Node>(0);
        Check(level?.GetNodeOrNull<Marker2D>("PlayerSpawn") != null, "level exposes a PlayerSpawn marker");
        var bg = level?.GetNodeOrNull<ColorRect>("Background");
        Check(bg != null, "level has a coloured Background");
        // World/Level is a plain Node, so level art is a separate top-level canvas item: it must sit
        // below Entities by z_index or it covers the player (found in Task 1.1).
        Check(bg != null && bg.ZIndex < 0, "level Background draws below entities (z_index < 0)");
        Check(level?.FindChild("Player", true, false) == null && level?.FindChild("Camera2D", true, false) == null,
            "level contains no player or camera");
        Check(main.GetNodeOrNull<Camera2D>("World/Entities/Camera2D") != null, "Camera2D lives in World/Entities");
        Check(main.GetNodeOrNull<Control>("HUD/UI") != null, "UI.tscn is instanced into HUD");

        // Task 1.3 nodes.
        Check(main.GetNodeOrNull<ClockDialView>("HUD/UI/ClockDial") != null, "HUD has the ClockDial");
        Check(main.GetNodeOrNull<SleepPromptView>("HUD/UI/SleepPrompt") != null, "HUD has the SleepPrompt");
        Check(main.GetNodeOrNull<DayTransitionView>("Transition/DayTransition") != null, "Transition layer has DayTransition");
        Check(main.GetNodeOrNull<SleepSequence>("Systems/SleepSequence") != null, "Systems has SleepSequence");
        var bed = level?.GetNodeOrNull<Bed>("Bed");
        Check(bed != null && bed.IsInGroup(Bed.Group) && bed.IsInGroup(IInteractable.Group), "level has a Bed in groups beds + interactables");
        Check(bed != null && bed.ZIndex < 0, "bed draws below Kit (z_index < 0)");

        var sprite = main.GetNodeOrNull<NamedSprite>("World/Entities/Player/Sprite");
        Check(sprite != null, "Player has NamedSprite");
        Check(sprite?.Texture != null && sprite.Visible, "player sprite has a texture and is visible");
        main.QueueFree();
    }

    // ---- Task 1.2: movement, facing, animation, camera -------------------------------------

    private void CheckFacingRules()
    {
        Check(PlayerController.FacingFor(new Vector2(1, 0), Facing.Down) == Facing.Right, "facing: right input faces Right");
        Check(PlayerController.FacingFor(new Vector2(-1, 0), Facing.Down) == Facing.Left, "facing: left input faces Left");
        Check(PlayerController.FacingFor(new Vector2(0, -1), Facing.Down) == Facing.Up, "facing: up input faces Up");
        Check(PlayerController.FacingFor(new Vector2(0, 1), Facing.Up) == Facing.Down, "facing: down input faces Down");
        Check(PlayerController.FacingFor(new Vector2(1, -1), Facing.Up) == Facing.Up, "facing: diagonal keeps Up when already Up");
        Check(PlayerController.FacingFor(new Vector2(1, -1), Facing.Right) == Facing.Right, "facing: diagonal keeps Right when already Right");
        Check(PlayerController.FacingFor(new Vector2(1, -1), Facing.Down) is Facing.Up or Facing.Right,
            "facing: diagonal from Down turns to one of its two parts");
        Check(PlayerController.FacingFor(new Vector2(1, -0.4f), Facing.Up) == Facing.Right, "facing: mostly-right input faces Right");
        Check(PlayerController.FacingFor(Vector2.Zero, Facing.Left) == Facing.Left, "facing: no input keeps facing");
    }

    private PlayerController _player;

    private async Task Frames(int n)
    {
        for (int i = 0; i < n; i++)
            await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
    }

    /// <summary>Applies <paramref name="press"/>, waits, releases, returns the displacement.</summary>
    private async Task<Vector2> Drive(Action press, Action release, int frames = 30)
    {
        await Frames(15); // let the previous move come to a stop first
        Vector2 start = _player.GlobalPosition;
        press();
        await Frames(frames);
        Vector2 moved = _player.GlobalPosition - start;
        release();
        return moved;
    }

    private static void Key(Key k, bool down) =>
        Input.ParseInputEvent(new InputEventKey { PhysicalKeycode = k, Pressed = down });

    private static void Stick(JoyAxis axis, float value) =>
        Input.ParseInputEvent(new InputEventJoypadMotion { Device = 0, Axis = axis, AxisValue = value });

    private static void Pad(JoyButton b, bool down) =>
        Input.ParseInputEvent(new InputEventJoypadButton { Device = 0, ButtonIndex = b, Pressed = down });

    private void CheckDirection(Vector2 moved, Vector2 dir, string what, float minPx = 40)
    {
        Check(moved.Length() > minPx && moved.Normalized().Dot(dir.Normalized()) > 0.98f,
            $"{what} moves {dir} (moved {moved.Round()})");
    }

    private async Task CheckMovement()
    {
        var main = ResourceLoader.Load<PackedScene>("res://scenes/main/Main.tscn").Instantiate();
        AddChild(main);
        _player = main.GetNodeOrNull<PlayerController>("World/Entities/Player");
        Check(_player != null, "Player root has PlayerController");
        if (_player == null) { main.QueueFree(); return; }
        var anim = _player.GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
        var sprite = _player.GetNode<NamedSprite>("Sprite");
        Check(anim != null, "Player has AnimationPlayer");
        foreach (var state in new[] { "idle", "walk" })
            foreach (var d in new[] { "down", "up", "left", "right" })
                Check(anim != null && anim.HasAnimation($"{state}_{d}"), $"animation '{state}_{d}' exists");
        await Frames(2);

        // Keyboard (WASD + arrows), 4 directions with facing + animation.
        Vector2 moved = await Drive(() => Key(Godot.Key.D, true), () => { }, 30);
        CheckDirection(moved, Vector2.Right, "keyboard D");
        Check(_player.Facing == Facing.Right && anim.CurrentAnimation.ToString() == "walk_right" && !sprite.FlipH,
            $"keyboard D: faces Right, plays walk_right ({anim.CurrentAnimation.ToString()})");
        Key(Godot.Key.D, false);
        await Frames(20);
        Check(anim.CurrentAnimation.ToString() == "idle_right", $"after release: idle_right ({anim.CurrentAnimation.ToString()})");

        moved = await Drive(() => Key(Godot.Key.Left, true), () => Key(Godot.Key.Left, false));
        CheckDirection(moved, Vector2.Left, "arrow Left");
        Check(_player.Facing == Facing.Left && sprite.FlipH, "arrow Left: faces Left, sprite flipped");
        moved = await Drive(() => Key(Godot.Key.W, true), () => Key(Godot.Key.W, false));
        CheckDirection(moved, Vector2.Up, "keyboard W");
        Check(_player.Facing == Facing.Up, "keyboard W: faces Up");
        moved = await Drive(() => Key(Godot.Key.Down, true), () => Key(Godot.Key.Down, false));
        CheckDirection(moved, Vector2.Down, "arrow Down");
        Check(_player.Facing == Facing.Down, "arrow Down: faces Down");
        await Frames(20);

        // 8-way: diagonals move diagonally at the same speed as straight lines; facing stays 4-way.
        Vector2 straight = await Drive(() => Key(Godot.Key.D, true), () => Key(Godot.Key.D, false), 40);
        await Frames(20);
        Vector2 diag = await Drive(() => { Key(Godot.Key.W, true); Key(Godot.Key.D, true); },
            () => { Key(Godot.Key.W, false); Key(Godot.Key.D, false); }, 40);
        CheckDirection(diag, new Vector2(1, -1), "keyboard W+D (diagonal)");
        Check(Mathf.Abs(diag.Length() - straight.Length()) < 0.05f * straight.Length(),
            $"diagonal speed equals straight speed ({diag.Length():0} vs {straight.Length():0})");
        Check(_player.Facing is Facing.Up or Facing.Right, $"diagonal keeps a 4-way facing ({_player.Facing})");
        await Frames(20);

        // Gamepad: left stick (full and half tilt) and D-pad. SDL layout = Xbox / PlayStation / Switch Pro.
        moved = await Drive(() => Stick(JoyAxis.LeftX, -1f), () => Stick(JoyAxis.LeftX, 0f));
        CheckDirection(moved, Vector2.Left, "gamepad left stick");
        Check(_player.Facing == Facing.Left, "gamepad stick: faces Left");
        await Frames(20);
        Vector2 half = await Drive(() => Stick(JoyAxis.LeftY, 0.5f), () => Stick(JoyAxis.LeftY, 0f));
        CheckDirection(half, Vector2.Down, "gamepad half-tilted stick", 15);
        Check(half.Length() < 0.75f * straight.Length() * 30 / 40,
            $"half tilt walks slower ({half.Length():0} px vs full {straight.Length() * 30 / 40:0} px)");
        await Frames(20);
        moved = await Drive(() => Pad(JoyButton.DpadUp, true), () => Pad(JoyButton.DpadUp, false));
        CheckDirection(moved, Vector2.Up, "gamepad D-pad Up");
        moved = await Drive(() => Pad(JoyButton.DpadRight, true), () => Pad(JoyButton.DpadRight, false));
        CheckDirection(moved, Vector2.Right, "gamepad D-pad Right");
        await Frames(20);

        // Click-to-move / walk-to-then-act.
        bool arrived = false;
        Vector2 goal = _player.GlobalPosition + new Vector2(-120, 60);
        _player.WalkTo(goal, () => arrived = true);
        await Frames(90);
        Check(arrived && _player.GlobalPosition.DistanceTo(goal) <= _player.ArriveDistance + 0.5f,
            $"WalkTo reaches the target and runs the act callback (off by {_player.GlobalPosition.DistanceTo(goal):0.0})");
        Check(!_player.HasMoveTarget && anim.CurrentAnimation.ToString().StartsWith("idle_"), "after arriving Kit stands idle");

        _player.WalkTo(_player.GlobalPosition + new Vector2(500, 0));
        await Frames(5);
        Key(Godot.Key.S, true);
        await Frames(2);
        Key(Godot.Key.S, false);
        Check(!_player.HasMoveTarget, "keyboard movement cancels a click target");
        await Frames(20);

        // A real left-click on the ground sets a walk target under the cursor.
        var vp = GetViewport();
        Vector2 screen = vp.GetCanvasTransform() * (_player.GlobalPosition + new Vector2(150, 0));
        vp.PushInput(new InputEventMouseMotion { Position = screen, GlobalPosition = screen }, true);
        vp.PushInput(new InputEventMouseButton { ButtonIndex = MouseButton.Left, Pressed = true, Position = screen, GlobalPosition = screen }, true);
        vp.PushInput(new InputEventMouseButton { ButtonIndex = MouseButton.Left, Pressed = false, Position = screen, GlobalPosition = screen }, true);
        Vector2 before = _player.GlobalPosition;
        Check(_player.HasMoveTarget, "left-click on the ground starts click-to-move");
        await Frames(60);
        CheckDirection(_player.GlobalPosition - before, Vector2.Right, "left-click right of Kit");

        // Camera: lags behind while walking (smooth lerp), then settles on Kit.
        var cam = main.GetNode<Camera2D>("World/Entities/Camera2D");
        Check(cam.IsCurrent(), "follow camera is the current camera");
        Key(Godot.Key.D, true);
        await Frames(45);
        float lag = cam.GetScreenCenterPosition().DistanceTo(_player.GlobalPosition);
        Key(Godot.Key.D, false);
        Check(lag > 2f && lag < 150f, $"camera trails Kit while walking (lag {lag:0.0} px)");
        await Frames(150);
        float settle = cam.GetScreenCenterPosition().DistanceTo(_player.GlobalPosition);
        Check(settle < 1.5f, $"camera settles on Kit after stopping ({settle:0.0} px)");

        main.QueueFree();
    }

    // ---- Task 1.3: day clock, dial, sleep, bedtime, pause --------------------------------

    /// <summary>A private clock for rule tests: in the tree for signals, but not self-ticking.</summary>
    private GameClock NewClock()
    {
        var c = new GameClock();
        AddChild(c);
        c.SetProcess(false);
        return c;
    }

    private void CheckClockRules()
    {
        Check(Mathf.IsEqualApprox(GameClock.RealSecondsPerStep * GameClock.PlayableMinutes / GameClock.MinutesPerStep, 1020.0),
            "playable day 04:30 → 21:30 lasts 17 real minutes (1020 s)");
        Check(GameClock.DayStartMinute == 4 * 60 + 30 && GameClock.BedtimeMinute == 21 * 60 + 30, "wake-up 04:30, forced bedtime 21:30");
        Check(GameClock.RealSecondsPerStep == 180.0, "one in-game hour = 60 real seconds (Harvest Moon pace)");
        Check(GameClock.SeasonLengthDays == 30, "season length is 30 days");
        Check(GameClock.PhaseCount == 8 && GameClock.MinutesPerStep == 180, "8 phases × 3 in-game hours");

        // A fresh clock (= what a restart gives until saving exists) starts at Day 1, 04:30.
        var c = NewClock();
        Check(c.Day == 1 && c.ShownTime == "04:30" && c.Phase == DayPhase.Dawn && c.Season == Season.Spring && c.DayOfSeason == 1,
            $"new clock: Spring Day 1, 04:30, Dawn ({c.Season} {c.DayOfSeason}, {c.ShownTime}, {c.Phase})");
        Check(c.IsRunning, "new clock is running");

        // Time-scaled run: one Tick = one 3-hour step.
        var phases = new List<int>();
        int bedtimes = 0, sleeps = 0, ended = 0, started = 0;
        var reasons = new List<int>();
        c.StepChanged += p => phases.Add(p);
        c.BedtimeReached += () => bedtimes++;
        c.SleepStarted += r => { sleeps++; reasons.Add(r); };
        c.DayEnded += _ => ended++;
        c.DayStarted += _ => started++;
        c.TimeScale = GameClock.RealSecondsPerStep / GameClock.MaxFrameSeconds;
        string[] times = { "07:30", "10:30", "13:30", "16:30", "19:30" };
        bool stepsOk = true;
        for (int i = 0; i < times.Length; i++)
        {
            c.Tick(GameClock.MaxFrameSeconds);
            stepsOk &= c.ShownTime == times[i] && (int)c.Phase == i + 1;
        }
        Check(stepsOk && phases.Count == 5, $"clock steps 04:30→07:30→…→19:30 one step per 3 min (signals {phases.Count})");
        Check(string.Join(",", phases) == "1,2,3,4,5", $"StepChanged fires once per step in order ({string.Join(",", phases)})");
        Check(bedtimes == 0 && c.Day == 1 && c.Phase == DayPhase.Dusk, "19:30 is Dusk, no bedtime before 21:30");
        c.TimeScale = 1;
        c.Tick(0.25); // 19:30 + 20 in-game seconds… still Dusk
        Check(c.Phase == DayPhase.Dusk && bedtimes == 0 && c.ShownTime == "19:30", $"a moment after 19:30 is still Dusk, readout 19:30 ({c.ShownTime})");
        c.TimeScale = 400;
        c.Tick(0.25); // +100 s real = +133 in-game min
        c.Tick(0.25); // … → past 21:30
        Check(bedtimes == 1 && sleeps == 1 && c.IsSleeping, $"21:30 bedtime fires once and starts sleep (bedtime {bedtimes}, sleep {sleeps})");
        Check(reasons.Count == 1 && reasons[0] == (int)SleepReason.Bedtime && c.LastSleepReason == SleepReason.Bedtime, "auto sleep reason is Bedtime");
        double frozen = c.MinutesIntoDay;
        c.Tick(0.25); c.Tick(0.25);
        Check(c.MinutesIntoDay == frozen && bedtimes == 1 && sleeps == 1, "time is stopped while asleep; bedtime does not repeat");
        c.BeginSleep(SleepReason.Bed);
        Check(sleeps == 1, "BeginSleep while asleep is ignored");
        c.CompleteRollover(); c.CompleteRollover(); c.Wake(); c.Wake();
        Check(ended == 1 && started == 1, $"DayEnded/DayStarted fire exactly once per rollover ({ended}/{started})");
        Check(c.Day == 2 && c.ShownTime == "04:30" && c.Phase == DayPhase.Dawn && c.IsRunning,
            $"after 21:30 bedtime: Day 2, 04:30, running ({c.Day}, {c.ShownTime})");

        // Bed path: ask → clock stops; not yet → runs; ask → sleep → one rollover.
        c.TimeScale = 1;
        int asked = 0, cancelled = 0;
        c.SleepAsked += () => asked++;
        c.SleepAskCancelled += () => cancelled++;
        c.AskToSleep();
        double t0 = c.MinutesIntoDay;
        c.Tick(0.2);
        Check(asked == 1 && c.IsAsking && c.MinutesIntoDay == t0, "bed prompt: asked once, clock stopped while open");
        c.CancelAsk();
        c.Tick(0.2);
        Check(cancelled == 1 && !c.IsAsking && c.MinutesIntoDay > t0, "bed prompt 'not yet': clock runs again");
        c.AskToSleep();
        c.BeginSleep(SleepReason.Bed);
        Check(!c.IsAsking && c.IsSleeping && c.LastSleepReason == SleepReason.Bed, "sleeping from the bed");
        c.Wake(); // Wake rolls over if the sequence did not
        Check(ended == 2 && started == 2 && c.Day == 3 && c.ShownTime == "04:30", $"bed sleep rolls the day once ({ended}/{started}, Day {c.Day})");

        // The readout ticks every 15 in-game minutes; the phase stays put in between.
        c.SetTime(3, 0);
        var ticks = new List<int>();
        c.TimeChanged += m => ticks.Add(m);
        c.TimeScale = 1;
        double realPer15 = GameClock.ReadoutMinutes * GameClock.RealSecondsPerStep / GameClock.MinutesPerStep;
        Check(Mathf.IsEqualApprox(realPer15, 15.0), $"15 in-game minutes = 15 real seconds ({realPer15})");
        for (int i = 0; i < 59; i++) c.Tick(0.25); // 14.75 s real → 14.75 in-game min
        Check(ticks.Count == 0 && c.ShownTime == "04:30", $"no readout tick before 15 minutes ({c.ShownTime})");
        c.Tick(0.25); // 15 s
        Check(ticks.Count == 1 && c.ShownTime == "04:45" && ticks[0] == 4 * 60 + 45, $"readout ticks to 04:45 ({c.ShownTime}, {ticks.Count})");
        c.TimeScale = 60; // one Tick = 15 in-game minutes
        for (int i = 0; i < 3; i++) c.Tick(0.25);
        Check(ticks.Count == 4 && c.ShownTime == "05:30" && c.Phase == DayPhase.Dawn, $"readout 05:00 → 05:15 → 05:30, still Dawn ({c.ShownTime})");
        Check(Mathf.IsEqualApprox(GameClock.RealSecondsPerStep * (18 * 60 - GameClock.DayStartMinute) / GameClock.MinutesPerStep, 810.0),
            "04:30 → 18:00 takes 810 real seconds (13 min 30 s)");
        c.TimeScale = 1;

        // Hitches never skip time; holds stop it.
        c.SetTime(3, 0);
        c.Tick(100.0);
        Check(Mathf.IsEqualApprox(c.MinutesIntoDay, GameClock.MaxFrameSeconds * GameClock.MinutesPerStep / GameClock.RealSecondsPerStep),
            $"a 100 s frame hitch only counts as {GameClock.MaxFrameSeconds} s");
        c.Hold("menu");
        double t1 = c.MinutesIntoDay;
        c.Tick(0.2);
        Check(c.MinutesIntoDay == t1 && !c.IsRunning, "a held clock (menu open) does not move");
        c.Release("menu");
        c.Tick(0.2);
        Check(c.MinutesIntoDay > t1, "released clock moves again");

        // Calendar.
        c.SetTime(30, 0);
        Check(c.Season == Season.Spring && c.DayOfSeason == 30, "day 30 is Spring 30");
        c.SetTime(31, 0);
        Check(c.Season == Season.Summer && c.DayOfSeason == 1, "day 31 is Summer 1");
        c.SetTime(121, 0);
        Check(c.Season == Season.Spring && c.DayOfSeason == 1 && c.Year == 2, "day 121 is Spring 1 of year 2");
        c.QueueFree();

        // Restart: a new clock knows nothing of the old one and never reads the wall clock.
        var fresh = NewClock();
        Check(fresh.Day == 1 && fresh.MinutesIntoDay == 0, "a restarted clock starts at Day 1 04:30 (no time passes while closed)");
        fresh.QueueFree();

        // Autoload registration.
        Check(GameClock.Instance != null && GameClock.Instance.GetParent() == GetTree().Root, "GameClock autoload is registered");
        Check(GameClock.Instance?.ProcessMode == ProcessModeEnum.Pausable, "GameClock process mode is Pausable");
    }

    private void CheckClockDial()
    {
        var c = NewClock();
        var dial = ResourceLoader.Load<PackedScene>("res://scenes/ui/ClockDial.tscn").Instantiate<ClockDialView>();
        dial.Clock = c;
        AddChild(dial);
        Check(dial.ShownPhase == DayPhase.Dawn && dial.ShownTimeText == "04:30" && dial.ShownDayText == "Day 1",
            $"dial starts at Dawn 04:30 Day 1 ({dial.ShownPhase} {dial.ShownTimeText} {dial.ShownDayText})");
        c.TimeScale = GameClock.RealSecondsPerStep / GameClock.MaxFrameSeconds;
        bool match = true;
        for (int i = 0; i < 6; i++)
        {
            c.Tick(GameClock.MaxFrameSeconds);
            match &= dial.ShownPhase == c.Phase && dial.ShownTimeText == c.ShownTime;
        }
        Check(match, "dial follows the clock step by step (phase + readout)");

        // All 8 phases draw: sun by day (Dawn–Evening), moon after.
        var angles = new HashSet<float>();
        bool markers = true;
        foreach (DayPhase p in Enum.GetValues(typeof(DayPhase)))
        {
            dial.Display(p, GameClock.FormatTime(GameClock.StepStartMinute(p)), Season.Spring, 1);
            markers &= dial.ShownPhase == p && dial.MarkerSprite == (p <= DayPhase.Evening ? "clock_sun" : "clock_moon");
            angles.Add(Mathf.Snapped(ClockDialView.WedgeCenterAngle(p), 0.001f));
        }
        Check(markers, "dial shows all 8 phases with the sun by day and the moon by night");
        Check(angles.Count == 8, "each phase has its own wedge on the dial");
        Check(Mathf.IsEqualApprox(Mathf.PosMod(ClockDialView.WedgeCenterAngle(DayPhase.Midday) - Mathf.Pi / 8, Mathf.Tau), 3 * Mathf.Pi / 2),
            "noon sits at the top of the dial");
        dial.QueueFree();
        c.QueueFree();
    }

    private async Task CheckClockPause()
    {
        var clock = GameClock.Instance;
        if (clock == null) { Check(false, "GameClock autoload missing"); return; }
        clock.ResetForNewGame();
        clock.TimeScale = 60;
        await Frames(10);
        double a = clock.MinutesIntoDay;
        Check(a > 0, $"autoload clock advances while playing ({a:0.0} min)");
        GetTree().Paused = true;
        await Frames(20);
        double b = clock.MinutesIntoDay;
        await Frames(20);
        Check(clock.MinutesIntoDay == b, "clock does not move while the game is paused");
        GetTree().Paused = false;
        await Frames(10);
        Check(clock.MinutesIntoDay > b, "clock resumes after unpausing");
        clock.ResetForNewGame();
    }

    private async Task<bool> WaitUntil(Func<bool> cond, int maxFrames = 600)
    {
        for (int i = 0; i < maxFrames && !cond(); i++)
            await Frames(1);
        return cond();
    }

    private async Task CheckSleepInGame()
    {
        var clock = GameClock.Instance;
        if (clock == null) return;
        clock.ResetForNewGame();
        var main = ResourceLoader.Load<PackedScene>("res://scenes/main/Main.tscn").Instantiate();
        AddChild(main);
        var player = main.GetNode<PlayerController>("World/Entities/Player");
        var bed = main.GetNode("World/Level").GetChild(0).GetNode<Bed>("Bed");
        var prompt = main.GetNode<SleepPromptView>("HUD/UI/SleepPrompt");
        var fade = main.GetNode<DayTransitionView>("Transition/DayTransition");
        var seq = main.GetNode<SleepSequence>("Systems/SleepSequence");
        var dial = main.GetNode<ClockDialView>("HUD/UI/ClockDial");
        fade.FadeSeconds = 0.05f;
        fade.HoldSeconds = 0.05f;
        int ended = 0, started = 0;
        void OnEnded(int d) => ended++;
        void OnStarted(int d) => started++;
        clock.DayEnded += OnEnded;
        clock.DayStarted += OnStarted;

        await Frames(5);
        Check(!bed.PromptVisible, "bed prompt hidden while Kit is far away");
        Check(dial.ShownTimeText == "04:30" && dial.ShownDayText == "Day 1", "HUD dial shows 04:30, Day 1 at start");

        // Walk up (teleport) and press Space.
        player.GlobalPosition = bed.WakePoint;
        await Frames(5);
        Check(bed.PromptVisible, "bed shows the button prompt when Kit is next to it");
        Key(Godot.Key.Space, true);
        await Frames(1);
        Key(Godot.Key.Space, false);
        await Frames(2);
        Check(clock.IsAsking && prompt.Visible && player.Frozen, "Space at the bed opens 'Go to sleep?' and Kit stands still");
        Check(prompt.YesButton.HasFocus(), "'Sleep' button has gamepad/keyboard focus");
        Check(!bed.PromptVisible, "bed prompt hides while the question is open");
        double m = clock.MinutesIntoDay;
        await Frames(10);
        Check(clock.MinutesIntoDay == m, "clock is stopped while the question is open");
        prompt.Decline();
        await Frames(2);
        Check(!clock.IsAsking && !prompt.Visible && !player.Frozen && clock.IsRunning, "'Not yet' closes it, Kit and the clock move again");

        // Gamepad A (bottom face button on every pad) does the same.
        Pad(JoyButton.A, true);
        await Frames(1);
        Pad(JoyButton.A, false);
        await Frames(2);
        Check(clock.IsAsking && prompt.Visible, "gamepad A at the bed opens the question");
        Input.ParseInputEvent(new InputEventJoypadButton { Device = 0, ButtonIndex = JoyButton.B, Pressed = true });
        await Frames(1);
        Input.ParseInputEvent(new InputEventJoypadButton { Device = 0, ButtonIndex = JoyButton.B, Pressed = false });
        await Frames(2);
        Check(!clock.IsAsking && !prompt.Visible, "gamepad B closes the question");

        // Click the bed from a distance: Kit walks there, then the question opens.
        player.GlobalPosition = bed.WakePoint + new Vector2(220, 160);
        await Frames(15);
        var vp = GetViewport();
        Vector2 screen = vp.GetCanvasTransform() * bed.GlobalPosition;
        Check(player.InteractableAt(bed.GlobalPosition) == bed, "a click on the bed sprite finds the bed");
        vp.PushInput(new InputEventMouseButton { ButtonIndex = MouseButton.Left, Pressed = true, Position = screen, GlobalPosition = screen }, true);
        vp.PushInput(new InputEventMouseButton { ButtonIndex = MouseButton.Left, Pressed = false, Position = screen, GlobalPosition = screen }, true);
        Check(player.HasMoveTarget, "clicking the bed makes Kit walk to it");
        bool askedByClick = await WaitUntil(() => clock.IsAsking, 240);
        Check(askedByClick && player.GlobalPosition.DistanceTo(bed.WakePoint) < 6, "Kit arrives at the bed and the question opens");

        // Sleep → fade → Day 2 at 04:30, signals once, Kit beside the bed.
        prompt.Confirm(true);
        bool woke = await WaitUntil(() => !clock.IsSleeping && !seq.IsRunning, 600);
        Check(woke, "sleep sequence finishes");
        Check(clock.Day == 2 && clock.ShownTime == "04:30" && ended == 1 && started == 1,
            $"bed sleep: Day 2, 04:30, DayEnded/DayStarted once each ({clock.Day}, {clock.ShownTime}, {ended}/{started})");
        Check(dial.ShownDayText == "Day 2" && dial.ShownTimeText == "04:30", "HUD dial shows the new day");
        Check(!player.Frozen && !fade.Visible, "Kit can move again and the fade is gone");

        // 21:30 bedtime far from home: Kit goes home and sleeps, no penalty.
        player.GlobalPosition = bed.WakePoint + new Vector2(400, 300);
        await Frames(5);
        clock.SetTime(clock.Day, GameClock.PlayableMinutes - 1);
        clock.TimeScale = 60;
        bool slept = await WaitUntil(() => clock.Day == 3 && !clock.IsSleeping && !seq.IsRunning, 900);
        Check(slept && clock.LastSleepReason == SleepReason.Bedtime, "21:30 bedtime starts the next day");
        Check(ended == 2 && started == 2, $"bedtime rollover fires DayEnded/DayStarted once ({ended}/{started})");
        Check(clock.ShownTime == "04:30" && player.GlobalPosition.DistanceTo(bed.WakePoint) < 1, "Kit wakes at home at 04:30");
        var cam = main.GetNode<Camera2D>("World/Entities/Camera2D");
        await Frames(2);
        Check(cam.GetScreenCenterPosition().DistanceTo(player.GlobalPosition) < 2, "camera snapped to Kit after waking");

        clock.DayEnded -= OnEnded;
        clock.DayStarted -= OnStarted;
        clock.ResetForNewGame();
        main.QueueFree();
        await Frames(2);

        // Button prompt glyphs per controller family (no hard-coded "A").
        Check(ButtonPrompt.FamilyFor("Xbox Series Controller") == ButtonPrompt.PadFamily.Xbox, "prompt: Xbox pad detected");
        Check(ButtonPrompt.FamilyFor("PS5 Controller") == ButtonPrompt.PadFamily.PlayStation, "prompt: PlayStation pad detected");
        Check(ButtonPrompt.FamilyFor("Nintendo Switch Pro Controller") == ButtonPrompt.PadFamily.Nintendo, "prompt: Switch Pro pad detected");
        Check(ButtonPrompt.FaceLabel(ButtonPrompt.PadFamily.Xbox, JoyButton.A) == "A"
              && ButtonPrompt.FaceLabel(ButtonPrompt.PadFamily.PlayStation, JoyButton.A) == "cross"
              && ButtonPrompt.FaceLabel(ButtonPrompt.PadFamily.Nintendo, JoyButton.A) == "B",
            "prompt: bottom face button shows A / cross / B per family");
        Check(ButtonPrompt.KeyLabel("interact") == "Space", $"prompt: keyboard shows Space ({ButtonPrompt.KeyLabel("interact")})");
    }
}
