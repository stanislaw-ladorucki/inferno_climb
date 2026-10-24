using InfernoClimb.entities;

namespace InfernoClimb.items.definitions;

public abstract class JumpHeightItem : IItem {
    public static void AddItem(Player player) {
        player.JumpForce += 10;
    }
}