using Godot;
using MyFarmingGame.Autoloads;
using MyFarmingGame.Core;
using MyFarmingGame.UI;

namespace MyFarmingGame.World;

/// <summary>
/// One feeding place in a coop or barn trough, one per animal (Task 1.13 playtest fix, owner
/// 2026-11-21: "need some slot to identify food already in or not"). With the feed scoop, A / Space
/// / a click on an empty slot puts one portion from the building's bin into it: grain (or hay)
/// shows in the slot and that animal is fed for today. The food stays visible until the night, when
/// the animals eat it, so an empty slot in the morning means "fill me". Slot <see cref="Index"/> belongs
/// to the house's animal with the same index; a slot without an animal (room for one more) stays empty.
/// </summary>
public partial class TroughSlot : Prop
{
    public AnimalHouse House { get; set; }
    public int Index { get; set; }

    private NamedModel _food;

    public TroughSlot()
    {
        ModelName = "trough_slot";
        SpriteName = "tool_feed_scoop";
        PromptSprite = "tool_feed_scoop";
        BodySize = Vector3.Zero; // Kit can step up to it from any side
        ClickSize = new Vector3(0.55f, 0.5f, 0.45f);
        ApproachOffset = new Vector3(0, 0, 0.6f);
        Reach = 0.55f;
        PromptHeight = 0.8f;
        CastsShadow = false;
    }

    /// <summary>The animal this slot feeds, or null.</summary>
    public AnimalData Animal => House != null && Index < House.Animals.Count ? House.Animals[Index] : null;

    /// <summary>True when food lies in the slot today.</summary>
    public bool HasFood => Animal?.FedToday ?? false;

    /// <summary>True when an animal is waiting for food here.</summary>
    public bool WantsFood => Animal != null && !Animal.FedToday;

    public override void _Ready()
    {
        base._Ready();
        _food = new NamedModel { Name = "Food", ModelName = House?.FeedItem == ItemIds.Hay ? "feed_hay" : "feed_grain" };
        AddChild(_food);
        Refresh();
        if (House != null) House.AnimalsChanged += Refresh;
    }

    public override void _ExitTree()
    {
        if (House != null && IsInstanceValid(House)) House.AnimalsChanged -= Refresh;
    }

    private void Refresh()
    {
        if (_food != null) _food.Visible = HasFood;
    }

    public override void Interact(Node3D player)
    {
        if (House == null) return;
        if (ToolBelt.Instance?.Active != Tool.FeedScoop)
        {
            // Hint ladder step 1: show the tool this needs.
            PictureBubbleView.Find(GetTree())?.Show("", "Choose the feed scoop, then fill the trough.",
                new[] { new BubbleChip("tool_feed_scoop", ""), new BubbleChip(ItemIds.SpriteName(House.FeedItem), "×" + House.Feed) },
                new[] { BubbleButton.Close() });
            return;
        }
        var a = Animal;
        if (a == null || a.FedToday) return; // nobody to feed here, or already full
        if (!House.FeedOne(a)) { FeedBin.ShowEmpty(GetTree(), House); return; }
        Puff.At(this, GlobalPosition + new Vector3(0, 0.2f, 0), House.FeedItem == ItemIds.Hay ? Puff.Leaf : Puff.Grain, 12, 1.6f);
        Refresh();
    }
}
