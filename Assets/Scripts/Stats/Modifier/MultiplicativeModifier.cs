
using System;
using Microsoft.CSharp;

/// <summary>
/// Generic class for multiplicative modifiers for stats.	
/// </summary>
public class MultiplicativeModifier : Modifier
{
    public override int Order { get; } = 1000;

    public MultiplicativeModifier(float value) : base(value) { }
    public MultiplicativeModifier(float value, StatType statType) : base(value, statType) { }

    public override float Calculate(float value)
    {
        return value * (this.value as dynamic);
    }
}