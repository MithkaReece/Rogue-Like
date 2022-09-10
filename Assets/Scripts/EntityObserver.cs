using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityObserver
{
    public delegate void OnDamageDelegate(DamageReport dr);
    public OnDamageDelegate OnDamageTaken = delegate { };
    public OnDamageDelegate OnDamageDealt = delegate { };

    public delegate void OnDeath();
    public delegate void OnHeal(float health);
    public delegate void OnAttack();
}
