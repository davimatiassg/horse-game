using Godot;
using System;

public partial class HealArea : Area2D
{
	Tween healtween;
	public float duration;
	public int healPerTick;
	public float tickTime;

    public override void _Ready()
    {
        base._Ready();
		var tween = CreateTween();
		
		this.Scale = Vector2.Zero;
		tween.TweenProperty(this, "scale", Vector2.One, 0.5f);
		tween.TweenInterval(duration);
		tween.TweenProperty(this, "scale", Vector2.Zero, 0.5f);
		tween.TweenCallback(Callable.From(QueueFree));
    }

	public override void _Process(double delta)
	{
		if(healtween == null || !healtween.IsRunning())
        {
            Heal();
            healtween = CreateTween();
			healtween.TweenInterval(tickTime);
        }
	}

	public void Heal()
	{
		var bodies = GetOverlappingBodies();
		foreach(var body in bodies)
		{
			if(body is IHealthPoints healthyBody)
			{
				healthyBody.HP += healPerTick;
			}
		}
	}
}
