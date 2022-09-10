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
    private MultiAttacks attacks;

    private AttackState attackState;
    private enum AttackState
    {
        Idle,
        CanAttack,
        DoAttack
    }

    [SerializeField] private GameObject sword;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        //Setup consecutive attack system
        attacks = new MultiAttacks(new string[] { "Attack1", "Attack2", "Attack3" });

        swordSR = sword.GetComponent<SpriteRenderer>();
        playerStats = GetComponent<PlayerStats>();
        //Equip first weapon
        SwapSword(swordEquiped);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        switch (state)
        {
            case State.Default:
                //Hides sword when not using it
                swordSR.sprite = null;
                LeftRightFlip();
                break;
            case State.Blocking:
                HandleParryBlocking();
                break;
            case State.Die:
                EndDie();
                break;
        }
        if (Input.GetButton("TabSave"))
        {
            DataPersistenceManager.instance.SaveGame();
            Debug.Log("Save");
        }
        if (Input.GetButton("PLoad"))
        {
            DataPersistenceManager.instance.LoadGame();
            Debug.Log("Load");
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        switch (state)
        {
            case State.Default:
                HandleNextAttack();
                HandleWalk();
                HandleParryBlock();
                HandleSwordSwap();
                HandleDodgeRoll();
                break;
            case State.Attack:
                HandleNextAttack();
                HandleQuickMove();
                HandleAttackLunge();
                break;
            case State.DodgeRoll:
                HandleDodgeRollMotion();
                break;

        }
    }

    #region Default State Functions
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

    float BlockCooldown = 0f;
    float BlockCooldownCounter = 0f;
    void HandleParryBlock()
    {//Go into block state on input
        if (BlockCooldownCounter > 0) { BlockCooldownCounter -= Time.deltaTime; }
        if (Input.GetButton("Attack2") && BlockCooldownCounter <= 0)
        {
            swordSR.sprite = SwordBlock;
            bodyAnimator.SetBool("Blocking", true);
            ParryWindowCooldownCounter = ParryWindowCooldown;
            state = State.Blocking;
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
    #endregion
    #region Attack State Functions
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



    //Moves forward in a certain portion of attack animation
    void HandleAttackLunge()
    {
        float attackLungeSpeed = 1f;
        if (base.attackLunging)
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
    //Called: End of attack animation, without transition to another attack
    //Sets attacks back to the first attack and makes the sword invisible
    public override void EndAttack()
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

    #endregion
    #region DodgeRoll State Functions

    [SerializeField] private float rollSpeed = 3f; //TODO: Put into stats object
    void HandleDodgeRollMotion()
    {
        //TODO: Allow dodge roll in direction of motion
        rb.velocity = new Vector2((transform.localScale.x / Mathf.Abs(transform.localScale.x)) * rollSpeed, 0);
    }
    //Ends roll: called by animation trigger
    public override void EndRoll()
    {
        canMove = true;
        state = State.Default;
    }

    #endregion
    #region Block-Parry State Functions

    float ParryWindowCooldown = 1f;
    float ParryWindowCooldownCounter = 0f;
    void HandleParryBlocking()
    {
        //Count down parry window cooldown
        if (ParryWindowCooldownCounter >= 0)
            ParryWindowCooldownCounter -= Time.deltaTime;

        if (Input.GetButtonUp("Attack2"))
        {//Stop blocking when input up
            bodyAnimator.SetBool("Blocking", false);
            BlockCooldownCounter = BlockCooldown;
            if (ParryWindowCooldownCounter > 0)
            {//If let go within parry window -> trigger parry
                bodyAnimator.SetTrigger("Parry");
                swordSR.sprite = SwordDefault;
                state = State.Parry;
            }
            else
            {
                state = State.Default;
            }
        }
    }
    #endregion
    #region Player hitting enemy

    [SerializeField] private LayerMask enemyLayers;
    //So far the only trigger is the collider around the sword when swinging
    void OnTriggerEnter2D(Collider2D collider)
    {
        EntityController opponent = collider.GetComponent<EntityController>();

        if (enemyLayers == (enemyLayers | (1 << collider.gameObject.layer)))
        {
            float swordDamage = 50f;
            //Player hitting enemy
            //collider.gameObject.GetComponent<HitBoxController>().TakeDamage(new DamageReport { causedBy = this, target = opponent, damage = playerStats.Combat.Damage.Value }, this);
            collider.gameObject.GetComponent<HitBoxController>().TakeDamage(new DamageReport { causedBy = this, target = opponent, damage = swordDamage }, this);
        }
    }

    public override void GotKill()
    {
        playerStats.IncrementKills();
    }
    #endregion
    #region Player collisions
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 9) //Ignore player's second collider
        { //2D colliders used to prevent entities pushing each other
            Physics2D.IgnoreCollision(collision.collider, GetComponent<CapsuleCollider2D>());
        }
    }
    #endregion
    #region Player damage
    public override void TakeDamage(DamageReport dr, EntityController dealer)
    {
        if (state == State.Blocking)
        {
            bodyAnimator.SetBool("Blocking", false);
            BlockCooldownCounter = BlockCooldown;
            Block();
            dealer.Block();
            return;
        }
        if (state == State.Parry)
        {
            dealer.Parried();
            return;
        }
        if (invulnerable) { return; }
        swordSR.sprite = null;
        base.TakeDamage(dr, dealer);
    }

    public override void EndHit()
    {
        base.EndHit();
        attacks.HardReset();
    }
    public override void EndDie()
    {
        playerStats.ResetOnDeath();
        transform.position = Vector2.zero;
        state = State.Default;
        //TODO: Player dies
    }
    #endregion
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