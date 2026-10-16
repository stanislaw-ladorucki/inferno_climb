using Godot;
using InfernoClimb.entities;

namespace InfernoClimb.ui;

public partial class ScoreDisplay : Label {
	[Export] public Player Player;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		Text = $"Score: {Player?.Score}";
	}
}