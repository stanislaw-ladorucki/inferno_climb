using Godot;

namespace InfernoClimb.effects;

public partial class FloatingText : Node2D {
	[Export] public string Text {
		get => GetNodeOrNull<Label>("Label")?.Text;
		set => GetNodeOrNull<Label>("Label").Text = value;
	}

	private void _OnFinished(StringName animName) {
		QueueFree();
	}
}