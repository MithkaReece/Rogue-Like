using System.Collections;
using UnityEngine;

public class PlayerStats : EntityStats, IDataPersistence
{
    [field: SerializeField] public float RollSpeed { get; private set; }
    [field: SerializeField] public float RollCooldown { get; private set; }
    [field: SerializeField] public Counter RollCooldownCounter { get; private set; }

    private int Kills;
    public void IncrementKills() { Kills++; }
    private int Deaths;

    public void ResetOnDeath()
    {
        Deaths++;
        CurrentHealth = MaxHealth;
    }

    public void LoadData(GameData data)
    {
        CurrentHealth = data.health;
        Kills = data.kills;
        Deaths = data.deaths;
    }
    public void SaveData(ref GameData data)
    {
        data.health = CurrentHealth;
        data.kills = Kills;
        data.deaths = Deaths;
    }
}
