using System.Collections.Generic;
using System.Linq;
using Godot;
using MyFarmingGame.Autoloads;
using MyFarmingGame.Core;
using MyFarmingGame.UI;

namespace MyFarmingGame.World;

/// <summary>
/// Mr. Marlow at his seed shed (Task 1.6, characters.md §8). Talking opens the picture bubble:
/// one picture row + one short line + big buttons. He names the want and points (layers 1–2), never
/// says how, never scolds, and restates an unfinished order kindly. First order: 3 handfuls of wild
/// berries → coins (once), then the order board opens (Task 1.7 builds it).
/// </summary>
public partial class Marlow : Prop
{
    public const string Portrait = "npc_marlow";

    public Marlow()
    {
        SpriteName = "npc_marlow";
        PromptSprite = "icon_talk";
        BodySize = new Vector2(40, 24);
        BodyOffset = new Vector2(0, 20);
        ApproachOffset = new Vector2(0, 64);
        Reach = 80f;
    }

    public override void Interact(Node2D player) => Talk();

    /// <summary>Opens the right conversation for the current orders.</summary>
    public void Talk()
    {
        var bubble = PictureBubbleView.Find(GetTree());
        var orders = OrderManager.Instance;
        if (bubble == null || orders == null) return;

        // Deliverable first, then accepted-but-not-ready, then new offers.
        var open = orders.OpenFrom("marlow").ToList();
        var ready = open.FirstOrDefault(o => o.State == OrderState.Accepted && orders.CanDeliver(o.Id));
        if (ready != null) { AskToDeliver(bubble, ready); return; }
        var waiting = open.FirstOrDefault(o => o.State == OrderState.Accepted);
        if (waiting != null) { Restate(bubble, waiting); return; }
        // He only asks in person for his scripted orders; daily orders wait on the board (Task 1.7).
        var offer = open.FirstOrDefault(o => o.State == OrderState.Offered && IsScripted(o));
        if (offer != null) { Offer(bubble, offer); return; }
        bubble.Show(Portrait, Lines.Idle, null, new[] { BubbleButton.Close() });
    }

    /// <summary>"have/need" pictures for each line of an order.</summary>
    public static List<BubbleChip> HaveNeedChips(Order o)
    {
        var chips = new List<BubbleChip>();
        foreach (var l in o.Lines)
            chips.Add(new BubbleChip(ItemIds.SpriteName(l.Item), $"{Inventory.Instance?.CountAll(l.Item) ?? 0}/{l.Count}"));
        return chips;
    }

    public static bool IsScripted(Order o) => o.Id == OrderManager.FirstOrderId || o.Id == OrderManager.SecondOrderId;

    /// <summary>"item × count → coins" pictures for an order.</summary>
    public static List<BubbleChip> OrderChips(Order o)
    {
        var chips = new List<BubbleChip>();
        foreach (var l in o.Lines) chips.Add(new BubbleChip(ItemIds.SpriteName(l.Item), "×" + l.Count));
        chips.Add(new BubbleChip("", "→"));
        chips.Add(new BubbleChip("coin", o.Reward.ToString()));
        int today = GameClock.Instance?.Day ?? 1;
        if (o.BonusOpen(today)) chips.Add(new BubbleChip("icon_clock", "+" + o.Bonus));
        if (o.Standing) chips.Add(new BubbleChip("icon_daily", ""));
        return chips;
    }

    private void Offer(PictureBubbleView bubble, Order o)
    {
        bubble.Show(Portrait, Lines.Ask(o), OrderChips(o), new[]
        {
            new BubbleButton { Icon = "icon_check", Text = "Yes", OnPress = () => Accept(o) },
            new BubbleButton { Icon = "", Text = "Not now", IsCancel = true, OnPress = () => OrderManager.Instance?.Decline(o.Id) },
        });
    }

    private void Accept(Order o)
    {
        if (OrderManager.Instance?.Accept(o.Id) != true) return;
        Ledger.Note(o.Id, ItemIds.SpriteName(o.ItemId), false);
        PictureBubbleView.Find(GetTree())?.Show(Portrait, Lines.Accepted(o), OrderChips(o), new[] { BubbleButton.Close() });
    }

    private void Restate(PictureBubbleView bubble, Order o)
    {
        var chips = HaveNeedChips(o);
        bubble.Show(Portrait, Lines.Restate(o), chips, new[] { BubbleButton.Close() });
    }

    private void AskToDeliver(PictureBubbleView bubble, Order o)
    {
        bubble.Show(Portrait, Lines.Deliver(o), OrderChips(o), new[]
        {
            new BubbleButton { Icon = ItemIds.SpriteName(o.ItemId), Text = "Give", OnPress = () => Deliver(o) },
            new BubbleButton { Icon = "", Text = "Not yet", IsCancel = true },
        });
    }

    private void Deliver(Order o)
    {
        var orders = OrderManager.Instance;
        bool firstBoard = !orders.BoardOpen;
        int before = Wallet.Instance?.Coins ?? 0;
        if (!orders.Deliver(o.Id)) return;
        Ledger.Note(o.Id, ItemIds.SpriteName(o.ItemId), !o.Standing);
        var chips = new List<BubbleChip> { new("coin", "+" + ((Wallet.Instance?.Coins ?? 0) - before)) };
        var carrots = orders.Get(OrderManager.SecondOrderId);
        string line = Lines.Thanks(o);
        if (firstBoard && orders.BoardOpen)
        {
            chips.Add(new BubbleChip("icon_order_board", ""));
            if (carrots != null)
                chips.Add(new BubbleChip(ItemIds.SpriteName(carrots.ItemId), "×" + carrots.Count));
            line = Lines.BoardOpens;
        }
        PictureBubbleView.Find(GetTree())?.Show(Portrait, line, chips, new[] { BubbleButton.Close() });
    }

    /// <summary>Marlow's lines: short, warm, never "you should have" (characters.md §8).</summary>
    public static class Lines
    {
        public const string Idle = "Ah, Kit. Lovely day for growing things.";
        public const string BoardOpens = "Your folk sold me berries every summer. Here's my board. Could you grow me 5 carrots next?";

        public static string Ask(Order o) => o.ItemId switch
        {
            ItemIds.Berries => $"Could you bring me {o.Count} handfuls of wild berries? The bushes along the lane are full.",
            _ => $"Could you bring me {o.Count} {ItemIds.DisplayName(o.ItemId).ToLowerInvariant()}?",
        };

        public static string Accepted(Order o) => "Wonderful. I'll write it in my ledger.";

        public static string Restate(Order o) => o.ItemId switch
        {
            ItemIds.Berries => $"{o.Count} handfuls of berries, whenever you're ready. No hurry at all.",
            _ => $"{o.Count} {ItemIds.DisplayName(o.ItemId).ToLowerInvariant()}, whenever you're ready. No hurry.",
        };

        public static string Deliver(Order o) => "Oh! Are those for me?";

        public static string Thanks(Order o) => "Thank you, Kit. Just what I needed.";
    }
}
