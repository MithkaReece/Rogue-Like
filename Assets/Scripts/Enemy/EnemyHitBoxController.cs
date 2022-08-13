using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitBoxController : MonoBehaviour
{
    [SerializeField] private EnemyController enemy;

    public void TakeDamage(DamageReport dr) { enemy.TakeDamage(dr); }
}
