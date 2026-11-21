using System.Collections.Generic;
using System.Linq;
using Godot;
using MyFarmingGame.UI;

namespace MyFarmingGame.World;

/// <summary>One ledger page picture: a promise (open) or something done.</summary>
public sealed class LedgerPage
{
    public string Key { get; init; } = "";
    public string Sprite { get; init; } = "";
    public bool Done { get; set; }
}

/// <summary>
/// Marlow's ledger on the seed-shed table (Task 1.6, world.md §4.2): picture pages for what Kit
/// promised and what has been done. No text, no numbers. Interact → a close-up of the pages in the
/// picture bubble. Other code records pages with <see cref="Note"/>; state is static so the
/// ledger keeps its pages whatever level is loaded (saved by Task 1.10).
/// </summary>
public partial class Ledger : Prop
{
    private static readonly List<LedgerPage> _pages = new();

    public static IReadOnlyList<LedgerPage> Pages => _pages;

    /// <summary>Adds or updates a page: <paramref name="done"/> false = promised, true = done.</summary>
    public static void Note(string key, string sprite, bool done)
    {
        var page = _pages.FirstOrDefault(p => p.Key == key);
        if (page == null)
            _pages.Add(new LedgerPage { Key = key, Sprite = sprite, Done = done });
        else
            page.Done = done;
    }

    public static void Reset() => _pages.Clear();

    public static void Restore(IEnumerable<LedgerPage> pages)
    {
        _pages.Clear();
        _pages.AddRange(pages);
    }

    public Ledger()
    {
        ModelName = "ledger";
        SpriteName = "icon_ledger";
        PromptSprite = "icon_ledger";
        BodySize = new Vector3(0.9f, 0.8f, 0.5f);
        ClickSize = new Vector3(1f, 1f, 0.7f);
        ApproachOffset = new Vector3(0, 0, 0.8f);
        PromptHeight = 1.2f;
    }

    public override void Interact(Node3D player)
    {
        var bubble = PictureBubbleView.Find(GetTree());
        if (bubble == null) return;
        var pics = new List<BubbleChip>();
        foreach (var p in _pages)
            pics.Add(new BubbleChip(p.Sprite, p.Done ? "✓" : "…", !p.Done));
        if (pics.Count == 0)
            pics.Add(new BubbleChip("icon_ledger", "", true));
        bubble.Show("icon_ledger", "", pics, new[] { BubbleButton.Close("icon_check") });
    }
}
