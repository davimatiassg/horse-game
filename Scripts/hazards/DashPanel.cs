using Godot;
using System;

public partial class DashPanel : Area2D
{
	[Export]
	public float dashIntensity = 500f;
	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
	}

    private void OnBodyEntered(Node2D node)
    {
        if(node is not CharacterBody2D body) return;
		
		body.Velocity += dashIntensity * Transform.X;

		GD.Print(body.Name);

		if(body is IStunnable stunnable) stunnable.Stun(0.5f);

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
		
	}

	
}
