using System;
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

/// <summary>Task 1.7: order generator, board, scripted carrot order, repeat orders.</summary>
public partial class TestRunner
{
    private void PayFirstOrder(OrderManager om)
    {
        om.Accept(OrderManager.FirstOrderId);
        Inventory.Instance.Add(ItemIds.Berries, 3);
        om.Deliver(OrderManager.FirstOrderId);
    }

    private void CheckOrderRules()
    {
        ResetTask14State();
        var om = new OrderManager { TestDay = 1 };
        AddChild(om);

        // Generator: only producible items, never more than the farm can supply.
        var supply = new Dictionary<string, int> { { ItemIds.Carrot, 3 }, { ItemIds.Berries, 5 } };
        bool onlyProducible = true, withinSupply = true;
        var carrotSizes = new HashSet<int>();
        for (int seed = 0; seed < 1000; seed++)
        {
            var o = om.GenerateOne(1 + seed % 30, supply, new Random(seed), "t" + seed);
            if (o == null || o.Lines.Any(l => !supply.ContainsKey(l.Item))) { onlyProducible = false; continue; }
            foreach (var l in o.Lines)
                if (l.Count < 1 || l.Count > supply[l.Item] || l.Count > OrderManager.MaxOrderCount) withinSupply = false;
            if (o.ItemId == ItemIds.Carrot && !o.IsBasket) carrotSizes.Add(o.Count);
        }
        Check(onlyProducible, "generator (1000 seeds): every line is something Kit can produce");
        Check(withinSupply, "generator: every count between 1 and the farm's supply (≤ 6 at T1)");
        Check(carrotSizes.SetEquals(new[] { 1, 2, 3 }), $"A: sizes vary (carrot orders of {string.Join(",", carrotSizes.OrderBy(c => c))})");
        var withAnimals = new Dictionary<string, int> { { ItemIds.Carrot, 3 }, { ItemIds.Egg, 5 }, { ItemIds.Milk, 4 } };
        var seen = new HashSet<string>();
        for (int seed = 0; seed < 500; seed++)
            seen.Add(om.GenerateOne(1, withAnimals, new Random(seed), "a" + seed).ItemId);
        Check(seen.SetEquals(new[] { ItemIds.Carrot, ItemIds.Egg, ItemIds.Milk }), "generator: eggs and milk appear once hens and the cow produce");
        Check(om.GenerateOne(1, new Dictionary<string, int>(), new Random(1), "x") == null
              && om.GenerateOne(1, new Dictionary<string, int> { { ItemIds.Egg, 0 } }, new Random(1), "y") == null,
            "generator: nothing producible → no order");

        // B mixed baskets and F quick bonuses appear now and then, priced by the rules.
        int baskets = 0, quick = 0, total = 0;
        bool basketPrice = true;
        for (int seed = 0; seed < 400; seed++)
        {
            var o = om.GenerateOne(5, withAnimals, new Random(seed), "v" + seed);
            total++;
            if (o.IsBasket)
            {
                baskets++;
                int sum = o.Lines.Sum(l => PriceTable.OrderReward(l.Item, l.Count));
                basketPrice &= o.Reward == Mathf.RoundToInt(sum * Order.BasketBonus) && o.Lines.Select(l => l.Item).Distinct().Count() == o.Lines.Count();
            }
            if (o.BonusUntilDay == 5) quick++;
        }
        Check(baskets > total / 10 && baskets < total / 3 && basketPrice, $"B: mixed baskets are uncommon ({baskets}/{total}) and pay ×1.1 of their lines");
        Check(quick > total / 8 && quick < total / 2, $"F: quick-bonus orders appear now and then ({quick}/{total})");

        // Before the first order: no board, no daily orders.
        om.OnDawn(2, supply);
        Check(!om.BoardOpen && om.Orders.Count == 1, "no daily orders before the board opens");
        PayFirstOrder(om);
        var carrots = om.Get(OrderManager.SecondOrderId);
        Check(om.BoardOpen && carrots != null && carrots.ItemId == ItemIds.Carrot && carrots.Count == 5 && carrots.State == OrderState.Offered,
            "paying the berries pins Marlow's '5 carrots' order on the board");
        Check(carrots.Reward == PriceTable.OrderReward(ItemIds.Carrot, 5), $"5 carrots pay {carrots.Reward} coins (5 × 4, no premium)");
        om.OnDawn(2, supply);
        var board = om.BoardOrders.ToList();
        var singles = board.Where(o => !o.IsBasket).Select(o => (o.ItemId, o.Count)).ToList();
        Check(board.Count >= 3 && board.Count <= OrderManager.MaxBoardOffers && board.All(o => o.Lines.All(l => supply.ContainsKey(l.Item))),
            $"D: Dawn fills the board with up to 5 producible orders ({board.Count})");
        Check(singles.Distinct().Count() == singles.Count && board.Count(o => o.ItemId == ItemIds.Carrot) >= 2,
            "D: the same item can appear twice, never in the same size");
        om.OnDawn(3, supply);
        Check(board.All(o => om.Get(o.Id) != null), "untouched orders wait (no deadline)");
        Check(!om.Decline(OrderManager.FirstOrderId), "a paid order can't be cancelled");
        var gen = board.First(o => o.Id.StartsWith("gen_"));
        om.Accept(gen.Id);
        Check(om.Decline(gen.Id) && gen.State == OrderState.Offered, "cancelling an accepted order: back on the board, no penalty");

        // F: quick bonus — extra coins if handed in on its day, the plain reward later.
        var fast = Order.Make("fast", Neighbours.Marlow, ItemIds.Carrot, 2, 3);
        fast.BonusUntilDay = 3;
        var slow = Order.Make("slow", Neighbours.Marlow, ItemIds.Carrot, 2, 3);
        slow.BonusUntilDay = 3;
        om.Add(fast); om.Add(slow);
        om.Accept("fast"); om.Accept("slow");
        Inventory.Instance.Add(ItemIds.Carrot, 4);
        om.TestDay = 3;
        int c0 = Wallet.Instance.Coins;
        om.Deliver("fast");
        Check(Wallet.Instance.Coins == c0 + fast.Reward + fast.Bonus && fast.Bonus > 0, $"F: delivered in time pays {fast.Reward} + {fast.Bonus} bonus");
        om.TestDay = 4;
        c0 = Wallet.Instance.Coins;
        om.Deliver("slow");
        Check(Wallet.Instance.Coins == c0 + slow.Reward && slow.State == OrderState.Paid, "F: late is no problem — the plain reward, no penalty");

        // Repeat orders after 3 deliveries to one neighbour (fast + slow + the berries = 3).
        var rep = om.Get(OrderManager.RepeatId(Neighbours.Marlow));
        Check(om.DeliveredBy(Neighbours.Marlow) == 3 && rep != null && rep.Repeat && rep.State == OrderState.Offered,
            "after 3 delivered orders Marlow offers a repeat order");
        int startCount = rep.Count;
        var big = new Dictionary<string, int> { { ItemIds.Carrot, 9 }, { ItemIds.Berries, 9 }, { ItemIds.Egg, 9 }, { ItemIds.Milk, 9 } };
        for (int i = 0; i < 2; i++)
        {
            om.Accept(rep.Id);
            Inventory.Instance.Add(rep.ItemId, rep.Count);
            Check(om.Deliver(rep.Id) && rep.State == OrderState.Paid, $"repeat order delivered (#{i + 1})");
            om.OnDawn(10 + i, big);
            Check(rep.State == OrderState.Offered, $"the repeat order comes back at Dawn (#{i + 1})");
        }
        Check(rep.Count == startCount + 1 && rep.Reward == PriceTable.OrderReward(rep.ItemId, rep.Count),
            $"the repeat order grows by one after two deliveries ({startCount} → {rep.Count})");
        Check(om.Orders.Count(o => o.Repeat && o.Giver == Neighbours.Marlow) == 1, "only one repeat order per neighbour");

        // C: a daily standing order after 4 deliveries (Marlow has 5 now).
        var daily = om.Get(OrderManager.StandingId(Neighbours.Marlow));
        Check(daily != null && daily.Standing && daily.ItemId == ItemIds.Egg && daily.State == OrderState.Offered,
            "C: after 4 deliveries Marlow offers a daily standing order (eggs)");
        om.Accept(daily.Id);
        om.TestDay = 12;
        Inventory.Instance.Add(ItemIds.Egg, 20);
        c0 = Wallet.Instance.Coins;
        Check(om.Deliver(daily.Id) && daily.State == OrderState.Accepted && daily.DeliveredOnDay == 12 && Wallet.Instance.Coins == c0 + daily.Reward,
            "C: delivering it pays and keeps the subscription");
        Check(!om.CanDeliver(daily.Id) && om.Subscriptions.Contains(daily), "C: once a day only");
        om.TestDay = 14;
        Check(om.CanDeliver(daily.Id), "C: a missed day isn't paid, and the next day it can be delivered again");
        Check(om.Decline(daily.Id) && !om.Subscriptions.Any(), "C: the subscription can be stopped any time");

        // E: new neighbours, one at a time, after visible actions (a fresh game).
        var om2 = new OrderManager { TestDay = 1 };
        AddChild(om2);
        var hello = new List<string>();
        void OnHello(string id) => hello.Add(id);
        om2.NeighbourIntroduced += OnHello;
        PayFirstOrder(om2);
        om2.OnDawn(2, big);
        Check(hello.Count == 0 && om2.ActiveNeighbours.Count == 1, "E: Day 1–2 only Mr. Marlow posts orders");
        om2.NoteProduced(ItemIds.Egg);
        om2.OnDawn(3, big);
        Check(hello.Count == 1 && hello[0] == Neighbours.Baker && om2.Get("hello_baker")?.Giver == Neighbours.Baker, "E: first eggs collected → the baker pins an order");
        om2.OnDawn(4, big);
        Check(hello.Count == 1, "E: the allotment neighbour waits for a delivered crop order");
        foreach (var n in new[] { "c1", "c2", "c3", "c4" })
        {
            var crop = Order.Make(n, Neighbours.Marlow, ItemIds.Carrot, 1, 4);
            om2.Add(crop); om2.Accept(n); Inventory.Instance.Add(ItemIds.Carrot, 1); om2.Deliver(n);
        }
        om2.OnDawn(5, big);
        Check(hello.Count == 2 && hello[1] == Neighbours.Allotment, "E: a delivered carrot order brings the allotment neighbour (one per Dawn)");
        om2.OnDawn(6, big);
        Check(hello.Count == 3 && hello[2] == Neighbours.Shopkeeper && om2.TotalDelivered >= 5, "E: 5 delivered orders bring the shopkeeper");
        Check(om2.BoardOrders.Select(o => o.Giver).Distinct().Count() >= 2, "E: the board now shows orders from several neighbours");
        Check(Neighbours.Get(Neighbours.Baker).LikesItem(ItemIds.Egg) && !Neighbours.Get(Neighbours.Baker).LikesItem(ItemIds.Carrot),
            "E: each neighbour orders what they like (the baker: eggs, milk, berries)");
        om2.NeighbourIntroduced -= OnHello;
        om2.QueueFree();
        var back = Order.FromDict(daily.ToDict());
        Check(back.Standing && back.DeliveredOnDay == 12, "Order dict round-trip keeps daily orders");
        var b2 = Order.FromDict(new Order { Id = "b", ItemId = "egg", Count = 2, Extra = { new OrderLine("milk", 1) }, BonusUntilDay = 4 }.ToDict());
        Check(b2.IsBasket && b2.Extra[0].Item == "milk" && b2.BonusUntilDay == 4, "…and baskets and quick bonuses");
        om.QueueFree();
        ResetTask14State();
    }

    private async Task CheckBoardInGame()
    {
        ResetTask14State();
        var clock = GameClock.Instance;
        var main = ResourceLoader.Load<PackedScene>("res://scenes/main/Main.tscn").Instantiate();
        AddChild(main);
        var level = main.GetNode("World/Level").GetChild(0);
        var player = main.GetNode<PlayerController>("World/Entities/Player");
        var boardProp = level.GetNodeOrNull<OrderBoard>("OrderBoard");
        var view = main.GetNodeOrNull<OrderBoardView>("HUD/UI/OrderBoard");
        var bubble = main.GetNode<PictureBubbleView>("HUD/UI/PictureBubble");
        var fade = main.GetNode<DayTransitionView>("Transition/DayTransition");
        fade.FadeSeconds = 0.05f;
        fade.HoldSeconds = 0.05f;
        var om = OrderManager.Instance;
        await Frames(3);
        Check(boardProp != null && view != null && !boardProp.Visible && !boardProp.CanInteract, "the order board is hidden until the first order is paid");
        Check(Production.Supply().TryGetValue(ItemIds.Carrot, out var c) && c >= 3 && Production.Supply()[ItemIds.Berries] == 5
              && Production.Supply()[ItemIds.Egg] == 5 && Production.Supply()[ItemIds.Milk] == 4,
            "farm supply: carrots from the garden, 5 berry bushes, 5 eggs (hens), 4 milk cans (cow)");
        Check(Production.Supply().ContainsKey(ItemIds.SunSprout), "farm supply: flowers (seeds on sale at the shed)");
        PayFirstOrder(om);
        await Frames(3);
        Check(boardProp.Visible && boardProp.CanInteract, "after the berries the board stands by the shed");

        // Next Dawn: 3 cards.
        clock.BeginSleep(SleepReason.Bed);
        await WaitUntil(() => !clock.IsSleeping && !main.GetNode<SleepSequence>("Systems/SleepSequence").IsRunning, 600);
        int cards = om.BoardOrders.Count();
        Check(cards >= 3 && cards <= OrderManager.MaxBoardOffers, $"the next Dawn the board holds up to 5 orders ({cards})");
        player.GlobalPosition = boardProp.ApproachPoint;
        await Frames(5);
        Check(boardProp.PromptVisible, "board prompt shows when Kit is in front of it");
        await Tap(JoyButton.A);
        await Frames(2);
        Check(view.IsOpen && view.Cards.Count == cards && clock.IsHeld(OrderBoardView.HoldReason) && player.Frozen,
            "gamepad A opens the board: one picture card per order, clock held, Kit still");
        Check(view.Cards[0].HasFocus(), "the first card has gamepad focus");
        string id = view.CardOrderIds[0];
        view.Cards[0].EmitSignal(BaseButton.SignalName.Pressed);
        await Frames(2);
        Check(!view.IsOpen && bubble.IsOpen && bubble.Buttons[0].Text == "Yes", "picking a card asks 'Take this order?'");
        bubble.PressButton(0);
        await Frames(2);
        Check(om.Get(id).State == OrderState.Accepted && view.IsOpen && view.Cards.Count == cards, "'Yes' accepts it and returns to the board");
        await Tap(JoyButton.B);
        Check(!view.IsOpen && !clock.IsHeld(OrderBoardView.HoldReason) && !player.Frozen, "gamepad B closes the board");

        // Deliver from the board.
        var o = om.Get(id);
        foreach (var l in o.Lines) Inventory.Instance.Add(l.Item, l.Count);
        int coins = Wallet.Instance.Coins;
        view.Open();
        view.PickCard(id);
        Check(bubble.IsOpen && bubble.Buttons[0].Text == "Give", "an order Kit can fill offers 'Give'");
        bubble.PressButton(0);
        Check(o.State == OrderState.Paid && Wallet.Instance.Coins == coins + o.Reward + (o.BonusOpen(clock.Day) ? o.Bonus : 0), "'Give' on the board pays the order once");
        bubble.PressCancel();
        await Frames(2);
        Check(view.IsOpen && view.Cards.Count == cards - 1, "the paid card leaves the board");
        view.Close();
        await Frames(2);

        main.QueueFree();
        await Frames(2);
        ResetTask14State();
    }
}
