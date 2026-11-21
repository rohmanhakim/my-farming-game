using System.Threading.Tasks;
using Godot;
using MyFarmingGame.Autoloads;
using MyFarmingGame.Core;
using MyFarmingGame.Player;
using MyFarmingGame.World;

namespace MyFarmingGame.Tests;

/// <summary>Task 1.5: farm level, indoor zones, clearing, planting, day-based growth and neglect.</summary>
public partial class TestRunner
{
    private FarmGrid NewGrid()
    {
        var g = new FarmGrid { ListenToClock = false };
        AddChild(g);
        g.SetProcess(false);
        return g;
    }

    private void CheckFarmRules()
    {
        ResetTask14State();
        var carrot = CropCatalog.Default.Get("carrot");
        Check(carrot != null && carrot.DaysToHarvest == 3 && carrot.SeedItem == ItemIds.CarrotSeeds && carrot.HarvestItem == ItemIds.Carrot,
            "crops.tres: carrot takes 3 days, grows from carrot seeds, gives carrots");
        Check(Inventory.Instance.Count(ItemIds.CarrotSeeds) == 9, "new game: Kit starts with 9 carrot seeds");

        var g = NewGrid();
        int tilled = 0, weeds = 0;
        foreach (var (_, t) in g.AllTiles())
        {
            if (t.Soil == Soil.Tilled) tilled++;
            if (t.Soil == Soil.Weeds) weeds++;
        }
        Check(g.Columns == 8 && g.Rows == 6 && tilled == 3 && weeds == 45, $"front garden 8×6: 3 beds clear, the rest weeds ({tilled}/{weeds})");

        var c = new Vector2I(0, 0);
        Check(g.UseTool(Tool.SeedBag, c) == TileAction.Nothing && g.LastHint == "tool_hoe", "seeds can't go into weeds; a puff shows the hoe instead");
        var hoed = new Vector2I(1, 0);
        int cleared = 0;
        void OnUsed(int action, Vector2I cell) { if ((TileAction)action == TileAction.Cleared) cleared++; }
        g.ToolUsed += OnUsed;
        Check(g.UseTool(Tool.Hoe, hoed) == TileAction.Tilled && g.Get(hoed).Soil == Soil.Tilled && cleared == 1,
            "the hoe digs weedy ground in one stroke (weeds turned under, counted as pulled) — owner playtest 2026-11-21");
        g.ToolUsed -= OnUsed;
        Check(g.UseTool(Tool.Hoe, hoed) == TileAction.Nothing && g.LastHint == "tool_seed_bag", "hoeing tilled soil again: a puff shows the seed bag");
        Check(g.UseTool(Tool.Hands, c) == TileAction.Cleared && g.Get(c).Soil == Soil.Clear, "hands pull weeds");
        Check(g.UseTool(Tool.SeedBag, c) == TileAction.Nothing, "seeds can't go into untilled ground");
        Check(g.UseTool(Tool.Hoe, c) == TileAction.Tilled && g.Get(c).Soil == Soil.Tilled, "hoe tills cleared ground");
        Check(g.UseTool(Tool.SeedBag, c) == TileAction.Planted && g.Get(c).CropId == "carrot"
              && Inventory.Instance.Count(ItemIds.CarrotSeeds) == 8, "seed bag plants a carrot and uses one seed");
        Check(g.UseTool(Tool.SeedBag, c) == TileAction.Nothing && Inventory.Instance.Count(ItemIds.CarrotSeeds) == 8,
            "a planted tile can't be planted again");
        Check(FarmGrid.CropSprite(g.Get(c)) == "crop_carrot_seed" && FarmGrid.SoilSprite(g.Get(c)) == "soil_tilled", "new seed: seed sprite on dry soil");

        // Growth only on watered days.
        Check(g.UseTool(Tool.WateringCan, c) == TileAction.Watered && FarmGrid.SoilSprite(g.Get(c)) == "soil_wet", "watering darkens the soil");
        g.EndDay();
        Check(g.Get(c).DaysGrown == 1 && !g.Get(c).Watered && g.Get(c).DryDays == 0, "watered night: grows one day; soil dry again in the morning");
        g.EndDay();
        Check(g.Get(c).DaysGrown == 1 && g.Get(c).DryDays == 1 && !g.Get(c).IsYellow, "dry night: no growth, 1 dry day, still green");
        g.EndDay();
        Check(g.Get(c).IsYellow && FarmGrid.CropSprite(g.Get(c)) == "crop_carrot_yellow", "2 dry days in a row: yellow");
        g.UseTool(Tool.WateringCan, c);
        Check(!g.Get(c).IsYellow && g.Get(c).DryDays == 0, "watering a yellow crop recovers it at once");
        g.EndDay();
        g.UseTool(Tool.WateringCan, c);
        g.EndDay();
        Check(g.Get(c).IsReady && FarmGrid.CropSprite(g.Get(c)) == "crop_carrot_ready", "3 watered nights: carrot ready");
        for (int i = 0; i < 5; i++) g.EndDay();
        Check(g.Get(c).IsReady, "a ready carrot waits for Kit (does not dry out)");
        int carrots = Inventory.Instance.Count(ItemIds.Carrot);
        Check(g.UseTool(Tool.WateringCan, c) == TileAction.Harvested && Inventory.Instance.Count(ItemIds.Carrot) == carrots + 1,
            "any tool harvests a ready carrot into the bag");
        Check(!g.Get(c).HasCrop && g.Get(c).Soil == Soil.Tilled, "after harvest the tile stays tilled and empty");

        // Lost after 3 dry days; tile stays tilled.
        var bed = new Vector2I(5, 3);
        g.UseTool(Tool.SeedBag, bed);
        int lost = 0;
        void OnLost(Vector2I cell) => lost++;
        g.CropLost += OnLost;
        g.EndDay(); g.EndDay();
        Check(g.Get(bed).IsYellow, "unwatered seed: yellow after 2 days");
        g.EndDay();
        Check(!g.Get(bed).HasCrop && g.Get(bed).Soil == Soil.Tilled && lost == 1, "3 dry days: the crop is gone, the tile stays tilled (one signal)");
        g.EndDay();
        Check(lost == 1, "an empty tile is never 'lost' again");
        g.CropLost -= OnLost;

        // No seeds → nothing planted.
        Inventory.Instance.Clear();
        Check(g.UseTool(Tool.SeedBag, bed) == TileAction.Nothing, "seed bag does nothing with no seeds in the bag");
        Check(g.UseTool(Tool.FeedScoop, new Vector2I(1, 1)) == TileAction.Nothing, "feed scoop does nothing on the garden");

        // Save shape round-trip.
        var tile = new CropTile { Soil = Soil.Tilled, Watered = true, CropId = "carrot", DaysGrown = 2, DaysToHarvest = 3, DryDays = 1 };
        var back = CropTile.FromDict(tile.ToDict());
        Check(back.Soil == Soil.Tilled && back.Watered && back.CropId == "carrot" && back.DaysGrown == 2 && back.DaysToHarvest == 3 && back.DryDays == 1,
            "CropTile ToDict/FromDict round-trip");

        // Indoors stops the clock (rule level).
        var clk = NewClock();
        clk.EnterIndoors("house");
        double m = clk.MinutesIntoDay;
        clk.Tick(10);
        Check(clk.IsIndoors && clk.IsPlayable && !clk.IsRunning && clk.MinutesIntoDay == m, "indoors: clock stopped, Kit can still act");
        clk.ExitIndoors("house");
        clk.Tick(0.2);
        Check(!clk.IsIndoors && clk.IsRunning && clk.MinutesIntoDay > m, "outdoors: clock runs again");
        clk.QueueFree();

        g.QueueFree();
        ResetTask14State();
    }

    private async Task<bool> WalkLeg(PlayerController p, Vector3 to, int frames = 400)
    {
        bool arrived = false;
        p.WalkTo(to, () => arrived = true);
        await WaitUntil(() => arrived || !p.HasMoveTarget, frames);
        return arrived;
    }

    private async Task CheckFarmInGame()
    {
        ResetTask14State();
        var clock = GameClock.Instance;
        var main = ResourceLoader.Load<PackedScene>("res://scenes/main/Main.tscn").Instantiate();
        AddChild(main);
        var level = main.GetNode("World/Level").GetChild(0);
        var player = main.GetNode<PlayerController>("World/Entities/Player");
        Check(level is FarmLevel, "Main runs the FarmLevel");
        var house = level.GetNodeOrNull<Building>("House");
        var coop = level.GetNodeOrNull<Building>("Coop");
        var barn = level.GetNodeOrNull<Building>("Barn");
        var grid = level.GetNodeOrNull<FarmGrid>("FrontGarden");
        var bed = level.FindChild("Bed", true, false) as Bed;
        Check(house != null && coop != null && barn != null && grid != null, "farm has house, old coop, old barn and front garden");
        Check(coop.SizeTiles == new Vector2I(4, 3) && barn.SizeTiles == new Vector2I(6, 5), "old coop 4×3, old barn 6×5 (buildings.md)");
        Check(house.RoomScene == Main.HouseRoomScene && coop.RoomScene.EndsWith("CoopRoom.tscn") && barn.RoomScene.EndsWith("BarnRoom.tscn")
              && level.GetNodeOrNull<Building>("SeedShed")?.RoomScene.EndsWith("ShedRoom.tscn") == true && bed == null,
            "the house, coop, barn and seed shed each lead into their own room scene; the bed is not on the farm (Phase-002 Task 2.2)");
        Check(house.Roof != null && house.Roof.Source != ModelLibrary.Source.Fallback && house.Bounds.Size.Y > Building.WallHeight,
            "the house has walls and a roof (model 'roof_house')");
        Check(((FarmLevel)level).Tufts?.Multimesh.InstanceCount > 900, $"overgrown fields are drawn as one MultiMesh of tufts ({((FarmLevel)level).Tufts?.Multimesh.InstanceCount})");
        await Frames(5);
        Check(!clock.IsIndoors && clock.IsRunning, "new game: Kit starts outside and the clock runs");

        // Owner 2026-11-21: walk into the doorway → a short fade → the room's own scene; walk out the same way.
        clock.TimeScale = 60;
        var rig = main.GetNode<CameraRig>("World/Entities/CameraRig");
        var mainNode = (Main)main;
        void Refetch()
        {
            level = mainNode.Level;
            house = level.GetNodeOrNull<Building>("House");
            coop = level.GetNodeOrNull<Building>("Coop");
            barn = level.GetNodeOrNull<Building>("Barn");
            grid = level.GetNodeOrNull<FarmGrid>("FrontGarden");
        }
        player.GlobalPosition = house.DoorOutside + new Vector3(0, 0, -1.2f);
        await Frames(10);
        int levelChanges = 0;
        mainNode.LevelChanged += _ => levelChanges++;
        player.WalkTo(house.DoorPoint + new Vector3(0, 0, 0.5f));
        Check(await WaitUntil(() => mainNode.CurrentLevelPath == Main.HouseRoomScene && !mainNode.Teleporting, 300),
            "walking into the house's doorway loads the house room scene");
        var houseRoom = mainNode.CurrentRoom;
        await Frames(10);
        double m = clock.MinutesIntoDay;
        await Frames(20);
        Check(levelChanges == 1 && houseRoom != null && mainNode.Level == houseRoom && mainNode.GetNode("World/Level").GetChildCount() == 1,
            "exactly one level is loaded (the farm was freed; LevelChanged once)");
        Check(clock.IsIndoors && clock.MinutesIntoDay == m && houseRoom.Zone.PlayerInside, "inside the room the clock stops");
        Check(houseRoom.Contains(player.Feet) && player.Feet.DistanceTo(houseRoom.DoorInside) < 0.3f && player.Facing == Facing.Down,
            "Kit stands just inside the room's door, facing in");
        Check(Mathf.IsEqualApprox(rig.Distance, mainNode.RoomCameraDistance) && rig.Focus.DistanceTo(player.Feet) < 1.5f, "the camera frames the room (closer)");
        Check(houseRoom.Roof == null && houseRoom.WallFaded(Vector3.Back) && !houseRoom.WallFaded(Vector3.Forward), "the room has no roof; the walls facing the camera are faded");
        Check(houseRoom.FindChild("Bed", true, false) is Bed b0 && houseRoom.Contains(b0.WakePoint), "the bed is in the house room");
        player.WalkTo(houseRoom.DoorPoint + new Vector3(0, 0, -0.5f));
        Check(await WaitUntil(() => mainNode.CurrentLevelPath == Main.FarmScene && !mainNode.Teleporting, 300), "walking into the room's doorway goes back to the farm scene");
        await Frames(10);
        Refetch();
        Check(!clock.IsIndoors && clock.MinutesIntoDay > m && player.Feet.DistanceTo(house.DoorOutside) < 0.3f && player.Facing == Facing.Up
              && Mathf.IsEqualApprox(rig.Distance, 13f), "outside again at the front door: clock runs, camera back to the farm");
        Check(house.Zone == null, "the house itself is not entered on the map");
        // Behind the house (north of it, seen from the south): the house fades so Kit stays visible.
        player.GlobalPosition = house.GlobalPosition + new Vector3(house.Size.X / 2 + 1.5f, 0, -0.4f);
        await Frames(30);
        Check(!clock.IsIndoors && house.RoofFaded, "Kit hidden behind the house: its roof fades (occluder fade)");
        player.GlobalPosition = house.DoorOutside + new Vector3(0, 0, -3);
        await Frames(30);
        Check(!house.RoofFaded, "Kit in the open again: the roof comes back");
        foreach (var (scene, door) in new[] { (coop.RoomScene, coop.DoorMarkerName), (barn.RoomScene, barn.DoorMarkerName) })
        {
            mainNode.LoadLevelNow(scene, Building.EntryMarker);
            await Frames(5);
            Check(clock.IsIndoors && mainNode.CurrentRoom?.Zone.PlayerInside == true, $"the {mainNode.CurrentRoom?.ZoneName} room also stops the clock");
            mainNode.LoadLevelNow(Main.FarmScene, door);
            await Frames(5);
            Check(!clock.IsIndoors, "back on the farm the clock runs again");
        }
        Refetch();
        clock.TimeScale = 1;

        // Walk from the house to the gate, and around the house to the barn.
        var spawn = level.GetNode<Marker3D>("PlayerSpawn").GlobalPosition;
        player.GlobalPosition = spawn;
        await Frames(5);
        Check(await WalkLeg(player, M(1600, 40)), "Kit walks from the front door to the gate");
        Check(await WalkLeg(player, M(1600, -200), 200), "the gate is open onto the lane (Task 1.6)");
        Check(await WalkLeg(player, M(1600, 40), 200), "back through the gate");
        bool toBarn = await WalkLeg(player, M(1290, 440)) && await WalkLeg(player, M(1290, 830))
                      && await WalkLeg(player, M(1600, 830)) && await WalkLeg(player, barn.DoorOutside);
        Check(toBarn, "Kit walks around the house and down the path to the old barn");
        string barnDoor = barn.DoorMarkerName;
        player.WalkTo(barn.DoorPoint + new Vector3(0, 0, 0.5f));
        Check(await WaitUntil(() => mainNode.CurrentLevelPath.EndsWith("BarnRoom.tscn") && !mainNode.Teleporting, 300), "Kit walks into the barn (its room scene)");
        mainNode.LoadLevelNow(Main.FarmScene, barnDoor);
        await Frames(5);
        Refetch();
        player.GlobalPosition = barn.DoorOutside + new Vector3(0, 0, -1f);
        await Frames(5);
        Check(!await WalkLeg(player, M(1200, 1100), 200), "overgrown Field A is blocked");

        // Gamepad A on the tile in front of Kit: clear → till → plant → water.
        var cell = new Vector2I(2, 5);
        Vector3 center = grid.CellCenter(cell);
        player.GlobalPosition = center + new Vector3(0, 0, 0.875f);
        player.Face(Facing.Up);
        await Frames(3);
        Check(grid.HighlightedCell == cell, "the tile in front of Kit is highlighted");
        var belt = ToolBelt.Instance;
        belt.Select(Tool.Hands);
        int usesBefore = player.UseCount;
        await Tap(JoyButton.A);
        Check(grid.Get(cell).Soil == Soil.Clear, "gamepad A with hands pulls the weeds in front");
        Check(player.UseCount == usesBefore + 1 && grid.LastPuff == TileAction.Cleared && !player.Hand.Visible,
            "pulling weeds plays Kit's use motion and a leaf puff; bare hands hold nothing");
        belt.Select(Tool.Hoe);
        Key(Godot.Key.Space, true); await Frames(1); Key(Godot.Key.Space, false); await Frames(2);
        Check(grid.Get(cell).Soil == Soil.Tilled, "Space with the hoe tills it");
        belt.Select(Tool.SeedBag);
        await Tap(JoyButton.A);
        Check(grid.Get(cell).HasCrop && Inventory.Instance.Count(ItemIds.CarrotSeeds) == 8, "gamepad A with the seed bag plants a carrot");
        belt.Select(Tool.WateringCan);
        await Tap(JoyButton.A);
        Check(grid.Get(cell).Watered && grid.LastPuff == TileAction.Watered && player.Hand.ModelName == "tool_watering_can",
            "gamepad A with the can waters it (water drops; Kit holds the can)");

        // Mouse: click a weed tile → Kit walks beside it, faces it, pulls the weeds.
        belt.Select(Tool.Hands);
        var target = new Vector2I(4, 4);
        ClickWorld(grid.CellCenter(target));
        bool clicked = await WaitUntil(() => grid.Get(target).Soil == Soil.Clear, 300);
        Check(clicked, "clicking a weed tile walks Kit there and pulls the weeds");

        // Owner playtest 2026-11-21: a tile within reach is dug at once; a far one is walked to, then dug.
        belt.Select(Tool.Hoe);
        await Frames(15);
        Vector3 here = player.GlobalPosition;
        var near = grid.WorldToCell(player.Feet + new Vector3(0, 0, -1));
        Check(grid.InBounds(near) && grid.Get(near).Soil == Soil.Weeds, $"a weed tile right beside Kit ({near})");
        ClickWorld(grid.CellCenter(near));
        await Frames(2);
        Check(grid.Get(near).Soil == Soil.Tilled && !player.HasMoveTarget && player.GlobalPosition.DistanceTo(here) < 0.03f && player.Facing == Facing.Up,
            "clicking a tile within reach with the hoe digs it at once (Kit turns to it, no walking)");
        var far = new Vector2I(0, 0);
        ClickWorld(grid.CellCenter(far));
        Check(player.HasMoveTarget, "clicking a far tile walks Kit there first");
        Check(await WaitUntil(() => grid.Get(far).Soil == Soil.Tilled, 400), "…and digs it on arrival (the corner by the field edge too)");
        bool adaClear = true;
        foreach (var (tileCell, _) in grid.AllTiles())
            adaClear &= player.InteractableAt(ScreenOf(grid.CellCenter(tileCell))) == null;
        Check(adaClear, "no interactable (Ada) sits over the garden, so clicks and A always reach the tiles");

        // The night grows the watered carrot (DayEnded hook).
        clock.BeginSleep(SleepReason.Bed);
        await WaitUntil(() => !clock.IsSleeping && !main.GetNode<SleepSequence>("Systems/SleepSequence").IsRunning, 900);
        var gardenState = FarmState.Instance.Garden;
        Check(clock.Day == 2 && gardenState.Get(cell).DaysGrown == 1 && !gardenState.Get(cell).Watered,
            "after a night the watered carrot grew one day (the garden kept growing while its scene was not loaded)");
        Check(clock.IsIndoors && mainNode.CurrentLevelPath == Main.HouseRoomScene, "Kit wakes in the house room (clock stopped until Kit goes out)");

        main.QueueFree();
        await Frames(2);
        Check(!clock.IsIndoors, "leaving the scene clears the indoor stop");
        ResetTask14State();
    }
}
