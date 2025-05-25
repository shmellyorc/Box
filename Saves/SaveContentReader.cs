namespace Box.Saves;

/// <summary>
/// Provides methods for reading serialized data from a binary file.
/// Inherits from BinaryReader for binary data handling.
/// </summary>
public sealed class SaveContentReader : BinaryReader
{
    /// <summary>
    /// Initializes a new instance of the SaveContentReader class with the specified stream.
    /// </summary>
    /// <param name="stream">The stream from which to read data.</param>
    public SaveContentReader(Stream stream) : base(stream) { }

    /// <summary>
    /// Reads a Vect2 (2D vector) from the underlying stream.
    /// </summary>
    /// <returns>The Vect2 read from the stream.</returns>
    public Vect2 ReadVect2()
    {
        float x = ReadSingle();
        float y = ReadSingle();

        return new Vect2(x, y);
    }

    /// <summary>
    /// Reads a Rect2 (2D rectangle) from the underlying stream.
    /// </summary>
    /// <returns>The Rect2 read from the stream.</returns>
    public Rect2 ReadRect2()
    {
        int x = ReadInt32();
        int y = ReadInt32();
        int w = ReadInt32();
        int h = ReadInt32();

        return new Rect2(x, y, w, h);
    }

    /// <summary>
    /// Reads a Color from the underlying stream.
    /// </summary>
    /// <returns>The Color read from the stream.</returns>
    public BoxColor ReadColor()
    {
        int r = ReadInt32();
        int g = ReadInt32();
        int b = ReadInt32();
        int a = ReadInt32();

        return new(r, g, b, a);
    }

    /// <summary>
    /// Reads an enumeration of type T from the underlying stream.
    /// </summary>
    /// <typeparam name="T">The type of enumeration to read.</typeparam>
    /// <returns>The enumeration value read from the stream.</returns>
    public T ReadEnum<T>() where T : Enum
    {
        byte value = ReadByte();

        return (T)Enum.ToObject(typeof(T), value);
    }

    /// <summary>
    /// Reads an object of type T from the underlying stream.
    /// </summary>
    /// <typeparam name="T">The type of object to read.</typeparam>
    /// <returns>The object of type T read from the stream.</returns>
    public T ReadObject<T>()
    {
        DataContractSerializer serializer = new(typeof(T));

        int length = ReadInt32();
        byte[] bytes = ReadBytes(length);

        using MemoryStream stream = new(bytes);
        using XmlDictionaryReader reader = XmlDictionaryReader.CreateBinaryReader(stream, XmlDictionaryReaderQuotas.Max);

        return (T)serializer.ReadObject(reader);
    }
}
