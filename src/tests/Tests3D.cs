using System.Linq;
using System.Threading.Tasks;
using Godot;
using MyFarmingGame.Autoloads;
using MyFarmingGame.Core;
using MyFarmingGame.Player;
using MyFarmingGame.UI;
using MyFarmingGame.World;
using MyFarmingGame.UI;

namespace MyFarmingGame.Tests;

/// <summary>Task 1.14: the 3D foundation — model library, camera rig, Main tree (3D), Kit in 3D.</summary>
public partial class TestRunner
{
    private const string MainPath = "res://scenes/main/Main.tscn";

    /// <summary>The open yard between the garden, the coop and the house (movement checks).</summary>
    private static readonly Vector3 Yard = new(32.6f, 0, 7.0f); // east of the coop, clear of its doorway (Task 2.2)

    private static bool Near(Vector3 a, Vector3 b, float eps = 0.01f) => a.DistanceTo(b) <= eps;

    // ---- rules (no scene) ----------------------------------------------------------------

    private void CheckModelLibraryRules()
    {
        ModelLibrary.ReloadTable();
        Check(ModelLibrary.PlaceholderNames.Contains("kit") && ModelLibrary.PlaceholderNames.Contains("test_box"),
            "models: placeholder table has kit and test_box");
        foreach (var name in ModelLibrary.PlaceholderNames)
            Check(ModelLibrary.PlaceholderParts(name).Count > 0, $"models: '{name}' has at least one part");

        var kit = ModelLibrary.Instantiate("kit", out var src);
        var kitMesh = kit.GetChildOrNull<MeshInstance3D>(0);
        Check(src == ModelLibrary.Source.Placeholder && kit.GetChildCount() == 1 && kitMesh?.Mesh is ArrayMesh am
              && am.GetSurfaceCount() == 1 && am.SurfaceGetArrays(0)[(int)Mesh.ArrayType.Color].AsColorArray().Length > 0,
            $"models: kit's {ModelLibrary.PlaceholderParts("kit").Count} placeholder parts are baked into one mesh with vertex colours (one draw call)");
        Check(kitMesh?.MaterialOverride == ModelLibrary.VertexColorMaterial, "models: baked placeholders share one vertex-colour material");
        var parts = ModelLibrary.PlaceholderParts("kit");
        Check(parts[0].Shape == "capsule" && Mathf.IsEqualApprox(parts[0].Size.Y, 1.7f) && Mathf.IsEqualApprox(parts[0].Offset.Y, 0.85f),
            "models: kit body is a 1.7 m capsule standing on the origin (feet)");

        var missing = ModelLibrary.Instantiate("no_such_model_xyz", out var src2);
        var fb = missing.GetChild<MeshInstance3D>(0);
        Check(src2 == ModelLibrary.Source.Fallback && fb.MaterialOverride is StandardMaterial3D m && m.AlbedoColor == ModelLibrary.FallbackColor,
            "models: an unknown name falls back to a magenta box");

        var kit2 = ModelLibrary.Instantiate("kit");
        Check(kit.GetChild<MeshInstance3D>(0).MaterialOverride == kit2.GetChild<MeshInstance3D>(0).MaterialOverride
              && kit.GetChild<MeshInstance3D>(0).Mesh == kit2.GetChild<MeshInstance3D>(0).Mesh,
            "models: two Kits share one mesh and one material (budget rule 2)");
        Check(ModelLibrary.MaterialFor(new Color("#a5784f")) == ModelLibrary.MaterialFor(new Color("#a5784f")),
            "models: equal colours share one material");
        kit.Free(); kit2.Free(); missing.Free();

        // A real model with the same name replaces the placeholder with no code change.
        const string root = "user://test_models";
        DirAccess.MakeDirRecursiveAbsolute(ProjectSettings.GlobalizePath(root + "/characters"));
        var real = new Node3D { Name = "RealKit" };
        real.AddChild(new MeshInstance3D { Name = "Mesh", Mesh = new SphereMesh() });
        real.GetChild(0).Owner = real;
        var packed = new PackedScene();
        packed.Pack(real);
        string path = root + "/characters/kit.tscn";
        Check(ResourceSaver.Save(packed, path) == Error.Ok, "models: test model saved");
        real.Free();
        ModelLibrary.ModelsRoot = root;
        try
        {
            Check(ModelLibrary.FindModelPath("kit") == path, $"models: the model file is found ({ModelLibrary.FindModelPath("kit")})");
            var swapped = ModelLibrary.Instantiate("kit", out var src3);
            Check(src3 == ModelLibrary.Source.Model && swapped.GetNodeOrNull("Mesh") is MeshInstance3D { Mesh: SphereMesh },
                "models: a model file named 'kit' replaces the capsule");
            swapped.Free();
            var nm = new NamedModel { ModelName = "kit" };
            AddChild(nm);
            Check(nm.Source == ModelLibrary.Source.Model && nm.Model?.GetNodeOrNull("Mesh") != null,
                "models: a NamedModel 'kit' picks up the model file too");
            nm.QueueFree();
            var missingButKnown = ModelLibrary.Instantiate("test_box", out var src4);
            Check(src4 == ModelLibrary.Source.Placeholder, "models: names without a model file still use placeholders");
            missingButKnown.Free();
        }
        finally
        {
            ModelLibrary.ModelsRoot = ModelLibrary.DefaultModelsRoot;
            DirAccess.RemoveAbsolute(ProjectSettings.GlobalizePath(path));
            DirAccess.RemoveAbsolute(ProjectSettings.GlobalizePath(root + "/characters"));
            DirAccess.RemoveAbsolute(ProjectSettings.GlobalizePath(root));
        }
        Check(ModelLibrary.FindModelPath("kit") == null, "models: no real kit model in the project yet (placeholder in use)");
        Check(ResourceLoader.Exists("res://assets/models/README.md") || FileAccess.FileExists("res://assets/models/README.md"),
            "models: assets/models/ has its README");
    }

    private void CheckCameraRigRules()
    {
        var rig = new CameraRig { Turn = 45, Corner = CameraCorner.SouthEast };
        float s = Mathf.Sqrt(0.5f);
        Check(Near(rig.ScreenUpOnGround, new Vector3(-s, 0, -s)), $"camera: SE 45° — screen up is north-west on the ground ({rig.ScreenUpOnGround})");
        Check(Near(rig.ScreenRightOnGround, new Vector3(s, 0, -s)), $"camera: SE 45° — screen right is north-east ({rig.ScreenRightOnGround})");
        Check(Near(rig.ScreenToGround(new Vector2(0, 0.5f)), new Vector3(s, 0, s) * 0.5f), "camera: half a stick down stays half, towards the camera");
        rig.Corner = CameraCorner.SouthWest;
        Check(Near(rig.ScreenUpOnGround, new Vector3(s, 0, -s)), "camera: SW 45° — screen up is north-east");
        rig.Corner = CameraCorner.SouthEast;

        // Tiles show as diamonds at 45° (grid axes lie on the screen diagonals).
        Vector3 north = Vector3.Forward;
        Check(Mathf.IsEqualApprox(Mathf.Abs(north.Dot(rig.ScreenUpOnGround)), Mathf.Abs(north.Dot(rig.ScreenRightOnGround))),
            "camera: at 45° north lies exactly between screen up and screen right (diamond tiles)");

        // Grid front from screen input (ground rule 4).
        rig.Turn = 37;
        Check(PlayerController.FacingFor(rig.ScreenToGround(new Vector2(0, -1)), Facing.Down) == Facing.Up, "front: turn 37° — up on screen faces north (Up)");
        Check(PlayerController.FacingFor(rig.ScreenToGround(new Vector2(1, 0)), Facing.Down) == Facing.Right, "front: turn 37° — right on screen faces east (Right)");
        Check(PlayerController.FacingFor(rig.ScreenToGround(new Vector2(0, 1)), Facing.Up) == Facing.Down, "front: turn 37° — down on screen faces south (Down)");
        Check(PlayerController.FacingFor(rig.ScreenToGround(new Vector2(-1, 0)), Facing.Up) == Facing.Left, "front: turn 37° — left on screen faces west (Left)");
        rig.Turn = 45;
        Vector3 up45 = rig.ScreenToGround(new Vector2(0, -1)); // north-west: between Up and Left
        Check(PlayerController.FacingFor(up45, Facing.Up) == Facing.Up && PlayerController.FacingFor(up45, Facing.Left) == Facing.Left,
            "front: turn 45° tie — up on screen keeps Up or Left if Kit already faces it");
        Check(PlayerController.FacingFor(up45, Facing.Down) is Facing.Up or Facing.Left, "front: turn 45° tie from Down turns to Up or Left");
        Check(Near(PlayerController.FacingVector(Facing.Up), Vector3.Forward) && Near(PlayerController.FacingVector(Facing.Right), Vector3.Right),
            "front: Up = north (−Z), Right = east (+X)");
        Check(Mathf.IsEqualApprox(PlayerController.YawFor(Vector3.Forward), 0f) && Mathf.IsEqualApprox(Mathf.Abs(PlayerController.YawFor(Vector3.Back)), Mathf.Pi),
            "model yaw: 0 looks north, π looks south");

        // Presets.
        rig.Tilt = 52; rig.CycleTilt();
        bool tiltOk = Mathf.IsEqualApprox(rig.Tilt, 60);
        rig.CycleTilt(); tiltOk &= Mathf.IsEqualApprox(rig.Tilt, 45);
        rig.CycleTilt(); tiltOk &= Mathf.IsEqualApprox(rig.Tilt, 52);
        Check(tiltOk, "camera presets: tilt cycles 52 → 60 → 45 → 52");
        rig.Turn = 45; rig.CycleTurn();
        bool turnOk = Mathf.IsEqualApprox(rig.Turn, 30);
        rig.CycleTurn(); turnOk &= Mathf.IsEqualApprox(rig.Turn, 37);
        rig.CycleTurn(); turnOk &= Mathf.IsEqualApprox(rig.Turn, 45);
        Check(turnOk, "camera presets: turn cycles 45 → 30 → 37 → 45");
        rig.ToggleCorner();
        Check(rig.Corner == CameraCorner.SouthWest, "camera presets: corner toggles to south-west");
        rig.ToggleProjection();
        Check(rig.Orthographic && rig.Describe().Contains("orthographic"), "camera presets: projection toggles to orthographic");
        rig.Free();
    }

    // ---- the 3D scene ------------------------------------------------------------------------

    private void CheckMainScene()
    {
        var packed = ResourceLoader.Load<PackedScene>(MainPath);
        Check(packed != null, "Main.tscn loads");
        if (packed == null) return;
        var main = packed.Instantiate();
        AddChild(main);
        CheckNode<Node>(main, ".", ProcessModeEnum.Always);
        CheckNode<Node>(main, "Systems", ProcessModeEnum.Inherit);
        CheckNode<Node3D>(main, "World", ProcessModeEnum.Pausable);
        CheckNode<Node>(main, "World/Level", ProcessModeEnum.Inherit);
        CheckNode<Node3D>(main, "World/Entities", ProcessModeEnum.Inherit);
        CheckNode<Node3D>(main, "World/Effects", ProcessModeEnum.Inherit);
        CheckNode<Node>(main, "Audio", ProcessModeEnum.Always);
        CheckNode<CanvasLayer>(main, "HUD", ProcessModeEnum.Pausable);
        CheckNode<CanvasLayer>(main, "Pause", ProcessModeEnum.WhenPaused);
        CheckNode<CanvasLayer>(main, "Transition", ProcessModeEnum.Always);
        CheckNode<CanvasLayer>(main, "Debug", ProcessModeEnum.Always);
        CheckLayer(main, "HUD", UILayers.Hud);
        CheckLayer(main, "Pause", UILayers.PauseMenu);
        CheckLayer(main, "Transition", UILayers.Transition);
        CheckLayer(main, "Debug", UILayers.Debug);

        Check(main.GetNode("World/Level").GetChildCount() == 1, "3D: exactly one level under World/Level");
        var level = main.GetNode("World/Level").GetChildOrNull<Node>(0);
        Check(level?.GetNodeOrNull<Marker3D>("PlayerSpawn") != null, "3D: level exposes a PlayerSpawn Marker3D");
        Check(level?.GetNodeOrNull<Marker3D>("CameraLimitMin") != null && level?.GetNodeOrNull<Marker3D>("CameraLimitMax") != null,
            "3D: level exposes CameraLimitMin/Max markers");
        Check(level?.FindChild("Player", true, false) == null && level?.FindChild("Camera3D", true, false) == null,
            "3D: level contains no player or camera");
        var rig = main.GetNodeOrNull<CameraRig>("World/Entities/CameraRig");
        Check(rig?.Camera != null && rig.Camera.Current, "3D: CameraRig with the current Camera3D lives in World/Entities");
        Check(main.GetNodeOrNull<WorldEnvironment>("World/Effects/WorldEnvironment") != null, "3D: WorldEnvironment in World/Effects");
        var sun = main.GetNodeOrNull<DirectionalLight3D>("World/Effects/Sun");
        Check(sun != null && sun.ShadowEnabled, "3D: one sun with shadows in World/Effects");
        Check(main.FindChildren("*", "Light3D", true, false).Count == 1, "3D: exactly one light (budget)");
        Check(main.GetNodeOrNull<Control>("HUD/UI") != null && main.GetNodeOrNull<PauseMenuView>("Pause/PauseMenu") != null,
            "3D: the 2D HUD and pause menu sit on top of the 3D world");
        var model = main.GetNodeOrNull<NamedModel>("World/Entities/Player/Model");
        Check(model != null && model.ModelName == "kit" && model.Source == ModelLibrary.Source.Placeholder,
            "3D: Kit's look is the NamedModel 'kit' (placeholder capsule)");
        if (SaveGame.Instance != null) SaveGame.Instance.Enabled = false;
        Check(level is FarmLevel, "3D: the level is the FarmLevel");
        Check(main.GetNodeOrNull<ClockDialView>("HUD/UI/ClockDial") != null && main.GetNodeOrNull<SleepPromptView>("HUD/UI/SleepPrompt") != null,
            "HUD has the ClockDial and the SleepPrompt");
        Check(main.GetNodeOrNull<DayTransitionView>("Transition/DayTransition") != null, "Transition layer has DayTransition");
        Check(main.GetNodeOrNull<SleepSequence>("Systems/SleepSequence") != null && main.GetNodeOrNull<SaveBinder>("Systems/SaveBinder") != null
              && main.GetNodeOrNull<OnboardingDirector>("Systems/OnboardingDirector") != null, "Systems has SleepSequence, SaveBinder and OnboardingDirector");
        var houseScene = ResourceLoader.Load<PackedScene>(Main.HouseRoomScene)?.Instantiate();
        AddChild(houseScene);
        var bedNode = houseScene?.FindChild("Bed", true, false) as Bed;
        Check(houseScene is Building { IsRoom: true } && bedNode != null && bedNode.IsInGroup(Bed.Group) && bedNode.IsInGroup(IInteractable.Group),
            "the house room scene is a room with a Bed in groups beds + interactables (Phase-002 Task 2.2)");
        houseScene?.Free();
        Check(FarmState.Instance != null && FarmState.Instance.Garden != null && FarmState.Instance.Herd("hen") != null && FarmState.Instance.Herd("cow") != null
              && FarmState.Instance.ProcessMode == ProcessModeEnum.Pausable, "FarmState autoload: garden, hens and cow, Pausable");
        Check(main.GetNodeOrNull<DayNightView>("World/Effects/DayNight") != null, "World/Effects has the DayNight light");
        Check(main.FindChildren("*", "CanvasModulate", true, false).Count == 0 && main.FindChildren("*", "Node2D", true, false).Count == 0,
            "no 2D world nodes are left (no Node2D, no CanvasModulate)");
        if (OS.IsDebugBuild())
            Check(main.GetNodeOrNull<Debugging.CameraDebugKeys>("Debug/CameraDebugKeys") != null, "3D: camera preset keys added in debug builds");
        main.QueueFree();
    }

    private PlayerController _kit;

    private async Task<Vector3> Drive3D(System.Action press, System.Action release, int frames = 30)
    {
        await Frames(15);
        _kit.GlobalPosition = Yard; // every move starts in the open yard
        await Frames(2);
        Vector3 start = _kit.GlobalPosition;
        press();
        await Frames(frames);
        Vector3 moved = _kit.GlobalPosition - start;
        release();
        return moved;
    }

    private void CheckGroundDirection(Vector3 moved, Vector3 dir, string what, float minM = 0.6f)
    {
        Check(moved.Length() > minM && moved.Normalized().Dot(dir.Normalized()) > 0.98f && Mathf.Abs(moved.Y) < 0.001f,
            $"{what} moves along {dir.Snapped(0.01f)} (moved {moved.Snapped(0.01f)})");
    }

    private async Task CheckMovement3D()
    {
        ResetTask14State();
        var main = ResourceLoader.Load<PackedScene>(MainPath).Instantiate<Main>();
        AddChild(main);
        await Frames(2);
        _kit = main.Player;
        var rig = main.Rig;
        var cam = rig.Camera;
        var spawn = main.Level.GetNode<Marker3D>("PlayerSpawn");
        Check(Near(_kit.GlobalPosition, new Vector3(spawn.GlobalPosition.X, 0, spawn.GlobalPosition.Z)), $"3D: Kit starts at PlayerSpawn ({_kit.GlobalPosition})");
        Check(Mathf.Abs(rig.MeasuredTilt - 45f) < 0.5f, $"camera: owner pick — looks down at 45° ({rig.MeasuredTilt:0.0}°), not straight down");
        Vector3 fwd = -cam.GlobalBasis.Z;
        Check(new Vector2(fwd.X, fwd.Z).Normalized().Dot(new Vector2(-1, -1).Normalized()) > 0.999f,
            "camera: turned 45°, looking north-west from the south-east corner");
        Check(cam.Projection == Camera3D.ProjectionType.Perspective && Mathf.IsEqualApprox(cam.Fov, 40f), "camera: perspective, 40° field of view");
        Basis camBasis = cam.GlobalBasis;
        Vector3 up = rig.ScreenUpOnGround, right = rig.ScreenRightOnGround;

        // Keyboard: WASD + arrows move relative to the screen.
        Vector3 straight = await Drive3D(() => Key(Godot.Key.D, true), () => Key(Godot.Key.D, false), 40);
        CheckGroundDirection(straight, right, "keyboard D (screen right)");
        Check(Mathf.Abs(straight.Length() - 2.5f * 40f / 60f) < 0.35f, $"Kit walks ≈ 2.5 m/s ({straight.Length():0.00} m in 40 frames)");
        Check(_kit.FrontCell == new Vector2I(Mathf.FloorToInt(_kit.FrontPoint.X), Mathf.FloorToInt(_kit.FrontPoint.Z)),
            "front: FrontCell is the whole tile under FrontPoint");
        Vector3 moved = await Drive3D(() => Key(Godot.Key.W, true), () => Key(Godot.Key.W, false));
        CheckGroundDirection(moved, up, "keyboard W (screen up)");
        Vector2 sBefore = cam.UnprojectPosition(_kit.GlobalPosition - moved), sAfter = cam.UnprojectPosition(_kit.GlobalPosition);
        Check((sAfter - sBefore).Normalized().Dot(Vector2.Up) > 0.95f, $"keyboard W moves Kit up the screen ({(sAfter - sBefore).Round()})");
        Check(_kit.Facing is Facing.Up or Facing.Left, $"keyboard W: front is a grid direction next to screen up ({_kit.Facing})");
        await Frames(30);
        float yaw = _kit.ModelYaw;
        Check(Mathf.Abs(Mathf.AngleDifference(yaw, PlayerController.YawFor(up))) < 0.1f,
            $"model turned smoothly to the walking direction (yaw {Mathf.RadToDeg(yaw):0}°)");
        moved = await Drive3D(() => Key(Godot.Key.Left, true), () => Key(Godot.Key.Left, false));
        CheckGroundDirection(moved, -right, "arrow Left (screen left)");
        moved = await Drive3D(() => Key(Godot.Key.Down, true), () => Key(Godot.Key.Down, false));
        CheckGroundDirection(moved, -up, "arrow Down (screen down)");
        await Frames(20);

        // 8-way: diagonals as fast as straight lines.
        Vector3 diag = await Drive3D(() => { Key(Godot.Key.W, true); Key(Godot.Key.D, true); },
            () => { Key(Godot.Key.W, false); Key(Godot.Key.D, false); }, 40);
        CheckGroundDirection(diag, up + right, "keyboard W+D (screen up-right = due north at 45°)");
        Check(Mathf.Abs(diag.Length() - straight.Length()) < 0.05f * straight.Length(),
            $"diagonal speed equals straight speed ({diag.Length():0.00} vs {straight.Length():0.00})");
        Check(_kit.Facing == Facing.Up, $"screen up-right at 45° faces the grid's north exactly ({_kit.Facing})");
        await Frames(20);

        // Gamepad: stick (full, half) and D-pad.
        moved = await Drive3D(() => Stick(JoyAxis.LeftX, -1f), () => Stick(JoyAxis.LeftX, 0f));
        CheckGroundDirection(moved, -right, "gamepad left stick left");
        await Frames(20);
        Vector3 half = await Drive3D(() => Stick(JoyAxis.LeftY, 0.5f), () => Stick(JoyAxis.LeftY, 0f));
        CheckGroundDirection(half, -up, "gamepad half-tilted stick", 0.2f);
        Check(half.Length() < 0.75f * straight.Length() * 30f / 40f, $"half tilt walks slower ({half.Length():0.00} m vs {straight.Length() * 30f / 40f:0.00} m)");
        await Frames(20);
        moved = await Drive3D(() => Pad(JoyButton.DpadUp, true), () => Pad(JoyButton.DpadUp, false));
        CheckGroundDirection(moved, up, "gamepad D-pad Up");
        moved = await Drive3D(() => Pad(JoyButton.DpadRight, true), () => Pad(JoyButton.DpadRight, false));
        CheckGroundDirection(moved, right, "gamepad D-pad Right");
        await Frames(30);

        // Camera: trails Kit, then settles; tilt and turn never change.
        _kit.GlobalPosition = Yard;
        rig.Snap();
        await Frames(5);
        Key(Godot.Key.D, true);
        await Frames(30);
        float lag = rig.Focus.DistanceTo(_kit.Feet);
        Key(Godot.Key.D, false);
        await Frames(150);
        float settled = rig.Focus.DistanceTo(_kit.Feet);
        Check(lag > 0.05f && settled < 0.02f, $"camera trails Kit ({lag:0.00} m) then settles ({settled:0.000} m)");
        Check(cam.GlobalBasis.IsEqualApprox(camBasis), "camera tilt and turn unchanged after walking (never rotates)");

        // Click-to-move: a click on the ground walks Kit to the point under the cursor.
        await Frames(10);
        Vector3 goal = _kit.Feet + new Vector3(-2, 0, 1.5f);
        Vector2 screen = cam.UnprojectPosition(goal);
        var vp = GetViewport();
        Vector3? hit = _kit.GroundAt(screen);
        Check(hit.HasValue && Near(hit.Value, goal, 0.02f), $"camera ray hits the ground at the clicked point ({hit})");
        vp.PushInput(new InputEventMouseMotion { Position = screen, GlobalPosition = screen }, true);
        vp.PushInput(new InputEventMouseButton { ButtonIndex = MouseButton.Left, Pressed = true, Position = screen, GlobalPosition = screen }, true);
        vp.PushInput(new InputEventMouseButton { ButtonIndex = MouseButton.Left, Pressed = false, Position = screen, GlobalPosition = screen }, true);
        Check(_kit.HasMoveTarget, "left-click on the ground starts click-to-move");
        await Frames(120);
        Check(_kit.Feet.DistanceTo(goal) <= _kit.ArriveDistance + 0.05f && !_kit.HasMoveTarget,
            $"click-to-move arrives at the clicked point (off by {_kit.Feet.DistanceTo(goal):0.00} m)");

        bool arrived = false;
        Vector3 g2 = _kit.Feet + new Vector3(1.5f, 0, -1f);
        _kit.WalkTo(g2, () => arrived = true);
        await Frames(90);
        Check(arrived && _kit.Feet.DistanceTo(g2) <= _kit.ArriveDistance + 0.01f, "WalkTo reaches the target and runs the act callback");
        _kit.WalkTo(_kit.Feet + new Vector3(5, 0, 0));
        await Frames(5);
        Key(Godot.Key.S, true);
        await Frames(2);
        Key(Godot.Key.S, false);
        Check(!_kit.HasMoveTarget, "keyboard movement cancels a click target");
        await Frames(20);

        // Walls block Kit (the house's north wall west of the door).
        _kit.GlobalPosition = new Vector3(22.5f, 0, 7.4f);
        await Frames(2);
        _kit.WalkTo(new Vector3(22.5f, 0, 9.5f));
        await Frames(120);
        Check(_kit.Feet.Z < 7.95f && !_kit.HasMoveTarget, $"a wall blocks Kit and the walk gives up ({_kit.Feet.Z:0.00})");

        // Menus freeze Kit (bag on gamepad Y).
        await Tap(JoyButton.Y);
        await Frames(3);
        var bag = main.GetNodeOrNull<BagView>("HUD/UI/Bag");
        Check(bag != null && bag.IsOpen && _kit.Frozen, "3D: gamepad Y opens the bag and Kit freezes");
        await Tap(JoyButton.B);
        await Frames(3);
        Check(bag != null && !bag.IsOpen && !_kit.Frozen, "3D: B closes the bag and Kit can walk again");

        // The player's corner switch (owner 2026-11-21): right stick left/right or Q, any pad.
        var ev = InputMap.ActionGetEvents(CameraRig.SwitchAction);
        Check(ev.Any(e => e is InputEventJoypadMotion { Axis: JoyAxis.RightX }) && ev.Any(e => e is InputEventKey { PhysicalKeycode: Godot.Key.Q }),
            "camera_switch bound to the right stick (any pad) and Q");
        _kit.GlobalPosition = Yard;
        rig.Snap();
        await Frames(2);
        Vector3 upBefore = rig.ScreenUpOnGround;
        await Press(Godot.Key.Q);
        Check(rig.Corner == CameraCorner.SouthWest && rig.IsSwitching, "Q switches to the south-west corner with a glide");
        await Frames(60);
        Check(!rig.IsSwitching && Near(rig.ScreenUpOnGround, new Vector3(-upBefore.X, 0, upBefore.Z)),
            $"the glide ends looking from the south-west ({rig.ScreenUpOnGround.Snapped(0.01f)})");
        Check(Mathf.Abs(rig.MeasuredTilt - 45f) < 0.5f, "the corner switch keeps the tilt");
        moved = await Drive3D(() => Key(Godot.Key.W, true), () => Key(Godot.Key.W, false));
        CheckGroundDirection(moved, rig.ScreenUpOnGround, "after the switch W still moves up the screen");
        Stick(JoyAxis.RightX, 1f);
        await Frames(3);
        Stick(JoyAxis.RightX, 0f);
        await Frames(60);
        Check(rig.Corner == CameraCorner.SouthEast && Near(rig.ScreenUpOnGround, upBefore), "gamepad right stick switches back to the south-east");
        await Tap(JoyButton.Y);
        await Frames(3);
        await Press(Godot.Key.Q);
        Check(rig.Corner == CameraCorner.SouthEast, "no corner switch while the bag is open");
        await Tap(JoyButton.B);
        await Frames(3);

        // The owner's preset keys (debug builds): 1 / F9 tilt, 2 / F10 turn, 3 / F11 corner, 4 / F12 projection.
        if (OS.IsDebugBuild())
        {
            float t0 = rig.Tilt, u0 = rig.Turn;
            await Press(Godot.Key.Key1);
            Check(!Mathf.IsEqualApprox(rig.Tilt, t0), $"debug key 1 changes the tilt ({t0} → {rig.Tilt})");
            await Press(Godot.Key.F10);
            Check(!Mathf.IsEqualApprox(rig.Turn, u0), $"debug key F10 changes the turn ({u0} → {rig.Turn})");
            var c0 = rig.Corner;
            await Press(Godot.Key.Key3);
            Check(rig.Corner != c0, "debug key 3 toggles the corner");
            await Press(Godot.Key.Key4);
            Check(rig.Orthographic, "debug key 4 toggles the projection");
            await Press(Godot.Key.Key4);
            var label = main.GetNodeOrNull<Label>("Debug/CameraDebugKeys/CameraLabel");
            Check(label != null && label.Text.Contains($"tilt {rig.Tilt:0}°"), $"camera label shows the new setting ({label?.Text.Split('\n')[0]})");
        }

        // Presets change the view live (owner picks one); limits keep the focus inside the level.
        _kit.GlobalPosition = Yard; // open ground
        rig.Tilt = 60; rig.Turn = 30; rig.Corner = CameraCorner.SouthWest; rig.Snap();
        await Frames(2);
        Vector3 f2 = -cam.GlobalBasis.Z;
        Check(Mathf.Abs(rig.MeasuredTilt - 60f) < 0.5f && f2.X > 0 && f2.Z < 0, "camera presets: 60° from the south-west looks north-east");
        moved = await Drive3D(() => Key(Godot.Key.W, true), () => Key(Godot.Key.W, false));
        CheckGroundDirection(moved, rig.ScreenUpOnGround, "after a preset change W still moves up the screen");
        rig.Orthographic = true; rig.Snap();
        Check(cam.Projection == Camera3D.ProjectionType.Orthogonal, "camera presets: orthographic projection");
        rig.Orthographic = false; rig.Tilt = 52; rig.Turn = 45; rig.Corner = CameraCorner.SouthEast;
        _kit.GlobalPosition = new Vector3(0.5f, 0, 0.5f);
        rig.Snap();
        Check(rig.Focus.X >= rig.LimitMin.X - 0.001f && rig.Focus.Z >= rig.LimitMin.Y - 0.001f, $"camera focus stays inside the level limits ({rig.Focus})");

        // Budget (project-structure.md §16). Headless runs draw nothing, so this only bites windowed.
        await Frames(3);
        double draws = Performance.GetMonitor(Performance.Monitor.RenderTotalDrawCallsInFrame);
        double prims = Performance.GetMonitor(Performance.Monitor.RenderTotalPrimitivesInFrame);
        Check(draws <= 300 && prims <= 150000, $"budget: {draws} draw calls ≤ 300, {prims} triangles ≤ 150k");

        main.QueueFree();
        await Frames(2);
        _kit = null;
    }
}
