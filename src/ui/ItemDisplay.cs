using Godot;
using InfernoClimb.items;

namespace InfernoClimb.ui;

public partial class ItemDisplay : Control {
	private ItemDefinition _itemDefinition;
	private int _stack;
	
	[Export]
	public ItemDefinition ItemDefinition {
		get => _itemDefinition;
		set { _itemDefinition = value; SetItemDefinition(); }
	}
	
	[Export] public int Stack {
		get => _stack;
		set { _stack = value; SetStack(); }
	}

	public void SetItemDefinition() {
		GetNode<TextureRect>("MarginContainer/TextureRect").Texture = _itemDefinition.Icon;
	}
	
	public void SetStack() {
		GetNode<Label>("Label").Text = $"x{_stack}";
	}
}