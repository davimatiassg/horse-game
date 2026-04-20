using Godot;
using System;

public partial class FireEnemy : Enemy
{
    [ExportGroup("Connections")]
    [Export] private GpuParticles2D fireParticles;

    [ExportGroup("Stats")]
    [Export] public float Speed = 120f;
    [Export] public float attackRadius = 60f;
    [Export] public int fireDamage = 50;
    [Export] public float attackCooldown = 1.5f;

    private HorseBody player;
    private bool isAttacking = false;
    private bool canAttack = true;

    public override void _Ready()
    {
        base._Ready();
        player = GetTree().GetFirstNodeInGroup("player") as HorseBody;
        AddToGroup("enemy");
    }

    public override void _PhysicsProcess(double delta)
    {
        if (player == null || isAttacking) return;

        float distanceSq = GlobalPosition.DistanceSquaredTo(player.GlobalPosition);

        if (distanceSq <= attackRadius * attackRadius && canAttack)
        {
            Attack();
            return;
        }

        // Movimento até o player
        Vector2 direction = GlobalPosition.DirectionTo(player.GlobalPosition);
        Velocity = direction * Speed;

        MoveAndSlide();
    }

    private async void Attack()
    {
        isAttacking = true;
        canAttack = false;

        // Para de se mover
        Velocity = Vector2.Zero;

        // Efeito visual (fogo)
        fireParticles.Emitting = true;

        // Pequeno "wind-up"
        await ToSignal(GetTree().CreateTimer(0.3f), "timeout");

        // Dano em área (curto alcance)
        if (player != null)
        {
            float distSq = GlobalPosition.DistanceSquaredTo(player.GlobalPosition);

            if (distSq <= attackRadius * attackRadius)
            {
                player.TakeDamage(fireDamage);
            }
        }

        // Mantém efeito ativo um pouco
        await ToSignal(GetTree().CreateTimer(0.3f), "timeout");

        fireParticles.Emitting = false;

        isAttacking = false;

        // Cooldown
        await ToSignal(GetTree().CreateTimer(attackCooldown), "timeout");
        canAttack = true;
    }
}
