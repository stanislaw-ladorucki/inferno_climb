using System;
using Godot;
using InfernoClimb.entities.attacks;
using InfernoClimb.extensions;

namespace InfernoClimb.entities.enemies;

public partial class Buer : BaseEnemy {
	[Export] public float Speed = 100;
	[Export] public float VelocityDampening = 0.0001f;
	
	public override void _Ready() {
		Velocity = Vector2.Down * Speed;
	}

	public override void _PhysicsProcess(double delta) {
		Velocity = Velocity.Lerp(Vector2.Zero, 1 - (float)Math.Pow(VelocityDampening, delta));
		if (GetTree().Root.GetCamera2D().IsInBounds(GlobalPosition, new Vector2(0, 200))) {
			MoveAndSlide();
		}
	}

	public override void ApplyKnockback(Vector2 knockback, KnockbackMode knockbackMode) {
		Velocity -= new Vector2(0.0f, knockback.Length()) * KnockbackMultiplier;
	}
}
