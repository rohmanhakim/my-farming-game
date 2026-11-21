using Godot;
using MyFarmingGame.Autoloads;

namespace MyFarmingGame.World;

/// <summary>
/// Kit's bed (Task 1.3; a 3D <see cref="Prop"/> since Task 1.15). Walk up and press <c>interact</c>
/// (A / Space), or click it, to get the "Go to sleep?" prompt. Kit also wakes up here every morning
/// (<see cref="WakePoint"/>), including after the automatic 21:30 bedtime. Model "bed".
/// </summary>
public partial class Bed : Prop
{
    /// <summary>Group of beds; Main's SleepSequence wakes Kit at the first one.</summary>
    public const string Group = "beds";

    public Bed()
    {
        ModelName = "bed";
        SpriteName = "bed";
        PromptSprite = "icon_sleep";
        BodySize = new Vector3(0.94f, 0.6f, 1.25f);
        ClickSize = new Vector3(1f, 0.9f, 1.5f);
        ApproachOffset = new Vector3(0, 0, 1.06f); // 2D: WakeSpot 68 px below
        Reach = 1.3f;
        PromptHeight = 1.2f;
    }

    /// <summary>Where Kit stands after waking up.</summary>
    public Vector3 WakePoint => ApproachPoint;

    public override void _Ready()
    {
        base._Ready();
        AddToGroup(Group);
    }

    public override void Interact(Node3D player)
    {
        if (CanInteract)
            GameClock.Instance?.AskToSleep();
    }
}
