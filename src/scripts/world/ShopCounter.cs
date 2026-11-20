using System.Collections.Generic;
using Godot;
using MyFarmingGame.Autoloads;
using MyFarmingGame.Core;
using MyFarmingGame.UI;

namespace MyFarmingGame.World;

/// <summary>
/// Marlow's seed-shed counter (Task 1.8): seed packets (carrot, flower; 20 coins, packet of 9) and
/// laying hens (175, only while the coop has room), at economy.csv prices. Open
/// <see cref="OpenMinute"/>–<see cref="CloseMinute"/> (07:30–16:30, GDD §4 phases); outside those
/// hours Marlow's picture bubble shows the closed sign and the opening time.
/// </summary>
public partial class ShopCounter : Prop
{
    public const int OpenMinute = 7 * 60 + 30;
    public const int CloseMinute = 16 * 60 + 30;
    public const int SeedsPerPacket = 9;

    public ShopCounter()
    {
        SpriteName = "shop_counter";
        DisplaySize = new Vector2(96, 64);
        PromptSprite = "item_carrot_seeds";
        BodySize = new Vector2(96, 30);
        BodyOffset = new Vector2(0, 10);
        ClickSize = new Vector2(96, 64);
        ApproachOffset = new Vector2(0, 60);
    }

    /// <summary>Is the shop open at <paramref name="minuteOfDay"/>?</summary>
    public static bool IsOpenAt(int minuteOfDay) => minuteOfDay >= OpenMinute && minuteOfDay < CloseMinute;

    public static bool IsOpenNow => IsOpenAt((int)(GameClock.Instance?.AbsoluteMinute ?? OpenMinute));

    public override void Interact(Node2D player)
    {
        if (!IsOpenNow)
        {
            PictureBubbleView.Find(GetTree())?.Show(Marlow.Portrait, Lines.Closed,
                new[] { new BubbleChip("icon_closed", ""), new BubbleChip("icon_clock", GameClock.FormatTime(OpenMinute)) },
                new[] { BubbleButton.Close() });
            return;
        }
        ShopView.Find(GetTree())?.Open("shop_counter", Entries(GetTree()));
    }

    /// <summary>What the seed shed sells (prices from economy.csv via <see cref="PriceTable"/>).</summary>
    public static List<ShopEntry> Entries(SceneTree tree) => new()
    {
        new ShopEntry { Id = ItemIds.CarrotSeeds, Sprite = ItemIds.SpriteName(ItemIds.CarrotSeeds), Count = SeedsPerPacket,
            Price = PriceTable.Get(ItemIds.CarrotSeeds), OnBuy = () => Inventory.Instance?.Add(ItemIds.CarrotSeeds, SeedsPerPacket) },
        new ShopEntry { Id = ItemIds.FlowerSeeds, Sprite = ItemIds.SpriteName(ItemIds.FlowerSeeds), Count = SeedsPerPacket,
            Price = PriceTable.Get(ItemIds.FlowerSeeds), OnBuy = () => Inventory.Instance?.Add(ItemIds.FlowerSeeds, SeedsPerPacket) },
        new ShopEntry { Id = "hen", Sprite = "hen", Count = 1, Price = PriceTable.Get("hen"),
            Blocked = () => AnimalHouse.For(tree, "hen")?.HasRoom == true ? null : "icon_coop_full",
            OnBuy = () => AnimalHouse.For(tree, "hen")?.AddAnimal() },
    };

    public static class Lines
    {
        public const string Closed = "The shop's closed now. Come back in the morning.";
    }
}
