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
    /// <summary>Main joins this group so doorways can find it.</summary>
    public const string Group = "main";

    /// <summary>Camera distance inside a room (closer than outside, so the room fills the screen).</summary>
    [Export] public float RoomCameraDistance { get; set; } = 9f;

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

        AddToGroup(Group);
        Player = GetNode<PlayerController>("World/Entities/Player");
        Rig = GetNode<CameraRig>("World/Entities/CameraRig");
        Rig.Target = Player;
        _outdoorDistance = Rig.Distance;
        _fade = GetNodeOrNull<UI.DayTransitionView>("Transition/DayTransition");
        if (Level != null && string.IsNullOrEmpty(CurrentLevelPath)) CurrentLevelPath = Level.SceneFilePath;
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

    private float _outdoorDistance;
    private UI.DayTransitionView _fade;

    /// <summary>The farm scene and the house room (where Kit sleeps).</summary>
    public const string FarmScene = "res://scenes/world/FarmLevel.tscn";
    public const string HouseRoomScene = "res://scenes/world/rooms/HouseRoom.tscn";

    /// <summary>The scene under World/Level now (res:// path).</summary>
    public string CurrentLevelPath { get; private set; } = "";

    /// <summary>The room Kit is in (the level when it is a room), or null outside.</summary>
    public World.Building CurrentRoom => Level as World.Building is { IsRoom: true } b ? b : null;

    /// <summary>A new level was loaded (load_level contract step 5). Argument: its res:// path.</summary>
    [Signal] public delegate void LevelChangedEventHandler(string path);

    /// <summary>The load_level contract (project-structure.md §4, Phase-002 Task 2.2): frees the
    /// current level only (Kit, camera, HUD and the FarmState stay), adds the new one, applies its
    /// camera framing and emits <see cref="LevelChanged"/>. Places Kit at <paramref name="marker"/>
    /// when given. Synchronous: call it while the screen is covered.</summary>
    public Node LoadLevelNow(string path, string marker = null, Facing? facing = null)
    {
        var holder = GetNode("World/Level");
        if (path != CurrentLevelPath || holder.GetChildCount() == 0)
        {
            var packed = ResourceLoader.Load<PackedScene>(path);
            if (packed == null) { GD.PushError($"[Main] cannot load level {path}"); return Level; }
            foreach (Node old in holder.GetChildren()) { holder.RemoveChild(old); old.QueueFree(); }
            holder.AddChild(packed.Instantiate());
            CurrentLevelPath = path;
            ApplyLevelMarkers();
            EmitSignal(SignalName.LevelChanged, path);
        }
        if (!string.IsNullOrEmpty(marker) && Level?.FindChild(marker, true, false) is Node3D m)
            Player.GlobalPosition = new Vector3(m.GlobalPosition.X, 0, m.GlobalPosition.Z);
        if (facing is Facing f) Player.Face(f);
        Rig.Snap();
        return Level;
    }

    /// <summary>True while a doorway fade is running.</summary>
    public bool Teleporting { get; private set; }

    /// <summary>Walks Kit through a doorway into another scene: quick fade, load, land on the marker
    /// facing <paramref name="facing"/>, fade back. Returns false if a trip is already running.</summary>
    public bool Travel(string scene, string marker, Facing facing)
    {
        if (Teleporting || string.IsNullOrEmpty(scene)) return false;
        Teleporting = true;
        RunTravel(scene, marker, facing);
        return true;
    }

    private async void RunTravel(string scene, string marker, Facing facing)
    {
        Player.Frozen = true;
        if (_fade != null) await _fade.Cover();
        if (!IsInsideTree()) return;
        CallDeferred(MethodName.SwapForTravel, scene, marker, (int)facing); // never free a level inside a physics callback
        await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
        await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
        if (!IsInsideTree()) return;
        Rig.Snap();
        if (_fade != null) await _fade.Uncover();
        if (!IsInsideTree()) return;
        Player.Frozen = AnyMenuOpen;
        Teleporting = false;
    }

    private void SwapForTravel(string scene, string marker, int facing) => LoadLevelNow(scene, marker, (Facing)facing);

    /// <summary>Puts Kit beside the bed in the house room (waking up, the 21:30 bedtime from anywhere).</summary>
    public World.Bed PutKitInBed()
    {
        LoadLevelNow(HouseRoomScene);
        var bed = Level?.FindChild("Bed", true, false) as World.Bed;
        if (bed != null)
        {
            Player.GlobalPosition = bed.WakePoint;
            Player.Face(Facing.Down);
        }
        Rig.Snap();
        return bed;
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

    /// <summary>The level's camera framing (load_level contract step 4): a room frames its floor from
    /// closer; an outdoor level uses its CameraLimitMin/Max markers. PlayerSpawn places Kit only for
    /// the first level (new game).</summary>
    private void ApplyLevelMarkers()
    {
        var level = Level;
        if (level == null) return;
        if (level is World.Building { IsRoom: true } room)
        {
            var c = room.Interior.GetCenter();
            Rig.LimitMin = c - new Vector2(0.6f, 0.6f);
            Rig.LimitMax = c + new Vector2(0.6f, 0.6f);
            Rig.Distance = room.CameraDistance > 0 ? room.CameraDistance : RoomCameraDistance;
            return;
        }
        Rig.Distance = _outdoorDistance > 0 ? _outdoorDistance : Rig.Distance;
        if (!_spawned && level.GetNodeOrNull<Marker3D>("PlayerSpawn") is { } spawn)
            Player.GlobalPosition = new Vector3(spawn.GlobalPosition.X, 0, spawn.GlobalPosition.Z);
        _spawned = true;
        if (level.GetNodeOrNull<Marker3D>("CameraLimitMin") is { } min && level.GetNodeOrNull<Marker3D>("CameraLimitMax") is { } max)
        {
            Rig.LimitMin = new Vector2(min.GlobalPosition.X, min.GlobalPosition.Z);
            Rig.LimitMax = new Vector2(max.GlobalPosition.X, max.GlobalPosition.Z);
        }
    }

    private bool _spawned;
}
