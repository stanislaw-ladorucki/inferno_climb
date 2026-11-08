using Godot;
using InfernoClimb.entities;
using InfernoClimb.entities.enemies;
using InfernoClimb.level;

namespace InfernoClimb.items.components;

public partial class Meatball : ItemComponent {
	public static readonly PackedScene MeatballScene = GD.Load<PackedScene>("res://scenes/enemies/meatball.tscn"); 
	public override void _Ready() {
		LevelManager level = LevelManager.GetInstance(this);
		level.EnemySpawned += OnEntitySpawned;
     
		// subscribe to living enemy signals
		foreach (Node node in GetTree().GetNodesInGroup("enemy")) {
			if (node is Entity entity) {
				entity.Died += () => OnEnemyDied(entity);
			}
		}
	}

	public void OnEntitySpawned(Entity entity) {
		// subscribe to newly spawned enemies
		if (entity is BaseEnemy enemy) {
			enemy.Died += () => OnEnemyDied(entity);
		}
	}
	private void OnEnemyDied(Entity enemy) {
		if (!(enemy is entities.enemies.Meatball)) {
			using RandomNumberGenerator rng = new RandomNumberGenerator();
			for (int i = 0; i < Stack / 2f + rng.Randf() * Stack / 2; i++) {
				entities.enemies.Meatball meatball = MeatballScene.Instantiate<entities.enemies.Meatball>();
				enemy.AddSibling(meatball);
				meatball.Position = enemy.Position;
				LevelManager.GetInstance(this).EmitSignal("EnemySpawned", meatball);
			}
		}
	}
}
