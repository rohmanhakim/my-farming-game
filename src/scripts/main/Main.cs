using Godot;
using MyFarmingGame.Core;
using MyFarmingGame.Player;

namespace MyFarmingGame;

/// <summary>
/// Root coordinator (project-structure.md §4; 3D since Tasks 1.14–1.15). Owns the layer stack, Kit,
/// the camera rig (follows Kit), the night's sleep sequence (Systems/SleepSequence), the save binder
/// (Systems/SaveBinder: loads the save at start), the guided start (Systems/OnboardingDirector),
/// freezing Kit while a menu is open, audio and, from a later task, level swapping (load_level
/// contract: PlayerSpawn and CameraLimitMin/Max markers). Process modes are set in Main.tscn.
/// </summary>
public partial class Main : Node
{
    public PlayerController Player { get; private set; }
    public CameraRig Rig { get; private set; }

    private UI.BagView _bag;
    private UI.PictureBubbleView _bubble;
    private UI.OrderBoardView _board;
    private UI.ShopView _shop;

    public override void _Ready()
    {
        GetNode<CanvasLayer>("HUD").Layer = UILayers.Hud;
        GetNode<CanvasLayer>("Pause").Layer = UILayers.PauseMenu;
        GetNode<CanvasLayer>("Transition").Layer = UILayers.Transition;
        GetNode<CanvasLayer>("Debug").Layer = UILayers.Debug;

        Player = GetNode<PlayerController>("World/Entities/Player");
        Rig = GetNode<CameraRig>("World/Entities/CameraRig");
        Rig.Target = Player;
        ApplyLevelMarkers();
        Rig.Snap();
        // Restore the save (if any) now that the level and Kit are in place (Task 1.10).
        GetNodeOrNull<SaveBinder>("Systems/SaveBinder")?.LoadAtStart();

        _bag = GetNodeOrNull<UI.BagView>("HUD/UI/Bag");
        _bubble = GetNodeOrNull<UI.PictureBubbleView>("HUD/UI/PictureBubble");
        _board = GetNodeOrNull<UI.OrderBoardView>("HUD/UI/OrderBoard");
        _shop = GetNodeOrNull<UI.ShopView>("HUD/UI/Shop");
        if (_bag != null) _bag.OpenChanged += OnMenuChanged;
        if (_bubble != null) _bubble.OpenChanged += OnMenuChanged;
        if (_board != null) _board.OpenChanged += OnMenuChanged;
        if (_shop != null) _shop.OpenChanged += OnMenuChanged;

        if (OS.IsDebugBuild())
        {
            // A run started from the editor on macOS can open without keyboard focus, so no key
            // reaches the game until the window is clicked (owner 2026-11-21). Ask for focus.
            if (DisplayServer.GetName() != "headless")
                CallDeferred(MethodName.GrabWindowFocus);
            GetNode("Debug").AddChild(new Debugging.ClockDebugKeys { Name = "ClockDebugKeys" });
            GetNode("Debug").AddChild(new Debugging.BagDebugKeys { Name = "BagDebugKeys" });
            GetNode("Debug").AddChild(new Debugging.CameraDebugKeys { Name = "CameraDebugKeys", Rig = Rig });
        }
    }

    private void GrabWindowFocus()
    {
        DisplayServer.WindowMoveToForeground();
        GetWindow().GrabFocus();
    }

    public override void _ExitTree()
    {
        if (_bag != null && IsInstanceValid(_bag)) _bag.OpenChanged -= OnMenuChanged;
        if (_bubble != null && IsInstanceValid(_bubble)) _bubble.OpenChanged -= OnMenuChanged;
        if (_board != null && IsInstanceValid(_board)) _board.OpenChanged -= OnMenuChanged;
        if (_shop != null && IsInstanceValid(_shop)) _shop.OpenChanged -= OnMenuChanged;
    }

    /// <summary>Kit stands still while the bag, a picture bubble, the board or the shop is open.</summary>
    private void OnMenuChanged(bool open) => Player.Frozen = AnyMenuOpen;

    /// <summary>True while the bag, a picture bubble, the order board or the shop is on screen.</summary>
    public bool AnyMenuOpen => (_bag?.IsOpen ?? false) || (_bubble?.IsOpen ?? false) || (_board?.IsOpen ?? false) || (_shop?.IsOpen ?? false);

    /// <summary>The level instance under World/Level.</summary>
    public Node Level => GetNode("World/Level").GetChildCount() > 0 ? GetNode("World/Level").GetChild(0) : null;

    /// <summary>PlayerSpawn → Kit; CameraLimitMin/Max → the rig's focus bounds (load_level contract step 4).</summary>
    private void ApplyLevelMarkers()
    {
        var level = Level;
        if (level == null) return;
        if (level.GetNodeOrNull<Marker3D>("PlayerSpawn") is { } spawn)
            Player.GlobalPosition = new Vector3(spawn.GlobalPosition.X, 0, spawn.GlobalPosition.Z);
        if (level.GetNodeOrNull<Marker3D>("CameraLimitMin") is { } min && level.GetNodeOrNull<Marker3D>("CameraLimitMax") is { } max)
        {
            Rig.LimitMin = new Vector2(min.GlobalPosition.X, min.GlobalPosition.Z);
            Rig.LimitMax = new Vector2(max.GlobalPosition.X, max.GlobalPosition.Z);
        }
    }
}
