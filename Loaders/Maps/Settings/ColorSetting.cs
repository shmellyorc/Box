namespace Box.Loaders.Maps.Settings;

/// <summary>
/// Represents a Color setting.
/// </summary>
public sealed class ColorSetting : MapSetting
{
    /// <summary>
    /// Gets the value of the Color setting.
    /// </summary>
    public new BoxColor Value => (BoxColor)base.Value;

    internal ColorSetting(BoxColor value) : base(value) { }
}
