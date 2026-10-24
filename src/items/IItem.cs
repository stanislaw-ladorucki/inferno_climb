using InfernoClimb.entities;

namespace InfernoClimb.items;

public interface IItem {
    public delegate void AddItemHandler(Player player);
    public static abstract void AddItem(Player player);
}