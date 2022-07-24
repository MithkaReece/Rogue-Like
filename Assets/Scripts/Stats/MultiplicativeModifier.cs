
using Microsoft.CSharp;

/// <summary>
/// Generic class for multiplicative modifiers for stats.	
/// </summary>
/// <typeparam name="T">Any type parameter, which implents the `*` operator</typeparam>
public class MultiplicativeModifier<T> : IModifier<T>
{
    private T value;

    public MultiplicativeModifier(T value)
    {
        this.value = value;
    }

    public T Apply(Stat<T> stat)
    {
        return (stat.BaseValue as dynamic) * (value as dynamic);
    }
}