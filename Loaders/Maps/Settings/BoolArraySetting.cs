namespace Box.Loaders.Maps.Settings;

/// <summary>
/// Represents a boolean array setting.
/// </summary>
public sealed class BoolArraySetting : MapSetting
{
    /// <summary>
    /// Gets the value of the boolean array setting as a new List of boolean values.
    /// </summary>
    public new List<bool> Value => new((List<bool>)base.Value);

    internal BoolArraySetting(List<bool> value) : base(value) { }
}
