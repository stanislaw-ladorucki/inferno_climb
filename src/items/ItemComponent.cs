using Godot;
using InfernoClimb.entities;

namespace InfernoClimb.items;

public partial class ItemComponent : Node2D {
    [Export] public Player Player;
    [Export] public int Stack;
    [Export] public ItemDefinition Definition;

    [Signal]
    public delegate void TriggerEventHandler();

    [Signal]
    public delegate void UpdateProgressEventHandler(float progress);

    public virtual void AddStack() {}

    public virtual void AnimateTrigger() {
        EmitSignalTrigger();
    }
    
    public virtual void AnimateProgress(float progress) {
        EmitSignalUpdateProgress(progress);
    }
    
}