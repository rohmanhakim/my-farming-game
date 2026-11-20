using System.Linq;
using System.Threading.Tasks;
using Godot;
using MyFarmingGame.Autoloads;
using MyFarmingGame.Core;
using MyFarmingGame.Player;
using MyFarmingGame.UI;
using MyFarmingGame.World;

namespace MyFarmingGame.Tests;

/// <summary>Task 1.8: seed shop, shop hours, phone feed orders, feed bins, coop capacity.</summary>
public partial class TestRunner
{
    private void CheckShopRules()
    {
        ResetTask14State();
        Check(ShopCounter.OpenMinute == 7 * 60 + 30 && ShopCounter.CloseMinute == 16 * 60 + 30, "shop hours 07:30–16:30");
        Check(!ShopCounter.IsOpenAt(7 * 60 + 29) && ShopCounter.IsOpenAt(7 * 60 + 30) && ShopCounter.IsOpenAt(16 * 60 + 29)
              && !ShopCounter.IsOpenAt(16 * 60 + 30) && !ShopCounter.IsOpenAt(GameClock.DayStartMinute),
            "closed before 07:30 and from 16:30 (also at the 04:30 wake-up)");
        Check(FeedStore.DeliveryDay(3, 17 * 60 + 59) == 4 && FeedStore.DeliveryDay(3, 18 * 60) == 5 && FeedStore.DeliveryDay(3, 21 * 60) == 5,
            "phone feed: before 18:00 → next Dawn, from 18:00 → the day after");
        FeedStore.Order(ItemIds.ChickenFeed, 5, 1, 9 * 60);
        FeedStore.Order(ItemIds.Hay, 7, 1, 19 * 60);
        Check(FeedStore.TakeDue(ItemIds.Hay, 2) == 0 && FeedStore.TakeDue(ItemIds.ChickenFeed, 2) == 5, "Dawn of day 2: the grain arrives, the late hay doesn't");
        Check(FeedStore.TakeDue(ItemIds.ChickenFeed, 3) == 0 && FeedStore.TakeDue(ItemIds.Hay, 3) == 7 && FeedStore.Pending.Count == 0,
            "Dawn of day 3: the hay arrives; each delivery only once");
        var crop = CropCatalog.Default.BySeed(ItemIds.FlowerSeeds);
        Check(crop != null && crop.Id == "sun_sprout" && crop.DaysToHarvest == 4, "flower seeds grow Sun Sprouts in 4 days (crops.md)");
        ResetTask14State();
    }

    private async Task CheckShopInGame()
    {
        ResetTask14State();
        var clock = GameClock.Instance;
        var main = ResourceLoader.Load<PackedScene>("res://scenes/main/Main.tscn").Instantiate();
        AddChild(main);
        var level = main.GetNode("World/Level").GetChild(0);
        var player = main.GetNode<PlayerController>("World/Entities/Player");
        var shop = main.GetNode<ShopView>("HUD/UI/Shop");
        var bubble = main.GetNode<PictureBubbleView>("HUD/UI/PictureBubble");
        var fade = main.GetNode<DayTransitionView>("Transition/DayTransition");
        fade.FadeSeconds = 0.05f;
        fade.HoldSeconds = 0.05f;
        var counter = level.FindChild("ShopCounter", true, false) as ShopCounter;
        var phone = level.FindChild("Phone", true, false) as Phone;
        var hens = AnimalHouse.For(GetTree(), "hen");
        var cows = AnimalHouse.For(GetTree(), "cow");
        await Frames(3);
        Check(counter?.GetParent()?.Name == "SeedShed" && phone?.GetParent()?.Name == "House", "the counter is in the seed shed, the phone in the house");
        Check(hens != null && hens.GetParent().Name == "Coop" && hens.Animals.Count == 5 && hens.Capacity == 5 && hens.Feed == 35 && hens.FeedItem == ItemIds.ChickenFeed,
            "old coop: 5 hens, room for 5, 35 portions of chicken feed (≈ 7 days)");
        Check(cows != null && cows.GetParent().Name == "Barn" && cows.Animals.Count == 1 && cows.Capacity == 5 && cows.Feed == 7 && cows.FeedItem == ItemIds.Hay,
            "old barn: 1 cow, 5 stalls, 7 portions of hay");
        Check(hens.Bin != null && hens.Bin.CountText == "35" && cows.Bin.CountText == "7", "feed bins show 35 and 7");

        // Closed at 04:30: the closed sign and the opening time.
        player.GlobalPosition = counter.ApproachPoint;
        await Frames(5);
        await Tap(JoyButton.A);
        Check(!shop.IsOpen && bubble.IsOpen && bubble.Chips.Any(c => c.Sprite == "icon_closed") && bubble.Chips.Any(c => c.Label == "07:30"),
            "at 04:30 the counter shows a closed sign and 07:30");
        bubble.PressCancel();
        await Frames(2);

        // Open: buy seeds; can't overspend; coop full.
        clock.SetTime(1, ShopCounter.OpenMinute - GameClock.DayStartMinute + 30);
        await Tap(JoyButton.A);
        Check(shop.IsOpen && shop.BuyButtons.Count == 3 && clock.IsHeld(ShopView.HoldReason) && player.Frozen,
            "08:00: gamepad A opens the shop (seeds, flowers, hen); clock held, Kit still");
        int seeds = Inventory.Instance.Count(ItemIds.CarrotSeeds);
        Check(shop.Buy(0) && Wallet.Instance.Coins == 480 && Inventory.Instance.Count(ItemIds.CarrotSeeds) == seeds + 9, "carrot seeds: −20 coins, +9 seeds");
        Check(shop.Buy(1) && Wallet.Instance.Coins == 460 && Inventory.Instance.Count(ItemIds.FlowerSeeds) == 9, "flower seeds: −20 coins, +9 seeds");
        Check(!shop.Buy(2) && shop.LastRefusal == "icon_coop_full" && Wallet.Instance.Coins == 460 && hens.Animals.Count == 5,
            "hen while the coop is full: refused with the 'coop is full' picture, no coins spent");
        hens.RemoveAnimal(hens.Animals[0]);
        Check(shop.Buy(2) && Wallet.Instance.Coins == 285 && hens.Animals.Count == 5, "with room in the coop a hen costs 175 and moves in");
        Wallet.Instance.SetCoins(10);
        Check(!shop.Buy(0) && shop.LastRefusal == "coin" && Wallet.Instance.Coins == 10, "not enough coins: refused, wallet unchanged");
        await Tap(JoyButton.B);
        Check(!shop.IsOpen && !clock.IsHeld(ShopView.HoldReason) && !player.Frozen, "B leaves the shop");
        clock.SetTime(1, ShopCounter.CloseMinute - GameClock.DayStartMinute);
        await Tap(JoyButton.A);
        Check(!shop.IsOpen && bubble.IsOpen, "at 16:30 the shop is closed again");
        bubble.PressCancel();
        await Frames(2);

        // Seed bag picks the chosen packet from the bag.
        var bag = main.GetNode<BagView>("HUD/UI/Bag");
        bag.Open();
        await Frames(2);
        Check(bag.SeedButton(ItemIds.FlowerSeeds) != null && bag.PlantingSeed == ItemIds.CarrotSeeds, "in the bag seed packets are buttons; carrots plant first");
        bag.SeedButton(ItemIds.FlowerSeeds).EmitSignal(BaseButton.SignalName.Pressed);
        Check(ToolBelt.Instance.SelectedSeed == ItemIds.FlowerSeeds && bag.PlantingSeed == ItemIds.FlowerSeeds, "pressing the flower packet makes the seed bag plant flowers");
        bag.Close();
        var grid = level.GetNode<FarmGrid>("FrontGarden");
        grid.UseTool(Tool.SeedBag, new Vector2I(5, 3));
        Check(grid.Get(new Vector2I(5, 3)).CropId == "sun_sprout" && Inventory.Instance.Count(ItemIds.FlowerSeeds) == 8, "the seed bag plants a Sun Sprout");

        // Phone at home: feed before 18:00 → bins at the next Dawn.
        Wallet.Instance.SetCoins(500);
        clock.SetTime(1, 17 * 60 - GameClock.DayStartMinute);
        player.GlobalPosition = phone.ApproachPoint;
        await Frames(5);
        Check(clock.IsIndoors, "the phone is indoors (clock stopped)");
        await Tap(JoyButton.A);
        Check(shop.IsOpen && shop.Title == "phone" && shop.BuyButtons.Count == 4, "gamepad A on the phone opens the feed order (2 grain packs, 2 hay packs)");
        Check(shop.Buy(1) && Wallet.Instance.Coins == 465, "a week of chicken feed (35 portions) costs 35");
        Check(shop.Buy(2) && Wallet.Instance.Coins == 440, "a day of hay costs 25");
        shop.Close();
        Check(hens.Feed == 35 && cows.Feed == 7, "nothing arrives the same day");
        clock.BeginSleep(SleepReason.Bed);
        await WaitUntil(() => !clock.IsSleeping && !main.GetNode<SleepSequence>("Systems/SleepSequence").IsRunning, 600);
        Check(hens.Feed == 70 && cows.Feed == 8 && hens.Bin.CountText == "70", "next Dawn: grain into the coop bin, hay into the barn bin");
        // After 18:00 → the day after.
        clock.SetTime(clock.Day, 19 * 60 - GameClock.DayStartMinute);
        player.GlobalPosition = phone.ApproachPoint;
        await Frames(5);
        await Tap(JoyButton.A);
        Check(shop.IsOpen && shop.Buy(2), "ordering hay at 19:00");
        shop.Close();
        clock.BeginSleep(SleepReason.Bed);
        await WaitUntil(() => !clock.IsSleeping && !main.GetNode<SleepSequence>("Systems/SleepSequence").IsRunning, 600);
        Check(cows.Feed == 8, "ordered after 18:00: not there the next Dawn");
        clock.BeginSleep(SleepReason.Bed);
        await WaitUntil(() => !clock.IsSleeping && !main.GetNode<SleepSequence>("Systems/SleepSequence").IsRunning, 600);
        Check(cows.Feed == 9, "…but the Dawn after");

        main.QueueFree();
        await Frames(2);
        ResetTask14State();
    }
}
