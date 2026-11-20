using System.Collections.Generic;
using Godot;

namespace MyFarmingGame.Core;

/// <summary>
/// Looks up game sprites by logical name (for example "player", "coop", "tool_hoe")
/// so final art can replace placeholders without code or scene changes.
///
/// Lookup order for a name:
///   1. Final art:   res://assets/art/{subfolder}/{name}.png  (see <see cref="ArtFolders"/>)
///   2. Placeholder: res://assets/placeholder/kenney/ph_{name}.png
///   3. Fallback:    a flat coloured square (so a missing sprite is obvious but never crashes)
/// </summary>
public static class SpriteLibrary
{
    public const string ArtRoot = "res://assets/art";
    public const string PlaceholderRoot = "res://assets/placeholder/kenney";

    /// <summary>Folders under assets/art/ searched for final art (project-structure.md §4).</summary>
    public static readonly string[] ArtFolders = { "characters", "world", "ui", "items", "vfx" };

    /// <summary>Every sprite name Phase-001 expects; used by the tests.</summary>
    public static readonly string[] KnownNames =
    {
        "player", "npc_marlow", "grass", "garden_bed", "crop_sprout", "hen", "coop",
        "tool_hands", "tool_hoe", "tool_watering_can", "tool_seed_bag", "tool_feed_scoop", "coin",
    };

    /// <summary>Colour used when neither final art nor a placeholder image exists.</summary>
    private static readonly Dictionary<string, Color> FallbackColors = new()
    {
        { "hen", new Color(0.97f, 0.95f, 0.88f) },
        { "tool_hands", new Color(0.96f, 0.78f, 0.62f) },
    };

    private static readonly Dictionary<string, Texture2D> Cache = new();

    public enum Source { FinalArt, Placeholder, Fallback }

    public static Texture2D Get(string name) => Get(name, out _);

    public static Texture2D Get(string name, out Source source)
    {
        string path = FindPath(name, out source);
        string key = path ?? "fallback:" + name;
        if (Cache.TryGetValue(key, out var cached))
            return cached;

        Texture2D tex = path != null ? ResourceLoader.Load<Texture2D>(path) : null;
        if (tex == null)
        {
            source = Source.Fallback;
            tex = MakeFallback(name);
        }
        Cache[key] = tex;
        return tex;
    }

    /// <summary>Returns the res:// path that would be used, or null when the fallback square is used.</summary>
    public static string FindPath(string name, out Source source)
    {
        foreach (var folder in ArtFolders)
        {
            string p = $"{ArtRoot}/{folder}/{name}.png";
            if (ResourceLoader.Exists(p)) { source = Source.FinalArt; return p; }
        }
        string ph = $"{PlaceholderRoot}/ph_{name}.png";
        if (ResourceLoader.Exists(ph)) { source = Source.Placeholder; return ph; }
        source = Source.Fallback;
        return null;
    }

    private static Texture2D MakeFallback(string name)
    {
        var color = FallbackColors.TryGetValue(name, out var c) ? c : new Color(1f, 0f, 1f);
        var img = Image.CreateEmpty(16, 16, false, Image.Format.Rgba8);
        img.Fill(color);
        return ImageTexture.CreateFromImage(img);
    }
}
