using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public string Name;
    //Icon image
    public Item(string name)
    {
        Name = name;
    }
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

//Basic Weapon, one attack move
public class Weapon : Item
{
    public int Damage;
    public float CritChance;
    private float attackSeconds; //How many seconds between attack
    public float AttackSpeed { get { return 1f / attackSeconds;  } set { attackSeconds = 1f/value; } }

    public Weapon(string name, int damage, float critChance, float attackSpeed) : base(name)
    {
        Damage = damage;
        CritChance = critChance;
        AttackSpeed = attackSpeed;
    }

    private System.DateTime LastAttackTime = System.DateTime.MaxValue;

    public bool CanAttack()
    {
        System.TimeSpan diff = LastAttackTime - System.DateTime.Now;
        if (diff.TotalSeconds > attackSeconds)
        {
            LastAttackTime = System.DateTime.Now;
            return true;
        }
        return false;
    }

    

    /**
     * Damage
     * Crit chance
     * 
     * Animation Frames
     */

}

public class Armour : Item
{
    private int _Type;
    public int Type { get { return _Type; } set { _Type = Mathf.Clamp(value, 0, 3); } }
    public int DamageNegation;

    public Armour(string name, int type, int damageNegation) : base(name)
    {
        Type = type;
        DamageNegation = damageNegation;
    }
    // Armour stat
    // Location where armour fits
    // How much poise is added
    //
    // Wearing image/overlay

    // Extra:
    //  Weight
}
