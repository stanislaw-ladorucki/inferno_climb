using Godot;

namespace InfernoClimb.ui;

public partial class PauseMenu : Control {
	public override void _Input(InputEvent @event) {
		base._Input(@event);
		if (@event.IsActionPressed("pause")) {
			if (GetTree().Paused) {
				Resume();
			}
			else {
				Pause();
			}
		}
	}

	public void Resume() {
		GetTree().Paused = false;
		GetNode<AnimationPlayer>("AnimationPlayer").PlayBackwards("blur");
	}

	public void Pause() {
		GetTree().Paused = true;
		GetNode<AnimationPlayer>("AnimationPlayer").Play("blur");
	}
	
	public void _OnResumePressed() {
		Resume();
	}
	
	public void _OnRestartPressed() {
		Resume();
		GetTree().ReloadCurrentScene();
	}
	
	public void _OnQuitPressed() {
		GetTree().Quit();
	}
}
