using System;
using Godot;

namespace InfernoClimb.entities.enemies;

public partial class Gaap : BaseEnemy {
	[Export] public float VelocityDampening = 0.001f;
	[Export] public Vector2 JumpVector = new(200, -300);
	[Export] public float AggroVerticalDistance = 200;
	
	public override void _Ready() {
		// randomize facing direction on spawn
		using RandomNumberGenerator rng = new();
		GetNode<AnimatedSprite2D>("AnimatedSprite2D").FlipH = rng.Randf() < 0.5f;
	}
	
	public override void _PhysicsProcess(double delta) {
		
		MoveAndSlide();
		if (!IsOnFloor()) {
			Velocity += GetGravity() * (float)delta;
		}
		else {
			Velocity = Velocity with {
				X = Mathf.Lerp(Velocity.X, 0.0f, 1 - (float)Math.Pow(VelocityDampening, delta)),
			};
		}
		
		UpdateSprite();
		
		Player player = FindPlayerTarget();
		if (player != null && Mathf.Abs(player.GlobalPosition.Y - GlobalPosition.Y) <= AggroVerticalDistance) {
			AnimatedSprite2D sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
			// only when the hurt animation is not playing, and is standing on the floor
			if (sprite.Animation == "idle") {
				sprite.Play("prepare");
			}
		}
	}
	
	private void UpdateSprite() {
		AnimatedSprite2D sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		if (sprite.Animation != "prepare" && (sprite.Animation != "hurt" || sprite.FrameProgress == 1.0f)) {
			sprite.Play(IsOnFloor() ? "idle" : "jump");
		}
	}

	public void _OnAnimationFinished() {
		AnimatedSprite2D sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		if (sprite.Animation == "prepare") {
			JumpAttack();
		}
	}

	public void JumpAttack() {
		Player player = FindPlayerTarget();
		using RandomNumberGenerator rng = new();
		float targetHorizontalPosition = player?.GlobalPosition.X ?? GlobalPosition.X + rng.RandfRange(-100, 100);
		Velocity = JumpVector * new Vector2(targetHorizontalPosition < GlobalPosition.X ? -1 : 1, 1);

		AnimatedSprite2D sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		sprite.FlipH = targetHorizontalPosition >= GlobalPosition.X;
		sprite.Play("jump");
		
		AudioStreamPlayer2D audio = GetNode<AudioStreamPlayer2D>("JumpAudioPlayer");
		audio.Play();
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
