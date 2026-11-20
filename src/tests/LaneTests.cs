using System.Linq;
using System.Threading.Tasks;
using Godot;
using MyFarmingGame.Autoloads;
using MyFarmingGame.Core;
using MyFarmingGame.Player;
using MyFarmingGame.UI;
using MyFarmingGame.World;

namespace MyFarmingGame.Tests;

/// <summary>Task 1.6: prices, the order flow, Marlow, berry bushes, the ledger and the lane.</summary>
public partial class TestRunner
{
    /// <summary>price_coins of one row of docs/data/economy.csv (null when the file is not there).</summary>
    private static int? CsvPrice(string name)
    {
        string path = ProjectSettings.GlobalizePath("res://") + "../docs/data/economy.csv";
        if (!System.IO.File.Exists(path)) return null;
        foreach (var line in System.IO.File.ReadAllLines(path))
            if (line.StartsWith(name + ","))
                return int.Parse(line.Split(',')[5]);
        return null;
    }

    private void CheckLaneRules()
    {
        ResetTask14State();
        // Prices (economy.csv → data/prices.json).
        Check(PriceTable.Get("berries") == 15 && PriceTable.Get("carrot") == 4 && PriceTable.Get("egg") == 6 && PriceTable.Get("milk") == 100,
            "prices: berries 15, carrot 4, egg 6, milk 100");
        Check(PriceTable.Get("carrot_seeds") == 20 && PriceTable.Get("flower_seeds") == 20 && PriceTable.Get("hen") == 175
              && PriceTable.Get("chicken_feed") == 1 && PriceTable.Get("hay") == 25,
            "prices: seed packets 20, hen 175, chicken feed 1, cow hay 25");
        foreach (var (id, name) in new[] { ("berries", "Wild berries (forage)"), ("carrot", "Carrot"), ("egg", "Egg"), ("milk", "Milk"),
                     ("carrot_seeds", "Carrot seeds"), ("flower_seeds", "Flower seeds"), ("hen", "Laying hen"),
                     ("chicken_feed", "Chicken feed"), ("hay", "Hay (cow portion)"), ("sun_sprout", "Sun Sprout (flower)") })
        {
            int? csv = CsvPrice(name);
            Check(csv == null || csv == PriceTable.Get(id), $"prices.json matches economy.csv for {id} ({csv})");
        }
        Check(PriceTable.OrderPremium == 1.0f && PriceTable.OrderReward("berries", 3) == 45, "first order pays 3 × 15 = 45 coins (no premium, owner 2026-11-21)");

        // Order flow on a private manager (inventory and wallet are the autoloads).
        var om = new OrderManager();
        AddChild(om);
        var o = om.Get(OrderManager.FirstOrderId);
        Check(o != null && o.State == OrderState.Offered && o.ItemId == "berries" && o.Count == 3 && o.Reward == 45,
            "new game: Marlow offers 3 handfuls of wild berries for 45");
        Check(!om.Deliver(o.Id) && Wallet.Instance.Coins == 500, "can't deliver an order that isn't accepted");
        Check(om.Accept(o.Id) && o.State == OrderState.Accepted, "accept → accepted");
        Check(!om.Deliver(o.Id) && Wallet.Instance.Coins == 500, "can't deliver without the berries");
        Check(om.Decline(o.Id) && o.State == OrderState.Offered, "declining puts it back on offer (still available)");
        om.Accept(o.Id);
        Inventory.Instance.Add(ItemIds.Berries, 4);
        int paid = 0, boards = 0;
        void OnPaid(string id, int coins) => paid++;
        void OnBoard() => boards++;
        om.OrderPaid += OnPaid;
        om.BoardOpened += OnBoard;
        Check(om.Deliver(o.Id) && o.State == OrderState.Paid && Wallet.Instance.Coins == 545 && Inventory.Instance.Count(ItemIds.Berries) == 1,
            "deliver: 3 berries taken, 45 coins paid");
        Check(!om.Deliver(o.Id) && Wallet.Instance.Coins == 545 && !om.Accept(o.Id) && !om.Decline(o.Id), "a paid order can't pay again or change");
        Check(paid == 1 && boards == 1 && om.BoardOpen, "OrderPaid and BoardOpened fire once; the board is open");
        om.OrderPaid -= OnPaid;
        om.BoardOpened -= OnBoard;
        var back = Order.FromDict(o.ToDict());
        Check(back.Id == o.Id && back.State == OrderState.Paid && back.Reward == 45 && back.Count == 3 && back.ItemId == "berries",
            "Order ToDict/FromDict round-trip");
        om.QueueFree();

        // Ledger pages.
        Ledger.Reset();
        Ledger.Note("a", "item_berries", false);
        Check(Ledger.Pages.Count == 1 && !Ledger.Pages[0].Done, "ledger: a promise is a page");
        Ledger.Note("a", "item_berries", true);
        Check(Ledger.Pages.Count == 1 && Ledger.Pages[0].Done, "ledger: the same page fills in when done");
        Ledger.Reset();

        // Berry bush regrow.
        GameClock.Instance.ResetForNewGame();
        var bush = new BerryBush();
        AddChild(bush);
        int before = Inventory.Instance.Count(ItemIds.Berries);
        bush.Interact(null);
        Check(!bush.HasBerries && Inventory.Instance.Count(ItemIds.Berries) == before + 1 && bush.Sprite.SpriteName == "berry_bush_empty",
            "picking a bush: one handful into the bag, bush bare");
        bush.Interact(null);
        Check(Inventory.Instance.Count(ItemIds.Berries) == before + 1 && !bush.CanInteract, "a bare bush gives nothing");
        bush.NewDay(2);
        Check(!bush.HasBerries, "still bare the next day");
        bush.NewDay(3);
        Check(bush.HasBerries && bush.Sprite.SpriteName == "berry_bush", "berries regrow after 2 days");
        bush.QueueFree();
        ResetTask14State();
    }

    private async Task Press(Key k)
    {
        Key(k, true);
        await IdleFrames(2);
        Key(k, false);
        await IdleFrames(2);
        await Frames(1);
    }

    private async Task CheckLaneInGame()
    {
        ResetTask14State();
        var clock = GameClock.Instance;
        var main = ResourceLoader.Load<PackedScene>("res://scenes/main/Main.tscn").Instantiate();
        AddChild(main);
        var level = main.GetNode("World/Level").GetChild(0);
        var player = main.GetNode<PlayerController>("World/Entities/Player");
        var bubble = main.GetNode<PictureBubbleView>("HUD/UI/PictureBubble");
        var fade = main.GetNode<DayTransitionView>("Transition/DayTransition");
        fade.FadeSeconds = 0.05f;
        fade.HoldSeconds = 0.05f;
        var marlow = level.GetNodeOrNull<Marlow>("Marlow");
        var shed = level.GetNodeOrNull<Building>("SeedShed");
        var ledger = level.FindChild("Ledger", true, false) as Ledger;
        var bushes = level.GetNode("Bushes").GetChildren().OfType<BerryBush>().ToList();
        Check(marlow != null && shed != null && shed.ZoneName == "shed" && ledger?.GetParent() == shed, "lane: Marlow at the seed shed, ledger inside the shed");
        Check(bushes.Count == 5 && bushes.All(b => b.HasBerries), "lane: 5 berry bushes, all full");
        var orders = OrderManager.Instance;
        await Frames(5);

        // Walk from the house up the lane to Marlow (≈ 20 tiles).
        var spawn = level.GetNode<Marker2D>("PlayerSpawn").GlobalPosition;
        player.GlobalPosition = spawn;
        await Frames(3);
        Check(await WalkLeg(player, new Vector2(1600, 40)) && await WalkLeg(player, new Vector2(1600, -1100), 800),
            "Kit walks from the front door through the gate up the lane to the seed shed");
        Check((spawn.Y - player.GlobalPosition.Y) / 64f >= 18, $"the shed is ≈ 20 tiles from the door ({(spawn.Y - player.GlobalPosition.Y) / 64f:0} tiles)");

        // Talk: gamepad A next to Marlow.
        player.GlobalPosition = marlow.ApproachPoint;
        await Frames(5);
        Check(marlow.PromptVisible, "Marlow shows a talk prompt when Kit is close");
        await Tap(JoyButton.A);
        var o = orders.Get(OrderManager.FirstOrderId);
        Check(bubble.IsOpen && bubble.Portrait == Marlow.Portrait && bubble.Line.Contains("berries"), "gamepad A: Marlow's picture bubble asks for berries");
        Check(bubble.Chips.Count == 3 && bubble.Chips[0].Sprite == "item_berries" && bubble.Chips[0].Label == "×3" && bubble.Chips[2].Label == "45",
            "bubble pictures: berries ×3 → coin 45");
        Check(clock.IsHeld(PictureBubbleView.HoldReason) && player.Frozen && bubble.Buttons[0].HasFocus() || bubble.Buttons[0].IsInsideTree(),
            "talking holds the clock and freezes Kit");
        await Tap(JoyButton.B);
        Check(!bubble.IsOpen && o.State == OrderState.Offered && !clock.IsHeld(PictureBubbleView.HoldReason) && !player.Frozen,
            $"[{bubble.IsOpen} {o.State} {clock.IsHeld(PictureBubbleView.HoldReason)} {player.Frozen}] gamepad B = 'Not now': bubble closes, order still offered, clock and Kit free");
        await Press(Godot.Key.Space);
        Check(bubble.IsOpen && bubble.Line.Contains("berries"), "talking again offers the berries again");
        bubble.PressButton(0);
        Check(o.State == OrderState.Accepted && bubble.IsOpen && Ledger.Pages.Count == 1 && !Ledger.Pages[0].Done,
            "'Yes' accepts; Marlow notes the promise in his ledger");
        bubble.PressCancel();
        await Press(Godot.Key.Space);
        Check(bubble.IsOpen && bubble.Line.Contains("No hurry") && bubble.Chips[0].Label == "0/3", "unfinished order restated kindly (0/3)");
        bubble.PressCancel();
        await Frames(2);

        // Pick three bushes.
        for (int i = 0; i < 3; i++)
        {
            player.GlobalPosition = bushes[i].ApproachPoint;
            await Frames(4);
            await Tap(JoyButton.A);
        }
        Check(Inventory.Instance.Count(ItemIds.Berries) == 3 && bushes.Take(3).All(b => !b.HasBerries), "gamepad A picks berries from 3 bushes");
        player.GlobalPosition = bushes[0].ApproachPoint;
        await Frames(4);
        await Tap(JoyButton.A);
        Check(Inventory.Instance.Count(ItemIds.Berries) == 3, "a picked bush gives nothing more");

        // Deliver: right-click = not yet; then Give.
        player.GlobalPosition = marlow.ApproachPoint;
        await Frames(4);
        await Tap(JoyButton.A);
        Check(bubble.IsOpen && bubble.Buttons.Count == 2 && bubble.Buttons[0].Text == "Give", "with 3 berries Marlow asks for them");
        var vp = GetViewport();
        Vector2 mid = vp.GetVisibleRect().Size / 2;
        vp.PushInput(new InputEventMouseButton { ButtonIndex = MouseButton.Right, Pressed = true, Position = mid, GlobalPosition = mid }, true);
        vp.PushInput(new InputEventMouseButton { ButtonIndex = MouseButton.Right, Pressed = false, Position = mid, GlobalPosition = mid }, true);
        await Frames(2);
        Check(!bubble.IsOpen && o.State == OrderState.Accepted && Inventory.Instance.Count(ItemIds.Berries) == 3, "right-click = 'Not yet': nothing handed over");
        await Tap(JoyButton.A);
        bubble.PressButton(0);
        Check(o.State == OrderState.Paid && Wallet.Instance.Coins == 545 && Inventory.Instance.Count(ItemIds.Berries) == 0 && orders.BoardOpen,
            "'Give': berries handed over, 45 coins, the order board opens");
        Check(bubble.IsOpen && bubble.Chips.Any(c => c.Sprite == "icon_order_board") && Ledger.Pages[0].Done, "thank-you bubble shows the board; ledger page done");
        bubble.PressCancel();
        await Tap(JoyButton.A);
        Check(bubble.IsOpen && Wallet.Instance.Coins == 545, "talking again pays nothing more");
        bubble.PressCancel();
        await Frames(2);

        // The ledger inside the shed (clock stopped indoors).
        player.GlobalPosition = shed.DoorInside;
        await Frames(5);
        Check(clock.IsIndoors && shed.Zone.PlayerInside, "the seed shed stops the clock");
        player.GlobalPosition = ledger.ApproachPoint;
        await Frames(4);
        await Tap(JoyButton.A);
        Check(bubble.IsOpen && bubble.Chips.Count == 1 && bubble.Chips[0].Sprite == "item_berries" && bubble.Chips[0].Label == "✓",
            "the ledger shows the berries page, done");
        await Tap(JoyButton.B);

        // Bushes regrow two days after picking.
        clock.BeginSleep(SleepReason.Bed);
        await WaitUntil(() => !clock.IsSleeping && !main.GetNode<SleepSequence>("Systems/SleepSequence").IsRunning, 600);
        Check(!bushes[0].HasBerries, "picked bushes are bare the next morning");
        clock.BeginSleep(SleepReason.Bed);
        await WaitUntil(() => !clock.IsSleeping && !main.GetNode<SleepSequence>("Systems/SleepSequence").IsRunning, 600);
        Check(bushes.All(b => b.HasBerries), "two days later every bush has berries again");

        main.QueueFree();
        await Frames(2);
        ResetTask14State();
    }
}
