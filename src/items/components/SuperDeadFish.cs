using Godot;
using InfernoClimb.util;

namespace InfernoClimb.items.components;

public partial class SuperDeadFish : ItemComponent {
    [Export] public float ProjectileVelocity = 700f;
    [Export] public float StackingProjectileVelocityIncrease = 50f;
    [Export] public float StackingCooldownMultiplier = 0.9f;
    
    public static readonly PackedScene ProjectileScene = GD.Load<PackedScene>("res://scenes/attacks/fish_projectile.tscn");
    
    public void _OnTimerTimeout() {
        using RandomNumberGenerator rng = new RandomNumberGenerator();
        RigidBody2D projectile = ProjectileScene.Instantiate<RigidBody2D>();
        GetParent().AddSibling(projectile);
        projectile.Position = GetParent<Node2D>().GlobalPosition;
        projectile.LinearVelocity = Vector2.FromAngle(rng.RandfRange(Mathf.Pi, Mathf.Pi * 2)) * ProjectileVelocity;
        AnimateTrigger();
    }
	
    public override void AddStack() {
        base.AddStack();
        if (Stack > 1) {
            GetNode<CooldownTimer>("Cooldown").Cooldown *= StackingCooldownMultiplier;
            ProjectileVelocity += StackingProjectileVelocityIncrease;
        }
    }
	
    public override void _Process(double delta) {
        base._Process(delta);
        CooldownTimer cooldown = GetNode<CooldownTimer>("Cooldown");
        AnimateProgress(cooldown.TimeLeft == 0 ? 1.0f : (float)(1 - cooldown.TimeLeft / cooldown.Cooldown));
    }
}