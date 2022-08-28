using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]

public class BaseEnemyController : EnemyController
{
    [SerializeField] private GameObject body;
    private Animator bodyAnimator;
    private Attack intendedAttack;
    public Attack[] AvailableAttacks = new Attack[]{
        new Attack("1",1)
    };

    public BaseEnemyController()
    {
        intendedAttack = AvailableAttacks[0];
    }

    void FixedUpdate()
    {
        BasicMovement();
        bodyAnimator.SetFloat("Speed", rb.velocity.magnitude);
        LeftRightFlip();
    }

    void BasicMovement()
    {
        Vector2 playerDirection = base.player.transform.position - transform.position;
        double errorAmount = 0.2f;
        if (playerDirection.magnitude > intendedAttack.Range - errorAmount)
        {
            rb.velocity = playerDirection.normalized * (float)base.enemyStats.MoveSpeed.Value;
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
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
        bodyAnimator = body.GetComponent<Animator>();
        base.Start();
    }

}

public class Attack
{
    public double Range { get; private set; }
    public string Name { get; private set; }

    public Attack(string name, double range)
    {
        Name = name;
        Range = range;
    }
}

