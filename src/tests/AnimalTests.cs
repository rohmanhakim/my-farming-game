using System.Linq;
using System.Threading.Tasks;
using Godot;
using MyFarmingGame.Autoloads;
using MyFarmingGame.Core;
using MyFarmingGame.Player;
using MyFarmingGame.UI;
using MyFarmingGame.World;

namespace MyFarmingGame.Tests;

/// <summary>Task 1.9: feeding, eggs and milk, heart meter, warnings and the loss rule.</summary>
public partial class TestRunner
{
    private AnimalHouse NewHouse(string kind)
    {
        var h = kind == "cow"
            ? new AnimalHouse { Kind = "cow", FeedItem = ItemIds.Hay, StartingFeed = 7, StartingNames = new[] { "Maple" }, ListenToClock = false }
            : new AnimalHouse { ListenToClock = false };
        AddChild(h);
        return h;
    }

    private void CheckAnimalRules()
    {
        ResetTask14State();
        var hens = NewHouse("hen");
        int lost = 0, warned = 0;
        string lostName = "";
        void OnLost(string n, string k) { lost++; lostName = n; }
        void OnWarn(string k) => warned++;
        hens.AnimalLost += OnLost;
        hens.HungryWarning += OnWarn;
        var b = hens.Animals[0];
        Check(hens.Animals.Count == 5 && hens.Animals.All(a => a.Hearts == 3 && !a.ShowsNeed), "new game: 5 hens, full hearts, no need icons");

        // Feeding: the scoop at the bin feeds every hungry hen once.
        Check(hens.FeedAll() == 5 && hens.Feed == 30 && hens.Animals.All(a => a.FedToday), "scoop at the bin: 5 hens fed, 5 portions used");
        Check(hens.FeedAll() == 0 && hens.Feed == 30 && !hens.FeedOne(b), "once per day: feeding again uses nothing");
        hens.EndDay();
        Check(hens.Animals.All(a => a.ProductReady == 1 && a.UnfedDays == 0 && !a.FedToday), "fed day → 1 egg per hen next morning");
        int eggs = Inventory.Instance.Count(ItemIds.Egg);
        Check(hens.Collect(b) == 1 && Inventory.Instance.Count(ItemIds.Egg) == eggs + 1 && b.ProductReady == 0, "collecting a hen's egg puts it in the bag");

        // Unfed: day 1 need icon ⅔, no egg; day 2 flashes ⅓ + warning; day 3 gone.
        hens.EndDay();
        Check(b.UnfedDays == 1 && b.Hearts == 2 && b.ShowsNeed && !b.NeedFlashes && b.ProductReady == 0, "1 unfed day: need icon, heart ⅔, no egg");
        Check(hens.Animals[1].ProductReady == 1, "an uncollected egg from a fed day waits");
        hens.Dawn(3);
        Check(warned == 0, "no warning after only one missed day");
        hens.EndDay();
        Check(b.UnfedDays == 2 && b.Hearts == 1 && b.NeedFlashes, "2 unfed days: flashing icon, heart ⅓");
        hens.Dawn(4);
        Check(warned == 1, "Dawn after 2 unfed days: one warning for the coop");
        hens.FeedOne(b);
        Check(b.Hearts == 3 && !b.ShowsNeed, "feeding shows a full heart at once");
        hens.EndDay();
        Check(b.UnfedDays == 0 && b.ProductReady == 1 && hens.Animals.Count(a => a.UnfedDays == 3) == 0 && hens.Animals.Count == 1 && lost == 4,
            $"day 3 unfed: the other 4 hens are gone (4 loss signals); the fed hen stays and lays ({hens.Animals.Count} left)");
        Check(lostName != "" && hens.Animals[0] == b, "only unfed animals are removed");
        Check(hens.Feed == 29 && hens.HasRoom, "the coop and its feed stay; there is room again");
        Check(CareNoticeView.LossLine("Bramble", "hen") == "Bramble the hen is gone. Hens need food every day.", "loss card text (animals.md §3)");
        hens.AnimalLost -= OnLost;
        hens.HungryWarning -= OnWarn;
        hens.QueueFree();

        // The cow: same rule, 4 milk cans.
        var cows = NewHouse("cow");
        var maple = cows.Animals[0];
        Check(cows.ProductItem == ItemIds.Milk && cows.ProductPerDay == 4, "the cow gives 4 milk cans per fed day");
        cows.FeedOne(maple);
        cows.EndDay();
        Check(maple.ProductReady == 4 && cows.Feed == 6, "fed cow: 4 milk cans next morning, one hay portion used");
        int milk = Inventory.Instance.Count(ItemIds.Milk);
        cows.Collect(maple);
        Check(Inventory.Instance.Count(ItemIds.Milk) == milk + 4, "collecting milk: 4 cans into the bag");
        int cowLost = 0;
        void OnCow(string n, string k) { if (k == "cow" && n == "Maple") cowLost++; }
        cows.AnimalLost += OnCow;
        cows.EndDay(); cows.EndDay();
        Check(cows.Animals.Count == 1 && maple.Hearts == 1, "cow after 2 unfed days: still there, heart ⅓");
        cows.EndDay();
        Check(cows.Animals.Count == 0 && cowLost == 1, "cow after 3 unfed days: gone, one loss");
        cows.EndDay();
        Check(cowLost == 1, "a loss is reported only once");
        cows.AnimalLost -= OnCow;

        // Empty bin: nothing fed.
        var empty = NewHouse("hen");
        empty.TakeFeed(empty.Feed);
        Check(empty.FeedAll() == 0 && empty.Animals.All(a => !a.FedToday), "an empty bin feeds nobody");
        empty.AddFeed(2);
        Check(empty.FeedAll() == 2 && empty.Feed == 0, "a nearly empty bin feeds as many as it can");
        var d = AnimalData.FromDict(maple.ToDict());
        Check(d.Name == "Maple" && d.Kind == "cow" && d.UnfedDays == 3, "AnimalData ToDict/FromDict round-trip");
        cows.QueueFree();
        empty.QueueFree();
        ResetTask14State();
    }

    private async Task CheckAnimalsInGame()
    {
        ResetTask14State();
        var clock = GameClock.Instance;
        var main = ResourceLoader.Load<PackedScene>("res://scenes/main/Main.tscn").Instantiate();
        AddChild(main);
        var level = main.GetNode("World/Level").GetChild(0);
        var player = main.GetNode<PlayerController>("World/Entities/Player");
        var bubble = main.GetNode<PictureBubbleView>("HUD/UI/PictureBubble");
        var notices = main.GetNode<CareNoticeView>("HUD/UI/CareNotices");
        var seq = main.GetNode<SleepSequence>("Systems/SleepSequence");
        var fade = main.GetNode<DayTransitionView>("Transition/DayTransition");
        fade.FadeSeconds = 0.05f;
        fade.HoldSeconds = 0.05f;
        await Frames(3); // houses freed by the rule tests leave the group first
        var hens = AnimalHouse.For(GetTree(), "hen");
        var cows = AnimalHouse.For(GetTree(), "cow");
        Check(hens.Entities.Count == 5 && hens.Entities.All(e => e is Hen) && cows.Entities.Count == 1 && cows.Entities[0] is Cow,
            "5 hens stand in the coop, the cow in the barn");
        var coop = level.GetNode<Building>("Coop");
        Check(hens.Entities.All(e => coop.Interior.HasPoint(e.GlobalPosition)), "the hens are inside the coop walls");

        async Task Sleep()
        {
            clock.BeginSleep(SleepReason.Bed);
            await WaitUntil(() => !clock.IsSleeping && !seq.IsRunning, 600);
            await Frames(2);
        }

        // Feed the hens with the scoop at the bin (gamepad A).
        ToolBelt.Instance.Select(Tool.FeedScoop);
        player.GlobalPosition = hens.Bin.ApproachPoint;
        await Frames(5);
        Check(clock.IsIndoors, "the coop stops the clock");
        await Tap(JoyButton.A);
        Check(hens.Animals.All(a => a.FedToday) && hens.Feed == 30, "feed scoop + A at the coop bin feeds all 5 hens");
        // Feed the cow at the cow itself.
        var cow = cows.Entities[0];
        player.GlobalPosition = cow.ApproachPoint;
        await Frames(5);
        await Tap(JoyButton.A);
        Check(cows.Animals[0].FedToday && cows.Feed == 6, "feed scoop + A at the cow feeds her");
        await Sleep();
        Check(hens.Animals.All(a => a.ProductReady == 1) && cows.Animals[0].ProductReady == 4, "next morning: an egg per hen, 4 milk cans");
        await Frames(2);
        Check(hens.Entities[0].ProductVisible && cow.ProductVisible && !hens.Entities[0].NeedVisible, "eggs and milk show on the animals");
        ToolBelt.Instance.Select(Tool.Hands);
        player.GlobalPosition = cow.ApproachPoint;
        await Frames(5);
        await Tap(JoyButton.A);
        Check(Inventory.Instance.Count(ItemIds.Milk) == 4 && cows.Animals[0].ProductReady == 0, "A at the cow collects 4 milk cans");
        var hen0 = hens.Entities[0];
        player.GlobalPosition = hen0.ApproachPoint;
        await Frames(5);
        await Tap(JoyButton.A);
        Check(Inventory.Instance.Count(ItemIds.Egg) >= 1, "A at a hen collects her egg");

        // Neglect: 1 day → need icons; 2 → flashing + a neighbour's warning; 3 → loss cards.
        player.GlobalPosition = level.GetNode<Marker2D>("PlayerSpawn").GlobalPosition;
        await Sleep();
        Check(hens.Entities.All(e => e.NeedVisible) && hens.Animals.All(a => a.Hearts == 2), "after a missed day: need icons, hearts ⅔");
        await Sleep();
        Check(notices.WarningsShown >= 1 && bubble.IsOpen && bubble.Portrait == CareNoticeView.WarningPortrait && bubble.Line.Contains("hungry"),
            "after 2 missed days a neighbour mentions the hungry animals when Kit wakes");
        while (bubble.IsOpen) { bubble.PressCancel(); await Frames(1); }
        await Sleep();
        Check(hens.Animals.Count == 0 && cows.Animals.Count == 0 && hens.Entities.Count == 0, "after 3 missed days the hens and the cow are gone");
        Check(notices.LossCardsShown == 6, $"one loss card per animal ({notices.LossCardsShown})");
        Check(bubble.IsOpen && bubble.Chips[0].Sprite == "gravestone" && bubble.Line.Contains("is gone") && bubble.Buttons.Count == 1,
            "the loss card: gravestone picture, name, one line, one button");
        int cards = 0;
        while (bubble.IsOpen && cards < 10) { cards++; bubble.PressCancel(); await Frames(1); }
        Check(cards == 6 && notices.Pending == 0, "each card shows once, one after another");
        Check(hens.Feed > 0 && level.GetNodeOrNull<Building>("Coop") != null && hens.HasRoom, "the coop, barn and feed stay");

        main.QueueFree();
        await Frames(2);
        ResetTask14State();
    }
}
