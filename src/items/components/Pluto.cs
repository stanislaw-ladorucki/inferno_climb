using Godot;
using InfernoClimb.entities.attacks;

namespace InfernoClimb.items.components;

public partial class Pluto : ItemComponent {
    [Export] public float RotationSpeed = 0.25f;
    [Export] public float StackingScaleMultiplier = 1.2f;
    [Export] public float StackingDamageMultiplier = 1.2f;

    public override void _Process(double delta) {
        Rotation += RotationSpeed * (float)delta;
    }
    
    public override void AddStack() {
        base.AddStack();
        Attack attack = GetNode<Attack>("Attack");
        attack.Scale *= StackingScaleMultiplier;
        attack.Damage *= StackingDamageMultiplier;
    }
}
