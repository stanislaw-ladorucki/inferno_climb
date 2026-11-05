using System.Diagnostics;
using Godot;
using InfernoClimb.entities;
using InfernoClimb.entities.enemies;
using InfernoClimb.level;
using InfernoClimb.util;

namespace InfernoClimb.items.components;

public partial class LaurelCrown : ItemComponent {
	[Export] public float CooldownReduction = 0.1f;
	[Export] public float StackingCooldownReductionIncrease = 0.1f;

	public override void _Ready() {
		LevelManager level = LevelManager.GetInstance(this);
		level.EnemySpawned += OnEntitySpawned;
     
		// subscribe to living enemy signals
		foreach (Node node in GetTree().GetNodesInGroup("enemy")) {
			if (node is Entity entity) {
				entity.Died += OnEnemyDied;
			}
		}
	}

	public void OnEntitySpawned(Entity entity) {
		// subscribe to newly spawned enemies
		if (entity is BaseEnemy enemy) {
			enemy.Died += OnEnemyDied;
		}
	}
    
	// TDE: consider checking if the enemy was killed by the player specifically 
	public void OnEnemyDied() {
		foreach (Node node in GetTree().GetNodesInGroup("cooldown")) {
			if (node is CooldownTimer timer) {
				double timeLeft = timer.TimeLeft; 
				timer.Advance(CooldownReduction);
			}
		}
	}
    
	public override void AddStack() {
		base.AddStack();
		if (Stack > 0) {
			CooldownReduction += StackingCooldownReductionIncrease;
		}
	}
}
