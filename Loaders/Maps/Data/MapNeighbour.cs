namespace Box.Loaders.Maps.Data;

/// <summary>
/// Represents the neighboring connections of a map element.
/// </summary>
public readonly struct MapNeighbour
{
    /// <summary>
    /// Gets the identifier or name of the northern neighbor.
    /// </summary>
    public string North { get; }

    /// <summary>
    /// Gets the identifier or name of the eastern neighbor.
    /// </summary>
    public string East { get; }

    /// <summary>
    /// Gets the identifier or name of the southern neighbor.
    /// </summary>
    public string South { get; }

    /// <summary>
    /// Gets the identifier or name of the western neighbor.
    /// </summary>
    public string West { get; }

    /// <summary>
    /// Indicates whether the reference is empty.
    /// </summary>
    public readonly bool IsEmpty => North.IsEmpty() && East.IsEmpty() && South.IsEmpty() && West.IsEmpty();

    internal MapNeighbour(string north, string east, string south, string west)
    {
        North = north;
        East = east;
        South = south;
        West = west;
    }
}
