using System;
using Godot;
using InfernoClimb.entities;
using InfernoClimb.entities.enemies;
using InfernoClimb.extensions;

public partial class Imp : BaseEnemy {
	[Export] public float Speed = 40;
	[Export] public float MoveChance = 0.045f;
	[Export] public float VelocityDampening = 0.0001f;
	[Export] public float FireballVelocity = 4.0f;
	[Export] public float FireballAttackKickback = 250.0f;
	[Export] public float PlayerAggroDistance = 350.0f;
	[Export] public float PlayerDeaggroDistance = 500.0f;

	public static readonly PackedScene FireballScene = GD.Load<PackedScene>("res://scenes/attacks/fireball.tscn");
	
	public override void _PhysicsProcess(double delta) {
		Velocity = Velocity.Lerp(Vector2.Zero, 1 - (float)Math.Pow(VelocityDampening, delta));
		if (GetTree().Root.GetCamera2D().IsInBounds(GlobalPosition, new Vector2(0, 200))) {
			using RandomNumberGenerator rng = new RandomNumberGenerator();
			if (rng.Randf() < MoveChance) {
				Velocity += Vector2.Right.Rotated(rng.Randf() * Mathf.Pi * 2) * Speed;
			}
			MoveAndSlide();
		}

		Player player = FindPlayerTarget();
		if (player != null) {
			Timer timer = GetNode<Timer>("Timer");
			if ((player.GlobalPosition - GlobalPosition).Length() < PlayerAggroDistance && timer.IsStopped()) {
				timer.Start();
			}
			else if ((player.GlobalPosition - GlobalPosition).Length() > PlayerDeaggroDistance) {
				timer.Stop();
			}
			
			AnimatedSprite2D sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
			sprite.FlipH = player.GlobalPosition.X <= GlobalPosition.X;
		}
	}

	public void _OnTimerTimeout() {
		GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("fire");
		GetNode<AudioStreamPlayer2D>("AttackStartAudioPlayer").Play();
	}

	public void _OnAnimationFinished() {
		AnimatedSprite2D sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		if (sprite.Animation == "fire") {
			// attacking
			Player player = FindPlayerTarget();
			if (player != null) {
				GetNode<AudioStreamPlayer2D>("AttackFinishAudioPlayer").Play();
				Vector2 attackDirection = (player.GlobalPosition - GlobalPosition).Normalized();
				
				RigidBody2D fireball = FireballScene.Instantiate<RigidBody2D>();
				AddSibling(fireball);
				fireball.LinearVelocity = attackDirection * FireballVelocity;
				fireball.GlobalPosition = GlobalPosition;
				Velocity += attackDirection * -FireballAttackKickback;
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
