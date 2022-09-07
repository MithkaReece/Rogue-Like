using System.Collections;
using UnityEngine;

public class PlayerStats : EntityStats, IDataPersistence
{
    [field: SerializeField] public Stat RollSpeed { get; private set; }
    [field: SerializeField] public Stat RollCooldown { get; private set; }
    [field: SerializeField] public Counter RollCooldownCounter { get; private set; }

    private int Kills;
    public void IncrementKills() { Kills++; }

    public void LoadData(GameData data)
    {
        CurrentHealth = data.health;
        Kills = data.kills;
    }
    public void SaveData(ref GameData data)
    {
        data.health = CurrentHealth;
        data.kills = Kills;
    }
}
