using Godot;

namespace InfernoClimb;

public partial class PlatformingChunk : Node2D {
	[Export] public Vector2 Size = new(600, 800);
	[Export] public Vector2 PlatformMinDistance = new (50, 30); // horizontal and vertical components
	[Export] public Vector2 PlatformMaxDistance = new (200, 100); // horizontal and vertical components

	private static readonly PackedScene PlatformScene = GD.Load<PackedScene>("res://scenes/platform.tscn");
	
	public Vector2 Populate(Vector2 platformPosition) {
		RandomNumberGenerator rng = new RandomNumberGenerator();
		while (true) {
			Node2D platform = CreatePlatform(GetNewPlatformPosition(platformPosition, rng));
			platformPosition = platform.GlobalPosition;
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
