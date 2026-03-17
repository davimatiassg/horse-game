using Godot;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

public interface IPlayer {}

public partial class HorseBody : CharacterBody2D, IPlayer, IHittable, IHealthPoints
{

	[ExportCategory("Connections")]
 
	[Export] Sprite2D horseSprite;

	[Export] Node3D horse;
	[Export] AnimationPlayer horseAnim;

	[Export] public Node2D cartStump;

	[Export] GpuParticles2D footPrints;

	[Export] Area2D trampleHitBox;


	[ExportCategory("Stats")]
	[Export] float speed = 500.0f;
	[Export] float aceleration = 300.0f;
	[Export] float deceleration = 100.0f;
	[Export] float turnAcel = 50.0f;

	[Export] int trampleDamage = 50;
	[Export] public int MaxHP { get; set; }


	[ExportCategory("Internal")]

	[Export] Vector2 directionalForce = Vector2.Left;
	[Export] float internalSpeed;
    
   [Export] public int HP { get; set; }

    public override void _Ready()
	{
		HP = MaxHP;
		trampleHitBox.BodyEntered += OnBodyEntered;
	}

    public override void _Process(double delta)
    {
        base._Process(delta);
		if (Velocity != Vector2.Zero)
			horse.LookAt((-horse.Transform.Basis.Z).MoveToward(new Vector3(-Velocity.Y, 0, Velocity.X), turnAcel * (float)delta));

		Animate();
    }

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		Vector2 inputDirection = Input.GetVector("game_left", "game_right", "game_up", "game_down");

		if (inputDirection != Vector2.Zero)
		{
			

			directionalForce = directionalForce.MoveToward(inputDirection, turnAcel * (float)delta);	
			internalSpeed = Mathf.MoveToward(internalSpeed, speed, aceleration * (float) delta);

			velocity = velocity.Slerp(directionalForce * internalSpeed, 0.5f);
		}
		else
		{
			velocity = velocity.MoveToward(Vector2.Zero, (float)delta * deceleration);
			internalSpeed = Mathf.MoveToward(internalSpeed, 0, (float)delta * deceleration);
		}

		Velocity = velocity;

		MoveAndSlide();
	}


	public void Animate()
	{
		if(Velocity == Vector2.Zero)
		{
			horseAnim.Play("down");
			horseAnim.Stop();
		}
		else if(Velocity.Length() < speed*0.6f)
		{
			horseAnim.Play("walk");
			footPrints.Emitting = false;
		}
		else
		{
			
			footPrints.Emitting = true;
			footPrints.Position = directionalForce;
			horseAnim.Play("run");
		}
	}

    public void TakeDamage(int damage)
    {
        HP -= damage;
    }



	private void OnBodyEntered(Node2D body)
    {
		
        if(Velocity.Length()/speed > 0.6f)
		{
			Trample(body);
			
		}
    }

	private void Trample(Node2D body)
	{

		if(body is IHittable hittable) 				hittable.TakeDamage((int)(trampleDamage * internalSpeed/speed));
		if(body is IKnockbackable knockbackable)  	knockbackable.Knockback((body.Position - Position).Normalized() * internalSpeed * 0.25f);
		
	}

}
