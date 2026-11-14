using Godot;
using InfernoClimb.entities.attacks;

namespace InfernoClimb.entities;

public partial class Entity : CharacterBody2D {
	[Export] public float Health = 1;
	[Export] public float KnockbackMultiplier = 1;
	[Export] public bool IsInvulnerable;
	
	public bool IsDead {
		get => Health <= 0;
		set => Health = value ? 0 : Health;
	}

	public bool HasDied;

	[Signal]
	public delegate void HitEventHandler(Node2D attack, float totalIncomingDamage);
	
	[Signal]
	public delegate void DiedEventHandler();

	public virtual void ApplyAttack(Node2D attack, float damage, Vector2 knockback, KnockbackMode knockbackMode) {
		if (!IsInvulnerable) {
			Health -= damage;
			ApplyKnockback(knockback, knockbackMode);
			EmitSignalHit(attack, damage);
			if (IsDead && !HasDied) {
				Die();
			}
		}
	}

	public void ApplyKnockback(Vector2 knockback, KnockbackMode knockbackMode) {
		if (KnockbackMultiplier == 0)
			// When knockback multiplier is set to zero, the entity should be immune.
			// Make sure that the entity won't become staggered by a `Set` mode knockback, with a zero strength.
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
		EmitSignalDied();
		QueueFree();

		HasDied = true;
	}
}
