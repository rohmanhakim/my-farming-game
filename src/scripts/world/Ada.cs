using Godot;
using MyFarmingGame.Autoloads;
using MyFarmingGame.UI;

namespace MyFarmingGame.World;

/// <summary>
/// Ada in solo play (Task 1.12, characters.md §7): the parents' old friend, standing by the house.
/// She waves while Kit arrives, says one short line about the current guided-start step when Kit
/// talks to her (what and where, never how — and she never does Kit's tasks), and speaks the
/// nudges and the hungry-animal warning. Co-play Ada is Phase-003.
/// </summary>
public partial class Ada : Prop
{
    public const string Portrait = "npc_ada";
    private bool _waving;
    private float _t;

    public Ada()
    {
        SpriteName = "npc_ada";
        PromptSprite = "icon_talk";
        BodySize = new Vector2(36, 20);
        BodyOffset = new Vector2(0, 20);
        ApproachOffset = new Vector2(56, 20);
        Reach = 72f;
    }

    public bool Waving => _waving;

    public void SetWaving(bool on)
    {
        _waving = on;
        if (!on && Sprite != null) Sprite.Rotation = 0;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (!_waving || Sprite == null) return;
        _t += (float)delta;
        Sprite.Rotation = Mathf.Sin(_t * 6f) * 0.15f;
    }

    public override void Interact(Node2D player)
    {
        var step = Onboarding.Instance?.Step ?? OnboardingStep.Done;
        PictureBubbleView.Find(GetTree())?.Show(Portrait, LineFor(step), null, new[] { BubbleButton.Close() });
    }

    /// <summary>Ada's line for a step: the goal and where, never the method.</summary>
    public static string LineFor(OnboardingStep step) => step switch
    {
        OnboardingStep.Arrival => "Welcome home, Kit! The house is just here.",
        OnboardingStep.Garden => "The garden's a bit wild. See the weeds by the beds?",
        OnboardingStep.Plant => "Those three beds are ready for seeds.",
        OnboardingStep.Water => "New seeds get thirsty.",
        OnboardingStep.MeetMarlow => "Mr. Marlow's seed shed is up the lane. He'd love to meet you.",
        OnboardingStep.Forage => "The bushes along the lane are full of berries.",
        OnboardingStep.FirstOrder => "Mr. Marlow is waiting for his berries.",
        OnboardingStep.SecondOrder => "Mr. Marlow pinned something on his board.",
        OnboardingStep.FeedAnimals => "The hens and the cow need food every day. Their feed is in the bins.",
        OnboardingStep.FreePlay => "The feed lasts about a week. Orders bring coins for more.",
        _ => "Lovely day, Kit.",
    };
}
