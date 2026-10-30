using Godot;

namespace InfernoClimb.level;

public partial class Platform : StaticBody2D {
	[Export] public float Size;
	[Export] public bool IsMainRoute = true;
}
