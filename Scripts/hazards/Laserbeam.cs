using Godot;
using System;

public partial class Laserbeam : Line2D
{

	[ExportGroup("Connections")]
    [Export] public Node2D source;
    [Export] public Node2D target;
    
    [ExportGroup("Connections/Shaders")]
    [Export] public Shader ElectricityShader;
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

        Points = [source.GlobalPosition, target.GlobalPosition];
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