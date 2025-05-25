namespace Box.Loaders.Maps.Layers;

/// <summary>
/// Represents a layer containing entities within a map, implementing the IMapLayer interface.
/// </summary>
public struct MapEntityLayer : IMapLayer
{
    /// <summary>
    /// Gets the location of the entity.
    /// </summary>
    public Vect2 Location { get; }

    /// <summary>
    /// Gets the identifier of the entity.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Gets the name of the entity.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the pivot point of the entity.
    /// </summary>
    public Vect2 Pivot { get; }

    /// <summary>
    /// Gets the list of tags associated with the entity.
    /// </summary>
    public List<string> Tags { get; }

    /// <summary>
    /// Gets the settings associated with the element.
    /// </summary>
    public Dictionary<string, MapSetting> Settings { get; }

    /// <summary>
    /// Gets the position of the element.
    /// </summary>
    public Vect2 Position { get; }

    /// <summary>
    /// Gets the size of the element.
    /// </summary>
    public Vect2 Size { get; }

    /// <summary>
    /// Gets the grid information associated with the element.
    /// </summary>
    public Vect2 Grid { get; }

    /// <summary>
    /// Gets or sets the map layer associated with this entity layer.
    /// </summary>
    [JsonIgnore]
    public MapLayer Layer { get; internal set; }

    internal MapEntityLayer(Vect2 location, string id, string name, Vect2 pivot, List<string> tags, Dictionary<string, MapSetting> settings, Vect2 position, Vect2 size, Vect2 grid)
    {
        Location = location;
        Id = id;
        Name = name;
        Pivot = pivot;
        Tags = tags;
        Settings = settings;
        Position = position;
        Size = size;
        Grid = grid;

        Layer = default;
    }
}
