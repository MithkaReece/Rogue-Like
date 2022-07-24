using System.Collections;
using UnityEngine;

[System.Serializable]
public class CombatStats
{
    [field: SerializeField] public Stat<float> AttackSpeed { get; private set; }
    [field: SerializeField] public Counter AttackCooldownCounter { get; private set; }
    [field: SerializeField] public Stat<float> Damage { get; private set; }
}

public class EntityStats : MonoBehaviour
{
    [field: SerializeField] public Stat<float> MaxHealth { get; private set; }
    [field: SerializeField] public float CurrentHealth { get; private set; }
    [field: SerializeField] public Stat<float> HealthRegen { get; private set; }
    [field: SerializeField] public Stat<float> Armour { get; private set; }
    [field: SerializeField] public Stat<float> MoveSpeed { get; private set; }
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
