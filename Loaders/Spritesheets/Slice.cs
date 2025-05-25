namespace Box.Loaders.Spritesheets;

/// <summary>
/// Represents a slice with bounds, ninepatch information, and pivot point.
/// </summary>
public readonly struct Slice
{
    /// <summary>
    /// Gets the bounds of the slice.
    /// </summary>
    public Rect2 Bounds { get; }

    /// <summary>
    /// Gets the ninepatch information of the slice.
    /// </summary>
    public Rect2 Ninepatch { get; }

    /// <summary>
    /// Gets the pivot point of the slice.
    /// </summary>
    public Vect2 Pivot { get; }

    internal Slice(Rect2 bounds, Rect2 ninepatch, Vect2 pivot)
    {
        Bounds = bounds;
        Ninepatch = ninepatch;
        Pivot = pivot;
    }
}
