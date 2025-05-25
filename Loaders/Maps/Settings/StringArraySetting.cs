namespace Box.Loaders.Maps.Settings;

/// <summary>
/// Represents a setting containing an array of strings.
/// </summary>
public sealed class StringArraySetting : MapSetting
{
    /// <summary>
    /// Gets the array of strings value from the setting.
    /// </summary>
    public new List<string> Value => new((List<string>)base.Value);

    internal StringArraySetting(List<string> value) : base(value) { }
}
