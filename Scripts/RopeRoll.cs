using Godot;
using System;

public partial class RopeRoll : Node2D
{
	[ExportCategory("Connections")]
	[Export] Area2D area;
	public override void _Ready()
	{
		area.BodyEntered += (Node2D body) =>
		{
			Collect();
		};
	}

	public override void _Process(double delta)
	{
	}

	public void Collect()
	{
		AudioPlayer.PlayRandomPitch("rope_grow");
		Pole.IncreaseRope(2);
		QueueFree();
	}
}
