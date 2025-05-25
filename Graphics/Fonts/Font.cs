namespace Box.Graphics.Fonts;

/// <summary>
/// Represents a font asset that implements the IAsset interface.
/// </summary>
public class Font : IAsset
{
	public uint Id { get; internal set; }

	/// <summary>
	/// Gets or sets the dictionary of glyphs associated with characters in the font.
	/// </summary>
	public Dictionary<char, SFMLGlyph> Glpyhs { get; protected set; } = new();

    /// <summary>
    /// Gets or sets the SFML texture associated with the font.
    /// </summary>
    public SFMLTexture Texture { get; internal set; }

    /// <summary>
    /// Gets a value indicating whether the font has been disposed.
    /// </summary>
    public bool IsDisposed { get; private set; }

    /// <summary>
    /// Gets or sets the filename of the font.
    /// </summary>
    public string Filename { get; internal set; }

    /// <summary>
    /// Gets or sets the line spacing of the font.
    /// </summary>
    public float LineSpacing { get; internal set; }

    /// <summary>
    /// Gets or sets the character spacing of the font.
    /// </summary>
    public float Spacing { get; internal set; }

    /// <summary>
    /// Gets or sets a value indicating whether the font has been initialized.
    /// </summary>
    public bool Initialized { get; internal set; }

	

	/// <summary>
	/// Releases all resources used by the font.
	/// </summary>
	public void Dispose()
    {
        if (IsDisposed)
            return;

        Texture?.Dispose();

        IsDisposed = true;
    }

    /// <summary>
    /// Initializes the font.
    /// </summary>
    public virtual void Initialize() { }

    /// <summary>
    /// Retrieves the SFML texture associated with the font.
    /// </summary>
    /// <returns>The SFML texture of the font.</returns>
    public virtual SFMLTexture GetTexture() { return default; }

    /// <summary>
    /// Retrieves the height of the text rendered using the font.
    /// </summary>
    /// <returns>The height of the text rendered by the font.</returns>
    public virtual float GetTextHeight() { return 0f; }


    #region Measure
    /// <summary>
    /// Measures the dimensions (width and height) of the given text.
    /// </summary>
    /// <param name="text">The text to measure.</param>
    /// <returns>A Vect2 representing the width and height of the text.</returns>
    public unsafe Vect2 Measure(string text)
    {
        if (text.IsEmpty())
            return Vect2.Zero;

        float lineSpacing = GetTextHeight();
        float advX = 0f, advY = lineSpacing;//this is GenericFont ? lineSpacing : 0;
        // float advX = 0f, advY = this is GenericFont ? lineSpacing : 0;

        fixed (char* ptr = text.ToCharArray())
        {
            for (int i = 0; i < text.Length; i++)
            {
                var c = ptr + i;

                if (*c == '\r')
                    continue;
                if (*c == '\n')
                {
                    advX = 0f;
                    advY += lineSpacing;
                    continue;
                }

                if (!Glpyhs.ContainsKey(*c))
                    continue;

                var table = Glpyhs[*c];

                advX += table.Advance + Spacing;
            }
        }

        return new Vect2(advX, advY);
    }

    /// <summary>
    /// Measures the width of the given text.
    /// </summary>
    /// <param name="text">The text to measure.</param>
    /// <returns>The width of the text.</returns>
    public float MeasureWidth(string text) => Measure(text).X;

    /// <summary>
    /// Measures the height of the given text.
    /// </summary>
    /// <param name="text">The text to measure.</param>
    /// <returns>The height of the text.</returns>
    public float MeasureHeight(string text) => Measure(text).Y;
    #endregion


    #region FormatText
    /// <summary>
    /// Formats the text to fit within the specified width and returns the formatted text.
    /// </summary>
    /// <param name="text">The text to format.</param>
    /// <param name="width">The width constraint.</param>
    /// <returns>The formatted text.</returns>
    public string FormatText(string text, float width)
        => FormatText(this, text, width);

    /// <summary>
    /// Formats the text to fit within the specified width and returns the formatted text.
    /// </summary>
    /// <param name="text">The text to format.</param>
    /// <param name="width">The width constraint.</param>
    /// <returns>The formatted text.</returns>
    public string FormatText(string text, int width)
        => FormatText(this, text, width);

    /// <summary>
    /// Formats the text using the specified font to fit within the specified width and returns the formatted text.
    /// </summary>
    /// <param name="font">The font used for formatting.</param>
    /// <param name="text">The text to format.</param>
    /// <param name="width">The width constraint.</param>
    /// <returns>The formatted text.</returns>
    public unsafe static string FormatText(Font font, string text, float width)
    {
        StringBuilder sb = new();
        string[] words = text.Split(' ');
        string current = string.Empty;

        fixed (string* ptr = words)
        {
            for (int i = 0; i < words.Length; i++)
            {
                string* word = ptr + i;

                if (*word == "\r" || *word == "\n" || *word == Environment.NewLine)
                {
                    sb.AppendLine(current.Trim());
                    current = string.Empty;

                    continue;
                }

                if (font.Measure((current + *word).Trim()).X > width)
                {
                    sb.AppendLine(current.Trim());
                    current = $"{*word} ";
                }
                else
                    current += $"{*word} ";
            }

            if (!current.IsEmpty())
            {
                if (font.Measure(current.Trim()).X > width)
                    sb.AppendLine(current.Trim());
                else
                    sb.Append(current.Trim());
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// Formats the text to fit within the specified width and measures its resulting dimensions.
    /// </summary>
    /// <param name="text">The text to format and measure.</param>
    /// <param name="width">The width constraint.</param>
    /// <returns>A Vect2 representing the width and height of the formatted text.</returns>
    public Vect2 FormatTextAndMeasure(string text, int width)
        => FormatTextAndMeasure(this, text, width);

    /// <summary>
    /// Formats the text using the specified font to fit within the specified width and measures its resulting dimensions.
    /// </summary>
    /// <param name="font">The font used for formatting.</param>
    /// <param name="text">The text to format and measure.</param>
    /// <param name="width">The width constraint.</param>
    /// <returns>A Vect2 representing the width and height of the formatted text.</returns>
    public static Vect2 FormatTextAndMeasure(Font font, string text, int width)
        => new(font.Measure(FormatText(font, text, width)));
    #endregion


    #region MaxLines
    /// <summary>
    /// Calculates the maximum number of lines required to display the text within the specified integer width.
    /// </summary>
    /// <param name="text">The text to measure.</param>
    /// <param name="width">The width constraint.</param>
    /// <returns>The maximum number of lines required.</returns>
    public int MaxLines(string text, int width)
        => MaxLines(this, text, width);

    /// <summary>
    /// Calculates the maximum number of lines required to display the text within the specified floating-point width.
    /// </summary>
    /// <param name="text">The text to measure.</param>
    /// <param name="width">The width constraint.</param>
    /// <returns>The maximum number of lines required.</returns>
    public int MaxLines(string text, float width)
        => MaxLines(this, text, (int)width);

    /// <summary>
    /// Calculates the maximum number of lines required to display the text using the specified font within the specified integer width.
    /// </summary>
    /// <param name="font">The font used for measurement.</param>
    /// <param name="text">The text to measure.</param>
    /// <param name="width">The width constraint.</param>
    /// <returns>The maximum number of lines required.</returns>
    public unsafe static int MaxLines(Font font, string text, int width)
    {
        string result = FormatText(font, text, width);
        int count = 0;

        fixed (char* ptr = result.ToCharArray())
        {
            for (int i = 0; i < result.Length; i++)
            {
                var c = ptr + i;

                if (*c == '\r')
                    continue;

                if (*c == '\n')
                {
                    count++;
                    continue;
                }
            }
        }

        return count;
    }
    #endregion
}
