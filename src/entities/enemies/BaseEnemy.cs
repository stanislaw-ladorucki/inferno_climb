namespace InfernoClimb.entities.enemies;

public partial class BaseEnemy : Entity {
	public override void _PhysicsProcess(double delta)  {
		MoveAndSlide();
	}

	public virtual Player FindPlayerTarget() {
		// TDE: make this make more sense
		return GetTree().GetCurrentScene().GetNodeOrNull<Player>("Player");
	}
}
