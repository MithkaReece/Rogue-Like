using System;

[System.Serializable]
public abstract class NumberGenerator
{
    public virtual float Generate()
    {
        return default(float);
    }
}