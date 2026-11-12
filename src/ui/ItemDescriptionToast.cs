using Godot;
using InfernoClimb.items;

namespace InfernoClimb.ui;

public partial class ItemDescriptionToast : PanelContainer {
	private ItemDefinition _itemDefinition;
	private bool _isShown;
	
	[Export]
	public ItemDefinition ItemDefinition {
		get => _itemDefinition;
		set { _itemDefinition = value; SetItemDefinition(); }
	}

	[Export] public ItemDefinition NextItemDefinition;
	
	public void Appear() {
		_isShown = true;
		GetNode<AnimationPlayer>("AnimationPlayer").Play("appear");
		GetNode<Timer>("Timer").Start();
	}

	public void Disappear() {
		_isShown = false;
		GetNode<AnimationPlayer>("AnimationPlayer").Play("disappear");
	}
	
	public void SetItemDefinition() {
		GetNode<TextureRect>("%ItemIcon").Texture = _itemDefinition.Icon;
		GetNode<Label>("%ItemName").Text = _itemDefinition.Name;
		GetNode<RichTextLabel>("%ItemDescription").Text = _itemDefinition.Description;
	}

	public void _OnTimeout() {
		Disappear();
	}

	public void _OnAnimationFinished(StringName animName) {
		if (animName == "disappear") {
			if (NextItemDefinition != null) {
				ItemDefinition = NextItemDefinition;
				NextItemDefinition = null;
				Appear();
			}			
		}
	}

	public void QueueItem(ItemDefinition itemDefinition) {
		if (_isShown) {
			NextItemDefinition = itemDefinition;
			Disappear();
		}
		else {
			ItemDefinition = itemDefinition;
			Appear();
		}
	}
}