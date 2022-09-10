using System;

/// <summary>
/// Generate a number between min and max.
/// </summary>
class Range : NumberGenerator
{
    public float min;
    public float max;

    public Range(float min, float max)
    {
        this.min = min;
        this.max = max;
    }

    public override float Generate()
    {
        Random rand = new Random();

        return (rand.Next() * (max - min) + min);
    }
}