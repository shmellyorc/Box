namespace Box.Loaders.BitmapFonts;

internal struct Page
{
    private string _fileName;
    private int _id;
    private byte[] _bytes;

    public Page(int id, string fileName)
      : this()
    {
        _fileName = fileName;
        _id = id;
    }

    public string FileName
    {
        get => _fileName;
        set => _fileName = value;
    }

    public byte[] Bytes
    {
        get => _bytes;
        set => _bytes = value;
    }

    public int Id
    {
        get => _id;
        set => _id = value;
    }

    public override string ToString()
    {
        return string.Format("{0} ({1})", _id, Path.GetFileName(_fileName));
    }
}
