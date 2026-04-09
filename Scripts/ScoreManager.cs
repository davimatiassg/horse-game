using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;

public partial class ScoreManager : Node
{
	public static ScoreManager Singleton;

	[Export] HorseBody player;

	[Export] CharrietBuyMenu charrietBuyMenu;

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

		if(Singleton.totalScore+1 >= Mathf.Pow(1.5, Singleton.player.carts.Count))
		{
			

			var cart = SpawnCart();
			cart.GlobalPosition = Singleton.player.GlobalPosition;
			Singleton.player.AddCart(cart);
			GameManager.PauseGame(true);
			Singleton.charrietBuyMenu.Visible = true;


			void ConfirmCartChoice()
			{
				GameManager.PauseGame(false);
				Singleton.charrietBuyMenu.Visible = false;
				var turret = Singleton.charrietBuyMenu.SelectedOption.turret.Instantiate<CartTurret>();

				AudioPlayer.PlayRandomPitch("new_cart");

				cart.AddTurret(turret);
				Singleton.charrietBuyMenu.confirmButton.Pressed -= ConfirmCartChoice;
			}


			Singleton.charrietBuyMenu.confirmButton.Pressed += ConfirmCartChoice;
			

		}
	}


	public static Cart SpawnCart()
	{
		//STUB
		var cart = Singleton.cartPrefab.Instantiate<Cart>();
		Singleton.GetTree().Root.CallDeferred(MethodName.AddChild, cart);
		return cart;
	}
}
