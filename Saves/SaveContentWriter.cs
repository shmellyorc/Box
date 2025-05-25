namespace Box.Saves;

/// <summary>
/// Provides methods for writing serialized data to a binary file.
/// Inherits from BinaryWriter for binary data handling.
/// </summary>
public sealed class SaveContentWriter : BinaryWriter
{
    /// <summary>
    /// Initializes a new instance of the SaveContentWriter class with the specified stream.
    /// </summary>
    /// <param name="stream">The stream to which data will be written.</param>
    public SaveContentWriter(Stream stream) : base(stream) { }

    /// <summary>
    /// Writes a Vect2 (2D vector) to the underlying stream.
    /// </summary>
    /// <param name="value">The Vect2 to write.</param>
    public void Write(Vect2 value)
    {
        Write(value.X);
        Write(value.Y);
    }

    /// <summary>
    /// Writes a Rect2 (2D rectangle) to the underlying stream.
    /// </summary>
    /// <param name="value">The Rect2 to write.</param>
    public void Write(Rect2 value)
    {
        Write(value.X);
        Write(value.Y);
        Write(value.Width);
        Write(value.Height);
    }

    /// <summary>
    /// Writes a Color to the underlying stream.
    /// </summary>
    /// <param name="value">The Color to write.</param>
    public void Write(BoxColor value)
    {
        Write(value.Red);
        Write(value.Green);
        Write(value.Blue);
        Write(value.Alpha);
    }

    /// <summary>
    /// Writes an enumeration value to the underlying stream.
    /// </summary>
    /// <param name="value">The enumeration value to write.</param>
    public void Write(Enum value) => Write(Convert.ToByte(value));

    /// <summary>
    /// Writes an object of type object to the underlying stream.
    /// </summary>
    /// <param name="value">The object to write.</param>
    public void WriteObject(object value)
    {
        DataContractSerializer serializer = new(value.GetType());

        using MemoryStream stream = new();
        using XmlDictionaryWriter writer = XmlDictionaryWriter.CreateBinaryWriter(stream);

        serializer.WriteObject(writer, value);

        byte[] data = stream.ToArray();

        Write(data.Length);
        Write(data);
    }
}
