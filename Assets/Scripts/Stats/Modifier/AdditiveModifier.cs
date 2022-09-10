/// <summary>
/// Generic class for additive modifiers for stats.	
/// </summary>
public class AdditiveModifier : Modifier
{
    public override int Order { get; } = 10000;
    public AdditiveModifier(float value) : base(value) { }
    public AdditiveModifier(float value, StatType statType) : base(value, statType) { }

    public override float Calculate(float value)
    {
        return value * (this.value as dynamic);
    }
}