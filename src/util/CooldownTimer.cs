using System;
using Godot;

namespace InfernoClimb.util;

[GlobalClass]
public partial class CooldownTimer : Timer {
	private float _cooldown = 0; 
	[Export] public float Cooldown {
		get => _cooldown;
		set {
			_cooldown = value;
			WaitTime = Math.Min(WaitTime, _cooldown);
		}
	}

	public override void _Ready() {
		Timeout += () => {
			WaitTime = Cooldown;
		};
	}

	public double Advance(double amount) {
		Start(Math.Max(TimeLeft - amount, Double.Epsilon));
		return TimeLeft;
	}
}