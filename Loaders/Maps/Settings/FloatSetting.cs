namespace Box.Loaders.Maps.Settings;

/// <summary>
/// Represents a float value setting.
/// </summary>
public sealed class FloatSetting : MapSetting
{
    /// <summary>
    /// Gets the float value from the setting.
    /// </summary>
    public new float Value => (float)base.Value;

    internal FloatSetting(float value) : base(value) { }
}
