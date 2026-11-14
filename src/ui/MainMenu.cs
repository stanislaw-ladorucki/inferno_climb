using Godot;
using InfernoClimb.entities;

namespace InfernoClimb.ui;

public partial class MainMenu : CenterContainer {
	[Export] public Player Player;
	[Export] public Satan Satan;
	[Export] public DummySatan DummySatan;
	private bool _isWaitingForInput;
	
	public override void _Input(InputEvent @event) {
		base._Input(@event);
		if (_isWaitingForInput) {
			if (@event is InputEventKey or InputEventJoypadButton) {
				Control scoreDisplay = GetNode<Control>("%ScoreDisplay");
				DummySatan.Release(() => {
					Player.ProcessMode = ProcessModeEnum.Pausable;
					Player.Visible = true;
					Satan.ProcessMode = ProcessModeEnum.Pausable;
					Player.Velocity = new(0.0f, -700.0f);
					scoreDisplay.Visible = true;
				});
				QueueFree();
			}
		}
	}

	public void _OnTimerTimeout() {
		GetNode<Control>("%PressAnyLabel").GetParentControl().Visible = true;
		_isWaitingForInput = true;
	}
}
