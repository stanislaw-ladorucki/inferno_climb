using Godot;
using InfernoClimb.entities.attacks;
using InfernoClimb.util;

namespace InfernoClimb.items.components;

public partial class FlamingSword : ItemComponent {
	[Export] public float StackingScaleMultiplier = 1.2f;
	[Export] public float StackingDamageMultiplier = 1.5f;
	
	public override void AddStack() {
		base.AddStack();
		if (Stack > 0) {
			Attack attack = GetNode<Attack>("Attack");
			attack.Scale *= StackingScaleMultiplier;
			attack.Damage *= StackingDamageMultiplier;
		}
	}

	public void _OnTimerTimeout() {
		AnimationPlayer animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		AnimateTrigger();
		if (!animationPlayer.IsPlaying()) {
			animationPlayer.Play("spin");
		}		
		GetNode<AudioStreamPlayer2D>("FlamingSwingAudioPlayer").Play();
	}

	public override void _Process(double delta) {
		base._Process(delta);
		CooldownTimer cooldown = GetNode<CooldownTimer>("Timer");
		AnimateProgress(cooldown.TimeLeft == 0 ? 1.0f : (float)(1 - cooldown.TimeLeft / cooldown.Cooldown));
		
		GpuParticles2D fire = GetNode<GpuParticles2D>("Attack/Dynamic_Fire");
		AnimationPlayer animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		fire.Emitting = animationPlayer.IsPlaying() && animationPlayer.CurrentAnimation == "spin";
	}
}
