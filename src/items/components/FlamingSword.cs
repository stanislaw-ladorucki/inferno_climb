using Godot;
using InfernoClimb.entities.attacks;

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
		if (!animationPlayer.IsPlaying()) {
			animationPlayer.Play("spin");
		}		
	}
}
