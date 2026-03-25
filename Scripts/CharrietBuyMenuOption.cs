using Godot;
using System;


[GlobalClass]
public partial class CharrietBuyMenuOption : Resource
{
    [Export] public PackedScene turret;

    [Export] public string description;

    [Export] public Texture2D thumbnail;

    [Export] public string name;
}