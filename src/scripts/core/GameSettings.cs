using System;
using Godot;

namespace MyFarmingGame.Core;

/// <summary>
/// Player settings (Task 1.11, pause menu → Settings): volume, nudges on/off, read-aloud (placeholder,
/// no voice yet) and the break reminder (off by default, GDD §4; only the toggle exists — the interval
/// is not agreed yet). Kept separate from the game save so a new game keeps them; written by
/// <c>SaveGame.SaveSettings</c> to <c>user://settings.json</c>.
/// </summary>
public static class GameSettings
{
    /// <summary>Master volume 0–1 (applied to the Master audio bus).</summary>
    public static float Volume { get; private set; } = 0.8f;
    /// <summary>One-time hints (Task 1.12) on or off.</summary>
    public static bool Nudges { get; set; } = true;
    /// <summary>Read dialogue aloud (placeholder: the hook exists, no voice yet).</summary>
    public static bool ReadAloud { get; set; }
    /// <summary>Break reminder (off by default, GDD §4). Interval still to be agreed.</summary>
    public static bool BreakReminder { get; set; }

    public const float VolumeStep = 0.1f;

    public static event Action Changed;

    public static void SetVolume(float v)
    {
        Volume = Mathf.Clamp(Mathf.Snapped(v, 0.01f), 0f, 1f);
        int bus = AudioServer.GetBusIndex("Master");
        if (bus >= 0)
        {
            AudioServer.SetBusVolumeDb(bus, Mathf.LinearToDb(Mathf.Max(Volume, 0.0001f)));
            AudioServer.SetBusMute(bus, Volume <= 0.001f);
        }
        Changed?.Invoke();
    }

    public static void Set(string key, bool value)
    {
        switch (key)
        {
            case nameof(Nudges): Nudges = value; break;
            case nameof(ReadAloud): ReadAloud = value; break;
            case nameof(BreakReminder): BreakReminder = value; break;
        }
        Changed?.Invoke();
    }

    public static void ResetToDefaults()
    {
        Nudges = true;
        ReadAloud = false;
        BreakReminder = false;
        SetVolume(0.8f);
    }

    public static Godot.Collections.Dictionary ToDict() => new()
    {
        { "volume", Volume }, { "nudges", Nudges }, { "readAloud", ReadAloud }, { "breakReminder", BreakReminder },
    };

    public static void FromDict(Godot.Collections.Dictionary d)
    {
        if (d.TryGetValue("nudges", out var n)) Nudges = n.AsBool();
        if (d.TryGetValue("readAloud", out var r)) ReadAloud = r.AsBool();
        if (d.TryGetValue("breakReminder", out var b)) BreakReminder = b.AsBool();
        SetVolume(d.TryGetValue("volume", out var v) ? (float)v.AsDouble() : 0.8f);
    }
}
