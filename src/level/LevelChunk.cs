using System.Collections.Generic;
using Godot;

namespace InfernoClimb.level;

public partial class LevelChunk : Node2D {
	[Export] public Vector2 Size = new(600, 800);
	[Export] public Vector2 PlatformMinDistance = new (50, 30); // horizontal and vertical components
	[Export] public Vector2 PlatformMaxDistance = new (200, 100); // horizontal and vertical components
	[Export] public float RedundantPlatformChance = 0.20f;
	
	public List<PackedScene> EnemyPool = new();

	private static readonly PackedScene PlatformScene = GD.Load<PackedScene>("res://scenes/platform.tscn");
	private static readonly PackedScene ItemScene = GD.Load<PackedScene>("res://scenes/items/item.tscn");
	
	public Vector2 Populate(Vector2 platformPosition) {
		using RandomNumberGenerator rng = new RandomNumberGenerator();
		while (true) {
			Node2D platform = CreatePlatform(GetNewPlatformPosition(platformPosition, rng));
			platformPosition = platform.GlobalPosition;
				
			// chance for an additional platform that does not in itself lead to another platform
			if (rng.Randf() <= RedundantPlatformChance) {
				CreatePlatform(GetNewPlatformPosition(platformPosition + Vector2.Down * 100, rng));
			}

			// the chance increases the more enemies remain in the pool,
			// and the closer we are to the end of the chunk
			if (EnemyPool.Count > 0 && rng.Randf() < EnemyPool.Count * (platform.Position.Y / -Size.Y)) {
				int enemyIndex = rng.RandiRange(0, EnemyPool.Count - 1);
				Node2D enemy = EnemyPool[enemyIndex].Instantiate<Node2D>();
				AddChild(enemy);
				// TDE: add a separate system for randomizing enemy placement
				enemy.GlobalPosition = GetNewPlatformPosition(platformPosition, rng);
				EnemyPool.RemoveAt(enemyIndex);
			}
				
			// TDE: very, very rough implementation - only temporarily
			if (rng.Randf() < 0.25) {
				Node2D item = (Node2D)ItemScene.Instantiate();
				AddChild(item);
				item.GlobalPosition = platformPosition + Vector2.Up * 20;
			}

			if (platform.Position.Y <= -Size.Y) {
				return platformPosition;
			}
		}
	}

	private Vector2 GetNewPlatformPosition(Vector2 currentPlatfomPosition, RandomNumberGenerator rng) {
		Vector2 offset = new(
			rng.RandfRange(PlatformMinDistance.X, PlatformMaxDistance.X) * (rng.Randf() > 0.5 ? -1 : 1),
			- rng.RandfRange(PlatformMinDistance.Y, PlatformMaxDistance.Y)
		);
		
		// make sure the platform does not exit the horizontal boundaries
		// TDE: include platform width in this calculation
		if ((currentPlatfomPosition + offset).X < -Size.X / 2 || (currentPlatfomPosition + offset).X > Size.X / 2) {
			offset.X *= -1;
		} 
		return currentPlatfomPosition + offset;
	}

	private Node2D CreatePlatform(Vector2 position) {
		Node2D platform = PlatformScene.Instantiate<Node2D>();
		AddChild(platform);
		platform.GlobalPosition = position;
		return platform;
	}
}
