using Godot;

namespace InfernoClimb.entities.attacks;

public partial class Fireball : RigidBody2D {
	public override void _Process(double delta) {
		if (MoveAndCollide(LinearVelocity) != null) {
			_ActivateFireExplosionEffect();
			QueueFree();
		}
	}

	public void _OnTimeout() {
		_ActivateFireExplosionEffect();
		QueueFree();
	}

	private void _ActivateFireExplosionEffect() {
		GpuParticles2D effect = GetNode<GpuParticles2D>("FireExplosion");
		effect.Reparent(GetParent());
		effect.Emitting = true;
		effect.Finished += effect.QueueFree;

		AudioStreamPlayer2D audio = GetNode<AudioStreamPlayer2D>("FireHitAudioPlayer");
		audio.Reparent(GetParent());
		audio.Play();
		audio.Finished += audio.QueueFree;
	}
}