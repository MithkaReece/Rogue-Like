using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<Item> items = new List<Item>();
    public int capacity = 10;

    private Equipment equipment;

    void Awake()
    {
        equipment = GetComponent<Equipment>();    
    }

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
        //AutoEquip(item);
    }

    //Auto Equips the best item
    private void AutoEquip(Item item)
    {
        if (ReferenceEquals(equipment, null))
            return;
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
                Debug.Log("Equiped");
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

