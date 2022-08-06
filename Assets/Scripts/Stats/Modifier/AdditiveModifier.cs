/// <summary>
/// Generic class for additive modifiers for stats.	
/// </summary>
public class AdditiveModifier : Modifier
{
    public override int Order { get; } = 10000;
    public AdditiveModifier(decimal value) : base(value) { }
    public AdditiveModifier(decimal value, StatType statType) : base(value, statType) { }

    public override decimal Calculate(decimal value)
    {
        return value * (this.value as dynamic);
    }
}