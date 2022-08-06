using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat
{
    [field: SerializeField] public decimal BaseValue { get; private set; }
    private bool isDirty = true;
    [SerializeField] private decimal currentValue;
    List<Modifier> modifiers;

    public decimal Value
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

    private decimal CalculateCurrentValue()
    {
        modifiers.Sort((a, b) => a.Order.CompareTo(b.Order));

        decimal currentValue = BaseValue;

        foreach (var mod in modifiers)
        {
            currentValue = mod.Calculate(currentValue);
        }

        return currentValue;
    }
}