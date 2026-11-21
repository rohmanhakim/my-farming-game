using System.Collections.Generic;
using Godot;

namespace MyFarmingGame.Core;

/// <summary>
/// Builds 3D models by logical name (for example "kit", "house", "crop_carrot_ready") so real models
/// can replace placeholders with no code or scene change (Task 1.14, project-structure.md §9).
///
/// Lookup order for a name:
///   1. Real model:  res://assets/models/{characters,world,items}/{name}.glb (or .tscn)
///   2. Placeholder: primitive mesh parts from res://data/placeholder_models.json
///   3. Fallback:    a 1 m magenta box (a missing model is obvious but never crashes)
///
/// Placeholder meshes and materials are cached and shared: equal colours use one material and equal
/// shapes one mesh (performance budget, project-structure.md §16).
/// </summary>
public static class ModelLibrary
{
    public const string DefaultModelsRoot = "res://assets/models";
    public const string PlaceholderTablePath = "res://data/placeholder_models.json";

    /// <summary>Folders under <see cref="ModelsRoot"/> searched for real models.</summary>
    public static readonly string[] ModelFolders = { "characters", "world", "items" };

    /// <summary>File types tried for a real model, in order.</summary>
    public static readonly string[] ModelExtensions = { "glb", "tscn" };

    /// <summary>Root folder for real models. Tests point it at user:// and restore it.</summary>
    public static string ModelsRoot { get; set; } = DefaultModelsRoot;

    public static readonly Color FallbackColor = new(1f, 0f, 1f);

    public enum Source { Model, Placeholder, Fallback }

    /// <summary>One primitive part of a placeholder model.</summary>
    public readonly record struct Part(string Shape, Vector3 Size, Color Color, Vector3 Offset, float RotationY = 0f);

    private static Dictionary<string, List<Part>> _table;
    private static readonly Dictionary<Color, StandardMaterial3D> Materials = new();
    private static readonly Dictionary<(string, Vector3), Mesh> Meshes = new();

    /// <summary>All names in the placeholder table (used by the tests).</summary>
    public static IEnumerable<string> PlaceholderNames => Table.Keys;

    /// <summary>The placeholder parts for <paramref name="name"/>, or null when it has none.</summary>
    public static IReadOnlyList<Part> PlaceholderParts(string name) =>
        Table.TryGetValue(name, out var parts) ? parts : null;

    /// <summary>A new node tree for <paramref name="name"/> (origin at the feet, facing −Z).</summary>
    public static Node3D Instantiate(string name) => Instantiate(name, out _);

    public static Node3D Instantiate(string name, out Source source)
    {
        string path = FindModelPath(name);
        if (path != null && ResourceLoader.Load<PackedScene>(path)?.Instantiate() is Node3D real)
        {
            source = Source.Model;
            real.Name = "Model";
            return real;
        }
        var root = new Node3D { Name = "Model" };
        if (Table.TryGetValue(name ?? "", out var parts) && parts.Count > 0)
        {
            source = Source.Placeholder;
            for (int i = 0; i < parts.Count; i++)
                root.AddChild(MakePart(parts[i], $"Part{i}"));
        }
        else
        {
            source = Source.Fallback;
            root.AddChild(MakePart(new Part("box", Vector3.One, FallbackColor, new Vector3(0, 0.5f, 0)), "Fallback"));
        }
        return root;
    }

    /// <summary>The res:// (or test user://) path of a real model for <paramref name="name"/>, or null.</summary>
    public static string FindModelPath(string name)
    {
        if (string.IsNullOrEmpty(name)) return null;
        foreach (var folder in ModelFolders)
            foreach (var ext in ModelExtensions)
            {
                string p = $"{ModelsRoot}/{folder}/{name}.{ext}";
                if (ResourceLoader.Exists(p)) return p;
            }
        return null;
    }

    /// <summary>The shared material for a flat colour.</summary>
    public static StandardMaterial3D MaterialFor(Color c)
    {
        if (Materials.TryGetValue(c, out var m)) return m;
        m = new StandardMaterial3D { AlbedoColor = c, Roughness = 0.9f };
        Materials[c] = m;
        return m;
    }

    /// <summary>Mesh, shared material and placement of each placeholder part (for MultiMesh drawing,
    /// e.g. the garden tiles). Null when the name has no placeholder.</summary>
    public static List<(Mesh Mesh, Material Material, Transform3D Local)> PlaceholderMeshes(string name)
    {
        if (!Table.TryGetValue(name ?? "", out var parts)) return null;
        var list = new List<(Mesh, Material, Transform3D)>();
        foreach (var p in parts)
            list.Add((MeshFor(p.Shape, p.Size), MaterialFor(p.Color),
                new Transform3D(Basis.FromEuler(new Vector3(0, Mathf.DegToRad(p.RotationY), 0)), p.Offset)));
        return list;
    }

    private static MeshInstance3D MakePart(Part p, string nodeName) => new()
    {
        Name = nodeName,
        Mesh = MeshFor(p.Shape, p.Size),
        MaterialOverride = MaterialFor(p.Color),
        Position = p.Offset,
        RotationDegrees = new Vector3(0, p.RotationY, 0),
    };

    /// <summary>The shared primitive mesh for a shape and size (metres).</summary>
    public static Mesh MeshFor(string shape, Vector3 size)
    {
        var key = (shape, size);
        if (Meshes.TryGetValue(key, out var mesh)) return mesh;
        mesh = shape switch
        {
            "capsule" => new CapsuleMesh { Radius = size.X / 2f, Height = Mathf.Max(size.Y, size.X), RadialSegments = 16, Rings = 4 },
            "cylinder" => new CylinderMesh { TopRadius = size.X / 2f, BottomRadius = size.X / 2f, Height = size.Y, RadialSegments = 12, Rings = 1 },
            "cone" => new CylinderMesh { TopRadius = 0f, BottomRadius = size.X / 2f, Height = size.Y, RadialSegments = 8, Rings = 1 },
            "sphere" => new SphereMesh { Radius = size.X / 2f, Height = size.Y, RadialSegments = 16, Rings = 8 },
            "prism" => new PrismMesh { Size = size },
            "plane" => new BoxMesh { Size = new Vector3(size.X, 0.02f, size.Z) },
            _ => new BoxMesh { Size = size },
        };
        Meshes[key] = mesh;
        return mesh;
    }

    private static Dictionary<string, List<Part>> Table => _table ??= LoadTable();

    /// <summary>Forgets the cached table (tests, or after editing the JSON while the game runs).</summary>
    public static void ReloadTable() => _table = null;

    private static Dictionary<string, List<Part>> LoadTable()
    {
        var table = new Dictionary<string, List<Part>>();
        using var f = FileAccess.Open(PlaceholderTablePath, FileAccess.ModeFlags.Read);
        if (f == null)
        {
            GD.PushError($"[ModelLibrary] cannot open {PlaceholderTablePath}");
            return table;
        }
        var parsed = Json.ParseString(f.GetAsText());
        if (parsed.VariantType != Variant.Type.Dictionary)
        {
            GD.PushError($"[ModelLibrary] {PlaceholderTablePath} is not a JSON object");
            return table;
        }
        foreach (var (key, value) in parsed.AsGodotDictionary())
        {
            string name = key.AsString();
            if (name.StartsWith("_") || value.VariantType != Variant.Type.Array) continue;
            var parts = new List<Part>();
            foreach (var pv in value.AsGodotArray())
            {
                var d = pv.AsGodotDictionary();
                parts.Add(new Part(
                    d.TryGetValue("shape", out var s) ? s.AsString() : "box",
                    ToVec(d, "size", Vector3.One),
                    d.TryGetValue("color", out var c) ? Color.FromString(c.AsString(), FallbackColor) : FallbackColor,
                    ToVec(d, "offset", Vector3.Zero),
                    d.TryGetValue("rotation_y", out var r) ? (float)r.AsDouble() : 0f));
            }
            table[name] = parts;
        }
        return table;
    }

    private static Vector3 ToVec(Godot.Collections.Dictionary d, string key, Vector3 fallback)
    {
        if (!d.TryGetValue(key, out var v) || v.VariantType != Variant.Type.Array) return fallback;
        var a = v.AsGodotArray();
        return a.Count < 3 ? fallback : new Vector3((float)a[0].AsDouble(), (float)a[1].AsDouble(), (float)a[2].AsDouble());
    }
}
