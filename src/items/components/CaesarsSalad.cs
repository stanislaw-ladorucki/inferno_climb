using Godot;
using InfernoClimb.entities.attacks;

namespace InfernoClimb.items.components;

public partial class CaesarsSalad : ItemComponent {
	[Export] public float StackingScaleIncrease = 0.5f; 
	[Export] public float StackingDamageIncrease = 1.0f; 
	public bool IsHidden = true;
	
	public override void _Process(double delta) {
		AnimationPlayer animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		if (IsHidden && Player.Velocity.Y < 0) {
			animationPlayer.Play("showhide", customSpeed: -1.0f, fromEnd: true);
			IsHidden = false;
		}
		if (!IsHidden && Player.Velocity.Y >= 0) {
			animationPlayer.Play("showhide", customSpeed: 1.0f);
			IsHidden = true;
		}
	}

	public override void AddStack() {
		base.AddStack();
		if (Stack > 1) {
			Scale += Vector2.One * StackingScaleIncrease;
			GetNode<Attack>("LeftDaggerAttack").Damage += StackingDamageIncrease;
			GetNode<Attack>("RightDaggerAttack").Damage += StackingDamageIncrease;
		}
	}
}
