using System;

[System.Serializable]
public abstract class NumberGenerator
{
    public virtual double Generate()
    {
        return default(double);
    }
}