using Godot;

public partial class MainCamera : Camera2D
{
    [Export]
    public Node2D Target;

    [Export]
    public float FollowSpeed = 5f;

    public override void _Process(double delta)
    {
        if (Target == null) return;

        GlobalPosition = GlobalPosition.MoveToward(
            Target.GlobalPosition,
            (float)(FollowSpeed * delta)
        );
    }
}