using Godot;
using System;
using System.ComponentModel;

public partial class Tree : AnimatableBody2D, IKnockbackable, IHittable
{
	[ExportGroup("Connections")]
	[Export]
	public CollisionShape2D uprightCollider;
	[Export]
	public CollisionShape2D fallenCollider;
	[Export]
	public MeshInstance3D treeMesh;

	[Export]
	public PackedScene impactEffect;

	[ExportGroup("stats")]
	[Export]
	public float knockback = 150;
	[Export]
	public int damage = 150;


	private bool isUpright = true;
	public override void _Ready()
	{
		fallenCollider.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public void Knockback(Vector2 knockback)
    {
        if (!isUpright) return;

		isUpright = false; 

		knockback = knockback.Normalized();

		var dir = new Vector3(knockback.X, 0, knockback.Y);

		Basis basis = Basis.LookingAt(dir, Vector3.Up);

		basis = basis * Basis.FromEuler(new Vector3(-Mathf.Pi / 2f, 0, 0));



		fallenCollider.SetDeferred(CollisionShape2D.PropertyName.Disabled, false);

		var shape = (CapsuleShape2D) fallenCollider.Shape;

		fallenCollider.Position = uprightCollider.Position + (((CircleShape2D)uprightCollider.Shape).Radius + ((CapsuleShape2D)fallenCollider.Shape).Height/2) * knockback;
		fallenCollider.LookAt(fallenCollider.GlobalPosition + knockback);
		fallenCollider.RotationDegrees += 90;


		Tween tween = CreateTween();
		tween.TweenProperty(treeMesh, "quaternion", basis.GetRotationQuaternion(), 1).SetTrans(Tween.TransitionType.Bounce);
		tween.TweenCallback(Callable.From(() =>
		{
			var impact = (ImpactArea)impactEffect.Instantiate();
			var impact2 = (ImpactArea)impactEffect.Instantiate();
			var impact3 = (ImpactArea)impactEffect.Instantiate();
			
			impact.damage = damage;
			impact2.damage = damage;
			impact3.damage = damage;

			impact.knockbackIntensity = this.knockback;
			impact2.knockbackIntensity = this.knockback;
			impact3.knockbackIntensity = this.knockback;

			AddChild(impact);
			AddChild(impact2);
			AddChild(impact3);

			
			var d = (fallenCollider.GlobalPosition - GlobalPosition).Normalized();
			
			
			impact.GlobalPosition = fallenCollider.GlobalPosition +  d*24;
			impact2.GlobalPosition = fallenCollider.GlobalPosition + d*88;
			impact3.GlobalPosition = fallenCollider.GlobalPosition - d*40;
		}));


		
    }

    public void TakeDamage(int damage) {}
}
