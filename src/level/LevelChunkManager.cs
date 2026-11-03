using System.Collections.Generic;
using System.Diagnostics;
using Godot;

namespace InfernoClimb.level;

public partial class LevelChunkManager : Node2D {
	[Export] public Vector2 ChunkSize = new(600, 1000);
	[Export] public float ChunkGenerateMargin = 200;
	
	[Export] public float ItemsOccurrenceInterval = 500;
	
	[Export] public float FinalPlatformDifficultyHeight = 14_000;
	[Export] public Vector2 StartingPlatformMinDistance = new (10, 50); // horizontal and vertical components
	[Export] public Vector2 StartingPlatformMaxDistance = new (120, 70); // horizontal and vertical components
	[Export] public Vector2 FinalPlatformMinDistance = new (50, 50); // horizontal and vertical components
	[Export] public Vector2 FinalPlatformMaxDistance = new (210, 100); // horizontal and vertical components
	[Export] public float StartingRedundantPlatformRouteChance = 0.1f;
	[Export] public float FinalRedundantPlatformRouteChance = 0.05f;
	
	[Export] public float[] StartingPlatformSpawnWeights = [0.25f, 2f, 0.5f, 0.25f]; // sm, md, lg, xl
	[Export] public float[] FinalPlatformSpawnWeights = [2.5f, 1.5f, 0.25f, 0.125f]; // sm, md, lg, xl
	

	public List<PackedScene> EnemyPool = new();
	public Stack<KeyValuePair<int, PackedScene[]>> EnemyPoolUpdateThresholds = new([
		new (10_000, [
			GD.Load<PackedScene>("res://scenes/enemies/bouncing_enemy.tscn")]),
		new (9_000, [
			GD.Load<PackedScene>("res://scenes/enemies/bouncing_enemy.tscn")]),
		new (7_000, [
			GD.Load<PackedScene>("res://scenes/enemies/descending_enemy.tscn")]),		
		new (6_000, [
			GD.Load<PackedScene>("res://scenes/enemies/floating_enemy.tscn")]),
		new (5_000, [
			GD.Load<PackedScene>("res://scenes/enemies/bouncing_enemy.tscn")]),
		new (4_000, [
			GD.Load<PackedScene>("res://scenes/enemies/descending_enemy.tscn")]),
		new (3_000, [
			GD.Load<PackedScene>("res://scenes/enemies/descending_enemy.tscn")]),
		new (2_000, [
			GD.Load<PackedScene>("res://scenes/enemies/floating_enemy.tscn")]),
		new (1_000, [
			GD.Load<PackedScene>("res://scenes/enemies/floating_enemy.tscn"),
			GD.Load<PackedScene>("res://scenes/enemies/floating_enemy.tscn")]),
	]);

	private Vector2 _lastPlatformPosition;
	private LevelChunk _lastChunk;
	private float _nextItemHeight;

	private static readonly PackedScene ItemScene = GD.Load<PackedScene>("res://scenes/items/item_pickup.tscn");
	private static readonly PackedScene ChunkScene = GD.Load<PackedScene>("res://scenes/level/chunk.tscn");

	public LevelChunk AddChunk() {
		LevelChunk chunk = ChunkScene.Instantiate<LevelChunk>();
		AddChild(chunk);
		
		chunk.Size = ChunkSize;
		if (_lastChunk != null) {
			// The first chunk is created at the origin of the manager.
			// The subsequent ones are generated at the top of the last chunk.
			// FIXME: the upper chunk boundary might not be working correctly - may cause trouble down the line
			chunk.Position = _lastChunk.Position with { Y = _lastChunk.Position.Y - ChunkSize.Y};
		}

		// update enemy pool after reaching a threshold
		if (EnemyPoolUpdateThresholds.Count > 0 && EnemyPoolUpdateThresholds.Peek().Key <= -chunk.Position.Y) {
			EnemyPool.AddRange(EnemyPoolUpdateThresholds.Pop().Value);
		}
		chunk.EnemyPool.AddRange(EnemyPool); // copy enumerable

		// make the distance between platforms increase with progress
		float difficulty = chunk.Position.Y == 0 ? 0 : Mathf.Min(chunk.Position.Y / -FinalPlatformDifficultyHeight, 1.0f);
		chunk.PlatformMinDistance = new(
			Mathf.Lerp(StartingPlatformMinDistance.X, FinalPlatformMinDistance.X, difficulty),
			Mathf.Lerp(StartingPlatformMinDistance.Y, FinalPlatformMinDistance.Y, difficulty)
		);
		chunk.PlatformMaxDistance = new(
			Mathf.Lerp(StartingPlatformMaxDistance.X, FinalPlatformMaxDistance.X, difficulty),
			Mathf.Lerp(StartingPlatformMaxDistance.Y, FinalPlatformMaxDistance.Y, difficulty)
		);
		chunk.PlatformSpawnWeights = [
			Mathf.Lerp(StartingPlatformSpawnWeights[0], FinalPlatformSpawnWeights[0], difficulty),
			Mathf.Lerp(StartingPlatformSpawnWeights[1], FinalPlatformSpawnWeights[1], difficulty),
			Mathf.Lerp(StartingPlatformSpawnWeights[2], FinalPlatformSpawnWeights[2], difficulty),
			Mathf.Lerp(StartingPlatformSpawnWeights[3], FinalPlatformSpawnWeights[3], difficulty),
		];

		chunk.RedundantPlatformRouteChance =
			Mathf.Lerp(StartingRedundantPlatformRouteChance, FinalRedundantPlatformRouteChance, difficulty);
		
		_lastChunk = chunk;
		return chunk;
	}

	public void PopulateItems(Node2D chunk, Platform[] platforms) {
		foreach (Platform platform in platforms) {
			if (-platform.GlobalPosition.Y >= _nextItemHeight) {
				Node2D item = ItemScene.Instantiate<Node2D>();
				chunk.AddChild(item);
				item.GlobalPosition = platform.GlobalPosition;
				_nextItemHeight += ItemsOccurrenceInterval;
			}
		}
	}
	
	
	public override void _Ready() {
		LevelChunk chunk = AddChunk();
		_nextItemHeight = ItemsOccurrenceInterval;
		// first platform is generated at the origin of the manager node
		Platform[] platforms = chunk.Populate(GlobalPosition);
		_lastPlatformPosition = platforms[^1].GlobalPosition;
		PopulateItems(chunk, platforms);
	}
	
	public override void _Process(double delta) {
		Camera2D camera = GetViewport().GetCamera2D();
		float cameraTop = (camera.GetScreenCenterPosition() - camera.GetViewportRect().Size / 2).Y;
		
		// when the top of the chunk is about to be in-view,
		// generate the next chunk offscreen
		if (cameraTop - ChunkGenerateMargin <= _lastChunk.GlobalPosition.Y - ChunkSize.Y) {
			LevelChunk chunk = AddChunk();
			Platform[] platforms = chunk.Populate(_lastPlatformPosition);
			_lastPlatformPosition = platforms[^1].GlobalPosition;
			PopulateItems(chunk, platforms);
		}
	}
}
