namespace Box.Loaders.Maps.Settings;

/// <summary>
/// Represents an array of entity references setting.
/// </summary>
public class EntityRefArraySetting : MapSetting
{
    /// <summary>
    /// Gets the value of the entity reference array setting.
    /// </summary>
    public new List<MapEntityRef> Value => new((List<MapEntityRef>)base.Value);

    internal EntityRefArraySetting(List<MapEntityRef> value) : base(value) { }
}
