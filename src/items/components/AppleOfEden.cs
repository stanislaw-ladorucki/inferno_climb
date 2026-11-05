namespace InfernoClimb.items.components;

public partial class AppleOfEden : ItemComponent {
	public override void AddStack() {
		base.AddStack();
		Player.JumpForce += 30f;
	}
}
