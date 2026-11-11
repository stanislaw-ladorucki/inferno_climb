using Godot;

namespace InfernoClimb.entities.attacks;

public partial class HarpeProjectile : RigidBody2D {
	public void _OnTimeout() {
		QueueFree();
	}
}