using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityController : MonoBehaviour
{
    protected Rigidbody2D rb;
    protected EntityStats entityStats;
    public EntityObserver EntityObserver { get; } = new EntityObserver();

    // Start is called before the first frame update
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        entityStats = GetComponent<EntityStats>();
    }

    public virtual void TakeDamage(DamageReport dr)
    {
        entityStats.TakeDamage(dr);

        //Invoke delegates for observers
        this.EntityObserver.OnDamageTaken(dr);
        dr.causedBy.EntityObserver.OnDamageDealt(dr);
    }
}
