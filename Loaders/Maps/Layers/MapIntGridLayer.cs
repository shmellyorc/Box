namespace Box.Loaders.Maps.Layers;

/// <summary>
/// Represents a grid-based layer of integers within a map.
/// </summary>
public struct MapIntGridLayer : IMapLayer
{
    /// <summary>
    /// Gets the value associated with the location.
    /// </summary>
    public int Value { get; }

    /// <summary>
    /// Gets the location associated with the value.
    /// </summary>
    public Vect2 Location { get; internal set; }

    /// <summary>
    /// Gets a value indicating whether the element is considered solid.
    /// </summary>
    [JsonIgnore]
    public readonly bool IsSolid => Value > 0;

    /// <summary>
    /// Gets a value indicating whether the element's value is zero.
    /// </summary>
    [JsonIgnore]
    public readonly bool IsZero => Value == 0;

    /// <summary>
    /// Gets the map layer associated with this element.
    /// </summary>
    [JsonIgnore]
    public MapLayer Layer { get; internal set; }

    internal MapIntGridLayer(int value, Vect2 location)
    {
        Value = value;
        Location = location;
        Layer = default;
    }
}
