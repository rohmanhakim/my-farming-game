using Godot;
using MyFarmingGame.Player;

namespace MyFarmingGame.World;

/// <summary>
/// An open doorway Kit walks into to go inside or back out (Phase-002 Task 2.2, owner 2026-11-21:
/// "just walk"). When Kit's body enters the box, <see cref="Main.Teleport"/> fades the screen, moves
/// Kit to <see cref="Target"/> facing <see cref="Facing"/>, and fades back. Created by
/// <see cref="Building"/> in the door gap of a building that has a room, and in the room's door gap;
/// placed by hand where one area meets another (the top of the lane ↔ the village green, Task 2.3).
/// Works the same with a gamepad, keys or a click-walk, since it only needs Kit to walk through.
/// </summary>
public partial class DoorTrigger : Area3D
{
    /// <summary>The scene to load (res:// path) and the marker in it where Kit lands.</summary>
    [Export] public string TargetScene { get; set; } = "";
    [Export] public string TargetMarker { get; set; } = "";

    /// <summary>Which way Kit faces after landing.</summary>
    [Export] public Facing Facing { get; set; } = Facing.Down;

    [Signal] public delegate void UsedEventHandler();

    public override void _Ready()
    {
        CollisionLayer = 0;
        CollisionMask = 1; // Kit's body layer
        Monitorable = false;
        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node3D body)
    {
        if (!body.IsInGroup(PlayerController.Group)) return;
        if (GetTree().GetFirstNodeInGroup(Main.Group) is not Main main) return;
        if (main.Travel(TargetScene, TargetMarker, Facing)) EmitSignal(SignalName.Used);
    }
}
