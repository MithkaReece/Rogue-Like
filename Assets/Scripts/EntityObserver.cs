using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityObserver
{
    public delegate DamageReport OnDamageDelegate(DamageReport dr);
    public OnDamageDelegate OnDamageTaken;
    public OnDamageDelegate OnDamageDealt;

    public delegate void OnDeath();
    public delegate void OnHeal(decimal health);
    public delegate void OnAttack();
}
