using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat<T>
{

    public T BaseValue { get; private set; }
    private bool isDirty = true;
    private T currentValue;

    public T Value
    {
        get
        {
            if (!isDirty)
                return currentValue;

            currentValue = CalculateCurrentValue();
            isDirty = false;
            return currentValue;
        }
    }

    List<IModifier<T>> modifiers;

    public void AddModifier(IModifier<T> modifier)
    {
        modifiers.Add(modifier);
        isDirty = true;
    }

    public void removeModifier(IModifier<T> modifier)
    {
        modifiers.Remove(modifier);
        isDirty = true;
    }

    private T CalculateCurrentValue()
    {
        throw new System.NotImplementedException();
    }
}