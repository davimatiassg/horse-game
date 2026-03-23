using Godot;
using System;

public partial class Cart : CharacterBody2D, IHittable, IHealthPoints
{
    [Export] private Node2D pivot;

    [ExportGroup("Connections")]
	
    [Export] public Node2D stump;
    [Export] private Sprite2D sprite;

	[Export] private Node3D cartModel;
    public event Action OnDie;

    [ExportGroup("stats")]

	[Export] private float speed = 5;
    [Export] private float aceleration = 1000;

    [Export] private float maxDistance = 96;

    public int MaxHP { get; set; }
    [Export] private int _HP;
    public int HP { get => _HP;

        set
        {
            _HP = value;
            if(_HP <= 0)
            {
                OnDie?.Invoke();
                QueueFree();
            }
        }
    }


    public override void _Ready()
    {
        base._Ready();
    }

    public override void _Process(double delta)
	{
		if(pivot == null || pivot.GlobalPosition.IsEqualApprox(GlobalPosition)) return;
		cartModel.LookAt(new Vector3(pivot.GlobalPosition.X - GlobalPosition.X, 0 , pivot.GlobalPosition.Y - GlobalPosition.Y));
        stump.Position = -(pivot.GlobalPosition - GlobalPosition).Normalized()*32;
	}

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        var velocity = Velocity;
		if(pivot != null)
        {
            if(pivot.GlobalPosition.DistanceSquaredTo(GlobalPosition) > maxDistance*maxDistance )
		        velocity = (pivot.GlobalPosition - GlobalPosition) * speed;
            else 
                velocity /= 2;
        }
        

        Velocity = velocity;

        MoveAndSlide();
    }


    public void AttachTo(Node2D pivot)
    {
        this.pivot = pivot;
    }

    public void Deattach()
    {
        pivot = null;
    }

    public void TakeDamage(int damage)
    {
        HP -= damage;
    }
}
