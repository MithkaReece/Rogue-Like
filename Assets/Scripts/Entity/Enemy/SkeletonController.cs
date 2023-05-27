using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonController : BaseEnemyController
{
    protected override void Start()
    {
        base.Start();
        intendedAttack = AvailableAttacks[0];
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    //All have (States)
    //Attack
    //Follow

    //Some back off
    //Some block
    //Some parry
    //Maybe some kite


}


/*
Plan of generic enemy
What an enemy will have
Enemy will have folder of animations (labelled by enemy name)
From this, available attacks can be extracted
Each attack is a group (sprites) animation

Variations of enemy
Easiest:
    Scales
    Stats:
        Health
        Speeds
        Armour
        Attack speed
        Damage

Which AIs to use:

If no player, then wander 
To optimise either have enemies spawn and despawn on the fly
Or freeze state when off screen

Detection radius
    -Sounds radius, large circle (if you attack it makes sound)
    -Sight radius, semi circle in direction of enemy (large)
    -Smell radisu, very small radius when enemy is close

When AI sees enemy it goes into attack mode 
Have all these states:
they can change between ones available
Types of AI:
    -Constant followers
    -Back and forth (rnd but slightly strategic)
    -Blockers
    -Quick and can parry
    
Types of enemies:
    -Quick, weak damage & health
    -Slow, high health
    -High damage, weak health, mid speed
    -Low health, parry, semi quick
    -High armour, health, slow with some quick attacks

*/