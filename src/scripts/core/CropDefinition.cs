using Godot;

namespace MyFarmingGame.Core;

/// <summary>One crop's growth data (crops.md §3). Loaded from <c>res://data/crops.tres</c>
/// through <see cref="CropCatalog"/>.</summary>
[GlobalClass]
public partial class CropDefinition : Resource
{
    /// <summary>Crop id, e.g. "carrot".</summary>
    [Export] public string Id { get; set; } = "";

    /// <summary>Inventory item planted with the seed bag, e.g. "carrot_seeds".</summary>
    [Export] public string SeedItem { get; set; } = "";

    /// <summary>Inventory item a harvest gives, e.g. "carrot".</summary>
    [Export] public string HarvestItem { get; set; } = "";

    /// <summary>Watered nights from planting to harvest (crops.md: carrot 3).</summary>
    [Export] public int DaysToHarvest { get; set; } = 3;

    /// <summary>How many items one harvest gives.</summary>
    [Export] public int HarvestCount { get; set; } = 1;

    /// <summary>Sprite name prefix: "&lt;prefix&gt;_seed/_sprout/_growing/_ready/_yellow".</summary>
    [Export] public string SpritePrefix { get; set; } = "";
}
