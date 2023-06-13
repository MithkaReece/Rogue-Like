using System.Collections;
using UnityEngine;


[System.Serializable]
public class CombatStats
{
    [field: SerializeField] public float AttackSpeed { get; private set; }
    [field: SerializeField] public float AttackRange { get; private set; }

    [field: SerializeField] public Counter AttackCooldownCounter { get; private set; }
    [field: SerializeField] public float Damage { get; private set; }
}

public class EntityStats : MonoBehaviour
{
    [field: SerializeField] public float MaxHealth { get; private set; }
    
    public delegate void EventHandler();
    public event EventHandler OnHealthChanged;

    private float currentHealth;
    [field: SerializeField] public float CurrentHealth
    {
        get
        {
            return currentHealth;
        }
        set
        {
            if (value != currentHealth)
            {
                currentHealth = value;
                OnHealthChanged?.Invoke();
            }
        }
    }
    [field: SerializeField] public float HealthRegen;
    [field: SerializeField] public float Armour;
    [field: SerializeField] public float MoveSpeed = 1f;
    [field: SerializeField] public CombatStats Combat;

    [field: SerializeField] public float Poise = 1f;
    [field: SerializeField] public float ReposRegenSpeed;
    [field: SerializeField] public float ReposCooldown;

    public void Start()
    {
        CurrentHealth = MaxHealth;
    }

    public void TakeDamage(DamageReport dr, EntityController dealer)
    {
        CurrentHealth -= dr.damage * armourDamageReduction(this.Armour);
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
