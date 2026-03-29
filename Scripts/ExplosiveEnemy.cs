using Godot;
using System;

public partial class ExplosiveEnemy : CharacterBody2D
{
    [Export] public float Speed = 150.0f;
    [Export] public int MaxHP = 100;
    [Export] public float ExplosionDamage = 40.0f;
    [Export] public float ExplosionRadius = 80.0f;
    
    [Export] public PackedScene CoinPrefab;
    [Export] public PackedScene DamageNumberPrefab;

    private int _currentHP;
    private Node2D _player;
    private bool _isExploding = false;
    
    private AnimatedSprite2D _sprite;
    private GpuParticles2D _particles;

    public override void _Ready()
    {
        _currentHP = MaxHP;
        _sprite = GetNode<AnimatedSprite2D>("SubViewport/AnimatedSprite2D");
        _particles = GetNode<GpuParticles2D>("SubViewport/GPUParticles2D");
        
        // Find player in the scene (assuming player is in "player" group)
        _player = (Node2D)GetTree().GetFirstNodeInGroup("player");
        
        AddToGroup("enemy");
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_isExploding || _player == null) return;

        // Move towards player
        Vector2 direction = GlobalPosition.DirectionTo(_player.GlobalPosition);
        Velocity = direction * Speed;
        
        MoveAndSlide();

        // Check for collision with player to trigger explosion
        for (int i = 0; i < GetSlideCollisionCount(); i++)
        {
            var collision = GetSlideCollision(i);
            if (collision.GetCollider() is Node colliderNode && colliderNode.IsInGroup("player"))
            {
                StartExplosion();
            }
        }
    }

    public void TakeDamage(int amount)
    {
        if (_isExploding) return;
        
        _currentHP -= amount;
        
        // Flash red or show damage numbers here
        
        if (_currentHP <= 0)
        {
            StartExplosion();
        }
    }

    private async void StartExplosion()
    {
        if (_isExploding) return;
        _isExploding = true;
        
        // Stop movement
        Velocity = Vector2.Zero;

        // Visual feedback: Flash red and shake before blowing up
        _sprite.Modulate = new Color(2, 0.5f, 0.5f); // Overdrive red
        await ToSignal(GetTree().CreateTimer(0.2f), "timeout");

        // Logic: Deal damage to things nearby
        if (_player != null && GlobalPosition.DistanceTo(_player.GlobalPosition) < ExplosionRadius)
        {
            // Assuming player has a TakeDamage method
            if (_player.HasMethod("TakeDamage"))
            {
                _player.Call("TakeDamage", ExplosionDamage);
            }
        }

        // Spawn loot
        if (CoinPrefab != null)
        {
            var coin = CoinPrefab.Instantiate<Node2D>();
            GetParent().AddChild(coin);
            coin.GlobalPosition = GlobalPosition;
        }

        // Final destruction
        QueueFree();
    }
}
