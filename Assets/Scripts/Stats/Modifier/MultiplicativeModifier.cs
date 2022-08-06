
using System;
using Microsoft.CSharp;

/// <summary>
/// Generic class for multiplicative modifiers for stats.	
/// </summary>
/// <typeparam name="decimal">Any type parameter, which implents the `*` operator</typeparam>
public class MultiplicativeModifier : Modifier
{
    public override int Order { get; } = 1000;

    public MultiplicativeModifier(decimal value) : base(value) { }
    public MultiplicativeModifier(decimal value, StatType statType) : base(value, statType) { }

    public override decimal Calculate(decimal value)
    {
        return value * (this.value as dynamic);
    }
}