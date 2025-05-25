namespace Box.Loaders.Maps.Settings;

/// <summary>
/// Represents a setting containing an array of MapTile values.
/// </summary>
public sealed class TileArraySetting : MapSetting
{
    /// <summary>
    /// Gets the list of MapTile values from the setting.
    /// </summary>
    public new List<MapTile> Value => (List<MapTile>)base.Value;

    internal TileArraySetting(List<MapTile> value) : base(value) { }
}
