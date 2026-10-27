using System;
using System.Diagnostics;
using Godot;

namespace InfernoClimb.ui;

public partial class VerticallyScrollingCamera : Camera2D {
	[Export] public float ScrollingSpeed = 50;
	[Export] public float MaxScrollingSpeed = 130;
	[Export] public float ScrollingAcceleration = 0.5f;
	
	[Export] public Node2D Follow;
	[Export] public float FollowMargin = -50f; // vertical distance from the center of the screen
	[Export] public float FollowLerp = 0.8f;
	
	public override void _Process(double delta) {
		ScrollingSpeed = Math.Min(MaxScrollingSpeed, ScrollingSpeed + ScrollingAcceleration * (float)delta);
		Position += ScrollingSpeed * Vector2.Up * (float)delta;
		Debug.WriteLine(ScrollingSpeed);

		if (IsInstanceValid(Follow) && Follow.Position.Y <= GetScreenCenterPosition().Y + FollowMargin) {
			Position = Position with {
				// lerp camera position to the position of the followed object, while not going downwards
				Y = Math.Min(Position.Y * (1 - FollowLerp * (float)delta) + (Follow.Position.Y - FollowMargin) * FollowLerp * (float)delta, Position.Y)
			};
		}
	}
}