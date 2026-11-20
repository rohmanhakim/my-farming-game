using Godot;
using MyFarmingGame.Autoloads;
using MyFarmingGame.UI;

namespace MyFarmingGame.World;

/// <summary>
/// A building's feed bin (Task 1.8): shows how many portions are left (a number on the bin).
/// Using it shows the feed icon and the count in the picture bubble. Task 1.9 adds feeding the
/// animals with the feed scoop from here.
/// </summary>
public partial class FeedBin : Prop
{
    public AnimalHouse House { get; set; }
    private Label _count;

    public FeedBin()
    {
        DisplaySize = new Vector2(56, 56);
        BodySize = new Vector2(52, 20);
        BodyOffset = new Vector2(0, 14);
        ApproachOffset = new Vector2(0, 52);
        Reach = 64f;
    }

    public override void _Ready()
    {
        SpriteName = House?.FeedItem == ItemIds.Hay ? "feed_bin_hay" : "feed_bin_grain";
        PromptSprite = House != null ? ItemIds.SpriteName(House.FeedItem) : "";
        base._Ready();
        _count = new Label
        {
            Position = new Vector2(-28, -6), Size = new Vector2(56, 28), HorizontalAlignment = HorizontalAlignment.Center,
            LabelSettings = new LabelSettings { FontSize = 20, FontColor = Colors.White, OutlineSize = 6, OutlineColor = new Color(0.15f, 0.1f, 0.05f) },
            MouseFilter = Control.MouseFilterEnum.Ignore,
        };
        AddChild(_count);
        if (House != null) House.FeedChanged += OnFeedChanged;
        OnFeedChanged(House?.Feed ?? 0);
    }

    public override void _ExitTree()
    {
        if (House != null && IsInstanceValid(House)) House.FeedChanged -= OnFeedChanged;
    }

    private void OnFeedChanged(int portions) { if (_count != null) _count.Text = portions.ToString(); }

    public string CountText => _count?.Text ?? "";

    public override void Interact(Node2D player)
    {
        if (House == null) return;
        PictureBubbleView.Find(GetTree())?.Show("", "", new[] { new BubbleChip(ItemIds.SpriteName(House.FeedItem), "×" + House.Feed) },
            new[] { BubbleButton.Close() });
    }
}
