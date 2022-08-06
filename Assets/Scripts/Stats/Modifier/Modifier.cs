public abstract class Modifier : IModifier
{
    protected decimal value;
    public StatType StatType { get; }
    public Stat Stat { get; private set; }

    // Determines the Order in which the types of modifiers are applied.
    public virtual int Order { get; } = 0;

    public Modifier(decimal value)
    {
        this.value = value;
    }

    public Modifier(decimal value, StatType statType)
    {
        this.value = value;
        this.StatType = statType;
    }

    public void Apply()
    {
        Stat.AddModifier(this);
    }

    public void Remove()
    {
        Stat.RemoveModifier(this);
    }

    public void PickedUp(PlayerStats playerStats)
    {
        Stat = StatTranslator.GetStat(playerStats, this);
        Apply();
    }

    public void Dropped()
    {
        Remove();
        Stat = null;
    }

    /// <summary>
    /// Calculates the effect of the modifier on the given value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual decimal Calculate(decimal value) => value;
}