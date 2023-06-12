using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EntityController : MonoBehaviour
{
    protected GameObject healthRing;

    protected Rigidbody2D rb;
    protected Animator bodyAnimator;
    public EntityStats entityStats;
    public EntityObserver EntityObserver { get; } = new EntityObserver();

    private Repos repos;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        entityStats = GetComponent<EntityStats>();
        GameObject body = transform.GetChild(0).gameObject;
        bodyAnimator = body.GetComponent<Animator>();
        healthRing = transform.GetChild(3).gameObject;
        repos = new Repos(healthRing, entityStats);
    }


    protected State state;
    protected enum State
    {
        Default,
        DodgeRoll,
        Attack,
        Block,
        Blocking,
        Parry,
        Hit,
        Stun,
        Die,
    }
    protected virtual void Update()
    {
        repos.UpdateRepos();
    }
    //Used for rigidbody (physics)
    protected virtual void FixedUpdate()
    {
        switch (state)
        {
            case State.Stun:
                HandleStun();
                break;
        }
    }

    #region Stun State Functions
    void HandleStun()
    {
        float stunSpeed = 1f;
        if (StunMovement)
            rb.velocity = new Vector2((transform.localScale.x / Mathf.Abs(transform.localScale.x)) * -stunSpeed, 0);
        else
            rb.velocity = Vector2.zero;
    }
    #endregion
    #region Enity Damage
    public virtual void TakeDamage(DamageReport dr, EntityController dealer)
    {
        if (repos.MaxRepos())
            dr.damage *= 4;
        rb.velocity = Vector2.zero;
        entityStats.TakeDamage(dr, dealer);

        if (entityStats.CurrentHealth <= 0)
        {
            state = State.Die;
            bodyAnimator.SetTrigger("Die");
            dealer.GotKill();
        }
        else if (repos.MaxRepos())
        {
            state = State.Stun;
            bodyAnimator.SetTrigger("Stun");
        }
        else
        {
            state = State.Hit;
            bodyAnimator.SetTrigger("Hit");
        }

        repos.AddRepos(dr.damage);
        //Invoke delegates for observers
        this.EntityObserver.OnDamageTaken(dr);
        dr.causedBy.EntityObserver.OnDamageDealt(dr);
    }

    #endregion

    public virtual void Block()
    {
        state = State.Block;
        bodyAnimator.SetTrigger("Block");
    }
    public virtual void Parried()
    {
        state = State.Stun;
        bodyAnimator.SetTrigger("Stun");
    }

    public virtual void GotKill()
    {

    }

    #region Animation Events
    protected bool attackLunging;
    //Called: Stage when attack starts lunging (moving)
    public virtual void StartAttackLunge() { attackLunging = true; }
    //Called: Stage when attack lunge ends (stops moving)
    public virtual void EndAttackLunge() { attackLunging = false; }
    public virtual void EndAttack()
    {
        state = State.Default;
        entityStats.Combat.AttackCooldownCounter.Reset(1f / entityStats.Combat.AttackSpeed);
    }

    public virtual void EndHit() { state = State.Default; }
    public virtual void EndDie()
    {
        GetComponent<CapsuleCollider2D>().enabled = false;
        transform.Find("Collision Blocker").GetComponent<CapsuleCollider2D>().enabled = false;
        transform.Find("HitBox").GetComponent<CapsuleCollider2D>().enabled = false;
        transform.Find("HealthRing").GetComponent<SpriteRenderer>().enabled = false;
        transform.Find("HealthRing").Find("ReposRing").GetComponent<SpriteRenderer>().enabled = false;
        this.enabled = false;
    }

    protected bool StunMovement;
    public void StartStun() { StunMovement = true; }

    public void EndStunMovement() { StunMovement = false; }
    public void EndStun()
    {
        state = State.Default;
        bodyAnimator.SetBool("Stun", false);
    }

    public void StopBlocking() { state = State.Default; }



    protected bool invulnerable;
    public void StartInv() { invulnerable = true; }
    public void EndInv() { invulnerable = false; }
    //TODO: Do for enemy as well as player
    public virtual void EndRoll() { }

    public void EndParry() { state = State.Default; }

    #endregion
}

public class Repos
{
    private Sprite[] RingSprites;
    private SpriteRenderer RingRenderer;
    private EntityStats Stats;

    [SerializeField] protected float repos = 0f;
    private float reposCooldownCounter = 0f;

    //TODO: Make values pointers to the live values (as they may change)
    //TODO: Could just give reference to stat object then reference needed attributes
    public Repos(GameObject healthRing, EntityStats stats)
    {
        GameObject reposRing = healthRing.transform.GetChild(0).gameObject;
        RingRenderer = reposRing.GetComponent<SpriteRenderer>();
        RingSprites = Resources.LoadAll<Sprite>("Repos Ring");
        Stats = stats;
    }

    public bool MaxRepos()
    {
        return Stats.Poise == repos;
    }

    public void UpdateRepos()
    {
        //Decay repos
        if (reposCooldownCounter <= 0f)
            repos = Mathf.Max(0, repos - Stats.ReposRegenSpeed * Time.deltaTime);
        reposCooldownCounter = Mathf.Max(0, reposCooldownCounter - Time.deltaTime);
        //Update ring sprite
        //TODO: Account for poise = 0
        int index = (int)Mathf.Floor(10f * repos / Stats.Poise);
        RingRenderer.sprite = RingSprites[index];
    }

    public void AddRepos(float damage)
    {
        if (MaxRepos()) //Reset after damaging at max
            repos = 0f;
        else //Increase repos based on damage taken
            repos = Mathf.Min(repos + (float)damage, Stats.Poise);
        reposCooldownCounter = Stats.ReposCooldown;
    }
}