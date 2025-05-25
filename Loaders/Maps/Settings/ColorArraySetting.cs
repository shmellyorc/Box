namespace Box.Loaders.Maps.Settings;

/// <summary>
/// Represents an array of Color settings.
/// </summary>
public sealed class ColorArraySetting : MapSetting
{
    /// <summary>
    /// Gets the value of the Color array setting.
    /// </summary>
    public new List<BoxColor> Value => new((List<BoxColor>)base.Value);

    internal ColorArraySetting(List<BoxColor> value) : base(value) { }
}
