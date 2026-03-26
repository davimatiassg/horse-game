using Godot;

public partial class PickableHorse : CharacterBody2D
{

	[ExportGroup("Connections")]
 
	[Export] Sprite2D horseSprite;
	[Export] public Node3D horse;
	[Export] AnimationPlayer horseAnim;

	[Export] GpuParticles2D footPrints;

	[Export] HorseBody followTarget;

	[Export] Area2D collectHitbox;

	[Export] Godot.Collections.Array<MeshInstance3D> shadedParts;

	[ExportGroup("Data")]
	[Export] public Color mainColor;

	[Export] float maxDistance = 128;

	

	public override void _Ready()
	{
		ChangeColor(mainColor);
		collectHitbox.BodyEntered += (Node2D body) => OnPlayerHit(body);
	}

    public override void _Process(double delta)
    {
        base._Process(delta);
		if (Velocity != Vector2.Zero)
			horse.LookAt((-horse.Transform.Basis.Z).MoveToward(new Vector3(-Velocity.Y, 0, Velocity.X), 50f * (float)delta));
		Animate();
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

		var velocity = Velocity;
		if(followTarget != null)
        {
            if(followTarget.GlobalPosition.DistanceSquaredTo(GlobalPosition) > maxDistance*maxDistance )
		        velocity = (followTarget.GlobalPosition - GlobalPosition);
            else 
                velocity /= 2;
        }
        

        Velocity = velocity;

        MoveAndSlide();
    }

	public void ChangeColor(Color newColor)
	{
		foreach(var part in shadedParts)
		{
			ShaderMaterial originalmaterial = (ShaderMaterial)part.GetSurfaceOverrideMaterial(0);
			var material = (ShaderMaterial)originalmaterial.Duplicate();
			part.SetSurfaceOverrideMaterial(0, material);
			material.SetShaderParameter("bg_color", newColor);
		}
	}
	public void Animate()
	{
		if(Velocity.LengthSquared() <= 1)
		{
			horseAnim.Play("RESET");

		}
		else if(Velocity.Length() < followTarget.speed*0.6f)
		{
			horseAnim.Play("walk");
			footPrints.Emitting = false;
		}
		else
		{
			
			footPrints.Emitting = true;
			footPrints.Position = Velocity.Normalized();
			horseAnim.Play("run");
		}
	}



	public void OnPlayerHit(Node2D body)
	{
		collectHitbox.QueueFree();
		followTarget = (HorseBody)body;
		((HorseBody)body).AddHorse(this);
	}

}
