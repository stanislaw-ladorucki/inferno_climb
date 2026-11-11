using Godot;
using InfernoClimb.entities.attacks;

namespace InfernoClimb.items.components;

public partial class Pluto : ItemComponent {
    [Export] public float RotationSpeed = 0.25f;
    [Export] public float StackingScaleMultiplier = 1.2f;
    [Export] public float StackingDamageMultiplier = 1.2f;

    public override void _Process(double delta) {
        Rotation += RotationSpeed * (float)delta;
        Attack attack = GetNode<Attack>("Attack");
        attack.KnockbackVector = attack.KnockbackVector.Rotated(Rotation);
        GetNode<Sprite2D>("Attack/Sprite2D").GlobalRotation = 0.0f;
    }
    
    public override void AddStack() {
        base.AddStack();
        Attack attack = GetNode<Attack>("Attack");
        attack.Scale *= StackingScaleMultiplier;
        attack.Damage *= StackingDamageMultiplier;
    }
}
