using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBoxController : MonoBehaviour
{
    [SerializeField] private EntityController entity;

    public void TakeDamage(DamageReport dr, EntityController dealer) { entity.TakeDamage(dr, dealer); }
}
