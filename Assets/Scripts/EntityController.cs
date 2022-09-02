using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityController : MonoBehaviour
{
    [SerializeField] protected GameObject body;
    [SerializeField] protected GameObject healthRing;

    protected Rigidbody2D rb;
    protected Animator bodyAnimator;
    protected EntityStats entityStats;
    public EntityObserver EntityObserver { get; } = new EntityObserver();

    // Start is called before the first frame update
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        entityStats = GetComponent<EntityStats>();
    }

    public virtual void TakeDamage(DamageReport dr, EntityController dealer)
    {
        entityStats.TakeDamage(dr, dealer);

        //Invoke delegates for observers
        this.EntityObserver.OnDamageTaken(dr);
        dr.causedBy.EntityObserver.OnDamageDealt(dr);
    }

    public virtual void Block()
    {

    }
}
