namespace Box.Loaders.Maps.Data;

/// <summary>
/// Represents a layer within a map, containing elements and settings.
/// </summary>
public readonly struct MapLayer
{
    /// <summary>
    /// Gets the identifier of the map layer.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Gets the grid dimensions of the map layer.
    /// </summary>
    public Vect2 Grid { get; }

    /// <summary>
    /// Gets the grid size of the map layer.
    /// </summary>
    public int GridSize { get; }

    /// <summary>
    /// Gets the name of the map layer.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the file associated with the tileset of the map layer.
    /// </summary>
    public string TilesetFile { get; }

    /// <summary>
    /// Gets the offset of the map layer.
    /// </summary>
    public Vect2 OFfset { get; }

    /// <summary>
    /// Gets the tileset identifier of the map layer.
    /// </summary>
    public int Tileset { get; }

    /// <summary>
    /// Gets the type of the map layer.
    /// </summary>
    public MapLayerType Type { get; }

    /// <summary>
    /// Gets the instances contained within the map layer.
    /// </summary>
    public List<IMapLayer> Instances { get; }

    /// <summary>
    /// Converts instances within the map layer to a specified type.
    /// </summary>
    /// <typeparam name="T">The type to convert instances to.</typeparam>
    /// <returns>An enumerable of instances cast to the specified type.</returns>
    public IEnumerable<T> InstanceAs<T>() => Instances.Cast<T>();

    /// <summary>
    /// Indicates whether the reference is empty.
    /// </summary>
    public readonly bool IsEmpty => Name.IsEmpty() || GridSize == 0 || Grid == Vect2.Zero;

    internal MapLayer(string id, Vect2 grid, int gridSize, string name, Vect2 oFfset, int tileset, string tilesetFile, MapLayerType type, List<IMapLayer> instances)
    {
        Id = id;
        Grid = grid;
        GridSize = gridSize;
        Name = name;
        OFfset = oFfset;
        Tileset = tileset;
        TilesetFile = tilesetFile;
        Type = type;
        Instances = instances;
    }
}
