using System.Collections.Generic;
using System.Linq;

namespace MyFarmingGame.Core;

/// <summary>One neighbour who posts orders on Marlow's board.</summary>
public sealed class Neighbour
{
    public string Id { get; init; } = "";
    public string Name { get; init; } = "";
    public string Portrait { get; init; } = "";
    /// <summary>Items they order (empty = anything Kit makes).</summary>
    public string[] Likes { get; init; } = System.Array.Empty<string>();
    /// <summary>One line when they first pin an order.</summary>
    public string Hello { get; init; } = "";

    public bool LikesItem(string item) => Likes.Length == 0 || Likes.Contains(item);
}

/// <summary>
/// Who orders from Kit (owner request 2026-11-21, characters.md §10 v0.x): Mr. Marlow from Day 1;
/// then, one at a time and each after a visible action of Kit's, the <b>baker</b> (after the
/// first eggs are collected), the <b>allotment neighbour</b> (after the first crop order is
/// delivered) and the <b>shopkeeper</b> (after 5 delivered orders). In Phase-001 they only pin
/// orders on Marlow's board (their buildings open with the village in Phase-002).
/// </summary>
public static class Neighbours
{
    public const string Marlow = "marlow";
    public const string Baker = "baker";
    public const string Allotment = "allotment";
    public const string Shopkeeper = "shopkeeper";

    /// <summary>Delivered orders before the shopkeeper writes.</summary>
    public const int ShopkeeperAfterDeliveries = 5;

    public static readonly Neighbour[] All =
    {
        new() { Id = Marlow, Name = "Mr. Marlow", Portrait = "npc_marlow" },
        new() { Id = Baker, Name = "The baker", Portrait = "npc_baker",
            Likes = new[] { "egg", "milk", "berries" }, Hello = "Hello, Kit! I bake in the village. Eggs and milk for my bread? I've pinned an order." },
        new() { Id = Allotment, Name = "The allotment neighbour", Portrait = "npc_allotment",
            Likes = new[] { "carrot", "sun_sprout", "berries" }, Hello = "Hi there! I keep the allotments. Your carrots looked lovely. I've pinned an order too." },
        new() { Id = Shopkeeper, Name = "The shopkeeper", Portrait = "npc_shopkeeper",
            Likes = new[] { "egg", "milk", "sun_sprout", "carrot" }, Hello = "Good day! I run the village shop. My customers want farm-fresh things. See the board!" },
    };

    public static Neighbour Get(string id) => All.FirstOrDefault(n => n.Id == id) ?? All[0];

    public static IEnumerable<Neighbour> Of(IEnumerable<string> ids) => All.Where(n => ids.Contains(n.Id));
}
