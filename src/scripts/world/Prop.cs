using Godot;
using MyFarmingGame.Autoloads;
using MyFarmingGame.Core;
using MyFarmingGame.Player;
using MyFarmingGame.UI;

namespace MyFarmingGame.World;

/// <summary>
/// Base for things Kit walks up to and uses (Task 1.6): Marlow, berry bushes, the ledger and,
/// later, the order board, shop counter, phone, feed bins and animals. Builds its own sprite
/// (<see cref="SpriteName"/>), solid body, click area (layer 8), and a button prompt above it from
/// the exports, so a prop scene is just a root node with this script's subclass.
/// <c>interact</c> (A / Space) uses it when Kit's feet are within <see cref="Reach"/> of
/// <see cref="ApproachPoint"/>; a left-click walks Kit to <see cref="ApproachPoint"/>, then uses it.
/// </summary>
public abstract partial class Prop : StaticBody2D, IInteractable
{
    [Export] public string SpriteName { get; set; } = "";
    [Export] public Vector2 DisplaySize { get; set; } = new(64, 64);
    /// <summary>Solid footprint (zero = Kit walks through).</summary>
    [Export] public Vector2 BodySize { get; set; } = new(48, 24);
    [Export] public Vector2 BodyOffset { get; set; } = new(0, 16);
    [Export] public Vector2 ClickSize { get; set; } = new(64, 64);
    /// <summary>Where Kit stands to use it, relative to the prop.</summary>
    [Export] public Vector2 ApproachOffset { get; set; } = new(0, 56);
    [Export] public float Reach { get; set; } = 72f;
    /// <summary>Icon next to the button prompt (e.g. icon_talk).</summary>
    [Export] public string PromptSprite { get; set; } = "";

    public NamedSprite Sprite { get; private set; }
    private Node2D _prompt;

    public Vector2 ApproachPoint => GlobalPosition + ApproachOffset;

    public bool PromptVisible => _prompt?.Visible ?? false;

    /// <summary>False while the prop can't be used right now (e.g. a bush with no berries).</summary>
    public virtual bool Usable => true;

    public bool CanInteract => Usable && (GameClock.Instance?.IsPlayable ?? true);

    public abstract void Interact(Node2D player);

    public bool IsInReach(Node2D player) =>
        player != null && (player.GlobalPosition + PlayerController.FeetOffset).DistanceTo(ApproachPoint + PlayerController.FeetOffset) <= Reach;

    public override void _Ready()
    {
        AddToGroup(IInteractable.Group);
        ZIndex = -1;
        if (!string.IsNullOrEmpty(SpriteName))
        {
            Sprite = new NamedSprite { Name = "Sprite", SpriteName = SpriteName, DisplaySize = DisplaySize };
            AddChild(Sprite);
        }
        if (BodySize.X > 0 && BodySize.Y > 0)
            AddChild(new CollisionShape2D { Shape = new RectangleShape2D { Size = BodySize }, Position = BodyOffset });
        else
            CollisionLayer = 0;
        var click = new Area2D { Name = "ClickArea", CollisionLayer = IInteractable.ClickLayer, CollisionMask = 0, Monitoring = false };
        click.AddChild(new CollisionShape2D { Shape = new RectangleShape2D { Size = ClickSize } });
        AddChild(click);
        _prompt = BuildPrompt(PromptSprite, -DisplaySize.Y / 2 - 26);
        AddChild(_prompt);
    }

    /// <summary>A small "[button] icon" bubble floating above a prop.</summary>
    public static Node2D BuildPrompt(string icon, float y)
    {
        var root = new Node2D { Name = "Prompt", ZIndex = 20, Position = new Vector2(0, y), Visible = false };
        var panel = new PanelContainer { MouseFilter = Control.MouseFilterEnum.Ignore };
        panel.AddThemeStyleboxOverride("panel", new StyleBoxFlat
        {
            BgColor = new Color(1, 0.98f, 0.92f, 0.92f), BorderColor = new Color(0.15f, 0.12f, 0.1f),
            BorderWidthLeft = 2, BorderWidthTop = 2, BorderWidthRight = 2, BorderWidthBottom = 2,
            CornerRadiusTopLeft = 10, CornerRadiusTopRight = 10, CornerRadiusBottomLeft = 10, CornerRadiusBottomRight = 10,
            ContentMarginLeft = 6, ContentMarginRight = 6, ContentMarginTop = 4, ContentMarginBottom = 4,
        });
        var row = new HBoxContainer { MouseFilter = Control.MouseFilterEnum.Ignore, Alignment = BoxContainer.AlignmentMode.Center };
        row.AddChild(new ButtonPrompt { Action = "interact", CustomMinimumSize = new Vector2(30, 30), MouseFilter = Control.MouseFilterEnum.Ignore });
        if (!string.IsNullOrEmpty(icon))
            row.AddChild(new TextureRect
            {
                Texture = SpriteLibrary.Get(icon), CustomMinimumSize = new Vector2(30, 30),
                ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize, StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
                MouseFilter = Control.MouseFilterEnum.Ignore,
            });
        panel.AddChild(row);
        root.AddChild(panel);
        panel.Position = new Vector2(-40, -22);
        return root;
    }

    public override void _Process(double delta)
    {
        if (_prompt == null) return;
        var p = GetTree().GetFirstNodeInGroup(PlayerController.Group) as PlayerController;
        _prompt.Visible = p != null && !p.Frozen && CanInteract && IsInReach(p);
    }
}
