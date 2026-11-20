using System.Collections.Generic;
using System.Linq;
using Godot;

namespace MyFarmingGame.Autoloads;

/// <summary>The guided start's ten steps (onboarding.md §2).</summary>
public enum OnboardingStep
{
    Arrival = 1, Garden, Plant, Water, MeetMarlow, Forage, FirstOrder, SecondOrder, FeedAnimals, FreePlay, Done,
}

/// <summary>
/// Onboarding progress (Task 1.12, autoload <c>Onboarding</c>): the current guided-start step, the
/// counters it needs (weeds pulled, seeds planted, beds watered) and which one-time nudges were
/// already shown. State only — Main's <c>OnboardingDirector</c> watches the game, advances the steps
/// and shows the nudges. Saved in its own <c>onboarding</c> section. Never blocks play: steps are
/// hints, and doing things early just skips them.
/// </summary>
public partial class Onboarding : Node
{
    public static Onboarding Instance { get; private set; }

    public const string Section = "onboarding";

    // One-time nudges (onboarding.md §3) that apply to Phase-001.
    public const string FirstEvening = "first_evening";
    public const string WaterMorning = "water_morning";
    public const string YellowCrop = "yellow_crop";
    public const string LostCrop = "lost_crop";
    public const string CarrotsReady = "carrots_ready";
    public const string FeedLow = "feed_low";
    public const string FirstMorning = "first_morning";
    public const string FirstProducts = "first_products";
    public static readonly string[] AllNudges = { FirstEvening, WaterMorning, YellowCrop, LostCrop, CarrotsReady, FeedLow, FirstMorning, FirstProducts };

    [Signal] public delegate void StepChangedEventHandler(int step);
    [Signal] public delegate void NudgeShownEventHandler(string id);

    public OnboardingStep Step { get; private set; } = OnboardingStep.Arrival;
    public int WeedsPulled { get; set; }
    public int Planted { get; set; }
    public int Watered { get; set; }
    private readonly HashSet<string> _shown = new();
    public IReadOnlyCollection<string> Shown => _shown;

    public bool IsDone => Step == OnboardingStep.Done;

    public override void _EnterTree()
    {
        if (Instance == null && GetParent() == GetTree().Root)
            Instance = this;
        ProcessMode = ProcessModeEnum.Pausable; // project-structure.md §4
    }

    public override void _Ready()
    {
        if (Instance == this)
            SaveGame.Instance?.Register(Section, ToDict, FromDict);
    }

    public override void _ExitTree()
    {
        if (Instance == this)
        {
            SaveGame.Instance?.Unregister(Section);
            Instance = null;
        }
    }

    public void Advance()
    {
        if (IsDone) return;
        Step++;
        EmitSignal(SignalName.StepChanged, (int)Step);
    }

    public bool WasShown(string nudge) => _shown.Contains(nudge);

    /// <summary>Marks a nudge shown; returns false if it already was (each shows once).</summary>
    public bool MarkShown(string nudge)
    {
        if (!_shown.Add(nudge)) return false;
        EmitSignal(SignalName.NudgeShown, nudge);
        return true;
    }

    public void ResetForNewGame()
    {
        Step = OnboardingStep.Arrival;
        WeedsPulled = Planted = Watered = 0;
        _shown.Clear();
        EmitSignal(SignalName.StepChanged, (int)Step);
    }

    public Godot.Collections.Dictionary ToDict()
    {
        var shown = new Godot.Collections.Array();
        foreach (var s in _shown.OrderBy(s => s)) shown.Add(s);
        return new Godot.Collections.Dictionary
        {
            { "step", (int)Step }, { "weeds", WeedsPulled }, { "planted", Planted }, { "watered", Watered }, { "shown", shown },
        };
    }

    public void FromDict(Godot.Collections.Dictionary d)
    {
        Step = (OnboardingStep)Mathf.Clamp(d.TryGetValue("step", out var s) ? s.AsInt32() : 1, 1, (int)OnboardingStep.Done);
        WeedsPulled = d.TryGetValue("weeds", out var w) ? w.AsInt32() : 0;
        Planted = d.TryGetValue("planted", out var p) ? p.AsInt32() : 0;
        Watered = d.TryGetValue("watered", out var wa) ? wa.AsInt32() : 0;
        _shown.Clear();
        if (d.TryGetValue("shown", out var sh))
            foreach (var v in sh.AsGodotArray()) _shown.Add(v.AsString());
        EmitSignal(SignalName.StepChanged, (int)Step);
    }
}
