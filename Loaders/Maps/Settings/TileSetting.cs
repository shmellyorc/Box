namespace Box.Loaders.Maps.Settings;

/// <summary>
/// Represents a setting containing a single MapTile value.
/// </summary>
public sealed class TileSetting : MapSetting
{
    /// <summary>
    /// Gets the MapTile value from the setting.
    /// </summary>
    public new MapTile Value => (MapTile)base.Value;

    internal TileSetting(MapTile value) : base(value) { }
}
