using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using MyFarmingGame.Core;
using MyFarmingGame.Player;

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
}
