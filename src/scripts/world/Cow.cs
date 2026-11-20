using Godot;

namespace MyFarmingGame.World;

/// <summary>The cow in the old barn (Task 1.9): 4 milk cans each morning after a fed day.</summary>
public partial class Cow : FarmAnimal
{
    public Cow()
    {
        SpriteName = "cow";
        DisplaySize = new Vector2(96, 96);
        ClickSize = new Vector2(96, 80);
        ApproachOffset = new Vector2(0, 64);
        Reach = 60f;
    }
}
