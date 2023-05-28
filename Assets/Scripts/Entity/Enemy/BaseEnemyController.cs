using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]

public class BaseEnemyController : EnemyController
{

    protected Attack intendedAttack;
    //TODO: Make this into a list so children can add attacks
    public Attack[] AvailableAttacks = new Attack[]{
        new Attack("Attack1", 0.6f, 1f, 1f, 1f)
    };

    public bool canSeePlayer;
    [SerializeField] public float maxFollowDist;
    [Range(0, 360)]
    public float FOVAngle;
    public Vector2 LookingDirection;

    protected override void Start()
    {
        base.Start();
        intendedAttack = AvailableAttacks[0];
        LookingDirection = new Vector2(transform.localScale.x, 0);
    }

    protected override void Update()
    {
        //IDK if I need this, or FOV will handle it
        //Player radius would have to always be larger than follow dit
        if (canSeePlayer)
        {
            LookingDirection = player.transform.position - transform.position;
            if (LookingDirection.magnitude > Mathf.Pow(maxFollowDist,2))
            {
                canSeePlayer = false;
            }
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        switch (state)
        {
            case State.Default:
                //Either wandering or following
                
                if (canSeePlayer)
                {
                    PathFindPlayer();
                }
                else
                    Wander();
                //BasicAI();
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

    void Wander()
    {
        rb.velocity = Vector2.zero;
    }


    #region Default State Functions
    void BasicAI()
    {
        FacePlayer();
        if (!enemyStats.Combat.AttackCooldownCounter.Passed)
            enemyStats.Combat.AttackCooldownCounter.PassTime(Time.deltaTime);
        Vector2 playerDirection = player.transform.position - transform.position;
        float errorAmount = 0.2f;
        if (playerDirection.magnitude > intendedAttack.Range - errorAmount)
        {
            rb.velocity = playerDirection.normalized * (float)enemyStats.MoveSpeed.Value;
        }
        else
        {
            rb.velocity = Vector2.zero;
            if (enemyStats.Combat.AttackCooldownCounter.Passed)
            {
                state = State.Attack;
                bodyAnimator.SetTrigger(intendedAttack.Name);
            }
        }
    }

    void FacePlayer()
    {
        float scale = 1f;
        Vector2 playerDirection = player.transform.position - transform.position;
        if (playerDirection.x > 0)
            transform.localScale = new Vector2(scale, scale);
        else if (playerDirection.x < 0)
            transform.localScale = new Vector2(-scale, scale);
    }
    #endregion
    #region Attack State Functions
    void HandleAttackLunge()
    {
        if (attackLunging)
            rb.velocity = new Vector2((transform.localScale.x / Mathf.Abs(transform.localScale.x)) * intendedAttack.LungeSpeed, 0);
        else
            rb.velocity = Vector2.zero;
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