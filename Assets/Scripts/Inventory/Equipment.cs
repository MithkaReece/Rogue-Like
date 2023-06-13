using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    public Armour[] pieces { get; private set; } //Head, Chest, Legs, Boots
    public Weapon weapon { get; private set; }

    void Start()
    {
        pieces = new Armour[4];
    }


    public bool Equip(Inventory inv, Item item)
    {
        if (item is Armour armour)
        {
            Armour pastPiece = pieces[armour.Type];
            pieces[armour.Type] = armour;
            inv.AddItem(pastPiece);
            return true;
        }
        else if (item is Weapon newWeapon)
        {
            Weapon pastWeapon = weapon;
            weapon = newWeapon;
            inv.AddItem(pastWeapon);
            return true;
        }
        return false;
    }

    public bool UnEquip(Inventory inv, int Index)
    {
        if (Index < 0 || Index > 3)
            return false;
        Armour pastPiece = pieces[Index];
        inv.AddItem(pastPiece);
        return true;
    }

    public bool UnEquip(Inventory inv)
    {
        Weapon pastWeapon = weapon;
        inv.AddItem(pastWeapon);
        return true;
    }

    public void Print()
    {
        if (weapon is not null)
            Debug.Log(weapon.Name);
    }

}