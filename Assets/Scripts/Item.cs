using UnityEngine;
using System.Collections.Generic;

public abstract class Item : IItem
{
    public string name;
    public Sprite icon;
    public int itemID;
    public List<IModifier> modifiers;

    public void onEquip()
    {
        modifiers.ForEach(mod => mod.Apply());
    }

    public void onUnEquip()
    {
        modifiers.ForEach(mod => mod.Remove());
    }
}