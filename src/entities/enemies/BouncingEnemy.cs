using Godot;
using InfernoClimb.extensions;

namespace InfernoClimb.entities.enemies;

public partial class BouncingEnemy : BaseEnemy {
    [Export] public float Speed = 150;
    
    public override void _Ready() {
        RandomNumberGenerator rng = new RandomNumberGenerator();
        Velocity = new Vector2(
            rng.Randf() < 0.5 ? -1 : 1,
            rng.Randf() < 0.5 ? -1 : 1
        ) * Speed;
    }

    public override void _PhysicsProcess(double delta) {
        if (GetTree().Root.GetCamera2D().IsInBounds(GlobalPosition, new Vector2(0, 200))) {
            KinematicCollision2D collision = MoveAndCollide(Velocity * (float)delta);
            if (collision != null) {
                Velocity = Velocity.Bounce(collision.GetNormal());
            }
        }
    }
}