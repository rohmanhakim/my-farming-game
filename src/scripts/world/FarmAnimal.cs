using Godot;
using MyFarmingGame.Autoloads;
using MyFarmingGame.Core;
using MyFarmingGame.UI;

namespace MyFarmingGame.World;

/// <summary>
/// A hen or the cow standing in its building (Task 1.9). Shows a heart bar (full / ⅔ / ⅓, no
/// numbers), a need icon that bounces after a missed day and flashes on the second, and its product
/// when ready (egg / milk can). <c>interact</c>: collect the product if there is one; with the feed
/// scoop, feed it from the building's bin; otherwise show its name and hearts. Kit walks through it.
/// </summary>
public abstract partial class FarmAnimal : Prop
{
    public AnimalData Data { get; set; }
    public AnimalHouse House { get; set; }

    private TextureRect _need;
    private TextureRect _product;
    private float _t;

    protected FarmAnimal()
    {
        BodySize = Vector2.Zero;
        ApproachOffset = new Vector2(0, 44);
        Reach = 48f;
        PromptSprite = "icon_talk";
    }

    public override void _Ready()
    {
        base._Ready();
        _need = MakeIcon(House != null ? ItemIds.SpriteName(House.FeedItem) : "", new Vector2(-14, -DisplaySize.Y / 2 - 44), 28);
        _product = MakeIcon(House != null ? ItemIds.SpriteName(House.ProductItem) : "", new Vector2(DisplaySize.X / 2 - 8, -6), 26);
        if (House != null) House.AnimalsChanged += QueueRedraw;
    }

    public override void _ExitTree()
    {
        if (House != null && IsInstanceValid(House)) House.AnimalsChanged -= QueueRedraw;
    }

    private TextureRect MakeIcon(string sprite, Vector2 pos, int size)
    {
        var r = new TextureRect
        {
            Texture = string.IsNullOrEmpty(sprite) ? null : SpriteLibrary.Get(sprite), Position = pos, Size = new Vector2(size, size),
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize, StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
            MouseFilter = Control.MouseFilterEnum.Ignore, Visible = false,
        };
        AddChild(r);
        return r;
    }

    public bool NeedVisible => _need?.Visible ?? false;
    public bool ProductVisible => _product?.Visible ?? false;

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (Data == null) return;
        _t += (float)delta;
        _need.Visible = Data.ShowsNeed;
        _need.Position = new Vector2(_need.Position.X, -DisplaySize.Y / 2 - 44 + Mathf.Sin(_t * 5) * 3);
        _need.Modulate = Data.NeedFlashes && Mathf.PosMod(_t, 0.8f) < 0.4f ? new Color(1, 0.5f, 0.5f) : Colors.White;
        _product.Visible = Data.ProductReady > 0;
    }

    public override void _Draw()
    {
        if (Data == null) return;
        // Heart bar: three hearts above the animal, filled for each third (no numbers).
        float y = -DisplaySize.Y / 2 - 12;
        for (int i = 0; i < 3; i++)
            DrawHeart(new Vector2(-16 + i * 16, y), i < Data.Hearts);
    }

    private void DrawHeart(Vector2 c, bool full)
    {
        var col = full ? new Color(0.92f, 0.25f, 0.35f) : new Color(1, 1, 1, 0.35f);
        DrawCircle(c + new Vector2(-3, -2), 3.5f, col);
        DrawCircle(c + new Vector2(3, -2), 3.5f, col);
        DrawColoredPolygon(new[] { c + new Vector2(-6.5f, -1), c + new Vector2(6.5f, -1), c + new Vector2(0, 7) }, col);
    }

    public override void Interact(Node2D player)
    {
        if (Data == null || House == null) return;
        if (Data.ProductReady > 0) { House.Collect(Data); QueueRedraw(); return; }
        if (ToolBelt.Instance?.Active == Tool.FeedScoop)
        {
            if (!House.FeedOne(Data) && !Data.FedToday)
                FeedBin.ShowEmpty(GetTree(), House);
            QueueRedraw();
            return;
        }
        PictureBubbleView.Find(GetTree())?.Show("", Data.Name, new[] { new BubbleChip(SpriteName, new string('♥', Data.Hearts)) },
            new[] { BubbleButton.Close() });
    }
}
