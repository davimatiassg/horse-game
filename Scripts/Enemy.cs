using Godot;
using System;
using System.Drawing;


public interface IHealthPoints
{
    public int MaxHP {get; set;}
    public int HP {get; set;}
}

public interface IHittable
{
    public void TakeDamage(int damage);
}

public interface IKnockbackable
{
    public void Knockback(Vector2 knockback);
}


public abstract partial class Enemy : CharacterBody2D, IHittable, IHealthPoints, IKnockbackable
{   
    [Export] public PackedScene damageLabel;
    [Export] public int MaxHP { get; set; }
    private int _HP;
    public virtual int HP { 
        get => _HP;
        set
        {
            _HP = value;
            if(value < 0) Die();        
        } 
    }

    public override void _Ready()
    {
        base._Ready();
        HP = MaxHP;

        this.Skew = Mathf.Pi/2;

        Tween skewtween = CreateTween();
        skewtween.TweenProperty(this, "skew", 0f, 0.5f);
    }

    public virtual void TakeDamage(int damage)
    {
        HP -= damage;
        var number = damageLabel.Instantiate<DamageNumber>();
        number.Text = $"[shake]{damage}[/shake]";
        
        number.SetPosition(GlobalPosition - number.Size/2 + Vector2.Up*64);
        GetTree().CurrentScene.AddChild(number);
    }

    public void Knockback(Vector2 force)
    {
        Velocity += force;
    }

    public abstract void Die();

    public override void _PhysicsProcess(double delta)
    {
        
        MoveAndSlide();
    }
}