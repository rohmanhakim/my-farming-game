using Godot;

namespace MyFarmingGame.Core;

/// <summary>
/// Something Kit can use with the <c>interact</c> action (A / Space) when close, or by
/// left-clicking it (Kit walks to <see cref="ApproachPoint"/>, then uses it). First user: the bed
/// (Task 1.3). Nodes add themselves to <see cref="Group"/>; an Area2D on
/// <see cref="ClickLayer"/> marks the clickable shape (its owner or an ancestor implements this).
/// </summary>
public interface IInteractable
{
    /// <summary>Scene-tree group every interactable joins.</summary>
    public const string Group = "interactables";

    /// <summary>Physics layer (bit value) of the click areas. Layer 8 in the editor.</summary>
    public const uint ClickLayer = 1u << 7;

    /// <summary>Where Kit stands to use it (click-to-use walks here).</summary>
    Vector2 ApproachPoint { get; }

    /// <summary>True when <paramref name="player"/> is close enough to use it with A / Space.</summary>
    bool IsInReach(Node2D player);

    /// <summary>False while it cannot be used (for example the clock is stopped for a menu).</summary>
    bool CanInteract { get; }

    void Interact(Node2D player);
}
