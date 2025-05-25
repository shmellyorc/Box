namespace Box.Systems;

/// <summary>
/// Represents a 2D rectangle used for pixel detection, rectangle-to-rectangle collision detection, and other utility methods.
/// </summary>
public struct Rect2 : IEquatable<Rect2>
{
    private readonly StringBuilder _sb = new();

    private Vect2 _position = Vect2.Zero, _size = Vect2.Zero;
    private static Rect2 s_rectEmpty = new(0, 0, 0, 0);
    private static Rect2 s_rectOne = new(0, 0, 0, 0);

    /// <summary>
    /// The X-coordinate of the top-left corner of the rectangle.
    /// </summary>
    public float X;

    /// <summary>
    /// The Y-coordinate of the top-left corner of the rectangle.
    /// </summary>
    public float Y;

    /// <summary>
    /// The width of the rectangle.
    /// </summary>
    public float Width;

    /// <summary>
    /// The height of the rectangle.
    /// </summary>
    public float Height;

    /// <summary>
    /// Represents an empty rectangle.
    /// </summary>
    public static Rect2 Empty => s_rectEmpty;

    /// <summary>
    /// Represents a rectangle with dimensions of one by one.
    /// </summary>
    public static Rect2 One => s_rectOne;

    /// <summary>
    /// Represents the top side of the rectangle.
    /// </summary>
    public readonly float Top => Y;

    /// <summary>
    /// Represents the left side of the rectangle.
    /// </summary>
    public readonly float Left => X;

    /// <summary>
    /// Represents the right side of the rectangle.
    /// </summary>
    public readonly float Right => X + Width;

    /// <summary>
    /// Represents the bottom side of the rectangle.
    /// </summary>
    public readonly float Bottom => Y + Height;

    /// <summary>
    /// Determines if the rectangle is empty.
    /// </summary>
    public readonly bool IsEmpty => Width <= 0 || Height <= 0;

    /// <summary>
    /// Represents the center of the rectangle.
    /// </summary>
    public readonly Vect2 Center => new(X + Width / 2, Y + Height / 2);

    /// <summary>
    /// The position of the rectangle.
    /// </summary>
    public Vect2 Position
    {
        readonly get => _position;
        set
        {
            X = value.X;
            Y = value.Y;

            _position.X = value.X;
            _position.Y = value.Y;
        }
    }

    /// <summary>
    /// The size of the rectangle.
    /// </summary>
    public Vect2 Size
    {
        readonly get => _size;
        set
        {
            Width = value.X;
            Height = value.Y;

            _size.X = value.X;
            _size.Y = value.Y;
        }
    }

    /// <summary>
    /// Represents a 2D rectangle defined by its position and size.
    /// </summary>
    /// <param name="x">The X-coordinate of the top-left corner.</param>
    /// <param name="y">The Y-coordinate of the top-left corner.</param>
    /// <param name="width">The width of the rectangle.</param>
    /// <param name="height">The height of the rectangle.</param>
    public Rect2(float x, float y, float width, float height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;

        _position = new Vect2(x, y);
        _size = new Vect2(width, height);
    }

    /// <summary>
    /// Represents a 2D rectangle defined by its top-left corner position and dimensions.
    /// </summary>
    /// <param name="position">The top-left corner position of the rectangle.</param>
    /// <param name="size">The dimensions (width and height) of the rectangle.</param>
    public Rect2(Vect2 position, Vect2 size) : this(position.X, position.Y, size.X, size.Y) { }

    /// <summary>
    /// Represents a deep clone of a 2D rectangle defined by its top-left corner position and dimensions.
    /// </summary>
    /// <param name="value">The rectangle to deep clone.</param>
    public Rect2(Rect2 value) : this(value.X, value.Y, value.Width, value.Height) { }

    internal Rect2(SFMLRectI value) : this(value.Left, value.Top, value.Width, value.Height) { }

    internal Rect2(SFMLRectF value) : this(value.Left, value.Top, value.Width, value.Height) { }

	internal Rect2(SFMLVectF posiion, SFMLVectF size) : this(posiion.X, posiion.Y, size.X, size.Y) { }

    #region Operators
    /// <summary>
    /// Checks if two rectangles are equal.
    /// </summary>
    /// <param name="left">The left rectangle.</param>
    /// <param name="right">The right rectangle.</param>
    /// <returns>True if the rectangles are equal; otherwise, false.</returns>
    public static bool operator ==(Rect2 left, Rect2 right)
        => (left.X, left.Y, left.Width, left.Height) == (right.X, right.Y, right.Width, right.Height);

    /// <summary>
    /// Checks if two rectangles are not equal.
    /// </summary>
    /// <param name="left">The left rectangle.</param>
    /// <param name="right">The right rectangle.</param>
    /// <returns>True if the rectangles are not equal; otherwise, false.</returns>
    public static bool operator !=(Rect2 left, Rect2 right) => !(left == right);

    /// <summary>
    /// Expands the rectangles.
    /// </summary>
    /// <param name="left">The rectangle to expand.</param>
    /// <param name="right">The vector to add.</param>
    /// <returns>The expanded rectangle.</returns>
    public static Rect2 operator +(Rect2 left, Vect2 right) => Expand(left, right);

    /// <summary>
    /// Shrinks the rectangles.
    /// </summary>
    /// <param name="left">The rectangle to shrink.</param>
    /// <param name="right">The vector to subtract.</param>
    /// <returns>The shrunk rectangle.</returns>
    public static Rect2 operator -(Rect2 left, Vect2 right) => Expand(left, right);

    /// <summary>
    /// Determines whether the area of the left rectangle is greater than the area of the right rectangle.
    /// </summary>
    /// <param name="left">The left rectangle to compare.</param>
    /// <param name="right">The right rectangle to compare.</param>
    /// <returns>True if the area of the left rectangle is greater than the area of the right rectangle; otherwise, false.</returns>
    public static bool operator >(Rect2 left, Rect2 right) => left.Area() > right.Area();

    /// <summary>
    /// Determines whether the area of the left rectangle is less than the area of the right rectangle.
    /// </summary>
    /// <param name="left">The left rectangle to compare.</param>
    /// <param name="right">The right rectangle to compare.</param>
    /// <returns>True if the area of the left rectangle is less than the area of the right rectangle; otherwise, false.</returns>
    public static bool operator <(Rect2 left, Rect2 right) => left.Area() < right.Area();

    /// <summary>
    /// Determines whether the area of the left rectangle is greater than or equal to the area of the right rectangle.
    /// </summary>
    /// <param name="left">The left rectangle to compare.</param>
    /// <param name="right">The right rectangle to compare.</param>
    /// <returns>True if the area of the left rectangle is greater than or equal to the area of the right rectangle; otherwise, false.</returns>
    public static bool operator >=(Rect2 left, Rect2 right) => left.Area() >= right.Area();

    /// <summary>
    /// Determines whether the area of the left rectangle is less than or equal to the area of the right rectangle.
    /// </summary>
    /// <param name="left">The left rectangle to compare.</param>
    /// <param name="right">The right rectangle to compare.</param>
    /// <returns>True if the area of the left rectangle is less than or equal to the area of the right rectangle; otherwise, false.</returns>
    public static bool operator <=(Rect2 left, Rect2 right) => left.Area() <= right.Area();
    #endregion


    #region Area

    /// <summary>
    /// Calculates the area of the rectangle.
    /// </summary>
    /// <returns>The area of the rectangle.</returns>
    public readonly float Area() => Area(this);

    /// <summary>
    /// Calculates the area of a given rectangle.
    /// </summary>
    /// <param name="rectangle">The rectangle for which to calculate the area.</param>
    /// <returns>The area of the rectangle.</returns>
    public static float Area(Rect2 rectangle) => rectangle.Width * rectangle.Height;
    #endregion


    #region Expand
    /// <summary>
    /// Expands or shrinks a rectangle.
    /// </summary>
    /// <param name="width">Positive value to expand the width, negative to shrink.</param>
    /// <param name="height">Positive value to expand the height, negative to shrink.</param>
    /// <returns>The expanded or shrunk rectangle.</returns>
    public readonly Rect2 Expand(int width, int height) => Expand(this, width, height);

    /// <summary>
    /// Expands or shrinks a rectangle.
    /// </summary>
    /// <param name="width">Positive value to expand the width, negative to shrink.</param>
    /// <param name="height">Positive value to expand the height, negative to shrink.</param>
    /// <returns>The expanded or shrunk rectangle.</returns>
    public readonly Rect2 Expand(float width, float height) => Expand(this, width, height);

    /// <summary>
    /// Expands or shrinks a rectangle by adding or subtracting values to its dimensions.
    /// </summary>
    /// <param name="rect">The rectangle to expand or shrink.</param>
    /// <param name="value">The values to expand or shrink the rectangle by.</param>
    /// <returns>The expanded or shrunk rectangle.</returns>
    public static Rect2 Expand(Rect2 rect, Vect2 value) => Expand(rect, value.X, value.Y);

    /// <summary>
    /// Expands or shrinks a rectangle.
    /// </summary>
    /// <param name="rect">The rectangle to expand or shrink.</param>
    /// <param name="width">Positive value to expand the width, negative to shrink.</param>
    /// <param name="height">Positive value to expand the height, negative to shrink.</param>
    /// <returns>The expanded or shrunk rectangle.</returns>
    public static Rect2 Expand(Rect2 rect, float width, float height)
    {
        var newX = rect.X - width;
        var newY = rect.Y - height;
        var newWidth = rect.Width + width * 2;
        var newHeight = rect.Height + height * 2;

        return new Rect2(newX, newY, newWidth, newHeight);
    }
    #endregion


    #region Contains
    /// <summary>
    /// Checks if a point (x, y) is within the bounds of the rectangle.
    /// </summary>
    /// <param name="x">The x-coordinate of the point.</param>
    /// <param name="y">The y-coordinate of the point.</param>
    /// <returns>True if the point is inside the rectangle; otherwise, false.</returns>
    public readonly bool Contains(float x, float y) => Contains(this, x, y);

    /// <summary>
    /// Checks if a point defined by a vector is within the bounds of the rectangle.
    /// </summary>
    /// <param name="vector">The vector representing the point.</param>
    /// <returns>True if the point is inside the rectangle; otherwise, false.</returns>
    public readonly bool Contains(Vect2 vector) => Contains(this, vector);

    /// <summary>
    /// Checks if a point defined by a vector is within the bounds of the rectangle.
    /// </summary>
    /// <param name="rect">The rectangle to check.</param>
    /// <param name="vector">The vector representing the point.</param>
    /// <returns>True if the point is inside the rectangle; otherwise, false.</returns>
    public static bool Contains(Rect2 rect, Vect2 vector) => Contains(rect, vector.X, vector.Y);

    /// <summary>
    /// Checks if a point (x, y) is within the bounds of the rectangle.
    /// </summary>
    /// <param name="rect">The rectangle to check.</param>
    /// <param name="x">The x-coordinate of the point.</param>
    /// <param name="y">The y-coordinate of the point.</param>
    /// <returns>True if the point is inside the rectangle; otherwise, false.</returns>
    public static bool Contains(Rect2 rect, float x, float y)
    {
        var v1 = rect.Left <= x;
        var v2 = x < rect.Right;
        var v3 = rect.Top <= y;
        var v4 = y < rect.Bottom;

        return v1 && v2 && v3 && v4;
    }
    #endregion


    #region Intersects
    /// <summary>
    /// Checks if this rectangle intersects with another rectangle.
    /// </summary>
    /// <param name="other">The other rectangle to check against.</param>
    /// <returns>True if this rectangle intersects with the other rectangle; otherwise, false.</returns>
    public readonly bool Intersects(Rect2 other) => Intersects(this, other);

    /// <summary>
    /// Checks if two rectangles intersect.
    /// </summary>
    /// <param name="left">The first rectangle to check.</param>
    /// <param name="right">The second rectangle to check.</param>
    /// <returns>True if the rectangles intersect; otherwise, false.</returns>
    public static bool Intersects(Rect2 left, Rect2 right)
    {
        return !(left.Right < right.Left ||
                right.Right < left.Left ||
                left.Bottom < right.Top ||
                right.Bottom < left.Top);
    }
    #endregion


    #region Merge
    /// <summary>
    /// Merges this rectangle with another rectangle into a single larger rectangle that encompasses both.
    /// </summary>
    /// <param name="other">The other rectangle to merge with this one.</param>
    /// <returns>A new rectangle that represents the merged area of this rectangle and the input rectangle.</returns>
    public readonly Rect2 Merge(Rect2 other) => Merge(this, other);


    /// <summary>
    /// Merges two rectangles into a single larger rectangle that encompasses both.
    /// </summary>
    /// <param name="left">The first rectangle to merge.</param>
    /// <param name="right">The second rectangle to merge.</param>
    /// <returns>A new rectangle that represents the merged area of the input rectangles.</returns>
    public static Rect2 Merge(Rect2 left, Rect2 right)
    {
        float minX = MathF.Min(left.X, right.X);
        float minY = MathF.Min(left.Y, right.Y);
        float maxX = MathF.Max(left.X + left.Width, right.X + right.Width);
        float maxY = MathF.Max(left.Y + left.Height, right.Y + right.Height);

        return new Rect2(minX, minY, maxX - minX, maxY - minY);
    }
    #endregion


    #region IEquatable
    /// <summary>
    /// Determines whether this rectangle is equal to another rectangle.
    /// </summary>
    /// <param name="other">The rectangle to compare with this rectangle.</param>
    /// <returns>True if the rectangles are equal; otherwise, false.</returns>
    public readonly bool Equals(Rect2 other)
        => (X, Y, Width, Height) == (other.X, other.Y, other.Width, other.Height);

    /// <summary>
    /// Determines whether this rectangle is equal to another object.
    /// </summary>
    /// <param name="obj">The object to compare with this rectangle.</param>
    /// <returns>True if the object is a Rect2 and is equal to this rectangle; otherwise, false.</returns>
    public readonly override bool Equals([NotNullWhen(true)] object obj)
    {
        if (obj is Rect2 value)
            return Equals(value);

        return false;
    }

    /// <summary>
    /// Returns a hash code for this rectangle.
    /// </summary>
    /// <returns>A hash code for this rectangle.</returns>
    public readonly override int GetHashCode() => HashCode.Combine(X, Y, Width, Height);

    /// <summary>
    /// Returns a string representation of this rectangle.
    /// </summary>
    /// <returns>A string representation of this rectangle.</returns>
    public readonly override string ToString()
    {
        if (_sb.Length > 0)
            _sb.Clear();

        _sb.Append("Rect2 [");
        _sb.Append($"X={X}, ");
        _sb.Append($"Y={Y}, ");
        _sb.Append($"Width={Width}, ");
        _sb.Append($"Height={Height}");
        _sb.Append(']');

        return _sb.ToString();
    }
    #endregion


    #region Engine
    internal readonly SFMLRectF ToSFMLF() => new(X, Y, Width, Height);
    internal readonly SFMLRectI ToSFMLI() => new((int)X, (int)Y, (int)Width, (int)Height);
    #endregion
}
