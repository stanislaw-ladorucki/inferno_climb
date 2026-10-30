using System.Collections.Generic;
using Godot;
using InfernoClimb.effects;
using InfernoClimb.entities;
using InfernoClimb.items.definitions;

namespace InfernoClimb.items;

public partial class Item : Node2D {
    public static PackedScene FloatingTextEffectScene = GD.Load<PackedScene>("res://scenes/effects/floating_text.tscn");
    public static readonly List<IItem.AddItemHandler> ItemPool = [
        SpeedItem.AddItem,
        JumpHeightItem.AddItem,
        OrbitalItem.AddItem,
    ];

    public void _OnBodyEntered(Node2D body) {
        if (body is Player player) {
            using RandomNumberGenerator rng = new();
            FloatingText text = (FloatingText)FloatingTextEffectScene.Instantiate();
            text.Text = "1UP!"; 
            player.AddSibling(text);
            text.GlobalPosition = player.GlobalPosition;
            
            ItemPool[(int)(rng.Randi() % ItemPool.Count)].Invoke(player);
            QueueFree();
        }
    }
}