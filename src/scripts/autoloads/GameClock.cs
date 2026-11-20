using System.Collections.Generic;
using Godot;

namespace MyFarmingGame.Autoloads;

/// <summary>The 8 day phases of 3 in-game hours each (GDD §4, mechanics.md §2).</summary>
public enum DayPhase { Dawn, Morning, Midday, Afternoon, Evening, Dusk, Night, SmallHours }

/// <summary>Seasons of 30 days each (GDD §4). Phase-001 only plays Spring.</summary>
public enum Season { Spring, Summer, Autumn, Winter }

/// <summary>Why Kit went to sleep.</summary>
public enum SleepReason { Bed, Bedtime }

/// <summary>
/// The in-game day (Task 1.3, autoload <c>GameClock</c>).
///
/// * The <b>digital readout</b> ticks every <see cref="ReadoutMinutes"/> (15) in-game minutes
///   (15 real seconds; owner 2026-11-20). The <b>day phases</b> are 3-hour steps counted from the
///   04:30 wake-up (04:30, 07:30, … 19:30); the dial's sun/moon marker jumps per phase.
///   Time runs underneath at <see cref="RealSecondsPerStep"/> (3 min) per step (60 s per
///   in-game hour, Harvest Moon pace — owner 2026-11-20), so the playable day 04:30 → 21:30
///   (17 in-game hours) lasts 17 real minutes.
/// * Time only moves from frame deltas while this node processes: it stops when the tree is paused
///   (pause menu), while a <see cref="Hold"/> is active (any menu), while Kit sleeps, and of course
///   while the game is closed. Nothing reads the wall clock.
/// * At <see cref="BedtimeMinute"/> (21:30, owner decision 2026-11-20) Kit goes home and sleeps automatically, no penalty.
/// * Sleep flow: <see cref="AskToSleep"/> (bed prompt) → <see cref="BeginSleep"/> →
///   <see cref="CompleteRollover"/> (screen is dark: <see cref="DayEnded"/>, day+1, 04:30,
///   <see cref="DayStarted"/>) → <see cref="Wake"/>. Main's SleepSequence drives the middle steps.
///
/// Other systems subscribe to <see cref="DayEnded"/> / <see cref="DayStarted"/>, which fire
/// exactly once per rollover:
/// <code>GameClock.Instance.DayEnded += day => ...;</code>
/// </summary>
public partial class GameClock : Node
{
    // ---- Design constants (GDD §4) ---------------------------------------------------------
    public const int PhaseCount = 8;
    public const int MinutesPerStep = 180;              // 3 in-game hours = one day phase
    public const int ReadoutMinutes = 15;               // the HUD time ticks every 15 in-game minutes
    public const double RealSecondsPerStep = 180.0;     // 3 min → 60 s per in-game hour (Harvest Moon pace)
    public const int DayStartMinute = 4 * 60 + 30;      // wake-up and Dawn, 04:30 (owner 2026-11-20)
    public const int BedtimeMinute = 21 * 60 + 30;      // 21:30 forced sleep (owner 2026-11-20)
    public const int PlayableMinutes = BedtimeMinute - DayStartMinute; // 1020 = 17 in-game hours
    public const int SeasonLengthDays = 30;
    public const int SeasonsPerYear = 4;

    /// <summary>A long hitch (window dragged, breakpoint) never counts as more than this.</summary>
    public const double MaxFrameSeconds = 0.25;

    public static GameClock Instance { get; private set; }

    // ---- Signals ---------------------------------------------------------------------------
    /// <summary>The digital readout ticked (every 15 in-game minutes, also on waking).
    /// Argument: minute of the day (0–1439).</summary>
    [Signal] public delegate void TimeChangedEventHandler(int minuteOfDay);
    /// <summary>The day phase changed (also on waking). Argument: <see cref="DayPhase"/> as int.</summary>
    [Signal] public delegate void StepChangedEventHandler(int phase);
    /// <summary>Midnight was reached; automatic sleep starts right after.</summary>
    [Signal] public delegate void BedtimeReachedEventHandler();
    /// <summary>Kit used the bed: show the "sleep now?" prompt.</summary>
    [Signal] public delegate void SleepAskedEventHandler();
    /// <summary>The sleep prompt was answered "not yet".</summary>
    [Signal] public delegate void SleepAskCancelledEventHandler();
    /// <summary>Kit is going to sleep. Argument: <see cref="SleepReason"/> as int.</summary>
    [Signal] public delegate void SleepStartedEventHandler(int reason);
    /// <summary>The day is over (fires once per rollover, before the day number changes).</summary>
    [Signal] public delegate void DayEndedEventHandler(int day);
    /// <summary>A new day began at 04:30 (fires once per rollover).</summary>
    [Signal] public delegate void DayStartedEventHandler(int day);
    /// <summary>Kit is awake and can play again.</summary>
    [Signal] public delegate void WokeUpEventHandler();

    // ---- State -----------------------------------------------------------------------------
    /// <summary>Absolute day number, starting at 1.</summary>
    public int Day { get; private set; } = 1;

    /// <summary>In-game minutes since 04:30 today (0 … <see cref="PlayableMinutes"/>).</summary>
    public double MinutesIntoDay { get; private set; }

    /// <summary>Speeds the clock up (tests, debug fast-forward). 1 = real design speed.</summary>
    public double TimeScale { get; set; } = 1.0;

    public bool IsAsking { get; private set; }
    public bool IsSleeping { get; private set; }
    public SleepReason LastSleepReason { get; private set; }

    private readonly HashSet<string> _holds = new();
    private int _lastStep;
    private int _lastReadout;
    private bool _rolledOver;

    /// <summary>True while time moves (nothing holds it, no prompt, not asleep).</summary>
    public bool IsRunning => _holds.Count == 0 && !IsAsking && !IsSleeping;

    // ---- Derived values --------------------------------------------------------------------
    /// <summary>In-game minutes since midnight of the current day, 270 … 1290.</summary>
    public double AbsoluteMinute => DayStartMinute + MinutesIntoDay;

    /// <summary>Index of the current 3-hour step counted from Dawn (0 = Dawn … 7 = Small hours).</summary>
    public int StepIndex => Mathf.Clamp((int)(MinutesIntoDay / MinutesPerStep), 0, PhaseCount - 1);

    public DayPhase Phase => (DayPhase)StepIndex;

    /// <summary>Index of the current 15-minute readout tick since the wake-up.</summary>
    public int ReadoutIndex => (int)(MinutesIntoDay / ReadoutMinutes);

    /// <summary>Minute of the day (0–1439) the HUD shows: the time rounded down to 15 minutes.</summary>
    public int ShownMinuteOfDay => (DayStartMinute + ReadoutIndex * ReadoutMinutes) % (24 * 60);

    /// <summary>The digital readout, e.g. "07:45".</summary>
    public string ShownTime => FormatTime(ShownMinuteOfDay);

    public int DayOfSeason => (Day - 1) % SeasonLengthDays + 1;
    public Season Season => (Season)((Day - 1) / SeasonLengthDays % SeasonsPerYear);
    public int Year => (Day - 1) / (SeasonLengthDays * SeasonsPerYear) + 1;

    /// <summary>Minute of the day (0–1439) at which a phase starts.</summary>
    public static int StepStartMinute(DayPhase phase) => (DayStartMinute + (int)phase * MinutesPerStep) % (24 * 60);

    /// <summary>"07:30"-style text for a minute of the day.</summary>
    public static string FormatTime(int minuteOfDay) => $"{minuteOfDay / 60 % 24:00}:{minuteOfDay % 60:00}";

    /// <summary>Daylight phases show the sun on the dial; the rest show the moon.</summary>
    public static bool IsDaylight(DayPhase phase) => phase <= DayPhase.Evening;

    // ---- Lifecycle -------------------------------------------------------------------------
    public override void _EnterTree()
    {
        // Only the autoload registers itself; a test-made extra clock leaves Instance alone.
        if (Instance == null && GetParent() == GetTree().Root)
            Instance = this;
        // Day logic stops with the game (project-structure.md §4 process-mode table).
        ProcessMode = ProcessModeEnum.Pausable;
    }

    public override void _ExitTree()
    {
        if (Instance == this)
            Instance = null;
    }

    public override void _Process(double delta) => Tick(delta);

    /// <summary>Advances time by one frame's delta. Public so tests can drive it directly.</summary>
    public void Tick(double realDelta)
    {
        if (!IsRunning || realDelta <= 0)
            return;
        double seconds = Mathf.Min(realDelta, MaxFrameSeconds) * TimeScale;
        double next = MinutesIntoDay + seconds * MinutesPerStep / RealSecondsPerStep;
        bool bedtime = next >= PlayableMinutes;
        MinutesIntoDay = bedtime ? PlayableMinutes : next;
        EmitStepChanges();
        if (ReadoutIndex != _lastReadout)
        {
            _lastReadout = ReadoutIndex;
            EmitSignal(SignalName.TimeChanged, ShownMinuteOfDay);
        }
        if (bedtime)
        {
            EmitSignal(SignalName.BedtimeReached);
            BeginSleep(SleepReason.Bedtime);
        }
    }

    private void EmitStepChanges()
    {
        int step = StepIndex;
        // Emit every crossed step (a fast TimeScale can cross several in one frame).
        while (_lastStep < step)
        {
            _lastStep++;
            EmitSignal(SignalName.StepChanged, _lastStep);
        }
        _lastStep = step;
    }

    // ---- Holds (menus) ---------------------------------------------------------------------
    /// <summary>Stops time while <paramref name="reason"/> is held (a menu is open).</summary>
    public void Hold(string reason) => _holds.Add(reason);

    public void Release(string reason) => _holds.Remove(reason);

    public bool IsHeld(string reason) => _holds.Contains(reason);

    // ---- Sleep -----------------------------------------------------------------------------
    /// <summary>Kit used the bed: stops time and asks "sleep now?". Ignored while asleep.</summary>
    public void AskToSleep()
    {
        if (IsAsking || IsSleeping)
            return;
        IsAsking = true;
        EmitSignal(SignalName.SleepAsked);
    }

    /// <summary>The prompt was closed with "not yet": time runs again.</summary>
    public void CancelAsk()
    {
        if (!IsAsking)
            return;
        IsAsking = false;
        EmitSignal(SignalName.SleepAskCancelled);
    }

    /// <summary>Starts sleeping (bed confirmed or 21:30). A second call while asleep does nothing,
    /// so a rollover can never happen twice.</summary>
    public void BeginSleep(SleepReason reason)
    {
        if (IsSleeping)
            return;
        IsAsking = false;
        IsSleeping = true;
        _rolledOver = false;
        LastSleepReason = reason;
        EmitSignal(SignalName.SleepStarted, (int)reason);
    }

    /// <summary>Ends the day and starts the next at 04:30. Call while the screen is dark.
    /// Only works once per sleep.</summary>
    public void CompleteRollover()
    {
        if (!IsSleeping || _rolledOver)
            return;
        _rolledOver = true;
        int ended = Day;
        EmitSignal(SignalName.DayEnded, ended);
        Day = ended + 1;
        MinutesIntoDay = 0;
        _lastStep = 0;
        _lastReadout = 0;
        EmitSignal(SignalName.DayStarted, Day);
        EmitSignal(SignalName.StepChanged, (int)DayPhase.Dawn);
        EmitSignal(SignalName.TimeChanged, ShownMinuteOfDay);
    }

    /// <summary>Kit is awake: time runs again. Rolls the day over first if that has not happened.</summary>
    public void Wake()
    {
        if (!IsSleeping)
            return;
        CompleteRollover();
        IsSleeping = false;
        EmitSignal(SignalName.WokeUp);
    }

    // ---- Setup (new game, save/load, tests, debug) -----------------------------------------
    /// <summary>Sets the calendar and time directly (no signals for days; StepChanged fires).
    /// <paramref name="minutesIntoDay"/> counts from 04:30.</summary>
    public void SetTime(int day, double minutesIntoDay)
    {
        Day = Mathf.Max(1, day);
        MinutesIntoDay = Mathf.Clamp(minutesIntoDay, 0, PlayableMinutes - 0.001);
        _lastStep = StepIndex;
        _lastReadout = ReadoutIndex;
        EmitSignal(SignalName.StepChanged, _lastStep);
        EmitSignal(SignalName.TimeChanged, ShownMinuteOfDay);
    }

    /// <summary>Day 1, 04:30, awake, no holds, normal speed.</summary>
    public void ResetForNewGame()
    {
        _holds.Clear();
        IsAsking = false;
        IsSleeping = false;
        _rolledOver = false;
        TimeScale = 1.0;
        SetTime(1, 0);
    }
}
