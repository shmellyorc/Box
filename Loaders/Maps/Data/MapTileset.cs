namespace Box.Loaders.Maps.Data;

/// <summary>
/// Represents an immutable data structure for defining a map tileset.
/// </summary>
public readonly struct MapTileset
{
    /// <summary>
    /// Represents the grid dimensions of the tileset.
    /// </summary>
    public Vect2 Grid { get; }

    /// <summary>
    /// Gets the name of the tileset.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the ID of the tileset.
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// Gets the list of tags associated with the tileset.
    /// </summary>
    public List<string> Tags { get; }

    /// <summary>
    /// Gets the padding between tiles in the tileset.
    /// </summary>
    public int Padding { get; }

    /// <summary>
    /// Gets the spacing between tiles in the tileset.
    /// </summary>
    public int Spacing { get; }

    /// <summary>
    /// Represents the dimensions of each tile in the tileset.
    /// </summary>
    public Vect2 Size { get; }

    /// <summary>
    /// Gets the size of the grid (number of tiles) in the tileset.
    /// </summary>
    public int GridSize { get; }

    /// <summary>
    /// Gets the filename of the tileset.
    /// </summary>
    public string Filename { get; }

    /// <summary>
    /// Indicates whether the tileset is considered empty based on its size and filename.
    /// </summary>
    public readonly bool IsEmpty => Size == Vect2.Zero || Filename.IsEmpty();

    internal MapTileset(Vect2 grid, string name, string filename, int id, List<string> tags, int padding, int spacing, Vect2 size, int gridSize)
    {
        Grid = grid;
        Name = name;
        Filename = filename;
        Id = id;
        Tags = tags;
        Padding = padding;
        Spacing = spacing;
        Size = size;
        GridSize = gridSize;
    }
}
