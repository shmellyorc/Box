namespace Box.Loaders.BitmapFonts;

internal class BitmapFont : IEnumerable<Character>
{
    public const int NoMaxWidth = -1;

    private int _alphaChannel;
    private int _baseHeight;
    private int _blueChannel;
    private bool _bold;
    private IDictionary<char, Character> _characters;
    private string _charset;
    private string _familyName;
    private int _fontSize;
    private int _greenChannel;
    private Character _invalid;
    private bool _italic;
    private IDictionary<Kerning, int> _kernings;
    private int _lineHeight;
    private int _outlineSize;
    private bool _packed;
    private Padding _padding;
    private Page[] _pages;
    private int _redChannel;
    private bool _smoothed;
    private Vect2 _spacing;
    private int _stretchedHeight;
    private int _superSampling;
    private Size _textureSize;
    private bool _unicode;

    public BitmapFont()
    {
        _invalid = Character.Empty;
        // _pages = new();
        _kernings = new Dictionary<Kerning, int>();
        _characters = new Dictionary<char, Character>();
    }

    public int AlphaChannel
    {
        get { return _alphaChannel; }
        set { _alphaChannel = value; }
    }

    public int BaseHeight
    {
        get { return _baseHeight; }
        set { _baseHeight = value; }
    }

    public int BlueChannel
    {
        get { return _blueChannel; }
        set { _blueChannel = value; }
    }

    public bool Bold
    {
        get { return _bold; }
        set { _bold = value; }
    }

    public IDictionary<char, Character> Characters
    {
        get { return _characters; }
        set { _characters = value; }
    }

    public string Charset
    {
        get { return _charset; }
        set { _charset = value; }
    }

    public string FamilyName
    {
        get { return _familyName; }
        set { _familyName = value; }
    }

    public int FontSize
    {
        get { return _fontSize; }
        set { _fontSize = value; }
    }

    public int GreenChannel
    {
        get { return _greenChannel; }
        set { _greenChannel = value; }
    }

    public Character InvalidChar
    {
        get { return _invalid; }
    }

    public bool Italic
    {
        get { return _italic; }
        set { _italic = value; }
    }

    public IDictionary<Kerning, int> Kernings
    {
        get { return _kernings; }
        set { _kernings = value; }
    }

    public int LineHeight
    {
        get { return _lineHeight; }
        set { _lineHeight = value; }
    }

    public int OutlineSize
    {
        get { return _outlineSize; }
        set { _outlineSize = value; }
    }

    public bool Packed
    {
        get { return _packed; }
        set { _packed = value; }
    }

    public Padding Padding
    {
        get { return _padding; }
        set { _padding = value; }
    }

    public Page[] Pages
    {
        get { return _pages; }
        set { _pages = value; }
    }

    public int RedChannel
    {
        get { return _redChannel; }
        set { _redChannel = value; }
    }

    public bool Smoothed
    {
        get { return _smoothed; }
        set { _smoothed = value; }
    }

    public Vect2 Spacing
    {
        get { return _spacing; }
        set { _spacing = value; }
    }

    public int StretchedHeight
    {
        get { return _stretchedHeight; }
        set { _stretchedHeight = value; }
    }

    public int SuperSampling
    {
        get { return _superSampling; }
        set { _superSampling = value; }
    }

    public Size TextureSize
    {
        get { return _textureSize; }
        set { _textureSize = value; }
    }

    public bool Unicode
    {
        get { return _unicode; }
        set { _unicode = value; }
    }

    public Character this[char character]
    {
        get { return _characters.TryGetValue(character, out Character value) ? value : _invalid; }
    }

    public IEnumerator<Character> GetEnumerator()
    {
        foreach (KeyValuePair<char, Character> pair in _characters)
        {
            yield return pair.Value;
        }
    }

    public int GetKerning(char previous, char current)
    {
        Kerning key;

        key = new Kerning(previous, current, 0);

        if (!_kernings.TryGetValue(key, out int result))
        {
            result = 0;
        }

        return result;
    }

    public virtual void Load(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        if (!stream.CanSeek)
        {
            throw new ArgumentException("Stream must be seekable in order to determine file format.", nameof(stream));
        }

        switch (BitmapFontLoader.GetFileFormat(stream))
        {
            case BitmapFontFormat.Binary:
                LoadBinary(stream);
                break;

            case BitmapFontFormat.Text:
                LoadText(stream);
                break;

            case BitmapFontFormat.Xml:
                LoadXml(stream);
                break;

            default:
                throw new InvalidDataException("Unknown file format.");
        }
    }

    public void Load(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            throw new ArgumentNullException(nameof(fileName));
        }

        if (!File.Exists(fileName))
        {
            throw new FileNotFoundException(string.Format("Cannot find file '{0}'.", fileName), fileName);
        }

        using (Stream stream = File.OpenRead(fileName))
        {
            Load(stream);
        }

        BitmapFontLoader.QualifyResourcePaths(this, Path.GetDirectoryName(fileName));
    }

    public void LoadBinary(Stream stream)
    {
        byte[] buffer;

        ArgumentNullException.ThrowIfNull(stream);

        buffer = new byte[1024];

        stream.Read(buffer, 0, 4);

        if (buffer[0] != 66 || buffer[1] != 77 || buffer[2] != 70)
            throw new InvalidDataException("Source steam does not contain BMFont data.");

        if (buffer[3] != 3)
            throw new InvalidDataException("Only BMFont version 3 format data is supported.");

        _invalid = Character.Empty;

        while (stream.Read(buffer, 0, 5) != 0)
        {
            byte blockType;
            int blockSize;

            blockType = buffer[0];

            blockSize = WordHelpers.MakeDWordLittleEndian(buffer, 1);

            if (blockSize > buffer.Length)
                buffer = new byte[blockSize];

            if (stream.Read(buffer, 0, blockSize) != blockSize)
                throw new InvalidDataException("Failed to read enough data to fill block.");

            switch (blockType)
            {
                case 1: // Block type 1: info
                    LoadInfoBlock(buffer);
                    break;

                case 2: // Block type 2: common
                    LoadCommonBlock(buffer);
                    break;

                case 3: // Block type 3: pages
                    LoadPagesBlock(buffer);
                    break;

                case 4: // Block type 4: chars
                    LoadCharactersBlock(buffer, blockSize);
                    break;

                case 5: // Block type 5: kerning pairs
                    LoadKerningsBlock(buffer, blockSize);
                    break;

                default: throw new InvalidDataException("Block type " + blockType + " is not a valid BMFont block");
            }
        }
    }

    public void LoadText(string text)
    {
        using StringReader reader = new(text);

        LoadText(reader);
    }

    public void LoadText(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        using TextReader reader = new StreamReader(stream);

        LoadText(reader);
    }

    public virtual void LoadText(TextReader reader)
    {
        string line;
        string[] parts;

        ArgumentNullException.ThrowIfNull(reader);

        IDictionary<int, Page> pageData = new SortedDictionary<int, Page>();
        IDictionary<Kerning, int> kerningDictionary = new Dictionary<Kerning, int>();
        IDictionary<char, Character> charDictionary = new Dictionary<char, Character>();
        parts = new string[13]; // the maximum number of fields on a single line;

        _invalid = Character.Empty;

        do
        {
            line = reader.ReadLine();

            if (line is not null)
            {
                BitmapFontLoader.Split(line, parts);

                if (parts.Length != 0)
                {
                    switch (parts[0])
                    {
                        case "info":
                            _familyName = BitmapFontLoader.GetNamedString(parts, "face", 1);
                            _fontSize = BitmapFontLoader.GetNamedInt(parts, "size", 2);
                            _bold = BitmapFontLoader.GetNamedBool(parts, "bold", 3);
                            _italic = BitmapFontLoader.GetNamedBool(parts, "italic", 4);
                            _charset = BitmapFontLoader.GetNamedString(parts, "charset", 5);
                            _unicode = BitmapFontLoader.GetNamedBool(parts, "unicode", 6);
                            _stretchedHeight = BitmapFontLoader.GetNamedInt(parts, "stretchH", 7);
                            _smoothed = BitmapFontLoader.GetNamedBool(parts, "smooth", 8);
                            _superSampling = BitmapFontLoader.GetNamedInt(parts, "aa", 9);
                            _padding = BitmapFontLoader.ParsePadding(BitmapFontLoader.GetNamedString(parts, "padding", 10));
                            _spacing = BitmapFontLoader.ParsePoint(BitmapFontLoader.GetNamedString(parts, "spacing", 11));
                            _outlineSize = BitmapFontLoader.GetNamedInt(parts, "outline", 12);
                            break;

                        case "common":
                            _lineHeight = BitmapFontLoader.GetNamedInt(parts, "lineHeight", 1);
                            _baseHeight = BitmapFontLoader.GetNamedInt(parts, "base", 2);
                            _textureSize = new Size(BitmapFontLoader.GetNamedInt(parts, "scaleW", 3),
                                                        BitmapFontLoader.GetNamedInt(parts, "scaleH", 4));
                            // TODO: 5 is pages, which we currently don't directly read
                            _packed = BitmapFontLoader.GetNamedBool(parts, "packed", 6);
                            _alphaChannel = BitmapFontLoader.GetNamedInt(parts, "alphaChnl", 7);
                            _redChannel = BitmapFontLoader.GetNamedInt(parts, "redChnl", 8);
                            _greenChannel = BitmapFontLoader.GetNamedInt(parts, "greenChnl", 9);
                            _blueChannel = BitmapFontLoader.GetNamedInt(parts, "blueChnl", 10);
                            break;

                        case "page":
                            int id;
                            string name;

                            id = BitmapFontLoader.GetNamedInt(parts, "id", 1);
                            name = BitmapFontLoader.GetNamedString(parts, "file", 2);

                            pageData.Add(id, new Page(id, name));
                            break;

                        case "char":
                            Character charData;
                            int index;

                            index = BitmapFontLoader.GetNamedInt(parts, "id", 1);

                            charData = new Character
                            (
                              index >= 0 ? (char)index : '\0',
                              BitmapFontLoader.GetNamedInt(parts, "x", 2),
                              BitmapFontLoader.GetNamedInt(parts, "y", 3),
                              BitmapFontLoader.GetNamedInt(parts, "width", 4),
                              BitmapFontLoader.GetNamedInt(parts, "height", 5),
                              BitmapFontLoader.GetNamedInt(parts, "xoffset", 6),
                              BitmapFontLoader.GetNamedInt(parts, "yoffset", 7),
                              BitmapFontLoader.GetNamedInt(parts, "xadvance", 8),
                              BitmapFontLoader.GetNamedInt(parts, "page", 9),
                              BitmapFontLoader.GetNamedInt(parts, "chnl", 10)
                            );

                            if (index == -1)
                            {
                                _invalid = charData;
                            }

                            charDictionary.Add(charData.Char, charData);
                            break;

                        case "kerning":
                            Kerning key;

                            key = new Kerning((char)BitmapFontLoader.GetNamedInt(parts, "first", 1),
                                              (char)BitmapFontLoader.GetNamedInt(parts, "second", 2),
                                              BitmapFontLoader.GetNamedInt(parts, "amount", 3));

                            if (!kerningDictionary.ContainsKey(key))
                            {
                                kerningDictionary.Add(key, key.Amount);
                            }
                            break;
                    }
                }
            }
        } while (line is not null);

        _pages = BitmapFontLoader.ToArray(pageData.Values);
        _characters = charDictionary;
        _kernings = kerningDictionary;
    }

    public void LoadXml(string xml)
    {
        using StringReader reader = new(xml);

        LoadXml(reader);
    }

    public virtual void LoadXml(TextReader reader)
    {
        XmlDocument document;
        IDictionary<int, Page> pageData;
        IDictionary<Kerning, int> kerningDictionary;
        IDictionary<char, Character> charDictionary;
        XmlNode root;
        XmlNode properties;

        ArgumentNullException.ThrowIfNull(reader);

        document = new XmlDocument();
        pageData = new SortedDictionary<int, Page>();
        kerningDictionary = new Dictionary<Kerning, int>();
        charDictionary = new Dictionary<char, Character>();

        document.Load(reader);
        root = document.DocumentElement;

        _invalid = Character.Empty;

        // load the basic attributes
        properties = root.SelectSingleNode("info");
        _familyName = properties.Attributes["face"].Value;
        _fontSize = Convert.ToInt32(properties.Attributes["size"].Value, CultureInfo.InvariantCulture);
        _bold = Convert.ToInt32(properties.Attributes["bold"].Value, CultureInfo.InvariantCulture) != 0;
        _italic = Convert.ToInt32(properties.Attributes["italic"].Value, CultureInfo.InvariantCulture) != 0;
        _unicode = Convert.ToInt32(properties.Attributes["unicode"].Value, CultureInfo.InvariantCulture) != 0;
        _stretchedHeight = Convert.ToInt32(properties.Attributes["stretchH"].Value, CultureInfo.InvariantCulture);
        _charset = properties.Attributes["charset"].Value;
        _smoothed = Convert.ToInt32(properties.Attributes["smooth"].Value, CultureInfo.InvariantCulture) != 0;
        _superSampling = Convert.ToInt32(properties.Attributes["aa"].Value, CultureInfo.InvariantCulture);
        _padding = BitmapFontLoader.ParsePadding(properties.Attributes["padding"].Value);
        _spacing = BitmapFontLoader.ParsePoint(properties.Attributes["spacing"].Value);
        _outlineSize = Convert.ToInt32(properties.Attributes["outline"].Value, CultureInfo.InvariantCulture);

        // common attributes
        properties = root.SelectSingleNode("common");
        _baseHeight = Convert.ToInt32(properties.Attributes["base"].Value, CultureInfo.InvariantCulture);
        _lineHeight = Convert.ToInt32(properties.Attributes["lineHeight"].Value, CultureInfo.InvariantCulture);
        _textureSize = new Size(Convert.ToInt32(properties.Attributes["scaleW"].Value, CultureInfo.InvariantCulture),
                                    Convert.ToInt32(properties.Attributes["scaleH"].Value, CultureInfo.InvariantCulture));
        _packed = Convert.ToInt32(properties.Attributes["packed"].Value, CultureInfo.InvariantCulture) != 0;
        _alphaChannel = Convert.ToInt32(properties.Attributes["alphaChnl"].Value, CultureInfo.InvariantCulture);
        _redChannel = Convert.ToInt32(properties.Attributes["redChnl"].Value, CultureInfo.InvariantCulture);
        _greenChannel = Convert.ToInt32(properties.Attributes["greenChnl"].Value, CultureInfo.InvariantCulture);
        _blueChannel = Convert.ToInt32(properties.Attributes["blueChnl"].Value, CultureInfo.InvariantCulture);

        // load texture information
        foreach (XmlNode node in root.SelectNodes("pages/page"))
        {
            Page page = new(Convert.ToInt32(node.Attributes["id"].Value, CultureInfo.InvariantCulture), node.Attributes["file"].Value);

            pageData.Add(page.Id, page);
        }

        _pages = BitmapFontLoader.ToArray(pageData.Values);

        // load character information
        foreach (XmlNode node in root.SelectNodes("chars/char"))
        {
            Character character;
            int index = Convert.ToInt32(node.Attributes["id"].Value, CultureInfo.InvariantCulture);

            character = new Character(
                index >= 0 ? (char)index : '\0',
                Convert.ToInt32(node.Attributes["x"].Value, CultureInfo.InvariantCulture),
                Convert.ToInt32(node.Attributes["y"].Value, CultureInfo.InvariantCulture),
                Convert.ToInt32(node.Attributes["width"].Value, CultureInfo.InvariantCulture),
                Convert.ToInt32(node.Attributes["height"].Value, CultureInfo.InvariantCulture),
                Convert.ToInt32(node.Attributes["xoffset"].Value, CultureInfo.InvariantCulture),
                Convert.ToInt32(node.Attributes["yoffset"].Value, CultureInfo.InvariantCulture),
                Convert.ToInt32(node.Attributes["xadvance"].Value, CultureInfo.InvariantCulture),
                Convert.ToInt32(node.Attributes["page"].Value, CultureInfo.InvariantCulture),
                Convert.ToInt32(node.Attributes["chnl"].Value, CultureInfo.InvariantCulture)
            );

            if (index == -1)
                _invalid = character;

            charDictionary.Add(character.Char, character);
        }

        _characters = charDictionary;

        // loading kerning information
        foreach (XmlNode node in root.SelectNodes("kernings/kerning"))
        {
            Kerning key = new((char)Convert.ToInt32(node.Attributes["first"].Value, CultureInfo.InvariantCulture),
                              (char)Convert.ToInt32(node.Attributes["second"].Value, CultureInfo.InvariantCulture),
                              Convert.ToInt32(node.Attributes["amount"].Value, CultureInfo.InvariantCulture));

            if (!kerningDictionary.ContainsKey(key))
                kerningDictionary.Add(key, key.Amount);
        }

        _kernings = kerningDictionary;
    }

    public void LoadXml(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        using TextReader reader = new StreamReader(stream);

        LoadXml(reader);
    }

    public Size MeasureFont(string text)
    {
        return MeasureFont(text, NoMaxWidth);
    }

    public Size MeasureFont(string text, double maxWidth)
    {
        Size result;

        if (!string.IsNullOrEmpty(text))
        {
            char previousCharacter;
            int currentLineWidth;
            int currentLineHeight;
            int blockWidth;
            int blockHeight;
            int length;
            List<int> lineHeights;

            length = text.Length;
            previousCharacter = ' ';
            currentLineWidth = 0;
            currentLineHeight = _lineHeight;
            blockWidth = 0;
            blockHeight = 0;
            lineHeights = new();

            for (int i = 0; i < length; i++)
            {
                char character;

                character = text[i];

                if (character == '\n' || character == '\r')
                {
                    if (character == '\n' || i + 1 == length || text[i + 1] != '\n')
                    {
                        lineHeights.Add(currentLineHeight);
                        blockWidth = Math.Max(blockWidth, currentLineWidth);
                        currentLineWidth = 0;
                        currentLineHeight = _lineHeight;
                    }
                }
                else
                {
                    Character data;
                    int width;

                    data = this[character];
                    width = data.XAdvance + GetKerning(previousCharacter, character);

                    if (maxWidth != NoMaxWidth && currentLineWidth + width >= maxWidth)
                    {
                        lineHeights.Add(currentLineHeight);
                        blockWidth = Math.Max(blockWidth, currentLineWidth);
                        currentLineWidth = 0;
                        currentLineHeight = _lineHeight;
                    }

                    currentLineWidth += width;
                    currentLineHeight = Math.Max(currentLineHeight, data.Height + data.YOffset);
                    previousCharacter = character;
                }
            }

            // finish off the current line if required
            if (currentLineHeight != 0)
            {
                lineHeights.Add(currentLineHeight);
            }

            // reduce any lines other than the last back to the base
            for (int i = 0; i < lineHeights.Count - 1; i++)
            {
                lineHeights[i] = _lineHeight;
            }

            // calculate the final block height
            foreach (int lineHeight in lineHeights)
            {
                blockHeight += lineHeight;
            }

            result = new Size(Math.Max(currentLineWidth, blockWidth), blockHeight);
        }
        else
        {
            result = Size.Empty;
        }

        return result;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private string GetString(byte[] buffer, int index)
    {
        StringBuilder sb;

        sb = new StringBuilder();

        for (int i = index; i < buffer.Length; i++)
        {
            byte chr;

            chr = buffer[i];

            if (chr == 0)
                break;

            sb.Append((char)chr);
        }

        return sb.ToString();
    }

    private void LoadCharactersBlock(byte[] buffer, int blockSize)
    {
        int charCount;
        charCount = blockSize / 20; // The number of characters in the file can be computed by taking the size of the block and dividing with the size of the charInfo structure, i.e.: numChars = charsBlock.blockSize/20.
        IDictionary<char, Character> characters = new Dictionary<char, Character>(charCount);

        for (int i = 0; i < charCount; i++)
        {
            int start = i * 20;
            int index = WordHelpers.MakeDWordLittleEndian(buffer, start);
            Character chr;

            chr = new Character
            (
                index >= 0 ? (char)index : '\0',
                WordHelpers.MakeWordLittleEndian(buffer, start + 4),
                WordHelpers.MakeWordLittleEndian(buffer, start + 6),
                WordHelpers.MakeWordLittleEndian(buffer, start + 8),
                WordHelpers.MakeWordLittleEndian(buffer, start + 10),
                WordHelpers.MakeWordLittleEndian(buffer, start + 12),
                WordHelpers.MakeWordLittleEndian(buffer, start + 14),
                WordHelpers.MakeWordLittleEndian(buffer, start + 16),
                buffer[start + 18],
                buffer[start + 19]
            );

            if (index == -1)
            {
                _invalid = chr;
            }

            characters.Add(chr.Char, chr);
        }

        _characters = characters;
    }

    private void LoadCommonBlock(byte[] buffer)
    {
        _lineHeight = WordHelpers.MakeWordLittleEndian(buffer, 0);
        _baseHeight = WordHelpers.MakeWordLittleEndian(buffer, 2);
        _textureSize = new Size(WordHelpers.MakeWordLittleEndian(buffer, 4), WordHelpers.MakeWordLittleEndian(buffer, 6));
        _pages = new Page[WordHelpers.MakeWordLittleEndian(buffer, 8)];
        _alphaChannel = buffer[11];
        _redChannel = buffer[12];
        _greenChannel = buffer[13];
        _blueChannel = buffer[14];
    }

    private void LoadInfoBlock(byte[] buffer)
    {
        byte bits;

        _fontSize = WordHelpers.MakeWordLittleEndian(buffer, 0);
        bits = buffer[2]; // 	bit 0: smooth, bit 1: unicode, bit 2: italic, bit 3: bold, bit 4: fixedHeigth, bits 5-7: reserved
        _smoothed = (bits & 1 << 7) != 0;
        _unicode = (bits & 1 << 6) != 0;
        _italic = (bits & 1 << 5) != 0;
        _bold = (bits & 1 << 4) != 0;
        _charset = string.Empty; // TODO: buffer[3]
        _stretchedHeight = WordHelpers.MakeWordLittleEndian(buffer, 4);
        _superSampling = WordHelpers.MakeWordLittleEndian(buffer, 6);
        _padding = new Padding(buffer[10], buffer[7], buffer[8], buffer[9]);
        _spacing = new Vect2(buffer[11], buffer[12]);
        _outlineSize = buffer[13];
        _familyName = GetString(buffer, 14);
    }

    private void LoadKerningsBlock(byte[] buffer, int blockSize)
    {
        int pairCount;
        Dictionary<Kerning, int> kernings;

        pairCount = blockSize / 10;
        kernings = new Dictionary<Kerning, int>(pairCount);

        for (int i = 0; i < pairCount; i++)
        {
            int start;
            Kerning kerning;

            start = i * 10;

            kerning = new Kerning
            (
                (char)WordHelpers.MakeDWordLittleEndian(buffer, start),
                (char)WordHelpers.MakeDWordLittleEndian(buffer, start + 4),
                WordHelpers.MakeWordLittleEndian(buffer, start + 8)
            );

            kernings.Add(kerning, kerning.Amount);
        }

        _kernings = kernings;
    }

    private void LoadPagesBlock(byte[] buffer)
    {
        int nextStringStart;

        nextStringStart = 0;

        for (int i = 0; i < _pages.Length; i++)
        {
            Page page;
            string name;

            page = _pages[i];

            name = GetString(buffer, nextStringStart);
            nextStringStart += name.Length + 1; // +1 for the null terminator

            page.Id = i;
            page.FileName = name;

            _pages[i] = page;
        }
    }

}
