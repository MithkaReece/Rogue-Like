using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    [SerializeField] private float moveSpeed;

    [SerializeField] private PlayerController player;
    public void SetPlayer(PlayerController inPlayer) { player = inPlayer; }
    private bool playerSeen = true;


    [SerializeField] private float knockbackDuration;
    [SerializeField] private float knockbackPower;

    private Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
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

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log(currentHealth);
        //Hurt animation

        if (currentHealth <= 0)
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
            player.TakeDamage(40);
            //StartCoroutine(player.Knockback(knockbackDuration, knockbackPower, (Vector2)transform.position + GetComponent<Collider2D>().offset));
        }
    }
}
