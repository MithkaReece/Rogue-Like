using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHelper : MonoBehaviour
{
    [SerializeField] private PlayerController player;

    public void StartAttack(int par) { player.StartAttack(par); }
    public void EndAttack(int par) { player.EndAttack(par); }
    public void ReadyForAttackInput(int par) { player.ReadyForAttackInput(par); }
    public void ReadyForNextAttack(int par) { player.ReadyForNextAttack(par); }


    public void EndRoll(int par) { player.EndRoll(par); }
}
