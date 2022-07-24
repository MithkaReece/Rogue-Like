using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : EntityController
{
    [SerializeField] private float scale = 4f;
    private Rigidbody2D rb;
    private Rigidbody2D drb;
    private Rigidbody2D krb;

    private Animator bodyAnimator;

    private bool canMove = true;

    private SpriteRenderer sr;
    private Sprite[] sprites;
    [SerializeField] private string swordEquiped = "Sword1";
    private float swapCooldown = 2f;
    private float swapCooldownCounter = 0f;

    private PlayerStats playerStats;
    private State state;
    private enum State
    {
        Normal,
        DodgeRoll,
        Attack,
        Knockback,
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        drb = GetComponent<Rigidbody2D>();
        krb = transform.Find("Blocker").GetComponent<Rigidbody2D>();


        rb = GetComponent<Rigidbody2D>();
        bodyAnimator = transform.Find("Body").GetComponent<Animator>();
        currentHealth = maxHealth;
        base.Start();

        bodyAnimator = transform.Find("Body").GetComponent<Animator>();
        sr = transform.Find("Body").transform.Find("Sword2").GetComponent<SpriteRenderer>();
        sprites = Resources.LoadAll<Sprite>("Swords/" + swordEquiped);
        playerStats = GetComponent<PlayerStats>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.Normal:
                if (canMove)
                {
                    HandleWalk();
                    LeftRightFlip();
                    HandleSwordSwap();
                    HandleAttack();
                    HandleDodgeRoll();
                }
                break;
            case State.DodgeRoll:
                HandleDodgeRollMotion();
                break;
            case State.Attack:
                break;
        }
    }

    [SerializeField] private float moveSpeed;
    private Vector2 inputDirection;
    void HandleWalk()
    {
        inputDirection = (new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"))).normalized;
        //drb.MovePosition(drb.position + inputDirection * moveSpeed * Time.deltaTime);
        drb.velocity = inputDirection * moveSpeed;

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

    //Allows right animation to be flipped and used as left
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
    private void HandleSwordSwap()
    {
        if (swapCooldownCounter > 0) { swapCooldownCounter -= Time.deltaTime; }
        if (Input.GetButton("Attack2") && swapCooldownCounter <= 0f)
        {
            swapCooldownCounter = swapCooldown;
            if (swordEquiped == "Sword1")
            {
                Debug.Log("To sword2");
                SwapSword("Sword2");
            }
            else
            {
                Debug.Log("To sword1");
                SwapSword("Sword1");
            }
        }
    }
    void SwapSword(string newSword)
    {
        swordEquiped = newSword;
        sprites = Resources.LoadAll<Sprite>("Swords/" + newSword);
    }

    private void HandleAttack()
    {
        if (!playerStats.Combat.AttackCooldownCounter.Passed)
            playerStats.Combat.AttackCooldownCounter.PassTime(Time.deltaTime);

        if (Input.GetButton("Attack1") && playerStats.Combat.AttackCooldownCounter.Passed)
        {
            rb.velocity = Vector2.zero;
            bodyAnimator.SetTrigger("Attack2");
            playerStats.Combat.AttackCooldownCounter.Reset(1f / playerStats.Combat.AttackSpeed.Value);
        }
    }


    void HandleDodgeRoll()
    {
        if (!playerStats.RollCooldownCounter.Passed)
            playerStats.RollCooldownCounter.PassTime(Time.deltaTime);

        if (Input.GetButton("Jump") && playerStats.RollCooldownCounter.Passed)
        {
            bodyAnimator.SetTrigger("Roll");
            playerStats.RollCooldownCounter.Reset(playerStats.RollCooldown);
            canMove = false;
            state = State.DodgeRoll;
        }
    }
    [SerializeField] private float rollSpeed = 3f;
    void HandleDodgeRollMotion()
    {
        rb.velocity = new Vector2((transform.localScale.x / Mathf.Abs(transform.localScale.x)) * rollSpeed, 0);
        /*if (transform.localScale.x < 0)
        {
            transform.position += new Vector3(-1, 0) * rollSpeed * Time.deltaTime;
        }
        else
        {
            transform.position += new Vector3(1, 0) * rollSpeed * Time.deltaTime;
        }*/

    }
    public void EndRoll(int par)
    {
        canMove = true;
        state = State.Normal;
    }

    [SerializeField] private LayerMask enemyLayers;
    //So far the only trigger is the collider around the sword when swinging
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (enemyLayers == (enemyLayers | (1 << collider.gameObject.layer)))
        {
            Debug.Log("Hit");
            collider.gameObject.GetComponent<EnemyHitBoxController>().TakeDamage(entityStats.Combat.Damage.Value);
        }
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 8)
        {
            Physics2D.IgnoreCollision(collision.collider, GetComponent<CapsuleCollider2D>());
        }
    }


    public IEnumerator Knockback(float knockbackDuration, float knockbackPower, Vector2 objPos)
    {
        //bodyAnimator.SetTrigger("Knockback");
        state = State.Knockback;
        canMove = false;
        Vector2 direction = (objPos - ((Vector2)transform.position + GetComponent<Collider2D>().offset)).normalized;
        rb.AddForce(-direction * knockbackPower, ForceMode2D.Impulse);
        yield return new WaitForSeconds(knockbackDuration);
        rb.velocity = Vector2.zero;
        canMove = true;
        state = State.Normal;
    }

    private Sprite sword1;
    public void DisplaySword1(int par)
    {
        state = State.Attack;
        sr.sprite = sprites[0];
    }
    private Sprite sword2;
    public void DisplaySword2(int par)
    {
        sr.sprite = sprites[1];
    }
    private Sprite sword3;
    public void DisplaySword3(int par)
    {
        sr.sprite = sprites[2];
    }

    public void DisplayNothing(int par)
    {
        state = State.Normal;
        sr.sprite = null;
    }
}
