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
        using RandomNumberGenerator rng = new RandomNumberGenerator();
        if (rng.Randf() < TriggerChance) {
            Player.InvulnerabilityTime = Math.Max(Player.InvulnerabilityTime, InvulnerabilityDuration);
        }
    }
    
    public override void AddStack() {
        base.AddStack();
        TriggerChance *= StackingTriggerChanceMultiplier;
        InvulnerabilityDuration += StackingInvulnerabilityDurabilityIncrease;
    }
}
