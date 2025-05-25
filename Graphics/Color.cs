namespace Box.Graphics;

/// <summary>
/// Represents a color in the RGB (Red, Green, Blue) color space.
/// </summary>
public struct Color : IEquatable<BoxColor>
{
    private readonly StringBuilder _sb = new();
    private int _red = 255, _green = 255, _blue = 255, _alpha = 255;


    #region Properties
    /// <summary>
    /// Gets or sets the red component of the color.
    /// </summary>
    public int Red
    {
        readonly get => _red;
        set => _red = Math.Clamp(value, 0, 255);
    }

    /// <summary>
    /// Gets or sets the green component of the color.
    /// </summary>
    public int Green
    {
        readonly get => _green;
        set => _green = Math.Clamp(value, 0, 255);
    }

    /// <summary>
    /// Gets or sets the blue component of the color.
    /// </summary>
    public int Blue
    {
        readonly get => _blue;
        set => _blue = Math.Clamp(value, 0, 255);
    }

    /// <summary>
    /// Gets or sets the alpha (transparency) component of the color.
    /// </summary>
    public int Alpha
    {
        readonly get => _alpha;
        set => _alpha = Math.Clamp(value, 0, 255);
    }
    #endregion


    #region Constructors
    /// <summary>
    /// Initializes a new instance of the Color struct with the specified RGBA values.
    /// </summary>
    /// <param name="red">The red component of the color (0-255).</param>
    /// <param name="green">The green component of the color (0-255).</param>
    /// <param name="blue">The blue component of the color (0-255).</param>
    /// <param name="alpha">The alpha (transparency) component of the color (0-255).</param>
    public Color(int red, int green, int blue, int alpha)
    {
        _red = Math.Clamp(red, 0, 255);
        _green = Math.Clamp(green, 0, 255);
        _blue = Math.Clamp(blue, 0, 255);
        _alpha = Math.Clamp(alpha, 0, 255);
    }

    /// <summary>
    /// Initializes a new instance of the Color struct with the specified RGB values and default alpha (255).
    /// </summary>
    /// <param name="red">The red component of the color (0-255).</param>
    /// <param name="green">The green component of the color (0-255).</param>
    /// <param name="blue">The blue component of the color (0-255).</param>
    public Color(int red, int green, int blue) : this(red, green, blue, 255) { }

    /// <summary>
    /// Initializes a new instance of the Color struct with the specified RGBA values using floats (0.0f-1.0f).
    /// </summary>
    /// <param name="red">The red component of the color (0.0f-1.0f).</param>
    /// <param name="green">The green component of the color (0.0f-1.0f).</param>
    /// <param name="blue">The blue component of the color (0.0f-1.0f).</param>
    /// <param name="alpha">The alpha (transparency) component of the color (0.0f-1.0f).</param>
    public Color(float red, float green, float blue, float alpha) : this(
        (int)Math.Clamp(red * 255f, 0f, 255f),
        (int)Math.Clamp(green * 255f, 0f, 255f),
        (int)Math.Clamp(blue * 255f, 0f, 255f),
        (int)Math.Clamp(alpha * 255f, 0f, 255f)
    )
    { }

    /// <summary>
    /// Initializes a new instance of the Color struct with the specified RGB values using floats (0.0f-1.0f) and default alpha (1.0f).
    /// </summary>
    /// <param name="red">The red component of the color (0.0f-1.0f).</param>
    /// <param name="green">The green component of the color (0.0f-1.0f).</param>
    /// <param name="blue">The blue component of the color (0.0f-1.0f).</param>
    public Color(float red, float green, float blue) : this(red, green, blue, 1.0f) { }

    /// <summary>
    /// Initializes a new instance of the Color struct by copying another Color instance.
    /// </summary>
    /// <param name="color">The Color instance to copy.</param>
    public Color(BoxColor color) : this(color.Red, color.Green, color.Blue, color.Alpha) { }

    /// <summary>
    /// Initializes a new instance of the Color struct from a hexadecimal string representation (#RRGGBB or #AARRGGBB).
    /// </summary>
    /// <param name="hex">The hexadecimal string representing the color.</param>
    /// <exception cref="ArgumentNullException">Thrown when the hex string is null or empty.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the hex string is invalid or cannot be parsed.</exception>
    public Color(string hex)
    {
        if (hex.IsEmpty())
            throw new ArgumentNullException(nameof(hex), "Is null or empty");

        var value = hex.TrimStart('#');

        if (value.Length == 8)
        {
            _alpha = int.Parse(value.Substring(0, 2), NumberStyles.HexNumber);
            _red = int.Parse(value.Substring(2, 2), NumberStyles.HexNumber);
            _green = int.Parse(value.Substring(4, 2), NumberStyles.HexNumber);
            _blue = int.Parse(value.Substring(6, 2), NumberStyles.HexNumber);
        }
        else if (value.Length == 6)
        {
            _red = int.Parse(value.Substring(0, 2), NumberStyles.HexNumber);
            _green = int.Parse(value.Substring(2, 2), NumberStyles.HexNumber);
            _blue = int.Parse(value.Substring(4, 2), NumberStyles.HexNumber);
        }
        else if (value.Length == 3)
        {
            _red = int.Parse(value.Substring(0, 1), NumberStyles.HexNumber);
            _green = int.Parse(value.Substring(1, 1), NumberStyles.HexNumber);
            _blue = int.Parse(value.Substring(2, 1), NumberStyles.HexNumber);
        }
        else
            throw new InvalidOperationException("Unable to process hex color. Your hex color doesn't match an web hex color. Use with or without '#' as: #AARRGGBB, #RRGGBB, or #RGB");
    }
    #endregion


    #region Operators
    /// <summary>
    /// Adds two colors component-wise and returns the result.
    /// </summary>
    /// <param name="left">The first color to add.</param>
    /// <param name="right">The second color to add.</param>
    /// <returns>The sum of the two colors.</returns>
    public static BoxColor operator +(BoxColor left, BoxColor right) =>
        new(
            left.Red + right.Red,
            left.Green + right.Green,
            left.Blue + right.Blue,
            left.Alpha + right.Alpha
        );

    /// <summary>
    /// Subtracts one color from another component-wise and returns the result.
    /// </summary>
    /// <param name="left">The color to subtract from (minuend).</param>
    /// <param name="right">The color to subtract (subtrahend).</param>
    /// <returns>The result of subtracting the second color from the first.</returns>
    public static BoxColor operator -(BoxColor left, BoxColor right) =>
        new(
            left.Red - right.Red,
            left.Green - right.Green,
            left.Blue - right.Blue,
            left.Alpha - right.Alpha
        );

    /// <summary>
    /// Multiplies each component of the color by a scalar value and returns the result.
    /// </summary>
    /// <param name="left">The color to multiply.</param>
    /// <param name="right">The scalar value to multiply by.</param>
    /// <returns>The resulting color after multiplication.</returns>
    public static BoxColor operator *(BoxColor left, float right) =>
        new(
            (int)Math.Clamp(left.Red * Math.Clamp(right, 0f, 1f), 0, 255),
            (int)Math.Clamp(left.Green * Math.Clamp(right, 0f, 1f), 0, 255),
            (int)Math.Clamp(left.Blue * Math.Clamp(right, 0f, 1f), 0, 255),
            (int)Math.Clamp(left.Alpha * Math.Clamp(right, 0f, 1f), 0, 255)
        );

    /// <summary>
    /// Divides each component of the color by a scalar value and returns the result.
    /// </summary>
    /// <param name="left">The color to divide.</param>
    /// <param name="right">The scalar value to divide by (non-zero).</param>
    /// <returns>The resulting color after division.</returns>
    public static BoxColor operator /(BoxColor left, float right) =>
        new(
            (int)Math.Clamp(left.Red / Math.Clamp(right, 0f, 1f), 0, 255),
            (int)Math.Clamp(left.Green / Math.Clamp(right, 0f, 1f), 0, 255),
            (int)Math.Clamp(left.Blue / Math.Clamp(right, 0f, 1f), 0, 255),
            (int)Math.Clamp(left.Alpha / Math.Clamp(right, 0f, 1f), 0, 255)
        );

    /// <summary>
    /// Divides each component of the color by an integer value and returns the result.
    /// </summary>
    /// <param name="left">The color to divide.</param>
    /// <param name="right">The integer value to divide by (non-zero).</param>
    /// <returns>The resulting color after division.</returns>
    public static BoxColor operator /(BoxColor left, int right) =>
        new(
            left.Red / right,
            left.Green / right,
            left.Blue / right,
            left.Alpha / right
        );

    /// <summary>
    /// Checks if two colors are equal by comparing their RGBA components.
    /// </summary>
    /// <param name="left">The first color to compare.</param>
    /// <param name="right">The second color to compare.</param>
    /// <returns>True if the colors are equal; false otherwise.</returns>
    public static bool operator ==(BoxColor left, BoxColor right)
    {
        return (left.Red, left.Green, left.Blue, left.Alpha)
            == (right.Red, right.Green, right.Blue, right.Alpha);
    }

    /// <summary>
    /// Checks if two colors are not equal by comparing their RGBA components.
    /// </summary>
    /// <param name="left">The first color to compare.</param>
    /// <param name="right">The second color to compare.</param>
    /// <returns>True if the colors are not equal; false if they are equal.</returns>
    public static bool operator !=(BoxColor left, BoxColor right)
    {
        return !(left == right);
    }
    #endregion


    #region BlendColors
    /// <summary>
    /// Blends this color with another color using the specified blending ratio.
    /// </summary>
    /// <param name="other">The color to blend with.</param>
    /// <param name="ratio">The blending ratio. 0.0 means entirely this color, 1.0 means entirely the other color.</param>
    /// <returns>The resulting blended color.</returns>
    public readonly BoxColor BlendColors(BoxColor other, double ratio) => BlendColors(this, other, ratio);

    /// <summary>
    /// Blends two colors using the specified blending ratio.
    /// </summary>
    /// <param name="color1">The first color to blend.</param>
    /// <param name="color2">The second color to blend.</param>
    /// <param name="ratio">The blending ratio. 0.0 means entirely color1, 1.0 means entirely color2.</param>
    /// <returns>The resulting blended color.</returns>
    public static BoxColor BlendColors(BoxColor color1, BoxColor color2, double ratio)
    {
        int blendedRed = (int)Math.Round(color1.Red * (1 - ratio) + color2.Red * ratio);
        int blendedGreen = (int)Math.Round(color1.Green * (1 - ratio) + color2.Green * ratio);
        int blendedBlue = (int)Math.Round(color1.Blue * (1 - ratio) + color2.Blue * ratio);
        int blendedAlpha = (int)Math.Round(color1.Alpha * (1 - ratio) + color2.Alpha * ratio);

        return new(blendedRed, blendedGreen, blendedBlue, blendedAlpha);
    }
    #endregion


    #region Lighten
    /// <summary>
    /// Lightens this color by the specified amount.
    /// </summary>
    /// <param name="amount">The amount by which to lighten the color.</param>
    public readonly void Lighten(int amount) => Lighten(this, amount);

    /// <summary>
    /// Lightens the specified color by the specified amount.
    /// </summary>
    /// <param name="color">The color to lighten.</param>
    /// <param name="amount">The amount by which to lighten the color.</param>
    public static void Lighten(BoxColor color, int amount)
    {
        color.Red = Math.Min(255, color.Red + amount);
        color.Green = Math.Min(255, color.Green + amount);
        color.Blue = Math.Min(255, color.Blue + amount);
    }

    /// <summary>
    /// Lightens this color, including its alpha channel, by the specified amount.
    /// </summary>
    /// <param name="amount">The amount by which to lighten the color.</param>
    public readonly void LightenWithAlpha(int amount) => LightenWithAlpha(this, amount);

    /// <summary>
    /// Lightens the specified color, including its alpha channel, by the specified amount.
    /// </summary>
    /// <param name="color">The color to lighten.</param>
    /// <param name="amount">The amount by which to lighten the color.</param>
    public static void LightenWithAlpha(BoxColor color, int amount)
    {
        color.Red = Math.Min(255, color.Red + amount);
        color.Green = Math.Min(255, color.Green + amount);
        color.Blue = Math.Min(255, color.Blue + amount);
        color.Alpha = Math.Min(255, color.Alpha + amount);
    }
    #endregion


    #region Darken
    /// <summary>
    /// Darkens this color by the specified amount.
    /// </summary>
    /// <param name="amount">The amount by which to darken the color.</param>
    public readonly void Darken(int amount) => Darken(this, amount);

    /// <summary>
    /// Darkens the specified color by the specified amount.
    /// </summary>
    /// <param name="color">The color to darken.</param>
    /// <param name="amount">The amount by which to darken the color.</param>
    public static void Darken(BoxColor color, int amount)
    {
        color.Red = Math.Max(0, color.Red - amount);
        color.Green = Math.Max(0, color.Green - amount);
        color.Blue = Math.Max(0, color.Blue - amount);
    }

    /// <summary>
    /// Darkens this color, including its alpha channel, by the specified amount.
    /// </summary>
    /// <param name="amount">The amount by which to darken the color.</param>
    public readonly void DarkenWithAlpha(int amount) => DarkenWithAlpha(this, amount);

    /// <summary>
    /// Darkens the specified color, including its alpha channel, by the specified amount.
    /// </summary>
    /// <param name="color">The color to darken.</param>
    /// <param name="amount">The amount by which to darken the color.</param>
    public static void DarkenWithAlpha(BoxColor color, int amount)
    {
        color.Red = Math.Max(0, color.Red - amount);
        color.Green = Math.Max(0, color.Green - amount);
        color.Blue = Math.Max(0, color.Blue - amount);
        color.Alpha = Math.Max(0, color.Alpha - amount);
    }
    #endregion


    #region IsDark
    /// <summary>
    /// Determines if this color is considered dark, based on the specified brightness threshold.
    /// </summary>
    /// <param name="brightnessThreshold">The brightness threshold (0-255) below which the color is considered dark.</param>
    /// <returns>True if the color is dark, false otherwise.</returns>
    public readonly bool IsDark(int brightnessThreshold = 128) => IsDark(this, brightnessThreshold);

    /// <summary>
    /// Determines if the specified color is considered dark, based on the specified brightness threshold.
    /// </summary>
    /// <param name="color">The color to evaluate.</param>
    /// <param name="brightnessThreshold">The brightness threshold (0-255) below which the color is considered dark.</param>
    /// <returns>True if the color is dark, false otherwise.</returns>
    public static bool IsDark(BoxColor color, int brightnessThreshold = 128)
    {
        // Calculate perceived brightness using average of RGB values
        double brightness = (color.Red + color.Green + color.Blue) / 3.0;

        // Compare brightness to the threshold
        return brightness < brightnessThreshold;
    }
    #endregion


    #region IsLight
    /// <summary>
    /// Determines if this color is considered light, based on the specified brightness threshold.
    /// </summary>
    /// <param name="brightnessThreshold">The brightness threshold (0-255) above which the color is considered light.</param>
    /// <returns>True if the color is light, false otherwise.</returns>
    public readonly bool IsLight(int brightnessThreshold = 128) => IsLight(this, brightnessThreshold);

    /// <summary>
    /// Determines if the specified color is considered light, based on the specified brightness threshold.
    /// </summary>
    /// <param name="color">The color to evaluate.</param>
    /// <param name="brightnessThreshold">The brightness threshold (0-255) above which the color is considered light.</param>
    /// <returns>True if the color is light, false otherwise.</returns>
    public static bool IsLight(BoxColor color, int brightnessThreshold = 128)
    {
        // Calculate perceived brightness using average of RGB values
        double brightness = (color.Red + color.Green + color.Blue) / 3.0;

        // Compare brightness to the threshold
        return brightness >= brightnessThreshold;
    }
    #endregion


    #region IEquatable
    /// <summary>
    /// Determines whether this color is equal to another color.
    /// </summary>
    /// <param name="other">The color to compare with this color.</param>
    /// <returns>True if the colors are equal, false otherwise.</returns>
    public readonly bool Equals(BoxColor other)
    {
        return (_red, _green, _blue, _alpha) == (other._red, other._green, other._blue, other._alpha);
    }

    /// <summary>
    /// Determines whether this color is equal to another object.
    /// </summary>
    /// <param name="obj">The object to compare with this color.</param>
    /// <returns>True if the object is a Color and is equal to this color, false otherwise.</returns>
    public readonly override bool Equals([NotNullWhen(true)] object obj)
    {
        if (obj is BoxColor value)
            return Equals(value);

        return false;
    }

    /// <summary>
    /// Returns the hash code for this color.
    /// </summary>
    /// <returns>A 32-bit signed integer hash code.</returns>
    public readonly override int GetHashCode() => HashCode.Combine(_red, _green, _blue, _alpha);

    /// <summary>
    /// Returns a string representation of this color.
    /// </summary>
    /// <returns>A string that represents the current color.</returns>
    public override string ToString()
    {
        if (_sb.Length > 0)
            _sb.Clear();

        _sb.Append($"{_red}, ");
        _sb.Append($"{_green}, ");
        _sb.Append($"{_blue}, ");
        _sb.Append($"{_alpha}");

        return _sb.ToString();
    }
    #endregion


    #region Colors
    /// <summary>
    /// Represents a collection of shades of black.
    /// </summary>
    public readonly struct ShadesOfBlack
    {
        /// <summary>
        /// Represents the color black (#000000).
        /// </summary>
        public static BoxColor Black => new("#000000");

        /// <summary>
        /// Represents the color charcoal (#36454F).
        /// </summary>
        public static BoxColor Charcoal => new("#36454F");

        /// <summary>
        /// Represents the color dark green (#023020).
        /// </summary>
        public static BoxColor DarkGreen => new("#023020");

        /// <summary>
        /// Represents the color dark purple (#301934).
        /// </summary>
        public static BoxColor DarkPurple => new("#301934");

        /// <summary>
        /// Represents the color jet black (#343434).
        /// </summary>
        public static BoxColor JetBlack => new("#343434");

        /// <summary>
        /// Represents the color licorice (#1B1212).
        /// </summary>
        public static BoxColor Licorice => new("#1B1212");

        /// <summary>
        /// Represents the color matte black (#28282B).
        /// </summary>
        public static BoxColor MatteBlack => new("#28282B");

        /// <summary>
        /// Represents the color midnight blue (#191970).
        /// </summary>
        public static BoxColor MidnightBlue => new("#191970");

        /// <summary>
        /// Represents the color onyx (#353935).
        /// </summary>
        public static BoxColor Onyx => new("#353935");

    }


    /// <summary>
    /// Represents a collection of predefined shades of blue.
    /// </summary>
    public readonly struct ShadesOfBlue
    {
        /// <summary>
        /// Represents the color Aqua (#00FFFF).
        /// </summary>
        public static BoxColor Aqua => new("#00FFFF");

        /// <summary>
        /// Represents the color Azure (#F0FFFF).
        /// </summary>
        public static BoxColor Azure => new("#F0FFFF");

        /// <summary>
        /// Represents the color Baby Blue (#89CFF0).
        /// </summary>
        public static BoxColor BabyBlue => new("#89CFF0");

        /// <summary>
        /// Represents the color Blue (#0000FF).
        /// </summary>
        public static BoxColor Blue => new("#0000FF");

        /// <summary>
        /// Represents the color Blue Gray (#7393B3).
        /// </summary>
        public static BoxColor BlueGray => new("#7393B3");

        /// <summary>
        /// Represents the color Blue Green (#088F8F).
        /// </summary>
        public static BoxColor BlueGreen => new("#088F8F");

        /// <summary>
        /// Represents the color Bright Blue (#0096FF).
        /// </summary>
        public static BoxColor BrightBlue => new("#0096FF");

        /// <summary>
        /// Represents the color Cadet Blue (#5F9EA0).
        /// </summary>
        public static BoxColor CadetBlue => new("#5F9EA0");

        /// <summary>
        /// Represents the color Cobalt Blue (#0047AB).
        /// </summary>
        public static BoxColor CobaltBlue => new("#0047AB");

        /// <summary>
        /// Represents the color Cornflower Blue (#6495ED).
        /// </summary>
        public static BoxColor CornflowerBlue => new("#6495ED");

        /// <summary>
        /// Represents the color Cyan (#00FFFF).
        /// </summary>
        public static BoxColor Cyan => new("#00FFFF");

        /// <summary>
        /// Represents the color Dark Blue (#00008B).
        /// </summary>
        public static BoxColor DarkBlue => new("#00008B");

        /// <summary>
        /// Represents the color Denim (#6F8FAF).
        /// </summary>
        public static BoxColor Denim => new("#6F8FAF");

        /// <summary>
        /// Represents the color Egyptian Blue (#1434A4).
        /// </summary>
        public static BoxColor EgyptianBlue => new("#1434A4");

        /// <summary>
        /// Represents the color Electric Blue (#7DF9FF).
        /// </summary>
        public static BoxColor ElectricBlue => new("#7DF9FF");

        /// <summary>
        /// Represents the color Glaucous (#6082B6).
        /// </summary>
        public static BoxColor Glaucous => new("#6082B6");

        /// <summary>
        /// Represents the color Jade (#00A36C).
        /// </summary>
        public static BoxColor Jade => new("#00A36C");

        /// <summary>
        /// Represents the color Indigo (#3F00FF).
        /// </summary>
        public static BoxColor Indigo => new("#3F00FF");

        /// <summary>
        /// Represents the color Iris (#5D3FD3).
        /// </summary>
        public static BoxColor Iris => new("#5D3FD3");

        /// <summary>
        /// Represents the color Light Blue (#ADD8E6).
        /// </summary>
        public static BoxColor LightBlue => new("#ADD8E6");

        /// <summary>
        /// Represents the color Midnight Blue (#191970).
        /// </summary>
        public static BoxColor MidnightBlue => new("#191970");

        /// <summary>
        /// Represents the color Navy Blue (#000080).
        /// </summary>
        public static BoxColor NavyBlue => new("#000080");

        /// <summary>
        /// Represents the color Neon Blue (#1F51FF).
        /// </summary>
        public static BoxColor NeonBlue => new("#1F51FF");

        /// <summary>
        /// Represents the color Pastel Blue (#A7C7E7).
        /// </summary>
        public static BoxColor PastelBlue => new("#A7C7E7");

        /// <summary>
        /// Represents the color Periwinkle (#CCCCFF).
        /// </summary>
        public static BoxColor Periwinkle => new("#CCCCFF");

        /// <summary>
        /// Represents the color Powder Blue (#B6D0E2).
        /// </summary>
        public static BoxColor PowderBlue => new("#B6D0E2");

        /// <summary>
        /// Represents the color Robin Egg Blue (#96DED1).
        /// </summary>
        public static BoxColor RobinEggBlue => new("#96DED1");

        /// <summary>
        /// Represents the color Royal Blue (#4169E1).
        /// </summary>
        public static BoxColor RoyalBlue => new("#4169E1");

        /// <summary>
        /// Represents the color Sapphire Blue (#0F52BA).
        /// </summary>
        public static BoxColor SapphireBlue => new("#0F52BA");

        /// <summary>
        /// Represents the color Seafoam Green (#9FE2BF).
        /// </summary>
        public static BoxColor SeafoamGreen => new("#9FE2BF");

        /// <summary>
        /// Represents the color Sky Blue (#87CEEB).
        /// </summary>
        public static BoxColor SkyBlue => new("#87CEEB");

        /// <summary>
        /// Represents the color Steel Blue (#4682B4).
        /// </summary>
        public static BoxColor SteelBlue => new("#4682B4");

        /// <summary>
        /// Represents the color Teal (#008080).
        /// </summary>
        public static BoxColor Teal => new("#008080");

        /// <summary>
        /// Represents the color Turquoise (#40E0D0).
        /// </summary>
        public static BoxColor Turquoise => new("#40E0D0");

        /// <summary>
        /// Represents the color Ultramarine (#0437F2).
        /// </summary>
        public static BoxColor Ultramarine => new("#0437F2");

        /// <summary>
        /// Represents the color Verdigris (#40B5AD).
        /// </summary>
        public static BoxColor Verdigris => new("#40B5AD");

        /// <summary>
        /// Represents the color Zaffre (#0818A8).
        /// </summary>
        public static BoxColor Zaffre => new("#0818A8");
    }


    /// <summary>
    /// Represents a readonly struct for handling shades of brown colors.
    /// </summary>
    public readonly struct ShadesOfBrown
    {
        /// <summary>
        /// Represents the color Almond (#EADDCA).
        /// </summary>
        public static BoxColor Almond => new("#EADDCA");

        /// <summary>
        /// Represents the color Brass (#E1C16E).
        /// </summary>
        public static BoxColor Brass => new("#E1C16E");

        /// <summary>
        /// Represents the color Bronze (#CD7F32).
        /// </summary>
        public static BoxColor Bronze => new("#CD7F32");

        /// <summary>
        /// Represents the color Brown (#A52A2A).
        /// </summary>
        public static BoxColor Brown => new("#A52A2A");

        /// <summary>
        /// Represents the color Buff (#DAA06D).
        /// </summary>
        public static BoxColor Buff => new("#DAA06D");

        /// <summary>
        /// Represents the color Burgundy (#800020).
        /// </summary>
        public static BoxColor Burgundy => new("#800020");

        /// <summary>
        /// Represents the color Burnt Sienna (#E97451).
        /// </summary>
        public static BoxColor BurntSienna => new("#E97451");

        /// <summary>
        /// Represents the color Burnt Umber (#6E260E).
        /// </summary>
        public static BoxColor BurntUmber => new("#6E260E");

        /// <summary>
        /// Represents the color Camel (#C19A6B).
        /// </summary>
        public static BoxColor Camel => new("#C19A6B");

        /// <summary>
        /// Represents the color Chestnut (#954535).
        /// </summary>
        public static BoxColor Chestnut => new("#954535");

        /// <summary>
        /// Represents the color Chocolate (#7B3F00).
        /// </summary>
        public static BoxColor Chocolate => new("#7B3F00");

        /// <summary>
        /// Represents the color Cinnamon (#D27D2D).
        /// </summary>
        public static BoxColor Cinnamon => new("#D27D2D");

        /// <summary>
        /// Represents the color Coffee (#6F4E37).
        /// </summary>
        public static BoxColor Coffee => new("#6F4E37");

        /// <summary>
        /// Represents the color Cognac (#834333).
        /// </summary>
        public static BoxColor Cognac => new("#834333");

        /// <summary>
        /// Represents the color Copper (#B87333).
        /// </summary>
        public static BoxColor Copper => new("#B87333");

        /// <summary>
        /// Represents the color Cordovan (#814141).
        /// </summary>
        public static BoxColor Cordovan => new("#814141");

        /// <summary>
        /// Represents the color Dark Brown (#5C4033).
        /// </summary>
        public static BoxColor DarkBrown => new("#5C4033");

        /// <summary>
        /// Represents the color Dark Red (#8B0000).
        /// </summary>
        public static BoxColor DarkRed => new("#8B0000");

        /// <summary>
        /// Represents the color Dark Tan (#988558).
        /// </summary>
        public static BoxColor DarkTan => new("#988558");

        /// <summary>
        /// Represents the color Ecru (#C2B280).
        /// </summary>
        public static BoxColor Ecru => new("#C2B280");

        /// <summary>
        /// Represents the color Fallow (#C19A6B).
        /// </summary>
        public static BoxColor Fallow => new("#C19A6B");

        /// <summary>
        /// Represents the color Fawn (#E5AA70).
        /// </summary>
        public static BoxColor Fawn => new("#E5AA70");

        /// <summary>
        /// Represents the color Garnet (#9A2A2A).
        /// </summary>
        public static BoxColor Garnet => new("#9A2A2A");

        /// <summary>
        /// Represents the color Golden Brown (#966919).
        /// </summary>
        public static BoxColor GoldenBrown => new("#966919");

        /// <summary>
        /// Represents the color Khaki (#F0E68C).
        /// </summary>
        public static BoxColor Khaki => new("#F0E68C");

        /// <summary>
        /// Represents the color Light Brown (#C4A484).
        /// </summary>
        public static BoxColor LightBrown => new("#C4A484");

        /// <summary>
        /// Represents the color Mahogany (#C04000).
        /// </summary>
        public static BoxColor Mahogany => new("#C04000");

        /// <summary>
        /// Represents the color Maroon (#800000).
        /// </summary>
        public static Color Maroon => new("#800000");

        /// <summary>
        /// Represents the color Mocha (#967969).
        /// </summary>
        public static BoxColor Mocha => new("#967969");

        /// <summary>
        /// Represents the color Nude (#F2D2BD).
        /// </summary>
        public static BoxColor Nude => new("#F2D2BD");

        /// <summary>
        /// Represents the color Ochre (#CC7722).
        /// </summary>
        public static BoxColor Ochre => new("#CC7722");

        /// <summary>
        /// Represents the color Olive Green (#808000).
        /// </summary>
        public static BoxColor OliveGreen => new("#808000");

        /// <summary>
        /// Represents the color Oxblood (#4A0400).
        /// </summary>
        public static BoxColor Oxblood => new("#4A0400");

        /// <summary>
        /// Represents the color Puce (#A95C68).
        /// </summary>
        public static BoxColor Puce => new("#A95C68");

        /// <summary>
        /// Represents the color Red Brown (#A52A2A).
        /// </summary>
        public static BoxColor RedBrown => new("#A52A2A");

        /// <summary>
        /// Represents the color Red Ochre (#913831).
        /// </summary>
        public static BoxColor RedOchre => new("#913831");

        /// <summary>
        /// Represents the color Russet (#80461B).
        /// </summary>
        public static BoxColor Russet => new("#80461B");

        /// <summary>
        /// Represents the color Saddle Brown (#8B4513).
        /// </summary>
        public static BoxColor SaddleBrown => new("#8B4513");

        /// <summary>
        /// Represents the color Sand (#C2B280).
        /// </summary>
        public static BoxColor Sand => new("#C2B280");

        /// <summary>
        /// Represents the color Sienna (#A0522D).
        /// </summary>
        public static BoxColor Sienna => new("#A0522D");

        /// <summary>
        /// Represents the color Tan (#D2B48C).
        /// </summary>
        public static BoxColor Tan => new("#D2B48C");

        /// <summary>
        /// Represents the color Taupe (#483C32).
        /// </summary>
        public static BoxColor Taupe => new("#483C32");

        /// <summary>
        /// Represents the color Tuscan Red (#7C3030).
        /// </summary>
        public static BoxColor TuscanRed => new("#7C3030");

        /// <summary>
        /// Represents the color Wheat (#F5DEB3).
        /// </summary>
        public static BoxColor Wheat => new("#F5DEB3");

        /// <summary>
        /// Represents the color Wine (#722F37).
        /// </summary>
        public static BoxColor Wine => new("#722F37");
    }


    /// <summary>
    /// Represents a collection of shades of gray colors.
    /// </summary>
    public readonly struct ShadesOfGray
    {
        /// <summary>
        /// Represents the color Ash Gray (#B2BEB5).
        /// </summary>
        public static BoxColor AshGray => new("#B2BEB5");

        /// <summary>
        /// Represents the color Blue Gray (#7393B3).
        /// </summary>
        public static BoxColor BlueGray => new("#7393B3");

        /// <summary>
        /// Represents the color Charcoal (#36454F).
        /// </summary>
        public static BoxColor Charcoal => new("#36454F");

        /// <summary>
        /// Represents the color Dark Gray (#A9A9A9).
        /// </summary>
        public static BoxColor DarkGray => new("#A9A9A9");

        /// <summary>
        /// Represents the color Glaucous (#6082B6).
        /// </summary>
        public static BoxColor Glaucous => new("#6082B6");

        /// <summary>
        /// Represents the color Gray (#808080).
        /// </summary>
        public static BoxColor Gray => new("#808080");

        /// <summary>
        /// Represents the color Gunmetal Gray (#818589).
        /// </summary>
        public static BoxColor GunmetalGray => new("#818589");

        /// <summary>
        /// Represents the color Light Gray (#D3D3D3).
        /// </summary>
        public static BoxColor LightGray => new("#D3D3D3");

        /// <summary>
        /// Represents the color Pewter (#899499).
        /// </summary>
        public static BoxColor Pewter => new("#899499");

        /// <summary>
        /// Represents the color Platinum (#E5E4E2).
        /// </summary>
        public static BoxColor Platinum => new("#E5E4E2");

        /// <summary>
        /// Represents the color Sage Green (#8A9A5B).
        /// </summary>
        public static BoxColor SageGreen => new("#8A9A5B");

        /// <summary>
        /// Represents the color Silver (#C0C0C0).
        /// </summary>
        public static BoxColor Silver => new("#C0C0C0");

        /// <summary>
        /// Represents the color Slate Gray (#708090).
        /// </summary>
        public static BoxColor SlateGray => new("#708090");

        /// <summary>
        /// Represents the color Smoke (#848884).
        /// </summary>
        public static BoxColor Smoke => new("#848884");

        /// <summary>
        /// Represents the color Steel Gray (#71797E).
        /// </summary>
        public static BoxColor SteelGray => new("#71797E");
    }


    /// <summary>
    /// Represents a collection of shades of green colors.
    /// </summary>
    public readonly struct ShadesOfGreen
    {
        /// <summary>
        /// Represents the color Aqua (#00FFFF).
        /// </summary>
        public static BoxColor Aqua => new("#00FFFF");

        /// <summary>
        /// Represents the color Aquamarine (#7FFFD4).
        /// </summary>
        public static BoxColor Aquamarine => new("#7FFFD4");

        /// <summary>
        /// Represents the color Army Green (#454B1B).
        /// </summary>
        public static BoxColor ArmyGreen => new("#454B1B");

        /// <summary>
        /// Represents the color Blue Green (#088F8F).
        /// </summary>
        public static BoxColor BlueGreen => new("#088F8F");

        /// <summary>
        /// Represents the color Bright Green (#AAFF00).
        /// </summary>
        public static BoxColor BrightGreen => new("#AAFF00");

        /// <summary>
        /// Represents the color Cadet Blue (#5F9EA0).
        /// </summary>
        public static BoxColor CadetBlue => new("#5F9EA0");

        /// <summary>
        /// Represents the color Cadmium Green (#097969).
        /// </summary>
        public static BoxColor CadmiumGreen => new("#097969");

        /// <summary>
        /// Represents the color Celadon (#AFE1AF).
        /// </summary>
        public static BoxColor Celadon => new("#AFE1AF");

        /// <summary>
        /// Represents the color Chartreuse (#DFFF00).
        /// </summary>
        public static BoxColor Chartreuse => new("#DFFF00");

        /// <summary>
        /// Represents the color Citrine (#E4D00A).
        /// </summary>
        public static BoxColor Citrine => new("#E4D00A");

        /// <summary>
        /// Represents the color Cyan (#00FFFF).
        /// </summary>
        public static BoxColor Cyan => new("#00FFFF");

        /// <summary>
        /// Represents the color Dark Green (#023020).
        /// </summary>
        public static BoxColor DarkGreen => new("#023020");

        /// <summary>
        /// Represents the color Electric Blue (#7DF9FF).
        /// </summary>
        public static BoxColor ElectricBlue => new("#7DF9FF");

        /// <summary>
        /// Represents the color Emerald Green (#50C878).
        /// </summary>
        public static BoxColor EmeraldGreen => new("#50C878");

        /// <summary>
        /// Represents the color Eucalyptus (#5F8575).
        /// </summary>
        public static BoxColor Eucalyptus => new("#5F8575");

        /// <summary>
        /// Represents the color Fern Green (#4F7942).
        /// </summary>
        public static BoxColor FernGreen => new("#4F7942");

        /// <summary>
        /// Represents the color Forest Green (#228B22).
        /// </summary>
        public static BoxColor ForestGreen => new("#228B22");

        /// <summary>
        /// Represents the color Grass Green (#7CFC00).
        /// </summary>
        public static BoxColor GrassGreen => new("#7CFC00");

        /// <summary>
        /// Represents the color Green (#008000).
        /// </summary>
        public static BoxColor Green => new("#008000");

        /// <summary>
        /// Represents the color Hunter Green (#355E3B).
        /// </summary>
        public static BoxColor HunterGreen => new("#355E3B");

        /// <summary>
        /// Represents the color Jade (#00A36C).
        /// </summary>
        public static BoxColor Jade => new("#00A36C");

        /// <summary>
        /// Represents the color Jungle Green (#2AAA8A).
        /// </summary>
        public static BoxColor JungleGreen => new("#2AAA8A");

        /// <summary>
        /// Represents the color Kelly Green (#4CBB17).
        /// </summary>
        public static BoxColor KellyGreen => new("#4CBB17");

        /// <summary>
        /// Represents the color Light Green (#90EE90).
        /// </summary>
        public static BoxColor LightGreen => new("#90EE90");

        /// <summary>
        /// Represents the color Lime Green (#32CD32).
        /// </summary>
        public static BoxColor LimeGreen => new("#32CD32");

        /// <summary>
        /// Represents the color Lincoln Green (#478778).
        /// </summary>
        public static BoxColor LincolnGreen => new("#478778");

        /// <summary>
        /// Represents the color Malachite (#0BDA51).
        /// </summary>
        public static BoxColor Malachite => new("#0BDA51");

        /// <summary>
        /// Represents the color Mint Green (#98FB98).
        /// </summary>
        public static BoxColor MintGreen => new("#98FB98");

        /// <summary>
        /// Represents the color Moss Green (#8A9A5B).
        /// </summary>
        public static BoxColor MossGreen => new("#8A9A5B");

        /// <summary>
        /// Represents the color Neon Green (#0FFF50).
        /// </summary>
        public static BoxColor NeonGreen => new("#0FFF50");

        /// <summary>
        /// Represents the color Nyanza (#ECFFDC).
        /// </summary>
        public static BoxColor Nyanza => new("#ECFFDC");

        /// <summary>
        /// Represents the color Olive Green (#808000).
        /// </summary>
        public static BoxColor OliveGreen => new("#808000");

        /// <summary>
        /// Represents the color Pastel Green (#C1E1C1).
        /// </summary>
        public static BoxColor PastelGreen => new("#C1E1C1");

        /// <summary>
        /// Represents the color Pear (#C9CC3F).
        /// </summary>
        public static BoxColor Pear => new("#C9CC3F");

        /// <summary>
        /// Represents the color Peridot (#B4C424).
        /// </summary>
        public static BoxColor Peridot => new("#B4C424");

        /// <summary>
        /// Represents the color Pistachio (#93C572).
        /// </summary>
        public static BoxColor Pistachio => new("#93C572");

        /// <summary>
        /// Represents the color Robin Egg Blue (#96DED1).
        /// </summary>
        public static BoxColor RobinEggBlue => new("#96DED1");

        /// <summary>
        /// Represents the color Sage Green (#8A9A5B).
        /// </summary>
        public static BoxColor SageGreen => new("#8A9A5B");

        /// <summary>
        /// Represents the color Sea Green (#2E8B57).
        /// </summary>
        public static BoxColor SeaGreen => new("#2E8B57");

        /// <summary>
        /// Represents the color Seafoam Green (#9FE2BF).
        /// </summary>
        public static BoxColor SeafoamGreen => new("#9FE2BF");

        /// <summary>
        /// Represents the color Shamrock Green (#009E60).
        /// </summary>
        public static BoxColor ShamrockGreen => new("#009E60");

        /// <summary>
        /// Represents the color Spring Green (#00FF7F).
        /// </summary>
        public static BoxColor SpringGreen => new("#00FF7F");

        /// <summary>
        /// Represents the color Teal (#008080).
        /// </summary>
        public static BoxColor Teal => new("#008080");

        /// <summary>
        /// Represents the color Turquoise (#40E0D0).
        /// </summary>
        public static BoxColor Turquoise => new("#40E0D0");

        /// <summary>
        /// Represents the color Vegas Gold (#C4B454).
        /// </summary>
        public static BoxColor VegasGold => new("#C4B454");

        /// <summary>
        /// Represents the color Verdigris (#40B5AD).
        /// </summary>
        public static BoxColor Verdigris => new("#40B5AD");

        /// <summary>
        /// Represents the color Viridian (#40826D).
        /// </summary>
        public static BoxColor Viridian => new("#40826D");
    }


    /// <summary>
    /// Represents a collection of shades of orange colors.
    /// </summary>
    public readonly struct ShadesOfOrange
    {
        /// <summary>
        /// Represents the color Amber (#FFBF00).
        /// </summary>
        public static Color Amber => new("#FFBF00");

        /// <summary>
        /// Represents the color Apricot (#FBCEB1).
        /// </summary>
        public static Color Apricot => new("#FBCEB1");

        /// <summary>
        /// Represents the color Bisque (#F2D2BD).
        /// </summary>
        public static Color Bisque => new("#F2D2BD");

        /// <summary>
        /// Represents the color Bright Orange (#FFAC1C).
        /// </summary>
        public static Color BrightOrange => new("#FFAC1C");

        /// <summary>
        /// Represents the color Bronze (#CD7F32).
        /// </summary>
        public static Color Bronze => new("#CD7F32");

        /// <summary>
        /// Represents the color Buff (#DAA06D).
        /// </summary>
        public static Color Buff => new("#DAA06D");

        /// <summary>
        /// Represents the color Burnt Orange (#CC5500).
        /// </summary>
        public static Color BurntOrange => new("#CC5500");

        /// <summary>
        /// Represents the color Burnt Sienna (#E97451).
        /// </summary>
        public static Color BurntSienna => new("#E97451");

        /// <summary>
        /// Represents the color Butterscotch (#E3963E).
        /// </summary>
        public static Color Butterscotch => new("#E3963E");

        /// <summary>
        /// Represents the color Cadmium Orange (#F28C28).
        /// </summary>
        public static Color CadmiumOrange => new("#F28C28");

        /// <summary>
        /// Represents the color Cinnamon (#D27D2D).
        /// </summary>
        public static Color Cinnamon => new("#D27D2D");

        /// <summary>
        /// Represents the color Copper (#B87333).
        /// </summary>
        public static Color Copper => new("#B87333");

        /// <summary>
        /// Represents the color Coral (#FF7F50).
        /// </summary>
        public static Color Coral => new("#FF7F50");

        /// <summary>
        /// Represents the color Coral Pink (#F88379).
        /// </summary>
        public static Color CoralPink => new("#F88379");

        /// <summary>
        /// Represents the color Dark Orange (#8B4000).
        /// </summary>
        public static Color DarkOrange => new("#8B4000");

        /// <summary>
        /// Represents the color Desert (#FAD5A5).
        /// </summary>
        public static Color Desert => new("#FAD5A5");

        /// <summary>
        /// Represents the color Gamboge (#E49B0F).
        /// </summary>
        public static Color Gamboge => new("#E49B0F");

        /// <summary>
        /// Represents the color Golden Yellow (#FFC000).
        /// </summary>
        public static Color GoldenYellow => new("#FFC000");

        /// <summary>
        /// Represents the color Goldenrod (#DAA520).
        /// </summary>
        public static Color Goldenrod => new("#DAA520");

        /// <summary>
        /// Represents the color Light Orange (#FFD580).
        /// </summary>
        public static Color LightOrange => new("#FFD580");

        /// <summary>
        /// Represents the color Mahogany (#C04000).
        /// </summary>
        public static Color Mahogany => new("#C04000");

        /// <summary>
        /// Represents the color Mango (#F4BB44).
        /// </summary>
        public static Color Mango => new("#F4BB44");

        /// <summary>
        /// Represents the color Navajo White (#FFDEAD).
        /// </summary>
        public static Color NavajoWhite => new("#FFDEAD");

        /// <summary>
        /// Represents the color Neon Orange (#FF5F1F).
        /// </summary>
        public static Color NeonOrange => new("#FF5F1F");

        /// <summary>
        /// Represents the color Ochre (#CC7722).
        /// </summary>
        public static Color Ochre => new("#CC7722");

        /// <summary>
        /// Represents the color Orange (#FFA500).
        /// </summary>
        public static Color Orange => new("#FFA500");

        /// <summary>
        /// Represents the color Pastel Orange (#FAC898).
        /// </summary>
        public static Color PastelOrange => new("#FAC898");

        /// <summary>
        /// Represents the color Peach (#FFE5B4).
        /// </summary>
        public static Color Peach => new("#FFE5B4");

        /// <summary>
        /// Represents the color Persimmon (#EC5800).
        /// </summary>
        public static Color Persimmon => new("#EC5800");

        /// <summary>
        /// Represents the color Pink Orange (#F89880).
        /// </summary>
        public static Color PinkOrange => new("#F89880");

        /// <summary>
        /// Represents the color Poppy (#E35335).
        /// </summary>
        public static Color Poppy => new("#E35335");

        /// <summary>
        /// Represents the color Pumpkin Orange (#FF7518).
        /// </summary>
        public static Color PumpkinOrange => new("#FF7518");

        /// <summary>
        /// Represents the color Red Orange (#FF4433).
        /// </summary>
        public static Color RedOrange => new("#FF4433");

        /// <summary>
        /// Represents the color Safety Orange (#FF5F15).
        /// </summary>
        public static Color SafetyOrange => new("#FF5F15");

        /// <summary>
        /// Represents the color Salmon (#FA8072).
        /// </summary>
        public static Color Salmon => new("#FA8072");

        /// <summary>
        /// Represents the color Seashell (#FFF5EE).
        /// </summary>
        public static Color Seashell => new("#FFF5EE");

        /// <summary>
        /// Represents the color Sienna (#A0522D).
        /// </summary>
        public static Color Sienna => new("#A0522D");

        /// <summary>
        /// Represents the color Sunset Orange (#FA5F55).
        /// </summary>
        public static Color SunsetOrange => new("#FA5F55");

        /// <summary>
        /// Represents the color Tangerine (#F08000).
        /// </summary>
        public static Color Tangerine => new("#F08000");

        /// <summary>
        /// Represents the color Terra Cotta (#E3735E).
        /// </summary>
        public static Color TerraCotta => new("#E3735E");

        /// <summary>
        /// Represents the color Yellow Orange (#FFAA33).
        /// </summary>
        public static Color YellowOrange => new("#FFAA33");
    }


    /// <summary>
    /// Represents a struct defining various shades of pink colors.
    /// </summary>
    public readonly struct ShadesOfPink
    {
        /// <summary>
        /// Represents the color Amaranth (#9F2B68).
        /// </summary>
        public static Color Amaranth => new("#9F2B68");

        /// <summary>
        /// Represents the color Bisque (#F2D2BD).
        /// </summary>
        public static Color Bisque => new("#F2D2BD");

        /// <summary>
        /// Represents the color Cerise (#DE3163).
        /// </summary>
        public static Color Cerise => new("#DE3163");

        /// <summary>
        /// Represents the color Claret (#811331).
        /// </summary>
        public static Color Claret => new("#811331");

        /// <summary>
        /// Represents the color Coral (#FF7F50).
        /// </summary>
        public static Color Coral => new("#FF7F50");

        /// <summary>
        /// Represents the color Coral Pink (#F88379).
        /// </summary>
        public static Color CoralPink => new("#F88379");

        /// <summary>
        /// Represents the color Crimson (#DC143C).
        /// </summary>
        public static Color Crimson => new("#DC143C");

        /// <summary>
        /// Represents the color Dark Pink (#AA336A).
        /// </summary>
        public static Color DarkPink => new("#AA336A");

        /// <summary>
        /// Represents the color Dusty Rose (#C9A9A6).
        /// </summary>
        public static Color DustyRose => new("#C9A9A6");

        /// <summary>
        /// Represents the color Fuchsia (#FF00FF).
        /// </summary>
        public static Color Fuchsia => new("#FF00FF");

        /// <summary>
        /// Represents the color Hot Pink (#FF69B4).
        /// </summary>
        public static Color HotPink => new("#FF69B4");

        /// <summary>
        /// Represents the color Light Pink (#FFB6C1).
        /// </summary>
        public static Color LightPink => new("#FFB6C1");

        /// <summary>
        /// Represents the color Magenta (#FF00FF).
        /// </summary>
        public static Color Magenta => new("#FF00FF");

        /// <summary>
        /// Represents the color Millennial Pink (#F3CFC6).
        /// </summary>
        public static Color MillennialPink => new("#F3CFC6");

        /// <summary>
        /// Represents the color Mulberry (#770737).
        /// </summary>
        public static Color Mulberry => new("#770737");

        /// <summary>
        /// Represents the color Neon Pink (#FF10F0).
        /// </summary>
        public static Color NeonPink => new("#FF10F0");

        /// <summary>
        /// Represents the color Orchid (#DA70D6).
        /// </summary>
        public static Color Orchid => new("#DA70D6");

        /// <summary>
        /// Represents the color Pastel Pink (#F8C8DC).
        /// </summary>
        public static Color PastelPink => new("#F8C8DC");

        /// <summary>
        /// Represents the color Pastel Red (#FAA0A0).
        /// </summary>
        public static Color PastelRed => new("#FAA0A0");

        /// <summary>
        /// Represents the color Pink (#FFC0CB).
        /// </summary>
        public static Color Pink => new("#FFC0CB");

        /// <summary>
        /// Represents the color Pink Orange (#F89880).
        /// </summary>
        public static Color PinkOrange => new("#F89880");

        /// <summary>
        /// Represents the color Plum (#673147).
        /// </summary>
        public static Color Plum => new("#673147");

        /// <summary>
        /// Represents the color Puce (#A95C68).
        /// </summary>
        public static Color Puce => new("#A95C68");

        /// <summary>
        /// Represents the color Purple (#800080).
        /// </summary>
        public static Color Purple => new("#800080");

        /// <summary>
        /// Represents the color Raspberry (#E30B5C).
        /// </summary>
        public static Color Raspberry => new("#E30B5C");

        /// <summary>
        /// Represents the color Red Purple (#953553).
        /// </summary>
        public static Color RedPurple => new("#953553");

        /// <summary>
        /// Represents the color Rose (#F33A6A).
        /// </summary>
        public static Color Rose => new("#F33A6A");

        /// <summary>
        /// Represents the color Rose Gold (#E0BFB8).
        /// </summary>
        public static Color RoseGold => new("#E0BFB8");

        /// <summary>
        /// Represents the color Rose Red (#C21E56).
        /// </summary>
        public static Color RoseRed => new("#C21E56");

        /// <summary>
        /// Represents the color Ruby Red (#E0115F).
        /// </summary>
        public static Color RubyRed => new("#E0115F");

        /// <summary>
        /// Represents the color Salmon (#FA8072).
        /// </summary>
        public static Color Salmon => new("#FA8072");

        /// <summary>
        /// Represents the color Seashell (#FFF5EE).
        /// </summary>
        public static Color Seashell => new("#FFF5EE");

        /// <summary>
        /// Represents the color Thistle (#D8BFD8).
        /// </summary>
        public static Color Thistle => new("#D8BFD8");

        /// <summary>
        /// Represents the color Watermelon Pink (#E37383).
        /// </summary>
        public static Color WatermelonPink => new("#E37383");
    }


    /// <summary>
    /// Represents a struct defining various shades of purple colors.
    /// </summary>
    public readonly struct ShadesOfPurple
    {
        /// <summary>
        /// Represents the color Amaranth (#9F2B68).
        /// </summary>
        public static Color Amaranth => new("#9F2B68");

        /// <summary>
        /// Represents the color Bright Purple (#BF40BF).
        /// </summary>
        public static Color BrightPurple => new("#BF40BF");

        /// <summary>
        /// Represents the color Burgundy (#800020).
        /// </summary>
        public static Color Burgundy => new("#800020");

        /// <summary>
        /// Represents the color Byzantium (#702963).
        /// </summary>
        public static Color Byzantium => new("#702963");

        /// <summary>
        /// Represents the color Dark Pink (#AA336A).
        /// </summary>
        public static Color DarkPink => new("#AA336A");

        /// <summary>
        /// Represents the color Dark Purple (#301934).
        /// </summary>
        public static Color DarkPurple => new("#301934");

        /// <summary>
        /// Represents the color Eggplant (#483248).
        /// </summary>
        public static Color Eggplant => new("#483248");

        /// <summary>
        /// Represents the color Iris (#5D3FD3).
        /// </summary>
        public static Color Iris => new("#5D3FD3");

        /// <summary>
        /// Represents the color Lavender (#E6E6FA).
        /// </summary>
        public static Color Lavender => new("#E6E6FA");

        /// <summary>
        /// Represents the color Light Purple (#CBC3E3).
        /// </summary>
        public static Color LightPurple => new("#CBC3E3");

        /// <summary>
        /// Represents the color Light Violet (#CF9FFF).
        /// </summary>
        public static Color LightViolet => new("#CF9FFF");

        /// <summary>
        /// Represents the color Lilac (#AA98A9).
        /// </summary>
        public static Color Lilac => new("#AA98A9");

        /// <summary>
        /// Represents the color Mauve (#E0B0FF).
        /// </summary>
        public static Color Mauve => new("#E0B0FF");

        /// <summary>
        /// Represents the color Mauve Taupe (#915F6D).
        /// </summary>
        public static Color MauveTaupe => new("#915F6D");

        /// <summary>
        /// Represents the color Mulberry (#770737).
        /// </summary>
        public static Color Mulberry => new("#770737");

        /// <summary>
        /// Represents the color Orchid (#DA70D6).
        /// </summary>
        public static Color Orchid => new("#DA70D6");

        /// <summary>
        /// Represents the color Pastel Purple (#C3B1E1).
        /// </summary>
        public static Color PastelPurple => new("#C3B1E1");

        /// <summary>
        /// Represents the color Periwinkle (#CCCCFF).
        /// </summary>
        public static Color Periwinkle => new("#CCCCFF");

        /// <summary>
        /// Represents the color Plum (#673147).
        /// </summary>
        public static Color Plum => new("#673147");

        /// <summary>
        /// Represents the color Puce (#A95C68).
        /// </summary>
        public static Color Puce => new("#A95C68");

        /// <summary>
        /// Represents the color Purple (#800080).
        /// </summary>
        public static Color Purple => new("#800080");

        /// <summary>
        /// Represents the color Quartz (#51414F).
        /// </summary>
        public static Color Quartz => new("#51414F");

        /// <summary>
        /// Represents the color Red Purple (#953553).
        /// </summary>
        public static Color RedPurple => new("#953553");

        /// <summary>
        /// Represents the color Thistle (#D8BFD8).
        /// </summary>
        public static Color Thistle => new("#D8BFD8");

        /// <summary>
        /// Represents the color Tyrian Purple (#630330).
        /// </summary>
        public static Color TyrianPurple => new("#630330");

        /// <summary>
        /// Represents the color Violet (#7F00FF).
        /// </summary>
        public static Color Violet => new("#7F00FF");

        /// <summary>
        /// Represents the color Wine (#722F37).
        /// </summary>
        public static Color Wine => new("#722F37");

        /// <summary>
        /// Represents the color Wisteria (#BDB5D5).
        /// </summary>
        public static Color Wisteria => new("#BDB5D5");
    }


    /// <summary>
    /// Represents a struct defining various shades of red colors.
    /// </summary>
    public readonly struct ShadesOfRed
    {
        /// <summary>
        /// Represents the color Blood Red (#880808).
        /// </summary>
        public static Color BloodRed => new("#880808");

        /// <summary>
        /// Represents the color Brick Red (#AA4A44).
        /// </summary>
        public static Color BrickRed => new("#AA4A44");

        /// <summary>
        /// Represents the color Bright Red (#EE4B2B).
        /// </summary>
        public static Color BrightRed => new("#EE4B2B");

        /// <summary>
        /// Represents the color Brown (#A52A2A).
        /// </summary>
        public static Color Brown => new("#A52A2A");

        /// <summary>
        /// Represents the color Burgundy (#800020).
        /// </summary>
        public static Color Burgundy => new("#800020");

        /// <summary>
        /// Represents the color Burnt Umber (#6E260E).
        /// </summary>
        public static Color BurntUmber => new("#6E260E");

        /// <summary>
        /// Represents the color Burnt Orange (#CC5500).
        /// </summary>
        public static Color BurntOrange => new("#CC5500");

        /// <summary>
        /// Represents the color Burnt Sienna (#E97451).
        /// </summary>
        public static Color BurntSienna => new("#E97451");

        /// <summary>
        /// Represents the color Byzantium (#702963).
        /// </summary>
        public static Color Byzantium => new("#702963");

        /// <summary>
        /// Represents the color Cadmium Red (#D22B2B).
        /// </summary>
        public static Color CadmiumRed => new("#D22B2B");

        /// <summary>
        /// Represents the color Cardinal Red (#C41E3A).
        /// </summary>
        public static Color CardinalRed => new("#C41E3A");

        /// <summary>
        /// Represents the color Carmine (#D70040).
        /// </summary>
        public static Color Carmine => new("#D70040");

        /// <summary>
        /// Represents the color Cerise (#DE3163).
        /// </summary>
        public static Color Cerise => new("#DE3163");

        /// <summary>
        /// Represents the color Cherry (#D2042D).
        /// </summary>
        public static Color Cherry => new("#D2042D");

        /// <summary>
        /// Represents the color Chestnut (#954535).
        /// </summary>
        public static Color Chestnut => new("#954535");

        /// <summary>
        /// Represents the color Claret (#811331).
        /// </summary>
        public static Color Claret => new("#811331");

        /// <summary>
        /// Represents the color Coral Pink (#F88379).
        /// </summary>
        public static Color CoralPink => new("#F88379");

        /// <summary>
        /// Represents the color Cordovan (#814141).
        /// </summary>
        public static Color Cordovan => new("#814141");

        /// <summary>
        /// Represents the color Crimson (#DC143C).
        /// </summary>
        public static Color Crimson => new("#DC143C");

        /// <summary>
        /// Represents the color Dark Red (#8B0000).
        /// </summary>
        public static Color DarkRed => new("#8B0000");

        /// <summary>
        /// Represents the color Falu Red (#7B1818).
        /// </summary>
        public static Color FaluRed => new("#7B1818");

        /// <summary>
        /// Represents the color Garnet (#9A2A2A).
        /// </summary>
        public static Color Garnet => new("#9A2A2A");

        /// <summary>
        /// Represents the color Mahogany (#C04000).
        /// </summary>
        public static Color Mahogany => new("#C04000");

        /// <summary>
        /// Represents the color Maroon (#800000).
        /// </summary>
        public static Color Maroon => new("#800000");

        /// <summary>
        /// Represents the color Marsala (#986868).
        /// </summary>
        public static Color Marsala => new("#986868");

        /// <summary>
        /// Represents the color Mulberry (#770737).
        /// </summary>
        public static Color Mulberry => new("#770737");

        /// <summary>
        /// Represents the color Neon Red (#FF3131).
        /// </summary>
        public static Color NeonRed => new("#FF3131");

        /// <summary>
        /// Represents the color Oxblood (#4A0404).
        /// </summary>
        public static Color Oxblood => new("#4A0404");

        /// <summary>
        /// Represents the color Pastel Red (#FAA0A0).
        /// </summary>
        public static Color PastelRed => new("#FAA0A0");

        /// <summary>
        /// Represents the color Persimmon (#EC5800).
        /// </summary>
        public static Color Persimmon => new("#EC5800");

        /// <summary>
        /// Represents the color Poppy (#E35335).
        /// </summary>
        public static Color Poppy => new("#E35335");

        /// <summary>
        /// Represents the color Puce (#A95C68).
        /// </summary>
        public static Color Puce => new("#A95C68");

        /// <summary>
        /// Represents the color Raspberry (#E30B5C).
        /// </summary>
        public static Color Raspberry => new("#E30B5C");

        /// <summary>
        /// Represents the color Red (#FF0000).
        /// </summary>
        public static Color Red => new("#FF0000");

        /// <summary>
        /// Represents the color Red Brown (#A52A2A).
        /// </summary>
        public static Color RedBrown => new("#A52A2A");

        /// <summary>
        /// Represents the color Red Ochre (#913831).
        /// </summary>
        public static Color RedOchre => new("#913831");

        /// <summary>
        /// Represents the color Red Orange (#FF4433).
        /// </summary>
        public static Color RedOrange => new("#FF4433");

        /// <summary>
        /// Represents the color Red Purple (#953553).
        /// </summary>
        public static Color RedPurple => new("#953553");

        /// <summary>
        /// Represents the color Rose Red (#C21E56).
        /// </summary>
        public static Color RoseRed => new("#C21E56");

        /// <summary>
        /// Represents the color Ruby Red (#E0115F).
        /// </summary>
        public static Color RubyRed => new("#E0115F");

        /// <summary>
        /// Represents the color Russet (#80461B).
        /// </summary>
        public static Color Russet => new("#80461B");

        /// <summary>
        /// Represents the color Salmon (#FA8072).
        /// </summary>
        public static Color Salmon => new("#FA8072");

        /// <summary>
        /// Represents the color Scarlet (#FF2400).
        /// </summary>
        public static Color Scarlet => new("#FF2400");

        /// <summary>
        /// Represents the color Sunset Orange (#FA5F55).
        /// </summary>
        public static Color SunsetOrange => new("#FA5F55");

        /// <summary>
        /// Represents the color Terra Cotta (#E3735E).
        /// </summary>
        public static Color TerraCotta => new("#E3735E");

        /// <summary>
        /// Represents the color Tuscan Red (#7C3030).
        /// </summary>
        public static Color TuscanRed => new("#7C3030");

        /// <summary>
        /// Represents the color Tyrian Purple (#630330).
        /// </summary>
        public static Color TyrianPurple => new("#630330");

        /// <summary>
        /// Represents the color Venetian Red (#A42A04).
        /// </summary>
        public static Color VenetianRed => new("#A42A04");

        /// <summary>
        /// Represents the color Vermillion (#E34234).
        /// </summary>
        public static Color Vermillion => new("#E34234");

        /// <summary>
        /// Represents the color Wine (#722F37).
        /// </summary>
        public static Color Wine => new("#722F37");
    }

    /// <summary>
    /// Struct representing shades of white colors.
    /// </summary>
    public readonly struct ShadesOfWhite
    {
        /// <summary>
        /// Represents the color Alabaster (#EDEADE).
        /// </summary>
        public static Color Alabaster => new("#EDEADE");

        /// <summary>
        /// Represents the color Beige (#F5F5DC).
        /// </summary>
        public static Color Beige => new("#F5F5DC");

        /// <summary>
        /// Represents the color Bone White (#F9F6EE).
        /// </summary>
        public static Color BoneWhite => new("#F9F6EE");

        /// <summary>
        /// Represents the color Cornsilk (#FFF8DC).
        /// </summary>
        public static Color Cornsilk => new("#FFF8DC");

        /// <summary>
        /// Represents the color Cream (#FFFDD0).
        /// </summary>
        public static Color Cream => new("#FFFDD0");

        /// <summary>
        /// Represents the color Eggshell (#F0EAD6).
        /// </summary>
        public static Color Eggshell => new("#F0EAD6");

        /// <summary>
        /// Represents the color Ivory (#FFFFF0).
        /// </summary>
        public static Color Ivory => new("#FFFFF0");

        /// <summary>
        /// Represents the color Linen (#E9DCC9).
        /// </summary>
        public static Color Linen => new("#E9DCC9");

        /// <summary>
        /// Represents the color Navajo White (#FFDEAD).
        /// </summary>
        public static Color NavajoWhite => new("#FFDEAD");

        /// <summary>
        /// Represents the color Off White (#FAF9F6).
        /// </summary>
        public static Color OffWhite => new("#FAF9F6");

        /// <summary>
        /// Represents the color Parchment (#FCF5E5).
        /// </summary>
        public static Color Parchment => new("#FCF5E5");

        /// <summary>
        /// Represents the color Peach (#FFE5B4).
        /// </summary>
        public static Color Peach => new("#FFE5B4");

        /// <summary>
        /// Represents the color Pearl (#E2DFD2).
        /// </summary>
        public static Color Pearl => new("#E2DFD2");

        /// <summary>
        /// Represents the color Seashell (#FFF5EE).
        /// </summary>
        public static Color Seashell => new("#FFF5EE");

        /// <summary>
        /// Represents the color Vanilla (#F3E5AB).
        /// </summary>
        public static Color Vanilla => new("#F3E5AB");

        /// <summary>
        /// Represents the color White (#FFFFFF).
        /// </summary>
        public static Color White => new("#FFFFFF");
    }


    /// <summary>
    /// Struct representing shades of yellow colors.
    /// </summary>
    public readonly struct ShadesOfYellow
    {
        /// <summary>
        /// Represents the color Almond (#EADDCA).
        /// </summary>
        public static Color Almond => new("#EADDCA");

        /// <summary>
        /// Represents the color Amber (#FFBF00).
        /// </summary>
        public static Color Amber => new("#FFBF00");

        /// <summary>
        /// Represents the color Apricot (#FBCEB1).
        /// </summary>
        public static Color Apricot => new("#FBCEB1");

        /// <summary>
        /// Represents the color Beige (#F5F5DC).
        /// </summary>
        public static Color Beige => new("#F5F5DC");

        /// <summary>
        /// Represents the color Brass (#E1C16E).
        /// </summary>
        public static Color Brass => new("#E1C16E");

        /// <summary>
        /// Represents the color BrightYellow (#FFEA00).
        /// </summary>
        public static Color BrightYellow => new("#FFEA00");

        /// <summary>
        /// Represents the color CadmiumYellow (#FDDA0D).
        /// </summary>
        public static Color CadmiumYellow => new("#FDDA0D");

        /// <summary>
        /// Represents the color CanaryYellow (#FFFF8F).
        /// </summary>
        public static Color CanaryYellow => new("#FFFF8F");

        /// <summary>
        /// Represents the color Chartreuse (#DFFF00).
        /// </summary>
        public static Color Chartreuse => new("#DFFF00");

        /// <summary>
        /// Represents the color Citrine (#E4D00A).
        /// </summary>
        public static Color Citrine => new("#E4D00A");

        /// <summary>
        /// Represents the color Cornsilk (#FFF8DC).
        /// </summary>
        public static Color Cornsilk => new("#FFF8DC");

        /// <summary>
        /// Represents the color Cream (#FFFDD0).
        /// </summary>
        public static Color Cream => new("#FFFDD0");

        /// <summary>
        /// Represents the color DarkYellow (#8B8000).
        /// </summary>
        public static Color DarkYellow => new("#8B8000");

        /// <summary>
        /// Represents the color Desert (#FAD5A5).
        /// </summary>
        public static Color Desert => new("#FAD5A5");

        /// <summary>
        /// Represents the color Ecru (#C2B280).
        /// </summary>
        public static Color Ecru => new("#C2B280");

        /// <summary>
        /// Represents the color Flax (#EEDC82).
        /// </summary>
        public static Color Flax => new("#EEDC82");

        /// <summary>
        /// Represents the color Gamboge (#E49B0F).
        /// </summary>
        public static Color Gamboge => new("#E49B0F");

        /// <summary>
        /// Represents the color Gold (#FFD700).
        /// </summary>
        public static Color Gold => new("#FFD700");

        /// <summary>
        /// Represents the color GoldenYellow (#FFC000).
        /// </summary>
        public static Color GoldenYellow => new("#FFC000");

        /// <summary>
        /// Represents the color Goldenrod (#DAA520).
        /// </summary>
        public static Color Goldenrod => new("#DAA520");

        /// <summary>
        /// Represents the color Icterine (#FCF55F).
        /// </summary>
        public static Color Icterine => new("#FCF55F");

        /// <summary>
        /// Represents the color Ivory (#FFFFF0).
        /// </summary>
        public static Color Ivory => new("#FFFFF0");

        /// <summary>
        /// Represents the color Jasmine (#F8DE7E).
        /// </summary>
        public static Color Jasmine => new("#F8DE7E");

        /// <summary>
        /// Represents the color Khaki (#F0E68C).
        /// </summary>
        public static Color Khaki => new("#F0E68C");

        /// <summary>
        /// Represents the color LemonYellow (#FAFA33).
        /// </summary>
        public static Color LemonYellow => new("#FAFA33");

        /// <summary>
        /// Represents the color Maize (#FBEC5D).
        /// </summary>
        public static Color Maize => new("#FBEC5D");

        /// <summary>
        /// Represents the color Mango (#F4BB44).
        /// </summary>
        public static Color Mango => new("#F4BB44");

        /// <summary>
        /// Represents the color MustardYellow (#FFDB58).
        /// </summary>
        public static Color MustardYellow => new("#FFDB58");

        /// <summary>
        /// Represents the color NaplesYellow (#FADA5E).
        /// </summary>
        public static Color NaplesYellow => new("#FADA5E");

        /// <summary>
        /// Represents the color NavajoWhite (#FFDEAD).
        /// </summary>
        public static Color NavajoWhite => new("#FFDEAD");

        /// <summary>
        /// Represents the color Nyanza (#ECFFDC).
        /// </summary>
        public static Color Nyanza => new("#ECFFDC");

        /// <summary>
        /// Represents the color PastelYellow (#FFFAA0).
        /// </summary>
        public static Color PastelYellow => new("#FFFAA0");

        /// <summary>
        /// Represents the color Peach (#FFE5B4).
        /// </summary>
        public static Color Peach => new("#FFE5B4");

        /// <summary>
        /// Represents the color Pear (#C9CC3F).
        /// </summary>
        public static Color Pear => new("#C9CC3F");

        /// <summary>
        /// Represents the color Peridot (#B4C424).
        /// </summary>
        public static Color Peridot => new("#B4C424");

        /// <summary>
        /// Represents the color Pistachio (#93C572).
        /// </summary>
        public static Color Pistachio => new("#93C572");

        /// <summary>
        /// Represents the color Saffron (#F4C430).
        /// </summary>
        public static Color Saffron => new("#F4C430");

        /// <summary>
        /// Represents the color Vanilla (#F3E5AB).
        /// </summary>
        public static Color Vanilla => new("#F3E5AB");

        /// <summary>
        /// Represents the color VegasGold (#C4B454).
        /// </summary>
        public static Color VegasGold => new("#C4B454");

        /// <summary>
        /// Represents the color Wheat (#F5DEB3).
        /// </summary>
        public static Color Wheat => new("#F5DEB3");

        /// <summary>
        /// Represents the color Yellow (#FFFF00).
        /// </summary>
        public static Color Yellow => new("#FFFF00");

        /// <summary>
        /// Represents the color YellowOrange (#FFAA33).
        /// </summary>
        public static Color YellowOrange => new("#FFAA33");
    }


    /// <summary>
    /// Represents a struct that defines various shades of transparency.
    /// </summary>
    public readonly struct ShadesOfTransparency
    {
        /// <summary>
        /// Fully transparent color (ARGB: 00-00-00-00).
        /// </summary>
        public static readonly Color Transparent = new("#00000000");

        /// <summary>
        /// 25% transparent black (ARGB: 40-00-00-00).
        /// </summary>
        public static readonly Color Black25Transparent = new("#40000000");

        /// <summary>
        /// 50% transparent black (ARGB: 80-00-00-00).
        /// </summary>
        public static readonly Color Black50Transparent = new("#80000000");

        /// <summary>
        /// 75% transparent black (ARGB: BF-00-00-00).
        /// </summary>
        public static readonly Color Black75Transparent = new("#BF000000");

        /// <summary>
        /// Fully transparent black (ARGB: 00-00-00-00).
        /// </summary>
        public static readonly Color TransparentBlack = new("#00000000");

        /// <summary>
        /// 25% transparent white (ARGB: 40-FF-FF-FF).
        /// </summary>
        public static readonly Color White25Transparent = new("#40FFFFFF");

        /// <summary>
        /// 50% transparent white (ARGB: 80-FF-FF-FF).
        /// </summary>
        public static readonly Color White50Transparent = new("#80FFFFFF");

        /// <summary>
        /// 75% transparent white (ARGB: BF-FF-FF-FF).
        /// </summary>
        public static readonly Color White75Transparent = new("#BFFFFFFF");

        /// <summary>
        /// Fully transparent white (ARGB: FF-FF-FF-00).
        /// </summary>
        public static readonly Color TransparentWhite = new("#FFFFFF00");
    }


    /// <summary>
    /// Struct representing various shades of colors.
    /// </summary>
    public readonly struct AllShades
    {
        /// <summary>
        /// Represents the color black (#000000).
        /// </summary>
        public static Color Black => new("#000000");

        /// <summary>
        /// Represents the color charcoal (#36454F).
        /// </summary>
        public static Color Charcoal => new("#36454F");

        /// <summary>
        /// Represents the color dark green (#023020).
        /// </summary>
        public static Color DarkGreen => new("#023020");

        /// <summary>
        /// Represents the color dark purple (#301934).
        /// </summary>
        public static Color DarkPurple => new("#301934");

        /// <summary>
        /// Represents the color jet black (#343434).
        /// </summary>
        public static Color JetBlack => new("#343434");

        /// <summary>
        /// Represents the color licorice (#1B1212).
        /// </summary>
        public static Color Licorice => new("#1B1212");

        /// <summary>
        /// Represents the color matte black (#28282B).
        /// </summary>
        public static Color MatteBlack => new("#28282B");

        /// <summary>
        /// Represents the color midnight blue (#191970).
        /// </summary>
        public static Color MidnightBlue => new("#191970");

        /// <summary>
        /// Represents the color onyx (#353935).
        /// </summary>
        public static Color Onyx => new("#353935");


        /// <summary>
        /// Represents the color Aqua (#00FFFF).
        /// </summary>
        public static Color Aqua => new("#00FFFF");

        /// <summary>
        /// Represents the color Azure (#F0FFFF).
        /// </summary>
        public static Color Azure => new("#F0FFFF");

        /// <summary>
        /// Represents the color Baby Blue (#89CFF0).
        /// </summary>
        public static Color BabyBlue => new("#89CFF0");

        /// <summary>
        /// Represents the color Blue (#0000FF).
        /// </summary>
        public static Color Blue => new("#0000FF");

        /// <summary>
        /// Represents the color Blue Gray (#7393B3).
        /// </summary>
        public static Color BlueGray => new("#7393B3");

        /// <summary>
        /// Represents the color Blue Green (#088F8F).
        /// </summary>
        public static Color BlueGreen => new("#088F8F");

        /// <summary>
        /// Represents the color Bright Blue (#0096FF).
        /// </summary>
        public static Color BrightBlue => new("#0096FF");

        /// <summary>
        /// Represents the color Cadet Blue (#5F9EA0).
        /// </summary>
        public static Color CadetBlue => new("#5F9EA0");

        /// <summary>
        /// Represents the color Cobalt Blue (#0047AB).
        /// </summary>
        public static Color CobaltBlue => new("#0047AB");

        /// <summary>
        /// Represents the color Cornflower Blue (#6495ED).
        /// </summary>
        public static Color CornflowerBlue => new("#6495ED");

        /// <summary>
        /// Represents the color Cyan (#00FFFF).
        /// </summary>
        public static Color Cyan => new("#00FFFF");

        /// <summary>
        /// Represents the color Dark Blue (#00008B).
        /// </summary>
        public static Color DarkBlue => new("#00008B");

        /// <summary>
        /// Represents the color Denim (#6F8FAF).
        /// </summary>
        public static Color Denim => new("#6F8FAF");

        /// <summary>
        /// Represents the color Egyptian Blue (#1434A4).
        /// </summary>
        public static Color EgyptianBlue => new("#1434A4");

        /// <summary>
        /// Represents the color Electric Blue (#7DF9FF).
        /// </summary>
        public static Color ElectricBlue => new("#7DF9FF");

        /// <summary>
        /// Represents the color Glaucous (#6082B6).
        /// </summary>
        public static Color Glaucous => new("#6082B6");

        /// <summary>
        /// Represents the color Jade (#00A36C).
        /// </summary>
        public static Color Jade => new("#00A36C");

        /// <summary>
        /// Represents the color Indigo (#3F00FF).
        /// </summary>
        public static Color Indigo => new("#3F00FF");

        /// <summary>
        /// Represents the color Iris (#5D3FD3).
        /// </summary>
        public static Color Iris => new("#5D3FD3");

        /// <summary>
        /// Represents the color Light Blue (#ADD8E6).
        /// </summary>
        public static Color LightBlue => new("#ADD8E6");

        /// <summary>
        /// Represents the color Navy Blue (#000080).
        /// </summary>
        public static Color NavyBlue => new("#000080");

        /// <summary>
        /// Represents the color Neon Blue (#1F51FF).
        /// </summary>
        public static Color NeonBlue => new("#1F51FF");

        /// <summary>
        /// Represents the color Pastel Blue (#A7C7E7).
        /// </summary>
        public static Color PastelBlue => new("#A7C7E7");

        /// <summary>
        /// Represents the color Periwinkle (#CCCCFF).
        /// </summary>
        public static Color Periwinkle => new("#CCCCFF");

        /// <summary>
        /// Represents the color Powder Blue (#B6D0E2).
        /// </summary>
        public static Color PowderBlue => new("#B6D0E2");

        /// <summary>
        /// Represents the color Robin Egg Blue (#96DED1).
        /// </summary>
        public static Color RobinEggBlue => new("#96DED1");

        /// <summary>
        /// Represents the color Royal Blue (#4169E1).
        /// </summary>
        public static Color RoyalBlue => new("#4169E1");

        /// <summary>
        /// Represents the color Sapphire Blue (#0F52BA).
        /// </summary>
        public static Color SapphireBlue => new("#0F52BA");

        /// <summary>
        /// Represents the color Seafoam Green (#9FE2BF).
        /// </summary>
        public static Color SeafoamGreen => new("#9FE2BF");

        /// <summary>
        /// Represents the color Sky Blue (#87CEEB).
        /// </summary>
        public static Color SkyBlue => new("#87CEEB");

        /// <summary>
        /// Represents the color Steel Blue (#4682B4).
        /// </summary>
        public static Color SteelBlue => new("#4682B4");

        /// <summary>
        /// Represents the color Teal (#008080).
        /// </summary>
        public static Color Teal => new("#008080");

        /// <summary>
        /// Represents the color Turquoise (#40E0D0).
        /// </summary>
        public static Color Turquoise => new("#40E0D0");

        /// <summary>
        /// Represents the color Ultramarine (#0437F2).
        /// </summary>
        public static Color Ultramarine => new("#0437F2");

        /// <summary>
        /// Represents the color Verdigris (#40B5AD).
        /// </summary>
        public static Color Verdigris => new("#40B5AD");

        /// <summary>
        /// Represents the color Zaffre (#0818A8).
        /// </summary>
        public static Color Zaffre => new("#0818A8");


        /// <summary>
        /// Represents the color Almond (#EADDCA).
        /// </summary>
        public static Color Almond => new("#EADDCA");

        /// <summary>
        /// Represents the color Brass (#E1C16E).
        /// </summary>
        public static Color Brass => new("#E1C16E");

        /// <summary>
        /// Represents the color Bronze (#CD7F32).
        /// </summary>
        public static Color Bronze => new("#CD7F32");

        /// <summary>
        /// Represents the color Brown (#A52A2A).
        /// </summary>
        public static Color Brown => new("#A52A2A");

        /// <summary>
        /// Represents the color Buff (#DAA06D).
        /// </summary>
        public static Color Buff => new("#DAA06D");

        /// <summary>
        /// Represents the color Burgundy (#800020).
        /// </summary>
        public static Color Burgundy => new("#800020");

        /// <summary>
        /// Represents the color Burnt Sienna (#E97451).
        /// </summary>
        public static Color BurntSienna => new("#E97451");

        /// <summary>
        /// Represents the color Burnt Umber (#6E260E).
        /// </summary>
        public static Color BurntUmber => new("#6E260E");

        /// <summary>
        /// Represents the color Camel (#C19A6B).
        /// </summary>
        public static Color Camel => new("#C19A6B");

        /// <summary>
        /// Represents the color Chestnut (#954535).
        /// </summary>
        public static Color Chestnut => new("#954535");

        /// <summary>
        /// Represents the color Chocolate (#7B3F00).
        /// </summary>
        public static Color Chocolate => new("#7B3F00");

        /// <summary>
        /// Represents the color Cinnamon (#D27D2D).
        /// </summary>
        public static Color Cinnamon => new("#D27D2D");

        /// <summary>
        /// Represents the color Coffee (#6F4E37).
        /// </summary>
        public static Color Coffee => new("#6F4E37");

        /// <summary>
        /// Represents the color Cognac (#834333).
        /// </summary>
        public static Color Cognac => new("#834333");

        /// <summary>
        /// Represents the color Copper (#B87333).
        /// </summary>
        public static Color Copper => new("#B87333");

        /// <summary>
        /// Represents the color Cordovan (#814141).
        /// </summary>
        public static Color Cordovan => new("#814141");

        /// <summary>
        /// Represents the color Dark Brown (#5C4033).
        /// </summary>
        public static Color DarkBrown => new("#5C4033");

        /// <summary>
        /// Represents the color Dark Red (#8B0000).
        /// </summary>
        public static Color DarkRed => new("#8B0000");

        /// <summary>
        /// Represents the color Dark Tan (#988558).
        /// </summary>
        public static Color DarkTan => new("#988558");

        /// <summary>
        /// Represents the color Ecru (#C2B280).
        /// </summary>
        public static Color Ecru => new("#C2B280");

        /// <summary>
        /// Represents the color Fallow (#C19A6B).
        /// </summary>
        public static Color Fallow => new("#C19A6B");

        /// <summary>
        /// Represents the color Fawn (#E5AA70).
        /// </summary>
        public static Color Fawn => new("#E5AA70");

        /// <summary>
        /// Represents the color Garnet (#9A2A2A).
        /// </summary>
        public static Color Garnet => new("#9A2A2A");

        /// <summary>
        /// Represents the color Golden Brown (#966919).
        /// </summary>
        public static Color GoldenBrown => new("#966919");

        /// <summary>
        /// Represents the color Khaki (#F0E68C).
        /// </summary>
        public static Color Khaki => new("#F0E68C");

        /// <summary>
        /// Represents the color Light Brown (#C4A484).
        /// </summary>
        public static Color LightBrown => new("#C4A484");

        /// <summary>
        /// Represents the color Mahogany (#C04000).
        /// </summary>
        public static Color Mahogany => new("#C04000");

        /// <summary>
        /// Represents the color Maroon (#800000).
        /// </summary>
        public static Color Maroon => new("#800000");

        /// <summary>
        /// Represents the color Mocha (#967969).
        /// </summary>
        public static Color Mocha => new("#967969");

        /// <summary>
        /// Represents the color Nude (#F2D2BD).
        /// </summary>
        public static Color Nude => new("#F2D2BD");

        /// <summary>
        /// Represents the color Ochre (#CC7722).
        /// </summary>
        public static Color Ochre => new("#CC7722");

        /// <summary>
        /// Represents the color Olive Green (#808000).
        /// </summary>
        public static Color OliveGreen => new("#808000");

        /// <summary>
        /// Represents the color Oxblood (#4A0400).
        /// </summary>
        public static Color Oxblood => new("#4A0400");

        /// <summary>
        /// Represents the color Puce (#A95C68).
        /// </summary>
        public static Color Puce => new("#A95C68");

        /// <summary>
        /// Represents the color Red Brown (#A52A2A).
        /// </summary>
        public static Color RedBrown => new("#A52A2A");

        /// <summary>
        /// Represents the color Red Ochre (#913831).
        /// </summary>
        public static Color RedOchre => new("#913831");

        /// <summary>
        /// Represents the color Russet (#80461B).
        /// </summary>
        public static Color Russet => new("#80461B");

        /// <summary>
        /// Represents the color Saddle Brown (#8B4513).
        /// </summary>
        public static Color SaddleBrown => new("#8B4513");

        /// <summary>
        /// Represents the color Sand (#C2B280).
        /// </summary>
        public static Color Sand => new("#C2B280");

        /// <summary>
        /// Represents the color Sienna (#A0522D).
        /// </summary>
        public static Color Sienna => new("#A0522D");

        /// <summary>
        /// Represents the color Tan (#D2B48C).
        /// </summary>
        public static Color Tan => new("#D2B48C");

        /// <summary>
        /// Represents the color Taupe (#483C32).
        /// </summary>
        public static Color Taupe => new("#483C32");

        /// <summary>
        /// Represents the color Tuscan Red (#7C3030).
        /// </summary>
        public static Color TuscanRed => new("#7C3030");

        /// <summary>
        /// Represents the color Wheat (#F5DEB3).
        /// </summary>
        public static Color Wheat => new("#F5DEB3");

        /// <summary>
        /// Represents the color Wine (#722F37).
        /// </summary>
        public static Color Wine => new("#722F37");

        /// <summary>
        /// Represents the color Ash Gray (#B2BEB5).
        /// </summary>
        public static Color AshGray => new("#B2BEB5");

        /// <summary>
        /// Represents the color Dark Gray (#A9A9A9).
        /// </summary>
        public static Color DarkGray => new("#A9A9A9");

        /// <summary>
        /// Represents the color Gray (#808080).
        /// </summary>
        public static Color Gray => new("#808080");

        /// <summary>
        /// Represents the color Gunmetal Gray (#818589).
        /// </summary>
        public static Color GunmetalGray => new("#818589");

        /// <summary>
        /// Represents the color Light Gray (#D3D3D3).
        /// </summary>
        public static Color LightGray => new("#D3D3D3");

        /// <summary>
        /// Represents the color Pewter (#899499).
        /// </summary>
        public static Color Pewter => new("#899499");

        /// <summary>
        /// Represents the color Platinum (#E5E4E2).
        /// </summary>
        public static Color Platinum => new("#E5E4E2");

        /// <summary>
        /// Represents the color Sage Green (#8A9A5B).
        /// </summary>
        public static Color SageGreen => new("#8A9A5B");

        /// <summary>
        /// Represents the color Silver (#C0C0C0).
        /// </summary>
        public static Color Silver => new("#C0C0C0");

        /// <summary>
        /// Represents the color Slate Gray (#708090).
        /// </summary>
        public static Color SlateGray => new("#708090");

        /// <summary>
        /// Represents the color Smoke (#848884).
        /// </summary>
        public static Color Smoke => new("#848884");

        /// <summary>
        /// Represents the color Steel Gray (#71797E).
        /// </summary>
        public static Color SteelGray => new("#71797E");

        /// <summary>
        /// Represents the color Aquamarine (#7FFFD4).
        /// </summary>
        public static Color Aquamarine => new("#7FFFD4");

        /// <summary>
        /// Represents the color Army Green (#454B1B).
        /// </summary>
        public static Color ArmyGreen => new("#454B1B");

        /// <summary>
        /// Represents the color Bright Green (#AAFF00).
        /// </summary>
        public static Color BrightGreen => new("#AAFF00");

        /// <summary>
        /// Represents the color Cadmium Green (#097969).
        /// </summary>
        public static Color CadmiumGreen => new("#097969");

        /// <summary>
        /// Represents the color Celadon (#AFE1AF).
        /// </summary>
        public static Color Celadon => new("#AFE1AF");

        /// <summary>
        /// Represents the color Chartreuse (#DFFF00).
        /// </summary>
        public static Color Chartreuse => new("#DFFF00");

        /// <summary>
        /// Represents the color Citrine (#E4D00A).
        /// </summary>
        public static Color Citrine => new("#E4D00A");

        /// <summary>
        /// Represents the color Emerald Green (#50C878).
        /// </summary>
        public static Color EmeraldGreen => new("#50C878");

        /// <summary>
        /// Represents the color Eucalyptus (#5F8575).
        /// </summary>
        public static Color Eucalyptus => new("#5F8575");

        /// <summary>
        /// Represents the color Fern Green (#4F7942).
        /// </summary>
        public static Color FernGreen => new("#4F7942");

        /// <summary>
        /// Represents the color Forest Green (#228B22).
        /// </summary>
        public static Color ForestGreen => new("#228B22");

        /// <summary>
        /// Represents the color Grass Green (#7CFC00).
        /// </summary>
        public static Color GrassGreen => new("#7CFC00");

        /// <summary>
        /// Represents the color Green (#008000).
        /// </summary>
        public static Color Green => new("#008000");

        /// <summary>
        /// Represents the color Hunter Green (#355E3B).
        /// </summary>
        public static Color HunterGreen => new("#355E3B");

        /// <summary>
        /// Represents the color Jungle Green (#2AAA8A).
        /// </summary>
        public static Color JungleGreen => new("#2AAA8A");

        /// <summary>
        /// Represents the color Kelly Green (#4CBB17).
        /// </summary>
        public static Color KellyGreen => new("#4CBB17");

        /// <summary>
        /// Represents the color Light Green (#90EE90).
        /// </summary>
        public static Color LightGreen => new("#90EE90");

        /// <summary>
        /// Represents the color Lime Green (#32CD32).
        /// </summary>
        public static Color LimeGreen => new("#32CD32");

        /// <summary>
        /// Represents the color Lincoln Green (#478778).
        /// </summary>
        public static Color LincolnGreen => new("#478778");

        /// <summary>
        /// Represents the color Malachite (#0BDA51).
        /// </summary>
        public static Color Malachite => new("#0BDA51");

        /// <summary>
        /// Represents the color Mint Green (#98FB98).
        /// </summary>
        public static Color MintGreen => new("#98FB98");

        /// <summary>
        /// Represents the color Moss Green (#8A9A5B).
        /// </summary>
        public static Color MossGreen => new("#8A9A5B");

        /// <summary>
        /// Represents the color Neon Green (#0FFF50).
        /// </summary>
        public static Color NeonGreen => new("#0FFF50");

        /// <summary>
        /// Represents the color Nyanza (#ECFFDC).
        /// </summary>
        public static Color Nyanza => new("#ECFFDC");

        /// <summary>
        /// Represents the color Pastel Green (#C1E1C1).
        /// </summary>
        public static Color PastelGreen => new("#C1E1C1");

        /// <summary>
        /// Represents the color Pear (#C9CC3F).
        /// </summary>
        public static Color Pear => new("#C9CC3F");

        /// <summary>
        /// Represents the color Peridot (#B4C424).
        /// </summary>
        public static Color Peridot => new("#B4C424");

        /// <summary>
        /// Represents the color Pistachio (#93C572).
        /// </summary>
        public static Color Pistachio => new("#93C572");

        /// <summary>
        /// Represents the color Sea Green (#2E8B57).
        /// </summary>
        public static Color SeaGreen => new("#2E8B57");

        /// <summary>
        /// Represents the color Shamrock Green (#009E60).
        /// </summary>
        public static Color ShamrockGreen => new("#009E60");

        /// <summary>
        /// Represents the color Spring Green (#00FF7F).
        /// </summary>
        public static Color SpringGreen => new("#00FF7F");

        /// <summary>
        /// Represents the color Vegas Gold (#C4B454).
        /// </summary>
        public static Color VegasGold => new("#C4B454");

        /// <summary>
        /// Represents the color Viridian (#40826D).
        /// </summary>
        public static Color Viridian => new("#40826D");


        /// <summary>
        /// Represents the color Amber (#FFBF00).
        /// </summary>
        public static Color Amber => new("#FFBF00");

        /// <summary>
        /// Represents the color Apricot (#FBCEB1).
        /// </summary>
        public static Color Apricot => new("#FBCEB1");

        /// <summary>
        /// Represents the color Bisque (#F2D2BD).
        /// </summary>
        public static Color Bisque => new("#F2D2BD");

        /// <summary>
        /// Represents the color Bright Orange (#FFAC1C).
        /// </summary>
        public static Color BrightOrange => new("#FFAC1C");

        /// <summary>
        /// Represents the color Burnt Orange (#CC5500).
        /// </summary>
        public static Color BurntOrange => new("#CC5500");

        /// <summary>
        /// Represents the color Butterscotch (#E3963E).
        /// </summary>
        public static Color Butterscotch => new("#E3963E");

        /// <summary>
        /// Represents the color Cadmium Orange (#F28C28).
        /// </summary>
        public static Color CadmiumOrange => new("#F28C28");

        /// <summary>
        /// Represents the color Coral (#FF7F50).
        /// </summary>
        public static Color Coral => new("#FF7F50");

        /// <summary>
        /// Represents the color Coral Pink (#F88379).
        /// </summary>
        public static Color CoralPink => new("#F88379");

        /// <summary>
        /// Represents the color Dark Orange (#8B4000).
        /// </summary>
        public static Color DarkOrange => new("#8B4000");

        /// <summary>
        /// Represents the color Desert (#FAD5A5).
        /// </summary>
        public static Color Desert => new("#FAD5A5");

        /// <summary>
        /// Represents the color Gamboge (#E49B0F).
        /// </summary>
        public static Color Gamboge => new("#E49B0F");

        /// <summary>
        /// Represents the color Golden Yellow (#FFC000).
        /// </summary>
        public static Color GoldenYellow => new("#FFC000");

        /// <summary>
        /// Represents the color Goldenrod (#DAA520).
        /// </summary>
        public static Color Goldenrod => new("#DAA520");

        /// <summary>
        /// Represents the color Light Orange (#FFD580).
        /// </summary>
        public static Color LightOrange => new("#FFD580");

        /// <summary>
        /// Represents the color Mango (#F4BB44).
        /// </summary>
        public static Color Mango => new("#F4BB44");

        /// <summary>
        /// Represents the color Navajo White (#FFDEAD).
        /// </summary>
        public static Color NavajoWhite => new("#FFDEAD");

        /// <summary>
        /// Represents the color Neon Orange (#FF5F1F).
        /// </summary>
        public static Color NeonOrange => new("#FF5F1F");

        /// <summary>
        /// Represents the color Orange (#FFA500).
        /// </summary>
        public static Color Orange => new("#FFA500");

        /// <summary>
        /// Represents the color Pastel Orange (#FAC898).
        /// </summary>
        public static Color PastelOrange => new("#FAC898");

        /// <summary>
        /// Represents the color Peach (#FFE5B4).
        /// </summary>
        public static Color Peach => new("#FFE5B4");

        /// <summary>
        /// Represents the color Persimmon (#EC5800).
        /// </summary>
        public static Color Persimmon => new("#EC5800");

        /// <summary>
        /// Represents the color Pink Orange (#F89880).
        /// </summary>
        public static Color PinkOrange => new("#F89880");

        /// <summary>
        /// Represents the color Poppy (#E35335).
        /// </summary>
        public static Color Poppy => new("#E35335");

        /// <summary>
        /// Represents the color Pumpkin Orange (#FF7518).
        /// </summary>
        public static Color PumpkinOrange => new("#FF7518");

        /// <summary>
        /// Represents the color Red Orange (#FF4433).
        /// </summary>
        public static Color RedOrange => new("#FF4433");

        /// <summary>
        /// Represents the color Safety Orange (#FF5F15).
        /// </summary>
        public static Color SafetyOrange => new("#FF5F15");

        /// <summary>
        /// Represents the color Salmon (#FA8072).
        /// </summary>
        public static Color Salmon => new("#FA8072");

        /// <summary>
        /// Represents the color Seashell (#FFF5EE).
        /// </summary>
        public static Color Seashell => new("#FFF5EE");

        /// <summary>
        /// Represents the color Sunset Orange (#FA5F55).
        /// </summary>
        public static Color SunsetOrange => new("#FA5F55");

        /// <summary>
        /// Represents the color Tangerine (#F08000).
        /// </summary>
        public static Color Tangerine => new("#F08000");

        /// <summary>
        /// Represents the color Terra Cotta (#E3735E).
        /// </summary>
        public static Color TerraCotta => new("#E3735E");

        /// <summary>
        /// Represents the color Yellow Orange (#FFAA33).
        /// </summary>
        public static Color YellowOrange => new("#FFAA33");

        /// <summary>
        /// Represents the color Amaranth (#9F2B68).
        /// </summary>
        public static Color Amaranth => new("#9F2B68");

        /// <summary>
        /// Represents the color Cerise (#DE3163).
        /// </summary>
        public static Color Cerise => new("#DE3163");

        /// <summary>
        /// Represents the color Claret (#811331).
        /// </summary>
        public static Color Claret => new("#811331");

        /// <summary>
        /// Represents the color Crimson (#DC143C).
        /// </summary>
        public static Color Crimson => new("#DC143C");

        /// <summary>
        /// Represents the color Dark Pink (#AA336A).
        /// </summary>
        public static Color DarkPink => new("#AA336A");

        /// <summary>
        /// Represents the color Dusty Rose (#C9A9A6).
        /// </summary>
        public static Color DustyRose => new("#C9A9A6");

        /// <summary>
        /// Represents the color Fuchsia (#FF00FF).
        /// </summary>
        public static Color Fuchsia => new("#FF00FF");

        /// <summary>
        /// Represents the color Hot Pink (#FF69B4).
        /// </summary>
        public static Color HotPink => new("#FF69B4");

        /// <summary>
        /// Represents the color Light Pink (#FFB6C1).
        /// </summary>
        public static Color LightPink => new("#FFB6C1");

        /// <summary>
        /// Represents the color Magenta (#FF00FF).
        /// </summary>
        public static Color Magenta => new("#FF00FF");

        /// <summary>
        /// Represents the color Millennial Pink (#F3CFC6).
        /// </summary>
        public static Color MillennialPink => new("#F3CFC6");

        /// <summary>
        /// Represents the color Mulberry (#770737).
        /// </summary>
        public static Color Mulberry => new("#770737");

        /// <summary>
        /// Represents the color Neon Pink (#FF10F0).
        /// </summary>
        public static Color NeonPink => new("#FF10F0");

        /// <summary>
        /// Represents the color Orchid (#DA70D6).
        /// </summary>
        public static Color Orchid => new("#DA70D6");

        /// <summary>
        /// Represents the color Pastel Pink (#F8C8DC).
        /// </summary>
        public static Color PastelPink => new("#F8C8DC");

        /// <summary>
        /// Represents the color Pastel Red (#FAA0A0).
        /// </summary>
        public static Color PastelRed => new("#FAA0A0");

        /// <summary>
        /// Represents the color Pink (#FFC0CB).
        /// </summary>
        public static Color Pink => new("#FFC0CB");

        /// <summary>
        /// Represents the color Plum (#673147).
        /// </summary>
        public static Color Plum => new("#673147");

        /// <summary>
        /// Represents the color Purple (#800080).
        /// </summary>
        public static Color Purple => new("#800080");

        /// <summary>
        /// Represents the color Raspberry (#E30B5C).
        /// </summary>
        public static Color Raspberry => new("#E30B5C");

        /// <summary>
        /// Represents the color Red Purple (#953553).
        /// </summary>
        public static Color RedPurple => new("#953553");

        /// <summary>
        /// Represents the color Rose (#F33A6A).
        /// </summary>
        public static Color Rose => new("#F33A6A");

        /// <summary>
        /// Represents the color Rose Gold (#E0BFB8).
        /// </summary>
        public static Color RoseGold => new("#E0BFB8");

        /// <summary>
        /// Represents the color Rose Red (#C21E56).
        /// </summary>
        public static Color RoseRed => new("#C21E56");

        /// <summary>
        /// Represents the color Ruby Red (#E0115F).
        /// </summary>
        public static Color RubyRed => new("#E0115F");

        /// <summary>
        /// Represents the color Thistle (#D8BFD8).
        /// </summary>
        public static Color Thistle => new("#D8BFD8");

        /// <summary>
        /// Represents the color Watermelon Pink (#E37383).
        /// </summary>
        public static Color WatermelonPink => new("#E37383");

        /// <summary>
        /// Represents the color Bright Purple (#BF40BF).
        /// </summary>
        public static Color BrightPurple => new("#BF40BF");

        /// <summary>
        /// Represents the color Byzantium (#702963).
        /// </summary>
        public static Color Byzantium => new("#702963");

        /// <summary>
        /// Represents the color Eggplant (#483248).
        /// </summary>
        public static Color Eggplant => new("#483248");

        /// <summary>
        /// Represents the color Lavender (#E6E6FA).
        /// </summary>
        public static Color Lavender => new("#E6E6FA");

        /// <summary>
        /// Represents the color Light Purple (#CBC3E3).
        /// </summary>
        public static Color LightPurple => new("#CBC3E3");

        /// <summary>
        /// Represents the color Light Violet (#CF9FFF).
        /// </summary>
        public static Color LightViolet => new("#CF9FFF");

        /// <summary>
        /// Represents the color Lilac (#AA98A9).
        /// </summary>
        public static Color Lilac => new("#AA98A9");

        /// <summary>
        /// Represents the color Mauve (#E0B0FF).
        /// </summary>
        public static Color Mauve => new("#E0B0FF");

        /// <summary>
        /// Represents the color Mauve Taupe (#915F6D).
        /// </summary>
        public static Color MauveTaupe => new("#915F6D");

        /// <summary>
        /// Represents the color Pastel Purple (#C3B1E1).
        /// </summary>
        public static Color PastelPurple => new("#C3B1E1");

        /// <summary>
        /// Represents the color Periwinkle (#CCCCFF).
        /// </summary>

        /// <summary>
        /// Represents the color Quartz (#51414F).
        /// </summary>
        public static Color Quartz => new("#51414F");

        /// <summary>
        /// Represents the color Tyrian Purple (#630330).
        /// </summary>
        public static Color TyrianPurple => new("#630330");

        /// <summary>
        /// Represents the color Violet (#7F00FF).
        /// </summary>
        public static Color Violet => new("#7F00FF");

        /// <summary>
        /// Represents the color Wisteria (#BDB5D5).
        /// </summary>
        public static Color Wisteria => new("#BDB5D5");


        /// <summary>
        /// Represents the color Blood Red (#880808).
        /// </summary>
        public static Color BloodRed => new("#880808");

        /// <summary>
        /// Represents the color Brick Red (#AA4A44).
        /// </summary>
        public static Color BrickRed => new("#AA4A44");

        /// <summary>
        /// Represents the color Bright Red (#EE4B2B).
        /// </summary>
        public static Color BrightRed => new("#EE4B2B");

        /// <summary>
        /// Represents the color Cadmium Red (#D22B2B).
        /// </summary>
        public static Color CadmiumRed => new("#D22B2B");

        /// <summary>
        /// Represents the color Cardinal Red (#C41E3A).
        /// </summary>
        public static Color CardinalRed => new("#C41E3A");

        /// <summary>
        /// Represents the color Carmine (#D70040).
        /// </summary>
        public static Color Carmine => new("#D70040");

        /// <summary>
        /// Represents the color Cherry (#D2042D).
        /// </summary>
        public static Color Cherry => new("#D2042D");

        /// <summary>
        /// Represents the color Falu Red (#7B1818).
        /// </summary>
        public static Color FaluRed => new("#7B1818");

        /// <summary>
        /// Represents the color Marsala (#986868).
        /// </summary>
        public static Color Marsala => new("#986868");

        /// <summary>
        /// Represents the color Neon Red (#FF3131).
        /// </summary>
        public static Color NeonRed => new("#FF3131");

        /// <summary>
        /// Represents the color Red (#FF0000).
        /// </summary>
        public static Color Red => new("#FF0000");

        /// <summary>
        /// Represents the color Scarlet (#FF2400).
        /// </summary>
        public static Color Scarlet => new("#FF2400");

        /// <summary>
        /// Represents the color Venetian Red (#A42A04).
        /// </summary>
        public static Color VenetianRed => new("#A42A04");

        /// <summary>
        /// Represents the color Vermillion (#E34234).
        /// </summary>
        public static Color Vermillion => new("#E34234");

        /// <summary>
        /// Represents the color Alabaster (#EDEADE).
        /// </summary>
        public static Color Alabaster => new("#EDEADE");

        /// <summary>
        /// Represents the color Beige (#F5F5DC).
        /// </summary>
        public static Color Beige => new("#F5F5DC");

        /// <summary>
        /// Represents the color Bone White (#F9F6EE).
        /// </summary>
        public static Color BoneWhite => new("#F9F6EE");

        /// <summary>
        /// Represents the color Cornsilk (#FFF8DC).
        /// </summary>
        public static Color Cornsilk => new("#FFF8DC");

        /// <summary>
        /// Represents the color Cream (#FFFDD0).
        /// </summary>
        public static Color Cream => new("#FFFDD0");

        /// <summary>
        /// Represents the color Eggshell (#F0EAD6).
        /// </summary>
        public static Color Eggshell => new("#F0EAD6");

        /// <summary>
        /// Represents the color Ivory (#FFFFF0).
        /// </summary>
        public static Color Ivory => new("#FFFFF0");

        /// <summary>
        /// Represents the color Linen (#E9DCC9).
        /// </summary>
        public static Color Linen => new("#E9DCC9");

        /// <summary>
        /// Represents the color Off White (#FAF9F6).
        /// </summary>
        public static Color OffWhite => new("#FAF9F6");

        /// <summary>
        /// Represents the color Parchment (#FCF5E5).
        /// </summary>
        public static Color Parchment => new("#FCF5E5");

        /// <summary>
        /// Represents the color Pearl (#E2DFD2).
        /// </summary>
        public static Color Pearl => new("#E2DFD2");

        /// <summary>
        /// Represents the color Vanilla (#F3E5AB).
        /// </summary>
        public static Color Vanilla => new("#F3E5AB");

        /// <summary>
        /// Represents the color White (#FFFFFF).
        /// </summary>
        public static Color White => new("#FFFFFF");

        /// <summary>
        /// Represents the color BrightYellow (#FFEA00).
        /// </summary>
        public static Color BrightYellow => new("#FFEA00");

        /// <summary>
        /// Represents the color CadmiumYellow (#FDDA0D).
        /// </summary>
        public static Color CadmiumYellow => new("#FDDA0D");

        /// <summary>
        /// Represents the color CanaryYellow (#FFFF8F).
        /// </summary>
        public static Color CanaryYellow => new("#FFFF8F");

        /// <summary>
        /// Represents the color DarkYellow (#8B8000).
        /// </summary>
        public static Color DarkYellow => new("#8B8000");

        /// <summary>
        /// Represents the color Flax (#EEDC82).
        /// </summary>
        public static Color Flax => new("#EEDC82");

        /// <summary>
        /// Represents the color Gold (#FFD700).
        /// </summary>
        public static Color Gold => new("#FFD700");

        /// <summary>
        /// Represents the color Icterine (#FCF55F).
        /// </summary>
        public static Color Icterine => new("#FCF55F");

        /// <summary>
        /// Represents the color Jasmine (#F8DE7E).
        /// </summary>
        public static Color Jasmine => new("#F8DE7E");

        /// <summary>
        /// Represents the color LemonYellow (#FAFA33).
        /// </summary>
        public static Color LemonYellow => new("#FAFA33");

        /// <summary>
        /// Represents the color Maize (#FBEC5D).
        /// </summary>
        public static Color Maize => new("#FBEC5D");

        /// <summary>
        /// Represents the color MustardYellow (#FFDB58).
        /// </summary>
        public static Color MustardYellow => new("#FFDB58");

        /// <summary>
        /// Represents the color NaplesYellow (#FADA5E).
        /// </summary>
        public static Color NaplesYellow => new("#FADA5E");

        /// <summary>
        /// Represents the color PastelYellow (#FFFAA0).
        /// </summary>
        public static Color PastelYellow => new("#FFFAA0");

        /// <summary>
        /// Represents the color Saffron (#F4C430).
        /// </summary>
        public static Color Saffron => new("#F4C430");

        /// <summary>
        /// Represents the color Yellow (#FFFF00).
        /// </summary>
        public static Color Yellow => new("#FFFF00");

        /// <summary>
        /// Fully transparent color (ARGB: 00-00-00-00).
        /// </summary>
        public static readonly Color Transparent = new("#00000000");

        /// <summary>
        /// 25% transparent black (ARGB: 40-00-00-00).
        /// </summary>
        public static readonly Color Black25Transparent = new("#40000000");

        /// <summary>
        /// 50% transparent black (ARGB: 80-00-00-00).
        /// </summary>
        public static readonly Color Black50Transparent = new("#80000000");

        /// <summary>
        /// 75% transparent black (ARGB: BF-00-00-00).
        /// </summary>
        public static readonly Color Black75Transparent = new("#BF000000");

        /// <summary>
        /// Fully transparent black (ARGB: 00-00-00-00).
        /// </summary>
        public static readonly Color TransparentBlack = new("#00000000");

        /// <summary>
        /// 25% transparent white (ARGB: 40-FF-FF-FF).
        /// </summary>
        public static readonly Color White25Transparent = new("#40FFFFFF");

        /// <summary>
        /// 50% transparent white (ARGB: 80-FF-FF-FF).
        /// </summary>
        public static readonly Color White50Transparent = new("#80FFFFFF");

        /// <summary>
        /// 75% transparent white (ARGB: BF-FF-FF-FF).
        /// </summary>
        public static readonly Color White75Transparent = new("#BFFFFFFF");

        /// <summary>
        /// Fully transparent white (ARGB: FF-FF-FF-00).
        /// </summary>
        public static readonly Color TransparentWhite = new("#FFFFFF00");

        /// <summary>
        /// Represents the color CornFlowerBlue (#6495ED).
        /// </summary>
        public static readonly Color CornFlowerBlue = new("#6495ED");
    }

    #endregion


    #region Engine
    internal readonly SFMLColor ToSFML() => new((byte)Red, (byte)Green, (byte)Blue, (byte)Alpha);
    #endregion
}
