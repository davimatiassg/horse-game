using Godot;

public partial class DynamicUIManager : Control
{
    public static DynamicUIManager Singleton;

    [Export] public CameraController camera;

    [Export] public PackedScene label;


    public override void _Ready()
    {
        base._Ready();
        if (Singleton == null) Singleton = this;
        else if (Singleton != this) QueueFree();
    }


    public static RichTextLabel SpawnLabel(string text, Vector2 worldPosition)
    {
        var spawnedLabel = Singleton.label.Instantiate<RichTextLabel>();
        Singleton.GetTree().Root.CallDeferred(MethodName.AddChild, spawnedLabel);

        spawnedLabel.Text = text;
        spawnedLabel.GlobalPosition = Singleton.GetViewport().GetGlobalCanvasTransform().AffineInverse() * worldPosition - spawnedLabel.Size / 2f + Vector2.Up*96;
    
        return (RichTextLabel)spawnedLabel;
    }
}