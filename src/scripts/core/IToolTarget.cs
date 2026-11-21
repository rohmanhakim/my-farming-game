using Godot;
using MyFarmingGame.Autoloads;

namespace MyFarmingGame.Core;

/// <summary>
/// Ground that Kit uses the active tool on (Task 1.5; 3D since Task 1.16): the front-garden
/// <c>FarmGrid</c>. <c>interact</c> (A / Space) with no interactable in reach uses the tool on the
/// tile in front of Kit; a left-click on a tile walks Kit next to it (or works it at once when it is
/// within reach), turns to it and uses the tool there. Points are on the ground (y ignored).
/// Nodes join <see cref="Group"/>.
/// </summary>
public interface IToolTarget
{
    public const string Group = "tool_targets";

    /// <summary>True when <paramref name="worldPoint"/> lies on one of this target's tiles.</summary>
    bool ContainsPoint(Vector3 worldPoint);

    /// <summary>Centre of the tile under <paramref name="worldPoint"/> (on the ground).</summary>
    Vector3 TileCenterAt(Vector3 worldPoint);

    /// <summary>Uses <paramref name="tool"/> on the tile under <paramref name="worldPoint"/>.
    /// Returns true when something happened.</summary>
    bool UseToolAt(Tool tool, Vector3 worldPoint);
}
