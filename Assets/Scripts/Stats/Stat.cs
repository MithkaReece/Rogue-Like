using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat
{
    [field: SerializeField] public double BaseValue { get; private set; }
    private bool isDirty = true;
    [SerializeField] private double currentValue;
    List<Modifier> modifiers = new List<Modifier>();

    public double Value
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

    private double CalculateCurrentValue()
    {
        modifiers.Sort((a, b) => a.Order.CompareTo(b.Order));

        double currentValue = BaseValue;

        foreach (var mod in modifiers)
        {
            currentValue = mod.Calculate(currentValue);
        }

        return currentValue;
    }
}