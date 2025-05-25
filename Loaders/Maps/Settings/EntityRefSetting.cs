namespace Box.Loaders.Maps.Settings;

/// <summary>
/// Represents a single entity reference setting.
/// </summary>
public class EntityRefSetting : MapSetting
{
    /// <summary>
    /// Gets the value of the entity reference setting.
    /// </summary>
    public new MapEntityRef Value => (MapEntityRef)base.Value;

    internal EntityRefSetting(MapEntityRef value) : base(value) { }
}
