using Godot;

namespace InfernoClimb.effects;

public partial class Corpse : RigidBody2D {
    [Export] public float MaxInitialAngularVelocity = 50;
    // [Export] public float MinInitialSpeed = 100;
    // [Export] public float MaxInitialSpeed = 290;
    
    private void _OnParentDied() {
        using RandomNumberGenerator rng = new RandomNumberGenerator();
        CharacterBody2D body = GetParent<CharacterBody2D>();
        ProcessMode = ProcessModeEnum.Inherit;
        Visible = true;
        
        Reparent(GetParent().GetParent());
        AngularVelocity = rng.RandfRange(-MaxInitialAngularVelocity, MaxInitialAngularVelocity);
        LinearVelocity = body.Velocity;
        // TODO: remove?
        // Vector2.FromAngle(Mathf.Pi * 2 * rng.Randf()).Normalized() * rng.RandfRange(MinInitialSpeed, MaxInitialSpeed);
    }
}