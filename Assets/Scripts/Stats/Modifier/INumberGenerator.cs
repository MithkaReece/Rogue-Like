using System;

[System.Serializable]
public abstract class NumberGenerator
{
    public virtual T Generate<T>() where T : IConvertible
    {
        return default(T);
    }
}