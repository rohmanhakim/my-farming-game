using Godot;

namespace MyFarmingGame.Core;

/// <summary>
/// A Node3D whose look comes from <see cref="ModelLibrary"/> by name (Task 1.14): a real model
/// from res://assets/models/ when one exists, otherwise the placeholder primitives. Dropping a real
/// model in with the same name needs no scene edit. The 3D version of the 2D build's NamedSprite (removed in Task 1.15).
/// </summary>
[GlobalClass]
public partial class NamedModel : Node3D
{
    private string _modelName = "";

    [Export]
    public string ModelName
    {
        get => _modelName;
        set { _modelName = value; if (IsInsideTree()) Rebuild(); }
    }

    /// <summary>Where the current look came from.</summary>
    public ModelLibrary.Source Source { get; private set; } = ModelLibrary.Source.Fallback;

    /// <summary>The built model (child named "Model").</summary>
    public Node3D Model { get; private set; }

    public override void _Ready() => Rebuild();

    /// <summary>Throws away the current look and builds it again (name changed, or a model was added).</summary>
    public void Rebuild()
    {
        if (Model != null && IsInstanceValid(Model))
        {
            RemoveChild(Model);
            Model.QueueFree();
        }
        Model = ModelLibrary.Instantiate(_modelName, out var source);
        Source = source;
        AddChild(Model);
    }
}
