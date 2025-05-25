namespace System;

/// <summary>
/// Extension methods for float values providing additional functionality and operations.
/// </summary>
public static class FloatExtentions
{
    /// <summary>
    /// Checks if the float value is an integer.
    /// </summary>
    /// <param name="value">The float value to check.</param>
    /// <returns>True if the value is an integer, false otherwise.</returns>
    public static bool IsInteger(this float value) => Math.Floor(value) == value;

    /// <summary>
    /// Checks if the float value is an float.
    /// </summary>
    /// <param name="value">The float value to check.</param>
    /// <returns>True if the value is an integer, false otherwise.</returns>
    public static bool IsFloat(this float value) => Math.Floor(value) != value;

    /// <summary>
    /// Checks if two double values are approximately equal within a specified tolerance.
    /// </summary>
    /// <param name="a">The first double value.</param>
    /// <param name="b">The second double value.</param>
    /// <param name="tolerance">The tolerance for comparison (defaults to double.Epsilon).</param>
    /// <returns>True if the values are approximately equal, false otherwise.</returns>
    public static bool IsCloseTo(this float a, float b, float tolerance = float.Epsilon)
        => Math.Abs(a - b) <= tolerance;

    /// <summary>
    /// Checks if two double values are approximately equal within a specified tolerance.
    /// </summary>
    /// <param name="a">The first double value.</param>
    /// <param name="b">The second double value.</param>
    /// <param name="tolerance">The tolerance for comparison (defaults to double.Epsilon).</param>
    /// <returns>True if the values are approximately equal, false otherwise.</returns>
    public static bool IsCloseTo(this double a, double b, double tolerance = double.Epsilon)
        => Math.Abs(a - b) <= tolerance;

    /// <summary>
    /// Checks if the value is between the specified minimum and maximum values (inclusive).
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    /// <returns>True if the value is between the min and max values (inclusive), otherwise false.</returns>
    public static bool IsBetween(this float value, float min, float max)
        => value >= min && value <= max;

    /// <summary>
    /// Checks if the value is positive (greater than zero).
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns>True if the value is greater than zero, otherwise false.</returns>
    public static bool IsPositive(this float value) => value > 0;

    /// <summary>
    /// Checks if the value is negative (less than zero).
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns>True if the value is less than zero, otherwise false.</returns>
    public static bool IsNegative(this float value) => value < 0;

    /// <summary>
    /// Converts a part of a whole value to a percentage.
    /// </summary>
    /// <param name="part">The part value.</param>
    /// <param name="whole">The whole value.</param>
    /// <param name="isFullPercent">True if the result should be multiplied by 100 (full percent), false if already in percentage form.</param>
    /// <returns>The percentage value.</returns>
    public static float ToPercent(this float part, float whole, bool isFullPercent)
    {
        if (whole == 0)
            throw new ArgumentException("Whole cannot be zero.");

        return isFullPercent
            ? (part / whole) * 100.0f
            : (part / whole)
            ;
    }

    /// <summary>
    /// Converts degrees to radians.
    /// </summary>
    /// <param name="degrees">Degrees value to convert.</param>
    /// <returns>Radians equivalent of the degrees.</returns>
    public static float ToRadians(this float degrees) => degrees * (float)Math.PI / 180f;

    /// <summary>
    /// Converts radians to degrees.
    /// </summary>
    /// <param name="radians">Radians value to convert.</param>
    /// <returns>Degrees equivalent of the radians.</returns>
    public static float ToDegrees(this float radians) => radians * (180 / MathF.PI);

    /// <summary>
    /// Gets the sign of the float value.
    /// </summary>
    /// <param name="value">Float value to determine the sign of.</param>
    /// <returns>Sign of the float value: -1 if negative, 0 if zero, 1 if positive.</returns>
    /// 

    // If the number is negative, Math.Sign returns -1.
    // If the number is zero, Math.Sign returns 0.
    // If the number is positive, Math.Sign returns 1.
    public static float ToSign(this float value) => MathF.Sign(value);

    /// <summary>
    /// Clamps the value between a minimum and maximum.
    /// </summary>
    /// <param name="value">Float value to clamp.</param>
    /// <param name="min">Minimum value.</param>
    /// <param name="max">Maximum value.</param>
    /// <returns>Clamped value.</returns>
    public static float Clamp(this float value, float min, float max)
        => Math.Clamp(value, min, max);

    /// <summary>
    /// Performs linear interpolation between two values.
    /// </summary>
    /// <param name="start">Starting value.</param>
    /// <param name="end">Ending value.</param>
    /// <param name="amount">Interpolation amount (0 to 1).</param>
    /// <returns>Interpolated value.</returns>
    public static float Lerp(this float start, float end, float amount)
        => start + (end - start) * amount;

    /// <summary>
    /// Rounds the float value to the specified number of decimal places.
    /// </summary>
    /// <param name="value">Float value to round.</param>
    /// <param name="digits">Number of decimal places to round to.</param>
    /// <returns>Rounded value.</returns>
    public static float Round(this float value, int digits)
        => (float)Math.Round(value, digits);

    /// <summary>
    /// Checks if the float value is approximately zero.
    /// </summary>
    /// <param name="value">Float value to check.</param>
    /// <param name="epsilon">Optional tolerance for the check. Default is float.Epsilon.</param>
    /// <returns>True if the value is approximately zero, false otherwise.</returns>
    public static bool IsZero(this float value, float epsilon = float.Epsilon)
        => MathF.Abs(value) <= epsilon;

    /// <summary>
    /// Checks if the float value is not approximately zero.
    /// </summary>
    /// <param name="value">Float value to check.</param>
    /// <param name="epsilon">Optional tolerance for the check. Default is float.Epsilon.</param>
    /// <returns>True if the value is not approximately zero, false otherwise.</returns>
    public static bool IsNonZero(this float value, float epsilon = float.Epsilon)
        => !value.IsZero(epsilon);
}
