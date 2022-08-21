using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : EntityStats
{
    [field: SerializeField] public Stat KnockbackDuration { get; private set; }
    [field: SerializeField] public Stat KnockbackPower { get; private set; }
}
