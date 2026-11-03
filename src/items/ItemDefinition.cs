using Godot;

namespace InfernoClimb.items;

[GlobalClass]
public partial class ItemDefinition : Resource {
    [Export] public string Name;
    [Export] public string Description;
    [Export] public Texture2D Icon;
    [Export] public PackedScene Scene;
}