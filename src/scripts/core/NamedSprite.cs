using Godot;

namespace MyFarmingGame.Core;

/// <summary>
/// A Sprite2D whose texture comes from <see cref="SpriteLibrary"/> by name, scaled to a fixed
/// on-screen size. Swapping placeholder art for final art (any resolution) needs no scene edit.
/// </summary>
[Tool]
[GlobalClass]
public partial class NamedSprite : Sprite2D
{
    private string _spriteName = "";
    private Vector2 _displaySize = new(64, 64);

    [Export]
    public string SpriteName
    {
        get => _spriteName;
        set { _spriteName = value; Refresh(); }
    }

    /// <summary>Size in pixels the sprite is drawn at, whatever the source texture size.</summary>
    [Export]
    public Vector2 DisplaySize
    {
        get => _displaySize;
        set { _displaySize = value; Refresh(); }
    }

    public override void _Ready() => Refresh();

    private void Refresh()
    {
        if (string.IsNullOrEmpty(_spriteName))
            return;
        Texture = SpriteLibrary.Get(_spriteName);
        var size = Texture?.GetSize() ?? Vector2.One;
        if (size.X > 0 && size.Y > 0)
            Scale = _displaySize / size;
    }
}
