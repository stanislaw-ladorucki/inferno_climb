using Godot;

namespace InfernoClimb.entities.attacks;

public partial class Fireball : RigidBody2D {
	public override void _Process(double delta) {
		if (MoveAndCollide(LinearVelocity) != null) {
			_ActivateFireExplosionEffect();
			QueueFree();
		}
		// not emitting from the start
		GetNode<GpuParticles2D>("DynamicFire").Emitting = true;
	}

	public void _OnTimeout() {
		_ActivateFireExplosionEffect();
		QueueFree();
	}

	private void _ActivateFireExplosionEffect() {
		GpuParticles2D effect = GetNode<GpuParticles2D>("FireExplosion");
		GpuParticles2D trail = GetNode<GpuParticles2D>("DynamicFire");
		
		effect.Reparent(GetParent());
		trail.Reparent(GetParent());
		
		effect.Emitting = true;
		trail.Emitting = false;
		effect.Finished += () => {
			effect.QueueFree();
			trail.QueueFree();
		};

		AudioStreamPlayer2D audio = GetNode<AudioStreamPlayer2D>("FireHitAudioPlayer");
		audio.Reparent(GetParent());
		audio.Play();
		audio.Finished += audio.QueueFree;
	}
}