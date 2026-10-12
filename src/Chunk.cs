using Godot;
using System;

public partial class Chunk : Node2D {
	[Export] public Vector2 Size = new(600, 1800);
	[Export] public Vector2 PlatformMinDistance = new (50, 50); // horizontal and vertical components
	[Export] public Vector2 PlatformMaxDistance = new (100, 100); // horizontal and vertical components

	public static PackedScene PlatfromScene = GD.Load<PackedScene>("res://scenes/platform.tscn");
	
	public void PopulateChunk(Vector2 startPosition) {
		RandomNumberGenerator rng = new RandomNumberGenerator();
		
		Node2D platform = CreatePlatform(startPosition);
		while (true) {
			Vector2 position = new(
				platform.GlobalPosition.X + rng.RandfRange(PlatformMinDistance.X, PlatformMaxDistance.X) * (rng.Randf() > 0.5 ? -1 : 1),
				platform.GlobalPosition.Y - rng.RandfRange(PlatformMinDistance.Y, PlatformMaxDistance.Y)
			);
			platform = CreatePlatform(position);
			if (platform.GlobalPosition.Y <= GlobalPosition.Y - Size.Y) {
				break;
			}
		}				
	}

	private Node2D CreatePlatform(Vector2 position) {
		Node2D platform = PlatfromScene.Instantiate<Node2D>();
		AddChild(platform);
		platform.GlobalPosition = position;
		return platform;
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		PopulateChunk(Position);
	}
}
