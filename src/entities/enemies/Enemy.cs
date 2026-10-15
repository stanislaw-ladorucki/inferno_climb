namespace InfernoClimb.entities.enemies;

public partial class Enemy : Entity {
    public override void _PhysicsProcess(double delta)  {
        MoveAndSlide();
    }
   
}