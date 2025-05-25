namespace Box.Loaders.Maps.Settings;

/// <summary>
/// Represents an array of 2D vectors (Vect2) setting.
/// </summary>
public sealed class PointArraySetting : MapSetting
{
    /// <summary>
    /// Gets the list of 2D vectors (Vect2) from the setting.
    /// </summary>
    public new List<Vect2> Value => new((List<Vect2>)base.Value);

    internal PointArraySetting(List<Vect2> value) : base(value) { }
}
