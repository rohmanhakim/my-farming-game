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
        ModelName = "order_board";
        SpriteName = "icon_order_board";
        PromptSprite = "icon_order_board";
        BodySize = new Vector3(1.2f, 1.6f, 0.3f);
        ClickSize = new Vector3(1.3f, 1.8f, 0.6f);
        ApproachOffset = new Vector3(0, 0, 1.0f);
        PromptHeight = 2.1f;
    }

    public override bool Usable => OrderManager.Instance?.BoardOpen ?? false;

    public override void _Process(double delta)
    {
        bool open = Usable;
        Visible = open;
        CollisionLayer = open ? 1u : 0u;
        base._Process(delta);
    }

    public override void Interact(Node3D player) => OrderBoardView.Find(GetTree())?.Open();
}
