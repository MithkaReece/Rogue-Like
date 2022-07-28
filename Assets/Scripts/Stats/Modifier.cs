public abstract class Modifier<T> : IModifier
{
    protected T value;
    public StatType StatType { get; }
    public Stat<T> Stat { get; }

    // Determines the Order in which the types of modifiers are applied.
    public virtual int Order { get; } = 0;

    public Modifier(T value)
    {
        this.value = value;
    }

    public void Apply()
    {
        Stat.AddModifier(this);
    }

    public void Remove()
    {
        Stat.RemoveModifier(this);
    }

    /// <summary>
    /// Calculates the effect of the modifier on the given value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual T Calculate(T value) => value;
}