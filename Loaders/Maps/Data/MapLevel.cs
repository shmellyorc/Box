namespace Box.Loaders.Maps.Data;

/// <summary>
/// Represents a level or layer within a map structure.
/// </summary>
public readonly struct MapLevel
{
    /// <summary>
    /// Gets the color associated with the map level.
    /// </summary>
    public readonly BoxColor Color { get; }

    /// <summary>
    /// Gets the unique identifier of the map level.
    /// </summary>
    public readonly string Id { get; }

    /// <summary>
    /// Gets the name of the map level.
    /// </summary>
    public readonly string Name { get; }

    /// <summary>
    /// Gets the size dimensions of the map level.
    /// </summary>
    public readonly Vect2 Size { get; }

    /// <summary>
    /// Gets the grid size dimensions of the map level.
    /// </summary>
    public readonly Vect2 GridSize { get; }

    /// <summary>
    /// Gets the depth or z-index of the map level.
    /// </summary>
    public readonly int Depth { get; }

    /// <summary>
    /// Gets the coordinates of the map level.
    /// </summary>
    public readonly Vect2 Coords { get; }

    /// <summary>
    /// Gets the settings associated with the map level.
    /// </summary>
    public readonly Dictionary<string, MapSetting> Settings { get; }

    /// <summary>
    /// Gets the layers comprising the map level.
    /// </summary>
    public readonly List<MapLayer> Layers { get; }

    /// <summary>
    /// Gets the neighboring connections of the map level.
    /// </summary>
    public readonly MapNeighbour Neighbours { get; }

    /// <summary>
    /// Indicates whether the map level is considered empty based on its properties.
    /// </summary>
    public readonly bool IsEmpty => Id.IsEmpty() || Size == Vect2.Zero || GridSize == Vect2.Zero;

    internal MapLevel(BoxColor color, string id, string name, Vect2 size, Vect2 gridSize, int depth, Vect2 coords, Dictionary<string, MapSetting> settings, List<MapLayer> layers, MapNeighbour neighbours)
    {
        Color = color;
        Id = id;
        Name = name;
        Size = size;
        GridSize = gridSize;
        Depth = depth;
        Coords = coords;
        Settings = settings;
        Layers = layers;
        Neighbours = neighbours;
    }
}
