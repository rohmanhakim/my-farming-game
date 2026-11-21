using System.Collections.Generic;
using Godot;
using MyFarmingGame.Autoloads;
using MyFarmingGame.Core;
using MyFarmingGame.UI;

namespace MyFarmingGame.World;

/// <summary>
/// The house phone with the feed seller's number on a note (Task 1.8, economy.md §2). Kit orders
/// chicken feed (1 coin per hen-day portion) and hay (25 per cow-day portion) any time Kit is home;
/// orders before 18:00 reach the coop/barn feed bins at the next Dawn, later ones the day after.
/// No fee. Packs are a day or a week for the starting animals (5 hens, 1 cow).
/// </summary>
public partial class Phone : Prop
{
    public Phone()
    {
        ModelName = "phone";
        SpriteName = "phone";
        PromptSprite = "phone";
        BodySize = new Vector3(0.5f, 0.9f, 0.4f);
        ClickSize = new Vector3(0.7f, 1.1f, 0.7f);
        ApproachOffset = new Vector3(0, 0, 0.8f);
        PromptHeight = 1.4f;
    }

    public override void Interact(Node3D player)
    {
        var clock = GameClock.Instance;
        int day = clock?.Day ?? 1;
        int minute = (int)(clock?.AbsoluteMinute ?? GameClock.DayStartMinute);
        bool tomorrow = FeedStore.DeliveryDay(day, minute) == day + 1;
        ShopView.Find(GetTree())?.Open("phone", Entries(), tomorrow ? Lines.Tomorrow : Lines.DayAfter);
    }

    private static ShopEntry Pack(string item, int portions) => new()
    {
        Id = item, Sprite = ItemIds.SpriteName(item), Count = portions, Price = PriceTable.Get(item) * portions,
        OnBuy = () =>
        {
            var c = GameClock.Instance;
            FeedStore.Order(item, portions, c?.Day ?? 1, (int)(c?.AbsoluteMinute ?? GameClock.DayStartMinute));
        },
    };

    /// <summary>Feed packs: a day and a week for 5 hens; a day and a week for 1 cow.</summary>
    public static List<ShopEntry> Entries() => new()
    {
        Pack(ItemIds.ChickenFeed, 5),
        Pack(ItemIds.ChickenFeed, 35),
        Pack(ItemIds.Hay, 1),
        Pack(ItemIds.Hay, 7),
    };

    public static class Lines
    {
        public const string Tomorrow = "Feed arrives in the bins tomorrow at dawn.";
        public const string DayAfter = "It's after six. Feed arrives the day after tomorrow.";
    }
}
