using Godot;

namespace InfernoClimb.ui;

public partial class BackgroundMusic : AudioStreamPlayer {
	[Export] public float OutOfGameVolume = -12.0f;
	[Export] public float InGameVolume = -7.0f;
	
	public override void _Process(double delta) {
		Node player = GetTree().GetCurrentScene().GetNodeOrNull("Player");
		VolumeDb = (player != null && IsInstanceValid(player) && player.ProcessMode != ProcessModeEnum.Disabled)
			? InGameVolume
			: OutOfGameVolume;
	}
}