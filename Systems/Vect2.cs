namespace Box.Systems;

/// <summary>
/// Represents a 2D vector with X and Y components.
/// </summary>
public struct Vect2 : IEquatable<Vect2>, IComparable<Vect2>
{
	private readonly StringBuilder _sb = new();

	private static Vect2 s_vectZero = new(0);
	private static Vect2 s_vectOne = new(1);
	private static Vect2 s_vectUp = new(0, -1);
	private static Vect2 s_vectRight = new(1, 0);
	private static Vect2 s_vectDown = new(0, 1);
	private static Vect2 s_vectLeft = new(-1, 0);

	/// <summary>
	/// Gets or sets the X component of the vector.
	/// </summary>
	public float X;

	/// <summary>
	/// Gets or sets the Y component of the vector.
	/// </summary>
	public float Y;

	/// <summary>
	/// Vector with components (0, 0).
	/// </summary>
	public static Vect2 Zero => s_vectZero;

	/// <summary>
	/// Vector with components (1, 1).
	/// </summary>
	public static Vect2 One => s_vectOne;

	/// <summary>
	/// Vector representing up direction (0, 1).
	/// </summary>
	public static Vect2 Up => s_vectUp;

	/// <summary>
	/// Vector representing right direction (1, 0).
	/// </summary>
	public static Vect2 Right => s_vectRight;

	/// <summary>
	/// Vector representing down direction (0, -1).
	/// </summary>
	public static Vect2 Down => s_vectDown;

	/// <summary>
	/// Vector representing left direction (-1, 0).
	/// </summary>
	public static Vect2 Left => s_vectLeft;

	/// <summary>
	/// Vector is empty.
	/// </summary>
	public readonly bool IsZero => X == 0 && Y == 0;

	/// <summary>
	/// Constructs a new Vect2 with the given X and Y components.
	/// </summary>
	/// <param name="x">The X component of the vector.</param>
	/// <param name="y">The Y component of the vector.</param>
	public Vect2(float x, float y)
	{
		X = x;
		Y = y;
	}

	/// <summary>
	/// Constructs a new Vect2 with both components set to the same value.
	/// </summary>
	/// <param name="value">The value to set both X and Y components.</param>
	public Vect2(float value) : this(value, value) { }

	/// <summary>
	/// Constructs a new Vect2 by copying another Vect2.
	/// </summary>
	/// <param name="value">The Vect2 to copy.</param>
	public Vect2(Vect2 value) : this(value.X, value.Y) { }

	internal Vect2(SFMLVectorF value) : this(value.X, value.Y) { }

	internal Vect2(SFMLVectorI value) : this(value.X, value.Y) { }

	internal Vect2(SFMLVectorU value) : this(value.X, value.Y) { }




	#region Operator: ==, !=
	/// <summary>
	/// Overloaded equality operator for comparing two Vect2 objects.
	/// </summary>
	/// <param name="left">The left-hand side Vect2 object.</param>
	/// <param name="right">The right-hand side Vect2 object.</param>
	/// <returns>True if the two Vect2 objects are equal; otherwise, false.</returns>
	public static bool operator ==(Vect2 left, Vect2 right)
		=> (left.X, left.Y) == (right.X, right.Y);

	/// <summary>
	/// Overloaded inequality operator for comparing two Vect2 objects.
	/// </summary>
	/// <param name="left">The left-hand side Vect2 object.</param>
	/// <param name="right">The right-hand side Vect2 object.</param>
	/// <returns>True if the two Vect2 objects are not equal; otherwise, false.</returns>
	public static bool operator !=(Vect2 left, Vect2 right) => !(left == right);

	#endregion


	#region Operator: -

	/// <summary>
	/// Unary negation operator for negating a Vect2 object.
	/// </summary>
	/// <param name="left">The Vect2 object to negate.</param>
	/// <returns>The negated Vect2 object.</returns>
	public static Vect2 operator -(Vect2 left)
	{
		left.X = -left.X;
		left.Y = -left.Y;

		return left;
	}

	/// <summary>
	/// Subtracts one Vect2 object from another Vect2 object.
	/// </summary>
	/// <param name="left">The Vect2 object from which to subtract.</param>
	/// <param name="right">The Vect2 object to subtract.</param>
	/// <returns>A new Vect2 object that is the result of subtracting <paramref name="right"/> from <paramref name="left"/>.</returns>
	public static Vect2 operator -(Vect2 left, Vect2 right)
	{
		left.X -= right.X;
		left.Y -= right.Y;

		return left;
	}

	/// <summary>
	/// Subtracts a scalar value from each component of a Vect2 object.
	/// </summary>
	/// <param name="left">The Vect2 object.</param>
	/// <param name="right">The scalar value to subtract.</param>
	/// <returns>A new Vect2 object where each component is subtracted by <paramref name="right"/>.</returns>
	public static Vect2 operator -(Vect2 left, float right)
	{
		left.X -= right;
		left.Y -= right;

		return left;
	}

	/// <summary>
	/// Subtracts each component of a Vect2 object from a scalar value.
	/// </summary>
	/// <param name="left">The scalar value.</param>
	/// <param name="right">The Vect2 object to subtract.</param>
	/// <returns>A new Vect2 object where each component is subtracted from <paramref name="left"/>.</returns>
	public static Vect2 operator -(float left, Vect2 right)
	{
		right.X -= left;
		right.Y -= left;

		return right;
	}

	#endregion


	#region Operator: +

	/// <summary>
	/// Adds two Vect2 objects together.
	/// </summary>
	/// <param name="left">The first Vect2 object.</param>
	/// <param name="right">The second Vect2 object.</param>
	/// <returns>A new Vect2 object that is the sum of <paramref name="left"/> and <paramref name="right"/>.</returns>
	public static Vect2 operator +(Vect2 left, Vect2 right)
	{
		left.X += right.X;
		left.Y += right.Y;

		return left;
	}

	/// <summary>
	/// Adds a scalar value to each component of a Vect2 object.
	/// </summary>
	/// <param name="left">The Vect2 object.</param>
	/// <param name="right">The scalar value to add.</param>
	/// <returns>A new Vect2 object where each component is added by <paramref name="right"/>.</returns>
	public static Vect2 operator +(Vect2 left, float right)
	{
		left.X += right;
		left.Y += right;

		return left;
	}

	/// <summary>
	/// Adds a Vect2 object to a scalar value.
	/// </summary>
	/// <param name="left">The scalar value.</param>
	/// <param name="right">The Vect2 object.</param>
	/// <returns>A new Vect2 object that is the sum of <paramref name="left"/> and <paramref name="right"/>.</returns>
	public static Vect2 operator +(float left, Vect2 right)
	{
		right.X += left;
		right.Y += left;

		return right;
	}

	#endregion


	#region Operator: /

	/// <summary>
	/// Divides one Vect2 object by another Vect2 object component-wise.
	/// </summary>
	/// <param name="left">The Vect2 object dividend.</param>
	/// <param name="right">The Vect2 object divisor.</param>
	/// <returns>A new Vect2 object that is the result of dividing each component of <paramref name="left"/> by the corresponding component of <paramref name="right"/>.</returns>
	public static Vect2 operator /(Vect2 left, Vect2 right)
	{
		left.X /= right.X;
		left.Y /= right.Y;

		return left;
	}

	/// <summary>
	/// Divides each component of a Vect2 object by a scalar value.
	/// </summary>
	/// <param name="left">The Vect2 object dividend.</param>
	/// <param name="right">The scalar value divisor.</param>
	/// <returns>A new Vect2 object where each component is divided by <paramref name="right"/>.</returns>
	public static Vect2 operator /(Vect2 left, float right)
	{
		left.X /= right;
		left.Y /= right;

		return left;
	}

	/// <summary>
	/// Divides a scalar value by each component of a Vect2 object.
	/// </summary>
	/// <param name="left">The scalar value dividend.</param>
	/// <param name="right">The Vect2 object divisor.</param>
	/// <returns>A new Vect2 object where each component is the result of dividing <paramref name="left"/> by the corresponding component of <paramref name="right"/>.</returns>

	public static Vect2 operator /(float left, Vect2 right)
	{
		right.X /= left;
		right.Y /= left;

		return right;
	}

	#endregion


	#region Operator: *

	/// <summary>
	/// Multiplies two Vect2 objects component-wise.
	/// </summary>
	/// <param name="left">The first Vect2 object.</param>
	/// <param name="right">The second Vect2 object.</param>
	/// <returns>A new Vect2 object that is the result of multiplying each component of <paramref name="left"/> by the corresponding component of <paramref name="right"/>.</returns>
	public static Vect2 operator *(Vect2 left, Vect2 right)
	{
		left.X *= right.X;
		left.Y *= right.Y;

		return left;
	}

	/// <summary>
	/// Multiplies each component of a Vect2 object by a scalar value.
	/// </summary>
	/// <param name="left">The Vect2 object.</param>
	/// <param name="right">The scalar value to multiply.</param>
	/// <returns>A new Vect2 object where each component is multiplied by <paramref name="right"/>.</returns>
	public static Vect2 operator *(Vect2 left, float right)
	{
		left.X *= right;
		left.Y *= right;

		return left;
	}

	/// <summary>
	/// Multiplies a scalar value by each component of a Vect2 object.
	/// </summary>
	/// <param name="left">The scalar value to multiply.</param>
	/// <param name="right">The Vect2 object.</param>
	/// <returns>A new Vect2 object where each component is multiplied by <paramref name="left"/>.</returns>
	public static Vect2 operator *(float left, Vect2 right)
	{
		right.X *= left;
		right.Y *= left;

		return right;
	}

	#endregion


	#region Operator: <, >

	/// <summary>
	/// Determines whether the first Vect2 object is less than the second Vect2 object.
	/// </summary>
	/// <param name="left">The first Vect2 object to compare.</param>
	/// <param name="right">The second Vect2 object to compare.</param>
	/// <returns>True if <paramref name="left"/> is less than <paramref name="right"/>; otherwise, false.</returns>
	public static bool operator <(Vect2 left, Vect2 right)
	{
		var x = left.X < right.X;
		var y = left.Y < right.Y;

		return x && y;
	}

	/// <summary>
	/// Determines whether the first Vect2 object is greater than the second Vect2 object.
	/// </summary>
	/// <param name="left">The first Vect2 object to compare.</param>
	/// <param name="right">The second Vect2 object to compare.</param>
	/// <returns>True if <paramref name="left"/> is greater than <paramref name="right"/>; otherwise, false.</returns>
	public static bool operator >(Vect2 left, Vect2 right)
	{
		var x = left.X > right.X;
		var y = left.Y > right.Y;

		return x && y;
	}

	#endregion


	#region Operator <=, >=

	/// <summary>
	/// Determines whether the first Vect2 object is less than or equal to the second Vect2 object.
	/// </summary>
	/// <param name="left">The first Vect2 object to compare.</param>
	/// <param name="right">The second Vect2 object to compare.</param>
	/// <returns>True if <paramref name="left"/> is less than or equal to <paramref name="right"/>; otherwise, false.</returns>
	public static bool operator <=(Vect2 left, Vect2 right)
	{
		var x = left.X <= right.X;
		var y = left.Y <= right.Y;

		return x && y;
	}

	/// <summary>
	/// Determines whether the first Vect2 object is greater than or equal to the second Vect2 object.
	/// </summary>
	/// <param name="left">The first Vect2 object to compare.</param>
	/// <param name="right">The second Vect2 object to compare.</param>
	/// <returns>True if <paramref name="left"/> is greater than or equal to <paramref name="right"/>; otherwise, false.</returns>
	public static bool operator >=(Vect2 left, Vect2 right)
	{
		var x = left.X >= right.X;
		var y = left.Y >= right.Y;

		return x && y;
	}

	#endregion


	#region Add

	/// <summary>
	/// Adds another Vect2 to this Vect2 instance.
	/// </summary>
	/// <param name="other">The Vect2 to add to this instance.</param>
	/// <returns>A new Vect2 that is the sum of this instance and <paramref name="other"/>.</returns>
	public readonly Vect2 Add(Vect2 other) => Add(this, other);

	/// <summary>
	/// Adds two Vect2 instances together.
	/// </summary>
	/// <param name="left">The first Vect2 instance.</param>
	/// <param name="right">The second Vect2 instance.</param>
	/// <returns>A new Vect2 that is the sum of <paramref name="left"/> and <paramref name="right"/>.</returns>
	public static Vect2 Add(Vect2 left, Vect2 right) => new(left.X + right.X, left.Y + right.Y);

	#endregion


	#region Rotate
	public readonly Vect2 Rotate(float radians) => Rotate(this, radians);
	public static Vect2 Rotate(Vect2 value, float radians)
	{
		var cos = MathF.Cos(radians);
		var sin = MathF.Sin(radians);

		return new Vect2(
			value.X * cos - value.Y * sin,
			value.X * sin + value.Y * cos
		);
	}
	#endregion


	#region Subtract

	/// <summary>
	/// Subtracts another Vect2 from this Vect2 instance.
	/// </summary>
	/// <param name="other">The Vect2 to subtract from this instance.</param>
	/// <returns>A new Vect2 that is the result of subtracting <paramref name="other"/> from this instance.</returns>
	public readonly Vect2 Subtract(Vect2 other) => Subtract(this, other);

	/// <summary>
	/// Subtracts one Vect2 instance from another Vect2 instance.
	/// </summary>
	/// <param name="left">The Vect2 instance from which to subtract.</param>
	/// <param name="right">The Vect2 instance to subtract.</param>
	/// <returns>A new Vect2 that is the result of subtracting <paramref name="right"/> from <paramref name="left"/>.</returns>
	public static Vect2 Subtract(Vect2 left, Vect2 right) => new(left.X - right.X, left.Y - right.Y);

	#endregion


	#region Multiply

	/// <summary>
	/// Multiplies this Vect2 instance by a scalar value.
	/// </summary>
	/// <param name="scaler">The scalar value to multiply this Vect2 instance.</param>
	/// <returns>A new Vect2 that is the result of multiplying this instance by <paramref name="scaler"/>.</returns>
	public readonly Vect2 Multiply(float scaler) => Multiply(this, scaler);

	/// <summary>
	/// Multiplies a Vect2 instance by a scalar value.
	/// </summary>
	/// <param name="vector">The Vect2 instance to multiply.</param>
	/// <param name="scalar">The scalar value to multiply by.</param>
	/// <returns>A new Vect2 that is the result of multiplying <paramref name="vector"/> by <paramref name="scalar"/>.</returns>
	public static Vect2 Multiply(Vect2 vector, float scalar) => new(vector.X * scalar, vector.Y * scalar);

	#endregion


	#region Divide

	/// <summary>
	/// Divides this Vect2 instance by a scalar value.
	/// </summary>
	/// <param name="scalar">The scalar value to divide this Vect2 instance.</param>
	/// <returns>A new Vect2 that is the result of dividing this instance by <paramref name="scalar"/>.</returns>
	public readonly Vect2 Divide(float scalar) => Divide(this, scalar);

	/// <summary>
	/// Divides a Vect2 instance by a scalar value.
	/// </summary>
	/// <param name="vector">The Vect2 instance to divide.</param>
	/// <param name="scalar">The scalar value to divide by.</param>
	/// <returns>A new Vect2 that is the result of dividing <paramref name="vector"/> by <paramref name="scalar"/>.</returns>
	public static Vect2 Divide(Vect2 vector, float scalar) => new(vector.X / scalar, vector.Y / scalar);

	#endregion


	#region Center

	/// <summary>
	/// Calculates the center point between this Vect2 instance and another Vect2, optionally rounding the result.
	/// </summary>
	/// <param name="other">The other Vect2 instance to calculate the center with.</param>
	/// <param name="rounded">True to round the resulting center point; false to keep it exact.</param>
	/// <returns>A new Vect2 that represents the center point between this instance and <paramref name="other"/>.</returns>
	public readonly Vect2 Center(Vect2 other, bool rounded) => Center(this, other, rounded);

	/// <summary>
	/// Calculates the center point between two Vect2 instances, optionally rounding the result.
	/// </summary>
	/// <param name="left">The first Vect2 instance.</param>
	/// <param name="right">The second Vect2 instance.</param>
	/// <param name="rounded">True to round the resulting center point; false to keep it exact. Default is true.</param>
	/// <returns>A new Vect2 that represents the center point between <paramref name="left"/> and <paramref name="right"/>.</returns>
	public static Vect2 Center(Vect2 left, Vect2 right, bool rounded)
	{
		return rounded
			? Round((left - right) / 2)
			: (left - right) / 2;
	}

	/// <summary>
	/// Calculates the center point between two float values, optionally rounding the result.
	/// </summary>
	/// <param name="left">The first float value.</param>
	/// <param name="right">The second float value.</param>
	/// <param name="rounded">True to round the resulting center point; false to keep it exact. Default is true.</param>
	/// <returns>The center point between <paramref name="left"/> and <paramref name="right"/>.</returns>
	public static float Center(float left, float right, bool rounded)
	{
		return rounded
			? MathF.Round((left - right) / 2)
			: (left - right) / 2;
	}
	#endregion


	#region Max
	/// <summary>
	/// Returns a new Vect2 that contains the maximum values from this Vect2 instance and another Vect2.
	/// </summary>
	/// <param name="other">The other Vect2 instance to compare with.</param>
	/// <returns>A new Vect2 where each component is the maximum of the corresponding components from this instance and <paramref name="other"/>.</returns>
	public readonly Vect2 Max(Vect2 other) => Max(this, other);

	/// <summary>
	/// Returns a new Vect2 that contains the maximum values from two Vect2 instances.
	/// </summary>
	/// <param name="left">The first Vect2 instance to compare.</param>
	/// <param name="right">The second Vect2 instance to compare.</param>
	/// <returns>A new Vect2 where each component is the maximum of the corresponding components from <paramref name="left"/> and <paramref name="right"/>.</returns>
	public static Vect2 Max(Vect2 left, Vect2 right)
	{
		var x = MathF.Max(left.X, right.X);
		var y = MathF.Max(left.Y, right.Y);

		return new Vect2(x, y);
	}
	#endregion


	#region Min
	/// <summary>
	/// Returns a new Vect2 that contains the minimum values from this Vect2 instance and another Vect2.
	/// </summary>
	/// <param name="other">The other Vect2 instance to compare with.</param>
	/// <returns>A new Vect2 where each component is the minimum of the corresponding components from this instance and <paramref name="other"/>.</returns>
	public readonly Vect2 Min(Vect2 other) => Min(this, other);

	/// <summary>
	/// Returns a new Vect2 that contains the minimum values from two Vect2 instances.
	/// </summary>
	/// <param name="left">The first Vect2 instance to compare.</param>
	/// <param name="right">The second Vect2 instance to compare.</param>
	/// <returns>A new Vect2 where each component is the minimum of the corresponding components from <paramref name="left"/> and <paramref name="right"/>.</returns>
	public static Vect2 Min(Vect2 left, Vect2 right)
	{
		var x = MathF.Min(left.X, right.X);
		var y = MathF.Min(left.Y, right.Y);

		return new Vect2(x, y);
	}
	#endregion


	#region Abs
	/// <summary>
	/// Returns a new Vect2 that contains the absolute values of this Vect2 instance.
	/// </summary>
	/// <returns>A new Vect2 where each component is the absolute value of the corresponding component in this instance.</returns>
	public readonly Vect2 Abs() => Abs(this);

	/// <summary>
	/// Returns a new Vect2 that contains the absolute values of a Vect2 instance.
	/// </summary>
	/// <param name="value">The Vect2 instance to compute absolute values for.</param>
	/// <returns>A new Vect2 where each component is the absolute value of the corresponding component in <paramref name="value"/>.</returns>
	public static Vect2 Abs(Vect2 value)
	{
		value.X = MathF.Abs(value.X);
		value.Y = MathF.Abs(value.Y);

		return value;
	}
	#endregion


	#region Floor
	/// <summary>
	/// Returns a new Vect2 that contains the floor values of this Vect2 instance.
	/// </summary>
	/// <returns>A new Vect2 where each component is the floor value of the corresponding component in this instance.</returns>
	public readonly Vect2 Floor() => Floor(this);

	/// <summary>
	/// Returns a new Vect2 that contains the floor values of a Vect2 instance.
	/// </summary>
	/// <param name="value">The Vect2 instance to compute floor values for.</param>
	/// <returns>A new Vect2 where each component is the floor value of the corresponding component in <paramref name="value"/>.</returns>
	public static Vect2 Floor(Vect2 value)
	{
		value.X = MathF.Floor(value.X);
		value.Y = MathF.Floor(value.Y);

		return value;
	}
	#endregion


	#region Ceiling
	/// <summary>
	/// Returns a new Vect2 that contains the ceiling values of this Vect2 instance.
	/// </summary>
	/// <returns>A new Vect2 where each component is the ceiling value of the corresponding component in this instance.</returns>
	public readonly Vect2 Ceiling() => Ceiling(this);

	/// <summary>
	/// Returns a new Vect2 that contains the ceiling values of a Vect2 instance.
	/// </summary>
	/// <param name="value">The Vect2 instance to compute ceiling values for.</param>
	/// <returns>A new Vect2 where each component is the ceiling value of the corresponding component in <paramref name="value"/>.</returns>
	public static Vect2 Ceiling(Vect2 value)
	{
		value.X = MathF.Ceiling(value.X);
		value.Y = MathF.Ceiling(value.Y);

		return value;
	}
	#endregion


	#region Clamp
	/// <summary>
	/// Returns a new Vect2 that is clamped between a minimum and maximum Vect2.
	/// </summary>
	/// <param name="min">The minimum Vect2 to clamp against.</param>
	/// <param name="max">The maximum Vect2 to clamp against.</param>
	/// <returns>A new Vect2 that is clamped between <paramref name="min"/> and <paramref name="max"/>.</returns>
	public readonly Vect2 Clamp(Vect2 min, Vect2 max) => Clamp(this, min, max);

	/// <summary>
	/// Returns a new Vect2 that is clamped between specified minimum and maximum Vect2 values.
	/// </summary>
	/// <param name="value">The Vect2 instance to clamp.</param>
	/// <param name="min">The minimum Vect2 to clamp against.</param>
	/// <param name="max">The maximum Vect2 to clamp against.</param>
	/// <returns>A new Vect2 that is clamped between <paramref name="min"/> and <paramref name="max"/>.</returns>
	public static Vect2 Clamp(Vect2 value, Vect2 min, Vect2 max)
	{
		value.X = Math.Clamp(value.X, min.X, max.X);
		value.Y = Math.Clamp(value.Y, min.Y, max.Y);

		return value;
	}
	#endregion


	#region Direction
	/// <summary>
	/// Computes the direction vector from this Vect2 instance to another Vect2.
	/// </summary>
	/// <param name="other">The target Vect2 to compute the direction towards.</param>
	/// <param name="normalized">True to return a normalized direction vector; false to return the raw direction vector. Default is true.</param>
	/// <returns>The direction Vect2 from this instance to <paramref name="other"/>.</returns>
	public readonly Vect2 Direction(Vect2 other, bool normalized) => Direction(this, other, normalized);

	/// <summary>
	/// Computes the direction vector from one Vect2 instance to another.
	/// </summary>
	/// <param name="left">The starting Vect2 instance.</param>
	/// <param name="right">The target Vect2 instance.</param>
	/// <param name="normalized">True to return a normalized direction vector; false to return the raw direction vector. Default is true.</param>
	/// <returns>The direction Vect2 from <paramref name="left"/> to <paramref name="right"/>.</returns>
	public static Vect2 Direction(Vect2 left, Vect2 right, bool normalized)
	{
		if (normalized)
			return Normalized(right - left);

		return right - left;
	}
	#endregion


	#region Normalized
	/// <summary>
	/// Returns a new Vect2 that is the normalized version of this Vect2 instance.
	/// </summary>
	/// <returns>A new Vect2 that is the normalized version of this instance.</returns>
	public readonly Vect2 Normalized() => Normalized(this);

	/// <summary>
	/// Calculates the normalized (unit) vector from the specified components.
	/// </summary>
	/// <param name="x">The x-component of the vector.</param>
	/// <param name="y">The y-component of the vector.</param>
	/// <returns>The normalized vector as a <see cref="Vect2"/>.</returns>
	public static Vect2 Normalized(float x, float y)
	{
		var length = Length(x, y);

		if (length == 0)
			return Zero;

		var inverseLength = 1.0f / length;

		return new Vect2(x * inverseLength, y * inverseLength);
	}

	/// <summary>
	/// Returns a new Vect2 that is the normalized version of a Vect2 instance.
	/// </summary>
	/// <param name="value">The Vect2 instance to normalize.</param>
	/// <returns>A new Vect2 that is the normalized version of <paramref name="value"/>.</returns>
	public static Vect2 Normalized(Vect2 value) => Normalized(value.X, value.Y);
	#endregion


	#region Lerp
	/// <summary>
	/// Performs a linear interpolation between two Vect2 instances.
	/// </summary>
	/// <param name="left">The starting Vect2.</param>
	/// <param name="right">The ending Vect2.</param>
	/// <param name="amount">The interpolation amount. Should be between 0 and 1.</param>
	/// <returns>The interpolated Vect2.</returns>
	public static float Lerp(float left, float right, float amount)
		=> left + (right - left) * amount;

	/// <summary>
	/// Performs a linear interpolation between two float values.
	/// </summary>
	/// <param name="left">The starting value.</param>
	/// <param name="right">The ending value.</param>
	/// <param name="amount">The interpolation amount. Should be between 0 and 1.</param>
	/// <returns>The interpolated float value.</returns>
	public static Vect2 Lerp(Vect2 left, Vect2 right, float amount)
		=> left + (right - left) * amount;

	/// <summary>
	/// Performs a more precise linear interpolation between two Vect2 instances.
	/// </summary>
	/// <param name="left">The starting Vect2.</param>
	/// <param name="right">The ending Vect2.</param>
	/// <param name="amount">The interpolation amount. Should be between 0 and 1.</param>
	/// <returns>The interpolated Vect2.</returns>
	public static float LerpPercise(float left, float right, float amount)
		=> (1 - amount) * left + right * amount;

	/// <summary>
	/// Performs a more precise linear interpolation between two float values.
	/// </summary>
	/// <param name="left">The starting value.</param>
	/// <param name="right">The ending value.</param>
	/// <param name="amount">The interpolation amount. Should be between 0 and 1.</param>
	/// <returns>The interpolated float value.</returns>
	public static Vect2 LerpPercise(Vect2 left, Vect2 right, float amount)
		=> (1 - amount) * left + right * amount;
	#endregion


	#region Distance
	/// <summary>
	/// Computes the Euclidean distance between this Vect2 instance and another Vect2.
	/// </summary>
	/// <param name="other">The other Vect2 to compute the distance to.</param>
	/// <returns>The Euclidean distance between this Vect2 and <paramref name="other"/>.</returns>
	public readonly float Distance(Vect2 other) => Distance(this, other);

	/// <summary>
	/// Computes the absolute difference between two float values.
	/// </summary>
	/// <param name="left">The first float value.</param>
	/// <param name="right">The second float value.</param>
	/// <returns>The absolute difference between <paramref name="left"/> and <paramref name="right"/>.</returns>
	public static float Distance(float left, float right) => MathF.Abs(left - right);

	/// <summary>
	/// Computes the Euclidean distance between two Vect2 instances.
	/// </summary>
	/// <param name="left">The first Vect2 instance.</param>
	/// <param name="right">The second Vect2 instance.</param>
	/// <returns>The Euclidean distance between <paramref name="left"/> and <paramref name="right"/>.</returns>
	public static float Distance(Vect2 left, Vect2 right)
	{
		var x = left.X - right.X;
		var y = left.Y - right.Y;

		return Length(x, y);
	}
	#endregion


	#region MoveTowards
	/// <summary>
	/// Moves this Vect2 instance towards another Vect2 instance by a specified delta amount for each component.
	/// </summary>
	/// <param name="other">The target Vect2 instance to move towards.</param>
	/// <param name="delta">The maximum amount to move towards the target for each component.</param>
	/// <returns>A new Vect2 instance moved towards the target by the delta amount for each component.</returns>
	public readonly Vect2 MoveTowards(Vect2 other, float delta) => MoveTowards(this, other, delta);

	/// <summary>
	/// Moves a Vect2 instance towards a target Vect2 instance by a specified delta amount for each component.
	/// </summary>
	/// <param name="current">The current Vect2 instance to be moved.</param>
	/// <param name="target">The target Vect2 instance to move towards.</param>
	/// <param name="delta">The maximum amount to move towards the target for each component.</param>
	/// <returns>A new Vect2 instance moved towards the target by the delta amount for each component.</returns>
	public static Vect2 MoveTowards(Vect2 current, Vect2 target, float delta)
	{
		if (delta <= 0f)
			return current;

		var direction = target - current;
		float sqrDist = DistanceSquared(current, target);
		float deltaSqr = delta * delta;

		// If already at—or within one step of—the target, snap to it
		if (sqrDist == 0f || sqrDist <= deltaSqr)
			return target;

		// Compute the true distance once
		float dist = MathF.Sqrt(sqrDist);
		return current + (direction / dist) * delta;
	}

	/// <summary>
	/// Moves current value towards the target value by a specified delta amount.
	/// </summary>
	/// <param name="current">The current value to be moved.</param>
	/// <param name="target">The target value to move towards.</param>
	/// <param name="delta">The maximum amount to move towards the target.</param>
	/// <returns>The new value moved towards the target by the delta amount.</returns>
	public static float MoveTowards(float current, float target, float delta)
	{
		if (delta <= 0f)
			return current;

		float diff = target - current;

		if (MathF.Abs(diff) <= delta)
			return target;

		return current + MathF.Sign(diff) * delta;
	}
	#endregion


	#region DistanceSquared
	/// <summary>
	/// Calculates the squared Euclidean distance between this Vect2 instance and another Vect2 instance.
	/// </summary>
	/// <param name="other">The other Vect2 instance to calculate the distance to.</param>
	/// <returns>The squared Euclidean distance between this Vect2 and <paramref name="other"/>.</returns>
	public readonly float DistanceSquared(Vect2 other) => DistanceSquared(this, other);

	/// <summary>
	/// Calculates the squared Euclidean distance between two Vect2 instances.
	/// </summary>
	/// <param name="left">The first Vect2 instance.</param>
	/// <param name="right">The second Vect2 instance.</param>
	/// <returns>The squared Euclidean distance between <paramref name="left"/> and <paramref name="right"/>.</returns>
	public static float DistanceSquared(Vect2 left, Vect2 right)
	{
		var x = left.X - right.X;
		var y = left.Y - right.Y;

		return LengthSquared(x, y);
	}
	#endregion


	#region Length
	/// <summary>
	/// Calculates the Euclidean length (magnitude) of this Vect2 instance.
	/// </summary>
	/// <returns>The Euclidean length (magnitude) of this Vect2.</returns>
	public readonly float Length() => Length(this);

	/// <summary>
	/// Calculates the Euclidean length (magnitude) of a vector defined by its x and y components.
	/// </summary>
	/// <param name="x">The x-component of the vector.</param>
	/// <param name="y">The y-component of the vector.</param>
	/// <returns>The Euclidean length (magnitude) of the vector.</returns>
	public static float Length(float x, float y) => MathF.Sqrt(x * x + y * y);

	/// <summary>
	/// Calculates the Euclidean length (magnitude) of a Vect2 instance.
	/// </summary>
	/// <param name="value">The Vect2 instance.</param>
	/// <returns>The Euclidean length (magnitude) of the Vect2.</returns>
	public static float Length(Vect2 value) => Length(value.X, value.Y);
	#endregion


	#region LengthSquared
	/// <summary>
	/// Calculates the squared Euclidean length (magnitude) of this Vect2 instance.
	/// </summary>
	/// <returns>The squared Euclidean length (magnitude) of this Vect2.</returns>
	public readonly float LengthSquared() => LengthSquared(this);

	/// <summary>
	/// Calculates the squared Euclidean length (magnitude) of a vector defined by its x and y components.
	/// </summary>
	/// <param name="x">The x-component of the vector.</param>
	/// <param name="y">The y-component of the vector.</param>
	/// <returns>The squared Euclidean length (magnitude) of the vector.</returns>
	public static float LengthSquared(float x, float y) => x * x + y * y; // also known as Dot


	/// <summary>
	/// Calculates the squared Euclidean length (magnitude) of a Vect2 instance.
	/// </summary>
	/// <param name="value">The Vect2 instance.</param>
	/// <returns>The squared Euclidean length (magnitude) of the Vect2.</returns>
	public static float LengthSquared(Vect2 value) => LengthSquared(value.X, value.Y);
	#endregion


	#region Dot
	/// <summary>
	/// Computes the dot product of this Vect2 instance with another Vect2.
	/// </summary>
	/// <param name="other">The other Vect2 instance.</param>
	/// <returns>The dot product of this Vect2 with the other Vect2.</returns>
	public readonly float Dot(Vect2 other) => Dot(this, other);

	/// <summary>
	/// Computes the dot product of two Vect2 instances.
	/// </summary>
	/// <param name="a">The first Vect2.</param>
	/// <param name="b">The second Vect2.</param>
	/// <returns>The dot product of the two Vect2 instances.</returns>
	public static float Dot(Vect2 a, Vect2 b) => a.X * b.X + a.Y * b.Y;
	#endregion


	#region Reflect
	/// <summary>
	/// Reflects this Vect2 instance off the given normal vector.
	/// </summary>
	/// <param name="normal">The normal vector.</param>
	/// <returns>The reflected vector.</returns>
	public readonly Vect2 Reflect(Vect2 normal) => Reflect(this, normal);

	/// <summary>
	/// Reflects a vector off the given normal vector.
	/// </summary>
	/// <param name="vector">The vector to reflect.</param>
	/// <param name="normal">The normal vector.</param>
	/// <returns>The reflected vector.</returns>
	public static Vect2 Reflect(Vect2 vector, Vect2 normal)
	{
		// Formula: reflected_vector = vector - 2 * Dot(vector, normal) * normal
		var dotProduct = Dot(vector, normal);
		var reflectedX = vector.X - 2 * dotProduct * normal.X;
		var reflectedY = vector.Y - 2 * dotProduct * normal.Y;

		return new Vect2(reflectedX, reflectedY); ;
	}
	#endregion


	#region Transform
	/// <summary>
	/// Transforms a Vect2 position using a specified camera's transformation.
	/// </summary>
	/// <param name="position">The position to transform.</param>
	/// <param name="camera">The camera used for transformation.</param>
	/// <returns>The transformed Vect2 position.</returns>
	public static Vect2 Transform(Vect2 position, Camera camera) => Transform(position.X, position.Y, camera);

	/// <summary>
	/// Transforms coordinates (x, y) using a specified camera's transformation.
	/// </summary>
	/// <param name="x">The x-coordinate to transform.</param>
	/// <param name="y">The y-coordinate to transform.</param>
	/// <param name="camera">The camera used for transformation.</param>
	/// <returns>The transformed Vect2 position.</returns>
	public static Vect2 Transform(float x, float y, Camera camera)
	{
		SFMLVectorF sfmlViewPosition =
			Engine.GetService<Engine>()._window.MapPixelToCoords(new SFMLVectorI((int)x, (int)y), camera.ToSFML());

		return new Vect2(sfmlViewPosition);
	}
	#endregion


	#region LookAt/Angle
	/// <summary>
	/// Calculates the angle in radians from the left Vect2 towards the right Vect2.
	/// </summary>
	/// <param name="left">The starting Vect2 position.</param>
	/// <param name="right">The target Vect2 position.</param>
	/// <returns>The angle in radians from the left Vect2 towards the right Vect2.</returns>
	public static float LookAt(Vect2 left, Vect2 right)
	{
		Vect2 direction = Direction(left, right, true);

		return MathF.Atan2(direction.Y, direction.X);
	}

	/// <summary>
	/// Converts an angle in radians to a normalized or non-normalized Vect2 direction vector.
	/// </summary>
	/// <param name="rotation">The angle in radians.</param>
	/// <param name="normalized">Whether to normalize the resulting direction vector (default true).</param>
	/// <returns>A Vect2 direction vector corresponding to the angle.</returns>
	public static Vect2 AngleTo(float rotation, bool normalized = true)
	{
		Vect2 result = new(
			MathF.Cos(rotation),
			MathF.Sin(rotation)
		);

		if (normalized)
			return result.Normalized();

		return result;
	}

	/// <summary>
	/// Wraps an angle in radians to the range [-π, π].
	/// </summary>
	/// <param name="radians">The angle in radians to wrap.</param>
	/// <returns>The wrapped angle in radians.</returns>
	public static float WrapAngle(float radians)
	{
		// Normalize the angle to the range [0, 2π]
		radians %= MathF.PI * 2f;

		// Ensure the angle is in the range [-π, π]
		if (radians > MathF.PI)
			radians -= MathF.PI * 2f;
		else if (radians < -MathF.PI)
			radians += MathF.PI * 2f;

		return radians;
	}

	/// <summary>
	/// Adjusts the current rotation towards a target direction using a specified turn speed.
	/// </summary>
	/// <param name="target">The position of the target to face.</param>
	/// <param name="targetRotation">The current rotation angle towards the target.</param>
	/// <param name="child">The position of the object or child that needs to face the target.</param>
	/// <param name="turnSpeed">The maximum turn speed in radians per frame.</param>
	/// <returns>The adjusted rotation angle towards the target after applying turn speed limits.</returns>
	public static float TurnToFace(Vect2 target, float targetRotation, Vect2 child, float turnSpeed)
	{
		var x = child.X - target.X;
		var y = child.Y - target.Y;

		// Calculate the desired angle to face the target
		var desiredAngle = MathF.Atan2(y, x);

		// Calculate the difference in angles, wrapping to ensure correct range
		var difference = WrapAngle(desiredAngle - targetRotation);

		// Clamp the difference to the maximum turn speed
		difference = (float)Math.Clamp(difference, -turnSpeed, turnSpeed);

		// Apply the adjusted angle to the current rotation and wrap it
		return WrapAngle(targetRotation + difference);
	}
	#endregion


	#region To2D
	/// <summary>
	/// Converts a 1-dimensional index to a 2-dimensional coordinate within a grid of specified size.
	/// </summary>
	/// <param name="index">The 1-dimensional index.</param>
	/// <param name="size">The size of the grid in one dimension.</param>
	/// <returns>A 2-dimensional vector representing the coordinates in the grid.</returns>
	public static Vect2 To2D(float index, float size) => To2D((int)index, (int)size);

	/// <summary>
	/// Converts a 1-dimensional index to a 2-dimensional coordinate within a grid of specified size.
	/// </summary>
	/// <param name="index">The 1-dimensional index.</param>
	/// <param name="size">The size of the grid in one dimension.</param>
	/// <returns>A 2-dimensional vector representing the coordinates in the grid.</returns>
	public static Vect2 To2D(int index, float size) => To2D(index, (int)size);

	/// <summary>
	/// Converts a 1-dimensional index to a 2-dimensional coordinate within a grid of specified size.
	/// </summary>
	/// <param name="index">The 1-dimensional index.</param>
	/// <param name="size">The size of the grid in both dimensions.</param>
	/// <returns>A 2-dimensional vector representing the coordinates in the grid.</returns>
	public static Vect2 To2D(int index, int size) => new(index % size, index / size);
	#endregion


	#region To1D
	/// <summary>
	/// Converts a 2-dimensional coordinate to a 1-dimensional index within a grid of specified size.
	/// </summary>
	/// <param name="location">The 2-dimensional coordinate.</param>
	/// <param name="size">The size of the grid in one dimension.</param>
	/// <returns>A 1-dimensional index representing the position in the grid.</returns>
	public static int To1D(Vect2 location, float size) => To1D(location, (int)size);

	/// <summary>
	/// Converts a 2-dimensional coordinate to a 1-dimensional index within a grid of specified size.
	/// </summary>
	/// <param name="location">The 2-dimensional coordinate.</param>
	/// <param name="size">The size of the grid in one dimension.</param>
	/// <returns>A 1-dimensional index representing the position in the grid.</returns>
	public static int To1D(Vect2 location, int size)
	{
		return (int)location.Y * size + (int)location.X;
	}
	#endregion


	#region Round
	/// <summary>
	/// Rounds the components of this vector to the nearest integer values.
	/// </summary>
	/// <returns>A new vector with rounded components.</returns>
	public readonly Vect2 Round() => Round(this);

	/// <summary>
	/// Rounds the components of this vector to the nearest integer values.
	/// </summary>
	/// <param name="digits">The number of decimal places to round to.</param>
	/// <returns>A new vector with rounded components.</returns>
	public readonly Vect2 Round(int digits) => Round(this, digits);

	/// <summary>
	/// Rounds the components of the specified vector to the nearest integer values.
	/// </summary>
	/// <param name="value">The vector to round.</param>
	/// <returns>A new vector with rounded components.</returns>
	public static Vect2 Round(Vect2 value) => Round(value, 0);

	/// <summary>
	/// Rounds the components of the specified vector to the specified number of decimal places.
	/// </summary>
	/// <param name="value">The vector to round.</param>
	/// <param name="digits">The number of decimal places to round to.</param>
	/// <returns>A new vector with rounded components.</returns>
	public static Vect2 Round(Vect2 value, int digits)
	{
		float multiplier = MathF.Pow(10, digits);

		return new Vect2(
			MathF.Round(value.X * multiplier) / multiplier,
			MathF.Round(value.Y * multiplier) / multiplier
		);
	}
	#endregion


	#region IEquatable
	/// <summary>
	/// Determines whether this vector is equal to another vector.
	/// </summary>
	/// <param name="other">The vector to compare with this vector.</param>
	public readonly bool Equals(Vect2 other) => (X, Y) == (other.X, other.Y);

	/// <summary>
	/// Compares this vector with another vector for sorting purposes.
	/// </summary>
	/// <param name="obj">The vector to compare with this vector.</param>
	/// <returns>A negative integer, zero, or a positive integer indicating whether this vector is less than, equal to, or greater than the other vector.</returns>
	public readonly override bool Equals([NotNullWhen(true)] object obj)
	{
		if (obj is Vect2 value)
			return Equals(value);

		return false;
	}

	/// <summary>
	/// Computes a hash code based on the X and Y components of the vector.
	/// </summary>
	/// <returns>A hash code representing the current Vect2 instance.</returns>
	public readonly override int GetHashCode() => HashCode.Combine(X, Y);

	/// <summary>
	/// Gets a string representation of this vector.
	/// </summary>
	/// <returns>A string that represents this vector.</returns>
	public readonly override string ToString()
	{
		if (_sb.Length > 0)
			_sb.Clear();

		_sb.Append("Vect2 [");
		_sb.Append($"X={X}, ");
		_sb.Append($"Y={Y}");
		_sb.Append(']');

		return _sb.ToString();
	}
	#endregion


	#region Engine
	internal readonly SFMLVectorF ToSFML_F() => new(X, Y);
	internal readonly SFMLVectorI ToSFML_I() => new((int)X, (int)Y);

	/// <summary>
	/// Compares this Vect2 instance with another Vect2 instance based on their X and Y components.
	/// <para>- Returns a negative integer if this instance is less than the other instance.</para>
	/// <para>- Returns zero if this instance is equal to the other instance.</para>
	/// <para>- Returns a positive integer if this instance is greater than the other instance.</para>
	/// </summary>
	/// <param name="other">The Vect2 instance to compare with this instance.</param>
	/// <returns>An integer that indicates the relative order of the instances.</returns>
	public readonly int CompareTo(Vect2 other)
	{
		// Compare the X components
		int xComparison = X.CompareTo(other.X);

		if (xComparison != 0)
		{
			// If X components are different, return the result of the comparison
			return xComparison;
		}
		else
		{
			// If X components are equal, compare the Y components
			return Y.CompareTo(other.Y);
		}
	}
	#endregion
}
