using Godot;
using InfernoClimb.entities.attacks;

namespace InfernoClimb.items.components;

public partial class KuiperBelt : ItemComponent {
	[Export] public float MinOrbitalRotationSpeed = 0.25f;
	[Export] public float MaxOrbitalRotationSpeed = 0.75f;
	[Export] public float MinOrbitalScale = 0.9f;
	[Export] public float MaxOrbitalScale = 1.1f;
	
	public static readonly PackedScene AsteroidScene =
		GD.Load<PackedScene>("res://scenes/attacks/asteroid_orbital.tscn");
	
	public override void AddStack() {
		using RandomNumberGenerator rng = new RandomNumberGenerator();
		for (int i = 0; i < 3; i++) {
			AsteroidOrbital asteroid = AsteroidScene.Instantiate<AsteroidOrbital>();
			AddChild(asteroid);
			asteroid.RotationSpeed = rng.RandfRange(MinOrbitalRotationSpeed, MaxOrbitalRotationSpeed);
			asteroid.Scale = Vector2.One * rng.RandfRange(MinOrbitalScale, MaxOrbitalScale);
			asteroid.Rotation = rng.Randf() * Mathf.Pi * 2;
		}
	}
}
