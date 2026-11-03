using Godot;

namespace InfernoClimb.items.components;

public partial class CaesarsSalad : ItemComponent {
    public bool IsHidden = true;
    
    public override void _Process(double delta) {
        AnimationPlayer animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        if (IsHidden && Player.Velocity.Y < 0) {
            animationPlayer.Play("showhide", customSpeed: -1.0f, fromEnd: true);
            IsHidden = false;
        }
        if (!IsHidden && Player.Velocity.Y >= 0) {
            animationPlayer.Play("showhide", customSpeed: 1.0f);
            IsHidden = true;
        }
    }

    public override void AddStack() {
        Scale += Vector2.One * 0.5f;
    }
}