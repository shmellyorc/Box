namespace Box.Loaders.Maps.Settings;

/// <summary>
/// Represents a boolean setting.
/// </summary>
public sealed class BoolSetting : MapSetting
{
    /// <summary>
    /// Gets the value of the boolean setting.
    /// </summary>
    public new bool Value => (bool)base.Value;

    internal BoolSetting(bool value) : base(value) { }
}
