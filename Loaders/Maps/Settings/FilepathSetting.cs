namespace Box.Loaders.Maps.Settings;

/// <summary>
/// Represents a single file path setting.
/// </summary>
public sealed class FilepathSetting : MapSetting
{
    /// <summary>
    /// Gets the file path from the setting.
    /// </summary>
    public new string Value => (string)base.Value;

    internal FilepathSetting(string value) : base(value) { }
}
