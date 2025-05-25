namespace Box.Loaders.BitmapFonts;

internal struct Character
{
    public static readonly Character Empty = new();
    private int _channel;
    private char _char;
    private int _height;
    private int _texturePage;
    private int _width;
    private int _x;
    private int _xAdvance;
    private int _xOffset;
    private int _y;
    private int _yOffset;

    public Character(char character, int x, int y, int width, int height, int xOffset, int yOffset, int xAdvance, int texturePage, int channel)
    {
        _char = character;
        _x = x;
        _y = y;
        _width = width;
        _height = height;
        _xAdvance = xAdvance;
        _texturePage = texturePage;
        _channel = channel;
        _xOffset = xOffset;
        _yOffset = yOffset;
    }

    [Obsolete("This property will be removed in a future update to the library. Please use the X, Y, Width and Height properties instead.")]
    public Rect2 Bounds
    {
        get => new Rect2(_x, _y, _width, _height);
        set
        {
            _x = (int)value.X;
            _y = (int)value.Y;
            _width = (int)value.Width;
            _height = (int)value.Height;
        }
    }

    public int Channel
    {
        get => _channel;
        set => _channel = value;
    }

    public char Char
    {
        get => _char;
        set => _char = value;
    }

    public int Height
    {
        get => _height;
        set => _height = value;
    }

    public bool IsEmpty
    {
        get { return _width == 0 && _height == 0; }
    }

    [Obsolete("This property will be removed in a future update to the library. Please use the XOffset and YOffset properties instead.")]
    public Vect2 Offset
    {
        get => new Vect2(_xOffset, _yOffset);
        set
        {
            _xOffset = (int)value.X;
            _yOffset = (int)value.Y;
        }
    }

    public int TexturePage
    {
        get => _texturePage;
        set => _texturePage = value;
    }

    public int Width
    {
        get => _width;
        set => _width = value;
    }

    public int X
    {
        get => _x;
        set => _x = value;
    }

    public int XAdvance
    {
        get => _xAdvance;
        set => _xAdvance = value;
    }

    public int XOffset
    {
        get => _xOffset;
        set => _xOffset = value;
    }

    public int Y
    {
        get => _y;
        set => _y = value;
    }

    public int YOffset
    {
        get => _yOffset;
        set => _yOffset = value;
    }

    public override string ToString()
    {
        return _char.ToString();
    }
}
