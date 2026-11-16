using Godot;
using InfernoClimb.entities.attacks;

namespace InfernoClimb.items.components;

public partial class Pluto : ItemComponent {
    [Export] public float RotationSpeed = 0.25f;
    [Export] public float StackingScaleMultiplier = 1.2f;
    [Export] public float StackingOrbitDistanceIncrease = 20f;
    [Export] public float StackingDamageIncrease = 2f;

    public override void _Process(double delta) {
        Rotation += RotationSpeed * (float)delta;
        Attack attack = GetNode<Attack>("Attack");
        attack.KnockbackVector = attack.KnockbackVector.Rotated(Rotation);
        GetNode<Sprite2D>("Attack/Sprite2D").GlobalRotation = 0.0f;
    }
    
    public override void AddStack() {
        base.AddStack();
        if (Stack > 1) {
            Attack attack = GetNode<Attack>("Attack");
            StaticBody2D block = GetNode<StaticBody2D>("ProjectileBlockBody");
            attack.Damage += StackingDamageIncrease;
            attack.Scale *= StackingScaleMultiplier;
            block.Scale *= StackingScaleMultiplier;
            attack.Position += Vector2.Right * StackingOrbitDistanceIncrease;
            block.Position += Vector2.Right * StackingOrbitDistanceIncrease;
        }
    }
}
