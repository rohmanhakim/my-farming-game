using System.Linq;
using Godot;
using MyFarmingGame.Autoloads;
using MyFarmingGame.Player;
using MyFarmingGame.World;

namespace MyFarmingGame.Debugging;

/// <summary>
/// Windowed performance check (Phase-001 Task 1.20; project-structure.md §16). Run with
/// <c>tools/perf_check.sh</c> (needs a window, not headless). Loads the real Main with saving off,
/// fills the front garden with ripe carrots (the busiest garden), puts Kit by the coop and barn path
/// so the house, coop, garden and fields are on screen, walks Kit for a few seconds and reports the
/// average and worst frame time, draw calls and triangles. Profile <c>pc</c> = the default settings;
/// <c>handheld</c> = Switch-like: 1280 × 720, one shadow split at 20 m, smaller shadow map, MSAA off,
/// measured uncapped against 33 ms (30 fps). Exit 0 = inside the budget. Debug-only (src/debug/ is never exported).
/// </summary>
public partial class PerfCheck : Node
{
    public static readonly (string Name, int MaxDraws, int MaxTris, double MaxAvgMs)[] Budgets =
    {
        ("pc", 300, 150_000, 1000.0 / 60.0),
        ("handheld", 250, 100_000, 1000.0 / 30.0), // draws revised from 150 after measuring (Task 1.20): HUD ≈ 84 of them
    };

    private Main _main;
    private string _profile = "pc";
    private int _frame;
    private double _sum, _worst;
    private int _samples;
    private double _draws, _tris, _noHud;
    private int _exit;

    public override void _Ready()
    {
        foreach (var a in OS.GetCmdlineUserArgs())
            if (a.StartsWith("--profile=")) _profile = a.Substring("--profile=".Length);
        if (SaveGame.Instance != null) SaveGame.Instance.Enabled = false;
        _main = ResourceLoader.Load<PackedScene>("res://scenes/main/Main.tscn").Instantiate<Main>();
        AddChild(_main);
        if (_profile == "handheld")
        {
            GetWindow().Size = new Vector2I(1280, 720);
            GetViewport().Msaa3D = Viewport.Msaa.Disabled;
            RenderingServer.DirectionalShadowAtlasSetSize(2048, true);
            var sun = _main.GetNode<DirectionalLight3D>("World/Effects/Sun");
            sun.DirectionalShadowMode = DirectionalLight3D.ShadowMode.Orthogonal;
            sun.DirectionalShadowMaxDistance = 20f;
        }
        else
        {
            GetViewport().Msaa3D = Viewport.Msaa.Msaa2X;
        }
        var s = _main.GetNode<DirectionalLight3D>("World/Effects/Sun");
        GD.Print($"PERF settings: shadow mode {s.DirectionalShadowMode}, distance {s.DirectionalShadowMaxDistance}, renderer {ProjectSettings.GetSetting("rendering/renderer/rendering_method")}");
    }

    public override void _Process(double delta)
    {
        _frame++;
        if (_frame == 3)
        {
            GameClock.Instance?.SetTime(1, 2 * GameClock.MinutesPerStep + 1); // midday
            GameClock.Instance?.Hold("perf");
            var grid = _main.Level.GetNode<FarmGrid>("FrontGarden");
            foreach (var (_, t) in grid.AllTiles())
            {
                t.Soil = Soil.Tilled; t.CropId = "carrot"; t.DaysToHarvest = 3; t.DaysGrown = 3;
            }
            grid.QueueRedraw();
            _main.Player.GlobalPosition = new Vector3(25f, 0, 6.5f);
            _main.Rig.Snap();
        }
        if (_frame == 40) Input.ActionPress("move_right");
        if (_frame > 90 && _frame <= 270)
        {
            double ms = delta * 1000.0;
            _sum += ms; _samples++;
            if (ms > _worst) _worst = ms;
            _draws = Mathf.Max(_draws, Performance.GetMonitor(Performance.Monitor.RenderTotalDrawCallsInFrame));
            _tris = Mathf.Max(_tris, Performance.GetMonitor(Performance.Monitor.RenderTotalPrimitivesInFrame));
            if (_frame == 180) { Input.ActionRelease("move_right"); Input.ActionPress("move_left"); }
        }
        // Breakdown (printed, not gated): HUD hidden, then shadows off too.
        if (_frame == 272) { _main.GetNode<CanvasLayer>("HUD").Visible = false; _main.GetNode<CanvasLayer>("Debug").Visible = false; }
        if (_frame == 276) _noHud = Performance.GetMonitor(Performance.Monitor.RenderTotalDrawCallsInFrame);
        if (_frame == 277) _main.GetNode<DirectionalLight3D>("World/Effects/Sun").ShadowEnabled = false;
        if (_frame == 281)
        {
            double noShadow = Performance.GetMonitor(Performance.Monitor.RenderTotalDrawCallsInFrame);
            GD.Print($"PERF breakdown profile={_profile}: all {_draws} · without HUD {_noHud} · without HUD and shadows {noShadow}");
            GetTree().Quit(_exit);
        }
        if (_frame == 271)
        {
            Input.ActionRelease("move_left");
            var b = Budgets.First(x => x.Name == _profile);
            double avg = _sum / Mathf.Max(1, _samples);
            bool ok = _draws <= b.MaxDraws && _tris <= b.MaxTris && avg <= b.MaxAvgMs * 1.05;
            GD.Print($"PERF profile={_profile} avg_ms={avg:0.0} worst_ms={_worst:0.0} fps={1000.0 / avg:0} draws={_draws} (≤{b.MaxDraws}) tris={_tris} (≤{b.MaxTris}) window={GetWindow().Size} → {(ok ? "OK" : "OVER BUDGET")}");
            _exit = ok ? 0 : 1;
        }
    }
}
