using Godot;
using System;

public partial class FlamethrowerTurret : CartTurret
{
    [ExportGroup("Connections")]
    [Export] private Node3D turretModel;

    [Export] private GpuParticles3D fireParticles;

    [Export] private String fireSoundName = "flamethrower";

    [ExportGroup("Stats")]
    [Export] private float maxRange = 250f;        // alcance total de detecção
    [Export] private float attackRadius = 80f;     // alcance do fogo (curto)
    [Export] private float rotateSpeed = 3f;
    [Export] private int fireDamage = 60;

    private Vector2 direction = Vector2.Down;

    public override void Activate()
    {
        var target = GetClosestEnemy();
        if (target == null) return;

        float distSq = GlobalPosition.DistanceSquaredTo(target.GlobalPosition);

        // Só ataca se estiver no alcance curto (tipo o FireEnemy)
        if (distSq > attackRadius * attackRadius) return;

        // Efeito visual + som
        fireParticles.Emitting = true;
        AudioPlayer.PlayRandomPitch(fireSoundName);

        // Dano em área (todos inimigos próximos na frente)
        var enemies = GetTree().GetNodesInGroup("enemy");

        foreach (Node node in enemies)
        {
            if (node is Enemy enemy)
            {
                float dSq = GlobalPosition.DistanceSquaredTo(enemy.GlobalPosition);

                if (dSq <= attackRadius * attackRadius)
                {
                    enemy.TakeDamage(fireDamage);
                }
            }
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        var closest = GetClosestEnemy();
        if (closest == null) return;

        // Rotação suave (igual à Cannon)
        direction = direction.MoveToward(
            (closest.GlobalPosition - GlobalPosition).Normalized(),
            (float)delta * rotateSpeed
        );
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        turretModel.LookAt(new Vector3(-direction.X, 0, -direction.Y));
    }

    public Node2D GetClosestEnemy()
    {
        var enemies = GetTree().GetNodesInGroup("enemy");

        Node2D closest = null;
        float closestDistanceSq = maxRange * maxRange;

        foreach (Node node in enemies)
        {
            if (node is Node2D enemy)
            {
                float distanceSq = GlobalPosition.DistanceSquaredTo(enemy.GlobalPosition);

                if (distanceSq < closestDistanceSq)
                {
                    closestDistanceSq = distanceSq;
                    closest = enemy;
                }
            }
        }

        return closest;
    }
}
