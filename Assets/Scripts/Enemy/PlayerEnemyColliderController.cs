using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnemyColliderController : MonoBehaviour
{
    [SerializeField] private EnemyController enemy;
    // Start is called before the first frame update

    void OnCollisionEnter2D(Collision2D collision) { enemy.KnockbackPlayer(collision); }
}
