using System.Collections;
using UnityEngine;

public class PlayerStats : EntityStats
{
    [field: SerializeField] public Stat<float> RollSpeed { get; private set; }
}
