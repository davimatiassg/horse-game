using Godot;
using System;
using System.Collections.Generic;

public partial class Pole : Sprite2D
{
	[ExportGroup("Connections")]
	[Export] private Line2D line;

	[Export] public Marker2D startPoint;

	[Export] public HorseBody player;


	[ExportGroup("Stats")]
	[Export] public int SegmentCount = 50;
    [Export] public float SegmentLength = 2f;
    [Export] public int Iterations = 4;

	private float RopeLenght { get => SegmentCount*SegmentLength; }


	private List<Vector2> points = new();
    private List<Vector2> previousPoints = new();
	
    

    public override void _Ready()
    {
        line = GetNode<Line2D>("Line2D");

        for (int i = 0; i < SegmentCount; i++)
        {
            Vector2 pos = startPoint.Position + new Vector2(0, i * SegmentLength);
            points.Add(pos);
            previousPoints.Add(pos);
        }
    }

	public override void _PhysicsProcess(double delta)
    {
        

		if(startPoint.GlobalPosition.DistanceSquaredTo(player.GlobalPosition) > RopeLenght*RopeLenght*16)
		{
			player.GlobalPosition = player.GlobalPosition.MoveToward(points[^2], (float) delta * points[^2].DistanceSquaredTo(player.GlobalPosition));
		}

		AnimateRope((float)delta);
        
    }



	public void AnimateRope(float dt)
	{
		for (int i = 0; i < points.Count; i++)
        {
            Vector2 velocity = points[i] - previousPoints[i];
            previousPoints[i] = points[i];

            points[i] += velocity;
        }


        for (int k = 0; k < Iterations; k++)
        {
            points[0] = startPoint.Position;
            points[^1] = player.Position;

            for (int i = 0; i < points.Count - 1; i++)
            {
                Vector2 diff = points[i + 1] - points[i];
                float dist = diff.Length();
                float error = dist - SegmentLength;

                Vector2 change = diff.Normalized() * (error * 0.5f);

                if (i != 0)
                    points[i] += change;

                if (i + 1 != points.Count - 1)
                    points[i + 1] -= change;
            }
        }

        line.Points = points.ToArray();
	}
}
