using Godot;
using InfernoClimb.entities;

namespace InfernoClimb.ui;

public partial class GameOverMenu : CenterContainer {
    [Export] public Player Player;
    public override void _Ready() {
        GetNode<ScoreDisplay>("BoxContainer/ScoreDisplay").Player = Player;
    }

    public void Start() {
        Visible = true;
        GetNode<AnimationPlayer>("AnimationPlayer").Play("appear");
    }
}
