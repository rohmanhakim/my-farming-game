using Godot;

namespace MyFarmingGame.Core;

/// <summary>
/// Every crop Kit can plant (Task 1.5, <c>res://data/crops.tres</c>; crops.md is the source).
/// Use <see cref="Default"/> to read it.
/// </summary>
[GlobalClass]
public partial class CropCatalog : Resource
{
    public const string Path = "res://data/crops.tres";

    /// <summary>The crops (each a <see cref="CropDefinition"/>).</summary>
    [Export] public Godot.Collections.Array<Resource> Crops { get; set; } = new();

    private static CropCatalog _default;

    /// <summary>The catalog loaded from <see cref="Path"/> (cached).</summary>
    public static CropCatalog Default => _default ??= ResourceLoader.Load<CropCatalog>(Path) ?? new CropCatalog();

    public CropDefinition Get(string id)
    {
        foreach (var r in Crops)
            if (r is CropDefinition c && c.Id == id)
                return c;
        return null;
    }

    /// <summary>The crop that grows from <paramref name="seedItem"/>, if any.</summary>
    public CropDefinition BySeed(string seedItem)
    {
        foreach (var r in Crops)
            if (r is CropDefinition c && c.SeedItem == seedItem)
                return c;
        return null;
    }
}
