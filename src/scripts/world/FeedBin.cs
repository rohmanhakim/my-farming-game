using Godot;
using MyFarmingGame.Autoloads;
using MyFarmingGame.Core;
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
    private WorldPin _countPin;

    public FeedBin()
    {
        BodySize = new Vector3(0.8f, 0.7f, 0.5f);
        ClickSize = new Vector3(1f, 1f, 0.9f);
        ApproachOffset = new Vector3(0, 0, 0.8f);
        Reach = 1.0f;
        PromptHeight = 1.3f;
    }

    public override void _Ready()
    {
        SpriteName = House?.FeedItem == ItemIds.Hay ? "feed_bin_hay" : "feed_bin_grain";
        ModelName = SpriteName;
        PromptSprite = House != null ? ItemIds.SpriteName(House.FeedItem) : "";
        base._Ready();
        _count = new Label
        {
            Position = new Vector2(-28, -14), Size = new Vector2(56, 28), HorizontalAlignment = HorizontalAlignment.Center,
            LabelSettings = new LabelSettings { FontSize = 20, FontColor = Colors.White, OutlineSize = 6, OutlineColor = new Color(0.15f, 0.1f, 0.05f) },
            MouseFilter = Control.MouseFilterEnum.Ignore,
        };
        // The number of portions, pinned on the bin as screen UI.
        _countPin = new WorldPin { Name = "Count", WorldOffset = new Vector3(0, 0.75f, 0) };
        _countPin.AddChild(_count);
        AddChild(_countPin);
        if (House != null) House.FeedChanged += OnFeedChanged;
        OnFeedChanged(House?.Feed ?? 0);
    }

    public override void _ExitTree()
    {
        if (House != null && IsInstanceValid(House)) House.FeedChanged -= OnFeedChanged;
    }

    private void OnFeedChanged(int portions) { if (_count != null) _count.Text = portions.ToString(); }

    public string CountText => _count?.Text ?? "";

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (_countPin != null) _countPin.Shown = !UnderRoof;
    }

    /// <summary>With the feed scoop: feed every hungry animal here one portion (Task 1.9).
    /// Otherwise show how much is in the bin.</summary>
    public override void Interact(Node3D player)
    {
        if (House == null) return;
        if (ToolBelt.Instance?.Active == Tool.FeedScoop && House.Hungry > 0)
        {
            if (House.FeedAll() == 0) ShowEmpty(GetTree(), House);
            return;
        }
        PictureBubbleView.Find(GetTree())?.Show("", "", new[] { new BubbleChip(ItemIds.SpriteName(House.FeedItem), "×" + House.Feed) },
            new[] { BubbleButton.Close() });
    }

    /// <summary>The bin is empty: show it, and the phone that brings more.</summary>
    public static void ShowEmpty(SceneTree tree, AnimalHouse house) =>
        PictureBubbleView.Find(tree)?.Show("", "The bin is empty. Order feed with the phone at home.",
            new[] { new BubbleChip(ItemIds.SpriteName(house.FeedItem), "×0"), new BubbleChip("phone", "") }, new[] { BubbleButton.Close() });
}
