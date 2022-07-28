using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat<T>
{
    [field: SerializeField] public T BaseValue { get; private set; }
    private bool isDirty = true;
    [SerializeField] private T currentValue;
    List<Modifier<T>> modifiers;

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

    public void AddModifier(Modifier<T> modifier)
    {
        modifiers.Add(modifier);
        isDirty = true;
    }

    public void RemoveModifier(Modifier<T> modifier)
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