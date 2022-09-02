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

    // Start is called before the first frame update
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        entityStats = GetComponent<EntityStats>();
        body = transform.Find("Body").gameObject;
        healthRing = transform.Find("HealthRing").gameObject;
        GetHealthRings();
        GetReposRings();
    }

    private Sprite[] HealthRings;
    private SpriteRenderer HealthRingSR;
    void GetHealthRings()
    {
        HealthRingSR = healthRing.GetComponent<SpriteRenderer>();
        HealthRings = Resources.LoadAll<Sprite>("Health Ring");
    }
    private Sprite[] ReposRings;
    private SpriteRenderer ReposRingSR;
    void GetReposRings()
    {
        GameObject reposRing = healthRing.transform.Find("ReposRing").gameObject;
        ReposRingSR = reposRing.GetComponent<SpriteRenderer>();
        ReposRings = Resources.LoadAll<Sprite>("Repos Ring");
    }

    public virtual void TakeDamage(DamageReport dr, EntityController dealer)
    {
        entityStats.TakeDamage(dr, dealer);
        AddRepos(dr.damage);
        //Invoke delegates for observers
        this.EntityObserver.OnDamageTaken(dr);
        dr.causedBy.EntityObserver.OnDamageDealt(dr);
    }

    public virtual void Block()
    {

    }

    protected virtual void FixedUpdate()
    {
        UpdateHealthRing();
        HandleReposDecay();
    }

    void UpdateHealthRing()
    {
        double Percent = 100 * entityStats.CurrentHealth / entityStats.MaxHealth.Value;
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

    void AddRepos(double damage)
    {
        repos = Mathf.Min(repos + (float)damage, poise);
        reposCountdownCounter = reposCountdown;
    }
    [SerializeField] protected float poise = 100f;
    [SerializeField] protected float repos = 0f;
    [SerializeField] protected float reposCountdown = 2f;
    private float reposCountdownCounter = 0f;
    [SerializeField] protected float reposRegenSpeed = 20f;
    void HandleReposDecay()
    {

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
        Debug.Log(ReposRings.Length);
        int index = (int)Mathf.Floor(10f * repos / poise);
        ReposRingSR.sprite = ReposRings[index];
    }
}
