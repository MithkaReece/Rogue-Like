using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public float attackStartAngle = 30f;
    public float attackSwingAngle = 110f;
    public LayerMask enemyLayers;

    [SerializeField] private float speed;
    [SerializeField] private float scale = 4f;
    private Vector2 direction;

    private Rigidbody2D rb;

    private Animator bodyAnimator;
    private Animator swordAnimator;

    private float AttackCooldown = 0.3f;
    private float AttackCooldownCounter = 0f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bodyAnimator = GetComponent<Animator>();
        swordAnimator = transform.Find("Sword").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        TakeInput();
        LeftRightFlip();
        Move();


        if (AttackCooldownCounter > 0)
        {
            AttackCooldownCounter -= Time.deltaTime;
        }

    }

    void TakeInput()
    {
        direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        direction = direction.normalized;

        if (Input.GetButton("Jump"))
        {
            Attack();
        }
    }

    void LeftRightFlip()
    {
        if (direction.x < 0)
        {
            transform.localScale = new Vector2(-scale, scale);
        }
        else if (direction.x > 0)
        {
            transform.localScale = new Vector2(scale, scale);
        }
    }

    void Move()
    {
        rb.MovePosition(rb.position + direction * speed * Time.deltaTime);


        if (direction.magnitude == 0)
        { //Stop walk animation as not moving
            bodyAnimator.SetLayerWeight(1, 0);
        }
        else
        {
            bodyAnimator.SetLayerWeight(1, 1);
            SetAnimatorMovement(direction);
        }


    }

    void SetAnimatorMovement(Vector2 direction)
    {
        bodyAnimator.SetFloat("xDir", direction.x);
        bodyAnimator.SetFloat("yDir", direction.y);
    }


    void Attack()
    {
        if (AttackCooldownCounter > 0) { return; }
        swordAnimator.SetTrigger("Attack");
        //Reset cooldown
        AttackCooldownCounter = AttackCooldown;
        /*
        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider2D enemy in enemiesHit)
        {
            Vector2 direction = (Vector2)attackPoint.position - enemy.ClosestPoint(attackPoint.position);
            float angle = Vector2.SignedAngle(new Vector2(0, 1), direction);
            //Debug.Log(angle);
            float resultAngle;
            //Debug.Log(transform.localScale.x);
            if (transform.localScale.x >= 0)
            {
                resultAngle = angle - attackStartAngle;
                Debug.Log(resultAngle);
                if (resultAngle < 0 || resultAngle > attackSwingAngle) { return; }
            }
            else
            {
                resultAngle = angle + attackStartAngle;
                Debug.Log(resultAngle);
                if (resultAngle > 0 || resultAngle < -attackSwingAngle) { return; }
            }


            enemy.GetComponent<EnemyController>().TakeDamage(50);
            //Debug.Log("We hit" + enemy.name);
    }*/
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint)
        {
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (enemyLayers == (enemyLayers | (1 << collider.gameObject.layer)))
        {
            collider.gameObject.GetComponent<EnemyController>().TakeDamage(50);
        }

    }
}
