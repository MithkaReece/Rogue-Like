using System.Collections;
using UnityEngine;

public class PlayerStats : EntityStats
{
    [field: SerializeField] public Stat RollSpeed { get; private set; }
    [field: SerializeField] public Stat RollCooldown { get; private set; }
    [field: SerializeField] public Counter RollCooldownCounter { get; private set; }
}
