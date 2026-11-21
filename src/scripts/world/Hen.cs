using Godot;

namespace MyFarmingGame.World;

/// <summary>A hen in the old coop (Task 1.9): 1 egg each morning after a fed day.</summary>
public partial class Hen : FarmAnimal
{
    public Hen()
    {
        ModelName = "hen";
        SpriteName = "hen";
        ClickSize = new Vector3(0.6f, 0.7f, 0.6f);
        PromptHeight = 0.9f;
        CastsShadow = false; // five tiny shadows cost five shadow-pass draws (budget §16)
    }
}
