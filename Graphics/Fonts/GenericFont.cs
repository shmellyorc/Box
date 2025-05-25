namespace Box.Graphics.Fonts;

/// <summary>
/// Represents a generic implementation of a font, providing methods for measuring, formatting, and handling text rendering.
/// </summary>
public sealed class GenericFont : BoxFont
{
    private readonly SFMLFont _font;
    private readonly float _spacing;
    private readonly int _size;
    private readonly float _lineSpacing;
    private readonly int _thickness;
    private readonly bool _smoothing;
    private readonly bool _bold;

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override SFMLTexture GetTexture()
    {
        if (Texture is null)
        {
            Texture = _font.GetTexture((uint)_size);

            Texture.Smooth = _smoothing;
        }

        return Texture;
    }

    internal GenericFont(string filename, byte[] bytes, int size, bool smoothing, bool bold, int thickness, float spacing, float lineSpacing)
    {
        _size = size;
        _smoothing = smoothing;
        _bold = bold;
        _thickness = thickness;
        _spacing = spacing;
        _lineSpacing = lineSpacing;
        _font = new SFMLFont(bytes);
        Filename = filename;

		Id = HashHelpers.Hash32($"{filename}{bytes.Length:X8}{(int)spacing:X8}{(int)lineSpacing:X8}");
    }

    /// <summary>
    /// Initializes the font, preparing it for use by loading necessary resources and setting internal states.
    /// </summary>
    public override void Initialize()
    {
        if (Initialized)
            return;

        Spacing = _spacing;
        LineSpacing = _lineSpacing;

        for (int i = 0; i < 127; i++)
            Glpyhs.Add((char)i, _font.GetGlyph((uint)i, (uint)_size, _bold, (uint)_thickness));

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
    public override float GetTextHeight() => _font.GetLineSpacing((uint)_size) + LineSpacing;
}
