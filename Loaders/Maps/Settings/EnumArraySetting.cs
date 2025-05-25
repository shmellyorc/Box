namespace Box.Loaders.Maps.Settings;

/// <summary>
/// Represents an array of enum values setting.
/// </summary>
public class EnumArraySetting : MapSetting
{
    /// <summary>
    /// Gets the value of the enum array setting.
    /// </summary>
    public new List<string> Value => new((List<string>)base.Value);

    internal EnumArraySetting(List<string> value) : base(value) { }
}
