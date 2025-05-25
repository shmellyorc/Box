namespace Box.Loaders.Maps.Settings;

/// <summary>
/// Represents an integer value setting.
/// </summary>
public sealed class IntSetting : MapSetting
{
    /// <summary>
    /// Gets the integer value from the setting.
    /// </summary>
    public new int Value => (int)base.Value;

    internal IntSetting(int value) : base(value) { }
}
