using Godot;

namespace MyFarmingGame.World;

/// <summary>The cow in the old barn (Task 1.9): 4 milk cans each morning after a fed day.</summary>
public partial class Cow : FarmAnimal
{
    public Cow()
    {
        ModelName = "cow";
        SpriteName = "cow";
        ClickSize = new Vector3(1.0f, 1.4f, 1.6f);
        ApproachOffset = new Vector3(0, 0, -1.0f); // from the stall side, clear of its trough slot
        Reach = 0.95f;
        PromptHeight = 1.8f;
    }
}
