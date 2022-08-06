using System;

/// <summary>
/// Generate a number between min and max.
/// </summary>
class Range : NumberGenerator
{
    public double min;
    public double max;

    public Range(double min, double max)
    {
        this.min = min;
        this.max = max;
    }

    public override T Generate<T>()
    {
        Random rand = new Random();

        IConvertible value = (rand.NextDouble() * (max - min) + min);
        return (T)value.ToType(typeof(T), null);
    }
}