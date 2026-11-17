using System;
using Godot;
using Godot.Collections;
using InfernoClimb.entities;
using InfernoClimb.entities.enemies;
using InfernoClimb.level;

namespace InfernoClimb.items.components;

public partial class Nehushtan : ItemComponent {
	[Export] public float TriggerChance = 0.1f;
	[Export] public float InvulnerabilityDuration = 1.0f;
	
	[Export] public float StackingTriggerChanceMultiplier = 1.25f;
	[Export] public float StackingInvulnerabilityDurabilityIncrease = 0.25f;

	public override void _Ready() {
		LevelManager.GetInstance(this).EnemyManager.EnemyDied += OnEnemyDied;
	}
	
	// TDE: consider checking if the enemy was killed by the player specifically 
	public void OnEnemyDied(Entity enemy) {
		using RandomNumberGenerator rng = new RandomNumberGenerator();
		if (rng.Randf() < TriggerChance) {
			Player.InvulnerabilityTime = Math.Max(Player.InvulnerabilityTime, InvulnerabilityDuration);
			AnimateTrigger();
		}
	}
	
	public override void AddStack() {
		base.AddStack();
		TriggerChance *= StackingTriggerChanceMultiplier;
		InvulnerabilityDuration += StackingInvulnerabilityDurabilityIncrease;
	}
}
