using Godot;
using Godot.Collections;


[Tool]
public partial class CameraController : Camera2D
{
 

    [Export] private Node2D player;

    // Lista de áreas retangulares
    [Export] public Godot.Collections.Array<Rect2> Areas
    {
        get => _areas;
        set
        {
            _areas = value;
            QueueRedraw();
        }
        
    }
    private Godot.Collections.Array<Rect2> _areas = new();
    private Rect2? currentArea = null;

    public override void _Ready()
    {

    }

    public override void _Process(double delta)
    {
        if (player == null)
            return;

        Vector2 playerPos = player.GlobalPosition;

        // Verifica em qual área o jogador está
        Rect2? foundArea = null;

        foreach (var area in Areas)
        {
            if (area.HasPoint(playerPos))
            {
                foundArea = area;
                break;
            }
        }

        currentArea = foundArea;

        // Segue o jogador
        

        // Se estiver dentro de uma área, limita a câmera
        if (currentArea != null)
        {
            Vector2 targetPos = playerPos;

            Rect2 area = currentArea.Value;

            float halfWidth = GetViewportRect().Size.X / 2;
            float halfHeight = GetViewportRect().Size.Y / 2;

            targetPos.X = Mathf.Clamp(playerPos.X,
                Mathf.Min(area.Position.X + halfWidth, area.End.X - halfWidth), 
                Mathf.Max(area.Position.X + halfWidth, area.End.X - halfWidth) 
            );

            targetPos.Y = Mathf.Clamp(playerPos.Y,
                Mathf.Min(area.Position.Y + halfHeight, area.End.Y - halfHeight), 
                Mathf.Max(area.Position.Y + halfHeight, area.End.Y - halfHeight) 
            );

            GlobalPosition = targetPos;
        }

        
    }




    public override void _Draw()
    {
        if (!Engine.IsEditorHint())
            return;
        
        int i = 0;
        foreach (var rect in Areas)
        {
            
            DrawRect(rect, Color.FromHsv(i*71f/360f, 1, 0.5f, 0.15f), true);
            i ++;
        }
    }
}