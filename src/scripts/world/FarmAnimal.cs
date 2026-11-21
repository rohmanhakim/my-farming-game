using Godot;
using MyFarmingGame.Autoloads;
using MyFarmingGame.Core;
using MyFarmingGame.UI;

namespace MyFarmingGame.World;

/// <summary>
/// A hen or the cow in its building (Task 1.9; 3D since Task 1.18). Shows a heart bar (full / ⅔ /
/// ⅓, no numbers), a need icon that bounces after a missed day and flashes on the second, and its
/// product when ready (egg / milk can) — all pinned above it as screen UI (<see cref="WorldPin"/>).
/// It ambles gently around its spot (no physics body; Kit walks through it).
/// <c>interact</c>: collect the product if there is one; with the feed scoop, feed it from the
/// building's bin; otherwise show its name and hearts.
/// </summary>
public abstract partial class FarmAnimal : Prop
{
    public AnimalData Data { get; set; }
    public AnimalHouse House { get; set; }

    /// <summary>How far it wanders from its spot (metres).</summary>
    [Export] public float WanderRadius { get; set; } = 0.3f;

    private WorldPin _hearts;
    private WorldPin _need;
    private WorldPin _product;
    private TextureRect _needIcon;
    private HeartBar _bar;
    private float _t;
    private Vector3 _home;
    private Vector3 _goal;
    private float _rest;
    private readonly RandomNumberGenerator _rng = new();

    protected FarmAnimal()
    {
        BodySize = Vector3.Zero;
        ApproachOffset = new Vector3(0, 0, 0.69f);
        Reach = 0.75f;
        PromptSprite = "icon_talk";
    }

    public override void _Ready()
    {
        base._Ready();
        _home = Position;
        _goal = _home;
        _rng.Seed = (ulong)(Data?.Name ?? Name).GetHashCode();
        _bar = new HeartBar { Data = Data };
        _hearts = Pin(_bar, PromptHeight - 0.15f, new Vector2(-24, -8));
        _needIcon = Icon(House != null ? ItemIds.SpriteName(House.FeedItem) : "", 28);
        _need = Pin(_needIcon, PromptHeight + 0.1f, new Vector2(-14, -52));
        _product = Pin(Icon(House != null ? ItemIds.SpriteName(House.ProductItem) : "", 26), PromptHeight * 0.5f, new Vector2(18, -13));
    }

    private WorldPin Pin(Control content, float height, Vector2 screen)
    {
        var pin = new WorldPin { WorldOffset = new Vector3(0, height, 0), Shown = false };
        content.Position = screen;
        pin.AddChild(content);
        AddChild(pin);
        return pin;
    }

    private static TextureRect Icon(string sprite, int size) => new()
    {
        Texture = string.IsNullOrEmpty(sprite) ? null : SpriteLibrary.Get(sprite), Size = new Vector2(size, size),
        ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize, StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
        MouseFilter = Control.MouseFilterEnum.Ignore,
    };

    public bool NeedVisible => _need != null && _need.Shown;
    public bool ProductVisible => _product != null && _product.Shown;
    public bool HeartsVisible => _hearts != null && _hearts.Shown;

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (Data == null) return;
        _t += (float)delta;
        bool seen = !UnderRoof; // hidden under the roof until Kit goes in (as in 2D)
        _hearts.Shown = seen;
        _bar.QueueRedraw();
        _need.Shown = seen && Data.ShowsNeed;
        _need.ScreenOffset = new Vector2(0, Mathf.Sin(_t * 5) * 3);
        _needIcon.Modulate = Data.NeedFlashes && Mathf.PosMod(_t, 0.8f) < 0.4f ? new Color(1, 0.5f, 0.5f) : Colors.White;
        _product.Shown = seen && Data.ProductReady > 0;
        Amble((float)delta);
    }

    /// <summary>Strolls to a random point near its spot, rests, and picks another.</summary>
    private void Amble(float delta)
    {
        if (WanderRadius <= 0) return;
        if (_rest > 0) { _rest -= delta; return; }
        Vector3 to = _goal - Position;
        if (to.Length() < 0.02f)
        {
            _rest = _rng.RandfRange(1.5f, 4f);
            float a = _rng.RandfRange(0, Mathf.Tau), r = _rng.RandfRange(0, WanderRadius);
            _goal = _home + new Vector3(Mathf.Cos(a) * r, 0, Mathf.Sin(a) * r);
            return;
        }
        Vector3 step = to.Normalized() * Mathf.Min(to.Length(), 0.35f * delta);
        Position += step;
        if (Model != null)
            Model.Rotation = new Vector3(0, Mathf.LerpAngle(Model.Rotation.Y, Player.PlayerController.YawFor(step), 0.2f), 0);
    }

    /// <summary>The spot it ambles around (tests).</summary>
    public Vector3 Home => _home;

    public override void Interact(Node3D player)
    {
        if (Data == null || House == null) return;
        if (Data.ProductReady > 0) { House.Collect(Data); return; }
        if (ToolBelt.Instance?.Active == Tool.FeedScoop)
        {
            if (!House.FeedOne(Data) && !Data.FedToday)
                FeedBin.ShowEmpty(GetTree(), House);
            return;
        }
        PictureBubbleView.Find(GetTree())?.Show("", Data.Name, new[] { new BubbleChip(SpriteName, new string('♥', Data.Hearts)) },
            new[] { BubbleButton.Close() });
    }
}

/// <summary>Three hearts, filled for each third (no numbers), drawn as screen UI above an animal.</summary>
public partial class HeartBar : Control
{
    public AnimalData Data { get; set; }

    public HeartBar()
    {
        Size = new Vector2(48, 16);
        MouseFilter = MouseFilterEnum.Ignore;
    }

    public override void _Draw()
    {
        if (Data == null) return;
        for (int i = 0; i < 3; i++)
            DrawHeart(new Vector2(8 + i * 16, 8), i < Data.Hearts);
    }

    private void DrawHeart(Vector2 c, bool full)
    {
        var col = full ? new Color(0.92f, 0.25f, 0.35f) : new Color(1, 1, 1, 0.45f);
        DrawCircle(c + new Vector2(-3, -2), 3.5f, col);
        DrawCircle(c + new Vector2(3, -2), 3.5f, col);
        DrawColoredPolygon(new[] { c + new Vector2(-6.5f, -1), c + new Vector2(6.5f, -1), c + new Vector2(0, 7) }, col);
    }
}
