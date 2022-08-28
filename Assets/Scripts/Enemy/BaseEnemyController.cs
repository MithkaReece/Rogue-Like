using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]

public class BaseEnemyController : EnemyController
{
    public Attack[] AvailableAttacks = new Attack[]{
        new Attack("1",5)
    };

    public BaseEnemyController()
    {

    }

    void FixedUpdate()
    {
        BasicMovement();
    }

    void BasicMovement()
    {

    }


    protected override void Start() { base.Start(); }

}

public class Attack
{
    public double Range { get; private set; }
    public string Name { get; private set; }

    public Attack(string name, double range)
    {
        Name = name;
        Range = range;
    }
}

