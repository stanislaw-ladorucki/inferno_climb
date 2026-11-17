using System;
using Godot;
using InfernoClimb.entities;
using InfernoClimb.entities.enemies;

public partial class Decara : BaseEnemy {
	[Export] public float MoveChance = 0.045f;
	[Export] public float VelocityDampening = 0.0001f;
	[Export] public float FireballVelocity = 4.0f;
	[Export] public float PlayerAggroDistance = 350.0f;

	public static readonly PackedScene FireballScene = GD.Load<PackedScene>("res://scenes/attacks/fireball.tscn");
	
	public override void _PhysicsProcess(double delta) {
		Velocity = Velocity with {
			X = Mathf.Lerp(Velocity.X, 0.0f, 1 - (float)Math.Pow(VelocityDampening, delta)),
		};
		
		MoveAndSlide();
		if (!IsOnFloor()) {
			Velocity += GetGravity() * (float)delta;
		}
		
		Player player = FindPlayerTarget();
		if (player != null && (player.GlobalPosition - GlobalPosition).Length() < PlayerAggroDistance) {
			Timer timer = GetNode<Timer>("Timer");
			if (timer.IsStopped()) {
				timer.Start();
			}
		}
		
		if (player != null) {
			AnimatedSprite2D sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
			sprite.FlipH = player.GlobalPosition.X <= GlobalPosition.X;
		}
	}

	public void _OnTimerTimeout() {
		AnimatedSprite2D sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		sprite.Play("fire");
	}

	public void _OnAnimationFinished() {
		AnimatedSprite2D sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		if (sprite.Animation == "fire") {
			// attacking
			Player player = FindPlayerTarget();
			if (player != null) {
				Vector2 attackDirection = (player.GlobalPosition - GlobalPosition).Normalized();
				
				RigidBody2D fireball = FireballScene.Instantiate<RigidBody2D>();
				AddSibling(fireball);
				fireball.LinearVelocity = attackDirection * FireballVelocity;
				fireball.GlobalPosition = GlobalPosition;
			}
			
			sprite.Play("idle");
		}
		
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
