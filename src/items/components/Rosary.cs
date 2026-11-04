using Godot;
using InfernoClimb.entities.attacks;

namespace InfernoClimb.items.components;

public partial class Rosary : ItemComponent {
    [Export] public float InvulnerabilityChance = 0.01f;
    [Export] public float InvulnerabilityChanceIncrease = 0.01f;
    [Export] public float InvulnerabilityDuration = 5f;
    
    public override void _Process(double delta) {
        using RandomNumberGenerator rng = new RandomNumberGenerator();
        if (rng.Randf() / (float)delta < InvulnerabilityChance && Player.InvulnerabilityTime <= 0) {
            Player.InvulnerabilityTime += InvulnerabilityDuration;
        }
        GetNode<Attack>("Attack").Monitoring = Player.IsInvulnerable;
    }

    public override void AddStack() {
        base.AddStack();
        if (Stack > 1) {
            InvulnerabilityChance += InvulnerabilityChanceIncrease / Stack;
        }
    }
}