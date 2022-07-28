
using System;
using Microsoft.CSharp;

/// <summary>
/// Generic class for multiplicative modifiers for stats.	
/// </summary>
/// <typeparam name="T">Any type parameter, which implents the `*` operator</typeparam>
public class MultiplicativeModifier<T> : Modifier<T>
{
    public MultiplicativeModifier(T value) : base(value) { }

    public override T Calculate(T value)
    {
        return value * (this.value as dynamic);
    }
}