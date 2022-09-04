using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHelper : MonoBehaviour
{
    [SerializeField] private EnemyController enemy;
    public void EndAttack(int par) { enemy.EndAttack(); }
    public void StartLunge(int par) { enemy.StartLunge(); }
    public void EndLunge(int par) { enemy.EndLunge(); }

    public void EndHit(int par) { enemy.EndHit(); }
    public void EndDie(int par) { enemy.EndDie(); }

    public void StartStun(int par)
    {
        enemy.StartStun();
    }
    public void EndStunMovement(int par) { enemy.EndStunMovement(); }
    public void EndStun(int par)
    {
        enemy.EndStun();
    }

    public void StopBlocking(int par) { enemy.StopBlocking(); }

}
