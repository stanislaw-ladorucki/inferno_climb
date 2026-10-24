using Godot;

namespace InfernoClimb.items.components;

public partial class OrbitalItemComponent : Node2D {
    [Export] public float RotationSpeed = 0.5f;

    public override void _Process(double delta) {
        Rotation += RotationSpeed * (float)delta;
    }

}