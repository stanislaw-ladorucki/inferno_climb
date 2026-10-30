using Godot;
using InfernoClimb.extensions;
using Vector2 = Godot.Vector2;

namespace InfernoClimb.entities.enemies;

public partial class FloatingEnemy : BaseEnemy {
	[Export] public float Speed = 50;
	[Export] public float MoveChance = 0.45f;
	
	public override void _PhysicsProcess(double delta) {
		if (GetTree().Root.GetCamera2D().IsInBounds(GlobalPosition, new Vector2(0, 200))) {
			using RandomNumberGenerator rng = new RandomNumberGenerator();
			if (rng.Randf() < MoveChance) {
				Velocity = Vector2.Right.Rotated(rng.Randf() * Mathf.Pi * 2) * Speed;
			}
			MoveAndSlide();
		}
	}
}
