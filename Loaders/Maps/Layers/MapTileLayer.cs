namespace Box.Loaders.Maps.Layers;

/// <summary>
/// Represents a tile-based layer within a map, implementing the IMapLayer interface.
/// </summary>
public struct MapTileLayer : IMapLayer
{
    /// <summary>
    /// Gets the surface effects associated with the tile element.
    /// </summary>
    public SurfaceEffects Effects { get; }

    /// <summary>
    /// Gets the location of the tile element.
    /// </summary>
    public Vect2 Location { get; }

    /// <summary>
    /// Gets the position of the tile element.
    /// </summary>
    public Vect2 Position { get; }

    /// <summary>
    /// Gets the source rectangle defining the tile's region within a larger image.
    /// </summary>
    public Rect2 Source { get; }

    /// <summary>
    /// Gets the index of the tile element.
    /// </summary>
    public int Index { get; }

    /// <summary>
    /// Gets or sets the map layer associated with this tile element.
    /// </summary>
    [JsonIgnore]
    public MapLayer Layer { get; internal set; }

    internal MapTileLayer(SurfaceEffects effects, Vect2 position, Rect2 source, int index, Vect2 location)
    {
        Effects = effects;
        Position = position;
        Source = source;
        Index = index;
        Location = location;

        Layer = default;
    }
}


