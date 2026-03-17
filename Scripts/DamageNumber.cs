using Godot;
using System;

public partial class DamageNumber : RichTextLabel
{
    [Export] float time = 0.25f;

    public override void _Ready()
    {
        var tween = CreateTween();

        tween.TweenProperty(this, "modulate:a", 0, time);

        tween.TweenCallback(Callable.From(() => CallDeferred(MethodName.QueueFree)));

    }

    public void SetDamage(int damage)
    {
        Text = damage.ToString();
    }



}