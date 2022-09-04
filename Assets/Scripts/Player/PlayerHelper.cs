using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHelper : Helper
{
    private PlayerController player
    {
        get { return (PlayerController)entity; }
    }

    public void StartAttack(int par) { player.StartAttack(); }
    public void ReadyForAttackInput(int par) { player.ReadyForAttackInput(); }
    public void ReadyForNextAttack(int par) { player.ReadyForNextAttack(); }
}
