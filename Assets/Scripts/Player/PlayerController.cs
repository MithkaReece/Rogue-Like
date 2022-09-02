using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : EntityController
{
    [SerializeField] private float scale = 1f;

    //True when movement causes transition back to default
    private bool canMove = true;

    private SpriteRenderer swordSR;

    [SerializeField] private string swordEquiped;
    private float swapCooldown = 2f;
    private float swapCooldownCounter = 0f;

    private PlayerStats playerStats;
    private State state;
    private enum State
    {
        Default,
        DodgeRoll,
        Attack,
        Knockback,
        Block,
        Parry,
        Hit,
        Die,
    }

    private MultiAttacks attacks;

    private AttackState attackState;
    private enum AttackState
    {
        Idle,
        CanAttack,
        DoAttack
    }

    [SerializeField] private GameObject sword;

    private Sprite[] HealthRings;
    private SpriteRenderer HealthRingSR;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        //Setup consecutive attack system
        attacks = new MultiAttacks(new string[] { "Attack1", "Attack2", "Attack3" });

        bodyAnimator = body.GetComponent<Animator>();
        swordSR = sword.GetComponent<SpriteRenderer>();
        playerStats = GetComponent<PlayerStats>();
        //Equip first weapon
        SwapSword(swordEquiped);

        HealthRingSR = healthRing.GetComponent<SpriteRenderer>();
        HealthRings = Resources.LoadAll<Sprite>("Health Ring");

    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.Default:
                //Hides sword when not using it
                swordSR.sprite = null;
                HandleWalk();
                LeftRightFlip();
                HandleSwordSwap();
                HandleNextAttack();
                HandleParryBlock();
                HandleDodgeRoll();
                break;
            case State.Attack:
                HandleNextAttack();
                HandleAttackLunge();
                HandleQuickMove();
                break;
            case State.DodgeRoll:
                HandleDodgeRollMotion();
                break;
            case State.Block:
                HandleParryBlocking();
                break;
        }
    }

    void FixedUpdate()
    {
        UpdateHealthRing();
    }

    void UpdateHealthRing()
    {
        double Percent = 100 * playerStats.CurrentHealth / playerStats.MaxHealth.Value;
        int index = 0;
        if (Percent < 90)
        {
            index++;
        }
        if (Percent < 75)
        {
            index++;
        }
        if (Percent < 55)
        {
            index++;
        }
        if (Percent < 45)
        {
            index++;
        }
        if (Percent < 30)
        {
            index++;
        }
        if (Percent < 20)
        {
            index++;
        }
        if (Percent < 10)
        {
            index++;
        }
        if (Percent < 5)
        {
            index++;
        }
        if (Percent <= 0)
        {
            index++;
        }
        HealthRingSR.sprite = HealthRings[index];
    }

    //================================================================================
    //State: Default Functions
    //================================================================================



    [SerializeField] private float moveSpeed; //TODO: Should be moved into player stats
    private Vector2 inputDirection;
    void HandleWalk()
    {
        inputDirection = (new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"))).normalized;
        rb.velocity = inputDirection * moveSpeed;
        bodyAnimator.SetFloat("Speed", inputDirection.magnitude);
        bodyAnimator.SetFloat("yDir", inputDirection.y);
    }

    //Allows right animation to be flipped and used as left
    //TODO: May be a better way than just changing the sign of the x scale
    void LeftRightFlip()
    {
        if (inputDirection.x < 0)
        {
            transform.localScale = new Vector2(-scale, scale);
            healthRing.transform.localScale = new Vector2(-(scale / Mathf.Abs(scale)) * Mathf.Abs(healthRing.transform.localScale.x), healthRing.transform.localScale.y);
        }
        else if (inputDirection.x > 0)
        {
            transform.localScale = new Vector2(scale, scale);
            healthRing.transform.localScale = new Vector2((scale / Mathf.Abs(scale)) * Mathf.Abs(healthRing.transform.localScale.x), healthRing.transform.localScale.y);
        }
    }
    //TODO: Temporary function to test out swapping weapons, later it will be done by equipping weapons using UI
    private void HandleSwordSwap()
    {
        if (swapCooldownCounter > 0) { swapCooldownCounter -= Time.deltaTime; }
        if (Input.GetButton("Swap") && swapCooldownCounter <= 0f)
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
    //TODO: Some sword animations will require different perspectives of weapons, therefore we need all variations of images
    //TODO: A useful system would be to preload these images so they can easily be used within animations
    private Sprite SwordDefault;
    private Sprite SwordBlock;
    //Swaps sword sprites to one with the given name
    void SwapSword(string newSword)
    {
        swordEquiped = newSword;
        //TODO: Need to validate if sword exists, and also preload weapons
        SwordDefault = Resources.Load<Sprite>("Swords/" + newSword + "/Default");
        SwordBlock = Resources.Load<Sprite>("Swords/" + newSword + "/Block");
    }

    float BlockCooldown = 2f;
    float BlockCooldownCounter = 0f;
    void HandleParryBlock()
    {//Go into block state on input
        //Debug.Log(BlockCooldownCounter);
        if (BlockCooldownCounter > 0) { BlockCooldownCounter -= Time.deltaTime; }
        if (Input.GetButton("Attack2") && BlockCooldownCounter <= 0)
        {

            state = State.Block;
            rb.velocity = Vector2.zero;
        }
    }

    void HandleDodgeRoll()
    {
        //Count down roll cooldown
        if (!playerStats.RollCooldownCounter.Passed)
            playerStats.RollCooldownCounter.PassTime(Time.deltaTime);
        //On input start rolling
        if (Input.GetButton("Jump") && playerStats.RollCooldownCounter.Passed)
        {
            bodyAnimator.SetTrigger("Roll");
            playerStats.RollCooldownCounter.Reset(playerStats.RollCooldown);
            canMove = false;
            state = State.DodgeRoll;
        }
    }

    //================================================================================
    //State Attack Functions
    //================================================================================


    //Consecutive attacks
    void HandleNextAttack()
    {
        //Count down attack cooldown
        if (!playerStats.Combat.AttackCooldownCounter.Passed)
            playerStats.Combat.AttackCooldownCounter.PassTime(Time.deltaTime);
        //If no cooldown and attack pressed
        if (Input.GetButton("Attack1") && playerStats.Combat.AttackCooldownCounter.Passed)
        {//Ready up next attack
            rb.velocity = Vector2.zero;
            state = State.Attack;
            canMove = false;
            attacks.ReceiveInput();
        }
        //If attack ready then trigger next attack
        if (attacks.ReadyForNextAttack())
            bodyAnimator.SetTrigger(attacks.GetNextAttack());
    }



    private bool attackLunging = false; //Controlled by animation triggers
    private float attackLungeSpeed = 1f; //TODO: Put into stats object
    //Moves forward in a certain portion of attack animation
    void HandleAttackLunge()
    {
        if (attackLunging)
            rb.velocity = new Vector2((transform.localScale.x / Mathf.Abs(transform.localScale.x)) * attackLungeSpeed, 0);
        else
            rb.velocity = Vector2.zero;
    }
    //Allows player to move after attack swing but still in attack state
    void HandleQuickMove()
    {
        if (!canMove || attacks.ReadyForNextAttack()) { return; }
        inputDirection = (new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")));
        if (inputDirection.magnitude > 0.01)
        {
            bodyAnimator.SetTrigger("Default");
            EndAttack();
        }
    }

    //--------------------------------------------------------------------------------
    //Animation Events
    //--------------------------------------------------------------------------------

    //Called: Stage of attack starting
    //Sets sword image
    public void StartAttack() { swordSR.sprite = SwordDefault; }
    //Called: Stage when attack starts lunging (moving)
    public void StartAttackLunge() { attackLunging = true; }
    //Called: Stage when attack lunge ends (stops moving)
    public void EndAttackLunge() { attackLunging = false; }
    //Called: End of attack animation, without transition to another attack
    //Sets attacks back to the first attack and makes the sword invisible
    public void EndAttack()
    {
        canMove = true;
        state = State.Default;
        attacks.HardReset();
        playerStats.Combat.AttackCooldownCounter.Reset(1f / playerStats.Combat.AttackSpeed.Value);
    }
    //Called: Stage of the attack animation which you can input to trigger another attack
    public void ReadyForAttackInput()
    {
        attacks.ReadyNextInput();
    }
    //Called: Stage of the attack animation which can transition to another attack
    //Sets boolean true for transition stage
    public void ReadyForNextAttack()
    {
        canMove = true;
        attacks.CanDoNextAttack();
    }

    //================================================================================
    //State: DodgeRoll Functions
    //================================================================================


    [SerializeField] private float rollSpeed = 3f; //TODO: Put into stats object
    void HandleDodgeRollMotion()
    {
        //TODO: Allow dodge roll in direction of motion
        rb.velocity = new Vector2((transform.localScale.x / Mathf.Abs(transform.localScale.x)) * rollSpeed, 0);
    }
    //Ends roll: called by animation trigger
    public void EndRoll()
    {
        canMove = true;
        state = State.Default;
    }

    //================================================================================
    //State Block-Parry Functions
    //================================================================================
    float ParryWindowCooldown = 1f;
    float ParryWindowCooldownCounter = 0f;
    void HandleParryBlocking()
    {
        if (Input.GetButton("Attack2"))
        {//Start blocking in input
            swordSR.sprite = SwordBlock;
            bodyAnimator.SetBool("Block", true);
            ParryWindowCooldownCounter = ParryWindowCooldown;
        }
        //Count down parry window cooldown
        if (ParryWindowCooldownCounter <= 0)
            ParryWindowCooldownCounter -= Time.deltaTime;

        if (Input.GetButtonUp("Attack2"))
        {//Stop blocking when input up
            bodyAnimator.SetBool("Block", false);
            if (ParryWindowCooldownCounter > 0)
            {//If let go within parry window -> trigger parry
                bodyAnimator.SetTrigger("Parry");
                swordSR.sprite = SwordDefault;
                state = State.Parry;
            }
        }
    }

    void StopBlocking()
    {
        bodyAnimator.SetBool("Block", false);
        BlockCooldownCounter = BlockCooldown;
        state = State.Default;
    }

    //--------------------------------------------------------------------------------
    //Animation Events
    //--------------------------------------------------------------------------------
    //Called: End of parry animation
    public void EndParry()
    {
        state = State.Default;
    }


    //================================================================================
    //Player hitting enemy
    //================================================================================
    [SerializeField] private LayerMask enemyLayers;
    //So far the only trigger is the collider around the sword when swinging
    void OnTriggerEnter2D(Collider2D collider)
    {
        EntityController opponent = collider.GetComponent<EntityController>();

        if (enemyLayers == (enemyLayers | (1 << collider.gameObject.layer)))
        {
            //Player hitting enemy
            Debug.Log("Hit");
            collider.gameObject.GetComponent<HitBoxController>().TakeDamage(new DamageReport { causedBy = this, target = opponent, damage = playerStats.Combat.Damage.Value }, this);
        }
    }

    //================================================================================
    //Player collisions
    //================================================================================
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 9) //Ignore player's second collider
        { //2D colliders used to prevent entities pushing each other
            Physics2D.IgnoreCollision(collision.collider, GetComponent<CapsuleCollider2D>());
        }
    }

    //TODO: Play a knockback animation
    public IEnumerator Knockback(float knockbackDuration, float knockbackPower, Vector2 objPos)
    {
        //bodyAnimator.SetTrigger("Knockback");
        state = State.Knockback;
        canMove = false;
        Vector2 direction = (objPos - ((Vector2)transform.position + GetComponent<Collider2D>().offset)).normalized;
        rb.AddForce(-direction * knockbackPower, ForceMode2D.Impulse);
        yield return new WaitForSeconds(knockbackDuration);
        //Resets after knockback
        rb.velocity = Vector2.zero;
        canMove = true;
        state = State.Default;
    }


    //================================================================================
    //Player damage
    //================================================================================
    private bool invulnerable;
    public override void TakeDamage(DamageReport dr, EntityController dealer)
    {
        if (state == State.Block)
        {
            StopBlocking();
            dealer.Block();
            return;
        }
        if (state == State.Parry)
        {

        }
        if (invulnerable) { return; }
        base.TakeDamage(dr, dealer);
        rb.velocity = Vector2.zero;

        //Debug.Log("p: " + playerStats.CurrentHealth);
        swordSR.sprite = null;
        if (playerStats.CurrentHealth <= 0)
        {
            state = State.Die;
        }
        else
        {
            state = State.Hit;
            bodyAnimator.SetTrigger("Hit");
        }
    }

    public void EndHit()
    {
        state = State.Default;
        attacks.HardReset();
    }

    public void StartInv()
    {
        invulnerable = true;
    }
    public void EndInv()
    {
        invulnerable = false;
    }

}




//Used to handle consecutive attacks
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
        HardReset();
    }

    //Ready to receive input for next attack
    public void ReadyNextInput()
    {
        nextInputReady = true;
        nextAttackIndex = Mathf.Min(attacks.Length - 1, nextAttackIndex + 1);
    }

    //Next attack input is received when it is ready to be received
    public void ReceiveInput() { inputReceived = nextInputReady; }
    //Triggered for when next attack can happen in the animation
    public void CanDoNextAttack() { canDoNextAttack = true; }
    //When input for next attack is received and at the transition stage of the animation
    public bool ReadyForNextAttack() { return inputReceived && canDoNextAttack; }
    //Gives string of the next attack and setsup for next attack
    public string GetNextAttack()
    {
        Reset();
        if (nextAttackIndex < attacks.Length)
            return attacks[nextAttackIndex];
        return null;
    }

    //Reset for next attack
    private void Reset()
    {
        nextInputReady = false;
        inputReceived = false;
        canDoNextAttack = false;
    }
    //Reset back ready for first attack
    public void HardReset()
    {
        nextInputReady = true;
        canDoNextAttack = true;
        inputReceived = false;
        nextAttackIndex = 0;
    }

}