using Godot;

namespace MyFarmingGame.World;

/// <summary>A hen in the old coop (Task 1.9): 1 egg each morning after a fed day.</summary>
public partial class Hen : FarmAnimal
{
    public Hen()
    {
        SpriteName = "hen";
        DisplaySize = new Vector2(40, 40);
        ClickSize = new Vector2(40, 40);
    }
}
