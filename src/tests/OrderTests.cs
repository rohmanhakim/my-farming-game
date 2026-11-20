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
        var om = new OrderManager();
        AddChild(om);

        // Generator: only producible items, never more than the farm can supply.
        var supply = new Dictionary<string, int> { { ItemIds.Carrot, 3 }, { ItemIds.Berries, 5 } };
        bool onlyProducible = true, withinSupply = true, sawCarrot = false, sawBerries = false;
        for (int seed = 0; seed < 1000; seed++)
        {
            var o = om.GenerateOne(1 + seed % 30, supply, new Random(seed), "t" + seed);
            if (o == null || !supply.ContainsKey(o.ItemId)) { onlyProducible = false; continue; }
            if (o.Count < 1 || o.Count > supply[o.ItemId] || o.Count > OrderManager.MaxOrderCount) withinSupply = false;
            sawCarrot |= o.ItemId == ItemIds.Carrot;
            sawBerries |= o.ItemId == ItemIds.Berries;
        }
        Check(onlyProducible && sawCarrot && !sawBerries, "generator (1000 seeds): only producible items; berries already on the board → a different item (carrots)");
        Check(withinSupply, "generator: counts between 1 and the farm's supply (≤ 6 at T1)");
        var withAnimals = new Dictionary<string, int> { { ItemIds.Carrot, 3 }, { ItemIds.Egg, 5 }, { ItemIds.Milk, 4 } };
        var seen = new HashSet<string>();
        for (int seed = 0; seed < 500; seed++)
            seen.Add(om.GenerateOne(1, withAnimals, new Random(seed), "a" + seed).ItemId);
        Check(seen.SetEquals(new[] { ItemIds.Carrot, ItemIds.Egg, ItemIds.Milk }), "generator: eggs and milk appear once hens and the cow produce");
        Check(om.GenerateOne(1, new Dictionary<string, int>(), new Random(1), "x") == null
              && om.GenerateOne(1, new Dictionary<string, int> { { ItemIds.Egg, 0 } }, new Random(1), "y") == null,
            "generator: nothing producible → no order");

        // Before the first order: no board, no daily orders.
        om.OnDawn(2, supply);
        Check(!om.BoardOpen && om.Orders.Count == 1, "no daily orders before the board opens");
        PayFirstOrder(om);
        var carrots = om.Get(OrderManager.SecondOrderId);
        Check(om.BoardOpen && carrots != null && carrots.ItemId == ItemIds.Carrot && carrots.Count == 5 && carrots.State == OrderState.Offered,
            "paying the berries pins Marlow's '5 carrots' order on the board");
        Check(carrots.Reward == PriceTable.OrderReward(ItemIds.Carrot, 5), $"5 carrots pay {carrots.Reward} coins (×1.25)");
        om.OnDawn(2, supply);
        var board = om.BoardOrders.ToList();
        Check(board.Count == 2 && board.All(o => supply.ContainsKey(o.ItemId)) && board.Select(o => o.ItemId).Distinct().Count() == 2,
            "Dawn fills the board with producible orders, one per item (carrots + berries = 2)");
        var three = new OrderManager();
        AddChild(three);
        PayFirstOrder(three);
        three.OnDawn(2, new Dictionary<string, int> { { ItemIds.Carrot, 3 }, { ItemIds.Berries, 5 }, { ItemIds.Egg, 5 }, { ItemIds.Milk, 4 } });
        Check(three.BoardOrders.Count() == OrderManager.MaxBoardOffers && three.BoardOrders.Select(o => o.ItemId).Distinct().Count() == 3,
            "with more to produce the board holds 3 different orders");
        three.QueueFree();
        om.OnDawn(3, supply);
        Check(om.BoardOrders.Count() == 2 && om.BoardOrders.Any(o => o.Id == board[1].Id), "untouched orders wait (no deadline)");
        Check(!om.Decline(OrderManager.FirstOrderId), "a paid order can't be cancelled");
        var gen = board.First(o => o.Id.StartsWith("gen_"));
        om.Accept(gen.Id);
        Check(om.Decline(gen.Id) && gen.State == OrderState.Offered, "cancelling an accepted order: back on the board, no penalty");

        // Repeat orders after 3 deliveries; they come back and grow slowly.
        int coins0 = Wallet.Instance.Coins;
        om.OnDawn(4, new Dictionary<string, int> { { ItemIds.Carrot, 3 }, { ItemIds.Berries, 5 }, { ItemIds.Egg, 5 } });
        foreach (var o in om.BoardOrders.Where(o => o.Id.StartsWith("gen_")).Take(2).ToList())
        {
            om.Accept(o.Id);
            Inventory.Instance.Add(o.ItemId, o.Count);
            om.Deliver(o.Id);
        }
        var rep = om.Get(OrderManager.RepeatId("marlow"));
        Check(om.DeliveredBy("marlow") == 3 && rep != null && rep.Repeat && rep.State == OrderState.Offered,
            "after 3 delivered orders Marlow offers a repeat order");
        int startCount = rep.Count;
        for (int i = 0; i < 2; i++)
        {
            om.Accept(rep.Id);
            Inventory.Instance.Add(rep.ItemId, rep.Count);
            Check(om.Deliver(rep.Id) && rep.State == OrderState.Paid, $"repeat order delivered (#{i + 1})");
            om.OnDawn(10 + i, new Dictionary<string, int> { { ItemIds.Carrot, 9 }, { ItemIds.Berries, 9 }, { ItemIds.Egg, 9 }, { ItemIds.Milk, 9 } });
            Check(rep.State == OrderState.Offered, $"the repeat order comes back at Dawn (#{i + 1})");
        }
        Check(rep.Count == startCount + 1 && rep.Reward == PriceTable.OrderReward(rep.ItemId, rep.Count),
            $"the repeat order grows by one after two deliveries ({startCount} → {rep.Count})");
        Check(om.Orders.Count(o => o.Repeat) == 1, "only one repeat order per neighbour");
        Check(Wallet.Instance.Coins > coins0, "deliveries paid");
        var next = om.GenerateOne(20, new Dictionary<string, int> { { ItemIds.Carrot, 9 } }, new Random(3), "big");
        Check(next.Count == Math.Min(OrderManager.BaseCount(ItemIds.Carrot) + om.TotalDelivered / 3, 6) && next.Count > OrderManager.BaseCount(ItemIds.Carrot),
            $"new orders grow slowly with deliveries ({next.Count} carrots after {om.TotalDelivered} orders)");
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
              && !Production.Supply().ContainsKey(ItemIds.Egg),
            "farm supply: carrots from the garden, 5 berry bushes; no eggs before Task 1.9's hens");
        Check(Production.Supply().ContainsKey(ItemIds.SunSprout), "farm supply: flowers (seeds on sale at the shed)");
        PayFirstOrder(om);
        await Frames(3);
        Check(boardProp.Visible && boardProp.CanInteract, "after the berries the board stands by the shed");

        // Next Dawn: 3 cards.
        clock.BeginSleep(SleepReason.Bed);
        await WaitUntil(() => !clock.IsSleeping && !main.GetNode<SleepSequence>("Systems/SleepSequence").IsRunning, 600);
        Check(om.BoardOrders.Count() == 3 && om.BoardOrders.Select(o => o.ItemId).Distinct().Count() == 3,
            "the next Dawn the board holds 3 different orders (carrots + berries + flowers)");
        player.GlobalPosition = boardProp.ApproachPoint;
        await Frames(5);
        Check(boardProp.PromptVisible, "board prompt shows when Kit is in front of it");
        await Tap(JoyButton.A);
        await Frames(2);
        Check(view.IsOpen && view.Cards.Count == 3 && clock.IsHeld(OrderBoardView.HoldReason) && player.Frozen,
            "gamepad A opens the board: 3 picture cards, clock held, Kit still");
        Check(view.Cards[0].HasFocus(), "the first card has gamepad focus");
        string id = view.CardOrderIds[0];
        view.Cards[0].EmitSignal(BaseButton.SignalName.Pressed);
        await Frames(2);
        Check(!view.IsOpen && bubble.IsOpen && bubble.Buttons[0].Text == "Yes", "picking a card asks 'Take this order?'");
        bubble.PressButton(0);
        await Frames(2);
        Check(om.Get(id).State == OrderState.Accepted && view.IsOpen && view.Cards.Count == 3, "'Yes' accepts it and returns to the board");
        await Tap(JoyButton.B);
        Check(!view.IsOpen && !clock.IsHeld(OrderBoardView.HoldReason) && !player.Frozen, "gamepad B closes the board");

        // Deliver from the board.
        var o = om.Get(id);
        Inventory.Instance.Add(o.ItemId, o.Count);
        int coins = Wallet.Instance.Coins;
        view.Open();
        view.PickCard(id);
        Check(bubble.IsOpen && bubble.Buttons[0].Text == "Give", "an order Kit can fill offers 'Give'");
        bubble.PressButton(0);
        Check(o.State == OrderState.Paid && Wallet.Instance.Coins == coins + o.Reward, "'Give' on the board pays the order once");
        bubble.PressCancel();
        await Frames(2);
        Check(view.IsOpen && view.Cards.Count == 2, "the paid card leaves the board");
        view.Close();
        await Frames(2);

        main.QueueFree();
        await Frames(2);
        ResetTask14State();
    }
}
