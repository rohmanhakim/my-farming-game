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

    private async Task<bool> WalkLeg(PlayerController p, Vector2 to, int frames = 400)
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
        Check(bed != null && bed.GetParent() == house && house.Interior.HasPoint(bed.WakePoint), "the bed is inside Kit's house");
        Check(grid.ZIndex < 0 && house.Roof.ZIndex > 0, "garden draws below Kit, roofs above");
        await Frames(5);
        Check(!clock.IsIndoors && clock.IsRunning, "new game: Kit starts outside and the clock runs");

        // Into the house: roof fades, clock stops; out again: roof back, clock runs.
        clock.TimeScale = 60;
        player.GlobalPosition = house.DoorInside;
        await Frames(30);
        double m = clock.MinutesIntoDay;
        await Frames(20);
        Check(clock.IsIndoors && house.Zone.PlayerInside && clock.MinutesIntoDay == m, "inside the house the clock stops");
        Check(house.Roof.Modulate.A < 0.05f, $"inside the house the roof fades out ({house.Roof.Modulate.A:0.00})");
        player.GlobalPosition = house.DoorOutside;
        await Frames(30);
        Check(!clock.IsIndoors && clock.MinutesIntoDay > m && house.Roof.Modulate.A > 0.95f, "outside again: roof back, clock runs");
        foreach (var b in new[] { coop, barn })
        {
            player.GlobalPosition = b.DoorInside;
            await Frames(5);
            Check(clock.IsIndoors && b.Zone.PlayerInside, $"the {b.ZoneName} also stops the clock");
            player.GlobalPosition = b.DoorOutside;
            await Frames(5);
            Check(!clock.IsIndoors, $"leaving the {b.ZoneName} restarts the clock");
        }
        clock.TimeScale = 1;

        // Walk from the house to the gate, and around the house to the barn.
        var spawn = level.GetNode<Marker2D>("PlayerSpawn").GlobalPosition;
        player.GlobalPosition = spawn;
        await Frames(5);
        Check(await WalkLeg(player, new Vector2(1600, 40)), "Kit walks from the front door to the gate");
        Check(await WalkLeg(player, new Vector2(1600, -200), 200), "the gate is open onto the lane (Task 1.6)");
        Check(await WalkLeg(player, new Vector2(1600, 40), 200), "back through the gate");
        bool toBarn = await WalkLeg(player, new Vector2(1290, 440)) && await WalkLeg(player, new Vector2(1290, 830))
                      && await WalkLeg(player, new Vector2(1600, 830)) && await WalkLeg(player, barn.DoorOutside);
        Check(toBarn, "Kit walks around the house and down the path to the old barn");
        Check(await WalkLeg(player, barn.DoorInside), "Kit walks into the barn");
        Check(!await WalkLeg(player, new Vector2(1200, 1100), 200), "overgrown Field A is blocked");

        // Gamepad A on the tile in front of Kit: clear → till → plant → water.
        var cell = new Vector2I(2, 5);
        Vector2 center = grid.CellCenter(cell);
        player.GlobalPosition = center + new Vector2(0, 56) - PlayerController.FeetOffset;
        player.Face(Facing.Up);
        await Frames(3);
        Check(grid.HighlightedCell == cell, "the tile in front of Kit is highlighted");
        var belt = ToolBelt.Instance;
        belt.Select(Tool.Hands);
        await Tap(JoyButton.A);
        Check(grid.Get(cell).Soil == Soil.Clear, "gamepad A with hands pulls the weeds in front");
        belt.Select(Tool.Hoe);
        Key(Godot.Key.Space, true); await Frames(1); Key(Godot.Key.Space, false); await Frames(2);
        Check(grid.Get(cell).Soil == Soil.Tilled, "Space with the hoe tills it");
        belt.Select(Tool.SeedBag);
        await Tap(JoyButton.A);
        Check(grid.Get(cell).HasCrop && Inventory.Instance.Count(ItemIds.CarrotSeeds) == 8, "gamepad A with the seed bag plants a carrot");
        belt.Select(Tool.WateringCan);
        await Tap(JoyButton.A);
        Check(grid.Get(cell).Watered, "gamepad A with the can waters it");

        // Mouse: click a weed tile → Kit walks beside it, faces it, pulls the weeds.
        belt.Select(Tool.Hands);
        var target = new Vector2I(4, 4);
        var vp = GetViewport();
        Vector2 screen = vp.GetCanvasTransform() * grid.CellCenter(target);
        vp.PushInput(new InputEventMouseButton { ButtonIndex = MouseButton.Left, Pressed = true, Position = screen, GlobalPosition = screen }, true);
        vp.PushInput(new InputEventMouseButton { ButtonIndex = MouseButton.Left, Pressed = false, Position = screen, GlobalPosition = screen }, true);
        bool clicked = await WaitUntil(() => grid.Get(target).Soil == Soil.Clear, 300);
        Check(clicked, "clicking a weed tile walks Kit there and pulls the weeds");

        // Owner playtest 2026-11-21: a tile within reach is dug at once; a far one is walked to, then dug.
        belt.Select(Tool.Hoe);
        await Frames(15);
        Vector2 here = player.GlobalPosition;
        var near = grid.WorldToCell(player.GlobalPosition + PlayerController.FeetOffset + new Vector2(0, -64));
        Check(grid.InBounds(near) && grid.Get(near).Soil == Soil.Weeds, "a weed tile right beside Kit");
        screen = vp.GetCanvasTransform() * grid.CellCenter(near);
        vp.PushInput(new InputEventMouseButton { ButtonIndex = MouseButton.Left, Pressed = true, Position = screen, GlobalPosition = screen }, true);
        vp.PushInput(new InputEventMouseButton { ButtonIndex = MouseButton.Left, Pressed = false, Position = screen, GlobalPosition = screen }, true);
        await Frames(2);
        Check(grid.Get(near).Soil == Soil.Tilled && !player.HasMoveTarget && player.GlobalPosition.DistanceTo(here) < 2 && player.Facing == Facing.Up,
            "clicking a tile within reach with the hoe digs it at once (Kit turns to it, no walking)");
        var far = new Vector2I(0, 0);
        screen = vp.GetCanvasTransform() * grid.CellCenter(far);
        vp.PushInput(new InputEventMouseButton { ButtonIndex = MouseButton.Left, Pressed = true, Position = screen, GlobalPosition = screen }, true);
        vp.PushInput(new InputEventMouseButton { ButtonIndex = MouseButton.Left, Pressed = false, Position = screen, GlobalPosition = screen }, true);
        Check(player.HasMoveTarget, "clicking a far tile walks Kit there first");
        Check(await WaitUntil(() => grid.Get(far).Soil == Soil.Tilled, 400), "…and digs it on arrival (the corner by the field edge too)");
        bool adaClear = true;
        foreach (var (tileCell, _) in grid.AllTiles())
            adaClear &= player.InteractableAt(grid.CellCenter(tileCell)) == null;
        Check(adaClear, "no interactable (Ada) sits over the garden, so clicks and A always reach the tiles");

        // The night grows the watered carrot (DayEnded hook).
        clock.BeginSleep(SleepReason.Bed);
        await WaitUntil(() => !clock.IsSleeping && !main.GetNode<SleepSequence>("Systems/SleepSequence").IsRunning, 900);
        Check(clock.Day == 2 && grid.Get(cell).DaysGrown == 1 && !grid.Get(cell).Watered, "after a night the watered carrot grew one day");
        Check(clock.IsIndoors, "Kit wakes in the house (clock stopped until Kit goes out)");

        main.QueueFree();
        await Frames(2);
        Check(!clock.IsIndoors, "leaving the scene clears the indoor stop");
        ResetTask14State();
    }
}
