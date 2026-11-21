using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using MyFarmingGame.Autoloads;
using MyFarmingGame.Core;
using MyFarmingGame.Player;
using MyFarmingGame.UI;
using MyFarmingGame.World;

namespace MyFarmingGame.Tests;

/// <summary>Task 1.12: guided start (10 steps), Ada, one-time nudges, settings toggle, save.</summary>
public partial class TestRunner
{
    private void CheckOnboardingRules()
    {
        ResetTask14State();
        var ob = Onboarding.Instance;
        Check(ob != null && ob.GetParent() == GetTree().Root && ob.ProcessMode == ProcessModeEnum.Pausable, "autoload Onboarding registered and Pausable");
        if (ob == null) return;
        Check(ob.Step == OnboardingStep.Arrival && ob.Shown.Count == 0, "new game: step 1 (arrival), no nudges shown");
        Check(SaveGame.Instance.Sections.Contains(Onboarding.Section), "onboarding registers its own save section");
        Check(ob.MarkShown(Onboarding.FirstEvening) && !ob.MarkShown(Onboarding.FirstEvening), "a nudge can be marked shown only once");
        ob.Advance(); ob.Advance();
        ob.WeedsPulled = 3;
        var d = ob.ToDict();
        ob.ResetForNewGame();
        ob.FromDict(d);
        Check(ob.Step == OnboardingStep.Plant && ob.WeedsPulled == 3 && ob.WasShown(Onboarding.FirstEvening), "onboarding step, counters and shown nudges survive save/load");
        Check((int)OnboardingStep.FreePlay == 10 && Onboarding.AllNudges.Length == 8, "10 steps; 8 one-time nudges in this phase (rain waits for Phase-002)");
        ResetTask14State();
    }

    private async Task CloseBubbles(PictureBubbleView bubble)
    {
        for (int i = 0; i < 20 && bubble.IsOpen; i++) { bubble.PressCancel(); await Frames(1); }
    }

    private async Task<bool> Until(System.Func<bool> cond) => await WaitUntil(cond, 120);

    private async Task CheckOnboardingInGame()
    {
        ResetTask14State();
        GameSettings.Nudges = true;
        var ob = Onboarding.Instance;
        var clock = GameClock.Instance;
        var om = OrderManager.Instance;
        var main = await NewMain();
        await Frames(3);
        var level = main.GetNode("World/Level").GetChild(0);
        var player = main.GetNode<PlayerController>("World/Entities/Player");
        var director = main.GetNode<OnboardingDirector>("Systems/OnboardingDirector");
        var chip = main.GetNode<NudgeView>("HUD/UI/Nudge");
        var bubble = main.GetNode<PictureBubbleView>("HUD/UI/PictureBubble");
        var seq = main.GetNode<SleepSequence>("Systems/SleepSequence");
        var fade = main.GetNode<DayTransitionView>("Transition/DayTransition");
        fade.FadeSeconds = 0.05f;
        fade.HoldSeconds = 0.05f;
        var grid = level.GetNode<FarmGrid>("FrontGarden");
        var house = level.GetNode<Building>("House");
        var ada = level.GetNodeOrNull<Ada>("Ada");
        var hens = AnimalHouse.For(GetTree(), "hen");
        var cows = AnimalHouse.For(GetTree(), "cow");
        async Task Sleep()
        {
            clock.BeginSleep(SleepReason.Bed);
            await WaitUntil(() => !clock.IsSleeping && !seq.IsRunning, 600);
            await Frames(3);
        }

        // 1 Arrival: Kit by the cart at the gate, Ada waves, the arrow points at the door.
        Check(ada != null && level.GetNodeOrNull<Node3D>("Cart") != null, "Ada stands by the house; the arrival cart is by the gate");
        Check(ob.Step == OnboardingStep.Arrival && chip.Shown && chip.Text.Contains("front door") && ada.Waving, "step 1: hint 'Walk to the front door', Ada waves");
        Check(director.Arrow.Visible && director.Arrow.Target == house.DoorOutside, "the guide arrow points at the front door");
        Check(player.Feet.DistanceTo(house.DoorOutside) > 3, "Kit starts away from the door (by the cart)");
        await WalkLeg(player, house.DoorOutside);
        Check(await Until(() => ob.Step == OnboardingStep.Garden) && !ada.Waving, "walking to the door → step 2 (the garden)");

        // Ada talks about the current step, never doing it.
        ada.Interact(player);
        Check(bubble.IsOpen && bubble.Portrait == Ada.Portrait && bubble.Line == Ada.LineFor(OnboardingStep.Garden), "Ada's line names the step's goal (weeds by the beds)");
        await CloseBubbles(bubble);

        // 2–4: weeds, plant, water (any tiles; the arrow points near the beds).
        Check(director.Arrow.Target.HasValue && grid.ContainsPoint(director.Arrow.Target.Value), "the arrow points at a weed in the garden");
        var weeds = grid.AllTiles().Where(t => t.Tile.Soil == Soil.Weeds).Select(t => t.Cell).Take(3).ToList();
        foreach (var c in weeds) grid.UseTool(Tool.Hands, c);
        Check(await Until(() => ob.Step == OnboardingStep.Plant) && chip.Text.Contains("Plant"), "3 weeds pulled → step 3 (plant)");
        var beds = grid.AllTiles().Where(t => t.Tile.Soil == Soil.Tilled && !t.Tile.HasCrop).Select(t => t.Cell).Take(3).ToList();
        foreach (var c in beds) grid.UseTool(Tool.SeedBag, c);
        Check(await Until(() => ob.Step == OnboardingStep.Water), "3 seeds planted → step 4 (water)");
        foreach (var c in beds) grid.UseTool(Tool.WateringCan, c);
        Check(await Until(() => ob.Step == OnboardingStep.MeetMarlow) && director.Arrow.Target == level.GetNode<Marlow>("Marlow").GlobalPosition,
            "3 beds watered → step 5: the arrow points at Marlow");

        // 5–8: Marlow, berries, first order, the board.
        om.Accept(OrderManager.FirstOrderId);
        Check(await Until(() => ob.Step == OnboardingStep.Forage) && director.Arrow.Target.HasValue, "accepting Marlow's order → step 6 (forage), arrow at a bush");
        Inventory.Instance.Add(ItemIds.Berries, 3);
        Check(await Until(() => ob.Step == OnboardingStep.FirstOrder), "3 handfuls of berries → step 7 (hand them over)");
        om.Deliver(OrderManager.FirstOrderId);
        Check(await Until(() => ob.Step == OnboardingStep.FeedAnimals), "order paid, board up → steps 7–8 done, step 9 (feed the animals)");
        Check(director.Arrow.Target == hens.Bin.GlobalPosition && chip.Text.Contains("Feed"), "the arrow points at the coop's feed bin");
        hens.FeedAll();
        Check(director.GuideTarget() == cows.Bin.GlobalPosition, "…then at the barn's bin");
        cows.FeedAll();
        Check(await Until(() => ob.Step == OnboardingStep.FreePlay) && chip.Text.Contains("Feed lasts 7 days"), "all fed → step 10: the free-play goal 'Feed lasts 7 days — earn coins for more'");
        Check(!director.Arrow.Visible, "no arrow in free play");
        FeedStore.Order(ItemIds.ChickenFeed, 5, clock.Day, 600);
        Check(await Until(() => ob.IsDone) && !chip.Shown, "ordering feed (or Day 8) ends the guided start; the hint chip goes away");
        FeedStore.Reset();

        // Nudges, each once. (Plenty of feed, so "feed running low" waits for its own check.)
        hens.AddFeed(100);
        cows.AddFeed(20);
        clock.SetTime(clock.Day, GameClock.MinutesPerStep * (int)DayPhase.Evening + 1);
        Check(await Until(() => bubble.IsOpen) && bubble.Line.Contains("getting late") && bubble.Chips.Any(c => c.Sprite == "icon_sleep"),
            "first evening: Ada says to sleep at home (bed icon)");
        await CloseBubbles(bubble);
        // Plant a fresh carrot and leave it dry: morning hint, yellow, lost.
        var dry = grid.AllTiles().First(t => t.Tile.Soil == Soil.Clear).Cell; // a pulled weed: hoe, then plant
        grid.UseTool(Tool.Hoe, dry);
        grid.UseTool(Tool.SeedBag, dry);
        foreach (var c in beds) grid.UseTool(Tool.WateringCan, c);
        hens.FeedAll(); cows.FeedAll();
        await Sleep();
        Check(grid.WaterHints && ob.WasShown(Onboarding.WaterMorning), "morning after planting: watering-can icons over the dry beds");
        Check(director.MorningMarkers.Count == 2 && ob.WasShown(Onboarding.FirstMorning), "first morning: egg and milk icons over the coop and barn");
        int eggOrders = om.BoardOrders.Count(o => o.ItemId == ItemIds.Egg);
        hens.Collect(hens.Animals[0]);
        Check(director.MorningMarkers.Count == 0 && ob.WasShown(Onboarding.FirstProducts) && om.BoardOrders.Count(o => o.ItemId == ItemIds.Egg) >= 1,
            "first eggs: the icons go and an egg order is on the board");
        await CloseBubbles(bubble);
        foreach (var c in beds) grid.UseTool(Tool.WateringCan, c);
        Check(grid.WaterHints, "the can icons stay while a planted bed is dry");
        grid.UseTool(Tool.WateringCan, dry);
        Check(!grid.WaterHints, "watering every bed clears the can icons");
        hens.FeedAll(); cows.FeedAll();
        // Tonight it grows (it was watered); then leave it dry for two nights → yellow nudge.
        await Sleep();
        await CloseBubbles(bubble);
        foreach (var c in beds) grid.UseTool(Tool.WateringCan, c);
        hens.FeedAll(); cows.FeedAll();
        await Sleep();
        await CloseBubbles(bubble);
        foreach (var c in beds) grid.UseTool(Tool.WateringCan, c);
        hens.FeedAll(); cows.FeedAll();
        await Sleep();
        Check(grid.Get(dry).IsYellow && ob.WasShown(Onboarding.YellowCrop), "a yellow crop: 'That one's thirsty!'");
        await CloseBubbles(bubble);
        hens.FeedAll(); cows.FeedAll();
        await Sleep();
        bool lostNote = await Until(() => ob.WasShown(Onboarding.LostCrop) || bubble.IsOpen && bubble.Line.Contains("Dry too long"));
        Check(!grid.Get(dry).HasCrop && lostNote, $"a lost crop: 'Dry too long. Water every day.' (once) [crop {grid.Get(dry).HasCrop}, bubble '{bubble.Line}']");
        Check(ob.WasShown(Onboarding.CarrotsReady) && Inventory.Instance.Count(ItemIds.Carrot) >= 3,
            "carrots ready: the beds sparkled (and the morning watering picked them — any tool harvests)");
        await CloseBubbles(bubble);

        // The board lights a card Kit can hand in.
        var carrots = om.Get(OrderManager.SecondOrderId);
        om.Accept(carrots.Id);
        Inventory.Instance.Add(ItemIds.Carrot, 5);
        Check(main.GetNode<OrderBoardView>("HUD/UI/OrderBoard").CardLit(carrots.Id), "the carrot order's card lights up when Kit has 5 carrots");

        // Feed running low (≤ 2 days left): Ada and the phone, once.
        cows.TakeFeed(cows.Feed);
        cows.AddFeed(10);
        hens.TakeFeed(hens.Feed - 8);
        Check(await Until(() => bubble.IsOpen && bubble.Chips.Any(c => c.Sprite == "phone")) && ob.WasShown(Onboarding.FeedLow),
            "feed running low: Ada points at the phone ('Before 6 in the evening!')");
        await CloseBubbles(bubble);
        await IdleFrames(20);
        Check(!bubble.IsOpen, "…only once");

        // Toggle off: nothing shows.
        ob.ResetForNewGame();
        GameSettings.Nudges = false;
        clock.SetTime(clock.Day, GameClock.MinutesPerStep * (int)DayPhase.Morning);
        clock.SetTime(clock.Day, GameClock.MinutesPerStep * (int)DayPhase.Evening + 1);
        await IdleFrames(20);
        Check(!bubble.IsOpen && !ob.WasShown(Onboarding.FirstEvening), "with Hints off no nudge shows");

        // Never blocks play: doing things early skips steps.
        ob.ResetForNewGame();
        ob.WeedsPulled = ob.Planted = ob.Watered = 3;
        player.GlobalPosition = house.DoorOutside;
        await Until(() => ob.Step > OnboardingStep.SecondOrder);
        Check(ob.Step >= OnboardingStep.FeedAnimals, $"steps already done are skipped at once ({ob.Step})");
        CareNoticeView.WarningPortrait = Ada.Portrait;
        Check(CareNoticeView.WarningPortrait == "npc_ada", "Ada now speaks the hungry-animal warning");

        main.QueueFree();
        await Frames(2);
        ResetTask14State();
    }
}
