using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;

public partial class ScoreManager : Node
{
	public static ScoreManager Singleton;

	[Export] HorseBody player;

	//STUB
	[Export] PackedScene cartPrefab;
	Stack<Node2D> cartStack = new();

	public int playerLevel;
	public int totalScore = 0;


	public override void _Ready()
	{
		if(Singleton == null) Singleton = this;
		else if (Singleton != this) { QueueFree(); return;}

		cartStack.Push(Singleton.player.cartStump);
	
	}


	public static void AddScore(int score = 1)
	{
		Singleton.totalScore += score;

		if(Singleton.totalScore > Mathf.Pow(2, Singleton.playerLevel))
		{
			Singleton.playerLevel ++;


			//STUB
		
			var cart = SpawnCart();
			cart.AttachTo(Singleton.cartStack.Peek());
			Singleton.cartStack.Push(cart.stump);
		}
	}


	public static Cart SpawnCart()
	{
		//STUB
		var cart = Singleton.cartPrefab.Instantiate<Cart>();
		Singleton.GetParent().AddChild(cart);
		return cart;
	}
}
