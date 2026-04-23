using Godot;
using System;

public partial class Cogwheel : RigidBody2D, IKnockbackable
{
	private Vector2 startPosition = Vector2.Zero;

	[Export]
	public float knockbackIntensity = 100f;
	
	public override void _Ready()
	{
		base._Ready();

		startPosition = GlobalPosition;

		BodyEntered += OnBodyEntered;
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		LinearVelocity = Vector2.Zero;
		GlobalPosition = GlobalPosition.MoveToward(startPosition, 100*(float)delta);
	}


	Tween playerDamageDelay = null;
	public void OnBodyEntered(Node body)
	{
		if(Mathf.Abs(AngularVelocity) <= 20) return;

		if(body is HorseBody && (playerDamageDelay != null && playerDamageDelay.IsRunning())) {GD.Print("Lol.");return;}

		if(body is IHittable hittable)													hittable.TakeDamage((int)Mathf.Abs(AngularVelocity));
		if(body is IKnockbackable knockbackable and Node2D body2D and not Cogwheel)    	knockbackable.Knockback((body2D.GlobalPosition - GlobalPosition).Normalized() * knockbackIntensity);
	}

    public void Knockback(Vector2 knockback)
    {
		
		ApplyTorqueImpulse(Mathf.Sign(knockback.X) * (Mathf.Abs(knockback.Y) + Mathf.Abs(knockback.X)));
		LinearVelocity = Vector2.Zero;

		if(playerDamageDelay == null || !playerDamageDelay.IsRunning())
		{
			playerDamageDelay = CreateTween();
			playerDamageDelay.TweenInterval(0.1f);
		}
		
    }

}
