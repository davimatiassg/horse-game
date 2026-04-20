using Godot;
using System;

public partial class ImpactArea : Area2D
{
	[ExportGroup("Connections")]
	[Export]
	public AnimatedSprite2D sprite;

	[ExportGroup("Status")]
	[Export]
	public int damage = 300;
	[Export]
	public float knockbackIntensity = 100;

	public override void _Ready()
	{
		sprite.AnimationFinished += End;
		BodyEntered += OnBodyEntered;
	}

	public void OnBodyEntered(Node2D body)
	{
		if(body is IHittable hittable) 				hittable.TakeDamage(damage);
		if(body is IKnockbackable knockbackable)  	knockbackable.Knockback((body.Position - Position).Normalized() * knockbackIntensity);
	}

	public void End()
	{
		CallDeferred(MethodName.QueueFree);		
	}
}
