using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : EntityController
{
    protected EnemyStats enemyStats;

    //[SerializeField] protected PlayerController player;
    public PlayerController player;
    public void SetPlayer(PlayerController inPlayer) { player = inPlayer; }

    protected Attack intendedAttack;
    //TODO: Make this into a list so children can add attacks
    public Attack[] AvailableAttacks = new Attack[]{
        new Attack("Attack1", 0.6f, 1f, 1f, 1f)
    };

    private EnemyDetection detect;

    protected override void Start()
    {
        base.Start();
        enemyStats = GetComponent<EnemyStats>();
        intendedAttack = AvailableAttacks[0];
        detect = GetComponent<EnemyDetection>();
    }
    //Default walk to player/attack
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        switch (state)
        {
            case State.Default:
                //Either wandering or following

                if (detect.canSeePlayer)
                {
                    PathFindPlayer();
                }
                else
                    rb.velocity = Vector2.zero;
                bodyAnimator.SetFloat("Speed", rb.velocity.magnitude);
                break;
            case State.Attack:
                HandleAttackLunge();
                break;
        }
    }

    void PathFindPlayer()
    {
        WalkTowardsPlayer();
    }


    // TODO: Replace this with path findings
    void WalkTowardsPlayer()
    {
        Vector2 playerDirection = player.transform.position - transform.position;
        //Face player
        transform.localScale = new Vector2(
            Mathf.Abs(transform.localScale.x) * playerDirection.x / Mathf.Abs(playerDirection.x),
            transform.localScale.y
        );
        //Decrease attack cooldown
        if (!enemyStats.Combat.AttackCooldownCounter.Passed)
            enemyStats.Combat.AttackCooldownCounter.PassTime(Time.deltaTime);
        //If outside of range walk

        if (playerDirection.magnitude > intendedAttack.Range - 0.2f)
            rb.velocity = playerDirection.normalized * (float)enemyStats.MoveSpeed.Value;
        else //If in range and can attack (switch to attack state & attack)
        {
            rb.velocity = Vector2.zero;
            if (enemyStats.Combat.AttackCooldownCounter.Passed)
            {
                state = State.Attack;
                bodyAnimator.SetTrigger(intendedAttack.Name);
            }
        }
    }

    #region Attack State Functions
    void HandleAttackLunge()
    {
        if (attackLunging)
            rb.velocity = new Vector2((transform.localScale.x / Mathf.Abs(transform.localScale.x)) * intendedAttack.LungeSpeed, 0);
        else
            rb.velocity = Vector2.zero;
    }
    #endregion


    private void Die()
    {
        GetComponent<CapsuleCollider2D>().enabled = false;
        transform.Find("Collision Blocker").GetComponent<CapsuleCollider2D>().enabled = false;
        this.enabled = false;
    }
    //Player blocks your attack
    public override void Block()
    {
        bodyAnimator.ResetTrigger("Attack1");
        base.Block();
    }

    public override void Parried()
    {
        bodyAnimator.ResetTrigger("Attack1");
        base.Parried();
    }

    #region Enemy hitting player
    [SerializeField] private LayerMask enemyLayers;
    //So far the only trigger is the collider around the sword when swinging
    void OnTriggerEnter2D(Collider2D collider)
    {
        EntityController opponent = collider.GetComponent<EntityController>();

        if (enemyLayers == (enemyLayers | (1 << collider.gameObject.layer)))
        {
            collider.gameObject.GetComponent<HitBoxController>().TakeDamage(
                new DamageReport { 
                    causedBy = this, 
                    target = opponent,
                    damage = enemyStats.Combat.Damage.Value 
                }, this );
        }
    }
    #endregion
}



public class Attack
{
    public string Name { get; private set; }
    // TODO: This is the range at which the enemy will attack
    // TODO: Should be similar to the range of the attack
    // TODO: So far just manually guessing
    public float Range { get; private set; }

    public float LungeStart { get; private set; }
    public float LungeDuration { get; private set; }
    public float LungeSpeed { get; private set; }

    public Attack(string name, float range, float lungeStart, float lungeDuration, float lungeSpeed)
    {
        Name = name;
        Range = range;
        LungeStart = lungeStart;
        LungeDuration = lungeDuration;
        LungeSpeed = lungeSpeed;
    }
}