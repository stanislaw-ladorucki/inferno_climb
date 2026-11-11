using System.Collections.Generic;
using Godot;
using InfernoClimb.effects;
using InfernoClimb.entities;

namespace InfernoClimb.items;

public partial class ItemPickup : Node2D {
    public static readonly PackedScene FloatingTextEffectScene = GD.Load<PackedScene>("res://scenes/effects/floating_text.tscn");
    public ItemDefinition Definition;

    public override void _Ready() {
        using RandomNumberGenerator rng = new();
        Definition = ItemPool[(int)(rng.Randi() % ItemPool.Count)];
        GetNode<Sprite2D>("Sprite2D").Texture = Definition.Icon;
    }

    public static readonly List<ItemDefinition> ItemPool = [
        GD.Load<ItemDefinition>("res://resources/items/caesar_salad.tres"),
        GD.Load<ItemDefinition>("res://resources/items/apple_of_eden.tres"),
        GD.Load<ItemDefinition>("res://resources/items/rosary.tres"),
        GD.Load<ItemDefinition>("res://resources/items/hermes_shoelace.tres"),
        GD.Load<ItemDefinition>("res://resources/items/pluto.tres"),
        GD.Load<ItemDefinition>("res://resources/items/kuiper_belt.tres"),
        GD.Load<ItemDefinition>("res://resources/items/nehushtan.tres"),
        GD.Load<ItemDefinition>("res://resources/items/flaming_sword.tres"),
        GD.Load<ItemDefinition>("res://resources/items/laurel_crown.tres"),
        GD.Load<ItemDefinition>("res://resources/items/meatball.tres"),
        GD.Load<ItemDefinition>("res://resources/items/harpe.tres"),
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
        player.AddItem(Definition);
        QueueFree();
    }
}