using System.Collections;
using UnityEngine;

[System.Serializable]
public class CombatStats
{
    [field: SerializeField] public Stat AttackSpeed { get; private set; }
    [field: SerializeField] public Stat AttackRange { get; private set; }

    [field: SerializeField] public Counter AttackCooldownCounter { get; private set; }
    [field: SerializeField] public Stat Damage { get; private set; }
}

public class EntityStats : MonoBehaviour
{
    [field: SerializeField] public Stat MaxHealth { get; private set; }
    [field: SerializeField] public float CurrentHealth { get; protected set; }
    [field: SerializeField] public Stat HealthRegen { get; private set; }
    [field: SerializeField] public Stat Armour { get; private set; }
    [field: SerializeField] public Stat MoveSpeed { get; private set; }
    [field: SerializeField] public CombatStats Combat { get; private set; }

    public void Start()
    {
        CurrentHealth = MaxHealth.Value;
    }

    public void TakeDamage(DamageReport dr, EntityController dealer)
    {
        CurrentHealth -= dr.damage * armourDamageReduction(this.Armour.Value);
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
