namespace Box.Signals;

/// <summary>
/// An EventHandler passed to the subscriber when an event occurs.
/// </summary>
public readonly struct SignalHandle
{
    internal string Id { get; }

    /// <summary>
    /// The name of the signal that was passed.
    /// </summary>
    public readonly string Name { get; }

    /// <summary>
    /// The data associated with the signal.
    /// </summary>
    public readonly object[] Data { get; }

    /// <summary>
    /// Checks if the signal contains any data.
    /// </summary>
    public bool IsEmpty => Data.IsEmpty();

    internal SignalHandle(string name, string id, object[] data)
    {
        Name = name;
        Id = id;
        Data = data;
    }

    /// <summary>
    /// Retrieves data based on the specified type and index within the data array.
    /// </summary>
    /// <typeparam name="T">The type to attempt to cast to.</typeparam>
    /// <param name="index">The index of the data parameter.</param>
    /// <returns>The data of type T at the specified index, or null if the type is incorrect or the index is out of range.</returns>
    public T Get<T>(int index)
    {
        if (IsEmpty)
            return default;

        if (!Exists<T>(index))
            return default;

        return (T)Data[index];
    }

    /// <summary>
    /// Checks if there is valid data of the specified type at the given index.
    /// </summary>
    /// <typeparam name="T">The type to attempt to cast to.</typeparam>
    /// <param name="index">The index of the data parameter.</param>
    /// <returns>True if there is valid data of the specified type at the given index; otherwise, false.</returns>
    public bool Exists<T>(int index)
    {
        if (IsEmpty)
            return default;

        var data = Data.ElementAtOrDefault(index);

        return data is not null && data is T;
    }

    /// <summary>
    /// Attempts to retrieve data of a specified type at a given index.
    /// </summary>
    /// <typeparam name="T">The type to attempt to cast to.</typeparam>
    /// <param name="index">The index of the data parameter.</param>
    /// <param name="value">When successful, contains the retrieved data; otherwise, null.</param>
    /// <returns>True if data is successfully retrieved; otherwise, false.</returns>
    public bool TryGet<T>(int index, out T value)
    {
        value = Get<T>(index);

        return value is not null;
    }
}
