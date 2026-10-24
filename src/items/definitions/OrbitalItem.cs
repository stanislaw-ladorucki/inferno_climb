using Godot;
using InfernoClimb.entities;

namespace InfernoClimb.items.definitions;

public abstract class OrbitalItem : IItem {
    private static readonly PackedScene ComponentScene = GD.Load<PackedScene>("res://scenes/items/orbital_item_component.tscn");
    public static void AddItem(Player player) {
        player.AddChild(ComponentScene.Instantiate());
    }
}