using System.Diagnostics;
using Godot;
using InfernoClimb.extensions;

namespace InfernoClimb.entities.enemies;

public partial class DescendingEnemy : BaseEnemy {
    [Export] public float Speed = 100;
    
    public override void _Ready() {
        Velocity = Vector2.Down * Speed;
    }

    public override void _PhysicsProcess(double delta) {
        if (GetTree().Root.GetCamera2D().IsInBounds(GlobalPosition, new Vector2(0, 200))) {
            MoveAndSlide();
        }
    }
}