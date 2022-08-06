using System;

[System.Serializable]
public abstract class NumberGenerator
{
    public virtual decimal Generate()
    {
        return default(decimal);
    }
}