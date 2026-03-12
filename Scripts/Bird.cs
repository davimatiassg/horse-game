using Godot;
using System;

public partial class Bird : Node2D
{
	[ExportCategory("Connections")]
	[Export] private AnimatedSprite2D sprite;

	[Export] private Area2D area;

	[Export] private VisibleOnScreenNotifier2D notifier;

	public static Random rng = new();

	[ExportCategory("Internal")]

	[Export] private Vector2 flyDirection = Vector2.Zero;

	[ExportCategory("External")]

	[Export] public float speed = 610;
	

	public override void _Ready()
	{
		area.BodyEntered += FlyAway;

		Scale = new Vector2(Scale.X * (rng.Next()%2)*2-1, Scale.Y);

		sprite.AnimationLooped += () =>
		{
			if(rng.Next()%2 == 0)
			{
				sprite.Pause();
				Tween tween = CreateTween();
				tween.TweenInterval(rng.NextDouble() * (rng.Next()%5));
				tween.TweenCallback(Callable.From(() => sprite.Play()));
			}
		};

		notifier.ScreenExited += () =>
		{
			if(sprite.Animation == "fly")
				QueueFree();
		};

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Position += flyDirection * (float)delta * speed;


	}

	

	public void FlyAway(Node2D _)
	{
		flyDirection = new Vector2((float)rng.NextDouble() - 0.5f, (float)rng.NextDouble() - 0.5f).Normalized();
		sprite.Play("fly");

		area.BodyEntered -= FlyAway;
	}
}
