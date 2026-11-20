using Godot;
using MyFarmingGame.Core;

namespace MyFarmingGame;

/// <summary>
/// Root coordinator (project-structure.md §4). Owns the layer stack, player, camera, audio and,
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
