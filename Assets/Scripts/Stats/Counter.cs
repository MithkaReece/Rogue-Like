using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Counter
{
    [SerializeField] float defaultValue;
    float currentValue;

    public Counter()
    {
        currentValue = defaultValue;
    }

    public void Reset()
    {
        currentValue = defaultValue;
    }

    public void Reset(float newDefault)
    {
        defaultValue = newDefault;
        currentValue = defaultValue;
    }

    public void Reset(decimal newDefault) => Reset((float)newDefault);

    public void Reset(Stat newDefault)
    {
        defaultValue = (float)newDefault.Value;
        currentValue = defaultValue;
    }

    /// <summary>
    /// Has the counter reached 0?
    /// </summary>
    /// <returns></returns>
    public bool Passed { get { return currentValue <= 0; } }

    public void PassTime(float time)
    {
        currentValue -= time;
    }
}