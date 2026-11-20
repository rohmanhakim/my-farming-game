using Godot;
using MyFarmingGame.Core;

namespace MyFarmingGame.World;

/// <summary>Which wall has the door.</summary>
public enum DoorSide { Top, Bottom }

/// <summary>
/// A building entered on the same map (Task 1.5, Phase-001 §1 Indoors): floor, walls with a door
/// gap, a roof drawn over Kit, and an <see cref="IndoorZone"/> over the floor (roof fades, clock
/// stops while Kit is inside). The node's position is the footprint's top-left corner; size is in
/// 64-px tiles. Things inside (bed, phone, feed bins, animals) are children placed in the scene.
/// Art by sprite name: <see cref="FloorSprite"/>, <see cref="RoofSprite"/>.
/// </summary>
public partial class Building : Node2D
{
    public const int Tile = 64;
    public const float WallThickness = 12f;

    [Export] public Vector2I SizeTiles { get; set; } = new(6, 4);
    [Export] public DoorSide Door { get; set; } = DoorSide.Top;
    [Export] public int DoorTiles { get; set; } = 2;
    [Export] public string ZoneName { get; set; } = "house";
    [Export] public string FloorSprite { get; set; } = "floor_wood";
    [Export] public string RoofSprite { get; set; } = "roof_house";
    [Export] public Color WallColor { get; set; } = new(0.45f, 0.32f, 0.22f);

    public IndoorZone Zone { get; private set; }
    public TextureRect Roof { get; private set; }

    public Vector2 SizePx => new(SizeTiles.X * Tile, SizeTiles.Y * Tile);

    /// <summary>A point just outside the door (world coordinates).</summary>
    public Vector2 DoorOutside => GlobalPosition + new Vector2(SizePx.X / 2, Door == DoorSide.Top ? -40 : SizePx.Y + 40);

    /// <summary>A point just inside the door (world coordinates).</summary>
    public Vector2 DoorInside => GlobalPosition + new Vector2(SizePx.X / 2, Door == DoorSide.Top ? 48 : SizePx.Y - 64);

    /// <summary>The floor inside the walls (world coordinates).</summary>
    public Rect2 Interior => new(GlobalPosition + new Vector2(WallThickness, WallThickness), SizePx - new Vector2(2 * WallThickness, 2 * WallThickness));

    public override void _Ready()
    {
        Vector2 size = SizePx;

        // Floor (texture tiled at 64 px: 32-px placeholder × 2).
        var floor = new TextureRect
        {
            Name = "Floor",
            Texture = SpriteLibrary.Get(FloorSprite),
            StretchMode = TextureRect.StretchModeEnum.Tile,
            Size = size / 2,
            Scale = new Vector2(2, 2),
            MouseFilter = Control.MouseFilterEnum.Ignore,
            ZIndex = -8,
            TextureRepeat = TextureRepeatEnum.Enabled,
        };
        AddChild(floor);

        // Walls (drawn and solid), with a door gap.
        var walls = new StaticBody2D { Name = "Walls" };
        AddChild(walls);
        float gapW = DoorTiles * Tile;
        float gapX = (size.X - gapW) / 2;
        float t = WallThickness;
        float topY = 0, bottomY = size.Y - t;
        AddWall(walls, new Rect2(0, 0, t, size.Y));                // left
        AddWall(walls, new Rect2(size.X - t, 0, t, size.Y));       // right
        foreach (var (y, hasDoor) in new[] { (topY, Door == DoorSide.Top), (bottomY, Door == DoorSide.Bottom) })
        {
            if (hasDoor)
            {
                AddWall(walls, new Rect2(0, y, gapX, t));
                AddWall(walls, new Rect2(gapX + gapW, y, size.X - gapX - gapW, t));
            }
            else
                AddWall(walls, new Rect2(0, y, size.X, t));
        }

        // Roof over everything (and over Kit) until Kit walks in.
        Roof = new TextureRect
        {
            Name = "Roof",
            Texture = SpriteLibrary.Get(RoofSprite),
            StretchMode = TextureRect.StretchModeEnum.Scale,
            Position = new Vector2(-8, -16),
            Size = size + new Vector2(16, 20),
            MouseFilter = Control.MouseFilterEnum.Ignore,
            ZIndex = 30,
        };
        AddChild(Roof);

        // Inside = the floor between the walls.
        Zone = new IndoorZone { Name = "IndoorZone", ZoneName = ZoneName, Roof = Roof };
        var shape = new CollisionShape2D
        {
            Shape = new RectangleShape2D { Size = size - new Vector2(2 * t, 2 * t) },
            Position = size / 2,
        };
        Zone.AddChild(shape);
        AddChild(Zone);
    }

    private void AddWall(StaticBody2D body, Rect2 r)
    {
        if (r.Size.X <= 0 || r.Size.Y <= 0) return;
        body.AddChild(new CollisionShape2D { Shape = new RectangleShape2D { Size = r.Size }, Position = r.GetCenter() });
        AddChild(new ColorRect { Position = r.Position, Size = r.Size, Color = WallColor, MouseFilter = Control.MouseFilterEnum.Ignore, ZIndex = -7 });
    }
}
