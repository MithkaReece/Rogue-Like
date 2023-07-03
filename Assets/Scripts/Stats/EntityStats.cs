using System.Collections;
using UnityEngine;


[System.Serializable]
//TODO: Remove combat stats as they will be tied with weapons
public class CombatStats
{
    [field: SerializeField] public float AttackSpeed { get; private set; }
    [field: SerializeField] public float AttackRange { get; private set; }

    [field: SerializeField] public Counter AttackCooldownCounter { get; private set; }
    [field: SerializeField] public float Damage { get; private set; }
}

public class EntityStats
{
    [field: SerializeField] public float MaxHealth { get; private set; }


    #region Health
    //Only Call OnHealthChanged event if health actually changed
    public delegate void EventHandler();
    public event EventHandler OnHealthChanged;
    //Subscribers: HealthRingController

    private float currentHealth;
    public float CurrentHealth
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
    #endregion

    [field: SerializeField] public float MoveSpeed = 2f;
    [field: SerializeField] public CombatStats Combat;

    [field: SerializeField] public float Poise = 1f;
    [field: SerializeField] public float ReposRegenSpeed;
    [field: SerializeField] public float ReposCooldownSeconds;


    [field: SerializeField] public float RollSpeed { get; private set; }
    [field: SerializeField] public float RollCooldownSeconds { get; private set; }
    private System.DateTime LastRoll;
    public bool CanRoll() {
        if(LastRoll == System.DateTime.MinValue) {
            LastRoll = System.DateTime.Now;
            return true;
        }
        if(LastRoll.AddSeconds(RollCooldownSeconds) < System.DateTime.Now) {
            LastRoll = System.DateTime.Now;
            return true;
        }
        return false;
    }

    public float Armour = 0f;


    public EntityStats(float maxHealth,
            float poise, float reposRegenSpeed, float reposCooldownSeconds,
            float rollSpeed, float rollCoolDownSeconds) {
        MaxHealth = maxHealth;
        CurrentHealth = MaxHealth;
        Poise = poise;
        ReposRegenSpeed = reposRegenSpeed;
        ReposCooldownSeconds = reposCooldownSeconds;
        RollSpeed = rollSpeed;
        RollCooldownSeconds = rollCoolDownSeconds;
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
