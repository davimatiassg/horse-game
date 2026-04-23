using Godot;
using System;

public partial class EyeEnemy : Enemy
{
    [ExportGroup("Connections")]
    [Export] PackedScene tear;

    [Export] AnimatedSprite2D eyeSprite;

    [ExportGroup("Stats")]

    [Export] public float Speed = 100f;
    [Export] public float ChangeDirectionTime = 2f;
    [Export] public float ShootCooldown = 1.5f;

    [Export] public int shotDamage = 30;

    private Vector2 _direction;
    private float _dirTimer = 0f;
    private float _shootTimer = 0f;
    [Export] private Node2D _player;

    public override void _Ready()
    {
        base._Ready();
        _player = (Node2D)GetTree().GetFirstNodeInGroup("player");

        
    }

    public override void _PhysicsProcess(double delta)
    {
        if(IsStunned) return;
 
        _dirTimer -= (float)delta;
        if (_dirTimer <= 0)
        {
            _dirTimer = ChangeDirectionTime;
            _direction = new Vector2(GD.Randf() > 0.5f ? 1f : -1f, GD.Randf() > 0.5f ? 1f : -1f).Normalized();
        }

        Velocity = Velocity.MoveToward(_direction * Speed, (float) delta * Speed);
        MoveAndSlide();

        // Atirar no player
        _shootTimer -= (float)delta;
        if (_shootTimer <= 0 && _player != null && _player.GlobalPosition.DistanceSquaredTo(this.GlobalPosition) < 640*640)
        {
            _shootTimer = ShootCooldown;
            Shoot();
        }
    }
    private void Shoot()
    {
        eyeSprite.Play("shot");

        Tween tween = CreateTween();

        tween.TweenInterval(0.3f);

        tween.TweenCallback(Callable.From(() => {

            var projectile = tear.Instantiate<Projectile>();
            GetParent().AddChild(projectile);

            projectile.damage = shotDamage;

            projectile.GlobalPosition = GlobalPosition;
            projectile.direction = (_player.GlobalPosition - GlobalPosition).Normalized();

            projectile.LookAt(projectile.Position + projectile.direction);

        }));
    }


}