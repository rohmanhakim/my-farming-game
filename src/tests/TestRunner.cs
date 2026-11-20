using System.Collections.Generic;
using Godot;
using MyFarmingGame.Core;

namespace MyFarmingGame.Tests;

/// <summary>
/// Headless smoke tests. Run: godot --headless --path src res://tests/TestRunner.tscn
/// Exit code 0 = all checks passed, 1 = at least one failure (each failure is printed).
/// </summary>
public partial class TestRunner : Node
{
    private int _failures;
    private int _checks;

    public override void _Ready()
    {
        CheckProjectSettings();
        CheckInputMap();
        CheckSprites();
        CheckMainScene();

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
}
