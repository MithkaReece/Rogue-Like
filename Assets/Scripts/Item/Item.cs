using UnityEngine;
using System.Collections.Generic;

public abstract class Item : IItem
{
    public string name;
    public Sprite icon;
    public int itemID;
    public List<IModifier> modifiers;

    public void OnEquip()
    {
        modifiers.ForEach(mod => mod.Apply());
    }

    public void OnUnEquip()
    {
        modifiers.ForEach(mod => mod.Remove());
    }

    public void OnPickUp(PlayerStats playerStats)
    {
        modifiers.ForEach(mod => mod.PickedUp(playerStats));
    }

    public void OnDrop()
    {
        modifiers.ForEach(mod => mod.Dropped());
    }
}