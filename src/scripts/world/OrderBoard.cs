using Godot;
using MyFarmingGame.Autoloads;
using MyFarmingGame.UI;

namespace MyFarmingGame.World;

/// <summary>
/// The order board at the seed shed (Task 1.7). Hidden until Marlow's first order is paid
/// (<see cref="OrderManager.BoardOpen"/>); then <c>interact</c> opens the board of picture cards
/// (<see cref="OrderBoardView"/>).
/// </summary>
public partial class OrderBoard : Prop
{
    public OrderBoard()
    {
        SpriteName = "icon_order_board";
        DisplaySize = new Vector2(72, 72);
        PromptSprite = "icon_order_board";
        BodySize = new Vector2(56, 20);
        BodyOffset = new Vector2(0, 26);
        ApproachOffset = new Vector2(0, 64);
    }

    public override bool Usable => OrderManager.Instance?.BoardOpen ?? false;

    public override void _Process(double delta)
    {
        bool open = Usable;
        Visible = open;
        CollisionLayer = open ? 1u : 0u;
        base._Process(delta);
    }

    public override void Interact(Node2D player) => OrderBoardView.Find(GetTree())?.Open();
}
