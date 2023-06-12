using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public string name;
}

/** What does an item need
 * Types of items
 *  Weapons
 *  Armour
 *  Other
 * 
 * 
 * 
 */

public class Weapon : Item
{
    public int Damage;
    public int CritChange;
    /**
     * Damage
     * Crit chance
     * 
     */

}

public class Armour : Item
{
    public int Type;
    // Armour stat
    // Location where armour fits
    // How much poise is added

    // Extra:
    //  Weight
}