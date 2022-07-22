using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityStats : MonoBehaviour
{
    [field: SerializeField] public float Damage { get; private set; }
    [field: SerializeField] public float MaxHealth { get; private set; }
    [field: SerializeField] public float CurrentHealth { get; private set; }
    [field: SerializeField] public float HealthRegen { get; private set; }
    [field: SerializeField] public float Armour { get; private set; }
    [field: SerializeField] public float MoveSpeed { get; private set; }

    public void Start()
    {
        CurrentHealth = MaxHealth;
    }

    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage * armourDamageReduction(this.Armour);
    }

    public float percentageHealth()
    {
        return CurrentHealth / MaxHealth;
    }

    public static float armourDamageReduction(float armour)
    {
        return 100 / (100 + armour);
    }
}
