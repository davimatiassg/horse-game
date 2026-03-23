using Godot;
using Godot.Collections;
using System.Collections.Generic;


public partial class EnemyManager : Node
{
    [ExportGroup("Connections")]
    [Export] public Array<PackedScene> enemiesToSpawn = new();

    [ExportGroup("Stats")]
    [Export] public int directorValue;
}