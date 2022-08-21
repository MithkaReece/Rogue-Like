
using System;
using Microsoft.CSharp;

/// <summary>
/// Generic class for multiplicative modifiers for stats.	
/// </summary>
public class MultiplicativeModifier : Modifier
{
    public override int Order { get; } = 1000;

    public MultiplicativeModifier(double value) : base(value) { }
    public MultiplicativeModifier(double value, StatType statType) : base(value, statType) { }

    public override double Calculate(double value)
    {
        return value * (this.value as dynamic);
    }
}