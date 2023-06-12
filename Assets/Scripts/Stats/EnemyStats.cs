using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : EntityStats
{
    [field: SerializeField] public float KnockbackDuration { get; private set; }
    [field: SerializeField] public float KnockbackPower { get; private set; }
}
