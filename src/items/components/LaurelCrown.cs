using Godot;
using InfernoClimb.entities;
using InfernoClimb.level;
using InfernoClimb.util;

namespace InfernoClimb.items.components;

public partial class LaurelCrown : ItemComponent {
	[Export] public float CooldownReduction = 0.05f;
	[Export] public float StackingCooldownReductionIncrease = 0.05f;

	public override void _Ready() {
		LevelManager.GetInstance(this).EnemyManager.EnemyDied += OnEnemyDied;
	}
    
	// TDE: consider checking if the enemy was killed by the player specifically 
	public void OnEnemyDied(Entity enemy) {
		if (!IsInstanceValid(this)) {
			return;
		}
		foreach (Node node in GetTree().GetNodesInGroup("cooldown")) {
			if (node is CooldownTimer timer) {
				timer.Advance(CooldownReduction);
			}
			AnimateTrigger();
		}
	}
    
	public override void AddStack() {
		base.AddStack();
		if (Stack > 0) {
			CooldownReduction += StackingCooldownReductionIncrease;
		}
	}
}
