using System.Collections.Generic;
using System.Linq;
using Godot;
using MyFarmingGame.Autoloads;
using MyFarmingGame.Core;
using MyFarmingGame.Player;
using MyFarmingGame.UI;
using MyFarmingGame.World;

namespace MyFarmingGame;

/// <summary>
/// Runs the guided start and the one-time nudges (Task 1.12, onboarding.md §2–§3) for Main
/// (Main/Systems/OnboardingDirector). State lives in the <see cref="Onboarding"/> autoload.
///
/// * <b>Steps 1–10</b> advance when the game shows they happened (Kit at the front door, 3 weeds
///   pulled, 3 seeds planted, 3 beds watered, Marlow's order accepted, 3 handfuls of berries, the
///   order paid, the board up, every animal fed, then the free-play goal). Doing things early
///   simply skips steps; nothing is ever locked. A non-blocking hint chip (<see cref="NudgeView"/>)
///   and a bouncing <see cref="GuideArrow"/> show the current step.
/// * <b>Nudges</b> show once each (also across save/load) and only while
///   <see cref="GameSettings.Nudges"/> is on: first evening, morning after planting, yellow crop,
///   lost crop, carrots ready, feed running low, first morning, first eggs and milk. (Hungry hens are
///   the animals' own warning, Task 1.9, spoken by Ada.) Bubbles wait until no menu is open.
/// </summary>
public partial class OnboardingDirector : Node
{
    [Export] public NodePath PlayerPath { get; set; }
    [Export] public NodePath LevelPath { get; set; }
    [Export] public NodePath EffectsPath { get; set; }

    /// <summary>Seconds between checks of the step conditions.</summary>
    public const double CheckEvery = 0.25;

    private Onboarding _ob;
    private PlayerController _player;
    private Node _level;
    private GameClock _clock;
    private FarmGrid _garden;
    private Building _house;
    private AnimalHouse _hens, _cows;
    private Marlow _marlow;
    private OrderBoard _board;
    private Ada _ada;
    private List<BerryBush> _bushes = new();
    private GuideArrow _arrow;
    private readonly List<Node3D> _morningMarkers = new();
    private readonly Queue<(string Id, string Portrait, string Line, BubbleChip[] Chips)> _pending = new();
    private double _t;
    private bool _lostToday;

    public GuideArrow Arrow => _arrow;
    public IReadOnlyList<Node3D> MorningMarkers => _morningMarkers;
    public int PendingBubbles => _pending.Count;

    public override void _Ready()
    {
        _ob = Onboarding.Instance;
        _clock = GameClock.Instance;
        _player = GetNodeOrNull<PlayerController>(PlayerPath);
        _level = GetNodeOrNull(LevelPath);
        CareNoticeView.WarningPortrait = Ada.Portrait;
        CallDeferred(MethodName.Bind);
    }

    /// <summary>Finds the level's pieces once everything is ready.</summary>
    private void Bind()
    {
        if (_level == null || _ob == null) return;
        var lvl = _level.GetChildCount() > 0 ? _level.GetChild(0) : null;
        if (lvl == null) return;
        _garden = lvl.GetNodeOrNull<FarmGrid>("FrontGarden");
        _house = lvl.GetNodeOrNull<Building>("House");
        _marlow = lvl.GetNodeOrNull<Marlow>("Marlow");
        _board = lvl.GetNodeOrNull<OrderBoard>("OrderBoard");
        _ada = lvl.FindChild("Ada", true, false) as Ada;
        _bushes = lvl.GetNodeOrNull("Bushes")?.GetChildren().OfType<BerryBush>().ToList() ?? new List<BerryBush>();
        _hens = AnimalHouse.For(GetTree(), "hen");
        _cows = AnimalHouse.For(GetTree(), "cow");
        var effects = GetNodeOrNull<Node>(EffectsPath);
        _arrow = new GuideArrow { Name = "GuideArrow" };
        effects?.AddChild(_arrow);
        if (_garden != null)
        {
            _garden.ToolUsed += OnToolUsed;
            _garden.CropLost += OnCropLost;
        }
        foreach (var h in new[] { _hens, _cows }.Where(h => h != null))
            h.Collected += OnCollected;
        if (_clock != null)
        {
            _clock.WokeUp += OnWokeUp;
            _clock.StepChanged += OnPhase;
        }
        Evaluate();
    }

    public override void _ExitTree()
    {
        if (_garden != null && IsInstanceValid(_garden)) { _garden.ToolUsed -= OnToolUsed; _garden.CropLost -= OnCropLost; }
        foreach (var h in new[] { _hens, _cows }.Where(h => h != null && IsInstanceValid(h)))
            h.Collected -= OnCollected;
        if (_clock != null && IsInstanceValid(_clock)) { _clock.WokeUp -= OnWokeUp; _clock.StepChanged -= OnPhase; }
    }

    public override void _Process(double delta)
    {
        _t += delta;
        if (_t < CheckEvery) return;
        _t = 0;
        Evaluate();
    }

    // ---- Events ------------------------------------------------------------------------------
    private void OnToolUsed(int action, Vector2I cell)
    {
        switch ((TileAction)action)
        {
            case TileAction.Cleared: _ob.WeedsPulled++; break;
            case TileAction.Planted: _ob.Planted++; break;
            case TileAction.Watered: _ob.Watered++; break;
        }
        if ((TileAction)action == TileAction.Watered && _garden != null && !_garden.AllTiles().Any(t => t.Tile.HasCrop && !t.Tile.Watered))
            _garden.WaterHints = false;
        Evaluate();
    }

    private void OnCropLost(Vector2I cell) => _lostToday = true;

    private void OnPhase(int phase)
    {
        if ((DayPhase)phase == DayPhase.Evening)
            Nudge(Onboarding.FirstEvening, Ada.Portrait, "It's getting late. Sleep at home to start a new day.",
                new[] { new BubbleChip("icon_sleep", ""), new BubbleChip("bed", "") });
    }

    private void OnWokeUp()
    {
        // First morning: egg and milk icons over the coop and barn until something is collected.
        if (Allowed && !_ob.WasShown(Onboarding.FirstMorning) && (_hens?.Animals.Any(a => a.ProductReady > 0) == true || _cows?.Animals.Any(a => a.ProductReady > 0) == true))
        {
            _ob.MarkShown(Onboarding.FirstMorning);
            AddMorningMarker(_hens, ItemIds.Egg);
            AddMorningMarker(_cows, ItemIds.Milk);
        }
        if (_garden != null)
        {
            var tiles = _garden.AllTiles().Select(t => t.Tile).ToList();
            if (tiles.Any(t => t.HasCrop && !t.IsReady) && Allowed && !_ob.WasShown(Onboarding.WaterMorning))
            {
                _ob.MarkShown(Onboarding.WaterMorning);
                _garden.WaterHints = true; // a watering-can icon over every dry bed until watered
            }
            if (tiles.Any(t => t.IsYellow))
                Nudge(Onboarding.YellowCrop, Ada.Portrait, "That one's thirsty! Water it today.",
                    new[] { new BubbleChip("crop_carrot_yellow", ""), new BubbleChip("tool_watering_can", "") });
            if (tiles.Any(t => t.IsReady) && Allowed)
                _ob.MarkShown(Onboarding.CarrotsReady); // ready crops sparkle; the board lights the card
        }
        if (_lostToday)
        {
            _lostToday = false;
            Nudge(Onboarding.LostCrop, "", "Dry too long. Water every day.",
                new[] { new BubbleChip("soil_tilled", ""), new BubbleChip("tool_watering_can", "") });
        }
        Evaluate();
    }

    private void OnCollected(string item, int count)
    {
        foreach (var m in _morningMarkers) m.QueueFree();
        _morningMarkers.Clear();
        if (!Allowed || _ob.WasShown(Onboarding.FirstProducts)) return;
        _ob.MarkShown(Onboarding.FirstProducts);
        // "Egg and milk orders appear on the board."
        var om = OrderManager.Instance;
        if (om != null && om.BoardOpen && !om.BoardOrders.Any(o => o.ItemId == item))
            om.AddDailyOrder(item, _clock?.Day ?? 1);
    }

    private void AddMorningMarker(AnimalHouse h, string item)
    {
        if (h == null || h.GetParent() is not Building b) return;
        // A big product icon pinned above the building's roof; freed with the level.
        var m = new Node3D { Name = "MorningMarker", Position = new Vector3(b.Size.X / 2, 0, b.Size.Y / 2) };
        var pin = new WorldPin { WorldOffset = new Vector3(0, Building.WallHeight + 2.0f, 0) };
        pin.AddChild(new TextureRect
        {
            Texture = SpriteLibrary.Get(ItemIds.SpriteName(item)), Size = new Vector2(48, 48), Position = new Vector2(-24, -48),
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize, StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
            MouseFilter = Control.MouseFilterEnum.Ignore,
        });
        m.AddChild(pin);
        b.AddChild(m);
        _morningMarkers.Add(m);
    }

    // ---- Nudges ------------------------------------------------------------------------------
    private static bool Allowed => GameSettings.Nudges;

    private void Nudge(string id, string portrait, string line, BubbleChip[] chips)
    {
        if (!Allowed || _ob.WasShown(id) || _pending.Any(p => p.Id == id)) return;
        _pending.Enqueue((id, portrait, line, chips));
        ShowPending();
    }

    private void ShowPending()
    {
        if (_pending.Count == 0) return;
        var bubble = PictureBubbleView.Find(GetTree());
        if (bubble == null || bubble.IsOpen || _clock == null || !_clock.IsPlayable) return;
        var (id, portrait, line, chips) = _pending.Peek();
        if (!bubble.Show(portrait, line, chips, new[] { BubbleButton.Close() })) return;
        _pending.Dequeue();
        _ob.MarkShown(id);
    }

    // ---- Steps -------------------------------------------------------------------------------
    /// <summary>Checks the current step (and any already-done ones after it), the feed-low nudge,
    /// queued bubbles and the guide arrow.</summary>
    public void Evaluate()
    {
        if (_ob == null) return;
        for (int guard = 0; guard < 12 && !_ob.IsDone && StepDone(_ob.Step); guard++)
            _ob.Advance();
        CheckFeedLow();
        ShowPending();
        UpdateGuide();
    }

    private bool StepDone(OnboardingStep s)
    {
        var om = OrderManager.Instance;
        var first = om?.Get(OrderManager.FirstOrderId);
        bool firstPaid = first?.State == OrderState.Paid;
        switch (s)
        {
            case OnboardingStep.Arrival:
                return _player == null || _house == null || _player.Feet.DistanceTo(_house.DoorOutside) < 1.72f
                       || _house.Zone?.PlayerInside == true;
            case OnboardingStep.Garden: return _ob.WeedsPulled >= 3;
            case OnboardingStep.Plant: return _ob.Planted >= 3;
            case OnboardingStep.Water: return _ob.Watered >= 3;
            case OnboardingStep.MeetMarlow: return first == null || first.State != OrderState.Offered;
            case OnboardingStep.Forage: return firstPaid || (Inventory.Instance?.CountAll(ItemIds.Berries) ?? 0) >= OrderManager.FirstOrderCount;
            case OnboardingStep.FirstOrder: return firstPaid;
            case OnboardingStep.SecondOrder: return om?.BoardOpen ?? true;
            case OnboardingStep.FeedAnimals: return FedOnce();
            case OnboardingStep.FreePlay:
                return FeedStore.Pending.Count > 0 || (_clock?.Day ?? 1) > 7;
            default: return false;
        }
    }

    /// <summary>Every animal in the coop and barn is fed today.</summary>
    private bool FedOnce()
    {
        var houses = new[] { _hens, _cows }.Where(h => h != null && h.Animals.Count > 0).ToList();
        return houses.Count == 0 || houses.All(h => h.Animals.All(a => a.FedToday));
    }

    private void CheckFeedLow()
    {
        foreach (var h in new[] { _hens, _cows }.Where(h => h != null && h.Animals.Count > 0))
        {
            if (h.Feed + FeedStore.PendingAmount(h.FeedItem) > h.Animals.Count * 2) continue; // more than 2 days left
            Nudge(Onboarding.FeedLow, Ada.Portrait, "We're nearly out of feed. Call the feed seller with the phone. Before 6 in the evening!",
                new[] { new BubbleChip(ItemIds.SpriteName(h.FeedItem), "×" + h.Feed), new BubbleChip("phone", ""), new BubbleChip("icon_clock", "18:00") });
            return;
        }
    }

    /// <summary>Where the guide arrow points for the current step (null = hidden).</summary>
    public Vector3? GuideTarget()
    {
        switch (_ob?.Step)
        {
            case OnboardingStep.Arrival: return _house?.DoorOutside;
            case OnboardingStep.Garden: return NearestTile(t => t.Soil == Soil.Weeds);
            case OnboardingStep.Plant: return NearestTile(t => t.Soil == Soil.Tilled && !t.HasCrop);
            case OnboardingStep.Water: return NearestTile(t => t.HasCrop && !t.Watered);
            case OnboardingStep.MeetMarlow:
            case OnboardingStep.FirstOrder: return _marlow?.GlobalPosition;
            case OnboardingStep.Forage:
                var bush = _bushes.Where(b => b.HasBerries).OrderBy(b => _player != null ? b.GlobalPosition.DistanceTo(_player.GlobalPosition) : 0).FirstOrDefault();
                return bush?.GlobalPosition;
            case OnboardingStep.SecondOrder: return _board?.GlobalPosition;
            case OnboardingStep.FeedAnimals:
                var h = new[] { _hens, _cows }.FirstOrDefault(x => x != null && x.Animals.Any(a => !a.FedToday));
                var slot = h?.Slots.FirstOrDefault(sl => sl.WantsFood);
                return slot?.GlobalPosition ?? h?.Bin?.GlobalPosition; // the first empty trough slot
            default: return null;
        }
    }

    private Vector3? NearestTile(System.Func<CropTile, bool> pick)
    {
        if (_garden == null) return null;
        // Near the starting beds first (the guided start happens by the path).
        var bed = new Vector2I(6, 3);
        var best = _garden.AllTiles().Where(t => pick(t.Tile)).OrderBy(t => (t.Cell - bed).LengthSquared()).FirstOrDefault();
        return best.Tile == null ? null : _garden.CellCenter(best.Cell);
    }

    private void UpdateGuide()
    {
        var target = GuideTarget();
        if (_arrow != null) _arrow.Point(target);
        NudgeView.Find(GetTree())?.ShowStep(_ob.Step, _ob);
        _ada?.SetWaving(_ob.Step == OnboardingStep.Arrival);
    }
}
