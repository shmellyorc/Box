namespace Box.Loaders.Maps.Settings;

/// <summary>
/// Represents a setting containing a single string value.
/// </summary>
public sealed class StringSetting : MapSetting
{
    /// <summary>
    /// Gets the string value from the setting.
    /// </summary>
    public new string Value => new((string)base.Value);

    internal StringSetting(string value) : base(value) { }
}
