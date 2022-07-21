using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float scale = 4f;
    private Rigidbody2D rb;
    private Animator bodyAnimator;

    private bool canMove = true;

    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bodyAnimator = GetComponent<Animator>();
        swordAnimator = transform.Find("Sword").GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        TakeInput();
        LeftRightFlip();
        if (canMove) { Move(); }



        if (AttackCooldownCounter > 0)
        {
            AttackCooldownCounter -= Time.deltaTime;
        }

    }
    private Vector2 inputDirection;
    void TakeInput()
    {
        inputDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        inputDirection = inputDirection.normalized;

        if (Input.GetButton("Jump"))
        {
            Attack();
        }
    }

    void LeftRightFlip()
    {
        if (inputDirection.x < 0)
        {
            transform.localScale = new Vector2(-scale, scale);
        }
        else if (inputDirection.x > 0)
        {
            transform.localScale = new Vector2(scale, scale);
        }
    }
    [SerializeField] private float moveSpeed;
    void Move()
    {
        rb.MovePosition(rb.position + inputDirection * moveSpeed * Time.deltaTime);

        if (inputDirection.magnitude == 0)
        { //Stop walk animation as not moving
            bodyAnimator.SetLayerWeight(1, 0);
        }
        else
        {
            bodyAnimator.SetLayerWeight(1, 1);
            SetAnimatorMovement(inputDirection);
        }
    }

    void SetAnimatorMovement(Vector2 direction)
    {
        bodyAnimator.SetFloat("xDir", direction.x);
        bodyAnimator.SetFloat("yDir", direction.y);
    }

    private Animator swordAnimator;
    private float AttackCooldown = 0.3f;
    private float AttackCooldownCounter = 0f;
    void Attack()
    {
        if (AttackCooldownCounter > 0) { return; }
        swordAnimator.SetTrigger("Attack");
        //Reset cooldown
        AttackCooldownCounter = AttackCooldown;
    }
    [SerializeField] private LayerMask enemyLayers;
    //So far the only trigger is the collider around the sword when swinging
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (enemyLayers == (enemyLayers | (1 << collider.gameObject.layer)))
        {
            collider.gameObject.GetComponent<EnemyController>().TakeDamage(50);
        }
    }


    public IEnumerator Knockback(float knockbackDuration, float knockbackPower, Vector2 objPos)
    {
        canMove = false;
        rb.isKinematic = false;
        Vector2 direction = (objPos - ((Vector2)transform.position + GetComponent<Collider2D>().offset)).normalized;
        rb.AddForce(-direction * knockbackPower, ForceMode2D.Impulse);
        yield return new WaitForSeconds(knockbackDuration);
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        canMove = true;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
    }
}
