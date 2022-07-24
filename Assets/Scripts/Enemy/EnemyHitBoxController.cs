using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitBoxController : MonoBehaviour
{
    [SerializeField] private EnemyController enemy;

    public void TakeDamage(int damage) { enemy.TakeDamage(damage); }
}
