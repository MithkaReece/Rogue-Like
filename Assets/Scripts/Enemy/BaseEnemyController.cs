using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]

public class BaseEnemyController : EnemyController
{
    private Attack intendedAttack;
    public Attack[] AvailableAttacks = new Attack[]{
        new Attack("Attack1",0.8f, 1f, 1f, 1f)
    };

    public BaseEnemyController()
    {
        intendedAttack = AvailableAttacks[0];
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        switch (state)
        {
            case State.Default:

                BasicAI();
                bodyAnimator.SetFloat("Speed", rb.velocity.magnitude);

                break;
            case State.Attack:

                HandleAttack();
                break;
        }

    }
    void HandleAttack()
    {
        if (base.AES.EndAttack)
        {
            state = State.Default;
            enemyStats.Combat.AttackCooldownCounter.Reset(1f / enemyStats.Combat.AttackSpeed.Value);
            base.AES.ResetAttack();
        }

        if (base.AES.StartLunge && !base.AES.EndLunge)
        {
            rb.velocity = new Vector2((transform.localScale.x / Mathf.Abs(transform.localScale.x)) * intendedAttack.LungeSpeed, 0);
        }
        else
            rb.velocity = Vector2.zero;
    }

    void BasicAI()
    {

        FacePlayer();
        if (!enemyStats.Combat.AttackCooldownCounter.Passed)
            enemyStats.Combat.AttackCooldownCounter.PassTime(Time.deltaTime);
        Vector2 playerDirection = player.transform.position - transform.position;
        double errorAmount = 0.2f;
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
        float scale = 2f;
        Vector2 playerDirection = player.transform.position - transform.position;
        if (playerDirection.x > 0)
            transform.localScale = new Vector2(scale, scale);
        else if (playerDirection.x < 0)
            transform.localScale = new Vector2(-scale, scale);
    }

    void LeftRightFlip()
    {
        float scale = 2f;
        if (rb.velocity.x < 0)
            transform.localScale = new Vector2(-scale, scale);
        else if (rb.velocity.x > 0)
            transform.localScale = new Vector2(scale, scale);
    }


    protected override void Start()
    {
        base.Start();
    }

}

public class Attack
{
    public float Range { get; private set; }
    public string Name { get; private set; }

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

