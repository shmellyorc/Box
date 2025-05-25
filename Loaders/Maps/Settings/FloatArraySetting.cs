namespace Box.Loaders.Maps.Settings;

/// <summary>
/// Represents an array of float values setting.
/// </summary>
public sealed class FloatArraySetting : MapSetting
{
    /// <summary>
    /// Gets the list of float values from the setting.
    /// </summary>
    public new List<float> Value => new((List<float>)base.Value);

    internal FloatArraySetting(List<float> value) : base(value) { }
}
