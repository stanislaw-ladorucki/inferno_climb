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
		Velocity = Velocity.Lerp(Vector2.Down * Speed, 1 - (float)Math.Pow(VelocityDampening, delta));
		if (GetTree().Root.GetCamera2D().IsInBounds(GlobalPosition, new Vector2(0, 200))) {
			MoveAndSlide();
		}
	}

	public override void ApplyKnockback(Vector2 knockback, KnockbackMode knockbackMode) {
		// all knockback works directly upwards, slowing down the descent
		Velocity -= new Vector2(0.0f, knockback.Length()) * KnockbackMultiplier;
	}
	
	public void _OnAnimationFinished() {
		AnimatedSprite2D sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		if (sprite.Animation == "hurt") {
			sprite.Play("idle");
		}
	}

	
	public void _OnHit(Node2D attack, float totalIncomingDamage) {
		AnimatedSprite2D sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		sprite.Play("hurt");
	}
	
	public void _OnDied() {
		AudioStreamPlayer2D audio = GetNode<AudioStreamPlayer2D>("EnemyDeathAudioPlayer");
		audio.Play();
		audio.Reparent(GetParent());
		audio.Finished += () => { audio.QueueFree(); };
	}
}
