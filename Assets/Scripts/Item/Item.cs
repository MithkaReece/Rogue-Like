using UnityEngine;
using System.Collections.Generic;

public abstract class Item : IItem
{
    public string name;
    public Sprite icon;
    public int itemID;
    public List<IModifier> modifiers;
    private PlayerStats heldBy;

    public virtual void OnEquip()
    {
        modifiers.ForEach(mod => mod.Apply());
    }

    public virtual void OnUnEquip()
    {
        modifiers.ForEach(mod => mod.Remove());
    }

    public virtual void OnPickUp(PlayerStats playerStats)
    {
        modifiers.ForEach(mod => mod.PickedUp(playerStats));
        heldBy = playerStats;
    }

    public virtual void OnDrop()
    {
        modifiers.ForEach(mod => mod.Dropped());
        heldBy = null;
    }
}