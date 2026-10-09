using System;
using Godot;

public partial class Player : CharacterBody2D {
    [Export] public float Speed = 100f;
    [Export] public float Acceleration = 16f;
    [Export] public float Deceleration = 18f;
    
    [Export] public float JumpForce = 400f;
    [Export] public float JumpReleaseDampen = 0.35f;
    
    public float GravityForce = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
    public Vector2 GravityVector = ProjectSettings.GetSetting("physics/2d/default_gravity_vector").AsVector2();
    public Vector2 Gravity { get => GravityForce * GravityVector; }
    
    public override void _PhysicsProcess(double delta)
    {
        // handle horizontal movement
        float horizontalInput = Input.GetAxis("move_left", "move_right");
        float targetHorizontalVelocity = horizontalInput * Speed;
        float lerpWeight = Deceleration;
        if (targetHorizontalVelocity != 0) {
            lerpWeight = Acceleration;
            // when slowing down, use the deceleration weight instead
            if (Math.Abs(targetHorizontalVelocity) < Math.Abs(Velocity.X)) {
                lerpWeight = Mathf.Max(Deceleration, Acceleration);
            }
        }
        // lerp horizontal velocity
        Velocity = Velocity with {
            X = Velocity.X * (1 - lerpWeight * (float)delta) + targetHorizontalVelocity * lerpWeight * (float)delta
        };
        
        // handle juping
        if (Input.IsActionJustPressed("jump") && CanJump()) {
            Velocity = Velocity with { Y = -JumpForce };
        }
        
        // hold jump for a longer jump - release for a short one
        if (Input.IsActionJustReleased("jump") && Velocity.Y < 0) {
            Velocity = Velocity with { Y = Velocity.Y * JumpReleaseDampen };
        }
        
        // apply gravity
        if (!IsOnFloor()) {
            Velocity += GetGravity() * (float)delta;
        }
        MoveAndSlide();
    }
    
    public bool CanJump() {
        return IsOnFloor();
    }
}