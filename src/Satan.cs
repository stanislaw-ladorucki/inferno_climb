using Godot;
using InfernoClimb.entities;
using InfernoClimb.extensions;
using InfernoClimb.ui;

namespace InfernoClimb;

public partial class Satan : Node2D {
	[Export] public float StartingVerticalSpeed = 50f;
	[Export] public float FinalVerticalSpeed = 120f;
	[Export] public float FinalVerticalSpeedHeight = 24_000;
	[Export] public float MaxDistanceFromPlayer = 2_000;
	[Export] public float MaxDistanceFromCorpse = 800;
	[Export] public float OpenDistance = 200f;

	[Export] public float MaxBiteScreenShakeStrength = 10f;
	[Export] public float MinBiteScreenShakeStrengthDistance = 1_000f;

	[Export] public GameOverMenu GameOverMenu; 

	public float VerticalSpeed => Mathf.Lerp(StartingVerticalSpeed, FinalVerticalSpeed,
		Mathf.Clamp(GlobalPosition.Y / -FinalVerticalSpeedHeight, 0.0f, 1.0f));
	
	public override void _Process(double delta) {
		Node2D player = GetPlayerOrCorpse();
		if (player != null) {
			float bottomLimit = player.GlobalPosition.Y + (player is Player ? MaxDistanceFromPlayer : MaxDistanceFromCorpse);
			GlobalPosition = GlobalPosition with {
				X = player.GlobalPosition.X,
				Y = Mathf.Min(GlobalPosition.Y - VerticalSpeed * (float)delta, bottomLimit),
			};
			if (player.GlobalPosition.DistanceTo(GlobalPosition) <= OpenDistance) {
				GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("open");
			}
		}
	}

	public Node2D GetPlayerOrCorpse() {
		return GetTree().GetCurrentScene().GetNodeOrNull<Node2D>("Player") ?? 
		       GetTree().GetCurrentScene().GetNodeOrNull<Node2D>("PlayerCorpse");
	}

	public void _OnBodyEnterKillZone(Node2D body) {
		Callable.From(() => {
			if (body is Entity entity) {
				entity.GetNodeOrNull("PlayerCorpse")?.QueueFree();
				entity.Die();
			}
			else {
				body.QueueFree();
				GetNode<AudioStreamPlayer2D>("EatAudioPlayer").Play();
			}
			GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("close");
		}).CallDeferred();
	}

	public void _OnAnimationFinished() {
		AnimatedSprite2D sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		Node2D player = GetPlayerOrCorpse();
		if (sprite.Animation == "close") {
			if (player != null && IsInstanceValid(player)) {
				sprite.Play("idle");
			}
			else {
				sprite.Play("chew");
				// yea game over is triggered by this animation, and not the player dying.
				// kind of flimsy, but who cares.
				GameOverMenu.Start();
			}
		}

		if (sprite.Animation == "open") {
			if (player != null && player.GlobalPosition.DistanceTo(GlobalPosition) > OpenDistance) {
				sprite.Play("close");
			}
		}
	}
	
	public void _OnAnimationLoopedOrFinished() {
		AnimatedSprite2D sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		if (sprite.Animation == "idle" || sprite.Animation == "close") {
			Node2D player = GetPlayerOrCorpse();
			GetTree().Root.GetCamera2D().AddScreenShake(Mathf.Lerp(MaxBiteScreenShakeStrength, 0.0f,
				Mathf.Clamp((player?.GlobalPosition.DistanceTo(GlobalPosition) ?? 0.0f) / MinBiteScreenShakeStrengthDistance,
					0.0f, 1.0f)));
			GetNode<AudioStreamPlayer2D>("BiteAudioPlayer").Play();
		} else if (sprite.Animation == "chew") {
			GetNode<AudioStreamPlayer2D>("ChewAudioPlayer").Play();
		}
	}
}