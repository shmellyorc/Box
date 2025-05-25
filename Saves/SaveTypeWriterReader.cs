namespace Box.Saves;

/// <summary>
/// Abstract base class for handling serialization and deserialization of objects of type T
/// to and from binary files, with optional compression.
/// </summary>
/// <typeparam name="T">The type of object to serialize and deserialize.</typeparam>
public abstract class SaveTypeWriterReader<T>
{
    /// <summary>
    /// Loads an object of type T from the specified file.
    /// Throws an exception if the file does not exist.
    /// </summary>
    /// <param name="filename">The name of the file to load.</param>
    /// <returns>The loaded object of type T.</returns>
    /// <exception cref="FileNotFoundException">Thrown when the specified file does not exist.</exception>
    public T Load(string filename)
    {
        if (!FileHelpers.SaveFileExists(filename))
            throw new FileNotFoundException($"Save file '{filename}' not found.");

        using FileStream file = File.OpenRead(FileHelpers.SaveFilePath(filename));
        using BinaryReader reader = new(file);

        bool isCompressed = reader.ReadBoolean();
        byte[] data = reader.ReadBytes((int)file.Length - 1);

        if (isCompressed)
        {
            using MemoryStream compressedStream = new(data);
            using DeflateStream decompressor = new(compressedStream, CompressionMode.Decompress);
            using MemoryStream decompressedStream = new();
            decompressor.CopyTo(decompressedStream);

            data = decompressedStream.ToArray();
        }

        using MemoryStream memoryStream = new(data);
        using SaveContentReader saveReader = new(memoryStream);

        return Load(saveReader);
    }

    /// <summary>
    /// Saves the specified object of type T to the given file.  
    /// The file will be compressed only if compression results in a smaller size; otherwise,it will be saved uncompressed.
    /// </summary>
    /// <param name="filename">The path to the file where the object will be saved.</param>
    /// <param name="data">The object of type T to save.</param>
    public void Save(string filename, T data)
    {
        using MemoryStream rawStream = new();
        using SaveContentWriter writer = new(rawStream);

        Save(writer, data);
        byte[] rawData = rawStream.ToArray();

        using MemoryStream compressedStream = new();
        using (DeflateStream comp = new(compressedStream, CompressionMode.Compress, true))
        {
            comp.Write(rawData, 0, rawData.Length);
        }

        byte[] compressedData = compressedStream.ToArray();
        bool useCompression = compressedData.Length < rawData.Length;

        using FileStream file = File.Open(FileHelpers.SaveFilePath(filename), FileMode.Create);
        using BinaryWriter fileWriter = new(file);

        fileWriter.Write(useCompression);
        fileWriter.Write(useCompression ? compressedData : rawData);
    }

    /// <summary>
    /// Saves the specified object of type T using a custom SaveContentWriter.
    /// Derived classes must implement this method to define how the object is serialized.
    /// </summary>
    /// <param name="writer">The SaveContentWriter used to write the object data.</param>
    /// <param name="value">The object of type T to serialize.</param>
    protected abstract void Save(SaveContentWriter writer, T value);

    /// <summary>
    /// Loads an object of type T using a custom SaveContentReader.
    /// Derived classes must implement this method to define how the object is deserialized.
    /// </summary>
    /// <param name="reader">The SaveContentReader used to read the object data.</param>
    /// <returns>The deserialized object of type T.</returns>
    protected abstract T Load(SaveContentReader reader);
}
