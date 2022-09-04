using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helper : MonoBehaviour
{
    [SerializeField] protected EntityController entity;
    public void StartAttackLunge(int par) { entity.StartAttackLunge(); }
    public void EndAttackLunge(int par) { entity.EndAttackLunge(); }
    public void EndAttack(int par) { entity.EndAttack(); }

    public void EndHit(int par) { entity.EndHit(); }
    public void EndDie(int par) { entity.EndDie(); }

    public void StartStun(int par) { entity.StartStun(); }
    public void EndStunMovement(int par) { entity.EndStunMovement(); }
    public void EndStun(int par) { entity.EndStun(); }

    public void StopBlocking(int par) { entity.StopBlocking(); }

    public void StartInv(int par) { entity.StartInv(); }
    public void EndInv(int par) { entity.EndInv(); }
    public void EndRoll(int par) { entity.EndRoll(); }

    public void EndParry(int par) { entity.EndParry(); }

}
