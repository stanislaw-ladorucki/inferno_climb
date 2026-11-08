using Godot;

namespace InfernoClimb.entities.attacks;

public enum KnockbackMode {
	Set,
	Additive,
}

public enum KnockbackAngle {
	AttackAngle,
	ConstantAngle,
}

public partial class Attack : Area2D {
	[Export] public float Damage = 1;
	
	[Export] public KnockbackMode KnockbackMode = KnockbackMode.Additive;
	[Export] public KnockbackAngle KnockbackAngle = KnockbackAngle.AttackAngle;
	[Export] public float KnockbackStrength;
	[Export] public Vector2 KnockbackVector;

	[Signal]
	public delegate void HitEventHandler(Node2D body);

	public void _OnBodyEntered(Node2D body) {
		if (body is Entity entity) {
			Callable.From(() => {
				entity.ApplyAttack(this, Damage, GetKnockback(body), KnockbackMode);
				EmitSignalHit(entity);
			}).CallDeferred();
		}
	}

	public Vector2 GetKnockback(Node2D body) {
		switch (KnockbackAngle) {
			case KnockbackAngle.AttackAngle:
				return (Position - body.Position).Normalized() * KnockbackStrength;
			case KnockbackAngle.ConstantAngle:
				return KnockbackVector.Normalized() * KnockbackStrength;
			default:
				return default;
		}
	}
}
