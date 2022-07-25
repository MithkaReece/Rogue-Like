using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : EntityController
{
    [SerializeField] private float scale = 4f;

    private Animator bodyAnimator;

    private bool canMove = true;

    private SpriteRenderer sr;
    private Sprite[] sprites;
    [SerializeField] private string swordEquiped = "Sword3";
    private float swapCooldown = 2f;
    private float swapCooldownCounter = 0f;

    private PlayerStats playerStats;
    private State state;
    private enum State
    {
        Normal,
        DodgeRoll,
        Attack,
        Attack2,
        Attack3,
        Knockback,
    }

    private MultiAttacks attacks;

    private AttackState attackState;
    private enum AttackState
    {
        Idle,
        CanAttack,
        DoAttack,
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        attacks = new MultiAttacks(new string[] { "Attack1", "Attack2", "Attack3" });

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
                HandleNextAttack();
                break;
        }
    }


    [SerializeField] private float moveSpeed;
    private Vector2 inputDirection;
    void HandleWalk()
    {
        inputDirection = (new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"))).normalized;
        //drb.MovePosition(drb.position + inputDirection * moveSpeed * Time.deltaTime);
        rb.velocity = inputDirection * moveSpeed;

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
            if (swordEquiped == "Sword3")
            {
                Debug.Log("To sword2");
                SwapSword("Sword2");
            }
            else
            {
                Debug.Log("To sword3");
                SwapSword("Sword3");
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
            state = State.Attack;
            bodyAnimator.SetTrigger(attacks.GetNextAttack());
            playerStats.Combat.AttackCooldownCounter.Reset(1f / playerStats.Combat.AttackSpeed.Value);
        }
    }

    void HandleNextAttack()
    {
        if (Input.GetButton("Attack1")) { attacks.ReceiveInput(); }
        if (attacks.CanDoNextAttack())
        {
            Debug.Log(attacks.GetNextAttack());
            bodyAnimator.SetTrigger(attacks.GetNextAttack());
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
            collider.gameObject.GetComponent<EnemyHitBoxController>().TakeDamage(40);
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

    public void StartAttack(int par)
    {  
        //Set sword sprite
        sr.sprite = sprites[0];
    }

    public void EndAttack(int par)
    {
        state = State.Normal;
        attacks.HardReset();
        sr.sprite = null;
    }

    public void ReadyForAttackInput(int par)
    {
        attacks.ReadyNextInput();
    }

    public void ReadyForNextAttack(int par)
    {
        attacks.DoNextAttack();
    }
}





public class MultiAttacks
{
    private bool nextInputReady;
    private bool inputReceived;
    private bool canDoNextAttack;

    private int nextAttackIndex = 0;
    private string[] attacks;

    public MultiAttacks(string[] attacks)
    {
        this.attacks = attacks;
    }

    //Ready to receive input for next attack
    public void ReadyNextInput()
    {
        nextInputReady = true;
        nextAttackIndex = Mathf.Min(attacks.Length - 1, nextAttackIndex + 1);
    }

    //Next attack input is received when it is ready to be received
    public void ReceiveInput() { inputReceived = nextInputReady; }

    public void DoNextAttack() { canDoNextAttack = true; }

    public bool CanDoNextAttack() { return inputReceived && canDoNextAttack; }
    public string GetNextAttack()
    {
        Reset();
        if (nextAttackIndex < attacks.Length)
        {
            return attacks[nextAttackIndex];
        }
        return null;
    }
    //Reset for next attack
    private void Reset()
    {
        nextInputReady = false;
        inputReceived = false;
        canDoNextAttack = false;
    }
    //Reset back to first attack
    public void HardReset()
    {
        Reset();
        nextAttackIndex = 0;
    }

}