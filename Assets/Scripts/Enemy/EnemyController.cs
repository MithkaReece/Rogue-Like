using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : EntityController
{
    protected EnemyStats enemyStats;

    [SerializeField] protected PlayerController player;
    public void SetPlayer(PlayerController inPlayer) { player = inPlayer; }
    private bool playerSeen = true;


    //protected AnimationEventSystem AES = new AnimationEventSystem();

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        enemyStats = GetComponent<EnemyStats>();
    }

    // Update is called once per frame
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void TakeDamage(DamageReport dr, EntityController dealer)
    {
        base.TakeDamage(dr, dealer);
    }
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



