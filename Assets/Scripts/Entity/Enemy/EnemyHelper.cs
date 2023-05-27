using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHelper : Helper
{
    private EnemyController enemy
    {
        get { return (EnemyController)entity; }
    }

}
