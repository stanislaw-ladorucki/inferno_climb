using Godot;

namespace InfernoClimb.entities.attacks;

public partial class Fireball : RigidBody2D {
	public override void _Process(double delta) {
		if (MoveAndCollide(LinearVelocity) != null) {
			QueueFree();
		}
	}

	public void _OnTimeout() {
		QueueFree();
	}
}