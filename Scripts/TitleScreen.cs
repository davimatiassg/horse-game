using Godot;
using System;

public partial class TitleScreen : Control
{
	[ExportGroup("Connections")]
	[Export] Node3D horse;

	[Export] Camera3D camera;
	[Export] PackedScene gameScene;

	private bool pressed;
    public override void _Ready()
    {
        FocusMode = FocusModeEnum.All;
        GrabFocus();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey keyEvent && keyEvent.Pressed && !keyEvent.Echo)
        {
            OnAnyKeyPressed(keyEvent);
        }
    }

    private void OnAnyKeyPressed(InputEventKey keyEvent)
    {
		if(pressed) return;
		pressed = true;
       
		Tween tween = CreateTween();

		tween.TweenCallback(Callable.From(() =>
		{	
			GetTree().Root.AddChild(gameScene.Instantiate());
		}
		));

		tween.TweenProperty(horse, "position", new Vector3(-1.135f, 0.15f, 0.28f), 2f).SetTrans(Tween.TransitionType.Spring);

		tween.TweenCallback(Callable.From(() => CallDeferred(MethodName.QueueFree)));
	
        
    }
}
