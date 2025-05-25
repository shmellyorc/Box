namespace Box.Interfaces;

/// <summary>
/// Represents a setting related to a tilemap or tile entity.
/// </summary>
public interface IMapSetting
{
    /// <summary>
    /// Gets the value of the setting.
    /// </summary>
    object Value { get; }
}
