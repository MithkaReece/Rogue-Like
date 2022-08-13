using UnityEngine;

public enum ModifierType
{
    Additive,
    Multiplier
}

[System.Serializable]
public class ModifierGenerator
{
    [SerializeField]
    public StatType statType = StatType.MaxHealth;

    [SerializeField]
    public ModifierType modifierType = ModifierType.Additive;

    public NumberGenerator numberGenerator;

    public IModifier GenerateModifier(Stat stat)
    {
        return InstantiateModifier(this.numberGenerator.Generate());
    }

    private IModifier InstantiateModifier(double value)
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