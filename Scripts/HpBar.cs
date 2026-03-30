using Godot;
using System;

public partial class HpBar : TextureProgressBar
{
	public Node2D nodeToFollow;
	public Vector2 offSet = Vector2.Up*96;

    public override void _Process(double delta)
    {
        base._Process(delta);
		if(nodeToFollow == null || !GodotObject.IsInstanceValid(nodeToFollow) ||  nodeToFollow.IsQueuedForDeletion()) 
		{ 
			QueueFree();
			return; 
		}

		GlobalPosition = GetViewport().GetGlobalCanvasTransform().AffineInverse() * nodeToFollow.GlobalPosition - Size / 2f + offSet * nodeToFollow.Scale.Y;
    }

}
