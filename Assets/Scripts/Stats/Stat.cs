using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat<T>
{
    [field: SerializeField] public T BaseValue { get; private set; }
    private bool isDirty = true;
    [SerializeField] private T currentValue;

    public T Value
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

    //TODO: Calculate current value from modifiers
    private T CalculateCurrentValue()
    {
        return BaseValue;
    }
}