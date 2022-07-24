using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : EntityStats
{
    [field: SerializeField] public Stat<float> KnockbackDuration { get; private set; }
    [field: SerializeField] public Stat<float> KnockbackPower { get; private set; }
}
