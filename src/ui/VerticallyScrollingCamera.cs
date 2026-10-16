using System;
using Godot;

namespace InfernoClimb;

public partial class VerticallyScrollingCamera : Camera2D {
	[Export] public float ScrollingSpeed = 80;
	[Export] public Node2D Follow;
	[Export] public float FollowMargin = 0f; // vertical distance from the center of the screen
	[Export] public float FollowLerp = 0.8f;
	
	public override void _Process(double delta) {
		Position += ScrollingSpeed * Vector2.Up * (float)delta;

		if (IsInstanceValid(Follow) && Follow.Position.Y <= GetScreenCenterPosition().Y + FollowMargin) {
			Position = Position with {
				// lerp camera posiotion to the position of the followed object, while not going downwards
				Y = Math.Min(Position.Y * (1 - FollowLerp * (float)delta) + (Follow.Position.Y - FollowMargin) * FollowLerp * (float)delta, Position.Y)
			};
		}
	}
}