using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBoxController : MonoBehaviour
{
    protected EntityController entity;
    protected virtual void Start()
    {
        entity = GetComponentInParent<EntityController>();
    }

    public void TakeDamage(DamageReport dr, EntityController dealer) { entity.TakeDamage(dr, dealer); }
}
