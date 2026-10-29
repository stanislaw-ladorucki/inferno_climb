using System.Collections.Generic;
using Godot;

namespace InfernoClimb.level;

public partial class LevelChunk : Node2D {
	[Export] public Vector2 Size = new(600, 800);
	[Export] public Vector2 PlatformMinDistance = new (50, 30); // horizontal and vertical components
	[Export] public Vector2 PlatformMaxDistance = new (200, 100); // horizontal and vertical components
	[Export] public float RedundantPlatformRouteChance;
	[Export] public int MinRedundantPlatformRouteLenght = 3;
	[Export] public int MaxRedundantPlatformRouteLenght = 7;
	[Export] public Vector2 CleanupClusterDistance = new (90, 50);
	
	public List<PackedScene> EnemyPool = [];

	private static readonly PackedScene PlatformScene = GD.Load<PackedScene>("res://scenes/platform.tscn");
	
	public Node2D[] Populate(Vector2 startPosition) {
		using RandomNumberGenerator rng = new RandomNumberGenerator();
		Node2D[] platforms = PopulateMainPlatformRoute(startPosition);

		int platformIndex = 0;
		foreach (Node2D platform in platforms) {
			if (rng.Randf() < RedundantPlatformRouteChance) {
				Node2D[] route = SpawnPlatformRoute(
					startPosition: platform.GlobalPosition, maxHeight: Size.Y - platform.Position.Y,
					maxPlatforms: rng.RandiRange(MinRedundantPlatformRouteLenght, MaxRedundantPlatformRouteLenght)
				);
				CleanupClusteredPlatforms(route, platforms);
			}
			
			// the chance increases the more enemies remain in the pool,
			// and the closer we are to the end of the chunk
			if (EnemyPool.Count > 0 && rng.Randf() < EnemyPool.Count * ((float)platformIndex / platforms.Length)) {
				int enemyIndex = rng.RandiRange(0, EnemyPool.Count - 1);
				Node2D enemy = EnemyPool[enemyIndex].Instantiate<Node2D>();
				AddChild(enemy);
				// TDE: add a separate system for randomizing enemy placement
				enemy.GlobalPosition = GetNewPlatformPosition(platform.GlobalPosition, rng);
				EnemyPool.RemoveAt(enemyIndex);								
			}
			platformIndex++;
		}

		return platforms;
	}

	private Node2D[] PopulateMainPlatformRoute(Vector2 startPosition) {
		return SpawnPlatformRoute(startPosition, Size.Y);
	}

	private Node2D[] SpawnPlatformRoute(Vector2 startPosition, float maxHeight = 0, int maxPlatforms = 0) {
		using RandomNumberGenerator rng = new RandomNumberGenerator();
		List<Node2D> platforms = new();
		Vector2 platformPosition = startPosition;
		while (maxPlatforms == 0 || platforms.Count < maxPlatforms) {
			Node2D platform = CreatePlatform(GetNewPlatformPosition(platformPosition, rng));
			platformPosition = platform.GlobalPosition;
			platforms.Add(platform);
			
			if (maxHeight != 0 && platform.Position.Y <= startPosition.Y - maxHeight) {
				break;
			}
		}
		return platforms.ToArray();
	}

	private void CleanupClusteredPlatforms(Node2D[] toCleanup, Node2D[] mainPlatforms) {
		foreach (Node2D platformA in toCleanup) {
			foreach (Node2D platformB in mainPlatforms) {
				Vector2 dist = (platformA.GlobalPosition - platformB.GlobalPosition).Abs();
				if (dist.X <= CleanupClusterDistance.X && dist.Y <= CleanupClusterDistance.Y) {
					platformA.QueueFree();
					break;
				} 
			}
		}
	}

	private Vector2 GetNewPlatformPosition(Vector2 currentPlatformPosition, RandomNumberGenerator rng) {
		Vector2 offset = new(
			rng.RandfRange(PlatformMinDistance.X, PlatformMaxDistance.X) * (rng.Randf() > 0.5 ? -1 : 1),
			- rng.RandfRange(PlatformMinDistance.Y, PlatformMaxDistance.Y)
		);
		
		// make sure the platform does not exit the horizontal boundaries
		// TDE: include platform width in this calculation
		if ((currentPlatformPosition + offset).X < -Size.X / 2 || (currentPlatformPosition + offset).X > Size.X / 2) {
			offset.X *= -1;
		} 
		return currentPlatformPosition + offset;
	}

	private Node2D CreatePlatform(Vector2 position) {
		Node2D platform = PlatformScene.Instantiate<Node2D>();
		AddChild(platform);
		platform.GlobalPosition = position;
		return platform;
	}
}
