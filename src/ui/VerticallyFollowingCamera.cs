using System;
using Godot;

namespace InfernoClimb.ui;

public partial class VerticallyFollowingCamera : Camera2D {
	[Export] public Node2D Follow;
	[Export] public float FollowLerp = 0.4f;
	
	public override void _Process(double delta) {
		if (IsInstanceValid(Follow)) {
			Position = Position with {
				// lerp camera position to the position of the followed object
				Y = Mathf.Lerp(Position.Y, Follow.GlobalPosition.Y, FollowLerp * (float)delta)
			};
		}
	}
}