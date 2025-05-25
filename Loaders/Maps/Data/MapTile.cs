namespace Box.Loaders.Maps.Data;

/// <summary>
/// Represents a tile within a map, sourced from a tileset.
/// </summary>
public struct MapTile
{
    /// <summary>
    /// Gets the ID of the tile within its tileset.
    /// </summary>
    public readonly int TilesetId { get; }

    /// <summary>
    /// Gets or sets the file associated with the tileset (internal use).
    /// </summary>
    public string TilesetFile { get; internal set; }

    /// <summary>
    /// Gets the rectangular area within the tileset that defines this tile's image.
    /// </summary>
    public readonly Rect2 Source { get; }

    /// <summary>
    /// Indicates whether the reference is empty.
    /// </summary>
    public readonly bool IsEmpty => TilesetFile.IsEmpty() && Source.IsEmpty;

    internal MapTile(int tilesetId, string tilesetFile, Rect2 source)
    {
        TilesetId = tilesetId;
        TilesetFile = tilesetFile;
        Source = source;
    }
}
