using System.Collections.Generic;
using UnityEngine;

public static class StatTranslator
{
    public static Stat<float> GetStat(EntityStats entityStats, Modifier<float> modifier) => modifier.StatType switch
    {
        StatType.MaxHealth => entityStats.MaxHealth,
        StatType.HealthRegen => entityStats.HealthRegen,
        StatType.Armour => entityStats.Armour,
        StatType.MoveSpeed => entityStats.MoveSpeed,
        StatType.AttackSpeed => entityStats.Combat.AttackSpeed,
        StatType.Damage => entityStats.Combat.Damage,
        _ => throw new System.Exception($"StatType {modifier.StatType} is not supported by EntityStats.")
    };

    public static Stat<float> GetStat(PlayerStats playerStats, Modifier<float> modifier) => modifier.StatType switch
    {
        StatType.RollSpeed => playerStats.RollSpeed,
        StatType.RollCooldown => playerStats.RollCooldown,
        _ => GetStat((EntityStats)playerStats, modifier)
    };

    public static Stat<T> GetStat<T>(EntityStats _entityStats, Modifier<T> modifier)
    {
        throw new System.NotSupportedException("Type not supported");
    }
}