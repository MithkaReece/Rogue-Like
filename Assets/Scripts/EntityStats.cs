using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CombatStats
{
    public readonly Stat<float> AttackSpeed;
    [field: SerializeField] public float AttackCooldownCounter { get; private set; }
    public readonly Stat<float> Damage;
}

public class EntityStats : MonoBehaviour
{
    public readonly Stat<float> MaxHealth;
    [field: SerializeField] public float CurrentHealth { get; private set; }
    public readonly Stat<float> HealthRegen;
    public readonly Stat<float> Armour;
    public readonly Stat<float> MoveSpeed;
    [field: SerializeField] public CombatStats Combat { get; private set; }

    public void Start()
    {
        CurrentHealth = MaxHealth.Value;
    }

    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage * armourDamageReduction(this.Armour.Value);
    }

    public float percentageHealth()
    {
        return CurrentHealth / MaxHealth.Value;
    }

    public static float armourDamageReduction(float armour)
    {
        return 100 / (100 + armour);
    }
}
