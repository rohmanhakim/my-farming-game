using Godot;
using MyFarmingGame.Autoloads;
using MyFarmingGame.Core;
using MyFarmingGame.UI;

namespace MyFarmingGame.World;

/// <summary>
/// Kit's bed (Task 1.3). Walk up and press <c>interact</c> (A / Space), or click it, to get the
/// "sleep now?" prompt. Kit also wakes up here every morning (<see cref="WakePoint"/>), including
/// after the automatic 21:30 bedtime. Placeholder sprite "bed" (swap art by name).
/// </summary>
public partial class Bed : StaticBody2D, IInteractable
{
    /// <summary>Group of beds; Main's SleepSequence wakes Kit at the first one.</summary>
    public const string Group = "beds";

    private Area2D _reach;
    private Marker2D _wake;
    private Node2D _prompt;
    private Node2D _playerInReach;

    public Vector2 ApproachPoint => _wake?.GlobalPosition ?? GlobalPosition;

    /// <summary>Where Kit stands after waking up.</summary>
    public Vector2 WakePoint => ApproachPoint;

    /// <summary>True while the button prompt above the bed is visible (tests).</summary>
    public bool PromptVisible => _prompt?.Visible ?? false;

    public bool CanInteract => GameClock.Instance == null || GameClock.Instance.IsPlayable;

    public override void _Ready()
    {
        AddToGroup(IInteractable.Group);
        AddToGroup(Group);
        _reach = GetNode<Area2D>("ReachArea");
        _wake = GetNode<Marker2D>("WakeSpot");
        _prompt = GetNode<Node2D>("Prompt");
        GetNode<TextureRect>("%PromptIcon").Texture = SpriteLibrary.Get("icon_sleep");
        _reach.BodyEntered += b => { if (b is CharacterBody2D p && p.IsInGroup("player")) _playerInReach = p; };
        _reach.BodyExited += b => { if (b == _playerInReach) _playerInReach = null; };
        _prompt.Visible = false;
    }

    public bool IsInReach(Node2D player) => player != null && _reach.OverlapsBody(player);

    public void Interact(Node2D player)
    {
        if (CanInteract)
            GameClock.Instance?.AskToSleep();
    }

    public override void _Process(double delta)
    {
        _prompt.Visible = _playerInReach != null && CanInteract;
    }
}
