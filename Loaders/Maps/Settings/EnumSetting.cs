namespace Box.Loaders.Maps.Settings;

/// <summary>
/// Represents a single enum value setting.
/// </summary>
public class EnumSetting : MapSetting
{
    /// <summary>
    /// Gets the value of the enum setting.
    /// </summary>
    public new string Value => (string)base.Value;

    internal EnumSetting(string value) : base(value) { }
}
