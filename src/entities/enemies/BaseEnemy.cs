namespace InfernoClimb.entities.enemies;

public partial class BaseEnemy : Entity {
    public override void _PhysicsProcess(double delta)  {
        MoveAndSlide();
    }
   
}