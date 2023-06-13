using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
    private float MAX_DAMAGE_NEGATION = 0.8f;//0.8 is at max negates 80% of damage
    private float REPOS_REGEN_SPEED = 0.2f; //0.2 is 20% per second


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /**
     * Entities need equiped items
     * Armour
     * Weapons
     * Entity Contacts StatsManager for its stats based on its level and items
     * Entity gives info on items, levels, etc
     * Stats Manager udates EntityStats with calculated information
     */



    // Updates dynamic stats
    public void Update_Stats(EntityStats entityStats)
    {

    }

    /**
     * Basic stats needed
     * Health
     * Armour (From items)
     * Damage + Critc change (From items)
     * Poise (From items)
     * 
     * Dodging:
     *  Roll speed & distance
     *  Invulnerability frames
     */

    /**
     * Initial player stats
     * Initial stats for each enemy
     * 
     * Weapons
     * Damage & speed (dps)
     * 
     * 
     * 
     * Poise is max repos taken
     * Repos is based on damage
     * Add damage to repos
     * Total Damage = (Weapon Damage + Crit) * (Armour * Max damage negation)
     * 
     * Armour = 0-100%
     * Max damage negation
     * 
     * Damage = weapon damage
     * 
     *
     * Repos regen speed should be constant percentage based
     * so decreases by a percentage every second
     * 
     * Repos cooldown:
     * Time decreases with no hits, if hit, increase time proportional 
     * to percentage repos received up to max cooldown
     * Max_Damage for full cooldown can be a function given poise
     * For example exponential function
     * 
     * 
     * 
     * Critical hits?
     * Some kind of boost to damage
     * Function can be defined later
     * 
     * 
     * 
     * Health does not normally regen
     * (Health regen can be an extra feature, for now have none)
     * MoveSpeed: Make constant for now
     */
}
