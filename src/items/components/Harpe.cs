using Godot;
using InfernoClimb.entities.attacks;
using InfernoClimb.util;

namespace InfernoClimb.items.components;

public partial class Harpe : ItemComponent {
	[Export] public float ProjectileDamage = 2.0f;
	[Export] public float StackingDamageIncrease = 1.0f;
	[Export] public float ProjectileVelocityMultiplier = 1.0f;
	[Export] public float StackingProjectileVelocityMultiplierIncrease = 0.1f;
	
	public static readonly PackedScene ProjectileScene = GD.Load<PackedScene>("res://scenes/attacks/harpe_projectile.tscn");
	public void _OnTimerTimeout() {
		using RandomNumberGenerator rng = new RandomNumberGenerator();
		RigidBody2D projectile = ProjectileScene.Instantiate<RigidBody2D>();
		GetParent().AddSibling(projectile);
		projectile.Position = GetParent<Node2D>().GlobalPosition;
		projectile.GetNode<Attack>("Attack").Damage = ProjectileDamage;
		projectile.LinearVelocity = projectile.LinearVelocity with {
			X = projectile.LinearVelocity.X * rng.RandfRange(-1, 1)
		} * ProjectileVelocityMultiplier;
		
		AnimateTrigger();
	}
	
	public override void AddStack() {
		base.AddStack();
		if (Stack > 1) {
			ProjectileDamage += StackingDamageIncrease;
			ProjectileVelocityMultiplier += StackingProjectileVelocityMultiplierIncrease;
		}
	}
	
	public override void _Process(double delta) {
		base._Process(delta);
		CooldownTimer cooldown = GetNode<CooldownTimer>("Cooldown");
		AnimateProgress(cooldown.TimeLeft == 0 ? 1.0f : (float)(1 - cooldown.TimeLeft / cooldown.Cooldown));
	}
}
