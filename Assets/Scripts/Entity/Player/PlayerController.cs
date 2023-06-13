using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;

public class PlayerController : EntityController
{
    [SerializeField] private float scale = 1f;

    private bool canMove = true; //Allows player to move during transition

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

    //TODO: Implement a way to tell up and down
    //Have a naming convention for up and down
    //E.g IDLE_0 for down, IDLE_1 for up


    public AnimationClip attack1_0;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        Name = "Player";

        //TODO: Remove
        //Setup consecutive attack system
        attacks = new MultiAttacks(new string[] { "Attack1", "Attack2", "Attack3" });
        //TODO: Remove
        swordSR = sword.GetComponent<SpriteRenderer>();

        playerStats = GetComponent<PlayerStats>();
        //Equip first weapon
        SwapSword(swordEquiped);

        //TODO: Temp, show states
        /*AnimatorController animatorController = bodyAnimator.runtimeAnimatorController as AnimatorController;
        AnimatorStateMachine stateMachine = animatorController.layers[0].stateMachine;
        foreach (ChildAnimatorState state in stateMachine.states)
        {
            Debug.Log("State Name: " + state.state.name);
        }*/
    }


    // Update is called once per frame
    protected void Update()
    {
        switch (state)
        {
            case State.Default:
                //Hides sword when not using it
                swordSR.sprite = null;
                break;
            case State.Blocking:
                HandleParryBlocking();
                break;
            case State.Die:
                EndDie();
                break;
        }
        //TODO: Saving and loading (only just started)
        //TODO: Need to separate this to a class
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
                HandleAttack();
                //HandleNextAttack();
                base.Move((new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"))).normalized);
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


    void HandleAttack()
    {
        if (Input.GetButton("Attack1"))
        {
            if (equipment.weapon?.CanAttack() ?? false)
            {
                rb.velocity = Vector2.zero;
                //state = State.Attack;
                canMove = false;
                //attacks.ReceiveInput();

                //TODO: Change this,
                //Instead get animation clip from weapon information
                //Add animation clip as a state
                //Transition to newly added state
                //ChangeState(attack1_0.name.Substring(0, attack1_0.name.Length - 2));
            }
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
        /*if (!canMove || attacks.ReadyForNextAttack()) { return; }
        inputDirection = (new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")));
        if (inputDirection.magnitude > 0.01)
        {
            bodyAnimator.SetTrigger("Default");
            EndAttack();
        }*/
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
        playerStats.Combat.AttackCooldownCounter.Reset(1f / playerStats.Combat.AttackSpeed);
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
            //TODO: Use sword's actual damage
            float swordDamage = 50f;
            //Player hitting enemy
            //collider.gameObject.GetComponent<HitBoxController>().TakeDamage(new DamageReport { causedBy = this, target = opponent, damage = playerStats.Combat.Damage.Value }, this);
            collider.gameObject.GetComponent<HitBoxController>().TakeDamage(
                new DamageReport
                {
                    causedBy = this,
                    target = opponent,
                    damage = swordDamage
                }, this);
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


public class WeaponPlayer
{
    private SpriteRenderer SR;
    private string WeaponEquiped;
    private GameObject weaponObj;

    public WeaponPlayer()
    {
        //Retrieve sprite
        //Select default weapon
        //Get weapon object (child object), given Controller
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