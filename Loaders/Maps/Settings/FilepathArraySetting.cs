namespace Box.Loaders.Maps.Settings;

/// <summary>
/// Represents an array of file path settings.
/// </summary>
public sealed class FilepathArraySetting : MapSetting
{
    /// <summary>
    /// Gets the list of file paths from the setting.
    /// </summary>
    public new List<string> Value => new((List<string>)base.Value);

    internal FilepathArraySetting(List<string> value) : base(value) { }
}
