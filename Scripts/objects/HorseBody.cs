using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public interface IPlayer {}

public partial class HorseBody : CharacterBody2D, IPlayer, IHittable, IHealthPoints, IKnockbackable, IStunnable
{

	[ExportGroup("Connections")]
 
	[Export] Sprite2D horseSprite;

	[Export] Node3D horse;
	[Export] AnimationPlayer horseAnim;

	[Export] public Node2D cartStump;

	[Export] GpuParticles2D footPrints;

	[Export] Area2D trampleHitBox;

	[Export] Godot.Collections.Array<MeshInstance3D> shadedParts;



	[ExportGroup("Stats")]
	[Export] public float speed = 500.0f;
	[Export] public float aceleration = 300.0f;
	[Export] public float deceleration = 100.0f;
	[Export] public float turnAcel = 50.0f;

	[Export] int trampleDamage = 50;
	[Export] public int MaxHP { get; set; }


	[ExportGroup("Internal")]

	[Export] public Vector2 directionalForce = Vector2.Left;
	[Export] float internalSpeed;

	public List<Cart> carts = new();

	public Stack<PickableHorse> horses = new();
    
	private bool dying = false;
	private int _HP;

	public  Action<int> OnChangeHP 
    { get; set; }
   [Export] public int HP 
   {
		get => _HP;
    	set
		{
			var displacement = value - _HP;
			var text = displacement <= 0 ? $"[color=red]{displacement}[/color]" : $"[color=green]{displacement}[/color]";
			
			var number = DynamicUIManager.SpawnLabel(text, GlobalPosition);

			var tween = number.CreateTween();

			tween.TweenProperty(number, "modulate:a", 0, 0.5f);

			tween.TweenCallback(Callable.From(() => number.CallDeferred(MethodName.QueueFree)));

			_HP = value;
			if(value > MaxHP)
				_HP = MaxHP;
			if(value <= 0 && !dying)
				HorseDie();
			OnChangeHP?.Invoke(_HP);
		} 
	}

    public Tween StunDelay { get; set; }

    public bool IsStunned => StunDelay != null && StunDelay.IsRunning();


    public override void _Ready()
	{
		HP = MaxHP;
		trampleHitBox.BodyEntered += OnBodyEntered;


		DynamicUIManager.SpawnHPBar(this);
	}

    public override void _Process(double delta)
    {
		
        base._Process(delta);
		if(dying) return;
		if (Velocity != Vector2.Zero)
			horse.LookAt((-horse.Transform.Basis.Z).MoveToward(new Vector3(-Velocity.Y, 0, Velocity.X), turnAcel * (float)delta));

		Animate();
    }

	public override void _PhysicsProcess(double delta)
	{
		if(dying) return;
		if(IsStunned) {MoveAndSlide(); return;}
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
		if(Velocity.LengthSquared() <= 1)
		{
			horseAnim.Play("RESET");
	
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

	public void ChangeColor(Color newColor)
	{
		foreach(var part in shadedParts)
		{
			ShaderMaterial material = (ShaderMaterial)part.GetSurfaceOverrideMaterial(0);
			material.SetShaderParameter("bg_color", newColor);
		}
	}

	Tween damageTween = null;
    public void TakeDamage(int damage)
    {
		
		if(damageTween != null && damageTween.IsRunning()) return;
        
		HP -= damage;

		Stun(0.3f);

		AudioPlayer.PlayRandomPitch("player_damage");

		//piscar

        var material = (ShaderMaterial) horseSprite.Material;

        damageTween = CreateTween();

        for(int i = 0; i < 10; i ++)
        {
            damageTween.TweenCallback(Callable.From(
                () => {
                    float currentFlash = (float)material.GetShaderParameter("flash_amount");
                    material.SetShaderParameter("flash_amount", 1f - currentFlash);
                }
            ));
            damageTween.TweenInterval(0.1f);
            
        }
		
    }

	public void Knockback(Vector2 knockback)
	{
		this.Position += knockback;
	}

	public void Stun(float time)
    {
		if(IsStunned) return;
       	StunDelay = CreateTween();
	   	StunDelay.TweenInterval(time);
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

		

		if(body is IHittable hittable) 				hittable.TakeDamage((int)(trampleDamage * Velocity.Length()/speed));
		if(body is IKnockbackable knockbackable)  	knockbackable.Knockback(Velocity.Normalized() * internalSpeed * 0.25f);
		
	}


#region cartManagement

	public void AddCart(Cart cart)
	{
		carts.Add(cart);
		if(carts.Count == 1)
			cart.AttachTo(cartStump);
		else
			cart.AttachTo(carts[^2].stump);

		cart.OnDie += () => RemoveCart(cart);
	}

	public void RemoveCart(Cart cart)
	{
		int index = carts.IndexOf(cart);
		if(index == -1)
		{
			GD.PrintErr("Tried to remove a cart that isn't there");
			return;
		}
		
		if(index >= carts.Count-1) {}
		else if(index == 0)
		{
			carts[1].AttachTo(this.cartStump);
		}
		else
		{
			carts[index+1].AttachTo(carts[index-1]);
		}

		carts.RemoveAt(index);
		return;
	}
#endregion

#region additionalHorseManagement

	public void AddHorse(PickableHorse horse)
	{
		horses.Push(horse);
	}

	public void HorseDie()
	{
		dying = true;

		AudioPlayer.Play("horse_down");

		horseAnim.Play("down");
		if(horses.Count <= 0)
		{
			
			TrueDeath();
			return;
		}

		ProcessMode = ProcessModeEnum.Always;
		GetTree().Paused = true;
		
		var newHorse = horses.Pop();
		Tween deathTween = CreateTween();
		deathTween.TweenInterval(1);
		deathTween.TweenCallback(Callable.From(() =>
		{
			horseAnim.Play("RESET", 0);
			GlobalPosition = newHorse.GlobalPosition;
			horse.Rotation = newHorse.horse.Rotation;
			ProcessMode = ProcessModeEnum.Inherit;
			GetTree().Paused = false;
			ChangeColor(newHorse.mainColor);
			newHorse.QueueFree();
			dying = false;
			HP = MaxHP;
		}));


	}

	public void TrueDeath()
	{
		Tween tween = CreateTween();

		tween.TweenInterval(2);
		tween.TweenCallback(Callable.From(() => GetTree().Quit()));
	}


    #endregion


}
