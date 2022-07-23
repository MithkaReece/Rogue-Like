using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : EntityStats
{
    public readonly Stat<float> knockbackDuration;
    public readonly Stat<float> knockbackPower;
}
