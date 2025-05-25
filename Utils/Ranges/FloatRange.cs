namespace Box.Utils.Ranges;

/// <summary>
/// Represents a floating-point range with a current clamped value between Min and Max.
/// </summary>
public struct FloatRange
{
    private float _min, _max, _value;

    /// <summary>
    /// Gets a <see cref="FloatRange"/> instance with all values (Min, Max, and Value) set to zero.
    /// </summary>
    public static FloatRange Zero => new(0, 0, 0);

    /// <summary>
    /// Gets a value indicating whether the <see cref="FloatRange"/> has Min, Max, and Value all equal to zero.
    /// </summary>
    public readonly bool IsZero => _min == 0 && _max == 0 && _value == 0;

    /// <summary>
    /// The minimum value of the range.
    /// If set greater than Max, Max is adjusted to match.
    /// </summary>
    public float Min
    {
        readonly get => _min;
        set
        {
            _min = value;

            if (_min > _max)
                _max = _min;

            Value = _value; // Re-Clamp
        }
    }

    /// <summary>
    /// The maximum value of the range.
    /// If set less than Min, Min is adjusted to match.
    /// </summary>
    public float Max
    {
        readonly get => _max;
        set
        {
            _max = value;

            if (_max < _min)
                _min = _max;

            Value = _value; // Re-Clamp
        }
    }

    /// <summary>
    /// The current value, automatically clamped between Min and Max.
    /// </summary>
    public float Value
    {
        get => _value;
        set => _value = Math.Clamp(value, _min, _max);
    }

    /// <summary>
    /// Initializes a new FloatRange with the specified value, minimum, and maximum.
    /// </summary>
    /// <param name="value">Initial value (will be clamped).</param>
    /// <param name="min">Minimum value of the range.</param>
    /// <param name="max">Maximum value of the range.</param>
    public FloatRange(float value, float min, float max)
    {
        _min = min;
        _max = Math.Max(min, max);
        _value = Math.Clamp(value, _min, _max);
    }

    /// <summary>
    /// Initializes a new FloatRange with a value and maximum. Min is assumed to be 0.
    /// </summary>
    /// <param name="value">Initial value.</param>
    /// <param name="max">Maximum value.</param>
    public FloatRange(float value, float max) : this(value, 0, max) { }

    /// <summary>
    /// Initializes a new FloatRange with Min = 0, Max = Value = value.
    /// </summary>
    /// <param name="value">Initial value used as both Max and Value.</param>
    public FloatRange(float value) : this(value, 0, value) { }

    /// <summary>
    /// Returns the sum of Min and Max. Rarely used.
    /// </summary>
    public readonly float Sum => _max + _min;

    /// <summary>
    /// Returns the total size of the range (Max - Min).
    /// </summary>
    public readonly float Total => _max - _min;

    /// <summary>
    /// Returns the normalized percent (0 to 1) of the current Value within the range.
    /// </summary>
    public readonly float Percent => Total > 0 ? (_value - _min) / Total : 0f;

    /// <summary>
    /// True if Value is at or below Min.
    /// </summary>
    public readonly bool AtStart => _value <= _min;

    /// <summary>
    /// True if Value is at or above Max.
    /// </summary>
    public readonly bool AtEnd => _value >= _max;

    /// <summary>
    /// Clamps the given value to the current Min and Max range.
    /// </summary>
    /// <param name="value">The value to clamp.</param>
    /// <returns>The clamped value.</returns>
    public readonly float Clamp(float value) => Math.Clamp(value, _min, _max);

    /// <summary>
    /// Returns a random float between Min and Max using the global Rand instance.
    /// </summary>
    public readonly float Random() => Rand.Instance.Range(_min, _max);

    /// <summary>
    /// Returns a string representation of the range and current value.
    /// </summary>
    public readonly override string ToString() => $"[{_min} .. {_max}] = {_value}";
}
