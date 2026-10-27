using System.Collections.Generic;
using System.Diagnostics;
using Godot;

namespace InfernoClimb.level;

public partial class LevelChunkManager : Node2D {
	[Export] public Vector2 ChunkSize = new(600, 1000);
	[Export] public float ChunkGenerateMargin = 200;
	
	[Export] public float FinalPlatformDifficultyHeight = 14_000;
	[Export] public Vector2 StartingPlatformMinDistance = new (10, 20); // horizontal and vertical components
	[Export] public Vector2 StartingPlatformMaxDistance = new (120, 70); // horizontal and vertical components
	[Export] public Vector2 FinalPlatformMinDistance = new (50, 30); // horizontal and vertical components
	[Export] public Vector2 FinalPlatformMaxDistance = new (210, 100); // horizontal and vertical components
	[Export] public float StartingRedundantPlatformChange = 0.20f;
	[Export] public float FinalRedundantPlatformChange = 0.01f;

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

	private static readonly PackedScene ChunkScene = GD.Load<PackedScene>("res://scenes/level_chunk.tscn");

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

		chunk.RedundantPlatformChance =
			Mathf.Lerp(StartingRedundantPlatformChange, FinalRedundantPlatformChange, difficulty);
		
		_lastChunk = chunk;
		return _lastChunk;
	}
	
	
	public override void _Ready() {
		LevelChunk chunk = AddChunk();
		// first platform is generated at the origin of the manager node
		_lastPlatformPosition = chunk.Populate(GlobalPosition);
	}
	
	public override void _Process(double delta) {
		Camera2D camera = GetViewport().GetCamera2D();
		float cameraTop = (camera.GetScreenCenterPosition() - camera.GetViewportRect().Size / 2).Y;
		
		// when the top of the chunk is about to be in-view,
		// generate the next chunk offscreen
		if (cameraTop - ChunkGenerateMargin <= _lastChunk.GlobalPosition.Y - ChunkSize.Y) {
			LevelChunk chunk = AddChunk();
			_lastPlatformPosition = chunk.Populate(_lastPlatformPosition);
		}
		
		// TDE: delete the chunks off the bottom of the screen  
	}
}
