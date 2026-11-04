using System.Collections.Generic;
using Godot;
using InfernoClimb.entities.enemies;

namespace InfernoClimb.level;

public partial class LevelChunk : Node2D {
	[Export] public Vector2 Size = new(600, 800);
	[Export] public Vector2 PlatformMinDistance = new (50, 30); // horizontal and vertical components
	[Export] public Vector2 PlatformMaxDistance = new (200, 100); // horizontal and vertical components
	[Export] public float RedundantPlatformRouteChance;
	[Export] public int MinRedundantPlatformRouteLenght = 3;
	[Export] public int MaxRedundantPlatformRouteLenght = 7;
	[Export] public Vector2 CleanupClusterDistance = new (90, 50);
	[Export] public float[] PlatformSpawnWeights = [1f, 1f, 1f, 1f];
	
	[Signal]
	public delegate void EnemySpawnedEventHandler(BaseEnemy enemy, Platform platform);
	
	private static readonly PackedScene[] PlatformScenes = [
		GD.Load<PackedScene>("res://scenes/level/platform_sm.tscn"),
		GD.Load<PackedScene>("res://scenes/level/platform_md.tscn"),
		GD.Load<PackedScene>("res://scenes/level/platform_lg.tscn"),
		GD.Load<PackedScene>("res://scenes/level/platform_xl.tscn"),
	];
	
	public Platform[] Populate(Vector2 startPosition) {
		using RandomNumberGenerator rng = new RandomNumberGenerator();
		Platform[] platforms = PopulateMainPlatformRoute(startPosition);

		int platformIndex = 0;
		foreach (Platform platform in platforms) {
			if (rng.Randf() < RedundantPlatformRouteChance) {
				Platform[] route = SpawnPlatformRoute(
					startPosition: platform.GlobalPosition, maxHeight: Size.Y - platform.Position.Y,
					maxPlatforms: rng.RandiRange(MinRedundantPlatformRouteLenght, MaxRedundantPlatformRouteLenght)
				);
				foreach (Platform routePlatform in route) {
					routePlatform.IsMainRoute = false;
				}
				CleanupClusteredPlatforms(route, platforms);
			}
			platformIndex++;
		}

		return platforms;
	}

	private Platform[] PopulateMainPlatformRoute(Vector2 startPosition) {
		return SpawnPlatformRoute(startPosition, Size.Y);
	}

	private Platform[] SpawnPlatformRoute(Vector2 startPosition, float maxHeight = 0, int maxPlatforms = 0) {
		using RandomNumberGenerator rng = new RandomNumberGenerator();
		List<Platform> platforms = [CreatePlatform(startPosition)];
		while (maxPlatforms == 0 || platforms.Count < maxPlatforms) {
			Platform platform = CreatePlatform(GetNewPlatformPosition(platforms[^1], rng));
			platforms.Add(platform);
			
			if (maxHeight != 0 && platform.Position.Y <= startPosition.Y - maxHeight) {
				break;
			}
		}
		return platforms.ToArray();
	}

	private void CleanupClusteredPlatforms(Platform[] toCleanup, Platform[] mainPlatforms) {
		foreach (Platform platformA in toCleanup) {
			foreach (Platform platformB in mainPlatforms) {
				Vector2 dist = (platformA.GlobalPosition - platformB.GlobalPosition).Abs();
				if (dist.X <= CleanupClusterDistance.X + platformA.Size / 2 + platformB.Size / 2 && dist.Y <= CleanupClusterDistance.Y) {
					platformA.QueueFree();
					break;
				} 
			}
		}
	}

	private Vector2 GetNewPlatformPosition(Platform fromPlatform, RandomNumberGenerator rng) {
		Vector2 offset = new(
			(rng.RandfRange(PlatformMinDistance.X, PlatformMaxDistance.X) + fromPlatform.Size) / 2 * (rng.Randf() > 0.5 ? -1 : 1),
			- rng.RandfRange(PlatformMinDistance.Y, PlatformMaxDistance.Y)
		);
		
		// make sure the platform does not exit the horizontal boundaries
		// TDE: include platform width in this calculation
		if (
			(fromPlatform.GlobalPosition + offset).X - fromPlatform.Size / 2 < -Size.X / 2 ||
			(fromPlatform.GlobalPosition + offset).X + fromPlatform.Size / 2 > Size.X / 2
		) {
			offset.X *= -1;
		} 
		return fromPlatform.GlobalPosition + offset;
	}

	private Platform CreatePlatform(Vector2 position) {
		using RandomNumberGenerator rng = new RandomNumberGenerator();
		Platform platform = PlatformScenes[rng.RandWeighted(PlatformSpawnWeights)].Instantiate<Platform>();
		AddChild(platform);
		platform.GlobalPosition = position;
		return platform;
	}
}
