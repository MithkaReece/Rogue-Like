using System.Collections;
using UnityEngine;

public class PlayerStats : EntityStats, IDataPersistence
{
    [field: SerializeField] public Stat RollSpeed { get; private set; }
    [field: SerializeField] public Stat RollCooldown { get; private set; }
    [field: SerializeField] public Counter RollCooldownCounter { get; private set; }

    public void LoadData(GameData data)
    {
        CurrentHealth = data.health;
    }
    public void SaveData(ref GameData data)
    {
        data.health = CurrentHealth;
    }
}
