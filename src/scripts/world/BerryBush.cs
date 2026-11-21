using Godot;
using MyFarmingGame.Autoloads;
using MyFarmingGame.Core;

namespace MyFarmingGame.World;

/// <summary>
/// A wild berry bush on the lane (Task 1.6, places.md §2.1). Interact → one handful of wild berries
/// into the bag; the bush is bare until it regrows <see cref="RegrowDays"/> days later (counted at
/// the new day's Dawn). Group <see cref="Group"/>; the node name is its save id (Task 1.10).
///
/// The KayKit bush model carries no berries of its own (its UVs only reach the pack's green swatch
/// in <c>berry_bush_forest_texture.png</c>), so the berries are a <see cref="MultiMeshInstance3D"/>
/// of small red spheres stuck onto the bush: one model for both states, one draw call for the
/// berries, hidden the moment Kit picks and shown again on regrow.
/// </summary>
public partial class BerryBush : Prop
{
    public const string Group = "berry_bushes";
    public const int RegrowDays = 2;

    public bool HasBerries { get; private set; } = true;
    /// <summary>Day the berries were picked (0 = never).</summary>
    public int PickedOnDay { get; private set; }

    /// <summary>True while the berry spheres are on the bush (tests, later feedback).</summary>
    public bool BerriesVisible => _berries != null && _berries.Visible;

    [Signal] public delegate void PickedEventHandler();

    /// <summary>Berry colour, matching <c>ph_item_berries.png</c> (#b8264a).</summary>
    private static readonly Color BerryColor = new("#b8264a");

    /// <summary>Berry radius in metres (≈ 0.12 m across — reads at a glance on the 1.2 m bush).</summary>
    private const float BerryRadius = 0.06f;

    /// <summary>Where the berries sit on the bush, in local metres (the bush is ~1.2 m wide and
    /// 0.8 m tall). Just inside the foliage so they read as growing on it, not floating.</summary>
    private static readonly (Vector3 At, float Scale)[] BerrySpots =
    {
        (new Vector3( 0.44f, 0.30f,  0.22f), 1.00f),
        (new Vector3(-0.42f, 0.40f,  0.18f), 1.10f),
        (new Vector3( 0.14f, 0.55f,  0.46f), 0.95f),
        (new Vector3(-0.20f, 0.32f,  0.45f), 1.05f),
        (new Vector3( 0.50f, 0.44f, -0.14f), 1.00f),
        (new Vector3(-0.46f, 0.50f, -0.22f), 0.95f),
        (new Vector3( 0.10f, 0.62f, -0.30f), 1.10f),
        (new Vector3( 0.30f, 0.26f, -0.42f), 1.00f),
        (new Vector3(-0.12f, 0.64f,  0.30f), 1.05f),
        (new Vector3( 0.00f, 0.46f, -0.50f), 0.95f),
    };

    private GameClock _clock;
    private MultiMeshInstance3D _berries;

    public override bool Usable => HasBerries;

    public BerryBush()
    {
        ModelName = "berry_bush";       // one model for both states; the berries do the swapping
        SpriteName = "berry_bush";
        PromptSprite = "item_berries";
        BodySize = new Vector3(1.1f, 0.8f, 1.1f); // KayKit bush is ~1.2 m wide (License.txt in assets/models/world)
        ClickSize = new Vector3(1f, 1f, 1f);
        PromptHeight = 1.2f;
        CastsShadow = false;
    }

    public override void _Ready()
    {
        base._Ready();
        AddToGroup(Group);
        _clock = GameClock.Instance;
        if (_clock != null) _clock.DayStarted += OnDayStarted;
        Production.Register(this, () => new[] { (ItemIds.Berries, 1) });
        _berries = BuildBerries();
        AddChild(_berries);
        Refresh();
    }

    public override void _ExitTree()
    {
        if (_clock != null && IsInstanceValid(_clock)) _clock.DayStarted -= OnDayStarted;
        Production.Unregister(this);
    }

    public override void Interact(Node3D player)
    {
        if (!HasBerries) return;
        HasBerries = false;
        PickedOnDay = GameClock.Instance?.Day ?? 1;
        Inventory.Instance?.Add(ItemIds.Berries);
        Puff.At(this, GlobalPosition + new Vector3(0, 0.5f, 0), new Color("#3b4fd0"), 12, 1.8f);
        Refresh();
        EmitSignal(SignalName.Picked);
    }

    private void OnDayStarted(int day) => NewDay(day);

    /// <summary>Regrows once <see cref="RegrowDays"/> days have passed since picking.</summary>
    public void NewDay(int day)
    {
        if (!HasBerries && day - PickedOnDay >= RegrowDays)
        {
            HasBerries = true;
            Refresh();
        }
    }

    /// <summary>Save/load (Task 1.10).</summary>
    public void SetState(bool hasBerries, int pickedOnDay)
    {
        HasBerries = hasBerries;
        PickedOnDay = pickedOnDay;
        Refresh();
    }

    private void Refresh()
    {
        if (_berries != null) _berries.Visible = HasBerries;
    }

    /// <summary>The berry spheres as one MultiMesh: a single draw call for the whole bush and no
    /// shadow (this prop casts none — budget project-structure.md §16), built the same way as
    /// <c>FarmLevel</c>'s grass tufts.</summary>
    private static MultiMeshInstance3D BuildBerries()
    {
        var mesh = new SphereMesh
        {
            Radius = BerryRadius,
            Height = BerryRadius * 2f,
            RadialSegments = 12,
            Rings = 6,
            Material = new StandardMaterial3D { AlbedoColor = BerryColor, Roughness = 0.6f },
        };
        var multi = new MultiMesh
        {
            TransformFormat = MultiMesh.TransformFormatEnum.Transform3D,
            Mesh = mesh,
            InstanceCount = BerrySpots.Length,
        };
        for (int i = 0; i < BerrySpots.Length; i++)
        {
            var (at, scale) = BerrySpots[i];
            multi.SetInstanceTransform(i, new Transform3D(Basis.FromScale(new Vector3(scale, scale, scale)), at));
        }
        return new MultiMeshInstance3D
        {
            Name = "Berries",
            Multimesh = multi,
            CastShadow = GeometryInstance3D.ShadowCastingSetting.Off,
        };
    }
}
