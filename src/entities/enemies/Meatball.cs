using Godot;
using Math = System.Math;

namespace InfernoClimb.entities.enemies;

public partial class Meatball : BaseEnemy {
    [Export] public float MinInitialSpeed = 60;
    [Export] public float MaxInitialSpeed = 290;
    [Export] public float VelocityDampening = 0.0001f;
    
    public override void _Ready() {
        IsInvulnerable = true;
        base._Ready();
        
        using RandomNumberGenerator rng = new RandomNumberGenerator();
        Velocity = Vector2.FromAngle(Mathf.Pi * 2 * rng.Randf()).Normalized() * rng.RandfRange(MinInitialSpeed, MaxInitialSpeed);
    }

    public override void _PhysicsProcess(double delta) {
        Velocity = Velocity.Lerp(Vector2.Zero, 1 - (float)Math.Pow(VelocityDampening, delta));
        MoveAndSlide();
    }

    private void _onInvulnerabilityTimerTimeout() {
        IsInvulnerable = false;
    }
}
