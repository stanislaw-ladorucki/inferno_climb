using System;
using Godot;
using InfernoClimb.extensions;

namespace InfernoClimb.ui;

public partial class DummySatan : Node2D {
	public void Release(Action callback) {
		AnimatedSprite2D sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		AnimationPlayer animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		GetTree().GetRoot().GetCamera2D().AddScreenShake(8);
		sprite.Play("release");
		sprite.AnimationFinished += () => {
			callback();
			GetTree().GetRoot().GetCamera2D().AddScreenShake(8);
			animationPlayer.Play("disappear");
			animationPlayer.AnimationFinished += name => QueueFree();
		};
	}
}