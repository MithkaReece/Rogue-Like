using System.Collections.Generic;
using UnityEngine;

class Inventory
{
    public List<Item> items = new List<Item>();
    public int capacity = 10;

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
        foreach (Item item in items)
        {
            Debug.Log(item.name);
        }
    }

    public void Equip(int num)
    {
        //Get item at num
        //if equiped, remove from inventory
    }

    

}

class Equipment
{
    Armour[] pieces; //Head, Chest, Legs, Boots
    Weapon weapon;


    public bool Equip(Inventory inv, Armour armour)
    {
        if (armour.Type < 0 || armour.Type > 3)
            return false;
        Armour pastPiece = pieces[armour.Type];
        pieces[armour.Type] = armour;
        inv.AddItem(pastPiece);
        return true;
    }

    public bool Equip(Inventory inv, Weapon newWeapon)
    {
        Weapon pastWeapon = weapon;
        weapon = newWeapon;
        inv.AddItem(pastWeapon);
        return true;
    }

}