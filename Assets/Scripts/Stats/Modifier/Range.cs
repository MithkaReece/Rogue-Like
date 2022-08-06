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

    public override decimal Generate()
    {
        Random rand = new Random();

        return (decimal)(rand.NextDouble() * (max - min) + min);
    }
}