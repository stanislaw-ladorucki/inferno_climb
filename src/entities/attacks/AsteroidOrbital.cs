using Godot;

namespace InfernoClimb.entities.attacks;

public partial class AsteroidOrbital : Node2D {
	[Export] public float RotationSpeed = 0.5f;

	public override void _Process(double delta) {
		Rotation += RotationSpeed * (float)delta;
		Attack attack = GetNode<Attack>("Attack");
		attack.KnockbackVector = attack.KnockbackVector.Rotated(Rotation);
	}
}