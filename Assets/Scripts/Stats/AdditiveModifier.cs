/// <summary>
/// Generic class for additive modifiers for stats.	
/// </summary>
/// <typeparam name="T">Any type parameter, which implents the `+` operator</typeparam>
public class AdditiveModifier<T> : IModifier<T>
{
    private T value;

    public AdditiveModifier(T value)
    {
        this.value = value;
    }

    public T Apply(Stat<T> stat)
    {
        return (stat.BaseValue as dynamic) + (value as dynamic);
    }
}