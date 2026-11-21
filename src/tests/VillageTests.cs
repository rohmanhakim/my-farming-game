using System.Linq;
using System.Threading.Tasks;
using Godot;
using MyFarmingGame.Autoloads;
using MyFarmingGame.Core;
using MyFarmingGame.Player;
using MyFarmingGame.UI;
using MyFarmingGame.World;

namespace MyFarmingGame.Tests;

/// <summary>Phase-002 Task 2.3: Bramblewick Green's first ring.</summary>
public partial class TestRunner
{
    private const string VillageScene = "res://scenes/world/VillageGreen.tscn";

    private async Task CheckVillageInGame()
    {
        ResetTask14State();
        GameSettings.Nudges = false;
        var main = ResourceLoader.Load<PackedScene>("res://scenes/main/Main.tscn").Instantiate<Main>();
        AddChild(main);
        await Frames(3);
        var player = main.Player;
        var bubble = main.GetNode<PictureBubbleView>("HUD/UI/PictureBubble");
        var director = main.GetNode<OnboardingDirector>("Systems/OnboardingDirector");

        // The lane runs on past Marlow's clearing to the green.
        var toGreen = main.Level.FindChild("ToGreen", true, false) as DoorTrigger;
        var laneTop = main.Level.FindChild("LaneTop", true, false) as Marker3D;
        Check(toGreen != null && toGreen.TargetScene == VillageScene && laneTop != null, "the top of the lane leads to the village green");
        player.GlobalPosition = laneTop.GlobalPosition;
        await Frames(3);
        Check(await WalkLeg(player, laneTop.GlobalPosition + new Vector3(0, 0, -2.2f), 200), "the lane goes on through the clearing's north edge");
        player.WalkTo(toGreen.GlobalPosition + new Vector3(0, 0, -0.5f));
        Check(await WaitUntil(() => main.CurrentLevelPath == VillageScene && !main.Teleporting, 300), "walking up the lane loads the village green");
        var green = main.Level;
        var fromLane = green.GetNode<Marker3D>("FromLane");
        Check(green is VillageGreen && player.Feet.DistanceTo(new Vector3(fromLane.GlobalPosition.X, 0, fromLane.GlobalPosition.Z)) < 0.05f
              && player.Facing == Facing.Up && !GameClock.Instance.IsIndoors, "Kit arrives at the south edge of the green, facing in; the clock runs");
        var rig = main.Rig;
        Check(Mathf.IsEqualApprox(rig.Distance, 13f) && rig.LimitMax.X > 50, "the outdoor camera frames the green");

        // The guide arrow (step 1, the front door) points back down the lane.
        director.Evaluate();
        var toLane = green.GetNode<DoorTrigger>("ToLane");
        Check(director.Arrow.Target == toLane.GlobalPosition, "from the green, the arrow for a farm target points at the lane home");

        // Closed places: picture sign, no way in.
        var places = green.FindChildren("*", "StaticBody3D", true, false).OfType<VillagePlace>().ToList();
        Check(places.Count == 3 && places.All(p => !p.IsOpen && p.Model.ModelName == "closed_sign"), "the bakery, store and allotments start closed (picture sign)");
        var bakery = green.GetNode<Building>("Bakery");
        Check(!bakery.Enterable && bakery.Zone == null && bakery.GetNodeOrNull("ClosedDoor") != null && green.GetNode<Building>("GeneralStore").Zone == null,
            "the bakery and the store have shut doors (no way in yet)");
        var bakeryDoor = places.First(p => p.NeighbourId == Neighbours.Baker);
        player.GlobalPosition = bakeryDoor.ApproachPoint;
        await Frames(4);
        Check(bakeryDoor.PromptVisible, "the closed sign shows a prompt when Kit is in front of it");
        await Tap(JoyButton.A);
        Check(bubble.IsOpen && bubble.Chips.Any(c => c.Sprite == "icon_closed") && bubble.Chips.Any(c => c.Sprite == "item_egg"),
            "the bakery's sign says what opens it (the first eggs)");
        await CloseBubbles(bubble);

        // The baker arrives → the bakery opens and the baker greets Kit.
        OrderManager.Instance.Restore(OrderManager.Instance.Orders.ToList(), false, null, 1, new[] { Neighbours.Marlow, Neighbours.Baker });
        await Frames(3);
        Check(bakeryDoor.IsOpen && bakeryDoor.Model.ModelName == "baker" && places.Where(p => p != bakeryDoor).All(p => !p.IsOpen),
            "once the baker has arrived the bakery is open (the baker at the door); the others stay closed");
        await Tap(JoyButton.A);
        Check(bubble.IsOpen && bubble.Portrait == Neighbours.Get(Neighbours.Baker).Portrait && bubble.Chips.Any(c => c.Sprite == "item_egg"),
            "the baker greets Kit (picture of what they like)");
        await CloseBubbles(bubble);

        // The pond and the hedges block; the ring path goes all round.
        Check(!await WalkLeg(player, new Vector3(32, 0, 22), 200), "the duck pond blocks");
        player.GlobalPosition = new Vector3(11, 0, 9);
        await Frames(2);
        var corners = new[] { new Vector3(53, 0, 9), new Vector3(53, 0, 35), new Vector3(11, 0, 35), new Vector3(11, 0, 9) };
        bool round = true;
        foreach (var c in corners)
        {
            bool ok = await WalkLeg(player, c, 2000);
            if (!ok) GD.Print($"  ring leg to {c} stopped at {player.Feet}");
            round &= ok;
        }
        Check(round, "the ring path goes all the way round the green");

        // Back home down the lane.
        player.GlobalPosition = fromLane.GlobalPosition;
        await Frames(3);
        player.WalkTo(toLane.GlobalPosition + new Vector3(0, 0, 0.5f));
        Check(await WaitUntil(() => main.CurrentLevelPath == Main.FarmScene && !main.Teleporting, 300), "walking down the lane goes back to the farm");
        var top = main.Level.FindChild("LaneTop", true, false) as Marker3D;
        Check(player.Feet.DistanceTo(new Vector3(top.GlobalPosition.X, 0, top.GlobalPosition.Z)) < 0.05f && player.Facing == Facing.Down,
            "Kit comes out at the top of the lane, facing home");

        main.QueueFree();
        await Frames(2);
        ResetTask14State();
    }
}
