using Godot;
using System;
using System.Collections.Generic;

public partial class LevelColorManager : ShaderGlobalsOverride {
	[Export] public Node2D Player;
	[Export] public float SectionDistance = 4000.0f;
	[Export] public float InterpolationExponent = 8.0f;
	private float _height;

	public KeyValuePair<Texture2D, Texture2D>[] Gradients = [
		new (GD.Load<Texture2D>("res://resources/001_background_gradient.tres"), GD.Load<Texture2D>("res://resources/001_foreground_gradient.tres")),
		new (GD.Load<Texture2D>("res://resources/002_background_gradient.tres"), GD.Load<Texture2D>("res://resources/002_foreground_gradient.tres")),
		new (GD.Load<Texture2D>("res://resources/003_background_gradient.tres"), GD.Load<Texture2D>("res://resources/003_foreground_gradient.tres")),
		new (GD.Load<Texture2D>("res://resources/004_background_gradient.tres"), GD.Load<Texture2D>("res://resources/004_foreground_gradient.tres")),
		new (GD.Load<Texture2D>("res://resources/005_background_gradient.tres"), GD.Load<Texture2D>("res://resources/005_foreground_gradient.tres")),
		new (GD.Load<Texture2D>("res://resources/006_background_gradient.tres"), GD.Load<Texture2D>("res://resources/006_foreground_gradient.tres")),
		new (GD.Load<Texture2D>("res://resources/007_background_gradient.tres"), GD.Load<Texture2D>("res://resources/007_foreground_gradient.tres")),
		new (GD.Load<Texture2D>("res://resources/008_background_gradient.tres"), GD.Load<Texture2D>("res://resources/008_foreground_gradient.tres")),
		new (GD.Load<Texture2D>("res://resources/009_background_gradient.tres"), GD.Load<Texture2D>("res://resources/009_foreground_gradient.tres")),
		// duplicate on purpose
		new (GD.Load<Texture2D>("res://resources/009_background_gradient.tres"), GD.Load<Texture2D>("res://resources/009_foreground_gradient.tres")),
	];
	
	public override void _Process(double delta) {
		if (Player != null && IsInstanceValid(Player)) {
			_height = Math.Max(-Player.GlobalPosition.Y, _height);
		}
		
		int gradientIndex = Mathf.FloorToInt(Mathf.Clamp(_height / SectionDistance, 0, Gradients.Length - 2));
		float mixRatio = Mathf.Clamp(
			Mathf.Lerp(0.0f, 1.0f, Mathf.Pow((_height - gradientIndex * SectionDistance) / SectionDistance, InterpolationExponent)),
			0.0f, 1.0f);
		Set("params/mix_ratio", mixRatio); 
		Set("params/background_gradient", Gradients[gradientIndex].Key);		
		Set("params/background_gradient_mix", Gradients[gradientIndex + 1].Key);
		Set("params/foreground_gradient", Gradients[gradientIndex].Value);		
		Set("params/foreground_gradient_mix", Gradients[gradientIndex + 1].Value);
	}
}
