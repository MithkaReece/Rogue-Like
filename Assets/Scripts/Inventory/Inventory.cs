using System;
using System.Collections.Generic;
using UnityEngine;

class Inventory : MonoBehaviour
{
    public List<Item> items = new List<Item>();
    public int capacity = 10;

    private Equipment equipment = new Equipment();

    public void AddItem(Item item)
    {
        if (item is null)
            return;
        if (items.Count >= capacity)
        {
            Debug.Log("Inventory is full");
            return;
        }
        items.Add(item);
        AutoEquip(item);
    }

    //Auto Equips the best item
    private void AutoEquip(Item item)
    {
        if (item is Armour armour)
        {
            if (equipment.pieces[armour.Type] is null || armour.DamageNegation > equipment.pieces[armour.Type].DamageNegation)
            {
                equipment.Equip(this, armour);
            }
        }
        if (item is Weapon weapon)
        {
            if (equipment.weapon is null || weapon.Damage > equipment.weapon.Damage)
            {
                equipment.Equip(this, weapon);
            }
        }
    }

    public void RemoveItem(Item item)
    {
        items.Remove(item);
    }
    //TODO: Probably remove this and implement somewhere else
    public void UseItem(Item item)
    {
        //item.Use();
        throw new System.NotImplementedException();
    }

    public void DiscardItem(Item item)
    {
        items.Remove(item);
    }

    //WILL REMOVE LATER 
    public void PrintItems()
    {
        if (items.Count == 0)
            Debug.Log("Empty Inventory");
        foreach (Item item in items)
        {
            Debug.Log(item.Name);
        }
    }

    public void Equip(int index)
    {
        if (index < 0 || index > items.Count - 1)
            return;

        if (equipment.Equip(this, items[index]))
            RemoveItem(items[index]);
    }

    

}

class Equipment
{
    public Armour[] pieces { get; private set; } //Head, Chest, Legs, Boots
    public Weapon weapon { get; private set; }

    public Equipment()
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
        }else if (item is Weapon newWeapon)
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

}