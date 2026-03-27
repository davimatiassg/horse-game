using System;
using System.Collections.Generic;
using System.Linq;
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

    [Export] public int difficulty = 5;


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
            credits += difficulty;
        }
    }  

    const float ELITE_SPAWN_CHANCE = 0.05f;

    private void TriggerSpawn()
    {
        List<PackedScene> enemiesToSpawn = BuyEnemies();

        foreach(PackedScene enemy in enemiesToSpawn)
        {
            Node2D enemyInstance = enemy.Instantiate<Node2D>();
            GetTree().Root.CallDeferred(MethodName.AddChild, enemyInstance);
            
            enemyInstance.GlobalPosition = player.GlobalPosition + new Vector2(rng.NextSingle() - 0.5f, rng.NextSingle() - 0.5f).Normalized() * 640;
        
            if(rng.NextSingle() <= ELITE_SPAWN_CHANCE)
            {
                enemyInstance.Scale = Vector2.One*3;
                if(enemyInstance is Enemy enemyController)
                {
                    enemyController.MaxHP *= 3;
                }
            }
        
        
        }




    }

    const int MAX_BUY_ATTEMPTS = 16;
    private List<PackedScene> BuyEnemies()
    {
        List<PackedScene> enemiesToSpawn = new();
        int remainingCredits = credits;

        int buyAttempts = 0;
        while(remainingCredits > 0 && buyAttempts < MAX_BUY_ATTEMPTS)
        {
            int index = rng.Next()%enemies.Count;
            PackedScene enemy = enemies.Keys.ToArray()[index];
            int value = enemies[enemy];
            if(value <= remainingCredits)
            {
                enemiesToSpawn.Add(enemy);
                remainingCredits -= value;
            }
            else buyAttempts++;
        }

        credits = remainingCredits;

        return enemiesToSpawn;
    }
}