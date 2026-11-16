using Godot;
using InfernoClimb.entities;
using InfernoClimb.entities.enemies;
using InfernoClimb.level;

namespace InfernoClimb.items.components;

public partial class Meatball : ItemComponent {
	public static readonly PackedScene MeatballScene = GD.Load<PackedScene>("res://scenes/enemies/meatball.tscn"); 
	public override void _Ready() {
		LevelManager.GetInstance(this).EnemyManager.EnemyDied += OnEnemyDied;
	}
	
	private void OnEnemyDied(Entity enemy) {
		if (!(enemy is entities.enemies.Meatball)) {
			using RandomNumberGenerator rng = new RandomNumberGenerator();
			for (int i = 0; i < Stack / 2f + rng.Randf() * Stack / 2; i++) {
				LevelManager.GetInstance(this).EnemyManager.SpawnAtPosition(
					MeatballScene, enemy.GetParent<Node2D>(), enemy.GlobalPosition
				);
			}
			AnimateTrigger();
		}
	}
}
