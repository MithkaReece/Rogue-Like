using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHelper : MonoBehaviour
{
    [SerializeField] private EnemyController enemy;
    public void EndAttack(int par) { enemy.EndAttack(); }
    public void StartLunge(int par) { enemy.StartLunge(); }
    public void EndLunge(int par) { enemy.EndLunge(); }

}
