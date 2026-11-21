using System.Linq;
using System.Threading.Tasks;
using Godot;
using MyFarmingGame.Autoloads;
using MyFarmingGame.Core;
using MyFarmingGame.Player;
using MyFarmingGame.UI;
using MyFarmingGame.World;

namespace MyFarmingGame.Tests;

/// <summary>Task 1.10: versioned save/load, safe write + backup, migrations, auto-save.</summary>
public partial class TestRunner
{
    public const string TestSavePath = "user://test_savegame.json";

    private static string Snap() => Json.Stringify(SaveGame.Instance.Snapshot());

    private async Task<Node> NewMain()
    {
        var main = ResourceLoader.Load<PackedScene>("res://scenes/main/Main.tscn").Instantiate();
        AddChild(main);
        await Frames(3);
        return main;
    }

    private async Task CheckSaveLoad()
    {
        var save = SaveGame.Instance;
        Check(save != null && save.GetParent() == GetTree().Root && save.ProcessMode == ProcessModeEnum.Always, "autoload SaveGame registered and Always");
        if (save == null) return;
        Check(SaveGame.SchemaVersion == 2 && SaveGame.DefaultPath == "user://savegame.json", "save: user://savegame.json, schemaVersion 2 (3D, Task 1.20)");
        ResetTask14State();
        save.SavePath = TestSavePath;
        save.DeleteSave();
        save.Enabled = true;
        var clock = GameClock.Instance;

        var main = await NewMain();
        Check(SaveBinder.SectionNames.All(n => save.Sections.Contains(n)), "Main registers clock, inventory, wallet, orders, feed, animals, farm, forage, ledger, player");
        Check(!save.Load() && save.LastLoadedFrom == "", "no save yet → new game");
        clock.Hold("test"); // keep time still while comparing snapshots

        // Change everything.
        var level = main.GetNode("World/Level").GetChild(0);
        var player = main.GetNode<PlayerController>("World/Entities/Player");
        var grid = level.GetNode<FarmGrid>("FrontGarden");
        clock.SetTime(4, 300);
        grid.UseTool(Tool.Hands, new Vector2I(0, 0));
        grid.UseTool(Tool.Hoe, new Vector2I(0, 0));
        grid.UseTool(Tool.SeedBag, new Vector2I(0, 0));
        grid.UseTool(Tool.WateringCan, new Vector2I(0, 0));
        grid.UseTool(Tool.SeedBag, new Vector2I(5, 3));
        grid.EndDay(); // (0,0) grows a day; (5,3) one dry day
        Inventory.Instance.Add(ItemIds.Egg, 7);
        Inventory.Instance.Add(ItemIds.Carrot, 2, 2);
        ToolBelt.Instance.Select(Tool.WateringCan);
        ToolBelt.Instance.SelectSeed(ItemIds.CarrotSeeds);
        Wallet.Instance.SetCoins(1234);
        var om = OrderManager.Instance;
        om.Accept(OrderManager.FirstOrderId);
        Inventory.Instance.Add(ItemIds.Berries, 3);
        om.Deliver(OrderManager.FirstOrderId);
        om.Accept(OrderManager.SecondOrderId);
        var hens = AnimalHouse.For(GetTree(), "hen");
        hens.FeedOne(hens.Animals[0]);
        hens.EndDay();
        hens.TakeFeed(3);
        FeedStore.Order(ItemIds.Hay, 7, 4, 600);
        level.GetNode<BerryBush>("Bushes/BerryBush2").Interact(null);
        Ledger.Note("test_page", "item_egg", true);
        player.GlobalPosition = M(1700, 300);
        await Frames(2);
        string before = Snap();
        int coins = Wallet.Instance.Coins; // 1234 + 45 for the berries
        Check(before.Contains("\"schemaVersion\"") && coins == 1279 && before.Contains("1279"), "snapshot is JSON with schemaVersion and the state");
        Check(save.Save() && FileAccess.FileExists(TestSavePath) && !FileAccess.FileExists(save.TempPath), "save writes the file (temp file renamed away)");
        var onDisk = SaveGame.Read(TestSavePath);
        Check(onDisk != null && onDisk["schemaVersion"].AsInt32() == 2 && onDisk.ContainsKey("farm") && onDisk.ContainsKey("animals"),
            "the file on disk is readable JSON with schemaVersion 2 and every section");
        var pl = onDisk["player"].AsGodotDictionary();
        Check(pl.ContainsKey("z") && !pl.ContainsKey("y") && Mathf.IsEqualApprox((float)pl["x"].AsDouble(), 1700f / 64f),
            $"the player is saved in metres on the ground (x, z) ({pl["x"]}, {pl["z"]})");

        // Restart: a fresh game, then load.
        main.QueueFree();
        await Frames(2);
        ResetTask14State();
        clock.Hold("test");
        main = await NewMain(); // Main loads the save at start
        var binder = main.GetNode<SaveBinder>("Systems/SaveBinder");
        await Frames(2);
        string after = Snap();
        Check(binder.LoadedAtStart && save.LastLoadedFrom == TestSavePath, "restarting loads the save automatically");
        Check(after == before, "round-trip: everything restored exactly (position, day, tiles, crops, dry counts, bag, belt, coins, orders, animals, hunger, feed bins, deliveries, bushes, ledger)");
        level = main.GetNode("World/Level").GetChild(0);
        grid = level.GetNode<FarmGrid>("FrontGarden");
        hens = AnimalHouse.For(GetTree(), "hen");
        Check(clock.Day == 4 && grid.Get(new Vector2I(0, 0)).DaysGrown == 1 && grid.Get(new Vector2I(5, 3)).DryDays == 1
              && Wallet.Instance.Coins == 1279 && Inventory.Instance.Count(ItemIds.Carrot, 2) == 2 && ToolBelt.Instance.Active == Tool.WateringCan,
            "spot checks: Day 4, crop growth and dry count, coins, ★★ carrots, active tool");
        Check(om.BoardOpen && om.Get(OrderManager.SecondOrderId).State == OrderState.Accepted && hens.Animals.Count(a => a.UnfedDays == 1) == 4
              && hens.Animals[0].ProductReady == 1 && FeedStore.PendingAmount(ItemIds.Hay) == 7 && !level.GetNode<BerryBush>("Bushes/BerryBush2").HasBerries
              && Ledger.Pages.Any(p => p.Key == "test_page"),
            "spot checks: board open, accepted order, hunger counts, egg waiting, hay on its way, picked bush, ledger page");

        // 50 save/load cycles without drift.
        bool stable = true;
        for (int i = 0; i < 50 && stable; i++)
        {
            save.Save();
            save.Load();
            stable = Snap() == before;
        }
        Check(stable, "50× save/load: no drift");

        // Corrupt main file → the backup loads.
        using (var f = FileAccess.Open(TestSavePath, FileAccess.ModeFlags.Write)) f.StoreString("{ this is not json");
        Wallet.Instance.SetCoins(1);
        Check(save.Load() && save.LastLoadedFrom == save.BackupPath && Wallet.Instance.Coins == 1279, "corrupt main file → the backup loads");
        save.Save();
        Check(SaveGame.Read(TestSavePath) != null, "saving again repairs the main file");

        // Migration chain.
        int ran = 0;
        SaveGame.Migrations[0] = d => { ran++; return d; };
        var old = new Godot.Collections.Dictionary { { "schemaVersion", 0 } };
        var up = save.Migrate(old);
        Check(ran == 1 && save.MigrationsRun == 2 && up["schemaVersion"].AsInt32() == SaveGame.SchemaVersion, "migration hooks run in order (v0 → v1 → v2)");
        SaveGame.Migrations.Remove(0);
        save.Migrate(new Godot.Collections.Dictionary { { "schemaVersion", 2 } });
        Check(save.MigrationsRun == 0, "a current (v2) save needs no migration");
        var v1 = new Godot.Collections.Dictionary { { "schemaVersion", 1 },
            { "player", new Godot.Collections.Dictionary { { "x", 1600.0 }, { "y", 200.0 }, { "facing", "Up" } } } };
        var v2 = save.Migrate(v1)["player"].AsGodotDictionary();
        Check(save.MigrationsRun == 1 && Mathf.IsEqualApprox((float)v2["x"].AsDouble(), 25f) && Mathf.IsEqualApprox((float)v2["z"].AsDouble(), 3.125f)
              && !v2.ContainsKey("y") && v2["facing"].AsString() == "Up", "v1 → v2: the player's pixels (1600, 200) become metres (25, 3.125)");

        // A save written by the 2D build (v1) loads into the 3D game with Kit on the same spot.
        var asV1 = SaveGame.Read(TestSavePath);
        asV1["schemaVersion"] = 1;
        var p1 = asV1["player"].AsGodotDictionary();
        p1["y"] = p1["z"].AsDouble() * 64.0;
        p1["x"] = p1["x"].AsDouble() * 64.0;
        p1.Remove("z");
        using (var f1 = FileAccess.Open(TestSavePath, FileAccess.ModeFlags.Write)) f1.StoreString(Json.Stringify(asV1, "  "));
        var kitNow = main.GetNode<PlayerController>("World/Entities/Player"); // the restarted Main's Kit
        kitNow.GlobalPosition = Vector3.Zero;
        Wallet.Instance.SetCoins(1);
        Check(save.Load() && save.MigrationsRun == 1 && kitNow.GlobalPosition.DistanceTo(M(1700, 300)) < 0.01f && Wallet.Instance.Coins == 1279,
            $"an old 2D (v1) save loads with Kit at the same spot ({kitNow.GlobalPosition}) and everything else unchanged");
        Check(Snap() == before, "after the v1 → v2 load the whole save matches the original snapshot");
        save.Save();

        // Auto-save: at the rollover, after a purchase, after a delivery.
        int saves = 0;
        void OnSaved(string p) => saves++;
        save.Saved += OnSaved;
        clock.Release("test");
        var fade = main.GetNode<DayTransitionView>("Transition/DayTransition");
        fade.FadeSeconds = 0.05f;
        fade.HoldSeconds = 0.05f;
        clock.BeginSleep(SleepReason.Bed);
        await WaitUntil(() => !clock.IsSleeping && !main.GetNode<SleepSequence>("Systems/SleepSequence").IsRunning, 600);
        Check(saves == 1 && SaveGame.Read(TestSavePath)["clock"].AsGodotDictionary()["day"].AsInt32() == 5, "auto-save after the night (Day 5 on disk)");
        var bubble = main.GetNode<PictureBubbleView>("HUD/UI/PictureBubble");
        while (bubble.IsOpen) { bubble.PressCancel(); await Frames(1); }
        var shop = main.GetNode<ShopView>("HUD/UI/Shop");
        shop.Open("phone", Phone.Entries());
        shop.Buy(0);
        shop.Close();
        Check(saves == 2, "auto-save after a purchase");
        Inventory.Instance.Add(ItemIds.Carrot, 5);
        om.Deliver(OrderManager.SecondOrderId);
        Check(saves == 3, "auto-save after a delivered order");
        save.Saved -= OnSaved;

        main.QueueFree();
        await Frames(2);
        save.DeleteSave();
        Check(!save.HasSave, "test save removed");
        save.SavePath = SaveGame.DefaultPath;
        save.Enabled = false;
        clock.Release("test");
        ResetTask14State();
    }
}
