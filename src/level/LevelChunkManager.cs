using System.Diagnostics;
using Godot;

namespace InfernoClimb.level;

public partial class LevelChunkManager : Node2D {
	[Export] public Vector2 ChunkSize = new(600, 800);
	[Export] public float ChunkGenerateMargin = 200;

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