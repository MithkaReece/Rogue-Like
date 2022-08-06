using System.Collections;
using UnityEngine;

[System.Serializable]
public class CombatStats
{
    [field: SerializeField] public Stat AttackSpeed { get; private set; }
    [field: SerializeField] public Counter AttackCooldownCounter { get; private set; }
    [field: SerializeField] public Stat Damage { get; private set; }
}

public class EntityStats : MonoBehaviour
{
    [field: SerializeField] public Stat MaxHealth { get; private set; }
    [field: SerializeField] public decimal CurrentHealth { get; private set; }
    [field: SerializeField] public Stat HealthRegen { get; private set; }
    [field: SerializeField] public Stat Armour { get; private set; }
    [field: SerializeField] public Stat MoveSpeed { get; private set; }
    [field: SerializeField] public CombatStats Combat { get; private set; }

    public void Start()
    {
        CurrentHealth = MaxHealth.Value;
    }

    public void TakeDamage(decimal damage)
    {
        CurrentHealth -= damage * armourDamageReduction(this.Armour.Value);
    }

    public decimal percentageHealth()
    {
        return CurrentHealth / MaxHealth.Value;
    }

    public static decimal armourDamageReduction(decimal armour)
    {
        return 100 / (100 + armour);
    }
}
