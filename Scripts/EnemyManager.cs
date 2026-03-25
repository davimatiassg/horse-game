using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Godot;
using Godot.Collections;



public partial class EnemyManager : Node
{
    [ExportGroup("Connections")]
    [Export] public Godot.Collections.Dictionary<PackedScene, int> enemies = new();

    [Export] public HorseBody player;

    [ExportGroup("Stats")]
    [Export] public int credits;

    [Export] public int step = 5;


    [Export] public float chanceConstant = 0.95f;
    

    private float chance;
    Tween ActivationTween;

    Random rng = new();

    public override void _Process(double delta)
    {
        base._Process(delta);
        if(ActivationTween == null || !ActivationTween.IsRunning())
        {
            
            AttemptTrigger();


            ActivationTween = CreateTween();
            ActivationTween.TweenInterval(4);

            
        }
    }


    public void AttemptTrigger()
    {
        if( rng.NextSingle() > chance )
        {
            TriggerSpawn();
            chance = chanceConstant;
        }
        else
        {
            chance *= chanceConstant;
            credits += step;
        }
    }

    private void TriggerSpawn()
    {
        List<PackedScene> enemiesToSpawn = BuyEnemies();

        foreach(PackedScene enemy in enemiesToSpawn)
        {
            Node2D enemyInstance = enemy.Instantiate<Node2D>();
            GetTree().Root.CallDeferred(MethodName.AddChild, enemyInstance);
            
            enemyInstance.GlobalPosition = player.GlobalPosition + new Vector2(rng.NextSingle() - 0.5f, rng.NextSingle() - 0.5f).Normalized() * 1280;
        }
    }

    private List<PackedScene> BuyEnemies()
    {
        List<PackedScene> enemiesToSpawn = new();
        int remainingCredits = credits;
        foreach(var enemySpawn in enemies)
        {
            int parcelToSpend = (int)(remainingCredits * rng.NextSingle());
            int ammountOfEnemies = parcelToSpend / enemySpawn.Value;
            remainingCredits -= ammountOfEnemies * enemySpawn.Value;

            for(int i = 0; i < ammountOfEnemies; i++) enemiesToSpawn.Add(enemySpawn.Key);
        }

        credits = remainingCredits;

        return enemiesToSpawn;
    }
}