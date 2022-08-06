using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityController : MonoBehaviour
{
    protected Rigidbody2D rb;
    protected EntityStats entityStats;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        entityStats = GetComponent<EntityStats>();
    }

    public virtual void TakeDamage(decimal damage)
    {
        entityStats.TakeDamage(damage);
    }
}
