using Godot;
using System;

public partial class TurretBasket : CartTurret
{
    [ExportGroup("Connections")]

    [Export] private PackedScene healArea;

    [ExportGroup("Stats")]
    [Export] public float duration = 5;
	[Export] public int healPerTick = 1;
	[Export] public float tickTime = 0.1f;

    public override void Activate()
    {
        HealArea area = healArea.Instantiate<HealArea>();
        area.duration = duration;
        area.healPerTick = healPerTick;
        area.tickTime = tickTime;
        
        AudioPlayer.PlayRandomPitch("heal_zone");

        GetTree().Root.AddChild(area);
        area.GlobalPosition = this.GlobalPosition;
    }
}