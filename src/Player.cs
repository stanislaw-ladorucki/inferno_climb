using System;
using Godot;
using InfernoClimb.attacks;

namespace InfernoClimb;

public partial class Player : CharacterBody2D {
    [Export] public float Speed = 200.0f;
    [Export] public float Acceleration = 16f;
    [Export] public float Deceleration = 11f;
    
    [Export] public float JumpForce = 450f;
    [Export] public float JumpReleaseDampen = 0.45f;

    [Export] public float EnemyJumpBounceForce = 550f; // when jump button is held, bounce higher
    [Export] public float EnemyBounceForce = 200f;

    public Attack JumpAttackHitbox; 
    
    public override void _Ready() {
        JumpAttackHitbox = GetNode<Attack>("JumpAttackHitbox");
    }

    public override void _PhysicsProcess(double delta)  {
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

        // update attack hitbox
        JumpAttackHitbox.Monitoring = Velocity.Y > 0; // hitbox collision disabled when not falling
        
        MoveAndSlide();
    }

    public void _OnJumpAttackHitboxHit(Node2D body) {
        Velocity = Velocity with { Y = Input.IsActionPressed("jump") ? -EnemyJumpBounceForce : -EnemyBounceForce };
    }
    
    public bool CanJump() {
        // TDE: add coyote time
        return IsOnFloor();
    }
}