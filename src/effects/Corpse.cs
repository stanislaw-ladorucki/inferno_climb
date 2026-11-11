using Godot;

namespace InfernoClimb.effects;

public partial class Corpse : RigidBody2D {
    [Export] public float MaxInitialAngularVelocity = 50;
    
    private void _OnParentDied() {
        using RandomNumberGenerator rng = new RandomNumberGenerator();
        ProcessMode = ProcessModeEnum.Inherit;
        Visible = true;
        if (GetParent() is CharacterBody2D body) {
            LinearVelocity = body.Velocity;
        }
        Reparent(GetParent().GetParent());
        AngularVelocity = rng.RandfRange(-MaxInitialAngularVelocity, MaxInitialAngularVelocity);
    }

    public void _OnTimeout() {
        QueueFree();
    }
}