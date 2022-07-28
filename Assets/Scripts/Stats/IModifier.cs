

/// <summary>
/// A generic-agnostic interface for stat modifiers. For generic-dependent implementation,
/// see the abstract class `Modifier`.
/// </summary>
public interface IModifier
{
    public StatType StatType { get; }

    /// <summary>
    /// Apply the modifier to the stat. Targeted stat defined in `Modifier` class.
    /// </summary>
    public void Apply();

    /// <summary>
    /// Remove the modifier from the stat. Targeted stat defined in `Modifier` class.
    /// </summary>
    public void Remove();
}