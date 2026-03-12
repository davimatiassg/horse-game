using Godot;
using System;

public partial class Coin : Node2D
{
	[ExportCategory("Connections")]
	[Export] AnimatedSprite2D sprite;
	[Export] Area2D area;
	public override void _Ready()
	{
		area.BodyEntered += (Node2D body) =>
		{
			if(body is Player)
				Collect();
		};
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Collect()
	{
		QueueFree();
	}
}
