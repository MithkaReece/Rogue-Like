using System.Collections;
using UnityEngine;

public class PlayerStats : EntityStats
{
    [field: SerializeField] public Stat<float> RollSpeed { get; private set; }
    [field: SerializeField] public Stat<float> RollCooldown { get; private set; }
    [field: SerializeField] public Counter RollCooldownCounter { get; private set; }
}
