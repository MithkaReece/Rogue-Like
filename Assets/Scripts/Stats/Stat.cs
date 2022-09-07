using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat
{
    [field: SerializeField] public float BaseValue { get; private set; }
    private bool isDirty = true;
    [SerializeField] private float currentValue;
    List<Modifier> modifiers = new List<Modifier>();

    public float Value
    {
        get
        {
            if (isDirty)
            {
                currentValue = CalculateCurrentValue();
                isDirty = false;
            }

            return currentValue;
        }
    }

    public void AddModifier(Modifier modifier)
    {
        modifiers.Add(modifier);
        isDirty = true;
    }

    public void RemoveModifier(Modifier modifier)
    {
        modifiers.Remove(modifier);
        isDirty = true;
    }

    private float CalculateCurrentValue()
    {
        //Debug.Log(modifiers);
        modifiers.Sort((a, b) => a.Order.CompareTo(b.Order));

        float currentValue = BaseValue;

        foreach (var mod in modifiers)
        {
            currentValue = (float)mod.Calculate(currentValue);
        }

        return currentValue;
    }
}