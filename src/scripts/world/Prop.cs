using Godot;
using MyFarmingGame.Autoloads;
using MyFarmingGame.Core;
using MyFarmingGame.Player;
using MyFarmingGame.UI;

namespace MyFarmingGame.World;

/// <summary>
/// Base for things Kit walks up to and uses (Task 1.6; 3D since Task 1.17): the bed, Marlow, Ada,
/// berry bushes, the ledger, the order board, shop counter, phone, feed bins and animals. Builds its
/// own look (<see cref="ModelName"/>, a <see cref="NamedModel"/>), solid body, click area (layer 8,
/// Area3D) and a button prompt pinned above it (<see cref="WorldPin"/>) from the exports, so a prop
/// scene is just a root node with this script's subclass. Sizes are in metres; the node's origin is
/// the prop's spot on the ground. <see cref="SpriteName"/> stays the 2D picture used in bubbles.
/// <c>interact</c> (A / Space) uses it when Kit's feet are within <see cref="Reach"/> of
/// <see cref="ApproachPoint"/>; a left-click walks Kit to <see cref="ApproachPoint"/>, then uses it.
/// </summary>
public abstract partial class Prop : StaticBody3D, IInteractable
{
    /// <summary>The 3D look by name (ModelLibrary).</summary>
    [Export] public string ModelName { get; set; } = "";
    /// <summary>The 2D picture of this prop in bubbles and cards (SpriteLibrary).</summary>
    [Export] public string SpriteName { get; set; } = "";
    /// <summary>Solid footprint (x, height, z); zero = Kit walks through.</summary>
    [Export] public Vector3 BodySize { get; set; } = new(0.75f, 1f, 0.4f);
    [Export] public Vector3 BodyOffset { get; set; } = Vector3.Zero;
    /// <summary>The clickable box (centred above the origin).</summary>
    [Export] public Vector3 ClickSize { get; set; } = new(1f, 1.4f, 1f);
    /// <summary>Where Kit stands to use it, relative to the prop (on the ground).</summary>
    [Export] public Vector3 ApproachOffset { get; set; } = new(0, 0, 0.875f);
    /// <summary>How close Kit's feet must be to <see cref="ApproachPoint"/> (2D: 72 px).</summary>
    [Export] public float Reach { get; set; } = 1.125f;
    /// <summary>Icon next to the button prompt (e.g. icon_talk).</summary>
    [Export] public string PromptSprite { get; set; } = "";
    /// <summary>How high above the origin the prompt floats (metres).</summary>
    [Export] public float PromptHeight { get; set; } = 1.6f;
    /// <summary>False for small props whose shadow is not worth a shadow-pass draw (hens).</summary>
    [Export] public bool CastsShadow { get; set; } = true;

    public NamedModel Model { get; private set; }
    private WorldPin _prompt;

    public Vector3 ApproachPoint => new Vector3(GlobalPosition.X, 0, GlobalPosition.Z) + ApproachOffset;

    public bool PromptVisible => _prompt != null && _prompt.Shown && _prompt.Visible;

    /// <summary>False while the prop can't be used right now (e.g. a bush with no berries).</summary>
    public virtual bool Usable => true;

    public bool CanInteract => Usable && (GameClock.Instance?.IsPlayable ?? true);

    public abstract void Interact(Node3D player);

    public bool IsInReach(Node3D player)
    {
        if (player == null) return false;
        var feet = new Vector3(player.GlobalPosition.X, 0, player.GlobalPosition.Z);
        return feet.DistanceTo(ApproachPoint) <= Reach;
    }

    public override void _Ready()
    {
        AddToGroup(IInteractable.Group);
        if (!string.IsNullOrEmpty(ModelName))
        {
            Model = new NamedModel { Name = "Model", ModelName = ModelName };
            AddChild(Model);
            if (!CastsShadow)
                foreach (var g in Model.FindChildren("*", "GeometryInstance3D", true, false))
                    ((GeometryInstance3D)g).CastShadow = GeometryInstance3D.ShadowCastingSetting.Off;
        }
        if (BodySize.X > 0 && BodySize.Z > 0)
            AddChild(new CollisionShape3D
            {
                Shape = new BoxShape3D { Size = BodySize },
                Position = BodyOffset + new Vector3(0, BodySize.Y / 2f, 0),
            });
        else
            CollisionLayer = 0;
        var click = new Area3D { Name = "ClickArea", CollisionLayer = IInteractable.ClickLayer, CollisionMask = 0, Monitoring = false };
        click.AddChild(new CollisionShape3D { Shape = new BoxShape3D { Size = ClickSize }, Position = new Vector3(0, ClickSize.Y / 2f, 0) });
        AddChild(click);
        _prompt = BuildPrompt(PromptSprite, PromptHeight);
        AddChild(_prompt);
    }

    /// <summary>A small "[button] icon" bubble pinned <paramref name="height"/> metres above its parent.</summary>
    public static WorldPin BuildPrompt(string icon, float height)
    {
        var pin = new WorldPin { Name = "Prompt", WorldOffset = new Vector3(0, height, 0), Shown = false };
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
                Name = "PromptIcon", Texture = SpriteLibrary.Get(icon), CustomMinimumSize = new Vector2(30, 30),
                ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize, StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
                MouseFilter = Control.MouseFilterEnum.Ignore,
            });
        panel.AddChild(row);
        pin.AddChild(panel);
        panel.Position = new Vector2(-40, -44);
        return pin;
    }

    /// <summary>True while this prop stands in a building whose roof is on (Kit outside): its
    /// pinned UI hides, like the 2D build's roof covered it.</summary>
    public bool UnderRoof
    {
        get
        {
            for (Node n = GetParent(); n != null; n = n.GetParent())
                if (n is Building b) return !b.RoofFaded;
            return false;
        }
    }

    public override void _Process(double delta)
    {
        if (_prompt == null) return;
        var p = GetTree().GetFirstNodeInGroup(PlayerController.Group) as PlayerController;
        _prompt.Shown = p != null && !p.Frozen && CanInteract && IsInReach(p);
    }
}
