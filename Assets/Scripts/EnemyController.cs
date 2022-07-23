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
        entityStats = GetComponent<EntityStats>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move()
    {
        if (!playerSeen) { return; }
        Vector2 direction = player.transform.position - transform.position;
        direction = direction.normalized;
        rb.MovePosition(rb.position + direction * entityStats.MoveSpeed.Value * Time.deltaTime);
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

        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
    }

    [SerializeField] private LayerMask enemyLayers;
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (enemyLayers == (enemyLayers | (1 << collision.collider.gameObject.layer)))
        {
            player.TakeDamage(entityStats.Combat.Damage.Value);
            StartCoroutine(player.Knockback(enemyStats.knockbackDuration.Value, enemyStats.knockbackPower.Value, (Vector2)transform.position + GetComponent<Collider2D>().offset));
        }
    }
}
