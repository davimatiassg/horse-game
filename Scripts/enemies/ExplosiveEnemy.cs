using Godot;
using System;

public partial class ExplosiveEnemy : Enemy
{
    [ExportGroup("Connections")]
    [Export] private GpuParticles2D particles;
    
    [ExportGroup("Stats")]
    [Export] public float Speed = 150.0f;
    [Export] public int explosionDamage = 40;
    [Export] public float explosionRadius = 80.0f;
    
    

    private HorseBody player;
    private bool isExploding = false;
    

    public override void _Ready()
    {
        base._Ready();
        player = (HorseBody)GetTree().GetFirstNodeInGroup("player");
        AddToGroup("enemy");
    }

    public override void _PhysicsProcess(double delta)
    {
        if(IsStunned) {MoveAndSlide(); return;}

        // Move towards player
        Vector2 direction = GlobalPosition.DirectionTo(player.GlobalPosition);
        Velocity = Velocity.MoveToward(direction * Speed, Speed * (float)delta);
        
        MoveAndSlide();

        if (isExploding || player == null) return;

        for (int i = 0; i < GetSlideCollisionCount(); i++)
        {
            var collision = GetSlideCollision(i);
            if (collision.GetCollider() is Node colliderNode && colliderNode.IsInGroup("player"))
            {
                Die();
            }
        }
    }

    public override void Knockback(Vector2 knockback)
    {
        base.Knockback(knockback);

    }

    public async override void Die()
    {
        if (isExploding) return;
        isExploding = true;
        
        // Stop movement
        Velocity = Vector2.Zero;

        sprite.Modulate = new Color(2, 0.5f, 0.5f);

        await ToSignal(GetTree().CreateTimer(0.5f), "timeout");

        particles.Emitting = true;
        sprite.Modulate = Colors.Transparent;

        if (player != null && GlobalPosition.DistanceSquaredTo(player.GlobalPosition) < explosionRadius*explosionRadius)
        {
            player.Stun(0.3f);
            player.Knockback((player.GlobalPosition - GlobalPosition).Normalized() * 50);
            player.TakeDamage(explosionDamage);
        }


        await ToSignal(GetTree().CreateTimer(0.5f), "timeout");

        base.Die();
    }
}
