using Godot;
using System;

public partial class ShieldGeneratorTurret : CartTurret
{
    [ExportGroup("Connections")]
    [Export] private Node2D shieldVisual;

    [ExportGroup("Stats")]
    private bool shieldActive = false;
    private int lastHP;

    public override void Activate()
    {
        if (player == null) return;

        // Só aplica se não tiver escudo ativo
        if (shieldActive) return;

        ActivateShield();
    }

    private void ActivateShield()
    {
        shieldActive = true;

        lastHP = player.HP;

        if (shieldVisual != null)
            shieldVisual.Visible = true;

        AudioPlayer.PlayRandomPitch("shield_on");

        // Escuta mudança de HP
        player.OnChangeHP += OnPlayerHPChanged;
    }

    private void OnPlayerHPChanged(int newHP)
    {
        if (!shieldActive) return;

        // Detecta se perdeu vida (tomou dano)
        if (newHP < lastHP)
        {
            int damageTaken = lastHP - newHP;

            // "Anula" o dano curando de volta
            player.HP += damageTaken;

            BreakShield();
        }

        lastHP = player.HP;
    }

    private void BreakShield()
    {
        shieldActive = false;

        AudioPlayer.PlayRandomPitch("shield_break");

        if (shieldVisual != null)
            shieldVisual.Visible = false;

        player.OnChangeHP -= OnPlayerHPChanged;
    }
}
