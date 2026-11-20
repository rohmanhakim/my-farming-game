using System;
using System.Collections.Generic;
using Godot;

namespace MyFarmingGame.Autoloads;

/// <summary>
/// One-slot, versioned auto-save (Task 1.10, GDD §13, project-structure.md rule 8): human-readable
/// JSON at <see cref="SavePath"/> with a top-level <c>schemaVersion</c>.
///
/// * <b>Sections</b> are owned by the systems that hold the state. Each registers a pair of
///   callbacks with <see cref="Register"/> (save → Dictionary, load ← Dictionary) and unregisters
///   when it leaves the tree. Autoload sections (clock, inventory, wallet, orders, feed, ledger) are
///   registered by Main's <c>SaveBinder</c>; the level's (farm, animals, forage, player) too;
///   onboarding registers its own in Task 1.12.
/// * <b>Safe write:</b> the new file is written to <c>savegame.tmp.json</c>, the old one becomes
///   <see cref="BackupPath"/>, then the temp file is renamed into place. If the main file can't be
///   parsed, the backup is loaded instead.
/// * <b>Migrations:</b> <see cref="Migrations"/> maps a version to a step that upgrades the data to
///   the next one; <see cref="Migrate"/> runs the chain up to <see cref="SchemaVersion"/>.
/// * Only this class writes to <c>user://</c>. Tests point <see cref="SavePath"/> at a temp file or
///   switch <see cref="Enabled"/> off.
/// </summary>
public partial class SaveGame : Node
{
    public static SaveGame Instance { get; private set; }

    public const int SchemaVersion = 1;
    public const string DefaultPath = "user://savegame.json";

    /// <summary>When false nothing is loaded or saved (tests).</summary>
    public bool Enabled { get; set; } = true;

    public string SavePath { get; set; } = DefaultPath;
    public string BackupPath => SavePath.Replace(".json", ".bak.json");
    public string TempPath => SavePath.Replace(".json", ".tmp.json");

    /// <summary>A save was written. Argument: path.</summary>
    [Signal] public delegate void SavedEventHandler(string path);
    /// <summary>A save was loaded. Argument: path it came from (main or backup).</summary>
    [Signal] public delegate void LoadedEventHandler(string path);

    private readonly Dictionary<string, (Func<Godot.Collections.Dictionary> Save, Action<Godot.Collections.Dictionary> Load)> _sections = new();

    /// <summary>Upgrade steps: version N → N+1. A no-op chain is ready for v1 → v2.</summary>
    public static readonly Dictionary<int, Func<Godot.Collections.Dictionary, Godot.Collections.Dictionary>> Migrations = new();

    /// <summary>How many migration steps ran on the last load (tests).</summary>
    public int MigrationsRun { get; private set; }
    /// <summary>Where the last load came from ("" = nothing loaded).</summary>
    public string LastLoadedFrom { get; private set; } = "";

    public IEnumerable<string> Sections => _sections.Keys;

    public override void _EnterTree()
    {
        if (Instance == null && GetParent() == GetTree().Root)
            Instance = this;
        ProcessMode = ProcessModeEnum.Always; // project-structure.md §4: saving works during pause
    }

    public override void _ExitTree()
    {
        if (Instance == this)
            Instance = null;
    }

    public void Register(string section, Func<Godot.Collections.Dictionary> save, Action<Godot.Collections.Dictionary> load) =>
        _sections[section] = (save, load);

    public void Unregister(string section) => _sections.Remove(section);

    public bool HasSave => FileAccess.FileExists(SavePath) || FileAccess.FileExists(BackupPath);

    /// <summary>Everything the registered sections hold, as the save file's dictionary.</summary>
    public Godot.Collections.Dictionary Snapshot()
    {
        var root = new Godot.Collections.Dictionary { { "schemaVersion", SchemaVersion } };
        foreach (var (name, s) in _sections)
            root[name] = s.Save();
        return root;
    }

    /// <summary>Writes the save safely (temp + rename, previous kept as backup). Returns success.</summary>
    public bool Save()
    {
        if (!Enabled) return false;
        string json = Json.Stringify(Snapshot(), "  ");
        using (var f = FileAccess.Open(TempPath, FileAccess.ModeFlags.Write))
        {
            if (f == null) { GD.PushError($"[SaveGame] can't write {TempPath}: {FileAccess.GetOpenError()}"); return false; }
            f.StoreString(json);
        }
        string main = ProjectSettings.GlobalizePath(SavePath), bak = ProjectSettings.GlobalizePath(BackupPath), tmp = ProjectSettings.GlobalizePath(TempPath);
        if (FileAccess.FileExists(SavePath))
        {
            if (FileAccess.FileExists(BackupPath)) DirAccess.RemoveAbsolute(bak);
            DirAccess.RenameAbsolute(main, bak);
        }
        var err = DirAccess.RenameAbsolute(tmp, main);
        if (err != Error.Ok) { GD.PushError($"[SaveGame] rename failed: {err}"); return false; }
        EmitSignal(SignalName.Saved, SavePath);
        return true;
    }

    /// <summary>Loads the main file, or the backup if the main file is missing or broken, into every
    /// registered section. Returns false when there is nothing to load (new game).</summary>
    public bool Load()
    {
        LastLoadedFrom = "";
        if (!Enabled) return false;
        foreach (var path in new[] { SavePath, BackupPath })
        {
            var data = Read(path);
            if (data == null) continue;
            Apply(data);
            LastLoadedFrom = path;
            EmitSignal(SignalName.Loaded, path);
            return true;
        }
        return false;
    }

    /// <summary>Parses one save file; null when missing, unreadable or not a v1+ save.</summary>
    public static Godot.Collections.Dictionary Read(string path)
    {
        if (!FileAccess.FileExists(path)) return null;
        string text = FileAccess.GetFileAsString(path);
        var json = new Json();
        if (json.Parse(text) != Error.Ok || json.Data.VariantType != Variant.Type.Dictionary) return null;
        var d = json.Data.AsGodotDictionary();
        return d.ContainsKey("schemaVersion") ? d : null;
    }

    /// <summary>Upgrades and applies <paramref name="data"/> to the registered sections.</summary>
    public void Apply(Godot.Collections.Dictionary data)
    {
        data = Migrate(data);
        foreach (var (name, s) in _sections)
            if (data.TryGetValue(name, out var v) && v.VariantType == Variant.Type.Dictionary)
                s.Load(v.AsGodotDictionary());
    }

    /// <summary>Runs the migration chain from the file's version up to <see cref="SchemaVersion"/>.</summary>
    public Godot.Collections.Dictionary Migrate(Godot.Collections.Dictionary data)
    {
        MigrationsRun = 0;
        int v = data.TryGetValue("schemaVersion", out var sv) ? sv.AsInt32() : 0;
        while (v < SchemaVersion)
        {
            if (Migrations.TryGetValue(v, out var step)) data = step(data);
            MigrationsRun++;
            v++;
            data["schemaVersion"] = v;
        }
        return data;
    }

    /// <summary>Deletes the save and its backup (tests, "new game").</summary>
    public void DeleteSave()
    {
        foreach (var p in new[] { SavePath, BackupPath, TempPath })
            if (FileAccess.FileExists(p)) DirAccess.RemoveAbsolute(ProjectSettings.GlobalizePath(p));
    }
}
