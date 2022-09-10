using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityController : MonoBehaviour
{
    protected GameObject body;
    protected GameObject healthRing;

    protected Rigidbody2D rb;
    protected Animator bodyAnimator;
    protected EntityStats entityStats;
    public EntityObserver EntityObserver { get; } = new EntityObserver();

    #region Initialisation
    // Start is called before the first frame update
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        entityStats = GetComponent<EntityStats>();
        body = transform.Find("Body").gameObject;
        bodyAnimator = body.GetComponent<Animator>();
        healthRing = transform.Find("HealthRing").gameObject;
        InitHealthRings();
        InitReposRings();
    }

    private Sprite[] HealthRings;
    private SpriteRenderer HealthRingSR;
    void InitHealthRings()
    {
        HealthRingSR = healthRing.GetComponent<SpriteRenderer>();
        HealthRings = Resources.LoadAll<Sprite>("Health Ring");
    }
    private Sprite[] ReposRings;
    private SpriteRenderer ReposRingSR;
    void InitReposRings()
    {
        GameObject reposRing = healthRing.transform.Find("ReposRing").gameObject;
        ReposRingSR = reposRing.GetComponent<SpriteRenderer>();
        ReposRings = Resources.LoadAll<Sprite>("Repos Ring");
    }
    #endregion

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
        UpdateHealthRing();
        HandleReposDecay();
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
    #region Health Ring Handling
    void UpdateHealthRing()
    {
        float Percent = 100 * entityStats.CurrentHealth / entityStats.MaxHealth.Value;
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
    #endregion
    #region Repos Handling
    [SerializeField] protected float poise = 100f;
    [SerializeField] protected float repos = 0f;
    [SerializeField] protected float reposCountdown = 2f;
    private float reposCountdownCounter = 0f;
    [SerializeField] protected float reposRegenSpeed = 20f;
    private bool resetRepos;
    void HandleReposDecay()
    {
        if (resetRepos)
        {
            resetRepos = false;
            repos = 0f;
        }

        if (reposCountdownCounter <= 0f)
        {
            if (repos > 0f)
            {
                repos = Mathf.Max(0, repos - reposRegenSpeed * Time.deltaTime);
            }
        }
        else
        {
            reposCountdownCounter -= Time.deltaTime;
        }
        UpdateReposRing();
    }

    void UpdateReposRing()
    {
        int index = (int)Mathf.Floor(10f * repos / poise);
        ReposRingSR.sprite = ReposRings[index];
    }
    #endregion
    #region Stun State Functions
    void HandleStun()
    {
        float stunSpeed = 1f;
        if (StunMovement)
        {
            rb.velocity = new Vector2((transform.localScale.x / Mathf.Abs(transform.localScale.x)) * -stunSpeed, 0);
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }
    #endregion
    #region Enity Damage
    public virtual void TakeDamage(DamageReport dr, EntityController dealer)
    {
        if (repos == poise)
        {
            dr.damage *= 4;
            resetRepos = true;
        }
        rb.velocity = Vector2.zero;
        entityStats.TakeDamage(dr, dealer);

        if (entityStats.CurrentHealth <= 0)
        {
            state = State.Die;
            //bodyAnimator.SetTrigger("Die");
            dealer.GotKill();
        }
        else if (repos == poise)
        {
            state = State.Stun;
            bodyAnimator.SetTrigger("Stun");
        }
        else
        {
            state = State.Hit;
            bodyAnimator.SetTrigger("Hit");
        }

        AddRepos(dr.damage);
        //Invoke delegates for observers
        this.EntityObserver.OnDamageTaken(dr);
        dr.causedBy.EntityObserver.OnDamageDealt(dr);
    }

    void AddRepos(float damage)
    {
        repos = Mathf.Min(repos + (float)damage, poise);
        reposCountdownCounter = reposCountdown;
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
        entityStats.Combat.AttackCooldownCounter.Reset(1f / entityStats.Combat.AttackSpeed.Value);
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
