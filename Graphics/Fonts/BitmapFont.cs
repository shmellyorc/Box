namespace Box.Graphics.Fonts;

/// <summary>
/// Represents a bitmap font, which is a font rendered from a texture atlas containing individual characters.
/// </summary>
public sealed class BitmapFont : BoxFont
{
    private readonly BmFont _font;
    private readonly float _spacing;
    private readonly float _lineSpacing;

    /// <summary>
    /// Retrieves the texture used by this bitmap font for rendering characters.
    /// </summary>
    /// <returns>The texture used for rendering characters of the bitmap font.</returns>
    public override SFMLTexture GetTexture()
    {
        if (Texture is null)
        {
            byte[] bytes;

            if (_font.Pages[0].Bytes is not null && _font.Pages[0].Bytes.Length > 0)
                bytes = _font.Pages[0].Bytes;
            else
                bytes = File.ReadAllBytes(_font.Pages[0].FileName);

            Texture = new SFMLTexture(bytes)
            {
                Smooth = false
            };
        }

        return Texture;
    }

    internal BitmapFont(string filename, float spacing, float lineSpacing)
    {
        _spacing = spacing;
        _lineSpacing = lineSpacing;

        _font = BmFontLoader.LoadFontFromFile(filename);

        Filename = filename;
		Id = HashHelpers.Hash32($"{filename}{(int)spacing:X8}{(int)lineSpacing:X8}");
	}
    internal BitmapFont(string filename, byte[] bytes, byte[] assetBytes, float spacing, float lineSpacing)
    {
        _spacing = spacing;
        _lineSpacing = lineSpacing;

        var path = Path.GetTempFileName();

        File.WriteAllText(path, Encoding.UTF8.GetString(bytes));

        _font = BmFontLoader.LoadFontFromFile(path);
        _font.Pages[0].Bytes = assetBytes;

        Filename = filename;
    }

    /// <summary>
    /// Initializes the bitmap font by loading its associated texture and configuring its internal settings.
    /// </summary>
    public override void Initialize()
    {
        if (Initialized)
            return;

        Spacing = _spacing;
        LineSpacing = _lineSpacing;

        for (int i = 0; i < 127; i++)
        {
            var charValue = (char)i;

            if (!_font.Characters.ContainsKey(charValue))
                continue;

            var character = _font.Characters[charValue];

            Glpyhs.Add((char)i, new SFMLGlyph()
            {
                Advance = character.XAdvance,
                Bounds = new SFMLRectF(character.XOffset, character.YOffset, character.Width, character.Height),
                TextureRect = new SFMLRectI(character.X, character.Y, character.Width, character.Height)
            });
        }

        Initialized = true;

        base.Initialize();
    }

    /// <summary>
    /// Calculates and returns the total height of a line of text, 
    /// including the font's line height and additional line spacing.
    /// </summary>
    /// <returns>
    /// The computed text height as a floating-point value.
    /// </returns>
    public override float GetTextHeight() => _font.LineHeight + LineSpacing;
}
