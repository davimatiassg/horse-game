using Godot;
using System;

public abstract partial class CartTurret : Node2D
{
    [ExportGroup("Connections")]
    [Export] protected HorseBody player;

    [ExportGroup("Stats")]
    [Export] float activationCooldown = 1;

    public abstract void Activate();

    Tween ActivationTween;
    public override void _Process(double delta)
    {
        base._Process(delta);
        if(ActivationTween == null || !ActivationTween.IsRunning())
        {
            Activate();
            ActivationTween = CreateTween();
            ActivationTween.TweenInterval(activationCooldown);
        }
    }
}
