using Godot;
using System;

public partial class Projectile : Area2D
{
    [Export] public float Speed = 300f;

    [Export] public float duration = 5f;
    [Export] public int damage;
    

    Tween durationTween;

    public Vector2 direction = Vector2.Zero;

    public override void _Ready()
    {
        base._Ready();
        BodyEntered += HitBody;
        BodyEntered += (Node2D _) => CallDeferred(MethodName.QueueFree);

        durationTween = CreateTween();
        durationTween.TweenInterval(duration);
        durationTween.TweenCallback(Callable.From(QueueFree));
    }
    public override void _PhysicsProcess(double delta)
    {
        Position = Position.MoveToward(Position + direction * Speed, Speed * (float)delta);
    }

    public void HitBody(Node2D body)
    {
        if(body is IHittable hittable) { hittable.TakeDamage(damage);  }
    }    

}