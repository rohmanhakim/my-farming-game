using System.Collections.Generic;
using System.Linq;
using Godot;

namespace MyFarmingGame.Autoloads;

/// <summary>Kit's tools, in belt order (Task 1.4). Hands are always there.</summary>
public enum Tool { Hands, Hoe, WateringCan, SeedBag, FeedScoop }

/// <summary>
/// The tool belt (Task 1.4, controls.md §2/§4, mechanics.md "Tool belt"): which tools Kit owns and
/// which one is active. State only; <c>UI/ToolBeltView</c> draws it and reads the cycle inputs
/// (gamepad X / RB = next, LB = previous, mouse wheel, or click a slot).
///
/// Other systems read <see cref="Active"/> (for example Task 1.5 uses it on the tile in front of
/// Kit) and listen to <see cref="ActiveToolChanged"/>. Old tools never disappear; new ones are
/// added with <see cref="Give"/> and slot into their fixed belt position.
/// </summary>
public partial class ToolBelt : Node
{
    public static ToolBelt Instance { get; private set; }

    /// <summary>Fixed belt order (left to right on screen).</summary>
    public static readonly Tool[] BeltOrder = { Tool.Hands, Tool.Hoe, Tool.WateringCan, Tool.SeedBag, Tool.FeedScoop };

    /// <summary>Tools Kit has on a new game. The feed scoop is included because Kit starts with
    /// 5 hens and a cow (Phase-001 v0.12), so "the scoop appears after buying hens" is Day 1.</summary>
    public static readonly Tool[] StartingTools = { Tool.Hands, Tool.Hoe, Tool.WateringCan, Tool.SeedBag, Tool.FeedScoop };

    /// <summary>Emitted when the active tool changes. Argument is the new <see cref="Tool"/> as int.</summary>
    [Signal] public delegate void ActiveToolChangedEventHandler(int tool);

    /// <summary>Emitted when a tool is added to or removed from the belt.</summary>
    [Signal] public delegate void ToolsChangedEventHandler();

    private readonly HashSet<Tool> _owned = new(StartingTools);

    public Tool Active { get; private set; } = Tool.Hands;

    /// <summary>Seed packet the seed bag plants (Task 1.8: chosen in the bag). Empty = the first
    /// seed packet in the bag.</summary>
    public string SelectedSeed { get; set; } = "";

    /// <summary>Emitted when the seed bag's packet changes.</summary>
    [Signal] public delegate void SelectedSeedChangedEventHandler(string item);

    public void SelectSeed(string item)
    {
        if (SelectedSeed == item) return;
        SelectedSeed = item ?? "";
        EmitSignal(SignalName.SelectedSeedChanged, SelectedSeed);
    }

    /// <summary>Owned tools in belt order.</summary>
    public IReadOnlyList<Tool> Tools => BeltOrder.Where(_owned.Contains).ToList();

    public bool Owns(Tool tool) => _owned.Contains(tool);

    /// <summary>Sprite name for a tool's icon (SpriteLibrary), e.g. "tool_watering_can".</summary>
    public static string SpriteName(Tool tool) => tool switch
    {
        Tool.Hands => "tool_hands",
        Tool.Hoe => "tool_hoe",
        Tool.WateringCan => "tool_watering_can",
        Tool.SeedBag => "tool_seed_bag",
        Tool.FeedScoop => "tool_feed_scoop",
        _ => "tool_hands",
    };

    /// <summary>Short name for tooltips and tests (the belt itself is picture-only).</summary>
    public static string DisplayName(Tool tool) => tool switch
    {
        Tool.Hands => "Hands",
        Tool.Hoe => "Hoe",
        Tool.WateringCan => "Watering can",
        Tool.SeedBag => "Seed bag",
        Tool.FeedScoop => "Feed scoop",
        _ => tool.ToString(),
    };

    public override void _EnterTree()
    {
        if (Instance == null && GetParent() == GetTree().Root)
            Instance = this;
        ProcessMode = ProcessModeEnum.Pausable; // project-structure.md §4 process-mode table
    }

    public override void _ExitTree()
    {
        if (Instance == this)
            Instance = null;
    }

    /// <summary>Makes <paramref name="tool"/> active if Kit owns it. Returns false otherwise.</summary>
    public bool Select(Tool tool)
    {
        if (!_owned.Contains(tool))
            return false;
        if (tool != Active)
        {
            Active = tool;
            EmitSignal(SignalName.ActiveToolChanged, (int)tool);
        }
        return true;
    }

    /// <summary>Next owned tool to the right, wrapping to the first.</summary>
    public void Next() => Cycle(+1);

    /// <summary>Previous owned tool to the left, wrapping to the last.</summary>
    public void Prev() => Cycle(-1);

    private void Cycle(int dir)
    {
        var tools = Tools;
        if (tools.Count == 0)
            return;
        int i = 0;
        for (int k = 0; k < tools.Count; k++)
            if (tools[k] == Active) { i = k; break; }
        Select(tools[(i + dir + tools.Count) % tools.Count]);
    }

    /// <summary>Adds a tool to the belt (it does not become active).</summary>
    public void Give(Tool tool)
    {
        if (_owned.Add(tool))
            EmitSignal(SignalName.ToolsChanged);
    }

    /// <summary>Removes a tool from the belt (hands can't be removed). If it was active, hands
    /// become active. Not used by gameplay (old tools never disappear); kept for save/load and tests.</summary>
    public void Take(Tool tool)
    {
        if (tool == Tool.Hands || !_owned.Remove(tool))
            return;
        EmitSignal(SignalName.ToolsChanged);
        if (Active == tool)
            Select(Tool.Hands);
    }

    /// <summary>Save/load (Task 1.10): owned tools, the active one and the seed packet.</summary>
    public void Restore(IEnumerable<Tool> owned, Tool active, string seed)
    {
        _owned.Clear();
        _owned.Add(Tool.Hands);
        foreach (var t in owned) _owned.Add(t);
        SelectedSeed = seed ?? "";
        EmitSignal(SignalName.ToolsChanged);
        if (!Select(active)) Select(Tool.Hands);
    }

    /// <summary>New game: the starting tools, hands active.</summary>
    public void ResetForNewGame()
    {
        _owned.Clear();
        foreach (var t in StartingTools)
            _owned.Add(t);
        SelectedSeed = "";
        EmitSignal(SignalName.ToolsChanged);
        Select(Tool.Hands);
    }
}
