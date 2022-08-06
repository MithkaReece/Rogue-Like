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

    public IModifier GenerateModifier(Stat stat)
    {
        return InstantiateModifier(this.numberGenerator.Generate());
    }

    private IModifier InstantiateModifier(decimal value)
    {
        switch (modifierType)
        {
            case ModifierType.Additive:
                return new AdditiveModifier(value, statType);
            case ModifierType.Multiplier:
                return new MultiplicativeModifier(value, statType);
            default:
                throw new System.SystemException("Invalid modifier type");
        }
    }
}