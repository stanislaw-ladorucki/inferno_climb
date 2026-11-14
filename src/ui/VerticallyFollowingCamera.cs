using System.Diagnostics;
using Godot;

namespace InfernoClimb.ui;

public partial class VerticallyFollowingCamera : Camera2D {
	[Export] public Node2D Player;
	[Export] public Node2D PlayerCorpse;
	[Export] public Satan Satan;

	[Export] public float BaseZoom = 1.2f;
	[Export] public float SatanZoom = 1.65f;

	[Export] public float SatanLimitBottomOffset = 80f;
	
	[Export] public float FollowLerp = 0.9f;
	[Export] public float ZoomLerp = 0.1f;

	[Export] public float ScreenShakeStrength; 
	[Export] public float ScreenShakeDecay = 0.01f; 
	
	public override void _Process(double delta) {
		Vector2 followPosition = Position;
		float followLerp = (float)(FollowLerp * delta);
		Vector2 targetZoom = Zoom;
		
		if (IsInstanceValid(Satan)) {
			LimitBottom = (int)(Satan.GlobalPosition.Y + SatanLimitBottomOffset);
		}
		
		if (IsInstanceValid(Player)) {
			targetZoom = Vector2.One * BaseZoom;
			if (IsInstanceValid(Satan)) {
				float satanDistance = Satan.GlobalPosition.DistanceTo(Player.GlobalPosition);
				if (satanDistance <= Satan.OpenDistance) {
					targetZoom = Vector2.One * Mathf.Lerp(BaseZoom, SatanZoom, 1.0f - satanDistance / Satan.OpenDistance);
				}
			}
			followPosition = Player.GlobalPosition * Vector2.Down;
			
		} else if (IsInstanceValid(PlayerCorpse)) {
			followPosition = PlayerCorpse.GlobalPosition * Vector2.Down;
			followLerp = 1.0f;
			targetZoom = Vector2.One * BaseZoom;
			
		} else if (IsInstanceValid(Satan)) {
			followPosition = Satan.GlobalPosition;
			targetZoom = Vector2.One * SatanZoom;
		}
			
		Position = Position.Lerp(followPosition, followLerp);
		Zoom = Zoom.Lerp(targetZoom, ZoomLerp);

		ApplyScreenShake(delta);
	}

	private void ApplyScreenShake(double delta) {
		using RandomNumberGenerator rng = new();
		ScreenShakeStrength = Mathf.Lerp(ScreenShakeStrength, 0.0f, 1.0f - (float)Mathf.Pow(ScreenShakeDecay, delta));
		Offset = new Vector2(
			rng.RandfRange(-ScreenShakeStrength, ScreenShakeStrength),
			rng.RandfRange(-ScreenShakeStrength, ScreenShakeStrength)
		);
	}
}