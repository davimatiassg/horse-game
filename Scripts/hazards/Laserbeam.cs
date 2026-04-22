using Godot;
using System;

public partial class Laserbeam : Line2D
{

	[ExportGroup("Connections")]
    [Export] public Node2D source;
    [Export] public Node2D target;
    
    [ExportGroup("Connections/Shaders")]
    [Export] public Shader ElectricityShader;
    [Export] public Texture2D texture1;
    [Export] public Texture2D texture2;
    [Export] public Texture2D texture3;
    [Export] public Shader TransparentShader;

	[ExportGroup("stats")]
	[Export] private float deactivatedTime = 2;
	[Export] private float activatedTime = 1;

	[Export] private int damage = 60;
    private ShaderMaterial _shaderMaterial;
    private bool _isVisible = true;

    public override void _Ready()
    {
        // Inicializa o material se não existir
        if (Material == null)
        {
            _shaderMaterial = new ShaderMaterial();
            Material = _shaderMaterial;
        }
        else
        {
            _shaderMaterial = (ShaderMaterial)Material;
        }

        _shaderMaterial.SetShaderParameter("tex1", texture1);
        _shaderMaterial.SetShaderParameter("tex2", texture2);
        _shaderMaterial.SetShaderParameter("tex3", texture3);
        _shaderMaterial.SetShaderParameter("tile_count", source.GlobalPosition.DistanceTo(target.GlobalPosition)/texture1.GetSize().X);

        source.LookAt(target.GlobalPosition);
        target.LookAt(source.GlobalPosition);

        ToggleLaser();
        

		Tween alternanceTween = CreateTween();
		
		alternanceTween.TweenInterval(deactivatedTime);
		alternanceTween.TweenCallback(Callable.From(ToggleLaser));
		alternanceTween.TweenInterval(activatedTime);
		alternanceTween.TweenCallback(Callable.From(ToggleLaser));
		alternanceTween.SetLoops();
    }

    public override void _Process(double delta)
    {

        Points = [source.Position, target.Position];
        if(_isVisible) PerformRaycast   ();
    }

    private void ToggleLaser()
    {
        _isVisible = !_isVisible;
        _shaderMaterial.Shader = _isVisible ? ElectricityShader : TransparentShader;

		if(_isVisible)
		{
			PerformRaycast();
		}
    }

    private void PerformRaycast()
    {
        var spaceState = GetWorld2D().DirectSpaceState;
        var query = PhysicsRayQueryParameters2D.Create(source.GlobalPosition, target.GlobalPosition);
        var result = spaceState.IntersectRay(query);

        if (result.Count > 0 && result["collider"].Obj is IHittable hittable)
        {
            hittable.TakeDamage(10);
        }
    }
}