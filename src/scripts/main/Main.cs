using Godot;
using MyFarmingGame.Core;

namespace MyFarmingGame;

/// <summary>
/// Root coordinator (project-structure.md §4). Owns the layer stack, player, camera (follows the
/// player from Task 1.2), the night's sleep sequence (Systems/SleepSequence, Task 1.3), freezing Kit
/// while the bag is open (Task 1.4), audio and,
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
        _bag = GetNodeOrNull<UI.BagView>("HUD/UI/Bag");
        if (_bag != null)
            _bag.OpenChanged += OnBagOpenChanged;
        _bubble = GetNodeOrNull<UI.PictureBubbleView>("HUD/UI/PictureBubble");
        if (_bubble != null)
            _bubble.OpenChanged += OnBagOpenChanged;
        if (OS.IsDebugBuild())
        {
            GetTree().CreateTimer(1.0).Timeout += PrintStartupState;
            GetNode("Debug").AddChild(new Debugging.ClockDebugKeys { Name = "ClockDebugKeys" });
            GetNode("Debug").AddChild(new Debugging.BagDebugKeys { Name = "BagDebugKeys" });
        }
    }

    private UI.BagView _bag;
    private UI.PictureBubbleView _bubble;

    public override void _ExitTree()
    {
        if (_bag != null && IsInstanceValid(_bag))
            _bag.OpenChanged -= OnBagOpenChanged;
        if (_bubble != null && IsInstanceValid(_bubble))
            _bubble.OpenChanged -= OnBagOpenChanged;
    }

    /// <summary>Kit stands still while the bag or a picture bubble is open (both hold the clock).</summary>
    private void OnBagOpenChanged(bool open) =>
        GetNode<Player.PlayerController>("World/Entities/Player").Frozen = open;

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
