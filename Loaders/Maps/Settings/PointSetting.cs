namespace Box.Loaders.Maps.Settings;

/// <summary>
/// Represents a single 2D vector (Vect2) setting.
/// </summary>
public sealed class PointSetting : MapSetting
{
    /// <summary>
    /// Gets the 2D vector (Vect2) value from the setting.
    /// </summary>
    public new Vect2 Value => new((Vect2)base.Value);

    internal PointSetting(Vect2 value) : base(value) { }
}
