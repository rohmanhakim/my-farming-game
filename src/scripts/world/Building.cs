using System.Collections.Generic;
using Godot;
using MyFarmingGame.Core;
using MyFarmingGame.Player;

namespace MyFarmingGame.World;

/// <summary>Which wall has the door. Top = north (−Z), Bottom = south (+Z).</summary>
public enum DoorSide { Top, Bottom, None }

/// <summary>
/// A building entered on the same map (Task 1.5; 3D since Task 1.15; Phase-001 §1 Indoors): floor,
/// four walls with a door gap, a roof (model by name, <see cref="RoofModel"/>), and an
/// <see cref="IndoorZone"/> over the floor. While Kit is inside, the roof and the walls facing the
/// camera fade out and the clock stops. Outdoors, while the building stands between the camera and
/// Kit, its roof and camera-side walls fade too, so Kit is never lost behind it.
///
/// The node's position is the footprint's north-west corner; size is in tiles (1 tile = 1 m).
/// Things inside (bed, phone, feed bins, animals) are children placed in the scene.
/// Walls and floor are built from colours; each faded piece has its own material (few buildings).
/// </summary>
public partial class Building : Node3D
{
    public const float WallThickness = 0.19f;   // 2D: 12 px
    public const float WallHeight = 2.2f;

    [Export] public Vector2I SizeTiles { get; set; } = new(6, 4);
    [Export] public DoorSide Door { get; set; } = DoorSide.Top;
    [Export] public int DoorTiles { get; set; } = 2;
    [Export] public string ZoneName { get; set; } = "house";
    [Export] public string RoofModel { get; set; } = "roof_house";
    [Export] public Color WallColor { get; set; } = new(0.94f, 0.88f, 0.76f);
    [Export] public Color FloorColor { get; set; } = new(0.62f, 0.45f, 0.30f);
    /// <summary>Adds a window on the south wall that glows at night (group night_windows).</summary>
    [Export] public bool HasWindow { get; set; }

    /// <summary>A real room scene (Phase-002 Task 2.2, owner 2026-11-21: a separate scene for each room):
    /// no roof, dark surroundings, the walls facing the camera always faded while Kit is inside, an
    /// "Entry" marker just inside the door, and an exit doorway to <see cref="ExitScene"/>.</summary>
    [Export] public bool IsRoom { get; set; }

    /// <summary>For a room: how far the camera stands inside it (0 = Main's default). Small rooms
    /// come closer so they fill the screen (owner 2026-11-21).</summary>
    [Export] public float CameraDistance { get; set; }

    /// <summary>For a building outside: the room scene its doorway leads into ("" = not enterable
    /// from here; without it the building is entered on the map as in Phase-001).</summary>
    [Export] public string RoomScene { get; set; } = "";

    /// <summary>For a room: the scene its doorway leads back to, and the marker Kit lands on there.</summary>
    [Export] public string ExitScene { get; set; } = "";
    [Export] public string ExitMarker { get; set; } = "";

    /// <summary>Name of the marker outside a building's door (where Kit comes out of its room).</summary>
    public string DoorMarkerName => "Door_" + Name;

    /// <summary>Marker name just inside a room's door.</summary>
    public const string EntryMarker = "Entry";

    /// <summary>The doorway Kit walks into (into the room outside, out of it inside).</summary>
    public DoorTrigger Doorway { get; private set; }

    /// <summary>Group of buildings that are rooms (Main adjusts the camera inside them).</summary>
    public const string RoomGroup = "rooms";

    public IndoorZone Zone { get; private set; }
    public NamedModel Roof { get; private set; }

    /// <summary>The fading pieces: roof and the four walls (north, south, west, east).</summary>
    private readonly List<Fader> _walls = new();
    private Fader _roofFader;
    private readonly List<Vector3> _wallNormals = new();

    /// <summary>Footprint size in metres (x = width on X, y = depth on Z).</summary>
    public Vector2 Size => new(SizeTiles.X, SizeTiles.Y);

    /// <summary>A point just outside the door (on the ground).</summary>
    public Vector3 DoorOutside => GlobalPosition + new Vector3(Size.X / 2, 0, Door == DoorSide.Top ? -0.625f : Size.Y + 0.625f);

    /// <summary>A point just inside the door (on the ground).</summary>
    public Vector3 DoorInside => GlobalPosition + new Vector3(Size.X / 2, 0, Door == DoorSide.Top ? 0.75f : Size.Y - 1.0f);

    /// <summary>The middle of the door gap on the wall line (on the ground).</summary>
    public Vector3 DoorPoint => GlobalPosition + new Vector3(Size.X / 2, 0, Door == DoorSide.Top ? 0.1f : Size.Y - 0.1f);

    /// <summary>True when a ground point lies on this building's floor.</summary>
    public bool Contains(Vector3 p) => Interior.HasPoint(new Vector2(p.X, p.Z));

    /// <summary>The floor inside the walls as (x, z, width, depth) on the ground.</summary>
    public Rect2 Interior => new(GlobalPosition.X + WallThickness, GlobalPosition.Z + WallThickness, Size.X - 2 * WallThickness, Size.Y - 2 * WallThickness);

    /// <summary>The whole building, roof included, in world space (occlusion test).</summary>
    public Aabb Bounds => new(GlobalPosition, new Vector3(Size.X, WallHeight + 1.6f, Size.Y));

    /// <summary>True while the roof is faded (Kit inside or hidden behind it).</summary>
    public bool RoofFaded => _roofFader == null || _roofFader.Target < 0.5f;

    /// <summary>True when the wall facing <paramref name="normal"/> (e.g. Vector3.Back = south) is faded.</summary>
    public bool WallFaded(Vector3 normal)
    {
        for (int i = 0; i < _walls.Count; i++)
            if (_wallNormals[i].IsEqualApprox(normal)) return _walls[i].Target < 0.5f;
        return false;
    }

    public override void _Ready()
    {
        float w = Size.X, d = Size.Y, t = WallThickness;
        var floor = new MeshInstance3D
        {
            Name = "Floor", Mesh = new BoxMesh { Size = new Vector3(w, 0.04f, d) },
            MaterialOverride = ModelLibrary.MaterialFor(FloorColor), Position = new Vector3(w / 2, 0.02f, d / 2),
            CastShadow = GeometryInstance3D.ShadowCastingSetting.Off, // flat: nothing to cast
        };
        AddChild(floor);

        var walls = new StaticBody3D { Name = "Walls" };
        AddChild(walls);
        // North and south walls run along X (one may have the door gap); west and east along Z.
        AddWallSide(walls, "WallNorth", Vector3.Forward, 0, d: t, along: w, z: 0, hasDoor: Door == DoorSide.Top);
        AddWallSide(walls, "WallSouth", Vector3.Back, 0, d: t, along: w, z: d - t, hasDoor: Door == DoorSide.Bottom);
        AddWallSide(walls, "WallWest", Vector3.Left, 0, d: d, along: t, z: 0, hasDoor: false);
        AddWallSide(walls, "WallEast", Vector3.Right, w - t, d: d, along: t, z: 0, hasDoor: false);

        if (!string.IsNullOrEmpty(RoofModel))
        {
            Roof = new NamedModel { Name = "Roof", ModelName = RoofModel, Position = new Vector3(w / 2, 0, d / 2) };
            AddChild(Roof);
            _roofFader = new Fader(Roof);
        }
        if (IsRoom)
        {
            AddToGroup(RoomGroup);
            // Dark surroundings so the room reads as "inside", not as a box floating in the sky.
            AddChild(new MeshInstance3D
            {
                Name = "Dark", Mesh = new BoxMesh { Size = new Vector3(40, 0.02f, 40) },
                MaterialOverride = ModelLibrary.MaterialFor(new Color(0.08f, 0.07f, 0.06f)),
                Position = new Vector3(w / 2, -0.03f, d / 2), CastShadow = GeometryInstance3D.ShadowCastingSetting.Off,
            });
        }

        if (HasWindow)
        {
            var glass = new StandardMaterial3D { AlbedoColor = new Color("#5a7ea8"), Emission = new Color("#ffd27a") };
            var win = new MeshInstance3D
            {
                Name = "Window", Mesh = new BoxMesh { Size = new Vector3(0.8f, 0.6f, 0.04f) },
                MaterialOverride = glass, Position = new Vector3(w * 0.25f, 1.3f, d + 0.01f),
            };
            win.AddToGroup(UI.DayNightView.WindowGroup);
            // The window belongs to the south wall's look so it fades with it.
            int south = _wallNormals.IndexOf(Vector3.Back);
            (south >= 0 ? _walls[south].Root : this).AddChild(win);
            win.Position = new Vector3(w * 0.25f, 1.3f, d + 0.01f) - (south >= 0 ? ((Node3D)_walls[south].Root).Position : Vector3.Zero);
            if (south >= 0) _walls[south].Refresh();
        }

        if (Door == DoorSide.None) return;
        float doorZ = Door == DoorSide.Top ? 0 : d;
        float outZ = Door == DoorSide.Top ? -0.625f : d + 0.625f;
        float inZ = Door == DoorSide.Top ? 0.75f : d - 1.0f;
        if (IsRoom)
        {
            AddChild(new Marker3D { Name = EntryMarker, Position = new Vector3(w / 2, 0, inZ) });
            if (!string.IsNullOrEmpty(ExitScene))
                Doorway = MakeDoorway(ExitScene, ExitMarker, Door == DoorSide.Top ? Player.Facing.Up : Player.Facing.Down);
        }
        else if (!string.IsNullOrEmpty(RoomScene))
        {
            // Entered through its room scene, not on the map: a doorway and a marker for coming back out.
            AddChild(new Marker3D { Name = DoorMarkerName, Position = new Vector3(w / 2, 0, outZ) });
            Doorway = MakeDoorway(RoomScene, EntryMarker, Door == DoorSide.Top ? Player.Facing.Down : Player.Facing.Up);
            return;
        }

        // Inside = the floor between the walls.
        Zone = new IndoorZone { Name = "IndoorZone", ZoneName = ZoneName, Building = this };
        Zone.AddChild(new CollisionShape3D
        {
            Shape = new BoxShape3D { Size = new Vector3(w - 2 * t, 2f, d - 2 * t) },
            Position = new Vector3(w / 2, 1f, d / 2),
        });
        AddChild(Zone);
    }

    /// <summary>The doorway box across this building's door gap, leading to another scene.</summary>
    private DoorTrigger MakeDoorway(string scene, string marker, Player.Facing facing)
    {
        var t = new DoorTrigger { Name = "Doorway", TargetScene = scene, TargetMarker = marker, Facing = facing };
        float z = Door == DoorSide.Top ? 0.1f : Size.Y - 0.1f;
        t.AddChild(new CollisionShape3D { Shape = new BoxShape3D { Size = new Vector3(DoorTiles - 0.2f, 2f, 0.4f) }, Position = new Vector3(Size.X / 2, 1f, z) });
        AddChild(t);
        return t;
    }

    /// <summary>One wall side: a solid body and a look (split around the door gap when it has one).</summary>
    private void AddWallSide(StaticBody3D body, string name, Vector3 normal, float x, float d, float along, float z, bool hasDoor)
    {
        var look = new Node3D { Name = name, Position = new Vector3(x, 0, z) };
        AddChild(look);
        var pieces = new List<Rect2>(); // local (x, z, w, d) relative to the wall side
        bool runsAlongX = along > d;
        if (hasDoor && runsAlongX)
        {
            float gapW = DoorTiles, gapX = (along - gapW) / 2;
            pieces.Add(new Rect2(0, 0, gapX, d));
            pieces.Add(new Rect2(gapX + gapW, 0, along - gapX - gapW, d));
        }
        else
            pieces.Add(new Rect2(0, 0, along, d));
        foreach (var r in pieces)
        {
            if (r.Size.X <= 0 || r.Size.Y <= 0) continue;
            var center = new Vector3(r.GetCenter().X, WallHeight / 2, r.GetCenter().Y);
            look.AddChild(new MeshInstance3D
            {
                Mesh = new BoxMesh { Size = new Vector3(r.Size.X, WallHeight, r.Size.Y) },
                MaterialOverride = new StandardMaterial3D { AlbedoColor = WallColor, Roughness = 0.9f },
                Position = center,
            });
            body.AddChild(new CollisionShape3D
            {
                Shape = new BoxShape3D { Size = new Vector3(r.Size.X, WallHeight, r.Size.Y) },
                Position = look.Position + center,
            });
        }
        _walls.Add(new Fader(look));
        _wallNormals.Add(normal);
    }

    /// <summary>Ground direction from this building towards the camera (null without a camera rig).</summary>
    private Vector3? CameraSide()
    {
        var cam = GetViewport()?.GetCamera3D();
        if (cam?.GetParent() is CameraRig rig) return rig.BackDirection;
        if (cam == null) return null;
        Vector3 b = cam.GlobalBasis.Z;
        return new Vector3(b.X, 0, b.Z).Normalized();
    }

    private bool _kitInside;
    private bool _hidingKit;

    /// <summary>Called by the indoor zone.</summary>
    public void SetKitInside(bool inside)
    {
        _kitInside = inside;
        UpdateFades();
    }

    public override void _Process(double delta)
    {
        bool hiding = false;
        var cam = GetViewport()?.GetCamera3D();
        if (!_kitInside && !IsRoom && cam != null && GetTree().GetFirstNodeInGroup(PlayerController.Group) is Node3D kit)
        {
            // Kit's chest → camera: does the segment pass through the building?
            Vector3 chest = kit.GlobalPosition + new Vector3(0, 0.9f, 0);
            hiding = Bounds.IntersectsSegment(chest, cam.GlobalPosition);
        }
        if (hiding != _hidingKit) { _hidingKit = hiding; UpdateFades(); }
        else if (_kitInside || _hidingKit) UpdateFades(); // the camera corner may switch
        _roofFader?.Step((float)delta);
        foreach (var f in _walls) f.Step((float)delta);
    }

    /// <summary>Inside or hiding Kit → roof and the walls facing the camera fade; otherwise all back.</summary>
    private void UpdateFades()
    {
        bool open = _kitInside || _hidingKit;
        if (_roofFader != null) _roofFader.Target = open ? 0f : 1f;
        Vector3? side = CameraSide();
        for (int i = 0; i < _walls.Count; i++)
        {
            bool cameraSide = side.HasValue && _wallNormals[i].Dot(side.Value) > 0.05f;
            _walls[i].Target = open && cameraSide ? 0f : 1f;
        }
    }

    /// <summary>Instant fades (tests, waking up).</summary>
    public void FinishFades()
    {
        _roofFader?.Step(10f);
        foreach (var f in _walls) f.Step(10f);
    }
}

/// <summary>
/// Fades every mesh under a node in and out (Task 1.15): each mesh gets its own copy of its
/// material; alpha blending is switched on only while partly faded and the node is hidden when
/// fully faded (performance budget rule 3: no permanently transparent meshes).
/// </summary>
public sealed class Fader
{
    public const float Seconds = 0.25f;
    public Node Root { get; }
    public float Target { get; set; } = 1f;
    public float Alpha { get; private set; } = 1f;
    private readonly List<StandardMaterial3D> _mats = new();

    public Fader(Node root) { Root = root; Refresh(); }

    /// <summary>Takes ownership of the materials under <see cref="Root"/> (call again after a model swap).</summary>
    public void Refresh()
    {
        _mats.Clear();
        Collect(Root);
    }

    private void Collect(Node n)
    {
        if (n is MeshInstance3D mi)
        {
            Material src = mi.MaterialOverride ?? (mi.Mesh != null && mi.Mesh.GetSurfaceCount() > 0 ? mi.Mesh.SurfaceGetMaterial(0) : null);
            var own = src?.Duplicate() as StandardMaterial3D ?? new StandardMaterial3D();
            mi.MaterialOverride = own;
            _mats.Add(own);
        }
        foreach (Node c in n.GetChildren()) Collect(c);
    }

    public void Step(float delta)
    {
        if (Mathf.IsEqualApprox(Alpha, Target)) return;
        Alpha = Mathf.MoveToward(Alpha, Target, delta / Seconds);
        bool partial = Alpha > 0.001f && Alpha < 0.999f;
        foreach (var m in _mats)
        {
            m.Transparency = partial ? BaseMaterial3D.TransparencyEnum.Alpha : BaseMaterial3D.TransparencyEnum.Disabled;
            var c = m.AlbedoColor;
            m.AlbedoColor = new Color(c.R, c.G, c.B, Alpha);
        }
        if (Root is Node3D n3) n3.Visible = Alpha > 0.001f;
    }
}
