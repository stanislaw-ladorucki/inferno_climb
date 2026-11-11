using Godot;

namespace InfernoClimb.effects;

public partial class Corpse : RigidBody2D {
    [Export] public float MaxInitialAngularVelocity = 50;
    
    private void _OnParentDied() {
        using RandomNumberGenerator rng = new RandomNumberGenerator();
        CharacterBody2D body = GetParent<CharacterBody2D>();
        ProcessMode = ProcessModeEnum.Inherit;
        Visible = true;
        
        Reparent(GetParent().GetParent());
        AngularVelocity = rng.RandfRange(-MaxInitialAngularVelocity, MaxInitialAngularVelocity);
        LinearVelocity = body.Velocity;
    }
}