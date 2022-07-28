/// <summary>
/// Generic class for additive modifiers for stats.	
/// </summary>
/// <typeparam name="T">Any type parameter, which implents the `+` operator</typeparam>
public class AdditiveModifier<T> : Modifier<T>
{
    public override int Order { get; } = 10000;
    public AdditiveModifier(T value) : base(value) { }

    public override T Calculate(T value)
    {
        return value * (this.value as dynamic);
    }
}