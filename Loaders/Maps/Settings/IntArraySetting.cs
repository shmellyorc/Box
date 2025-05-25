namespace Box.Loaders.Maps.Settings;

/// <summary>
/// Represents an array of integer values setting.
/// </summary>
public sealed class IntArraySetting : MapSetting
{
    /// <summary>
    /// Gets the integer array value from the setting.
    /// </summary>
    public new List<int> Value => new((List<int>)base.Value);

    internal IntArraySetting(List<int> value) : base(value) { }
}
