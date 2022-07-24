using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : EntityController
{
    private EnemyStats enemyStats;

    [SerializeField] private PlayerController player;
    public void SetPlayer(PlayerController inPlayer) { player = inPlayer; }
    private bool playerSeen = true;

    // Start is called before the first frame update
    protected override void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyStats = GetComponent<EnemyStats>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        if (!playerSeen) { return; }
        Vector2 direction = player.transform.position - transform.position;
        direction = direction.normalized;
        //rb.MovePosition(rb.position + direction * moveSpeed * Time.deltaTime);
        rb.velocity = direction * moveSpeed;
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        Debug.Log(entityStats.CurrentHealth);
        //Hurt animation

        if (entityStats.CurrentHealth <= 0)
        {
            Die();
            //Die animation

            //Disable enemy
        }
    }
    private void Die()
    {
        Debug.Log("Enemy died");
        //Die animation
        rb.velocity = Vector2.zero;
        GetComponent<CapsuleCollider2D>().enabled = false;
        transform.Find("Collision Blocker").GetComponent<CapsuleCollider2D>().enabled = false;
        this.enabled = false;
    }

    [SerializeField] private LayerMask enemyLayers;
    /*void OnCollisionEnter2D(Collision2D collision)
    {
        if (enemyLayers == (enemyLayers | (1 << collision.collider.gameObject.layer)))
        {
            player.TakeDamage(40);
            //StartCoroutine(player.Knockback(knockbackDuration, knockbackPower, (Vector2)transform.position + GetComponent<Collider2D>().offset));
        }
    }*/

    public void KnockbackPlayer(Collision2D collision)
    {
        if (enemyLayers == (enemyLayers | (1 << collision.collider.gameObject.layer)))
        {
            player.TakeDamage(entityStats.Combat.Damage.Value);
            Debug.Log(enemyStats.KnockbackDuration.Value);
            StartCoroutine(player.Knockback(enemyStats.KnockbackDuration.Value, enemyStats.KnockbackPower.Value, (Vector2)transform.position + GetComponent<Collider2D>().offset));
        }
    }
}
