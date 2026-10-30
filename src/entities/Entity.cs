
using Godot;
using InfernoClimb.entities.attacks;

namespace InfernoClimb.entities;

public partial class Entity : CharacterBody2D {
	[Export] public float Health = 1;
	[Export] public float KnockbackMultiplier = 1;
	[Export] public PackedScene BloodScene;

	public bool IsDead {
		get => Health <= 0;
		set => Health = value ? 0 : Health;
	}

	[Signal]
	public delegate void HitEventHandler(Node2D attack, float totalIncomingDamage);

	[Signal]
	public delegate void DiedEventHandler();

	public void ApplyAttack(Node2D attack, float damage, Vector2 knockback, KnockbackMode knockbackMode) {
		Health -= damage;
		ApplyKnockback(knockback, knockbackMode);
		EmitSignalHit(attack, damage);

		if (IsDead) {
			Die();
		}
	}

	public void ApplyKnockback(Vector2 knockback, KnockbackMode knockbackMode) {
		if (KnockbackMultiplier == 0)
			return;

		switch (knockbackMode) {
			case KnockbackMode.Set:
				Velocity = knockback * KnockbackMultiplier;
				break;
			case KnockbackMode.Additive:
				Velocity += knockback * KnockbackMultiplier;
				break;
		}
	}

	public void Die() {
		if (BloodScene != null) {
			CpuParticles2D blood = BloodScene.Instantiate<CpuParticles2D>();
			GetTree().CurrentScene.AddChild(blood);
			blood.GlobalPosition = GlobalPosition;
			blood.Emitting = true;
		}

		EmitSignalDied();
		QueueFree();
	}
}
