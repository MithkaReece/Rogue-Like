using System;
using System.Collections.Generic;
using UnityEngine;

public static class StatTranslator
{
    public static Stat GetStat(EntityStats entityStats, Modifier modifier) => modifier.StatType switch
    {
        StatType.MaxHealth => entityStats.MaxHealth,
        StatType.HealthRegen => entityStats.HealthRegen,
        StatType.Armour => entityStats.Armour,
        StatType.MoveSpeed => entityStats.MoveSpeed,
        StatType.AttackSpeed => entityStats.Combat.AttackSpeed,
        StatType.Damage => entityStats.Combat.Damage,
        _ => throw new System.Exception($"StatType {modifier.StatType} is not supported by EntityStats.")
    };

    public static Stat GetStat(PlayerStats playerStats, Modifier modifier) => modifier.StatType switch
    {
        StatType.RollSpeed => playerStats.RollSpeed,
        StatType.RollCooldown => playerStats.RollCooldown,
        _ => GetStat((EntityStats)playerStats, modifier)
    };

    public static Type GetDefaultStat(StatType statType)
    {
        return typeof(float);
    }
}