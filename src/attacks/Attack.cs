using Godot;
using InfernoClimb.enemies;

namespace InfernoClimb.attacks;

public partial class Attack : Area2D {
	[Export] public float Damage = 1;

	[Signal]
	public delegate void HitEventHandler(Node2D body);

	public void _OnBodyEntered(Node2D body) {
		if (body is Enemy enemy) {
			enemy.ApplyAttack(this, Damage);
			EmitSignalHit(enemy);
		}
	}
}