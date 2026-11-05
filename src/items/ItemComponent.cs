using Godot;
using InfernoClimb.entities;

namespace InfernoClimb.items;

public partial class ItemComponent : Node2D {
    [Export] public Player Player;
    [Export] public int Stack;
    [Export] public ItemDefinition Definition;

    public virtual void AddStack() {}
}