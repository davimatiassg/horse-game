using Godot;

public partial class GameManager : Node
{
    public static GameManager Singleton;

    public override void _Ready()
    {
        base._Ready();
       	if(Singleton == null) Singleton = this;
		else if (Singleton != this) { QueueFree(); return;}
    }


    public static void PauseGame(bool paused)
    {
        Singleton.GetTree().Paused = paused;
    }
}