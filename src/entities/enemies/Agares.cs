using System;
using Godot;

namespace InfernoClimb.entities.enemies;

public partial class Agares : BaseEnemy {
	[Export] public float VelocityDampening = 0.0001f;

	public override void _PhysicsProcess(double delta) {
		Velocity = Velocity with {
			X = Mathf.Lerp(Velocity.X, 0.0f, 1 - (float)Math.Pow(VelocityDampening, delta)),
		};
		
		MoveAndSlide();
		if (!IsOnFloor()) {
			Velocity += GetGravity() * (float)delta;
		}
		
		UpdateSprite();
	}
	
	private void UpdateSprite() {
		AnimatedSprite2D sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		if (sprite.Animation == "hurt" && sprite.FrameProgress == 1.0f) {
			sprite.Play("idle");
		}

		Player player = FindPlayerTarget();
		if (player != null) {
			sprite.FlipH = player.GlobalPosition.X <= GlobalPosition.X;
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
