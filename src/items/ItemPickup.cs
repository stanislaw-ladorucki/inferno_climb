using System;
using System.Collections.Generic;
using Godot;
using InfernoClimb.effects;
using InfernoClimb.entities;

namespace InfernoClimb.items;

public partial class ItemPickup : Node2D {
    public static PackedScene FloatingTextEffectScene = GD.Load<PackedScene>("res://scenes/effects/floating_text.tscn");
    public ItemDefinition Definition;

    public override void _Ready() {
        using RandomNumberGenerator rng = new();
        Definition = ItemPool[(int)(rng.Randi() % ItemPool.Count)];
        GetNode<Sprite2D>("Sprite2D").Texture = Definition.Icon;
    }

    public static readonly List<ItemDefinition> ItemPool = [
        GD.Load<ItemDefinition>("res://resources/items/caesar_salad.tres"),
    ];

    public void _OnBodyEntered(Node2D body) {
        if (body is Player player) {
            Callable.From(() => Pickup(player)).CallDeferred();
        }
    }

    public void Pickup(Player player) {
        FloatingText text = (FloatingText)FloatingTextEffectScene.Instantiate();
        text.Text = Definition.Name; 
        player.AddSibling(text);
        text.GlobalPosition = player.GlobalPosition;
        ItemComponent component = player.GetItem(Definition);
        if (component == null) {
            component = Definition.Scene.Instantiate<ItemComponent>();
            component.Player = player;
            component.Definition = Definition;
            player.AddChild(component);
        }
        else {
            component.AddStack();
        }
        QueueFree();
    }
}