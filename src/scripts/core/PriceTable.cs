using System.Collections.Generic;
using Godot;

namespace MyFarmingGame.Core;

/// <summary>
/// Prices in coins by item id (economy.md; source docs/data/economy.csv). Read from
/// <c>res://data/prices.json</c>, which <c>tools/make_prices.py</c> generates from the CSV
/// (Task 1.6). Buy prices are ≈ half real price; sell prices ≈ real retail.
/// </summary>
public static class PriceTable
{
    public const string Path = "res://data/prices.json";

    /// <summary>Neighbour orders pay this × the sell price. Owner decision 2026-11-21: no premium on
    /// normal orders (was a ×1.25 proposal); only mixed baskets and quick-bonus orders pay extra
    /// (<see cref="Order.BasketBonus"/>, <see cref="Order.QuickBonus"/>).</summary>
    public const float OrderPremium = 1.0f;

    private static Dictionary<string, int> _coins;

    private static Dictionary<string, int> Coins
    {
        get
        {
            if (_coins != null) return _coins;
            _coins = new Dictionary<string, int>();
            var text = FileAccess.GetFileAsString(Path);
            var parsed = Json.ParseString(text);
            if (parsed.VariantType == Variant.Type.Dictionary &&
                parsed.AsGodotDictionary().TryGetValue("items", out var items))
            {
                foreach (var (k, v) in items.AsGodotDictionary())
                    _coins[k.AsString()] = v.AsGodotDictionary()["coins"].AsInt32();
            }
            else
                GD.PushError($"[PriceTable] could not read {Path}");
            return _coins;
        }
    }

    /// <summary>Price in coins of one unit of <paramref name="itemId"/> (0 when unknown).</summary>
    public static int Get(string itemId) => Coins.TryGetValue(itemId, out var c) ? c : 0;

    public static bool Has(string itemId) => Coins.ContainsKey(itemId);

    /// <summary>What a neighbour pays for <paramref name="count"/> × <paramref name="itemId"/>.</summary>
    public static int OrderReward(string itemId, int count) => Mathf.RoundToInt(Get(itemId) * count * OrderPremium);
}
