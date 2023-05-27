using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    public List<Item> items = new List<Item>();
    public int capacity = 10;

    public void AddItem(Item item)
    {
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

    public void UseItem(Item item)
    {
        //item.Use();
        throw new System.NotImplementedException();
    }

    public void DiscardItem(Item item)
    {
        items.Remove(item);
    }

    public void DiscardAllItems()
    {
        items.Clear();
    }

    public void PrintItems()
    {
        foreach (Item item in items)
        {
            Debug.Log(item.name);
        }
    }

}