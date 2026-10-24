using InfernoClimb.entities;

namespace InfernoClimb.items.definitions;

public abstract class SpeedItem : IItem {
    public static void AddItem(Player player) {
        player.Speed += 10;
    }
}