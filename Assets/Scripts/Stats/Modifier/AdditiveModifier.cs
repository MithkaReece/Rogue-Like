/// <summary>
/// Generic class for additive modifiers for stats.	
/// </summary>
public class AdditiveModifier : Modifier
{
    public override int Order { get; } = 10000;
    public AdditiveModifier(double value) : base(value) { }
    public AdditiveModifier(double value, StatType statType) : base(value, statType) { }

    public override double Calculate(double value)
    {
        return value * (this.value as dynamic);
    }
}