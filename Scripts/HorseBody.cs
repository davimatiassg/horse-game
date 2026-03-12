using Godot;
using System;

public partial class HorseBody : CharacterBody2D
{

	[ExportCategory("Connections")]
	[Export] Sprite2D knightSprite;
	[Export] Sprite2D horseSprite;

	[Export] Node3D horse;
	[Export] AnimationPlayer horseAnim;

	[Export] Sprite2D gunSprite;

	[Export] GpuParticles2D footPrints;


	[ExportCategory("Stats")]
	[Export] float speed = 300.0f;
	[Export] float aceleration = 200.0f;
	[Export] float deceleration = 100.0f;
	[Export] float turnAcel = 50.0f;


	[ExportCategory("Internal")]

	[Export] Vector2 force = Vector2.Left;

	[Export] float turnTime;

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
			

			force = force.MoveToward(inputDirection, turnAcel * (float)delta);	


			velocity = velocity.Slerp(force * speed, 0.5f);
		}
		else
		{
			velocity = velocity.MoveToward(Vector2.Zero, (float)delta * deceleration);
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
			footPrints.Position = force;
			horseAnim.Play("run");
		}
	}
}
