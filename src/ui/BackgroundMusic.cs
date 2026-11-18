using Godot;

namespace InfernoClimb.ui;

public partial class BackgroundMusic : AudioStreamPlayer {
	[Export] public float InGameVolume = -12.0f;		// default
	[Export] public float OutOfGameVolume = -17.0f;		// main menu, and game over screen
	[Export] public float PausedVolume = -22.0f;		// pause menu
	
	public override void _Process(double delta) {
		if (GetTree().IsPaused()) {
			VolumeDb = PausedVolume;
		}
		else {
			Node player = GetTree().GetCurrentScene()?.GetNodeOrNull("Player");
			VolumeDb = (player != null && IsInstanceValid(player) && player.ProcessMode != ProcessModeEnum.Disabled)
				? InGameVolume
				: OutOfGameVolume;
			
		}
	}
}