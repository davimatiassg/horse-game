using Godot;
using System;

public partial class TurretCannon : CartTurret
{
    [ExportGroup("Connections")]
    [Export] private Node3D cannonModel;
    
    [Export] PackedScene cannonball;

    [Export] GpuParticles3D blastParticles;

    [ExportGroup("Stats")]

    [Export] private float maxRange = 500;
    
    [Export] private float rotateSpeed = 3;

    [Export] private int shotDamage = 80;
    
    private Vector2 direction = Vector2.Down;

    

    public override void Activate()
    {
        blastParticles.Emitting = true;

        var projectile = cannonball.Instantiate<Projectile>();
        GetTree().Root.AddChild(projectile);

        projectile.damage = shotDamage;
        projectile.LookAt(projectile.GlobalPosition+direction);
        projectile.GlobalPosition = GlobalPosition;
        projectile.direction = direction;
    }


    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        var closest = GetClosestEnemy();
        if (closest == null) return;


        direction = direction.MoveToward((closest.GlobalPosition - GlobalPosition).Normalized(), (float)delta * rotateSpeed);
        
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        cannonModel.LookAt(new Vector3(-direction.X, 0 , -direction.Y));
        
    }

    public Node2D GetClosestEnemy()
    {
        var enemies = GetTree().GetNodesInGroup("enemy");

        Node2D closest = null;
        float closestDistanceSq = maxRange*maxRange;

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