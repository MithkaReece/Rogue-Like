using System;
public enum ModifierType
{
    Additive,
    Multiplier
}

[System.Serializable]
class ModifierGenerator
{
    public StatType statType;

    public ModifierType modifierType;

    public NumberGenerator numberGenerator;

    public IModifier GenerateModifier<T>(Stat<T> stat) where T : IConvertible
    {
        return InstantiateModifier<T>(this.numberGenerator.Generate<T>());
    }

    private IModifier InstantiateModifier<T>(T value)
    {
        switch (modifierType)
        {
            case ModifierType.Additive:
                return new AdditiveModifier<T>(value, statType);
            case ModifierType.Multiplier:
                return new MultiplicativeModifier<T>(value, statType);
            default:
                throw new System.SystemException("Invalid modifier type");
        }
    }
}