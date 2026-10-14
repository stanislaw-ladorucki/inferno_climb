using Godot;

namespace InfernoClimb.enemies;


public partial class Enemy : CharacterBody2D {
    // TDE: move this behavior to a separate class to be shared between the player and the enemies
    [Export] public float Health = 1;
    public bool IsDead {
        get => Health <= 0;
        set => Health = value ? 0 : Health;
    }

    [Signal]
    public delegate void HitEventHandler(Node2D attack, float totalIncomingDamage);
    
    [Signal]
    public delegate void DiedEventHandler();

    public void ApplyAttack(Node2D attack, float damage) {
        Health -= damage;
        EmitSignalHit(attack, damage);
        if (IsDead) {
            Die();
        }
    }

    public void Die() {
        EmitSignalDied();
        QueueFree();
    }
}