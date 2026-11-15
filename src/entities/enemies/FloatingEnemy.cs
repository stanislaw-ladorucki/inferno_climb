using System;
using Godot;
using InfernoClimb.extensions;

namespace InfernoClimb.entities.enemies;

public partial class FloatingEnemy : BaseEnemy {
	[Export] public float Speed = 40;
	[Export] public float MoveChance = 0.045f;
	[Export] public float VelocityDampening = 0.0001f;
	
	public override void _PhysicsProcess(double delta) {
		Velocity = Velocity.Lerp(Vector2.Zero, 1 - (float)Math.Pow(VelocityDampening, delta));
		if (GetTree().Root.GetCamera2D().IsInBounds(GlobalPosition, new Vector2(0, 200))) {
			using RandomNumberGenerator rng = new RandomNumberGenerator();
			if (rng.Randf() < MoveChance) {
				Velocity += Vector2.Right.Rotated(rng.Randf() * Mathf.Pi * 2) * Speed;
			}
			MoveAndSlide();
		}

		UpdateSprite();
	}
	
	private void UpdateSprite() {
		AnimatedSprite2D sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		if (sprite.Animation == "hurt" && sprite.FrameProgress == 1.0f) {
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
