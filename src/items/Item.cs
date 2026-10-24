using System.Collections.Generic;
using Godot;
using InfernoClimb.entities;
using InfernoClimb.items.definitions;

namespace InfernoClimb.items;

public partial class Item : Node2D {
    public static readonly List<IItem.AddItemHandler> ItemPool = [
        SpeedItem.AddItem,
        JumpHeightItem.AddItem,
        OrbitalItem.AddItem,
    ];

    public void _OnBodyEntered(Node2D body) {
        if (body is Player player) {
            RandomNumberGenerator rng = new();
            ItemPool[(int)(rng.Randi() % ItemPool.Count)].Invoke(player);
            QueueFree();
        }
    }
}