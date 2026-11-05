using System;
using System.Linq;
using Godot;
using InfernoClimb.entities.attacks;
using InfernoClimb.items;

namespace InfernoClimb.entities;

public partial class Player : Entity {
	[Export] public float Speed = 200.0f;
	[Export] public float Acceleration = 16f;
	[Export] public float Deceleration = 11f;
	
	[Export] public float JumpForce = 450f;
	[Export] public float JumpReleaseDampen = 0.45f;
	[Export] public float SubsequentJumpsMultiplier = 0.75f;
	[Export] public int AdditionalJumps;

	[Export] public float EnemyJumpBounceForce = 550f; // when jump button is held, bounce higher
	[Export] public float EnemyBounceForce = 200f;

	[Export] public float InvulnerabilityTime;

	[Export] public Node ItemDisplayContainer;
	public static readonly PackedScene ItemDisplayScene = GD.Load<PackedScene>("res://scenes/ui/item_display.tscn"); 

	private Attack _jumpAttackHitbox;
	private AnimatedSprite2D _sprite;
	
	public int Score;
	public int AdditionalJumpsRemaining = 1;
	
	public override void _Ready() {
		_jumpAttackHitbox = GetNode<Attack>("JumpAttackHitbox");
		_sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
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
			Velocity = Velocity with { Y = -JumpForce * (float)Math.Pow(SubsequentJumpsMultiplier, AdditionalJumps - AdditionalJumpsRemaining) };
			if (!IsOnFloor()) {
				AdditionalJumpsRemaining--;
			}
		}
		
		// hold jump for a longer jump - release for a short one
		if (Input.IsActionJustReleased("jump") && Velocity.Y < 0) {
			Velocity = Velocity with { Y = Velocity.Y * JumpReleaseDampen };
		}
		
		// apply gravity
		if (!IsOnFloor()) {
			Velocity += GetGravity() * (float)delta;
		}
		else {
			// reset jumps
			AdditionalJumpsRemaining = AdditionalJumps;
		}

		// update attack hitbox
		_jumpAttackHitbox.Monitoring = Velocity.Y > 0; // hitbox collision disabled when not falling
		
		UpdateSprite(horizontalInput);
		
		MoveAndSlide();

		if (InvulnerabilityTime > 0) {
			InvulnerabilityTime = Math.Max(InvulnerabilityTime - (float)delta, 0.0f);
			IsInvulnerable = true;
		}
		else {
			IsInvulnerable = false;
		}

		Score = (int)Math.Max(Score, -Position.Y);
	}
	
	private void UpdateSprite(float horizontalInput) {
		// handle sprite flashing to signify invulnerability
		_sprite.Visible = !IsInvulnerable || Math.Floor(Time.GetUnixTimeFromSystem() * 10) % 2 == 0;
		
		if (IsOnFloor()) {
			if (Math.Abs(horizontalInput) > 0) {
				_sprite.Play("walk");
			}
			else {
				_sprite.Play("idle");
			}
		}
		else {
			if (Velocity.Y < 0) {
				_sprite.Play("jump");
			} else {
				_sprite.Play("fall");
			}
		}
		
		// update player facing direction
		if (horizontalInput != 0) {
			_sprite.FlipH = horizontalInput < 0;
		}
	}

	private void _OnJumpAttackHitboxHit(Node2D body) {
		Velocity = Velocity with { Y = Input.IsActionPressed("jump") ? -EnemyJumpBounceForce : -EnemyBounceForce };
	}
	
	private bool CanJump() {
		// TDE: add coyote time
		return IsOnFloor() || AdditionalJumpsRemaining > 0;
	}

	public void AddItem(ItemDefinition itemDefinition) {
		ItemComponent component = GetItem(itemDefinition);
		if (component == null) {
			component = itemDefinition.Scene.Instantiate<ItemComponent>();
			component.Player = this;
			component.Definition = itemDefinition;
			AddChild(component);
		}
		component.Stack++;
		component.AddStack();

		if (ItemDisplayContainer != null) {
			ItemDisplay itemDisplay = (ItemDisplay)Array.Find(
				ItemDisplayContainer.GetChildren().ToArray(),
				itemDisplay => ((ItemDisplay)itemDisplay).ItemDefinition == itemDefinition
			);
			if (itemDisplay != null) {
				itemDisplay.Stack = component.Stack;
			}
			else {
				itemDisplay = ItemDisplayScene.Instantiate<ItemDisplay>();
				itemDisplay.ItemDefinition = itemDefinition;
				itemDisplay.Stack = component.Stack;
				ItemDisplayContainer.AddChild(itemDisplay);
			} 
		}
	}
	public ItemComponent GetItem(ItemDefinition itemDefinition) {
		foreach (Node child in GetChildren()) {
			if (child is ItemComponent item) {
				if (item.Definition == itemDefinition) {
					return item;
				}
			}
		}
		return null;
	}

	public bool HasItem(ItemDefinition itemDefinition) {
		return GetItem(itemDefinition) != null;
	}
}
