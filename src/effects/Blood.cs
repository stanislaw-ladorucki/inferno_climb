using Godot;

namespace InfernoClimb.effects;

public partial class Blood : CpuParticles2D {
    private void _OnFinished() {
        QueueFree();
    }

    private void _OnParentDied() {
        Emitting = true;
        Reparent(GetParent().GetParent());
    }
}