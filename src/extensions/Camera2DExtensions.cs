using Godot;

namespace InfernoClimb.extensions;

public static class Camera2DExtensions {
    public static bool IsInBounds(this Camera2D camera2D, Vector2 point, Vector2 margin = default) {
        return (
            point >= camera2D.GetScreenCenterPosition() - margin - camera2D.GetViewportRect().Size / 2 &&
            point <= camera2D.GetScreenCenterPosition() + margin + camera2D.GetViewportRect().Size / 2
        );
    }
}