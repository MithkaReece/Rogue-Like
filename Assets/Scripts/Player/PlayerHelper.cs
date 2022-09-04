using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHelper : MonoBehaviour
{
    [SerializeField] private PlayerController player;

    public void StartAttack(int par) { player.StartAttack(); }
    public void StartAttackLunge(int par) { player.StartAttackLunge(); }
    public void EndAttackLunge(int par) { player.EndAttackLunge(); }
    public void EndAttack(int par) { player.EndAttack(); }
    public void ReadyForAttackInput(int par) { player.ReadyForAttackInput(); }
    public void ReadyForNextAttack(int par) { player.ReadyForNextAttack(); }


    public void StartInv(int par) { player.StartInv(); }
    public void EndInv(int par) { player.EndInv(); }
    public void EndRoll(int par) { player.EndRoll(); }

    public void EndParry(int par) { player.EndParry(); }

    public void EndHit(int par) { player.EndHit(); }

    public void StopBlocking(int par) { player.StopBlocking(); }

}
