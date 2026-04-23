using Godot;
using System;


public interface IHealthPoints
{
    public int MaxHP {get; set;}
    public int HP {get; set;}

    public Action<int> OnChangeHP 
    { get; set; }
}

public interface IHittable
{
    public void TakeDamage(int damage);
}

public interface IKnockbackable
{
    public void Knockback(Vector2 knockback);
}

public interface IStunnable
{
    public Tween StunDelay {get; set;}
    public bool IsStunned {get;}
    public void Stun(float time);
}


public abstract partial class Enemy : CharacterBody2D, IHittable, IHealthPoints, IKnockbackable
{
    [Export] public CanvasItem sprite;
    [Export] public int MaxHP { get; set; }
    private int _HP;
    
    public  Action<int> OnChangeHP 
    { get; set; }
    public virtual int HP { 
        get => _HP;
        set
        {
            var displacement = value - _HP;
            var text = displacement <= 0 ? $"[shake]{displacement}[/shake]" : $"[color=green]{displacement}[/color]";
            
            var number = DynamicUIManager.SpawnLabel(text, GlobalPosition);

            var tween = number.CreateTween();

            tween.TweenProperty(number, "modulate:a", 0, 0.5f);

            tween.TweenCallback(Callable.From(() => number.CallDeferred(MethodName.QueueFree)));
            
            
            _HP = value;
            if(_HP < 0) Die();
            if(_HP > MaxHP)  _HP = MaxHP;     

            OnChangeHP?.Invoke(_HP);
        } 
    }

    public override void _Ready()
    {
        base._Ready();
        HP = MaxHP;

        this.Skew = Mathf.Pi/2;

        Tween skewtween = CreateTween();
        skewtween.TweenProperty(this, "skew", 0f, 0.5f);
        DynamicUIManager.SpawnHPBar(this);

        AudioPlayer.PlayRandomPitch("eye_spawn");
    }

    public virtual void TakeDamage(int damage)
    {
        HP -= damage;

        AudioPlayer.PlayRandomPitch("enemy_damage");

        var material = (ShaderMaterial) sprite.Material;

        Tween tween = CreateTween();

        for(int i = 0; i < 10; i ++)
        {
            tween.TweenCallback(Callable.From(
                () => {
                    float currentFlash = (float)material.GetShaderParameter("flash_amount");
                    material.SetShaderParameter("flash_amount", 1f - currentFlash);
                }
            ));
            tween.TweenInterval(0.05f);
            
        }

    }

    public virtual void Knockback(Vector2 force)
    {
        Velocity += force*2f;
    }

    public virtual void Die()
    {
        EnemyManager.DropFromEnemy(this);
        CallDeferred(MethodName.QueueFree);
    }

    public override void _PhysicsProcess(double delta)
    {
        
        MoveAndSlide();
    }
}