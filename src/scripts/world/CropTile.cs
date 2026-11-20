using Godot;

namespace MyFarmingGame.World;

/// <summary>What the ground of a garden tile looks like (Task 1.5).</summary>
public enum Soil { Weeds, Clear, Tilled }

/// <summary>
/// One front-garden tile (Task 1.5): soil state, today's watering and the crop growing on it.
/// Plain data, owned by <see cref="FarmGrid"/>; saved by Task 1.10 through
/// <see cref="ToDict"/> / <see cref="FromDict"/>.
///
/// Rules (crops.md §2): a crop grows one day each night it was watered that day; a dry day adds
/// one to <see cref="DryDays"/>; 2 dry days in a row → yellow; 3 → the crop is gone and the tile
/// stays tilled. Watering resets the dry count at once (a yellow crop recovers).
/// </summary>
public sealed class CropTile
{
    public Soil Soil { get; set; } = Soil.Weeds;

    /// <summary>Watered today (dark soil). Cleared every night.</summary>
    public bool Watered { get; set; }

    /// <summary>Crop id (crops.tres), or null for an empty tile.</summary>
    public string CropId { get; set; }

    /// <summary>Watered nights since planting.</summary>
    public int DaysGrown { get; set; }

    /// <summary>Days to harvest of the planted crop (copied at planting).</summary>
    public int DaysToHarvest { get; set; }

    /// <summary>Dry days in a row.</summary>
    public int DryDays { get; set; }

    public const int YellowAfterDryDays = 2;
    public const int LostAfterDryDays = 3;

    public bool HasCrop => !string.IsNullOrEmpty(CropId);
    public bool IsReady => HasCrop && DaysGrown >= DaysToHarvest;
    public bool IsYellow => HasCrop && !IsReady && DryDays >= YellowAfterDryDays;

    /// <summary>0 = seed, 1 = sprout, 2 = growing, 3 = ready (picture stage).</summary>
    public int Stage
    {
        get
        {
            if (!HasCrop) return -1;
            if (IsReady) return 3;
            if (DaysGrown == 0) return 0;
            return DaysGrown * 2 < DaysToHarvest ? 1 : 2;
        }
    }

    public void ClearCrop()
    {
        CropId = null;
        DaysGrown = 0;
        DaysToHarvest = 0;
        DryDays = 0;
    }

    public Godot.Collections.Dictionary ToDict() => new()
    {
        { "soil", Soil.ToString() },
        { "watered", Watered },
        { "crop", CropId ?? "" },
        { "daysGrown", DaysGrown },
        { "daysToHarvest", DaysToHarvest },
        { "dryDays", DryDays },
    };

    public static CropTile FromDict(Godot.Collections.Dictionary d)
    {
        var t = new CropTile();
        if (d.TryGetValue("soil", out var s) && System.Enum.TryParse<Soil>(s.AsString(), out var soil))
            t.Soil = soil;
        t.Watered = d.TryGetValue("watered", out var w) && w.AsBool();
        string crop = d.TryGetValue("crop", out var c) ? c.AsString() : "";
        t.CropId = string.IsNullOrEmpty(crop) ? null : crop;
        t.DaysGrown = d.TryGetValue("daysGrown", out var g) ? g.AsInt32() : 0;
        t.DaysToHarvest = d.TryGetValue("daysToHarvest", out var h) ? h.AsInt32() : 0;
        t.DryDays = d.TryGetValue("dryDays", out var dd) ? dd.AsInt32() : 0;
        return t;
    }
}
