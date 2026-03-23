using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;

public partial class ScoreManager : Node
{
	public static ScoreManager Singleton;

	[Export] HorseBody player;

	[Export] PackedScene cartPrefab;

	public int playerLevel;
	public int totalScore = 0;


	public override void _Ready()
	{
		if(Singleton == null) Singleton = this;
		else if (Singleton != this) { QueueFree(); return;}
	
	}


	public static void AddScore(int score = 1)
	{
		Singleton.totalScore += score;

		if(Singleton.totalScore > Mathf.Pow(2, Singleton.playerLevel))
		{
			Singleton.playerLevel ++;


			//STUB
		
			var cart = SpawnCart();
			Singleton.player.AddCart(cart);

		}
	}


	public static Cart SpawnCart()
	{
		//STUB
		var cart = Singleton.cartPrefab.Instantiate<Cart>();
		Singleton.GetTree().Root.AddChild(cart);
		return cart;
	}
}
