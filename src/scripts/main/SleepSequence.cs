using Godot;
using MyFarmingGame.Autoloads;
using MyFarmingGame.Player;
using MyFarmingGame.UI;
using MyFarmingGame.World;

namespace MyFarmingGame;

/// <summary>
/// Runs Kit's night (Task 1.3) for Main, which owns the player, camera and transitions
/// (project-structure.md §4). Listens to <see cref="GameClock"/>:
/// * SleepAsked → Kit stands still while the prompt is open (cancel → Kit can move again).
/// * SleepStarted (bed or 21:30 bedtime) → fade out → <see cref="GameClock.CompleteRollover"/>
///   → Kit is put at the bed's wake spot, camera snapped → "Day N" → fade in → <see cref="GameClock.Wake"/>.
/// </summary>
public partial class SleepSequence : Node
{
    [Export] public NodePath PlayerPath { get; set; }
    [Export] public NodePath CameraPath { get; set; }
    [Export] public NodePath TransitionPath { get; set; }

    public bool IsRunning { get; private set; }

    private PlayerController _player;
    private CameraFollow _camera;
    private DayTransitionView _fade;
    private GameClock _clock;

    public override void _Ready()
    {
        _player = GetNodeOrNull<PlayerController>(PlayerPath);
        _camera = GetNodeOrNull<CameraFollow>(CameraPath);
        _fade = GetNodeOrNull<DayTransitionView>(TransitionPath);
        _clock = GameClock.Instance;
        if (_clock == null)
            return;
        _clock.SleepAsked += OnSleepAsked;
        _clock.SleepAskCancelled += OnSleepAskCancelled;
        _clock.SleepStarted += OnSleepStarted;
    }

    public override void _ExitTree()
    {
        if (_clock == null || !IsInstanceValid(_clock))
            return;
        _clock.SleepAsked -= OnSleepAsked;
        _clock.SleepAskCancelled -= OnSleepAskCancelled;
        _clock.SleepStarted -= OnSleepStarted;
    }

    private void OnSleepAsked() { if (_player != null) _player.Frozen = true; }

    private void OnSleepAskCancelled() { if (_player != null) _player.Frozen = false; }

    private void OnSleepStarted(int reason) => Run();

    private async void Run()
    {
        if (IsRunning)
            return;
        IsRunning = true;
        if (_player != null) _player.Frozen = true;

        if (_fade != null)
            await _fade.FadeOut();
        if (!IsInsideTree())
            return;

        _clock.CompleteRollover();
        MoveKitToBed();

        if (_fade != null)
            await _fade.ShowDayAndFadeIn(_clock.Season, _clock.DayOfSeason);
        if (!IsInsideTree())
            return;

        _clock.Wake();
        if (_player != null) _player.Frozen = false;
        IsRunning = false;
    }

    /// <summary>Kit wakes up beside the bed ("goes home" at 21:30).</summary>
    private void MoveKitToBed()
    {
        if (_player == null)
            return;
        foreach (Node n in GetTree().GetNodesInGroup(Bed.Group))
        {
            if (n is Bed bed && bed.IsInsideTree())
            {
                _player.GlobalPosition = bed.WakePoint;
                break;
            }
        }
        _camera?.SnapToTarget();
    }
}
