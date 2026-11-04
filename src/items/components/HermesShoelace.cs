namespace InfernoClimb.items.components;

public partial class HermesShoelace : ItemComponent {
	public override void AddStack() {
		Player.AdditionalJumps++;
	}
}