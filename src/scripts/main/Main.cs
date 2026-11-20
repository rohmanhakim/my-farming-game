using Godot;
using MyFarmingGame.Core;

namespace MyFarmingGame;

/// <summary>
/// Root coordinator (project-structure.md §4). Owns the layer stack, player, camera (follows the
/// player from Task 1.2), the night's sleep sequence (Systems/SleepSequence, Task 1.3), audio and,
/// from a later task, level swapping (load_level contract). Process modes are set in Main.tscn.
/// </summary>
public partial class Main : Node
{
    public override void _Ready()
    {
        GetNode<CanvasLayer>("HUD").Layer = UILayers.Hud;
        GetNode<CanvasLayer>("Pause").Layer = UILayers.PauseMenu;
        GetNode<CanvasLayer>("Transition").Layer = UILayers.Transition;
        GetNode<CanvasLayer>("Debug").Layer = UILayers.Debug;
        PlacePlayerAtSpawn();
        GetNode<CameraFollow>("World/Entities/Camera2D").SnapToTarget();
        if (OS.IsDebugBuild())
        {
            GetTree().CreateTimer(1.0).Timeout += PrintStartupState;
            GetNode("Debug").AddChild(new Debugging.ClockDebugKeys { Name = "ClockDebugKeys" });
        }
    }

    /// <summary>Debug builds only: one line in Output to diagnose "blank screen" reports.</summary>
    private void PrintStartupState()
    {
        var player = GetNode<Node2D>("World/Entities/Player");
        var cam = GetNode<Camera2D>("World/Entities/Camera2D");
        GD.Print($"[Main] 1s state: player={player.GlobalPosition} visible={player.IsVisibleInTree()} " +
                 $"camera={cam.GetScreenCenterPosition()} current={cam.IsCurrent()} " +
                 $"viewport={GetViewport().GetVisibleRect().Size} window={GetWindow().Size} " +
                 $"level={GetNode("World/Level").GetChildCount()}");
    }

    /// <summary>Moves the player to the current level's PlayerSpawn marker, if it has one.</summary>
    private void PlacePlayerAtSpawn()
    {
        var level = GetNode("World/Level");
        if (level.GetChildCount() == 0)
            return;
        var spawn = level.GetChild(0).GetNodeOrNull<Marker2D>("PlayerSpawn");
        if (spawn != null)
            GetNode<Node2D>("World/Entities/Player").GlobalPosition = spawn.GlobalPosition;
    }
}
