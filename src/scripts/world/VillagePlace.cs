using System.Linq;
using Godot;
using MyFarmingGame.Autoloads;
using MyFarmingGame.Core;
using MyFarmingGame.UI;

namespace MyFarmingGame.World;

/// <summary>
/// The front of a neighbour's place on the village green (Phase-002 Task 2.3; places.md §2.1):
/// the bakery, the allotments, the general store. Until the neighbour has arrived
/// (<see cref="OrderManager.ActiveNeighbours"/>), a picture "closed" sign stands at the door and
/// using it shows what opens the place (a gentle lock, places.md §1): the first eggs for the
/// bakery, the first crop order for the allotments, five delivered orders for the shop. Once
/// open, the neighbour stands at the door and greets Kit. Door signs with orders come in Task 2.6.
/// </summary>
public partial class VillagePlace : Prop
{
    /// <summary>The neighbour who opens this place (<see cref="Neighbours"/> id).</summary>
    [Export] public string NeighbourId { get; set; } = Neighbours.Baker;

    /// <summary>The picture of what opens it (shown on the closed sign's bubble).</summary>
    [Export] public string OpensWith { get; set; } = "item_egg";

    /// <summary>The neighbour's look once open.</summary>
    [Export] public string NeighbourModel { get; set; } = "baker";

    public bool IsOpen { get; private set; }

    public VillagePlace()
    {
        ModelName = "closed_sign";
        SpriteName = "icon_closed";
        PromptSprite = "icon_closed";
        BodySize = new Vector3(0.5f, 1.5f, 0.4f);
        ApproachOffset = new Vector3(0, 0, 1.0f);
        Reach = 1.2f;
    }

    public override void _Ready()
    {
        base._Ready();
        Refresh();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        bool open = OrderManager.Instance?.ActiveNeighbours.Contains(NeighbourId) == true;
        if (open != IsOpen) Refresh();
    }

    private void Refresh()
    {
        IsOpen = OrderManager.Instance?.ActiveNeighbours.Contains(NeighbourId) == true;
        if (Model != null) Model.ModelName = IsOpen ? NeighbourModel : "closed_sign";
    }

    public override void Interact(Node3D player)
    {
        var n = Neighbours.Get(NeighbourId);
        var bubble = PictureBubbleView.Find(GetTree());
        if (bubble == null) return;
        if (!IsOpen)
        {
            bubble.Show("", "Closed for now.", new[] { new BubbleChip("icon_closed", ""), new BubbleChip(OpensWith, "") },
                new[] { BubbleButton.Close() });
            return;
        }
        var likes = new System.Collections.Generic.List<BubbleChip>();
        foreach (var item in n.Likes) likes.Add(new BubbleChip(ItemIds.SpriteName(item), ""));
        bubble.Show(n.Portrait, $"Hello, Kit! Come again soon.", likes.ToArray(), new[] { BubbleButton.Close() });
    }
}
