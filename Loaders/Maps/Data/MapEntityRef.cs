namespace Box.Loaders.Maps.Data;

/// <summary>
/// Represents a reference to an entity within a map.
/// </summary>
public readonly struct MapEntityRef
{
    /// <summary>
    /// Gets the unique identifier of the entity.
    /// </summary>
    public string EntityId { get; }

    /// <summary>
    /// Gets the unique identifier of the layer containing the entity.
    /// </summary>
    public string LayerId { get; }

    /// <summary>
    /// Gets the unique identifier of the level containing the entity.
    /// </summary>
    public string LevelId { get; }

    /// <summary>
    /// Gets the unique identifier of the world or map containing the entity.
    /// </summary>
    public string WorldId { get; }

    /// <summary>
    /// Indicates whether the reference is empty.
    /// </summary>
    public readonly bool IsEmpty => EntityId.IsEmpty() && LayerId.IsEmpty() && LevelId.IsEmpty() && WorldId.IsEmpty();

    internal MapEntityRef(string entityId, string layerId, string levelId, string worldId)
    {
        EntityId = entityId;
        LayerId = layerId;
        LevelId = levelId;
        WorldId = worldId;
    }
}
