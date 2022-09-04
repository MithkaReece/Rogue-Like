using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : EntityController
{
    protected EnemyStats enemyStats;

    [SerializeField] protected PlayerController player;
    public void SetPlayer(PlayerController inPlayer) { player = inPlayer; }
    private bool playerSeen = true;


    protected AnimationEventSystem AES = new AnimationEventSystem();

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        enemyStats = GetComponent<EnemyStats>();
        entityStats = GetComponent<EnemyStats>();
        bodyAnimator = body.GetComponent<Animator>();
    }

    // Update is called once per frame
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        switch (state)
        {
            case State.Hit:
                break;
        }
        //Move();
    }


    //OLD
    void Move()
    {
        if (!playerSeen) { return; }
        Vector2 direction = player.transform.position - transform.position;
        direction = direction.normalized;
        //rb.MovePosition(rb.position + direction * moveSpeed * Time.deltaTime);
        rb.velocity = direction * (float)enemyStats.MoveSpeed.Value;
    }

    public override void TakeDamage(DamageReport dr, EntityController dealer)
    {
        base.TakeDamage(dr, dealer);
        rb.velocity = Vector2.zero;

        //Hurt animation

        if (enemyStats.CurrentHealth <= 0)
        {
            state = State.Die;
            bodyAnimator.SetTrigger("Die");
        }
        else
        {
            state = State.Hit;
            bodyAnimator.SetTrigger("Hit");
        }
    }
    private void Die()
    {
        Debug.Log("Enemy died");
        //Die animation
        rb.velocity = Vector2.zero;
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
    /*
    public void KnockbackPlayer(Collision2D collision)
    {
        if (enemyLayers == (enemyLayers | (1 << collision.collider.gameObject.layer)))
        {
            player.TakeDamage(new DamageReport { causedBy = this, target = player, damage = entityStats.Combat.Damage.Value }, this);
            StartCoroutine(player.Knockback((float)enemyStats.KnockbackDuration.Value, (float)enemyStats.KnockbackPower.Value, (Vector2)transform.position + GetComponent<Collider2D>().offset));
        }
    }*/

    public void EndAttack() { AES.EndAttack = true; }
    public void StartLunge() { AES.StartLunge = true; }
    public void EndLunge() { AES.EndLunge = true; }

    public void EndHit() { state = State.Default; }
    public void EndDie()
    {
        GetComponent<CapsuleCollider2D>().enabled = false;
        transform.Find("Collision Blocker").GetComponent<CapsuleCollider2D>().enabled = false;
        transform.Find("HitBox").GetComponent<CapsuleCollider2D>().enabled = false;
        this.enabled = false;
    }

    [SerializeField] private LayerMask enemyLayers;
    //So far the only trigger is the collider around the sword when swinging
    void OnTriggerEnter2D(Collider2D collider)
    {
        EntityController opponent = collider.GetComponent<EntityController>();

        if (enemyLayers == (enemyLayers | (1 << collider.gameObject.layer)))
        {
            collider.gameObject.GetComponent<HitBoxController>().TakeDamage(new DamageReport { causedBy = this, target = opponent, damage = enemyStats.Combat.Damage.Value }, this);
        }
    }
}

public class AnimationEventSystem
{
    public bool EndAttack;
    public bool StartLunge;
    public bool EndLunge;
    public void ResetAttack()
    {
        EndAttack = false;
        StartLunge = false;
        EndLunge = false;
    }
    //When you are hit

}

